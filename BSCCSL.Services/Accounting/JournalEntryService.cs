using BSCCSL.Models;
using BSCCSL.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class JournalEntryService
    {
        public object GetAccountList()
        {
            using (var db = new BSCCSLEntity())
            {
                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetAccountList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<AccountList> objAccountList = ((IObjectContextAdapter)db).ObjectContext.Translate<AccountList>(reader).ToList();
                reader.NextResult();


                sql.Close();
                db.Dispose();
                return objAccountList;
            }
        }

        public int SaveJournalEntry(JournalEntryViewModel objJournalEntryViewModel, User objUser)
        {
            int Count = 0;

            using (var db = new BSCCSLEntity())
            {
                JournalEntry objJournalEntry = new JournalEntry();


                if (objJournalEntryViewModel.Id == Guid.Empty)
                {
                    int JvNo = db.JournalEntry.OrderByDescending(x => x.CreatedDate).Select(x => x.VoucherNo).FirstOrDefault();
                    JvNo = JvNo + 1;

                    objJournalEntry.EntryType = objJournalEntryViewModel.EntryType;
                    objJournalEntry.ReferenceDate = objJournalEntryViewModel.ReferenceDate;
                    objJournalEntry.PostingDate = objJournalEntryViewModel.PostingDate;
                    objJournalEntry.ReferenceNo = objJournalEntryViewModel.ReferenceNo;
                    objJournalEntry.Description = objJournalEntryViewModel.Description;
                    objJournalEntry.BranchId = objJournalEntryViewModel.BranchId;
                    objJournalEntry.VoucherNo = JvNo;
                    objJournalEntry.Prefix = "JV-";
                    objJournalEntry.CreatedDate = DateTime.Now;
                    objJournalEntry.CreatedBy = objUser.UserId;
                    objJournalEntry.IsDelete = false;
                    db.JournalEntry.Add(objJournalEntry);
                }
                else
                {
                    objJournalEntry = db.JournalEntry.Where(x => x.Id == objJournalEntryViewModel.Id).FirstOrDefault();
                    if (objJournalEntry != null)
                    {
                        objJournalEntry.EntryType = objJournalEntryViewModel.EntryType;
                        objJournalEntry.ReferenceDate = objJournalEntryViewModel.ReferenceDate;
                        objJournalEntry.PostingDate = objJournalEntryViewModel.PostingDate;
                        objJournalEntry.ReferenceNo = objJournalEntryViewModel.ReferenceNo;
                        objJournalEntry.Description = objJournalEntryViewModel.Description;
                        objJournalEntry.ModifiedDate = DateTime.Now;
                        objJournalEntry.ModifiedBy = objUser.UserId;

                    }
                }

                Count = db.SaveChanges();
                foreach (var item in objJournalEntryViewModel.EntryList)
                {
                    JournalEntryTransactions objJournalEntryTransactions = new JournalEntryTransactions();
                    if (item.Id == Guid.Empty)
                    {
                        objJournalEntryTransactions.AccountId = item.AccountId;
                        objJournalEntryTransactions.JournalEntryId = objJournalEntry.Id;
                        objJournalEntryTransactions.Credit = item.Credit;
                        objJournalEntryTransactions.Debit = item.Debit;
                        db.JournalEntryTransactions.Add(objJournalEntryTransactions);
                    }
                    else
                    {
                        objJournalEntryTransactions = db.JournalEntryTransactions.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (objJournalEntryTransactions != null)
                        {
                            objJournalEntryTransactions.AccountId = item.AccountId;
                            objJournalEntryTransactions.Credit = item.Credit;
                            objJournalEntryTransactions.Debit = item.Debit;
                        }
                    }
                    Count = db.SaveChanges();
                }

            }
            return Count;
        }


        public object GetJournalEntryList(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var query = (from ac in db.JournalEntry.Where(s => s.IsDelete == false)
                             join co in db.Branch on ac.BranchId equals co.BranchId
                             select new
                             {
                                 Id = ac.Id,
                                 VoucherNo = ac.VoucherNo,
                                 PostingDate = ac.PostingDate,
                                 Prefix = ac.Prefix,
                                 EntryType = ac.EntryType,
                                 EntryTypeName = ac.EntryType.ToString().Replace("_", " "),
                                 VoucherNoString = ac.Prefix + " " + ac.VoucherNo.ToString(),
                                 BranchId = ac.BranchId,
                                 CreatedDate = ac.CreatedDate,
                                 TotalAmount = db.JournalEntryTransactions.Where(x => x.JournalEntryId == ac.Id).Select(x => x.Debit).Sum()
                             }).ToList();

                if (search.EntryType != 0)
                    query = query.Where(x => x.EntryType == (EntryType)search.EntryType).ToList();

                if (search.FromPostingDate != null)
                    query = query.Where(x => x.PostingDate >= search.FromPostingDate).ToList();

                if (search.ToPostingDate != null)
                    query = query.Where(x => x.PostingDate <= search.ToPostingDate).ToList();

                Branch objBranch = db.Branch.FirstOrDefault(x => x.BranchId == search.BranchId);
                if (objBranch != null)
                {
                    if(!objBranch.IsHO)
                    {
                        if (search.BranchId != null)
                            query = query.Where(x => x.BranchId == search.BranchId).ToList();
                    }
                }



                //if (search.AccountType != 0)
                //    query = query.Where(x => x.AccountType == (AccountType)search.AccountType).ToList();


                var acclist = query.OrderByDescending(x => x.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = acclist.Count(),
                    iTotalDisplayRecords = query.Count(),
                    aaData = acclist
                };
                return data;
            }
        }

        public int DeleteJournalEntry(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                int flag = 0;
                JournalEntry objJournalEntry = db.JournalEntry.Where(x => x.Id == Id).FirstOrDefault();
                if (objJournalEntry != null)
                {
                    objJournalEntry.IsDelete = true;
                    flag = db.SaveChanges();
                }
                return flag;
            }
        }


        public JournalEntryViewModel GetJournalEntryDetailsById(Guid Id)
        {
            JournalEntryViewModel objJournalEntryViewModel = new JournalEntryViewModel();
            using (var db = new BSCCSLEntity())
            {
                JournalEntry objJournalEntry = db.JournalEntry.Where(s => s.IsDelete == false && s.Id == Id).FirstOrDefault();
                if (objJournalEntry != null)
                {
                    objJournalEntryViewModel.EntryType = objJournalEntry.EntryType;
                    objJournalEntryViewModel.Description = objJournalEntry.Description;
                    objJournalEntryViewModel.ReferenceNo = objJournalEntry.ReferenceNo;
                    objJournalEntryViewModel.ReferenceDate = objJournalEntry.ReferenceDate;
                    objJournalEntryViewModel.PostingDate = objJournalEntry.PostingDate;
                    objJournalEntryViewModel.Id = objJournalEntry.Id;
                    List<JournalEntryTransactions> objJournalEntryTransactionsList = db.JournalEntryTransactions.Where(x => x.JournalEntryId == Id).ToList();
                    List<EntryList> objEntryListing = new List<EntryList>();
                    foreach (var item in objJournalEntryTransactionsList)
                    {
                        EntryList objEntryList = new EntryList();
                        objEntryList.Id = item.Id;
                        objEntryList.AccountId = item.AccountId;
                        objEntryList.Credit = item.Credit;
                        objEntryList.Debit = item.Debit;
                        objEntryList.Name = db.Accounts.Where(x => x.Id == item.AccountId).Select(x => x.Name).FirstOrDefault();

                        objEntryListing.Add(objEntryList);
                    }
                    objJournalEntryViewModel.EntryList = new List<EntryList>();
                    objJournalEntryViewModel.EntryList = objEntryListing;


                }

                return objJournalEntryViewModel;
            }
        }

    }
}

