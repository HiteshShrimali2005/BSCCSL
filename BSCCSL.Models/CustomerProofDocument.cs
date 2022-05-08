using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class CustomerProofDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DocumentId { get; set; }
        public Guid PersonalDetailId { get; set; }
        public string DocumentName { get; set; }
        public ProofType ProofTypeId { get; set; }
        public string Path { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("PersonalDetailId")]
        public CustomerPersonalDetail CustomerPersonalDetail { get; set; }
        public CustomerProofDocument()
        {
           IsDelete = false;
        }
        public virtual string documenttype
        {
            get
            {
                return ProofTypeId.ToString().Replace("_", " ");
            }
        }
    }
    public enum ProofType
    {
        BirthCertificate = 1,
        DrivingLicence=2,
        Passport=3,
        PanCard=4,
        Other=5,
        IdentityProof=6,
        AddressProof=7,
        Adharcard=8,
        PerAddressProof=9
       //HolderPhotograph=9,
        //HolderSign = 10
    }
}
