using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSCCSL.Models
{
 public class Reference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ReferenceId { get; set; }
        public Guid LoanId { get; set; }
        public Guid? Title { get; set; }
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

        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }


        [StringLength(10)]
        [Column(TypeName = "varchar")]
        public string Sex { get; set; }
        [StringLength(25)]
        [Column(TypeName = "varchar")]
        public string DOB { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string FullName { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string Former_OtherName { get; set; }
        public decimal AnnualIncome { get; set; }
        public int? FamilyMember { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string MotherName { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Employeeid { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Department { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string AadharNumber { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string PancardNumber { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string PrimaryPostOfficeAddress { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string PersentAddressMonthYear { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string YearinCurrentCity { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string Address1 { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string Address2 { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Landmark { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string City { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string State { get; set; }
        [StringLength(10)]
        [Column(TypeName = "varchar")]
        public string Pincode { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Country { get; set; }
        [StringLength(10)]
        [Column(TypeName = "varchar")]
        public string CountryCode { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string StdCode { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string TelephoneNo { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string PhoneNumber { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Extension { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string Email { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string MobileNumber { get; set; }

        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }

        public Reference()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }
}
