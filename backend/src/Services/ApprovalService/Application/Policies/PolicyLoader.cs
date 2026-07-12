using System;
using System.IO;
using System.Text.RegularExpressions;
using ApprovalService.Application.Common.Abstractions;
using Microsoft.Extensions.Logging;

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
                return File.ReadAllText(_policyFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read policy file.");
                throw;
            }
        }

        public decimal GetDecimalThreshold(string key, decimal defaultValue)
        {
            try
            {
                string policyContent = LoadPolicy();

                var match = Regex.Match(policyContent, $@"\|?\s*`{key}`\s*\|\s*\*?\$?([0-9.,]+)\*?\s*\|");

                if (match.Success && decimal.TryParse(match.Groups[1].Value, out decimal result))
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to parse dynamic threshold for {key}. Using default: {defaultValue}");
            }
            return defaultValue;
        }

        public decimal GetSaaSLimit(decimal defaultValue)
        {
            try
            {
                string policyContent = LoadPolicy();
                var match = Regex.Match(policyContent, @"\|?\s*`SAAS-01`\s*\|.*?up to\s*\*?\$?([0-9.,]+)");
                if (match.Success && decimal.TryParse(match.Groups[1].Value, out decimal result))
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to parse SAAS limit. Using default: {defaultValue}");
            }
            return defaultValue;
        }
    }
}