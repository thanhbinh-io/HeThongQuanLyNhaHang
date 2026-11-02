using Microsoft.EntityFrameworkCore;
using NhaHangThanhThao.Data;
using NhaHangThanhThao.Patterns;
using NhaHangThanhThao.Repositories;
using NhaHangThanhThao.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<NhaHangThanhThaoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBanChoiRepository, BanChoiRepository>();
builder.Services.AddScoped<IMonAnRepository, MonAnRepository>();
builder.Services.AddScoped<IThamSoRepository, ThamSoRepository>();
builder.Services.AddScoped<IDonDatBanRepository, DonDatBanRepository>();
builder.Services.AddScoped<IDonGoiMonRepository, DonGoiMonRepository>();
builder.Services.AddScoped<IHoaDonRepository, HoaDonRepository>();

builder.Services.AddScoped<ITrangThaiService, TrangThaiService>();
builder.Services.AddScoped<IMatKhauService, MatKhauService>();
builder.Services.AddScoped<IMaChungTuFactory, MaChungTuFactory>();
builder.Services.AddScoped<IThanhToanStrategy, ThanhToanMacDinhStrategy>();
builder.Services.AddScoped<IDonDatBanService, DonDatBanService>();
builder.Services.AddScoped<IDonGoiMonService, DonGoiMonService>();
builder.Services.AddScoped<IHoaDonService, HoaDonService>();

// Thêm Session để lưu thông tin đăng nhập
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
