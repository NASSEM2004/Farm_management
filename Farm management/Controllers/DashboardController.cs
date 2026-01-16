using Microsoft.AspNetCore.Mvc;

namespace Farm_management.Controllers // تأكد أن Farm_management هو اسم مشروعك
{
    public class DashboardController : Controller
    {
        // هذه الدالة وظيفتها فقط عرض الصفحة (View)
        public IActionResult Index()
        {
            return View();
        }
    }
}