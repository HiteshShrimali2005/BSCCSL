using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSCCSL.Models
{
   public class Lookup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LookupId { get; set; }
        public Guid LookupCategoryId { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }
        public int? Order { get; set; }
        public bool IsDelete { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [ForeignKey("LookupCategoryId")]
        public virtual LookupCategory LookupCategory { get; set; }
    }
}
