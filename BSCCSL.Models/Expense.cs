using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class Expense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ExpenseId { get; set; }
        [StringLength(500)]
        [Column(TypeName = "varchar")]
        public string ExpenseName { get; set; }
        public string Description { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public Guid AccountsHeadId { get; set; }
        public decimal Amount { get; set; }
        public Guid BranchId { get; set; }
        public bool IsDelete { get; set; }
        public bool? Status { get; set; }
        public DateTime? BillDate { get; set; }
        public Guid? ApprovedBy { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public DateTime? ApprovedDate { get; set; }
        [StringLength(1000)]
        [Column(TypeName = "nvarchar")]
        public string ApproveComment { get; set; }
        [StringLength(200)]
        [Column(TypeName = "nvarchar")]
        public string FileName { get; set; }
        [StringLength(200)]
        [Column(TypeName = "nvarchar")]
        public string OrgFileName { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [ForeignKey("AccountsHeadId")]
        public AccountsHead AccountsHead { get; set; }
        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }
        public Expense()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }

        public string TransactionMode { get; set; }
        public string ReferenceNumber { get; set; }

        public bool? IsPaid { get; set; }
        public Guid? PaidTo { get; set; }

    }
}
