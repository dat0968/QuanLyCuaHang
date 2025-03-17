using APIQuanLyCuaHang.DbInitializer;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<QuanLyCuaHangContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuanLyCuaHangContext"));
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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