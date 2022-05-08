using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    /// <summary>
    /// Login model for Customer App
    /// </summary>
    public class CustomerLogin
    {
        public string ClientId { get; set; }
        public string Password { get; set; }
    }

    public class CustomerRegister
    {
        public string ClientId { get; set; }
        public string MobileNumber { get; set; }
    }

    public class CustomerForgotPassword
    {
        public string ClientId { get; set; }
        public string MobileNumber { get; set; }
    }

    public class CustomerForgotPasswordOTP
    {
        public Guid CustomerId { get; set; }
        public string ClientId { get; set; }
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public string OTP { get; set; }
    }

    public class CustomerRegisterOPT
    {
        public Guid CustomerId { get; set; }
        public string ClientId { get; set; }
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public string OTP { get; set; }
        public string Password { get; set; }
    }

    public class VerifyOTP
    {
        public Guid CustomerId { get; set; }
        public string Token { get; set; }
        public TokenType TokenType { get; set; }
    }

    public class SetPasswordModel
    {
        public Guid CustomerId { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    public class CustomerViewModel
    {
        public Guid CustomerId { get; set; }
        public string ClientId { get; set; }
        public IEnumerable<string> Customers { get; set; }
        public string CustomerName
        {
            get
            {
                return string.Join(", ", Customers);
            }
        }
    }

    public class CustomerAccountModel
    {
        public Guid CustomerProductId { get; set; }
        public string AccountNumber { get; set; }
        public ProductType ProductType { get; set; }
    }

    public class CustomerNotificationModel
    {
        public Guid NotificationId { get; set; }
        public string NotificationText { get; set; }
    }
}
