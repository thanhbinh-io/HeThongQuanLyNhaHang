using NhaHangThanhThao.Models;
using NhaHangThanhThao.Patterns;
using NhaHangThanhThao.Repositories;
using NhaHangThanhThao.ViewModels;

namespace NhaHangThanhThao.Services
{
    public interface IDonDatBanService
    {
        Task<List<DonDatBanIndexViewModel>> LayDanhSachAsync(DateTime? ngayDat = null);
        Task<List<BanChoi>> LayTatCaBanAsync();
        Task<List<BanChoi>> LayBanTrongAsync();
        Task<ServiceResult<PhieuDatBanViewModel>> LayPhieuDatBanAsync(string maDonDatBan);
        Task<ServiceResult> TaoDonDatBanAsync(DonDatBanCreateViewModel model, int? maNguoiLap);
        Task<ServiceResult> ChuyenSangDaDatAsync(string maDonDatBan, string maBan);
        Task<ServiceResult> XoaDonDatBanAsync(string maDonDatBan);
    }

    public class DonDatBanService : IDonDatBanService
    {
        private const string SoTienCocToiThieu = "SoTienCocToiThieu";

        private readonly IDonDatBanRepository _donDatBanRepository;
        private readonly IBanChoiRepository _banChoiRepository;
        private readonly IThamSoRepository _thamSoRepository;
        private readonly ITrangThaiService _trangThaiService;
        private readonly IMaChungTuFactory _maChungTuFactory;
        private readonly IUnitOfWork _unitOfWork;

        public DonDatBanService(
            IDonDatBanRepository donDatBanRepository,
            IBanChoiRepository banChoiRepository,
            IThamSoRepository thamSoRepository,
            ITrangThaiService trangThaiService,
            IMaChungTuFactory maChungTuFactory,
            IUnitOfWork unitOfWork)
        {
            _donDatBanRepository = donDatBanRepository;
            _banChoiRepository = banChoiRepository;
            _thamSoRepository = thamSoRepository;
            _trangThaiService = trangThaiService;
            _maChungTuFactory = maChungTuFactory;
            _unitOfWork = unitOfWork;
        }

        public Task<List<DonDatBanIndexViewModel>> LayDanhSachAsync(DateTime? ngayDat = null)
        {
            return _donDatBanRepository.LayDanhSachAsync(ngayDat);
        }

        public Task<List<BanChoi>> LayTatCaBanAsync()
        {
            return _banChoiRepository.LayTatCaAsync();
        }

        public Task<List<BanChoi>> LayBanTrongAsync()
        {
            return _banChoiRepository.LayBanTrongAsync();
        }

        public async Task<ServiceResult<PhieuDatBanViewModel>> LayPhieuDatBanAsync(string maDonDatBan)
        {
            var phieu = await _donDatBanRepository.LayPhieuDatBanAsync(maDonDatBan);

            if (phieu == null)
            {
                return ServiceResult<PhieuDatBanViewModel>.NotFound();
            }

            return ServiceResult<PhieuDatBanViewModel>.Ok(phieu);
        }

        public async Task<ServiceResult> TaoDonDatBanAsync(DonDatBanCreateViewModel model, int? maNguoiLap)
        {
            if (model.ThoiGianDen < DateTime.Now)
            {
                return ServiceResult.Loi("ThoiGianDen", "Thời gian đến không được nhỏ hơn thời gian hiện tại");
            }

            if (maNguoiLap == null)
            {
                return ServiceResult.Login();
            }

            var ban = await _banChoiRepository.TimTheoMaAsync(model.MaBan);

            if (ban == null)
            {
                return ServiceResult.Loi("MaBan", "Bàn/chòi không tồn tại");
            }

            if (ban.TrangThai == TrangThaiNhaHang.BanNgungSuDung)
            {
                return ServiceResult.Loi("MaBan", "Bàn/chòi này đang ngừng sử dụng");
            }

            bool banDaCoDonTrongNgay = await _donDatBanRepository.BanDaCoDonTrongNgayAsync(
                model.MaBan,
                model.ThoiGianDen);

            if (banDaCoDonTrongNgay)
            {
                return ServiceResult.Loi("MaBan", "Bàn/chòi này đã có đơn đặt trong ngày đã chọn");
            }

            decimal tienCocToiThieu = await _thamSoRepository.LayGiaTriAsync(SoTienCocToiThieu) ?? 300000;

            if (model.TienCoc < tienCocToiThieu)
            {
                return ServiceResult.Loi("TienCoc", $"Tiền cọc tối thiểu là {tienCocToiThieu:N0} VNĐ");
            }

            string maDonDatBan = _maChungTuFactory.TaoMaDonDatBan();

            var donDatBan = new DonDatBan
            {
                MaDonDatBan = maDonDatBan,
                TenKhachHang = model.TenKhachHang,
                SoDienThoai = model.SoDienThoai,
                NgayLap = DateTime.Now,
                ThoiGianDen = model.ThoiGianDen,
                SoLuongKhach = model.SoLuongKhach,
                MaNguoiLap = maNguoiLap.Value,
                GhiChu = model.GhiChu
            };

            var chiTiet = new ChiTietDonDatBan
            {
                MaDonDatBan = maDonDatBan,
                MaBan = model.MaBan,
                TienCoc = model.TienCoc,
                GhiChu = model.GhiChu
            };

            _donDatBanRepository.ThemDonDatBan(donDatBan);
            _donDatBanRepository.ThemChiTiet(chiTiet);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok(maDonDatBan);
        }

        public async Task<ServiceResult> ChuyenSangDaDatAsync(string maDonDatBan, string maBan)
        {
            var donDatBan = await _donDatBanRepository.TimTheoMaAsync(maDonDatBan);

            if (donDatBan == null)
            {
                return ServiceResult.NotFound();
            }

            var ban = await _banChoiRepository.TimTheoMaAsync(maBan);

            if (ban == null)
            {
                return ServiceResult.NotFound();
            }

            if (!_trangThaiService.BanCoTheDatTruoc(ban.TrangThai))
            {
                return ServiceResult.Loi(null, "Bàn/chòi này hiện không trống, không thể chuyển sang đã đặt.");
            }

            ban.TrangThai = TrangThaiNhaHang.BanDaDatTruoc;
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> XoaDonDatBanAsync(string maDonDatBan)
        {
            var donDatBan = await _donDatBanRepository.TimTheoMaAsync(maDonDatBan);

            if (donDatBan == null)
            {
                return ServiceResult.NotFound();
            }

            bool daPhatSinhDonGoiMon = await _donDatBanRepository.DaPhatSinhDonGoiMonAsync(maDonDatBan);

            if (daPhatSinhDonGoiMon)
            {
                return ServiceResult.Loi(null, "Không thể xóa đơn đặt bàn đã phát sinh đơn gọi món.");
            }

            var chiTiets = await _donDatBanRepository.LayChiTietTheoDonAsync(maDonDatBan);

            foreach (var chiTiet in chiTiets)
            {
                var ban = await _banChoiRepository.TimTheoMaAsync(chiTiet.MaBan);

                if (ban?.TrangThai == TrangThaiNhaHang.BanDaDatTruoc)
                {
                    ban.TrangThai = TrangThaiNhaHang.BanTrong;
                }
            }

            _donDatBanRepository.XoaChiTiet(chiTiets);
            _donDatBanRepository.XoaDonDatBan(donDatBan);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok();
        }
    }
}
