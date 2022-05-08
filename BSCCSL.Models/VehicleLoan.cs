using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSCCSL.Models
{
    public class VehicleLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid VehicleLoanId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid LoanId { get; set; }
        public Guid NewAssetType { get; set; }
        //public DateTime ApplicationDate { get; set; }
        //public long ApplicationNo { get; set; }
        public int AssetLife { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string ProducerName { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string AccetMacvariant { get; set; }
        public string Category { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string DealerName { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string Model { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string DealerCode { get; set; }

        public decimal Xshowroomprice { get; set; }

        public decimal IfAnyOtherTax { get; set; }
        public decimal RegistrationCost { get; set; }
        public decimal TotalOnroadPrice { get; set; }
        public decimal Insurance { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }

        public VehicleLoan()
        {
            CreatedDate = DateTime.Now;
            IsDeleted = false;
        }
    }
}
