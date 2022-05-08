using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class ProductEnquiryService
    {
        public object GetProductEnquiryList(DataTableSearch search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                bool IsHo = db.Branch.Where(a => a.BranchId == search.BranchId).Select(a => a.IsHO).FirstOrDefault();

                if (IsHo && user.Role == Role.Admin)
                {
                    search.BranchId = null;
                }

                var list = (from e in db.ProductEnquiry
                            join c in db.Customer on e.CustomerId equals c.CustomerId into cust
                            from cu in cust.DefaultIfEmpty()
                            join p in db.CustomerPersonalDetail.AsEnumerable() on cu.CustomerId equals p.CustomerId into person
                            from persondetail in person.DefaultIfEmpty()
                            join u in db.User on e.CreatedBy equals u.UserId into usr
                            from us in usr.DefaultIfEmpty()
                            where cu.BranchId == search.BranchId || us.BranchId == search.BranchId
                            group new { persondetail, us } by new { e } into grp
                            select new
                            {
                                grp.Key.e.ProductEnquiryId,
                                ProductTypeName = grp.Key.e.ProductType.ToString(),
                                EnquiryStatusName = grp.Key.e.Status.ToString(),
                                grp.Key.e.FirstName,
                                grp.Key.e.LastName,
                                grp.Key.e.Comments,
                                grp.Key.e.ContactNumber,
                                grp.Key.e.EnquiryDate,
                                grp.Key.e.CustomerId,
                                grp.Key.e.ProductType,
                                grp.Key.e.Status,
                                StatusName = ((EnquiryStatus)grp.Key.e.Status).ToString(),
                                EnquirySourceName = grp.Key.e.EnquirySource.ToString(),
                                EnquiryBy = grp.Select(a => a.persondetail != null ? a.persondetail.FirstName + " " + a.persondetail.LastName : "").ToList(),
                                EnquiryByUser = grp.Select(a => a.us != null ? a.us.FirstName + " " + a.us.LastName : "")
                            })
                           .AsQueryable();


                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    list = list.Where(a => a.Comments.Contains(search.sSearch.Trim()) || a.EnquiryBy.Contains(search.sSearch.Trim()) || a.EnquiryByUser.Contains(search.sSearch.Trim()));
                }

                if (search.fromDate != null)
                {
                    list = list.Where(a => DbFunctions.TruncateTime(a.EnquiryDate) >= DbFunctions.TruncateTime(search.fromDate));
                }
                if (search.toDate != null)
                {
                    list = list.Where(a => DbFunctions.TruncateTime(a.EnquiryDate) >= DbFunctions.TruncateTime(search.toDate));
                }





                //.Select(p => new
                //{
                //    ProductEnquiryId = p.ProductEnquiryId,
                //    ProductTypeName = p.ProductTypeName,
                //    EnquiryStatusName = p.EnquiryStatusName,
                //    FirstName = p.FirstName,
                //    LastName = p.LastName,
                //    Comments = p.Comments,
                //    ContactNumber = p.ContactNumber,
                //    EnquiryDate = p.EnquiryDate,
                //    ProductType = p.ProductType,
                //    Status = p.Status,
                //    EnquirySourceName = p.EnquirySourceName,
                //    EnquiryBy = p.EnquiryByCustomer != null ? string.Join(",", p.EnquiryByCustomer) : p.EnquiryByUser

                //}).AsQueryable();



                var ProductEnquiryList = list.OrderBy(c => c.EnquiryDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = ProductEnquiryList.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = ProductEnquiryList
                };
                return data;
            }
        }

        //For web Product Enquiry
        public bool SaveProductEnquiry(ProductEnquiry productEnquiry, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                if (productEnquiry.ProductEnquiryId == Guid.Empty)
                {
                    productEnquiry.EnquirySource = EnquirySource.Web;
                    productEnquiry.Status = EnquiryStatus.New;
                    productEnquiry.CreatedBy = user.UserId;
                    //productEnquirydata.UserId = EnquirySource.Web;
                    db.ProductEnquiry.Add(productEnquiry);
                }
                else
                {
                    var productEnquirydata = db.ProductEnquiry.Where(a => a.ProductEnquiryId == productEnquiry.ProductEnquiryId).FirstOrDefault();

                    productEnquirydata.FirstName = productEnquiry.FirstName;
                    productEnquirydata.LastName = productEnquiry.LastName;
                    productEnquirydata.Comments = productEnquiry.Comments;
                    productEnquirydata.ContactNumber = productEnquiry.ContactNumber;
                    productEnquirydata.ModifiedBy = user.UserId;
                    productEnquirydata.Status = productEnquiry.Status;
                    productEnquirydata.ModifiedDate = DateTime.Now;
                }
                db.SaveChanges();
                return true;
            }
        }

        public ProductEnquiry GetProductEnquiryById(Guid productEnquiryId)
        {
            using (var db = new BSCCSLEntity())
            {
                var productEnquirydata = db.ProductEnquiry.Where(a => a.ProductEnquiryId == productEnquiryId).FirstOrDefault();

                return productEnquirydata;
            }
        }
    }
}
