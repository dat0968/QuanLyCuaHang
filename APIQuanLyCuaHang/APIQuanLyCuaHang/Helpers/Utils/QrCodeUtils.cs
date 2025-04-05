using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.Helpers.Utils
{
    public static class QrCodeUtils
    {
        public static string GenerateQrCodeData(int maCaKip, DateOnly ngayLam)
        {
            string qrData = $"{maCaKip}-{ngayLam.ToString("yyyyMMdd")}";
            return qrData;
        }

        public static (int, DateOnly) ParseQrCodeData(string qrCodeData)
        {
            string[] parts = qrCodeData.Split('-');
            if (parts.Length != 2)
            {
                throw new Exception($"QR Code không hợp lệ [{qrCodeData}].");
            }

            if (!int.TryParse(parts[0], out int maCaKip))
            {
                throw new Exception("Mã ca kíp không hợp lệ.");
            }

            if (!DateOnly.TryParseExact(parts[1], "yyyyMMdd", out DateOnly ngayLam))
            {
                throw new Exception("Ngày làm không hợp lệ.");
            }

            return (maCaKip, ngayLam);
        }
    }
}