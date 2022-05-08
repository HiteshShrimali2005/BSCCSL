using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class MortgageLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MortgageLoanId { get; set; }
        public Guid LoanId { get; set; }
        public DateTime? ItemDate { get; set; }
        public double ItemDatePrice { get; set; }
        public DateTime? ValuationDate { get; set; }
        public double ValuationPrice { get; set; }
        public double TotalPrice { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }

        public MortgageLoan()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }

    public class MortgageItemInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MortgageItemInformationId { get; set; }
        public Guid MortgageLoanId { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string Item { get; set; }
        [StringLength(255)]
        [Column(TypeName = "varchar")]
        public string Type { get; set; }
        public decimal ItemPrice { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("MortgageLoanId")]
        public virtual MortgageLoan MortgageLoan { get; set; }

        public MortgageItemInformation()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }
}
