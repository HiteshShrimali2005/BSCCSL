using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSCCSL.Models
{
    public class Borrower
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BorrowerId { get; set; }
        // public Guid CustomerId { get; set; }
        public Guid PersonalDetailId { get; set; }
        public Guid LoanId { get; set; }
        public Guid? GroupLoanId { get; set; }
        // public Guid? Title { get; set; }
        public Guid? Category { get; set; }
        public Guid? EducationDetail { get; set; }
        public Guid? EmployementType { get; set; }
        public Guid? IncomeSource { get; set; }
        public Guid? IfSalaried { get; set; }
        public Guid? OrganisationNature { get; set; }
        public Guid? Proffesion { get; set; }
        public Guid? AccomodationType { get; set; }
        public Guid? BusinessType { get; set; }
        public Guid? AccountType { get; set; }
        public Guid? Caste { get; set; }
        public Guid? MartialStatus { get; set; }
        public Referencertype Referencertype { get; set; }

        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }
        
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string Former_OtherName { get; set; }
        public decimal? AnnualIncome { get; set; }
      
        public int? FamilyMember { get; set; }
        public string Employeeid { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Department { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string PrimaryPostOfficeAddress { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string PersentAddressMonthYear { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string YearinCurrentCity { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Country { get; set; }

        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string OfficePrimaryPostOfficeAddress { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficePersentAddressMonthYear { get; set; }
        public int? YearofExperience { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string Company_firmName { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Position { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string OfficeAddress1 { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string OfficeAddress2 { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string OfficeLandmark { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string OfficeCity { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string OfficeState { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string OfficePinCode { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string OfficeCountry { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string AccCountryCode { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string AccStdCode { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string AccPhoneNum { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string AccExtension { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string AccEmail { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string AccMobileNum { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string CompanyfirmAccDetail { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string FirmBranch { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string OrganizationAccNum { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string AccountOpenyear { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Accountodcclimit { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string IssuerName1 { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string CardNumber1 { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string CreditLimit1 { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string IssuerName2 { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string CradNum2 { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string CreditLimit2 { get; set; }

        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }

        [ForeignKey("PersonalDetailId")]
        public virtual CustomerPersonalDetail CustomerPersonalDetail { get; set; }

        public ICollection<BorrowerLoanDetails> LoanDetails { get; set; }

        public Borrower()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }

    public class DisplayBorrower
    {
        public CustomerPersonalDetail Personal { get; set; }
        public Customer Customer { get; set; }
        public CustomerAddress Address { get; set; }
        public List<CustomerProofDocument> Documents { get; set; }
        public Borrower Borrower { get; set; }
        public List<BorrowerLoanDetails> LoanDetails { get; set; }
    }

    public enum Referencertype
    {
        Borrower = 1,
        Referencer = 2,
        Guarantor = 3,
        Owner = 4
    }
}
