using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Bill;
using APIQuanLyCuaHang.Repositories.DetailBill;
using System.Runtime.CompilerServices;

namespace APIQuanLyCuaHang.Services
{
    public class OrderService
    {
        private readonly QuanLyCuaHangContext db;
        private readonly IBillRepository billRepository;
        private readonly IDetailBill detailBill;
        public OrderService(QuanLyCuaHangContext db, IBillRepository billRepository, IDetailBill detailBill)
        {
            this.db = db;
            this.billRepository = billRepository;
            this.detailBill = detailBill;
        }
        public async Task AddOrder(OrderRequestDTO NewOrder)
        {
            await db.Database.BeginTransactionAsync();
            try
            {
                var ModelOrder = new Hoadon
                {
                    MaKh = NewOrder.MaKh,
                    NgayTao = DateTime.Now,
                    BatDauGiao = null,
                    NgayNhan = null,
                    DiaChiNhanHang = NewOrder.DiaChiNhanHang,
                    NgayThanhToan = NewOrder.NgayThanhToan,
                    HinhThucTt = NewOrder.HinhThucTt,
                    TinhTrang = "Chờ xác nhận",
                    MoTa = NewOrder.MoTa,
                    HoTen = NewOrder.HoTen,
                    Sdt = NewOrder.Sdt,
                    LyDoHuy = null,
                    IsDelete = false,
                    PhiVanChuyen = NewOrder.PhiVanChuyen,
                    TienGoc = NewOrder.TienGoc,
                };
                ModelOrder = await billRepository.CreateOrder(ModelOrder);

                foreach (var detail in NewOrder.Cthoadons) {
                    var ModelDetailOrder = new Cthoadon
                    {
                        MaHd = ModelOrder.MaHd,
                        MaCtsp = detail.MaCtsp,
                        SoLuong = detail.SoLuong,
                    };
                    await detailBill.CreateDetailOrder(ModelDetailOrder);
                }
                await db.Database.CommitTransactionAsync();
            }catch(Exception ex)
            {
                await db.Database.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
