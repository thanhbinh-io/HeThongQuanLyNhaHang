using NhaHangThanhThao.Models;
using NhaHangThanhThao.Patterns;
using NhaHangThanhThao.Repositories;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Services
{
    public interface IHoaDonService
    {
        Task<List<HoaDonIndexViewModel>> LayDanhSachAsync();
        Task<ServiceResult<PhieuTamTinhViewModel>> TaoPhieuTamTinhAsync(string maDonGoiMon, decimal phuThu = 0);
        Task<ServiceResult> XacNhanThanhToanAsync(PhieuTamTinhViewModel model);
        Task<ServiceResult<HoaDonDetailsViewModel>> LayChiTietHoaDonAsync(string maHoaDon);
    }

    public class HoaDonService : IHoaDonService
    {
        private readonly IHoaDonRepository _hoaDonRepository;
        private readonly IDonGoiMonRepository _donGoiMonRepository;
        private readonly IBanChoiRepository _banChoiRepository;
        private readonly IThanhToanStrategy _thanhToanStrategy;
        private readonly IMaChungTuFactory _maChungTuFactory;
        private readonly ITrangThaiService _trangThaiService;
        private readonly IUnitOfWork _unitOfWork;

        public HoaDonService(
            IHoaDonRepository hoaDonRepository,
            IDonGoiMonRepository donGoiMonRepository,
            IBanChoiRepository banChoiRepository,
            IThanhToanStrategy thanhToanStrategy,
            IMaChungTuFactory maChungTuFactory,
            ITrangThaiService trangThaiService,
            IUnitOfWork unitOfWork)
        {
            _hoaDonRepository = hoaDonRepository;
            _donGoiMonRepository = donGoiMonRepository;
            _banChoiRepository = banChoiRepository;
            _thanhToanStrategy = thanhToanStrategy;
            _maChungTuFactory = maChungTuFactory;
            _trangThaiService = trangThaiService;
            _unitOfWork = unitOfWork;
        }

        public Task<List<HoaDonIndexViewModel>> LayDanhSachAsync()
        {
            return _hoaDonRepository.LayDanhSachAsync();
        }

        public async Task<ServiceResult<PhieuTamTinhViewModel>> TaoPhieuTamTinhAsync(string maDonGoiMon, decimal phuThu = 0)
        {
            var don = await _donGoiMonRepository.TimTheoMaAsync(maDonGoiMon);

            if (don == null)
            {
                return ServiceResult<PhieuTamTinhViewModel>.NotFound();
            }

            if (!_trangThaiService.DonDangPhucVu(don.TrangThai))
            {
                return ServiceResult<PhieuTamTinhViewModel>.RedirectDetails(maDonGoiMon);
            }

            var daCoHoaDon = await _hoaDonRepository.CoHoaDonChoDonAsync(maDonGoiMon);

            if (daCoHoaDon)
            {
                return ServiceResult<PhieuTamTinhViewModel>.RedirectDetails(maDonGoiMon);
            }

            var ban = await _banChoiRepository.TimTheoMaAsync(don.MaBan);
            var chiTietMons = await _hoaDonRepository.LayChiTietTamTinhAsync(maDonGoiMon);

            decimal tongTienMon = chiTietMons.Sum(x => x.ThanhTien);
            decimal tienCoc = 0;

            if (!string.IsNullOrEmpty(don.MaDonDatBan))
            {
                tienCoc = await _hoaDonRepository.LayTienCocAsync(don.MaDonDatBan, don.MaBan);
            }

            decimal tienCocKhauTru = _thanhToanStrategy.TinhTienCocKhauTru(tienCoc, tongTienMon, phuThu);

            var model = new PhieuTamTinhViewModel
            {
                MaDonGoiMon = don.MaDonGoiMon,
                TenBan = ban?.TenBan ?? don.MaBan,
                GioVao = don.ThoiGianTao,
                TongTienMon = tongTienMon,
                PhuThu = phuThu,
                TienCocKhauTru = tienCocKhauTru,
                ChiTietMons = chiTietMons
            };

            return ServiceResult<PhieuTamTinhViewModel>.Ok(model);
        }

        public async Task<ServiceResult> XacNhanThanhToanAsync(PhieuTamTinhViewModel model)
        {
            var phieuTamTinhResult = await TaoPhieuTamTinhAsync(model.MaDonGoiMon, model.PhuThu);

            if (!phieuTamTinhResult.ThanhCong || phieuTamTinhResult.DuLieu == null)
            {
                if (phieuTamTinhResult.KhongTimThay)
                {
                    return ServiceResult.NotFound();
                }

                if (phieuTamTinhResult.ChuyenVeChiTietDon && !string.IsNullOrEmpty(phieuTamTinhResult.MaCanChuyen))
                {
                    return ServiceResult.RedirectDetails(phieuTamTinhResult.MaCanChuyen);
                }

                return ServiceResult.Loi(null, phieuTamTinhResult.ThongBao ?? "Không thể thanh toán đơn gọi món");
            }

            var don = await _donGoiMonRepository.TimTheoMaAsync(model.MaDonGoiMon);

            if (don == null)
            {
                return ServiceResult.NotFound();
            }

            var phieuTamTinh = phieuTamTinhResult.DuLieu;

            if (phieuTamTinh.ChiTietMons.Count == 0)
            {
                return ServiceResult.Loi(null, "Đơn gọi món không có món hợp lệ để thanh toán");
            }

            string maHoaDon = _maChungTuFactory.TaoMaHoaDon();

            var hoaDon = new HoaDon
            {
                MaHoaDon = maHoaDon,
                MaDonGoiMon = don.MaDonGoiMon,
                GioVao = don.ThoiGianTao,
                GioRa = DateTime.Now,
                TongTienMon = phieuTamTinh.TongTienMon,
                PhuThu = phieuTamTinh.PhuThu,
                TienCocKhauTru = phieuTamTinh.TienCocKhauTru
            };

            _hoaDonRepository.ThemHoaDon(hoaDon);

            foreach (var item in phieuTamTinh.ChiTietMons)
            {
                var chiTietHoaDon = new HoaDonChiTiet
                {
                    MaHoaDon = maHoaDon,
                    MaMon = item.MaMon,
                    TenMon = item.TenMon,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia
                };

                _hoaDonRepository.ThemChiTiet(chiTietHoaDon);
            }

            don.TrangThai = TrangThaiNhaHang.DonDaThanhToan;

            var ban = await _banChoiRepository.TimTheoMaAsync(don.MaBan);

            if (ban != null)
            {
                ban.TrangThai = TrangThaiNhaHang.BanTrong;
            }

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok(maHoaDon);
        }

        public async Task<ServiceResult<HoaDonDetailsViewModel>> LayChiTietHoaDonAsync(string maHoaDon)
        {
            var hoaDon = await _hoaDonRepository.TimTheoMaAsync(maHoaDon);

            if (hoaDon == null)
            {
                return ServiceResult<HoaDonDetailsViewModel>.NotFound();
            }

            var donGoiMon = await _donGoiMonRepository.TimTheoMaAsync(hoaDon.MaDonGoiMon);
            var ban = donGoiMon == null
                ? null
                : await _banChoiRepository.TimTheoMaAsync(donGoiMon.MaBan);

            var chiTietMons = await _hoaDonRepository.LayChiTietHoaDonAsync(maHoaDon);

            var model = new HoaDonDetailsViewModel
            {
                MaHoaDon = hoaDon.MaHoaDon,
                MaDonGoiMon = hoaDon.MaDonGoiMon,
                TenBan = ban != null ? ban.TenBan : "",
                GioVao = hoaDon.GioVao,
                GioRa = hoaDon.GioRa,
                TongTienMon = hoaDon.TongTienMon,
                PhuThu = hoaDon.PhuThu,
                TienCocKhauTru = hoaDon.TienCocKhauTru,
                TongThanhToan = hoaDon.TongThanhToan ?? _thanhToanStrategy.TinhTongThanhToan(
                    hoaDon.TongTienMon,
                    hoaDon.PhuThu,
                    hoaDon.TienCocKhauTru),
                ChiTietMons = chiTietMons
            };

            return ServiceResult<HoaDonDetailsViewModel>.Ok(model);
        }
    }
}
