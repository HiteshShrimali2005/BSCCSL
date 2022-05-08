using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class JournalVoucher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Guid JournalVoucherId { get; set; }

        public string JournalVoucherName { get; set; }

        public string FromAccount { get; set; }
        public string ToAccount { get; set; }

        public Guid FromAccountHead { get; set; }
        public Guid ToAccountHead { get; set; }

        public HeadType FromHeadType { get; set; }
        public HeadType ToHeadType { get; set; }

        public decimal Amount { get; set; }

        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDelete { get; set; }

        [Column(TypeName = "nvarchar")]
        public string Description { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        public Guid BranchId { get; set; }

        public Guid? FromBranchId { get; set; }

        public Guid? ToBranchId { get; set; }

        public string TransactionMode { get; set; }

        public DateTime? JVDate { get; set; }

        public string JVNumber { get; set; }

        public Guid? ToCustomerProductId { get; set; }

        public int Type { get; set; }


        public JournalVoucher()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }

    }
}
