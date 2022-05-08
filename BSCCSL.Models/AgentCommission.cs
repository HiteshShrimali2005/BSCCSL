using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class AgentCommission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AgentCommissionId { get; set; }
        public Guid CustomerProductId { get; set; }
        public Guid AgentId { get; set; }
        public Guid RankId { get; set; }
        public Guid? RDPaymentId { get; set; }
        public decimal Commission { get; set; }
        public decimal CommissionPercentage { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public Guid? PaidBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        //public Guid? ModifiedBy { get; set; }
        //public Guid? ModifiedDate { get; set; }
        [ForeignKey("RDPaymentId")]
        public RDPayment RDPayment { get; set; }
        [ForeignKey("CustomerProductId")]
        public CustomerProduct CustomerProduct { get; set; }
        [ForeignKey("AgentId")]
        public User User { get; set; }
        [ForeignKey("RankId")]
        public AgentRank AgentRank { get; set; }

        public AgentCommission()
        {
            CreatedDate = DateTime.Now;
        }
    }

    public class ProductAgentCommission
    {
        public decimal Amount { get; set; }
        public DateTime OpeningDate { get; set; }
        public Guid ProductId { get; set; }
        public Guid AgentId { get; set; }
        public Guid RankId { get; set; }
        public Guid CustomerProductId { get; set; }
        public Guid? RDPaymentId { get; set; }
        public int TotalDays { get; set; }
        public DateTime Date { get; set; }
    }
}
