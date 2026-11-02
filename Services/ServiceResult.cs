namespace NhaHangThanhThao.Services
{
    public class ServiceResult
    {
        public bool ThanhCong { get; set; }
        public bool KhongTimThay { get; set; }
        public bool ChuyenVeDangNhap { get; set; }
        public bool ChuyenVeChiTietDon { get; set; }
        public string? MaCanChuyen { get; set; }
        public string? TruongLoi { get; set; }
        public string? ThongBao { get; set; }
        public string? MaMoi { get; set; }

        public static ServiceResult Ok(string? maMoi = null)
        {
            return new ServiceResult { ThanhCong = true, MaMoi = maMoi };
        }

        public static ServiceResult Loi(string? truongLoi, string thongBao)
        {
            return new ServiceResult { TruongLoi = truongLoi, ThongBao = thongBao };
        }

        public static ServiceResult NotFound()
        {
            return new ServiceResult { KhongTimThay = true };
        }

        public static ServiceResult Login()
        {
            return new ServiceResult { ChuyenVeDangNhap = true };
        }

        public static ServiceResult RedirectDetails(string maDonGoiMon)
        {
            return new ServiceResult { ChuyenVeChiTietDon = true, MaCanChuyen = maDonGoiMon };
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? DuLieu { get; set; }

        public static ServiceResult<T> Ok(T duLieu)
        {
            return new ServiceResult<T> { ThanhCong = true, DuLieu = duLieu };
        }

        public static new ServiceResult<T> NotFound()
        {
            return new ServiceResult<T> { KhongTimThay = true };
        }

        public static new ServiceResult<T> Login()
        {
            return new ServiceResult<T> { ChuyenVeDangNhap = true };
        }

        public static new ServiceResult<T> RedirectDetails(string maDonGoiMon)
        {
            return new ServiceResult<T> { ChuyenVeChiTietDon = true, MaCanChuyen = maDonGoiMon };
        }

        public static ServiceResult<T> Loi(string? truongLoi, string thongBao, T? duLieu = default)
        {
            return new ServiceResult<T>
            {
                TruongLoi = truongLoi,
                ThongBao = thongBao,
                DuLieu = duLieu
            };
        }
    }
}
