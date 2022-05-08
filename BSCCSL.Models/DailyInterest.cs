using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
        public class DailyInterest
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public Guid DailyInterestId { get; set; }
            public Guid CustomerProductId { get; set; }
            public decimal TodaysInterest { get; set; }
            public decimal InterestRate { get; set; }
            public DateTime CreatedDate { get; set; }
            public bool IsPaid { get; set; }
            [ForeignKey("CustomerProductId")]
            public CustomerProduct CustomerProduct { get; set; }
            public DailyInterest()
            {
                CreatedDate = DateTime.Now;
            }
        }
}
