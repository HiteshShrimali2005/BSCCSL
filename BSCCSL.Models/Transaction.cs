using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TransactionId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public TypeCRDR Type { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? BranchId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CustomerProductId { get; set; }
        public Guid? RefCustomerProductId { get; set; }
        public TransactionType TransactionType { get; set; }
        public long? CheckNumber { get; set; }
        public DateTime? ChequeDate { get; set; }
        public int? BounceReason { get; set; }
        public decimal? Penalty { get; set; }
        public DateTime? ChequeClearDate { get; set; }
        public Status Status { get; set; }
        //  public decimal? UnclearBalance { get; set;}
        public DateTime? TransactionTime { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
        public DescIndentify? DescIndentify { get; set; }
        public Guid? AccountsHeadId { get; set; }
        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        [ForeignKey("RefCustomerProductId")]
        public CustomerProduct RefCustomerProduct { get; set; }
        [ForeignKey("CustomerProductId")]
        public CustomerProduct CustomerProduct { get; set; }
        [ForeignKey("AccountsHeadId")]
        public AccountsHead AccountsHead { get; set; }

        [NotMapped]
        public string TempThirdPartyAccNo { get; set; }

        public Transactions()
        {
            CreatedDate = DateTime.Now;
        }


        public virtual string TypeName
        {
            get
            {
                return Type.ToString();
            }
        }
        public long? NEFTNumber { get; set; }
        public DateTime? NEFTDate { get; set; }

    }

    public enum TypeCRDR
    {
        CR = 1,
        DR = 2
    }

    public enum TransactionType
    {
        Cash = 1,
        DD = 2,
        Cheque = 3,
        BankTransfer = 4,
        BalanceTransfer = 5,
        IMPS_NEFT = 6,
        NEFT = 7,
        Expense = 8,
        JV = 9,
        DDSAmountTransfer = 10
    }


    public enum Status
    {
        Clear = 0,
        Unclear = 1
    }

    public enum DescIndentify
    {
        Interest = 1,
        Maturity = 2,
        Panelty = 3,
        LatePaymentCharges = 4,
        Installment = 5,
        MaturityInterest = 6,
        Share = 7,
        Transfer = 8,
        AdmissionFee = 9,
        Cash_Deposit = 10,
        Cheque_Deposit = 11,
        Cash_Withdraw = 12,
        Cheque_Withdraw = 13,
        Premature_Charges = 14,
        Agent_Commission = 15,
        Balance_Transfer = 16,
        Bank_Transfer_Credit = 17,
        Bank_Transfer_Debit = 18,
        Cheque_Bounce = 19,
        IMPS_NEFT_Credit = 20,
        IMPS_NEFT_Debit = 21,
        Loan_PrePayment = 22,
        Expense = 23,
        JV = 32,
        Loan_disbursement = 33,
        Loan_Charges=34,
        Loan_Interest_Amount = 35,
        DDSAmountTransfer = 36,
        Super_Saving_Commission = 37,
        Smart_Saving_Plus_Commission = 39,
    }

    public class UnclearCheck
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerProdctId { get; set; }
        public string AccountNo { get; set; }
        public ProductType ProductType { get; set; }
        public decimal? Balance { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal ChequeAmount { get; set; }
        public long? ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string BankName { get; set; }
        public Guid TransactionId { get; set; }
        public string ProductTypeName { get; set; }
        public DateTime? ChequeClearDate { get; set; }
        public List<string> Name { get; set; }

        public virtual string ProductName
        {
            get
            {
                return ProductType.ToString();
            }
        }
    }

    public class PassbookPrintSearch
    {
        public string AccountNo { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class TransactionCustomer
    {
        public Guid? RefCustomerProductId { get; set; }
        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        public TypeCRDR Type { get; set; }
        public TransactionType TransactionType { get; set; }
        public string MobNo { get; set; }
        public ProductType? RefProductType { get; set; }
        public DescIndentify? DescIndentify { get; set; }
        public Status? Status { get; set; }
        public ProductType ProductType { get; set; }

        public decimal Balance { get; set; }
        public long? CheckNumber { get; set; }
    }


    public class PrintStatement
    {
        public string AccountNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TypeCRDR? Type { get; set; }
    }

}
