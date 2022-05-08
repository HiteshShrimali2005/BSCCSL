using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class Messages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SMSId { get; set; }

        [StringLength(200)]
        [Column(TypeName = "varchar")]
        public string Message { get; set; }
        public MessageType Name { get; set; }

        public Type Type { get; set; }
    }

}


public enum Type
{
    SMS = 1,
    TRANDESC = 2
}

public enum MessageType
{
    CASH_DEPOSIT = 1,
    CHEQUE_DEPOSIT = 2,
    INTEREST_DEPOSIT = 3,
    MATURITY_DEPOSIT = 4,
    CASH_WITHDRAW = 5,
    CHEQUE_WITHDRAW = 6,
    CHEQUE_BOUNCE = 7,
    LATE_PAYMENT_CHARGE = 8,
    SHARE_PURCHASE = 9,
    INSTALLMENTS = 10,
    TRANSFER = 11,
    CUSTOMER_REGISTRATION = 12,
    NEW_ACCOUNT = 13,
    DEPOSIT = 14,
    WITHDRAWAL = 15,
    ADMISSIONFEE = 16,
    PREMATURECHARGES = 17,
    CUSTOMER_REGISTRATION_OTP = 18,
    CUSTOMER_FORGOT_PASSWORD_OTP = 19,
    AGENT_COMMISSION = 20,
    BALANCE_TRANSFER = 21,
    BANK_TRANSFER_CREDIT = 22,
    BANK_TRANSFER_DEBIT = 23,
    LOAN_STATUS = 24,
    INSTALLMENT_REMINDER = 25,
    CHEQUE_BOUNCE_SMS = 26,
    CHEQUE_BOUNCE_PANELTY = 27,
    IMPS_NEFT_Credit = 28,
    IMPS_NEFT_Debit = 29,
    LOAN_PREPAYMENT = 30,

}