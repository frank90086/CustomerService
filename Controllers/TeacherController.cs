using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Omi.Education.Web.Management.Controllers
{
    public class TeacherController : Controller
    {
        //private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public TeacherController(

            //IEmailSender emailSender,
            ILogger<TeacherController> logger)
        {
            //_emailSender = emailSender;
            _logger = logger;
        }

        public IActionResult DefaultWorktime()
        {
            return View();
        }
    }
}