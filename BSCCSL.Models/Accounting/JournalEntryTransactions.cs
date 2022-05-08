using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models.Accounting
{
    public class JournalEntryTransactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid JournalEntryId { get; set; }

        [ForeignKey("JournalEntryId")]
        public JournalEntry JournalEntry { get; set; }

        public Guid AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Accounts Accounts { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? VendorId { get; set; }

        public Guid? EmployeeId { get; set; }

        public decimal Credit { get; set; }

        public decimal Debit { get; set; }
    }
}
