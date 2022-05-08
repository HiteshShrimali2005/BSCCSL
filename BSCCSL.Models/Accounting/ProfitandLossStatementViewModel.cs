using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models.Accounting
{
    public class ProfitandLossStatementViewModel
    {


        public class ProfitandLossStatementforParentExpense
        {
            public string Name { get; set; }

            public decimal Credit { get; set; }

            public decimal Debit { get; set; }

            public decimal TotalAmount { get; set; }

            public string TotalAmountstring { get; set; }

            public Guid AccountId { get; set; }

            public Guid ParentAccountId { get; set; }

            public List<ProfitandLossStatementforChildExpense> ListProfitandLossStatementforChildExpense { get; set; }

        }

        public class ProfitandLossStatementforChildExpense
        {
            public string Name { get; set; }

            public decimal Credit { get; set; }

            public decimal Debit { get; set; }

            public decimal TotalAmount { get; set; }

            public string TotalAmountstring { get; set; }

            public Guid AccountId { get; set; }

            public Guid ParentAccountId { get; set; }
        }


        public class ProfitandLossStatementforParentIncome
        {
            public string Name { get; set; }

            public decimal Credit { get; set; }

            public decimal Debit { get; set; }

            public decimal TotalAmount { get; set; }

            public string TotalAmountstring { get; set; }

            public Guid AccountId { get; set; }

            public Guid ParentAccountId { get; set; }

            public List<ProfitandLossStatementforChildIncome> ListProfitandLossStatementforChildIncome { get; set; }
        }

        public class ProfitandLossStatementforChildIncome
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
