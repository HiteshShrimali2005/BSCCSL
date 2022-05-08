using BSCCSL.Models;
using BSCCSL.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static BSCCSL.Models.Accounting.ProfitandLossStatementViewModel;

namespace BSCCSL.Services
{
    public class CashFlowService
    {
        public object GetCashFlowData(DateTime FromDate, DateTime ToDate, Guid BranchId, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetCashFlowData", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@StartDate", SqlDbType.DateTime);
                paramFromDate.Value = (object)FromDate ?? DBNull.Value;

                SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@EndDate", SqlDbType.DateTime);
                paramToDate.Value = (object)ToDate ?? DBNull.Value;

                var IsHO = db.Branch.Where(c => c.BranchId == BranchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = BranchId;
                }

                //Execute the query
                sql.Open();

                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<CashFlowViewModel> objCashFlowViewModel = ((IObjectContextAdapter)db).ObjectContext.Translate<CashFlowViewModel>(reader).ToList();
                reader.NextResult();

                sql.Close();

                string StartDateString = Convert.ToDateTime("2017-04-21").ToString("yyyy-MM-dd");
                DateTime StartDate = Convert.ToDateTime(StartDateString);

                #region Get Previous Days data to calculate opening Balance
                SqlConnection sql1 = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet1 = new SqlCommand("GetCashFlowData", sql1);
                cmdTimesheet1.CommandType = CommandType.StoredProcedure;

                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet1.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet1.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = BranchId;
                }
                if (string.IsNullOrEmpty(FromDate.ToString()))
                {
                    SqlParameter paramfinYear = cmdTimesheet1.Parameters.Add("@StartDate", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;

                    SqlParameter paramenddate = cmdTimesheet1.Parameters.Add("@EndDate", SqlDbType.VarChar);
                    paramenddate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet1.Parameters.Add("@StartDate", SqlDbType.VarChar);
                    paramfinYear.Value = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd");

                    SqlParameter paramenddate = cmdTimesheet1.Parameters.Add("@EndDate", SqlDbType.VarChar);
                    paramenddate.Value = Convert.ToDateTime(FromDate).AddDays(-1).ToString("yyyy-MM-dd");
                }
                sql1.Open();
                var reader1 = cmdTimesheet1.ExecuteReader();
                List<CashFlowViewModel> rptlist2 = ((IObjectContextAdapter)db).ObjectContext.Translate<CashFlowViewModel>(reader1).ToList();
                //rptlist2 = rptlist2.GroupBy(x => x.AccountNumber).Select(y => y.First()).ToList();

                var TotalCredittilldate = rptlist2.Sum(x => x.Credit);
                var TotalDebittilldate = rptlist2.Sum(x => x.Debit);
                sql1.Close();
                #endregion


                var NewOpeningBalance = 0.00;


                var TotalCredit = objCashFlowViewModel.Sum(x => x.Credit);
                var TotalDebit = objCashFlowViewModel.Sum(x => x.Debit);
                var FirstDate = Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy");
                var LastDate = Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy");
                if (string.IsNullOrEmpty(ToDate.ToString()))
                {
                    if (!string.IsNullOrEmpty(FromDate.ToString()))
                    {
                        LastDate = DateTime.Now.ToString("dd-MMM-yyyy");
                    }
                }

                decimal OpeningBalance = 0;
                decimal ClosingBalance = 0;

                string Closingbalancestring = "";
                string Openingbalancestring = "";

                //ClosingBalance = (OpeningBalance + Convert.ToDecimal(TotalCredit)) - Convert.ToDecimal(TotalDebit);
                ClosingBalance = (OpeningBalance + Convert.ToDecimal(TotalDebit)) - Convert.ToDecimal(TotalCredit);
                if (StartDate == FromDate)
                    NewOpeningBalance = 0.00;



                //OpeningBalance = (Convert.ToDecimal(NewOpeningBalance) + Convert.ToDecimal(TotalCredittilldate)) - Convert.ToDecimal(TotalDebittilldate);
                OpeningBalance = (Convert.ToDecimal(NewOpeningBalance) + Convert.ToDecimal(TotalDebittilldate)) - Convert.ToDecimal(TotalCredittilldate);

                if (OpeningBalance < 0)
                    Openingbalancestring = " " + OpeningBalance.ToString().Split('-')[1] + " DR";
                else if (OpeningBalance == 0)
                    Openingbalancestring = "0.00";
                else
                    Openingbalancestring = " " + OpeningBalance.ToString() + " CR";


                if (FromDate != StartDate)
                {
                    if (OpeningBalance < 0)
                    {
                        //ClosingBalance = (Convert.ToDecimal(TotalCredit)) - (Convert.ToDecimal(TotalDebit) - (OpeningBalance));
                        ClosingBalance = (Convert.ToDecimal(TotalDebit)) - (Convert.ToDecimal(TotalCredit) - (OpeningBalance));
                    }
                    else
                    {
                        //ClosingBalance = (Convert.ToDecimal(TotalCredit) + OpeningBalance) - (Convert.ToDecimal(TotalDebit));
                        ClosingBalance = (Convert.ToDecimal(TotalDebit) + OpeningBalance) - (Convert.ToDecimal(TotalCredit));
                    }
                }

                decimal Balance = OpeningBalance;
                foreach (var item in objCashFlowViewModel)
                {
                    if (item.Debit != 0)
                        Balance = Balance + Convert.ToDecimal(item.Debit);
                    if (item.Credit != 0)
                        Balance = Balance - Convert.ToDecimal(item.Credit);

                    item.Balance = Balance;
                }



                if (ClosingBalance < 0)
                    Closingbalancestring = " " + ClosingBalance.ToString().Split('-')[1] + " DR";
                else if (ClosingBalance == 0)
                    Closingbalancestring = "0.00";
                else
                    Closingbalancestring = " " + ClosingBalance.ToString() + " CR";


                if (FromDate < StartDate)
                {
                    Openingbalancestring = "0.00";
                    Closingbalancestring = "0.00";
                }
                else if (FromDate == StartDate)
                    Openingbalancestring = "0.00";


                var data = new
                {
                    CashFlowData = objCashFlowViewModel,
                    TotalCreditAmount = TotalCredit,
                    TotalDebitAmount = TotalDebit,
                    OpeningBalance = Openingbalancestring,
                    ClosingBalance = Closingbalancestring

                };

                //sql.Close();
                db.Dispose();
                return data;
            }
        }
    }
}
