using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class AgentHierarchy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AgentHierarchyId { get; set; }
        public Guid UserId { get; set; }
        public Guid? EmpAgentId { get; set; }
        public Guid RankId { get; set; }
        public bool IsDelete { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [ForeignKey("UserId")]
        public User user { get; set; }
        [ForeignKey("RankId")]
        public AgentRank AgentRank { get; set; }

        public AgentHierarchy()
        {
            IsDelete = false;
            CreatedDate = DateTime.Now;
        }
    }
}
