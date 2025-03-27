using System.Reflection;
using APIQuanLyCuaHang.DbInitializer;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.CaKip;
using APIQuanLyCuaHang.Repositories.Dashboard;
using APIQuanLyCuaHang.Repositories.LichLamViec;
using APIQuanLyCuaHang.Repositories.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
#region Đăng ký Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    #region Format thêm comment lên môi action
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    #endregion

});
#endregion
builder.Services.AddDbContext<QuanLyCuaHangContext>(options =>
{
options.UseSqlServer(builder.Configuration.GetConnectionString("QuanLyCuaHangContext"));
});

builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddScoped<IDbInitializer, DbInitializer>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.UseAuthorization();

app.MapControllers();

SeedDb();

app.Run();

void SeedDb()
{
    using (var scope = app.Services.CreateScope())
    {
        {
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            try
            {
                dbInitializer.Initializer();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gặp lỗi khi khởi tại dữ liệu: " + ex.Message);
            }
        }
    }
}