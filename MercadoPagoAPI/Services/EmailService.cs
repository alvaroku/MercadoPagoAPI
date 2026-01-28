using Microsoft.Extensions.Options;

namespace MercadoPagoAPI.Services
{
    public class EmailSettings
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
    }

    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/email/4/messages");

                // Usamos el ApiKey del setting
                request.Headers.Add("Authorization", $"App {_settings.ApiKey}");
                request.Headers.Add("Accept", "application/json");

                // Construcción del string con interpolación ($)
                // Nota: Las llaves del JSON se deben duplicar {{ }} para que C# no las confunda con variables
                var jsonString = $@"{{
            ""messages"": [
                {{
                    ""destinations"": [
                        {{
                            ""to"": [
                                {{
                                    ""destination"": ""{toEmail}""
                                }}
                            ]
                        }}
                    ],
                   ""sender"": ""{_settings.FromName} <{_settings.From}>"",
                    ""content"": {{
                        ""subject"": ""{subject}"",
                        ""text"": ""{body}""
                    }}
                }}
            ]
        }}";

                var content = new StringContent(jsonString, null, "application/json");
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
