using QRCoder;
using System;
using System.Drawing;
using System.IO;

namespace APIQuanLyCuaHang.Services.QRCode
{
    public class QRCodeService
    {
        public static byte[] GenerateQRCode(string content)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}