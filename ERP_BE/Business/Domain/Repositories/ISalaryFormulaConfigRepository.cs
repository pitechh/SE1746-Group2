using Business.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Domain.Repositories
{
    public interface ISalaryFormulaConfigRepository
    {
        Task<SalaryFormulaConfig?> GetActiveByTypeAsync(string type);
        Task<List<SalaryFormulaConfig>> GetAllByTypeAsync(string type);
        Task AddAsync(SalaryFormulaConfig config);
        Task UpdateAsync(SalaryFormulaConfig config);
    }
}
