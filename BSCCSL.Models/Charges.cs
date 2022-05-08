using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class Charges
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ChargesId { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }
        public decimal Value { get; set; }
        public bool IsDelete { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public Charges()
        {
            CreatedDate = DateTime.Now;
        }

    }

}
