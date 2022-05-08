using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class AgentCommissionHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PaymentId { get; set; }
        public Guid AgentId { get; set; }
        public Guid CustomerId { get; set; }
        public ProductType ProductType { get; set; }
        public string AccountNumber { get; set; }
        public decimal PaidAmount { get; set; }
        public Guid? PaidBy { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime? PaidDate { get; set; }
        public Guid? BranchId { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        [ForeignKey("AgentId")]
        public User User { get; set; }
    }

    public class CommissionData
    {
        public List<RptAgentCommissionByMonth> Commission { get; set; }
        public Guid? PaidBy { get; set; }
        public Guid? BranchId { get; set; }
    }
    
    public class CData
    {
        public Guid AgentId { get; set; }
        public List<RptAgentCommissionByMonth> CHistory { get; set; }
    }

    public class RptAgentHierarchy
    {
        public string AgentName { get; set; }
        public string SR1 { get; set; }
        public string SR2 { get; set; }
        public string SR3 { get; set; }
    }
}
