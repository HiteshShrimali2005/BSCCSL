using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models.Accounting
{
    public class CashFlowViewModel
    {
        public string PostingDate { get; set; }

        public string BranchName { get; set; }

        public string Particular { get; set; }

        public string Description { get; set; }

        public decimal Credit { get; set; }

        public decimal Debit { get; set; }

        public decimal Balance { get; set; }

        public EntryType EntryType { get; set; }

        public string VoucherNumber { get; set; }

        public string EntryTypeName
        {
            get
            {
                return EntryType.ToString().Replace('_', ' ');
            }
        }
    }
}
