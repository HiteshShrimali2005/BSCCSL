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
    public class ExpenseService
    {
        public Expense GetExpenseById(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                Expense expense = db.Expense.Where(a => a.ExpenseId == Id).FirstOrDefault();
                return expense;
            }
        }

        public object GetExpenseList(DataTableSearch search, User user)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {

                var IsHO = db.Branch.Where(c => c.BranchId == search.BranchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    search.BranchId = null;
                }

                var list = (from e in db.Expense.Where(s => s.IsDelete == false)
                            join a in db.AccountsHead on e.AccountsHeadId equals a.AccountsHeadId
                            join c in db.User on e.CreatedBy equals c.UserId
                            join b in db.User on e.ApprovedBy equals b.UserId into approve
                            from app in approve.DefaultIfEmpty()
                            select new
                            {
                                e.ExpenseId,
                                e.ExpenseName,
                                e.ExpenseDate,
                                e.AccountsHeadId,
                                a.HeadName,
                                e.CreatedDate,
                                e.Description,
                                e.Amount,
                                e.BranchId,
                                RequestedBy = c.FirstName + " " + c.LastName,
                                ApprovedByName = app.FirstName + " " + app.LastName,
                                e.Status,
                                e.ApprovedDate,
                                e.ApprovedAmount,
                                e.FileName,
                                e.OrgFileName,
                                e.IsPaid,
                                e.PaidTo,
                                e.ModifiedDate
                            }).AsQueryable();

                if (search.BranchId != null)
                {
                    list = list.Where(c => c.BranchId == search.BranchId);
                }
                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    list = list.Where(c => c.ExpenseName.Contains(search.sSearch.Trim()));
                }
                if (search.fromDate != null)
                {
                    list = list.Where(c => DbFunctions.TruncateTime(c.ExpenseDate) >= DbFunctions.TruncateTime(search.fromDate));
                }
                if (search.toDate != null)
                {
                    list = list.Where(c => DbFunctions.TruncateTime(c.ExpenseDate) <= DbFunctions.TruncateTime(search.toDate));
                }

                var expenselist = list.OrderBy(c => c.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();
                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = expenselist.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = expenselist
                };
                return data;
            }
        }

        public bool SaveExpense(Expense expense, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (expense.ExpenseId == Guid.Empty)
                {
                    expense.CreatedBy = user.UserId;
                    db.Expense.Add(expense);
                }
                else
                {
                    var expensedata = db.Expense.Where(a => a.ExpenseId == expense.ExpenseId).FirstOrDefault();
                    expensedata.ExpenseName = expense.ExpenseName;
                    expensedata.Description = expense.Description;
                    expensedata.ExpenseDate = expense.ExpenseDate;
                    expensedata.Amount = expense.Amount;
                    expensedata.AccountsHeadId = expense.AccountsHeadId;
                    expensedata.BillDate = expense.BillDate;
                    if (!string.IsNullOrWhiteSpace(expense.FileName))
                    {
                        expensedata.FileName = expense.FileName;
                        expensedata.OrgFileName = expense.OrgFileName;
                    }

                    if (expense.Status != null)
                    {
                        expensedata.ApprovedBy = user.UserId;
                        expensedata.ApprovedDate = DateTime.Now;
                        expensedata.ApproveComment = expense.ApproveComment;
                        expensedata.ApprovedAmount = expense.ApprovedAmount;
                        expensedata.TransactionMode = expense.TransactionMode;
                        expensedata.ReferenceNumber = expense.ReferenceNumber;
                        expensedata.Status = expense.Status;
                    }
                    else
                    {
                        expensedata.ModifiedBy = user.UserId;
                        expensedata.ModifiedDate = DateTime.Now;
                    }
                }

                db.SaveChanges();
                return true;
            }
        }

        public bool DeleteExpenseById(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                var expense = new Expense { ExpenseId = Id, IsDelete = true };
                db.Expense.Attach(expense);
                db.Entry(expense).Property(s => s.IsDelete).IsModified = true;
                db.SaveChanges();
                return true;
            }
        }

        //For Get Customer Product Details 
        public object GetCustomerDataByProductId(string accNo)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {

                var CustomerProductDetails = (from cp in db.CustomerProduct.Where(x => x.AccountNumber.ToLower() == accNo.ToLower() && x.IsDelete == false && (x.ProductType == ProductType.Saving_Account || x.ProductType == ProductType.Current_Account))
                                              join cpd in db.CustomerPersonalDetail.Where(a => a.IsDelete == false) on cp.CustomerId equals cpd.CustomerId
                                              join c in db.Customer.Where(a => a.IsDelete == false) on cp.CustomerId equals c.CustomerId
                                              select new
                                              {
                                                  CustomerProductId = cp.CustomerProductId,
                                                  CustomerId = cp.CustomerId,
                                                  CustomerName = cpd.FirstName + " " + cpd.MiddleName + " " + cpd.LastName,
                                              }).ToList();



                return new
                {
                    CustomerProductDetails,
                };
            }

        }


        public bool PaidExpense(Expense expense, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (expense.ExpenseId == Guid.Empty)
                {
                    expense.CreatedBy = user.UserId;
                    db.Expense.Add(expense);
                }
                else
                {
                    var expensedata = db.Expense.Where(a => a.ExpenseId == expense.ExpenseId).FirstOrDefault();
                    
                    if(expense.PaidTo != null)
                    {
                        var CustomerDetails = db.CustomerProduct.Where(x => x.CustomerId == expense.PaidTo && x.ProductType == ProductType.Saving_Account && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                        TransactionService transactionService = new TransactionService();
                        Transactions transaction = new Transactions
                        {
                            CustomerProductId = CustomerDetails.CustomerProductId,
                            CustomerId = CustomerDetails.CustomerId,
                            Amount =(decimal) expense.ApprovedAmount,
                            Type = TypeCRDR.CR,
                            CreatedBy = user.UserId,
                            CreatedDate = DateTime.Now,
                            TransactionType = TransactionType.Expense,
                            DescIndentify = DescIndentify.Expense,
                            BranchId = CustomerDetails.BranchId,
                            Status = Status.Clear,
                            TransactionTime = DateTime.Now.AddSeconds(5)
                        };

                        Guid TransactionId = transactionService.InsertTransctionUsingSP(transaction);
                        expensedata.PaidTo = expense.PaidTo;
                    }
                    expensedata.IsPaid = true;
                    expensedata.ModifiedBy = user.UserId;
                    expensedata.ModifiedDate = DateTime.Now;
                }

                db.SaveChanges();
                return true;
            }
        }

    }
}
