using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class LoanDetail
    {

        public decimal? LoanAmount { get; set; }
        public decimal? DisbursementAmount { get; set; }
        public DateTime DateofApplication { get; set; }
        public decimal? LoanIntrestRate { get; set; }
        public string Term { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string BranchPhone { get; set; }
        public string Name { get; set; }
        public DateTime? NomineeDOB { get; set; }
        public string PlaceofBirth { get; set; }
        public decimal? MonthlyInstallmentAmount { get; set; }
        public decimal? TotalCharges { get; set; }
        public decimal? TotalLoanAmount { get; set; }
        public string Reason { get; set; }

    }

    public class HolderDetail
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
    }

    public class ListLoanCharges
    {
        public string Name { get; set; }
        public Decimal Value { get; set; }
    }
}
