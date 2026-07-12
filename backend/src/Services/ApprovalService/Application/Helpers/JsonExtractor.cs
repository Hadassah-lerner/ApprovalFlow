using System.Text.Json;
using System.Text.RegularExpressions;

namespace ApprovalService.Application.Helpers
{
    public class JsonExtractor
    {
        private readonly ILogger<JsonExtractor> _logger;

        public JsonExtractor(ILogger<JsonExtractor> logger)
        {
            _logger = logger;
        }

        public T ExtractJson<T>(string rawAiOutput) where T : class
        {
            if (string.IsNullOrWhiteSpace(rawAiOutput))
            {
                _logger.LogWarning("Raw AI output is empty. Cannot extract JSON.");
                return null;
            }

            try
            {
                var match = Regex.Match(rawAiOutput, @"\{.*\}", RegexOptions.Singleline);

                if (!match.Success)
                {
                    _logger.LogWarning("No valid JSON block found in raw AI output. Trying direct parsing...");
                    return JsonSerializer.Deserialize<T>(rawAiOutput, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                string cleanJson = match.Value;

                var result = JsonSerializer.Deserialize<T>(cleanJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to parse or extract JSON. Raw input was: {rawAiOutput}");
                return null;
            }
        }
    }
}
