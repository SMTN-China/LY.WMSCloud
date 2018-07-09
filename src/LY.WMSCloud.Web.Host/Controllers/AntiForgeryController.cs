using Microsoft.AspNetCore.Antiforgery;
using LY.WMSCloud.Controllers;

namespace LY.WMSCloud.Web.Host.Controllers
{
    public class AntiForgeryController : WMSCloudControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
