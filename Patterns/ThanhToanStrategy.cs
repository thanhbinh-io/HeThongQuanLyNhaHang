namespace NhaHangThanhThao.Patterns
{
    public interface IThanhToanStrategy
    {
        decimal TinhTienCocKhauTru(decimal tienCoc, decimal tongTienMon, decimal phuThu);
        decimal TinhTongThanhToan(decimal tongTienMon, decimal phuThu, decimal tienCocKhauTru);
    }

    public class ThanhToanMacDinhStrategy : IThanhToanStrategy
    {
        public decimal TinhTienCocKhauTru(decimal tienCoc, decimal tongTienMon, decimal phuThu)
        {
            return Math.Min(tienCoc, tongTienMon + phuThu);
        }

        public decimal TinhTongThanhToan(decimal tongTienMon, decimal phuThu, decimal tienCocKhauTru)
        {
            return tongTienMon + phuThu - tienCocKhauTru;
        }
    }
}
