using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string UserCode { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string FirstName { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string LastName { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string UserName { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Password { get; set; }
        [StringLength(10)]
        [Column(TypeName = "varchar")]
        public string Gender { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Address { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar")]
        public string PhoneNumber { get; set; }
        public Role Role { get; set; }
        public Guid? RankId { get; set; }
        public Guid BranchId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? CustomerId { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        public virtual string RoleName
        {
            get
            {
                return Role.ToString().Replace("_", "-");
            }
        }
        [NotMapped]
        public string SavingAccountNo { get; set; }

        public bool? IsExecutive { get; set; }

        public User()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }
    public enum Role
    {
        Admin = 1,
        Manager = 2,
        Cashier = 3,
        Agent = 4,
        Clerk = 5,
        CashierPlusClerk = 6,
        Scree_Sales = 7,
        Sales_Manager = 8
    }
    public class UserPassworData
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
