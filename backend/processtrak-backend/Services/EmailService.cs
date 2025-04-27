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
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<EmailService> logger
        )
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.mailersend.com/v1/email"
                );

                // Validate configuration values
                var fromEmail = $"noreply@{_configuration["MAILER_SEND_DOMAIN"]}";
                var apiKey = _configuration["MAILER_SEND_API"];

                if (string.IsNullOrEmpty(fromEmail))
                    throw new ArgumentNullException("MAILER_SEND_DOMAIN configuration is missing");
                if (string.IsNullOrEmpty(apiKey))
                    throw new ArgumentNullException("MAILER_SEND_API configuration is missing");
                if (string.IsNullOrEmpty(toEmail))
                    throw new ArgumentException("Recipient email cannot be null or empty");

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

                // Log the response for debugging
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError(
                        $"Email sending failed. Status: {response.StatusCode}. Response: {responseContent}"
                    );
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                return false;
            }
        }
    }
}
