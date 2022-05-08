using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class AccountsHeadService
    {
        public AccountsHead GetaccountsheadDataById(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                AccountsHead accountshead = db.AccountsHead.Where(a => a.AccountsHeadId == Id).FirstOrDefault();
                return accountshead;
            }
        }

        public object GetAccountListforParent()
        {
            using (var db = new BSCCSLEntity())
            {
                var list = db.AccountsHead.Where(a => a.IsDelete == false && a.ParentHead == null).Select(a => new { a.AccountsHeadId, a.HeadName }).ToList();
                return list;
            }
        }

        public object GetAccountsHead(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = (from a1 in db.AccountsHead.Where(s => s.IsDelete == false)
                            join a2 in db.AccountsHead on a1.ParentHead equals a2.AccountsHeadId into self
                            from a3 in self.DefaultIfEmpty()
                            select new
                            {
                                a1.AccountsHeadId,
                                a1.HeadName,
                                a1.HeadType,
                                HeadTypeName = ((HeadType)a1.HeadType).ToString(),
                                a1.ParentHead,
                                ParentHeadName = a3.HeadName,
                                a1.CreatedDate,
                                a1.Description
                            }).AsQueryable();

                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    list = list.Where(c => c.HeadName.Contains(search.sSearch.Trim()));
                }

                var headList = list.OrderBy(c => c.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();
                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = headList.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = headList
                };
                return data;
            }
        }

        public bool SaveAccountsHead(AccountsHead accountsHead, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (accountsHead.AccountsHeadId == Guid.Empty)
                {
                    accountsHead.CreatedBy = user.UserId;
                    db.AccountsHead.Add(accountsHead);
                }
                else
                {
                    var accountheaddata = db.AccountsHead.Where(a => a.AccountsHeadId == accountsHead.AccountsHeadId).FirstOrDefault();
                    accountheaddata.HeadName = accountsHead.HeadName;
                    accountheaddata.Description = accountsHead.Description;
                    accountheaddata.HeadType = accountsHead.HeadType;
                    accountheaddata.ParentHead = accountsHead.ParentHead;
                    accountheaddata.ModifiedBy = user.UserId;
                    accountheaddata.ModifiedDate = DateTime.Now;
                }

                db.SaveChanges();
                return true;
            }
        }

        public bool DeleteAccountsHead(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                var accounthead = new AccountsHead { AccountsHeadId = Id, IsDelete = true };
                db.AccountsHead.Attach(accounthead);
                db.Entry(accounthead).Property(s => s.IsDelete).IsModified = true;
                db.SaveChanges();
                return true;
            }
        }

        public object GetHeadWiseAccountsHead(HeadType head)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = db.AccountsHead.Where(a => a.IsDelete == false && a.HeadType == head && a.ParentHead == null).Select(a => new { a.AccountsHeadId, a.HeadName }).ToList();
                return list;
            }
        }

        public object GetSubHead(HeadType head)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = db.AccountsHead.Where(a => a.IsDelete == false && a.HeadType == head && a.ParentHead != null).Select(a => new { a.AccountsHeadId, a.HeadName }).ToList();
                return list;
            }
        }
    }
}
