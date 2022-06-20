using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductId { get; set; }
        public ProductType ProductType { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public Guid? LoanTypeId { get; set; }
        public double InterestRate { get; set; }
        public Interest InterestType { get; set; }
        public Frequency Frequency { get; set; }
        public Frequency PaymentType { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        [ForeignKey("CreatedBy")]
        public User User { get; set; }
        public Guid? ModifiedBy { get; set; }
        [ForeignKey("ModifiedBy")]
        public User UserModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal? LatePaymentFees { get; set; }
        public TimePeriod TimePeriod { get; set; }
        public int? NoOfMonthsORYears { get; set; }
        public decimal? CommissionPercentage { get; set; }
        [ForeignKey("LoanTypeId")]
        public Lookup Lookup { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]

        public virtual string InterestName
        {
            get
            {
                return InterestType.ToString().Replace("_", " ");
            }
        }
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
        public virtual string FrequencyName
        {
            get
            {
                return Frequency.ToString().Replace("_", "-");
            }
        }
        public Product()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }

        [NotMapped]
        public string LoanType { get; set; }
    }

    public enum Interest
    {
        Credit = 1,
        Debit = 2
    }
    public enum ProductType
    {
        [Description("Saving")]
        Saving_Account = 1,
        [Description("Current")]
        Current_Account = 2,
        [Description("FD")]
        Fixed_Deposit = 3,
        [Description("RD")]
        Recurring_Deposit = 4,
        [Description("Loan")]
        Loan = 5,
        [Description("RIP")]
        Regular_Income_Planner = 6,
        [Description("MIS")]
        Monthly_Income_Scheme = 7,
        [Description("TYP")]
        Three_Year_Product = 8,
        [Description("CB")]
        Capital_Builder = 9,
        [Description("WC")]
        Wealth_Creator = 10
    }
    public enum Frequency
    {
        Daily = 1,
        Monthly = 2,
        Quarterly = 3,
        Half_Yearly = 4,
        Yearly = 5
    }

    public enum TimePeriod
    {
        Months = 1,
        Years = 2,
        Days=3
    }
}
