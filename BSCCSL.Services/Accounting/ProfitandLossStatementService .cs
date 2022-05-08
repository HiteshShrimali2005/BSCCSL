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
    public class ProfitandLossStatementService
    {
        public object GetProfitandLossStatement(DateTime FromDate, DateTime ToDate, Guid BranchId, User user)
        {
            using (var db = new BSCCSLEntity())
            {

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
                sql.Close();

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

                    #region Get Sub Account Data
                    SqlConnection sql1 = new SqlConnection(connectionstring);
                    SqlCommand cmdTimesheet1 = new SqlCommand("ProfitandLosssStatementforChildAccount", sql1);
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
                    List<ProfitandLossStatementforChildExpense> rptlist2 = ((IObjectContextAdapter)db).ObjectContext.Translate<ProfitandLossStatementforChildExpense>(reader1).ToList();

                    foreach (var listitem in rptlist2)
                    {
                        if (listitem.TotalAmount > 0)
                            listitem.TotalAmountstring = listitem.TotalAmount.ToString() + " DR";
                        else
                            listitem.TotalAmountstring = (listitem.TotalAmount * -1).ToString() + " CR";

                    }
                    item.ListProfitandLossStatementforChildExpense = rptlist2;
                    sql1.Close();
                    #endregion

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

                    #region Get Child Account Data
                    SqlConnection sql1 = new SqlConnection(connectionstring);
                    SqlCommand cmdTimesheet1 = new SqlCommand("ProfitandLosssStatementforChildAccount", sql1);
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
                    List<ProfitandLossStatementforChildIncome> rptlist2 = ((IObjectContextAdapter)db).ObjectContext.Translate<ProfitandLossStatementforChildIncome>(reader1).ToList();

                    foreach (var listitem in rptlist2)
                    {
                        if (listitem.TotalAmount > 0)
                            listitem.TotalAmountstring = listitem.TotalAmount.ToString() + " DR";
                        else
                            listitem.TotalAmountstring = (listitem.TotalAmount * -1).ToString() + " CR";

                    }
                    item.ListProfitandLossStatementforChildIncome = rptlist2;
                    sql1.Close();

                    #endregion

                }

                if (FinalExpenseTotalAmount > 0)
                    FinalExpenseTotalAmountstring = FinalExpenseTotalAmount.ToString() + " DR";
                else
                {
                    if (FinalExpenseTotalAmount == 0)
                        FinalExpenseTotalAmountstring = (FinalExpenseTotalAmount * -1).ToString() + "0.00 CR";
                    else
                        FinalExpenseTotalAmountstring = (FinalExpenseTotalAmount * -1).ToString() + " CR";
                }

                if (FinalIncomeTotalAmount > 0)
                    FinalIncomeTotalAmountstring = FinalIncomeTotalAmount.ToString() + " DR";
                else
                {
                    if (FinalIncomeTotalAmount == 0)
                        FinalIncomeTotalAmountstring = (FinalIncomeTotalAmount * -1).ToString() + ".00 CR";
                    else
                        FinalIncomeTotalAmountstring = (FinalIncomeTotalAmount * -1).ToString() + " CR";
                }

                string GrossLossString = "0.00 CR";
                string GrossProfitString = "0.00 DR";
                bool isLoss = false;
                bool isProfit = false;

                if (FinalIncomeTotalAmount < 0)
                    FinalIncomeTotalAmount = (FinalIncomeTotalAmount * -1);
                if (FinalExpenseTotalAmount < 0)
                    FinalExpenseTotalAmount = (FinalExpenseTotalAmount * -1);


                if (FinalExpenseTotalAmount > FinalIncomeTotalAmount)
                {
                    decimal grossLoss = 0;
                    if (FinalIncomeTotalAmount < 0)
                        grossLoss = FinalExpenseTotalAmount - (FinalIncomeTotalAmount * -1);
                    else
                        grossLoss = FinalExpenseTotalAmount - FinalIncomeTotalAmount;

                    GrossLossString = grossLoss.ToString() + " CR";
                    isLoss = true;
                    isProfit = false;

                    FinalIncomeTotalAmount = FinalExpenseTotalAmount;
                    if (FinalIncomeTotalAmount == 0)
                        FinalIncomeTotalAmountstring = (FinalIncomeTotalAmount).ToString() + ".00 CR";
                    else
                        FinalIncomeTotalAmountstring = (FinalIncomeTotalAmount).ToString() + " CR";

                }
                else
                {
                    isLoss = false;
                    isProfit = true;
                    decimal grossProfit = 0;
                    if (FinalExpenseTotalAmount < 0)
                        grossProfit = FinalIncomeTotalAmount - (FinalExpenseTotalAmount * -1);
                    else
                        grossProfit = FinalIncomeTotalAmount - FinalExpenseTotalAmount;

                    GrossProfitString = grossProfit.ToString() + " DR";

                    FinalExpenseTotalAmount = FinalIncomeTotalAmount;
                    if (FinalExpenseTotalAmount == 0)
                        FinalExpenseTotalAmountstring = (FinalExpenseTotalAmount).ToString() + ".00 DR";
                    else
                        FinalExpenseTotalAmountstring = (FinalExpenseTotalAmount).ToString() + " DR";

                }

                var data = new
                {
                    ProfitandLossStatementforParentExpenseList = objProfitandLossStatementforParentExpenseList,
                    ProfitandLossStatementforParentIncomeList = objProfitandLossStatementforParentIncomeList,
                    FinalExpenseTotalAmount = FinalExpenseTotalAmountstring,
                    FinalIncomeTotalAmount = FinalIncomeTotalAmountstring,
                    GrossLoss = GrossLossString,
                    isLoss = isLoss,
                    isProfit = isProfit,
                    GrossProfit = GrossProfitString
                };

                //sql1.Close();
                db.Dispose();
                return data;
            }
        }


        public int ClosePandLFinancialYear(PandLClosingYearModel data, User user)
        {
            int Count = 0;

            try
            {
                using (var db = new BSCCSLEntity())
                {
                    if (db.PandLFinancialYearClosingBalance.Where(x => x.FinancialYear == data.FinancialYear && x.BranchId == data.BranchId).Any())
                    {
                        List<PandLFinancialYearClosingBalance> objPandLFinancialYearClosingBalanceData = db.PandLFinancialYearClosingBalance.Where(x => x.FinancialYear == data.FinancialYear).ToList();
                        db.PandLFinancialYearClosingBalance.RemoveRange(objPandLFinancialYearClosingBalanceData);
                        db.SaveChanges();
                    }

                    PandLFinancialYearClosingBalance objPandLFinancialYearClosingBalance = new PandLFinancialYearClosingBalance();

                    if (data != null)
                    {
                        objPandLFinancialYearClosingBalance.Name = "Profit and Loss A/C";
                        objPandLFinancialYearClosingBalance.FinancialYear = data.FinancialYear;
                        objPandLFinancialYearClosingBalance.BranchId = data.BranchId;
                        if (data.isLoss == true && data.isProfit == false)
                            objPandLFinancialYearClosingBalance.ClosingBalanceCR = Convert.ToDecimal(data.FinalIncomeTotalAmount.Split(' ')[0]);
                        else
                            objPandLFinancialYearClosingBalance.ClosingBalanceDR = Convert.ToDecimal(data.FinalExpenseTotalAmount.Split(' ')[0]);
                        objPandLFinancialYearClosingBalance.CreatedDate = DateTime.Now;
                        objPandLFinancialYearClosingBalance.CreatedBy = user.UserId;
                        db.PandLFinancialYearClosingBalance.Add(objPandLFinancialYearClosingBalance);
                        db.SaveChanges();

                        Count = 1;
                    }
                    else
                    {
                        objPandLFinancialYearClosingBalance = null;
                        Count = 0;

                    }


                }
            }
            catch (Exception ex)
            {
                Count = 0;
            }
            return Count;
        }



    }
}
