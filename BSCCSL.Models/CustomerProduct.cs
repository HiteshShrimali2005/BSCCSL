using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSCCSL.Models
{
    public class CustomerProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CustomerProductId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? AgentId { get; set; }
        public string AccountNumber { get; set; }
        public ProductType ProductType { get; set; }
        public Guid? LoanTypeId { get; set; }
        public decimal InterestRate { get; set; }
        // public Interest InterestType { get; set; }
        // public Frequency FrequencyType { get; set; }
        public Frequency PaymentType { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        public DateTime OpeningDate { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public Guid? BranchId { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        public decimal? LatePaymentFees { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifyBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public decimal? Balance { get; set; }
        public bool? InsuranceTypeLI { get; set; }
        public bool? InsuranceTypeGI { get; set; }
        public LITYPE LIType { get; set; }
        public decimal? GIPremium { get; set; }
        public DateTime? GICommencementDate { get; set; }
        public DateTime? GIDueDate { get; set; }
        public decimal? LIPremium { get; set; }
        public DateTime? LICommencementDate { get; set; }
        public DateTime? LIDueDate { get; set; }
        public decimal? UpdatedBalance { get; set; }
        public TimePeriod? TimePeriod { get; set; }
        public int? NoOfMonthsORYears { get; set; }
        public DateTime? MaturityDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal Amount { get; set; }
        public decimal? MaturityAmount { get; set; }
        public DateTime? LastInstallmentDate { get; set; }
        public DateTime? NextInstallmentDate { get; set; }
        public decimal? AgentCommission { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string CertificateNumber { get; set; }
        public int? TotalInstallment { get; set; }
        public bool? IsFreeze { get; set; }
        public int? TotalDays { get; set; }
        public string OldAccountNumber { get; set; }
        public decimal? OpeningBalance { get; set; }
        public bool? SkipFirstInstallment { get; set; }
        [ForeignKey("LoanTypeId")]
        public Lookup Lookup { get; set; }
        public virtual string PaymentName
        {
            get
            {
                return PaymentType.ToString().Replace("_", "-");
            }
        }
        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }
        public virtual string ProductName
        {
            get
            {
                return ProductType.ToString();
            }
        }
        public virtual string TimePeriodName
        {
            get
            {
                return TimePeriod.ToString().Replace("_", "-");
            }
        }
        public CustomerProductStatus? Status { get; set; }
        public bool? CommissionPaid { get; set; }
        public bool? IsPrematured { get; set; }
        public decimal? PrematureAmount { get; set; }
        public decimal? PrematurePercentage { get; set; }
        public Guid? PrematuredBy { get; set; }
        public DateTime? PrematureDate { get; set; }
        public bool? IsCertificatePrinted { get; set; }

        public bool? IsAutoFD { get; set; }
        public Guid? ReferenceCustomerProductId { get; set; }

        [NotMapped]
        public string EmpName { get; set; }

        public CustomerProduct()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
        }

    }


    public class PrintCertificateHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CertificateHistoryId { get; set; }
        public Guid CustomerProductId { get; set; }
        public string Reason { get; set; }
        public bool IsDuplicate { get; set; }
        public Guid PrintedBy { get; set; }
        public DateTime PrintedDate { get; set; }
        [ForeignKey("CustomerProductId")]
        public CustomerProduct CustomerProduct { get; set; }

        public PrintCertificateHistory()
        {
            PrintedDate = DateTime.Now;
        }
    }

    public enum LITYPE
    {
        Regular = 1,
        Single = 2
    }

    public enum CustomerProductStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4,
        Completed = 5,
        Closed = 6
    }

    public class AccountExist
    {
        public Guid CustomerId { get; set; }
        public ProductType ProductType { get; set; }
        public Guid ProductId { get; set; }
    }

    public class CustomerLoanDetail
    {
        public CustomerPersonalDetail Personal { get; set; }
        public CustomerProduct Product { get; set; }
        public Customer customer { get; set; }
    }

    public class CalculateMaturityAmount
    {
        public ProductType ProductType { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Amount { get; set; }
        public Frequency InterestType { get; set; }
        public DateTime? DueDate { get; set; }
        public Frequency? PaymentType { get; set; }
        public TimePeriod? TimePeriod { get; set; }
        public int? NoOfMonthsORYears { get; set; }
        public decimal? OpeningBalance { get; set; }
        public bool? SkipFirstInstallment { get; set; }
        public string ProductName { get; set; }
    }

    public class InterAccountList
    {
        public Guid CustomerProductId { get; set; }
        public decimal Amount { get; set; }
        public string ProductName { get; set; }
        public string AccountNumber { get; set; }
        public ProductType ProductType { get; set; }
    }

    public class PrematureRDFD
    {
        public Guid CustomerProductId { get; set; }
        public decimal PrematureCharges { get; set; }
        public decimal PrematureAmount { get; set; }
        public DateTime OpeningDate { get; set; }
        public decimal PrematurePercentage { get; set; }
        public decimal InterestRate { get; set; }
        public ProductType ProductType { get; set; }
        public TimePeriod TimePeriod { get; set; }
        public string Period { get; set; }
        public int NoOfMonthsORYears { get; set; }
        public decimal Balance { get; set; }

        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }
    }

    public class RegularIncomeData
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Amount { get; set; }
        public DateTime OpeningDate { get; set; }
        public string TimePeriod { get; set; }
        public int NoofMonthOrYear { get; set; }
        public ProductType ProductType { get; set; }
        public DateTime MaturityDate { get; set; }
        public Frequency PaymentType { get; set; }
        public decimal MaturityAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public List<RIPList> RIPList { get; set; }
        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }
        public virtual string PaymentName
        {
            get
            {
                return PaymentType.ToString().Replace("_", "-");
            }
        }
    }

    public class CalculateRIP
    {
        public decimal Amount { get; set; }
        public DateTime OpeningDate { get; set; }
        public TimePeriod TimePeriod { get; set; }
        public int NoofMonthOrYear { get; set; }
        public ProductType ProductType { get; set; }
        public DateTime MaturityDate { get; set; }
        public Frequency PaymentType { get; set; }
        public decimal MaturityAmount { get; set; }
        public decimal? InterestRate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? NoofMonthsandYearforParentRD { get; set; }
        public int? TotalProductYear { get; set; }
        public decimal? PreviousYearsBalance { get; set; }
    }

    public class RIPList
    {
        public DateTime InstallmentDate { get; set; }
        public decimal Amount { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal MaturityAmount { get; set; }
        public string Years { get; set; }
        public string Account { get; set; }
    }

    public class PrematureRIPData
    {
        public string CustomerName { get; set; }
        public DateTime OpeningDate { get; set; }
        public Guid CustomerProductId { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal PrematureAmount { get; set; }
        public decimal InterestRate { get; set; }
        public ProductType ProductType { get; set; }
        public int TimePeriod { get; set; }
        public string Period { get; set; }
        public int NoOfMonthsORYears { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid PrematureBy { get; set; }
    }
    public class HolderDataResult
    {
        public ProductType ProductType { get; set; }
        public string ProductTypeName { get; set; }
        public Guid BranchId { get; set; }
        public string BranchName { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CustomerProductId { get; set; }
        public decimal? Balance { get; set; }
        public string AccountNumber { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? LastInstallmentDate { get; set; }
        public bool? IsFreeze { get; set; }
        public CustomerProductStatus Status { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
    }
}
