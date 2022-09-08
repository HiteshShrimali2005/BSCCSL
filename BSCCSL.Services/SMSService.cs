using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSCCSL.Models;
using System.Data.Entity;
using System.IO;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Net;
using System.Web;
using System.ComponentModel;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace BSCCSL.Services
{
    public class SMSService
    {
        public static void SendNewCustomerSMS(CustomerDetail customerdetail)
        {
            using (var db = new BSCCSLEntity())
            {
                var NewCustomerSMS = db.Messages.Where(x => x.Name == MessageType.CUSTOMER_REGISTRATION).FirstOrDefault();
                string sms = "", smsname = "";

                smsname = MessageType.CUSTOMER_REGISTRATION.ToString().Replace("_", " ");
                foreach (var cpd in customerdetail.Personal)
                {
                    sms = NewCustomerSMS.Message;
                    sms = sms.Replace("{FirstName}", cpd.Personal.FirstName);
                    sms = sms.Replace("{LastName}", cpd.Personal.LastName);
                    sms = sms.Replace("{CustomerID}", customerdetail.Customer.ClienId);
                    bool flag = SendSMS(sms, cpd.Address.MobileNo);
                    if (flag)
                    {
                        InserSMSLog(cpd.Address.MobileNo, sms, smsname);
                    }
                    sms = "";
                }
            }

        }

        public static void SendNewAccountOpenSMS(Guid CustomerProductId)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var CustomerProductDetail = (from c in db.CustomerProduct.Where(a => a.CustomerProductId == CustomerProductId)
                                                 join p in db.CustomerPersonalDetail.AsEnumerable() on c.CustomerId equals p.CustomerId
                                                 join m in db.CustomerAddress.AsEnumerable() on p.PersonalDetailId equals m.PersonalDetailId
                                                 select new
                                                 {
                                                     CustomerId = c.CustomerId,
                                                     FirstName = p.FirstName,
                                                     AccountType = c.ProductType,
                                                     MobileNo = m.MobileNo,
                                                     AccountNumber = c.AccountNumber,
                                                 }).ToList();

                    foreach (var det in CustomerProductDetail)
                    {
                        var NewCustomerSMS = db.Messages.Where(x => x.Name == MessageType.NEW_ACCOUNT).FirstOrDefault();
                        string sms = "", smsname = "";

                        smsname = MessageType.NEW_ACCOUNT.ToString().Replace("_", " ");
                        sms = NewCustomerSMS.Message;
                        sms = sms.Replace("{FirstName}", det.FirstName);
                        sms = sms.Replace("{ACCOUNTType}", det.AccountType.ToString().Replace("_", " "));
                        sms = sms.Replace("{AccountNumber}", det.AccountNumber);
                        bool flag = SendSMS(sms, det.MobileNo);
                        if (flag)
                        {
                            InserSMSLog(det.MobileNo, sms, smsname);
                        }

                        sms = "";
                    }
                }
            }
            catch (Exception ex)
            { 
            }

        }

        public static void SendTransactionSMS(Guid TranId)
        {
            using (var db = new BSCCSLEntity())
            {
                //var TranCustDetails = (from t in db.Transaction.AsEnumerable().Where(x => x.TransactionId == TranId)
                //                       join c in db.CustomerProduct.AsEnumerable() on t.CustomerProductId equals c.CustomerProductId
                //                       join m in db.CustomerAddress.AsEnumerable() on t.CustomerId equals m.CustomerId
                //                       //join r in db.CustomerProduct on t.RefCustomerProductId equals r.CustomerProductId into refcust
                //                       //from r in refcust.DefaultIfEmpty()
                //                       group new { t, c.AccountNumber, m.MobileNo } by new { c.CustomerId, c.Balance, c.ProductType } into custdetail
                //                       select new TransactionCustomer
                //                       {
                //                           RefCustomerProductId = custdetail.Select(a => a.t.RefCustomerProductId).FirstOrDefault(),
                //                           Amount = custdetail.Select(a => a.t.Amount).FirstOrDefault(),
                //                           Date = custdetail.Select(a => a.t.TransactionTime).FirstOrDefault(),
                //                           AccountNo = custdetail.Select(a => a.AccountNumber).FirstOrDefault(),
                //                           Type = custdetail.Select(a => a.t.Type).FirstOrDefault(),
                //                           MobNo = string.Join(", ", custdetail.Select(a => a.MobileNo)),
                //                           TransactionType = custdetail.Select(a => a.t.TransactionType).FirstOrDefault(),
                //                           DescIndentify = custdetail.Select(a => a.t.DescIndentify).FirstOrDefault(),
                //                           Balance = custdetail.Key.Balance.Value,
                //                           Status = custdetail.Select(a => a.t.Status).FirstOrDefault(),
                //                           ProductType = custdetail.Key.ProductType
                //                           //RefProductType = custdetail.Select(a=>a.r != null ? a.r.ProductType : null)

                //                       }).FirstOrDefault();


                var TranCustDetails = GetDetailsForSMS(TranId);


                if (TranCustDetails != null)
                {
                    var type = typeof(ProductType);
                    var memInfo = type.GetMember(TranCustDetails.ProductType.ToString());
                    var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                    var description = ((DescriptionAttribute)attributes[0]).Description;
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("AccountNumber", TranCustDetails.AccountNo.Substring((TranCustDetails.AccountNo.Length - 4), 4));
                    dic.Add("Amount", TranCustDetails.Amount.ToString());
                    dic.Add("Date", TranCustDetails.Date.ToString());
                    dic.Add("TransactionType", TranCustDetails.TransactionType.ToString());
                    dic.Add("Type", TranCustDetails.Type.ToString());
                    dic.Add("AC", description);
                    dic.Add("Status", TranCustDetails.Status.ToString());

                    if (TranCustDetails.RefCustomerProductId != null)
                    {
                        Guid RefCustomerProductId =(Guid) TranCustDetails.RefCustomerProductId;
                        var RefTransaction = db.CustomerProduct.Where(x => x.CustomerProductId == RefCustomerProductId).FirstOrDefault();
                        dic.Add("RefProductType", ((ProductType)RefTransaction.ProductType).ToString());
                        dic.Add("RefAccountNumber", RefTransaction.AccountNumber);
                    }

                    if (TranCustDetails.Type == TypeCRDR.CR)
                    {
                        //decimal bal = TranCustDetails.Balance + TranCustDetails.Amount;
                        dic.Add("Balance", TranCustDetails.Balance.ToString());
                        string sms = UpdateToken(Type.SMS, MessageType.DEPOSIT, dic);
                        bool flag = SendSMS(sms, TranCustDetails.MobNo);
                        if (flag)
                        {
                            InserSMSLog(TranCustDetails.MobNo, sms, MessageType.DEPOSIT.ToString().Replace("_", " "));
                        }
                    }
                    else
                    {
                       // decimal bal = TranCustDetails.Balance - TranCustDetails.Amount;
                        dic.Add("Balance", TranCustDetails.Balance.ToString());
                        string sms = UpdateToken(Type.SMS, MessageType.WITHDRAWAL, dic);
                        bool flag = SendSMS(sms, TranCustDetails.MobNo);
                        if (flag)
                        {
                            InserSMSLog(TranCustDetails.MobNo, sms, MessageType.WITHDRAWAL.ToString().Replace("_", " "));
                        }
                    }
                }
            }
        }

        public void SendRegistrationOTP(CustomerRegisterOPT customerRegisterOPT)
        {
            using (var db = new BSCCSLEntity())
            {
                var message = db.Messages.Where(x => x.Name == MessageType.CUSTOMER_REGISTRATION_OTP).FirstOrDefault();

                string sms = "", smsname = "";

                smsname = MessageType.CUSTOMER_REGISTRATION_OTP.ToString().Replace("_", " ");
                sms = message.Message;
                sms = sms.Replace("{OTP}", customerRegisterOPT.OTP);
                bool flag = SendSMS(sms, customerRegisterOPT.MobileNumber);
                if (flag)
                {
                    InserSMSLog(customerRegisterOPT.MobileNumber, sms, smsname);
                }
            }
        }

        public static void SendForgotPasswordOTP(CustomerForgotPasswordOTP customerRegisterOPT)
        {
            using (var db = new BSCCSLEntity())
            {
                var message = db.Messages.Where(x => x.Name == MessageType.CUSTOMER_FORGOT_PASSWORD_OTP).FirstOrDefault();

                string sms = "", smsname = "";

                smsname = MessageType.CUSTOMER_FORGOT_PASSWORD_OTP.ToString().Replace("_", " ");
                sms = message.Message;
                sms = sms.Replace("{OTP}", customerRegisterOPT.OTP);
                bool flag = SendSMS(sms, customerRegisterOPT.MobileNumber);
                if (flag)
                {
                    InserSMSLog(customerRegisterOPT.MobileNumber, sms, smsname);
                }
            }
        }

        public static void SendLoanStatusSMS(Guid loanId)
        {
            using (var db = new BSCCSLEntity())
            {

                var CustomerProductDetail = (from l in db.Loan.Where(a => a.LoanId == loanId)
                                             join c in db.CustomerProduct on l.CustomerProductId equals c.CustomerProductId
                                             join p in db.CustomerPersonalDetail.AsEnumerable() on c.CustomerId equals p.CustomerId
                                             join m in db.CustomerAddress.AsEnumerable() on p.PersonalDetailId equals m.PersonalDetailId
                                             select new
                                             {
                                                 CustomerId = c.CustomerId,
                                                 FirstName = p.FirstName,
                                                 DisburseThrough = ((DisburseThrough)l.DisburseThrough).ToString().Replace("_", " "),
                                                 DisbursementAmount = l.DisbursementAmount,
                                                 MobileNo = m.MobileNo,
                                                 AccountNumber = c.AccountNumber,
                                                 LoanStatus = ((LoanStatus)l.LoanStatus).ToString()
                                             }).FirstOrDefault();


                var message = db.Messages.Where(x => x.Name == MessageType.LOAN_STATUS).FirstOrDefault();
                string sms = "", smsname = "";
                sms = message.Message;
                smsname = MessageType.LOAN_STATUS.ToString().Replace("_", " ");
                sms = sms.Replace("{FirstName}", CustomerProductDetail.FirstName);
                sms = sms.Replace("{Status}", CustomerProductDetail.LoanStatus);
                sms = sms.Replace("{AccountNumber}", CustomerProductDetail.AccountNumber.Substring(CustomerProductDetail.AccountNumber.Length - 4));

                if (CustomerProductDetail.LoanStatus == "Disbursed")
                {
                    sms += " through " + CustomerProductDetail.DisburseThrough;
                }
                bool flag = SendSMS(sms, CustomerProductDetail.MobileNo);
                if (flag)
                {
                    InserSMSLog(CustomerProductDetail.MobileNo, sms, smsname);
                }
            }
        }

        public static void SendBounceSMS(Guid TranId)
        {
            using (var db = new BSCCSLEntity())
            {

                //var TranCustDetails = (from t in db.Transaction.AsEnumerable().Where(x => x.TransactionId == TranId)
                //                       join c in db.CustomerProduct.AsEnumerable() on t.CustomerProductId equals c.CustomerProductId
                //                       join m in db.CustomerAddress.AsEnumerable() on t.CustomerId equals m.CustomerId
                //                       //join r in db.CustomerProduct on t.RefCustomerProductId equals r.CustomerProductId into refcust
                //                       //from r in refcust.DefaultIfEmpty()
                //                       group new { t, c.AccountNumber, m.MobileNo } by new { c.CustomerId, c.Balance } into custdetail
                //                       select new
                //                       {
                //                           Amount = custdetail.Select(a => a.t.Amount).FirstOrDefault(),
                //                           Date = custdetail.Select(a => a.t.TransactionTime).FirstOrDefault(),
                //                           AccountNo = custdetail.Select(a => a.AccountNumber).FirstOrDefault(),
                //                           Type = custdetail.Select(a => a.t.Type).FirstOrDefault(),
                //                           MobNo = string.Join(", ", custdetail.Select(a => a.MobileNo)),
                //                           TransactionType = custdetail.Select(a => a.t.TransactionType).FirstOrDefault(),
                //                           DescIndentify = custdetail.Select(a => a.t.DescIndentify).FirstOrDefault(),
                //                           Balance = custdetail.Key.Balance.Value,
                //                           ChequeNo = custdetail.Select(a => a.t.CheckNumber).FirstOrDefault(),

                //                           //RefProductType = custdetail.Select(a=>a.r != null ? a.r.ProductType : null)

                //                       }).FirstOrDefault();

                var TranCustDetails = GetDetailsForSMS(TranId);
                var message = db.Messages.Where(x => x.Name == MessageType.CHEQUE_BOUNCE_SMS).FirstOrDefault();
                string sms = "", smsname = "";
                sms = message.Message;
                smsname = MessageType.CHEQUE_BOUNCE_SMS.ToString().Replace("_", " ");
                sms = sms.Replace("{chqNumber}", TranCustDetails.CheckNumber.ToString());

                bool flag = SendSMS(sms, TranCustDetails.MobNo);
                if (flag)
                {
                    InserSMSLog(TranCustDetails.MobNo, sms, smsname);
                }
            }
        }

        public static void SendPaneltySMS(Guid TranId)
        {
            using (var db = new BSCCSLEntity())
            {

                //var TranCustDetails = (from t in db.Transaction.AsEnumerable().Where(x => x.TransactionId == TranId)
                //                       join c in db.CustomerProduct.AsEnumerable() on t.CustomerProductId equals c.CustomerProductId
                //                       join m in db.CustomerAddress.AsEnumerable() on t.CustomerId equals m.CustomerId
                //                       //join r in db.CustomerProduct on t.RefCustomerProductId equals r.CustomerProductId into refcust
                //                       //from r in refcust.DefaultIfEmpty()
                //                       group new { t, c.AccountNumber, m.MobileNo } by new { c.CustomerId, c.Balance } into custdetail
                //                       select new
                //                       {
                //                           Amount = custdetail.Select(a => a.t.Amount).FirstOrDefault(),
                //                           Date = custdetail.Select(a => a.t.TransactionTime).FirstOrDefault(),
                //                           AccountNo = custdetail.Select(a => a.AccountNumber).FirstOrDefault(),
                //                           Type = custdetail.Select(a => a.t.Type).FirstOrDefault(),
                //                           MobNo = string.Join(", ", custdetail.Select(a => a.MobileNo)),
                //                           Balance = custdetail.Key.Balance.Value,
                //                           ChequeNo = custdetail.Select(a => a.t.CheckNumber).FirstOrDefault(),
                //                       }).FirstOrDefault();
                var TranCustDetails = GetDetailsForSMS(TranId);


                var message = db.Messages.Where(x => x.Name == MessageType.CHEQUE_BOUNCE_PANELTY).FirstOrDefault();
                string sms = "", smsname = "";
                sms = message.Message;
                smsname = MessageType.CHEQUE_BOUNCE_PANELTY.ToString().Replace("_", " ");
                sms = sms.Replace("{chqNumber}", TranCustDetails.CheckNumber.ToString());
                sms = sms.Replace("{amount}", TranCustDetails.Amount.ToString());
                sms = sms.Replace("{date}", TranCustDetails.Date.ToString());
                sms = sms.Replace("{AccountNumber}", TranCustDetails.AccountNo.ToString());

                bool flag = SendSMS(sms, TranCustDetails.MobNo);
                if (flag)
                {
                    InserSMSLog(TranCustDetails.MobNo, sms, smsname);
                }
            }
        }

        public static string UpdateToken(Type type, MessageType name, Dictionary<string, string> dic)
        {
            string SMS = "";
            try
            {
                
                using (var db = new BSCCSLEntity())
                {
                    var msg = db.Messages.Where(a => a.Type == type && a.Name == name).Select(a => a.Message).FirstOrDefault();
                    SMS = msg;
                    SMS = SMS.Replace("{amount}", dic["Amount"]);
                    SMS = SMS.Replace("{AccountNumber}", dic["AccountNumber"]);
                    SMS = SMS.Replace("{TYPE}", dic["TransactionType"]);
                    SMS = SMS.Replace("{date}", dic["Date"]);
                    SMS = SMS.Replace("{AC}", dic["AC"]);
                    if (dic["TransactionType"] == TransactionType.Cheque.ToString() && dic["Type"] == TypeCRDR.CR.ToString() && dic["Status"] == Status.Unclear.ToString())
                    {
                        SMS = SMS.Replace("Avbl Bal Rs. {Balance}", string.Empty);
                        SMS += "-Subject to realization";
                    }
                    else
                    {
                        SMS = SMS.Replace("{Balance}", dic["Balance"]);
                    }

                    if (dic.ContainsKey("RefProductType"))
                    {
                        SMS += " towards AccountNumber " + dic["RefProductType"].Replace("_", "-") + " " + dic["RefAccountNumber"];
                    }
                }
            }
            catch (Exception ex)
            { 

            }
            return SMS;
        }

        public static void InserSMSLog(string Mobno, string sms, string smsname)
        {
            try
            {
                using (BSCCSLEntity db = new BSCCSLEntity())
                {
                    SMSLog smslog = new SMSLog();
                    smslog.MobileNo = Mobno;
                    smslog.Message = sms;
                    smslog.SMSname = smsname;
                    smslog.Send = true;
                    smslog.CreatedDate = DateTime.Now;
                    db.SMSLog.Add(smslog);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
            }
        }

        //public static bool SendSMS(string msg, string mobileNo)
        //{
        //    string SMS = System.Configuration.ConfigurationManager.AppSettings["SMS"].ToString();

        //    if (SMS == "1")
        //    {
        //        string authKey = "148479AywGa6TFD58ec6fb7";
        //        //Multiple mobiles numbers separated by comma
        //        string mobileNumber = "9277901804";
        //        //string mobileNumber = mobileNo;
        //        //Sender ID,While using route4 sender id should be 6 characters long.
        //        string senderId = "BPSTRM";

        //        string message = HttpUtility.UrlEncode(msg);

        //        //Prepare you post parameters
        //        StringBuilder sbPostData = new StringBuilder();
        //        sbPostData.AppendFormat("authkey={0}", authKey);
        //        sbPostData.AppendFormat("&mobiles={0}", mobileNumber);
        //        sbPostData.AppendFormat("&message={0}", message);
        //        sbPostData.AppendFormat("&sender={0}", senderId);
        //        sbPostData.AppendFormat("&route={0}", "4");
        //        sbPostData.AppendFormat("&country={0}", "91");

        //        try
        //        {
        //            //Call Send SMS API
        //            string sendSMSUri = "https://control.msg91.com/api/sendhttp.php";
        //            //Create HTTPWebrequest
        //            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
        //            //Prepare and Add URL Encoded data
        //            UTF8Encoding encoding = new UTF8Encoding();
        //            byte[] data = encoding.GetBytes(sbPostData.ToString());
        //            //Specify post method
        //            httpWReq.Method = "POST";
        //            httpWReq.ContentType = "application/x-www-form-urlencoded";
        //            httpWReq.ContentLength = data.Length;
        //            using (Stream stream = httpWReq.GetRequestStream())
        //            {
        //                stream.Write(data, 0, data.Length);
        //            }
        //            //Get the response
        //            HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
        //            StreamReader reader = new StreamReader(response.GetResponseStream());
        //            string responseString = reader.ReadToEnd();

        //            //Close the response
        //            reader.Close();
        //            response.Close();

        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorLogService.InsertLog(ex);
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}


        public static bool SendSMS(string msg, string mobileNo)
        {
            string SMS = System.Configuration.ConfigurationManager.AppSettings["SMS"].ToString();

            if (SMS == "1")
            {
                try
                {
                    //Call Send SMS API
                    //string sendSMSUri = "http://148.251.80.111:5665/api/SendSMS?api_id=API105185934809&api_password=123456789&sms_type=T&encoding=T&sender_id=BSCOSL&phonenumber={mobileno}&textmessage={message}";
                    string sendSMSUri = "http://api.msg91.com/api/sendhttp.php?sender=BPSTRM&route=4&mobiles={mobileno}&authkey=148479AywGa6TFD58ec6fb7&country=91&message={message}";
                    sendSMSUri = sendSMSUri.Replace("{mobileno}", mobileNo);
                    sendSMSUri = sendSMSUri.Replace("{message}", msg);

                    using (var client = new WebClient())
                    {
                        //string APIURL = ConfigurationManager.AppSettings.Get("APIURL");
                        var response1 = client.DownloadString(sendSMSUri);
                    }


                    ////string sendSMSUri = "http://148.251.80.111:5665/api/SendSMS?";
                    ////Create HTTPWebrequest
                    //HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                    ////Prepare and Add URL Encoded data
                    //UTF8Encoding encoding = new UTF8Encoding();
                    //byte[] data = encoding.GetBytes(sbPostData.ToString());
                    ////Specify post method
                    //httpWReq.Method = "GET";
                    //httpWReq.ContentType = "application/xml";
                    //httpWReq.ContentLength = data.Length;
                    //using (Stream stream = httpWReq.GetRequestStream())
                    //{
                    //    stream.Write(data, 0, data.Length);
                    //}
                    ////Get the response
                    //HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                    //StreamReader reader = new StreamReader(response.GetResponseStream());
                    //string responseString = reader.ReadToEnd();

                    ////Close the response
                    //reader.Close();
                    //response.Close();
                    //var result = "";
                    //UTF8Encoding encoding = new UTF8Encoding();
                    //byte[] data = encoding.GetBytes(sbPostData.ToString());

                    //if (sendSMSUri != "")
                    //{
                    //    var httpWebRequest = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                    //    httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                    //    httpWebRequest.Method = "POST";
                    //    httpWReq.ContentLength = data.Length;
                    //    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    //    {
                    //        streamWriter.Flush();
                    //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    //        {
                    //            result = streamReader.ReadToEnd();
                    //        }
                    //    }
                    //}
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorLogService.InsertLog(ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public static TransactionCustomer GetDetailsForSMS(Guid TranId)
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetCustomerDetailsforTransactionSMS", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                SqlParameter paramuser = cmdTimesheet.Parameters.Add("@TransactionId", SqlDbType.UniqueIdentifier);
                paramuser.Value = TranId;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<TransactionCustomer> TranCustDetails = ((IObjectContextAdapter)db).ObjectContext.Translate<TransactionCustomer>(reader).ToList();
                reader.NextResult();
                sql.Close();
                db.Dispose();

                return TranCustDetails.FirstOrDefault();
            }
        }


    }
}
