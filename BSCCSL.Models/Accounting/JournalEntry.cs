using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models.Accounting
{
    public class JournalEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public EntryType EntryType { get; set; }

        public DateTime PostingDate { get; set; }

        public Guid BranchId { get; set; }

        public string ReferenceNo { get; set; }

        public int VoucherNo { get; set; }

        public string Prefix { get; set; }

        public string Description { get; set; }

        public DateTime? ReferenceDate { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        public bool IsDelete { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

    }
}
