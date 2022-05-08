using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models.Accounting
{
    public class BalanceSheetViewModel
    {
        public class BalanceSheetforParentLiabilities
        {
            public string Name { get; set; }

            public decimal Credit { get; set; }

            public decimal Debit { get; set; }

            public decimal TotalAmount { get; set; }

            public string TotalAmountstring { get; set; }

            public Guid AccountId { get; set; }

            public Guid ParentAccountId { get; set; }

            public List<BalanceSheetforChildLiabilities> ListBalanceSheetforChildLiabilities { get; set; }

        }

        public class BalanceSheetforChildLiabilities
        {
            public string Name { get; set; }

            public decimal Credit { get; set; }

            public decimal Debit { get; set; }

            public decimal TotalAmount { get; set; }

            public string TotalAmountstring { get; set; }

            public Guid AccountId { get; set; }

            public Guid ParentAccountId { get; set; }
        }


        public class BalanceSheetforParentAssests
        {
            public string Name { get; set; }

            public decimal Credit { get; set; }

            public decimal Debit { get; set; }

            public decimal TotalAmount { get; set; }

            public string TotalAmountstring { get; set; }

            public Guid AccountId { get; set; }

            public Guid ParentAccountId { get; set; }

            public List<BalanceSheetforChildAssests> ListBalanceSheetforChildAssests { get; set; }
        }

        public class BalanceSheetforChildAssests
        {
            public string Name { get; set; }

            public decimal Credit { get; set; }

            public decimal Debit { get; set; }

            public decimal TotalAmount { get; set; }

            public string TotalAmountstring { get; set; }

            public Guid AccountId { get; set; }

            public Guid ParentAccountId { get; set; }
        }


    }
}
