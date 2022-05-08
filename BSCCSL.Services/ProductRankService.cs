using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class ProductRankService
    {
        public bool SaveProductRank(ProductRank productRank, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                //var branchcode = db.Branch.Where(x => x.BranchId == user.BranchId).Select(x => x.BranchCode).FirstOrDefault();

                if (productRank.ProductRankId == Guid.Empty)
                {
                    productRank.CreatedBy = user.UserId;
                    db.ProductRank.Add(productRank);
                }
                else
                {
                    var productRankData = db.ProductRank.Where(a => a.ProductRankId == productRank.ProductRankId).FirstOrDefault();
                    productRankData.ProductId = productRank.ProductId;
                    productRankData.Percentage = productRank.Percentage;
                    productRankData.ModifiedBy = user.UserId;
                    productRankData.ModifiedDate = DateTime.Now;
                    db.Entry(productRankData).State = EntityState.Modified;
                }
                db.SaveChanges();
                return true;
            }
        }

        public object GetProductRankList(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = (from r in db.ProductRank.Where(a => a.IsDelete == false)
                            join p in db.Product on r.ProductId equals p.ProductId
                            select new
                            {
                                r.ProductRankId,
                                r.ProductId,
                                r.Percentage,
                                r.CreatedDate,
                                p.ProductName,
                                ProductTypeName = ((ProductType)p.ProductType).ToString().Replace("_", " ")
                            }).AsQueryable();


                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    list = list.Where(c => c.ProductName.Contains(search.sSearch.Trim()) || c.ProductTypeName.Contains(search.sSearch.Trim()) || c.Percentage.ToString().Contains(search.sSearch.Trim()));
                }

                var productRankList = list.OrderBy(c => c.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = productRankList.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = productRankList
                };
                return data;
            }
        }

        public ProductRank GetProductRankDataById(Guid Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var productrank = db.ProductRank.Where(c => c.ProductRankId == Id && c.IsDelete == false).FirstOrDefault();
                return productrank;
            }
        }

        public bool DeleteProductRankById(Guid Id)
        {
            try
            {

                using (var db = new BSCCSLEntity())
                {
                    var Ref = new ProductRank() { ProductRankId = Id, IsDelete = true };
                    db.ProductRank.Attach(Ref);
                    db.Entry(Ref).Property(s => s.IsDelete).IsModified = true;
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
