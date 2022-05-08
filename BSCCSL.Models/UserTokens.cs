using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class UserTokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TokenId { get; set; }

        public Guid UserId { get; set; }

        public TokenType TokenType { get; set; }

        public string Token { get; set; }

        public DateTime ExpireTime { get; set; }

        public bool IsUsed { get; set; }

        public bool IsExpired { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public enum TokenType
    {
        Customer_Register = 1,
        Customer_Forgot_Password = 2
    }
}
