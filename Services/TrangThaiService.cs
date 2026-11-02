namespace NhaHangThanhThao.Services
{
    public static class TrangThaiNhaHang
    {
        public const string BanTrong = "Trong";
        public const string BanDaDatTruoc = "DaDatTruoc";
        public const string BanDangPhucVu = "DangPhucVu";
        public const string BanNgungSuDung = "NgungSuDung";

        public const string MonDangKinhDoanh = "DangKinhDoanh";

        public const string DonDangPhucVu = "DangPhucVu";
        public const string DonDaThanhToan = "DaThanhToan";

        public const string ChiTietMoi = "Moi";
        public const string ChiTietDangCheBien = "DangCheBien";
        public const string ChiTietDaPhucVu = "DaPhucVu";
        public const string ChiTietDaHuy = "DaHuy";
        public const string ChiTietTraMon = "TraMon";
    }

    public interface ITrangThaiService
    {
        bool BanCoTheDatTruoc(string trangThai);
        bool BanCoTheLapDonGoiMon(string trangThai);
        bool BanDangPhucVu(string trangThai);
        bool DonDangPhucVu(string trangThai);
        bool MonCoTheGoi(string trangThai);
        bool TrangThaiTraHuyMonHopLe(string trangThai);
        bool ChiTietMonDuocTinhTien(string trangThai);
    }

    public class TrangThaiService : ITrangThaiService
    {
        public bool BanCoTheDatTruoc(string trangThai)
        {
            return trangThai == TrangThaiNhaHang.BanTrong;
        }

        public bool BanCoTheLapDonGoiMon(string trangThai)
        {
            return trangThai == TrangThaiNhaHang.BanTrong
                || trangThai == TrangThaiNhaHang.BanDaDatTruoc;
        }

        public bool BanDangPhucVu(string trangThai)
        {
            return trangThai == TrangThaiNhaHang.BanDangPhucVu;
        }

        public bool DonDangPhucVu(string trangThai)
        {
            return trangThai == TrangThaiNhaHang.DonDangPhucVu;
        }

        public bool MonCoTheGoi(string trangThai)
        {
            return trangThai == TrangThaiNhaHang.MonDangKinhDoanh;
        }

        public bool TrangThaiTraHuyMonHopLe(string trangThai)
        {
            return trangThai == TrangThaiNhaHang.ChiTietDaHuy
                || trangThai == TrangThaiNhaHang.ChiTietTraMon;
        }

        public bool ChiTietMonDuocTinhTien(string trangThai)
        {
            return trangThai != TrangThaiNhaHang.ChiTietDaHuy
                && trangThai != TrangThaiNhaHang.ChiTietTraMon;
        }
    }
}
