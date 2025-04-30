using APIQuanLyCuaHang.DTO.Requests;
using APIQuanLyCuaHang.Models;

public static class TrangThaiLichLamViec
{
    public const string ChoXacNhan = "Chờ xác nhận";
    public const string DiLam = "Đi làm";
    public const string KetThucCa = "Kết thúc ca";
    public const string NghiPhep = "Nghỉ phép";
    public const string Tre = "Trễ";
    public const string NghiKhongPhep = "Nghỉ không phép";
    public const string KhongDuocXacNhan = "Không được xác nhận";

    public static readonly string ValidateRegex = String.Join("|", (new[]
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

    public static void XuLyTrangThaiLichLamViec(Lichsulamviec schedule, SetStatusOneRequest request, Cakip caKip, int currentEmployees)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        switch (request.TrangThaiCapNhap)
        {
            case ChoXacNhan:
                XuLyChoXacNhan(schedule);
                break;

            case DiLam:
                XuLyDiLam(schedule, caKip, currentEmployees);
                break;

            case KetThucCa:
                XuLyKetThucCa(schedule);
                break;

            case NghiPhep:
            case NghiKhongPhep:
            case Tre:
                XuLyTrangThaiBatThuong(schedule, request);
                break;

            case KhongDuocXacNhan:
                XuLyKhongDuocXacNhan(schedule);
                break;

            default:
                throw new ArgumentException("Trạng thái cập nhật không hợp lệ.");
        }

        schedule.TrangThai = request.TrangThaiCapNhap;
    }

    public static void XuLyTrangThaiLichLamViec(Lichsulamviec schedule, SetStatusListRequest request, Cakip caKip, int currentEmployees)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        switch (request.TrangThaiCapNhap)
        {
            case ChoXacNhan:
                XuLyChoXacNhan(schedule);
                break;

            case DiLam:
                XuLyDiLam(schedule, caKip, currentEmployees, request.MaNvs.Length);
                break;

            case KetThucCa:
                XuLyKetThucCa(schedule);
                break;

            case NghiPhep:
            case NghiKhongPhep:
            case Tre:
                XuLyTrangThaiBatThuong(schedule, request);
                break;

            case KhongDuocXacNhan:
                XuLyKhongDuocXacNhan(schedule);
                break;

            default:
                throw new ArgumentException("Trạng thái cập nhật không hợp lệ.");
        }

        schedule.TrangThai = request.TrangThaiCapNhap;
    }

    private static void XuLyChoXacNhan(Lichsulamviec schedule)
    {
        // Nếu trạng thái là "Chờ xác nhận", không cần hành động gì thêm.
        schedule.TrangThai = ChoXacNhan;
    }

    private static void XuLyKhongDuocXacNhan(Lichsulamviec schedule)
    {
        // Nếu trạng thái là "Không được xác nhận", cập nhật ghi chú nếu cần thiết
        schedule.GhiChu = "Trạng thái không được xác nhận.";
    }

    private static void XuLyDiLam(Lichsulamviec schedule, Cakip caKip, int currentEmployees, int numberOfEmployees = 1)
    {
        if (currentEmployees + numberOfEmployees > caKip.SoNguoiToiDa)
        {
            throw new InvalidOperationException($"Số lượng nhân viên vượt quá giới hạn {caKip.SoNguoiToiDa} của CaKip.");
        }
        schedule.GioVao = TimeOnly.FromDateTime(DateTime.Now);
    }

    private static void XuLyKetThucCa(Lichsulamviec schedule)
    {
        if (schedule.GioVao == null)
        {
            throw new InvalidOperationException("Không thể kết thúc ca nếu không có giờ vào.");
        }
        schedule.GioRa = TimeOnly.FromDateTime(DateTime.Now);
        schedule.SoGioLam = (schedule.GioRa.Value.ToTimeSpan() - schedule.GioVao.Value.ToTimeSpan()).TotalHours;
    }

    private static void XuLyTrangThaiBatThuong(Lichsulamviec schedule, dynamic request)
    {
        if (string.IsNullOrEmpty(request.GhiChu))
        {
            throw new ArgumentException("Ghi chú không được để trống cho trạng thái bất thường.");
        }
        schedule.GhiChu = request.GhiChu;
    }
}