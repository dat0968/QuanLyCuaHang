using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIQuanLyCuaHang.DbInitializer
{
    //Random dữ liệu cho csdl
    public class RandomData_DB
    {
        private static readonly Lazy<RandomData_DB> instance = new Lazy<RandomData_DB>(() => new RandomData_DB());
        public static RandomData_DB Instance = instance.Value;

        #region//Tên Cake
        List<string> _CakeName1 = new List<string>() { "Hoang", "Thiên", "Địa", "Nhân" };
        List<string> _CakeName2 = new List<string>() { "Thực", "Bảo", "Liên", "Mễ" };
        #endregion

        #region//Danh sách tên
        string[] listName1 = { "Trần", "Chí", "Trung", "Tráng", "Khánh", "Lương", "Trúc", "Anh", "Đài" };
        string[] listName2 = { "Giang", "Bát", "Ánh", "Sĩ", "Sơn", "Thực", "Vi", "Anh", "Loan" };
        string[] listName3 = { "Đế", "Quỵ", "Nguyệt", "Trai", "Sắc", "Phàm", "Vị", "Quái", "Bối" };
        #endregion

        #region//Tên hành tinh
        string[] planets = { "Mặt trăng", "Sao Thiên Vương", "Sao Hải Vương" };
        #endregion

        #region//Tên đơn vị tính
        List<string> _cakeDonViTinh = new List<string>() { "Tô", "Bát", "Đĩa", "1 phần", "1 bàn", "Tự do gọi" };
        #endregion

        #region//Danh sách ảnh thức ăn
        List<string> _linkFoodImage = new List<string>() { "https://i.pinimg.com/564x/29/2c/56/292c563994044c90473cd7cd4d58078a.jpg", "https://i.pinimg.com/736x/fb/5a/b0/fb5ab077e12b4c74181cc5f651e6e984.jpg", "https://i.pinimg.com/736x/0d/66/3c/0d663c7ff659fae2ab56ea135515a8d1.jpg", "https://i.pinimg.com/736x/0d/66/3c/0d663c7ff659fae2ab56ea135515a8d1.jpg", "https://i.pinimg.com/736x/f9/24/a1/f924a113b4e560fe5dcaff31716aa098.jpg", "https://i.pinimg.com/564x/f6/be/31/f6be31b5bfc8bb6f119f18dd8ca3a270.jpg" };
        #endregion

        #region//Danh sách ảnh bánh mì
        List<string> _breadImage = new List<string>() { "https://i.pinimg.com/736x/83/d4/1b/83d41b3be59a8f2b325891bf996e44e8.jpg", "https://i.pinimg.com/564x/7d/af/1e/7daf1e5c315b2cc805d0f80ca476fcf8.jpg", "https://i.pinimg.com/564x/c0/9f/39/c09f39a802af839ef5a3e60052177aa4.jpg", "https://i.pinimg.com/564x/0e/fc/23/0efc23915d063ee5f7881d3b04b350b0.jpg", "https://i.pinimg.com/564x/f6/c8/86/f6c8862d867fdee7dd5b194d27747cae.jpg", "https://i.pinimg.com/564x/23/1c/1a/231c1a4e9c8f2f1e2a4d18dc086cb8ae.jpg" };
        #endregion

        #region//Danh sách người dùng
        List<string> _userImage = new List<string>() { "https://i.pinimg.com/736x/c2/8d/d8/c28dd8b5e5ab6c5668c627e7129b5f9d.jpg", "https://i.pinimg.com/736x/9f/5d/25/9f5d254e9c42897b90dc86e82ba44bd9.jpg", "https://i.pinimg.com/736x/47/d6/03/47d6035cf8f8b957b1da35e8c6d93511.jpg", "https://i.pinimg.com/564x/e1/ff/68/e1ff68caa7eb40a5c903152f100e4fc3.jpg", "https://i.pinimg.com/736x/2b/d2/72/2bd2720fc597ae583f25c4ba9e0c48f9.jpg", "https://i.pinimg.com/564x/b8/a2/d1/b8a2d11155df433c8423dad1758868fd.jpg", "https://images.pexels.com/photos/3035875/pexels-photo-3035875.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/5857355/pexels-photo-5857355.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/28874283/pexels-photo-28874283/free-photo-of-khong-gian-lam-vi-c-t-i-gi-n-v-i-may-tinh-xach-tay-va-c-c-ca-phe.jpeg?auto=compress&cs=tinysrgb&w=600&lazy=load", "https://images.pexels.com/photos/28494944/pexels-photo-28494944.jpeg?auto=compress&cs=tinysrgb&w=600&lazy=load", "https://images.pexels.com/photos/28570315/pexels-photo-28570315/free-photo-of-ng-i-ph-n-tr-v-i-may-tinh-b-ng-va-tai-nghe.jpeg?auto=compress&cs=tinysrgb&w=600&lazy=load", "https://i.pinimg.com/control/564x/b2/9a/90/b29a904f241d59034715eaf44eaad426.jpg" };
        #endregion

        #region//Danh sách ảnh chi nhánh
        List<string> _linkBranchImage = new List<string>() { "https://i.pinimg.com/564x/55/08/e1/5508e15188aff6e7d287399d1395bd56.jpg", "https://i.pinimg.com/564x/8f/88/c2/8f88c2018a75f40ef7a14016d1890d75.jpg", "https://i.pinimg.com/564x/00/7d/b4/007db4a042d296c457ef18af3b095e78.jpg", "https://i.pinimg.com/564x/95/10/8d/95108d00785a290e4d73cfe67094ec30.jpg", "https://i.pinimg.com/564x/c5/f8/f8/c5f8f8da8950a2004e40ce9ea4051116.jpg", "https://i.pinimg.com/564x/6c/cd/31/6ccd315173ea7a1ffa7d0849b526d5bc.jpg" };
        #endregion

        #region//Chuỗi chữ cái
        string _stringLetter = "qwertyuiopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
        #endregion

        #region//Đuôi email
        string[] domains = { "@gmail.com", "@yahoo.com", "@outlook.com", "@example.com" };
        #endregion

        #region//Mô tả cake
        string[] descript = { "Món này ngon nhưng nhiều mỡ", "Ăn nhiều dễ béo", "Ăn ít thôi" };
        #endregion

        #region//Random video Bonsai
        string[] linkVideos = { "https://videos.pexels.com/video-files/853839/853839-sd_640_360_30fps.mp4", "https://videos.pexels.com/video-files/3132090/3132090-sd_640_360_24fps.mp4", "https://videos.pexels.com/video-files/856030/856030-sd_640_360_25fps.mp4", "https://videos.pexels.com/video-files/3176704/3176704-sd_360_640_30fps.mp4", "https://videos.pexels.com/video-files/28967565/12531601_640_360_60fps.mp4", "https://videos.pexels.com/video-files/1494279/1494279-sd_640_360_24fps.mp4", "https://videos.pexels.com/video-files/4947342/4947342-sd_640_360_30fps.mp4" };
        #endregion

        #region//Random image Bonsai
        string[] linkImgBonsai = { "https://i.pinimg.com/736x/39/0c/0a/390c0a43f21becd7305ad73a48760f12.jpg", "https://i.pinimg.com/564x/0b/7d/ae/0b7dae3946327c7b03726c0f41ae1def.jpg", "https://i.pinimg.com/564x/4f/97/20/4f9720e73caa7c1a276df615457188c7.jpg", "https://images.pexels.com/photos/1382195/pexels-photo-1382195.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/2149105/pexels-photo-2149105.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/6072061/pexels-photo-6072061.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/4050790/pexels-photo-4050790.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/5765694/pexels-photo-5765694.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/5831008/pexels-photo-5831008.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/28822467/pexels-photo-28822467/free-photo-of-cay-bonsai-phong-mua-thu-r-c-r-tren-n-n-t-i.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/6852309/pexels-photo-6852309.jpeg?auto=compress&cs=tinysrgb&w=600" };
        #endregion


        #region//Hinh ảnh combo Bonsai
        string[] imagesComboBonsai = new string[] { "https://cdn.pixabay.com/photo/2016/02/10/21/59/landscape-1192669_640.jpg", "https://cdn.pixabay.com/photo/2023/10/12/07/34/mountain-8310076_640.jpg", "https://cdn.pixabay.com/photo/2016/10/19/08/57/mountains-1752433_640.jpg", "https://cdn.pixabay.com/photo/2016/08/12/23/44/river-1590010_640.jpg", "https://cdn.pixabay.com/photo/2022/05/23/11/26/tree-7215935_640.jpg", "https://cdn.pixabay.com/photo/2023/06/16/21/13/landscape-8068792_640.jpg" };
        #endregion

        #region//Mô tả phổ biến
        string[] descriptTexts = new string[] { "Bạn đang tìm kiếm một sản phẩm với thiết kế tinh tế, tính năng vượt trội và độ bền cao? Đây chính là giải pháp hoàn hảo dành cho bạn!\r\n\r\nSản phẩm được tạo ra từ sự kết hợp hoàn hảo giữa công nghệ hiện đại và sự tỉ mỉ trong sản xuất. Với chất liệu cao cấp và kiểu dáng thời thượng, đây không chỉ là công cụ hỗ trợ mà còn là món phụ kiện mang lại sự đẳng cấp.\r\nKhả năng ứng dụng rộng rãi: sử dụng linh hoạt từ công việc, học tập đến giải trí, khiến sản phẩm phù hợp với mọi hoàn cảnh và nhu cầu của bạn.\r\nĐược sản xuất trên dây chuyền tiên tiến đạt tiêu chuẩn quốc tế, sản phẩm đảm bảo chất lượng vượt trội từ nguồn gốc vật liệu đến khâu hoàn thiện.\r\nĐiểm nổi bật nhất là tính năng độc quyền, mang lại hiệu suất cao và trải nghiệm sử dụng khác biệt, giúp bạn tối ưu hóa thời gian và năng suất.\r\nCách sử dụng đơn giản và thân thiện, người dùng có thể dễ dàng vận hành mà không cần mất nhiều thời gian tìm hiểu.\r\nĐừng bỏ lỡ, hãy thử Date để trải nghiệm sự khác biệt mà sản phẩm này mang lại — một sự đầu tư vượt xa mong đợi của bạn!", "Được thiết kế với sự tỉ mỉ trong từng chi tiết, sản phẩm này nổi bật nhờ kiểu dáng hiện đại và chất lượng vượt trội.\r\n\r\nSản phẩm được sản xuất từ nguyên liệu cao cấp và quy trình kiểm tra nghiêm ngặt, đảm bảo độ bền và sự an toàn tuyệt đối cho người sử dụng.\r\nPhù hợp với nhiều mục đích sử dụng khác nhau: làm việc, giải trí hoặc cho những chuyến đi xa.\r\nVới tính năng thông minh và dễ sử dụng, sản phẩm này giúp bạn tiết kiệm thời gian và tận dụng tối đa mọi tiềm năng cá nhân.", "Mang lại sự tiện ích tối đa, sản phẩm này được tích hợp các tính năng hiện đại với hiệu suất vượt trội.\r\n\r\nKích thước nhỏ gọn, thiết kế tối giản nhưng vẫn đảm bảo sự mạnh mẽ và bền bỉ trong từng công năng.\r\nĐược chế tạo từ vật liệu thân thiện với môi trường, giúp giảm tác động tiêu cực đến thiên nhiên.\r\nĐiểm nổi bật nằm ở khả năng hoạt động linh hoạt cùng tuổi thọ lâu dài, phù hợp với nhu cầu sử dụng cá nhân và chuyên nghiệp.", "Không chỉ đơn thuần là một sản phẩm, đây thực sự là người bạn đồng hành lý tưởng, giúp nâng tầm chất lượng cuộc sống của bạn.\r\n\r\nĐược sản xuất dựa trên công nghệ tiên tiến, mang lại hiệu suất cao, đảm bảo đáp ứng mọi yêu cầu khắt khe nhất của người dùng.\r\nVới thiết kế đẹp mắt và sự tiện dụng, sản phẩm này phù hợp cho mọi không gian và môi trường sử dụng.\r\nĐộc đáo hơn nữa là tính năng tiết kiệm năng lượng, giúp giảm thiểu chi phí sử dụng đáng kể.", "Sản phẩm là sự lựa chọn lý tưởng cho những ai yêu thích sự hiện đại và tiện nghi thuộc phân khúc cao cấp.\r\n\r\nVới thiết kế tinh tế, sản phẩm mang lại sự sang trọng và phong cách khác biệt khi sử dụng.\r\nCác tính năng tối ưu hóa được nghiên cứu tỉ mỉ, cung cấp trải nghiệm hiệu quả mà không gây phức tạp hay khó chịu.\r\nĐặc biệt, tính an toàn và độ tin cậy được nhà sản xuất đặt lên hàng đầu, giúp bạn thoải mái tận hưởng mà không cần lo lắng.", "Nếu bạn đang tìm kiếm một sản phẩm vừa bền, vừa thẩm mỹ lại đáp ứng được mọi nhu cầu sử dụng, thì đây chính là sự lựa chọn hoàn hảo.\r\n\r\nĐược chế tác từ vật liệu cao cấp, sản phẩm đảm bảo độ cứng cáp và tuổi thọ lâu dài Date cả trong điều kiện khắc nghiệt.\r\nĐặc biệt phù hợp cho những ai cần sự chuyên nghiệp và hiệu quả, với khả năng vận hành mượt mà đáng kinh ngạc.\r\nSản phẩm còn sở hữu nhiều tùy chọn để cá nhân hóa, phù hợp với mọi gu thẩm mỹ và sở thích của khách hàng.", "Sản phẩm được thiết kế để tối ưu hóa mọi trải nghiệm của bạn, dù là làm việc, giải trí hay trong cuộc sống thường ngày.\r\n\r\nĐiểm nhấn của sản phẩm nằm ở thiết kế đa năng, cho phép sử dụng trong mọi tình huống mà không làm giảm chất lượng.\r\nĐược sản xuất từ các chất liệu bền vững, sản phẩm vừa đẹp mắt, vừa thân thiện với môi trường.\r\nCác chi tiết được chăm chút hoàn hảo, thể hiện được sự chuyên nghiệp và tận tâm từ đội ngũ sản xuất.", "Một lựa chọn hoàn hảo dành cho những ai tìm kiếm sản phẩm vừa hiệu quả, vừa mạng đậm phong cách cá nhân.\r\n\r\nSản phẩm sở hữu các tính năng vượt trội, mang lại cảm giác thoải mái và tiện nghi dù sử dụng trong thời gian dài.\r\nVới thiết kế có độ thẩm mỹ cao, đây không chỉ là công cụ hỗ trợ mà còn là yếu tố làm nổi bật không gian sống của bạn.\r\nĐặc biệt, khả năng vận hành êm ái cùng việc tiết kiệm năng lượng là ưu thế lớn khiến sản phẩm này được yêu thích.", "Tạo ra sự đột phá trong cuộc sống hàng ngày với sản phẩm mang tính đột phá vượt xa mong đợi của bạn.\r\n\r\nĐược trang bị các công nghệ mới nhất, sản phẩm này giúp công việc của bạn trở nên đơn giản và đạt hiệu suất cao hơn.\r\nThiết kế linh hoạt, dễ dàng mang theo, sản phẩm này sẵn sàng hỗ trợ bạn bất kỳ nơi đâu.\r\nCác tính năng an toàn hiện đại giúp người dùng yên tâm trong suốt quá trình trải nghiệm.", "Khám phá bước ngoặt mới trong công nghệ với sản phẩm được thiết kế dành riêng để đáp ứng nhu cầu phong phú của bạn.\r\n\r\nKhông chỉ nổi bật với ngoại hình bắt mắt, sản phẩm còn được trang bị các tính năng mạnh mẽ đáng kinh ngạc.\r\nSẵn sàng đồng hành cùng bạn mọi lúc, mọi nơi với độ bền cao, khả năng chống chịu tốt trong mọi điều kiện.\r\nDễ dàng sử dụng cho mọi đối tượng, dù bạn là người mới hay đã quen thuộc với sản phẩm tương tự trước đây." };
        #endregion

        Random rd = new Random();
        public string _CakeName()
        {
            return String.Join(" ", new string[] { _CakeName1[rd.Next(0, _CakeName1.Count)], _CakeName2[rd.Next(0, _CakeName2.Count)] });
        }
        public string _CakeDonViTinh()
        {
            return _cakeDonViTinh[rd.Next(0, _cakeDonViTinh.Count)];
        }

        public string _CakeImage()
        {
            return _linkFoodImage[rd.Next(0, _linkFoodImage.Count)];
        }

        public string _BranchImage()
        {
            return _linkBranchImage[rd.Next(_linkBranchImage.Count)];
        }

        public string _UserImage()
        {
            return _userImage[rd.Next(_userImage.Count)];
        }
        public string rdString()
        {
            return new string(new char[10].Select(C => _stringLetter[rd.Next(0, _stringLetter.Length)]).ToArray());
        }

        public string rdName()
        {
            return $"{listName1[rd.Next(0, listName1.Length)]} {listName2[rd.Next(0, listName2.Length)]} {listName3[rd.Next(0, listName3.Length)]}";
        }

        public string rdAddress()
        {
            return $"{planets[rd.Next(0, planets.Length)]}, tọa độ: {rd.Next(-90, 90)} Kinh độ, {rd.Next(-180, 180)} Vĩ độ";
        }

        public string _Email()
        {
            return rdString() + domains[rd.Next(0, domains.Length)];

        }

        public string _Descript()
        {
            return descript[rd.Next(0, descript.Length)];
        }

        public string _BreadImage()
        {
            return _breadImage[rd.Next(_breadImage.Count)];
        }

        #region CS4

        // Random danh mục sản phẩm
        public string RandomCategoryName()
        {
            string[] categories = { "Cây lá màu", "Cây thân gỗ bonsai", "Cây hoa cảnh",
                                "Cây xương rồng và cây mọng nước", "Cây cảnh để bàn",
                                "Cây cảnh thủy sinh", "Cây phong thủy", "Cây leo và cây treo" };
            return categories[rd.Next(categories.Length)];
        }

        // Random mô tả danh mục
        public string RandomCategoryDescription()
        {
            string[] descriptions = { "Trang trí nội thất", "Độ bền cao", "Dễ chăm sóc",
                                  "Phù hợp văn phòng", "Trang trí ban công",
                                  "Tăng vượng khí", "Thu hút tài lộc", "Làm sạch không khí" };
            return descriptions[rd.Next(descriptions.Length)];
        }

        // Random tên sản phẩm cây
        public string RandomProductName()
        {
            string[] products = { "Cây Lan Ý", "Cây Kim Tiền", "Cây Lưỡi Hổ", "Cây Trầu Bà",
                              "Cây Sen Đá", "Cây Xương Rồng", "Cây Tùng La Hán",
                              "Cây Cọ Nhật", "Cây Đuôi Công" };
            return products[rd.Next(products.Length)];
        }

        // Random mô tả sản phẩm cây
        public string RandomProductDescription()
        {
            string[] descriptions = { "Cây dễ chăm sóc", "Cây thu hút tài lộc", "Lá đẹp, làm sạch không khí",
                                  "Cây chịu bóng tốt", "Trang trí nội thất và bàn làm việc" };
            return descriptions[rd.Next(descriptions.Length)];
        }

        // Random ảnh sản phẩm mẫu
        public string RandomProductImage()
        {
            string[] imageUrls = { "https://example.com/image1.jpg", "https://example.com/image2.jpg",
                               "https://example.com/image3.jpg", "https://example.com/image4.jpg" };
            return imageUrls[rd.Next(imageUrls.Length)];
        }

        // Random ngày thuê nhân viên
        public DateTime RandomHiredDate()
        {
            return DateTime.UtcNow.AddDays(-rd.Next(365, 1825)); // Từ 1 đến 5 năm trước
        }
        // Random số điện thoại
        public string RandomPhone()
        {
            return "09" + rd.Next(10000000, 99999999).ToString();
        }
        // Random video
        public string RandomVideoBonsai()
        {
            return linkVideos[rd.Next(linkVideos.Length)];
        }
        // Random image bonsai
        public string RandomImageBonsai()
        {
            return linkImgBonsai[rd.Next(linkImgBonsai.Length)];
        }

        // Random image combo bonsai
        public string RandomImageComboBonsai()
        {
            return imagesComboBonsai[rd.Next(imagesComboBonsai.Length)];
        }
        #endregion
    }
}