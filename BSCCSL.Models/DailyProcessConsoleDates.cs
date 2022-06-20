using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class DailyProcessConsoleDates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public string DailyProcessName { get; set; }
        public string DailyProcessCode { get; set; }
        public DateTime DailyProcessDate { get; set; }
        public DateTime CreatedDateTime { get; set; }

    }
}
