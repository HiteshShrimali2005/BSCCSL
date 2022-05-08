using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class BorrowerLoanDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BorrowerLoanId { get; set; }
        public Guid BorrowerId { get; set; }
        public Guid LoanId { get; set; }
        public Guid LoanType { get; set; }
        [StringLength(250)]
        [Column(TypeName = "varchar")]
        public string OrganizationName { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal LoanLimit { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal PaidInstallment { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }
        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }

        [ForeignKey("BorrowerId")]
        public virtual Borrower Borrower { get; set; }

        public BorrowerLoanDetails()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }
}
