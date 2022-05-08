using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class RDFDPrematurePercentage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RDFDPrematurePercentageId { get; set; }
        public int TotalYear { get; set; }
        public int PrematureMonth { get; set; }
        public decimal Percentage { get; set; }
    }
}
