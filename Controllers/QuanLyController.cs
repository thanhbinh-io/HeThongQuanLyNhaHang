using Microsoft.AspNetCore.Mvc;

namespace NhaHangThanhThao.Controllers
{
    public class QuanLyController : Controller
    {
        public IActionResult Index()
        {
            var maVaiTro = HttpContext.Session.GetString("MaVaiTro");

            if (maVaiTro != "QL")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}