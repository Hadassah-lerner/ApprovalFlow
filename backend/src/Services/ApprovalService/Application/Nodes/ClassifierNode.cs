using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Features.ProcessInvoice;
using ApprovalService.Application.Helpers;
using ApprovalService.Domain.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApprovalService.Application.Nodes
{
    public class ClassifierNode : IWorkflowNode
    {
        private readonly PromptBuilder _promptBuilder;
        private readonly JsonExtractor _jsonExtractor;
        private readonly HttpClient _httpClient;

        public ClassifierNode(PromptBuilder promptBuilder, JsonExtractor jsonExtractor, HttpClient httpClient)
        {
            _promptBuilder = promptBuilder;
            _jsonExtractor = jsonExtractor;
            _httpClient = httpClient;
        }

        public async Task ExecuteAsync(WorkflowContext context)
        {

            context.FinalPrompt = _promptBuilder.BuildClassificationPrompt(context.ProcessedInvoiceText, context.PolicyText);

            var requestBody = new
            {
                model = "mistral:latest", 
                prompt = context.FinalPrompt,
                stream = false
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:11434/api/generate", requestBody);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Ollama returned error {response.StatusCode}: {errorContent}");
                }

                var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (jsonResponse.TryGetProperty("response", out var responseProperty))
                {
                    context.AiRawResult = responseProperty.GetString();
                }
                else
                {
                    context.AiRawResult = jsonResponse.GetRawText();
                }

                var result = _jsonExtractor.ExtractJson<ClassificationResult>(context.AiRawResult);
                if (result != null)
                {
                    context.Category = result.SuggestedCategory;
                    context.Confidence = result.Confidence;
                    context.PolicyViolations = result.DetectedViolations ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                context.Category = "Other";
                context.Confidence = 0.5m;
                context.PolicyViolations = new List<string> { "AI_CLASSIFICATION_FAILED" };
                context.AiRawResult = "Fallback applied due to exception: " + ex.Message;
            }
        }
    }
}