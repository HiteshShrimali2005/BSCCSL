using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{

    public class RptProductwiseReportList
    {

        public string ProductName { get; set; }
        public ProductType ProductType { get; set; }
        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }

        public decimal Balance { get; set; }

    }

    public class RptDayScroll
    {
        public Guid CustomerId { get; set; }
        public string ClientId { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime TransactionTime { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public TypeCRDR Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

    }

    public class RptDayScrollList
    {
        public List<RptDayScroll> rptDayScroll { get; set; }
        public int Count { get; set; }
    }

    public class RptAgentCustomers
    {
        public Guid CustomerId { get; set; }
        public string ClienId { get; set; }
        public string CustomerName { get; set; }
        public string AgentName { get; set; }
        public string AgentMobileNo { get; set; }
        public string CustomerAddress { get; set; }
        public string MobileNo { get; set; }

        public string ProductName { get; set; }
        public ProductType ProductType { get; set; }
        public DateTime OpeningDate { get; set; }
        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }

    }

    public class RptEmployeeProductList
    {
        public string EmployeeName { get; set; }
        public string ClienId { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string ProductName { get; set; }
        public ProductType ProductType { get; set; }
        public DateTime OpeningDate { get; set; }
        public string PhoneNumber { get; set; }
        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }

        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }

    public class RptEmployeePerfomanceList
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string ProductName { get; set; }
        public ProductType ProductType { get; set; }
        public int TotalCount { get; set; }
        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }

        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }


    public class RptDueInstallmentList
    {
        public Guid CustomerId { get; set; }
        public string ClienId { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string AgentName { get; set; }
        public string AgentMobileNo { get; set; }
        public string ProductName { get; set; }
        public DateTime? InstallmentDate { get; set; }
        public decimal? PendingInstallment { get; set; }
        public decimal? LatePayment { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public decimal? InterestRate { get; set; }
        public DateTime? OpeningDate { get; set; }
        public ProductType ProductType { get; set; }
        public Frequency PaymentType { get; set; }
        public bool? IsPaid { get; set; }
        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }
        public virtual string PaymentName
        {
            get
            {
                return PaymentType.ToString().Replace("_", "-");
            }
        }
        public int ProductCurrentStatus { get; set; }
    }

    public class RptAgentCommission
    {
        public Guid UserId { get; set; }
        public string UserCode { get; set; }
        public string AgentName { get; set; }
        public string PhoneNumber { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Commission { get; set; }
        public decimal PaidCommission { get; set; }
        public Guid BranchId { get; set; }
        public List<Guid> UserIds { get; set; }
        public List<int> Months { get; set; }
        public List<int> Years { get; set; }
        public string AccountNumber { get; set; }
        public string ProductName { get; set; }
        public bool UserStatus { get; set; }
    }

    public class RptAgentCommissionByMonth
    {
        public Guid CommissionId { get; set; }
        public Guid? RDPaymentId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AgentId { get; set; }
        public string AgentName { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string ProductName { get; set; }
        public ProductType ProductType { get; set; }
        public decimal? Amount { get; set; }
        public decimal? agentCommission { get; set; }
        public DateTime? Date { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool? CommissionPaid { get; set; }
        public string Rank { get; set; }
    }

    public class RptCommissionPayment
    {
        public Guid AgentId { get; set; }
        public Guid CustomerId { get; set; }
        public ProductType ProductType { get; set; }
        public string AccountNumber { get; set; }
        public decimal PaidAmount { get; set; }
        public Guid PaidBy { get; set; }
        public string PaidName { get; set; }
        public string PaidDate { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string AgentName { get; set; }
        public string AgentMobileNo { get; set; }
        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }
    }

    public class CommissionSubdata
    {
        public Guid BranchId { get; set; }
        public List<string> UserIDs { get; set; }
        public List<ProductType> ProductTypes { get; set; }
    }

    public class RptCustomerShares
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string ClientId { get; set; }
        public Maturity Maturity { get; set; }
        public decimal ShareAmount { get; set; }
        public decimal TotalShare { get; set; }
        public virtual string MaturityName
        {
            get
            {
                return Maturity.ToString();
            }
        }
    }

    public class RptAccountsCRDR
    {
        public string BranchName { get; set; }
        public string ProductName { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }
        public decimal? OpeningBalance { get; set; }
    }

    public class RptProfitLossforExpense
    {
        public string BranchName { get; set; }
        public string Particular { get; set; }
        public decimal? Amount { get; set; }
    }


    public class RptProfitLossforInCome
    {
        public string BranchName { get; set; }
        public string Particular { get; set; }
        public decimal? Amount { get; set; }
    }


    public class RptCashBook
    {
        public string Date { get; set; }
        public string Particular { get; set; }
        public string BranchName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }

        public static DateTime? GlobalFromDate { get; set; }
        public static decimal GlobalOpeningBalance { get; set; }

        public decimal Balance { get; set; }

        public string AccountNumber { get; set; }
    }


    public class RptBankBook
    {
        public string Date { get; set; }
        public string Particular { get; set; }
        public string BranchName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }

        public string AccountNumber { get; set; }

    }


    public class RptTrailBalance
    {
        public string Particular { get; set; }
        public string BranchName { get; set; }
        public string Description { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }
    }


    public class RptMaturityList
    {
        public Guid CustomerId { get; set; }
        public string ClienId { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string ProductName { get; set; }
        public decimal? MaturityAmount { get; set; }
        public decimal? Balance { get; set; }
        public DateTime? OpeningDate { get; set; }
        public DateTime? MaturityDate { get; set; }
        public ProductType ProductType { get; set; }
        public Frequency PaymentType { get; set; }
        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }
    }


    public class RptPrematureProductList
    {
        public string ClienId { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime? OpeningDate { get; set; }
        public DateTime? PrematureDate { get; set; }
        public decimal? PrematurePercentage { get; set; }
    }

    public class RptInterestDepositList
    {
        public string ClienId { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public decimal? TotalInterest { get; set; }
        public DateTime? OpeningDate { get; set; }
        public ProductType ProductType { get; set; }
        public virtual string ProductTypeName
        {
            get
            {
                return ProductType.ToString().Replace("_", " ");
            }
        }
    }

    public class RptInterestDepositTotalCount
    {
        public int TotalRecord { get; set; }
    }




    public enum PandLIncome
    {
        Admission_Fee = 1,
        Loan_Interest = 2,
        Prematured_Charges = 3,
        Processing_Charges = 4,
        Loan_Form_Cost = 5,
        Flexi_Loan_Interest = 6,
        Income = 7
    }

    public enum PandLExpense
    {
        FD_Interest = 1,
        Super_Saving_Acc_Comm = 2,
        Saving_Acc_Interest = 3,
        RD_Interest = 4,
        MIS_Interest = 5,
        RIP_Interest = 6,
        Expense = 7,
        Agent_Commission = 8,
        Dhan_Vruddhi_Yojana_Interest = 9 //Dhan Vruddhi Yojana
    }

    public class RptProfitLossDetails
    {
        public string BranchName { get; set; }
        public string Particular { get; set; }
        public decimal? Amount { get; set; }
        public string Date { get; set; }
        public int ProductType { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
    }

    public class RptLoanStatement
    {
        public string ClienId { get; set; }
        public string BranchName { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public decimal? TotalLoanAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? PendingAmount { get; set; }
        public string LoanTypeName { get; set; }
        public string TotalInstallment { get; set; }
        public int TotalPaidInstallment { get; set; }
        public int TotalUnPaidInstallment { get; set; }

        public int TotalPendingInstallment
        {
            get
            {
                int data = 0;
                if (TotalInstallment != null)
                    data = Convert.ToInt32(TotalInstallment) - (TotalPaidInstallment + TotalUnPaidInstallment);
                else
                    data = 0;
                return data;
            }
        }

    }

    public class RptLoanStatementDetails
    {
        public string Particular { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }
        public string Balance { get; set; }
    }


    public enum DayScrollStateType
    {
        Saving_Deposite = 1,
        Saving_Deposite_By_Cash = 2,
        Saving_Deposite_By_Cheque = 3,
        Total_Customers_Deposited_By_Cash=4,
        Total_Customers_Deposited_By_Cheque = 5,
        Total_Current_Deposite = 6,
        Total_Recurring_Deposite = 7,
        Total_Fixed_Deposite = 8,
        Total_Dhan_Vruddhi_Yojana_Deposite = 9,
        Saving_Withdrawals = 10,
        Saving_Withdrawals_By_Cash = 11,
        Saving_Withdrawals_By_Cheque = 12,
        Total_Customers_Withdrawals_By_Cash = 13,
        Total_Customers_Withdrawals_By_Cheque = 14,
        Total_Current_Withdrawals = 15,
    }


    public class RptAccountsCRDRViewModel
    {
        public List<RptAccountsCRDR> Data { get; set; }
        public ExportTotalViewModel ExportData { get; set; }
    }

    public class ExportTotalViewModel
    {
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalOpeningBalance { get; set; }
        public bool IsHo { get; set; }
    }


}


