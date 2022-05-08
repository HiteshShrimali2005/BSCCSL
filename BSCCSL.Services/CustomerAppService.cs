using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class CustomerAppService
    {
        readonly SMSService smsService;

        public CustomerAppService()
        {
            smsService = new SMSService();
        }

        public CustomerViewModel Login(CustomerLogin login)
        {
            using (var db = new BSCCSLEntity())
            {
                try
                {
                    var password = UserService.Encrypt(login.Password);
                    var customer = (from c in db.Customer.Where(x => !x.IsDelete)
                                    join cd in db.CustomerPersonalDetail.Where(x => !x.IsDelete) on c.CustomerId equals cd.CustomerId
                                    where c.ClienId == login.ClientId && c.Password == password
                                    group new { CustomerName = cd.FirstName + " " + cd.LastName } by new { c.CustomerId, c.ClienId } into cg
                                    select new CustomerViewModel
                                    {
                                        CustomerId = cg.Key.CustomerId,
                                        ClientId = cg.Key.ClienId,
                                        //CustomerName = cd.FirstName + " " + cd.LastName
                                        Customers = cg.Select(x => x.CustomerName)
                                    }).FirstOrDefault();

                    if (customer == null)
                        throw new Exception("Invalid Username and Password");

                    return customer;
                }
                catch (Exception ex)
                {
                    ErrorLogService.InsertLog(ex);
                    throw ex;
                }
            }
        }

        public Guid Register(CustomerRegister customerRegister)
        {
            using (var db = new BSCCSLEntity())
            {
                var customer = (from c in db.Customer.Where(x => !x.IsDelete)
                                join cd in db.CustomerPersonalDetail.Where(x => !x.IsDelete) on c.CustomerId equals cd.CustomerId
                                join ca in db.CustomerAddress.Where(x => !x.IsDelete) on cd.PersonalDetailId equals ca.PersonalDetailId
                                where c.ClienId == customerRegister.ClientId && ca.MobileNo == customerRegister.MobileNumber
                                select new CustomerRegisterOPT
                                {
                                    CustomerId = c.CustomerId,
                                    ClientId = c.ClienId,
                                    CustomerName = cd.FirstName + " " + cd.LastName,
                                    MobileNumber = ca.MobileNo,
                                    Password = c.Password
                                }).FirstOrDefault();

                if (customer == null)
                {
                    throw new Exception("Invalid details");
                }

                if (!string.IsNullOrWhiteSpace(customer.Password))
                {
                    throw new Exception("You have already registered");
                }

                if (string.IsNullOrWhiteSpace(customer.MobileNumber))
                {
                    throw new Exception("Mobile number is not registered");
                }

                var oldToken = db.UserTokens.Where(x => x.UserId == customer.CustomerId && x.TokenType == TokenType.Customer_Register && !x.IsUsed && !x.IsExpired).FirstOrDefault();
                if (oldToken != null && oldToken.ExpireTime >= DateTime.Now)
                {
                    oldToken.IsExpired = true;
                }

                customer.OTP = GenerateOTP();

                db.UserTokens.Add(new UserTokens
                {
                    UserId = customer.CustomerId,
                    TokenType = TokenType.Customer_Register,
                    Token = customer.OTP,
                    ExpireTime = DateTime.Now.AddMinutes(30),
                    CreatedDate = DateTime.Now,
                    IsUsed = false
                });
                db.SaveChanges();

                smsService.SendRegistrationOTP(customer);
                return customer.CustomerId;

            }
        }

        public void VerifyToken(VerifyOTP verifyOTP)
        {
            using (var db = new BSCCSLEntity())
            {
                var token = db.UserTokens.Where(x => x.UserId == verifyOTP.CustomerId && x.TokenType == verifyOTP.TokenType && x.Token == verifyOTP.Token)
                              .OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                if (token == null)
                {
                    throw new Exception("Invalid OTP");
                }

                if (token.IsExpired || token.ExpireTime < DateTime.Now)
                {
                    throw new Exception("OTP is expired");
                }

                if (token.IsUsed)
                {
                    throw new Exception("OTP has been used");
                }
            }
        }

        public void SetPassword(SetPasswordModel setPasswordModel)
        {
            using (var db = new BSCCSLEntity())
            {
                var customer = new Customer
                {
                    CustomerId = setPasswordModel.CustomerId,
                    Password = UserService.Encrypt(setPasswordModel.Password)
                };

                db.Customer.Attach(customer);
                db.Entry(customer).Property(x => x.Password).IsModified = true;
                db.SaveChanges();
            }
        }

        public bool CheckUpdate(string currentVersion, string platform)
        {
            var version = Decimal.Parse(currentVersion);
            using (var db = new BSCCSLEntity())
            {
                if (Decimal.Parse(db.Application.Where(x => x.Platform == platform && x.ApplicationType == "Customer").FirstOrDefault().Version) > version)
                {
                    return true;
                }
            }
            return false;
        }

        public Guid ForgotPassword(CustomerForgotPassword customerForgotPassword)
        {
            using (var db = new BSCCSLEntity())
            {
                var customer = (from c in db.Customer.Where(x => !x.IsDelete)
                                join cd in db.CustomerPersonalDetail.Where(x => !x.IsDelete) on c.CustomerId equals cd.CustomerId
                                join ca in db.CustomerAddress.Where(x => !x.IsDelete) on cd.PersonalDetailId equals ca.PersonalDetailId
                                where c.ClienId == customerForgotPassword.ClientId && ca.MobileNo == customerForgotPassword.MobileNumber
                                select new CustomerForgotPasswordOTP
                                {
                                    CustomerId = c.CustomerId,
                                    ClientId = c.ClienId,
                                    CustomerName = cd.FirstName + " " + cd.LastName,
                                    MobileNumber = ca.MobileNo
                                }).FirstOrDefault();

                if (customer == null)
                {
                    throw new Exception("Invalid details");
                }

                if (string.IsNullOrWhiteSpace(customer.MobileNumber))
                {
                    throw new Exception("Mobile number is not registered");
                }

                var oldToken = db.UserTokens.Where(x => x.UserId == customer.CustomerId && x.TokenType == TokenType.Customer_Forgot_Password && !x.IsUsed && !x.IsExpired).FirstOrDefault();
                if (oldToken != null && oldToken.ExpireTime >= DateTime.Now)
                {
                    oldToken.IsExpired = true;
                }

                customer.OTP = GenerateOTP();

                db.UserTokens.Add(new UserTokens
                {
                    UserId = customer.CustomerId,
                    TokenType = TokenType.Customer_Forgot_Password,
                    Token = customer.OTP,
                    ExpireTime = DateTime.Now.AddMinutes(30),
                    CreatedDate = DateTime.Now,
                    IsUsed = false
                });
                db.SaveChanges();

                SMSService.SendForgotPasswordOTP(customer);
                return customer.CustomerId;
            }
        }

        public List<CustomerAccountModel> GetAccountList(Guid CustomerId)
        {
            using (var db = new BSCCSLEntity())
            {
                return db.CustomerProduct.Where(x => x.IsActive && x.CustomerId == CustomerId)
                         .Select(x => new CustomerAccountModel
                         {
                             AccountNumber = x.AccountNumber,
                             CustomerProductId = x.CustomerProductId,
                             ProductType = x.ProductType
                         }).ToList();
            }
        }

        public AccountDetail GetAccountDetail(Guid CustomerProductId)
        {
            using (var db = new BSCCSLEntity())
            {
                var details = (from cp in db.CustomerProduct.Where(x => x.CustomerProductId == CustomerProductId)
                               join t in db.Transaction.Where(a => a.Status == Status.Unclear) on cp.CustomerProductId equals t.CustomerProductId into trans
                               from t in trans.DefaultIfEmpty()
                               group new { t } by new { cp.ProductType, cp.Balance, cp.AccountNumber, cp.CustomerProductId, cp.IsFreeze } into grp
                               select new AccountDetail
                               {
                                   ProductType = grp.Key.ProductType,
                                   CustomerProductId = grp.Key.CustomerProductId,
                                   AccountNo = grp.Key.AccountNumber,
                                   Balance = grp.Key.Balance ?? 0,
                                   UnclearBalance = grp.Select(a => a.t != null ? a.t.Amount : 0).Sum(),
                                   IsFreeze = grp.Key.IsFreeze
                               }).FirstOrDefault();

                if (details.ProductType == ProductType.Loan)
                {
                    var loan = (db.Loan.Where(a => a.CustomerProductId == details.CustomerProductId).Select(p => new { p.MonthlyInstallmentAmount, p.Term })).FirstOrDefault();

                    decimal TotalEMIAmount = 0;
                    if (!string.IsNullOrWhiteSpace(loan.Term))
                    {
                        TotalEMIAmount = (loan.MonthlyInstallmentAmount ?? 0) * Convert.ToDecimal(loan.Term);
                    }

                    int c = 0;
                    decimal TotalPaid = 0;
                    c = db.RDPayment.Where(a => a.CustomerProductId == details.CustomerProductId && a.IsPaid == true && a.RDPaymentType == RDPaymentType.Installment).Count();
                    if (c > 0)
                    {
                        TotalPaid = (db.RDPayment.Where(a => a.CustomerProductId == details.CustomerProductId && a.IsPaid == true && a.RDPaymentType == RDPaymentType.Installment).Sum(a => a.Amount));
                    }

                    details.PendingEMIAmount = TotalEMIAmount - TotalPaid;
                }

                details.MiniStatement = db.Transaction.Where(x => x.CustomerProductId == CustomerProductId)
                                          .Select(x => new MiniStatement
                                          {
                                              Date = x.TransactionTime,
                                              Amount = x.Amount,
                                              Type = x.Type,
                                              Remarks = x.Description
                                          }).OrderByDescending(x => x.Date).Take(10).ToList();

                return details;
            }
        }

        public object GetCustomerDetail(Guid CustomerId)
        {
            using (var db = new BSCCSLEntity())
            {
                return (from cd in db.CustomerPersonalDetail.Where(x => !x.IsDelete)
                        join ca in db.CustomerAddress.Where(x => !x.IsDelete) on cd.PersonalDetailId equals ca.PersonalDetailId
                        where cd.CustomerId == CustomerId
                        select new
                        {
                            CustomerId = cd.CustomerId,
                            FirstName = cd.FirstName,
                            LastName = cd.LastName,
                            ContactNumber = ca.MobileNo
                        }).FirstOrDefault();
            }
        }
        //For Mobile Product Enquiry
        public void SaveProductEnquiry(ProductEnquiry productEnquiry)
        {
            using (var db = new BSCCSLEntity())
            {

                productEnquiry.EnquirySource = EnquirySource.MobileApp;
                productEnquiry.Status = EnquiryStatus.New;
                db.ProductEnquiry.Add(productEnquiry);
                db.SaveChanges();
            }
        }

        public IList<CustomerNotificationModel> GetNotification(Guid CustomerId)
        {
            using (var db = new BSCCSLEntity())
            {
                var Today = DateTime.Now;
                return (from n in db.CustomerNotification
                        where (n.CustomerId == CustomerId || n.CustomerId == null) && n.ExpiryDate >= Today
                        select new CustomerNotificationModel
                        {
                            NotificationId = n.NotificationId,
                            NotificationText = n.Notification
                        }).ToList();
            }
        }

        #region Helpers

        private string GenerateOTP(int length = 6)
        {
            string chars = "0123456789";

            var stringChars = new char[length];
            var random = new Random();

            stringChars = Enumerable.Repeat(chars, stringChars.Length).Select(s => s[random.Next(s.Length)]).ToArray();

            return new string(stringChars);
        }

        #endregion
    }
}
