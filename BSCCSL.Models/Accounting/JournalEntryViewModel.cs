using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models.Accounting
{
    public class JournalEntryViewModel
    {
        public Guid Id { get; set; }

        public EntryType EntryType { get; set; }

        public DateTime PostingDate { get; set; }

        public List<EntryList> EntryList { get; set; }

        public Guid BranchId { get; set; }

        public string ReferenceNo { get; set; }

        public int VoucherNo { get; set; }

        public string Description { get; set; }

        public DateTime? ReferenceDate { get; set; }

        public string Prefix { get; set; }

        public virtual string EntryTypeName
        {
            get
            {
                return EntryType.ToString().Replace("_", " ");
            }
        }

        public virtual string VoucherNoString
        {
            get
            {
                return Prefix + " " + VoucherNo.ToString();
            }
        }


    }

    public class AccountList
    {
        public Guid AccountId { get; set; }
        public string Name { get; set; }
    }

    public enum EntryType
    {
        Journal_Entry = 1,
        Cash_Entry = 2,
        Bank_Entry = 3
    }

    public class EntryList
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Name { get; set; }
    }




}
