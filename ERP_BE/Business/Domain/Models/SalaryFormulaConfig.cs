using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Domain.Models
{
    public class SalaryFormulaConfig
    {
        public int Id { get; set; }
        public string Type { get; set; }           // e.g. "PIT", "SocialInsurance"
        public string Expression { get; set; }     // e.g. "gross * 0.105"
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

}
