using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
  public class Term
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TermId { get; set; }
        public TimePeriod? TimePeriod { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public decimal InterestRate { get; set;}
        public int TotalFrom { get; set; }
        public int TotalTo { get; set;}
        public bool IsDelete { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public Term()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
        public virtual string Time
        {
            get
            {
                return TimePeriod.ToString();
            }
        }
    }
}
