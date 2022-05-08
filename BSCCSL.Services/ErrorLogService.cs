using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class ErrorLogService
    {
        public static void InsertLog(Exception ex)
        {
            try
            {
                using (BSCCSLEntity db = new BSCCSLEntity())
                {
                    ErrorLog elog = new ErrorLog();
                    elog.Message = ex.Message;
                    elog.Description = Convert.ToString(ex.InnerException + " \n" + ex.StackTrace);
                    db.ErrorLog.Add(elog);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void LogError(Exception ex, string UserId, string Desc)
        {
            try
            {
                using (BSCCSLEntity db = new BSCCSLEntity())
                {
                    ErrorLog elog = new ErrorLog();
                    elog.Message = ex.Message;
                    elog.Description = Convert.ToString(ex.InnerException + " \n" + ex.StackTrace);
                    elog.Message = Desc.ToString();
                    db.ErrorLog.Add(elog);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void LogMessage(string Message, string UserId = "", string Desc = "", LogLevel logLevel = LogLevel.Low)
        {
            try
            {
                using (BSCCSLEntity db = new BSCCSLEntity())
                {
                    ErrorLog elog = new ErrorLog();
                    elog.Message = Message;
                    elog.Description = Desc;
                    elog.UserId = UserId;
                    elog.LogLevel = logLevel;
                    db.ErrorLog.Add(elog);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }



        public static void WriteLog(string Message)
        {
            try
            {
                using (BSCCSLEntity db = new BSCCSLEntity())
                {
                    ErrorLog elog = new ErrorLog();
                    elog.Message = Message;
                    //elog.Description = Convert.ToString(ex.InnerException + " \n" + ex.StackTrace);
                    db.ErrorLog.Add(elog);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                InsertLog(e);
            }
        }
    }
}
