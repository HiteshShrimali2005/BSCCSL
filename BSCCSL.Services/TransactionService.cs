using BSCCSL.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace BSCCSL.Services
{
    public class TransactionService
    {

        CustomerProductService customerProductService;
        LoanService loanService;
        public TransactionService()
        {
            loanService = new LoanService();
            customerProductService = new CustomerProductService();
        }

        //For Transaction
        public object GetCustomerDataByProductId(string accNo)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {

                var CustomerProductDetails = (from cp in db.CustomerProduct.Where(x => x.AccountNumber.ToLower() == accNo.ToLower() && x.IsDelete == false && (x.ProductType == ProductType.Saving_Account || x.ProductType == ProductType.Current_Account))
                                              join cpd in db.CustomerPersonalDetail.Where(a => a.IsDelete == false) on cp.CustomerId equals cpd.CustomerId
                                              join ca in db.CustomerAddress.Where(a => a.IsDelete == false) on cpd.PersonalDetailId equals ca.PersonalDetailId
                                              join c in db.Customer.Where(a => a.IsDelete == false) on cp.CustomerId equals c.CustomerId
                                              select new
                                              {
                                                  CustomerProductId = cp.CustomerProductId,
                                                  CustomerId = cp.CustomerId,
                                                  Balance = cp.Balance,
                                                  CustomerName = cpd.FirstName + " " + cpd.MiddleName + " " + cpd.LastName,
                                                  Holdersign = cpd.HolderSign,
                                                  DOB = cpd.DOB,
                                                  MobileNo = ca.MobileNo,
                                                  Address = (!string.IsNullOrEmpty(ca.DoorNo) ? ca.DoorNo + ", " : "") + (!string.IsNullOrEmpty(ca.BuildingName) ? ca.BuildingName + ", " : "") + (!string.IsNullOrEmpty(ca.PlotNo_Street) ? ca.PlotNo_Street + ", " : "") + (!string.IsNullOrEmpty(ca.Landmark) ? ca.Landmark + ", " : "") + (!string.IsNullOrEmpty(ca.Area) ? ca.Area + ", " : "") + (!string.IsNullOrEmpty(ca.City) ? ca.City + ", " : "") + (!string.IsNullOrEmpty(ca.District) ? ca.District + ", " : "") + (!string.IsNullOrEmpty(ca.State) ? ca.State + ", " : "") + (!string.IsNullOrEmpty(ca.Pincode) ? ca.Pincode : ""),
                                                  ProductType = cp.ProductType,
                                                  //CustomerProductId = cp.CustomerProductId,
                                                  //AccountNumber = cp.AccountNumber,
                                              }).ToList();

                var Balance = db.CustomerProduct.Where(x => x.AccountNumber == accNo && (x.ProductType == ProductType.Saving_Account || x.ProductType == ProductType.Current_Account)).Select(x => x.Balance).FirstOrDefault();


                return new
                {
                    CustomerProductDetails,
                    Balance = Balance != null ? Balance : 0.0M,
                    //ProductType= productname
                };
            }

        }

        public object GetTransactionData(DataTableSearch search)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = db.Transaction.Where(t => t.CustomerProductId == search.id).AsQueryable();

                if (search.Type != 0)
                {
                    //list = list.Where(c => c.Type == search.Type);
                    if (search.Type == TypeCRDR.CR)
                        list = list.Where(c => c.Balance >= 0);
                    else
                        list = list.Where(c => c.Balance < 0);
                }
                if (search.fromDate != null)
                {
                    list = list.Where(c => DbFunctions.TruncateTime(c.TransactionTime) >= search.fromDate);
                }
                if (search.toDate != null)
                {
                    list = list.Where(c => DbFunctions.TruncateTime(c.TransactionTime) <= search.toDate);
                }

                var TransactionList = list.OrderBy(a => a.TransactionTime).ThenBy(a => a.TransactionId).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();
                var TotalTransactionList = list.OrderBy(a => a.TransactionTime).ThenBy(a => a.TransactionId).ToList();
                var LastTransId = TotalTransactionList.OrderByDescending(p => p.TransactionTime).FirstOrDefault();
                var data = new
                {
                    LastTransactionId = LastTransId != null ? LastTransId.TransactionId.ToString() : "",
                    sEcho = search.sEcho,
                    iTotalRecords = TransactionList.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = TransactionList,
                    CurrentDate = DateTime.Now
                };
                return data;
            }
        }

        public Guid InsertTransctionUsingSP(Transactions transaction)
        {
            using (var db = new BSCCSLEntity())
            {

                Guid transctionId = new Guid();

                SqlParameter CustomerProductId = new SqlParameter("CustomerProductId", transaction.CustomerProductId);
                SqlParameter CustomerId = new SqlParameter("CustomerId", transaction.CustomerId);
                SqlParameter Amount = new SqlParameter("Amount", transaction.Amount);
                SqlParameter Balance = new SqlParameter("Balance", transaction.Balance);
                SqlParameter Type = new SqlParameter("Type", transaction.Type);
                SqlParameter CreatedBy = new SqlParameter("CreatedBy", transaction.CreatedBy);
                SqlParameter CreatedDate = new SqlParameter("CreatedDate", transaction.CreatedDate);
                SqlParameter ModifiedBy = new SqlParameter("ModifiedBy", (object)transaction.ModifiedBy ?? DBNull.Value);
                SqlParameter ModifiedDate = new SqlParameter("ModifiedDate", (object)transaction.ModifiedDate ?? DBNull.Value);
                SqlParameter TransactionType = new SqlParameter("TransactionType", transaction.TransactionType);
                SqlParameter CheckNumber = new SqlParameter("CheckNumber", (object)transaction.CheckNumber ?? DBNull.Value);
                SqlParameter ChequeDate = new SqlParameter("ChequeDate", (object)transaction.ChequeDate ?? DBNull.Value);
                SqlParameter BounceReason = new SqlParameter("BounceReason", (object)transaction.BounceReason ?? DBNull.Value);
                SqlParameter Penalty = new SqlParameter("Penalty", (object)transaction.Penalty ?? DBNull.Value);
                SqlParameter ChequeClearDate = new SqlParameter("ChequeClearDate", (object)transaction.ChequeClearDate ?? DBNull.Value);
                SqlParameter Status = new SqlParameter("Status", (object)transaction.Status ?? DBNull.Value);
                SqlParameter TransactionTime = new SqlParameter("TransactionTime", (object)transaction.TransactionTime ?? DBNull.Value);
                SqlParameter BankName = new SqlParameter("BankName", (object)transaction.BankName ?? DBNull.Value);
                SqlParameter BranchId = new SqlParameter("BranchId", transaction.BranchId);
                SqlParameter DescIndentify = new SqlParameter("DescIndentify", (object)transaction.DescIndentify ?? DBNull.Value);
                SqlParameter RefCustomerProductId = new SqlParameter("RefCustomerProductId", (object)transaction.RefCustomerProductId ?? DBNull.Value);

                SqlParameter NEFTNumber = new SqlParameter("NEFTNumber", (object)transaction.NEFTNumber ?? DBNull.Value);
                SqlParameter NEFTDate = new SqlParameter("NEFTDate", (object)transaction.NEFTDate ?? DBNull.Value);

                SqlParameter Id = new SqlParameter
                {

                    ParameterName = "@Id",
                    SqlDbType = System.Data.SqlDbType.UniqueIdentifier,
                    Size = 50,
                    Direction = System.Data.ParameterDirection.Output
                };
                db.Database.CommandTimeout = 3600;
                var trans = db.Database.SqlQuery<object>("SaveTransaction @CustomerProductId,@CustomerId, @Amount ,@Balance, @Type, @CreatedBy, @CreatedDate,@ModifiedBy, @ModifiedDate,@TransactionType,@CheckNumber,@ChequeDate,@BounceReason,@Penalty,@ChequeClearDate,@Status,@TransactionTime,@BankName,@BranchId,@DescIndentify,@RefCustomerProductId,@NEFTNumber,@NEFTDate,@Id  OUT", CustomerProductId, CustomerId, Amount, Balance, Type, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, TransactionType, CheckNumber, ChequeDate, BounceReason, Penalty, ChequeClearDate, Status, TransactionTime, BankName, BranchId, DescIndentify, RefCustomerProductId, NEFTNumber, NEFTDate, Id).FirstOrDefault();
                transctionId = Guid.Parse(Id.Value.ToString());
                SaveWealthTransaction(transaction, null);
                if (transaction.DescIndentify != Models.DescIndentify.Cheque_Bounce && transaction.DescIndentify != Models.DescIndentify.Panelty)
                {
                    CustomerProduct objCustomerProduct = db.CustomerProduct.FirstOrDefault(a => a.CustomerProductId == transaction.CustomerProductId);
                    if (objCustomerProduct != null)
                    {
                        if (objCustomerProduct.ProductType == ProductType.Saving_Account)
                            SMSService.SendTransactionSMS(transctionId);

                    }

                    //Change By Akhilesh... To Send only Saving Account Transactions SMS
                    //SMSService.SendTransactionSMS(transctionId);


                    //Thread sms = new Thread(delegate ()
                    //{
                    //    SMSService.SendTransactionSMS(transctionId);
                    //})
                    //{
                    //    IsBackground = true
                    //};
                    //sms.Start();

                }

                return transctionId;
            }
        }
        public bool SaveWealthTransaction(Transactions transaction,DateTime? MaturityDate1)
        {
            using (var db = new BSCCSLEntity())
            {
                //SaveWealthCreatorTransaction
                SqlParameter CustomerProductId = new SqlParameter("CustomerProductId", transaction.CustomerProductId);               
                SqlParameter Type = new SqlParameter("Type", transaction.Type);
                SqlParameter CreatedBy = new SqlParameter("CreatedBy", transaction.CreatedBy);
                SqlParameter CreatedDate = new SqlParameter("CreatedDate", transaction.CreatedDate);                
                SqlParameter DateOfCreditAmount = new SqlParameter("DateOfCreditAmount", transaction.CreatedDate);
                SqlParameter BalanceAmt = new SqlParameter("BalanceAmt", transaction.Amount);
                SqlParameter MaturityDate = new SqlParameter("MaturityDate", (object)MaturityDate1 ?? DBNull.Value );
                //SqlParameter TransactionType = new SqlParameter("TransactionType", transaction.TransactionType);
                //SqlParameter CheckNumber = new SqlParameter("CheckNumber", (object)transaction.CheckNumber ?? DBNull.Value);
                //SqlParameter ChequeDate = new SqlParameter("ChequeDate", (object)transaction.ChequeDate ?? DBNull.Value);
                //SqlParameter BounceReason = new SqlParameter("BounceReason", (object)transaction.BounceReason ?? DBNull.Value);
                //SqlParameter Penalty = new SqlParameter("Penalty", (object)transaction.Penalty ?? DBNull.Value);
                //SqlParameter ChequeClearDate = new SqlParameter("ChequeClearDate", (object)transaction.ChequeClearDate ?? DBNull.Value);
                //SqlParameter Status = new SqlParameter("Status", (object)transaction.Status ?? DBNull.Value);
                //SqlParameter TransactionTime = new SqlParameter("TransactionTime", (object)transaction.TransactionTime ?? DBNull.Value);
                //SqlParameter BankName = new SqlParameter("BankName", (object)transaction.BankName ?? DBNull.Value);
                //SqlParameter BranchId = new SqlParameter("BranchId", transaction.BranchId);
                //SqlParameter DescIndentify = new SqlParameter("DescIndentify", (object)transaction.DescIndentify ?? DBNull.Value);
                //SqlParameter RefCustomerProductId = new SqlParameter("RefCustomerProductId", (object)transaction.RefCustomerProductId ?? DBNull.Value);

                //SqlParameter NEFTNumber = new SqlParameter("NEFTNumber", (object)transaction.NEFTNumber ?? DBNull.Value);
                //SqlParameter NEFTDate = new SqlParameter("NEFTDate", (object)transaction.NEFTDate ?? DBNull.Value);

                //SqlParameter Id = new SqlParameter
                //{

                //    ParameterName = "@Id",
                //    SqlDbType = System.Data.SqlDbType.UniqueIdentifier,
                //    Size = 50,
                //    Direction = System.Data.ParameterDirection.Output
                //};
                db.Database.CommandTimeout = 3600;
                var trans = db.Database.SqlQuery<object>("SaveWealthCreatorTransaction @CustomerProductId,@Type, @DateOfCreditAmount ,@BalanceAmt, @MaturityDate, @CreatedDate , @CreatedBy", CustomerProductId, Type,DateOfCreditAmount,BalanceAmt,MaturityDate, CreatedDate, CreatedBy).FirstOrDefault();
            }
            return true;
        }
        public bool SaveTransaction(Transactions transaction)
        {
            using (var db = new BSCCSLEntity())
            {
                if (transaction.TransactionId == Guid.Empty)
                {
                    transaction.CreatedBy = transaction.CreatedBy;
                    if (transaction.Type == TypeCRDR.DR)
                    {
                        decimal balance = db.CustomerProduct.Where(x => x.CustomerProductId == transaction.CustomerProductId).Select(a => a.Balance.Value).FirstOrDefault();

                        if (balance < transaction.Amount)
                        {
                            return false;
                        }
                    }
                    transaction.TransactionId = InsertTransctionUsingSP(transaction);


                    #region Add the Description
                    Transactions objTransactions = db.Transaction.Where(x => x.TransactionId == transaction.TransactionId).FirstOrDefault();
                    if (objTransactions != null)
                    {
                        if (!string.IsNullOrEmpty(transaction.Description))
                        {
                            if (!string.IsNullOrEmpty(objTransactions.Description))
                                objTransactions.Description = objTransactions.Description + ". " + transaction.Description;
                            else
                                objTransactions.Description = transaction.Description;

                            db.SaveChanges();
                        }
                    }
                    #endregion
                }
                return true;
            }
        }

        public decimal UpdateTransactionData(Transactions Transactiondata, User user)
        {

            using (var db = new BSCCSLEntity())
            {
                using (var tr = new TransactionScope())
                {
                    try
                    {
                        var finalbalance = 0.0M;
                        var transctiondetails = db.Transaction.Where(x => x.TransactionId == Transactiondata.TransactionId).FirstOrDefault();
                        int TransactionType = Convert.ToInt32(transctiondetails.Type);
                        if (TransactionType == 1)
                        {
                            finalbalance = transctiondetails.Balance;
                        }
                        if (transctiondetails != null)
                        {
                            transctiondetails.Balance = finalbalance;
                            transctiondetails.ChequeClearDate = Transactiondata.ChequeClearDate;
                            transctiondetails.Status = 0;
                            transctiondetails.ModifiedBy = user.UserId;
                            db.Entry(transctiondetails).State = EntityState.Modified;
                            db.SaveChanges();

                            CustomerProduct objCustomerProduct = db.CustomerProduct.FirstOrDefault(a => a.CustomerProductId == transctiondetails.CustomerProductId);
                            if (objCustomerProduct != null)
                            {
                                if (objCustomerProduct.ProductType == ProductType.Saving_Account)
                                    SMSService.SendTransactionSMS(transctiondetails.TransactionId);

                            }

                            //Change By Akhilesh... To Send only Saving Account Transactions SMS
                            //SMSService.SendTransactionSMS(transctiondetails.TransactionId);

                            //CustomerProduct customerProduct = db.CustomerProduct.Where(cp => cp.CustomerProductId == transctiondetails.CustomerProductId).FirstOrDefault();
                            //customerProduct.Balance = customerProduct.Balance + transctiondetails.Amount;

                            decimal bal = db.CustomerProduct.Where(x => x.CustomerProductId == transctiondetails.CustomerProductId).Select(a => a.Balance.Value).FirstOrDefault();
                            decimal balance = bal + transctiondetails.Amount;

                            //var customerprod = new CustomerProduct() { CustomerProductId = transctiondetails.CustomerProductId.Value, Balance = balance };
                            //db.CustomerProduct.Attach(customerprod);
                            //db.Entry(customerprod).Property(x => x.Balance).IsModified = true;
                            //db.SaveChanges();


                            CustomerProduct objcp = db.CustomerProduct.Where(a => a.CustomerProductId == transctiondetails.CustomerProductId.Value).FirstOrDefault();
                            if (objcp != null)
                            {
                                objcp.Balance = balance;
                                db.SaveChanges();
                            }

                            //customerProduct.UpdatedBalance = transctiondetails.Balance;

                            var DepositorAccNo = db.CustomerProduct.Where(x => x.AccountNumber == Transactiondata.TempThirdPartyAccNo).FirstOrDefault();
                            if (DepositorAccNo != null)
                            {
                                if (DepositorAccNo.Balance >= transctiondetails.Amount)
                                {
                                    //var latesttransactionbalance = db.CustomerProduct.Where(x => x.CustomerProductId == DepositorAccNo.CustomerProductId).Select(x => x.UpdatedBalance).FirstOrDefault();

                                    Transactions DepositorTransaction = new Transactions();
                                    DepositorTransaction.Amount = transctiondetails.Amount;
                                    //DepositorTransaction.Balance = Convert.ToDecimal(latesttransactionbalance - transctiondetails.Amount);
                                    DepositorTransaction.CustomerId = DepositorAccNo.CustomerId;
                                    DepositorTransaction.CustomerProductId = DepositorAccNo.CustomerProductId;
                                    DepositorTransaction.TransactionType = transctiondetails.TransactionType;
                                    DepositorTransaction.CheckNumber = transctiondetails.CheckNumber;
                                    DepositorTransaction.ChequeClearDate = transctiondetails.ChequeClearDate;
                                    DepositorTransaction.ChequeDate = transctiondetails.ChequeDate;
                                    DepositorTransaction.BankName = transctiondetails.BankName;
                                    DepositorTransaction.Status = 0;
                                    DepositorTransaction.Type = BSCCSL.Models.TypeCRDR.DR;
                                    DepositorTransaction.TransactionTime = DateTime.Now;
                                    DepositorTransaction.CreatedBy = transctiondetails.CreatedBy;
                                    DepositorTransaction.CreatedDate = DateTime.Now;
                                    DepositorTransaction.BranchId = Transactiondata.BranchId;
                                    //db.Entry(DepositorTransaction).State = EntityState.Added;
                                    Guid transactionId = InsertTransctionUsingSP(DepositorTransaction);
                                    //smsService.SendTransactionSMS(transactionId);
                                    //DepositorAccNo.Balance = DepositorAccNo.Balance - transctiondetails.Amount;
                                    //DepositorAccNo.UpdatedBalance = DepositorTransaction.Balance;
                                    //DepositorAccNo.ModifyBy = user.UserId;
                                    //db.Entry(DepositorAccNo).State = EntityState.Modified;
                                }
                                else
                                {
                                    return 0;
                                }
                            }
                            db.SaveChanges();

                        }
                        tr.Complete();
                        return finalbalance;
                    }
                    catch (Exception ex)
                    {
                        ErrorLogService.InsertLog(ex);
                    }
                    return 0;
                }

            }
        }

        public decimal UpdateTransactionDataForBounce(Transactions BounceDetails)
        {
            using (var db = new BSCCSLEntity())
            {
                try
                {
                    using (var tr = new TransactionScope())
                    {

                        var transctiondetails = db.Transaction.Where(x => x.TransactionId == BounceDetails.TransactionId).FirstOrDefault();
                        int penlty = Convert.ToInt32(BounceDetails.Penalty) != 0 ? Convert.ToInt32(BounceDetails.Penalty) : 0;

                        var CustomerProductData = db.CustomerProduct.Where(x => x.CustomerProductId == transctiondetails.CustomerProductId).FirstOrDefault();
                        CustomerProductData.Balance = CustomerProductData.Balance + transctiondetails.Amount;
                        db.CustomerProduct.Attach(CustomerProductData);
                        var entry = db.Entry(CustomerProductData);
                        entry.Property(e => e.Balance).IsModified = true;
                        db.Entry(CustomerProductData).State = EntityState.Modified;
                        db.SaveChanges();
                        if (transctiondetails != null)
                        {
                            if (transctiondetails.Type == TypeCRDR.CR)
                            {
                                transctiondetails.TransactionId = transctiondetails.TransactionId;
                                transctiondetails.Penalty = penlty;
                                transctiondetails.BounceReason = BounceDetails.BounceReason;
                                transctiondetails.Status = 0;
                                transctiondetails.ChequeClearDate = BounceDetails.ChequeClearDate;
                                //db.Entry(transctiondetails).State = EntityState.Modified;
                                db.SaveChanges();
                                SMSService.SendBounceSMS(transctiondetails.TransactionId);

                                Transactions transactionBounce = new Transactions();
                                transactionBounce.Amount = transctiondetails.Amount;
                                transactionBounce.CustomerId = transctiondetails.CustomerId;
                                transactionBounce.CustomerProductId = transctiondetails.CustomerProductId;
                                transactionBounce.Status = Status.Clear;
                                transactionBounce.TransactionTime = DateTime.Now;
                                transactionBounce.ChequeDate = transctiondetails.ChequeDate;
                                transactionBounce.CheckNumber = transctiondetails.CheckNumber;
                                transactionBounce.ChequeClearDate = transctiondetails.ChequeClearDate;
                                transactionBounce.BankName = transctiondetails.BankName;
                                transactionBounce.CreatedBy = transctiondetails.CreatedBy;
                                transactionBounce.CreatedDate = transctiondetails.CreatedDate;
                                transactionBounce.Type = TypeCRDR.DR;
                                transactionBounce.TransactionType = Models.TransactionType.Cheque;
                                //transactionBounce.Balance = Deposite_Subracted_balance;
                                transactionBounce.BranchId = BounceDetails.BranchId;
                                transactionBounce.DescIndentify = DescIndentify.Cheque_Bounce;
                                transactionBounce.TransactionId = InsertTransctionUsingSP(transactionBounce);
                                if (BounceDetails.Penalty != 0)
                                {

                                    if (transctiondetails != null)
                                    {
                                        Transactions transactionPanelty = new Transactions();
                                        transactionPanelty.Status = 0;
                                        transactionPanelty.Penalty = penlty;
                                        transactionPanelty.BounceReason = BounceDetails.BounceReason;
                                        //transactionPanelty.Balance = PenaltyAddedBalance;
                                        transactionPanelty.Amount = penlty;
                                        transactionPanelty.CustomerId = transctiondetails.CustomerId;
                                        transactionPanelty.CustomerProductId = transctiondetails.CustomerProductId;
                                        transactionPanelty.Type = TypeCRDR.DR;
                                        transactionPanelty.DescIndentify = DescIndentify.Panelty;
                                        transactionPanelty.TransactionTime = DateTime.Now.AddSeconds(5);
                                        transactionPanelty.ChequeDate = transctiondetails.ChequeDate;
                                        transactionPanelty.CheckNumber = transctiondetails.CheckNumber;
                                        transactionPanelty.ChequeClearDate = transctiondetails.ChequeClearDate;
                                        transactionPanelty.BankName = transctiondetails.BankName;
                                        transactionPanelty.CreatedBy = transctiondetails.CreatedBy;
                                        transactionPanelty.CreatedDate = transctiondetails.CreatedDate;
                                        transactionPanelty.BranchId = BounceDetails.BranchId;
                                        transactionPanelty.TransactionType = Models.TransactionType.BankTransfer;
                                        transactionPanelty.DescIndentify = DescIndentify.Panelty;
                                        transactionPanelty.TransactionId = InsertTransctionUsingSP(transactionPanelty);

                                        SMSService.SendPaneltySMS(transactionPanelty.TransactionId);

                                    }

                                    var CustomerProduct = db.CustomerProduct.Where(x => x.AccountNumber == BounceDetails.TempThirdPartyAccNo && (x.ProductType == ProductType.Saving_Account || x.ProductType == ProductType.Current_Account)).FirstOrDefault();
                                    if (CustomerProduct != null)
                                    {

                                        Transactions DepositorTransaction = new Transactions();
                                        DepositorTransaction.Amount = penlty;
                                        //DepositorTransaction.Balance = Convert.ToDecimal(LastBalance - penlty);
                                        DepositorTransaction.CustomerId = CustomerProduct.CustomerId;
                                        DepositorTransaction.CustomerProductId = CustomerProduct.CustomerProductId;
                                        DepositorTransaction.TransactionType = Models.TransactionType.BankTransfer;
                                        DepositorTransaction.CheckNumber = transctiondetails.CheckNumber;
                                        DepositorTransaction.ChequeClearDate = transctiondetails.ChequeClearDate;

                                        DepositorTransaction.ChequeDate = transctiondetails.ChequeDate;
                                        DepositorTransaction.BankName = transctiondetails.BankName;
                                        DepositorTransaction.Status = 0;
                                        DepositorTransaction.Type = TypeCRDR.DR;
                                        DepositorTransaction.TransactionTime = DateTime.Now.AddSeconds(10);
                                        DepositorTransaction.CreatedBy = transctiondetails.CreatedBy;
                                        DepositorTransaction.BounceReason = transctiondetails.BounceReason;
                                        DepositorTransaction.Penalty = transctiondetails.Penalty;
                                        DepositorTransaction.CreatedDate = DateTime.Now;
                                        DepositorTransaction.BranchId = BounceDetails.BranchId;
                                        DepositorTransaction.DescIndentify = DescIndentify.Panelty;
                                        DepositorTransaction.TransactionId = InsertTransctionUsingSP(DepositorTransaction);
                                        SMSService.SendPaneltySMS(DepositorTransaction.TransactionId);

                                    }
                                }
                                tr.Complete();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogService.InsertLog(ex);
                }
                return 0;
            }
        }

        public object GetUnclearChequeList(DataTableSearch search)
        {
            using (var db = new BSCCSLEntity())
            {

                var query = (from ut in db.Customer.Where(s => s.IsDelete == false).AsEnumerable()
                             join pd in db.CustomerPersonalDetail.AsEnumerable() on ut.CustomerId equals pd.CustomerId
                             join co in db.CustomerProduct.AsEnumerable() on ut.CustomerId equals co.CustomerId
                             join cp in db.Transaction.Where(t => t.TransactionType == TransactionType.Cheque && t.Status == Status.Unclear && t.BranchId == search.BranchId).AsEnumerable() on co.CustomerProductId equals cp.CustomerProductId
                             group new { co, cp, pd } by new { ut.CustomerId, co, cp } into g
                             select new UnclearCheck
                             {
                                 CustomerId = g.Key.CustomerId,
                                 CustomerProdctId = g.Key.co.CustomerProductId,
                                 Name = g.Select(x => x.pd.FirstName + " " + x.pd.MiddleName + " " + x.pd.LastName).ToList(),
                                 //Name = string.Join(", ", g.Select(x => x.pd.FirstName + " " + x.pd.MiddleName + " " + x.pd.LastName)),
                                 AccountNo = g.Key.co.AccountNumber,
                                 ProductTypeName = g.Key.co.ProductType.ToString().Replace("_", " "),
                                 Balance = g.Key.co.Balance,
                                 TransactionDate = g.Key.cp.TransactionTime,
                                 ChequeAmount = g.Key.cp.Amount,
                                 ChequeNo = g.Key.cp.CheckNumber,
                                 ChequeDate = g.Key.cp.ChequeDate,
                                 BankName = g.Key.cp.BankName,
                                 TransactionId = g.Key.cp.TransactionId,
                             }).AsQueryable();


                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    query = query.Where(c => c.AccountNo.Contains(search.sSearch.Trim()) || c.Name.Contains(search.sSearch.ToLower().Trim()));
                }
                var unclearchque = query.OrderBy(c => c.AccountNo).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = unclearchque.Count(),
                    iTotalDisplayRecords = query.Count(),
                    aaData = unclearchque
                };
                return data;
            }
        }

        public object GetClearCheckList(DataTableSearch search)
        {
            using (var db = new BSCCSLEntity())
            {

                var query = (from ut in db.Customer.Where(s => s.IsDelete == false).AsEnumerable()
                             join pd in db.CustomerPersonalDetail.AsEnumerable() on ut.CustomerId equals pd.CustomerId
                             join co in db.CustomerProduct.AsEnumerable() on ut.CustomerId equals co.CustomerId
                             join cp in db.Transaction.Where(t => t.TransactionType == TransactionType.Cheque && t.Status == Status.Clear && t.BranchId == search.BranchId).AsEnumerable() on co.CustomerProductId equals cp.CustomerProductId
                             group new { co, cp, pd } by new { ut.CustomerId, co, cp } into g
                             select new UnclearCheck
                             {
                                 CustomerId = g.Key.CustomerId,
                                 CustomerProdctId = g.Key.co.CustomerProductId,
                                 Name = g.Select(x => x.pd.FirstName + " " + x.pd.MiddleName + " " + x.pd.LastName).ToList(),
                                 AccountNo = g.Key.co.AccountNumber,
                                 ProductTypeName = g.Key.co.ProductType.ToString().Replace("_", " "),
                                 Balance = g.Key.cp.Balance,
                                 TransactionDate = g.Key.cp.TransactionTime,
                                 ChequeAmount = g.Key.cp.Amount,
                                 ChequeNo = g.Key.cp.CheckNumber,
                                 ChequeDate = g.Key.cp.ChequeDate,
                                 BankName = g.Key.cp.BankName,
                                 TransactionId = g.Key.cp.TransactionId,
                                 ChequeClearDate = g.Key.cp.ChequeClearDate
                             }).AsQueryable();


                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    query = query.Where(c => c.AccountNo.Contains(search.sSearch.Trim()) || c.Name.Contains(search.sSearch.ToLower().Trim()));
                }
                var clearcheque = query.OrderBy(c => c.AccountNo).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = clearcheque.Count(),
                    iTotalDisplayRecords = query.Count(),
                    aaData = clearcheque
                };
                return data;
            }
        }

        public object GetTransactionRptData(DataTableSearch Search)
        {
            using (var db = new BSCCSLEntity())
            {
                //var CustomerProductdData = db.CustomerProduct.Where(s => s.AccountNumber == Search.AccountNumber).FirstOrDefault().CustomerProductId;
                //var List = db.Transaction.Where(x => x.CustomerProductId == CustomerProductdData).AsQueryable();
                var TransactionData = (from cp in db.CustomerProduct.Where(s => s.AccountNumber == Search.AccountNumber)
                                       join t in db.Transaction on cp.CustomerProductId equals t.CustomerProductId
                                       select t).AsQueryable();

                if (Search.fromDate != null)
                {
                    TransactionData = TransactionData.Where(c => DbFunctions.TruncateTime(c.TransactionTime) >= Search.fromDate);
                }

                if (Search.toDate != null)
                {
                    TransactionData = TransactionData.Where(c => DbFunctions.TruncateTime(c.TransactionTime) <= Search.toDate);
                }

                var TransactionList = TransactionData.OrderBy(a => a.TransactionTime).Skip(Search.iDisplayStart).Take(Search.iDisplayLength).ToList();
                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = TransactionList.Count(),
                    iTotalDisplayRecords = TransactionData.Count(),
                    aaData = TransactionList
                };
                return data;
            }
        }

        public object PrintPassbookData(PassbookPrintSearch search)
        {

            using (var db = new BSCCSLEntity())
            {
                var TransactionData = (from cp in db.CustomerProduct.Where(a => a.AccountNumber == search.AccountNo && (a.ProductType == ProductType.Saving_Account || a.ProductType == ProductType.Current_Account || a.ProductType == ProductType.Recurring_Deposit))
                                       join c in db.Customer.Where(a => a.IsDelete == false) on cp.CustomerId equals c.CustomerId
                                       join t in db.Transaction on cp.CustomerProductId equals t.CustomerProductId
                                       select new
                                       {
                                           t.TransactionTime,
                                           t.Amount,
                                           t.CheckNumber,
                                           t.NEFTNumber,
                                           t.Balance,
                                           t.Type,
                                           t.Description
                                       }).AsQueryable();

                var passbookprint = db.PassbookPrint.Where(a => a.AccountNo == search.AccountNo).OrderByDescending(a => a.PassbookPrintDate).FirstOrDefault();

                //if (passbookprint != null)
                //{
                //    TransactionData = TransactionData.Where(a => a.TransactionTime >= passbookprint.PassbookPrintDate).AsQueryable();
                //}
                if (search.FromDate != null)
                {
                    TransactionData = TransactionData.Where(a => a.TransactionTime >= search.FromDate);
                }

                if (search.ToDate != null)
                {
                    TransactionData = TransactionData.Where(a => a.TransactionTime <= search.ToDate);
                }

                var data = new
                {
                    TransactionData = TransactionData.OrderBy(a => a.TransactionTime).ToList(),
                    //Passbookprint = passbookprint,
                };
                return data;
            }
        }

        public bool SavePrintPassbook(PassbookPrint printpassbook)
        {
            using (var db = new BSCCSLEntity())
            {
                db.PassbookPrint.Add(printpassbook);
                db.SaveChanges();
                return true;
            }
        }

        public object PrintAccountDetailOnPassbook(string AccNo)
        {
            using (var db = new BSCCSLEntity())
            {
                var query = (from c in db.CustomerProduct.Where(a => a.AccountNumber == AccNo)
                             join ut in db.Customer.Where(s => s.IsDelete == false) on c.CustomerId equals ut.CustomerId
                             join co in db.Branch on ut.BranchId equals co.BranchId
                             join cp in db.CustomerPersonalDetail on ut.CustomerId equals cp.CustomerId
                             join ca in db.CustomerAddress on cp.PersonalDetailId equals ca.PersonalDetailId
                             join n in db.Nominee on cp.CustomerId equals n.CustomerId into nom
                             from n in nom.DefaultIfEmpty()
                             select new
                             {
                                 ut.CustomerId,
                                 co.BranchId,
                                 co.BranchCode,
                                 co.BranchName,
                                 ut.ClienId,
                                 cp.FirstName,
                                 cp.MiddleName,
                                 cp.LastName,
                                 ca.Area,
                                 ca.BuildingName,
                                 ca.City,
                                 ca.District,
                                 ca.DoorNo,
                                 ca.Email,
                                 ca.Landmark,
                                 ca.MobileNo,
                                 ca.State,
                                 ca.TelephoneNo,
                                 ca.Place,
                                 ca.Pincode,
                                 ca.PlotNo_Street,
                                 n.Name,
                                 n.AppointeeName,
                                 c.OpeningDate,
                                 c.AccountNumber,
                                 c.InterestRate
                             }).AsQueryable();

                var res = (from q in query.AsEnumerable()
                           group q by new { q.CustomerId, q.BranchCode, q.BranchName, q.ClienId, q.AccountNumber, q.OpeningDate, q.InterestRate } into g
                           select new
                           {
                               CustomerId = g.Key.CustomerId,
                               BranchCode = g.Key.BranchCode,
                               ClientId = g.Key.ClienId,
                               BranchName = g.Key.BranchName,
                               CustomerName = string.Join(", ", g.Select(tg => tg.FirstName + " " + tg.MiddleName + " " + tg.LastName)),
                               Address = g.Select(a => (!string.IsNullOrEmpty(a.DoorNo) ? a.DoorNo + "," : "")).FirstOrDefault() + g.Select(a => (!string.IsNullOrEmpty(a.BuildingName) ? a.BuildingName + ", " : "")).FirstOrDefault() + g.Select(a => (!string.IsNullOrEmpty(a.PlotNo_Street) ? a.PlotNo_Street + ", " : "")).FirstOrDefault() + g.Select(a => (!string.IsNullOrEmpty(a.Landmark) ? a.Landmark + ", " : "")).FirstOrDefault() + g.Select(a => (!string.IsNullOrEmpty(a.Area) ? a.Area + ", " : "")).FirstOrDefault() + g.Select(a => (!string.IsNullOrEmpty(a.City) ? a.City + ", " : "")).FirstOrDefault() + g.Select(a => (!string.IsNullOrEmpty(a.District) ? a.District + ", " : "")).FirstOrDefault() + g.Select(a => (!string.IsNullOrEmpty(a.State) ? a.State + ", " : "")).FirstOrDefault() + g.Select(a => (!string.IsNullOrEmpty(a.Pincode) ? a.Pincode : "")).FirstOrDefault(),
                               MobileNo = string.Join(", ", g.Select(tg => tg.MobileNo)),
                               Nominee = g.Select(a => (!string.IsNullOrEmpty(a.Name) ? a.Name : "")).FirstOrDefault(),
                               OpeningDate = g.Key.OpeningDate,
                               AccountNo = g.Key.AccountNumber,
                               InterestRate = g.Key.InterestRate,
                           }).FirstOrDefault();

                return res;
            }
        }

        public decimal UpdateRDPaymentStatus(RDPendingPayment rdPendingPayment, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                CustomerProduct customerProductnew = db.CustomerProduct.Where(a => a.CustomerProductId == rdPendingPayment.transaction.CustomerProductId).FirstOrDefault();
                Transactions transaction = new Transactions();
                transaction = rdPendingPayment.transaction;

                Guid CustomerProductId = rdPendingPayment.rdPaymentList[0].CustomerProductId;
                CustomerProduct customerProduct = db.CustomerProduct.Where(a => a.CustomerProductId == CustomerProductId).FirstOrDefault();

                if (customerProductnew.Balance >= rdPendingPayment.rdPaymentList.Sum(a => a.Amount))
                {

                    DateTime NextInstallmentdate = rdPendingPayment.rdPaymentList.Max(b => b.NextDate.Value);

                    if (!transaction.TransactionTime.HasValue)
                    {
                        transaction.TransactionTime = DateTime.Now.Date;
                    }

                    if (customerProduct.ProductType == ProductType.Recurring_Deposit && customerProduct.PaymentType != Frequency.Daily)
                    {
                        CalculateMaturityAmount calculateMaturityAmount = new CalculateMaturityAmount();
                        calculateMaturityAmount.Amount = customerProduct.Amount;
                        calculateMaturityAmount.InterestRate = customerProduct.InterestRate;
                        calculateMaturityAmount.DueDate = customerProduct.DueDate;
                        calculateMaturityAmount.MaturityDate = NextInstallmentdate;
                        calculateMaturityAmount.NoOfMonthsORYears = customerProduct.NoOfMonthsORYears;
                        calculateMaturityAmount.OpeningDate = customerProduct.OpeningDate;
                        calculateMaturityAmount.ProductType = customerProduct.ProductType;
                        calculateMaturityAmount.PaymentType = customerProduct.PaymentType;
                        calculateMaturityAmount.TimePeriod = customerProduct.TimePeriod;
                        calculateMaturityAmount.OpeningBalance = customerProduct.OpeningBalance;
                        calculateMaturityAmount.InterestType = db.Product.Where(a => a.ProductId == customerProduct.ProductId).FirstOrDefault().Frequency;

                        decimal maturity = customerProductService.CalculateRDMaturity(calculateMaturityAmount);

                        // decimal TotalInstallmentPaid = db.RDPayment.Where(a => a.CustomerProductId == CustomerProductId && a.RDPaymentType == RDPaymentType.Installment && a.CreatedDate < NextInstallmentdate).Sum(a => a.Amount);

                        decimal TotalInterstTillDate = db.DailyInterest.Where(a => a.CustomerProductId == CustomerProductId && a.CreatedDate < NextInstallmentdate && a.IsPaid == false).Select(a => a.TodaysInterest).DefaultIfEmpty(0).Sum();
                        //if (TotalInterstTillDate == null)
                        //    TotalInterstTillDate = 0;
                        //decimal total = TotalInstallmentPaid + TotalInterstTillDate;

                        decimal TotalInstallmenttobePaid = rdPendingPayment.rdPaymentList.Sum(a => a.Amount);

                        decimal total = customerProduct.Balance.Value + TotalInterstTillDate + TotalInstallmenttobePaid;

                        decimal interesttoCredit = maturity - total;

                        //Remove this code due to Interest Amount wrongly Paid
                        //DailyInterest dailyInterest = new DailyInterest();
                        //dailyInterest.TodaysInterest = interesttoCredit;
                        //dailyInterest.CreatedDate = DateTime.Now;
                        //dailyInterest.CustomerProductId = CustomerProductId;
                        //dailyInterest.IsPaid = false;
                        //dailyInterest.InterestRate = customerProduct.InterestRate;
                        //db.Entry(dailyInterest).State = EntityState.Added;
                        //db.SaveChanges();
                    }

                    decimal updatedBalance = customerProduct.UpdatedBalance.Value;

                    foreach (var installment in rdPendingPayment.rdPaymentList.OrderBy(a => a.CreatedDate))
                    {
                        decimal comm = 0;
                        if (customerProduct.ProductType == ProductType.Recurring_Deposit || customerProduct.ProductType == ProductType.Regular_Income_Planner || customerProduct.ProductType == ProductType.Three_Year_Product)
                        {
                            decimal totalyear = customerProduct.TotalDays.Value / 365;

                            totalyear = Math.Ceiling(totalyear);

                            if (totalyear == 0)
                            {
                                totalyear = 1;
                            }

                            //var commissionyear = installment.CreatedDate.Year * 10000 + installment.CreatedDate.Month * 100 +  - customerProduct.OpeningDate.Month

                            double diff = (installment.CreatedDate - customerProduct.OpeningDate).TotalDays;

                            double yeartillnow = Math.Ceiling(diff / 365);
                            if (yeartillnow == 0)
                            {
                                yeartillnow = 1;
                            }

                            Guid RankId = db.User.Where(s => s.UserId == customerProduct.AgentId).Select(s => s.RankId.Value).FirstOrDefault();
                            List<ProductCommission> commissiondata = db.ProductCommission.Where(s => s.ProductId == customerProduct.ProductId && s.RankId == RankId && s.ProductYear == totalyear).ToList();

                            foreach (var com in commissiondata)
                            {
                                if (yeartillnow == com.CommissionYear || com.IsAbove == true)
                                {
                                    comm = (customerProduct.Amount * com.Commission) / 100;
                                }
                            }

                            //if (commissiondata != null)
                            //{
                            //    comm = (customerProduct.Amount * commissiondata.Commission) / 100;
                            //}

                            ProductAgentCommission productAgentCommission = new ProductAgentCommission();
                            productAgentCommission.RankId = RankId;
                            productAgentCommission.CustomerProductId = customerProduct.CustomerProductId;
                            productAgentCommission.ProductId = customerProduct.ProductId;
                            productAgentCommission.RDPaymentId = installment.RDPaymentId;
                            productAgentCommission.AgentId = customerProduct.AgentId.Value;
                            productAgentCommission.OpeningDate = customerProduct.OpeningDate;
                            productAgentCommission.Amount = customerProduct.Amount;
                            productAgentCommission.TotalDays = customerProduct.TotalDays.Value;
                            productAgentCommission.Date = installment.CreatedDate;
                            customerProductService.ProductAgentCommission(productAgentCommission);
                        }

                        var rdPayment = new RDPayment() { RDPaymentId = installment.RDPaymentId, IsPaid = true, AgentCommission = comm, PaidDate = transaction.TransactionTime };
                        db.RDPayment.Attach(rdPayment);
                        db.Entry(rdPayment).Property(x => x.IsPaid).IsModified = true;
                        db.Entry(rdPayment).Property(x => x.AgentCommission).IsModified = true;
                        db.Entry(rdPayment).Property(x => x.PaidDate).IsModified = true;
                        db.SaveChanges();

                        //transaction.TransactionTime = transaction.TransactionTime.Value.AddSeconds(5);

                        //customerProductnew.Balance = customerProductnew.Balance - installment.Amount;
                        //customerProductnew.UpdatedBalance = customerProductnew.UpdatedBalance - installment.Amount;
                        Transactions transactionSaving = new Transactions();
                        transactionSaving.Amount = installment.Amount;
                        //transactionSaving.Balance = customerProductnew.UpdatedBalance.Value;
                        transactionSaving.TransactionType = transaction.TransactionType;
                        transactionSaving.Type = transaction.Type;
                        transactionSaving.BankName = transaction.BankName;
                        transactionSaving.CheckNumber = transaction.CheckNumber;
                        transactionSaving.ChequeClearDate = transaction.ChequeClearDate;
                        transactionSaving.ChequeDate = transaction.ChequeDate;
                        transactionSaving.CustomerProductId = customerProductnew.CustomerProductId;
                        transactionSaving.BranchId = transaction.BranchId;
                        transactionSaving.CreatedBy = transaction.CreatedBy;
                        transactionSaving.CreatedDate = transaction.CreatedDate;
                        transactionSaving.TransactionTime = transaction.TransactionTime;
                        transactionSaving.DescIndentify = DescIndentify.Maturity;
                        transactionSaving.CustomerId = customerProduct.CustomerId;
                        transactionSaving.RefCustomerProductId = CustomerProductId;
                        transactionSaving.TransactionId = InsertTransctionUsingSP(transactionSaving);


                        //transaction.TransactionTime = transaction.TransactionTime.Value.AddSeconds(5);

                        updatedBalance = updatedBalance + installment.Amount;
                        //customerProduct.Balance = customerProduct.Balance + installment.Amount;

                        Transactions transactionRD = new Transactions();
                        transactionRD.Amount = installment.Amount;
                        //transactionRD.Balance = updatedBalance;
                        transactionRD.TransactionType = transaction.TransactionType;
                        transactionRD.Type = TypeCRDR.CR;
                        transactionRD.BankName = transaction.BankName;
                        transactionRD.CheckNumber = transaction.CheckNumber;
                        transactionRD.ChequeClearDate = transaction.ChequeClearDate;
                        transactionRD.ChequeDate = transaction.ChequeDate;
                        transactionRD.CustomerProductId = CustomerProductId;
                        transactionRD.BranchId = transaction.BranchId;
                        transactionRD.CreatedBy = transaction.CreatedBy;
                        transactionRD.CreatedDate = transaction.CreatedDate;
                        transactionRD.TransactionTime = transaction.TransactionTime;
                        if (customerProduct.ProductType == ProductType.Recurring_Deposit || customerProduct.ProductType == ProductType.Regular_Income_Planner)
                        {
                            transactionRD.DescIndentify = DescIndentify.Maturity;
                        }
                        else
                        {
                            transactionRD.DescIndentify = DescIndentify.Installment;
                        }
                        transactionRD.CustomerId = customerProduct.CustomerId;
                        transactionRD.RefCustomerProductId = customerProductnew.CustomerProductId;
                        transactionRD.TransactionId = InsertTransctionUsingSP(transactionRD);

                        //transaction.TransactionTime = transaction.TransactionTime.Value.AddSeconds(5);

                        if (installment.LatePaymentCharges > 0)
                        {
                            Transactions transactionLatePayment = new Transactions();
                            //customerProductnew.Balance = customerProductnew.Balance - installment.LatePaymentCharges;
                            //customerProductnew.UpdatedBalance = customerProductnew.UpdatedBalance - installment.LatePaymentCharges;

                            transactionLatePayment.Amount = installment.LatePaymentCharges;
                            //transactionLatePayment.Balance = customerProductnew.UpdatedBalance.Value;
                            transactionLatePayment.TransactionType = transaction.TransactionType;
                            transactionLatePayment.Type = transaction.Type;
                            transactionLatePayment.BankName = transaction.BankName;
                            transactionLatePayment.CheckNumber = transaction.CheckNumber;
                            transactionLatePayment.ChequeClearDate = transaction.ChequeClearDate;
                            transactionLatePayment.ChequeDate = transaction.ChequeDate;
                            transactionLatePayment.CustomerProductId = customerProductnew.CustomerProductId;
                            transactionLatePayment.BranchId = transaction.BranchId;
                            transactionLatePayment.CreatedBy = transaction.CreatedBy;
                            transactionLatePayment.CreatedDate = transaction.CreatedDate;
                            transactionLatePayment.CustomerId = customerProduct.CustomerId;
                            transactionLatePayment.TransactionTime = transaction.TransactionTime;
                            transactionLatePayment.DescIndentify = DescIndentify.LatePaymentCharges;
                            transactionLatePayment.RefCustomerProductId = CustomerProductId;
                            transactionLatePayment.TransactionId = InsertTransctionUsingSP(transactionLatePayment);
                        }

                        db.RDPayment.Where(a => a.CreatedDate >= installment.CreatedDate && a.CreatedDate < installment.NextDate && a.CustomerProductId == CustomerProductId && a.RDPaymentType == RDPaymentType.LatePaymentCharges).ToList().ForEach(u => { u.IsPaid = true; u.PaidDate = transaction.TransactionTime; });

                        //customerProduct.UpdatedBalance = updatedBalance;
                        //db.CustomerProduct.Attach(customerProduct);
                        //db.Entry(customerProduct).Property(x => x.Balance).IsModified = true;
                        //db.Entry(customerProduct).Property(x => x.UpdatedBalance).IsModified = true;
                        //db.SaveChanges();
                    }

                    if (customerProduct.ProductType == ProductType.Loan)
                    {
                        Loan loan = db.Loan.Where(a => a.CustomerProductId == customerProduct.CustomerProductId).FirstOrDefault();

                        decimal totalPaid = db.RDPayment.Where(a => a.CustomerProductId == customerProduct.CustomerProductId && a.RDPaymentType == RDPaymentType.Installment && a.IsPaid == true).Sum(a => a.Amount);

                        if (totalPaid >= loan.TotalAmountToPay)
                        {
                            loan.LoanStatus = LoanStatus.Completed;

                            db.Loan.Attach(loan);
                            db.Entry(loan).Property(x => x.LoanStatus).IsModified = true;

                            customerProduct.Status = CustomerProductStatus.Completed;
                            db.CustomerProduct.Attach(customerProduct);
                            db.Entry(customerProduct).Property(x => x.Status).IsModified = true;

                            UpdateLoanStatus updateLoanStatus = new UpdateLoanStatus();
                            updateLoanStatus.LoanId = loan.LoanId;
                            updateLoanStatus.LoanStatus = LoanStatus.Completed;
                            updateLoanStatus.UpdatedBy = user.UserId;
                            updateLoanStatus.UpdatedDate = DateTime.Now;

                            loanService.SaveUpdatedLoanStatus(updateLoanStatus);

                            db.Loan.Attach(loan);
                            db.Entry(loan).Property(x => x.LoanStatus).IsModified = true;

                            LoanCharges loanCharges = db.LoanCharges.Where(a => a.LoanId == loan.LoanId && a.Name.Contains("Share") && a.IsDelete == false).FirstOrDefault();

                            if (loanCharges != null)
                            {
                                CustomerShare customerShare = db.CustomerShare.Where(a => a.CustomerId == loan.CustomerId && a.Maturity == Maturity.Nominal && a.Total == loanCharges.Value && a.IsReverted == null).FirstOrDefault();

                                if (customerShare != null)
                                {
                                    Transactions transactionShare = new Transactions();
                                    transactionShare.Amount = customerShare.Total;
                                    //transactionShare.Balance = customerProductnew.UpdatedBalance.Value + customerShare.Total;
                                    transactionShare.TransactionType = TransactionType.BankTransfer;
                                    transactionShare.Type = TypeCRDR.CR;
                                    transactionShare.CustomerProductId = customerProductnew.CustomerProductId;
                                    transactionShare.BranchId = transaction.BranchId;
                                    transactionShare.CreatedBy = user.UserId;
                                    transactionShare.CreatedDate = DateTime.Now;
                                    transactionShare.TransactionTime = transaction.TransactionTime;
                                    transactionShare.DescIndentify = DescIndentify.Share;
                                    transactionShare.CustomerId = customerProduct.CustomerId;
                                    transactionShare.RefCustomerProductId = loan.CustomerProductId;
                                    transactionShare.TransactionId = InsertTransctionUsingSP(transactionShare);

                                    customerShare.IsReverted = true;

                                    db.CustomerShare.Attach(customerShare);
                                    db.Entry(customerShare).Property(x => x.IsReverted).IsModified = true;

                                    //customerProductnew.Balance = customerProductnew.Balance + customerShare.Total; ;
                                    //customerProductnew.UpdatedBalance = customerProductnew.UpdatedBalance + customerShare.Total;
                                }
                            }
                            db.SaveChanges();
                        }
                    }

                    if (customerProductnew.ProductType == ProductType.Saving_Account)
                    {
                        //if (transaction.Type == TypeCRDR.DR)
                        //{
                        //    db.CustomerProduct.Attach(customerProductnew);
                        //    db.Entry(customerProductnew).Property(x => x.Balance).IsModified = true;
                        //    db.Entry(customerProductnew).Property(x => x.UpdatedBalance).IsModified = true;
                        //    db.SaveChanges();
                        //}

                        decimal savingBalance = db.CustomerProduct.Where(a => a.CustomerProductId == customerProductnew.CustomerProductId).Select(a => a.Balance.Value).FirstOrDefault();

                        return savingBalance;
                    }
                    else
                    {
                        decimal balance = db.CustomerProduct.Where(a => a.CustomerProductId == customerProduct.CustomerProductId).Select(a => a.Balance.Value).FirstOrDefault();
                        return balance;

                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public object TransactionStatement(PrintStatement printStatement)
        {

            using (var db = new BSCCSLEntity())
            {
                var list = (from c in db.CustomerProduct.Where(a => a.AccountNumber == printStatement.AccountNumber)
                            join t in db.Transaction on c.CustomerProductId equals t.CustomerProductId
                            select new
                            {
                                t.Type,
                                t.TransactionTime,
                                t.Balance,
                                t.Amount,
                                t.CheckNumber,
                                t.Description
                            }).AsQueryable();

                if (printStatement.Type != null)
                {
                    list = list.Where(a => a.Type == printStatement.Type);
                }

                if (printStatement.FromDate != null)
                {
                    list = list.Where(a => DbFunctions.TruncateTime(a.TransactionTime) >= DbFunctions.TruncateTime(printStatement.FromDate));
                }
                if (printStatement.ToDate != null)
                {
                    list = list.Where(a => DbFunctions.TruncateTime(a.TransactionTime) <= DbFunctions.TruncateTime(printStatement.ToDate));
                }

                var transactionlist = list.OrderBy(a => a.TransactionTime).ToList();

                dynamic detail = PrintAccountDetailOnPassbook(printStatement.AccountNumber);

                string fileName = detail.AccountNo + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                string path = HttpContext.Current.Server.MapPath("~/PDFStatement/" + fileName);
                //string companyAddress = ConfigurationManager.AppSettings.Get("CompanyAddress");

                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                using (Document doc = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 50f))
                using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                {
                    doc.Open();

                    PdfPTable pdfDetail = new PdfPTable(4);
                    pdfDetail.DefaultCell.Border = Rectangle.NO_BORDER;

                    Image logo = Image.GetInstance(HttpContext.Current.Server.MapPath("~/dist/img/bslogo.png")); //create image with company logo
                    logo.ScalePercent(50f); //scale it  to fit in page

                    PdfPCell pdfCell = new PdfPCell(logo); //create a new table cell with logo image
                    pdfCell.Colspan = 4;
                    pdfCell.BorderWidth = 0; // set border width = 0px
                    pdfCell.HorizontalAlignment = Element.ALIGN_CENTER; //align logo to center :: 0=Left, 1=Centre, 2=Right
                    pdfCell.UseAscender = true;
                    pdfDetail.AddCell(pdfCell); //add cell to table

                    pdfDetail.AddCell("Name");
                    PdfPCell pcellname = new PdfPCell(new Phrase(": " + detail.CustomerName));
                    pcellname.Colspan = 3;
                    pcellname.Border = Rectangle.NO_BORDER;
                    pdfDetail.AddCell(pcellname);
                    pdfDetail.AddCell("Address");
                    PdfPCell pcellAdd = new PdfPCell(new Phrase(": " + detail.Address));
                    pcellAdd.Colspan = 3;
                    pcellAdd.Border = Rectangle.NO_BORDER;
                    pdfDetail.AddCell(pcellAdd);
                    pdfDetail.AddCell("AccountNo");
                    pdfDetail.AddCell(": " + detail.AccountNo);
                    pdfDetail.AddCell("Date");
                    pdfDetail.AddCell(": " + DateTime.Now.Date.ToString("dd/MM/yyyy"));
                    pdfDetail.AddCell("Branch");
                    pdfDetail.AddCell(": " + detail.BranchName);
                    pdfDetail.AddCell("Interest Rate");
                    pdfDetail.AddCell(": " + detail.InterestRate);
                    doc.Add(pdfDetail);

                    PdfPTable pdfTable = new PdfPTable(6);
                    pdfTable.SpacingBefore = 20f;

                    Font boldFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);
                    PdfPCell cell1 = new PdfPCell(new Phrase("Date", boldFont));
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    pdfTable.AddCell(cell1);
                    PdfPCell cell2 = new PdfPCell(new Phrase("Description", boldFont));
                    cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                    pdfTable.AddCell(cell2);
                    PdfPCell cell3 = new PdfPCell(new Phrase("Cheque No", boldFont));
                    cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                    pdfTable.AddCell(cell3);
                    PdfPCell cell4 = new PdfPCell(new Phrase("WithDrawal", boldFont));
                    cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                    pdfTable.AddCell(cell4);
                    PdfPCell cell5 = new PdfPCell(new Phrase("Deposit", boldFont));
                    cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                    pdfTable.AddCell(cell5);
                    PdfPCell cell6 = new PdfPCell(new Phrase("Balance", boldFont));
                    cell6.HorizontalAlignment = Element.ALIGN_CENTER;
                    pdfTable.AddCell(cell6);


                    Font noramlFont = new Font(Font.FontFamily.HELVETICA, 10);

                    foreach (var tran in transactionlist)
                    {
                        PdfPCell trnTime = new PdfPCell(new Phrase(tran.TransactionTime.Value.ToString("dd/MM/yyyy"), noramlFont));
                        pdfTable.AddCell(trnTime);
                        PdfPCell desc = new PdfPCell(new Phrase(tran.Description == null ? "" : tran.Description.ToString(), noramlFont));
                        pdfTable.AddCell(desc);
                        PdfPCell chqno = new PdfPCell(new Phrase(tran.CheckNumber == null ? "" : tran.CheckNumber.Value.ToString(), noramlFont));
                        pdfTable.AddCell(chqno);

                        //pdfTable.AddCell(tran.TransactionTime.Value.ToString("dd/MM/yyyy"));
                        //pdfTable.AddCell(tran.CheckNumber == null ? "" : tran.CheckNumber.Value.ToString());
                        //pdfTable.AddCell(tran.Description == null ? "" : tran.Description.ToString());
                        if (tran.Type == TypeCRDR.DR)
                        {
                            PdfPCell cellAmt = new PdfPCell(new Phrase(tran.Amount.ToString(), noramlFont));
                            cellAmt.HorizontalAlignment = Element.ALIGN_RIGHT; //Tried with Element.Align_Center Also. Tried Adding this line before adding element also. 
                            pdfTable.AddCell(cellAmt);
                        }
                        else
                        {
                            pdfTable.AddCell("");
                        }

                        if (tran.Type == TypeCRDR.CR)
                        {
                            PdfPCell cellAmt = new PdfPCell(new Phrase(tran.Amount.ToString(), noramlFont));
                            cellAmt.HorizontalAlignment = Element.ALIGN_RIGHT;
                            pdfTable.AddCell(cellAmt);
                        }
                        else
                        {
                            pdfTable.AddCell("");
                        }

                        PdfPCell cellbal = new PdfPCell(new Phrase(tran.Balance.ToString(), noramlFont));
                        cellbal.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pdfTable.AddCell(cellbal);
                    }

                    doc.Add(pdfTable);

                    Paragraph para = new Paragraph();
                    para.Add("** This is computer generated statement and does not require a siganture.");
                    para.SpacingBefore = 20f;

                    doc.Add(para);

                    doc.Close();

                    return fileName;
                }
            }
        }

        public bool AddPrePayment(LoanPrePayment loanPrePayment, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                db.LoanPrePayment.Add(loanPrePayment);


                Loan loan = db.Loan.Where(a => a.LoanId == loanPrePayment.LoanId).FirstOrDefault();
                Lookup objLookup = db.Lookup.Where(x => x.LookupId == loan.LoanType).FirstOrDefault();

                if (loanPrePayment.TotalPendingInstallmentAmount > 0)
                {
                    List<RDPayment> objRDPayment = db.RDPayment.Where(a => a.CustomerProductId == loan.CustomerProductId && a.RDPaymentType == RDPaymentType.Installment && a.IsPaid == false).ToList();
                    if (objRDPayment.Count > 0)
                    {
                        objRDPayment.ForEach(s => s.IsPaid = true);
                        objRDPayment.ForEach(s => s.Amount = 0);
                        objRDPayment.ForEach(s => s.InterestAmount = 0);
                        objRDPayment.ForEach(s => s.PrincipalAmount = 0);
                        db.SaveChanges();
                    }
                }


                var savingAcc = db.CustomerProduct.Where(a => a.CustomerProductId == loanPrePayment.CustomerProductIdSaving).Select(a => new { a.Balance, a.UpdatedBalance, a.BranchId }).FirstOrDefault();

                var loanAcc = db.CustomerProduct.Where(a => a.CustomerProductId == loan.CustomerProductId).FirstOrDefault();

                //int iTerm = loanPrePayment.Term;
                //var b = loanAcc.LastInstallmentDate.Value;
                //decimal SavingBalance = savingAcc.Balance.Value;
                //decimal SavingUpdatedBalance = savingAcc.UpdatedBalance.Value;
                if (savingAcc.Balance >= loanPrePayment.PaymentAmount)
                {

                    Transactions transactionSavingPrePayment = new Transactions();
                    transactionSavingPrePayment.Amount = loanPrePayment.PaymentAmount.Value;
                    //transactionSavingPrePayment.Balance = savingAcc.UpdatedBalance.Value - loanPrePayment.PaymentAmount.Value;
                    transactionSavingPrePayment.TransactionType = TransactionType.BankTransfer;
                    transactionSavingPrePayment.Type = TypeCRDR.DR;
                    transactionSavingPrePayment.CustomerProductId = loanPrePayment.CustomerProductIdSaving;
                    transactionSavingPrePayment.BranchId = savingAcc.BranchId;
                    transactionSavingPrePayment.CreatedBy = user.UserId;
                    transactionSavingPrePayment.CreatedDate = DateTime.Now;
                    transactionSavingPrePayment.TransactionTime = loanPrePayment.TransactionTime;
                    transactionSavingPrePayment.DescIndentify = DescIndentify.Loan_PrePayment;
                    transactionSavingPrePayment.CustomerId = loan.CustomerId;
                    transactionSavingPrePayment.RefCustomerProductId = loan.CustomerProductId;
                    transactionSavingPrePayment.TransactionId = InsertTransctionUsingSP(transactionSavingPrePayment);

                    //SavingBalance = SavingBalance - loanPrePayment.PaymentAmount.Value;
                    //SavingUpdatedBalance = SavingUpdatedBalance - loanPrePayment.PaymentAmount.Value;

                    Transactions transactionLoanPrePayment = new Transactions();
                    transactionLoanPrePayment.Amount = loanPrePayment.PaymentAmount.Value;
                    //transactionLoanPrePayment.Balance = loanAcc.UpdatedBalance.Value + loanPrePayment.PaymentAmount.Value;
                    transactionLoanPrePayment.TransactionType = TransactionType.BankTransfer;
                    transactionLoanPrePayment.Type = TypeCRDR.CR;
                    transactionLoanPrePayment.CustomerProductId = loan.CustomerProductId;
                    transactionLoanPrePayment.BranchId = loanAcc.BranchId;
                    transactionLoanPrePayment.CreatedBy = user.UserId;
                    transactionLoanPrePayment.CreatedDate = DateTime.Now;
                    transactionLoanPrePayment.TransactionTime = loanPrePayment.TransactionTime;
                    transactionLoanPrePayment.DescIndentify = DescIndentify.Loan_PrePayment;
                    transactionLoanPrePayment.CustomerId = loan.CustomerId;
                    transactionLoanPrePayment.RefCustomerProductId = loanPrePayment.CustomerProductIdSaving;
                    transactionLoanPrePayment.TransactionId = InsertTransctionUsingSP(transactionLoanPrePayment);

                    RDPayment rdPayment = new RDPayment();
                    rdPayment.Amount = loanPrePayment.PaymentAmount.Value;
                    rdPayment.CustomerProductId = loan.CustomerProductId;
                    rdPayment.CustomerId = loan.CustomerId;
                    rdPayment.PrincipalAmount = loanPrePayment.PaymentAmount.Value;
                    rdPayment.InterestAmount = 0;
                    rdPayment.IsPaid = true;
                    rdPayment.RDPaymentType = RDPaymentType.Installment;
                    rdPayment.PaidDate = DateTime.Now;
                    rdPayment.CreatedDate = DateTime.Now;
                    db.Entry(rdPayment).State = EntityState.Added;
                    db.SaveChanges();

                    int loanEMICount = db.RDPayment.Where(a => a.CustomerProductId == loan.CustomerProductId && a.RDPaymentType == RDPaymentType.Installment).Count();

                    int PendingEMI = db.RDPayment.Where(a => a.CustomerProductId == loan.CustomerProductId && a.RDPaymentType == RDPaymentType.Installment && a.IsPaid == false).Count();

                    DateTime maturityDate;
                    if (loanAcc.LastInstallmentDate.HasValue)
                    {
                        maturityDate = loanAcc.LastInstallmentDate.Value.AddMonths(loanPrePayment.Term);
                    }
                    else
                    {
                        maturityDate = loanAcc.DueDate.Value.AddMonths(loanPrePayment.Term);
                    }


                    int totaldays = Convert.ToInt32((maturityDate - loanAcc.DueDate.Value).TotalDays);

                    if (loanPrePayment.PaymentAmount >= loanPrePayment.RemainingAmount && PendingEMI == 0)
                    {
                        loanAcc.Status = CustomerProductStatus.Completed;
                        loan.LoanStatus = LoanStatus.Completed;

                        UpdateLoanStatus updateLoanStatus = new UpdateLoanStatus();
                        updateLoanStatus.LoanId = loan.LoanId;
                        updateLoanStatus.LoanStatus = LoanStatus.Completed;
                        updateLoanStatus.UpdatedBy = user.UserId;
                        updateLoanStatus.UpdatedDate = DateTime.Now;


                        if (objLookup.Name == "Flexi Loan")
                        {
                            if (loan.TotalAmountToPay != null)
                            {
                                decimal InterestAmount = (decimal)loan.TotalAmountToPay - loan.LoanAmount;
                                if (InterestAmount != 0)
                                {
                                    Transactions transactionLoanPrePaymentInterest = new Transactions();
                                    transactionLoanPrePaymentInterest.Amount = InterestAmount;
                                    //transactionLoanPrePayment.Balance = loanAcc.UpdatedBalance.Value + loanPrePayment.PaymentAmount.Value;
                                    transactionLoanPrePaymentInterest.TransactionType = TransactionType.BankTransfer;
                                    transactionLoanPrePaymentInterest.Type = TypeCRDR.DR;
                                    transactionLoanPrePaymentInterest.CustomerProductId = loan.CustomerProductId;
                                    transactionLoanPrePaymentInterest.BranchId = loanAcc.BranchId;
                                    transactionLoanPrePaymentInterest.CreatedBy = user.UserId;
                                    transactionLoanPrePaymentInterest.CreatedDate = DateTime.Now;
                                    transactionLoanPrePaymentInterest.TransactionTime = loanPrePayment.TransactionTime;
                                    transactionLoanPrePaymentInterest.DescIndentify = DescIndentify.Loan_Interest_Amount;
                                    transactionLoanPrePaymentInterest.CustomerId = loan.CustomerId;
                                    transactionLoanPrePaymentInterest.RefCustomerProductId = loanPrePayment.CustomerProductIdSaving;
                                    transactionLoanPrePaymentInterest.TransactionId = InsertTransctionUsingSP(transactionLoanPrePaymentInterest);

                                }
                            }
                        }

                        loanService.SaveUpdatedLoanStatus(updateLoanStatus);

                        if (loanPrePayment.TotalPendingInteresttillDate != null)
                        {
                            if (loanPrePayment.TotalPendingInteresttillDate > 0)
                            {
                                Transactions transactionInterest = new Transactions();
                                transactionInterest.Amount = (decimal)loanPrePayment.TotalPendingInteresttillDate;
                                transactionInterest.TransactionType = TransactionType.BankTransfer;
                                transactionInterest.Type = TypeCRDR.DR;
                                transactionInterest.CustomerProductId = loan.CustomerProductId;
                                transactionInterest.BranchId = loanAcc.BranchId;
                                transactionInterest.CreatedBy = user.UserId;
                                transactionInterest.CreatedDate = DateTime.Now;
                                transactionInterest.TransactionTime = loanPrePayment.TransactionTime;
                                transactionInterest.DescIndentify = DescIndentify.Loan_Interest_Amount;
                                transactionInterest.CustomerId = loan.CustomerId;
                                transactionInterest.RefCustomerProductId = loan.CustomerProductId;
                                transactionInterest.TransactionId = InsertTransctionUsingSP(transactionInterest);
                            }
                        }




                        LoanCharges loanCharges = db.LoanCharges.Where(a => a.LoanId == loan.LoanId && a.Name.Contains("Share") && a.IsDelete == false).FirstOrDefault();

                        if (loanCharges != null)
                        {
                            CustomerShare customerShare = db.CustomerShare.Where(a => a.CustomerId == loan.CustomerId && a.Maturity == Maturity.Nominal && a.Total == loanCharges.Value && a.IsReverted == null).FirstOrDefault();

                            if (customerShare != null)
                            {
                                if (customerShare.IsReverted == null || customerShare.IsReverted == false)
                                {

                                    Transactions transactionShare = new Transactions();
                                    transactionShare.Amount = customerShare.Total;
                                    //transactionShare.Balance = SavingUpdatedBalance + customerShare.Total;
                                    transactionShare.TransactionType = TransactionType.BankTransfer;
                                    transactionShare.Type = TypeCRDR.CR;
                                    transactionShare.CustomerProductId = loanPrePayment.CustomerProductIdSaving.Value;
                                    transactionShare.BranchId = loanAcc.BranchId;
                                    transactionShare.CreatedBy = user.UserId;
                                    transactionShare.CreatedDate = DateTime.Now;
                                    transactionShare.TransactionTime = loanPrePayment.TransactionTime;
                                    transactionShare.DescIndentify = DescIndentify.Share;
                                    transactionShare.CustomerId = loan.CustomerId;
                                    transactionShare.RefCustomerProductId = loan.CustomerProductId;
                                    transactionShare.TransactionId = InsertTransctionUsingSP(transactionShare);

                                    customerShare.IsReverted = true;

                                    db.CustomerShare.Attach(customerShare);
                                    db.Entry(customerShare).Property(x => x.IsReverted).IsModified = true;
                                }
                                //SavingBalance = SavingBalance + customerShare.Total;
                                //SavingUpdatedBalance = SavingUpdatedBalance + +customerShare.Total;
                            }
                        }
                    }

                    //CustomerProduct customerProductSaving = new CustomerProduct() { CustomerProductId = loanPrePayment.CustomerProductIdSaving.Value, UpdatedBalance = SavingUpdatedBalance, Balance = SavingBalance };
                    //db.CustomerProduct.Attach(customerProductSaving);
                    //db.Entry(customerProductSaving).Property(x => x.Balance).IsModified = true;
                    //db.Entry(customerProductSaving).Property(x => x.UpdatedBalance).IsModified = true;
                    //db.SaveChanges();
                    //db.Entry(customerProductSaving).State = EntityState.Detached;


                    //loanAcc.UpdatedBalance = loanAcc.UpdatedBalance.Value + loanPrePayment.PaymentAmount.Value;
                    //loanAcc.Balance = loanAcc.Balance.Value + loanPrePayment.PaymentAmount.Value;
                    loanAcc.Amount = loanPrePayment.MonthlyInstallmentAmount;
                    loanAcc.MaturityDate = maturityDate;
                    loanAcc.TotalInstallment = loanEMICount + Convert.ToInt32(loanPrePayment.Term);
                    loanAcc.TotalDays = totaldays;
                    loanAcc.Status = loanAcc.Status;
                    db.CustomerProduct.Attach(loanAcc);
                    //db.Entry(loanAcc).Property(x => x.Balance).IsModified = true;
                    //db.Entry(loanAcc).Property(x => x.UpdatedBalance).IsModified = true;
                    db.Entry(loanAcc).Property(x => x.Amount).IsModified = true;
                    db.Entry(loanAcc).Property(x => x.MaturityDate).IsModified = true;
                    db.Entry(loanAcc).Property(x => x.TotalInstallment).IsModified = true;
                    db.Entry(loanAcc).Property(x => x.TotalDays).IsModified = true;
                    db.Entry(loanAcc).Property(x => x.Status).IsModified = true;
                    db.SaveChanges();
                    db.Entry(loanAcc).State = EntityState.Detached;

                    //loan.Term = Convert.ToString(loanEMI.Count + Convert.ToInt32(loanPrePayment.Term));
                    loan.MonthlyInstallmentAmount = loanPrePayment.MonthlyInstallmentAmount;
                    if (loanPrePayment.TotalPendingInstallmentAmount > 0)
                        loan.LastPrincipalAmount = loanPrePayment.LoanAmount + loanPrePayment.TotalPendingInstallmentAmount;
                    else
                        loan.LastPrincipalAmount = loanPrePayment.LoanAmount;

                    loan.LastTenure = loanPrePayment.Term;

                    List<RDPayment> objRDPaymentPaidCount = db.RDPayment.Where(a => a.CustomerProductId == loan.CustomerProductId && a.RDPaymentType == RDPaymentType.Installment && a.IsPaid == true && a.Amount != 0).ToList();


                    loan.Term = (objRDPaymentPaidCount.Count() + loanPrePayment.Term).ToString();
                    loan.LastInstallmentDate = loanPrePayment.InstallmentDate;
                    db.Loan.Attach(loan);
                    db.Entry(loan).Property(x => x.Term).IsModified = true;
                    db.Entry(loan).Property(x => x.MonthlyInstallmentAmount).IsModified = true;
                    db.Entry(loan).Property(x => x.LastPrincipalAmount).IsModified = true;
                    db.Entry(loan).Property(x => x.LastTenure).IsModified = true;
                    db.Entry(loan).Property(x => x.LastInstallmentDate).IsModified = true;
                    db.Entry(loan).Property(x => x.LoanStatus).IsModified = true;
                    db.SaveChanges();
                    db.Entry(loan).State = EntityState.Detached;

                    if (objLookup != null)
                    {
                        if (objLookup.Name != "Flexi Loan")
                        {
                            Amountisation amountisation = loanService.DisplayAmountisation(loan.LoanId);

                            decimal RemainingPrincipalInterest = amountisation.ListLoanAmountisation.Sum(a => a.Interest);
                            decimal TotalAmountToPay = Convert.ToDecimal(loan.LoanAmount + RemainingPrincipalInterest);
                            if (loanPrePayment.TotalPendingInstallmentAmount > 0)
                            {
                                List<RDPayment> objRDPaymentPaid = db.RDPayment.Where(a => a.CustomerProductId == loan.CustomerProductId && a.RDPaymentType == RDPaymentType.Installment && a.IsPaid == true).ToList();
                                if (objRDPaymentPaid != null)
                                {
                                    decimal loanpaidamount = objRDPaymentPaid.Select(s => s.Amount != null ? s.Amount : 0).Sum();
                                    loan.TotalAmountToPay = loanpaidamount + amountisation.ListLoanAmountisation.Where(a => a.IsPaid == null).Sum(a => a.MonthlyEMI);
                                }
                            }
                            else
                                loan.TotalAmountToPay = TotalAmountToPay;

                            //loan.TotalAmountToPay = amountisation.ListLoanAmountisation.Where(a => a.IsPaid != true).Sum(a => a.MonthlyEMI);

                            db.Loan.Attach(loan);
                            db.Entry(loan).Property(x => x.TotalAmountToPay).IsModified = true;
                            db.SaveChanges();
                            db.Entry(loan).State = EntityState.Detached;
                        }
                    }


                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public CustomerProduct DeleteLastTransaction(Guid Id, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                Guid CustomerProductId = new Guid();
                Transactions objTransaction = new Transactions();
                CustomerProduct objCustomerProduct = new CustomerProduct();
                objTransaction = db.Transaction.Where(x => x.TransactionId == Id).FirstOrDefault();
                if (objTransaction != null)
                    CustomerProductId = (Guid)objTransaction.CustomerProductId;

                SqlParameter TransId = new SqlParameter("TransId", Id);
                db.Database.CommandTimeout = 3600;
                var trans = db.Database.SqlQuery<object>("DeleteTransaction @TransId", TransId).FirstOrDefault();
                objCustomerProduct = db.CustomerProduct.Where(x => x.CustomerProductId == CustomerProductId).FirstOrDefault();

                if (objCustomerProduct != null)
                {
                    AuditLog objAuditLog = new AuditLog();
                    objAuditLog.FieldName = "Delete Transaction";
                    objAuditLog.TYPE = "Delete";
                    objAuditLog.TableID = CustomerProductId;
                    objAuditLog.OldValue = objTransaction.Amount.ToString();
                    objAuditLog.NewValue = "0";
                    objAuditLog.UpdatedDate = DateTime.Now;
                    objAuditLog.UpdatedBy = user.UserId;
                    objAuditLog.TableName = "Transaction";
                    objAuditLog.ReferenceId = "";

                    db.AuditLog.Add(objAuditLog);
                    db.SaveChanges();
                }
                return objCustomerProduct;
            }
        }



    }
}
