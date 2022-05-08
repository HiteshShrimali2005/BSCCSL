using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class DDSPaymentListModel
    {
        public string AgentName { get; set; }
        public string CustomerName { get; set; }
        public string RDAccountNumber { get; set; }
        public string SavingAccountNumber { get; set; }
        public string ClienId { get; set; }
        public decimal Amount { get; set; }
        public decimal RDAmount { get; set; }
        public string Description { get; set; }
        public Guid SavingAccountCustomerProductId { get; set; }
        public Guid CustomerId { get; set; }
    }
}
