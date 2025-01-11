using System.Net.Http;
using System.Threading.Tasks;
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
        public IActionResult HelloWorld()
        {
            return Ok("Hello World");
        }

        [HttpGet("joke")]
        public async Task<IActionResult> GetJoke()
        {
            // Replace with the actual API endpoint for jokes
            var jokeApiUrl = "https://official-joke-api.appspot.com/random_joke";
            var response = await _httpClient.GetAsync(jokeApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var joke = await response.Content.ReadAsStringAsync();
                return Ok(joke);
            }

            return StatusCode((int)response.StatusCode, "Failed to fetch joke");
        }
    }
}
