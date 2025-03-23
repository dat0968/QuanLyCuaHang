using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Category;
using APIQuanLyCuaHang.Repositories.DetailProduct;
using APIQuanLyCuaHang.Repositories.ImageProduct;
using APIQuanLyCuaHang.Repositories.Product;
using APIQuanLyCuaHang.Services;
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", ops =>
    {
        ops.AllowAnyHeader();
        ops.AllowAnyMethod();
        ops.AllowAnyOrigin();
    });
});
builder.Services.AddScoped<IProduct, Product>();
builder.Services.AddScoped<IDetailProduct, DetailProduct>();
builder.Services.AddScoped<IimageProduct, imageProduct>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ICategory, Category>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("MyPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
