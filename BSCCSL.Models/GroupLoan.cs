using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class GroupLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid GroupLoanId { get; set; }
        public string GroupLoanNumber { get; set; }
        public Guid ProductId { get; set; }
        public DateTime? DateofApplication { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string IDNO { get; set; }
        [StringLength(300)]
        [Column(TypeName = "varchar")]
        public string GroupName { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string DistanceFromBranch { get; set; }
        public decimal InterestRate { get; set; }
        public decimal GroupLoanAmount { get; set; }
        public string CreditCheque { get; set; }
        public DateTime? DateOfCredit { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime? InstallmentDate { get; set; }
        public Guid? InstallmentDuration { get; set; }
        public Guid? NeedOfLoan { get; set; }
        public bool PreviouslyBorrowed { get; set; }
        public double? PreviousLoanAmount { get; set; }
        public DateTime? PreviousLoanCompleted { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? AgentId { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public Guid? BranchId { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        public GroupLoan()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }

    public class GroupLoanData
    {
        public GroupLoan GroupLoan { get; set; }
        public List<Loan> Loan { get; set; }
        public List<Borrower> Borrower { get; set; }
    }

    public class GroupLoanDetails
    {
        public Guid GroupLoanId { get; set; }
        public string GroupLoanNumber { get; set; }
        public Guid ProductId { get; set; }
        public DateTime? DateofApplication { get; set; }
        public string IDNO { get; set; }
        public string GroupName { get; set; }
        public string DistanceFromBranch { get; set; }
        public decimal InterestRate { get; set; }
        public decimal GroupLoanAmount { get; set; }
        public string CreditCheque { get; set; }
        public DateTime? DateOfCredit { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime? InstallmentDate { get; set; }
        public Guid? InstallmentDuration { get; set; }
        public Guid? NeedOfLoan { get; set; }
        public bool PreviouslyBorrowed { get; set; }
        public double? PreviousLoanAmount { get; set; }
        public DateTime? PreviousLoanCompleted { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? AgentId { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }
        public Guid LoanId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CustomerProductId { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal LoanIntrestRate { get; set; }
        public double ProcessingCharge { get; set; }
        public double ServiceTax { get; set; }
        public Guid BorrowerId { get; set; }
        public Guid PersonalDetailId { get; set; }
        public string AccountNumber { get; set; }
        public int LoanStatus { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ClienId { get; set; }
        public string AgentName { get; set; }
        public string AgentCode { get; set; }
        public string EmpName { get; set; }
        public string EmpCode { get; set; }
    }

    public class ApproveGroupLoan
    {
        public List<Guid> LoanId { get; set; }
        public string Comment { get; set; }
    }

}
