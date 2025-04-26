using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Repositories.Staff
{
    public class StaffRepository : IStaffRepository
    {
        private readonly QuanLyCuaHangContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public StaffRepository(QuanLyCuaHangContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<List<NhanvienDTO>> GetAllStaff(string? search, string? gioiTinh, string? tinhTrang)
        {
            var query = _context.Nhanviens
                .Include(p => p.MaChucVuNavigation)
                .Where(nv => nv.IsDelete != true)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(nv => nv.HoTen.ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(gioiTinh))
            {
                gioiTinh = gioiTinh.ToLower();
                query = query.Where(nv => nv.GioiTinh.ToLower() == gioiTinh);
            }

            if (!string.IsNullOrEmpty(tinhTrang))
            {
                tinhTrang = tinhTrang.ToLower();
                query = query.Where(nv => nv.TinhTrang != null && nv.TinhTrang.ToLower() == tinhTrang);
            }

            var staffList = await query
                .Select(nv => new NhanvienDTO
                {
                    MaNv = nv.MaNv,
                    HoTen = nv.HoTen,
                    GioiTinh = nv.GioiTinh,
                    NgaySinh = nv.NgaySinh,
                    DiaChi = nv.DiaChi,
                    Cccd = nv.Cccd,
                    Sdt = nv.Sdt,
                    Email = nv.Email,
                    NgayVaoLam = nv.NgayVaoLam,
                    TenTaiKhoan = nv.TenTaiKhoan,
                    MatKhau = nv.MatKhau,
                    TinhTrang = nv.TinhTrang,
                    IsDelete = nv.IsDelete,
                    MaChucVu = nv.MaChucVu,
                    TenChucVu = nv.MaChucVuNavigation.TenChucVu,
                    HinhAnh = null,
                    HinhAnhDuongDan = nv.HinhAnh,
                })
                .ToListAsync();

            return staffList;
        }

        public async Task<NhanvienDTO> GetStaffById(int id)
        {
            var staff = await _context.Nhanviens
                .FirstOrDefaultAsync(nv => nv.MaNv == id);

            if (staff == null)
            {
                return null;
            }

            return new NhanvienDTO
            {
                MaNv = staff.MaNv,
                HoTen = staff.HoTen,
                GioiTinh = staff.GioiTinh,
                NgaySinh = staff.NgaySinh,
                DiaChi = staff.DiaChi,
                Cccd = staff.Cccd,
                Sdt = staff.Sdt,
                Email = staff.Email,
                NgayVaoLam = staff.NgayVaoLam,
                TenTaiKhoan = staff.TenTaiKhoan,
                MatKhau = staff.MatKhau,
                TinhTrang = staff.TinhTrang,
                IsDelete = staff.IsDelete,
                MaChucVu = staff.MaChucVu,
                HinhAnh = null // Không cần IFormFile ở đây
            };
        }

        public async Task<NhanvienDTO> AddStaff(NhanvienDTO staffDto)
        {
            if (staffDto.MaChucVu.HasValue)
            {
                var chucVuExists = await _context.Chucvus.AnyAsync(cv => cv.MaChucVu == staffDto.MaChucVu.Value);
                if (!chucVuExists)
                {
                    throw new Exception($"Chức vụ với MaChucVu {staffDto.MaChucVu} không tồn tại.");
                }
            }

            var newStaff = new Nhanvien
            {
                HoTen = staffDto.HoTen,
                GioiTinh = staffDto.GioiTinh,
                NgaySinh = staffDto.NgaySinh,
                DiaChi = staffDto.DiaChi,
                Cccd = staffDto.Cccd,
                Sdt = staffDto.Sdt,
                Email = staffDto.Email,
                NgayVaoLam = staffDto.NgayVaoLam,
                TenTaiKhoan = staffDto.TenTaiKhoan,
                MatKhau = staffDto.MatKhau,
                TinhTrang = staffDto.TinhTrang,
                MaChucVu = staffDto.MaChucVu,
                IsDelete = false,
                HinhAnh = await SaveImage(staffDto.HinhAnh)
            };

            _context.Nhanviens.Add(newStaff);
            await _context.SaveChangesAsync();

            var addedStaff = await _context.Nhanviens
                .FirstOrDefaultAsync(nv => nv.MaNv == newStaff.MaNv);

            return new NhanvienDTO
            {
                MaNv = addedStaff.MaNv,
                HoTen = addedStaff.HoTen,
                GioiTinh = addedStaff.GioiTinh,
                NgaySinh = addedStaff.NgaySinh,
                DiaChi = addedStaff.DiaChi,
                Cccd = addedStaff.Cccd,
                Sdt = addedStaff.Sdt,
                Email = addedStaff.Email,
                NgayVaoLam = addedStaff.NgayVaoLam,
                TenTaiKhoan = addedStaff.TenTaiKhoan,
                MatKhau = addedStaff.MatKhau,
                TinhTrang = addedStaff.TinhTrang,
                IsDelete = addedStaff.IsDelete,
                MaChucVu = addedStaff.MaChucVu,
                HinhAnh = null
            };
        }

        public async Task<NhanvienDTO> UpdateStaff(int id, NhanvienDTO staffDto)
        {
            var staff = await _context.Nhanviens
                .FirstOrDefaultAsync(nv => nv.MaNv == id);

            if (staff == null)
            {
                return null;
            }

            if (staffDto.MaChucVu.HasValue)
            {
                var chucVuExists = await _context.Chucvus.AnyAsync(cv => cv.MaChucVu == staffDto.MaChucVu.Value);
                if (!chucVuExists)
                {
                    throw new Exception($"Chức vụ với MaChucVu {staffDto.MaChucVu} không tồn tại.");
                }
            }

            if (staffDto.HinhAnh != null && !string.IsNullOrEmpty(staff.HinhAnh))
            {
                DeleteImage(staff.HinhAnh);
            }

            staff.HoTen = staffDto.HoTen;
            staff.GioiTinh = staffDto.GioiTinh;
            staff.NgaySinh = staffDto.NgaySinh;
            staff.DiaChi = staffDto.DiaChi;
            staff.Cccd = staffDto.Cccd;
            staff.Sdt = staffDto.Sdt;
            staff.Email = staffDto.Email;
            staff.NgayVaoLam = staffDto.NgayVaoLam;
            staff.TenTaiKhoan = staffDto.TenTaiKhoan;
            staff.MatKhau = staffDto.MatKhau;
            staff.TinhTrang = staffDto.TinhTrang;
            staff.MaChucVu = staffDto.MaChucVu;
            staff.HinhAnh = staffDto.HinhAnh != null ? await SaveImage(staffDto.HinhAnh) : staff.HinhAnh;

            await _context.SaveChangesAsync();

            return new NhanvienDTO
            {
                MaNv = staff.MaNv,
                HoTen = staff.HoTen,
                GioiTinh = staff.GioiTinh,
                NgaySinh = staff.NgaySinh,
                DiaChi = staff.DiaChi,
                Cccd = staff.Cccd,
                Sdt = staff.Sdt,
                Email = staff.Email,
                NgayVaoLam = staff.NgayVaoLam,
                TenTaiKhoan = staff.TenTaiKhoan,
                MatKhau = staff.MatKhau,
                TinhTrang = staff.TinhTrang,
                IsDelete = staff.IsDelete,
                MaChucVu = staff.MaChucVu,
                HinhAnh = null
            };
        }

        public async Task<bool> ToggleDeleteStaff(int id)
        {
            var staff = await _context.Nhanviens.FirstOrDefaultAsync(nv => nv.MaNv == id);
            if (staff == null)
            {
                return false;
            }

            staff.IsDelete = true;
            staff.Cccd = null;
            staff.Email = "";
            staff.TenTaiKhoan = null;
            staff.Sdt = "";
            _context.Nhanviens.Update(staff);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            if (image == null) return null;

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "HinhAnh", "AnhKhachHang", fileName);
            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return image.FileName;
        }

        private void DeleteImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var fullPath = Path.Combine(_hostingEnvironment.WebRootPath, imagePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }
    }
}
