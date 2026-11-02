using Microsoft.AspNetCore.Identity;
using NhaHangThanhThao.Models;

namespace NhaHangThanhThao.Services
{
    public interface IMatKhauService
    {
        string BamMatKhau(NguoiDung nguoiDung, string matKhau);
        bool KiemTraMatKhau(NguoiDung nguoiDung, string matKhauNhap);
        bool CanNangCapMatKhau(NguoiDung nguoiDung, string matKhauNhap);
    }

    public class MatKhauService : IMatKhauService
    {
        private readonly PasswordHasher<NguoiDung> _passwordHasher = new();

        public string BamMatKhau(NguoiDung nguoiDung, string matKhau)
        {
            return _passwordHasher.HashPassword(nguoiDung, matKhau);
        }

        public bool KiemTraMatKhau(NguoiDung nguoiDung, string matKhauNhap)
        {
            if (string.IsNullOrWhiteSpace(matKhauNhap) || string.IsNullOrEmpty(nguoiDung.MatKhau))
            {
                return false;
            }

            if (LaMatKhauIdentityHash(nguoiDung.MatKhau))
            {
                var ketQua = XacMinhHash(nguoiDung, matKhauNhap);

                return ketQua == PasswordVerificationResult.Success
                    || ketQua == PasswordVerificationResult.SuccessRehashNeeded;
            }

            return string.Equals(nguoiDung.MatKhau, matKhauNhap, StringComparison.Ordinal);
        }

        public bool CanNangCapMatKhau(NguoiDung nguoiDung, string matKhauNhap)
        {
            if (string.IsNullOrWhiteSpace(matKhauNhap) || string.IsNullOrEmpty(nguoiDung.MatKhau))
            {
                return false;
            }

            if (!LaMatKhauIdentityHash(nguoiDung.MatKhau))
            {
                return string.Equals(nguoiDung.MatKhau, matKhauNhap, StringComparison.Ordinal);
            }

            return XacMinhHash(nguoiDung, matKhauNhap) == PasswordVerificationResult.SuccessRehashNeeded;
        }

        private PasswordVerificationResult XacMinhHash(NguoiDung nguoiDung, string matKhauNhap)
        {
            try
            {
                return _passwordHasher.VerifyHashedPassword(nguoiDung, nguoiDung.MatKhau, matKhauNhap);
            }
            catch (FormatException)
            {
                return PasswordVerificationResult.Failed;
            }
        }

        private static bool LaMatKhauIdentityHash(string matKhauDangLuu)
        {
            return matKhauDangLuu.StartsWith("AQAAAA", StringComparison.Ordinal)
                || matKhauDangLuu.StartsWith("AA", StringComparison.Ordinal);
        }
    }
}
