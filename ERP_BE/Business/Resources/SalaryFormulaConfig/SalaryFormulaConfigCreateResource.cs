using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Resources.SalaryFormulaConfig
{
    public class SalaryFormulaConfigCreateResource
    {
        public string Type { get; set; } = default!;
        public string Expression { get; set; } = default!;
        public string? CreatedBy { get; set; }
    }
}
