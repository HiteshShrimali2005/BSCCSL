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

            List<DailyProcess> lst = new List<DailyProcess>();
            using (var db = new BSCCSLEntity())
            {

                // var list = db.DailyProcessConsoleDates.AsQueryable();

                string connectionstring = db.Database.Connection.ConnectionString;
                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmd = new SqlCommand("GetDailyProcessLoanConsoleDates", sql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 3600;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter               

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmd.ExecuteReader();

                // Read Blogs from the first result set
                lst = ((IObjectContextAdapter)db).ObjectContext.Translate<DailyProcess>(reader).ToList();

                sql.Close();
                db.Dispose();
            }

            DateTime date = lst.Where(x => x.DailyProcessCode == "020").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            DeductAmountofLoanFromSaving(date);
            date = lst.Where(x => x.DailyProcessCode == "021").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            CalculateLatePaymentCharges(date);
            date = lst.Where(x => x.DailyProcessCode == "022").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            DailyInterestCalculationForFlexiLoan(date);
        }

        public object DailyInterestCalculationForFlexiLoan(DateTime Date)
        {
            int days = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            for (int i = 0; i < days; i++)
            {
                Date = Date.AddDays(1);
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
                        SqlParameter paramdate = cmd.Parameters.Add("@Date",SqlDbType.DateTime);
                        paramdate.Value = (object)Date;

                        //Execute the query
                        sql.Open();
                        //int result = cmdTimesheet.ExecuteNonQuery();
                        var reader = cmd.ExecuteReader();

                        // Read Blogs from the first result set
                        var result = ((IObjectContextAdapter)db).ObjectContext.Translate<object>(reader).FirstOrDefault();
                        sql.Close();
                        db.Dispose();

                        //return result;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogService.InsertLog(ex);
                    return false;
                }
            }
            return true;

        }

        public object DeductAmountofLoanFromSaving(DateTime Date)
        {
            int days = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            for (int i = 0; i < days; i++)
            {
                Date = Date.AddDays(1);
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
                        SqlParameter paramdate = cmd.Parameters.Add("@Date", SqlDbType.DateTime);
                        paramdate.Value = (object)Date;

                        //Execute the query
                        sql.Open();
                        //int result = cmdTimesheet.ExecuteNonQuery();
                        var reader = cmd.ExecuteReader();

                        // Read Blogs from the first result set
                        var result = ((IObjectContextAdapter)db).ObjectContext.Translate<object>(reader).FirstOrDefault();
                        sql.Close();
                        db.Dispose();

                        //return result;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogService.InsertLog(ex);
                    return false;
                }
            }
            return true;
        }

        public object CalculateLatePaymentCharges(DateTime Date)
        {
            int days = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            for (int i = 0; i < days; i++)
            {
                Date = Date.AddDays(1);
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
                        SqlParameter paramdate = cmd.Parameters.Add("@Date", SqlDbType.DateTime);
                        paramdate.Value = (object)Date;

                        //Execute the query
                        sql.Open();
                        //int result = cmdTimesheet.ExecuteNonQuery();
                        var reader = cmd.ExecuteReader();

                        // Read Blogs from the first result set
                        var result = ((IObjectContextAdapter)db).ObjectContext.Translate<object>(reader).FirstOrDefault();
                        sql.Close();
                        db.Dispose();


                        //return result;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogService.InsertLog(ex);
                    return false;

                }

            }
            return true;
        }

    }
}
