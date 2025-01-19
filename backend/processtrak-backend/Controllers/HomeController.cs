using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace processtrak_backend.Controllers
{
    [Route("api/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet("helloworld")]
        [AllowAnonymous]
        public IActionResult HelloWorld()
        {
            return Ok("Hello World");
        }
    }
}
