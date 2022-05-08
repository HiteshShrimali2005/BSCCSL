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
    public class ActivateRDService
    {

        public object GetList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetFreezedRDList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                SqlParameter paramuser = cmdTimesheet.Parameters.Add("@CustomerName", SqlDbType.NVarChar);
                paramuser.Value = (object)Search.CustomerName ?? DBNull.Value;
                SqlParameter paramagent = cmdTimesheet.Parameters.Add("@AccountNumber", SqlDbType.NVarChar);
                paramagent.Value = (object)Search.AccountNumber ?? DBNull.Value;


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
                List<FreezedRDModel> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<FreezedRDModel>(reader).ToList();
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

        public bool UnFreezedRDAccount(string AccountNumber)
        {
            using (var db = new BSCCSLEntity())
            {
                if (!string.IsNullOrEmpty(AccountNumber))
                {
                    string connectionstring = db.Database.Connection.ConnectionString;

                    SqlConnection sql = new SqlConnection(connectionstring);
                    SqlCommand cmdTimesheet = new SqlCommand("ActivateFreezeRD", sql);
                    cmdTimesheet.CommandType = CommandType.StoredProcedure;
                    //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                    SqlParameter paramuser = cmdTimesheet.Parameters.Add("@AccountNumber", SqlDbType.NVarChar);
                    paramuser.Value = (object)AccountNumber ?? DBNull.Value;
                    
                    //Execute the query
                    sql.Open();

                    //int result = cmdTimesheet.ExecuteNonQuery();
                    var reader = cmdTimesheet.ExecuteReader();

                    // Read Blogs from the first result set
                    sql.Close();
                    db.Dispose();
                }

                return true;
            }
        }


    }
}
