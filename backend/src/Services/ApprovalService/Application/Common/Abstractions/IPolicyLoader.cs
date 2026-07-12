namespace ApprovalService.Application.Common.Abstractions
{
    public interface IPolicyLoader
    {
        string LoadPolicy();
        decimal GetDecimalThreshold(string key, decimal defaultValue); 
        decimal GetSaaSLimit(decimal defaultValue);
    }
}
