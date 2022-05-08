using BSCCSL.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BSCCSL.Services
{
    public class CustomerProductService
    {

        NumericToWordService numericToWordService;

        public CustomerProductService()
        {
            numericToWordService = new NumericToWordService();
        }

        public bool CheckCustomerAccountExist(AccountExist accountExist)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {

                    int account = db.CustomerProduct.Where(x => x.IsDelete == false && x.IsActive == true && x.ProductType == accountExist.ProductType && x.CustomerId == accountExist.CustomerId && x.ProductId == accountExist.ProductId).Count();

                    if (account > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public object GetSavingAccountNo(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                var customeraccount = db.CustomerProduct.Where(x => x.CustomerId == Id && x.ProductType == ProductType.Saving_Account && x.IsDelete == false && x.IsActive == true).Select(x => x.AccountNumber).FirstOrDefault();
                return customeraccount; ;
            }
        }

        public object GetProductNameAsSelectedType(ProductType Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var PrList = (from p in db.Product.Where(x => x.ProductType == Id && x.IsActive && x.IsDelete == false)
                                  join l in db.Lookup.Where(x => x.IsDelete == false) on p.LoanTypeId equals l.LookupId into lnList
                                  from l in lnList.DefaultIfEmpty()
                                  select new
                                  {
                                      ProductId = p.ProductId,
                                      ProductType = p.ProductType,
                                      ProductName = p.ProductName,
                                      ProductCode = p.ProductCode,
                                      LoanTypeId = p.LoanTypeId,
                                      InterestRate = p.InterestRate,
                                      InterestType = p.InterestType,
                                      Frequency = p.Frequency,
                                      PaymentType = p.PaymentType,
                                      StartDate = p.StartDate,
                                      EndDate = p.EndDate,
                                      LatePaymentFees = p.LatePaymentFees,
                                      TimePeriod = p.TimePeriod,
                                      NoOfMonthsORYears = p.NoOfMonthsORYears,
                                      LoanType = l.Name
                                  }).ToList();

                    //var productList = db.Product.Where(x => x.ProductType == Id && x.IsActive && x.IsDelete == false).ToList();
                    return PrList;
                }
            }

            catch (Exception ex)
            {

            }
            return null;
        }

        public object GetProductNameOfGroupLoan()
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var Lookup = db.Lookup.Where(s => s.Name == "Group Loan").Select(s => s.LookupId).FirstOrDefault();
                    var productList = db.Product.Where(x => x.IsActive && x.IsDelete == false && x.ProductType == ProductType.Loan && x.LoanTypeId == Lookup).ToList();
                    return productList;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public object GetProductDetailsAsSelectedName(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var ProductDetails = db.Product.Where(x => x.ProductId == Id && x.IsDelete == false && x.IsActive).FirstOrDefault();
                    return ProductDetails;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public object SaveCustomerProductData(CustomerProduct CustomerProductdata, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                TransactionService transactionService = new TransactionService();

                var branchcode = db.Branch.Where(x => x.BranchId == CustomerProductdata.BranchId).Select(x => x.BranchCode).FirstOrDefault();

                if (CustomerProductdata.CustomerProductId == Guid.Empty)
                {
                    if (CustomerProductdata.IsAutoFD != null)
                    {
                        if ((bool)CustomerProductdata.IsAutoFD)
                            CustomerProductdata.ProductId = db.Product.Where(a => a.ProductName == "FIX DEPOSIT").Select(a => a.ProductId).FirstOrDefault();
                    }

                    string productcode = db.Product.Where(a => a.ProductId == CustomerProductdata.ProductId).Select(a => a.ProductCode).FirstOrDefault();

                    if (CustomerProductdata.TimePeriod.HasValue && CustomerProductdata.NoOfMonthsORYears.HasValue && (CustomerProductdata.ProductType == ProductType.Recurring_Deposit || CustomerProductdata.ProductType == ProductType.Fixed_Deposit || CustomerProductdata.ProductType == ProductType.Regular_Income_Planner || CustomerProductdata.ProductType == ProductType.Three_Year_Product))
                    {
                        int totalmonthyear = CalculateTotalInstallment(CustomerProductdata.TimePeriod.Value, CustomerProductdata.PaymentType, CustomerProductdata.NoOfMonthsORYears.Value);
                        CustomerProductdata.TotalInstallment = totalmonthyear;
                        CustomerProductdata.TotalDays = Convert.ToInt32((CustomerProductdata.MaturityDate.Value - CustomerProductdata.OpeningDate).TotalDays);
                    }

                    if (!string.IsNullOrEmpty(productcode))
                    {
                        Setting setting = db.Setting.Where(a => a.SettingName == CustomerProductdata.ProductTypeName + "No").FirstOrDefault();

                        if (setting != null)
                        {
                            CustomerProductdata.AccountNumber = branchcode + productcode + setting.Value.ToString();

                            int Prodcutnum = Convert.ToInt32(setting.Value) + 1;
                            setting.Value = Prodcutnum.ToString().PadLeft(6, '0');
                            CustomerProductdata.Balance = 0;
                            CustomerProductdata.UpdatedBalance = 0;
                            db.CustomerProduct.Add(CustomerProductdata);
                            db.SaveChanges();
                            SMSService.SendNewAccountOpenSMS(CustomerProductdata.CustomerProductId);
                        }
                    }

                    Guid SuperSavingAccountProductId = Guid.Empty;
                    Guid SmartSavingPlusAccountProductId = Guid.Empty;
                    Guid AkshyaTrityaProductId = Guid.Empty;
                    //Guid.TryParse("fe3d226d-5c2e-e711-80e4-00505621610d", out SuperSavingAccountProductId);

                    Product objProduct = db.Product.Where(x => x.ProductName.Contains("Super Saving") && x.ProductCode == "SA02").FirstOrDefault();
                    if (objProduct != null)
                        SuperSavingAccountProductId = objProduct.ProductId;

                    if (CustomerProductdata.ProductId == SuperSavingAccountProductId)
                    {
                        User objuser = db.User.Where(x => x.UserId == CustomerProductdata.AgentId && (x.IsExecutive == null || x.IsExecutive == false)).FirstOrDefault();
                        if (objuser != null)
                        {
                            CustomerProduct objCustomerProduct = db.CustomerProduct.Where(a => a.CustomerId == objuser.CustomerId && a.IsActive == true && a.IsDelete == false && (a.ProductType == ProductType.Saving_Account)).FirstOrDefault();
                            if (objCustomerProduct != null)
                            {
                                Transactions transaction = new Transactions();
                                transaction.BranchId = objCustomerProduct.BranchId;
                                transaction.CustomerId = objCustomerProduct.CustomerId;
                                transaction.CustomerProductId = objCustomerProduct.CustomerProductId;
                                transaction.Amount = 200;
                                transaction.Status = Status.Clear;
                                transaction.TransactionType = TransactionType.BankTransfer;
                                transaction.Type = TypeCRDR.CR;
                                transaction.TransactionTime = DateTime.Now;
                                transaction.CreatedDate = DateTime.Now;
                                transaction.CreatedBy = user.UserId;
                                transaction.RefCustomerProductId = objCustomerProduct.CustomerProductId;
                                transaction.DescIndentify = DescIndentify.Super_Saving_Commission;
                                transaction.TransactionId = transactionService.InsertTransctionUsingSP(transaction);
                            }
                        }
                    }


                    Product objProduct1 = db.Product.Where(x => x.ProductName.Contains("Smart Saving Plus") && x.ProductCode == "SA04").FirstOrDefault();
                    if (objProduct1 != null)
                        SmartSavingPlusAccountProductId = objProduct1.ProductId;

                    if (CustomerProductdata.ProductId == SmartSavingPlusAccountProductId)
                    {
                        User objuser = db.User.Where(x => x.UserId == CustomerProductdata.AgentId && (x.IsExecutive == null || x.IsExecutive == false)).FirstOrDefault();
                        if (objuser != null)
                        {
                            CustomerProduct objCustomerProduct = db.CustomerProduct.Where(a => a.CustomerId == objuser.CustomerId && a.IsActive == true && a.IsDelete == false && a.ProductType == ProductType.Saving_Account).FirstOrDefault();
                            if (objCustomerProduct != null)
                            {
                                Transactions transaction = new Transactions();
                                transaction.BranchId = objCustomerProduct.BranchId;
                                transaction.CustomerId = objCustomerProduct.CustomerId;
                                transaction.CustomerProductId = objCustomerProduct.CustomerProductId;
                                transaction.Amount = 500;
                                transaction.Status = Status.Clear;
                                transaction.TransactionType = TransactionType.BankTransfer;
                                transaction.Type = TypeCRDR.CR;
                                transaction.TransactionTime = DateTime.Now;
                                transaction.CreatedDate = DateTime.Now;
                                transaction.CreatedBy = user.UserId;
                                transaction.RefCustomerProductId = objCustomerProduct.CustomerProductId;
                                transaction.DescIndentify = DescIndentify.Smart_Saving_Plus_Commission;
                                transaction.TransactionId = transactionService.InsertTransctionUsingSP(transaction);
                            }
                        }
                    }

                    Product objProduct2 = db.Product.Where(x => x.ProductName.Contains("Akshaya Tritiya") && x.ProductCode == "RD03").FirstOrDefault();
                    if (objProduct2 != null)
                        AkshyaTrityaProductId = objProduct2.ProductId;
                    if (CustomerProductdata.ProductId == AkshyaTrityaProductId)
                    {
                        CreateReferencedRDProducts(CustomerProductdata, user);
                    }

                }
                else
                {
                    var CustomerProductData = db.CustomerProduct.Where(x => x.CustomerProductId == CustomerProductdata.CustomerProductId).FirstOrDefault();
                    CustomerProductData.ModifiedDate = DateTime.Now.Date;
                    CustomerProductData.ProductId = CustomerProductdata.ProductId;
                    CustomerProductData.EmployeeId = CustomerProductdata.EmployeeId;
                    CustomerProductData.AgentId = CustomerProductdata.AgentId;
                    CustomerProductData.PaymentType = CustomerProductdata.PaymentType;
                    CustomerProductData.ProductType = CustomerProductdata.ProductType;
                    CustomerProductData.AccountNumber = CustomerProductdata.AccountNumber;
                    CustomerProductData.InterestRate = CustomerProductdata.InterestRate;
                    CustomerProductData.LatePaymentFees = CustomerProductdata.LatePaymentFees;
                    CustomerProductData.IsActive = CustomerProductdata.IsActive;
                    CustomerProductData.InsuranceTypeLI = CustomerProductdata.InsuranceTypeLI;
                    CustomerProductData.InsuranceTypeGI = CustomerProductdata.InsuranceTypeGI;
                    CustomerProductData.LIType = CustomerProductdata.LIType;
                    CustomerProductData.LICommencementDate = CustomerProductdata.LICommencementDate;
                    CustomerProductData.LIPremium = CustomerProductdata.LIPremium;
                    CustomerProductData.GIPremium = CustomerProductdata.GIPremium;
                    CustomerProductData.GICommencementDate = CustomerProductdata.GICommencementDate;
                    CustomerProductData.LIDueDate = CustomerProductdata.LIDueDate;
                    CustomerProductData.GIDueDate = CustomerProductdata.GIDueDate;

                    CustomerProductData.CertificateNumber = CustomerProductdata.CertificateNumber;
                    CustomerProductData.OldAccountNumber = CustomerProductdata.OldAccountNumber;

                    if (CustomerProductData.Status != null && CustomerProductData.Status == CustomerProductStatus.Pending)
                    {
                        CustomerProductData.OpeningDate = CustomerProductdata.OpeningDate;
                        CustomerProductData.MaturityDate = CustomerProductdata.MaturityDate;
                        CustomerProductData.MaturityAmount = CustomerProductdata.MaturityAmount;
                        CustomerProductData.OpeningBalance = CustomerProductdata.OpeningBalance;
                        CustomerProductData.NoOfMonthsORYears = CustomerProductdata.NoOfMonthsORYears;
                        CustomerProductData.TimePeriod = CustomerProductdata.TimePeriod;
                        CustomerProductData.SkipFirstInstallment = CustomerProductdata.SkipFirstInstallment;
                        CustomerProductData.DueDate = CustomerProductdata.DueDate;
                        CustomerProductData.Amount = CustomerProductdata.Amount;

                        if (CustomerProductData.TimePeriod.HasValue && CustomerProductdata.NoOfMonthsORYears.HasValue && CustomerProductdata.Status == CustomerProductStatus.Pending &&
                            (CustomerProductdata.ProductType == ProductType.Recurring_Deposit || CustomerProductdata.ProductType == ProductType.Fixed_Deposit
                            || CustomerProductdata.ProductType == ProductType.Regular_Income_Planner || CustomerProductdata.ProductType == ProductType.Monthly_Income_Scheme))
                        {
                            int totalmonthyear = CalculateTotalInstallment(CustomerProductdata.TimePeriod.Value, CustomerProductdata.PaymentType, CustomerProductdata.NoOfMonthsORYears.Value);
                            CustomerProductData.TotalInstallment = totalmonthyear;
                            CustomerProductData.TotalDays = Convert.ToInt32((CustomerProductdata.MaturityDate.Value - CustomerProductdata.OpeningDate).TotalDays) - 1;
                        }
                    }
                    // CustomerProductData.InsuranceCommencementDate = CustomerProductdata.InsuranceCommencementDate;
                    //CustomerProductData.Premium = CustomerProductdata.Premium;
                    CustomerProductData.ModifyBy = user.UserId;


                    db.Entry(CustomerProductData).State = EntityState.Modified;

                    db.SaveChanges();
                    Guid AkshyaTrityaProductId = Guid.Empty;
                    Product objProduct2 = db.Product.Where(x => x.ProductName.Contains("Akshaya Tritiya") && x.ProductCode == "RD03").FirstOrDefault();
                    if (objProduct2 != null)
                        AkshyaTrityaProductId = objProduct2.ProductId;
                    if (CustomerProductdata.ProductId == AkshyaTrityaProductId)
                    {
                        CreateReferencedRDProducts(CustomerProductdata, user);
                    }

                }
            }
            return CustomerProductdata.CustomerProductId;
        }

        public int CreateReferencedRDProducts(CustomerProduct CustomerProductdata, User user)
        {
            try
            {
                BSCCSLEntity db = new BSCCSLEntity();
                int count = 0;
                if (CustomerProductdata.TimePeriod == TimePeriod.Years)
                {
                    List<CustomerProduct> oldMappedRdAccounts = db.CustomerProduct.Where(a => a.ReferenceCustomerProductId == CustomerProductdata.CustomerProductId).ToList();
                    if (oldMappedRdAccounts != null && oldMappedRdAccounts.Count > 0)
                    {
                        db.CustomerProduct.RemoveRange(oldMappedRdAccounts);
                        db.SaveChanges();
                    }
                    var branchcode = db.Branch.Where(x => x.BranchId == CustomerProductdata.BranchId).Select(x => x.BranchCode).FirstOrDefault();
                    Product objProduct = db.Product.Where(a => a.ProductName == "Recurring Deposit").FirstOrDefault();
                    List<CustomerProduct> NewMappedRdAccounts = new List<CustomerProduct>();
                    for (int i = 0; i < CustomerProductdata.NoOfMonthsORYears; i++)
                    {
                        CustomerProduct CustomerProductData = new CustomerProduct();
                        CustomerProductData.CustomerProductId = Guid.Empty;
                        CustomerProductData.ProductId = objProduct.ProductId;
                        CustomerProductData.CustomerId = CustomerProductdata.CustomerId;
                        CustomerProductData.BranchId = CustomerProductdata.BranchId;
                        CustomerProductData.CreatedDate = DateTime.Now.Date;
                        CustomerProductData.ModifiedDate = DateTime.Now.Date;
                        CustomerProductData.ProductId = CustomerProductdata.ProductId;
                        CustomerProductData.EmployeeId = CustomerProductdata.EmployeeId;
                        CustomerProductData.AgentId = CustomerProductdata.AgentId;
                        CustomerProductData.PaymentType = CustomerProductdata.PaymentType;
                        CustomerProductData.ProductType = CustomerProductdata.ProductType;
                        CustomerProductData.AccountNumber = CustomerProductdata.AccountNumber;
                        CustomerProductData.InterestRate = CustomerProductdata.InterestRate;
                        CustomerProductData.LatePaymentFees = CustomerProductdata.LatePaymentFees;
                        CustomerProductData.IsActive = CustomerProductdata.IsActive;
                        CustomerProductData.InsuranceTypeLI = CustomerProductdata.InsuranceTypeLI;
                        CustomerProductData.InsuranceTypeGI = CustomerProductdata.InsuranceTypeGI;
                        CustomerProductData.LIType = CustomerProductdata.LIType;
                        CustomerProductData.LICommencementDate = CustomerProductdata.LICommencementDate;
                        CustomerProductData.LIPremium = CustomerProductdata.LIPremium;
                        CustomerProductData.GIPremium = CustomerProductdata.GIPremium;
                        CustomerProductData.GICommencementDate = CustomerProductdata.GICommencementDate;
                        CustomerProductData.LIDueDate = CustomerProductdata.LIDueDate;
                        CustomerProductData.GIDueDate = CustomerProductdata.GIDueDate;

                        CustomerProductData.CertificateNumber = CustomerProductdata.CertificateNumber;
                        CustomerProductData.OldAccountNumber = CustomerProductdata.OldAccountNumber;
                        CustomerProductData.Status = CustomerProductStatus.Pending;
                        //if (CustomerProductData.Status != null && CustomerProductData.Status == CustomerProductStatus.Pending)
                        //{
                        CustomerProductData.OpeningDate = CustomerProductdata.OpeningDate;
                        CustomerProductData.MaturityDate = CustomerProductdata.MaturityDate;
                        CustomerProductData.MaturityAmount = CustomerProductdata.MaturityAmount;
                        CustomerProductData.OpeningBalance = CustomerProductdata.OpeningBalance;
                        CustomerProductData.NoOfMonthsORYears = CustomerProductdata.NoOfMonthsORYears;
                        CustomerProductData.TimePeriod = CustomerProductdata.TimePeriod;
                        CustomerProductData.SkipFirstInstallment = CustomerProductdata.SkipFirstInstallment;
                        CustomerProductData.DueDate = CustomerProductdata.DueDate;
                        CustomerProductData.Amount = CustomerProductdata.Amount;

                        //if (CustomerProductData.TimePeriod.HasValue && CustomerProductdata.NoOfMonthsORYears.HasValue && CustomerProductdata.Status == CustomerProductStatus.Pending &&
                        //    (CustomerProductdata.ProductType == ProductType.Recurring_Deposit || CustomerProductdata.ProductType == ProductType.Fixed_Deposit
                        //    || CustomerProductdata.ProductType == ProductType.Regular_Income_Planner || CustomerProductdata.ProductType == ProductType.Monthly_Income_Scheme))
                        //{
                        //}
                        //}
                        // CustomerProductData.InsuranceCommencementDate = CustomerProductdata.InsuranceCommencementDate;
                        //CustomerProductData.Premium = CustomerProductdata.Premium;
                        CustomerProductData.CreatedBy = user.UserId;
                        CustomerProductData.ModifyBy = user.UserId;
                        CustomerProductData.ProductId = objProduct.ProductId;
                        if (CustomerProductdata.NoOfMonthsORYears == 3)
                        {
                            if (i == 0)
                            {
                                CustomerProductData.NoOfMonthsORYears = 1;
                                CustomerProductData.Amount = (CustomerProductdata.Amount * 30) / 100;
                                CustomerProductData.MaturityDate = CustomerProductdata.OpeningDate.AddYears(1);
                                CalculateMaturityAmount calculatematurity = new CalculateMaturityAmount();
                                calculatematurity.Amount = CustomerProductData.Amount;
                                calculatematurity.InterestRate = CustomerProductData.InterestRate;
                                calculatematurity.OpeningDate = CustomerProductData.OpeningDate;
                                calculatematurity.MaturityDate = (DateTime)CustomerProductData.MaturityDate;
                                calculatematurity.InterestType = objProduct.Frequency;
                                CustomerProductData.MaturityAmount = (decimal)CalculateMaturityAmount(calculatematurity);
                                int totalmonthyear = CalculateTotalInstallment(CustomerProductData.TimePeriod.Value, CustomerProductData.PaymentType, CustomerProductData.NoOfMonthsORYears.Value);
                                CustomerProductData.TotalInstallment = totalmonthyear;
                                CustomerProductData.TotalDays = Convert.ToInt32((CustomerProductData.MaturityDate.Value - CustomerProductData.OpeningDate).TotalDays) - 1;

                            }
                            else if (i == 1)
                            {
                                CustomerProductData.NoOfMonthsORYears = 2;
                                CustomerProductData.MaturityDate = CustomerProductData.OpeningDate.AddYears(2);

                                CustomerProductData.Amount = (CustomerProductdata.Amount * 30) / 100;
                                CalculateMaturityAmount calculatematurity = new CalculateMaturityAmount();
                                calculatematurity.Amount = CustomerProductData.Amount;
                                calculatematurity.InterestRate = CustomerProductData.InterestRate;
                                calculatematurity.OpeningDate = CustomerProductData.OpeningDate;
                                calculatematurity.MaturityDate = (DateTime)CustomerProductData.MaturityDate;
                                calculatematurity.InterestType = objProduct.Frequency;
                                CustomerProductData.MaturityAmount = (decimal)CalculateMaturityAmount(calculatematurity);
                                int totalmonthyear = CalculateTotalInstallment(CustomerProductData.TimePeriod.Value, CustomerProductData.PaymentType, CustomerProductData.NoOfMonthsORYears.Value);
                                CustomerProductData.TotalInstallment = totalmonthyear;
                                CustomerProductData.TotalDays = Convert.ToInt32((CustomerProductData.MaturityDate.Value - CustomerProductData.OpeningDate).TotalDays) - 1;

                            }
                            else if (i == 2)
                            {
                                CustomerProductData.NoOfMonthsORYears = 3;
                                CustomerProductData.MaturityDate = CustomerProductdata.OpeningDate.AddYears(3);
                                CustomerProductData.Amount = (CustomerProductdata.Amount * 40) / 100;

                                CalculateMaturityAmount calculatematurity = new CalculateMaturityAmount();
                                calculatematurity.Amount = CustomerProductData.Amount;
                                calculatematurity.InterestRate = CustomerProductData.InterestRate;
                                calculatematurity.OpeningDate = CustomerProductData.OpeningDate;
                                calculatematurity.MaturityDate = (DateTime)CustomerProductData.MaturityDate;
                                calculatematurity.InterestType = objProduct.Frequency;
                                CustomerProductData.MaturityAmount = (decimal)CalculateMaturityAmount(calculatematurity);
                                int totalmonthyear = CalculateTotalInstallment(CustomerProductData.TimePeriod.Value, CustomerProductData.PaymentType, CustomerProductData.NoOfMonthsORYears.Value);
                                CustomerProductData.TotalInstallment = totalmonthyear;
                                CustomerProductData.TotalDays = Convert.ToInt32((CustomerProductData.MaturityDate.Value - CustomerProductData.OpeningDate).TotalDays) - 1;

                            }
                            if (!string.IsNullOrEmpty(objProduct.ProductCode))
                            {
                                Setting setting = db.Setting.Where(a => a.SettingName == CustomerProductData.ProductTypeName + "No").FirstOrDefault();

                                if (setting != null)
                                {
                                    CustomerProductData.AccountNumber = branchcode + objProduct.ProductCode + setting.Value.ToString();

                                    int Prodcutnum = Convert.ToInt32(setting.Value) + 1;
                                    setting.Value = Prodcutnum.ToString().PadLeft(6, '0');
                                    CustomerProductData.Balance = 0;
                                    CustomerProductData.UpdatedBalance = 0;
                                    CustomerProductData.ReferenceCustomerProductId = CustomerProductdata.CustomerProductId;
                                    NewMappedRdAccounts.Add(CustomerProductData);
                                    //db.CustomerProduct.Add(objCustomerProduct);
                                    //db.SaveChanges();
                                }
                            }

                        }

                        if (CustomerProductdata.NoOfMonthsORYears == 5)
                        {
                            if (i == 0)
                            {
                                CustomerProductData.NoOfMonthsORYears = 1;
                                CustomerProductData.Amount = (CustomerProductdata.Amount * 20) / 100;
                                CustomerProductData.MaturityDate = CustomerProductdata.OpeningDate.AddYears(1);
                                CalculateMaturityAmount calculatematurity = new CalculateMaturityAmount();
                                calculatematurity.Amount = CustomerProductData.Amount;
                                calculatematurity.InterestRate = CustomerProductData.InterestRate;
                                calculatematurity.OpeningDate = CustomerProductData.OpeningDate;
                                calculatematurity.MaturityDate = (DateTime)CustomerProductData.MaturityDate;
                                calculatematurity.InterestType = objProduct.Frequency;
                                CustomerProductData.MaturityAmount = (decimal)CalculateMaturityAmount(calculatematurity);
                                int totalmonthyear = CalculateTotalInstallment(CustomerProductData.TimePeriod.Value, CustomerProductData.PaymentType, CustomerProductData.NoOfMonthsORYears.Value);
                                CustomerProductData.TotalInstallment = totalmonthyear;
                                CustomerProductData.TotalDays = Convert.ToInt32((CustomerProductData.MaturityDate.Value - CustomerProductData.OpeningDate).TotalDays) - 1;

                            }
                            else if (i == 1)
                            {
                                CustomerProductData.NoOfMonthsORYears = 2;
                                CustomerProductData.MaturityDate = CustomerProductData.OpeningDate.AddYears(2);

                                CustomerProductData.Amount = (CustomerProductdata.Amount * 20) / 100;
                                CalculateMaturityAmount calculatematurity = new CalculateMaturityAmount();
                                calculatematurity.Amount = CustomerProductData.Amount;
                                calculatematurity.InterestRate = CustomerProductData.InterestRate;
                                calculatematurity.OpeningDate = CustomerProductData.OpeningDate;
                                calculatematurity.MaturityDate = (DateTime)CustomerProductData.MaturityDate;
                                calculatematurity.InterestType = objProduct.Frequency;
                                CustomerProductData.MaturityAmount = (decimal)CalculateMaturityAmount(calculatematurity);
                                int totalmonthyear = CalculateTotalInstallment(CustomerProductData.TimePeriod.Value, CustomerProductData.PaymentType, CustomerProductData.NoOfMonthsORYears.Value);
                                CustomerProductData.TotalInstallment = totalmonthyear;
                                CustomerProductData.TotalDays = Convert.ToInt32((CustomerProductData.MaturityDate.Value - CustomerProductData.OpeningDate).TotalDays) - 1;

                            }
                            else if (i == 2)
                            {
                                CustomerProductData.NoOfMonthsORYears = 3;
                                CustomerProductData.MaturityDate = CustomerProductdata.OpeningDate.AddYears(3);
                                CustomerProductData.Amount = (CustomerProductdata.Amount * 20) / 100;

                                CalculateMaturityAmount calculatematurity = new CalculateMaturityAmount();
                                calculatematurity.Amount = CustomerProductData.Amount;
                                calculatematurity.InterestRate = CustomerProductData.InterestRate;
                                calculatematurity.OpeningDate = CustomerProductData.OpeningDate;
                                calculatematurity.MaturityDate = (DateTime)CustomerProductData.MaturityDate;
                                calculatematurity.InterestType = objProduct.Frequency;
                                CustomerProductData.MaturityAmount = (decimal)CalculateMaturityAmount(calculatematurity);
                                int totalmonthyear = CalculateTotalInstallment(CustomerProductData.TimePeriod.Value, CustomerProductData.PaymentType, CustomerProductData.NoOfMonthsORYears.Value);
                                CustomerProductData.TotalInstallment = totalmonthyear;
                                CustomerProductData.TotalDays = Convert.ToInt32((CustomerProductData.MaturityDate.Value - CustomerProductData.OpeningDate).TotalDays) - 1;

                            }
                            else if (i == 3)
                            {
                                CustomerProductData.NoOfMonthsORYears = 4;
                                CustomerProductData.MaturityDate = CustomerProductdata.OpeningDate.AddYears(4);
                                CustomerProductData.Amount = (CustomerProductdata.Amount * 20) / 100;

                                CalculateMaturityAmount calculatematurity = new CalculateMaturityAmount();
                                calculatematurity.Amount = CustomerProductData.Amount;
                                calculatematurity.InterestRate = CustomerProductData.InterestRate;
                                calculatematurity.OpeningDate = CustomerProductData.OpeningDate;
                                calculatematurity.MaturityDate = (DateTime)CustomerProductData.MaturityDate;
                                calculatematurity.InterestType = objProduct.Frequency;
                                CustomerProductData.MaturityAmount = (decimal)CalculateMaturityAmount(calculatematurity);
                                int totalmonthyear = CalculateTotalInstallment(CustomerProductData.TimePeriod.Value, CustomerProductData.PaymentType, CustomerProductData.NoOfMonthsORYears.Value);
                                CustomerProductData.TotalInstallment = totalmonthyear;
                                CustomerProductData.TotalDays = Convert.ToInt32((CustomerProductData.MaturityDate.Value - CustomerProductData.OpeningDate).TotalDays) - 1;

                            }
                            else if (i == 4)
                            {
                                CustomerProductData.NoOfMonthsORYears = 5;
                                CustomerProductData.MaturityDate = CustomerProductdata.OpeningDate.AddYears(5);
                                CustomerProductData.Amount = (CustomerProductdata.Amount * 20) / 100;

                                CalculateMaturityAmount calculatematurity = new CalculateMaturityAmount();
                                calculatematurity.Amount = CustomerProductData.Amount;
                                calculatematurity.InterestRate = CustomerProductData.InterestRate;
                                calculatematurity.OpeningDate = CustomerProductData.OpeningDate;
                                calculatematurity.MaturityDate = (DateTime)CustomerProductData.MaturityDate;
                                calculatematurity.InterestType = objProduct.Frequency;
                                CustomerProductData.MaturityAmount = (decimal)CalculateMaturityAmount(calculatematurity);
                                int totalmonthyear = CalculateTotalInstallment(CustomerProductData.TimePeriod.Value, CustomerProductData.PaymentType, CustomerProductData.NoOfMonthsORYears.Value);
                                CustomerProductData.TotalInstallment = totalmonthyear;
                                CustomerProductData.TotalDays = Convert.ToInt32((CustomerProductData.MaturityDate.Value - CustomerProductData.OpeningDate).TotalDays) - 1;

                            }

                            if (!string.IsNullOrEmpty(objProduct.ProductCode))
                            {
                                Setting setting = db.Setting.Where(a => a.SettingName == CustomerProductData.ProductTypeName + "No").FirstOrDefault();

                                if (setting != null)
                                {
                                    CustomerProductData.AccountNumber = branchcode + objProduct.ProductCode + setting.Value.ToString();

                                    int Prodcutnum = Convert.ToInt32(setting.Value) + 1;
                                    setting.Value = Prodcutnum.ToString().PadLeft(6, '0');
                                    CustomerProductData.Balance = 0;
                                    CustomerProductData.UpdatedBalance = 0;
                                    CustomerProductData.ReferenceCustomerProductId = CustomerProductdata.CustomerProductId;
                                    NewMappedRdAccounts.Add(CustomerProductData);
                                    //db.CustomerProduct.Add(objCustomerProduct);
                                    //db.SaveChanges();
                                }
                            }

                        }

                    }
                    db.CustomerProduct.AddRange(NewMappedRdAccounts);
                    db.SaveChanges();

                }

                return count;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public object GetAllProductDataByCustomerId(DataTableSearch search)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = db.CustomerProduct.Include("Product").Where(s => s.IsDelete == false && s.CustomerId == search.id).AsQueryable();

                var ProductList = list.OrderBy(c => c.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = ProductList.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = ProductList
                };
                return data;
            }
        }

        public object GetCustomerProductById(Guid ProductId)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {

                    var Productdata = (from c in db.CustomerProduct.Where(a => a.IsDelete == false)
                                       join p in db.User on c.EmployeeId equals p.UserId into tm
                                       from mt in tm.DefaultIfEmpty()
                                       join a in db.User on c.AgentId equals a.UserId
                                        into temp
                                       from b in temp.DefaultIfEmpty()
                                       where c.CustomerProductId == ProductId
                                       select new
                                       {
                                           CustomerProductId = c.CustomerProductId,
                                           CustomerId = c.CustomerId,
                                           ProductId = c.ProductId,
                                           AccountNumber = c.AccountNumber,
                                           Balance = c.Balance,
                                           CreatedBy = c.CreatedBy,
                                           DueDate = c.DueDate,
                                           EmployeeId = c.EmployeeId,
                                           EmpName = mt.FirstName + " " + mt.LastName,
                                           EmpCode = mt.UserCode,
                                           AgentId = c.AgentId,
                                           AgentCode = b.UserCode,
                                           AgentName = b.FirstName + " " + b.LastName,
                                           GICommencementDate = c.GICommencementDate,
                                           GIPremium = c.GIPremium,
                                           GIDueDate = c.GIDueDate,
                                           LICommencementDate = c.LICommencementDate,
                                           LIPremium = c.LIPremium,
                                           LIDueDate = c.LIDueDate,
                                           InsuranceTypeLI = c.InsuranceTypeLI,
                                           LIType = c.LIType,
                                           InsuranceTypeGI = c.InsuranceTypeGI,
                                           InterestRate = c.InterestRate,
                                           LoanTypeId = c.LoanTypeId,
                                           LatePaymentFees = c.LatePaymentFees,
                                           IsActive = c.IsActive,
                                           MaturityDate = c.MaturityDate,
                                           NoOfMonthsORYears = c.NoOfMonthsORYears,
                                           OpeningDate = c.OpeningDate,
                                           PaymentType = c.PaymentType,
                                           BranchId = c.BranchId,
                                           //Premium = c.Premium,
                                           //StartDate = c.StartDate,
                                           TimePeriod = c.TimePeriod,
                                           UpdatedBalance = c.UpdatedBalance,
                                           ProductType = c.ProductType,
                                           Amount = c.Amount,
                                           MaturityAmount = c.MaturityAmount,
                                           Status = c.Status,
                                           CertificateNumber = c.CertificateNumber,
                                           OldAccountNumber = c.OldAccountNumber,
                                           OpeningBalance = c.OpeningBalance,
                                           SkipFirstInstallment = c.SkipFirstInstallment,
                                       }).FirstOrDefault();

                    return Productdata;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public object GetProductPrintData(Guid ProductId)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {

                    var Productdata = (from cpd in db.CustomerProduct.Where(c => c.IsDelete == false && c.CustomerProductId == ProductId)
                                       join b in db.Branch on cpd.BranchId equals b.BranchId
                                       join cp in db.CustomerPersonalDetail.Where(a => a.IsDelete == false) on cpd.CustomerId equals cp.CustomerId
                                       join c in db.CustomerAddress.Where(a => a.IsDelete == false) on cp.PersonalDetailId equals c.PersonalDetailId
                                       join ci in db.Customer.Where(a => a.IsDelete == false) on cpd.CustomerId equals ci.CustomerId
                                       join p in db.Product.Where(a => a.IsDelete == false) on cpd.ProductId equals p.ProductId
                                       join n in db.Nominee on cpd.CustomerId equals n.CustomerId into no
                                       from n in no.DefaultIfEmpty()

                                       select new
                                       {
                                           CustomerProductId = cpd.CustomerProductId,
                                           CustomerId = cpd.CustomerId,
                                           ClientId = ci.ClienId,
                                           AccountNumber = cpd.AccountNumber,
                                           MaturityDate = cpd.MaturityDate,
                                           InterestRate = cpd.InterestRate,
                                           Amount = cpd.Amount,
                                           MaturityAmount = cpd.MaturityAmount != null ? cpd.MaturityAmount : 0,
                                           Nomineename = n.Name,
                                           Relation = n.RelationtoAccountholder,
                                           Openingdate = cpd.OpeningDate,
                                           TimePeriodName = ((TimePeriod)cpd.TimePeriod).ToString(),
                                           TimePeriod = cpd.TimePeriod,
                                           NoofMonthOrYear = cpd.NoOfMonthsORYears,
                                           Address = (!string.IsNullOrEmpty(c.DoorNo) ? c.DoorNo + ", " : "") + (!string.IsNullOrEmpty(c.BuildingName) ? c.BuildingName + ", " : "") + (!string.IsNullOrEmpty(c.PlotNo_Street) ? c.PlotNo_Street + ", " : "") + (!string.IsNullOrEmpty(c.Landmark) ? c.Landmark + ", " : "") + (!string.IsNullOrEmpty(c.Area) ? c.Area + ", " : "") + (!string.IsNullOrEmpty(c.City) ? c.City + ", " : "") + (!string.IsNullOrEmpty(c.District) ? c.District + ", " : "") + (!string.IsNullOrEmpty(c.State) ? c.State + ", " : "") + (!string.IsNullOrEmpty(c.Pincode) ? c.Pincode : ""),
                                           CertificateNo = cpd.CertificateNumber,
                                           ProductType = cpd.ProductType,
                                           ProductName = p.ProductName != "Akshaya Tritiya" ? ((ProductType)cpd.ProductType).ToString().Replace("_", " ") : p.ProductName,
                                           PaymentTypeName = ((Frequency)cpd.PaymentType).ToString(),
                                           PaymentType = cpd.PaymentType,
                                           BranchCode = b.BranchCode,
                                           BranchName = b.BranchName,
                                           DueDate = cpd.DueDate,
                                           IsCertificatePrinted = cpd.IsCertificatePrinted,
                                       }).FirstOrDefault();

                    decimal EMI = 0;

                    if (Productdata.ProductType == ProductType.Monthly_Income_Scheme)
                    {
                        CalculateRIP calculate = new CalculateRIP();
                        //calculate.TimePeriod = Productdata.TimePeriod.Value;
                        //calculate.NoofMonthOrYear = Productdata.NoofMonthOrYear.Value;
                        calculate.OpeningDate = Productdata.Openingdate;
                        calculate.DueDate = Productdata.DueDate.Value;
                        calculate.MaturityDate = Productdata.MaturityDate.Value;
                        calculate.MaturityAmount = Productdata.MaturityAmount.Value;
                        calculate.PaymentType = Productdata.PaymentType;
                        calculate.InterestRate = Productdata.InterestRate;
                        calculate.Amount = Productdata.Amount;

                        List<RIPList> list = MISMaturityCalculation(calculate);

                        EMI = list.FirstOrDefault().Amount;
                    }


                    string Amount = numericToWordService.AmountInWords(Productdata.Amount);
                    var Holdername = string.Join(",", from pd in db.CustomerPersonalDetail.Where(c => c.CustomerId == Productdata.CustomerId) select pd.FirstName + " " + pd.MiddleName + " " + pd.LastName);
                    var data = new
                    {
                        Productdata = Productdata,
                        Holdername = Holdername,
                        Amount = Amount,
                        EMI = EMI
                    };
                    return data;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public object GetRIPPrintData(Guid ProductId)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var Productdata = (from cpd in db.CustomerProduct.Where(c => c.IsDelete == false && c.CustomerProductId == ProductId)
                                       select new RegularIncomeData
                                       {
                                           CustomerId = cpd.CustomerId,
                                           InterestRate = cpd.InterestRate,
                                           Amount = cpd.Amount,
                                           OpeningDate = cpd.OpeningDate,
                                           TimePeriod = ((TimePeriod)cpd.TimePeriod).ToString(),
                                           NoofMonthOrYear = cpd.NoOfMonthsORYears ?? 0,
                                           ProductType = cpd.ProductType,
                                           PaymentType = cpd.PaymentType,
                                           MaturityDate = cpd.MaturityDate.Value,
                                           DueDate = cpd.DueDate,
                                           MaturityAmount = cpd.MaturityAmount.Value,
                                       }).FirstOrDefault();


                    CalculateRIP calculate = new CalculateRIP();
                    calculate.TimePeriod = ((TimePeriod)Enum.Parse(typeof(TimePeriod), Productdata.TimePeriod));
                    calculate.NoofMonthOrYear = Productdata.NoofMonthOrYear;
                    calculate.OpeningDate = Productdata.OpeningDate;
                    calculate.MaturityDate = Productdata.MaturityDate;
                    calculate.MaturityAmount = Productdata.MaturityAmount;
                    calculate.PaymentType = Productdata.PaymentType;
                    calculate.Amount = Productdata.Amount;
                    Productdata.CustomerName = string.Join(",", from pd in db.CustomerPersonalDetail.Where(c => c.CustomerId == Productdata.CustomerId) select pd.FirstName + " " + pd.MiddleName + " " + pd.LastName);
                    if (Productdata.ProductType == ProductType.Regular_Income_Planner)
                    {
                        //Productdata.RIPList = db.Database.SqlQuery<RIPList>("RIPMaturityCalculation @CustomerProductId", new SqlParameter("CustomerProductId", ProductId)).ToList();
                        Productdata.RIPList = RIPMaturityCalculation(calculate);
                    }
                    else
                    {
                        calculate.InterestRate = Productdata.InterestRate;
                        calculate.DueDate = Productdata.DueDate;
                        Productdata.RIPList = MISMaturityCalculation(calculate);
                    }


                    return Productdata;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public List<RIPList> RIPMaturityCalculation(CalculateRIP calculate)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = db.Database.SqlQuery<RIPList>("RIPMaturityCalculation @TimePeriod, @NoOfMonthsORYears, @OpeningDate, @MaturityDate, @MaturityAmount, @PaymentType, @Amount",

                        new SqlParameter("TimePeriod", calculate.TimePeriod),
                        new SqlParameter("NoOfMonthsORYears", calculate.NoofMonthOrYear),
                        new SqlParameter("OpeningDate", calculate.OpeningDate),
                        new SqlParameter("MaturityDate", calculate.MaturityDate),
                        new SqlParameter("MaturityAmount", calculate.MaturityAmount),
                        new SqlParameter("PaymentType", calculate.PaymentType),
                        new SqlParameter("Amount", calculate.Amount)).ToList();

                return list;
            }
        }

        public List<RIPList> MISMaturityCalculation(CalculateRIP calculate)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = db.Database.SqlQuery<RIPList>("MISMaturityCalculation @Amount, @InterestRate, @OpeningDate, @MaturityDate, @InterestType, @DueDate",

                        //new SqlParameter("TimePeriod", calculate.TimePeriod),
                        //new SqlParameter("NoOfMonthsORYears", calculate.NoofMonthOrYear),
                        new SqlParameter("OpeningDate", calculate.OpeningDate),
                        new SqlParameter("MaturityDate", calculate.MaturityDate),
                        new SqlParameter("DueDate", calculate.DueDate),
                        new SqlParameter("InterestType", calculate.PaymentType),
                        new SqlParameter("InterestRate", calculate.InterestRate),
                        new SqlParameter("Amount", calculate.Amount)).ToList();

                return list;
            }
        }

        public object GetAkshyaTrityaPrintData(Guid ProductId)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var Productdata = (from cpd in db.CustomerProduct.Where(c => c.IsDelete == false && c.CustomerProductId == ProductId)
                                       select new RegularIncomeData
                                       {
                                           CustomerId = cpd.CustomerId,
                                           InterestRate = cpd.InterestRate,
                                           Amount = cpd.Amount,
                                           OpeningDate = cpd.OpeningDate,
                                           TimePeriod = ((TimePeriod)cpd.TimePeriod).ToString(),
                                           NoofMonthOrYear = cpd.NoOfMonthsORYears ?? 0,
                                           ProductType = cpd.ProductType,
                                           PaymentType = cpd.PaymentType,
                                           MaturityDate = cpd.MaturityDate.Value,
                                           DueDate = cpd.DueDate,
                                           MaturityAmount = cpd.MaturityAmount.Value,
                                       }).FirstOrDefault();


                    Productdata.CustomerName = string.Join(",", from pd in db.CustomerPersonalDetail.Where(c => c.CustomerId == Productdata.CustomerId) select pd.FirstName + " " + pd.MiddleName + " " + pd.LastName);
                    Product objProduct = db.Product.Where(a => a.ProductName == "Recurring Deposit").FirstOrDefault();
                    if (Productdata.RIPList == null)
                        Productdata.RIPList = new List<RIPList>();
                    for (int i = 0; i < Productdata.NoofMonthOrYear; i++)
                    {
                        if (Productdata.NoofMonthOrYear == 3)
                        {
                            if (i == 0)
                            {
                                for (int j = 1; j < 4; j++)
                                {
                                    CalculateRIP calculate = new CalculateRIP();
                                    calculate.TimePeriod = ((TimePeriod)Enum.Parse(typeof(TimePeriod), Productdata.TimePeriod));
                                    calculate.NoofMonthsandYearforParentRD = Productdata.NoofMonthOrYear;
                                    calculate.OpeningDate = Productdata.OpeningDate;
                                    calculate.MaturityDate = Productdata.OpeningDate.AddYears(1);
                                    calculate.PaymentType = Productdata.PaymentType;
                                    calculate.DueDate = Productdata.DueDate;
                                    calculate.Amount = Productdata.Amount;
                                    calculate.InterestRate = Productdata.InterestRate;
                                    calculate.NoofMonthOrYear = j;
                                    calculate.TotalProductYear = i;
                                    List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                    for (int k = 0; k < RIPList.Count; k++)
                                    {
                                        RIPList[k].Years = "1 Year";
                                        if (j == 1)
                                            RIPList[k].Account = "1st Acc";
                                        if (j == 2)
                                            RIPList[k].Account = "2nd Acc";
                                        if (j == 3)
                                            RIPList[k].Account = "3rd Acc";
                                    }
                                    Productdata.RIPList.AddRange(RIPList);

                                }
                            }
                            if (i == 1)
                            {
                                for (int j = 2; j < 4; j++)
                                {
                                    CalculateRIP calculate = new CalculateRIP();
                                    calculate.TimePeriod = ((TimePeriod)Enum.Parse(typeof(TimePeriod), Productdata.TimePeriod));
                                    calculate.NoofMonthsandYearforParentRD = Productdata.NoofMonthOrYear;
                                    calculate.OpeningDate = Productdata.OpeningDate;
                                    calculate.MaturityDate = Productdata.OpeningDate.AddYears(1);
                                    calculate.PaymentType = Productdata.PaymentType;
                                    calculate.DueDate = Productdata.DueDate;
                                    calculate.Amount = Productdata.Amount;
                                    calculate.InterestRate = Productdata.InterestRate;
                                    calculate.NoofMonthOrYear = j;
                                    calculate.TotalProductYear = i;
                                    List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                    for (int k = 0; k < RIPList.Count; k++)
                                    {
                                        RIPList[k].Years = "2 Year";
                                        if (j == 1)
                                            RIPList[k].Account = "1st Acc";
                                        if (j == 2)
                                            RIPList[k].Account = "2nd Acc";
                                        if (j == 3)
                                            RIPList[k].Account = "3rd Acc";
                                    }

                                    Productdata.RIPList.AddRange(RIPList);

                                }
                            }
                            if (i == 2)
                            {
                                for (int j = 3; j < 4; j++)
                                {
                                    CalculateRIP calculate = new CalculateRIP();
                                    calculate.TimePeriod = ((TimePeriod)Enum.Parse(typeof(TimePeriod), Productdata.TimePeriod));
                                    calculate.NoofMonthsandYearforParentRD = Productdata.NoofMonthOrYear;
                                    calculate.OpeningDate = Productdata.OpeningDate;
                                    calculate.MaturityDate = Productdata.OpeningDate.AddYears(1);
                                    calculate.PaymentType = Productdata.PaymentType;
                                    calculate.DueDate = Productdata.DueDate;
                                    calculate.Amount = Productdata.Amount;
                                    calculate.InterestRate = Productdata.InterestRate;
                                    calculate.NoofMonthOrYear = j;
                                    calculate.TotalProductYear = i;
                                    List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                    for (int k = 0; k < RIPList.Count; k++)
                                    {
                                        RIPList[k].Years = "3 Year";
                                        if (j == 1)
                                            RIPList[k].Account = "1st Acc";
                                        if (j == 2)
                                            RIPList[k].Account = "2nd Acc";
                                        if (j == 3)
                                            RIPList[k].Account = "3rd Acc";
                                    }
                                    Productdata.RIPList.AddRange(RIPList);

                                }
                            }

                        }
                        if (Productdata.NoofMonthOrYear == 5)
                        {
                            if (i == 0)
                            {
                                for (int j = 1; j < 6; j++)
                                {
                                    CalculateRIP calculate = new CalculateRIP();
                                    calculate.TimePeriod = ((TimePeriod)Enum.Parse(typeof(TimePeriod), Productdata.TimePeriod));
                                    calculate.NoofMonthsandYearforParentRD = Productdata.NoofMonthOrYear;
                                    calculate.OpeningDate = Productdata.OpeningDate;
                                    calculate.MaturityDate = Productdata.OpeningDate.AddYears(1);
                                    calculate.PaymentType = Productdata.PaymentType;
                                    calculate.DueDate = Productdata.DueDate;
                                    calculate.Amount = Productdata.Amount;
                                    calculate.InterestRate = Productdata.InterestRate;
                                    calculate.NoofMonthOrYear = j;
                                    calculate.TotalProductYear = i;
                                    List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                    for (int k = 0; k < RIPList.Count; k++)
                                    {
                                        RIPList[k].Years = "1 Year";
                                        if (j == 1)
                                            RIPList[k].Account = "1st Acc";
                                        if (j == 2)
                                            RIPList[k].Account = "2nd Acc";
                                        if (j == 3)
                                            RIPList[k].Account = "3rd Acc";
                                        if (j == 4)
                                            RIPList[k].Account = "4th Acc";
                                        if (j == 5)
                                            RIPList[k].Account = "5th Acc";

                                    }
                                    Productdata.RIPList.AddRange(RIPList);

                                }
                            }
                            if (i == 1)
                            {
                                for (int j = 2; j < 6; j++)
                                {
                                    CalculateRIP calculate = new CalculateRIP();
                                    calculate.TimePeriod = ((TimePeriod)Enum.Parse(typeof(TimePeriod), Productdata.TimePeriod));
                                    calculate.NoofMonthsandYearforParentRD = Productdata.NoofMonthOrYear;
                                    calculate.OpeningDate = Productdata.OpeningDate;
                                    calculate.MaturityDate = Productdata.OpeningDate.AddYears(1);
                                    calculate.PaymentType = Productdata.PaymentType;
                                    calculate.DueDate = Productdata.DueDate;
                                    calculate.Amount = Productdata.Amount;
                                    calculate.InterestRate = Productdata.InterestRate;
                                    calculate.NoofMonthOrYear = j;
                                    calculate.TotalProductYear = i;
                                    List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                    for (int k = 0; k < RIPList.Count; k++)
                                    {
                                        RIPList[k].Years = "2 Year";
                                        if (j == 1)
                                            RIPList[k].Account = "1st Acc";
                                        if (j == 2)
                                            RIPList[k].Account = "2nd Acc";
                                        if (j == 3)
                                            RIPList[k].Account = "3rd Acc";
                                        if (j == 4)
                                            RIPList[k].Account = "4th Acc";
                                        if (j == 5)
                                            RIPList[k].Account = "5th Acc";

                                    }

                                    Productdata.RIPList.AddRange(RIPList);

                                }
                            }
                            if (i == 2)
                            {
                                for (int j = 3; j < 6; j++)
                                {
                                    CalculateRIP calculate = new CalculateRIP();
                                    calculate.TimePeriod = ((TimePeriod)Enum.Parse(typeof(TimePeriod), Productdata.TimePeriod));
                                    calculate.NoofMonthsandYearforParentRD = Productdata.NoofMonthOrYear;
                                    calculate.OpeningDate = Productdata.OpeningDate;
                                    calculate.MaturityDate = Productdata.OpeningDate.AddYears(1);
                                    calculate.PaymentType = Productdata.PaymentType;
                                    calculate.DueDate = Productdata.DueDate;
                                    calculate.Amount = Productdata.Amount;
                                    calculate.InterestRate = Productdata.InterestRate;
                                    calculate.NoofMonthOrYear = j;
                                    calculate.TotalProductYear = i;
                                    List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                    for (int k = 0; k < RIPList.Count; k++)
                                    {
                                        RIPList[k].Years = "3 Year";
                                        if (j == 1)
                                            RIPList[k].Account = "1st Acc";
                                        if (j == 2)
                                            RIPList[k].Account = "2nd Acc";
                                        if (j == 3)
                                            RIPList[k].Account = "3rd Acc";
                                        if (j == 4)
                                            RIPList[k].Account = "4th Acc";
                                        if (j == 5)
                                            RIPList[k].Account = "5th Acc";

                                    }
                                    Productdata.RIPList.AddRange(RIPList);

                                }
                            }
                            if (i == 3)
                            {
                                for (int j = 4; j < 6; j++)
                                {
                                    CalculateRIP calculate = new CalculateRIP();
                                    calculate.TimePeriod = ((TimePeriod)Enum.Parse(typeof(TimePeriod), Productdata.TimePeriod));
                                    calculate.NoofMonthsandYearforParentRD = Productdata.NoofMonthOrYear;
                                    calculate.OpeningDate = Productdata.OpeningDate;
                                    calculate.MaturityDate = Productdata.OpeningDate.AddYears(1);
                                    calculate.PaymentType = Productdata.PaymentType;
                                    calculate.DueDate = Productdata.DueDate;
                                    calculate.Amount = Productdata.Amount;
                                    calculate.InterestRate = Productdata.InterestRate;
                                    calculate.NoofMonthOrYear = j;
                                    calculate.TotalProductYear = i;
                                    List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                    for (int k = 0; k < RIPList.Count; k++)
                                    {
                                        RIPList[k].Years = "4 Year";
                                        if (j == 1)
                                            RIPList[k].Account = "1st Acc";
                                        if (j == 2)
                                            RIPList[k].Account = "2nd Acc";
                                        if (j == 3)
                                            RIPList[k].Account = "3rd Acc";
                                        if (j == 4)
                                            RIPList[k].Account = "4th Acc";
                                        if (j == 5)
                                            RIPList[k].Account = "5th Acc";

                                    }
                                    Productdata.RIPList.AddRange(RIPList);

                                }
                            }
                            if (i == 4)
                            {
                                for (int j = 5; j < 6; j++)
                                {
                                    CalculateRIP calculate = new CalculateRIP();
                                    calculate.TimePeriod = ((TimePeriod)Enum.Parse(typeof(TimePeriod), Productdata.TimePeriod));
                                    calculate.NoofMonthsandYearforParentRD = Productdata.NoofMonthOrYear;
                                    calculate.OpeningDate = Productdata.OpeningDate;
                                    calculate.MaturityDate = Productdata.OpeningDate.AddYears(1);
                                    calculate.PaymentType = Productdata.PaymentType;
                                    calculate.DueDate = Productdata.DueDate;
                                    calculate.Amount = Productdata.Amount;
                                    calculate.InterestRate = Productdata.InterestRate;
                                    calculate.NoofMonthOrYear = j;
                                    calculate.TotalProductYear = i;
                                    List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                    for (int k = 0; k < RIPList.Count; k++)
                                    {
                                        RIPList[k].Years = "5 Year";
                                        if (j == 1)
                                            RIPList[k].Account = "1st Acc";
                                        if (j == 2)
                                            RIPList[k].Account = "2nd Acc";
                                        if (j == 3)
                                            RIPList[k].Account = "3rd Acc";
                                        if (j == 4)
                                            RIPList[k].Account = "4th Acc";
                                        if (j == 5)
                                            RIPList[k].Account = "5th Acc";

                                    }
                                    Productdata.RIPList.AddRange(RIPList);

                                }
                            }

                        }

                    }

                    return Productdata;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public List<RIPList> AkshyaTrityaMaturityCalculation(CalculateRIP calculate)
        {
            using (var db = new BSCCSLEntity())
            {
                if (calculate.PreviousYearsBalance == null)
                    calculate.PreviousYearsBalance = 0;
                var list = db.Database.SqlQuery<RIPList>("AkshyaTrityaMaturityCalculation @Amount, @InterestRate, @OpeningDate, @MaturityDate, @InterestType, @DueDate,@NoofMonthsandYearforParentRD,@TotalProductYear,@NoofMonthandYear,@PreviousYearsBalance",

                        //new SqlParameter("TimePeriod", calculate.TimePeriod),
                        //new SqlParameter("NoOfMonthsORYears", calculate.NoofMonthOrYear),
                        new SqlParameter("OpeningDate", calculate.OpeningDate),
                        new SqlParameter("MaturityDate", calculate.MaturityDate),
                        new SqlParameter("DueDate", calculate.DueDate),
                        new SqlParameter("InterestType", calculate.PaymentType),
                        new SqlParameter("InterestRate", calculate.InterestRate),
                        new SqlParameter("Amount", calculate.Amount),
                        new SqlParameter("NoofMonthsandYearforParentRD", calculate.NoofMonthsandYearforParentRD),
                        new SqlParameter("TotalProductYear", calculate.TotalProductYear),
                        new SqlParameter("NoofMonthandYear", calculate.NoofMonthOrYear),
                        new SqlParameter("PreviousYearsBalance", calculate.PreviousYearsBalance)
                        ).ToList();

                return list;
            }
        }

        public bool SubmitCustomerRDFD(CustomerProduct CustomerProduct)
        {
            using (var db = new BSCCSLEntity())
            {
                TransactionService transactionService = new TransactionService();

                var CustomerProductData = db.CustomerProduct.Where(x => x.CustomerProductId == CustomerProduct.CustomerProductId).FirstOrDefault();

                var SavingAccount = db.CustomerProduct.Where(x => x.CustomerId == CustomerProductData.CustomerId && x.ProductType == ProductType.Saving_Account && x.IsActive == true && x.IsDelete == false).FirstOrDefault();

                if (SavingAccount != null)
                {
                    Guid AkshyaTrityaProductId = Guid.Empty;
                    Product objProduct2 = db.Product.Where(x => x.ProductName.Contains("Akshaya Tritiya") && x.ProductCode == "RD03").FirstOrDefault();
                    if (objProduct2 != null)
                        AkshyaTrityaProductId = objProduct2.ProductId;
                    //if (CustomerProduct.OpeningDate.Date == DateTime.Now.Date)
                    //{

                    //Update fields 
                    CustomerProductData.TotalDays = Convert.ToInt32((CustomerProduct.MaturityDate.Value - CustomerProduct.OpeningDate).TotalDays) - 1;
                    CustomerProductData.LastInstallmentDate = DateTime.Now.Date;

                    if (CustomerProductData.ProductType != ProductType.Fixed_Deposit)
                    {

                        if (CustomerProduct.PaymentType == Frequency.Daily || CustomerProduct.PaymentType == Frequency.Monthly)
                        {
                            CustomerProductData.NextInstallmentDate = CustomerProduct.DueDate;
                        }
                        else if (CustomerProduct.PaymentType == Frequency.Quarterly)
                        {
                            CustomerProductData.NextInstallmentDate = CustomerProduct.DueDate.Value.AddMonths(3);
                        }
                        else if (CustomerProduct.PaymentType == Frequency.Half_Yearly)
                        {
                            CustomerProductData.NextInstallmentDate = CustomerProduct.DueDate.Value.AddMonths(6);
                        }
                        else if (CustomerProduct.PaymentType == Frequency.Yearly)
                        {
                            CustomerProductData.NextInstallmentDate = CustomerProduct.DueDate.Value.AddYears(1);
                        }
                    }
                    CustomerProductData.ModifiedDate = DateTime.Now.Date;
                    CustomerProductData.ProductId = CustomerProduct.ProductId;
                    CustomerProductData.EmployeeId = CustomerProduct.EmployeeId;
                    CustomerProductData.AgentId = CustomerProduct.AgentId;
                    // CustomerProductData.InterestType = CustomerProduct.InterestType;
                    CustomerProductData.PaymentType = CustomerProduct.PaymentType;
                    CustomerProductData.ProductType = CustomerProduct.ProductType;
                    CustomerProductData.AccountNumber = CustomerProduct.AccountNumber;
                    CustomerProductData.InterestRate = CustomerProduct.InterestRate;
                    //CustomerProductData.FrequencyType = CustomerProduct.FrequencyType;
                    CustomerProductData.LatePaymentFees = CustomerProduct.LatePaymentFees;
                    CustomerProductData.IsActive = CustomerProduct.IsActive;
                    CustomerProductData.InsuranceTypeLI = false;
                    CustomerProductData.InsuranceTypeGI = false;
                    CustomerProductData.OpeningDate = CustomerProduct.OpeningDate;
                    CustomerProductData.MaturityDate = CustomerProduct.MaturityDate;

                    CustomerProductData.NoOfMonthsORYears = CustomerProduct.NoOfMonthsORYears;
                    CustomerProductData.TimePeriod = CustomerProduct.TimePeriod;
                    CustomerProductData.DueDate = CustomerProduct.DueDate;
                    CustomerProductData.CertificateNumber = CustomerProduct.CertificateNumber;
                    CustomerProductData.Status = CustomerProductStatus.Approved;
                    CustomerProductData.DueDate = CustomerProduct.DueDate;
                    CustomerProductData.LatePaymentFees = CustomerProduct.LatePaymentFees;
                    CustomerProductData.OldAccountNumber = CustomerProduct.OldAccountNumber;
                    CustomerProductData.SkipFirstInstallment = CustomerProduct.SkipFirstInstallment;

                    int totalmonthyear = CalculateTotalInstallment(CustomerProduct.TimePeriod.Value, CustomerProduct.PaymentType, CustomerProduct.NoOfMonthsORYears.Value);
                    CustomerProductData.TotalInstallment = totalmonthyear;

                    decimal totalyear = 0;

                    totalyear = Convert.ToInt32(CustomerProductData.TotalDays / 365);
                    //if (CustomerProductData.TimePeriod == TimePeriod.Days)
                    //{
                    //    if ((CustomerProductData.NoOfMonthsORYears % 365) == 0)
                    //    {
                    //        totalyear = CustomerProductData.NoOfMonthsORYears / 365 ?? 0;
                    //    }
                    //    else
                    //    {
                    //        totalyear = (CustomerProductData.NoOfMonthsORYears / 365) + 1 ?? 0;
                    //    }
                    //}
                    //else if (CustomerProductData.TimePeriod == TimePeriod.Months)
                    //{
                    //    if ((CustomerProductData.NoOfMonthsORYears % 12) == 0)
                    //    {
                    //        totalyear = CustomerProductData.NoOfMonthsORYears / 12 ?? 0;
                    //    }
                    //    else
                    //    {
                    //        totalyear = (CustomerProductData.NoOfMonthsORYears / 12) + 1 ?? 0;
                    //    }
                    //}
                    //else if (CustomerProductData.TimePeriod == TimePeriod.Years)
                    //{
                    //    totalyear = CustomerProductData.NoOfMonthsORYears ?? 0;
                    //}

                    totalyear = Math.Ceiling(totalyear);

                    if (totalyear == 0)
                    {
                        totalyear = 1;
                    }

                    if (SavingAccount.Balance >= CustomerProduct.Amount && CustomerProduct.OpeningBalance == null && CustomerProduct.OpeningDate.Date == DateTime.Now.Date)
                    {


                        Guid RankId = db.User.Where(s => s.UserId == CustomerProductData.AgentId && (s.IsExecutive == null || s.IsExecutive == false)).Select(s => s.RankId.Value).FirstOrDefault();
                        //Chage by Akhilesh for Apply New Commission to only agent
                        //decimal Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();
                        decimal Comm = 0;
                        Guid AgentId = db.AgentRank.Where(a => a.Rank == "Agent").Select(a => a.RankId).FirstOrDefault();
                        if (AgentId == RankId)
                        {
                            Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1 && s.Version == "2").Select(a => a.Commission).FirstOrDefault();
                            if (Comm == 0)
                                Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();
                        }
                        else
                            Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();

                        if (CustomerProductData.TimePeriod.HasValue && CustomerProductData.NoOfMonthsORYears.HasValue)
                        /*&& (CustomerProductData.ProductType == ProductType.Recurring_Deposit || CustomerProductData.ProductType == ProductType.Regular_Income_Planner)*/
                        {
                            Guid? RdPaymentId = null;

                            if (CustomerProductData.ProductType == ProductType.Recurring_Deposit || CustomerProductData.ProductType == ProductType.Regular_Income_Planner || CustomerProductData.ProductType == ProductType.Three_Year_Product)
                            {
                                RDPayment rdPayment = new RDPayment();
                                rdPayment.CustomerId = CustomerProductData.CustomerId;
                                rdPayment.CustomerProductId = CustomerProductData.CustomerProductId;
                                rdPayment.Amount = CustomerProductData.Amount;
                                rdPayment.IsPaid = true;
                                rdPayment.RDPaymentType = RDPaymentType.Installment;
                                rdPayment.PaidDate = DateTime.Now;
                                if (Comm != null)
                                {
                                    rdPayment.AgentCommission = (CustomerProductData.Amount * Comm) / 100;
                                }
                                db.Entry(rdPayment).State = EntityState.Added;
                                db.SaveChanges();
                                RdPaymentId = rdPayment.RDPaymentId;
                            }

                            ProductAgentCommission productAgentCommission = new ProductAgentCommission();
                            productAgentCommission.RankId = RankId;
                            productAgentCommission.CustomerProductId = CustomerProductData.CustomerProductId;
                            productAgentCommission.ProductId = CustomerProductData.ProductId;
                            productAgentCommission.RDPaymentId = RdPaymentId;
                            productAgentCommission.AgentId = CustomerProductData.AgentId.Value;
                            productAgentCommission.OpeningDate = CustomerProductData.OpeningDate;
                            productAgentCommission.Amount = CustomerProductData.Amount;
                            productAgentCommission.TotalDays = CustomerProductData.TotalDays.Value;
                            productAgentCommission.Date = DateTime.Now.Date;
                            ProductAgentCommission(productAgentCommission);

                        }

                        if (CustomerProductData.TimePeriod.HasValue && CustomerProductData.NoOfMonthsORYears.HasValue &&
                            (CustomerProductData.ProductType == ProductType.Fixed_Deposit || CustomerProductData.ProductType == ProductType.Monthly_Income_Scheme))
                        {
                            CustomerProductData.AgentCommission = (CustomerProductData.Amount * Comm) / 100;
                        }

                        Transactions transaction = new Transactions();
                        transaction.BranchId = CustomerProductData.BranchId;
                        transaction.CustomerId = SavingAccount.CustomerId;
                        transaction.CustomerProductId = SavingAccount.CustomerProductId;
                        transaction.Amount = CustomerProductData.Amount;
                        //transaction.Balance = SavingAccount.UpdatedBalance.Value - CustomerProduct.Amount;
                        transaction.Status = Status.Clear;
                        transaction.TransactionType = TransactionType.BankTransfer;
                        transaction.Type = TypeCRDR.DR;
                        transaction.TransactionTime = DateTime.Now;
                        transaction.CreatedDate = DateTime.Now;
                        transaction.CreatedBy = CustomerProduct.ModifyBy;
                        transaction.RefCustomerProductId = CustomerProduct.CustomerProductId;
                        transaction.DescIndentify = DescIndentify.Maturity;
                        transaction.TransactionId = transactionService.InsertTransctionUsingSP(transaction);

                        Transactions transaction1 = new Transactions();
                        transaction1.BranchId = CustomerProductData.BranchId;
                        transaction1.CustomerId = CustomerProductData.CustomerId;
                        transaction1.CustomerProductId = CustomerProductData.CustomerProductId;
                        transaction1.Amount = CustomerProductData.Amount;
                        //transaction1.Balance = CustomerProductData.Amount;
                        transaction1.Status = Status.Clear;
                        transaction1.TransactionType = TransactionType.BankTransfer;
                        transaction1.Type = TypeCRDR.CR;
                        transaction1.TransactionTime = DateTime.Now;
                        transaction1.CreatedDate = DateTime.Now;
                        transaction1.CreatedBy = CustomerProduct.ModifyBy;
                        transaction1.RefCustomerProductId = SavingAccount.CustomerProductId;
                        transaction1.DescIndentify = DescIndentify.Maturity;
                        transaction1.TransactionId = transactionService.InsertTransctionUsingSP(transaction1);

                        //SavingAccount.Balance = SavingAccount.Balance - CustomerProduct.Amount;
                        //SavingAccount.UpdatedBalance = SavingAccount.UpdatedBalance - CustomerProduct.Amount;

                        db.SaveChanges();
                        DailyInterestCalculation(CustomerProductData.CustomerProductId, DateTime.Now);
                        if (CustomerProduct.ProductId == AkshyaTrityaProductId)
                        {
                            List<CustomerProduct> objMappedRds = db.CustomerProduct.Where(a => a.ReferenceCustomerProductId == CustomerProductData.CustomerProductId && a.IsDelete == false).ToList(); ;

                            foreach (var item in objMappedRds)
                            {
                                submitMappedRD(item);
                            }
                        }


                        return true;
                    }

                    else if (CustomerProduct.OpeningBalance != null && CustomerProduct.OldAccountNumber != null)
                    {
                        if (CustomerProductData.TimePeriod.HasValue && CustomerProductData.NoOfMonthsORYears.HasValue && CustomerProductData.ProductType == ProductType.Recurring_Deposit &&
                            SavingAccount.Balance >= CustomerProduct.Amount && (CustomerProductData.SkipFirstInstallment == null || CustomerProductData.SkipFirstInstallment == false))
                        {
                            Transactions transaction2 = new Transactions();
                            transaction2.BranchId = CustomerProductData.BranchId;
                            transaction2.CustomerId = CustomerProductData.CustomerId;
                            transaction2.CustomerProductId = CustomerProductData.CustomerProductId;
                            transaction2.Amount = CustomerProductData.OpeningBalance.Value;
                            //transaction2.Balance = CustomerProductData.OpeningBalance.Value;
                            transaction2.Status = Status.Clear;
                            transaction2.TransactionType = TransactionType.BankTransfer;
                            transaction2.Type = TypeCRDR.CR;
                            transaction2.TransactionTime = DateTime.Now;
                            transaction2.CreatedDate = DateTime.Now;
                            transaction2.CreatedBy = CustomerProduct.ModifyBy;
                            //transaction2.RefCustomerProductId = SavingAccount.CustomerProductId;
                            transaction2.DescIndentify = DescIndentify.Balance_Transfer;
                            transaction2.TransactionId = transactionService.InsertTransctionUsingSP(transaction2);
                            //db.Entry(transaction1).State = EntityState.Added;
                            //CustomerProductData.Balance = CustomerProduct.Balance;
                            CustomerProductData.OpeningBalance = CustomerProductData.OpeningBalance;
                            //CustomerProductData.Balance = CustomerProductData.OpeningBalance;
                            //CustomerProductData.UpdatedBalance = CustomerProductData.Balance;


                            Guid RankId = db.User.Where(s => s.UserId == CustomerProductData.AgentId && (s.IsExecutive == null || s.IsExecutive == false)).Select(s => s.RankId.Value).FirstOrDefault();
                            //Chage by Akhilesh for Apply New Commission to only agent
                            //decimal Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();
                            decimal Comm = 0;
                            Guid AgentId = db.AgentRank.Where(a => a.Rank == "Agent").Select(a => a.RankId).FirstOrDefault();
                            if (AgentId == RankId)
                            {
                                Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1 && s.Version == "2").Select(a => a.Commission).FirstOrDefault();
                                if (Comm == 0)
                                    Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();
                            }
                            else
                                Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();

                            RDPayment rdPayment = new RDPayment();
                            rdPayment.CustomerId = CustomerProduct.CustomerId;
                            rdPayment.CustomerProductId = CustomerProductData.CustomerProductId;
                            rdPayment.Amount = CustomerProductData.Amount;
                            rdPayment.IsPaid = true;
                            rdPayment.RDPaymentType = RDPaymentType.Installment;
                            rdPayment.PaidDate = DateTime.Now;
                            if (Comm != null)
                            {
                                rdPayment.AgentCommission = (CustomerProduct.Amount * Comm) / 100;
                            }
                            db.Entry(rdPayment).State = EntityState.Added;
                            db.SaveChanges();

                            ProductAgentCommission productAgentCommission = new ProductAgentCommission();
                            productAgentCommission.RankId = RankId;
                            productAgentCommission.CustomerProductId = CustomerProductData.CustomerProductId;
                            productAgentCommission.ProductId = CustomerProductData.ProductId;
                            productAgentCommission.RDPaymentId = rdPayment.RDPaymentId;
                            productAgentCommission.AgentId = CustomerProductData.AgentId.Value;
                            productAgentCommission.OpeningDate = CustomerProductData.OpeningDate;
                            productAgentCommission.Amount = CustomerProductData.Amount;
                            productAgentCommission.TotalDays = CustomerProductData.TotalDays.Value;
                            productAgentCommission.Date = DateTime.Now.Date;
                            ProductAgentCommission(productAgentCommission);

                            //deduct First Installment  from saving
                            Transactions transaction3 = new Transactions();
                            transaction3.BranchId = CustomerProductData.BranchId;
                            transaction3.CustomerId = SavingAccount.CustomerId;
                            transaction3.CustomerProductId = SavingAccount.CustomerProductId;
                            transaction3.Amount = CustomerProductData.Amount;
                            //transaction3.Balance = SavingAccount.UpdatedBalance.Value - CustomerProduct.Amount;
                            transaction3.Status = Status.Clear;
                            transaction3.TransactionType = TransactionType.BankTransfer;
                            transaction3.Type = TypeCRDR.DR;
                            transaction3.TransactionTime = DateTime.Now;
                            transaction3.CreatedDate = DateTime.Now;
                            transaction3.CreatedBy = CustomerProduct.ModifyBy;
                            transaction3.RefCustomerProductId = CustomerProduct.CustomerProductId;
                            transaction3.DescIndentify = DescIndentify.Maturity;
                            transaction3.TransactionId = transactionService.InsertTransctionUsingSP(transaction3);

                            //deduct First Installment in RD
                            Transactions transaction4 = new Transactions();
                            transaction4.BranchId = CustomerProductData.BranchId;
                            transaction4.CustomerId = CustomerProductData.CustomerId;
                            transaction4.CustomerProductId = CustomerProductData.CustomerProductId;
                            transaction4.Amount = CustomerProductData.Amount;
                            //transaction4.Balance = CustomerProductData.UpdatedBalance.Value + CustomerProductData.Amount;
                            transaction4.Status = Status.Clear;
                            transaction4.TransactionType = TransactionType.BankTransfer;
                            transaction4.Type = TypeCRDR.CR;
                            transaction4.TransactionTime = DateTime.Now;
                            transaction4.CreatedDate = DateTime.Now;
                            transaction4.CreatedBy = CustomerProduct.ModifyBy;
                            transaction4.RefCustomerProductId = SavingAccount.CustomerProductId;
                            transaction4.DescIndentify = DescIndentify.Maturity;
                            transaction4.TransactionId = transactionService.InsertTransctionUsingSP(transaction4);

                            db.SaveChanges();

                            //SavingAccount.Balance = SavingAccount.Balance - CustomerProduct.Amount;
                            //SavingAccount.UpdatedBalance = SavingAccount.UpdatedBalance - CustomerProduct.Amount;

                            //CustomerProductData.UpdatedBalance = CustomerProductData.UpdatedBalance + CustomerProductData.Amount;
                            //CustomerProductData.Balance = CustomerProductData.Balance + CustomerProductData.Amount;
                        }
                        else if (CustomerProductData.SkipFirstInstallment == true && CustomerProductData.ProductType == ProductType.Recurring_Deposit)
                        {
                            Transactions transaction2 = new Transactions();
                            transaction2.BranchId = CustomerProductData.BranchId;
                            transaction2.CustomerId = CustomerProductData.CustomerId;
                            transaction2.CustomerProductId = CustomerProductData.CustomerProductId;
                            transaction2.Amount = CustomerProductData.OpeningBalance.Value;
                            //transaction2.Balance = CustomerProductData.OpeningBalance.Value;
                            transaction2.Status = Status.Clear;
                            transaction2.TransactionType = TransactionType.BankTransfer;
                            transaction2.Type = TypeCRDR.CR;
                            transaction2.TransactionTime = DateTime.Now;
                            transaction2.CreatedDate = DateTime.Now;
                            transaction2.CreatedBy = CustomerProduct.ModifyBy;
                            //transaction2.RefCustomerProductId = SavingAccount.CustomerProductId;
                            transaction2.DescIndentify = DescIndentify.Balance_Transfer;
                            transaction2.TransactionId = transactionService.InsertTransctionUsingSP(transaction2);
                            //db.Entry(transaction1).State = EntityState.Added;
                            //CustomerProductData.Balance = CustomerProduct.Balance;
                            CustomerProductData.OpeningBalance = CustomerProductData.OpeningBalance;
                            //CustomerProductData.Balance = CustomerProductData.OpeningBalance;
                            //CustomerProductData.UpdatedBalance = CustomerProductData.Balance;
                            CustomerProductData.TotalInstallment = CustomerProductData.TotalInstallment - 1;
                        }

                        else
                        {
                            return false;
                        }

                        decimal interestAmount = 0;

                        for (DateTime date = CustomerProduct.OpeningDate; date.Date < DateTime.Now.Date; date = date.AddDays(1))
                        {
                            decimal monthlyInterestRate = CustomerProduct.InterestRate / 12;
                            decimal dailyInterestrate = monthlyInterestRate / DateTime.DaysInMonth(date.Year, date.Month);
                            decimal interest = ((CustomerProductData.Balance.Value * dailyInterestrate) / 100);
                            interestAmount = interestAmount + interest;
                        }

                        if (interestAmount > 0)
                        {
                            DailyInterest dailyInterest = new DailyInterest();
                            dailyInterest.CustomerProductId = CustomerProductData.CustomerProductId;
                            dailyInterest.InterestRate = CustomerProductData.InterestRate;
                            dailyInterest.TodaysInterest = interestAmount;
                            dailyInterest.IsPaid = false;
                            dailyInterest.CreatedDate = DateTime.Now;
                            db.Entry(dailyInterest).State = EntityState.Added;
                        }

                        db.SaveChanges();
                        DailyInterestCalculation(CustomerProductData.CustomerProductId, DateTime.Now);
                        if (CustomerProduct.ProductId == AkshyaTrityaProductId)
                        {
                            List<CustomerProduct> objMappedRds = db.CustomerProduct.Where(a => a.ReferenceCustomerProductId == CustomerProductData.CustomerProductId && a.IsDelete == false).ToList(); ;

                            foreach (var item in objMappedRds)
                            {
                                submitMappedRD(item);
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    //}
                    //else
                    //{
                    //    return true;
                    //}
                }
                else
                {
                    return false;
                }


            }
        }

        public void submitMappedRD(CustomerProduct CustomerProduct)
        {
            using (var db = new BSCCSLEntity())
            {

                TransactionService transactionService = new TransactionService();

                var CustomerProductData = db.CustomerProduct.Where(x => x.CustomerProductId == CustomerProduct.CustomerProductId).FirstOrDefault();

                var RefProduct = db.CustomerProduct.Where(x => x.CustomerProductId == CustomerProductData.ReferenceCustomerProductId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();

                if (RefProduct != null)
                {
                    //if (CustomerProduct.OpeningDate.Date == DateTime.Now.Date)
                    //{

                    //Update fields 
                    CustomerProductData.TotalDays = Convert.ToInt32((CustomerProduct.MaturityDate.Value - CustomerProduct.OpeningDate).TotalDays) - 1;
                    CustomerProductData.LastInstallmentDate = DateTime.Now.Date;

                    if (CustomerProductData.ProductType != ProductType.Fixed_Deposit)
                    {

                        if (CustomerProduct.PaymentType == Frequency.Daily || CustomerProduct.PaymentType == Frequency.Monthly)
                        {
                            CustomerProductData.NextInstallmentDate = CustomerProduct.DueDate;
                        }
                        else if (CustomerProduct.PaymentType == Frequency.Quarterly)
                        {
                            CustomerProductData.NextInstallmentDate = CustomerProduct.DueDate.Value.AddMonths(3);
                        }
                        else if (CustomerProduct.PaymentType == Frequency.Half_Yearly)
                        {
                            CustomerProductData.NextInstallmentDate = CustomerProduct.DueDate.Value.AddMonths(6);
                        }
                        else if (CustomerProduct.PaymentType == Frequency.Yearly)
                        {
                            CustomerProductData.NextInstallmentDate = CustomerProduct.DueDate.Value.AddYears(1);
                        }
                    }
                    CustomerProductData.ModifiedDate = DateTime.Now.Date;
                    CustomerProductData.ProductId = CustomerProduct.ProductId;
                    CustomerProductData.EmployeeId = CustomerProduct.EmployeeId;
                    CustomerProductData.AgentId = CustomerProduct.AgentId;
                    // CustomerProductData.InterestType = CustomerProduct.InterestType;
                    CustomerProductData.PaymentType = CustomerProduct.PaymentType;
                    CustomerProductData.ProductType = CustomerProduct.ProductType;
                    CustomerProductData.AccountNumber = CustomerProduct.AccountNumber;
                    CustomerProductData.InterestRate = CustomerProduct.InterestRate;
                    //CustomerProductData.FrequencyType = CustomerProduct.FrequencyType;
                    CustomerProductData.LatePaymentFees = CustomerProduct.LatePaymentFees;
                    CustomerProductData.IsActive = CustomerProduct.IsActive;
                    CustomerProductData.InsuranceTypeLI = false;
                    CustomerProductData.InsuranceTypeGI = false;
                    CustomerProductData.OpeningDate = CustomerProduct.OpeningDate;
                    CustomerProductData.MaturityDate = CustomerProduct.MaturityDate;

                    CustomerProductData.NoOfMonthsORYears = CustomerProduct.NoOfMonthsORYears;
                    CustomerProductData.TimePeriod = CustomerProduct.TimePeriod;
                    CustomerProductData.DueDate = CustomerProduct.DueDate;
                    CustomerProductData.CertificateNumber = CustomerProduct.CertificateNumber;
                    CustomerProductData.Status = CustomerProductStatus.Approved;
                    CustomerProductData.DueDate = CustomerProduct.DueDate;
                    CustomerProductData.LatePaymentFees = CustomerProduct.LatePaymentFees;
                    CustomerProductData.OldAccountNumber = CustomerProduct.OldAccountNumber;
                    CustomerProductData.SkipFirstInstallment = CustomerProduct.SkipFirstInstallment;

                    int totalmonthyear = CalculateTotalInstallment(CustomerProduct.TimePeriod.Value, CustomerProduct.PaymentType, CustomerProduct.NoOfMonthsORYears.Value);
                    CustomerProductData.TotalInstallment = totalmonthyear;

                    decimal totalyear = 0;

                    totalyear = Convert.ToInt32(CustomerProductData.TotalDays / 365);
                    //if (CustomerProductData.TimePeriod == TimePeriod.Days)
                    //{
                    //    if ((CustomerProductData.NoOfMonthsORYears % 365) == 0)
                    //    {
                    //        totalyear = CustomerProductData.NoOfMonthsORYears / 365 ?? 0;
                    //    }
                    //    else
                    //    {
                    //        totalyear = (CustomerProductData.NoOfMonthsORYears / 365) + 1 ?? 0;
                    //    }
                    //}
                    //else if (CustomerProductData.TimePeriod == TimePeriod.Months)
                    //{
                    //    if ((CustomerProductData.NoOfMonthsORYears % 12) == 0)
                    //    {
                    //        totalyear = CustomerProductData.NoOfMonthsORYears / 12 ?? 0;
                    //    }
                    //    else
                    //    {
                    //        totalyear = (CustomerProductData.NoOfMonthsORYears / 12) + 1 ?? 0;
                    //    }
                    //}
                    //else if (CustomerProductData.TimePeriod == TimePeriod.Years)
                    //{
                    //    totalyear = CustomerProductData.NoOfMonthsORYears ?? 0;
                    //}

                    totalyear = Math.Ceiling(totalyear);

                    if (totalyear == 0)
                    {
                        totalyear = 1;
                    }

                    if (CustomerProduct.OpeningDate.Date == DateTime.Now.Date)
                    {


                        Guid RankId = db.User.Where(s => s.UserId == CustomerProductData.AgentId && (s.IsExecutive == null || s.IsExecutive == false)).Select(s => s.RankId.Value).FirstOrDefault();
                        //Chage by Akhilesh for Apply New Commission to only agent
                        //decimal Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();
                        decimal Comm = 0;
                        Guid AgentId = db.AgentRank.Where(a => a.Rank == "Agent").Select(a => a.RankId).FirstOrDefault();
                        if (AgentId == RankId)
                        {
                            Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1 && s.Version == "2").Select(a => a.Commission).FirstOrDefault();
                            if (Comm == 0)
                                Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();
                        }
                        else
                            Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();

                        if (CustomerProductData.TimePeriod.HasValue && CustomerProductData.NoOfMonthsORYears.HasValue)
                        /*&& (CustomerProductData.ProductType == ProductType.Recurring_Deposit || CustomerProductData.ProductType == ProductType.Regular_Income_Planner)*/
                        {
                            Guid? RdPaymentId = null;

                            if (CustomerProductData.ProductType == ProductType.Recurring_Deposit || CustomerProductData.ProductType == ProductType.Regular_Income_Planner || CustomerProductData.ProductType == ProductType.Three_Year_Product)
                            {
                                RDPayment rdPayment = new RDPayment();
                                rdPayment.CustomerId = CustomerProductData.CustomerId;
                                rdPayment.CustomerProductId = CustomerProductData.CustomerProductId;
                                rdPayment.Amount = CustomerProductData.Amount;
                                rdPayment.IsPaid = true;
                                rdPayment.RDPaymentType = RDPaymentType.Installment;
                                rdPayment.PaidDate = DateTime.Now;
                                if (Comm != null)
                                {
                                    rdPayment.AgentCommission = (CustomerProductData.Amount * Comm) / 100;
                                }
                                db.Entry(rdPayment).State = EntityState.Added;
                                db.SaveChanges();
                                RdPaymentId = rdPayment.RDPaymentId;
                            }

                            ProductAgentCommission productAgentCommission = new ProductAgentCommission();
                            productAgentCommission.RankId = RankId;
                            productAgentCommission.CustomerProductId = CustomerProductData.CustomerProductId;
                            productAgentCommission.ProductId = CustomerProductData.ProductId;
                            productAgentCommission.RDPaymentId = RdPaymentId;
                            productAgentCommission.AgentId = CustomerProductData.AgentId.Value;
                            productAgentCommission.OpeningDate = CustomerProductData.OpeningDate;
                            productAgentCommission.Amount = CustomerProductData.Amount;
                            productAgentCommission.TotalDays = CustomerProductData.TotalDays.Value;
                            productAgentCommission.Date = DateTime.Now.Date;
                            ProductAgentCommission(productAgentCommission);

                        }

                        if (CustomerProductData.TimePeriod.HasValue && CustomerProductData.NoOfMonthsORYears.HasValue &&
                            (CustomerProductData.ProductType == ProductType.Fixed_Deposit || CustomerProductData.ProductType == ProductType.Monthly_Income_Scheme))
                        {
                            CustomerProductData.AgentCommission = (CustomerProductData.Amount * Comm) / 100;
                        }

                        Transactions transaction = new Transactions();
                        transaction.BranchId = CustomerProductData.BranchId;
                        transaction.CustomerId = RefProduct.CustomerId;
                        transaction.CustomerProductId = RefProduct.CustomerProductId;
                        transaction.Amount = CustomerProductData.Amount;
                        //transaction.Balance = SavingAccount.UpdatedBalance.Value - CustomerProduct.Amount;
                        transaction.Status = Status.Clear;
                        transaction.TransactionType = TransactionType.BankTransfer;
                        transaction.Type = TypeCRDR.DR;
                        transaction.TransactionTime = DateTime.Now;
                        transaction.CreatedDate = DateTime.Now;
                        transaction.CreatedBy = CustomerProduct.ModifyBy;
                        transaction.RefCustomerProductId = CustomerProduct.CustomerProductId;
                        transaction.DescIndentify = DescIndentify.Maturity;
                        transaction.TransactionId = transactionService.InsertTransctionUsingSP(transaction);

                        Transactions transaction1 = new Transactions();
                        transaction1.BranchId = CustomerProductData.BranchId;
                        transaction1.CustomerId = CustomerProductData.CustomerId;
                        transaction1.CustomerProductId = CustomerProductData.CustomerProductId;
                        transaction1.Amount = CustomerProductData.Amount;
                        //transaction1.Balance = CustomerProductData.Amount;
                        transaction1.Status = Status.Clear;
                        transaction1.TransactionType = TransactionType.BankTransfer;
                        transaction1.Type = TypeCRDR.CR;
                        transaction1.TransactionTime = DateTime.Now;
                        transaction1.CreatedDate = DateTime.Now;
                        transaction1.CreatedBy = CustomerProduct.ModifyBy;
                        transaction1.RefCustomerProductId = RefProduct.CustomerProductId;
                        transaction1.DescIndentify = DescIndentify.Maturity;
                        transaction1.TransactionId = transactionService.InsertTransctionUsingSP(transaction1);

                        //SavingAccount.Balance = SavingAccount.Balance - CustomerProduct.Amount;
                        //SavingAccount.UpdatedBalance = SavingAccount.UpdatedBalance - CustomerProduct.Amount;

                        db.SaveChanges();
                        //DailyInterestCalculation(CustomerProductData.CustomerProductId, DateTime.Now);
                        using (var db1 = new BSCCSLEntity())
                        {
                            //var result = db.Database.SqlQuery<object>("InterestCalculation @Date",
                            //new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

                            string connectionstring = db1.Database.Connection.ConnectionString;
                            SqlConnection sql = new SqlConnection(connectionstring);
                            SqlCommand cmd = new SqlCommand("InterestCalculationByCPId", sql);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 3600;
                            //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                            SqlParameter paramdate = cmd.Parameters.Add("@Date", DateTime.Now);
                            paramdate.Value = DateTime.Now;
                            SqlParameter paramCPID = cmd.Parameters.Add("@CPId", CustomerProductData.CustomerProductId);
                            paramCPID.Value = CustomerProductData.CustomerProductId;

                            //Execute the query
                            sql.Open();
                            //int result = cmdTimesheet.ExecuteNonQuery();
                            var reader = cmd.ExecuteReader();

                            // Read Blogs from the first result set
                            var result = ((IObjectContextAdapter)db).ObjectContext.Translate<object>(reader).FirstOrDefault();
                            sql.Close();
                            db1.Dispose();

                        }

                    }

                    //else if (CustomerProduct.OpeningBalance != null && CustomerProduct.OldAccountNumber != null)
                    //{
                    //    if (CustomerProductData.TimePeriod.HasValue && CustomerProductData.NoOfMonthsORYears.HasValue && CustomerProductData.ProductType == ProductType.Recurring_Deposit &&
                    //        SavingAccount.Balance >= CustomerProduct.Amount && (CustomerProductData.SkipFirstInstallment == null || CustomerProductData.SkipFirstInstallment == false))
                    //    {
                    //        Transactions transaction2 = new Transactions();
                    //        transaction2.BranchId = CustomerProductData.BranchId;
                    //        transaction2.CustomerId = CustomerProductData.CustomerId;
                    //        transaction2.CustomerProductId = CustomerProductData.CustomerProductId;
                    //        transaction2.Amount = CustomerProductData.OpeningBalance.Value;
                    //        //transaction2.Balance = CustomerProductData.OpeningBalance.Value;
                    //        transaction2.Status = Status.Clear;
                    //        transaction2.TransactionType = TransactionType.BankTransfer;
                    //        transaction2.Type = TypeCRDR.CR;
                    //        transaction2.TransactionTime = DateTime.Now;
                    //        transaction2.CreatedDate = DateTime.Now;
                    //        transaction2.CreatedBy = CustomerProduct.ModifyBy;
                    //        //transaction2.RefCustomerProductId = SavingAccount.CustomerProductId;
                    //        transaction2.DescIndentify = DescIndentify.Balance_Transfer;
                    //        transaction2.TransactionId = transactionService.InsertTransctionUsingSP(transaction2);
                    //        //db.Entry(transaction1).State = EntityState.Added;
                    //        //CustomerProductData.Balance = CustomerProduct.Balance;
                    //        CustomerProductData.OpeningBalance = CustomerProductData.OpeningBalance;
                    //        //CustomerProductData.Balance = CustomerProductData.OpeningBalance;
                    //        //CustomerProductData.UpdatedBalance = CustomerProductData.Balance;


                    //        Guid RankId = db.User.Where(s => s.UserId == CustomerProductData.AgentId && (s.IsExecutive == null || s.IsExecutive == false)).Select(s => s.RankId.Value).FirstOrDefault();
                    //        //Chage by Akhilesh for Apply New Commission to only agent
                    //        //decimal Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();
                    //        decimal Comm = 0;
                    //        Guid AgentId = db.AgentRank.Where(a => a.Rank == "Agent").Select(a => a.RankId).FirstOrDefault();
                    //        if (AgentId == RankId)
                    //        {
                    //            Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1 && s.Version == "2").Select(a => a.Commission).FirstOrDefault();
                    //            if (Comm == 0)
                    //                Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();
                    //        }
                    //        else
                    //            Comm = db.ProductCommission.Where(s => s.ProductId == CustomerProductData.ProductId && s.RankId == RankId && s.ProductYear == totalyear && s.CommissionYear == 1).Select(a => a.Commission).FirstOrDefault();

                    //        RDPayment rdPayment = new RDPayment();
                    //        rdPayment.CustomerId = CustomerProduct.CustomerId;
                    //        rdPayment.CustomerProductId = CustomerProductData.CustomerProductId;
                    //        rdPayment.Amount = CustomerProductData.Amount;
                    //        rdPayment.IsPaid = true;
                    //        rdPayment.RDPaymentType = RDPaymentType.Installment;
                    //        rdPayment.PaidDate = DateTime.Now;
                    //        if (Comm != null)
                    //        {
                    //            rdPayment.AgentCommission = (CustomerProduct.Amount * Comm) / 100;
                    //        }
                    //        db.Entry(rdPayment).State = EntityState.Added;
                    //        db.SaveChanges();

                    //        ProductAgentCommission productAgentCommission = new ProductAgentCommission();
                    //        productAgentCommission.RankId = RankId;
                    //        productAgentCommission.CustomerProductId = CustomerProductData.CustomerProductId;
                    //        productAgentCommission.ProductId = CustomerProductData.ProductId;
                    //        productAgentCommission.RDPaymentId = rdPayment.RDPaymentId;
                    //        productAgentCommission.AgentId = CustomerProductData.AgentId.Value;
                    //        productAgentCommission.OpeningDate = CustomerProductData.OpeningDate;
                    //        productAgentCommission.Amount = CustomerProductData.Amount;
                    //        productAgentCommission.TotalDays = CustomerProductData.TotalDays.Value;
                    //        productAgentCommission.Date = DateTime.Now.Date;
                    //        ProductAgentCommission(productAgentCommission);

                    //        //deduct First Installment  from saving
                    //        //Transactions transaction3 = new Transactions();
                    //        //transaction3.BranchId = CustomerProductData.BranchId;
                    //        //transaction3.CustomerId = SavingAccount.CustomerId;
                    //        //transaction3.CustomerProductId = SavingAccount.CustomerProductId;
                    //        //transaction3.Amount = CustomerProductData.Amount;
                    //        ////transaction3.Balance = SavingAccount.UpdatedBalance.Value - CustomerProduct.Amount;
                    //        //transaction3.Status = Status.Clear;
                    //        //transaction3.TransactionType = TransactionType.BankTransfer;
                    //        //transaction3.Type = TypeCRDR.DR;
                    //        //transaction3.TransactionTime = DateTime.Now;
                    //        //transaction3.CreatedDate = DateTime.Now;
                    //        //transaction3.CreatedBy = CustomerProduct.ModifyBy;
                    //        //transaction3.RefCustomerProductId = CustomerProduct.CustomerProductId;
                    //        //transaction3.DescIndentify = DescIndentify.Maturity;
                    //        //transaction3.TransactionId = transactionService.InsertTransctionUsingSP(transaction3);

                    //        //deduct First Installment in RD
                    //        Transactions transaction4 = new Transactions();
                    //        transaction4.BranchId = CustomerProductData.BranchId;
                    //        transaction4.CustomerId = CustomerProductData.CustomerId;
                    //        transaction4.CustomerProductId = CustomerProductData.CustomerProductId;
                    //        transaction4.Amount = CustomerProductData.Amount;
                    //        //transaction4.Balance = CustomerProductData.UpdatedBalance.Value + CustomerProductData.Amount;
                    //        transaction4.Status = Status.Clear;
                    //        transaction4.TransactionType = TransactionType.BankTransfer;
                    //        transaction4.Type = TypeCRDR.CR;
                    //        transaction4.TransactionTime = DateTime.Now;
                    //        transaction4.CreatedDate = DateTime.Now;
                    //        transaction4.CreatedBy = CustomerProduct.ModifyBy;
                    //        transaction4.RefCustomerProductId = SavingAccount.CustomerProductId;
                    //        transaction4.DescIndentify = DescIndentify.Maturity;
                    //        transaction4.TransactionId = transactionService.InsertTransctionUsingSP(transaction4);

                    //        db.SaveChanges();

                    //        //SavingAccount.Balance = SavingAccount.Balance - CustomerProduct.Amount;
                    //        //SavingAccount.UpdatedBalance = SavingAccount.UpdatedBalance - CustomerProduct.Amount;

                    //        //CustomerProductData.UpdatedBalance = CustomerProductData.UpdatedBalance + CustomerProductData.Amount;
                    //        //CustomerProductData.Balance = CustomerProductData.Balance + CustomerProductData.Amount;
                    //    }
                    //    else if (CustomerProductData.SkipFirstInstallment == true && CustomerProductData.ProductType == ProductType.Recurring_Deposit)
                    //    {
                    //        Transactions transaction2 = new Transactions();
                    //        transaction2.BranchId = CustomerProductData.BranchId;
                    //        transaction2.CustomerId = CustomerProductData.CustomerId;
                    //        transaction2.CustomerProductId = CustomerProductData.CustomerProductId;
                    //        transaction2.Amount = CustomerProductData.OpeningBalance.Value;
                    //        //transaction2.Balance = CustomerProductData.OpeningBalance.Value;
                    //        transaction2.Status = Status.Clear;
                    //        transaction2.TransactionType = TransactionType.BankTransfer;
                    //        transaction2.Type = TypeCRDR.CR;
                    //        transaction2.TransactionTime = DateTime.Now;
                    //        transaction2.CreatedDate = DateTime.Now;
                    //        transaction2.CreatedBy = CustomerProduct.ModifyBy;
                    //        //transaction2.RefCustomerProductId = SavingAccount.CustomerProductId;
                    //        transaction2.DescIndentify = DescIndentify.Balance_Transfer;
                    //        transaction2.TransactionId = transactionService.InsertTransctionUsingSP(transaction2);
                    //        //db.Entry(transaction1).State = EntityState.Added;
                    //        //CustomerProductData.Balance = CustomerProduct.Balance;
                    //        CustomerProductData.OpeningBalance = CustomerProductData.OpeningBalance;
                    //        //CustomerProductData.Balance = CustomerProductData.OpeningBalance;
                    //        //CustomerProductData.UpdatedBalance = CustomerProductData.Balance;
                    //        CustomerProductData.TotalInstallment = CustomerProductData.TotalInstallment - 1;
                    //    }

                    //    else
                    //    {
                    //        return false;
                    //    }

                    //    decimal interestAmount = 0;

                    //    for (DateTime date = CustomerProduct.OpeningDate; date.Date < DateTime.Now.Date; date = date.AddDays(1))
                    //    {
                    //        decimal monthlyInterestRate = CustomerProduct.InterestRate / 12;
                    //        decimal dailyInterestrate = monthlyInterestRate / DateTime.DaysInMonth(date.Year, date.Month);
                    //        decimal interest = ((CustomerProductData.Balance.Value * dailyInterestrate) / 100);
                    //        interestAmount = interestAmount + interest;
                    //    }

                    //    if (interestAmount > 0)
                    //    {
                    //        DailyInterest dailyInterest = new DailyInterest();
                    //        dailyInterest.CustomerProductId = CustomerProductData.CustomerProductId;
                    //        dailyInterest.InterestRate = CustomerProductData.InterestRate;
                    //        dailyInterest.TodaysInterest = interestAmount;
                    //        dailyInterest.IsPaid = false;
                    //        dailyInterest.CreatedDate = DateTime.Now;
                    //        db.Entry(dailyInterest).State = EntityState.Added;
                    //    }

                    //    db.SaveChanges();
                    //    DailyInterestCalculation(CustomerProductData.CustomerProductId, DateTime.Now);

                    //    return true;
                    //}
                    else
                    {
                    }
                    //}
                    //else
                    //{
                    //    return true;
                    //}
                }
                else
                {
                }
            }
        }


        public object GetCustomerByIdForProduct(Guid id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var customerdetail = (from c in db.Customer.Where(x => x.CustomerId == id && x.IsDelete == false)
                                      join cp in db.CustomerPersonalDetail.Where(a => a.IsDelete == false) on c.CustomerId equals cp.CustomerId
                                      join ca in db.CustomerAddress.Where(a => a.IsDelete == false) on cp.PersonalDetailId equals ca.PersonalDetailId
                                      join b in db.Branch on c.BranchId equals b.BranchId
                                      select new
                                      {
                                          Sex = cp.Sex,
                                          DOB = cp.DOB,
                                          Address = (!string.IsNullOrEmpty(ca.DoorNo) ? ca.DoorNo + ", " : "") + (!string.IsNullOrEmpty(ca.BuildingName) ? ca.BuildingName + ", " : "") + (!string.IsNullOrEmpty(ca.PlotNo_Street) ? ca.PlotNo_Street + ", " : "") + (!string.IsNullOrEmpty(ca.Landmark) ? ca.Landmark + ", " : "") + (!string.IsNullOrEmpty(ca.Area) ? ca.Area + ", " : "") + (!string.IsNullOrEmpty(ca.City) ? ca.City + ", " : "") + (!string.IsNullOrEmpty(ca.District) ? ca.District + ", " : "") + (!string.IsNullOrEmpty(ca.State) ? ca.State + ", " : "") + (!string.IsNullOrEmpty(ca.Pincode) ? ca.Pincode : ""),
                                          Mobile = ca.MobileNo,
                                          BranchId = c.BranchId,
                                          Email = ca.Email,
                                          CustomerName = cp.FirstName + " " + cp.MiddleName + " " + cp.LastName,
                                          HolderPhoto = cp.HolderPhotograph,
                                          ClienId = c.ClienId
                                      }).ToList();

                var customer = (from c in db.Customer.Where(x => x.CustomerId == id && x.IsDelete == false)
                                join e in db.User on c.EmployeeId equals e.UserId
                                join a in db.User on c.AgentId equals a.UserId into ag
                                from agent in ag.DefaultIfEmpty()
                                select new
                                {
                                    c.CustomerId,
                                    c.AgentId,
                                    c.EmployeeId,
                                    EmpCode = e.UserCode,
                                    EmpName = e.FirstName + " " + e.LastName,
                                    AgentCode = agent.UserCode,
                                    AgentName = agent != null ? agent.FirstName + " " + agent.LastName : string.Empty
                                }).FirstOrDefault();



                var data = new
                {
                    CustomerDetails = customerdetail,
                    Customer = customer
                };

                return data;
            }
        }

        public object GetInterestRate(int? Id1, TimePeriod? Id2)
        {
            using (var db = new BSCCSLEntity())
            {
                var interestrate = "";
                if (Id2 == TimePeriod.Months || Id2 == TimePeriod.Years)
                {
                    interestrate = db.Term.Where(c => c.TotalFrom <= Id1 && c.TotalTo >= Id1 && (c.TimePeriod == Id2)).Select(c => c.InterestRate).FirstOrDefault().ToString();
                    return interestrate;
                }
                else
                {
                    interestrate = db.Term.Where(c => c.From <= Id1 && c.To >= Id1 && (c.TimePeriod == Id2)).Select(c => c.InterestRate).FirstOrDefault().ToString();
                    return interestrate;
                }
            }
        }

        public object CalculateMaturityAmount(CalculateMaturityAmount calculatematurity)
        {
            using (var db = new BSCCSLEntity())
            {
                decimal amount = 0;

                if (calculatematurity.ProductType == ProductType.Fixed_Deposit || calculatematurity.ProductType == ProductType.Regular_Income_Planner)
                {
                    amount = db.Database.SqlQuery<decimal>("FDMaturityCalculation @Amount, @InterestRate, @OpeningDate, @MaturityDate, @InterestType",
                   new SqlParameter("Amount", calculatematurity.Amount),
                   new SqlParameter("InterestRate", calculatematurity.InterestRate),
                   new SqlParameter("OpeningDate", calculatematurity.OpeningDate),
                   new SqlParameter("MaturityDate", calculatematurity.MaturityDate),
                   new SqlParameter("InterestType", calculatematurity.InterestType)).FirstOrDefault();
                }
                else if (calculatematurity.ProductType == ProductType.Recurring_Deposit || calculatematurity.ProductType == ProductType.Three_Year_Product)
                {
                    if (calculatematurity.ProductName != "Akshaya Tritiya")
                        amount = CalculateRDMaturity(calculatematurity);
                    else
                    {
                        //Product objProduct = db.Product.Where(a => a.ProductName == "Recurring Deposit").FirstOrDefault();
                        //List<CustomerProduct> NewMappedRdAccounts = new List<CustomerProduct>();
                        //for (int i = 0; i < calculatematurity.NoOfMonthsORYears; i++)
                        //{

                        //    if (calculatematurity.NoOfMonthsORYears == 3)
                        //    {
                        //        if (i == 0)
                        //        {
                        //            int NoOfMonthsORYears = 1;
                        //            decimal Amount = (calculatematurity.Amount * 30) / 100;
                        //            DateTime MaturityDate = calculatematurity.OpeningDate.AddYears(1);
                        //            CalculateMaturityAmount calculatematurity1 = new CalculateMaturityAmount();
                        //            calculatematurity1.Amount = Amount;
                        //            calculatematurity1.InterestRate = calculatematurity.InterestRate;
                        //            calculatematurity1.OpeningDate = calculatematurity.OpeningDate;
                        //            calculatematurity1.MaturityDate = (DateTime)MaturityDate;
                        //            calculatematurity1.DueDate = calculatematurity.DueDate;
                        //            calculatematurity1.InterestType = objProduct.Frequency;
                        //            calculatematurity1.TimePeriod = calculatematurity.TimePeriod;
                        //            calculatematurity1.PaymentType = calculatematurity.PaymentType;
                        //            calculatematurity1.NoOfMonthsORYears = NoOfMonthsORYears;
                        //            amount = amount + CalculateAkshtaTrityaMaturity(calculatematurity1);




                        //        }
                        //        else if (i == 1)
                        //        {
                        //            int NoOfMonthsORYears = 2;
                        //            decimal Amount = (calculatematurity.Amount * 30) / 100;
                        //            DateTime MaturityDate = calculatematurity.OpeningDate.AddYears(2);
                        //            CalculateMaturityAmount calculatematurity1 = new CalculateMaturityAmount();
                        //            calculatematurity1.Amount = Amount;
                        //            calculatematurity1.InterestRate = calculatematurity.InterestRate;
                        //            calculatematurity1.OpeningDate = calculatematurity.OpeningDate;
                        //            calculatematurity1.MaturityDate = (DateTime)MaturityDate;
                        //            calculatematurity1.DueDate = calculatematurity.DueDate;
                        //            calculatematurity1.InterestType = objProduct.Frequency;
                        //            calculatematurity1.TimePeriod = calculatematurity.TimePeriod;
                        //            calculatematurity1.PaymentType = calculatematurity.PaymentType;
                        //            calculatematurity1.NoOfMonthsORYears = NoOfMonthsORYears;
                        //            amount = amount + CalculateAkshtaTrityaMaturity(calculatematurity1);

                        //        }
                        //        else if (i == 2)
                        //        {
                        //            int NoOfMonthsORYears = 3;
                        //            decimal Amount = (calculatematurity.Amount * 40) / 100;
                        //            DateTime MaturityDate = calculatematurity.OpeningDate.AddYears(3);
                        //            CalculateMaturityAmount calculatematurity1 = new CalculateMaturityAmount();
                        //            calculatematurity1.Amount = Amount;
                        //            calculatematurity1.InterestRate = calculatematurity.InterestRate;
                        //            calculatematurity1.OpeningDate = calculatematurity.OpeningDate;
                        //            calculatematurity1.MaturityDate = (DateTime)MaturityDate;
                        //            calculatematurity1.DueDate = calculatematurity.DueDate;
                        //            calculatematurity1.InterestType = objProduct.Frequency;
                        //            calculatematurity1.TimePeriod = calculatematurity.TimePeriod;
                        //            calculatematurity1.PaymentType = calculatematurity.PaymentType;
                        //            calculatematurity1.NoOfMonthsORYears = NoOfMonthsORYears;
                        //            amount = amount + CalculateAkshtaTrityaMaturity(calculatematurity1);

                        //        }

                        //    }

                        //    if (calculatematurity.NoOfMonthsORYears == 5)
                        //    {
                        //        if (i == 0)
                        //        {
                        //            int NoOfMonthsORYears = 1;
                        //            decimal Amount = (calculatematurity.Amount * 20) / 100;
                        //            DateTime MaturityDate = calculatematurity.OpeningDate.AddYears(1);
                        //            CalculateMaturityAmount calculatematurity1 = new CalculateMaturityAmount();
                        //            calculatematurity1.Amount = Amount;
                        //            calculatematurity1.InterestRate = calculatematurity.InterestRate;
                        //            calculatematurity1.OpeningDate = calculatematurity.OpeningDate;
                        //            calculatematurity1.MaturityDate = (DateTime)MaturityDate;
                        //            calculatematurity1.DueDate = calculatematurity.DueDate;
                        //            calculatematurity1.InterestType = objProduct.Frequency;
                        //            calculatematurity1.TimePeriod = calculatematurity.TimePeriod;
                        //            calculatematurity1.PaymentType = calculatematurity.PaymentType;
                        //            calculatematurity1.NoOfMonthsORYears = NoOfMonthsORYears;
                        //            amount = amount + CalculateAkshtaTrityaMaturity(calculatematurity1);
                        //        }
                        //        else if (i == 1)
                        //        {
                        //            int NoOfMonthsORYears = 2;
                        //            decimal Amount = (calculatematurity.Amount * 20) / 100;
                        //            DateTime MaturityDate = calculatematurity.OpeningDate.AddYears(2);
                        //            CalculateMaturityAmount calculatematurity1 = new CalculateMaturityAmount();
                        //            calculatematurity1.Amount = Amount;
                        //            calculatematurity1.InterestRate = calculatematurity.InterestRate;
                        //            calculatematurity1.OpeningDate = calculatematurity.OpeningDate;
                        //            calculatematurity1.MaturityDate = (DateTime)MaturityDate;
                        //            calculatematurity1.DueDate = calculatematurity.DueDate;
                        //            calculatematurity1.InterestType = objProduct.Frequency;
                        //            calculatematurity1.TimePeriod = calculatematurity.TimePeriod;
                        //            calculatematurity1.PaymentType = calculatematurity.PaymentType;
                        //            calculatematurity1.NoOfMonthsORYears = NoOfMonthsORYears;
                        //            amount = amount + CalculateAkshtaTrityaMaturity(calculatematurity1);

                        //        }
                        //        else if (i == 2)
                        //        {
                        //            int NoOfMonthsORYears = 3;
                        //            decimal Amount = (calculatematurity.Amount * 20) / 100;
                        //            DateTime MaturityDate = calculatematurity.OpeningDate.AddYears(3);
                        //            CalculateMaturityAmount calculatematurity1 = new CalculateMaturityAmount();
                        //            calculatematurity1.Amount = Amount;
                        //            calculatematurity1.InterestRate = calculatematurity.InterestRate;
                        //            calculatematurity1.OpeningDate = calculatematurity.OpeningDate;
                        //            calculatematurity1.MaturityDate = (DateTime)MaturityDate;
                        //            calculatematurity1.DueDate = calculatematurity.DueDate;
                        //            calculatematurity1.InterestType = objProduct.Frequency;
                        //            calculatematurity1.TimePeriod = calculatematurity.TimePeriod;
                        //            calculatematurity1.PaymentType = calculatematurity.PaymentType;
                        //            calculatematurity1.NoOfMonthsORYears = NoOfMonthsORYears;
                        //            amount = amount + CalculateAkshtaTrityaMaturity(calculatematurity1);

                        //        }
                        //        else if (i == 3)
                        //        {
                        //            int NoOfMonthsORYears = 4;
                        //            decimal Amount = (calculatematurity.Amount * 20) / 100;
                        //            DateTime MaturityDate = calculatematurity.OpeningDate.AddYears(4);
                        //            CalculateMaturityAmount calculatematurity1 = new CalculateMaturityAmount();
                        //            calculatematurity1.Amount = Amount;
                        //            calculatematurity1.InterestRate = calculatematurity.InterestRate;
                        //            calculatematurity1.OpeningDate = calculatematurity.OpeningDate;
                        //            calculatematurity1.MaturityDate = (DateTime)MaturityDate;
                        //            calculatematurity1.DueDate = calculatematurity.DueDate;
                        //            calculatematurity1.InterestType = objProduct.Frequency;
                        //            calculatematurity1.TimePeriod = calculatematurity.TimePeriod;
                        //            calculatematurity1.PaymentType = calculatematurity.PaymentType;
                        //            calculatematurity1.NoOfMonthsORYears = NoOfMonthsORYears;
                        //            amount = amount + CalculateAkshtaTrityaMaturity(calculatematurity1);

                        //        }
                        //        else if (i == 4)
                        //        {
                        //            int NoOfMonthsORYears = 5;
                        //            decimal Amount = (calculatematurity.Amount * 20) / 100;
                        //            DateTime MaturityDate = calculatematurity.OpeningDate.AddYears(5);
                        //            CalculateMaturityAmount calculatematurity1 = new CalculateMaturityAmount();
                        //            calculatematurity1.Amount = Amount;
                        //            calculatematurity1.InterestRate = calculatematurity.InterestRate;
                        //            calculatematurity1.OpeningDate = calculatematurity.OpeningDate;
                        //            calculatematurity1.MaturityDate = (DateTime)MaturityDate;
                        //            calculatematurity1.DueDate = calculatematurity.DueDate;
                        //            calculatematurity1.InterestType = objProduct.Frequency;
                        //            calculatematurity1.TimePeriod = calculatematurity.TimePeriod;
                        //            calculatematurity1.PaymentType = calculatematurity.PaymentType;
                        //            calculatematurity1.NoOfMonthsORYears = NoOfMonthsORYears;
                        //            amount = amount + CalculateAkshtaTrityaMaturity(calculatematurity1);

                        //        }


                        //    }

                        //}
                        List<RIPList> MainList = new List<RIPList>();
                        for (int i = 0; i < calculatematurity.NoOfMonthsORYears; i++)
                        {
                            if (calculatematurity.NoOfMonthsORYears == 3)
                            {
                                if (i == 0)
                                {
                                    for (int j = 1; j < 4; j++)
                                    {
                                        CalculateRIP calculate = new CalculateRIP();
                                        calculate.TimePeriod = TimePeriod.Years;
                                        calculate.NoofMonthsandYearforParentRD = calculatematurity.NoOfMonthsORYears;
                                        calculate.OpeningDate = calculatematurity.OpeningDate;
                                        calculate.MaturityDate = calculatematurity.OpeningDate.AddYears(1);
                                        calculate.PaymentType = (Frequency)calculatematurity.PaymentType;
                                        calculate.DueDate = calculatematurity.DueDate;
                                        calculate.Amount = calculatematurity.Amount;
                                        calculate.InterestRate = calculatematurity.InterestRate;
                                        calculate.NoofMonthOrYear = j;
                                        calculate.TotalProductYear = i;
                                        List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                        for (int k = 0; k < RIPList.Count; k++)
                                        {
                                            RIPList[k].Years = "1 Year";
                                            if (j == 1)
                                                RIPList[k].Account = "1st Acc";
                                            if (j == 2)
                                                RIPList[k].Account = "2nd Acc";
                                            if (j == 3)
                                                RIPList[k].Account = "3rd Acc";
                                        }

                                        MainList.AddRange(RIPList);

                                    }
                                }
                                if (i == 1)
                                {
                                    for (int j = 2; j < 4; j++)
                                    {
                                        CalculateRIP calculate = new CalculateRIP();
                                        calculate.TimePeriod = TimePeriod.Years;
                                        calculate.NoofMonthsandYearforParentRD = calculatematurity.NoOfMonthsORYears;
                                        calculate.OpeningDate = calculatematurity.OpeningDate;
                                        calculate.MaturityDate = calculatematurity.OpeningDate.AddYears(1);
                                        calculate.PaymentType = (Frequency)calculatematurity.PaymentType;
                                        calculate.DueDate = calculatematurity.DueDate;
                                        calculate.Amount = calculatematurity.Amount;
                                        calculate.InterestRate = calculatematurity.InterestRate;
                                        calculate.NoofMonthOrYear = j;
                                        calculate.TotalProductYear = i;
                                        if (j == 2)
                                            calculate.PreviousYearsBalance = MainList.Where(a=>a.Years == "1 Year" && a.Account == "2nd Acc").Select(a=>a.MaturityAmount).FirstOrDefault();
                                        if (j == 3)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "1 Year" && a.Account == "3rd Acc").Select(a => a.MaturityAmount).FirstOrDefault();

                                        List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                        for (int k = 0; k < RIPList.Count; k++)
                                        {
                                            RIPList[k].Years = "2 Year";
                                            if (j == 1)
                                                RIPList[k].Account = "1st Acc";
                                            if (j == 2)
                                                RIPList[k].Account = "2nd Acc";
                                            if (j == 3)
                                                RIPList[k].Account = "3rd Acc";
                                        }

                                        MainList.AddRange(RIPList);

                                    }
                                }
                                if (i == 2)
                                {
                                    for (int j = 3; j < 4; j++)
                                    {
                                        CalculateRIP calculate = new CalculateRIP();
                                        calculate.TimePeriod = TimePeriod.Years;
                                        calculate.NoofMonthsandYearforParentRD = calculatematurity.NoOfMonthsORYears;
                                        calculate.OpeningDate = calculatematurity.OpeningDate;
                                        calculate.MaturityDate = calculatematurity.OpeningDate.AddYears(1);
                                        calculate.PaymentType = (Frequency)calculatematurity.PaymentType;
                                        calculate.DueDate = calculatematurity.DueDate;
                                        calculate.Amount = calculatematurity.Amount;
                                        calculate.InterestRate = calculatematurity.InterestRate;
                                        calculate.NoofMonthOrYear = j;
                                        calculate.TotalProductYear = i;
                                        if (j == 3)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "2 Year" && a.Account == "3rd Acc").Select(a => a.MaturityAmount).FirstOrDefault();

                                        List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                        for (int k = 0; k < RIPList.Count; k++)
                                        {
                                            RIPList[k].Years = "3 Year";
                                            if (j == 1)
                                                RIPList[k].Account = "1st Acc";
                                            if (j == 2)
                                                RIPList[k].Account = "2nd Acc";
                                            if (j == 3)
                                                RIPList[k].Account = "3rd Acc";
                                        }

                                        MainList.AddRange(RIPList);

                                    }
                                }
                                decimal FirstAcc = MainList.Where(a => a.Years == "1 Year" && a.Account == "1st Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                decimal SecondAcc = MainList.Where(a => a.Years == "2 Year" && a.Account == "2nd Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                decimal ThirdAcc = MainList.Where(a => a.Years == "3 Year" && a.Account == "3rd Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                amount = FirstAcc + SecondAcc + ThirdAcc;

                            }
                            if (calculatematurity.NoOfMonthsORYears == 5)
                            {
                                if (i == 0)
                                {
                                    for (int j = 1; j < 6; j++)
                                    {
                                        CalculateRIP calculate = new CalculateRIP();
                                        calculate.TimePeriod = TimePeriod.Years;
                                        calculate.NoofMonthsandYearforParentRD = calculatematurity.NoOfMonthsORYears;
                                        calculate.OpeningDate = calculatematurity.OpeningDate;
                                        calculate.MaturityDate = calculatematurity.OpeningDate.AddYears(1);
                                        calculate.PaymentType =(Frequency) calculatematurity.PaymentType;
                                        calculate.DueDate = calculatematurity.DueDate;
                                        calculate.Amount = calculatematurity.Amount;
                                        calculate.InterestRate = calculatematurity.InterestRate;
                                        calculate.NoofMonthOrYear = j;
                                        calculate.TotalProductYear = i;
                                        List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                        for (int k = 0; k < RIPList.Count; k++)
                                        {
                                            RIPList[k].Years = "1 Year";
                                            if (j == 1)
                                                RIPList[k].Account = "1st Acc";
                                            if (j == 2)
                                                RIPList[k].Account = "2nd Acc";
                                            if (j == 3)
                                                RIPList[k].Account = "3rd Acc";
                                            if (j == 4)
                                                RIPList[k].Account = "4th Acc";
                                            if (j == 5)
                                                RIPList[k].Account = "5th Acc";

                                        }
                                        MainList.AddRange(RIPList);

                                    }
                                }
                                if (i == 1)
                                {
                                    for (int j = 2; j < 6; j++)
                                    {
                                        CalculateRIP calculate = new CalculateRIP();
                                        calculate.TimePeriod = TimePeriod.Years;
                                        calculate.NoofMonthsandYearforParentRD = calculatematurity.NoOfMonthsORYears;
                                        calculate.OpeningDate = calculatematurity.OpeningDate;
                                        calculate.MaturityDate = calculatematurity.OpeningDate.AddYears(1);
                                        calculate.PaymentType = (Frequency)calculatematurity.PaymentType;
                                        calculate.DueDate = calculatematurity.DueDate;
                                        calculate.Amount = calculatematurity.Amount;
                                        calculate.InterestRate = calculatematurity.InterestRate;
                                        calculate.NoofMonthOrYear = j;
                                        calculate.TotalProductYear = i;
                                        if (j == 2)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "1 Year" && a.Account == "2nd Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                        if (j == 3)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "1 Year" && a.Account == "3rd Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                        if (j == 4)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "1 Year" && a.Account == "4th Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                        if (j == 5)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "1 Year" && a.Account == "5th Acc").Select(a => a.MaturityAmount).FirstOrDefault();

                                        List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                        for (int k = 0; k < RIPList.Count; k++)
                                        {
                                            RIPList[k].Years = "2 Year";
                                            if (j == 1)
                                                RIPList[k].Account = "1st Acc";
                                            if (j == 2)
                                                RIPList[k].Account = "2nd Acc";
                                            if (j == 3)
                                                RIPList[k].Account = "3rd Acc";
                                            if (j == 4)
                                                RIPList[k].Account = "4th Acc";
                                            if (j == 5)
                                                RIPList[k].Account = "5th Acc";

                                        }

                                        MainList.AddRange(RIPList);

                                    }
                                }
                                if (i == 2)
                                {
                                    for (int j = 3; j < 6; j++)
                                    {
                                        CalculateRIP calculate = new CalculateRIP();
                                        calculate.TimePeriod = TimePeriod.Years;
                                        calculate.NoofMonthsandYearforParentRD = calculatematurity.NoOfMonthsORYears;
                                        calculate.OpeningDate = calculatematurity.OpeningDate;
                                        calculate.MaturityDate = calculatematurity.OpeningDate.AddYears(1);
                                        calculate.PaymentType = (Frequency)calculatematurity.PaymentType;
                                        calculate.DueDate = calculatematurity.DueDate;
                                        calculate.Amount = calculatematurity.Amount;
                                        calculate.InterestRate = calculatematurity.InterestRate;
                                        calculate.NoofMonthOrYear = j;
                                        calculate.TotalProductYear = i;
                                        if (j == 3)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "2 Year" && a.Account == "3rd Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                        if (j == 4)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "2 Year" && a.Account == "4th Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                        if (j == 5)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "2 Year" && a.Account == "5th Acc").Select(a => a.MaturityAmount).FirstOrDefault();

                                        List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                        for (int k = 0; k < RIPList.Count; k++)
                                        {
                                            RIPList[k].Years = "3 Year";
                                            if (j == 1)
                                                RIPList[k].Account = "1st Acc";
                                            if (j == 2)
                                                RIPList[k].Account = "2nd Acc";
                                            if (j == 3)
                                                RIPList[k].Account = "3rd Acc";
                                            if (j == 4)
                                                RIPList[k].Account = "4th Acc";
                                            if (j == 5)
                                                RIPList[k].Account = "5th Acc";

                                        }
                                        MainList.AddRange(RIPList);

                                    }
                                }
                                if (i == 3)
                                {
                                    for (int j = 4; j < 6; j++)
                                    {
                                        CalculateRIP calculate = new CalculateRIP();
                                        calculate.TimePeriod = TimePeriod.Years;
                                        calculate.NoofMonthsandYearforParentRD = calculatematurity.NoOfMonthsORYears;
                                        calculate.OpeningDate = calculatematurity.OpeningDate;
                                        calculate.MaturityDate = calculatematurity.OpeningDate.AddYears(1);
                                        calculate.PaymentType = (Frequency)calculatematurity.PaymentType;
                                        calculate.DueDate = calculatematurity.DueDate;
                                        calculate.Amount = calculatematurity.Amount;
                                        calculate.InterestRate = calculatematurity.InterestRate;
                                        calculate.NoofMonthOrYear = j;
                                        calculate.TotalProductYear = i;
                                        if (j == 4)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "3 Year" && a.Account == "4th Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                        if (j == 5)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "3 Year" && a.Account == "5th Acc").Select(a => a.MaturityAmount).FirstOrDefault();

                                        List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                        for (int k = 0; k < RIPList.Count; k++)
                                        {
                                            RIPList[k].Years = "4 Year";
                                            if (j == 1)
                                                RIPList[k].Account = "1st Acc";
                                            if (j == 2)
                                                RIPList[k].Account = "2nd Acc";
                                            if (j == 3)
                                                RIPList[k].Account = "3rd Acc";
                                            if (j == 4)
                                                RIPList[k].Account = "4th Acc";
                                            if (j == 5)
                                                RIPList[k].Account = "5th Acc";

                                        }
                                        MainList.AddRange(RIPList);

                                    }
                                }
                                if (i == 4)
                                {
                                    for (int j = 5; j < 6; j++)
                                    {
                                        CalculateRIP calculate = new CalculateRIP();
                                        calculate.TimePeriod = TimePeriod.Years;
                                        calculate.NoofMonthsandYearforParentRD = calculatematurity.NoOfMonthsORYears;
                                        calculate.OpeningDate = calculatematurity.OpeningDate;
                                        calculate.MaturityDate = calculatematurity.OpeningDate.AddYears(1);
                                        calculate.PaymentType = (Frequency)calculatematurity.PaymentType;
                                        calculate.DueDate = calculatematurity.DueDate;
                                        calculate.Amount = calculatematurity.Amount;
                                        calculate.InterestRate = calculatematurity.InterestRate;
                                        calculate.NoofMonthOrYear = j;
                                        calculate.TotalProductYear = i;
                                        if (j == 5)
                                            calculate.PreviousYearsBalance = MainList.Where(a => a.Years == "4 Year" && a.Account == "5th Acc").Select(a => a.MaturityAmount).FirstOrDefault();

                                        List<RIPList> RIPList = AkshyaTrityaMaturityCalculation(calculate);
                                        for (int k = 0; k < RIPList.Count; k++)
                                        {
                                            RIPList[k].Years = "5 Year";
                                            if (j == 1)
                                                RIPList[k].Account = "1st Acc";
                                            if (j == 2)
                                                RIPList[k].Account = "2nd Acc";
                                            if (j == 3)
                                                RIPList[k].Account = "3rd Acc";
                                            if (j == 4)
                                                RIPList[k].Account = "4th Acc";
                                            if (j == 5)
                                                RIPList[k].Account = "5th Acc";

                                        }
                                        MainList.AddRange(RIPList);

                                    }
                                }
                                decimal FirstAcc = MainList.Where(a => a.Years == "1 Year" && a.Account == "1st Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                decimal SecondAcc = MainList.Where(a => a.Years == "2 Year" && a.Account == "2nd Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                decimal ThirdAcc = MainList.Where(a => a.Years == "3 Year" && a.Account == "3rd Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                decimal FourthAcc = MainList.Where(a => a.Years == "4 Year" && a.Account == "4th Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                decimal FifthAcc = MainList.Where(a => a.Years == "5 Year" && a.Account == "5th Acc").Select(a => a.MaturityAmount).FirstOrDefault();
                                amount = FirstAcc + SecondAcc + ThirdAcc + FourthAcc + FifthAcc;

                            }

                        }

                    }
                }

                return amount;
            }
        }

        public decimal CalculateRDMaturity(CalculateMaturityAmount calculatematurity)
        {
            using (var db = new BSCCSLEntity())
            {
                int totalmonthyear = CalculateTotalInstallment(calculatematurity.TimePeriod.Value, calculatematurity.PaymentType.Value, calculatematurity.NoOfMonthsORYears.Value);


                SqlParameter Amount = new SqlParameter("Amount", calculatematurity.Amount);
                SqlParameter InterestRate = new SqlParameter("InterestRate", calculatematurity.InterestRate);
                SqlParameter OpeningDate = new SqlParameter("OpeningDate", calculatematurity.OpeningDate);
                SqlParameter MaturityDate = new SqlParameter("MaturityDate", calculatematurity.MaturityDate);
                SqlParameter DueDate = new SqlParameter("DueDate", calculatematurity.DueDate);
                SqlParameter PaymentType = new SqlParameter("PaymentType", calculatematurity.PaymentType);
                SqlParameter InterestType = new SqlParameter("InterestType", calculatematurity.InterestType);
                SqlParameter NoOfMonthsORYears = new SqlParameter("NoOfMonthsORYears", totalmonthyear);
                SqlParameter OpeningBalance = new SqlParameter("OpeningBalance", (object)calculatematurity.OpeningBalance ?? DBNull.Value);
                SqlParameter SkipFirstInstallment = new SqlParameter("SkipFirstInstallment", (object)calculatematurity.SkipFirstInstallment ?? DBNull.Value);
                SqlParameter MaturityAmount = new SqlParameter
                {
                    ParameterName = "@MaturityAmount",
                    SqlDbType = System.Data.SqlDbType.Decimal,
                    Size = 18,
                    Direction = System.Data.ParameterDirection.Output
                };


                var maturity = db.Database.SqlQuery<object>("RDMaturityCalculation @Amount, @InterestRate, @OpeningDate, @MaturityDate, @DueDate, @PaymentType, @InterestType, @NoOfMonthsORYears, @OpeningBalance, @SkipFirstInstallment, @MaturityAmount OUT", Amount, InterestRate, OpeningDate, MaturityDate, DueDate, PaymentType, InterestType, NoOfMonthsORYears, OpeningBalance, SkipFirstInstallment, MaturityAmount).FirstOrDefault();
                decimal amount = Convert.ToDecimal(MaturityAmount.Value);
                return amount;
            }
        }

        public decimal CalculateAkshtaTrityaMaturity(CalculateMaturityAmount calculatematurity)
        {
            using (var db = new BSCCSLEntity())
            {
                int totalmonthyear = CalculateTotalInstallment(calculatematurity.TimePeriod.Value, calculatematurity.PaymentType.Value, calculatematurity.NoOfMonthsORYears.Value);


                SqlParameter Amount = new SqlParameter("Amount", calculatematurity.Amount);
                SqlParameter InterestRate = new SqlParameter("InterestRate", calculatematurity.InterestRate);
                SqlParameter OpeningDate = new SqlParameter("OpeningDate", calculatematurity.OpeningDate);
                SqlParameter MaturityDate = new SqlParameter("MaturityDate", calculatematurity.MaturityDate);
                SqlParameter DueDate = new SqlParameter("DueDate", calculatematurity.DueDate);
                SqlParameter PaymentType = new SqlParameter("PaymentType", calculatematurity.PaymentType);
                SqlParameter InterestType = new SqlParameter("InterestType", calculatematurity.InterestType);
                SqlParameter NoOfMonthsORYears = new SqlParameter("NoOfMonthsORYears", totalmonthyear);
                SqlParameter OpeningBalance = new SqlParameter("OpeningBalance", (object)calculatematurity.OpeningBalance ?? DBNull.Value);
                SqlParameter SkipFirstInstallment = new SqlParameter("SkipFirstInstallment", (object)calculatematurity.SkipFirstInstallment ?? DBNull.Value);
                SqlParameter MaturityAmount = new SqlParameter
                {
                    ParameterName = "@MaturityAmount",
                    SqlDbType = System.Data.SqlDbType.Decimal,
                    Size = 18,
                    Direction = System.Data.ParameterDirection.Output
                };


                var maturity = db.Database.SqlQuery<object>("RDMaturityCalculation @Amount, @InterestRate, @OpeningDate, @MaturityDate, @DueDate, @PaymentType, @InterestType, @NoOfMonthsORYears, @OpeningBalance, @SkipFirstInstallment, @MaturityAmount OUT", Amount, InterestRate, OpeningDate, MaturityDate, DueDate, PaymentType, InterestType, NoOfMonthsORYears, OpeningBalance, SkipFirstInstallment, MaturityAmount).FirstOrDefault();
                decimal amount = Convert.ToDecimal(MaturityAmount.Value);
                return amount;
            }
        }


        public int CalculateTotalInstallment(TimePeriod timePeriod, Frequency paymentType, int NoOfMonthsORYears)
        {
            int totalmonthyear = 0;

            if (timePeriod == TimePeriod.Years && paymentType == Frequency.Monthly)
            {
                totalmonthyear = NoOfMonthsORYears * 12;
            }
            else if (timePeriod == TimePeriod.Days && paymentType == Frequency.Monthly)
            {
                totalmonthyear = NoOfMonthsORYears / 30;
            }
            else if (timePeriod == TimePeriod.Months && paymentType == Frequency.Monthly)
            {
                totalmonthyear = NoOfMonthsORYears;
            }
            else if (timePeriod == TimePeriod.Months && paymentType == Frequency.Quarterly)
            {
                totalmonthyear = NoOfMonthsORYears / 3;
            }
            else if (timePeriod == TimePeriod.Days && paymentType == Frequency.Quarterly)
            {
                totalmonthyear = NoOfMonthsORYears / 90;
            }
            else if (timePeriod == TimePeriod.Years && paymentType == Frequency.Quarterly)
            {
                totalmonthyear = NoOfMonthsORYears * 4;
            }
            else if (timePeriod == TimePeriod.Months && paymentType == Frequency.Half_Yearly)
            {
                totalmonthyear = NoOfMonthsORYears / 6;
            }
            else if (timePeriod == TimePeriod.Days && paymentType == Frequency.Half_Yearly)
            {
                totalmonthyear = NoOfMonthsORYears / 180;
            }
            else if (timePeriod == TimePeriod.Years && paymentType == Frequency.Half_Yearly)
            {
                totalmonthyear = NoOfMonthsORYears * 2;
            }
            else if (timePeriod == TimePeriod.Months && paymentType == Frequency.Yearly)
            {
                totalmonthyear = NoOfMonthsORYears / 12;
            }
            else if (timePeriod == TimePeriod.Days && paymentType == Frequency.Yearly)
            {
                totalmonthyear = NoOfMonthsORYears / 365;
            }
            else if (timePeriod == TimePeriod.Years && paymentType == Frequency.Yearly)
            {
                totalmonthyear = NoOfMonthsORYears;
            }
            else if (timePeriod == TimePeriod.Months && paymentType == Frequency.Daily)
            {
                totalmonthyear = NoOfMonthsORYears * 30;
            }
            else if (timePeriod == TimePeriod.Days && paymentType == Frequency.Daily)
            {
                totalmonthyear = NoOfMonthsORYears;
            }
            else if (timePeriod == TimePeriod.Years && paymentType == Frequency.Daily)
            {
                totalmonthyear = NoOfMonthsORYears * 365;
            }
            return totalmonthyear;
        }

        public object PendingRDInstallments(Guid CustomerProductId)
        {
            using (var db = new BSCCSLEntity())
            {
                DateTime today = DateTime.Now;
                var list1 = (from p in db.RDPayment.Where(a => a.IsDelete == false && a.CustomerProductId == CustomerProductId && a.IsPaid == false && a.RDPaymentType == RDPaymentType.Installment)
                             join c in db.CustomerProduct.Where(a => a.IsDelete == false) on p.CustomerProductId equals c.CustomerProductId
                             select new RDPendingPaymentInstallment
                             {
                                 RDPaymentId = p.RDPaymentId,
                                 CustomerProductId = c.CustomerProductId,
                                 Amount = p.Amount,
                                 CreatedDate = p.CreatedDate,
                                 NextDate = c.NextInstallmentDate,
                                 ReferenceCustomerProductId = c.ReferenceCustomerProductId
                             }).AsEnumerable();

                var list2 = list1.Skip(1).AsEnumerable();
                //var list2 = list1;

                List<RDPendingPaymentInstallment> combined = new List<RDPendingPaymentInstallment>();


                combined = list1.Zip(list2, (first, second) => new RDPendingPaymentInstallment
                {
                    CustomerProductId = first.CustomerProductId,
                    CreatedDate = first.CreatedDate,
                    NextDate = second.CreatedDate,
                    Amount = first.Amount,
                    RDPaymentId = first.RDPaymentId,
                    ReferenceCustomerProductId = first.ReferenceCustomerProductId,
                    LatePaymentCharges = first.ReferenceCustomerProductId == null ? db.RDPayment.Where(a => a.RDPaymentType == RDPaymentType.LatePaymentCharges && a.IsPaid == false && a.CreatedDate >= first.CreatedDate && a.CreatedDate < second.CreatedDate && a.CustomerProductId == first.CustomerProductId).Select(a => a.Amount).DefaultIfEmpty(0).Sum() : 0
                    //LatePaymentCharges = db.RDPayment.Where(a => a.IsDelete == false && a.RDPaymentType == RDPaymentType.LatePaymentCharges && a.IsPaid == false && a.CreatedDate >= first.CreatedDate && a.CreatedDate < second.CreatedDate && a.CustomerProductId == first.CustomerProductId).Count() == 0 ? 0 : db.RDPayment.Where(a => a.RDPaymentType == RDPaymentType.LatePaymentCharges && a.IsPaid == false && a.CreatedDate >= first.CreatedDate && a.CreatedDate < second.CreatedDate && a.CustomerProductId == first.CustomerProductId).Sum(a => a.Amount)
                }).ToList();


                if (list1.Count() > 0)
                {
                    RDPendingPaymentInstallment rdPayment = new RDPendingPaymentInstallment();
                    rdPayment = list1.LastOrDefault();
                    //rdPayment.LatePaymentCharges = db.RDPayment.Where(a => a.IsDelete == false && a.RDPaymentType == RDPaymentType.LatePaymentCharges && a.IsPaid == false && a.CreatedDate >= rdPayment.CreatedDate && a.CreatedDate < rdPayment.NextDate && a.CustomerProductId == rdPayment.CustomerProductId).Count() == 0 ? 0 : db.RDPayment.Where(a => a.RDPaymentType == RDPaymentType.LatePaymentCharges && a.IsPaid == false && a.CreatedDate >= rdPayment.CreatedDate && a.CreatedDate < rdPayment.NextDate && a.CustomerProductId == rdPayment.CustomerProductId).Sum(a => a.Amount);
                    rdPayment.LatePaymentCharges = list1.LastOrDefault().ReferenceCustomerProductId == null ? db.RDPayment.Where(a => a.RDPaymentType == RDPaymentType.LatePaymentCharges && a.IsPaid == false && a.CreatedDate >= rdPayment.CreatedDate && a.CreatedDate < rdPayment.NextDate && a.CustomerProductId == rdPayment.CustomerProductId).Select(a => a.Amount).DefaultIfEmpty(0).Sum() : 0;
                    combined.Add(rdPayment);
                }

                return combined;
            }
        }

        public object GetCustomerRDAccount(Guid CustomerId)
        {
            using (var db = new BSCCSLEntity())
            {
                var list1 = (from c in db.CustomerProduct.Where(a => a.IsDelete == false && a.IsActive == true && a.IsFreeze == null && a.Status == CustomerProductStatus.Approved && (a.ProductType == ProductType.Recurring_Deposit || a.ProductType == ProductType.Regular_Income_Planner || a.ProductType == ProductType.Three_Year_Product) && a.CustomerId == CustomerId && a.ReferenceCustomerProductId == null)
                             join p in db.Product on c.ProductId equals p.ProductId
                             select new InterAccountList
                             {
                                 CustomerProductId = c.CustomerProductId,
                                 ProductName = p.ProductName,
                                 Amount = c.Amount,
                                 AccountNumber = c.AccountNumber,
                                 ProductType = c.ProductType
                             }).ToList();

                var list2 = (from c in db.CustomerProduct.Where(a => a.IsDelete == false && a.IsActive == true && a.ProductType == ProductType.Loan && a.CustomerId == CustomerId && a.ReferenceCustomerProductId == null)
                             join p in db.Product on c.ProductId equals p.ProductId
                             join l in db.Loan.Where(a => a.LoanStatus == LoanStatus.Disbursed) on c.CustomerProductId equals l.CustomerProductId
                             select new InterAccountList
                             {
                                 CustomerProductId = c.CustomerProductId,
                                 ProductName = p.ProductName,
                                 Amount = l.MonthlyInstallmentAmount.Value,
                                 AccountNumber = c.AccountNumber,
                                 ProductType = c.ProductType
                             }).ToList();

                var list = list1.Union(list2);

                return list;
            }
        }


        public object GetCustomerMappedRDAccount(Guid CustomerProductId)
        {
            using (var db = new BSCCSLEntity())
            {
                var list1 = (from c in db.CustomerProduct.Where(a => a.IsDelete == false && a.IsActive == true && a.IsFreeze == null && a.Status == CustomerProductStatus.Approved && (a.ProductType == ProductType.Recurring_Deposit || a.ProductType == ProductType.Regular_Income_Planner || a.ProductType == ProductType.Three_Year_Product) && a.ReferenceCustomerProductId == CustomerProductId)
                             join p in db.Product on c.ProductId equals p.ProductId
                             select new InterAccountList
                             {
                                 CustomerProductId = c.CustomerProductId,
                                 ProductName = p.ProductName,
                                 Amount = c.Amount,
                                 AccountNumber = c.AccountNumber,
                                 ProductType = c.ProductType
                             }).ToList();

                var list2 = (from c in db.CustomerProduct.Where(a => a.IsDelete == false && a.IsActive == true && a.ProductType == ProductType.Loan && a.ReferenceCustomerProductId == CustomerProductId)
                             join p in db.Product on c.ProductId equals p.ProductId
                             join l in db.Loan.Where(a => a.LoanStatus == LoanStatus.Disbursed) on c.CustomerProductId equals l.CustomerProductId
                             select new InterAccountList
                             {
                                 CustomerProductId = c.CustomerProductId,
                                 ProductName = p.ProductName,
                                 Amount = l.MonthlyInstallmentAmount.Value,
                                 AccountNumber = c.AccountNumber,
                                 ProductType = c.ProductType
                             }).ToList();

                var list = list1.Union(list2);

                return list;
            }
        }

        public object CalculatePrematureWithdrawalRDFD(Guid customerProductId)
        {
            using (var db = new BSCCSLEntity())
            {

                PrematureRDFD prematureRDFD = new PrematureRDFD();

                CustomerProduct customerProduct = db.CustomerProduct.Where(a => a.CustomerProductId == customerProductId).FirstOrDefault();

                //double diff = (DateTime.Now.Date - customerProduct.OpeningDate).TotalDays;

                //double yeartillnow = Math.Ceiling(diff / 365);
                //if (yeartillnow == 0)
                //{
                //    yeartillnow = 1;
                //}

                int TotalMonthtillDate = 12 * (DateTime.Now.Date.Year - customerProduct.OpeningDate.Year) + DateTime.Now.Date.Month - customerProduct.OpeningDate.Month;
                if (TotalMonthtillDate == 0)
                    TotalMonthtillDate = 1;

                decimal totalyear = Math.Ceiling(Convert.ToDecimal(customerProduct.TotalDays.Value) / 365);

                if (totalyear == 0)
                {
                    totalyear = 1;
                }

                //if (totalyear > 5)
                //{ totalyear = 5; }

                //if (yeartillnow > 5)
                //{ yeartillnow = 5; }

                //decimal percentage = db.PrematureRDFDPercentage.Where(a => a.Year == totalyear && a.PrematureYear == yeartillnow).OrderBy(a => a.Year).Select(a => a.Percentage).DefaultIfEmpty(0).FirstOrDefault();
                decimal percentage = db.RDFDPrematurePercentage.Where(a => a.TotalYear == totalyear && a.PrematureMonth == TotalMonthtillDate).OrderBy(a => a.TotalYear).Select(a => a.Percentage).DefaultIfEmpty(0).FirstOrDefault();

                decimal interest = db.DailyInterest.Where(a => a.CustomerProductId == customerProductId && a.IsPaid == false).Select(a => a.TodaysInterest).DefaultIfEmpty(0).Sum();

                decimal balance = customerProduct.Balance.Value;

                if (percentage > 0)
                {
                    if (customerProduct.OpeningDate.Date != DateTime.Now.Date)
                    {

                        if (customerProduct.IsAutoFD == null)
                        {
                            if (interest > 0)
                            {
                                balance = balance + interest;
                            }
                            prematureRDFD.CustomerProductId = customerProductId;
                            prematureRDFD.PrematurePercentage = percentage;
                            prematureRDFD.PrematureCharges = (balance * percentage) / 100;
                            prematureRDFD.PrematureAmount = balance - prematureRDFD.PrematureCharges;
                            prematureRDFD.OpeningDate = customerProduct.OpeningDate;
                            prematureRDFD.NoOfMonthsORYears = customerProduct.NoOfMonthsORYears.Value;
                            prematureRDFD.Period = ((TimePeriod)customerProduct.TimePeriod).ToString();
                            prematureRDFD.ProductType = customerProduct.ProductType;
                            prematureRDFD.InterestRate = customerProduct.InterestRate;
                            prematureRDFD.Balance = balance;
                        }
                        else
                        {
                            prematureRDFD.CustomerProductId = customerProductId;
                            prematureRDFD.PrematurePercentage = 0;
                            prematureRDFD.PrematureCharges = 0;
                            prematureRDFD.PrematureAmount = balance;
                            prematureRDFD.OpeningDate = customerProduct.OpeningDate;
                            prematureRDFD.NoOfMonthsORYears = customerProduct.NoOfMonthsORYears.Value;
                            prematureRDFD.Period = ((TimePeriod)customerProduct.TimePeriod).ToString();
                            prematureRDFD.ProductType = customerProduct.ProductType;
                            prematureRDFD.InterestRate = customerProduct.InterestRate;
                            prematureRDFD.Balance = balance;
                        }

                    }
                    else
                    {
                        prematureRDFD.CustomerProductId = customerProductId;
                        prematureRDFD.PrematurePercentage = 0;
                        prematureRDFD.PrematureCharges = 0;
                        prematureRDFD.PrematureAmount = balance;
                        prematureRDFD.OpeningDate = customerProduct.OpeningDate;
                        prematureRDFD.NoOfMonthsORYears = customerProduct.NoOfMonthsORYears.Value;
                        prematureRDFD.Period = ((TimePeriod)customerProduct.TimePeriod).ToString();
                        prematureRDFD.ProductType = customerProduct.ProductType;
                        prematureRDFD.InterestRate = customerProduct.InterestRate;
                        prematureRDFD.Balance = balance;
                    }


                }
                else
                {
                    prematureRDFD.CustomerProductId = customerProductId;
                    prematureRDFD.PrematurePercentage = 0;
                    prematureRDFD.PrematureCharges = 0;
                    prematureRDFD.PrematureAmount = balance;
                    prematureRDFD.OpeningDate = customerProduct.OpeningDate;
                    prematureRDFD.NoOfMonthsORYears = customerProduct.NoOfMonthsORYears.Value;
                    prematureRDFD.Period = ((TimePeriod)customerProduct.TimePeriod).ToString();
                    prematureRDFD.ProductType = customerProduct.ProductType;
                    prematureRDFD.InterestRate = customerProduct.InterestRate;
                    prematureRDFD.Balance = balance;

                }
                return prematureRDFD;
            }
        }

        public object PrematureWithDrawalRDFD(PrematureRDFD prematureRDFD, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                TransactionService transactionService = new TransactionService();

                CustomerProduct customerProduct = db.CustomerProduct.Where(a => a.CustomerProductId == prematureRDFD.CustomerProductId).FirstOrDefault();

                //double diff = (DateTime.Now.Date - customerProduct.OpeningDate).TotalDays;

                //double yeartillnow = Math.Ceiling(diff / 365);
                //if (yeartillnow == 0)
                //{
                //    yeartillnow = 1;
                //}

                //decimal totalyear = Math.Floor(Convert.ToDecimal(customerProduct.TotalDays.Value) / 365);

                //decimal percentage = db.PrematureRDFDPercentage.Where(a => a.Year == totalyear && a.PrematureYear == yeartillnow).OrderBy(a => a.Year).Select(a => a.Percentage).DefaultIfEmpty(0).FirstOrDefault();

                decimal interest = db.DailyInterest.Where(a => a.CustomerProductId == prematureRDFD.CustomerProductId && a.IsPaid == false).Select(a => a.TodaysInterest).DefaultIfEmpty(0).Sum();

                if (interest > 0)
                {

                    if (customerProduct.OpeningDate.Date != DateTime.Now.Date)
                    {

                        Transactions transaction1 = new Transactions();
                        transaction1.BranchId = customerProduct.BranchId;
                        transaction1.CustomerId = customerProduct.CustomerId;
                        transaction1.CustomerProductId = customerProduct.CustomerProductId;
                        transaction1.Amount = interest;
                        transaction1.Balance = customerProduct.UpdatedBalance.Value + interest;
                        transaction1.Status = Status.Clear;
                        transaction1.TransactionType = TransactionType.BankTransfer;
                        transaction1.Type = TypeCRDR.CR;
                        transaction1.TransactionTime = DateTime.Now;
                        transaction1.CreatedDate = DateTime.Now;
                        transaction1.CreatedBy = user.UserId;
                        transaction1.DescIndentify = DescIndentify.Interest;
                        transaction1.TransactionId = transactionService.InsertTransctionUsingSP(transaction1);

                        db.DailyInterest.Where(a => a.CustomerProductId == prematureRDFD.CustomerProductId && a.IsPaid == false).ToList().ForEach(u => { u.IsPaid = true; });

                    }
                    //customerProduct.Balance = customerProduct.Balance + interest;
                    //customerProduct.UpdatedBalance = customerProduct.UpdatedBalance + interest;

                }

                var customerProductSaving = db.CustomerProduct.Where(a => a.CustomerId == customerProduct.CustomerId && a.ProductType == ProductType.Saving_Account && a.IsActive == true && a.IsDelete == false).Select(a => new { a.CustomerProductId, a.UpdatedBalance, a.Balance }).FirstOrDefault();

                //decimal prematureCharges = ((customerProduct.Balance.Value) * percentage) / 100;

                //decimal prematureAmount = customerProduct.Balance.Value - prematureCharges;


                //customerProduct.Balance = customerProduct.Balance - prematureRDFD.PrematureCharges;
                //customerProduct.UpdatedBalance = customerProduct.UpdatedBalance - prematureRDFD.PrematureCharges;

                Transactions transaction2 = new Transactions();
                transaction2.BranchId = customerProduct.BranchId;
                transaction2.CustomerId = customerProduct.CustomerId;
                transaction2.CustomerProductId = customerProduct.CustomerProductId;
                transaction2.Amount = prematureRDFD.PrematureCharges;
                transaction2.Balance = customerProduct.UpdatedBalance.Value;
                transaction2.Status = Status.Clear;
                transaction2.TransactionType = TransactionType.BankTransfer;
                transaction2.Type = TypeCRDR.DR;
                transaction2.TransactionTime = DateTime.Now.AddSeconds(5);
                transaction2.CreatedDate = DateTime.Now;
                transaction2.CreatedBy = user.UserId;
                transaction2.DescIndentify = DescIndentify.Premature_Charges;
                transaction2.RefCustomerProductId = customerProductSaving.CustomerProductId;
                transaction2.TransactionId = transactionService.InsertTransctionUsingSP(transaction2);

                //customerProduct.Balance = customerProduct.Balance - prematureRDFD.PrematureAmount;
                //customerProduct.UpdatedBalance = customerProduct.UpdatedBalance - prematureRDFD.PrematureAmount;

                Transactions transaction3 = new Transactions();
                transaction3.BranchId = customerProduct.BranchId;
                transaction3.CustomerId = customerProduct.CustomerId;
                transaction3.CustomerProductId = customerProduct.CustomerProductId;
                transaction3.Amount = prematureRDFD.PrematureAmount;
                transaction3.Balance = customerProduct.UpdatedBalance.Value;
                transaction3.Status = Status.Clear;
                transaction3.TransactionType = TransactionType.BankTransfer;
                transaction3.Type = TypeCRDR.DR;
                transaction3.TransactionTime = DateTime.Now.AddSeconds(10);
                transaction3.CreatedDate = DateTime.Now;
                transaction3.CreatedBy = user.UserId;
                transaction3.DescIndentify = DescIndentify.Maturity;
                transaction3.RefCustomerProductId = customerProductSaving.CustomerProductId;
                transaction3.TransactionId = transactionService.InsertTransctionUsingSP(transaction3);



                Transactions transaction4 = new Transactions();
                transaction4.BranchId = customerProduct.BranchId;
                transaction4.CustomerId = customerProduct.CustomerId;
                transaction4.CustomerProductId = customerProductSaving.CustomerProductId;
                transaction4.Amount = prematureRDFD.PrematureAmount; ;
                transaction4.Balance = customerProductSaving.UpdatedBalance.Value + prematureRDFD.PrematureAmount;
                transaction4.Status = Status.Clear;
                transaction4.TransactionType = TransactionType.BankTransfer;
                transaction4.Type = TypeCRDR.CR;
                transaction4.TransactionTime = DateTime.Now;
                transaction4.CreatedDate = DateTime.Now;
                transaction4.CreatedBy = user.UserId; ;
                transaction4.DescIndentify = DescIndentify.Maturity;
                transaction4.RefCustomerProductId = customerProduct.CustomerProductId;
                transaction4.TransactionId = transactionService.InsertTransctionUsingSP(transaction4);

                customerProduct.PrematurePercentage = prematureRDFD.PrematurePercentage;
                customerProduct.PrematureAmount = prematureRDFD.PrematureAmount;
                customerProduct.PrematuredBy = user.UserId;
                customerProduct.PrematureDate = DateTime.Now;
                customerProduct.Status = CustomerProductStatus.Completed;
                customerProduct.IsPrematured = true;
                db.SaveChanges();
                //var Ref = new CustomerProduct() { CustomerProductId = customerProductSaving.CustomerProductId, UpdatedBalance = customerProductSaving.UpdatedBalance + prematureRDFD.PrematureAmount, Balance = customerProductSaving.Balance + prematureRDFD.PrematureAmount };
                //db.CustomerProduct.Attach(Ref);
                //db.Entry(Ref).Property(s => s.UpdatedBalance).IsModified = true;
                //db.Entry(Ref).Property(s => s.Balance).IsModified = true;
                //db.SaveChanges();
                return true;
            }
        }

        public object GetPremetureRIPData(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                var MaturityData = db.Database.SqlQuery<PrematureRIPData>("PrematureRIP @CustomerProductId, @Date",
               new SqlParameter("CustomerProductId", Id),
               new SqlParameter("Date", DateTime.Now.Date)).FirstOrDefault();
                MaturityData.Period = ((TimePeriod)MaturityData.TimePeriod).ToString();
                return MaturityData;
            }
        }

        public object PrematureWithDrawalRIP(PrematureRIPData PrematureRIP, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                TransactionService transactionService = new TransactionService();

                CustomerProduct customerProduct = db.CustomerProduct.Where(a => a.CustomerProductId == PrematureRIP.CustomerProductId).FirstOrDefault();

                var customerProductSaving = db.CustomerProduct.Where(a => a.CustomerId == customerProduct.CustomerId && a.ProductType == ProductType.Saving_Account && a.IsActive == true).Select(a => a.CustomerProductId).FirstOrDefault();

                //customerProduct.Balance = customerProduct.Balance - PrematureRIP.TotalAmount;
                //customerProduct.UpdatedBalance = customerProduct.UpdatedBalance - PrematureRIP.TotalAmount;

                Transactions transaction3 = new Transactions();
                transaction3.BranchId = customerProduct.BranchId;
                transaction3.CustomerId = customerProduct.CustomerId;
                transaction3.CustomerProductId = customerProduct.CustomerProductId;
                transaction3.Amount = PrematureRIP.TotalAmount;
                //transaction3.Balance = customerProduct.UpdatedBalance.Value;
                transaction3.Status = Status.Clear;
                transaction3.TransactionType = TransactionType.BankTransfer;
                transaction3.Type = TypeCRDR.DR;
                transaction3.TransactionTime = DateTime.Now.AddSeconds(10);
                transaction3.CreatedDate = DateTime.Now;
                transaction3.CreatedBy = user.UserId;
                transaction3.DescIndentify = DescIndentify.Maturity;
                transaction3.RefCustomerProductId = customerProductSaving;
                transaction3.TransactionId = transactionService.InsertTransctionUsingSP(transaction3);

                Transactions transaction4 = new Transactions();
                transaction4.BranchId = customerProduct.BranchId;
                transaction4.CustomerId = customerProduct.CustomerId;
                transaction4.CustomerProductId = customerProductSaving;
                transaction4.Amount = PrematureRIP.PrematureAmount;
                //transaction4.Balance = customerProductSaving.UpdatedBalance.Value + PrematureRIP.PrematureAmount;
                transaction4.Status = Status.Clear;
                transaction4.TransactionType = TransactionType.BankTransfer;
                transaction4.Type = TypeCRDR.CR;
                transaction4.TransactionTime = DateTime.Now;
                transaction4.CreatedDate = DateTime.Now;
                transaction4.CreatedBy = user.UserId;
                transaction4.DescIndentify = DescIndentify.Maturity;
                transaction4.RefCustomerProductId = customerProduct.CustomerProductId;
                transaction4.TransactionId = transactionService.InsertTransctionUsingSP(transaction4);

                //customerProduct.PrematurePercentage = percentage;
                customerProduct.PrematureAmount = PrematureRIP.PrematureAmount;
                customerProduct.PrematuredBy = user.UserId;
                customerProduct.PrematureDate = DateTime.Now;
                customerProduct.Status = CustomerProductStatus.Completed;
                customerProduct.IsPrematured = true;
                db.SaveChanges();

                //var Ref = new CustomerProduct() { CustomerProductId = customerProductSaving.CustomerProductId, UpdatedBalance = customerProductSaving.UpdatedBalance + PrematureRIP.PrematureAmount, Balance = customerProductSaving.Balance + PrematureRIP.PrematureAmount };
                //db.CustomerProduct.Attach(Ref);
                //db.Entry(Ref).Property(s => s.UpdatedBalance).IsModified = true;
                //db.Entry(Ref).Property(s => s.Balance).IsModified = true;
                //db.SaveChanges();

                return true;
            }
        }

        public bool UpdatePrintFlag(Guid customerProductId)
        {
            using (var db = new BSCCSLEntity())
            {
                var Ref = new CustomerProduct() { CustomerProductId = customerProductId, IsCertificatePrinted = true };
                db.CustomerProduct.Attach(Ref);
                db.Entry(Ref).Property(s => s.IsCertificatePrinted).IsModified = true;
                db.SaveChanges();
                return true;
            }
        }

        public bool SaveCertificatePrintHistory(PrintCertificateHistory PrintCertificateHistory, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                PrintCertificateHistory.PrintedBy = user.UserId;
                db.PrintCertificateHistory.Add(PrintCertificateHistory);
                db.SaveChanges();
                return true;
            }
        }

        public object GetCertificatePrintHistory(DataTableSearch search)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = (from h in db.PrintCertificateHistory.Include("Product").Where(s => s.CustomerProductId == search.id)
                            join u in db.User on h.PrintedBy equals u.UserId
                            select new
                            {
                                h.CertificateHistoryId,
                                h.CustomerProductId,
                                h.Reason,
                                PrintBy = u.FirstName,
                                h.PrintedDate,
                                h.IsDuplicate
                            }).AsQueryable();

                var ProductList = list.OrderBy(c => c.PrintedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = ProductList.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = ProductList
                };
                return data;
            }
        }

        public bool ProductAgentCommission(ProductAgentCommission agentCommission)
        {
            using (var db = new BSCCSLEntity())
            {
                var agentcommission = db.Database.SqlQuery<object>("ProductAgentCommission @Amount, @OpeningDate, @Date, @ProductId, @AgentId, @RankId, @TotalDays, @CustomerProductId, @RDPaymentId",
                  new SqlParameter("Amount", agentCommission.Amount),
                  new SqlParameter("OpeningDate", agentCommission.OpeningDate),
                  new SqlParameter("Date", agentCommission.Date),
                  new SqlParameter("ProductId", agentCommission.ProductId),
                  new SqlParameter("AgentId", agentCommission.AgentId),
                  new SqlParameter("RankId", agentCommission.RankId),
                  new SqlParameter("TotalDays", agentCommission.TotalDays),
                  new SqlParameter("CustomerProductId", agentCommission.CustomerProductId),
                  new SqlParameter("RDPaymentId", (object)agentCommission.RDPaymentId ?? DBNull.Value)).FirstOrDefault();

                return true;
            }

        }

        public bool CloseCustomerAccount(Guid Id, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                TransactionService objTransactionService = new TransactionService();
                var data = db.CustomerProduct.Where(x => x.CustomerProductId == Id && x.IsDelete == false && x.IsActive == true).Select(a => new { a.Balance, a.UpdatedBalance, a.CustomerProductId, a.CustomerId, a.BranchId }).FirstOrDefault();

                decimal diff = data.UpdatedBalance.Value - data.Balance.Value;

                //if (diff == 0 && data.Balance == 0)
                if (diff == 0)
                {
                    if (data.Balance != 0 || data.Balance != null)
                    {
                        Transactions WithdrawTransaction = new Transactions();

                        WithdrawTransaction.Amount = (decimal)data.Balance;
                        WithdrawTransaction.CustomerId = data.CustomerId;
                        WithdrawTransaction.CustomerProductId = data.CustomerProductId;
                        WithdrawTransaction.TransactionType = TransactionType.Cash;
                        WithdrawTransaction.Status = 0;
                        WithdrawTransaction.Type = BSCCSL.Models.TypeCRDR.DR;
                        WithdrawTransaction.TransactionTime = DateTime.Now;
                        WithdrawTransaction.CreatedBy = user.UserId;
                        WithdrawTransaction.CreatedDate = DateTime.Now;
                        WithdrawTransaction.BranchId = data.BranchId;
                        WithdrawTransaction.DescIndentify = DescIndentify.Cash_Withdraw;
                        Guid transactionId = objTransactionService.InsertTransctionUsingSP(WithdrawTransaction);
                    }

                    var Ref = new CustomerProduct() { CustomerProductId = Id, Status = CustomerProductStatus.Closed, IsActive = false, ModifyBy = user.UserId, ModifiedDate = DateTime.Now };
                    db.CustomerProduct.Attach(Ref);
                    db.Entry(Ref).Property(s => s.Status).IsModified = true;
                    db.Entry(Ref).Property(s => s.IsActive).IsModified = true;
                    db.Entry(Ref).Property(s => s.ModifyBy).IsModified = true;
                    db.Entry(Ref).Property(s => s.ModifiedDate).IsModified = true;
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public object DailyInterestCalculation(Guid CustomerProductId, DateTime date)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    //var result = db.Database.SqlQuery<object>("InterestCalculation @Date",
                    //new SqlParameter("Date", DateTime.Now)).FirstOrDefault();

                    string connectionstring = db.Database.Connection.ConnectionString;
                    SqlConnection sql = new SqlConnection(connectionstring);
                    SqlCommand cmd = new SqlCommand("InterestCalculationByCPId", sql);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter
                    SqlParameter paramdate = cmd.Parameters.Add("@Date", date);
                    paramdate.Value = date;
                    SqlParameter paramCPID = cmd.Parameters.Add("@CPId", CustomerProductId);
                    paramCPID.Value = CustomerProductId;

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
