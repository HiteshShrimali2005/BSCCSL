using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class ProductCommission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CommissionId { get; set; }
        public Guid ProductId { get; set; }
        public int ProductYear { get; set; }
        public int CommissionYear { get; set; }
        public decimal Commission { get; set; }
        public bool IsAbove { get; set; }
        public Guid RankId { get; set; }
        [ForeignKey("RankId")]
        public AgentRank AgentRank { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
       
        public string Version { get; set; }
    }
}
