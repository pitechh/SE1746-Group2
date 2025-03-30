using Business.Domain.Models;
using Business.Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SalaryFormulaConfigRepository : BaseRepository<SalaryFormulaConfig>, ISalaryFormulaConfigRepository
    {
        private readonly AppDbContext _context;

        public SalaryFormulaConfigRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<SalaryFormulaConfig?> GetActiveByTypeAsync(string type)
        {
            return await _context.SalaryFormulaConfigs
                .Where(x => x.Type == type && x.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<List<SalaryFormulaConfig>> GetAllByTypeAsync(string type)
        {
            return await _context.SalaryFormulaConfigs
                .Where(x => x.Type == type)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(SalaryFormulaConfig config)
        {
            await _context.SalaryFormulaConfigs.AddAsync(config);
        }

        public async Task UpdateAsync(SalaryFormulaConfig config)
        {
            _context.SalaryFormulaConfigs.Update(config);
            await Task.CompletedTask;
        }
    }
}
