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
    public class DDSPaymentService
    {

        public object GetDDSPaymentList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetDDSPaymentList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramuser = cmdTimesheet.Parameters.Add("@CustomerName", SqlDbType.NVarChar);
                paramuser.Value = (object)Search.CustomerName ?? DBNull.Value;
                SqlParameter paramagent = cmdTimesheet.Parameters.Add("@AgentName", SqlDbType.NVarChar);
                paramagent.Value = (object)Search.AgentName ?? DBNull.Value;


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

                // Read Blogs from the first result set
                List<DDSPaymentListModel> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<DDSPaymentListModel>(reader).ToList();
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

        public bool AddPayment(DDSPaymentListModel ddspaymentmodel, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                TransactionService transactionService = new TransactionService();
                if (ddspaymentmodel != null)
                {
                    CustomerProduct objCustomerProduct = db.CustomerProduct.Where(x => x.CustomerProductId == ddspaymentmodel.SavingAccountCustomerProductId).FirstOrDefault();
                    if (objCustomerProduct != null)
                    {

                        Transactions transaction = new Transactions
                        {
                            CustomerProductId = ddspaymentmodel.SavingAccountCustomerProductId,
                            CustomerId = objCustomerProduct.CustomerId,
                            Amount = (decimal)ddspaymentmodel.Amount,
                            Type = TypeCRDR.CR,
                            CreatedBy = user.UserId,
                            CreatedDate = DateTime.Now,
                            TransactionType = TransactionType.DDSAmountTransfer,
                            DescIndentify = DescIndentify.DDSAmountTransfer,
                            BranchId = objCustomerProduct.BranchId,
                            Status = Status.Clear,
                            TransactionTime = DateTime.Now.AddSeconds(5)
                        };
                        transaction.TransactionId = transactionService.InsertTransctionUsingSP(transaction);

                        #region Replace the Description
                        Transactions objTransactions = db.Transaction.Where(x => x.TransactionId == transaction.TransactionId).FirstOrDefault();
                        if (objTransactions != null)
                        {
                            if (!string.IsNullOrEmpty(ddspaymentmodel.Description))
                            {
                                if (!string.IsNullOrEmpty(objTransactions.Description))
                                    objTransactions.Description = objTransactions.Description + ". " + ddspaymentmodel.Description;
                                else
                                    objTransactions.Description = ddspaymentmodel.Description;

                                db.SaveChanges();
                            }
                        }
                        #endregion
                    }
                }

                return true;
            }
        }


    }
}
