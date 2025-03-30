using Business.Domain.Models;
using Business.Domain.Repositories;
using Business.Domain.Services;
using Business.Resources.SalaryFormulaConfig;

namespace Business.Services
{
    public class SalaryFormulaConfigService : ISalaryFormulaConfigService
    {
        private readonly ISalaryFormulaConfigRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SalaryFormulaConfigService(
            ISalaryFormulaConfigRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SalaryFormulaConfig>> GetAllByTypeAsync(string type)
        {
            return await _repository.GetAllByTypeAsync(type);
        }

        public async Task<SalaryFormulaConfig?> GetActiveByTypeAsync(string type)
        {
            return await _repository.GetActiveByTypeAsync(type);
        }

        public async Task<bool> CreateAsync(SalaryFormulaConfigCreateResource resource)
        {
            var newConfig = new SalaryFormulaConfig
            {
                Type = resource.Type,
                Expression = resource.Expression,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = resource.CreatedBy ?? "system"
            };

            await _repository.AddAsync(newConfig);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> SetActiveAsync(int id, string type)
        {
            var allConfigs = await _repository.GetAllByTypeAsync(type);
            foreach (var cfg in allConfigs)
                cfg.IsActive = (cfg.Id == id);

            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
