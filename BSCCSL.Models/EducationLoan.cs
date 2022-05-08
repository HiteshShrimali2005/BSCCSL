using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSCCSL.Models
{
   public class EducationLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EducationLoanId { get; set; }
        public Guid LoanId { get; set; }
        //public DateTime? DateofApplication { get; set; }
        //[StringLength(100)]
        //[Column(TypeName = "varchar")]
        //public string EduApplicationNo { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string EduCourseName { get; set; }
        public int EduTotalYearofCourse { get; set; }
        public DateTime? CourseStartingDate { get; set; }
        public DateTime? CourseEndingDate { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string EduCoursePlace { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string EduNameofCollege { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string EduAddressofCollege { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string EducationCity { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Educationstate { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string EducationPincode { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string EducationPhoneNum { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string EducationFaxNum { get; set; }
        public double EduExpenditures { get; set; }
        public double EduNonRePayableScholarship { get; set; }
        public double EduRePayableScholarship { get; set; }
        public double EduFamilyFunded { get; set; }
        public double EduLoanAmount { get; set; }
        public bool EduSecurityoffer { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string EduSecurityType { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string EduReferenceNumber { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string EduSecurityInformation { get; set; }
        public double EduSecurityAmount { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string EduSecurityType2 { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string EduReferenceNumber2 { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string EduSecurityInformation2 { get; set; }
        public double EduSecurityAmount2 { get; set; }

        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }

        public EducationLoan()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }

    public class AcademicDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AcademicDetailsId { get; set; }
        public Guid EducationLoanId { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string ExamQualified { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string University_Institution { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string EducationMedium { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string YearofQualifing { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string Qualifiedtrial { get; set; }
        public int MarksinFirstTrial { get; set; }
        public double MarksPercentage { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string Class_Grade { get; set; }

        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("EducationLoanId")]
        public virtual EducationLoan EducationLoan { get; set; }

        public AcademicDetails()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }

    public class PurposeofEducationLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PurposeofEducationLoanId { get; set; }
        public Guid EducationLoanId { get; set; }
        public Guid LookupId { get; set; }
        public double TutionFees { get; set; }
        public double ExamFees { get; set; }
        public double BookFees { get; set; }
        public double Rent { get; set; }
        public double Board { get; set; }
        public double Clothe { get; set; }
        public double Casual { get; set; }
        public double InsurancePremium { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("EducationLoanId")]
        public virtual EducationLoan EducationLoan { get; set; }
        [ForeignKey("LookupId")]
        public virtual Lookup Lookup { get; set; }

        public PurposeofEducationLoan()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }

    }

    public class SaveEducationInfo
    {
        public List<AcademicDetails> EducationInfo { get; set; }
        public List<PurposeofEducationLoan> EducationLoanPurpose { get; set; }
    }
}
