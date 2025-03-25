using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Constants
{
    public static class TrangThaiLichLamViec
    {
        public const string ChoXacNhan = "Chờ xác nhận";
        public const string DiLam = "Đi làm";
        public const string KetThucCa = "Kết thúc ca";
        public const string NghiPhep = "Nghỉ phép";
        public const string Tre = "Trễ";
        public const string NghiKhongPhep = "Nghỉ không phép";
        public const string KhongDuocXacNhan = "Không được xác nhận";

        public static readonly string ValidateRegex = String.Join("|", (new
        {
            ChoXacNhan,
            DiLam,
            KetThucCa,
            NghiPhep,
            Tre,
            NghiKhongPhep,
            KhongDuocXacNhan
        }));
        public static readonly List<string> TatCaTrangThai = new List<string>
        {
            ChoXacNhan,
            DiLam,
            KetThucCa,
            NghiPhep,
            Tre,
            NghiKhongPhep,
            KhongDuocXacNhan
        };
        public static readonly List<string> TrangThaiBatThuong = new List<string>
        {
            NghiPhep,
            Tre,
            NghiKhongPhep,
            KhongDuocXacNhan
        };
    }
}