using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class CustomerShare
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ShareId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Share { get; set; }
        public decimal ShareAmount { get; set; }
        public decimal Total { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Maturity Maturity { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string CertificateNumber { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string FromNumber { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string ToNumber { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string OldFromNumber { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string OldToNumber { get; set; }
        public bool? IsReverted { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public CustomerShare()
        {
            CreatedDate = DateTime.Now;
        }
        public virtual string MaturityName
        {
            get
            {
                return Maturity.ToString();
            }
        }

        public bool? DeductShareAmount { get; set; }
        public bool? DeductAdmissionFee { get; set; }

    }


    public enum Maturity
    {
        Nominal = 1,
        Regular = 2,
        Premium = 3
    }
}
