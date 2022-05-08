using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class FreezedRDModel
    {
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public Guid CustomerProductId { get; set; }
        public string ClienId { get; set; }
        public Guid CustomerId { get; set; }
    }

}
