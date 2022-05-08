using BSCCSL.Models;
using BSCCSL.Models.Accounting;
using System;
using System.Collections;
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
using static BSCCSL.Models.Accounting.TrialBalanceViewModel;

namespace BSCCSL.Services
{
    public class TrailBalanceService
    {

        public object GetTrialBalanceListforParentAccount(ReportSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetParentAccountsTrialBalance", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                //SqlParameter paramAgentId = cmdTimesheet.Parameters.Add("@AgentId", SqlDbType.UniqueIdentifier);
                //paramAgentId.Value = Search.UserId;

                var IsHO = db.Branch.Where(c => c.BranchId == Search.BranchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = Search.BranchId;
                }


                SqlParameter paramStart = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                paramStart.Value = Search.start;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.length;

                SqlParameter paramstartdate = cmdTimesheet.Parameters.Add("@StartDate", SqlDbType.DateTime);
                paramstartdate.Value = Search.fromDate;

                SqlParameter paramenddate = cmdTimesheet.Parameters.Add("@EndDate", SqlDbType.DateTime);
                paramenddate.Value = Search.toDate;

                DateTime LastYearsDate = Search.fromDate.Value.AddYears(-1);
                int SelectedfromYear = LastYearsDate.Year;
                int SelectedtoYear = Search.fromDate.Value.Year;

                string LastFInancialYear = "";
                LastFInancialYear = SelectedfromYear.ToString() + '-' + SelectedtoYear.ToString();


                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<ParentAccountTrialBalanceViewModel> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<ParentAccountTrialBalanceViewModel>(reader).ToList();
                reader.NextResult();



                foreach (var item in rptlist)
                {
                    FinancialYearClosingBalance objFinancialYearClosingBalance = db.FinancialYearClosingBalance.Where(x => x.FinancialYear == LastFInancialYear && x.BranchId == Search.BranchId && x.AccountId == item.AccountId).FirstOrDefault();
                    if (objFinancialYearClosingBalance != null)
                    {
                        item.OpeningDR = objFinancialYearClosingBalance.ClosingBalanceDR;
                        item.OpeningCR = objFinancialYearClosingBalance.ClosingBalanceCR;
                    }

                    decimal closingBalance = 0;


                    closingBalance = (item.OpeningDR - item.OpeningCR) + (item.Debit) - (item.Credit);

                    if (closingBalance > 0)
                    {
                        item.ClosingDR = closingBalance;
                        item.ClosingCR = 0;
                    }
                    else
                    {
                        item.ClosingCR = closingBalance * -1;
                        item.ClosingDR = 0;
                    }

                }
                decimal TotalCreditAmount = 0;
                decimal TotalDebitAmount = 0;

                TotalCreditAmount = rptlist.Select(x => x.ClosingCR).Sum();
                TotalDebitAmount = rptlist.Select(x => x.ClosingDR).Sum();


                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    data = rptlist,
                    draw = Search.draw,
                    recordsFiltered = Count,
                    recordsTotal = rptlist.Count(),
                    TotalDebitAmount = TotalDebitAmount,
                    TotalCreditAmount = TotalCreditAmount
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }


        public object GetTrialBalanceListforSubAccount(AccountDetails Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetSubAccountsTrialBalance", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                SqlParameter paramAgentId = cmdTimesheet.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier);
                paramAgentId.Value = Search.AccountId;

                var IsHO = db.Branch.Where(c => c.BranchId == Search.BranchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = Search.BranchId;
                }

                SqlParameter paramstartdate = cmdTimesheet.Parameters.Add("@StartDate", SqlDbType.DateTime);
                paramstartdate.Value = Search.fromDate;

                SqlParameter paramenddate = cmdTimesheet.Parameters.Add("@EndDate", SqlDbType.DateTime);
                paramenddate.Value = Search.toDate;

                DateTime LastYearsDate = Search.fromDate.Value.AddYears(-1);
                int SelectedfromYear = LastYearsDate.Year;
                int SelectedtoYear = Search.fromDate.Value.Year;

                string LastFInancialYear = "";
                LastFInancialYear = SelectedfromYear.ToString() + '-' + SelectedtoYear.ToString();


                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<SubAccountTrialBalanceViewModel> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<SubAccountTrialBalanceViewModel>(reader).ToList();


                foreach (var item in rptlist)
                {

                    FinancialYearClosingBalance objFinancialYearClosingBalance = db.FinancialYearClosingBalance.Where(x => x.FinancialYear == LastFInancialYear && x.BranchId == Search.BranchId && x.AccountId == item.SubAccountId).FirstOrDefault();
                    if (objFinancialYearClosingBalance != null)
                    {
                        item.OpeningDR = objFinancialYearClosingBalance.ClosingBalanceDR;
                        item.OpeningCR = objFinancialYearClosingBalance.ClosingBalanceCR;
                    }

                    decimal closingBalance = 0;

                    closingBalance = (item.OpeningDR - item.OpeningCR) + (item.Debit) - (item.Credit);

                    if (closingBalance > 0)
                    {
                        item.ClosingDR = closingBalance;
                        item.ClosingCR = 0;
                    }
                    else
                    {
                        item.ClosingCR = closingBalance * -1;
                        item.ClosingDR = 0;
                    }
                }


                sql.Close();
                db.Dispose();
                return rptlist;
            }
        }


        public object GetTrialBalanceListforChildAccount(AccountDetails Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetChildAccountsTrialBalance", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                SqlParameter paramAgentId = cmdTimesheet.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier);
                paramAgentId.Value = Search.AccountId;

                var IsHO = db.Branch.Where(c => c.BranchId == Search.BranchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = Search.BranchId;
                }

                SqlParameter paramstartdate = cmdTimesheet.Parameters.Add("@StartDate", SqlDbType.DateTime);
                paramstartdate.Value = Search.fromDate;

                SqlParameter paramenddate = cmdTimesheet.Parameters.Add("@EndDate", SqlDbType.DateTime);
                paramenddate.Value = Search.toDate;


                DateTime LastYearsDate = Search.fromDate.Value.AddYears(-1);
                int SelectedfromYear = LastYearsDate.Year;
                int SelectedtoYear = Search.fromDate.Value.Year;

                string LastFInancialYear = "";
                LastFInancialYear = SelectedfromYear.ToString() + '-' + SelectedtoYear.ToString();

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<ChildAccountTrialBalanceViewModel> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<ChildAccountTrialBalanceViewModel>(reader).ToList();
                foreach (var item in rptlist)
                {
                    FinancialYearClosingBalance objFinancialYearClosingBalance = db.FinancialYearClosingBalance.Where(x => x.FinancialYear == LastFInancialYear && x.BranchId == Search.BranchId && x.AccountId == item.ChildAccountId).FirstOrDefault();
                    if (objFinancialYearClosingBalance != null)
                    {
                        item.OpeningDR = objFinancialYearClosingBalance.ClosingBalanceDR;
                        item.OpeningCR = objFinancialYearClosingBalance.ClosingBalanceCR;
                    }

                    decimal closingBalance = 0;


                    closingBalance = (item.OpeningDR - item.OpeningCR) + (item.Debit) - (item.Credit);

                    if (closingBalance > 0)
                    {
                        item.ClosingDR = closingBalance;
                        item.ClosingCR = 0;
                    }
                    else
                    {
                        item.ClosingCR = closingBalance * -1;
                        item.ClosingDR = 0;
                    }
                }

                sql.Close();
                db.Dispose();
                return rptlist;
            }
        }


        public int SaveFinancialYearClosingBalance(FinancialYearViewModel data, User user)
        {
            int Count = 0;

            try
            {
                using (var db = new BSCCSLEntity())
                {
                    if (db.FinancialYearClosingBalance.Where(x => x.FinancialYear == data.FinancialYear && x.BranchId == data.BranchId).Any())
                    {
                        List<FinancialYearClosingBalance> objFinancialYearClosingBalance = db.FinancialYearClosingBalance.Where(x => x.FinancialYear == data.FinancialYear).ToList();
                        db.FinancialYearClosingBalance.RemoveRange(objFinancialYearClosingBalance);
                        db.SaveChanges();
                    }

                    List<FinancialYearClosingBalance> objFinancialYearClosingBalanceList = new List<FinancialYearClosingBalance>();

                    List<SubAccountTrialBalanceViewModel> GlobalobjSubAccountTrialBalanceViewModelList = new List<SubAccountTrialBalanceViewModel>();
                    List<ChildAccountTrialBalanceViewModel> GlobalobjChildAccountTrialBalanceViewModelList = new List<ChildAccountTrialBalanceViewModel>();

                    foreach (var item in data.ParentAccountTrialBalanceViewModel)
                    {
                        FinancialYearClosingBalance objFinancialYearClosingBalance = new FinancialYearClosingBalance();
                        objFinancialYearClosingBalance.FinancialYear = data.FinancialYear;
                        objFinancialYearClosingBalance.AccountId = item.AccountId;
                        objFinancialYearClosingBalance.ClosingBalanceCR = item.ClosingCR;
                        objFinancialYearClosingBalance.ClosingBalanceDR = item.ClosingDR;
                        objFinancialYearClosingBalance.BranchId = data.BranchId;
                        objFinancialYearClosingBalance.CreatedBy = user.UserId;
                        objFinancialYearClosingBalance.CreatedDate = DateTime.Now;
                        objFinancialYearClosingBalanceList.Add(objFinancialYearClosingBalance);

                        AccountDetails objAccountDetail = new AccountDetails();
                        objAccountDetail.AccountId = item.AccountId;
                        objAccountDetail.BranchId = data.BranchId;
                        objAccountDetail.fromDate = data.FromDate;
                        objAccountDetail.toDate = data.ToDate;

                        var SubData = GetTrialBalanceListforSubAccount(objAccountDetail, user);

                        List<SubAccountTrialBalanceViewModel> objSubAccountTrialBalanceViewModelListnew = ((List<SubAccountTrialBalanceViewModel>)SubData);
                        GlobalobjSubAccountTrialBalanceViewModelList.AddRange(objSubAccountTrialBalanceViewModelListnew);
                    }

                    foreach (var item in GlobalobjSubAccountTrialBalanceViewModelList)
                    {
                        FinancialYearClosingBalance objFinancialYearClosingBalance = new FinancialYearClosingBalance();
                        objFinancialYearClosingBalance.FinancialYear = data.FinancialYear;
                        objFinancialYearClosingBalance.AccountId = item.SubAccountId;
                        objFinancialYearClosingBalance.ClosingBalanceCR = item.ClosingCR;
                        objFinancialYearClosingBalance.ClosingBalanceDR = item.ClosingDR;
                        objFinancialYearClosingBalance.BranchId = data.BranchId;
                        objFinancialYearClosingBalance.CreatedBy = user.UserId;
                        objFinancialYearClosingBalance.CreatedDate = DateTime.Now;
                        objFinancialYearClosingBalanceList.Add(objFinancialYearClosingBalance);


                        AccountDetails objAccountDetail = new AccountDetails();
                        objAccountDetail.AccountId = item.SubAccountId;
                        objAccountDetail.BranchId = data.BranchId;
                        objAccountDetail.fromDate = data.FromDate;
                        objAccountDetail.toDate = data.ToDate;

                        var ChildData = GetTrialBalanceListforChildAccount(objAccountDetail, user);

                        List<ChildAccountTrialBalanceViewModel> objChildAccountTrialBalanceViewModelListnew = ((List<ChildAccountTrialBalanceViewModel>)ChildData);
                        GlobalobjChildAccountTrialBalanceViewModelList.AddRange(objChildAccountTrialBalanceViewModelListnew);

                    }

                    foreach (var item in GlobalobjChildAccountTrialBalanceViewModelList)
                    {
                        FinancialYearClosingBalance objFinancialYearClosingBalance = new FinancialYearClosingBalance();
                        objFinancialYearClosingBalance.FinancialYear = data.FinancialYear;
                        objFinancialYearClosingBalance.AccountId = item.ChildAccountId;
                        objFinancialYearClosingBalance.ClosingBalanceCR = item.ClosingCR;
                        objFinancialYearClosingBalance.ClosingBalanceDR = item.ClosingDR;
                        objFinancialYearClosingBalance.BranchId = data.BranchId;
                        objFinancialYearClosingBalance.CreatedBy = user.UserId;
                        objFinancialYearClosingBalance.CreatedDate = DateTime.Now;
                        objFinancialYearClosingBalanceList.Add(objFinancialYearClosingBalance);
                    }

                    if (objFinancialYearClosingBalanceList.Count > 0)
                    {
                        db.FinancialYearClosingBalance.AddRange(objFinancialYearClosingBalanceList);
                        db.SaveChanges();
                    }


                    Count = 1;
                }
            }
            catch (Exception ex)
            {

            }
            return Count;
        }
    }
}
