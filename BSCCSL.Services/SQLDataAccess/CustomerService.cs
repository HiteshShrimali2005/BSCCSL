using BSCCSL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services.SQLDataAccess
{
    public class CustomerServiceSQL
    {
        private readonly string _BSCCSLConnection;

        public CustomerServiceSQL()
        {
            this._BSCCSLConnection = System.Configuration.ConfigurationManager.ConnectionStrings["BSCCSLConnection"].ToString(); //
        }

        public object GetHolderDataSQL(string Account)
        {
            //var db1 = new BSCCSLEntity();
            //string connectionstring = db1.Database.Connection.ConnectionString;
            var data = new object();
            var customerpersonaldetail = new object();
            decimal TotalPendingAmount = 0;
            var holderDataResult = new HolderDataResult();

            using (IDbConnection connection = new SqlConnection(this._BSCCSLConnection))
            {
                var parameters = new DynamicParameters();
                var storedProcedureName = "GetHolderData";

                parameters.Add("@ACCOUNTNUMBER", Account);
                var dbResult = connection.Query(storedProcedureName, parameters, null, true, commandType: CommandType.StoredProcedure);

                foreach (var item in dbResult)
                {
                    if (item.status != null)
                    {
                        holderDataResult = new HolderDataResult()
                        {
                            ProductType = (ProductType)Enum.Parse(typeof(ProductType), Convert.ToString(item.PRODUCTTYPE)),
                            ProductTypeName = Convert.ToString(Enum.GetName(typeof(ProductType), item.PRODUCTTYPE)).Replace("_", " "),
                            BranchId = item.BRANCHID,
                            BranchName = item.BRANCHNAME,
                            CustomerId = item.CUSTOMERID,
                            CustomerProductId = item.CUSTOMERPRODUCTID,
                            Balance = item.BALANCE,
                            AccountNo = item.ACCOUNTNUMBER,
                            Amount = item.AMOUNT,
                            LastInstallmentDate = item.LASTINSTALLMENTDATE,
                            IsFreeze = item.ISFREEZE,
                            Status = (CustomerProductStatus)Enum.Parse(typeof(CustomerProductStatus), Convert.ToString(item.STATUS)),
                            ProductName = item.PRODUCTNAME,
                            ProductCode = item.PRODUCTCODE,
                            UnclearBalance = item.UNCLEARBALANCE
                        };
                    }
                    else {
                        holderDataResult = new HolderDataResult()
                        {
                            ProductType = (ProductType)Enum.Parse(typeof(ProductType), Convert.ToString(item.PRODUCTTYPE)),
                            ProductTypeName = Convert.ToString(Enum.GetName(typeof(ProductType), item.PRODUCTTYPE)).Replace("_"," "),                            
                            BranchId = item.BRANCHID,
                            BranchName = item.BRANCHNAME,
                            CustomerId = item.CUSTOMERID,
                            CustomerProductId = item.CUSTOMERPRODUCTID,
                            Balance = item.BALANCE,
                            AccountNo = item.ACCOUNTNUMBER,
                            Amount = item.AMOUNT,
                            LastInstallmentDate = item.LASTINSTALLMENTDATE,
                            IsFreeze = item.ISFREEZE,
                            //Status = (CustomerProductStatus)Enum.Parse(typeof(CustomerProductStatus), Convert.ToString(item.STATUS)),
                            ProductName = item.PRODUCTNAME,
                            ProductCode = item.PRODUCTCODE,
                            UnclearBalance = item.UNCLEARBALANCE
                        };
                    }
            }

            if (holderDataResult != null && holderDataResult.AccountNo.Length > 0)
            {
                using (var db = new BSCCSLEntity())
                {
                    customerpersonaldetail = (from cp in db.CustomerPersonalDetail.Where(a => !a.IsDelete && a.CustomerId == holderDataResult.CustomerId)
                                              join ca in db.CustomerAddress on cp.PersonalDetailId equals ca.PersonalDetailId
                                              orderby cp.CustomerId
                                              select new
                                              {
                                                  FirstName = cp.FirstName,
                                                  MiddleName = cp.MiddleName,
                                                  LastName = cp.LastName,
                                                  DOB = cp.DOB,
                                                  Sex = cp.Sex,
                                                  Address = (!string.IsNullOrEmpty(ca.DoorNo) ? ca.DoorNo + ", " : "") + (!string.IsNullOrEmpty(ca.BuildingName) ? ca.BuildingName + ", " : "") + (!string.IsNullOrEmpty(ca.PlotNo_Street) ? ca.PlotNo_Street + ", " : "") + (!string.IsNullOrEmpty(ca.Area) ? ca.Area + ", " : "") + (!string.IsNullOrEmpty(ca.City) ? ca.City + ", " : "") + (!string.IsNullOrEmpty(ca.District) ? ca.District + ", " : "") + (!string.IsNullOrEmpty(ca.State) ? ca.State + ", " : "") + (!string.IsNullOrEmpty(ca.Pincode) ? ca.Pincode : ""),
                                                  CustomerId = cp.CustomerId,
                                                  HolderPhoto = cp.HolderPhotograph,
                                                  Holdersign = cp.HolderSign
                                              }).ToList();
                }



                if (holderDataResult.ProductType == ProductType.Loan)
                {
                    decimal TotalEMIAmount = 0;
                    using (var db = new BSCCSLEntity())
                    {
                        TotalEMIAmount = db.Loan.Where(a => a.CustomerProductId == holderDataResult.CustomerProductId).Select(p => p.TotalAmountToPay.Value).FirstOrDefault();
                    }


                    int c = 0;
                    decimal TotalPaid = 0;
                    parameters = new DynamicParameters();
                    storedProcedureName = "GetCustomerRDPaymentData";

                    parameters.Add("@CustomerProductId", holderDataResult.CustomerProductId);
                    dbResult = connection.Query(storedProcedureName, parameters, null, true, commandType: CommandType.StoredProcedure);

                    foreach (var item in dbResult)
                    {
                        TotalPaid = item.Amount;
                    }

                    TotalPendingAmount = TotalEMIAmount - TotalPaid;
                    if (TotalPendingAmount < 0)
                    {
                        TotalPendingAmount = 0;
                    }
                }

            }
        }
        data = new
            {
                details = customerpersonaldetail,
                AccountDetail = holderDataResult,
                TotalLoanPendingAmount = TotalPendingAmount
    };

            return data;
        }
    }
}
