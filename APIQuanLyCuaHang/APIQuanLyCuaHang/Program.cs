using System.Reflection;
using APIQuanLyCuaHang.DbInitializer;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Repositories.Bill;
using APIQuanLyCuaHang.Repositories.Category;
using APIQuanLyCuaHang.Repositories.Combo;
using APIQuanLyCuaHang.Repositories.Customer;
using APIQuanLyCuaHang.Repositories.DetailBill;
using APIQuanLyCuaHang.Repositories.DetailCombo;
using APIQuanLyCuaHang.Repositories.DetailProduct;
using APIQuanLyCuaHang.Repositories.ImageProduct;
using APIQuanLyCuaHang.Repositories.Product;
using APIQuanLyCuaHang.Repositories.Table;
using APIQuanLyCuaHang.Respositoies.HashPassword;
using APIQuanLyCuaHang.Respositoies.Token;
using APIQuanLyCuaHang.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using APIQuanLyCuaHang.Repository.MaCoupon;
using APIQuanLyCuaHang.Repositories;
using System.Text;
using APIQuanLyCuaHang.Repositories.UnitOfWork;
using APIQuanLyCuaHang.Repositories.Dashboard;
var builder = WebApplication.CreateBuilder(args);
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Đăng ký Swagger
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    #region Format thêm comment lên môi action
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    option.IncludeXmlComments(xmlPath);
    #endregion    
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
#endregion
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
builder.Services.AddScoped<ComboService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ICategory, Category>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
var SecretKey = builder.Configuration["JWT:SecretKey"];
var SecretKeyBytes = Encoding.UTF8.GetBytes(SecretKey);

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenServices, TokenServices>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IComboRepository, ComboRepository>();
builder.Services.AddScoped<IDetailCombo, DetailCombo>();
builder.Services.AddScoped<IBillRepository, BillRepository>();
builder.Services.AddScoped<IDetailBill, DetailBill>();
builder.Services.AddScoped<IBillRepository, BillRepository>();
builder.Services.AddScoped<IMaCouponRepository, MaCouponRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();


builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
        ClockSkew = TimeSpan.Zero
    };
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddGoogle(options =>
{
    var googleAuth = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuth["ClientId"];
    options.ClientSecret = googleAuth["ClientSecret"];
});
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

app.UseStaticFiles();
app.UseCors("MyPolicy");
app.UseAuthentication();
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