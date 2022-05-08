using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class PrematureRDFDPercentage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PrematureRDFDPercentageId { get; set; }
        public int Year { get; set; }
        public int PrematureYear { get; set; }
        public decimal Percentage { get; set; }
    }
}
