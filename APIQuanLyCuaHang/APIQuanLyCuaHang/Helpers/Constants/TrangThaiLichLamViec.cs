using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIQuanLyCuaHang.DTO.Requests;
using APIQuanLyCuaHang.Models;

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

        public static void XuLyTrangThaiLichLamViec(Lichsulamviec schedule, SetStatusOneRequest request, Cakip caKip, int currentEmployees)
        {
            switch (request.TrangThaiCapNhap)
            {
                case TrangThaiLichLamViec.DiLam:
                    if (currentEmployees + 1 > caKip.SoNguoiToiDa)
                    {
                        throw new InvalidOperationException($"Số lượng nhân viên vượt quá giới hạn {caKip.SoNguoiToiDa} của CaKip.");
                    }
                    schedule.GioVao = TimeOnly.FromDateTime(DateTime.Now);
                    break;

                case TrangThaiLichLamViec.KetThucCa:
                    schedule.GioRa = TimeOnly.FromDateTime(DateTime.Now);
                    schedule.SoGioLam = (schedule.GioRa!.Value.ToTimeSpan() - schedule.GioVao!.Value.ToTimeSpan()).TotalHours;
                    break;

                case TrangThaiLichLamViec.NghiPhep:
                case TrangThaiLichLamViec.NghiKhongPhep:
                case TrangThaiLichLamViec.Tre:
                case TrangThaiLichLamViec.KhongDuocXacNhan:
                    if (string.IsNullOrEmpty(request.GhiChu))
                    {
                        throw new ArgumentException("Ghi chú không được để trống cho trạng thái bất thường.");
                    }
                    schedule.GhiChu = request.GhiChu;
                    break;

                default:
                    throw new ArgumentException("Trạng thái cập nhật không hợp lệ.");
            }

            schedule.TrangThai = request.TrangThaiCapNhap;
        }
        public static void XuLyTrangThaiLichLamViec(Lichsulamviec schedule, SetStatusListRequest request, Cakip caKip, int currentEmployees)
        {
            switch (request.TrangThaiCapNhap)
            {
                case TrangThaiLichLamViec.DiLam:
                    if (currentEmployees + request.MaNvs.Length > caKip.SoNguoiToiDa)
                    {
                        throw new InvalidOperationException($"Số lượng nhân viên vượt quá giới hạn {caKip.SoNguoiToiDa} của CaKip.");
                    }
                    schedule.GioVao = TimeOnly.FromDateTime(DateTime.Now);
                    break;

                case TrangThaiLichLamViec.KetThucCa:
                    schedule.GioRa = TimeOnly.FromDateTime(DateTime.Now);
                    schedule.SoGioLam = (schedule.GioRa!.Value.ToTimeSpan() - schedule.GioVao!.Value.ToTimeSpan()).TotalHours;
                    break;

                case TrangThaiLichLamViec.NghiPhep:
                case TrangThaiLichLamViec.NghiKhongPhep:
                case TrangThaiLichLamViec.Tre:
                    if (string.IsNullOrEmpty(request.GhiChu))
                    {
                        throw new ArgumentException("Ghi chú không được để trống cho trạng thái bất thường.");
                    }
                    schedule.GhiChu = request.GhiChu;
                    break;

                default:
                    throw new ArgumentException("Trạng thái cập nhật không hợp lệ.");
            }

            schedule.TrangThai = request.TrangThaiCapNhap;
        }
    }
}