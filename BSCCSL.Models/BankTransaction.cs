using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class BankTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BankTransactionId { get; set; }
        public Guid BankId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public TypeCRDR Type { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? BranchId { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [ForeignKey("BankId")]
        public BankMaster BankMaster { get; set; }
        public TransactionType TransactionType { get; set; }
        public long? CheckNumber { get; set; }
        public DateTime? ChequeDate { get; set; }
        public int? BounceReason { get; set; }
        public decimal? Penalty { get; set; }
        public DateTime? ChequeClearDate { get; set; }
        public Status Status { get; set; }
        //  public decimal? UnclearBalance { get; set;}
        public DateTime? TransactionTime { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
        public DescIndentify? DescIndentify { get; set; }
        
        public BankTransaction()
        {
            CreatedDate = DateTime.Now;
        }
        
        public virtual string TypeName
        {
            get
            {
                return Type.ToString();
            }
        }
    }
}
