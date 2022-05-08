using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class ProductService
    {
        public Product GetProductDataById(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                Product product = db.Product.Where(a => a.ProductId == Id).FirstOrDefault();
                return product;
            }
        }

        public object GetAllProductData(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = db.Product.Where(s => s.IsDelete == false).AsQueryable();
                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    list = list.Where(c => c.ProductName.Contains(search.sSearch.Trim()) || c.ProductType.ToString().Trim().Contains(search.sSearch.Trim()));

                    //c.ProductType.Contains(search.sSearch.Trim())
                }
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

        public bool ProductRegister(Product product, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (product.ProductId == Guid.Empty)
                {
                    string productcode = db.Setting.Where(p => p.SettingName == product.ProductTypeName).Select(a => a.Value).FirstOrDefault();

                    if (!string.IsNullOrEmpty(productcode))
                    {
                        var setting = db.Setting.Where(p => p.SettingName == productcode).FirstOrDefault();

                        if (setting != null)
                        {
                            product.ProductCode = productcode + setting.Value;
                            db.Product.Add(product);

                            int saaccode = Convert.ToInt32(setting.Value) + 1;
                            setting.Value = saaccode.ToString().PadLeft(2, '0');
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    var productdate = db.Product.Where(a => a.ProductId == product.ProductId).FirstOrDefault();
                    productdate.ProductName = product.ProductName;
                    productdate.ProductType = product.ProductType;
                    productdate.InterestRate = product.InterestRate;
                    productdate.InterestType = product.InterestType;
                    productdate.Frequency = product.Frequency;
                    productdate.PaymentType = product.PaymentType;
                    productdate.StartDate = product.StartDate;
                    productdate.EndDate = product.EndDate;
                    productdate.IsDelete = product.IsDelete;
                    productdate.IsActive = product.IsActive;
                    productdate.ModifiedBy = user.UserId;
                    productdate.ModifiedDate = DateTime.Now;
                    productdate.LatePaymentFees = product.LatePaymentFees;
                    productdate.TimePeriod = product.TimePeriod;
                    productdate.LoanTypeId = product.LoanTypeId;
                    productdate.NoOfMonthsORYears = product.NoOfMonthsORYears;
                }
                db.SaveChanges();
                return true;
            }
        }

        public bool DeleteProduct(Guid Id)
        {
            try
            {

                using (var db = new BSCCSLEntity())
                {
                    var product = db.Product.Where(x => x.ProductId == Id).FirstOrDefault();
                    product.IsDelete = true;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {

            }
            return false;
        }

        public object SaveTerm(Term term)
        {
            using (var db = new BSCCSLEntity())
            {
                if (term.TermId == Guid.Empty)
                {
                    db.Term.Add(term);
                }
                else
                {
                    var termdata = db.Term.Where(a => a.TermId == term.TermId).FirstOrDefault();

                    termdata.From = term.From;
                    termdata.To = term.To;
                    termdata.TimePeriod = term.TimePeriod;
                    termdata.InterestRate = term.InterestRate;
                    termdata.TotalFrom = term.TotalFrom;
                    termdata.TotalTo = term.TotalTo;
                    termdata.ModifiedBy = term.ModifiedBy;
                    termdata.ModifiedDate = DateTime.Now;
                }
                db.SaveChanges();
                return term;
            }
        }

        public object GetTermList(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = db.Term.Where(s => s.IsDelete == false).AsQueryable();
                if (!string.IsNullOrEmpty(search.sSearch))
                {

                    list = list.Where(c => c.From.ToString().Contains(search.sSearch.Trim().ToString()) || c.To.ToString().Contains(search.sSearch.Trim().ToString()) || c.TimePeriod.ToString().Contains(search.sSearch.Trim().ToString()));
                }
                var userList = list.OrderBy(c => c.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();
                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = userList.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = userList
                };
                return data;
            }
        }

        public Term GetTermDataById(Guid Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var term = db.Term.Where(c => c.TermId == Id && c.IsDelete == false).FirstOrDefault();

                return term;
            }
        }

        public bool DeleteTerm(Guid Id)
        {
            try
            {

                using (var db = new BSCCSLEntity())
                {
                    var term = db.Term.Where(x => x.TermId == Id).FirstOrDefault();
                    term.IsDelete = true;
                    db.Entry(term).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {

            }
            return false;
        }

        public object GetLoanType()
        {
            using (var db = new BSCCSLEntity())
            {
                var loanTypeList = (from c in db.LookupCategory.Where(x => x.CategoryName == "LoanType")
                                    join l in db.Lookup on c.LookupCategoryId equals l.LookupCategoryId
                                    select new
                                    {
                                        l.Name,
                                        l.LookupId
                                    }).ToList();

                return loanTypeList;

            }
        }
    }
}
