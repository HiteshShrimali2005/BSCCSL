using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSCCSL.Models
{
    public class BusinessLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BusinessLoanId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid LoanId { get; set; }
        public Guid AnualTurnOver { get; set; }
        public Guid IndustryType { get; set; }
        public Guid AgricultureIfApplicable { get; set; }
        public Guid CompanyType { get; set; }
        // public Guid HomeAccomodationType { get; set; }
        public Guid AccomodationType { get; set; }
        public Guid OfficeAccomodationType { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string CustomerName { get; set; }
        public DateTime EstablishDate { get; set; }
        public decimal NumberOfYearsInBusiness { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string PANOrGRINumber { get; set; }
        [StringLength(500)]
        [Column(TypeName = "varchar")]
        public string PrimaryPostOfficeAddress { get; set; }
        public decimal NumberOfYearsOfCurrentAddress { get; set; }
        public decimal NumberOfYearsOfCurrentCity { get; set; }
        [StringLength(500)]
        [Column(TypeName = "varchar")]
        public string AccomodationAddress1 { get; set; }
        [StringLength(500)]
        [Column(TypeName = "varchar")]
        public string AccomodationAddress2 { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string LandMark { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string City { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string State { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string Pincode { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string Country { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string CountryCode { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string STDCode { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string PhoneNo { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string Accetension { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string Email { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string MobileNo { get; set; }
        //Office address
        [StringLength(500)]
        [Column(TypeName = "varchar")]
        public string OfficePrimaryPostOfficeAddress { get; set; }
        public decimal NumberOfYearsOfCurrentOrganization { get; set; }
        public decimal NumberOfYearsOfExperience { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string CompanyName { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string Position { get; set; }
        [StringLength(500)]
        [Column(TypeName = "varchar")]
        public string OfficeAccomodationAddress1 { get; set; }
        [StringLength(500)]
        [Column(TypeName = "varchar")]
        public string OfficeAccomodationAddress2 { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficeLandMark { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficeCity { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficeState { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficePincode { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficeCountry { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficeCountryCode { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficeSTDCode { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficePhoneNo { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficeAccExtension { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficeEmail { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string OfficeMobileNo { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        [ForeignKey("LoanId")]
        public Loan Loan { get; set; }
        public bool IsDeleted { get; set; }
        public BusinessLoan()
        {
            IsDeleted = false;
            CreatedDate = DateTime.Now;
        }
    }



    public class BusinessEconomicDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BusinessEconomicDetailsId { get; set; }
        public Guid BusinessLoanId { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string EconomicDetails { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string FY1 { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string FY2 { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string FY3 { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey("BusinessLoanId")]
        public BusinessLoan BusinessLoan { get; set; }
        public BusinessEconomicDetails()
        {
            IsDeleted = false;
            CreatedDate = DateTime.Now;
        }
    }
}
