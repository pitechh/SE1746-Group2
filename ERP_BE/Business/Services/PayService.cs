using AutoMapper;
using Business.Communication;
using Business.CustomException;
using Business.Domain.Models;
using Business.Domain.Repositories;
using Business.Domain.Services;
using Business.Resources;
using Business.Resources.Pay;
using Microsoft.Extensions.Options;

namespace Business.Services
{
    public class PayService : BaseService<PayResource, CreatePayResource, UpdatePayResource, Pay>, IPayService
    {
        private readonly IPayRepository _payRepository;
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly ISalaryFormulaConfigRepository _formulaRepository;

        public PayService(
            IPayRepository payRepository,
            ITimesheetRepository timesheetRepository,
            ISalaryFormulaConfigRepository formulaRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IOptionsMonitor<ResponseMessage> responseMessage
        ) : base(payRepository, mapper, unitOfWork, responseMessage)
        {
            _payRepository = payRepository;
            _timesheetRepository = timesheetRepository;
            _formulaRepository = formulaRepository;
        }

        public override async Task<BaseResponse<PayResource>> InsertAsync(CreatePayResource createPayResource)
        {
            try
            {
                var workDayResource = await _timesheetRepository.GetTotalWorkDayAsync(createPayResource.PersonId, createPayResource.Date);
                if (workDayResource is null)
                    return new BaseResponse<PayResource>(ResponseMessage.Values["Timesheet_NoData"]);

                var pay = Mapper.Map<CreatePayResource, Pay>(createPayResource);
                pay.WorkDay = workDayResource.WorkDay;
                pay.TotalWorkDay = workDayResource.TotalWorkDay;

                decimal grossWithoutBonus = (decimal)pay.WorkDay * pay.BaseSalary / (decimal)pay.TotalWorkDay;
                decimal gross = grossWithoutBonus + pay.Allowance + pay.Bonus;

                var pitFormula = await _formulaRepository.GetActiveByTypeAsync("PIT");
                var siFormula = await _formulaRepository.GetActiveByTypeAsync("SocialInsurance");

                decimal pitAmount = 0;
                decimal siAmount = 0;
                decimal hiAmount = Math.Round(pay.BaseSalary * 0.01m, 3); // BHYT 1%

                if (!string.IsNullOrWhiteSpace(pitFormula?.Expression))
                {
                    var pitParser = new NCalc.Expression(pitFormula.Expression);
                    pitParser.Parameters["baseSalary"] = pay.BaseSalary;
                    pitParser.Parameters["allowance"] = pay.Allowance;
                    pitParser.Parameters["bonus"] = pay.Bonus;
                    pitParser.Parameters["gross"] = gross;
                    pitAmount = Math.Round(Convert.ToDecimal(pitParser.Evaluate()), 3);
                }

                if (!string.IsNullOrWhiteSpace(siFormula?.Expression))
                {
                    var siParser = new NCalc.Expression(siFormula.Expression);
                    siParser.Parameters["baseSalary"] = pay.BaseSalary;
                    siParser.Parameters["allowance"] = pay.Allowance;
                    siParser.Parameters["bonus"] = pay.Bonus;
                    siParser.Parameters["gross"] = gross;
                    siAmount = Math.Round(Convert.ToDecimal(siParser.Evaluate()), 3);
                }

                pay.PIT = (float)pitAmount;
                pay.SocialInsurance = (float)siAmount;
                pay.HealthInsurance = (float)hiAmount;

                await _payRepository.InsertAsync(pay);
                await UnitOfWork.CompleteAsync();

                var payResource = Mapper.Map<Pay, PayResource>(pay);
                payResource.Gross = Math.Round(gross, 3);
                payResource.NET = Math.Round(gross - pitAmount - siAmount - hiAmount, 3);

                // ✅ Lấy phần trăm từ biểu thức gốc (nếu có)
                payResource.PITPercent = ExtractPercent(pitFormula?.Expression);
                payResource.SocialInsurancePercent = ExtractPercent(siFormula?.Expression);
                payResource.HealthInsurancePercent = 1.0f; // cố định

                payResource.PIT = pitAmount;
                payResource.SocialInsurance = siAmount;
                payResource.HealthInsurance = hiAmount;

                return new BaseResponse<PayResource>(payResource);
            }
            catch (Exception ex)
            {
                throw new MessageResultException(ResponseMessage.Values["Pay_Saving_Error"], ex);
            }
        }


        private float ExtractPercent(string? expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return 0f;

            try
            {
                // Regex đơn giản để lấy phần trăm như: gross * 0.05 => 5%
                var match = System.Text.RegularExpressions.Regex.Match(expression, @"\* *(\d+(\.\d+)?)");
                if (match.Success && float.TryParse(match.Groups[1].Value, out float result))
                {
                    return result * 100;
                }
            }
            catch
            {
                // Bỏ qua nếu không parse được
            }

            return 0f;
        }


    }

}
