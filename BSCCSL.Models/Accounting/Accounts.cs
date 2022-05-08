using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models.Accounting
{
    public class Accounts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public AccountType? AccountType { get; set; }
        public bool IsDelete { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsPermanent { get; set; }


        public Guid BranchId { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        public virtual string AccountTypeName
        {
            get
            {
                return AccountType.ToString().Replace("_", " ");
            }
        }
        public virtual string ParentAccountName
        {
            get
            {
                return ParentId.ToString().Replace("_", " ");
            }
        }


    }

    public enum AccountType
    {
        Accumulated = 1,
        Depreciation = 2,
        Bank = 3,
        Cash = 4,
        Chargeable = 5,
        Cost_of_Goods_Sold = 6,
        Equity = 7,
        Expense_Account = 8,
        Expenses_Included_In_Valuation = 9,
        Fixed_Asset = 10,
        Income_Account = 11,
        Payable = 12,
        Receivable = 13,
        Round_Off = 14,
        Stock = 15,
        Stock_Adjustment = 16,
        Stock_Received_But_Not_Billed = 17,
        Tax = 18,
        Temporary = 19,
    }


    public class AccountingEntries
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Particular { get; set; }
        public decimal Amount { get; set; }
        public Guid BranchId { get; set; }
        public AccountingEntryType AccountingEntryType { get; set; }

    }

    public enum AccountingEntryType
    {
        Saving_Account_Cash_WithDraw = 1,
        Saving_Account_Cash_Deposite = 2,
        Saving_Account_Share_Amount_Deduction = 3,
        Saving_Account_Admission_Fee_Deduction = 4,
        Saving_Account_Installment_Deduction = 5,
        Saving_Account_Late_Payment_Charges_Deduction = 6,
        Saving_Account_Cheque_WithDraw = 7,
        Saving_Account_Cheque_Deposite = 8,
        Saving_Account_Bank_Transfer_WithDraw = 9,
        Saving_Account_Bank_Transfer__Deposite = 10,
        Saving_Account_Balance_Transfer__Deposite = 11,
        Saving_Account_Maturity_Amount__Deposite = 12,
        Saving_Account_Interest__Deposite = 13,
        Saving_Account_Agent_Commission__Deposite = 14,
        Super_Saving_Account_commission = 15,
        RD_Account_Installment_Credited = 16,
        RD_Interest = 17,
        RD_Account_Maturity_Deduction =18,
        Loan_Account_Installment_Credited = 20,
        Loan_Account_Prepayment_Amount_Credited = 21,
        Loan_Interest = 22,
        Flexi_Loan_Interest = 31,
        FD_Installment_Credited = 23,
        FD_Interest = 24,
        FD_Maturity_Amount_Debited = 25,
        MIS_Installment_Credited = 26,
        MIS_Interest = 34,
        MIS_Maturity_Amount_Debited = 27,
        RIP_Installment_Credited = 28,
        Dhan_Vruddhi_Installment_Credited = 29,
        Dhan_Vruddhi_Interest_Credited = 30,
        Prematured_Charges = 32,
        Processing_Charges= 33,
        ALL_Expenses = 35,
        Loan_Disbursement = 36
         



    }

}
