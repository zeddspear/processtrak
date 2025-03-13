using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace processtrak_backend.Services
{
    public class EmailService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public EmailService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.mailersend.com/v1/email"
            );

            var fromEmail = $"noreply@{_configuration["MAILER_SEND_DOMAIN"]}"; // Using MAILER_SEND_DOMAIN
            var apiKey = _configuration["MAILER_SEND_API"];

            var payload = new
            {
                from = new { email = fromEmail },
                to = new[] { new { email = toEmail } },
                subject,
                text = body,
                html = body,
            };

            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
