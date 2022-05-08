using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class PassbookPrint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PassbookPrintId { get; set; }
        public string AccountNo { get; set; }
        public DateTime PassbookPrintDate { get; set; }
        public int PrintedCount { get; set; }
        public int LastPageCount { get; set; }

        public PassbookPrint()
        {
            PassbookPrintDate = DateTime.Now;
        }

    }
}
