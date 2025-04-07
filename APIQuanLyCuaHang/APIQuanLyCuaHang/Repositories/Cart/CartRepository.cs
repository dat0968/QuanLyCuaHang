using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly QuanLyCuaHangContext db;

        public CartRepository(QuanLyCuaHangContext db)
        {
            this.db = db;
        }

        public async Task AddToCart(CartItemResquestDTO cartItem)
        {
            try
            {
                await db.Database.BeginTransactionAsync();
                if (cartItem.MaCombo == null)
                {
                    var CheckCart = await db.Giohangs
                        .FirstOrDefaultAsync(gh => gh.MaKh == cartItem.MaKh
                        && gh.MaCtsp == cartItem.MaCtsp);
                    var Findproduct = await db.Chitietsanphams.FirstOrDefaultAsync(p => p.MaCtsp == cartItem.MaCtsp);
                    var QuantityProduct = Findproduct?.SoLuongTon;
                    if (CheckCart != null)
                    {
                        CheckCart.SoLuong += cartItem.SoLuong;                        
                        await db.SaveChangesAsync();
                        if (CheckCart.SoLuong > QuantityProduct)
                        {
                            throw new Exception($"Số lượng trong giỏ hàng vượt quá số lượng tồn kho tối đa là {QuantityProduct} sản phẩm");
                        }
                    }
                    else
                    {
                        var NewCart = new Giohang
                        {
                            MaKh = cartItem.MaKh,
                            MaCtsp = cartItem.MaCtsp,
                            MaCombo = null,
                            SoLuong = cartItem.SoLuong,
                            DonGia = cartItem.DonGia,
                        };
                        db.Giohangs.Add(NewCart);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    var selectedVariants = cartItem.CartDetailRequestCombos.Select(ct => ct.MaCTSp).ToList();
                    var CheckComboCart = await db.Giohangs.Include(gh => gh.GioHangCTCombos)
                                        .FirstOrDefaultAsync(gh =>
                                        gh.MaKh == cartItem.MaKh && gh.MaCombo == cartItem.MaCombo
                                        && gh.GioHangCTCombos.All(ghct => selectedVariants.Contains(ghct.MaCTSp)));
                    if (CheckComboCart != null)
                    {
                        var findCombo = await db.Combos.FirstOrDefaultAsync(p => p.MaCombo == cartItem.MaCombo);
                        var QuantityCombo = findCombo?.SoLuong;
                        CheckComboCart.SoLuong += cartItem.SoLuong;
                        db.Giohangs.Update(CheckComboCart);
                        await db.SaveChangesAsync();
                        if(CheckComboCart.SoLuong > QuantityCombo)
                        {
                            throw new Exception($"Số lượng trong giỏ hàng vượt quá số lượng tồn kho tối đa là {QuantityCombo} combo");
                        }
                        foreach (var details in CheckComboCart.GioHangCTCombos)
                        {
                            var GetMaSp = await db.Chitietsanphams.AsNoTracking().FirstOrDefaultAsync(p => p.MaCtsp == details.MaCTSp);
                            var GetDetailProduct = await db.Chitietcombos.AsNoTracking().FirstOrDefaultAsync(p => p.MaSp == GetMaSp.MaSp);
                            var QuantityProduct = GetDetailProduct.SoLuongSp;
                            details.SoLuong = QuantityProduct.Value * CheckComboCart.SoLuong;
                        }
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        var NewCart = new Giohang
                        {
                            MaKh = cartItem.MaKh,
                            MaCtsp = null,
                            MaCombo = cartItem.MaCombo,
                            SoLuong = cartItem.SoLuong,
                            DonGia = cartItem.DonGia,
                        };
                        db.Giohangs.Add(NewCart);
                        await db.SaveChangesAsync();
                        foreach(var detail in cartItem.CartDetailRequestCombos)
                        {
                            var NewCartDetail = new GioHangCTCombo
                            {
                                MaGioHang = NewCart.Id,
                                MaCTSp = detail.MaCTSp,
                                DonGia = detail.DonGia,
                                SoLuong = detail.SoLuong,
                            };
                            db.GioHangCTCombos.Add(NewCartDetail);
                            await db.SaveChangesAsync();
                        }
                        
                    }
                }
                await db.Database.CommitTransactionAsync();

            }
            catch (Exception ex)
            {
                await db.Database.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<List<CartItemDTO>> GetCart(int maKh)
        {
            try
            {
                var gioHang = await db.Giohangs
                .Include(gh => gh.MaCtspNavigation)
                    .ThenInclude(ctsp => ctsp.MaSpNavigation)
                .Include(gh => gh.MaCtspNavigation)
                    .ThenInclude(ctsp => ctsp.Hinhanhs)
                .Include(gh => gh.MaComboNavigation)
                .Include(gh => gh.GioHangCTCombos)
                    .ThenInclude(ghct => ghct.MaCTSpNavigation)
                        .ThenInclude(ctsp => ctsp.MaSpNavigation)
                .Where(gh => gh.MaKh == maKh)
                .ToListAsync();
                if (gioHang != null)
                {
                    var gioHangDTO = gioHang.Select(gh => new CartItemDTO
                    {
                        Id = gh.Id,
                        MaKh = gh.MaKh,
                        MaCtsp = gh.MaCtsp,
                        MaCombo = gh.MaCombo,
                        SoLuong = gh.SoLuong,
                        DonGia = gh.DonGia,
                        TenSanPham = gh.MaCombo == null ? gh.MaCtspNavigation.MaSpNavigation.TenSanPham : gh.MaComboNavigation.TenCombo,
                        KichThuoc = gh.MaCombo == null ? gh.MaCtspNavigation.KichThuoc : null,
                        HuongVi = gh.MaCombo == null ? gh.MaCtspNavigation.HuongVi : null,
                        HinhAnh = gh.MaCombo == null
                            ? gh.MaCtspNavigation.Hinhanhs.Select(p => p.TenHinhAnh).FirstOrDefault()
                            : gh.MaComboNavigation.Hinh,
                        
                        TenCombo = gh.MaCombo != null ? gh.MaComboNavigation.TenCombo : null,
                        CartDetailCombos = gh.MaCombo != null
                            ? gh.GioHangCTCombos.Select(ghct => new CartDetailCombo 
                            {
                                Id = ghct.Id,
                                MaCTSp = ghct.MaCTSp,
                                SoLuong = ghct.SoLuong,
                                DonGia = ghct.DonGia,
                                MaGioHang = ghct.MaGioHang,
                                MaSp = ghct.MaCTSpNavigation.MaSp,
                                TenSanPham = ghct.MaCTSpNavigation.MaSpNavigation.TenSanPham,
                                HuongVi = ghct.MaCTSpNavigation.HuongVi,
                                KichThuoc = ghct.MaCTSpNavigation.KichThuoc
                            }).ToList() : new List<CartDetailCombo>() }).ToList();
                    return gioHangDTO;
                }
                return null;

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task UpdateCartItem(int id, CartItemResquestDTO cartItem)
        {
            await db.Database.BeginTransactionAsync(); 
            try
            {
                if (cartItem == null)
                {
                    throw new ArgumentNullException(nameof(cartItem), "Cart item cannot be null.");
                }

                if (cartItem.MaCombo == null)
                {
                    var findProductCart = await db.Giohangs.FirstOrDefaultAsync(p => p.MaCtsp == cartItem.MaCtsp && p.MaKh == cartItem.MaKh);

                    if (findProductCart == null)
                    {
                        throw new Exception($"Sản phẩm với mã chi tiết {cartItem.MaCtsp} không tồn tại trong giỏ hàng của khách hàng MaKh {cartItem.MaKh}.");
                    }

                    var FindProduct = await db.Chitietsanphams.AsNoTracking().FirstOrDefaultAsync(p => p.MaCtsp == cartItem.MaCtsp);
                    var quantityProduct = FindProduct.SoLuongTon;
                    var quantityCartProduct = findProductCart.SoLuong + cartItem.SoLuong;
                    if (quantityCartProduct > quantityProduct)
                    {
                        throw new Exception($"Số lượng sản phẩm trong kho không đủ! Chỉ còn {quantityProduct} sản phẩm.");
                    }

                    findProductCart.SoLuong = quantityCartProduct;
                    db.Giohangs.Update(findProductCart);
                }
                else
                {
                    var findComboCart = await db.Giohangs.Include(p => p.GioHangCTCombos).FirstOrDefaultAsync(p => p.Id == id && p.MaKh == cartItem.MaKh && p.MaCombo == cartItem.MaCombo);               
                    if (findComboCart == null)
                    {
                        throw new Exception($"Combo này không tồn tại trong giỏ hàng của khách hàng có mã {cartItem.MaKh}.");
                    }

                    // Kiểm tra số lượng tồn
                    var FindCombo = await db.Combos
                        .Where(p => p.MaCombo == cartItem.MaCombo)
                        .FirstOrDefaultAsync();
                    var QuantityCombo = FindCombo.SoLuong;


                    var QuantityCartCombo = findComboCart.SoLuong + cartItem.SoLuong;
                    if(QuantityCartCombo > QuantityCombo)
                    {
                        throw new Exception($"Số lượng combo trong kho không đủ! Chỉ còn {QuantityCombo} combo.");
                    }
                    findComboCart.SoLuong = QuantityCartCombo;
                    foreach(var details in findComboCart.GioHangCTCombos)
                    {
                        var GetMaSp = await db.Chitietsanphams.AsNoTracking().FirstOrDefaultAsync(p => p.MaCtsp == details.MaCTSp);
                        var GetDetailProduct = await db.Chitietcombos.AsNoTracking().FirstOrDefaultAsync(p => p.MaSp == GetMaSp.MaSp);
                        var QuantityProduct = GetDetailProduct.SoLuongSp;
                        details.SoLuong = QuantityCartCombo * QuantityProduct.Value;
                        db.GioHangCTCombos.Update(details);
                    }
                    db.Giohangs.Update(findComboCart);
                }

                await db.SaveChangesAsync();
                await db.Database.CommitTransactionAsync();
            }
            catch (Exception ex) {
                await db.Database.RollbackTransactionAsync();
                throw;
            }
            
        }

        public async Task RemoveFromCart(int maKh, int maCtsp)
        {
            var cartItem = await db.Giohangs
                .FirstOrDefaultAsync(c => c.MaKh == maKh && c.MaCtsp == maCtsp);

            if (cartItem == null)
            {
                throw new Exception("Sản phẩm không tồn tại trong giỏ hàng.");
            }

            db.Giohangs.Remove(cartItem);
            await db.SaveChangesAsync();
        }

        public async Task RemoveComboFromCart(int maKh, int maCombo)
        {
            var cartItem = await db.Giohangs
                .FirstOrDefaultAsync(c => c.MaKh == maKh && c.MaCombo.HasValue && c.MaCombo == maCombo);

            if (cartItem == null)
            {
                throw new Exception("Combo không tồn tại trong giỏ hàng.");
            }

            db.Giohangs.Remove(cartItem);
            await db.SaveChangesAsync();
        }
    }
}