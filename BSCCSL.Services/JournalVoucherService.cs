using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class JournalVoucherService
    {
        public JournalVoucher GetJournalVoucherById(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                JournalVoucher journalVoucher = db.JournalVoucher.Where(a => a.JournalVoucherId == Id).FirstOrDefault();
                return journalVoucher;
            }
        }

        public object GetJournalVoucherList(DataTableSearch search, User user)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {

                var IsHO = db.Branch.Where(c => c.BranchId == search.BranchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    search.BranchId = null;
                }

                //var list = (from jv in db.JournalVoucher.Where(s => s.IsDelete == false)
                //            join fa in db.AccountsHead on jv.FromAccountHead equals fa.AccountsHeadId
                //            join ta in db.AccountsHead on jv.ToAccountHead equals ta.AccountsHeadId
                //            join c in db.User on jv.CreatedBy equals c.UserId
                //            select new
                //            {
                //                jv.JournalVoucherId,
                //                jv.JournalVoucherName,
                //                jv.CreatedDate,
                //                jv.Description,
                //                jv.Amount,
                //                jv.BranchId,
                //                jv.FromAccount,
                //                jv.ToAccount,
                //                jv.Branch.BranchName,
                //                jv.UpdatedDate,
                //                jv.JVDate,
                //                jv.JVNumber,
                //                jv.FromBranchId,
                //                jv.ToBranchId,
                //                fa,
                //                ta
                //            }).AsQueryable();

                var list = (from jv in db.JournalVoucher.Where(s => s.IsDelete == false)
                            join cp in db.CustomerProduct on jv.ToCustomerProductId equals cp.CustomerProductId
                            let cpd = db.CustomerPersonalDetail.FirstOrDefault(p => p.CustomerId == cp.CustomerId)
                            //  join c in db.CustomerPersonalDetail on cp.CustomerId equals c.CustomerId
                            select new
                            {
                                jv.JournalVoucherId,
                                jv.Amount,
                                jv.CreatedDate,
                                jv.ToAccount,
                                jv.UpdatedDate,
                                jv.JVDate,
                                jv.JVNumber,
                                jv.BranchId,
                                jv.Type,
                                cpd.FirstName,
                                cpd.MiddleName,
                                cpd.LastName
                            }).AsQueryable();



                if (search.BranchId != null)
                {
                    list = list.Where(c => c.BranchId == search.BranchId);
                }
                //if (!string.IsNullOrEmpty(search.sSearch))
                //{
                //    list = list.Where(c => c.ExpenseName.Contains(search.sSearch.Trim()));
                //}
                //if (search.fromDate != null)
                //{
                //    list = list.Where(c => DbFunctions.TruncateTime(c.ExpenseDate) >= DbFunctions.TruncateTime(search.fromDate));
                //}
                //if (search.toDate != null)
                //{
                //    list = list.Where(c => DbFunctions.TruncateTime(c.ExpenseDate) <= DbFunctions.TruncateTime(search.toDate));
                //}

                var journalVoucherlist = list.OrderBy(c => c.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();
                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = journalVoucherlist.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = journalVoucherlist
                };
                return data;
            }
        }

        public bool SaveJournalVoucher(JournalVoucher journalVoucher, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (!string.IsNullOrEmpty(journalVoucher.ToAccount))
                {
                    var CustomerDetails = db.CustomerProduct.Where(x => x.AccountNumber.ToLower() == journalVoucher.ToAccount.ToLower()).FirstOrDefault();
                    if (CustomerDetails != null)
                    {
                        journalVoucher.ToCustomerProductId = CustomerDetails.CustomerProductId;

                        TransactionService transactionService = new TransactionService();
                        Transactions transaction = new Transactions
                        {
                            CustomerProductId = CustomerDetails.CustomerProductId,
                            CustomerId = CustomerDetails.CustomerId,
                            Amount = (decimal)journalVoucher.Amount,
                            Type = (TypeCRDR)journalVoucher.Type,
                            CreatedBy = user.UserId,
                            CreatedDate = DateTime.Now,
                            TransactionType = TransactionType.JV,
                            DescIndentify = DescIndentify.JV,
                            BranchId = CustomerDetails.BranchId,
                            Status = Status.Clear,
                            TransactionTime = DateTime.Now.AddSeconds(5)
                        };

                        Guid TransactionId = transactionService.InsertTransctionUsingSP(transaction);

                    }

                }

                if (journalVoucher.JournalVoucherId == Guid.Empty)
                {
                    journalVoucher.CreatedBy = user.UserId;
                    journalVoucher.IsActive = true;
                    journalVoucher.UpdatedDate = DateTime.Now;
                    db.JournalVoucher.Add(journalVoucher);
                }
                else
                {
                    var journalvoucherdata = db.JournalVoucher.Where(a => a.JournalVoucherId == journalVoucher.JournalVoucherId).FirstOrDefault();

                    //journalvoucherdata.JournalVoucherName = journalVoucher.JournalVoucherName;
                    //journalvoucherdata.ToHeadType = journalVoucher.ToHeadType;
                    //journalvoucherdata.FromHeadType = journalVoucher.FromHeadType;
                    //journalvoucherdata.FromAccountHead = journalVoucher.FromAccountHead;
                    //journalvoucherdata.ToAccountHead = journalVoucher.ToAccountHead;
                    //journalvoucherdata.FromAccount = journalVoucher.FromAccount;
                    //journalvoucherdata.Description = journalVoucher.Description;
                    //journalvoucherdata.FromBranchId = journalVoucher.FromBranchId;
                    //journalvoucherdata.ToBranchId = journalVoucher.ToBranchId;
                    //journalvoucherdata.TransactionMode = journalVoucher.TransactionMode;

                    journalvoucherdata.ToAccount = journalVoucher.ToAccount;
                    journalvoucherdata.Amount = journalVoucher.Amount;
                    journalvoucherdata.ModifiedBy = user.UserId;
                    journalvoucherdata.UpdatedDate = DateTime.Now;
                    journalvoucherdata.IsActive = true;
                    journalvoucherdata.JVDate = journalVoucher.JVDate;
                    journalvoucherdata.JVNumber = journalVoucher.JVNumber;

                }

                db.SaveChanges();
                return true;
            }
        }

        public bool DeleteJournalVoucherById(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                var journalVoucher = new JournalVoucher { JournalVoucherId = Id, IsDelete = true };
                db.JournalVoucher.Attach(journalVoucher);
                db.Entry(journalVoucher).Property(s => s.IsDelete).IsModified = true;
                db.SaveChanges();
                return true;
            }
        }

        //For Get Customer Product Details 
        public object GetCustomerDataByProductId(string accNo)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {

                var CustomerProductDetails = (from cp in db.CustomerProduct.Where(x => x.AccountNumber.ToLower() == accNo.ToLower() && x.IsDelete == false)
                                              join cpd in db.CustomerPersonalDetail.Where(a => a.IsDelete == false) on cp.CustomerId equals cpd.CustomerId
                                              join c in db.Customer.Where(a => a.IsDelete == false) on cp.CustomerId equals c.CustomerId
                                              select new
                                              {
                                                  CustomerProductId = cp.CustomerProductId,
                                                  CustomerId = cp.CustomerId,
                                                  CustomerName = cpd.FirstName + " " + cpd.MiddleName + " " + cpd.LastName,
                                                  productType = cp.ProductType.ToString()
                                              }).ToList();



                return new
                {
                    CustomerProductDetails,
                };
            }

        }

    }
}
