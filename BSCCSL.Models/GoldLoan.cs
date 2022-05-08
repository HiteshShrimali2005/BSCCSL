using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSCCSL.Models
{
   public class GoldLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid GoldLoanId { get; set; }
        public Guid LoanId { get; set; }
        //public DateTime? DateofApplication { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string GoldApplicationNo { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string GoldloanType { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string GoldType { get; set; }
        public DateTime? JewelleryDate { get; set; }
        public double JewelleryDatePrice { get; set; }
        public DateTime? ValuationDate { get; set; }
        public double ValuationPrice { get; set; }
        public double TotalPrice { get; set; }
        public double? TotalWeight { get; set; }
        public double InterestRate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }

        public GoldLoan()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }

    public class JewelleryInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid JewelleryInformationId { get; set; }
        public Guid GoldLoanId { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string Item { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string Type { get; set; }
        public double ItemWeight { get; set; }
        public double NetWeight { get; set; }
        public decimal ItemPrice { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("GoldLoanId")]
        public virtual GoldLoan GoldLoan { get; set; }

        public JewelleryInformation()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }
}
