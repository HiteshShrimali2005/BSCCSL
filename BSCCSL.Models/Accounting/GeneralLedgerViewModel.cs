using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models.Accounting
{
    public class GeneralLedgerViewModel
    {
        public string BranchName { get; set; }

        public string VoucherNumber { get; set; }

        public string PostingDate { get; set; }

        public string ReferenceDate { get; set; }

        public string ReferenceNo { get; set; }

        public decimal Credit { get; set; }

        public decimal Debit { get; set; }

        public decimal Balance { get; set; }

        public string AccountName { get; set; }

        public EntryType EntryType { get; set; }

        public string EntryTypeName
        {
            get
            {
                return EntryType.ToString().Replace('_', ' ');
            }
        }

        public string Description { get; set; }

    }
}
