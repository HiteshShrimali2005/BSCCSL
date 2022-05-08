using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class Income
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IncomeId { get; set; }
        [StringLength(500)]
        [Column(TypeName = "varchar")]
        public string IncomeName { get; set; }
        public string Description { get; set; }
        public DateTime? IncomeDate { get; set; }
        public Guid AccountsHeadId { get; set; }
        public decimal Amount { get; set; }
        public Guid BranchId { get; set; }
        public bool IsDelete { get; set; }
        public bool? Status { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [ForeignKey("AccountsHeadId")]
        public AccountsHead AccountsHead { get; set; }
        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }
        public Income()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }

        public string TransactionMode { get; set; }
        public string ReferenceNumber { get; set; }

    }

}
