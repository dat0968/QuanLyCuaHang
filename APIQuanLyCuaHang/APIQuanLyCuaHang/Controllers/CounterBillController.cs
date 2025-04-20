using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Bill;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> CreateCounterBill([FromBody] CounterBillCreateDTO hoaDonDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Lấy mã nhân viên từ access token
                var maNvClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(maNvClaim))
                {
                    return Unauthorized(new { Success = false, Message = "Không tìm thấy thông tin nhân viên trong token." });
                }

                if (!int.TryParse(maNvClaim, out var maNv))
                {
                    return BadRequest(new { Success = false, Message = "Mã nhân viên không hợp lệ." });
                }

                // Lấy tên nhân viên từ MaNv
                var nhanVien = await _db.Nhanviens
                    .Where(nv => nv.MaNv == maNv)
                    .FirstOrDefaultAsync();

                if (nhanVien == null)
                {
                    return NotFound("Không tìm thấy nhân viên.");
                }

                // Tạo hóa đơn
                var hoaDon = new Hoadon
                {
                    MaKh = (int)hoaDonDTO.MaKh,
                    MaNv = hoaDonDTO.MaNv,
                    TinhTrang = hoaDonDTO.TinhTrang,
                    NgayTao = hoaDonDTO.NgayTao,
                    HinhThucTt = hoaDonDTO.HinhThucTt,
                    TienGoc = hoaDonDTO.TienGoc,
                    PhiVanChuyen = hoaDonDTO.PhiVanChuyen,
                    DiaChiNhanHang = $"Tại Quầy ", 
                    HoTen = "Khách tại quầy",
                    Sdt = "",
                    //MaCoupon = hoaDonDTO. // Thêm MaCoupon
                };

                var createdBill = await _billRepository.CreateOrder(hoaDon);

                // Thêm chi tiết hóa đơn
                foreach (var ct in hoaDonDTO.Cthoadons)
                {
                    if (ct.DonGia <= 0)
                    {
                        return BadRequest(new { Success = false, Message = $"Đơn giá của sản phẩm (MaCtsp: {ct.MaCtsp}) không hợp lệ." });
                    }

                    var chiTiet = new Cthoadon
                    {
                        MaHd = createdBill.MaHd,
                        MaCtsp = ct.MaCtsp.HasValue ? ct.MaCtsp.Value : 0,
                        SoLuong = ct.SoLuong,
                        DonGia = ct.DonGia // Thêm ánh xạ DonGia
                    };
                    _db.Cthoadons.Add(chiTiet);
                }
                await _db.SaveChangesAsync();

                return Ok(new HoaDonDTO
                {
                    MaHd = createdBill.MaHd,
                    MaKh = createdBill.MaKh,
                    MaNv = createdBill.MaNv,
                    HoTenNv = nhanVien.HoTen,
                    TinhTrang = createdBill.TinhTrang,
                    NgayTao = createdBill.NgayTao,
                    HinhThucTt = createdBill.HinhThucTt,
                    TienGoc = createdBill.TienGoc,
                    PhiVanChuyen = createdBill.PhiVanChuyen,
                    HoTenNguoiNhan = "Khách tại quầy",
                    Sdt = "0000000000"
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

            // Lấy tên nhân viên từ MaNv
            var nhanVien = await _db.Nhanviens
                .Where(nv => nv.MaNv == billDetails.MaNv)
                .FirstOrDefaultAsync();

            if (nhanVien == null)
            {
                return NotFound("Không tìm thấy nhân viên.");
            }

            string tenNv = nhanVien.HoTen;

            // Xử lý số điện thoại
            string sdtDisplay = string.IsNullOrWhiteSpace(billDetails.Sdt) ? "N/A" : billDetails.Sdt.Trim();

            // Tạo HTML cải tiến
            var html = $@"
        <!DOCTYPE html>
        <html lang='vi'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Chi tiết hóa đơn - Mã HD: {billDetails.MaHd}</title>
            <link href='https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap' rel='stylesheet'>
            <style>
                * {{
                    margin: 0;
                    padding: 0;
                    box-sizing: border-box;
                }}
                body {{
                    font-family: 'Roboto', sans-serif;
                    background-color: #f4f7fa;
                    color: #333;
                    line-height: 1.6;
                }}
                .container {{
                    max-width: 900px;
                    margin: 30px auto;
                    background: #fff;
                    border-radius: 12px;
                    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
                    overflow: hidden;
                }}
                .header {{
                    background: linear-gradient(135deg, #007bff, #00c4ff);
                    color: white;
                    text-align: center;
                    padding: 30px 20px;
                    border-bottom: 4px solid #0056b3;
                }}
                .header h1 {{
                    font-size: 28px;
                    font-weight: 700;
                    margin-bottom: 10px;
                }}
                .header p {{
                    font-size: 16px;
                    opacity: 0.9;
                }}
                .bill-details {{
                    padding: 30px;
                }}
                .bill-details h2 {{
                    font-size: 24px;
                    color: #007bff;
                    text-align: center;
                    margin-bottom: 20px;
                    border-bottom: 2px solid #e9ecef;
                    padding-bottom: 10px;
                }}
                .bill-info {{
                    display: grid;
                    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
                    gap: 15px;
                    margin-bottom: 30px;
                }}
                .bill-info p {{
                    font-size: 16px;
                    margin: 5px 0;
                }}
                .bill-info p strong {{
                    color: #0056b3;
                }}
                .bill-table {{
                    width: 100%;
                    border-collapse: collapse;
                    margin-bottom: 30px;
                    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
                }}
                .bill-table th, .bill-table td {{
                    padding: 12px 15px;
                    text-align: left;
                    border-bottom: 1px solid #e9ecef;
                }}
                .bill-table th {{
                    background: #007bff;
                    color: white;
                    font-weight: 500;
                    text-transform: uppercase;
                    font-size: 14px;
                }}
                .bill-table td {{
                    font-size: 15px;
                    color: #444;
                }}
                .bill-table tr:nth-child(even) {{
                    background-color: #f8f9fa;
                }}
                .bill-table tr:hover {{
                    background-color: #e6f0ff;
                }}
                .total-section {{
                    text-align: right;
                    font-size: 20px;
                    font-weight: 700;
                    color: #dc3545;
                    margin-top: 20px;
                    padding-top: 15px;
                    border-top: 2px dashed #e9ecef;
                }}
                .footer {{
                    background: #f8f9fa;
                    text-align: center;
                    padding: 20px;
                    border-top: 1px solid #e9ecef;
                    font-size: 14px;
                    color: #666;
                }}
                .footer p {{
                    margin: 5px 0;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>QUÁN ĂN JOLLY</h1>
                    <p>Địa chỉ: 123 Đường Ẩm Thực, Quận 1, TP. HCM</p>
                    <p>Số điện thoại: 0123 456 789</p>
                </div>
                <div class='bill-details'>
                    <h2>CHI TIẾT HÓA ĐƠN</h2>
                    <div class='bill-info'>
                        <p><strong>Mã hóa đơn:</strong> {billDetails.MaHd}</p>
                        <p><strong>Nhân viên:</strong> {tenNv}</p>
                        {(billDetails.MaKh != 0 ? $"<p><strong>Khách hàng:</strong> {billDetails.MaKh}</p>" : "")}
                        <p><strong>Ngày tạo:</strong> {billDetails.NgayTao:dd/MM/yyyy HH:mm:ss}</p>
                        <p><strong>Hình thức thanh toán:</strong> {billDetails.HinhThucTt}</p>
                        <p><strong>Địa chỉ:</strong> {billDetails.DiaChiNhanHang}</p>
                        <p><strong>Số điện thoại:</strong> {sdtDisplay}</p>
                    </div>
                    <table class='bill-table'>
                        <thead>
                            <tr>
                                <th>STT</th>
                                <th>Tên sản phẩm</th>
                                <th>Kích thước</th>
                                <th>Hương vị</th>
                                <th>Số lượng</th>
                                <th>Đơn giá</th>
                                <th>Tổng</th>
                            </tr>
                        </thead>
                        <tbody>
                            {string.Join("", billDetails.ChiTietHoaDon.Select((ct, index) => $@"
                                <tr>
                                    <td>{index + 1}</td>
                                    <td>{ct.TenSanPham}</td>
                                    <td>{(string.IsNullOrEmpty(ct.KichThuoc) ? "N/A" : ct.KichThuoc)}</td>
                                    <td>{(string.IsNullOrEmpty(ct.HuongVi) ? "N/A" : ct.HuongVi)}</td>
                                    <td>{ct.SoLuong}</td>
                                    <td>{(ct.DonGia == 0 ? billDetails.TienGoc / ct.SoLuong : ct.DonGia)} VNĐ</td>
                                    <td>{(ct.DonGia == 0 ? billDetails.TienGoc : ct.SoLuong * ct.DonGia)} VNĐ</td>
                                </tr>
                            "))}
                        </tbody>
                    </table>
                    <div class='total-section'>
                        Tổng tiền: {billDetails.Tongtien} VNĐ
                    </div>
                </div>
                <div class='footer'>
                    <p>Cảm ơn quý khách đã ủng hộ Quán Ăn Jolly!</p>
                    <p>Mọi thắc mắc xin liên hệ: 0123 456 789</p>
                </div>
            </div>
        </body>
        </html>
    ";

            return Content(html, "text/html");
        }
    }
}
