using BSCCSL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class BranchService
    {

        public List<Branch> GetAllBranch(Guid userId)
        {
            using (var db = new BSCCSLEntity())
            {

                List<Branch> list = new List<Branch>();

                Role role = db.User.Where(a => a.UserId == userId).Select(a => a.Role).FirstOrDefault(); ;

                if (role == Role.Admin)
                {
                    list = db.Branch.Where(a => a.IsDelete == false).ToList();
                }
                else
                {
                    list = (from b in db.Branch.Where(a => a.IsDelete == false)
                            join u in db.User on b.BranchId equals u.BranchId
                            where u.UserId == userId
                            select b).ToList();
                }
                return list;
            }
        }

        public Branch GetBranchDataById(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                Branch Branch = db.Branch.Where(a => a.BranchId == Id).FirstOrDefault();
                return Branch;
            }
        }

        public object GetAllBranchData(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = db.Branch.Where(s => s.IsDelete == false).AsQueryable();

                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    list = list.Where(c => c.BranchName.Contains(search.sSearch.Trim()) || c.BranchAddress.Contains(search.sSearch.Trim()) || c.BranchCode.Contains(search.sSearch.Trim()) || c.RegistrationNo.Contains(search.sSearch.Trim()));
                }

                var BranchList = list.OrderBy(c => c.CreatedDate).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = BranchList.Count(),
                    iTotalDisplayRecords = list.Count(),
                    aaData = BranchList
                };

                return data;
            }
        }

        public bool BranchRegister(Branch branch, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (branch.BranchId == Guid.Empty)
                {
                    var branchCode = db.Branch.Where(x => x.BranchCode == branch.BranchCode).ToList();

                    if (branchCode.Count == 0 || branchCode[0].BranchId == branch.BranchId)
                    {

                        Setting setting = db.Setting.Where(a => a.SettingName == "BranchCode").FirstOrDefault();
                        branch.BranchCode = setting.Value.ToString();
                        db.Branch.Add(branch);

                        int branchcode = Convert.ToInt32(setting.Value) + 1;
                        setting.Value = branchcode.ToString().PadLeft(2, '0');
                        db.SaveChanges();


                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                else
                {
                    var branchdata = db.Branch.Where(a => a.BranchId == branch.BranchId).FirstOrDefault();

                    branchdata.BranchName = branch.BranchName;
                    branchdata.BranchAddress = branch.BranchAddress;
                    branchdata.BranchCode = branch.BranchCode;
                    branchdata.RegistrationNo = branch.RegistrationNo;
                    branchdata.IsHO = branch.IsHO;
                    branchdata.IsDelete = branch.IsDelete;
                    branchdata.IsActive = branch.IsActive;
                    branchdata.ModifiedBy = user.UserId;
                    branchdata.ModifiedDate = DateTime.Now;
                    branchdata.BranchPhone = branch.BranchPhone;
                    db.SaveChanges();
                    return true;
                }
            }
        }

        public bool DeleteBranch(Guid Id)
        {
            try
            {

                using (var db = new BSCCSLEntity())
                {
                    var branch = db.Branch.Where(x => x.BranchId == Id).FirstOrDefault();
                    branch.IsDelete = true;
                    db.Entry(branch).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {

            }
            return false;
        }
        //else if (HOactiveBranch[0].BranchId == branchdata.BranchId)
        //                {
        //                    return true;
        //                }

        public bool CheckHO(Branch branchdata)
        {
            Branch HO = new Branch();

            using (var db = new BSCCSLEntity())
            {
                if (branchdata.IsHO == true)
                {
                    var HOactiveBranch = db.Branch.Where(x => x.IsHO == true).ToList();

                    if (HOactiveBranch.Count == 1)
                    {
                        if (branchdata.BranchId == Guid.Empty)
                        {
                            return false;
                        }
                        else if (branchdata.BranchId != Guid.Empty)
                        {
                            if (HOactiveBranch[0].BranchId == branchdata.BranchId)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                {
                    return true;
                }
            }
        }

    }
}
