using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class BankMasterService
    {

        public object GetAllBankList(DataTableSearch search, User user)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var isHo = db.Branch.Where(s => s.BranchId == search.BranchId).Select(s => s.IsHO).FirstOrDefault();
                if (isHo && user.Role == Role.Admin)
                {
                    var query = db.BankMaster.Where(s => !s.IsDelete).AsQueryable();

                    if (!string.IsNullOrEmpty(search.sSearch))
                    {
                        query = query.Where(c => c.AccountNumber.Contains(search.sSearch.Trim()) || c.BankName.ToLower().Contains(search.sSearch.ToLower().Trim()));
                    }
                    var customer = query.OrderBy(c => c.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                    var data = new
                    {
                        sEcho = search.sEcho,
                        iTotalRecords = customer.Count(),
                        iTotalDisplayRecords = query.Count(),
                        aaData = customer
                    };
                    return data;
                }
                else
                {
                    var query = (from bm in db.BankBranchMapping.Where(s => s.BranchId == search.BranchId && !s.IsDelete)
                                 join b in db.BankMaster on bm.BankId equals b.BankId
                                 select new
                                 {
                                     BankId = b.BankId,
                                     AccountNumber = b.AccountNumber,
                                     BankName = b.BankName,
                                     Address = b.Address,
                                     AccountType = b.AccountType,
                                     Balance = b.Balance,
                                     CreatedDate = b.CreatedDate,
                                     IsActive = b.IsActive
                                 }).AsQueryable();

                    if (!string.IsNullOrEmpty(search.sSearch))
                    {
                        query = query.Where(c => c.AccountNumber.Contains(search.sSearch.Trim()) || c.BankName.ToLower().Contains(search.sSearch.ToLower().Trim()));
                    }
                    var customer = query.OrderBy(c => c.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                    var data = new
                    {
                        sEcho = search.sEcho,
                        iTotalRecords = customer.Count(),
                        iTotalDisplayRecords = query.Count(),
                        aaData = customer
                    };
                    return data;
                }
            }
        }

        public bool SaveBankDetails(BankDetails bank)
        {
            using (var db = new BSCCSLEntity())
            {
                if (bank.Bank.BankId == Guid.Empty)
                {
                    bank.Bank.UpdatedBalance = bank.Bank.Balance;
                    db.BankMaster.Add(bank.Bank);
                    db.SaveChanges();
                }
                else
                {
                    var bankData = db.BankMaster.Where(a => a.BankId == bank.Bank.BankId).FirstOrDefault();
                    bankData.BankName = bank.Bank.BankName;
                    bankData.AccountNumber = bank.Bank.AccountNumber;
                    bankData.AccountType = bank.Bank.AccountType;
                    bankData.Address = bank.Bank.Address;
                    bankData.Balance = bank.Bank.Balance;
                    bankData.IsActive = bank.Bank.IsActive;
                    bankData.ModifiedBy = bank.Bank.ModifiedBy;
                    bankData.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                }

                if (bank.BranchIds.Count != 0)
                {
                    var mappinglist = db.BankBranchMapping.Where(s => s.BankId == bank.Bank.BankId).AsNoTracking().ToList();
                    foreach (Guid id in bank.BranchIds)
                    {
                        var MData = mappinglist.Where(s => s.BranchId == id).FirstOrDefault();
                        if (MData != null)
                        {
                            var data = new BankBranchMapping() { BranchMappingId = MData.BranchMappingId, IsDelete = false };
                            db.BankBranchMapping.Attach(data);
                            db.Entry(data).Property(s => s.IsDelete).IsModified = true;
                            db.SaveChanges();
                            db.Entry(data).State = EntityState.Detached;
                        }
                        else
                        {
                            BankBranchMapping daata = new BankBranchMapping();
                            daata.BankId = bank.Bank.BankId;
                            daata.BranchId = id;
                            daata.IsDelete = false;
                            db.BankBranchMapping.Add(daata);
                            db.SaveChanges();
                        }
                    }
                }
                return true;
            }
        }

        public bool RemoveAllPresentMapping(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var mappinglist = db.BankBranchMapping.Where(s => s.BankId == Id).ToList();
                    if (mappinglist.Count != 0)
                    {
                        foreach (BankBranchMapping map in mappinglist)
                        {
                            map.IsDelete = true;
                            db.Entry(map).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public bool DeleteBank(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var bankdata = new BankMaster() { BankId = Id, IsDelete = true };
                    db.BankMaster.Attach(bankdata);
                    db.Entry(bankdata).Property(s => s.IsDelete).IsModified = true;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public object GetBankData(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var cpdetail = (from bm in db.BankMaster.AsEnumerable().Where(x => x.BankId == Id)
                                    join t in db.BankTransaction.Where(a => a.Status == Status.Unclear) on bm.BankId equals t.BankId into trans
                                    from t in trans.DefaultIfEmpty()
                                    group new { t } by new { bm } into grp
                                    select new
                                    {
                                        BankId = grp.Key.bm.BankId,
                                        AccountNumber = grp.Key.bm.AccountNumber,
                                        BankName = grp.Key.bm.BankName,
                                        Address = grp.Key.bm.Address,
                                        AccountType = grp.Key.bm.AccountType,
                                        Balance = grp.Key.bm.Balance,
                                        UpdatedBalance = grp.Key.bm.UpdatedBalance,
                                        UnclearBalance = grp.Select(a => a.t != null ? a.t.Amount : 0).Sum(),
                                        AccountTypeName = Enum.GetName(typeof(ProductType), grp.Key.bm.AccountType).Replace("_", " ")
                                    }).FirstOrDefault();
                    return cpdetail;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public object GetBankDetailById(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var Bank = (from b in db.BankMaster.Where(s => s.BankId == Id)
                                join bm in db.BankBranchMapping.Where(s => !s.IsDelete) on b.BankId equals bm.BankId
                                group new { bm } by new { b } into gplist
                                select new
                                {
                                    BankId = gplist.Key.b.BankId,
                                    AccountNumber = gplist.Key.b.AccountNumber,
                                    BankName = gplist.Key.b.BankName,
                                    Address = gplist.Key.b.Address,
                                    AccountType = gplist.Key.b.AccountType,
                                    Balance = gplist.Key.b.Balance,
                                    IsDelete = gplist.Key.b.IsDelete,
                                    IsActive = gplist.Key.b.IsActive,
                                    CreatedBy = gplist.Key.b.CreatedBy,
                                    CreatedDate = gplist.Key.b.CreatedDate,
                                    ModifiedBy = gplist.Key.b.ModifiedBy,
                                    ModifiedDate = gplist.Key.b.ModifiedDate,
                                    BranchMapping = gplist.Select(s => s.bm.BranchId).ToList(),
                                    TransactionCount = db.BankTransaction.Where(s => s.BankId == gplist.Key.b.BankId).Count()
                                }).FirstOrDefault();

                    return Bank;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public object GetBankTransactionData(DataTableSearch search)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = db.BankTransaction.Where(t => t.BankId == search.id).AsQueryable();

                if (search.Type != 0)
                {
                    list = list.Where(c => c.Type == search.Type);
                }
                if (search.fromDate != null)
                {
                    list = list.Where(c => DbFunctions.TruncateTime(c.TransactionTime) >= search.fromDate);
                }
                if (search.toDate != null)
                {
                    list = list.Where(c => DbFunctions.TruncateTime(c.TransactionTime) <= search.toDate);
                }
                if (search.BranchId.HasValue)
                {
                    list = list.Where(c => c.BranchId == search.BranchId);
                }

                var TransactionList = list.OrderBy(a => a.TransactionTime).ThenBy(a => a.BankTransactionId).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var TotalTransactionList = list.OrderBy(a => a.TransactionTime).ThenBy(a => a.BankTransactionId).ToList();
                var LastTransId = TotalTransactionList.OrderByDescending(p => p.TransactionTime).FirstOrDefault();
                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = TransactionList.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = TransactionList,
                    LastTransactionId = LastTransId != null ? LastTransId.BankTransactionId.ToString() : "",
                    CurrentDate = DateTime.Now

                };
                return data;
            }
        }

        public decimal SaveBankTransaction(BankTransaction transaction)
        {
            using (var db = new BSCCSLEntity())
            {
                if (transaction.BankTransactionId == Guid.Empty)
                {
                    if (transaction.Type == TypeCRDR.DR)
                    {
                        var Latestbalance = db.BankMaster.Where(x => x.BankId == transaction.BankId).Select(x => x.UpdatedBalance).FirstOrDefault();
                        transaction.Balance = Convert.ToDecimal(Latestbalance) - transaction.Amount;
                    }
                    transaction.CreatedBy = transaction.CreatedBy;

                    transaction.BankTransactionId = InsertBankTransctionUsingSP(transaction);


                    var bankmasterdata = db.BankMaster.Where(x => x.BankId == transaction.BankId).FirstOrDefault();
                    bankmasterdata.UpdatedBalance = transaction.Balance;

                    if (transaction.Type == TypeCRDR.CR)
                    {
                        if (transaction.TransactionType == TransactionType.Cash || transaction.TransactionType == TransactionType.IMPS_NEFT)
                        {
                            bankmasterdata.Balance = bankmasterdata.Balance + transaction.Amount;
                        }
                    }
                    else
                    {
                        bankmasterdata.Balance = bankmasterdata.Balance - transaction.Amount;
                    }
                    db.Entry(bankmasterdata).State = EntityState.Modified;
                    db.SaveChanges();
                }
                #region Add the Description
                BankTransaction objTransactions = db.BankTransaction.Where(x => x.BankTransactionId == transaction.BankTransactionId).FirstOrDefault();
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
                db.SaveChanges();
                // smsService.SendTransactionSMS(transaction.TransactionId);
                return transaction.Balance;
            }
        }

        public Guid InsertBankTransctionUsingSP(BankTransaction transaction)
        {
            using (var db = new BSCCSLEntity())
            {
                Guid transctionId = new Guid();

                SqlParameter BankId = new SqlParameter("BankId", transaction.BankId);
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
                SqlParameter TransactionTime = new SqlParameter("TransactionTime", transaction.TransactionTime);
                SqlParameter BankName = new SqlParameter("BankName", (object)transaction.BankName ?? DBNull.Value);
                SqlParameter BranchId = new SqlParameter("BranchId", transaction.BranchId);
                SqlParameter DescIndentify = new SqlParameter("DescIndentify", (object)transaction.DescIndentify ?? DBNull.Value);
                SqlParameter Id = new SqlParameter
                {
                    ParameterName = "@Id",
                    SqlDbType = System.Data.SqlDbType.UniqueIdentifier,
                    Size = 50,
                    Direction = System.Data.ParameterDirection.Output
                };

                var trans = db.Database.SqlQuery<object>("SaveBankTransaction @BankId, @Amount ,@Balance, @Type, @CreatedBy, @CreatedDate,@ModifiedBy, @ModifiedDate,@TransactionType,@CheckNumber,@ChequeDate,@BounceReason,@Penalty,@ChequeClearDate,@Status,@TransactionTime,@BankName,@BranchId,@DescIndentify,@Id  OUT", BankId, Amount, Balance, Type, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, TransactionType, CheckNumber, ChequeDate, BounceReason, Penalty, ChequeClearDate, Status, TransactionTime, BankName, BranchId, DescIndentify, Id).FirstOrDefault();

                transctionId = Guid.Parse(Id.Value.ToString());
                return transctionId;
            }
        }

        public object gettransactionDetails(Guid id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var transactionData = (from c in db.BankTransaction.Where(x => x.BankTransactionId == id)
                                       join b in db.BankMaster on c.BankId equals b.BankId
                                       select new
                                       {
                                           CheckNumber = c.CheckNumber,
                                           TransactionType = c.TransactionType,
                                           ChequeDate = c.ChequeDate,
                                           BankName = c.BankName,
                                           Amount = c.Amount,
                                           TransactionTime = c.TransactionTime,
                                           AccountNumber = b.AccountNumber,
                                           TransactionId = c.BankTransactionId,
                                           BankId = b.BankId
                                       }).FirstOrDefault();
                var TransactionType = Enum.GetName(typeof(TransactionType), transactionData.TransactionType);

                return new
                {
                    transactionData,
                    TransactionType
                };
            }
        }

        public decimal UpdateTransactionData(BankTransaction Transactiondata, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                var finalbalance = 0.0M;
                var transctiondetails = db.BankTransaction.Where(x => x.BankTransactionId == Transactiondata.BankTransactionId).FirstOrDefault();
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
                    //smsService.SendTransactionSMS(transctiondetails.TransactionId);

                    BankMaster customerProduct = db.BankMaster.Where(cp => cp.BankId == transctiondetails.BankId).FirstOrDefault();
                    customerProduct.Balance = customerProduct.Balance + transctiondetails.Amount;
                    db.SaveChanges();
                }
                return finalbalance;
            }
        }

        public decimal UpdateTransactionDataForBounce(BankTransaction BounceDetails)
        {
            using (var db = new BSCCSLEntity())
            {
                // var bal = db.Transaction.Where(x => x.CustomerProductId.Value == BounceDetails.CustomerProductId.Value).OrderByDescending(a => a.CreatedDate).FirstOrDefault();

                var bankData = db.BankMaster.Where(x => x.BankId == BounceDetails.BankId).FirstOrDefault();
                var transctiondetails = db.BankTransaction.Where(x => x.BankTransactionId == BounceDetails.BankTransactionId).FirstOrDefault();
                int penlty = Convert.ToInt32(BounceDetails.Penalty) != 0 ? Convert.ToInt32(BounceDetails.Penalty) : 0;
                var PenaltyAddedBalance = 0.0M;

                if (transctiondetails != null)
                {
                    if (transctiondetails.Type == TypeCRDR.CR)
                    {
                        transctiondetails.BankTransactionId = transctiondetails.BankTransactionId;
                        transctiondetails.Penalty = penlty;
                        transctiondetails.BounceReason = BounceDetails.BounceReason;
                        transctiondetails.Status = 0;
                        transctiondetails.ChequeClearDate = BounceDetails.ChequeClearDate;
                        db.Entry(transctiondetails).State = EntityState.Modified;
                        db.SaveChanges();

                        //decimal Deposite_Subracted_balance = bankData.Balance - transctiondetails.Amount;
                        decimal Deposite_Subracted_Updatedbalance = bankData.UpdatedBalance - transctiondetails.Amount;

                        BankTransaction transactionBounce = new BankTransaction();
                        transactionBounce.Amount = transctiondetails.Amount;
                        transactionBounce.BankId = transctiondetails.BankId;
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
                        transactionBounce.Balance = Deposite_Subracted_Updatedbalance;
                        transactionBounce.BranchId = BounceDetails.BranchId;
                        transactionBounce.BankTransactionId = InsertBankTransctionUsingSP(transactionBounce);


                        if (BounceDetails.Penalty != 0)
                        {
                            PenaltyAddedBalance = Convert.ToDecimal(Deposite_Subracted_Updatedbalance) - penlty;
                            BankTransaction transactionPanelty = new BankTransaction();
                            transactionPanelty.Status = 0;
                            transactionPanelty.Penalty = penlty;
                            transactionPanelty.BounceReason = BounceDetails.BounceReason;
                            transactionPanelty.Balance = PenaltyAddedBalance;
                            transactionPanelty.Amount = penlty;
                            transactionPanelty.BankId = transctiondetails.BankId;
                            transactionPanelty.Type = TypeCRDR.DR;
                            transactionPanelty.DescIndentify = DescIndentify.Panelty;
                            transactionPanelty.TransactionTime = DateTime.Now;
                            transactionPanelty.ChequeDate = transctiondetails.ChequeDate;
                            transactionPanelty.CheckNumber = transctiondetails.CheckNumber;
                            transactionPanelty.ChequeClearDate = transctiondetails.ChequeClearDate;
                            transactionPanelty.BankName = transctiondetails.BankName;
                            transactionPanelty.CreatedBy = transctiondetails.CreatedBy;
                            transactionPanelty.CreatedDate = transctiondetails.CreatedDate;
                            transactionPanelty.BranchId = BounceDetails.BranchId;
                            transactionPanelty.BankTransactionId = InsertBankTransctionUsingSP(transactionPanelty);
                            //smsService.SendTransactionSMS(transactionPanelty.TransactionId);
                            //db.Entry(transctiondetails).State = EntityState.Added;
                            // db.SaveChanges();

                            bankData.Balance = bankData.Balance - penlty;
                            bankData.UpdatedBalance = PenaltyAddedBalance;
                            db.Entry(bankData).State = EntityState.Modified;

                        }
                        else
                        {
                            bankData.UpdatedBalance = Deposite_Subracted_Updatedbalance;
                            db.Entry(bankData).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                return PenaltyAddedBalance;
            }
        }


        public BankMaster DeleteLastTransaction(Guid Id, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                Guid BankId = new Guid();
                BankTransaction objTransaction = new BankTransaction();
                BankMaster objBankMaster = new BankMaster();
                objTransaction = db.BankTransaction.Where(x => x.BankTransactionId == Id).FirstOrDefault();
                if (objTransaction != null)
                    BankId = (Guid)objTransaction.BankId;

                SqlParameter TransId = new SqlParameter("TransId", Id);
                db.Database.CommandTimeout = 3600;
                var trans = db.Database.SqlQuery<object>("DeleteBankTransaction @TransId", TransId).FirstOrDefault();
                objBankMaster = db.BankMaster.Where(x => x.BankId == BankId).FirstOrDefault();

                if (objBankMaster != null)
                {
                    AuditLog objAuditLog = new AuditLog();
                    objAuditLog.FieldName = "Delete Bank Transaction";
                    objAuditLog.TYPE = "Delete";
                    objAuditLog.TableID = BankId;
                    objAuditLog.OldValue = objTransaction.Amount.ToString();
                    objAuditLog.NewValue = "0";
                    objAuditLog.UpdatedDate = DateTime.Now;
                    objAuditLog.UpdatedBy = user.UserId;
                    objAuditLog.TableName = "Transaction";
                    objAuditLog.ReferenceId = "";

                    db.AuditLog.Add(objAuditLog);
                    db.SaveChanges();
                }
                return objBankMaster;
            }
        }

    }
}
