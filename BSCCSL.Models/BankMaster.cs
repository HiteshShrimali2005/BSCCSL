using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class BankMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BankId { get; set; }
        [StringLength(30)]
        [Column(TypeName = "nvarchar")]
        public string AccountNumber { get; set; }
        [StringLength(70)]
        [Column(TypeName = "nvarchar")]
        public string BankName { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Address { get; set; }
        public ProductType AccountType { get; set; }
        public decimal Balance { get; set; }
        public decimal UpdatedBalance { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual string AccountTypeName
        {
            get
            {
                return AccountType.ToString().Replace("_", " ");
            }
        }

        public BankMaster()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }

    public class BankBranchMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BranchMappingId { get; set; }
        public Guid BranchId { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        public Guid BankId { get; set; }
        [ForeignKey("BankId")]
        public virtual BankMaster BankMaster { get; set; }
        public bool IsDelete { get; set; }
    }

    public class BankDetails
    {
        public BankMaster Bank { get; set; }
        public List<Guid> BranchIds { get; set; }
    }
}
