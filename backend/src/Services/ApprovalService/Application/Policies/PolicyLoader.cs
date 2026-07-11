using ApprovalService.Application.Common.Abstractions;

namespace ApprovalService.Infrastructure.Services
{
    public class PolicyLoader : IPolicyLoader
    {
        private readonly string _policyFilePath;
        private readonly ILogger<PolicyLoader> _logger;

        public PolicyLoader(ILogger<PolicyLoader> logger)
        {
            _logger = logger;
            _policyFilePath = Path.Combine(AppContext.BaseDirectory, "Policies", "policy.md");
        }

        public string LoadPolicy()
        {
            try
            {
                if (!File.Exists(_policyFilePath))
                {
                    _logger.LogError($"Policy file not found at path: {_policyFilePath}");
                    throw new FileNotFoundException($"Critical Error: policy.md is missing from {_policyFilePath}");
                }

                _logger.LogInformation("Successfully loading policy.md from disk.");
                return File.ReadAllText(_policyFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read policy file.");
                throw;
            }
        }
    }
}
