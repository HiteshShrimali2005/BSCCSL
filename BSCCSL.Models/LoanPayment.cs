using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class LoanPrePayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LoanPaymentId { get; set; }
        public Guid LoanId { get; set; }
        public decimal MonthlyInstallmentAmount { get; set; }
        public decimal? PaymentAmount { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime InstallmentDate { get; set; }
        public int Term { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [ForeignKey("LoanId")]
        public Loan Loan { get; set; }
        [NotMapped]
        public DateTime? TransactionTime { get; set; }
        [NotMapped]
        public Guid? CustomerProductIdSaving { get; set; }
        [NotMapped]
        public decimal? RemainingAmount { get; set; }
            
        public decimal? TotalPendingInstallmentAmount { get; set; }

        public decimal? TotalPendingInteresttillDate { get; set; }



        public LoanPrePayment()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
