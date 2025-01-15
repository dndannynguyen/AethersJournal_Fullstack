using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // Add this line
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace GeminiAIChatTherapist.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SummaryController : ControllerBase
    {
        private readonly ILogger<SummaryController> _logger;
        private readonly string API_KEY; // Remove static
        private readonly string API_URL; // Remove static
        private static readonly string InitialPrompt = @"
            I am going to give you a journal entry of a person. You have to summarize the journal entry in a few sentences. You can use your own words to summarize the journal entry. Help analyze and point out the emotional aspects of the journal entry, as well as give me key actions, activities in the summary. But the main point is to summarize the journal entry in a professional way, so later, the therapist can read the summary, but still understand what is going on with the person. Here is the journal entry:
        ";

        // Variable to store conversation history
        private static StringBuilder conversationHistory = new StringBuilder(InitialPrompt);

        public SummaryController(IConfiguration configuration, ILogger<SummaryController> logger)
        {
            API_KEY = configuration["API_KEY"]; // Load API_KEY from environment variable
            API_URL = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={API_KEY}"; // Set API_URL
            _logger = logger;
        }

        [HttpPost("send-summary")]
        public async Task<IActionResult> SendMessage([FromBody] UserMessage userMessage)
        {
            if (userMessage == null || string.IsNullOrEmpty(userMessage.Message))
            {
                return BadRequest("Invalid message.");
            }

            // Add user input to conversation history
            conversationHistory.AppendLine($"You: {userMessage.Message}");

            // Generate response from Gemini AI
            var aiResponse = await GenerateAIResponse(conversationHistory.ToString());

            // Add AI response to conversation history
            conversationHistory.AppendLine($"Therapist: {aiResponse}");

            return Ok(new { Response = aiResponse });
        }

        private async Task<string> GenerateAIResponse(string conversationHistory)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var requestBody = new
                    {
                        contents = new[]
                        {
                            new
                            {
                                parts = new[]
                                {
                                    new { text = conversationHistory }
                                }
                            }
                        }
                    };

                    var jsonContent = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(API_URL, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        return $"Error: {response.StatusCode}, {errorContent}";
                    }

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(jsonResponse);

                    string? aiResponse = json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString()?.Trim();
                    return aiResponse ?? "Sorry, I couldn't generate a response.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while generating AI response");
                return $"Exception occurred: {ex.Message}";
            }
        }
    }

    public class UserJournalEntry
    {
        public required string JournalEntry { get; set; }
    }
}
