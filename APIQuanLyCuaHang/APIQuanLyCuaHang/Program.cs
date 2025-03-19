using APIQuanLyCuaHang.DbInitializer;
using APIQuanLyCuaHang.Models;
using APIQuanLyCuaHang.Respositoies.HashPassword;
using APIQuanLyCuaHang.Respositoies.Token;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
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
builder.Services.AddDbContext<QuanLyCuaHangContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuanLyCuaHangContext"));
});
var SecretKey = builder.Configuration["JWT:SecretKey"];
var SecretKeyBytes = Encoding.UTF8.GetBytes(SecretKey);
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenServices, TokenServices>();

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