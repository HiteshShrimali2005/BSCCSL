using BSCCSL.Models;
using BSCCSL.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class ChartsofAccountService
    {
        public object GetChartofAccountList(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var query = (from ac in db.Accounts.Where(s => s.IsDelete == false)
                             join co in db.Branch on ac.BranchId equals co.BranchId
                             select new
                             {
                                 Id = ac.Id,
                                 Name = ac.Name,
                                 ParentId = ac.ParentId,
                                 AccountType = ac.AccountType,
                                 BranchId = co.BranchId,
                                 BranchCode = co.BranchCode,
                                 BranchName = co.BranchName,
                                 ParentAccountName = db.Accounts.Where(s => s.IsDelete == false && s.Id == ac.ParentId).Select(x => x.Name).FirstOrDefault(),
                                 AccountTypeName = ac.AccountType.ToString().Replace("_", " "),
                                 CreateDate = ac.CreatedDate
                             }).ToList();

                if (!string.IsNullOrEmpty(search.ChartofAccountName))
                    query = query.Where(x => x.Name == search.ChartofAccountName).ToList();


                if (search.AccountType != 0)
                    query = query.Where(x => x.AccountType == (AccountType)search.AccountType).ToList();

                //if (search.BranchId != null)
                //    query = query.Where(x => x.BranchId == search.BranchId).ToList();

                if (!string.IsNullOrEmpty(search.RootAccount))
                {
                    Guid RootAccount = Guid.Empty;
                    Guid.TryParse(search.RootAccount.Split(':')[1], out RootAccount);
                    query = query.Where(x => x.ParentId == RootAccount).ToList();

                }

                if (!string.IsNullOrEmpty(search.ParentAccount))
                {
                    Guid ParentAccount = Guid.Empty;
                    Guid.TryParse(search.ParentAccount.Split(':')[1], out ParentAccount);
                    query = query.Where(x => x.Id == ParentAccount).ToList();
                }



                var acclist = query.OrderByDescending(c => c.CreateDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

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


        public object GetParentAccouts()
        {
            using (var db = new BSCCSLEntity())
            {
                var list = db.Accounts.Where(a => a.IsDelete == false && a.ParentId == null).Select(a => new { a.Id, a.Name }).ToList();
                return list;
            }
        }

        public object GetSubAccounts(Guid RootId)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = db.Accounts.Where(a => a.IsDelete == false && a.ParentId == RootId).Select(a => new { a.Id, a.Name }).ToList();
                return list;
            }
        }

        public object GetAccountDetailsById(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                var query = (from ac in db.Accounts.Where(s => s.IsDelete == false && s.Id == Id)
                             select new
                             {
                                 Id = ac.Id,
                                 Name = ac.Name,
                                 ParentId = ac.ParentId,
                                 AccountType = ac.AccountType,
                                 RootId = db.Accounts.Where(x => x.Id == ac.ParentId).Select(x => x.ParentId).FirstOrDefault(),
                                 ParentAccountName = db.Accounts.Where(s => s.IsDelete == false && s.Id == ac.ParentId).Select(x => x.Name).FirstOrDefault(),
                                 AccountTypeName = ac.AccountType.ToString().Replace("_", " "),
                             }).FirstOrDefault();
                return query;
            }
        }

        public bool SaveAccount(Accounts objAccounts, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (objAccounts.Id == Guid.Empty)
                {
                    objAccounts.CreatedBy = user.UserId;
                    objAccounts.CreatedDate = DateTime.Now;
                    db.Accounts.Add(objAccounts);
                }
                else
                {
                    var accountdata = db.Accounts.Where(a => a.Id == objAccounts.Id).FirstOrDefault();
                    accountdata.Name = objAccounts.Name;
                    accountdata.ParentId = objAccounts.ParentId;
                    accountdata.AccountType = objAccounts.AccountType;
                    accountdata.ModifiedBy = user.UserId;
                    accountdata.ModifiedDate = DateTime.Now;
                }

                db.SaveChanges();
                return true;
            }
        }


        public bool DeleteAccount(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                bool flag = false;
                Accounts objAccounts = db.Accounts.Where(x => x.Id == Id).FirstOrDefault();
                if (objAccounts != null)
                {
                    if (objAccounts.IsPermanent == null) //delete only when the account is not permanent
                    {
                        objAccounts.IsDelete = true;
                        db.SaveChanges();
                        flag = true;
                    }
                    else
                        flag = false;
                }
                return flag;
            }
        }

    }
}
