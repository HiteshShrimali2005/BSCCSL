using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models.Accounting
{
    public class FinancialYearClosingBalance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public Guid BranchId { get; set; }

        public string FinancialYear { get; set; }

        public decimal ClosingBalanceDR { get; set; }

        public decimal ClosingBalanceCR { get; set; }

        [ForeignKey("AccountId")]
        public Accounts Accounts { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        public bool IsDelete { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

    }
}
