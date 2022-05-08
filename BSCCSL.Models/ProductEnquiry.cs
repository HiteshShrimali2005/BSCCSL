using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class ProductEnquiry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductEnquiryId { get; set; }

        public Guid? CustomerId { get; set; }

        [Required(ErrorMessage = "Please select Product Type")]
        public ProductType ProductType { get; set; }

        public DateTime EnquiryDate { get; set; }

        [Required(ErrorMessage = "Please enter First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter Contact Number")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Please enter Comments")]
        public string Comments { get; set; }

        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public EnquirySource EnquirySource { get; set; }

        public EnquiryStatus Status { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }

        public virtual string EnquirySourceName
        {
            get
            {
                return EnquirySource.ToString().Replace("_", " ");
            }
        }

        public virtual string StatusName
        {
            get
            {
                return Status.ToString().Replace("_", " ");
            }
        }

        public ProductEnquiry()
        {
            EnquiryDate = DateTime.Now;
        }
    }

    public enum EnquirySource
    {
        MobileApp = 1,
        Web = 2
    }

    public enum EnquiryStatus
    {
        New = 1,
        InProcess = 2,
        Assigned = 3,
        Completed = 4
    }

}
