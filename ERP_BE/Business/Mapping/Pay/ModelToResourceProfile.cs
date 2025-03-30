using AutoMapper;
using Business.Resources.Pay;

namespace Business.Mapping.Pay
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<Domain.Models.Pay, PayResource>()
                .ForMember(dest => dest.Gross, opt => opt.MapFrom(src => CalculateGross(src)))
                .ForMember(dest => dest.NET, opt => opt.MapFrom(src => CalculateNet(src)));
        }

        private static decimal CalculateGross(Domain.Models.Pay src)
        {
            if (src.TotalWorkDay == 0) return 0;
            var grossWithoutBonus = (decimal)src.WorkDay * src.BaseSalary / (decimal)src.TotalWorkDay;
            var gross = grossWithoutBonus + src.Allowance + src.Bonus;
            return Math.Round(gross, 3);
        }

        private static decimal CalculateNet(Domain.Models.Pay src)
        {
            var gross = CalculateGross(src);
            var totalDeductions = (decimal)src.PIT + (decimal)src.SocialInsurance + (decimal)src.HealthInsurance;
            var net = gross - totalDeductions;
            return Math.Round(net, 3);
        }
    }
}
