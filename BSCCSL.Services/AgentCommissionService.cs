using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class AgentCommissionService
    {

        public object GetAgentCommissionList(ReportSearch Search, User user)
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

                SqlParameter paramUserStatus = cmdTimesheet.Parameters.Add("@UserStatus", SqlDbType.Int);
                paramUserStatus.Value = (object)Search.UserStatus ?? DBNull.Value;


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

        public object GetCommissionSubData(CommissionSubdata Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("AgentCommissionByMonthAndYear", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                using (var table = new DataTable())
                {
                    table.Columns.Add("AgentId");
                    table.Columns.Add("Month");
                    table.Columns.Add("Year");

                    foreach (string ss in Search.UserIDs)
                    {
                        string[] sdata = ss.Split('_');
                        table.Rows.Add(new Guid(sdata[0]), Convert.ToInt16(sdata[1]), Convert.ToInt32(sdata[2]));
                    }
                    SqlParameter paramAgentId = cmdTimesheet.Parameters.Add("@list", SqlDbType.Structured);
                    paramAgentId.TypeName = "dbo.CommData";
                    paramAgentId.Value = table;
                }

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

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                var rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptAgentCommissionByMonth>(reader).ToList();


                List<RptAgentCommissionByMonth> rpt = new List<RptAgentCommissionByMonth>();


                if (Search.ProductTypes.Count > 0)
                {
                    rpt = rptlist.Where(a => Search.ProductTypes.Contains(a.ProductType)).ToList();
                }
                else
                {
                    rpt = rptlist.ToList();
                }

                sql.Close();
                db.Dispose();
                return rpt.OrderBy(s => s.Date);
            }
        }

        public object AgentHierarchyCommissionByMonthAndYear(CommissionSubdata Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("AgentHierarchyCommissionByMonthAndYear", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                using (var table = new DataTable())
                {
                    table.Columns.Add("AgentId");
                    table.Columns.Add("Month");
                    table.Columns.Add("Year");

                    foreach (string ss in Search.UserIDs)
                    {
                        string[] sdata = ss.Split('_');
                        table.Rows.Add(new Guid(sdata[0]), Convert.ToInt16(sdata[1]), Convert.ToInt32(sdata[2]));
                    }
                    SqlParameter paramAgentId = cmdTimesheet.Parameters.Add("@list", SqlDbType.Structured);
                    paramAgentId.TypeName = "dbo.CommData";
                    paramAgentId.Value = table;
                }

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

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                var rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptAgentCommissionByMonth>(reader).ToList();


                List<RptAgentCommissionByMonth> rpt = new List<RptAgentCommissionByMonth>();


                if (Search.ProductTypes.Count > 0)
                {
                    rpt = rptlist.Where(a => Search.ProductTypes.Contains(a.ProductType)).ToList();
                }
                else
                {
                    rpt = rptlist.ToList();
                }

                sql.Close();
                db.Dispose();
                return rpt.OrderBy(s => s.Date);
            }
        }

        public bool SaveAgentCommissionPayment(CommissionData CommData)
        {
            using (var db = new BSCCSLEntity())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var AgentData = (from c in CommData.Commission
                                         group new { c } by new { c.AgentId } into alist
                                         select new CData
                                         {
                                             AgentId = alist.Key.AgentId,
                                             CHistory = alist.Select(s => s.c).ToList()
                                         }).ToList();

                        DateTime today = DateTime.Now;

                        foreach (CData ss in AgentData)
                        {
                            var CustId = db.User.Where(s => s.UserId == ss.AgentId).Select(s => s.CustomerId).FirstOrDefault();
                            if (CustId != null)
                            {
                                var CustProduct = db.CustomerProduct.Where(a => a.CustomerId == CustId && a.ProductType == ProductType.Saving_Account && a.IsActive == true).FirstOrDefault();
                                if (CustProduct != null)
                                {
                                    foreach (RptAgentCommissionByMonth IId in ss.CHistory)
                                    {
                                        //if (IId.ProductType == ProductType.Fixed_Deposit)
                                        //{
                                        //    var Cp = new CustomerProduct() { CustomerProductId = IId.CommissionId, CommissionPaid = true };
                                        //    db.CustomerProduct.Attach(Cp);
                                        //    db.Entry(Cp).Property(s => s.CommissionPaid).IsModified = true;
                                        //}
                                        //else if (IId.ProductType == ProductType.Recurring_Deposit)
                                        //{
                                        //    var Rp = new RDPayment() { RDPaymentId = IId.CommissionId, CommissionPaid = true };
                                        //    db.RDPayment.Attach(Rp);
                                        //    db.Entry(Rp).Property(s => s.CommissionPaid).IsModified = true;
                                        //}

                                        //var ac = new AgentCommission() { AgentCommissionId = IId.CommissionId, IsPaid = true, PaidBy = CommData.PaidBy, PaidDate = today };
                                        //db.AgentCommission.Attach(ac);
                                        //db.Entry(ac).Property(s => s.IsPaid).IsModified = true;
                                        //db.Entry(ac).Property(s => s.PaidBy).IsModified = true;
                                        //db.Entry(ac).Property(s => s.PaidDate).IsModified = true;
                                        AgentCommission objAgentCommission = db.AgentCommission.Where(x => x.AgentCommissionId == IId.CommissionId).FirstOrDefault();
                                        if (objAgentCommission != null)
                                        {
                                            objAgentCommission.IsPaid = true;
                                            objAgentCommission.PaidBy = CommData.PaidBy;
                                            objAgentCommission.PaidDate = today;
                                            db.SaveChanges();
                                        }
                                    }
                                }

                                var HData = (from h in ss.CHistory
                                             group new { h } by new { h.CustomerId, h.ProductType, h.Month, h.Year } into Hlist
                                             select new RptAgentCommissionByMonth
                                             {
                                                 Month = Hlist.Key.Month,
                                                 Year = Hlist.Key.Year,
                                                 ProductType = Hlist.Key.ProductType,
                                                 CustomerId = Hlist.Key.CustomerId,
                                                 agentCommission = Hlist.Select(s => s.h.agentCommission).Sum()
                                             }).ToList();

                                if (CustProduct != null)
                                {
                                    List<AgentCommissionHistory> _history = new List<AgentCommissionHistory>();
                                    foreach (RptAgentCommissionByMonth ddata in HData)
                                    {
                                        AgentCommissionHistory _comm = new AgentCommissionHistory();
                                        _comm.AccountNumber = CustProduct.AccountNumber;
                                        _comm.AgentId = ss.AgentId;
                                        _comm.CustomerId = ddata.CustomerId;
                                        _comm.BranchId = CommData.BranchId;
                                        _comm.Month = ddata.Month;
                                        _comm.Year = ddata.Year;
                                        _comm.PaidAmount = ddata.agentCommission ?? 0;
                                        _comm.PaidBy = CommData.PaidBy;
                                        _comm.PaidDate = DateTime.Now;
                                        _comm.ProductType = ddata.ProductType;
                                        _history.Add(_comm);
                                    }
                                    db.AgentCommissionHistory.AddRange(_history);

                                    decimal DepositAmount = ss.CHistory.Select(s => s.agentCommission).Sum() ?? 0;


                                    SqlParameter CustomerProductId = new SqlParameter("CustomerProductId", CustProduct.CustomerProductId);
                                    SqlParameter CustomerId = new SqlParameter("CustomerId", CustId);
                                    SqlParameter Amount = new SqlParameter("Amount", DepositAmount);
                                    SqlParameter Balance = new SqlParameter("Balance", CustProduct.UpdatedBalance + DepositAmount);
                                    SqlParameter Type = new SqlParameter("Type", TypeCRDR.CR);
                                    SqlParameter CreatedBy = new SqlParameter("CreatedBy", CommData.PaidBy);
                                    SqlParameter CreatedDate = new SqlParameter("CreatedDate", DateTime.Now);
                                    SqlParameter ModifiedBy = new SqlParameter("ModifiedBy", (object)CommData.PaidBy ?? DBNull.Value);
                                    SqlParameter ModifiedDate = new SqlParameter("ModifiedDate", (object)DateTime.Now ?? DBNull.Value);
                                    SqlParameter TransactionType = new SqlParameter("TransactionType", 4);
                                    SqlParameter CheckNumber = new SqlParameter("CheckNumber", DBNull.Value);
                                    SqlParameter ChequeDate = new SqlParameter("ChequeDate", DBNull.Value);
                                    SqlParameter BounceReason = new SqlParameter("BounceReason", DBNull.Value);
                                    SqlParameter Penalty = new SqlParameter("Penalty", DBNull.Value);
                                    SqlParameter ChequeClearDate = new SqlParameter("ChequeClearDate", DBNull.Value);
                                    SqlParameter Status = new SqlParameter("Status", (object)0 ?? DBNull.Value);
                                    SqlParameter TransactionTime = new SqlParameter("TransactionTime", DateTime.Now);
                                    SqlParameter BankName = new SqlParameter("BankName", DBNull.Value);
                                    SqlParameter BranchId = new SqlParameter("BranchId", CommData.BranchId);
                                    SqlParameter DescIndentify = new SqlParameter("DescIndentify", 15);
                                    SqlParameter RefCustomerProductId = new SqlParameter("RefCustomerProductId", DBNull.Value);
                                    SqlParameter NEFTNumber = new SqlParameter("NEFTNumber", DBNull.Value);
                                    SqlParameter NEFTDate = new SqlParameter("NEFTDate", DBNull.Value);

                                    SqlParameter Id = new SqlParameter
                                    {
                                        ParameterName = "@Id",
                                        SqlDbType = System.Data.SqlDbType.UniqueIdentifier,
                                        Size = 50,
                                        Direction = System.Data.ParameterDirection.Output
                                    };

                                    var transc = db.Database.SqlQuery<object>("SaveTransaction @CustomerProductId,@CustomerId, @Amount ,@Balance, @Type, @CreatedBy, @CreatedDate,@ModifiedBy, @ModifiedDate,@TransactionType,@CheckNumber,@ChequeDate,@BounceReason,@Penalty,@ChequeClearDate,@Status,@TransactionTime,@BankName,@BranchId,@DescIndentify,@RefCustomerProductId,@NEFTNumber,@NEFTDate,@Id  OUT", CustomerProductId, CustomerId, Amount, Balance, Type, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, TransactionType, CheckNumber, ChequeDate, BounceReason, Penalty, ChequeClearDate, Status, TransactionTime, BankName, BranchId, DescIndentify, RefCustomerProductId, NEFTNumber, NEFTDate, Id).FirstOrDefault();

                                    Guid TransactionId = Guid.Parse(Id.Value.ToString());

                                    //CustProduct.Balance = CustProduct.UpdatedBalance + DepositAmount;
                                    //CustProduct.UpdatedBalance = CustProduct.UpdatedBalance + DepositAmount;
                                    //db.Entry(CustProduct).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }

                        trans.Commit();
                        return true;

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public object RptAgentHierarchy(DataTableSearch search, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("RptAgentHierarchy", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                if (!string.IsNullOrEmpty(search.AgentName))
                {
                    SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                    paramAgentName.Value = search.AgentName.Trim().ToLower();
                }
                else
                {
                    SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                    paramAgentName.Value = DBNull.Value;
                }



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

                SqlParameter paramStart = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                paramStart.Value = search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<RptAgentHierarchy> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<RptAgentHierarchy>(reader).ToList();
                reader.NextResult();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }
    }
}
