using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSCCSL.Models
{
    public class Loan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LoanId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CustomerProductId { get; set; }
        public Guid LoanType { get; set; }
        public Guid? ReasonForUse { get; set; }
        public Guid? CustomerType { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }
        public LoanStatus LoanStatus { get; set; }
        public DateTime? DateofApplication { get; set; }
        [StringLength(10)]
        [Column(TypeName = "varchar")]
        public string PresentcustomerBank { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Place { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal LoanIntrestRate { get; set; }
        //public double ProcessingCharge { get; set; }
        //public double ServiceTax { get; set; }
        public string Term { get; set; }
        public decimal? DisbursementAmount { get; set; }
        public decimal? MonthlyInstallmentAmount { get; set; }
        public decimal? TotalCharges { get; set; }
        public DateTime? InstallmentDate { get; set; }
        public decimal? TotalDisbursementAmount { get; set; }
        public DisburseThrough? DisburseThrough { get; set; }
        public long ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string BankName { get; set; }
        public Guid? DisbursementBy { get; set; }
        public bool IsDisbursed { get; set; }
        public Guid? GroupLoanId { get; set; }
        public decimal? LastPrincipalAmount { get; set; }
        public int? LastTenure { get; set; }
        public DateTime? LastInstallmentDate { get; set; }
        public decimal? TotalAmountToPay { get; set; }

        public string ConsumerProductName { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("CustomerProductId")]
        public virtual CustomerProduct CustomerProduct { get; set; }

        public Loan()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }

    public class UpdateLoanStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UpdateLoanStatusId { get; set; }
        public Guid LoanId { get; set; }
        public LoanStatus LoanStatus { get; set; }
        public string Comment { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }

        public virtual string LoanStatusName
        {
            get
            {
                return LoanStatus.ToString().Replace("_", "-");
            }
        }

        [NotMapped]
        public string UpdatedByName { get; set; }

        public UpdateLoanStatus()
        {
            UpdatedDate = DateTime.Now;
        }
    }

    public class LoanDocuments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LoanDocumentId { get; set; }
        public Guid LoanId { get; set; }
        public string DocumentName { get; set; }
        public string Path { get; set; }
        public bool IsDelete { get; set; }
        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }
    }

    public class LoanCharges
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LoanChargesId { get; set; }
        public Guid LoanId { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }
        public decimal? Value { get; set; }
        public decimal? NoOfItem { get; set; }
        public bool IsDelete { get; set; }
        public Guid? ChargesId { get; set; }
        public string CertificateNo { get; set; }
        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }

    }

    public class ReferencerDetail
    {
        public string ClientId { get; set; }
        public string CustomerName { get; set; }
        public string Sex { get; set; }
        public Guid? PersonalId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? BorrowerId { get; set; }
        public Guid? ReferenceId { get; set; }
        public Referencertype? Referencertype { get; set; }
        public virtual string ReferencertypeName
        {
            get
            {
                return Referencertype.ToString().Replace("_", "-");
            }
        }
    }

    public class ExpensesofEducation
    {
        public decimal? TutionFees { get; set; }
        public decimal? ExamFees { get; set; }
        public decimal? BookFees { get; set; }
        public decimal? Rent { get; set; }
        public decimal? Board { get; set; }
        public decimal? Clothe { get; set; }
        public decimal? Casual { get; set; }
        public decimal? InsurancePremium { get; set; }
        public Guid? LookupCategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid? LookupId { get; set; }
        public string Name { get; set; }

    }

    public enum LoanStatus
    {
        Draft = 1,
        Submitted = 2,
        InProcess = 3,
        Approved = 4,
        Rejected = 5,
        Cancelled = 6,
        Disbursed = 7,
        Completed = 8
    }

    public enum DisburseThrough
    {
        Saving_Account = 1,
        Cheque = 2,
    }

    public class ListLoanDetails
    {
        public string AccountNumber { get; set; }
        public int ProductType { get; set; }
        public decimal InterestRate { get; set; }
        public Guid LoanId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CustomerProductId { get; set; }
        public string LoanTypeName { get; set; }
        public string ProductName { get; set; }
        public decimal LoanAmount { get; set; }
        public Guid? GroupLoanId { get; set; }
        public string GroupName { get; set; }
        public string GroupLoanNumber { get; set; }
        public int NoOfCustomer { get; set; }
        public LoanStatus LoanStatus { get; set; }
        public string CustomerName { get; set; }
        public virtual string LoanStatusName
        {
            get
            {
                return LoanStatus.ToString().Replace("_", "-");
            }
        }
    }

    public class LoanStatusData
    {
        public Loan Loan { get; set; }
        public UpdateLoanStatus LoanStatus { get; set; }
        public List<LoanCharges> LoanCharges { get; set; }
    }

    public class MemberData
    {
        public Guid LoanId { get; set; }
        public Guid CustomerProductId { get; set; }
    }

    public class LoanAmountisation
    {
        public decimal PrincipalAmount { get; set; }
        public decimal LoanIntrestRate { get; set; }
        public int Term { get; set; }
        public Guid? LoanId { get; set; }
        public DateTime InstallmentDate { get; set; }
        public Guid? CustomerProductId { get; set; }
        public LoanStatus? LoanStatus { get; set; }
        public decimal TotalPendingInstallmentAmount { get; set; }
        public bool IsPrePayment { get; set; }
    }

    public class ListLoanAmountisation
    {
        public decimal MonthlyEMI { get; set; }
        public decimal PrincipalAmt { get; set; }
        public decimal Interest { get; set; }
        public DateTime Installmentdate { get; set; }
        public bool? IsPaid { get; set; }

        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }

    }

    public class AllLoanCharges
    {
        public List<Guid> LoanIds { get; set; }
        public List<LoanCharges> Charges { get; set; }
    }

    public class Amountisation
    {
        public List<ListLoanAmountisation> ListLoanAmountisation { get; set; }
        public decimal MonthlyInstallmentAmount { get; set; }
    }

}
