using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSCCSL.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CustomerId { get; set; }
        //[StringLength(200)]
        //[Column(TypeName = "varchar")]
        //public string AccountNo { get; set; }
        public Guid BranchId { get; set; }
        public Guid? AgentId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string BranchCode { get; set; }
        /// <summary>
        /// Password for mobile Application
        /// </summary>
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string Password { get; set; }

        //   public decimal? Balance { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string ApplicationNo { get; set; }
        [StringLength(10)]
        [Column(TypeName = "varchar")]
        public string Sector { get; set; }
        [StringLength(10)]
        [Column(TypeName = "varchar")]
        public string FormType { get; set; }
        public bool IsDelete { get; set; }
        public string OldClientId { get; set; }
        [NotMapped]
        public string BranchName { get; set; }

        [NotMapped]
        public Guid CustomerProductId { get; set; }
        [NotMapped]
        public decimal? ProductwiseBalance { get; set; }

        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string ClienId { get; set; }

        [ForeignKey("AgentId")]
        public User User { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }


        public Customer()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }


    public class CustomerUpdateData
    {
        public Guid? CustomerProductId { get; set; }
        public decimal Balance { get; set; }
        public TransactionType TransactionType { get; set; }

        //public decimal? Balance { get; set; }
        //public Guid? CreatedBy { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public Guid? ModifiedBy { get; set; }
    }

    public class CustomerPersonalDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PersonalDetailId { get; set; }
        public Guid CustomerId { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string FirstName { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string MiddleName { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string LastName { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string FatherorSpouseName { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string MotherName { get; set; }
        public DateTime? DOB { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Nationality { get; set; }
        [StringLength(10)]
        [Column(TypeName = "varchar")]
        public string Sex { get; set; }
        public int Age { get; set; }
        [StringLength(200)]
        [Column(TypeName = "nvarchar")]
        public string PlaceOfBirth { get; set; }
        [StringLength(200)]
        [Column(TypeName = "nvarchar")]
        public string Occupation { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string BirthCertificate { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string DrivingLicence { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Passport { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PanCard { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Other { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string IdentityProof { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string HolderPhotograph { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string HolderSign { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Adharcard { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }

    public class CustomerAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AddressId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid PersonalDetailId { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string DoorNo { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string BuildingName { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PlotNo_Street { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string CustomerName { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Landmark { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Area { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string District { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Place { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string City { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string State { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Pincode { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string TelephoneNo { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string MobileNo { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Email { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string AddressProof { get; set; }

        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerDoorNo { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerBuildingName { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerPlotNo_Street { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerCustomerName { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerLandmark { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerArea { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerDistrict { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerPlace { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerCity { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerState { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerPincode { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerTelephoneNo { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerMobileNo { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerEmail { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PerAddressProof { get; set; }
        public bool IsDelete { get; set; }
        [ForeignKey("PersonalDetailId")]
        public virtual CustomerPersonalDetail CustomerPersonalDetail { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }

    public class PersonalDetails
    {
        public CustomerPersonalDetail Personal { get; set; }
        public CustomerAddress Address { get; set; }
        public List<CustomerProofDocument> Documents { get; set; }
        //public CustomerProofDocument Documents{get;set;}
    }
    public class DisplayCustomerDetail
    {
        public CustomerPersonalDetail Personal { get; set; }
        public Customer Customer { get; set; }
        public CustomerAddress Address { get; set; }
        public List<CustomerProofDocument> Documents { get; set; }

        //public CustomerProofDocument Documents{get;set;}
    }
    public class CustomerDetail
    {
        public Customer Customer { get; set; }
        public List<PersonalDetails> Personal { get; set; }
        public List<CustomerPersonalDetail> CustomerPersonal { get; set; }
    }
    public class UploadPhotograph
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public int PersonId { get; set; }
        public string NewDocument { get; set; }
    }

    public class CustomerDocuments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DocumentId { get; set; }

        [StringLength(200)]
        public string Path { get; set; }

        public Guid? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [StringLength(200)]
        public string DocumentName { get; set; }

        public bool IsDeleted { get; set; }
    }

}
