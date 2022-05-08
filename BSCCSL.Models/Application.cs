using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class Application
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ApplicationId { get; set; }

        [StringLength(100)]
        public string ApplicationType { get; set; }

        [StringLength(100)]
        public string Platform { get; set; }

        [StringLength(10)]
        public string Version { get; set; }
    }
}