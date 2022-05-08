using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
   public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BranchId { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string BranchName { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string BranchAddress { get; set; }
        [StringLength(12)]
        [Column(TypeName = "varchar")]
        public string BranchPhone { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string BranchCode { get; set; }
        public string RegistrationNo { get; set; }
        public bool IsHO { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        //[ForeignKey("CreatedBy")]
        //public User User { get; set; }
        public Guid? ModifiedBy { get; set; }
        //[ForeignKey("ModifiedBy")]
        //public User UserModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public Branch()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }

    }
}
