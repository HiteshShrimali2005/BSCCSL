using BSCCSL.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class DataTableSearch
    {
        public string sEcho { get; set; }
        public string sSearch { get; set; }
        public int iDisplayLength { get; set; }
        public int iDisplayStart { get; set; }
        public int iColumns { get; set; }
        public int iSortingCols { get; set; }
        public string sColumns { get; set; }
        public int iSortCol_0 { get; set; }
        public string sSortDir_0 { get; set; }
        public string iSortDir { get; set; }
        public Guid? id { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public bool? Status { get; set; }
        public TypeCRDR Type { get; set; }
        public Guid? BranchId { get; set; }
        public string FirstName { get; set; }
        public string AccountNumber { get; set; }
        public string role { get; set; }
        public Guid? GroupLoanId { get; set; }
        public string AgentName { get; set; }
        public string EmployeeName { get; set; }
        public string ProductName { get; set; }
        public int? Maturity { get; set; }
        public decimal? InterestRate { get; set; }
        public string FinYear { get; set; }
        public string PrematurePercentage { get; set; }
        public string CustomerName { get; set; }
        public LoanStatus? LoanStatus { get; set; }

        public string RootAccount { get; set; }
        public string ParentAccount { get; set; }
        public int AccountType { get; set; }
        public string ChartofAccountName { get; set; }


        public int EntryType { get; set; }
        public DateTime? FromPostingDate { get; set; }
        public DateTime? ToPostingDate { get; set; }

        public Guid? AccountId { get; set; }

        public int? ProductStatus { get; set; }
        public int? PaymentType { get; set; }
        public int? DayScrollStatsType { get; set; }
        public string ConsumerProductName { get; set; }
    }

    public class DataTable<T>
    {
        public List<T> data { get; set; }
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
    }

    #region Search Models

    public abstract class ReportDataTableSearch
    {
        public string draw { get; set; }
        public int length { get; set; }
        public int start { get; set; }
        public string Search { get; set; }
       
    }

    public class ReportSearch : ReportDataTableSearch
    {
        public Guid? BranchId { get; set; }
        public int? Month { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public string AgentName { get; set; }
        public string ProductName { get; set; }
        public int? Year { get; set; }
        public int? UserStatus { get; set; }
    }
    #endregion
}
