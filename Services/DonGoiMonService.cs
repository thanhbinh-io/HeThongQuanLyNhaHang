using NhaHangThanhThao.Models;
using NhaHangThanhThao.Patterns;
using NhaHangThanhThao.Repositories;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Services
{
    public interface IDonGoiMonService
    {
        Task<List<DonGoiMonIndexViewModel>> LayDanhSachAsync();
        Task<List<BanChoi>> LayBanCoTheGoiMonAsync();
        Task<List<MonAn>> LayMonDangKinhDoanhAsync();
        Task<ServiceResult> TaoDonGoiMonAsync(DonGoiMonCreateViewModel model, int? maNguoiTao);
        Task<ServiceResult<DonGoiMonDetailsViewModel>> LayChiTietAsync(string id);
        Task<ServiceResult<ThemMonVaoDonViewModel>> TaoModelThemMonAsync(string id);
        Task<ServiceResult> ThemMonAsync(ThemMonVaoDonViewModel model);
        Task<ServiceResult> CapNhatTrangThaiMonAsync(int id, string trangThai);
        Task<ServiceResult<PhieuBepViewModel>> TaoPhieuBepAsync(string id, bool chuyenSangDangCheBien);
    }

    public class DonGoiMonService : IDonGoiMonService
    {
        private readonly IDonGoiMonRepository _donGoiMonRepository;
        private readonly IBanChoiRepository _banChoiRepository;
        private readonly IMonAnRepository _monAnRepository;
        private readonly ITrangThaiService _trangThaiService;
        private readonly IMaChungTuFactory _maChungTuFactory;
        private readonly IUnitOfWork _unitOfWork;

        public DonGoiMonService(
            IDonGoiMonRepository donGoiMonRepository,
            IBanChoiRepository banChoiRepository,
            IMonAnRepository monAnRepository,
            ITrangThaiService trangThaiService,
            IMaChungTuFactory maChungTuFactory,
            IUnitOfWork unitOfWork)
        {
            _donGoiMonRepository = donGoiMonRepository;
            _banChoiRepository = banChoiRepository;
            _monAnRepository = monAnRepository;
            _trangThaiService = trangThaiService;
            _maChungTuFactory = maChungTuFactory;
            _unitOfWork = unitOfWork;
        }

        public Task<List<DonGoiMonIndexViewModel>> LayDanhSachAsync()
        {
            return _donGoiMonRepository.LayDanhSachAsync();
        }

        public Task<List<BanChoi>> LayBanCoTheGoiMonAsync()
        {
            return _banChoiRepository.LayBanCoTheGoiMonAsync();
        }

        public Task<List<MonAn>> LayMonDangKinhDoanhAsync()
        {
            return _monAnRepository.LayMonDangKinhDoanhAsync();
        }

        public async Task<ServiceResult> TaoDonGoiMonAsync(DonGoiMonCreateViewModel model, int? maNguoiTao)
        {
            if (maNguoiTao == null)
            {
                return ServiceResult.Login();
            }

            var ban = await _banChoiRepository.TimTheoMaAsync(model.MaBan);

            if (ban == null)
            {
                return ServiceResult.Loi("MaBan", "Bàn/chòi không tồn tại");
            }

            if (_trangThaiService.BanDangPhucVu(ban.TrangThai))
            {
                return ServiceResult.Loi("MaBan", "Bàn/chòi này đang phục vụ");
            }

            if (ban.TrangThai == TrangThaiNhaHang.BanNgungSuDung)
            {
                return ServiceResult.Loi("MaBan", "Bàn/chòi này đang ngừng sử dụng");
            }

            if (!_trangThaiService.BanCoTheLapDonGoiMon(ban.TrangThai))
            {
                return ServiceResult.Loi("MaBan", "Bàn/chòi này không thể lập đơn gọi món");
            }

            var daCoDonDangPhucVu = await _donGoiMonRepository.CoDonDangPhucVuAsync(model.MaBan);

            if (daCoDonDangPhucVu)
            {
                return ServiceResult.Loi("MaBan", "Bàn/chòi này đã có đơn đang phục vụ");
            }

            var monAn = await _monAnRepository.TimTheoMaAsync(model.MaMon);

            if (monAn == null || !_trangThaiService.MonCoTheGoi(monAn.TrangThai))
            {
                return ServiceResult.Loi("MaMon", "Món ăn không hợp lệ hoặc đang không kinh doanh");
            }

            string maDonGoiMon = _maChungTuFactory.TaoMaDonGoiMon();

            var donGoiMon = new DonGoiMon
            {
                MaDonGoiMon = maDonGoiMon,
                MaBan = model.MaBan,
                MaDonDatBan = null,
                MaNguoiTao = maNguoiTao.Value,
                NguonTao = "NhanVien",
                ThoiGianTao = DateTime.Now,
                TrangThai = TrangThaiNhaHang.DonDangPhucVu,
                GhiChu = model.GhiChu
            };

            var chiTiet = new ChiTietDonGoiMon
            {
                MaDonGoiMon = maDonGoiMon,
                MaMon = model.MaMon,
                SoLuong = model.SoLuong,
                DonGia = monAn.DonGia,
                GhiChu = model.GhiChu,
                TrangThai = TrangThaiNhaHang.ChiTietMoi
            };

            _donGoiMonRepository.ThemDonGoiMon(donGoiMon);
            _donGoiMonRepository.ThemChiTiet(chiTiet);

            ban.TrangThai = TrangThaiNhaHang.BanDangPhucVu;

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok(maDonGoiMon);
        }

        public async Task<ServiceResult<DonGoiMonDetailsViewModel>> LayChiTietAsync(string id)
        {
            var don = await _donGoiMonRepository.TimTheoMaAsync(id);

            if (don == null)
            {
                return ServiceResult<DonGoiMonDetailsViewModel>.NotFound();
            }

            var ban = await _banChoiRepository.TimTheoMaAsync(don.MaBan);
            var chiTietMons = await _donGoiMonRepository.LayChiTietAsync(id);

            var viewModel = new DonGoiMonDetailsViewModel
            {
                MaDonGoiMon = don.MaDonGoiMon,
                MaBan = don.MaBan,
                TenBan = ban?.TenBan ?? don.MaBan,
                ThoiGianTao = don.ThoiGianTao,
                TrangThai = don.TrangThai,
                ChiTietMons = chiTietMons,
                TongTien = chiTietMons
                    .Where(x => _trangThaiService.ChiTietMonDuocTinhTien(x.TrangThai))
                    .Sum(x => x.ThanhTien)
            };

            return ServiceResult<DonGoiMonDetailsViewModel>.Ok(viewModel);
        }

        public async Task<ServiceResult<ThemMonVaoDonViewModel>> TaoModelThemMonAsync(string id)
        {
            var don = await _donGoiMonRepository.TimTheoMaAsync(id);

            if (don == null)
            {
                return ServiceResult<ThemMonVaoDonViewModel>.NotFound();
            }

            if (!_trangThaiService.DonDangPhucVu(don.TrangThai))
            {
                return ServiceResult<ThemMonVaoDonViewModel>.RedirectDetails(id);
            }

            var model = new ThemMonVaoDonViewModel
            {
                MaDonGoiMon = id,
                SoLuong = 1
            };

            return ServiceResult<ThemMonVaoDonViewModel>.Ok(model);
        }

        public async Task<ServiceResult> ThemMonAsync(ThemMonVaoDonViewModel model)
        {
            var don = await _donGoiMonRepository.TimTheoMaAsync(model.MaDonGoiMon);

            if (don == null)
            {
                return ServiceResult.NotFound();
            }

            if (!_trangThaiService.DonDangPhucVu(don.TrangThai))
            {
                return ServiceResult.RedirectDetails(model.MaDonGoiMon);
            }

            var monAn = await _monAnRepository.TimTheoMaAsync(model.MaMon);

            if (monAn == null || !_trangThaiService.MonCoTheGoi(monAn.TrangThai))
            {
                return ServiceResult.Loi("MaMon", "Món ăn không hợp lệ hoặc đang không kinh doanh");
            }

            var chiTiet = new ChiTietDonGoiMon
            {
                MaDonGoiMon = model.MaDonGoiMon,
                MaMon = model.MaMon,
                SoLuong = model.SoLuong,
                DonGia = monAn.DonGia,
                GhiChu = model.GhiChu,
                TrangThai = TrangThaiNhaHang.ChiTietMoi
            };

            _donGoiMonRepository.ThemChiTiet(chiTiet);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> CapNhatTrangThaiMonAsync(int id, string trangThai)
        {
            var chiTiet = await _donGoiMonRepository.TimChiTietAsync(id);

            if (chiTiet == null)
            {
                return ServiceResult.NotFound();
            }

            var don = await _donGoiMonRepository.TimTheoMaAsync(chiTiet.MaDonGoiMon);

            if (don == null)
            {
                return ServiceResult.NotFound();
            }

            if (!_trangThaiService.DonDangPhucVu(don.TrangThai))
            {
                return ServiceResult.RedirectDetails(chiTiet.MaDonGoiMon);
            }

            if (!_trangThaiService.TrangThaiTraHuyMonHopLe(trangThai))
            {
                return ServiceResult.Loi(null, "Trạng thái món không hợp lệ");
            }

            chiTiet.TrangThai = trangThai;
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok(chiTiet.MaDonGoiMon);
        }

        public async Task<ServiceResult<PhieuBepViewModel>> TaoPhieuBepAsync(string id, bool chuyenSangDangCheBien)
        {
            var don = await _donGoiMonRepository.TimTheoMaAsync(id);

            if (don == null)
            {
                return ServiceResult<PhieuBepViewModel>.NotFound();
            }

            if (!_trangThaiService.DonDangPhucVu(don.TrangThai))
            {
                return ServiceResult<PhieuBepViewModel>.RedirectDetails(id);
            }

            var ban = await _banChoiRepository.TimTheoMaAsync(don.MaBan);
            var danhSachMonMoi = await _donGoiMonRepository.LayMonMoiPhieuBepAsync(id);
            bool laPhieuBoSung = await _donGoiMonRepository.CoMonDaChuyenBepAsync(id);

            var model = new PhieuBepViewModel
            {
                MaDonGoiMon = don.MaDonGoiMon,
                MaBan = don.MaBan,
                TenBan = ban?.TenBan ?? don.MaBan,
                ThoiGianTao = don.ThoiGianTao,
                ThoiGianIn = DateTime.Now,
                LaPhieuBoSung = laPhieuBoSung,
                DanhSachMon = danhSachMonMoi
            };

            if (chuyenSangDangCheBien && danhSachMonMoi.Any())
            {
                var ids = danhSachMonMoi
                    .Select(x => x.MaChiTietDonGoiMon)
                    .ToList();

                var chiTietCanChuyen = await _donGoiMonRepository.LayChiTietTheoIdsAsync(ids);

                foreach (var chiTiet in chiTietCanChuyen)
                {
                    if (chiTiet.TrangThai == TrangThaiNhaHang.ChiTietMoi)
                    {
                        chiTiet.TrangThai = TrangThaiNhaHang.ChiTietDangCheBien;
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                model.DaChuyenBep = true;
            }

            return ServiceResult<PhieuBepViewModel>.Ok(model);
        }
    }
}
