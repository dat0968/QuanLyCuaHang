using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories;
using APIQuanLyCuaHang.Repositories.Bill;
using APIQuanLyCuaHang.Repositories.Combo;
using APIQuanLyCuaHang.Repositories.DetailBill;
using APIQuanLyCuaHang.Repositories.DetailComboOrder;
using APIQuanLyCuaHang.Repositories.DetailMaCoupon;
using APIQuanLyCuaHang.Repositories.DetailProduct;
using APIQuanLyCuaHang.Repository.MaCoupon;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;

namespace APIQuanLyCuaHang.Services
{
    public class OrderService
    {
        private readonly QuanLyCuaHangContext db;
        private readonly IBillRepository billRepository;
        private readonly IDetailBill detailBill;
        private readonly IMaCouponRepository MaCouponRepository;
        private readonly IComboRepository ComboRepository;
        private readonly IDetailComboOrderRepository DetailComboOrderRepository;
        private readonly IDetailProduct DetailProductRepository;
        private readonly ICartRepository CartRepository;
        private readonly IDetailMaCoupon DetailMaCouponRepository;
        public OrderService(QuanLyCuaHangContext db, IBillRepository billRepository, IDetailBill detailBill, IMaCouponRepository MaCouponRepository, 
        IComboRepository ComboRepository, IDetailComboOrderRepository DetailComboOrderRepository, IDetailProduct DetailProductRepository, ICartRepository CartRepository, IDetailMaCoupon DetailMaCouponRepository)
        {
            this.db = db;
            this.billRepository = billRepository;
            this.detailBill = detailBill;
            this.MaCouponRepository = MaCouponRepository;
            this.ComboRepository = ComboRepository;
            this.DetailComboOrderRepository = DetailComboOrderRepository;
            this.DetailProductRepository = DetailProductRepository;
            this.CartRepository = CartRepository;
            this.DetailMaCouponRepository = DetailMaCouponRepository;
        }
        public async Task<Hoadon> AddOrder(OrderRequestDTO NewOrder)
        {
            await db.Database.BeginTransactionAsync();
            try
            {
                var tinhtrangthanhtoan = "Chờ xác nhận";
                if(NewOrder.HinhThucTt.ToLower() == "vnpay")
                {
                    tinhtrangthanhtoan = "Đang xử lí VNPAY";
                }
                if(NewOrder.HinhThucTt.ToLower() == "tại quầy")
                {
                    tinhtrangthanhtoan = "Tại quầy";
                }
                // Thêm hóa đơn mới
                var ModelOrder = new Hoadon
                {
                    MaKh = NewOrder.MaKh,
                    MaNv = NewOrder.MaNv ?? null,
                    NgayTao = DateTime.Now,
                    BatDauGiao = null,
                    NgayNhan = null,
                    DiaChiNhanHang = NewOrder.DiaChiNhanHang,
                    NgayThanhToan = NewOrder.HinhThucTt.ToLower() == "cod" ? null : DateTime.Now,
                    HinhThucTt = NewOrder.HinhThucTt,
                    TinhTrang = tinhtrangthanhtoan,
                    MoTa = NewOrder.MoTa,
                    HoTen = NewOrder.HoTen,
                    Sdt = NewOrder.Sdt,
                    LyDoHuy = null,
                    IsDelete = false,
                    PhiVanChuyen = NewOrder.PhiVanChuyen,
                    TienGoc = NewOrder.TienGoc,
                    MaCoupon = NewOrder.MaCoupon,
                };
                ModelOrder = await billRepository.CreateOrder(ModelOrder);

                //Thêm CTHOADON mới
                foreach (var detail in NewOrder.Cthoadons) {
                    var ModelDetailOrder = new Cthoadon
                    {
                        MaHd = ModelOrder.MaHd,
                        MaCtsp = detail.MaCtsp,
                        SoLuong = detail.SoLuong,
                        DonGia = detail.DonGia,
                        GiamGia = detail.GiamGia,
                        MaCombo = detail.MaCombo,
                    };
                    ModelDetailOrder = await detailBill.CreateDetailOrder(ModelDetailOrder);
                    /* Cập nhật số lượng Combo và sản phẩm và cập thông sản phẩm trong
                     combo vào chitietcombohoadon ( nếu có )*/
                    if (detail.MaCombo != null)
                    {
                        var FindCombo = await ComboRepository.GetById(detail.MaCombo.Value);
                        if(FindCombo != null)
                        {
                            //Cập nhật số lượng combo
                            FindCombo.SoLuong = FindCombo.SoLuong - detail.SoLuong;
                           // UpdateCombo.TenCombo = FindCombo.TenCombo;
                            if(FindCombo.SoLuong < 0)
                            {
                                throw new Exception($"Số lượng còn lại của combo {FindCombo.TenCombo} ({FindCombo.MaCombo}) không đủ");
                            }
                            await ComboRepository.EditCombo(FindCombo);
                            // Lọc DetailCombo_OrderResquests theo MaCombo của ModelDetailOrder
                            var filteredDetailComboOrders = NewOrder.DetailCombo_OrderResquests
                                ?.Where(d => d.MaCombo == detail.MaCombo.Value)
                                .ToList();
                            // Cập thông sp trong combo vào chitietcombohoadon
                            if (filteredDetailComboOrders != null && filteredDetailComboOrders.Any())
                            {
                                foreach (var detailComboOrder in filteredDetailComboOrders)
                                {
                                    var NewModel = new Chitietcombohoadon
                                    {
                                        MaHd = ModelOrder.MaHd,
                                        MaCombo = detailComboOrder.MaCombo,
                                        MaCTSp = detailComboOrder.MaCTSp,
                                        SoLuong = detailComboOrder.SoLuong,
                                        DonGia = detailComboOrder.DonGia
                                    };
                                    NewModel = await DetailComboOrderRepository.AddDetailComboOrder(NewModel);

                                    // Cập nhật lại số lượng sản phẩm 
                                    var UpdateProduct = await DetailProductRepository.GetDetailByMaCTSp(detailComboOrder.MaCTSp);
                                    if (UpdateProduct == null)
                                    {
                                        throw new Exception("Sản phẩm không tồn tại");
                                    }
                                    UpdateProduct.SoLuongTon = UpdateProduct.SoLuongTon - detailComboOrder.SoLuong;
                                    if (UpdateProduct.SoLuongTon < 0)
                                    {
                                        throw new Exception($"Số lượng còn lại của sản phẩm {UpdateProduct.MaSpNavigation.TenSanPham} ({UpdateProduct.MaSp}) không đủ để đáp ứng cho combo mã {NewModel.MaCombo}");
                                    }
                                    else
                                    {
                                        await DetailProductRepository.UpdateDetailProduct(UpdateProduct);
                                    }
                                }
                            }                                
                            
                        }
                        
                    }
                    else
                    {
                        // Cập nhật lại số lượng sản phẩm 
                        var UpdateProduct = await DetailProductRepository.GetDetailByMaCTSp(detail.MaCtsp.Value);
                        UpdateProduct.SoLuongTon = UpdateProduct.SoLuongTon - ModelDetailOrder.SoLuong;
                        if (UpdateProduct.SoLuongTon < 0)
                        {
                            throw new Exception($"Sản phẩm {UpdateProduct.MaSpNavigation.TenSanPham} ({UpdateProduct.MaSp}) đã hết hàng");
                        }
                        else
                        {
                            await DetailProductRepository.UpdateDetailProduct(UpdateProduct);
                        }
                    }
                }
                //Cộng cột số lượng đã dùng Mã Coupon và cập nhật bảng chitietmacoupon ( nếu có )
                if (!string.IsNullOrEmpty(NewOrder.MaCoupon))
                {
                    var FindCoupon = await MaCouponRepository.GetById(NewOrder.MaCoupon);
                    if(FindCoupon != null)
                    {
                        FindCoupon.SoLuongDaDung++;
                        MaCouponRepository.Update(FindCoupon);
                        await DetailMaCouponRepository.AddDetailMacoupon(FindCoupon.MaCode, NewOrder.MaKh.Value);
                    }
                    else
                    {
                        throw new Exception("CouponCode not Found");
                    }                  
                }
                if(NewOrder.MaKh != null)
                {
                    await CartRepository.RemoveAllCart(NewOrder.MaKh.Value);              
                }
                await db.Database.CommitTransactionAsync();
                ModelOrder = await db.Hoadons
                .Include(h => h.MaNvNavigation)
                .FirstOrDefaultAsync(h => h.MaHd == ModelOrder.MaHd);
                return ModelOrder;
            }
            catch(Exception ex)
            {
                await db.Database.RollbackTransactionAsync();
                throw new Exception("Lỗi", ex);
            }
        }
    }
}
