using Business.Domain.Models;
using Business.Resources.SalaryFormulaConfig;

namespace Business.Domain.Services
{
    public interface ISalaryFormulaConfigService
    {
        Task<List<SalaryFormulaConfig>> GetAllByTypeAsync(string type);
        Task<SalaryFormulaConfig?> GetActiveByTypeAsync(string type);
        Task<bool> CreateAsync(SalaryFormulaConfigCreateResource resource);
        Task<bool> SetActiveAsync(int id, string type);
    }       
}
