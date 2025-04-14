using System.Text.Json;
using APIQuanLyCuaHang.DTO;
using APIQuanLyCuaHang.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanLyCuaHang.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    // ? Quản lý cakip
    public class ShiftController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IConfiguration _configuration;

        public ShiftController(IUnitOfWork unit, IConfiguration configuration)
        {
            _unit = unit;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            // await UpdateDataIfNeededAsync();
            var result = await _unit.CaKips.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Employees(int? id)
        {
            var result = await _unit.CaKips.GetAllEmployeesInShiftAsync(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await _unit.CaKips.RemoveAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpsertCrew([FromBody] CaKipDTO caKip)
        {
            var result = await _unit.CaKips.UpsertCrewAsync(caKip);
            return Ok(result);
        }

        [HttpPatch]
        public async Task<IActionResult> ChangeStatusShift(int? id)
        {
            var result = await _unit.CaKips.ChangeStatusAsync(id);
            return Ok(result);
        }

        #region [NON API]
        private async Task UpdateDataIfNeededAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Đọc cấu hình từ file UpdateSettings.json
            var updateSettings = await LoadUpdateSettingsAsync();

            // Nếu không bật cập nhật tự động, không làm gì cả
            if (!updateSettings.EnableDailyUpdate)
            {
                Console.WriteLine("Cập nhật tự động đã bị tắt trong cấu hình.");
                return;
            }

            // Kiểm tra ngày cập nhật cuối
            if (updateSettings.LastUpdated.HasValue && DateOnly.FromDateTime(updateSettings.LastUpdated.Value) >= today)
            {
                Console.WriteLine("Dữ liệu đã được cập nhật hôm nay. Không thực hiện cập nhật.");
                return;
            }

            // Chưa cập nhật hôm nay => Tiến hành cập nhật
            Console.WriteLine("Tiến hành cập nhật dữ liệu...");
            await _unit.CaKips.AutoUpdateAsync(); // Giả sử đây là phương thức cập nhật

            // Lưu lại thời gian hiện tại trong cấu hình
            updateSettings.LastUpdated = DateTime.Now;
            await SaveUpdateSettingsAsync(updateSettings);
            Console.WriteLine("Cập nhật hoàn tất và đã lưu cấu hình mới.");
        }
        private async Task SaveUpdateSettingsAsync(UpdateSettings settings)
        {
            // Đường dẫn tới file UpdateSettings.json
            var settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateSettings.json");

            // Serialize cấu hình thành JSON
            var jsonData = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });

            // Ghi dữ liệu xuống file
            await System.IO.File.WriteAllTextAsync(settingsFilePath, jsonData);

            Console.WriteLine("Đã lưu cấu hình mới vào UpdateSettings.json.");
        }
        private async Task<UpdateSettings> LoadUpdateSettingsAsync()
        {
            // Đường dẫn tới file UpdateSettings.json
            var settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateSettings.json");

            // Nếu file không tồn tại, tạo cấu hình mặc định
            if (!System.IO.File.Exists(settingsFilePath))
            {
                Console.WriteLine("Không tìm thấy file cấu hình. Tạo file mặc định...");
                var defaultSettings = new UpdateSettings
                {
                    EnableDailyUpdate = false,
                    LastUpdated = null
                };

                await SaveUpdateSettingsAsync(defaultSettings);
                return defaultSettings;
            }

            // Đọc và parse nội dung JSON từ file
            var jsonData = await System.IO.File.ReadAllTextAsync(settingsFilePath);
            var settings = JsonSerializer.Deserialize<UpdateSettings>(jsonData);

            return settings ?? new UpdateSettings();
        }
        public class UpdateSettings
        {
            public DateTime? LastUpdated { get; set; }
            public bool EnableDailyUpdate { get; set; }
        }

        #endregion
    }
}
