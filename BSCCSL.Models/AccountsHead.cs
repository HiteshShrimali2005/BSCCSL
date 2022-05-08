using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class AccountsHead
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AccountsHeadId { get; set; }
        public HeadType HeadType { get; set; }
        public string HeadName { get; set; }
        public string Description { get; set; }
        public Guid? ParentHead { get; set; }
        public bool IsDelete { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string AccountsHeadCode { get; set; }
        public virtual string HeadTypeName
        {
            get
            {
                return HeadType.ToString().Replace("_", "-");
            }
        }
        public AccountsHead()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }

    public enum HeadType
    {
        Income = 1,
        Expense = 2,
        Assets = 3,
        Liability = 4
    }
}
