using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSCCSL.Models;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Globalization;

namespace BSCCSL.Services
{

    public class ReportService
    {
        public object GetAccountList(DataTableSearch Search)
        {
            using (var db = new BSCCSLEntity())
            {
                Branch objBranch = db.Branch.Where(x => x.BranchId == Search.BranchId).FirstOrDefault();

                var ListofAccount = (from cp in db.CustomerProduct.Where(s => s.IsDelete == false && (s.AccountNumber == Search.sSearch || s.IsActive == true)).AsEnumerable()
                                     join c in db.Customer.Where(c => c.IsDelete == false) on cp.CustomerId equals c.CustomerId
                                     join pd in db.CustomerPersonalDetail.Where(s => s.IsDelete == false).AsEnumerable() on c.CustomerId equals pd.CustomerId
                                     join ca in db.CustomerAddress.Where(s => s.IsDelete == false).AsEnumerable() on pd.PersonalDetailId equals ca.PersonalDetailId
                                     group new { cp, c, pd, ca } by new { c, cp } into g
                                     select new
                                     {
                                         ClientId = g.Key.c.ClienId,
                                         CustomerName = string.Join(", ", g.Select(p => p.pd.FirstName + " " + p.pd.MiddleName + " " + p.pd.LastName).ToArray()),
                                         MobileNo = string.Join(", ", g.Select(p => p.ca.MobileNo).ToArray()),
                                         AccountNumber = g.Select(c => c.cp.AccountNumber).FirstOrDefault(),
                                         InterestRate = g.Select(i => i.cp.InterestRate).FirstOrDefault(),
                                         ProductTypeName = g.Select(pt => pt.cp.ProductTypeName).FirstOrDefault(),
                                         Balance = g.Select(b => b.cp.Balance).FirstOrDefault(),
                                         Openingdate = g.Select(o => o.cp.OpeningDate).FirstOrDefault(),
                                         BranchId = g.Select(x => x.c.BranchId).FirstOrDefault(),
                                     }).AsQueryable();


                if (!string.IsNullOrEmpty(Search.sSearch))
                    ListofAccount = ListofAccount.Where(c => c.AccountNumber.Contains(Search.sSearch.Trim()) || c.CustomerName.ToLower().Contains(Search.sSearch.ToLower().Trim()));

                if (objBranch != null)
                {
                    if (!objBranch.IsHO)
                    {
                        if (!string.IsNullOrEmpty(Search.BranchId.ToString()))
                            ListofAccount = ListofAccount.Where(c => c.BranchId == Search.BranchId);
                    }
                }

                if (!string.IsNullOrEmpty(Search.ProductName))
                    ListofAccount = ListofAccount.Where(c => c.ProductTypeName.Contains(Search.ProductName.Trim()));

                if (!string.IsNullOrEmpty(Search.InterestRate.ToString()))
                    ListofAccount = ListofAccount.Where(c => c.InterestRate == Search.InterestRate);

                if (!string.IsNullOrEmpty(Search.fromDate.ToString()))
                    ListofAccount = ListofAccount.Where(c => c.Openingdate >= Search.fromDate);

                if (!string.IsNullOrEmpty(Search.toDate.ToString()))
                    ListofAccount = ListofAccount.Where(c => c.Openingdate <= Search.toDate);


                var Accountlist = ListofAccount.OrderBy(c => c.AccountNumber).Skip(Search.iDisplayStart).Take(Search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = Accountlist.Count(),
                    iTotalDisplayRecords = ListofAccount.Count(),
                    aaData = Accountlist
                };
                return data;
            }
        }

        public object GetProductwiseBalanceList(DataTableSearch Search)
        {

            using (var db = new BSCCSLEntity())
            {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                List<RptProductwiseReportList> listother = new List<RptProductwiseReportList>();
                var list = new List<RptProductwiseReportList>();
                Branch objBranch = db.Branch.Where(x => x.BranchId == Search.BranchId).FirstOrDefault();

                if (!string.IsNullOrEmpty(Search.FinYear))
                {
                    string startDateString = "";
                    string endDateString = "";
                    startDateString = Search.FinYear.Split('-')[0] + "-04-01";
                    endDateString = Search.FinYear.Split('-')[1] + "-03-31";
                    startDate = Convert.ToDateTime(startDateString);
                    endDate = Convert.ToDateTime(endDateString);

                    if (objBranch != null)
                    {
                        if (objBranch.IsHO)
                        {
                            var productlist = (from p in db.Product
                                               join cp in db.CustomerProduct.Where(s => s.IsDelete == false && s.IsActive == true && s.CreatedDate >= startDate && s.CreatedDate <= endDate) on p.ProductId equals cp.ProductId
                                               join c in db.Customer.Where(a => a.IsDelete == false) on cp.CustomerId equals c.CustomerId
                                               group new { p, cp.Balance } by new { p.ProductId, p.ProductName, p.ProductType } into g
                                               select new RptProductwiseReportList
                                               {
                                                   ProductType = g.Key.ProductType,
                                                   ProductName = g.Key.ProductName,
                                                   Balance = g.Select(a => a.Balance.Value).Sum(),
                                               }).ToList();


                            var admisn = new RptProductwiseReportList();
                            admisn.Balance = (from cp in db.CustomerProduct.Where(s => s.IsDelete == false && s.ProductType == ProductType.Saving_Account && s.IsActive == true)
                                              join c in db.Customer.Where(a => a.IsDelete == false)
                                              on cp.CustomerId equals c.CustomerId
                                              // join t in db.Transaction.Where(a => a.Description.Contains("admisn fee")) on cp.CustomerProductId equals t.CustomerProductId
                                              join t in db.Transaction.Where(a => a.DescIndentify == DescIndentify.AdmissionFee && a.TransactionTime >= startDate && a.TransactionTime <= endDate) on cp.CustomerProductId equals t.CustomerProductId
                                              select t.Amount
                                        ).ToList().Sum();
                            admisn.ProductName = "Admission Fee";


                            var share = new RptProductwiseReportList();
                            share.Balance = (from c in db.Customer.Where(a => a.IsDelete == false)
                                             join s in db.CustomerShare.Where(a => a.IsDelete == false && a.CreatedDate >= startDate && a.CreatedDate <= endDate) on c.CustomerId equals s.CustomerId
                                             select s.Total
                                         ).ToList().Sum();

                            share.ProductName = "Share";

                            listother.Add(admisn);
                            listother.Add(share);

                            list = productlist.Union(listother).ToList();
                        }
                        else
                        {
                            var productlist = (from p in db.Product
                                               join cp in db.CustomerProduct.Where(s => s.IsDelete == false && s.IsActive == true && s.CreatedDate >= startDate && s.CreatedDate <= endDate) on p.ProductId equals cp.ProductId
                                               join c in db.Customer.Where(a => a.IsDelete == false && a.BranchId == Search.BranchId) on cp.CustomerId equals c.CustomerId
                                               group new { p, cp.Balance } by new { p.ProductId, p.ProductName, p.ProductType } into g
                                               select new RptProductwiseReportList
                                               {
                                                   ProductType = g.Key.ProductType,
                                                   ProductName = g.Key.ProductName,
                                                   Balance = g.Select(a => a.Balance.Value).Sum(),
                                               }).ToList();


                            var admisn = new RptProductwiseReportList();
                            admisn.Balance = (from cp in db.CustomerProduct.Where(s => s.IsDelete == false && s.ProductType == ProductType.Saving_Account && s.IsActive == true)
                                              join c in db.Customer.Where(a => a.IsDelete == false && a.BranchId == Search.BranchId)
                                              on cp.CustomerId equals c.CustomerId
                                              join t in db.Transaction.Where(a => a.DescIndentify == DescIndentify.AdmissionFee && a.TransactionTime >= startDate && a.TransactionTime <= endDate) on cp.CustomerProductId equals t.CustomerProductId
                                              select t.Amount
                                        ).ToList().Sum();
                            admisn.ProductName = "Admission Fee";


                            var share = new RptProductwiseReportList();
                            share.Balance = (from c in db.Customer.Where(a => a.IsDelete == false && a.BranchId == Search.BranchId)
                                             join s in db.CustomerShare.Where(a => a.IsDelete == false && a.CreatedDate >= startDate && a.CreatedDate <= endDate) on c.CustomerId equals s.CustomerId
                                             select s.Total
                                         ).ToList().Sum();

                            share.ProductName = "Share";

                            listother.Add(admisn);
                            listother.Add(share);

                            list = productlist.Union(listother).ToList();
                        }
                    }

                }


                //var productlist = (from p in db.Product
                //                   join cp in db.CustomerProduct.Where(s => s.IsDelete == false) on p.ProductId equals cp.ProductId
                //                   join c in db.Customer.Where(a => a.IsDelete == false && a.BranchId == Search.BranchId) on cp.CustomerId equals c.CustomerId
                //                   group new { p, cp.Balance } by new { p.ProductId, p.ProductName, p.ProductType } into g
                //                   select new RptProductwiseReportList
                //                   {
                //                       ProductType = g.Key.ProductType,
                //                       ProductName = g.Key.ProductName,
                //                       Balance = g.Select(a => a.Balance.Value).Sum(),
                //                   }).ToList();


                //var admisn = new RptProductwiseReportList();
                //admisn.Balance = (from cp in db.CustomerProduct.Where(s => s.IsDelete == false)
                //                  join c in db.Customer.Where(a => a.IsDelete == false && a.BranchId == Search.BranchId)
                //                  on cp.CustomerId equals c.CustomerId
                //                  join t in db.Transaction.Where(a => a.Description.Contains("admisn fee")) on cp.CustomerProductId equals t.CustomerProductId
                //                  select t.Amount
                //            ).ToList().Sum();
                //admisn.ProductName = "Admission Fee";


                //var share = new RptProductwiseReportList();
                //share.Balance = (from c in db.Customer.Where(a => a.IsDelete == false && a.BranchId == Search.BranchId)
                //                 join s in db.CustomerShare.Where(a => a.IsDelete == false) on c.CustomerId equals s.CustomerId
                //                 select s.Total
                //             ).ToList().Sum();

                //share.ProductName = "Share";

                //listother.Add(admisn);
                //listother.Add(share);

                //var list = productlist.Union(listother);
                var result = list.OrderBy(c => c.ProductName).Skip(Search.iDisplayStart).Take(Search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = result.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = result,
                    Total = result.Sum(a => a.Balance)
                };

                return data;
            }

        }

        public object RptDayScrollList(DataTableSearch search, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptDayScroll", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramuser = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                paramuser.Value = (object)search.fromDate ?? DBNull.Value;

                SqlParameter paramnewtemp = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.Date);
                paramnewtemp.Value = (object)search.toDate ?? DBNull.Value;

                var IsHO = db.Branch.Where(c => c.BranchId == search.BranchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = search.BranchId;
                }

                SqlParameter paramEdt = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                paramEdt.Value = search.iDisplayStart;

                SqlParameter parampartpost = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                parampartpost.Value = search.iDisplayLength;



                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptDayScroll> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptDayScroll>(reader).ToList();
                reader.NextResult();

                decimal TotalDeposite = 0; decimal TotalWithdrawal = 0;
                TotalDeposite = rptlist.Where(x => x.Type == TypeCRDR.CR).Select(x => x.Amount).Sum();
                TotalWithdrawal = rptlist.Where(x => x.Type == TypeCRDR.DR).Select(x => x.Amount).Sum();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                    TotalDeposite = TotalDeposite,
                    TotalWithdrawal = TotalWithdrawal,
                };
                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object GetAllDayScrollTotal(DataTableSearch search)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = (from td in db.Transaction.Where(s => DbFunctions.TruncateTime(s.TransactionTime) >= search.fromDate && DbFunctions.TruncateTime(s.TransactionTime) <= search.toDate)
                            join cp in db.CustomerProduct.Where(s => s.IsActive == true && s.IsDelete == false && (s.Status == CustomerProductStatus.Approved || s.Status == null))
                            on td.CustomerProductId equals cp.CustomerProductId
                            join p in db.Product on cp.ProductId equals p.ProductId
                            join c in db.Customer.Where(a => a.IsDelete == false && a.BranchId == search.BranchId) on cp.CustomerId equals c.CustomerId
                            select new
                            {
                                Type = td.Type,
                                Status = td.Status,
                                TransactionType = td.TransactionType,
                                Amount = td.Amount,
                                ProductType = p.ProductType
                            }).ToList();

                var Data = new
                {
                    SavingDepositSum = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Saving_Account).Sum(p => p.Amount),
                    SavingCashDepositSum = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cash).Sum(p => p.Amount),
                    SavingChequeDepositSum = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cheque && p.Status == Status.Clear).Sum(p => p.Amount),
                    NoOfCustomerSavingChequeDeposit = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cheque && p.Status == Status.Clear).Count(),
                    NoOfCustomerSavingCashDeposit = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cash).Count(),
                    SavingWithdrawSum = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Saving_Account).Sum(p => p.Amount),
                    SavingCashWithdrawSum = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cash).Sum(p => p.Amount),
                    SavingChequeWithdrawSum = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cheque).Sum(p => p.Amount),
                    NoOfCustomerSavingChequeWithdraw = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cheque).Count(),
                    NoOfCustomerSavingCashWithdraw = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cash).Count(),
                    CurrentDepositSum = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Current_Account).Sum(p => p.Amount),
                    CurrentWithdrawSum = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Current_Account).Sum(p => p.Amount),
                    RecuringDepositeSum = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Recurring_Deposit).Sum(p => p.Amount),
                    FixDipositeSum = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Fixed_Deposit).Sum(p => p.Amount),
                    TYPDipositeSum = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Three_Year_Product).Sum(p => p.Amount)
                };
                return Data;
            }
        }

        public object GetDayScrollDetails(DataTableSearch search)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = (from td in db.Transaction.Where(s => DbFunctions.TruncateTime(s.TransactionTime) >= search.fromDate && DbFunctions.TruncateTime(s.TransactionTime) <= search.toDate)
                            join cp in db.CustomerProduct.Where(s => s.IsActive == true && s.IsDelete == false && (s.Status == CustomerProductStatus.Approved || s.Status == null))
                            on td.CustomerProductId equals cp.CustomerProductId
                            join p in db.Product on cp.ProductId equals p.ProductId
                            join c in db.Customer.Where(a => a.IsDelete == false && a.BranchId == search.BranchId) on cp.CustomerId equals c.CustomerId
                            join cpd in db.CustomerPersonalDetail.Where(a => a.IsDelete == false) on c.CustomerId equals cpd.CustomerId
                            select new
                            {
                                Type = td.Type,
                                Status = td.Status,
                                TransactionType = td.TransactionType,
                                Amount = td.Amount,
                                ProductType = p.ProductType,
                                CustomerName = cpd.FirstName + " " + cpd.MiddleName + " " + cpd.LastName,
                                AccountNumber = cp.AccountNumber,
                                Date = td.TransactionTime
                            }).OrderBy(a => a.Date).ToList();


                if (search.DayScrollStatsType != null)
                {
                    if (DayScrollStateType.Saving_Deposite == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Saving_Account).ToList();
                    else if (DayScrollStateType.Saving_Deposite_By_Cash == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cash).ToList();
                    else if (DayScrollStateType.Saving_Deposite_By_Cheque == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cheque && p.Status == Status.Clear).ToList();
                    else if (DayScrollStateType.Total_Customers_Deposited_By_Cheque == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cheque && p.Status == Status.Clear).ToList();
                    else if (DayScrollStateType.Total_Customers_Deposited_By_Cash == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cash).ToList();
                    else if (DayScrollStateType.Saving_Withdrawals == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Saving_Account).ToList();
                    else if (DayScrollStateType.Saving_Withdrawals_By_Cash == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cash).ToList();
                    else if (DayScrollStateType.Saving_Withdrawals_By_Cheque == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cheque).ToList();
                    else if (DayScrollStateType.Total_Customers_Withdrawals_By_Cheque == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cheque).ToList();
                    else if (DayScrollStateType.Total_Customers_Withdrawals_By_Cash == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Saving_Account && p.TransactionType == TransactionType.Cash).ToList();
                    else if (DayScrollStateType.Total_Current_Deposite == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Current_Account).ToList();
                    else if (DayScrollStateType.Total_Current_Withdrawals == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.DR && p.ProductType == ProductType.Current_Account).ToList();
                    else if (DayScrollStateType.Total_Recurring_Deposite == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Recurring_Deposit).ToList();
                    else if (DayScrollStateType.Total_Fixed_Deposite == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Fixed_Deposit).ToList();
                    else if (DayScrollStateType.Total_Dhan_Vruddhi_Yojana_Deposite == (DayScrollStateType)search.DayScrollStatsType)
                        list = list.Where(p => p.Type == TypeCRDR.CR && p.ProductType == ProductType.Three_Year_Product).ToList();
                }
                var Data = new
                {
                    DayScrollDetailsList = list
                };
                return Data;
            }
        }


        public object GetAllAgentCustomerList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptAgentCustomerList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramuser = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                paramuser.Value = (object)Search.sSearch ?? DBNull.Value;

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

                SqlParameter parampname = cmdTimesheet.Parameters.Add("@ProductName", SqlDbType.NVarChar);
                parampname.Value = (object)Search.ProductName ?? DBNull.Value;

                SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = (object)Search.fromDate ?? DBNull.Value;

                SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = (object)Search.toDate ?? DBNull.Value;


                SqlParameter paramEdt = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                paramEdt.Value = Search.iDisplayStart;

                SqlParameter parampartpost = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                parampartpost.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();

                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptAgentCustomers> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptAgentCustomers>(reader).ToList();
                reader.NextResult();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                };
                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object GetAllDueInstallmentList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptDueInstallmentList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = (object)Search.fromDate ?? DBNull.Value;

                SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = (object)Search.toDate ?? DBNull.Value;

                SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                paramAgentName.Value = (object)Search.AgentName ?? DBNull.Value;

                SqlParameter paramCustomerName = cmdTimesheet.Parameters.Add("@CustomerName", SqlDbType.NVarChar);
                paramCustomerName.Value = (object)Search.sSearch ?? DBNull.Value;

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
                paramStart.Value = Search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                decimal TotalDueInst = 0; decimal TotalLatePayment = 0;
                decimal Amount = 0; decimal TotalAmount = 0;

                List<RptDueInstallmentList> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptDueInstallmentList>(reader).ToList();
                reader.NextResult();
                TotalDueInst = Convert.ToDecimal(rptlist.Select(x => x.PendingInstallment).Sum());
                TotalLatePayment = Convert.ToDecimal(rptlist.Select(x => x.LatePayment).Sum());
                Amount = Convert.ToDecimal(rptlist.Select(x => x.Amount).Sum());
                TotalAmount = TotalDueInst + TotalLatePayment + Amount;
                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                    TotalDueInst = TotalDueInst,
                    TotalLatePayment = TotalLatePayment,
                    TotalAmount = TotalAmount,
                    Amount = Amount
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object GetAllAgentCommissionList(ReportSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptAgentCommission", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                if (!string.IsNullOrEmpty(Search.AgentName))
                {
                    SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                    paramAgentName.Value = Search.AgentName.Trim().ToLower();
                }
                else
                {
                    SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                    paramAgentName.Value = DBNull.Value;
                }

                SqlParameter paramMonth = cmdTimesheet.Parameters.Add("@Month", SqlDbType.Int);
                paramMonth.Value = (object)Search.Month ?? DBNull.Value;

                SqlParameter paramYear = cmdTimesheet.Parameters.Add("@Year", SqlDbType.Int);
                paramYear.Value = (object)Search.Year ?? DBNull.Value;

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

                SqlParameter paramProductName = cmdTimesheet.Parameters.Add("@ProductName", SqlDbType.NVarChar);
                paramProductName.Value = (object)Search.ProductName ?? DBNull.Value;


                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptAgentCommission> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptAgentCommission>(reader).ToList();
                reader.NextResult();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    data = rptlist,
                    draw = Search.draw,
                    recordsFiltered = Count,
                    recordsTotal = rptlist.Count()
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object GetCommissionData(RptAgentCommission Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptAgentCommissionByMonth", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                SqlParameter paramAgentId = cmdTimesheet.Parameters.Add("@AgentId", SqlDbType.UniqueIdentifier);
                paramAgentId.Value = Search.UserId;

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

                SqlParameter paramStart = cmdTimesheet.Parameters.Add("@Month", SqlDbType.Int);
                paramStart.Value = Search.Month;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Year", SqlDbType.Int);
                paramLength.Value = Search.Year;

                SqlParameter paramProductName = cmdTimesheet.Parameters.Add("@ProductName", SqlDbType.NVarChar);
                paramProductName.Value = (object)Search.ProductName ?? DBNull.Value;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptAgentCommissionByMonth> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptAgentCommissionByMonth>(reader).ToList();

                sql.Close();
                db.Dispose();
                return rptlist.OrderBy(s => s.Date);
            }
        }

        public object GetRDFDPendingInstallmentList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RDFDPendingInstallment", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = (object)Search.fromDate ?? DBNull.Value;

                SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = (object)Search.toDate ?? DBNull.Value;

                SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@CustomerName", SqlDbType.NVarChar);
                paramAgentName.Value = (object)Search.sSearch ?? DBNull.Value;

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
                paramStart.Value = Search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                decimal TotalAmount = 0;
                // Read Blogs from the first result set
                List<RptDueInstallmentList> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptDueInstallmentList>(reader).ToList();
                reader.NextResult();
                TotalAmount = rptlist.Select(a => a.Amount).Sum();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                    TotalAmount = TotalAmount
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object GetAllCommissionPaymentList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("AgentCommissionPaymentList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = (object)Search.fromDate ?? DBNull.Value;

                SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = (object)Search.toDate ?? DBNull.Value;

                SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                paramAgentName.Value = (object)Search.AgentName ?? DBNull.Value;

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
                paramStart.Value = Search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptCommissionPayment> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptCommissionPayment>(reader).ToList();
                reader.NextResult();

                decimal PaidCommission = ((IObjectContextAdapter)db).ObjectContext.Translate<decimal>(reader).FirstOrDefault();
                reader.NextResult();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                    PaidCommission = PaidCommission
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object GetProductInstallmentList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptProductInstallment", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = (object)Search.fromDate ?? DBNull.Value;

                SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = (object)Search.toDate ?? DBNull.Value;

                SqlParameter paramCustomerName = cmdTimesheet.Parameters.Add("@CustomerName", SqlDbType.NVarChar);
                paramCustomerName.Value = (object)Search.sSearch ?? DBNull.Value;

                SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                paramAgentName.Value = (object)Search.AgentName ?? DBNull.Value;


                SqlParameter paramProductName = cmdTimesheet.Parameters.Add("@ProductName", SqlDbType.NVarChar);
                paramProductName.Value = (object)Search.ProductName ?? DBNull.Value;

                SqlParameter paramStatus = cmdTimesheet.Parameters.Add("@Status", SqlDbType.Bit);
                paramStatus.Value = (object)Search.Status ?? DBNull.Value;

                SqlParameter paramProductStatus = cmdTimesheet.Parameters.Add("@ProductStatus", SqlDbType.Int);
                paramProductStatus.Value = (object)Search.ProductStatus ?? DBNull.Value;

                SqlParameter paramPaymentType = cmdTimesheet.Parameters.Add("@PaymentType", SqlDbType.Int);
                paramPaymentType.Value = (object)Search.PaymentType ?? DBNull.Value;


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
                paramStart.Value = Search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptDueInstallmentList> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptDueInstallmentList>(reader).ToList();
                reader.NextResult();

                decimal PaidInstallment = ((IObjectContextAdapter)db).ObjectContext.Translate<decimal>(reader).FirstOrDefault();
                reader.NextResult();

                decimal PendingInstallment = ((IObjectContextAdapter)db).ObjectContext.Translate<decimal>(reader).FirstOrDefault();
                reader.NextResult();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                    PaidTotal = PaidInstallment,
                    PendingTotal = PendingInstallment
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object GetProductList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptProductList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = (object)Search.fromDate ?? DBNull.Value;

                SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = (object)Search.toDate ?? DBNull.Value;

                SqlParameter paramCustomerName = cmdTimesheet.Parameters.Add("@CustomerName", SqlDbType.NVarChar);
                paramCustomerName.Value = (object)Search.sSearch ?? DBNull.Value;

                SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                paramAgentName.Value = (object)Search.AgentName ?? DBNull.Value;


                SqlParameter paramProductName = cmdTimesheet.Parameters.Add("@ProductName", SqlDbType.NVarChar);
                paramProductName.Value = (object)Search.ProductName ?? DBNull.Value;


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
                paramStart.Value = Search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptDueInstallmentList> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptDueInstallmentList>(reader).ToList();
                reader.NextResult();


                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }


        public object GetMaturityList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                DateTime date = DateTime.Now;
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptMaturityList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = (object)Search.fromDate ?? DBNull.Value;



                SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = (object)Search.toDate ?? DBNull.Value;


                if (Search.fromDate == null && Search.toDate == null)
                {
                    paramFromDate.Value = (object)firstDayOfMonth;
                }

                if (Search.toDate == null && Search.fromDate == null)
                {
                    paramToDate.Value = (object)lastDayOfMonth;
                }

                SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@CustomerName", SqlDbType.NVarChar);
                paramAgentName.Value = (object)Search.sSearch ?? DBNull.Value;

                SqlParameter paramProductName = cmdTimesheet.Parameters.Add("@ProductName", SqlDbType.NVarChar);
                paramProductName.Value = (object)Search.ProductName ?? DBNull.Value;

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
                paramStart.Value = Search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                decimal TotalMaturityAmount = 0;

                // Read Blogs from the first result set
                List<RptMaturityList> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptMaturityList>(reader).ToList();
                reader.NextResult();
                TotalMaturityAmount = Convert.ToDecimal(rptlist.Where(x => x.MaturityAmount != null).Select(x => x.MaturityAmount).Sum());


                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                    TotalMaturityAmount = TotalMaturityAmount,
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object GetEmployeeProductList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptEmployeeProductList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramuser = cmdTimesheet.Parameters.Add("@ProductName", SqlDbType.NVarChar);
                paramuser.Value = (object)Search.ProductName ?? DBNull.Value;

                SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = (object)Search.fromDate ?? DBNull.Value;

                SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = (object)Search.toDate ?? DBNull.Value;

                SqlParameter paramEmployeeName = cmdTimesheet.Parameters.Add("@EmployeeName", SqlDbType.NVarChar);
                paramEmployeeName.Value = (object)Search.EmployeeName ?? DBNull.Value;

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

                SqlParameter paramEdt = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                paramEdt.Value = Search.iDisplayStart;

                SqlParameter parampartpost = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                parampartpost.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();

                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                decimal TotalAmount = 0;

                // Read Blogs from the first result set
                List<RptEmployeeProductList> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptEmployeeProductList>(reader).ToList();
                reader.NextResult();

                TotalAmount = rptlist.Select(x => x.Amount).Sum();
                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                    TotalAmount = TotalAmount,
                };
                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object GetCustomerShares(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptCustomerShare", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramuser = cmdTimesheet.Parameters.Add("@CustomerName", SqlDbType.NVarChar);
                paramuser.Value = (object)Search.sSearch ?? DBNull.Value;

                SqlParameter paramMaturity = cmdTimesheet.Parameters.Add("@Maturity", SqlDbType.Int);
                paramMaturity.Value = (object)Search.Maturity ?? DBNull.Value;

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

                SqlParameter paramEdt = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                paramEdt.Value = Search.iDisplayStart;

                SqlParameter parampartpost = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                parampartpost.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();

                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                decimal TotalShareAmount = 0;

                // Read Blogs from the first result set
                List<RptCustomerShares> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptCustomerShares>(reader).ToList();
                reader.NextResult();

                TotalShareAmount = rptlist.Select(a => a.ShareAmount).Sum();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                    TotalShareAmount = TotalShareAmount,
                };
                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object RptAccountsCRDR(Guid branchId, string finYear, User user, DateTime? startDate, DateTime? endDate)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptAccountsCRDR", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;

                var IsHO = db.Branch.Where(c => c.BranchId == branchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = branchId;
                }
                if (string.IsNullOrEmpty(finYear))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = finYear;
                }

                if (startDate == null)
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = startDate;
                }
                if (endDate == null)
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = endDate;
                }

                //SqlParameter paramEdt = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                //paramEdt.Value = Search.iDisplayStart;

                //SqlParameter parampartpost = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                //parampartpost.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();

                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptAccountsCRDR> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptAccountsCRDR>(reader).ToList();
                //reader.NextResult();
                decimal TotalCredit = 0; decimal TotalDebit = 0; decimal TotalOpeningBalance = 0;
                TotalCredit = Convert.ToDecimal(rptlist.Select(x => x.Credit).Sum());
                TotalDebit = Convert.ToDecimal(rptlist.Select(x => x.Debit).Sum());
                TotalOpeningBalance = Convert.ToDecimal(rptlist.Select(x => x.OpeningBalance).Sum());
                var data = new
                {
                    rptlist = rptlist,
                    TotalCredit = TotalCredit,
                    TotalDebit = TotalDebit,
                    TotalOpeningBalance = TotalOpeningBalance,
                };


                // int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                //var data = new
                //{
                //    sEcho = Search.sEcho,
                //    iTotalRecords = rptlist.Count(),
                //    iTotalDisplayRecords = rptlist.Count,
                //    aaData = rptlist,
                //};
                sql.Close();
                db.Dispose();
                return data;
            }
        }
        public object RptSummaryCRDR(Guid branchId, string finYear, User user, DateTime? startDate, DateTime? endDate)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptSummaryCRDR", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;

                var IsHO = db.Branch.Where(c => c.BranchId == branchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = branchId;
                }
                if (string.IsNullOrEmpty(finYear))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = finYear;
                }

                if (startDate == null)
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = startDate;
                }
                if (endDate == null)
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = endDate;
                }

                //SqlParameter paramEdt = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                //paramEdt.Value = Search.iDisplayStart;

                //SqlParameter parampartpost = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                //parampartpost.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();

                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptAccountsCRDR> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptAccountsCRDR>(reader).ToList();
                //reader.NextResult();
                decimal TotalCredit = 0; decimal TotalDebit = 0; decimal TotalOpeningBalance = 0;
                TotalCredit = Convert.ToDecimal(rptlist.Select(x => x.Credit).Sum());
                TotalDebit = Convert.ToDecimal(rptlist.Select(x => x.Debit).Sum());
                TotalOpeningBalance = Convert.ToDecimal(rptlist.Select(x => x.OpeningBalance).Sum());
                var data = new
                {
                    rptlist = rptlist,
                    TotalCredit = TotalCredit,
                    TotalDebit = TotalDebit,
                    TotalOpeningBalance = TotalOpeningBalance,
                };


                // int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                //var data = new
                //{
                //    sEcho = Search.sEcho,
                //    iTotalRecords = rptlist.Count(),
                //    iTotalDisplayRecords = rptlist.Count,
                //    aaData = rptlist,
                //};
                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object RptAgentHierarchyCommission(ReportSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptAgentHierarchyCommission", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                if (!string.IsNullOrEmpty(Search.AgentName))
                {
                    SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                    paramAgentName.Value = Search.AgentName.Trim().ToLower();
                }
                else
                {
                    SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                    paramAgentName.Value = DBNull.Value;
                }

                SqlParameter paramMonth = cmdTimesheet.Parameters.Add("@Month", SqlDbType.Int);
                paramMonth.Value = (object)Search.Month ?? DBNull.Value;

                SqlParameter paramYear = cmdTimesheet.Parameters.Add("@Year", SqlDbType.Int);
                paramYear.Value = (object)Search.Year ?? DBNull.Value;

                SqlParameter paramUserStatus = cmdTimesheet.Parameters.Add("@UserStatus", SqlDbType.Int);
                paramUserStatus.Value = (object)Search.UserStatus ?? DBNull.Value;


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

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptAgentCommission> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptAgentCommission>(reader).ToList();
                reader.NextResult();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    data = rptlist,
                    draw = Search.draw,
                    recordsFiltered = Count,
                    recordsTotal = rptlist.Count()
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object RptAgentHierarchyCommissionByMonth(RptAgentCommission Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptAgentHierarchyCommissionByMonth", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                SqlParameter paramAgentId = cmdTimesheet.Parameters.Add("@AgentId", SqlDbType.UniqueIdentifier);
                paramAgentId.Value = Search.UserId;

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

                SqlParameter paramStart = cmdTimesheet.Parameters.Add("@Month", SqlDbType.Int);
                paramStart.Value = Search.Month;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Year", SqlDbType.Int);
                paramLength.Value = Search.Year;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptAgentCommissionByMonth> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptAgentCommissionByMonth>(reader).ToList();

                sql.Close();
                db.Dispose();
                return rptlist.OrderBy(s => s.Date);
            }
        }

        public object RptProfitandLossforExpense(Guid branchId, string finYear, User user, DateTime? startDate, DateTime? endDate)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptProfitandLossforExpense", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;

                var IsHO = db.Branch.Where(c => c.BranchId == branchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = branchId;
                }
                if (string.IsNullOrEmpty(finYear))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = finYear;
                }
                if (startDate == null)
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = startDate;
                }
                if (endDate == null)
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = endDate;
                }



                sql.Open();

                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptProfitLossforExpense> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptProfitLossforExpense>(reader).ToList();
                //reader.NextResult();
                if (string.IsNullOrEmpty(finYear))
                    rptlist = new List<RptProfitLossforExpense>();


                sql.Close();
                db.Dispose();
                return rptlist;
            }
        }

        public object RptProfitandLossforIncome(Guid branchId, string finYear, User user, DateTime? startDate, DateTime? endDate)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptProfitandLossforIncome", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;

                var IsHO = db.Branch.Where(c => c.BranchId == branchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = branchId;
                }
                if (string.IsNullOrEmpty(finYear))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = finYear;
                }
                if (startDate == null)
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = startDate;
                }
                if (endDate == null)
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = endDate;
                }



                sql.Open();

                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptProfitLossforInCome> rptlist1 = ((IObjectContextAdapter)db).ObjectContext.Translate<RptProfitLossforInCome>(reader).ToList();
                //reader.NextResult();

                if (string.IsNullOrEmpty(finYear))
                    rptlist1 = new List<RptProfitLossforInCome>();

                sql.Close();
                db.Dispose();
                return rptlist1;
            }
        }


        public object RptCashBook(Guid branchId, DateTime? fromDate, DateTime? ToDate, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptCashBook", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;

                var IsHO = db.Branch.Where(c => c.BranchId == branchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = branchId;
                }
                if (string.IsNullOrEmpty(fromDate.ToString()))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@StartDate", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@StartDate", SqlDbType.VarChar);
                    paramfinYear.Value = Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrEmpty(ToDate.ToString()))
                {
                    if (string.IsNullOrEmpty(fromDate.ToString()))
                    {
                        SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@EndDate", SqlDbType.VarChar);
                        paramfinYear.Value = DBNull.Value;
                    }
                    else
                    {
                        SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@EndDate", SqlDbType.VarChar);
                        paramfinYear.Value = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@EndDate", SqlDbType.VarChar);
                    paramfinYear.Value = Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");
                }


                sql.Open();

                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptCashBook> rptlist1 = ((IObjectContextAdapter)db).ObjectContext.Translate<RptCashBook>(reader).ToList();

                //rptlist1 = rptlist1.GroupBy(x => x.AccountNumber).Select(y => y.First()).ToList();

                string StartDateString = Convert.ToDateTime("2017-04-21").ToString("yyyy-MM-dd");
                DateTime StartDate = Convert.ToDateTime(StartDateString);

                #region Get Previous Days data to calculate opening Balance
                SqlCommand cmdTimesheet1 = new SqlCommand("RptCashBook", sql);
                cmdTimesheet1.CommandType = CommandType.StoredProcedure;

                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet1.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet1.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = branchId;
                }
                if (string.IsNullOrEmpty(fromDate.ToString()))
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
                    paramenddate.Value = Convert.ToDateTime(fromDate).AddDays(-1).ToString("yyyy-MM-dd");
                }

                var reader1 = cmdTimesheet1.ExecuteReader();
                List<RptCashBook> rptlist2 = ((IObjectContextAdapter)db).ObjectContext.Translate<RptCashBook>(reader1).ToList();
                //rptlist2 = rptlist2.GroupBy(x => x.AccountNumber).Select(y => y.First()).ToList();

                var TotalCredittilldate = rptlist2.Sum(x => x.Credit);
                var TotalDebittilldate = rptlist2.Sum(x => x.Debit);

                #endregion


                var NewOpeningBalance = 0.00;


                var TotalCredit = rptlist1.Sum(x => x.Credit);
                var TotalDebit = rptlist1.Sum(x => x.Debit);
                var FirstDate = Convert.ToDateTime(fromDate).ToString("dd-MMM-yyyy");
                var LastDate = Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy");
                if (string.IsNullOrEmpty(ToDate.ToString()))
                {
                    if (!string.IsNullOrEmpty(fromDate.ToString()))
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
                if (StartDate == fromDate)
                    NewOpeningBalance = 0.00;



                //OpeningBalance = (Convert.ToDecimal(NewOpeningBalance) + Convert.ToDecimal(TotalCredittilldate)) - Convert.ToDecimal(TotalDebittilldate);
                OpeningBalance = (Convert.ToDecimal(NewOpeningBalance) + Convert.ToDecimal(TotalDebittilldate)) - Convert.ToDecimal(TotalCredittilldate);

                if (OpeningBalance < 0)
                    Openingbalancestring = " " + OpeningBalance.ToString().Split('-')[1] + " DR";
                else
                    Openingbalancestring = " " + OpeningBalance.ToString() + " CR";

                if (fromDate != StartDate)
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
                foreach (var item in rptlist1)
                {
                    if (item.Debit != 0)
                        Balance = Balance + Convert.ToDecimal(item.Debit);
                    if (item.Credit != 0)
                        Balance = Balance - Convert.ToDecimal(item.Credit);

                    item.Balance = Balance;
                }



                if (ClosingBalance < 0)
                    Closingbalancestring = " " + ClosingBalance.ToString().Split('-')[1] + " DR";
                else
                    Closingbalancestring = " " + ClosingBalance.ToString() + " CR";


                if (fromDate < StartDate)
                {
                    Openingbalancestring = " " + "0";
                    Closingbalancestring = " " + "0";
                }
                else if (fromDate == StartDate)
                    Openingbalancestring = " " + "0";

                //reader.NextResult();
                var Data = new
                {
                    rptlist1 = rptlist1,
                    TotalCredit = TotalCredit,
                    TotalDebit = TotalDebit,
                    FirstDate = FirstDate,
                    LastDate = LastDate,
                    OpeningBalance = Openingbalancestring,
                    ClosingBalance = Closingbalancestring
                };

                sql.Close();
                db.Dispose();
                return Data;
            }
        }

        public object RptBankBook(Guid branchId, DateTime? fromDate, DateTime? ToDate, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptBankBook", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;

                var IsHO = db.Branch.Where(c => c.BranchId == branchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = branchId;
                }
                if (string.IsNullOrEmpty(fromDate.ToString()))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@StartDate", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@StartDate", SqlDbType.VarChar);
                    paramfinYear.Value = Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrEmpty(ToDate.ToString()))
                {
                    if (string.IsNullOrEmpty(fromDate.ToString()))
                    {
                        SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@EndDate", SqlDbType.VarChar);
                        paramfinYear.Value = DBNull.Value;
                    }
                    else
                    {
                        SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@EndDate", SqlDbType.VarChar);
                        paramfinYear.Value = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@EndDate", SqlDbType.VarChar);
                    paramfinYear.Value = Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");
                }


                sql.Open();

                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptBankBook> rptlist1 = ((IObjectContextAdapter)db).ObjectContext.Translate<RptBankBook>(reader).ToList();
                //rptlist1 = rptlist1.GroupBy(x => x.AccountNumber).Select(y => y.First()).ToList();

                var TotalCredit = rptlist1.Sum(x => x.Credit);
                var TotalDebit = rptlist1.Sum(x => x.Debit);
                var FirstDate = Convert.ToDateTime(fromDate).ToString("dd-MMM-yyyy");
                var LastDate = Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy");
                if (string.IsNullOrEmpty(ToDate.ToString()))
                {
                    if (!string.IsNullOrEmpty(fromDate.ToString()))
                    {
                        LastDate = DateTime.Now.ToString("dd-MMM-yyyy");
                    }
                }
                //reader.NextResult();
                var Data = new
                {
                    rptlist1 = rptlist1,
                    TotalCredit = TotalCredit,
                    TotalDebit = TotalDebit,
                    FirstDate = FirstDate,
                    LastDate = LastDate
                };

                sql.Close();
                db.Dispose();
                return Data;
            }
        }


        public object RptTrailBalance(Guid branchId, string finYear, User user, DateTime? startDate, DateTime? endDate)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptTrailBalance", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;

                var IsHO = db.Branch.Where(c => c.BranchId == branchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = branchId;
                }
                if (string.IsNullOrEmpty(finYear))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = finYear;
                }
                if (startDate == null)
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = startDate;
                }
                if (endDate == null)
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = endDate;
                }


                sql.Open();

                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptTrailBalance> rptlist1 = ((IObjectContextAdapter)db).ObjectContext.Translate<RptTrailBalance>(reader).ToList();
                var TotalCredit = rptlist1.Sum(x => x.Credit);
                var TotalDebit = rptlist1.Sum(x => x.Debit);

                if (string.IsNullOrEmpty(finYear))
                    rptlist1 = new List<RptTrailBalance>();

                //reader.NextResult();
                var Data = new
                {
                    rptlist1 = rptlist1,
                    TotalCredit = TotalCredit,
                    TotalDebit = TotalDebit,
                };

                sql.Close();
                db.Dispose();
                return Data;
            }
        }

        public object RptProfitandLossDetails(Guid branchId, string Particular, string PandLType, string FinYear, List<int> productTypes, User user, DateTime? startDate, DateTime? endDate)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptProfitandLossDetails", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;

                var IsHO = db.Branch.Where(c => c.BranchId == branchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = branchId;
                }
                if (string.IsNullOrEmpty(PandLType))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@PandLType", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@PandLType", SqlDbType.VarChar);
                    paramfinYear.Value = PandLType;
                }

                if (string.IsNullOrEmpty(FinYear))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FinYear", SqlDbType.VarChar);
                    paramfinYear.Value = FinYear;
                }

                if (startDate == null)
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    paramfromDate.Value = startDate;
                }
                if (endDate == null)
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramtoDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    paramtoDate.Value = endDate;
                }

                int PandLParticular = 0;
                if (PandLType == "1")
                {
                    if (PandLExpense.FD_Interest.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLExpense.FD_Interest);
                    else if (PandLExpense.RD_Interest.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLExpense.RD_Interest);
                    else if (PandLExpense.MIS_Interest.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLExpense.MIS_Interest);
                    else if (PandLExpense.RIP_Interest.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLExpense.RIP_Interest);
                    else if (PandLExpense.Super_Saving_Acc_Comm.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLExpense.Super_Saving_Acc_Comm);
                    else if (PandLExpense.Saving_Acc_Interest.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLExpense.Saving_Acc_Interest);
                    else if (PandLExpense.Agent_Commission.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLExpense.Agent_Commission);
                    else if (PandLExpense.Dhan_Vruddhi_Yojana_Interest.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLExpense.Dhan_Vruddhi_Yojana_Interest);
                    else
                        PandLParticular = Convert.ToInt16(PandLExpense.Expense);
                }
                else
                {
                    if (PandLIncome.Admission_Fee.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLIncome.Admission_Fee);
                    else if (PandLIncome.Loan_Interest.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLIncome.Loan_Interest);
                    else if (PandLIncome.Prematured_Charges.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLIncome.Prematured_Charges);
                    else if (PandLIncome.Processing_Charges.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLIncome.Processing_Charges);
                    else if (PandLIncome.Loan_Form_Cost.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLIncome.Loan_Form_Cost);
                    else if (PandLIncome.Flexi_Loan_Interest.ToString() == Particular.Replace(' ', '_'))
                        PandLParticular = Convert.ToInt16(PandLIncome.Flexi_Loan_Interest);
                    else
                        PandLParticular = Convert.ToInt16(PandLIncome.Income);

                }

                if (string.IsNullOrEmpty(PandLParticular.ToString()))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@PandLParticular", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@PandLParticular", SqlDbType.VarChar);
                    paramfinYear.Value = PandLParticular;
                }


                if (PandLParticular != 7)
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@AccountHeadName", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@AccountHeadName", SqlDbType.VarChar);
                    paramfinYear.Value = Particular;
                }


                sql.Open();

                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptProfitLossDetails> rptlist1 = ((IObjectContextAdapter)db).ObjectContext.Translate<RptProfitLossDetails>(reader).ToList();
                //reader.NextResult();
                if (productTypes != null)
                {
                    if (productTypes.Count() > 0)
                        rptlist1 = rptlist1.Where(a => productTypes.Contains(a.ProductType)).ToList();
                }

                sql.Close();
                db.Dispose();
                return rptlist1;
            }
        }

        public object RptLoanStatement(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptLoanStatement", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

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
                SqlParameter paramCustomerName = cmdTimesheet.Parameters.Add("@CustomerName", SqlDbType.NVarChar);
                paramCustomerName.Value = (object)Search.sSearch ?? DBNull.Value;

                SqlParameter paramAccountNumber = cmdTimesheet.Parameters.Add("@AccountNumber", SqlDbType.NVarChar);
                paramAccountNumber.Value = (object)Search.AccountNumber ?? DBNull.Value;


                SqlParameter paramStart = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                paramStart.Value = Search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptLoanStatement> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptLoanStatement>(reader).ToList();
                reader.NextResult();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }


        //Get Data for Employee Perfomance report
        //public object RptEmployeePerfomanceList(DataTableSearch Search, User user)
        //{
        //    using (var db = new BSCCSLEntity())
        //    {
        //        string connectionstring = db.Database.Connection.ConnectionString;

        //        SqlConnection sql = new SqlConnection(connectionstring);
        //        SqlCommand cmdTimesheet = new SqlCommand("RptEmployeePerfomance", sql);
        //        cmdTimesheet.CommandType = CommandType.StoredProcedure;
        //        //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

        //        SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
        //        paramFromDate.Value = (object)Search.fromDate ?? DBNull.Value;

        //        SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
        //        paramToDate.Value = (object)Search.toDate ?? DBNull.Value;

        //        SqlParameter paramEmployeeName = cmdTimesheet.Parameters.Add("@EmployeeName", SqlDbType.NVarChar);
        //        paramEmployeeName.Value = (object)Search.EmployeeName ?? DBNull.Value;

        //        var IsHO = db.Branch.Where(c => c.BranchId == Search.BranchId).Select(s => s.IsHO).FirstOrDefault();
        //        if (IsHO && user.Role == Role.Admin)
        //        {
        //            SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
        //            paramBranchId.Value = DBNull.Value;
        //        }
        //        else
        //        {
        //            SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
        //            paramBranchId.Value = Search.BranchId;
        //        }

        //        SqlParameter paramEdt = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
        //        paramEdt.Value = Search.iDisplayStart;

        //        SqlParameter parampartpost = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
        //        parampartpost.Value = Search.iDisplayLength;

        //        //Execute the query
        //        sql.Open();

        //        //int result = cmdTimesheet.ExecuteNonQuery();
        //        var reader = cmdTimesheet.ExecuteReader();

        //        // Read Blogs from the first result set
        //        List<RptEmployeePerfomanceList> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptEmployeePerfomanceList>(reader).ToList();
        //        reader.NextResult();

        //        int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

        //        var data = new
        //        {
        //            sEcho = Search.sEcho,
        //            iTotalRecords = rptlist.Count(),
        //            iTotalDisplayRecords = rptlist.Count(),
        //            aaData = rptlist,
        //        };
        //        sql.Close();
        //        db.Dispose();
        //        return data;
        //    }
        //}


        public object RptEmployeePerfomanceList(Guid branchId, string EmployeeName, DateTime? fromDate, DateTime? ToDate, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptEmployeePerfomance_New", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                if (string.IsNullOrEmpty(fromDate.ToString()))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.VarChar);
                    paramfinYear.Value = Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrEmpty(ToDate.ToString()))
                {
                    if (string.IsNullOrEmpty(fromDate.ToString()))
                    {
                        SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.VarChar);
                        paramfinYear.Value = DBNull.Value;
                    }
                    else
                    {
                        SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.VarChar);
                        paramfinYear.Value = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.VarChar);
                    paramfinYear.Value = Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");
                }
                SqlParameter paramEmployeeName = cmdTimesheet.Parameters.Add("@EmployeeName", SqlDbType.NVarChar);
                paramEmployeeName.Value = (object)EmployeeName ?? DBNull.Value;

                var IsHO = db.Branch.Where(c => c.BranchId == branchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = branchId;
                }


                //Execute the query
                sql.Open();

                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptEmployeePerfomanceList> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptEmployeePerfomanceList>(reader).ToList();
                reader.NextResult();


                var data = new
                {
                    rptlist = rptlist,
                };
                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object RptAgentPerfomanceList(Guid branchId, string EmployeeName, DateTime? fromDate, DateTime? ToDate, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptAgentPerfomance", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                if (string.IsNullOrEmpty(fromDate.ToString()))
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.VarChar);
                    paramfinYear.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.VarChar);
                    paramfinYear.Value = Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrEmpty(ToDate.ToString()))
                {
                    if (string.IsNullOrEmpty(fromDate.ToString()))
                    {
                        SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.VarChar);
                        paramfinYear.Value = DBNull.Value;
                    }
                    else
                    {
                        SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.VarChar);
                        paramfinYear.Value = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                }
                else
                {
                    SqlParameter paramfinYear = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.VarChar);
                    paramfinYear.Value = Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");
                }
                SqlParameter paramEmployeeName = cmdTimesheet.Parameters.Add("@EmployeeName", SqlDbType.NVarChar);
                paramEmployeeName.Value = (object)EmployeeName ?? DBNull.Value;

                var IsHO = db.Branch.Where(c => c.BranchId == branchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = branchId;
                }


                //Execute the query
                sql.Open();

                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptEmployeePerfomanceList> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptEmployeePerfomanceList>(reader).ToList();
                reader.NextResult();


                var data = new
                {
                    rptlist = rptlist,
                };
                sql.Close();
                db.Dispose();
                return data;
            }
        }


        public object GetEmployeeListByBranchId(Guid? Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = db.User.Where(s => s.IsDelete == false && s.IsActive == true).Select(s => new { AgentId = s.UserId, AgentName = s.FirstName + " " + s.LastName, BranchId = s.BranchId, AgentCode = s.UserCode }).AsQueryable();
                if (Id != null)
                {
                    var IsHO = db.Branch.Where(c => c.BranchId == Id).Select(s => s.IsHO).FirstOrDefault();
                    if (IsHO)
                    {
                        return list.ToList();
                    }
                    else
                    {
                        var agnetList = list.Where(a => a.BranchId == Id).ToList();
                        return agnetList;
                    }
                }
                else
                {
                    return list.ToList();
                }
            }
        }


        public object RptLoanStatementDetails(string AccountNumber, Guid BranchId, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                LoanAmountisation loanAmountisation = new LoanAmountisation();

                CustomerProduct objCustomerProduct = new CustomerProduct();

                var IsHO = db.Branch.Where(c => c.BranchId == BranchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                    objCustomerProduct = db.CustomerProduct.Where(a => a.AccountNumber.ToLower() == AccountNumber.ToLower() && a.IsActive == true && a.IsDelete == false && a.ProductType == ProductType.Loan).FirstOrDefault();
                else
                    objCustomerProduct = db.CustomerProduct.Where(a => a.AccountNumber.ToLower() == AccountNumber.ToLower() && a.IsActive == true && a.IsDelete == false && a.ProductType == ProductType.Loan && a.BranchId == BranchId).FirstOrDefault();

                string CustomerName = "";
                string LoanAccountNumber = "";
                decimal LoanAmount = 0;
                decimal LoanTotalAmounttoPay = 0;
                DateTime? DisbursedDate = new DateTime();
                if (objCustomerProduct != null)
                {
                    Loan objLoan = db.Loan.Where(x => x.IsDelete == false && x.CustomerProductId == objCustomerProduct.CustomerProductId).FirstOrDefault();
                    if (objLoan != null)
                    {
                        loanAmountisation.PrincipalAmount = (decimal)objLoan.LastPrincipalAmount;
                        loanAmountisation.LoanIntrestRate = objLoan.LoanIntrestRate;
                        loanAmountisation.CustomerProductId = objLoan.CustomerProductId;
                        loanAmountisation.Term = Convert.ToInt32((objLoan.Term != "0" ? objLoan.Term : "1"));
                        loanAmountisation.InstallmentDate = (DateTime)objLoan.InstallmentDate;
                        loanAmountisation.LoanId = objLoan.LoanId;
                        LoanAmount = objLoan.LoanAmount;
                        LoanTotalAmounttoPay = (decimal)objLoan.TotalAmountToPay;

                        UpdateLoanStatus objUpdateLoanStatus = db.UpdateLoanStatus.Where(a => a.LoanStatus == LoanStatus.Disbursed && a.LoanId == objLoan.LoanId).FirstOrDefault();
                        if (objUpdateLoanStatus != null)
                            DisbursedDate = objUpdateLoanStatus.UpdatedDate;
                        else
                            DisbursedDate = null;

                    }
                    CustomerPersonalDetail objCustomerPersonalDetail = db.CustomerPersonalDetail.Where(a => a.CustomerId == objCustomerProduct.CustomerId && a.IsDelete == false).FirstOrDefault();
                    if (objCustomerPersonalDetail != null)
                    {
                        CustomerName = objCustomerPersonalDetail.FirstName + " " + objCustomerPersonalDetail.MiddleName + " " + objCustomerPersonalDetail.LastName;
                        LoanAccountNumber = objCustomerProduct.AccountNumber;
                    }
                }



                var result = LoanAmountisation(loanAmountisation);




                var data = new
                {
                    result = result,
                    LoanTerm = loanAmountisation.Term,
                    InterestRate = loanAmountisation.LoanIntrestRate,
                    LoanAmount = LoanAmount,
                    CustomerName = CustomerName,
                    LoanTotalAmounttoPay = LoanTotalAmounttoPay,
                    DisbursedDate = DisbursedDate,
                    LoanAccountNumber = LoanAccountNumber,
                };

                db.Dispose();
                return data;
            }
        }

        public object LoanAmountisation(LoanAmountisation loanAmountisation)
        {
            using (var db = new BSCCSLEntity())
            {
                List<RptLoanStatementDetails> Installments = new List<RptLoanStatementDetails>();
                decimal TotalDebitAmount = 0;
                decimal TotalCreditAmount = 0;
                string TotalBalanceAmount = "";
                if (loanAmountisation.LoanId != null)
                {


                    string connectionstring = db.Database.Connection.ConnectionString;

                    SqlConnection sql = new SqlConnection(connectionstring);
                    SqlCommand cmdTimesheet = new SqlCommand("RptLoanStatementDetails", sql);
                    cmdTimesheet.CommandType = CommandType.StoredProcedure;

                    SqlParameter CustomerProductId = cmdTimesheet.Parameters.Add("@CustomerProductId", SqlDbType.UniqueIdentifier);
                    CustomerProductId.Value = loanAmountisation.CustomerProductId;

                    SqlParameter LoanId = cmdTimesheet.Parameters.Add("@LoanId", SqlDbType.UniqueIdentifier);
                    LoanId.Value = loanAmountisation.LoanId;

                    SqlParameter FromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    FromDate.Value = null;

                    SqlParameter ToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    ToDate.Value = null;

                    sql.Open();
                    var reader = cmdTimesheet.ExecuteReader();

                    List<RptLoanStatementDetails> listLoanAmountisation = ((IObjectContextAdapter)db).ObjectContext.Translate<RptLoanStatementDetails>(reader).ToList();
                    reader.NextResult();
                    Installments = listLoanAmountisation;


                    decimal Balance = 0;
                    foreach (var item in Installments)
                    {
                        decimal BalanceAmt = 0;
                        if (item.Debit != 0)
                            Balance = Balance - Convert.ToDecimal(item.Debit);
                        if (item.Credit != 0)
                            Balance = Balance + Convert.ToDecimal(item.Credit);
                        BalanceAmt = Balance;
                        if (Balance < 0)
                        {
                            BalanceAmt = BalanceAmt * -1;
                            item.Balance = BalanceAmt.ToString() + " DR";
                        }
                        else
                            item.Balance = BalanceAmt.ToString() + " CR";
                    }



                    TotalDebitAmount = (decimal)Installments.Sum(a => a.Debit);
                    TotalCreditAmount = (decimal)Installments.Sum(a => a.Credit);
                    TotalBalanceAmount = Installments.OrderByDescending(a => a.Date).FirstOrDefault().Balance;
                    sql.Close();


                }
                var data = new
                {
                    LoanAmountisationList = Installments,
                    TotalDebitAmount = TotalDebitAmount,
                    TotalCreditAmount = TotalCreditAmount,
                    TotalBalanceAmount = TotalBalanceAmount
                };

                db.Dispose();
                return data;
            }
        }



        //public object LoanAmountisation(LoanAmountisation loanAmountisation)
        //{
        //    using (var db = new BSCCSLEntity())
        //    {
        //        List<ListLoanAmountisation> Installments = new List<ListLoanAmountisation>();
        //        decimal monthlyInstallment = 0;

        //        if (loanAmountisation.LoanId != null)
        //        {


        //            string connectionstring = db.Database.Connection.ConnectionString;

        //            SqlConnection sql = new SqlConnection(connectionstring);
        //            SqlCommand cmdTimesheet = new SqlCommand("LoanAmountisation", sql);
        //            cmdTimesheet.CommandType = CommandType.StoredProcedure;
        //            SqlParameter PrincipalAmount = cmdTimesheet.Parameters.Add("@PrincipalAmount", SqlDbType.Decimal);
        //            PrincipalAmount.Value = loanAmountisation.PrincipalAmount;
        //            SqlParameter InterestRate = cmdTimesheet.Parameters.Add("@InterestRate", SqlDbType.Decimal);
        //            InterestRate.Value = loanAmountisation.LoanIntrestRate;
        //            SqlParameter TotalMonth = cmdTimesheet.Parameters.Add("@TotalMonth", SqlDbType.Int);
        //            TotalMonth.Value = (loanAmountisation.Term != 0 ? loanAmountisation.Term : 1);

        //            SqlParameter InstallmentDate = cmdTimesheet.Parameters.Add("@InstallmentDate", SqlDbType.DateTime);
        //            InstallmentDate.Value = loanAmountisation.InstallmentDate;


        //            SqlParameter LoanId = cmdTimesheet.Parameters.Add("@LoanId", SqlDbType.UniqueIdentifier);
        //            LoanId.Value = loanAmountisation.LoanId;

        //            SqlParameter IsPrePayment = cmdTimesheet.Parameters.Add("@IsPrePayment", SqlDbType.Int);
        //            IsPrePayment.Value = 0;

        //            sql.Open();
        //            var reader = cmdTimesheet.ExecuteReader();

        //            List<ListLoanAmountisation> listLoanAmountisation = ((IObjectContextAdapter)db).ObjectContext.Translate<ListLoanAmountisation>(reader).ToList();
        //            reader.NextResult();

        //            monthlyInstallment = ((IObjectContextAdapter)db).ObjectContext.Translate<decimal>(reader).FirstOrDefault();

        //            //data.ListLoanAmountisation = listLoanAmountisation;
        //            //data.MonthlyInstallmentAmount = monthlyInstallment;


        //            var PaidInstallments = (from r in db.RDPayment.Where(a => a.CustomerProductId == loanAmountisation.CustomerProductId && a.RDPaymentType == RDPaymentType.Installment)
        //                                    select new ListLoanAmountisation
        //                                    {
        //                                        MonthlyEMI = r.Amount,
        //                                        PrincipalAmt = r.PrincipalAmount.Value,
        //                                        Interest = r.InterestAmount.Value,
        //                                        Installmentdate = r.CreatedDate,
        //                                        IsPaid = r.IsPaid
        //                                    }).ToList();
        //            DateTime lastEMI;

        //            if (PaidInstallments.Count() > 0)
        //            {
        //                lastEMI = PaidInstallments.Max(a => a.Installmentdate);
        //            }
        //            else
        //            {
        //                lastEMI = DateTime.Now.Date;
        //            }

        //            var EMIAmountisation = listLoanAmountisation.Where(a => a.Installmentdate > lastEMI).ToList();

        //            Installments = PaidInstallments.Union(EMIAmountisation).OrderBy(a => a.Installmentdate).ToList();
        //            decimal LoanAmount = 0;

        //            Loan objLoan = db.Loan.Where(x => x.LoanId == loanAmountisation.LoanId).FirstOrDefault();
        //            if (objLoan != null)
        //                LoanAmount = objLoan.LoanAmount;
        //            Installments = Installments.Where(a => a.IsPaid == true && a.MonthlyEMI != 0).ToList();

        //            decimal OpeningBalance = 0;
        //            decimal ClosingBalance = 0;
        //            int Count = 0;
        //            int RowCount = 0;
        //            foreach (var item in Installments)
        //            {
        //                if (Count == 0)
        //                {
        //                    OpeningBalance = LoanAmount;
        //                    ClosingBalance = OpeningBalance - item.PrincipalAmt;
        //                }
        //                else
        //                {
        //                    OpeningBalance = Installments[RowCount].ClosingBalance;
        //                    ClosingBalance = OpeningBalance - item.PrincipalAmt;
        //                    if (ClosingBalance < 0)
        //                        ClosingBalance = 0;
        //                    RowCount++;
        //                }
        //                Count++;
        //                item.OpeningBalance = OpeningBalance;
        //                item.ClosingBalance = ClosingBalance;
        //            }
        //            sql.Close();


        //        }
        //        var data = new
        //        {
        //            LoanAmountisationList = Installments,
        //            MonthlyInstallmentAmount = monthlyInstallment,
        //        };

        //        db.Dispose();
        //        return data;
        //    }
        //}

        public object RptPrematureProductList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                DateTime date = DateTime.Now;
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptPrematureProductList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = (object)Search.fromDate ?? DBNull.Value;

                SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = (object)Search.toDate ?? DBNull.Value;

                SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@CustomerName", SqlDbType.NVarChar);
                paramAgentName.Value = (object)Search.sSearch ?? DBNull.Value;

                SqlParameter paramPrematurePercentage = cmdTimesheet.Parameters.Add("@PrematurePercentage", SqlDbType.NVarChar);
                paramPrematurePercentage.Value = (object)Search.PrematurePercentage ?? DBNull.Value;


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
                paramStart.Value = Search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptPrematureProductList> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptPrematureProductList>(reader).ToList();
                reader.NextResult();



                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object RptInterestDepositList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {


                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptInterestDeposit", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                SqlParameter paramFromDate = cmdTimesheet.Parameters.Add("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = (object)Search.fromDate ?? DBNull.Value;

                SqlParameter paramToDate = cmdTimesheet.Parameters.Add("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = (object)Search.toDate ?? DBNull.Value;

                SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@CustomerName", SqlDbType.NVarChar);
                paramAgentName.Value = (object)Search.sSearch ?? DBNull.Value;

                SqlParameter paramProductName = cmdTimesheet.Parameters.Add("@ProductName", SqlDbType.NVarChar);
                paramProductName.Value = (object)Search.ProductName ?? DBNull.Value;


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
                paramStart.Value = Search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                decimal TotalInterest = 0;
                // Read Blogs from the first result set
                List<RptInterestDepositList> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptInterestDepositList>(reader).ToList();
                reader.NextResult();
                TotalInterest = Convert.ToDecimal(rptlist.Select(x => x.TotalInterest).Sum());


                //List<RptInterestDepositTotalCount> TotalRecordsList = ((IObjectContextAdapter)db).ObjectContext.Translate<RptInterestDepositTotalCount>(reader).ToList();
                //int Count = 0;
                //Count = TotalRecordsList.Count();
                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                    TotalInterest = TotalInterest,
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

    }
}
