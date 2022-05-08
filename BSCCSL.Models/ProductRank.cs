using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class ProductRank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductRankId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Percentage { get; set; }
        public bool IsDelete { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public ProductRank()
        {
            IsDelete = false;
            CreatedDate = DateTime.Now;
        }
    }
}
