namespace NhaHangThanhThao.Patterns
{
    public interface IMaChungTuFactory
    {
        string TaoMaDonDatBan();
        string TaoMaDonGoiMon();
        string TaoMaHoaDon();
    }

    public class MaChungTuFactory : IMaChungTuFactory
    {
        public string TaoMaDonDatBan()
        {
            return "DDB" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public string TaoMaDonGoiMon()
        {
            return "DGM" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public string TaoMaHoaDon()
        {
            return "HD" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
    }
}
