using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models.Accounting
{
    public class PandLClosingYearModel
    {
        public bool isLoss { get; set; }

        public bool isProfit { get; set; }

        public string FinancialYear { get; set; }

        public Guid BranchId { get; set;}

        public string FinalExpenseTotalAmount { get; set; }

        public string FinalIncomeTotalAmount { get; set; }

    }
}
