using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BSCCSL.Models
{
    public class SMSLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SMSLogId {get; set;}

        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string MobileNo { get; set; }

        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Message { get; set; }
        public bool Send { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string SMSname { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
