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

namespace BSCCSL.Services
{
    public class GeneralLedgerService
    {
        public object GetGeneralLedgerList(DateTime FromDate, DateTime ToDate, Guid BranchId, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetGeneralLedgerList", sql);
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


                //SqlParameter paramStart = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                //paramStart.Value = Search.iDisplayStart;

                //SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                //paramLength.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<GeneralLedgerViewModel> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<GeneralLedgerViewModel>(reader).ToList();
                reader.NextResult();

                decimal Openingbalance = 0;
                decimal TotalCreditAmount = rptlist.Sum(x => x.Credit);
                decimal TotalDebitAmount = rptlist.Sum(x => x.Debit);
                decimal ClosingCreditAmount = 0;
                decimal ClosingDebitAmount = 0;
                ClosingCreditAmount = Openingbalance + TotalCreditAmount;
                ClosingDebitAmount = Openingbalance + TotalDebitAmount;

                //int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                decimal Balance = 0;
                foreach (var item in rptlist)
                {
                    if (item.Debit != 0)
                        Balance = Balance + Convert.ToDecimal(item.Debit);
                    if (item.Credit != 0)
                        Balance = Balance - Convert.ToDecimal(item.Credit);

                    item.Balance = Balance;
                }



                var data = new
                {
                    aaData = rptlist,
                    TotalCreditAmount = TotalCreditAmount,
                    TotalDebitAmount = TotalDebitAmount,
                    Openingbalance = Openingbalance,
                    ClosingCreditAmount = ClosingCreditAmount,
                    ClosingDebitAmount = ClosingDebitAmount
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }


    }
}
