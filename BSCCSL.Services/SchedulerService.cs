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
using System.Threading;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class SchedulerService
    {
        SMSService smsService;

        public SchedulerService()
        {
            smsService = new SMSService();
        }


        public void RunSchedular()
        {
            //Exception ex = new Exception("This is Custom Exception for Console");
            // ErrorLogService.InsertLog(ex);
            List<DailyProcess> lst = new List<DailyProcess>();
            using (var db = new BSCCSLEntity())
            {

                // var list = db.DailyProcessConsoleDates.AsQueryable();

                string connectionstring = db.Database.Connection.ConnectionString;
                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmd = new SqlCommand("GetDailyProcessConsoleDates", sql);
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
            DateTime date;
            date = lst.Where(x => x.DailyProcessCode == "001").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            AddAmountofMISInSaving(date);
            ////DeductAmountofLoanFromSaving();
            date = lst.Where(x => x.DailyProcessCode == "002").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            DeductAmountofRDFromSaving(date);

            date = lst.Where(x => x.DailyProcessCode == "003").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            DeductAmountofTYPEFromSaving(date);

            //date = lst.Where(x => x.DailyProcessCode == "004").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            //DeductAmountofRIPFromSaving(date);

            ////date = lst.Where(x => x.DailyProcessCode == "005").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            ////CalculateLatePaymentCharges(date);

            ////date = lst.Where(x => x.DailyProcessCode == "006").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            ////DailyInterestCalculation(date);
            //////////DailyInterestCalculationForFlexiLoan();

            ////date = lst.Where(x => x.DailyProcessCode == "011").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            ////InterestCalculation_WealthCreator(date);

            date = lst.Where(x => x.DailyProcessCode == "007").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            CreditInterestInCustomerAccount(date);

            date = lst.Where(x => x.DailyProcessCode == "008").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            MaturedAmountTransfer(date);
            date = lst.Where(x => x.DailyProcessCode == "010").Select(Y => Y.DailyProcessDate).FirstOrDefault();
            MaturedAmountTransfer_WealthCreator(date);


        }
        public object DailyInterestCalculation(DateTime Date)
        {
            int days1 = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            var db = new BSCCSLEntity();
            for (int k = 0; k < days1; k++)
            {
                try
                {
                    
                    Console.WriteLine(System.DateTime.Now.ToString());
                    Date = Date.AddDays(1);
                    //string SqlconString = @"Data Source=DESKTOP-NTOI5AC\MSSQLSERVER_DEV;Initial Catalog=BSCCSL_New;user id=sa;password=vishal@123;MultipleActiveResultSets=true";
                    string SqlconString = db.Database.Connection.ConnectionString;
                    string date = Date.ToString("yyyy-MM-dd");
                    List<inputval> lst = new List<inputval>();
                    using (SqlConnection con = new SqlConnection(SqlconString))
                    {
                        DataTable dt = new DataTable();
                        using (SqlCommand cmd = new SqlCommand("select c.CustomerProductId,c.Balance,c.InterestRate, c.IsFreeze, c.CustomerId, c.ProductType from CustomerProduct c, Customer m where c.CustomerId = m.CustomerId and c.IsDelete = 0 and m.IsDelete = 0 and c.OpeningDate <= '" + date + "' and c.ProductType in (1,3,4,7,8,9) and c.IsActive = 1 and(c.Status = 2 or c.Status is null) and c.CustomerProductId not in (select distinct CustomerProductId from DailyInterest where CONVERT(char(10), CreatedDate,126) = CONVERT(char(10),'" + date + "',126))", con))
                        {
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            //Thread.Sleep(4000);
                        }

                        for (int j = 0; j <= dt.Rows.Count - 1; j++)
                        {
                            inputval inp = new inputval();
                            inp.fromdate = dt.Rows[j][2].ToString();
                            inp.CustomerProductId = dt.Rows[j][0].ToString();
                            inp.ProductType = Convert.ToInt32(dt.Rows[j][1]);
                            lst.Add(inp);
                        }
                    }
                    Parallel.ForEach(lst, new ParallelOptions { MaxDegreeOfParallelism = 10 }, item =>
                    {
                        //string fromdate1 = item.fromdate;
                        string customerproductid = item.CustomerProductId;
                        //if (item.ProductType.ToString() == "1")
                        //    fromdate1 = "2022-07-01";
                        DateTime fromdate = Convert.ToDateTime(date);
                        // DateTime todate = System.DateTime.Today.Date;
                        //int days = Convert.ToInt32((todate - fromdate).TotalDays);
                        //for (int i = 0; i <= days; i++)
                        //{
                        //    if (fromdate <= todate)
                        //    {
                        SqlConnection sqlCon = null;
                        string SqlconString1 = db.Database.Connection.ConnectionString;
                        //string SqlconString1 = @"Data Source=DESKTOP-NTOI5AC\MSSQLSERVER_DEV;Initial Catalog=BSCCSL_New;user id=sa;password=vishal@123;MultipleActiveResultSets=true";
                        using (sqlCon = new SqlConnection(SqlconString1))
                        {
                            try
                            {
                                SqlCommand sql_cmnd = new SqlCommand("InterestCalculationByCPId", sqlCon);
                                sql_cmnd.CommandType = CommandType.StoredProcedure;
                                sql_cmnd.Parameters.AddWithValue("@CPId", SqlDbType.UniqueIdentifier).Value = customerproductid;
                                sql_cmnd.Parameters.AddWithValue("@Date", SqlDbType.DateTime).Value = fromdate.ToString("yyyy-MM-dd");
                                //sql_cmnd.Parameters.AddWithValue("@AGE", SqlDbType.Int).Value = age;
                                if (sqlCon.State == ConnectionState.Closed)
                                    sqlCon.Open();
                                sql_cmnd.ExecuteNonQuery();
                                sqlCon.Close();
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show("Error:" + ex.Message);
                                sqlCon.Close();
                            }
                        }
                        //    }
                        //    fromdate = fromdate.AddDays(1);
                        //}
                    });

                    using (SqlConnection con = new SqlConnection(SqlconString))
                    {
                        using (SqlCommand cmd = new SqlCommand("update DailyProcess_Console_Log set DailyProcessDate = '" + date + "',CreatedDateTime=getdate() where DailyProcessCode = '006'", con))
                        {
                            if (con.State == ConnectionState.Closed)
                                con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                Console.WriteLine(System.DateTime.Now.ToString());
            }
            //    select c.CustomerProductId,c.Balance,c.InterestRate, c.IsFreeze, c.CustomerId, c.ProductType from CustomerProduct c, Customer m where
            //c.CustomerId = m.CustomerId and c.IsDelete = 0 and m.IsDelete = 0 and c.OpeningDate <= @Date and c.ProductType in (1) and
            //c.IsActive = 1 and(c.Status = 2 or c.Status is null) and c.CustomerProductId not in (select distinct CustomerProductId from DailyInterest where CreatedDate = @Date)

            //int days = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            //for (int i = 0; i < days; i++)
            //{
            //    Date = Date.AddDays(1);
            //    try
            //    {
            //        //using (var db = new BSCCSLEntity())
            //        //{
            //        //    //var result = db.Database.SqlQuery<object>("InterestCalculation @Date",
            //        //    //new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

            //        //    string connectionstring = db.Database.Connection.ConnectionString;
            //        //    SqlConnection sql = new SqlConnection(connectionstring);
            //        //    SqlCommand cmd = new SqlCommand("InterestCalculation", sql);
            //        //    cmd.CommandType = CommandType.StoredProcedure;
            //        //    cmd.CommandTimeout = 3600;
            //        //    //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
            //        //    SqlParameter paramdate = cmd.Parameters.Add("@Date", SqlDbType.DateTime);
            //        //    paramdate.Value = (object)Date;

            //        //    //Execute the query
            //        //    sql.Open();
            //        //    //int result = cmdTimesheet.ExecuteNonQuery();
            //        //    var reader = cmd.ExecuteReader();

            //        //    // Read Blogs from the first result set
            //        //    var result = ((IObjectContextAdapter)db).ObjectContext.Translate<object>(reader).FirstOrDefault();
            //        //    sql.Close();
            //        //    db.Dispose();

            //        //    //return result;
            //        //}
            //        Task task1 = new Task(() => insertdata(Date, "InterestCalculation"));
            //        task1.Start();
            //        Task task2 = new Task(() => insertdata(Date, "InterestCalculation_Prod1"));
            //        task2.Start();
            //        Task task3 = new Task(() => insertdata(Date, "InterestCalculation_Prod3"));
            //        task3.Start();
            //        Task task4 = new Task(() => insertdata(Date, "InterestCalculation_Prod789"));
            //        task4.Start();
            //        //InterestCalculation_Prod1
            //        //InterestCalculation_Prod3
            //        //InterestCalculation_Prod789
            //        Task.WaitAll(new Task[] { task1, task2, task3, task4 });
            //        //Thread t = new Thread(() => insertdata(Date));          // Kick off a new thread
            //        //                                                       //new Thread(() => myMethod(myGrid));
            //        //t.Start();
            //        Console.WriteLine("Completed all tasks");
            //    }
            //    catch (Exception ex)
            //    {
            //        ErrorLogService.InsertLog(ex);
            //        return false;
            //    }
            //}
            return true;

        }
        public static async void insertdata(DateTime Date, string procedurename)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    //var result = db.Database.SqlQuery<object>("InterestCalculation @Date",
                    //new SqlParameter("Date", DateTime.Now)).FirstOrDefault();
                    //            SqlConnectionStringBuilder connectionBuilder
                    //= new SqlConnectionStringBuilder(db.Database.Connection.ConnectionString)
                    //{
                    //    ConnectTimeout = 4000,
                    //    AsynchronousProcessing = true
                    //};
                    int count = 0;
                    string connectionstring = db.Database.Connection.ConnectionString;
                    SqlConnection sql = new SqlConnection(connectionstring);
                    SqlCommand cmd = new SqlCommand(procedurename, sql);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 4000;
                    //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                    SqlParameter paramdate = cmd.Parameters.Add("@Date", SqlDbType.DateTime);
                    paramdate.Value = (object)Date;

                    //Execute the query
                    sql.Open();
                    //int result = cmdTimesheet.ExecuteNonQuery();
                    //cmd.BeginExecuteReader(new AsyncCallback(MyCallbackFunction), cmd);
                    IAsyncResult result = cmd.BeginExecuteNonQuery();
                    while (!result.IsCompleted)
                    {
                        //Console.WriteLine("Waiting ({0})", count++);
                        // Wait for 1/10 second, so the counter
                        // does not consume all available resources
                        // on the main thread.
                        //System.Threading.Thread.Sleep(100);
                    }
                    //await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine("Command complete. Affected {0} rows.",
                    cmd.EndExecuteNonQuery(result));
                    // Read Blogs from the first result set
                    // var result = ((IObjectContextAdapter)db).ObjectContext.Translate<object>(reader).FirstOrDefault();
                    sql.Close();
                    db.Dispose();

                    //return result;
                }
            }
            catch (Exception ex)
            {
                //ErrorLogService.InsertLog(ex);
                //return false;
                throw ex;
            }
        }
        private void MyCallbackFunction(IAsyncResult result)
        {
            try
            {
                //un-box the AsynState back to the SqlCommand
                SqlCommand cmd = (SqlCommand)result.AsyncState;
                SqlDataReader reader = cmd.EndExecuteReader(result);


                if (cmd.Connection.State.Equals(ConnectionState.Open))
                {
                    cmd.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                //ToDo : Swallow exception log
            }
        }
        public object InterestCalculation_WealthCreator(DateTime Date)
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
                        SqlCommand cmd = new SqlCommand("InterestCalculation_WealthCreator", sql);
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


        public object DeductRDFDAmountOpeningDate()
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    //var result = db.Database.SqlQuery<object>("DeductRDFDAmountOpeningDate @Date",
                    //  new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

                    string connectionstring = db.Database.Connection.ConnectionString;
                    SqlConnection sql = new SqlConnection(connectionstring);
                    SqlCommand cmd = new SqlCommand("DeductRDFDAmountOpeningDate", sql);
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

        public object DeductAmountofRDFromSaving(DateTime Date)
        {
            int days = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            for (int i = 0; i < days; i++)
            {
                Date = Date.AddDays(1);
                try
                {
                    using (var db = new BSCCSLEntity())
                    {

                        //var result = db.Database.SqlQuery<object>("DeductAmountofRDFromSaving @Date",
                        //  new SqlParameter("Date", DateTime.Now)).FirstOrDefault();
                        string connectionstring = db.Database.Connection.ConnectionString;
                        SqlConnection sql = new SqlConnection(connectionstring);
                        SqlCommand cmd = new SqlCommand("DeductAmountofRDFromSaving", sql);
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

        public object DeductAmountofTYPEFromSaving(DateTime Date)
        {
            int days = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            for (int i = 0; i < days; i++)
            {
                Date = Date.AddDays(1);
                try
                {
                    using (var db = new BSCCSLEntity())
                    {

                        //var result = db.Database.SqlQuery<object>("DeductAmountofRDFromSaving @Date",
                        //  new SqlParameter("Date", DateTime.Now)).FirstOrDefault();
                        string connectionstring = db.Database.Connection.ConnectionString;
                        SqlConnection sql = new SqlConnection(connectionstring);
                        SqlCommand cmd = new SqlCommand("DeductAmountofTYPEFromSaving", sql);
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
                        SqlCommand cmd = new SqlCommand("CalculateLatePaymentCharges", sql);
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

        public object CreditInterestInCustomerAccount(DateTime Date)
        {
            int days = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            for (int i = 0; i < days; i++)
            {
                Date = Date.AddDays(1);
                try
                {
                    using (var db = new BSCCSLEntity())
                    {
                        //var result = db.Database.SqlQuery<object>("CreditInterestInCustomerAccount @Date",
                        //  new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

                        string connectionstring = db.Database.Connection.ConnectionString;
                        SqlConnection sql = new SqlConnection(connectionstring);
                        SqlCommand cmd = new SqlCommand("CreditInterestInCustomerAccount", sql);
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

        public object MaturedAmountTransfer(DateTime Date)
        {
            int days = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            for (int i = 0; i < days; i++)
            {
                Date = Date.AddDays(1);
                try
                {
                    using (var db = new BSCCSLEntity())
                    {
                        //var result = db.Database.SqlQuery<object>("MaturedAmountTransfer @Date",
                        //  new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

                        string connectionstring = db.Database.Connection.ConnectionString;
                        SqlConnection sql = new SqlConnection(connectionstring);
                        SqlCommand cmd = new SqlCommand("MaturedAmountTransfer", sql);
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

        public object MaturedAmountTransfer_WealthCreator(DateTime Date)
        {
            int days = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            for (int i = 0; i < days; i++)
            {
                Date = Date.AddDays(1);
                try
                {
                    using (var db = new BSCCSLEntity())
                    {
                        //var result = db.Database.SqlQuery<object>("MaturedAmountTransfer @Date",
                        //  new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

                        string connectionstring = db.Database.Connection.ConnectionString;
                        SqlConnection sql = new SqlConnection(connectionstring);
                        SqlCommand cmd = new SqlCommand("MaturedAmountTransfer_WealthCreator", sql);
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

        public object DeductAmountofRIPFromSaving(DateTime Date)
        {
            int days = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            for (int i = 0; i < days; i++)
            {
                Date = Date.AddDays(1);
                try
                {
                    using (var db = new BSCCSLEntity())
                    {
                        //var result = db.Database.SqlQuery<object>("DeductAmountofRIPFromSaving @Date",
                        //  new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

                        string connectionstring = db.Database.Connection.ConnectionString;
                        SqlConnection sql = new SqlConnection(connectionstring);
                        SqlCommand cmd = new SqlCommand("DeductAmountofRIPFromSaving", sql);
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

        public object AddAmountofMISInSaving(DateTime Date)
        {
            int days = Convert.ToInt32((System.DateTime.Today.Date - Date).TotalDays);
            for (int i = 0; i < days; i++)
            {
                Date = Date.AddDays(1);
                try
                {
                    using (var db = new BSCCSLEntity())
                    {
                        //var result = db.Database.SqlQuery<object>("AddAmountofMISInSaving @Date",
                        //  new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

                        string connectionstring = db.Database.Connection.ConnectionString;
                        SqlConnection sql = new SqlConnection(connectionstring);
                        SqlCommand cmd = new SqlCommand("AddAmountofMISInSaving", sql);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 3600;
                        //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                        SqlParameter paramdate = cmd.Parameters.Add("@Date", SqlDbType.DateTime);
                        //SqlParameter paramdate = cmd.Parameters.Add("@Date", DateTime);
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

        //SMS Methods

        public void RunSMSSchedular()
        {
            Get7DaysPaymentReminder();
            Get1DaysPaymentReminder();
        }

        public void Get7DaysPaymentReminder()
        {
            try
            {

                using (var db = new BSCCSLEntity())
                {
                    //var today = DateTime.Now.Date;

                    DateTime today = DateTime.Now.Date;

                    var getcustomerList = (from d in db.CustomerProduct.Where(b => b.IsActive == true && b.IsDelete == false && b.IsFreeze == null && b.PaymentType != Frequency.Daily &&
                                   (b.ProductType == ProductType.Recurring_Deposit || b.ProductType == ProductType.Loan || b.ProductType == ProductType.Regular_Income_Planner) &&
                                   DbFunctions.AddDays((b.NextInstallmentDate), -7) == DbFunctions.TruncateTime(today))
                                           join c in db.Customer on d.CustomerId equals c.CustomerId
                                           //join p in db.CustomerPersonalDetail.AsEnumerable() on c.CustomerId equals p.CustomerId
                                           join m in db.CustomerAddress on c.CustomerId equals m.CustomerId
                                           select new
                                           {
                                               CustomerId = c.CustomerId,
                                               Amount = d.Amount,
                                               InstallmentDate = d.NextInstallmentDate,
                                               //FirstName = p.FirstName,
                                               AccountType = d.ProductType,
                                               MobileNo = m.MobileNo,
                                               AccountNumber = d.AccountNumber,
                                           }).ToList();

                    foreach (var customer in getcustomerList)
                    {

                        if (customer.Amount > 0)
                        {

                            string SMS = "";

                            //DateTime dt = DateTime.ParseExact(customer.InstallmentDate.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

                            string installmentdate = customer.InstallmentDate.Value.ToString("dd-MM-yyyy");

                            var msg = db.Messages.Where(a => a.Type == Type.SMS && a.Name == MessageType.INSTALLMENT_REMINDER).Select(a => a.Message).FirstOrDefault();
                            SMS = msg;
                            SMS = SMS.Replace("{amount}", customer.Amount.ToString());
                            SMS = SMS.Replace("{AccountNumber}", customer.AccountNumber.ToString());
                            SMS = SMS.Replace("{date}", installmentdate);
                            SMSService.SendSMS(SMS, customer.MobileNo);
                            SMSService.InserSMSLog(customer.MobileNo, SMS, MessageType.INSTALLMENT_REMINDER.ToString().Replace("_", " "));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
            }
        }

        //public void Get5DaysPaymentReminder()
        //{
        //    try
        //    {

        //        using (var db = new BSCCSLEntity())
        //        {
        //            var today = DateTime.Now.Date;

        //            var getcustomerList = (from d in db.CustomerProduct.Where(b => b.IsActive == true && b.IsDelete == false && b.IsFreeze == null && b.PaymentType != Frequency.Daily &&
        //                           (b.ProductType == ProductType.Recurring_Deposit || b.ProductType == ProductType.Loan || b.ProductType == ProductType.Regular_Income_Planner) &&
        //                           DbFunctions.AddDays((b.NextInstallmentDate), -5) == DbFunctions.TruncateTime(today))
        //                                   join c in db.Customer.AsEnumerable() on d.CustomerId equals c.CustomerId
        //                                   //join p in db.CustomerPersonalDetail.AsEnumerable() on c.CustomerId equals p.CustomerId
        //                                   join m in db.CustomerAddress.AsEnumerable() on c.CustomerId equals m.CustomerId
        //                                   select new
        //                                   {
        //                                       CustomerId = c.CustomerId,
        //                                       Amount = d.Amount,
        //                                       InstallmentDate = d.NextInstallmentDate,
        //                                       //FirstName = p.FirstName,
        //                                       AccountType = d.ProductType,
        //                                       MobileNo = m.MobileNo,
        //                                       AccountNumber = d.AccountNumber,
        //                                   }).ToList();

        //            foreach (var customer in getcustomerList)
        //            {

        //                string SMS = "";

        //                DateTime dt = DateTime.ParseExact(customer.InstallmentDate.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

        //                string installmentdate = dt.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

        //                var msg = db.Messages.Where(a => a.Type == Type.SMS && a.Name == MessageType.INSTALLMENT_REMINDER).Select(a => a.Message).FirstOrDefault();
        //                SMS = msg;
        //                SMS = SMS.Replace("{amount}", customer.Amount.ToString());
        //                SMS = SMS.Replace("{AccountNumber}", customer.AccountNumber.ToString());
        //                SMS = SMS.Replace("{date}", installmentdate);
        //                smsService.SendSMS(SMS, customer.MobileNo);
        //                smsService.InserSMSLog(customer.MobileNo, SMS, MessageType.INSTALLMENT_REMINDER.ToString().Replace("_", " "));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogService.InsertLog(ex);
        //    }
        //}

        //public void Get3DaysPaymentReminder()
        //{
        //    try
        //    {

        //        using (var db = new BSCCSLEntity())
        //        {
        //            var today = DateTime.Now.Date;

        //            var getcustomerList = (from d in db.CustomerProduct.Where(b => b.IsActive == true && b.IsDelete == false && b.IsFreeze == null && b.PaymentType != Frequency.Daily &&
        //                           (b.ProductType == ProductType.Recurring_Deposit || b.ProductType == ProductType.Loan || b.ProductType == ProductType.Regular_Income_Planner) &&
        //                           DbFunctions.AddDays((b.NextInstallmentDate), -3) == DbFunctions.TruncateTime(today))
        //                                   join c in db.Customer.AsEnumerable() on d.CustomerId equals c.CustomerId
        //                                   //join p in db.CustomerPersonalDetail.AsEnumerable() on c.CustomerId equals p.CustomerId
        //                                   join m in db.CustomerAddress.AsEnumerable() on c.CustomerId equals m.CustomerId
        //                                   select new
        //                                   {
        //                                       CustomerId = c.CustomerId,
        //                                       Amount = d.Amount,
        //                                       InstallmentDate = d.NextInstallmentDate,
        //                                       //FirstName = p.FirstName,
        //                                       AccountType = d.ProductType,
        //                                       MobileNo = m.MobileNo,
        //                                       AccountNumber = d.AccountNumber,
        //                                   }).ToList();

        //            foreach (var customer in getcustomerList)
        //            {

        //                string SMS = "";

        //                DateTime dt = DateTime.ParseExact(customer.InstallmentDate.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

        //                string installmentdate = dt.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

        //                var msg = db.Messages.Where(a => a.Type == Type.SMS && a.Name == MessageType.INSTALLMENT_REMINDER).Select(a => a.Message).FirstOrDefault();
        //                SMS = msg;
        //                SMS = SMS.Replace("{amount}", customer.Amount.ToString());
        //                SMS = SMS.Replace("{AccountNumber}", customer.AccountNumber.ToString());
        //                SMS = SMS.Replace("{date}", installmentdate);
        //                smsService.SendSMS(SMS, customer.MobileNo);
        //                smsService.InserSMSLog(customer.MobileNo, SMS, MessageType.INSTALLMENT_REMINDER.ToString().Replace("_", " "));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogService.InsertLog(ex);
        //    }
        //}

        public void Get1DaysPaymentReminder()
        {
            try
            {

                using (var db = new BSCCSLEntity())
                {
                    DateTime today = DateTime.Now.Date;

                    var getcustomerList = (from d in db.CustomerProduct.Where(b => b.IsActive == true && b.IsDelete == false && b.IsFreeze == null && b.Status == CustomerProductStatus.Approved && b.PaymentType != Frequency.Daily &&
                                   (b.ProductType == ProductType.Recurring_Deposit || b.ProductType == ProductType.Loan || b.ProductType == ProductType.Regular_Income_Planner) &&
                                   DbFunctions.AddDays((b.NextInstallmentDate), -1) == DbFunctions.TruncateTime(today))
                                           join c in db.Customer on d.CustomerId equals c.CustomerId
                                           //join p in db.CustomerPersonalDetail.AsEnumerable() on c.CustomerId equals p.CustomerId
                                           join m in db.CustomerAddress on c.CustomerId equals m.CustomerId
                                           select new
                                           {
                                               CustomerId = c.CustomerId,
                                               Amount = d.Amount,
                                               InstallmentDate = d.NextInstallmentDate,
                                               //FirstName = p.FirstName,
                                               AccountType = d.ProductType,
                                               MobileNo = m.MobileNo,
                                               AccountNumber = d.AccountNumber,
                                           }).ToList();

                    foreach (var customer in getcustomerList)
                    {
                        if (customer.Amount > 0)
                        {

                            string SMS = "";

                            string installmentdate = customer.InstallmentDate.Value.ToString("dd-MM-yyyy");

                            var msg = db.Messages.Where(a => a.Type == Type.SMS && a.Name == MessageType.INSTALLMENT_REMINDER).Select(a => a.Message).FirstOrDefault();
                            SMS = msg;
                            SMS = SMS.Replace("{amount}", customer.Amount.ToString());
                            SMS = SMS.Replace("{AccountNumber}", customer.AccountNumber.ToString());
                            SMS = SMS.Replace("{date}", installmentdate);
                            SMSService.SendSMS(SMS, customer.MobileNo);
                            SMSService.InserSMSLog(customer.MobileNo, SMS, MessageType.INSTALLMENT_REMINDER.ToString().Replace("_", " "));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
            }
        }
    }
    public class inputval
    {
        public string CustomerProductId { get; set; }
        public string fromdate { get; set; }
        public DateTime todate { get; set; }
        public int ProductType { get; set; }
        public int days { get; set; }
    }
}
