using ApprovalService.Domain.Interfaces;
using ApprovalService.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApprovalService.Infrastructure.Services
{
    public class OllamaClassifierService : IOllamaClassifierService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OllamaClassifierService> _logger;

        public OllamaClassifierService(HttpClient httpClient, ILogger<OllamaClassifierService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ClassificationResult> ClassifyInvoiceAsync(string vendor, decimal total, string itemsDescription)
        {
            _logger.LogInformation("Starting AI classification for vendor {Vendor}.", vendor);

            var prompt = $@"
            You are an automated invoice classification system. Analyze the following invoice data and return a JSON object.
            
            Invoice Data:
            - Vendor: {vendor}
            - Total Amount: {total}
            - Items: {itemsDescription}

            You MUST respond ONLY with a valid JSON object matching this schema: {{
                ""suggestedCategory"": ""Software"" or ""Hardware"" or ""Marketing"" or ""OfficeSupplies"" or ""Other"",
                ""confidence"": 0.95,
                ""reasoning"": ""A short one-sentence explanation for your decision""
            }}
            Do not include any markdown formatting, backticks (```), or extra text. Just raw JSON.";

            var requestBody = new OllamaChatRequest
            {
                Model = "mistral", 
                Prompt = prompt,
                Stream = false,
                Format = "json"
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/generate", requestBody);
                response.EnsureSuccessStatusCode();

                var ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaChatResponse>();

                if (ollamaResponse == null || string.IsNullOrWhiteSpace(ollamaResponse.ResponseText))
                {
                    throw new Exception("Ollama returned an empty response.");
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ClassificationResult>(ollamaResponse.ResponseText, options);

                return result ?? GetDefaultFallback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call Ollama. Returning fallback classification.");
                return GetDefaultFallback();
            }
        }

        private ClassificationResult GetDefaultFallback()
        {
            return new ClassificationResult
            {
                SuggestedCategory = "Other",
                Confidence = 0.50, 
                Reasoning = "Fallback applied due to AI service unavailability.",
                DetectedViolations = new System.Collections.Generic.List<string> { "AI_SERVICE_UNAVAILABLE" }
            };
        }
    }

    public class OllamaChatRequest
    {
        [JsonPropertyName("model")] public string Model { get; set; } = string.Empty;
        [JsonPropertyName("prompt")] public string Prompt { get; set; } = string.Empty;
        [JsonPropertyName("stream")] public bool Stream { get; set; }
        [JsonPropertyName("format")] public string Format { get; set; } = string.Empty;
    }

    public class OllamaChatResponse
    {
        [JsonPropertyName("response")] public string ResponseText { get; set; } = string.Empty;
    }
}