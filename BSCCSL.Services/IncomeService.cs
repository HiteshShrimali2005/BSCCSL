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
    public class IncomeService
    {
        public Income GetIncomeById(Guid Id)
        {
            Income income = new Income();
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    income = db.Income.Where(a => a.IncomeId == Id).FirstOrDefault();
                    return income;
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return income;
            }
        }

        public object GetIncomeList(DataTableSearch search, User user)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {

                var IsHO = db.Branch.Where(c => c.BranchId == search.BranchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    search.BranchId = null;
                }

                var list = (from e in db.Income.Where(s => s.IsDelete == false)
                            join a in db.AccountsHead on e.AccountsHeadId equals a.AccountsHeadId
                            join c in db.User on e.CreatedBy equals c.UserId
                            select new
                            {
                                e.IncomeId,
                                e.IncomeName,
                                e.IncomeDate,
                                e.AccountsHeadId,
                                a.HeadName,
                                e.CreatedDate,
                                e.Description,
                                e.Amount,
                                e.BranchId,
                                RequestedBy = c.FirstName + " " + c.LastName,
                                e.ModifiedDate
                            }).AsQueryable();

                if (search.BranchId != null)
                {
                    list = list.Where(c => c.BranchId == search.BranchId);
                }
                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    list = list.Where(c => c.IncomeName.Contains(search.sSearch.Trim()));
                }
                if (search.fromDate != null)
                {
                    list = list.Where(c => DbFunctions.TruncateTime(c.IncomeDate) >= DbFunctions.TruncateTime(search.fromDate));
                }
                if (search.toDate != null)
                {
                    list = list.Where(c => DbFunctions.TruncateTime(c.IncomeDate) <= DbFunctions.TruncateTime(search.toDate));
                }

                var incomelist = list.OrderBy(c => c.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();
                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = incomelist.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = incomelist
                };
                return data;
            }
        }

        public bool SaveIncome(Income income, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (income.IncomeId == Guid.Empty)
                {
                    income.CreatedBy = user.UserId;
                    db.Income.Add(income);
                }
                else
                {
                    var incomedata = db.Income.Where(a => a.IncomeId == income.IncomeId).FirstOrDefault();
                    incomedata.IncomeName = income.IncomeName;
                    incomedata.Description = income.Description;
                    incomedata.IncomeDate = income.IncomeDate;
                    incomedata.Amount = income.Amount;
                    incomedata.ReferenceNumber = income.ReferenceNumber;
                    incomedata.AccountsHeadId = income.AccountsHeadId;
                    incomedata.TransactionMode = income.TransactionMode;
                }

                db.SaveChanges();
                return true;
            }
        }

        public bool DeleteincomeById(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                var income = new Income { IncomeId = Id, IsDelete = true };
                db.Income.Attach(income);
                db.Entry(income).Property(s => s.IsDelete).IsModified = true;
                db.SaveChanges();
                return true;
            }
        }


    }
}
