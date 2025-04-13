using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Bill;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CounterBillController : ControllerBase
    {
        private readonly IBillRepository _billRepository;
        private readonly QuanLyCuaHangContext _db;

        public CounterBillController(IBillRepository billRepository, QuanLyCuaHangContext db)
        {
            _billRepository = billRepository;
            _db = db;
        }

        [HttpPost]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> CreateCounterBill([FromBody] CounterBillCreateDTO hoaDonDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Tạo hóa đơn
                var hoaDon = new Hoadon
                {
                    MaKh = 122,
                    MaNv = hoaDonDTO.MaNv,
                    TinhTrang = hoaDonDTO.TinhTrang,
                    NgayTao = hoaDonDTO.NgayTao,
                    HinhThucTt = hoaDonDTO.HinhThucTt,
                    TienGoc = hoaDonDTO.TienGoc,
                    PhiVanChuyen = hoaDonDTO.PhiVanChuyen,
                    DiaChiNhanHang = hoaDonDTO.MaKh.HasValue ? null : "Tại quầy",
                    HoTen = hoaDonDTO.MaKh.HasValue ? null : "Khách tại quầy",
                    Sdt = null,
                };

                var createdBill = await _billRepository.CreateOrder(hoaDon);

                // Thêm chi tiết hóa đơn
                foreach (var ct in hoaDonDTO.Cthoadons)
                {
                    var chiTiet = new Cthoadon
                    {
                        MaHd = createdBill.MaHd,
                        MaCtsp = ct.MaCtsp.HasValue ? ct.MaCtsp.Value : 0,
                        SoLuong = ct.SoLuong,
                    };
                    _db.Cthoadons.Add(chiTiet);
                }
                await _db.SaveChangesAsync();

                return Ok(new HoaDonDTO
                {
                    MaHd = createdBill.MaHd,
                    MaKh = createdBill.MaKh,
                    MaNv = createdBill.MaNv,
                    TinhTrang = createdBill.TinhTrang,
                    NgayTao = createdBill.NgayTao,
                    HinhThucTt = createdBill.HinhThucTt,
                    TienGoc = createdBill.TienGoc,
                    PhiVanChuyen = createdBill.PhiVanChuyen,
                    HoTen = "Khách tại quầy",
                    Sdt = ""
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetBillDetails/details/{id}")]
        public async Task<IActionResult> GetBillDetailsHtml(int id)
        {
            var billDetails = await _billRepository.GetBillDetails(id);
            if (billDetails == null)
            {
                return NotFound("Hóa đơn không tồn tại.");
            }

            // Tạo HTML
            var html = $@"
                <!DOCTYPE html>
                <html lang='vi'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Chi tiết hóa đơn - Mã HD: {billDetails.MaHd}</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            max-width: 800px;
                            margin: 20px auto;
                            padding: 20px;
                            background-color: #f5f5f5;
                        }}
                        h1 {{
                            text-align: center;
                            color: #333;
                        }}
                        .bill-details {{
                            background-color: white;
                            padding: 20px;
                            border-radius: 8px;
                            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        }}
                        .bill-details p {{
                            margin: 10px 0;
                            font-size: 16px;
                        }}
                        .bill-details table {{
                            width: 100%;
                            border-collapse: collapse;
                            margin-top: 20px;
                        }}
                        .bill-details th, .bill-details td {{
                            border: 1px solid #ddd;
                            padding: 10px;
                            text-align: left;
                        }}
                        .bill-details th {{
                            background-color: #28a745;
                            color: white;
                        }}
                        .bill-details tr:nth-child(even) {{
                            background-color: #f9f9f9;
                        }}
                        .total {{
                            font-size: 18px;
                            font-weight: bold;
                            text-align: right;
                            margin-top: 20px;
                        }}
                    </style>
                </head>
                <body>
                    <h1>Chi tiết hóa đơn</h1>
                    <div class='bill-details'>
                        <p><strong>Mã hóa đơn:</strong> {billDetails.MaHd}</p>
                        <p><strong>Nhân viên:</strong> {billDetails.MaNv}</p>
                        {(billDetails.MaKh != 0 ? $"<p><strong>Khách hàng:</strong> {billDetails.MaKh}</p>" : "")}
                        <p><strong>Ngày tạo:</strong> {billDetails.NgayTao:dd/MM/yyyy HH:mm:ss}</p>
                        <p><strong>Hình thức thanh toán:</strong> {billDetails.HinhThucTt}</p>
                        <table>
                            <thead>
                                <tr>
                                    <th>Tên sản phẩm</th>
                                    <th>Kích thước</th>
                                    <th>Hương vị</th>
                                    <th>Số lượng</th>
                                    <th>Đơn giá</th>
                                    <th>Tổng</th>
                                </tr>
                            </thead>
                            <tbody>
                                {string.Join("", billDetails.ChiTietHoaDon.Select(ct => $@"
                                    <tr>
                                        <td>{ct.TenSanPham}</td>
                                        <td>{ct.KichThuoc}</td>
                                        <td>{ct.HuongVi}</td>
                                        <td>{ct.SoLuong}</td>
                                        <td>{ct.DonGia} VNĐ</td>
                                        <td>{ct.SoLuong * ct.DonGia} VNĐ</td>
                                    </tr>
                                "))}
                            </tbody>
                        </table>
                        <p class='total'>Tổng tiền: {billDetails.Tongtien} VNĐ</p>
                    </div>
                </body>
                </html>
            ";

            return Content(html, "text/html");
        }
    }
}