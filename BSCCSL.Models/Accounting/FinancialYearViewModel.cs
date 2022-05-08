using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSCCSL.Models.Accounting.TrialBalanceViewModel;

namespace BSCCSL.Models.Accounting
{
    public class FinancialYearViewModel
    {
        public string FinancialYear { get; set; }

        public Guid BranchId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public List<ParentAccountTrialBalanceViewModel> ParentAccountTrialBalanceViewModel { get; set; }
    }
}
