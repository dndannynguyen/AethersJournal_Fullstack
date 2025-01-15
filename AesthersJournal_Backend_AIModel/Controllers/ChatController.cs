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
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly string API_KEY; // Remove static
        private readonly string API_URL; // Remove static
        private static readonly string InitialPrompt = @"
            You are a professional therapist engaging in a conversation with the user. Always reply with empathy, understanding, and encouragement. Keep your responses focused on the user's emotions and concerns most of the time, unless that is something casual (but must be formal), you should answer. If the user asks irrelevant or off-topic questions, politely steer the conversation back to their feelings and well-being. Avoid answering irrelevant or non-therapeutic questions. Keep responses short, supportive, and aligned with therapeutic principles.
            When you respond, just give the text in what the professional therapist would say, but keep the friendly vibe, like you are a human.
            Context: You are on a phone call with the client.
            Never put 'AI:' before your response.
        ";

        // Variable to store conversation history
        private static StringBuilder conversationHistory = new StringBuilder(InitialPrompt);

        public ChatController(IConfiguration configuration, ILogger<ChatController> logger)
        {
            API_KEY = configuration["API_KEY"]; // Load API_KEY from environment variable
            API_URL = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={API_KEY}"; // Set API_URL
            _logger = logger;
        }

        [HttpPost("send-message")]
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

    public class UserMessage
    {
        public required string Message { get; set; }
    }
}
