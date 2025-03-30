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

        public async Task AddToCart(CartItemDTO cartItem)
        {
            if (cartItem.MaCombo.HasValue)
            {
                var combo = await db.Combos
                    .Where(c => c.MaCombo == cartItem.MaCombo && (c.IsDelete == false || c.IsDelete == null))
                    .FirstOrDefaultAsync();

                if (combo == null)
                {
                    throw new Exception($"Combo với MaCombo {cartItem.MaCombo} không tồn tại hoặc đã bị xóa.");
                }

                // Kiểm tra số lượng tồn của combo
                if (combo.SoLuong < cartItem.SoLuong)
                {
                    throw new Exception($"Số lượng combo trong kho không đủ! Chỉ còn {combo.SoLuong} combo.");
                }

                // Kiểm tra xem combo đã có trong giỏ hàng chưa
                var existingCartItem = await db.Giohangs
                    .FirstOrDefaultAsync(c => c.MaKh == cartItem.MaKh && c.MaCombo.HasValue && c.MaCombo == cartItem.MaCombo.Value);

                if (existingCartItem != null)
                {
                    // Cập nhật số lượng nếu combo đã có trong giỏ hàng
                    existingCartItem.SoLuong += cartItem.SoLuong;
                    existingCartItem.DonGia = cartItem.DonGia;
                }
                else
                {
                    // Thêm mới combo vào giỏ hàng
                    var newCartItem = new Giohang
                    {
                        MaKh = cartItem.MaKh,
                        MaCombo = cartItem.MaCombo,
                        MaCtsp = cartItem.MaCtsp,
                        SoLuong = cartItem.SoLuong,
                        DonGia = cartItem.DonGia
                    };
                    db.Giohangs.Add(newCartItem);
                }
            }
            else
            {
                // Xử lý sản phẩm đơn lẻ
                var product = await db.Chitietsanphams
                    .Where(p => p.MaCtsp == cartItem.MaCtsp && p.IsDelete == false)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    throw new Exception($"Sản phẩm với MaCtsp {cartItem.MaCtsp} không tồn tại hoặc đã bị xóa.");
                }

                // Kiểm tra số lượng tồn
                if (product.SoLuongTon < cartItem.SoLuong)
                {
                    throw new Exception($"Số lượng trong kho không đủ! Chỉ còn {product.SoLuongTon} sản phẩm.");
                }

                // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
                var existingCartItem = await db.Giohangs
                    .FirstOrDefaultAsync(c => c.MaKh == cartItem.MaKh && c.MaCtsp == cartItem.MaCtsp);

                if (existingCartItem != null)
                {
                    // Cập nhật số lượng nếu sản phẩm đã có trong giỏ hàng
                    existingCartItem.SoLuong += cartItem.SoLuong;
                    existingCartItem.DonGia = cartItem.DonGia;
                }
                else
                {
                    // Thêm mới vào giỏ hàng
                    var newCartItem = new Giohang
                    {
                        MaKh = cartItem.MaKh,
                        MaCtsp = cartItem.MaCtsp,
                        MaCombo = null,
                        SoLuong = cartItem.SoLuong,
                        DonGia = cartItem.DonGia
                    };
                    db.Giohangs.Add(newCartItem);
                }
            }

            await db.SaveChangesAsync();
        }

        public async Task<List<CartItemDTO>> GetCart(int maKh)
        {
            var cartItems = await db.Giohangs
                .Where(c => c.MaKh == maKh)
                .Select(c => new CartItemDTO
                {
                    MaKh = c.MaKh,
                    MaCtsp = c.MaCtsp,
                    MaCombo = c.MaCombo,
                    SoLuong = c.SoLuong,
                    DonGia = c.DonGia,
                    TenSanPham = c.MaCtsp != null
                        ? c.MaCtspNavigation.MaSpNavigation.TenSanPham
                        : "",
                    HinhAnhUrls = c.MaCtsp != null
                        ? (c.MaCtspNavigation.Hinhanhs != null
                            ? c.MaCtspNavigation.Hinhanhs.Select(h => h.TenHinhAnh).Where(h => h != null).ToList()
                            : new List<string>())
                        : (
                            db.Combos
                                .Where(com => com.MaCombo == c.MaCombo)
                                .Select(com => com.Hinh)
                                .FirstOrDefault() != null
                                ? new List<string> { db.Combos.Where(com => com.MaCombo == c.MaCombo).Select(com => com.Hinh).FirstOrDefault() }
                                : new List<string>()
                        ),
                    KichThuoc = c.MaCtsp != null ? c.MaCtspNavigation.KichThuoc : "NO",
                    HuongVi = c.MaCtsp != null ? c.MaCtspNavigation.HuongVi : "NO",
                    SoLuongTon = c.MaCtsp != null
                        ? c.MaCtspNavigation.SoLuongTon
                        : db.Combos.Where(com => com.MaCombo == c.MaCombo).Select(com => com.SoLuong).FirstOrDefault()
                })
                .ToListAsync();

            return cartItems;
        }


        public async Task UpdateCartItem(CartItemDTO cartItem)
        {
            if (cartItem == null)
            {
                throw new ArgumentNullException(nameof(cartItem), "Cart item cannot be null.");
            }

            if (cartItem.MaCombo.HasValue)
            {
                // Cập nhật combo
                var existingCartItem = await db.Giohangs
                    .FirstOrDefaultAsync(c => c.MaKh == cartItem.MaKh && c.MaCombo.HasValue && c.MaCombo == cartItem.MaCombo.Value);

                if (existingCartItem == null)
                {
                    throw new Exception($"Combo với MaCombo {cartItem.MaCombo} không tồn tại trong giỏ hàng của khách hàng MaKh {cartItem.MaKh}.");
                }

                // Kiểm tra số lượng tồn của combo
                var combo = await db.Combos
                    .Where(c => c.MaCombo == cartItem.MaCombo)
                    .FirstOrDefaultAsync();

                if (combo == null || combo.SoLuong < cartItem.SoLuong)
                {
                    throw new Exception($"Số lượng combo trong kho không đủ! Chỉ còn {combo?.SoLuong ?? 0} combo.");
                }

                existingCartItem.SoLuong = cartItem.SoLuong;
                existingCartItem.DonGia = cartItem.DonGia;
            }
            else
            {
                // Cập nhật sản phẩm đơn lẻ
                if (!cartItem.MaCtsp.HasValue)
                {
                    throw new Exception("MaCtsp là bắt buộc khi không có MaCombo.");
                }

                var existingCartItem = await db.Giohangs
                    .FirstOrDefaultAsync(c => c.MaKh == cartItem.MaKh && c.MaCtsp == cartItem.MaCtsp);

                if (existingCartItem == null)
                {
                    throw new Exception($"Sản phẩm với MaCtsp {cartItem.MaCtsp} không tồn tại trong giỏ hàng của khách hàng MaKh {cartItem.MaKh}.");
                }

                // Kiểm tra số lượng tồn
                var product = await db.Chitietsanphams
                    .Where(p => p.MaCtsp == cartItem.MaCtsp)
                    .FirstOrDefaultAsync();

                if (product == null || product.SoLuongTon < cartItem.SoLuong)
                {
                    throw new Exception($"Số lượng trong kho không đủ! Chỉ còn {product?.SoLuongTon ?? 0} sản phẩm.");
                }

                existingCartItem.SoLuong = cartItem.SoLuong;
                existingCartItem.DonGia = cartItem.DonGia;
            }

            await db.SaveChangesAsync();
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