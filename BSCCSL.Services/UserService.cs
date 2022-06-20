using BSCCSL.Models;
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
    public class UserService
    {

        public object Login(string username, string password)
        {
            using (var db = new BSCCSLEntity())
            {
                string pwddec = Decrypt("HI+zKCbwgXYuTNbM4/eRCA==");
                string pwd = Encrypt(pwddec);
                pwd = "HI+zKCbwgXYuTNbM4/eRCA==";
                var user = db.User.Where(u => u.UserCode == username && u.Password == pwd).FirstOrDefault();
                if (user != null)
                {
                    var data = new
                    {
                        UserId = user.UserId,
                        BranchId = user.BranchId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role,
                        RoleName = Enum.GetName(typeof(Role), user.Role)
                    };
                    return data;
                }
                else
                {
                    return null;
                }
            }
        }

        public object UserRegister(User user, object p)
        {
            throw new NotImplementedException();
        }

        public Guid UserRegister(User user, User userLog)
        {
            using (var db = new BSCCSLEntity())
            {
                //var branchcode = db.Branch.Where(x => x.BranchId == user.BranchId).Select(x => x.BranchCode).FirstOrDefault();
                Guid rankId = db.AgentRank.Where(a => a.Rank == "Agent").Select(a => a.RankId).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(user.SavingAccountNo))
                {
                    user.CustomerId = db.CustomerProduct.Where(a => a.AccountNumber == user.SavingAccountNo.Trim()).Select(a => a.CustomerId).FirstOrDefault();
                }

                if (user.UserId == Guid.Empty)
                {
                    Setting setting = db.Setting.Where(a => a.SettingName == "UserCode").FirstOrDefault();


                    user.UserCode = setting.Value.ToString();
                    string password = Encrypt(user.Password);
                    user.Password = password;
                    user.CreatedDate = DateTime.Now;

                    if (user.Role == Role.Agent)
                    {
                        user.RankId = rankId;
                    }
                    db.User.Add(user);
                    db.SaveChanges();

                    int usercode = Convert.ToInt32(setting.Value) + 1;
                    setting.Value = usercode.ToString().PadLeft(2, '0');
                    db.SaveChanges();
                }
                else
                {
                    var userdata = db.User.Where(a => a.UserId == user.UserId).FirstOrDefault();
                    userdata.FirstName = user.FirstName;
                    userdata.LastName = user.LastName;
                    userdata.DateOfBirth = user.DateOfBirth;
                    userdata.PhoneNumber = user.PhoneNumber;
                    userdata.Gender = user.Gender;
                    userdata.Address = user.Address;
                    userdata.UserName = user.UserName;
                    if (user.CustomerId != null && user.CustomerId != Guid.Empty)
                    {
                        userdata.CustomerId = user.CustomerId;
                    }
                    // userdata.Password = user.Password;

                    if (!string.IsNullOrEmpty(user.Password))
                    {
                        userdata.Password = Encrypt(user.Password);
                    }

                    if (user.Role == Role.Agent)
                    {
                        userdata.RankId = rankId;
                    }
                    userdata.Role = user.Role;
                    userdata.EmployeeId = user.EmployeeId;
                    userdata.IsActive = user.IsActive;
                    if (user.Role == Role.Agent)
                        userdata.IsExecutive = user.IsExecutive;
                    else
                        userdata.IsExecutive = false;
                    userdata.BranchId = user.BranchId;
                    userdata.ModifiedBy = userLog.UserId;
                    userdata.ModifiedDate = DateTime.Now;
                }
                db.SaveChanges();
                return user.UserId;
            }
        }

        public object GetUserList(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = db.User.Include("Branch").Where(s => s.IsDelete == false).AsQueryable();
                if (search.BranchId.HasValue)
                {
                    list = list.Where(s => s.BranchId == search.BranchId);
                }

                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    list = list.Where(c => c.FirstName.Contains(search.sSearch.Trim()) || c.LastName.Contains(search.sSearch.Trim()) || c.Gender.Contains(search.sSearch.Trim()) || c.UserCode.Contains(search.sSearch.Trim()) || c.PhoneNumber.Contains(search.sSearch.Trim()));
                }
                if (!string.IsNullOrEmpty(search.role))
                {
                    list = list.Where(c => c.Role.ToString().Contains(search.role.Trim()));
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

        public object GetAllUser(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                //var userList = db.User.Where(c => c.Role == Role.Agent && c.IsDelete == false).ToList();
                //return userList;
                var query = (from ut in db.User.Where(s => s.IsDelete == false && s.FirstName == search.FirstName && s.Role == Role.Agent)
                             join co in db.Branch on ut.BranchId equals co.BranchId
                             select new
                             {
                                 UserId = ut.UserId,
                                 //FirstName = ut.FirstName,
                                 //LastName = ut.LastName,
                                 AgentName = ut.FirstName + " " + ut.LastName,
                                 PhoneNumber = ut.PhoneNumber,
                                 AgentCode = ut.UserCode,
                                 BranchId = co.BranchId,
                                 BranchCode = co.BranchCode,
                                 BranchName = co.BranchName,

                             }).ToList();

                var employee = query.OrderBy(c => c.UserId).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = employee.Count(),
                    iTotalDisplayRecords = query.Count(),
                    aaData = employee
                };
                return data;
            }
        }

        public User GetUserDataById(Guid Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var user = db.User.Where(c => c.UserId == Id && c.IsDelete == false).FirstOrDefault();
                return user;
            }
        }

        public bool DeleteUser(Guid Id)
        {
            try
            {

                using (var db = new BSCCSLEntity())
                {
                    var user = db.User.Where(x => x.UserId == Id).FirstOrDefault();
                    user.IsDelete = true;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {

            }
            return false;
        }

        public object UserChangePassword(UserPassworData User)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var user = db.User.Where(x => x.UserId == User.UserId && x.IsActive).FirstOrDefault();
                    user.Password = Decrypt(user.Password);
                    if (user.Password == User.OldPassword)
                    {
                        user.Password = Encrypt(User.NewPassword);
                        db.SaveChanges();
                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            catch (Exception ex)
            {

            }
            return null;
        }

        public object GetAllEmployee()
        {
            using (var db = new BSCCSLEntity())
            {
                var employeedata = db.User.Where(u => u.IsActive == true && u.IsDelete == false && u.Role != Role.Agent).ToList();
                return employeedata;
            }
        }

        public object GetAgentList(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = db.User.Include("Branch").Where(s => s.IsDelete == false && s.IsActive == true && s.Role == Role.Agent).AsQueryable();
                if (search.BranchId.HasValue)
                {
                    list = list.Where(s => s.BranchId == search.BranchId);
                }

                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    list = list.Where(c => c.FirstName.Contains(search.sSearch.Trim()) || c.LastName.Contains(search.sSearch.Trim()) || c.Gender.Contains(search.sSearch.Trim()) || c.UserCode.Contains(search.sSearch.Trim()) || c.PhoneNumber.Contains(search.sSearch.Trim()) || c.Role.ToString().Contains(search.sSearch.Trim()));
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

        public object GetScreeSalesList(DataTableSearch search)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = db.User.Include("Branch").Where(s => s.IsDelete == false && s.IsActive == true && s.Role == Role.Scree_Sales).AsQueryable();
                if (search.BranchId.HasValue)
                {
                    list = list.Where(s => s.BranchId == search.BranchId);
                }

                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    list = list.Where(c => c.FirstName.Contains(search.sSearch.Trim()) || c.LastName.Contains(search.sSearch.Trim()) || c.Gender.Contains(search.sSearch.Trim()) || c.UserCode.Contains(search.sSearch.Trim()) || c.PhoneNumber.Contains(search.sSearch.Trim()) || c.Role.ToString().Contains(search.sSearch.Trim()));
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

        public object GetAgentListByBranchId(Guid? Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = db.User.Where(s => s.IsDelete == false && s.IsActive == true && s.Role == Role.Agent).Select(s => new { AgentId = s.UserId, AgentName = s.FirstName + " " + s.LastName, BranchId = s.BranchId, AgentCode = s.UserCode }).AsQueryable();
                if (Id != null)
                {
                    var IsHO = db.Branch.Where(c => c.BranchId == Id).Select(s => s.IsHO).FirstOrDefault();
                    if (IsHO)
                    {
                        return list.ToList();
                    }
                    else
                    {
                        var agnetList = list.Where(a => a.BranchId == Id).ToList();
                        return agnetList;
                    }
                }
                else
                {
                    return list.ToList();
                }
            }
        }

        public object GetAgentRankList(Guid? Id)
        {
            using (BSCCSLEntity db = new BSCCSLEntity())
            {
                var list = (from a in db.AgentRank.Where(s => s.Rank != "Agent")
                            join u in db.AgentHierarchy.Where(b => Id != null ? b.UserId == Id : true) on a.RankId equals u.RankId into rank
                            from agentrank in rank.DefaultIfEmpty()
                            select new
                            {
                                a.RankId,
                                a.Rank,
                                a.Order,
                                AgentHierarchyId = agentrank.AgentHierarchyId != null ? agentrank.AgentHierarchyId : Guid.Empty,
                                UserId = agentrank.UserId != null ? agentrank.UserId : Guid.Empty,
                                EmpAgentId = agentrank.EmpAgentId != null ? agentrank.EmpAgentId : Guid.Empty,
                            }).ToList();

                return list;
            }
        }

        public object SaveAgentHierarchy(List<AgentHierarchy> listagentHierarchy, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                foreach (var hierarchy in listagentHierarchy)
                {
                    if (hierarchy.AgentHierarchyId == Guid.Empty && hierarchy.EmpAgentId != Guid.Empty)
                    {
                        hierarchy.CreatedBy = user.UserId;
                        db.AgentHierarchy.Add(hierarchy);
                    }
                    else if (hierarchy.AgentHierarchyId != Guid.Empty)
                    {
                        var agentHierarchy = db.AgentHierarchy.Where(a => a.AgentHierarchyId == hierarchy.AgentHierarchyId).FirstOrDefault();
                        agentHierarchy.RankId = hierarchy.RankId;
                        agentHierarchy.EmpAgentId = hierarchy.EmpAgentId;
                        agentHierarchy.ModifiedBy = user.UserId;
                        agentHierarchy.ModifiedDate = DateTime.Now;
                    }
                    db.SaveChanges();
                }
                return true;
            }
        }

        public static string Encrypt(string toEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            // Get the key from config file

            string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //Always release the resources and flush data
            // of the Cryptographic service provide. Best Practice

            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));

            //if hashing was used get the hash code with regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //release any resource held by the MD5CryptoServiceProvider

            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public bool GetUsersEmailId(User userdata)
        {
            User user = new User();
            using (var db = new BSCCSLEntity())
            {
                user = db.User.Where(x => x.UserName == userdata.UserName && !x.IsDelete).FirstOrDefault();

            }
            if (user != null)
            {
                if (userdata.UserId == Guid.Empty)
                {
                    return false;
                }
                else if (user.UserId == userdata.UserId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
