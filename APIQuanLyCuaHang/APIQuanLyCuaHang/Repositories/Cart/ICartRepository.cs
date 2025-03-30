using APIQuanLyCuaHang.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Repositories
{
    public interface ICartRepository
    {
        Task AddToCart(CartItemDTO cartItem);
        Task<List<CartItemDTO>> GetCart(int maKh);
        Task UpdateCartItem(CartItemDTO cartItem);
        Task RemoveFromCart(int maKh, int maCtsp);
        Task RemoveComboFromCart(int maKh, int maCombo); // Thêm phương thức này
    }
}