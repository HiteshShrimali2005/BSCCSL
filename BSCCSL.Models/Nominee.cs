using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
   public class Nominee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NomineeId { get; set; }
        public Guid CustomerId { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string PlaceofBirth { get; set; }
        public DateTime? NomineeDOB { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string RelationtoAccountholder { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string AppointeeName { get; set; }
        [StringLength(200)]
        [Column(TypeName ="varchar")]
        public string AppointeePlaceofBirth { get; set; }
        public DateTime? AppointeeDOB { get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string AppointeeRelationtoAcholder{ get; set; }
        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string RelationtoNominee { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public Nominee()
        {
            CreatedDate = DateTime.Now;
            IsDelete = false;
        }
    }
}
