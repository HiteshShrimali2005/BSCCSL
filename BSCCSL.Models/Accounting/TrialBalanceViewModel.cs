using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models.Accounting
{
    public class TrialBalanceViewModel
    {
        public class ParentAccountTrialBalanceViewModel
        {
            public decimal OpeningDR { get; set; }

            public decimal OpeningCR { get; set; }

            public Guid AccountId { get; set; }

            public string Name { get; set; }

            public decimal Credit { get; set; }

            public decimal Debit { get; set; }

            public decimal ClosingDR { get; set; }

            public decimal ClosingCR { get; set; }


        }

        public class SubAccountTrialBalanceViewModel
        {
            public decimal OpeningDR { get; set; }

            public decimal OpeningCR { get; set; }

            public Guid AccountId { get; set; }

            public Guid SubAccountId { get; set; }

            public string Name { get; set; }

            public decimal Credit { get; set; }

            public decimal Debit { get; set; }

            public decimal Balance { get; set; }

            public decimal ClosingDR { get; set; }

            public decimal ClosingCR { get; set; }


        }

        public class ChildAccountTrialBalanceViewModel
        {

            public Guid ChildAccountId { get; set; }

            public decimal OpeningDR { get; set; }

            public decimal OpeningCR { get; set; }

            public Guid AccountId { get; set; }

            public string Name { get; set; }

            public decimal Credit { get; set; }

            public decimal Debit { get; set; }

            public decimal Balance { get; set; }

            public decimal ClosingDR { get; set; }

            public decimal ClosingCR { get; set; }


        }


    }


    public class AccountDetails
    {
        public Guid AccountId { get; set; }

        public Guid BranchId { get; set; }

        public DateTime? fromDate { get; set; }

        public DateTime? toDate { get; set; }

    }
}
