using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSCCSL.Models;
using System.Data.Entity;
using BSCCSL.Services.SQLDataAccess;


namespace BSCCSL.Services
{
    public class CustomerService
    {

        //TransactionService transactionService;

        //public CustomerService()
        //{
        //    transactionService = new TransactionService();
        //}

        public object CustomerRegister(CustomerDetail customerdetail)
        {
            using (var db = new BSCCSLEntity())
            {
                //var branchcode = db.Branch.Where(x => x.BranchId == customerdetail.Customer.BranchId).Select(x => x.BranchCode).FirstOrDefault();

                //var branchcode = db.Branch.Where(x=> x.BranchCode == customerdetail.Customer.BranchCode)
                if (customerdetail.Customer.CustomerId == Guid.Empty)
                {
                    var Branchcode = db.Branch.Where(x => x.BranchId == customerdetail.Customer.BranchId).Select(a => a.BranchCode).FirstOrDefault();
                    Setting setting = db.Setting.Where(a => a.SettingName == "ClientId").FirstOrDefault();
                    customerdetail.Customer.ClienId = Branchcode + setting.Value.ToString();

                    Setting setting1 = db.Setting.Where(s => s.SettingName == "ApplicationNo").FirstOrDefault();
                    customerdetail.Customer.ApplicationNo = setting1.Value.ToString();

                    db.Customer.Add(customerdetail.Customer);
                    db.SaveChanges();

                    foreach (var personalDetail in customerdetail.Personal)
                    {
                        personalDetail.Personal.CustomerId = customerdetail.Customer.CustomerId;
                        db.CustomerPersonalDetail.Add(personalDetail.Personal);
                        db.SaveChanges();

                        personalDetail.Address.CustomerId = customerdetail.Customer.CustomerId;
                        personalDetail.Address.PersonalDetailId = personalDetail.Personal.PersonalDetailId;
                        db.CustomerAddress.Add(personalDetail.Address);

                        foreach (var documents in personalDetail.Documents)
                        {
                            documents.PersonalDetailId = personalDetail.Personal.PersonalDetailId;
                            db.CustomerProofDocument.Add(documents);
                        }
                    }

                    int clientid = Convert.ToInt32(setting.Value) + 1;
                    setting.Value = clientid.ToString().PadLeft(6, '0');

                    int applicationnum = Convert.ToInt32(setting1.Value) + 1;
                    setting1.Value = applicationnum.ToString().PadLeft(6, '0');
                    db.Entry(setting).State = EntityState.Modified;
                    db.SaveChanges();
                    SMSService.SendNewCustomerSMS(customerdetail);

                }
                else if (customerdetail.Customer.CustomerId != Guid.Empty)
                {
                    var customerdata = db.Customer.Where(a => a.CustomerId == customerdetail.Customer.CustomerId).FirstOrDefault();

                    customerdata.AgentId = customerdetail.Customer.AgentId;
                    //customerdata.ApplicationNo = customerdetail.Customer.ApplicationNo;
                    customerdata.EmployeeId = customerdetail.Customer.EmployeeId;
                    customerdata.Sector = customerdetail.Customer.Sector;
                    customerdata.FormType = customerdetail.Customer.FormType;
                    customerdata.OldClientId = customerdetail.Customer.OldClientId;
                    customerdata.ModifiedBy = customerdetail.Customer.ModifiedBy;
                    customerdata.ModifiedDate = DateTime.Now;

                    foreach (var personalDetail in customerdetail.Personal)
                    {
                        if (personalDetail.Personal.PersonalDetailId == Guid.Empty)
                        {
                            personalDetail.Personal.CustomerId = customerdetail.Customer.CustomerId;
                            db.CustomerPersonalDetail.Add(personalDetail.Personal);
                            db.SaveChanges();

                            personalDetail.Address.CustomerId = customerdetail.Customer.CustomerId;
                            personalDetail.Address.PersonalDetailId = personalDetail.Personal.PersonalDetailId;
                            db.CustomerAddress.Add(personalDetail.Address);

                            foreach (var documents in personalDetail.Documents)
                            {
                                documents.PersonalDetailId = personalDetail.Personal.PersonalDetailId;
                                db.CustomerProofDocument.Add(documents);
                            }
                        }
                        else
                        {
                            var personal = db.CustomerPersonalDetail.Where(a => a.PersonalDetailId == personalDetail.Personal.PersonalDetailId).FirstOrDefault();

                            personal.FirstName = personalDetail.Personal.FirstName;
                            personal.MiddleName = personalDetail.Personal.MiddleName;
                            personal.LastName = personalDetail.Personal.LastName;
                            personal.FatherorSpouseName = personalDetail.Personal.FatherorSpouseName;
                            personal.MotherName = personalDetail.Personal.MotherName;
                            personal.DOB = personalDetail.Personal.DOB;
                            personal.Nationality = personalDetail.Personal.Nationality;
                            personal.Sex = personalDetail.Personal.Sex;
                            personal.Age = personalDetail.Personal.Age;
                            personal.PlaceOfBirth = personalDetail.Personal.PlaceOfBirth;
                            personal.Occupation = personalDetail.Personal.Occupation;
                            personal.BirthCertificate = personalDetail.Personal.BirthCertificate;
                            personal.DrivingLicence = personalDetail.Personal.DrivingLicence;
                            personal.Passport = personalDetail.Personal.Passport;
                            personal.PanCard = personalDetail.Personal.PanCard;
                            personal.Adharcard = personalDetail.Personal.Adharcard;
                            personal.IdentityProof = personalDetail.Personal.IdentityProof;
                            personal.Other = personalDetail.Personal.Other;
                            foreach (var documents in personalDetail.Documents)
                            {

                                var doc = db.CustomerProofDocument.Where(a => a.PersonalDetailId == personalDetail.Personal.PersonalDetailId && a.ProofTypeId == documents.ProofTypeId).FirstOrDefault();
                                if (doc != null)
                                {
                                    doc.DocumentName = documents.DocumentName;
                                    doc.Path = documents.Path;
                                }
                                else
                                {
                                    documents.PersonalDetailId = personalDetail.Personal.PersonalDetailId;
                                    db.CustomerProofDocument.Add(documents);
                                }
                            }

                            var addressDetail = personalDetail.Address;
                            var addressdata = db.CustomerAddress.Where(a => a.AddressId == addressDetail.AddressId).FirstOrDefault();
                            addressdata.DoorNo = addressDetail.DoorNo;
                            addressdata.BuildingName = addressDetail.BuildingName;
                            addressdata.PlotNo_Street = addressDetail.PlotNo_Street;
                            addressdata.CustomerName = addressDetail.CustomerName;
                            addressdata.Landmark = addressDetail.Landmark;
                            addressdata.Area = addressDetail.Area;
                            addressdata.District = addressDetail.District;
                            addressdata.Place = addressDetail.Place;
                            addressdata.City = addressDetail.City;
                            addressdata.State = addressDetail.State;
                            addressdata.Pincode = addressDetail.Pincode;
                            addressdata.TelephoneNo = addressDetail.TelephoneNo;
                            addressdata.MobileNo = addressDetail.MobileNo;
                            addressdata.Email = addressDetail.Email;
                            addressdata.AddressProof = addressDetail.AddressProof;
                            addressdata.PersonalDetailId = addressDetail.PersonalDetailId;
                            addressdata.PerDoorNo = addressDetail.PerDoorNo;
                            addressdata.PerBuildingName = addressDetail.PerBuildingName;
                            addressdata.PerPlotNo_Street = addressDetail.PerPlotNo_Street;
                            addressdata.PerCustomerName = addressDetail.PerCustomerName;
                            addressdata.PerLandmark = addressDetail.PerLandmark;
                            addressdata.PerArea = addressDetail.PerArea;
                            addressdata.PerDistrict = addressDetail.PerDistrict;
                            addressdata.PerPlace = addressDetail.PerPlace;
                            addressdata.PerCity = addressDetail.PerCity;
                            addressdata.PerState = addressDetail.PerState;
                            addressdata.PerPincode = addressDetail.PerPincode;
                            addressdata.PerTelephoneNo = addressDetail.PerTelephoneNo;
                            addressdata.PerMobileNo = addressDetail.PerMobileNo;
                            addressdata.PerEmail = addressDetail.PerEmail;
                            addressdata.PerAddressProof = addressDetail.PerAddressProof;
                        }
                    }

                    var uids = customerdetail.Personal.Select(p => p.Personal.PersonalDetailId).ToList();

                    db.CustomerPersonalDetail.Where(t => !uids.Contains(t.PersonalDetailId) && t.CustomerId == customerdetail.Customer.CustomerId).ToList().ForEach(a => a.IsDelete = true);
                    SMSService.SendNewCustomerSMS(customerdetail);
                }

                db.SaveChanges();
                var result = GetCustomerDetailsbyId(customerdetail.Customer.CustomerId);
                return result;
            }
        }

        public bool CheckCustomerExist(string Name, DateTime BirthDate)
        {
            using (var db = new BSCCSLEntity())
            {
                int c = db.CustomerPersonalDetail.Where(x => (x.FirstName + " " + x.MiddleName + " " + x.LastName).Contains(Name) && DbFunctions.TruncateTime(x.DOB) == DbFunctions.TruncateTime(BirthDate)).Count();

                if (c > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public object GetBalanceById(Guid Id)
        {

            using (var db = new BSCCSLEntity())
            {
                var getbalance = db.CustomerProduct.Where(x => x.CustomerId == Id && x.ProductType == ProductType.Saving_Account && x.IsActive == true).FirstOrDefault();
                return getbalance;
            }

        }

        public string GetBranchCode(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var branchcode = db.Branch.Where(x => x.BranchId == Id).Select(x => x.BranchCode).FirstOrDefault();
                    return branchcode;
                }
            }

            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);

            }
            return "";
        }

        public object GetEmployeeDetail(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {

                var query = (from ut in db.User.Where(s => s.IsDelete == false)
                             join co in db.Branch on ut.BranchId equals co.BranchId

                             select new
                             {

                                 UserId = ut.UserId,
                                 FirstName = ut.FirstName,
                                 LastName = ut.LastName,
                                 PhoneNumber = ut.PhoneNumber,
                                 UserCode = ut.UserCode,
                                 BranchId = co.BranchId,
                                 BranchCode = co.BranchCode,
                                 BranchName = co.BranchName,


                             }).AsQueryable();

                var employee = query.OrderBy(c => c.UserId).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = employee.Count(),
                    iTotalDisplayRecords = query.Count(),
                    aaData = employee
                };
                return data;
            }
        }

        public User GetEmployeeDetailById(Guid Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var employee = db.User.Where(c => c.UserId == Id && c.IsDelete == false).FirstOrDefault();
                return employee;
            }
        }

        public object GetCustomerList(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {

                var query = (from ut in db.Customer.Where(s => s.IsDelete == false && s.BranchId == search.BranchId)
                             join co in db.Branch on ut.BranchId equals co.BranchId
                             join cp in db.CustomerPersonalDetail on ut.CustomerId equals cp.CustomerId
                             select new
                             {
                                 ut.CustomerId,
                                 //ut.AccountNo,
                                 co.BranchId,
                                 co.BranchCode,
                                 co.BranchName,
                                 ut.ClienId,
                                 cp.FirstName,
                                 cp.MiddleName,
                                 cp.LastName,
                                 ut.OldClientId
                             }).AsQueryable();

                var res = (from q in query.AsEnumerable()
                           group q by new { q.CustomerId, q.BranchCode, q.BranchName, q.ClienId, q.OldClientId } into g
                           select new
                           {
                               CustomerId = g.Key.CustomerId,
                               BranchCode = g.Key.BranchCode,
                               ClientId = g.Key.ClienId,
                               BranchName = g.Key.BranchName,
                               CustomerName = string.Join(", ", g.Select(tg => tg.FirstName + " " + tg.MiddleName + " " + tg.LastName)),
                               OldClientId = g.Key.OldClientId
                           }).AsQueryable();

                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    res = res.Where(c => c.ClientId.Contains(search.sSearch.Trim()) || c.CustomerName.ToLower().Contains(search.sSearch.ToLower().Trim()) || (!string.IsNullOrEmpty(c.OldClientId) && c.OldClientId.Contains(search.sSearch.Trim())));
                }
                var customer = res.OrderBy(c => c.ClientId).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();
                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = customer.Count(),
                    iTotalDisplayRecords = res.Count(),
                    aaData = customer
                };
                return data;
            }
        }

        public object gettransactionDetails(Guid id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                //var transactionData = db.Transaction.Where(x => x.TransactionId == id).FirstOrDefault();
                //var  = Enum.GetName(typeof(TransactionType), transactionData.TransactionType).Cast<TransactionType>();

                var transactionData = (from c in db.Transaction.Where(x => x.TransactionId == id)
                                       join cp in db.CustomerProduct.Where(a => a.IsDelete == false) on c.CustomerId equals cp.CustomerId
                                       select new
                                       {
                                           CheckNumber = c.CheckNumber,
                                           TransactionType = c.TransactionType,
                                           ChequeDate = c.ChequeDate,
                                           BankName = c.BankName,
                                           Amount = c.Amount,
                                           TransactionTime = c.TransactionTime,
                                           AccountNumber = cp.AccountNumber,
                                           TransactionId = c.TransactionId,
                                           CustomerProductId = cp.CustomerProductId

                                       }).FirstOrDefault();
                var TransactionType = Enum.GetName(typeof(TransactionType), transactionData.TransactionType);

                return new
                {
                    transactionData,
                    TransactionType
                };
            }
        }

        public bool DeleteCustomer(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var user = db.Customer.Where(x => x.CustomerId == Id).FirstOrDefault();
                    user.IsDelete = true;
                    db.Entry(user).State = EntityState.Modified;

                    db.CustomerProduct.Where(a => a.CustomerId == Id).ToList().ForEach(u => u.IsDelete = true);

                    db.RDPayment.Where(a => a.CustomerId == Id).ToList().ForEach(u => u.IsDelete = true);

                    db.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public object GetCustomerDetailsbyId(Guid Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {

                var Cust = db.Customer.Where(X => X.CustomerId == Id).FirstOrDefault();

                var details = (from p in db.CustomerPersonalDetail.Where(a => a.CustomerId == Id && !a.IsDelete)
                               join a in db.CustomerAddress on p.PersonalDetailId equals a.PersonalDetailId
                               join d in db.CustomerProofDocument on p.PersonalDetailId equals d.PersonalDetailId into docs
                               from d in docs.DefaultIfEmpty()
                               group new { p, a, docs } by new { p, a } into g
                               select new DisplayCustomerDetail
                               {
                                   Personal = g.Key.p,
                                   Address = g.Key.a,
                                   Documents = g.FirstOrDefault().docs.ToList()
                               }).ToList();

                var customer = (from e in db.Customer.Where(a => a.CustomerId == Id && a.IsDelete == false)
                                join u in db.User on e.EmployeeId equals u.UserId into emp
                                from em in emp.DefaultIfEmpty()
                                join a in db.User on e.AgentId equals a.UserId into temp
                                from b in temp.DefaultIfEmpty()
                                join aa in db.User on e.CustomerId equals aa.CustomerId into templist
                                from aa in templist.DefaultIfEmpty()
                                select new
                                {
                                    CustomerId = e.CustomerId,
                                    e.AgentId,
                                    e.EmployeeId,
                                    ApplicationNo = e.ApplicationNo,
                                    Sector = e.Sector,
                                    FormType = e.FormType,
                                    EmpCode = em.UserCode,
                                    EmpName = em.FirstName + " " + em.LastName,
                                    AgentName = b.FirstName + " " + b.LastName,
                                    AgentCode = b.UserCode,
                                    OldClientId = e.OldClientId,
                                    BranchCode = e.BranchCode != null ? e.BranchCode : (db.Branch.Where(p => p.BranchId == Cust.BranchId).FirstOrDefault().BranchCode),
                                    UserAgent = aa == null ? null : aa.UserId.ToString()
                                }).FirstOrDefault();

                var nominee = db.Nominee.Where(b => b.CustomerId == Id).FirstOrDefault();

                var CustomerDocuments = db.CustomerDocuments.Where(b => b.CustomerId == Id && b.IsDeleted == false).ToList();


                var data = new
                {
                    Customer = customer,
                    Details = details,
                    Nominee = nominee,
                    CustomerDocuments = CustomerDocuments,
                };
                return data;
            }
        }

        public object GetCustomerDetailsForAgentCreationbyId(Guid Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {

                var Cust = db.Customer.Where(X => X.CustomerId == Id).FirstOrDefault();

                var details = (from p in db.CustomerPersonalDetail.Where(a => a.CustomerId == Id && !a.IsDelete)
                               join a in db.CustomerAddress on p.PersonalDetailId equals a.PersonalDetailId
                               select new DisplayCustomerDetail
                               {
                                   Personal = p,
                                   Address = a
                               }).FirstOrDefault();
                return details;
            }
        }

        public object SaveCustomerShare(CustomerShare customershare)
        {
            using (var db = new BSCCSLEntity())
            {

                TransactionService transactionService = new TransactionService();

                string shareno = customershare.Share.ToString();
                CustomerProduct customerProduct = db.CustomerProduct.Where(a => a.IsDelete == false && a.ProductType == ProductType.Saving_Account && a.IsActive == true && a.CustomerId == customershare.CustomerId).FirstOrDefault();
                // decimal transctionbalance = db.Transaction.Where(t => t.CustomerId == customershare.CustomerId).OrderByDescending(a => a.CreatedDate).FirstOrDefault().Balance;

                var setting = db.Setting.Where(s => s.SettingName == "ShareNumber").FirstOrDefault();
                Transactions transaction = new Transactions();
                if (customershare.ShareId == Guid.Empty && customershare.CustomerId != Guid.Empty)
                {

                    if (customershare.Maturity == Maturity.Regular)
                        customershare.DeductShareAmount = true;
                    else
                    {
                        if (customershare.Maturity == Maturity.Nominal || customershare.Maturity == Maturity.Premium)
                        {
                            if (customershare.DeductShareAmount == true)
                            {
                                customershare.DeductShareAmount = true;
                                if (customershare.DeductAdmissionFee == true)
                                {
                                    customershare.DeductAdmissionFee = true;
                                }
                                else
                                {
                                    customershare.DeductAdmissionFee = false;
                                }
                            }
                            else
                            {
                                customershare.DeductShareAmount = false;
                            }
                        }
                    }


                    int totalfrom = Convert.ToInt32(setting.Value) + Convert.ToInt32(1);
                    customershare.FromNumber = totalfrom.ToString();

                    int sharenum = Convert.ToInt32(setting.Value) + Convert.ToInt32(shareno);
                    setting.Value = sharenum.ToString();
                    db.SaveChanges();

                    customershare.ToNumber = sharenum.ToString();
                    db.CustomerShare.Add(customershare);
                    db.SaveChanges();

                    decimal deductAmt = 0;

                    bool DeductAmount = false;

                    if (customershare.Maturity == Maturity.Regular)
                    {
                        DeductAmount = true;
                    }
                    else
                    {
                        if (customershare.Maturity == Maturity.Nominal || customershare.Maturity == Maturity.Premium)
                        {
                            if (customershare.DeductShareAmount == true)
                                DeductAmount = true;
                        }
                    }
                    //if (customershare.Maturity == Maturity.Regular)
                    if (DeductAmount)
                    {
                        transaction.Amount = customershare.Total;
                        //transaction.Balance = customerProduct.UpdatedBalance.Value - customershare.Total;
                        transaction.CustomerProductId = customerProduct.CustomerProductId;
                        transaction.CustomerId = customershare.CustomerId;
                        transaction.CreatedBy = customershare.CreatedBy;
                        transaction.CreatedDate = customershare.CreatedDate;
                        transaction.TransactionTime = DateTime.Now;
                        transaction.Type = TypeCRDR.DR;
                        transaction.Status = Status.Clear;
                        transaction.TransactionType = TransactionType.BankTransfer;
                        transaction.DescIndentify = DescIndentify.Share;
                        transaction.BranchId = customerProduct.BranchId;
                        transaction.TransactionId = transactionService.InsertTransctionUsingSP(transaction);
                        deductAmt = customershare.Total;
                        int sharecount = 0;
                        if (customershare.Maturity == Maturity.Regular)
                            sharecount = db.CustomerShare.Where(a => a.CustomerId == customershare.CustomerId && a.Maturity == Maturity.Regular).Count();
                        else
                        {
                            if (customershare.DeductShareAmount == true)
                            {
                                if (customershare.DeductAdmissionFee == true)
                                {
                                    sharecount = 1;
                                }
                                else
                                {
                                    sharecount = 0;
                                }
                            }
                        }

                        if (sharecount == 1)
                        {
                            deductAmt = deductAmt + 100;
                            Transactions transactionfee = new Transactions();
                            transactionfee.Amount = 100;
                            transactionfee.Balance = customerProduct.UpdatedBalance.Value - deductAmt;
                            transactionfee.CustomerProductId = customerProduct.CustomerProductId;
                            transactionfee.CustomerId = customershare.CustomerId;
                            transactionfee.CreatedBy = customershare.CreatedBy;
                            transactionfee.CreatedDate = customershare.CreatedDate;
                            transactionfee.TransactionTime = DateTime.Now.AddSeconds(5);
                            transactionfee.Type = TypeCRDR.DR;
                            transactionfee.Status = Status.Clear;
                            transactionfee.TransactionType = TransactionType.BankTransfer;
                            transactionfee.DescIndentify = DescIndentify.AdmissionFee;
                            transactionfee.BranchId = customerProduct.BranchId;
                            transactionfee.TransactionId = transactionService.InsertTransctionUsingSP(transactionfee);
                        }
                    }

                    //customerProduct.Balance = customerProduct.Balance - deductAmt;
                    //customerProduct.UpdatedBalance = customerProduct.UpdatedBalance - deductAmt;
                    //db.SaveChanges();
                }

                return customershare;
            }
        }

        public object GetShareList(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = db.CustomerShare.Where(s => s.CustomerId == search.id && s.IsDelete == false).AsQueryable();

                var share = list.OrderBy(c => c.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = share.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = share
                };
                return data;
            }
        }

        public bool DeleteShare(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var user = db.CustomerShare.Where(x => x.ShareId == Id).FirstOrDefault();
                    user.IsDelete = true;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public object GetHolderData(string Account)
        {
            CustomerServiceSQL customerService = new CustomerServiceSQL();

            return customerService.GetHolderDataSQL(Account);

            #region "Old Method"
            //using (var db = new BSCCSLEntity())
            //{

            //    var cpdetail = (from cp in db.CustomerProduct.AsEnumerable().Where(x => x.IsDelete == false && x.AccountNumber == Account)
            //                    join c in db.Customer.Where(x => x.IsDelete == false) on cp.CustomerId equals c.CustomerId
            //                    join b in db.Branch on c.BranchId equals b.BranchId
            //                    join p in db.Product on cp.ProductId equals p.ProductId
            //                    join t in db.Transaction.Where(a => a.Status == Status.Unclear) on cp.CustomerProductId equals t.CustomerProductId into trans
            //                    from t in trans.DefaultIfEmpty()
            //                    group new { t } by new { cp.ProductType, cp.ProductTypeName, b.BranchId, b.BranchName, cp.CustomerId, cp.CustomerProductId, 
            //                        cp.Balance, cp.AccountNumber, cp.Amount, cp.LastInstallmentDate, cp.IsFreeze, cp.Status,p.ProductName,p.ProductCode } into grp
            //                    select new
            //                    {
            //                        ProductType = grp.Key.ProductType,
            //                        ProductTypeName = grp.Key.ProductTypeName,
            //                        BranchID = grp.Key.BranchId,
            //                        BranchName = grp.Key.BranchName,
            //                        CustomerId = grp.Key.CustomerId,
            //                        AccountNo = grp.Key.AccountNumber,
            //                        CustomerProductId = grp.Key.CustomerProductId,
            //                        Balance = grp.Key.Balance != null ? grp.Key.Balance : 0,
            //                        UnclearBalance = grp.Select(a => a.t != null ? a.t.Amount : 0).Sum(),
            //                        Amount = grp.Key.Amount,
            //                        LastInstallmentDate = grp.Key.LastInstallmentDate,
            //                        IsFreeze = grp.Key.IsFreeze,
            //                        Status = grp.Key.Status,
            //                        ProductCode = grp.Key.ProductCode,
            //                        ProductName = grp.Key.ProductName
            //                        //transactionBlance = (db.Transaction.Where(x => x.CustomerId == grp.Key.CustomerId).OrderByDescending(x => x.TransactionTime).Select(x=> x.Balance).FirstOrDefault())
            //                        //transactionBlance = db.CustomerProduct.Where(x => x.CustomerId == grp.Key.CustomerProductId).Select(x => x.Balance)
            //                    }).FirstOrDefault();

            //    var data = new object();

            //    if (cpdetail != null)
            //    {
            //        var customerpersonaldetail = new object();

            //        if (cpdetail != null)
            //        {
            //            customerpersonaldetail = (from cp in db.CustomerPersonalDetail.Where(a => !a.IsDelete && a.CustomerId == cpdetail.CustomerId)
            //                                      join ca in db.CustomerAddress on cp.PersonalDetailId equals ca.PersonalDetailId
            //                                      orderby cp.CustomerId
            //                                      select new
            //                                      {
            //                                          FirstName = cp.FirstName,
            //                                          MiddleName = cp.MiddleName,
            //                                          LastName = cp.LastName,
            //                                          DOB = cp.DOB,
            //                                          Sex = cp.Sex,
            //                                          Address = (!string.IsNullOrEmpty(ca.DoorNo) ? ca.DoorNo + ", " : "") + (!string.IsNullOrEmpty(ca.BuildingName) ? ca.BuildingName + ", " : "") + (!string.IsNullOrEmpty(ca.PlotNo_Street) ? ca.PlotNo_Street + ", " : "") + (!string.IsNullOrEmpty(ca.Area) ? ca.Area + ", " : "") + (!string.IsNullOrEmpty(ca.City) ? ca.City + ", " : "") + (!string.IsNullOrEmpty(ca.District) ? ca.District + ", " : "") + (!string.IsNullOrEmpty(ca.State) ? ca.State + ", " : "") + (!string.IsNullOrEmpty(ca.Pincode) ? ca.Pincode : ""),
            //                                          CustomerId = cp.CustomerId,
            //                                          HolderPhoto = cp.HolderPhotograph,
            //                                          Holdersign = cp.HolderSign
            //                                      }).ToList();
            //        }

            //        decimal TotalPendingAmount = 0;

            //        try
            //        {
            //            if (cpdetail.ProductType == ProductType.Loan)
            //            {
            //                decimal TotalEMIAmount = 0;
            //                TotalEMIAmount = db.Loan.Where(a => a.CustomerProductId == cpdetail.CustomerProductId).Select(p => p.TotalAmountToPay.Value).FirstOrDefault();



            //                int c = 0;
            //                decimal TotalPaid = 0;
            //                c = db.RDPayment.Where(a => a.CustomerProductId == cpdetail.CustomerProductId && a.IsPaid == true && a.RDPaymentType == RDPaymentType.Installment).Count();
            //                if (c > 0)
            //                {
            //                    TotalPaid = (db.RDPayment.Where(a => a.CustomerProductId == cpdetail.CustomerProductId && a.IsPaid == true && a.RDPaymentType == RDPaymentType.Installment).Sum(a => a.Amount));
            //                }

            //                TotalPendingAmount = TotalEMIAmount - TotalPaid;
            //                if (TotalPendingAmount < 0)
            //                {
            //                    TotalPendingAmount = 0;
            //                }
            //            }

            //        }
            //        catch (Exception ex)
            //        {
            //            ErrorLogService.InsertLog(ex);
            //        }

            //        data = new
            //        {
            //            details = customerpersonaldetail,
            //            AccountDetail = cpdetail,
            //            TotalLoanPendingAmount = TotalPendingAmount
            //        };
            //    }

            //    return data;
            //}
            #endregion
        }

        //public bool UpdateBalance(CustomerUpdateData customer)
        //{
        //    using (var db = new BSCCSLEntity())
        //    {
        //        //Customer result = (from c in db.Customer where c.CustomerId == customer.CustomerId select c).FirstOrDefault();
        //        //result.Balance = customer.ProductwiseBalance;

        //        if (customer.TransactionType == 0)
        //        if (customer.TransactionType == BSCCSL.Models.TransactionType.Cash)
        //        {
        //            CustomerProduct customerProduct = (from cp in db.CustomerProduct where cp.CustomerProductId == customer.CustomerProductId select cp).FirstOrDefault();
        //            customerProduct.Balance = customer.Balance;
        //        }
        //        db.SaveChanges();
        //        return true;
        //    }
        //}

        public CustomerShare GetShareDataById(Guid Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var share = db.CustomerShare.Where(c => c.ShareId == Id && c.IsDelete == false).FirstOrDefault();
                return share;
            }
        }

        public object GetShareDetailForPrint(Guid Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var ShareDetails = db.CustomerShare.Where(x => x.ShareId == Id).FirstOrDefault();
                var CustomerPersonalDetails = db.CustomerPersonalDetail.Where(x => x.CustomerId == ShareDetails.CustomerId && !x.IsDelete).Select(x => x.FirstName + " " + x.MiddleName + " " + x.LastName).ToList();

                string trimmedString = string.Join<string>(" , ", CustomerPersonalDetails);
                var Address = (from c in db.Customer.Where(x => x.CustomerId == ShareDetails.CustomerId)
                               join cp in db.CustomerPersonalDetail.Where(a => a.IsDelete == false) on c.CustomerId equals cp.CustomerId
                               join ca in db.CustomerAddress on cp.PersonalDetailId equals ca.PersonalDetailId
                               select new
                               {
                                   MembershipNo = c.ClienId,
                                   address = (!string.IsNullOrEmpty(ca.DoorNo) ? ca.DoorNo + ", " : "") + (!string.IsNullOrEmpty(ca.PlotNo_Street) ? ca.PlotNo_Street + ", " : "") + (!string.IsNullOrEmpty(ca.Landmark) ? ca.Landmark + ", " : "") + (!string.IsNullOrEmpty(ca.Area) ? ca.Area + ", " : "") + (!string.IsNullOrEmpty(ca.District) ? ca.District + ", " : "") + (!string.IsNullOrEmpty(ca.Pincode) ? ca.Pincode : "")
                               }).FirstOrDefault();

                var nominee = db.Nominee.Where(x => x.CustomerId == ShareDetails.CustomerId).Select(x => x.Name).FirstOrDefault();
                return new
                {
                    ShareDetails,
                    trimmedString,
                    Address,
                    nominee
                };
            }
        }

        public Nominee SaveNominee(Nominee nominee, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (nominee.NomineeId == Guid.Empty && nominee.CustomerId != Guid.Empty)
                {
                    nominee.CreatedBy = nominee.CreatedBy;
                    nominee.CustomerId = nominee.CustomerId;
                    nominee.NomineeDOB = nominee.NomineeDOB;
                    nominee.AppointeeDOB = nominee.AppointeeDOB;
                    db.Nominee.Add(nominee);
                    db.SaveChanges();
                }
                else
                {
                    var nomineedata = db.Nominee.Where(a => a.CustomerId == nominee.CustomerId).FirstOrDefault();
                    nomineedata.Name = nominee.Name;
                    nomineedata.PlaceofBirth = nominee.PlaceofBirth;
                    nomineedata.NomineeDOB = nominee.NomineeDOB;
                    nomineedata.RelationtoAccountholder = nominee.RelationtoAccountholder;
                    nomineedata.AppointeeName = nominee.AppointeeName;
                    nomineedata.AppointeePlaceofBirth = nominee.AppointeePlaceofBirth;
                    nomineedata.AppointeeDOB = nominee.AppointeeDOB;
                    nomineedata.AppointeeRelationtoAcholder = nominee.AppointeeRelationtoAcholder;
                    nomineedata.RelationtoNominee = nominee.RelationtoNominee;
                    nomineedata.ModifiedBy = user.UserId;
                    nomineedata.ModifiedDate = DateTime.Now;
                }
                db.SaveChanges();
                return nominee;
            }
        }

        public List<Guid> GetHolders(Guid Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                List<Guid> holder = db.CustomerPersonalDetail.Where(c => c.CustomerId == Id && !c.IsDelete).Select(c => c.PersonalDetailId).ToList();
                return holder;
            }
        }

        public List<UploadPhotograph> SaveHolderPhoto(List<UploadPhotograph> customerdetail)
        {
            using (var db = new BSCCSLEntity())
            {
                foreach (var Holder in customerdetail)
                {
                    CustomerPersonalDetail p = db.CustomerPersonalDetail.Where(c => c.PersonalDetailId == Holder.Id && !c.IsDelete).FirstOrDefault();
                    p.HolderPhotograph = Holder.Path;
                    db.SaveChanges();
                    //db.CustomerPersonalDetail.Add(p);
                }
                db.SaveChanges();
                return customerdetail;
            }
        }

        public List<UploadPhotograph> SaveHolderSign(List<UploadPhotograph> customerdetail)
        {
            using (var db = new BSCCSLEntity())
            {
                foreach (var Holder in customerdetail)
                {
                    CustomerPersonalDetail p = db.CustomerPersonalDetail.Where(c => c.PersonalDetailId == Holder.Id && !c.IsDelete).FirstOrDefault();
                    p.HolderSign = Holder.Path;
                    db.SaveChanges();
                    //db.CustomerPersonalDetail.Add(p);
                }

                db.SaveChanges();

                return customerdetail;
            }
        }

        public object GetCustomerDocument(Guid Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var documentlist = db.CustomerProofDocument.Where(x => x.PersonalDetailId == Id && x.IsDelete == false).ToList();

                return documentlist;

                //var list = (from A in db.Customer.Where(s => s.IsDelete == false)
                //            join co in db.CustomerPersonalDetail on A.CustomerId equals co.CustomerId
                //            join cp in db.CustomerProofDocument on co.PersonalDetailId equals cp.PersonalDetailId

                //            select new
                //            {
                //                ProofTypeId = cp.ProofTypeId,
                //                DocumentName = cp.DocumentName
                //            }).Distinct().AsQueryable();
                //return list;
            }
        }

        public List<CustomerDocuments> SaveCustomerDocuments(CustomerDocuments customerdetail)
        {
            List<CustomerDocuments> objCustomerDocuments = new List<CustomerDocuments>();
            using (var db = new BSCCSLEntity())
            {
                db.CustomerDocuments.Add(customerdetail);
                db.SaveChanges();

                objCustomerDocuments = db.CustomerDocuments.Where(x => x.CustomerId == customerdetail.CustomerId && x.IsDeleted == false).ToList();

                return objCustomerDocuments;
            }
        }

        public object DeleteCustomerDocument(Guid DocumentId)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                CustomerDocuments objCustomerDocuments = db.CustomerDocuments.Where(x => x.DocumentId == DocumentId).FirstOrDefault();
                Guid CustomerId = Guid.Empty;
                if (objCustomerDocuments != null)
                {
                    CustomerId = (Guid)objCustomerDocuments.CustomerId;
                    objCustomerDocuments.IsDeleted = true;
                    db.SaveChanges();
                }

                var documentlist = db.CustomerDocuments.Where(x => x.CustomerId == CustomerId && x.IsDeleted == false).ToList();

                return documentlist;

            }
        }


        public int RefundShareAmount(Guid Id, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                int Count = 0;
                CustomerShare objCustomerShare = new CustomerShare();
                CustomerProduct objCustomerProduct = new CustomerProduct();
                objCustomerShare = db.CustomerShare.Where(a => a.ShareId == Id).FirstOrDefault();
                if (objCustomerShare != null)
                {
                    if (objCustomerShare.IsReverted == null || objCustomerShare.IsReverted == false)
                    {
                        objCustomerProduct = db.CustomerProduct.Where(a => a.CustomerId == objCustomerShare.CustomerId && a.ProductType == ProductType.Saving_Account && a.IsActive == true && a.IsDelete == false).FirstOrDefault();
                        TransactionService transactionService = new TransactionService();

                        Transactions transaction = new Transactions();
                        transaction.BranchId = objCustomerProduct.BranchId;
                        transaction.CustomerId = objCustomerProduct.CustomerId;
                        transaction.CustomerProductId = objCustomerProduct.CustomerProductId;
                        transaction.Amount = objCustomerShare.Total;
                        transaction.Status = Status.Clear;
                        transaction.TransactionType = TransactionType.BankTransfer;
                        transaction.Type = TypeCRDR.CR;
                        transaction.TransactionTime = DateTime.Now;
                        transaction.CreatedDate = DateTime.Now;
                        transaction.CreatedBy = user.UserId;
                        transaction.RefCustomerProductId = null;
                        transaction.DescIndentify = DescIndentify.Share;
                        transaction.TransactionId = transactionService.InsertTransctionUsingSP(transaction);
                        objCustomerShare.IsReverted = true;
                        Count = db.SaveChanges();

                    }
                    else
                    {
                        Count = 1;
                    }
                }

                return Count;
            }
        }



    }
}
