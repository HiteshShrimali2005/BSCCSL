using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class RDPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RDPaymentId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CustomerProductId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsPaid { get; set; }
        public bool? CommissionPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public RDPaymentType RDPaymentType { get; set; }
        public decimal? AgentCommission { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? InterestAmount { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("CustomerProductId")]
        public CustomerProduct CustomerProduct { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public virtual string RDPaymentTypeName
        {
            get
            {
                return RDPaymentType.ToString().Replace("_", " ");
            }
        }
        public RDPayment()
        {
            CreatedDate = DateTime.Now;
        }
    }

    public enum RDPaymentType
    {
        Installment = 1,
        LatePaymentCharges = 2
    }

    public class RDPendingPaymentInstallment
    {
        public Guid RDPaymentId { get; set; }
        public Guid CustomerProductId { get; set; }
        public decimal Amount { get; set; }
        public decimal LatePaymentCharges { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? NextDate { get; set; }
        public Guid? ReferenceCustomerProductId { get; set; }
    }

    public class RDPendingPayment
    {
        public List<RDPendingPaymentInstallment> rdPaymentList { get; set; }
        public Transactions transaction { get; set; }
    }

}
