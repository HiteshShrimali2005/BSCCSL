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
using static BSCCSL.Models.Accounting.BalanceSheetViewModel;
using static BSCCSL.Models.Accounting.ProfitandLossStatementViewModel;

namespace BSCCSL.Services
{
    public class BalanceSheetService
    {
        public object GetBalanceSheetData(DateTime FromDate, DateTime ToDate, Guid BranchId, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("BalanceSheetforParentAccount", sql);
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
                List<BalanceSheetforParentLiabilities> objBalanceSheetforParentLiabilitiesList = ((IObjectContextAdapter)db).ObjectContext.Translate<BalanceSheetforParentLiabilities>(reader).ToList();
                reader.NextResult();

                List<BalanceSheetforParentAssests> objBalanceSheetforParentAssestsList = ((IObjectContextAdapter)db).ObjectContext.Translate<BalanceSheetforParentAssests>(reader).ToList();

                decimal FinalLiabilitiesTotalAmount = 0;
                decimal FinalAssestsTotalAmount = 0;

                string FinalLiabilitiesTotalAmountstring = "";
                string FinalAssestsTotalAmountstring = "";
                foreach (var item in objBalanceSheetforParentLiabilitiesList)
                {
                    item.TotalAmount = item.Debit - item.Credit;
                    if (item.TotalAmount > 0)
                        item.TotalAmountstring = item.TotalAmount.ToString() + " DR";
                    else
                        item.TotalAmountstring = (item.TotalAmount * -1).ToString() + " CR";

                    FinalLiabilitiesTotalAmount = FinalLiabilitiesTotalAmount + item.TotalAmount;

                    if (item.ListBalanceSheetforChildLiabilities == null)
                        item.ListBalanceSheetforChildLiabilities = new List<BalanceSheetforChildLiabilities>();

                    #region Get Sub Account Data
                    SqlConnection sql1 = new SqlConnection(connectionstring);
                    SqlCommand cmdTimesheet1 = new SqlCommand("BalanceSheetforChildAccount", sql1);
                    cmdTimesheet1.CommandType = CommandType.StoredProcedure;


                    SqlParameter paramAgentId = cmdTimesheet1.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier);
                    paramAgentId.Value = item.AccountId;

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
                    SqlParameter paramFromDate1 = cmdTimesheet1.Parameters.Add("@StartDate", SqlDbType.DateTime);
                    paramFromDate1.Value = (object)FromDate ?? DBNull.Value;

                    SqlParameter paramToDate1 = cmdTimesheet1.Parameters.Add("@EndDate", SqlDbType.DateTime);
                    paramToDate1.Value = (object)ToDate ?? DBNull.Value;


                    sql1.Open();
                    var reader1 = cmdTimesheet1.ExecuteReader();
                    List<BalanceSheetforChildLiabilities> rptlist2 = ((IObjectContextAdapter)db).ObjectContext.Translate<BalanceSheetforChildLiabilities>(reader1).ToList();

                    foreach (var listitem in rptlist2)
                    {
                        if (listitem.TotalAmount > 0)
                            listitem.TotalAmountstring = listitem.TotalAmount.ToString() + " DR";
                        else
                            listitem.TotalAmountstring = (listitem.TotalAmount * -1).ToString() + " CR";

                    }
                    item.ListBalanceSheetforChildLiabilities = rptlist2;
                    sql1.Close();
                    #endregion

                }


                foreach (var item in objBalanceSheetforParentAssestsList)
                {

                    item.TotalAmount = item.Debit - item.Credit;

                    if (item.TotalAmount > 0)
                        item.TotalAmountstring = item.TotalAmount.ToString() + " DR";
                    else
                        item.TotalAmountstring = (item.TotalAmount * -1).ToString() + " CR";

                    FinalAssestsTotalAmount = FinalAssestsTotalAmount + item.TotalAmount;


                    if (item.ListBalanceSheetforChildAssests == null)
                        item.ListBalanceSheetforChildAssests = new List<BalanceSheetforChildAssests>();

                    #region Get Child Account Data
                    SqlConnection sql1 = new SqlConnection(connectionstring);
                    SqlCommand cmdTimesheet1 = new SqlCommand("BalanceSheetforChildAccount", sql1);
                    cmdTimesheet1.CommandType = CommandType.StoredProcedure;


                    SqlParameter paramAgentId = cmdTimesheet1.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier);
                    paramAgentId.Value = item.AccountId;

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

                    SqlParameter paramFromDate1 = cmdTimesheet1.Parameters.Add("@StartDate", SqlDbType.DateTime);
                    paramFromDate1.Value = (object)FromDate ?? DBNull.Value;

                    SqlParameter paramToDate1 = cmdTimesheet1.Parameters.Add("@EndDate", SqlDbType.DateTime);
                    paramToDate1.Value = (object)ToDate ?? DBNull.Value;

                    sql1.Open();
                    var reader1 = cmdTimesheet1.ExecuteReader();
                    List<BalanceSheetforChildAssests> rptlist2 = ((IObjectContextAdapter)db).ObjectContext.Translate<BalanceSheetforChildAssests>(reader1).ToList();

                    foreach (var listitem in rptlist2)
                    {
                        if (listitem.TotalAmount > 0)
                            listitem.TotalAmountstring = listitem.TotalAmount.ToString() + " DR";
                        else
                            listitem.TotalAmountstring = (listitem.TotalAmount * -1).ToString() + " CR";

                    }
                    item.ListBalanceSheetforChildAssests = rptlist2;
                    sql1.Close();
                    #endregion

                }

                DateTime LastYearsDate = FromDate.AddYears(-1);
                int SelectedfromYear = LastYearsDate.Year;
                int SelectedtoYear = FromDate.Year;

                string LastFInancialYear = "";
                LastFInancialYear = SelectedfromYear.ToString() + '-' + SelectedtoYear.ToString();

                bool isLoss = false;
                bool isProfit = false;
                decimal PandLTotalAmount = 0;
                string PandLTotalAmountstring = "";
                decimal PandLOpeningBalance = 0;
                string PandLCurrentPeriodBalancestring = "0.00";
                string PandLOpeningBalancestring = "0.00";
                #region Get Last Financial Years Profit and Lost Reports Opening Balance

                PandLFinancialYearClosingBalance objPandLFinancialYearClosingBalance = db.PandLFinancialYearClosingBalance.Where(x => x.BranchId == BranchId && x.FinancialYear == LastFInancialYear).FirstOrDefault();
                if (objPandLFinancialYearClosingBalance != null)
                {
                    if (objPandLFinancialYearClosingBalance.ClosingBalanceCR != 0)
                    {
                        isLoss = true;
                        PandLOpeningBalance = objPandLFinancialYearClosingBalance.ClosingBalanceCR * -1;
                        PandLOpeningBalancestring = (PandLOpeningBalance * -1).ToString() + " CR";
                    }
                    else
                    {
                        isProfit = true;
                        PandLOpeningBalance = objPandLFinancialYearClosingBalance.ClosingBalanceDR;
                        PandLOpeningBalancestring = PandLOpeningBalance.ToString() + " DR";

                    }
                }

                #endregion

                #region Get Current Financial Years Profit and Loss

                string Amount = GetProfitandLossStatement(FromDate, ToDate, BranchId, user);
                PandLCurrentPeriodBalancestring = Amount;
                if (Amount.Contains("CR"))
                {
                    isLoss = true;
                    isProfit = false;
                }
                else
                {
                    isProfit = true;
                    isLoss = false;
                }

                #endregion

                decimal PandLCurrentPeriodBalance = 0;
                PandLCurrentPeriodBalance = Convert.ToDecimal(Amount.Split(' ')[0]);


                PandLTotalAmount = PandLOpeningBalance - PandLCurrentPeriodBalance;

                if (PandLTotalAmount < 0)
                    PandLTotalAmount = PandLTotalAmount * -1;

                if (isLoss)
                {
                    FinalAssestsTotalAmount = FinalAssestsTotalAmount - (PandLTotalAmount);
                    if (FinalAssestsTotalAmount < 0)
                        PandLTotalAmountstring = PandLTotalAmount.ToString() + " CR";
                    else
                        PandLTotalAmountstring = PandLTotalAmount.ToString() + " DR";
                }
                if (isProfit)
                {
                    FinalLiabilitiesTotalAmount = FinalLiabilitiesTotalAmount + (PandLTotalAmount);
                    if (FinalLiabilitiesTotalAmount < 0)
                        PandLTotalAmountstring = PandLTotalAmount.ToString() + " CR";
                    else
                        PandLTotalAmountstring = PandLTotalAmount.ToString() + " DR";
                }



                if (FinalLiabilitiesTotalAmount > 0)
                    FinalLiabilitiesTotalAmountstring = FinalLiabilitiesTotalAmount.ToString() + " DR";
                else
                    FinalLiabilitiesTotalAmountstring = (FinalLiabilitiesTotalAmount * -1).ToString() + " CR";

                if (FinalAssestsTotalAmount > 0)
                    FinalAssestsTotalAmountstring = FinalAssestsTotalAmount.ToString() + " DR";
                else
                    FinalAssestsTotalAmountstring = (FinalAssestsTotalAmount * -1).ToString() + " CR";

                if (FinalAssestsTotalAmount < 0)
                    FinalAssestsTotalAmount = (FinalAssestsTotalAmount * -1);
                if (FinalLiabilitiesTotalAmount < 0)
                    FinalLiabilitiesTotalAmount = (FinalLiabilitiesTotalAmount * -1);


                var data = new
                {
                    BalanceSheetforParentLiabilitiesList = objBalanceSheetforParentLiabilitiesList,
                    BalanceSheetforParentAssestsList = objBalanceSheetforParentAssestsList,
                    FinalLiabilitiesTotalAmount = FinalLiabilitiesTotalAmountstring,
                    FinalAssestsTotalAmount = FinalAssestsTotalAmountstring,
                    isLoss = isLoss,
                    isProfit = isProfit,
                    PandLCurrentPeriodBalance = PandLCurrentPeriodBalancestring,
                    PandLTotalAmount = PandLTotalAmountstring,
                    PandLOpeningBalance = PandLOpeningBalancestring
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }


        public string GetProfitandLossStatement(DateTime FromDate, DateTime ToDate, Guid BranchId, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string Amount = "";
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("ProfitandLosssStatementforParentAccount", sql);
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
                List<ProfitandLossStatementforParentExpense> objProfitandLossStatementforParentExpenseList = ((IObjectContextAdapter)db).ObjectContext.Translate<ProfitandLossStatementforParentExpense>(reader).ToList();
                reader.NextResult();

                List<ProfitandLossStatementforParentIncome> objProfitandLossStatementforParentIncomeList = ((IObjectContextAdapter)db).ObjectContext.Translate<ProfitandLossStatementforParentIncome>(reader).ToList();

                decimal FinalExpenseTotalAmount = 0;
                decimal FinalIncomeTotalAmount = 0;

                string FinalExpenseTotalAmountstring = "";
                string FinalIncomeTotalAmountstring = "";
                foreach (var item in objProfitandLossStatementforParentExpenseList)
                {
                    item.TotalAmount = item.Debit - item.Credit;
                    if (item.TotalAmount > 0)
                        item.TotalAmountstring = item.TotalAmount.ToString() + " DR";
                    else
                        item.TotalAmountstring = (item.TotalAmount * -1).ToString() + " CR";

                    FinalExpenseTotalAmount = FinalExpenseTotalAmount + item.TotalAmount;

                    if (item.ListProfitandLossStatementforChildExpense == null)
                        item.ListProfitandLossStatementforChildExpense = new List<ProfitandLossStatementforChildExpense>();
                }


                foreach (var item in objProfitandLossStatementforParentIncomeList)
                {

                    item.TotalAmount = item.Debit - item.Credit;

                    if (item.TotalAmount > 0)
                        item.TotalAmountstring = item.TotalAmount.ToString() + " DR";
                    else
                        item.TotalAmountstring = (item.TotalAmount * -1).ToString() + " CR";

                    FinalIncomeTotalAmount = FinalIncomeTotalAmount + item.TotalAmount;


                    if (item.ListProfitandLossStatementforChildIncome == null)
                        item.ListProfitandLossStatementforChildIncome = new List<ProfitandLossStatementforChildIncome>();
                }

                if (FinalExpenseTotalAmount > 0)
                    FinalExpenseTotalAmountstring = FinalExpenseTotalAmount.ToString() + " DR";
                else
                    FinalExpenseTotalAmountstring = (FinalExpenseTotalAmount * -1).ToString() + " CR";

                if (FinalIncomeTotalAmount > 0)
                    FinalIncomeTotalAmountstring = FinalIncomeTotalAmount.ToString() + " DR";
                else
                    FinalIncomeTotalAmountstring = (FinalIncomeTotalAmount * -1).ToString() + " CR";

                //string NetLossString = "0.00 CR";
                //string GrossProfitString = "0.00 DR";
                //bool isLoss = false;
                //bool isProfit = false;

                if (FinalIncomeTotalAmount < 0)
                    FinalIncomeTotalAmount = (FinalIncomeTotalAmount * -1);
                if (FinalExpenseTotalAmount < 0)
                    FinalExpenseTotalAmount = (FinalExpenseTotalAmount * -1);


                if (FinalExpenseTotalAmount > FinalIncomeTotalAmount)
                {
                    FinalIncomeTotalAmount = FinalExpenseTotalAmount;
                    Amount = (FinalIncomeTotalAmount).ToString() + " CR";
                }
                else
                {
                    FinalExpenseTotalAmount = FinalIncomeTotalAmount;
                    Amount = (FinalExpenseTotalAmount).ToString() + " DR";
                }


                sql.Close();
                db.Dispose();
                return Amount;
            }
        }


    }
}
