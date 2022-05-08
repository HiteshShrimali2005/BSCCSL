using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
   public class Setting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SeetingId { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string SettingName { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Value { get; set; }
        public Guid? BranchId { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        //public Setting()
        //{
            
        //}
    }
    
}
