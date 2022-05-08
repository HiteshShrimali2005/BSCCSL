using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class LoanSchedulerService
    {

        public void RunSchedular()
        {
            Exception ex = new Exception("This is Custom Exception for Loan Console");
            ErrorLogService.InsertLog(ex);

            DeductAmountofLoanFromSaving();
            CalculateLatePaymentCharges();
            DailyInterestCalculationForFlexiLoan();
        }

        public object DailyInterestCalculationForFlexiLoan()
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    //var result = db.Database.SqlQuery<object>("InterestCalculation @Date",
                    //new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

                    string connectionstring = db.Database.Connection.ConnectionString;
                    SqlConnection sql = new SqlConnection(connectionstring);
                    SqlCommand cmd = new SqlCommand("InterestCalculationforFlexiLoan", sql);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                    SqlParameter paramdate = cmd.Parameters.Add("@Date", DateTime.Now);
                    paramdate.Value = (object)DateTime.Now;

                    //Execute the query
                    sql.Open();
                    //int result = cmdTimesheet.ExecuteNonQuery();
                    var reader = cmd.ExecuteReader();

                    // Read Blogs from the first result set
                    var result = ((IObjectContextAdapter)db).ObjectContext.Translate<object>(reader).FirstOrDefault();
                    sql.Close();
                    db.Dispose();

                    return result;
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return false;
            }

        }

        public object DeductAmountofLoanFromSaving()
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    //var result = db.Database.SqlQuery<object>("DeductAmountofLoanFromSaving @Date",
                    //  new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

                    string connectionstring = db.Database.Connection.ConnectionString;
                    SqlConnection sql = new SqlConnection(connectionstring);
                    SqlCommand cmd = new SqlCommand("DeductAmountofLoanFromSaving", sql);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                    SqlParameter paramdate = cmd.Parameters.Add("@Date", DateTime.Now);
                    paramdate.Value = (object)DateTime.Now;

                    //Execute the query
                    sql.Open();
                    //int result = cmdTimesheet.ExecuteNonQuery();
                    var reader = cmd.ExecuteReader();

                    // Read Blogs from the first result set
                    var result = ((IObjectContextAdapter)db).ObjectContext.Translate<object>(reader).FirstOrDefault();
                    sql.Close();
                    db.Dispose();

                    return result;
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return false;
            }
        }

        public object CalculateLatePaymentCharges()
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    //var result = db.Database.SqlQuery<object>("CalculateLatePaymentCharges @Date",
                    //  new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

                    string connectionstring = db.Database.Connection.ConnectionString;
                    SqlConnection sql = new SqlConnection(connectionstring);
                    SqlCommand cmd = new SqlCommand("CalculateLatePaymentChargesForLoan", sql);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                    SqlParameter paramdate = cmd.Parameters.Add("@Date", DateTime.Now);
                    paramdate.Value = (object)DateTime.Now;

                    //Execute the query
                    sql.Open();
                    //int result = cmdTimesheet.ExecuteNonQuery();
                    var reader = cmd.ExecuteReader();

                    // Read Blogs from the first result set
                    var result = ((IObjectContextAdapter)db).ObjectContext.Translate<object>(reader).FirstOrDefault();
                    sql.Close();
                    db.Dispose();


                    return result;
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return false;
            }
        }



    }
}
