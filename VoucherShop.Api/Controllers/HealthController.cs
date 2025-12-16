using Microsoft.AspNetCore.Mvc;

namespace VoucherShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
            => Ok("VoucherShop API is running");

    }
}
