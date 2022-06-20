using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSCCSL.Models;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace BSCCSL.Services
{
    public class LoanService
    {

        //TransactionService transactionService;

        //public LoanService()
        //{
        //    transactionService = new TransactionService();
        //}

        public object GetAllLookup()
        {
            using (var db = new BSCCSLEntity())
            {
                var Getlookup = (from l in db.Lookup.Where(l => l.IsDelete == false)
                                 join lc in db.LookupCategory on l.LookupCategoryId equals lc.LookupCategoryId
                                 select new ExpensesofEducation
                                 {
                                     LookupCategoryId = lc.LookupCategoryId,
                                     CategoryName = lc.CategoryName,
                                     LookupId = l.LookupId,
                                     Name = l.Name
                                 }).ToList();
                return Getlookup;
            }
        }

        public object SaveBusinessLoanData(BusinessLoan Businessloan, User user)
        {

            if (Businessloan.BusinessLoanId == Guid.Empty)
            {
                using (var db = new BSCCSLEntity())
                {
                    Businessloan.CreatedBy = Businessloan.CreatedBy;
                    db.BusinessLoan.Add(Businessloan);
                    db.SaveChanges();
                }
            }
            else
            {
                using (var bdb = new BSCCSLEntity())
                {
                    var businessdata = bdb.BusinessLoan.Where(x => x.BusinessLoanId == Businessloan.BusinessLoanId).FirstOrDefault();

                    businessdata.IsDeleted = false;
                    businessdata.ModifiedBy = user.UserId;
                    businessdata.ModifiedDate = DateTime.Now;
                    businessdata.Accetension = Businessloan.Accetension;
                    businessdata.AccomodationAddress1 = Businessloan.AccomodationAddress1;
                    businessdata.AccomodationAddress2 = Businessloan.AccomodationAddress2;
                    businessdata.AccomodationType = Businessloan.AccomodationType;
                    businessdata.AgricultureIfApplicable = Businessloan.AgricultureIfApplicable;
                    businessdata.AnualTurnOver = Businessloan.AnualTurnOver;
                    //businessdata.BusinessLoanId = Businessloan.BusinessLoanId;
                    businessdata.City = Businessloan.City;
                    businessdata.CompanyName = Businessloan.CompanyName;
                    businessdata.CompanyType = Businessloan.CompanyType;
                    businessdata.Country = Businessloan.Country;
                    businessdata.CountryCode = Businessloan.CountryCode;
                    businessdata.CreatedBy = Businessloan.CreatedBy;
                    //businessdata.CustomerId = new Guid("96962EFF-CD0B-E711-82E5-B5BD377C5F38");
                    //businessdata.LoanId = new Guid("0DD9734B-CE0B-E711-82E5-B5BD377C5F38");
                    businessdata.CustomerName = Businessloan.CustomerName;
                    businessdata.Email = Businessloan.Email;
                    businessdata.IndustryType = Businessloan.IndustryType;
                    businessdata.LandMark = Businessloan.LandMark;
                    businessdata.MobileNo = Businessloan.MobileNo;
                    businessdata.NumberOfYearsInBusiness = Businessloan.NumberOfYearsInBusiness;
                    businessdata.NumberOfYearsOfCurrentAddress = Businessloan.NumberOfYearsOfCurrentAddress;
                    businessdata.NumberOfYearsOfCurrentCity = Businessloan.NumberOfYearsOfCurrentCity;
                    businessdata.NumberOfYearsOfCurrentOrganization = Businessloan.NumberOfYearsOfCurrentOrganization;
                    businessdata.NumberOfYearsOfExperience = Businessloan.NumberOfYearsOfExperience;
                    businessdata.OfficeAccExtension = Businessloan.OfficeAccExtension;
                    businessdata.OfficeAccomodationAddress1 = Businessloan.OfficeAccomodationAddress1;
                    businessdata.OfficeAccomodationAddress2 = Businessloan.OfficeAccomodationAddress2;
                    businessdata.OfficeAccomodationType = Businessloan.OfficeAccomodationType;
                    businessdata.OfficeCity = Businessloan.OfficeCity;
                    businessdata.OfficeCountry = Businessloan.OfficeCountry;
                    businessdata.OfficeCountryCode = Businessloan.OfficeCountryCode;
                    businessdata.OfficeEmail = Businessloan.OfficeEmail;
                    businessdata.OfficeLandMark = Businessloan.OfficeLandMark;
                    businessdata.OfficeMobileNo = Businessloan.OfficeMobileNo;
                    businessdata.OfficePhoneNo = Businessloan.OfficePhoneNo;
                    businessdata.OfficePincode = Businessloan.OfficePincode;
                    businessdata.OfficePrimaryPostOfficeAddress = Businessloan.OfficePrimaryPostOfficeAddress;
                    businessdata.OfficeState = Businessloan.OfficeState;
                    businessdata.OfficeSTDCode = Businessloan.OfficeSTDCode;
                    businessdata.PANOrGRINumber = Businessloan.PANOrGRINumber;
                    businessdata.PhoneNo = Businessloan.PhoneNo;
                    businessdata.Pincode = Businessloan.Pincode;
                    businessdata.Position = Businessloan.Position;
                    businessdata.PrimaryPostOfficeAddress = Businessloan.PrimaryPostOfficeAddress;
                    businessdata.State = Businessloan.State;
                    businessdata.STDCode = Businessloan.STDCode;

                    bdb.Entry(businessdata).State = EntityState.Modified;

                    bdb.SaveChanges();
                }
            }
            return Businessloan;
        }

        public object SaveEconomicDetails(List<BusinessEconomicDetails> businessEconomicDetails, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                foreach (var eco in businessEconomicDetails)
                {
                    if (eco.BusinessEconomicDetailsId == Guid.Empty)
                    {
                        db.BusinessEconomicDetails.Add(eco);
                    }
                    else
                    {
                        var economicdata = db.BusinessEconomicDetails.Where(x => x.BusinessEconomicDetailsId == eco.BusinessEconomicDetailsId).FirstOrDefault();
                        economicdata.EconomicDetails = eco.EconomicDetails;
                        economicdata.BusinessLoanId = economicdata.BusinessLoanId;
                        economicdata.FY1 = eco.FY1;
                        economicdata.FY2 = eco.FY2;
                        economicdata.FY3 = eco.FY3;
                        economicdata.ModifiedBy = user.UserId;
                        economicdata.ModifiedDate = DateTime.Now;
                    }
                    db.SaveChanges();
                }
            }
            return businessEconomicDetails;

        }

        public object GetBusinessLoanById(Guid BusinessLoanId)
        {
            using (var db = new BSCCSLEntity())
            {
                var BusinessLaondata = db.BusinessLoan.Where(x => x.BusinessLoanId == BusinessLoanId).FirstOrDefault();
                return BusinessLaondata;
            }
        }

        public object SaveVehicleLoanData(VehicleLoan VehicleLoandata, User user)
        {
            if (VehicleLoandata.VehicleLoanId == Guid.Empty)
            {
                using (var db = new BSCCSLEntity())
                {
                    db.VehicleLoan.Add(VehicleLoandata);
                    db.SaveChanges();
                }
            }
            else
            {
                using (var bdb = new BSCCSLEntity())
                {
                    var VehileLoanData = bdb.VehicleLoan.Where(x => x.VehicleLoanId == VehicleLoandata.VehicleLoanId).FirstOrDefault();
                    VehicleLoandata.ModifiedDate = DateTime.Now.Date;

                    VehileLoanData.NewAssetType = VehicleLoandata.NewAssetType;
                    VehileLoanData.AssetLife = VehicleLoandata.AssetLife;
                    VehileLoanData.ProducerName = VehicleLoandata.ProducerName;
                    VehileLoanData.Category = VehicleLoandata.Category;
                    VehileLoanData.DealerName = VehicleLoandata.DealerName;
                    VehileLoanData.Model = VehicleLoandata.Model;
                    VehileLoanData.DealerCode = VehicleLoandata.DealerCode;
                    VehileLoanData.Xshowroomprice = VehicleLoandata.Xshowroomprice;
                    VehileLoanData.IfAnyOtherTax = VehicleLoandata.IfAnyOtherTax;
                    VehileLoanData.TotalOnroadPrice = VehicleLoandata.TotalOnroadPrice;
                    VehileLoanData.RegistrationCost = VehicleLoandata.RegistrationCost;
                    VehileLoanData.ModifiedBy = user.UserId;
                    VehileLoanData.Insurance = VehicleLoandata.Insurance;
                    VehileLoanData.AccetMacvariant = VehicleLoandata.AccetMacvariant;
                    VehileLoanData.IsDeleted = false;

                    bdb.Entry(VehileLoanData).State = EntityState.Modified;
                    bdb.SaveChanges();
                }

            }
            return VehicleLoandata;

        }

        public object GetVehicleLoanById(Guid VehicleLoanId)
        {
            using (var db = new BSCCSLEntity())
            {
                var VehicleLoanData = db.VehicleLoan.Where(x => x.VehicleLoanId == VehicleLoanId).FirstOrDefault();
                return VehicleLoanData;
            }
        }

        public object SaveLoan(Loan loan, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (loan.LoanId == Guid.Empty && loan.CustomerId != Guid.Empty)
                {
                    loan.LoanStatus = LoanStatus.Draft;
                    loan.LastPrincipalAmount = loan.LoanAmount;
                    db.Loan.Add(loan);
                    db.SaveChanges();

                    UpdateLoanStatus _loanStaus = new UpdateLoanStatus();
                    _loanStaus.LoanId = loan.LoanId;
                    _loanStaus.LoanStatus = LoanStatus.Draft;
                    _loanStaus.UpdatedBy = loan.CreatedBy ?? new Guid();
                    _loanStaus.Comment = null;
                    LoanStatus status = SaveUpdatedLoanStatus(_loanStaus);
                }
                else
                {
                    var Loandata = db.Loan.Where(a => a.LoanId == loan.LoanId).FirstOrDefault();

                    Loandata.DateofApplication = loan.DateofApplication;
                    Loandata.LoanType = loan.LoanType;
                    Loandata.ReasonForUse = loan.ReasonForUse;
                    Loandata.CustomerType = loan.CustomerType;
                    Loandata.PresentcustomerBank = loan.PresentcustomerBank;
                    Loandata.Place = loan.Place;
                    Loandata.LoanAmount = loan.LoanAmount;
                    Loandata.LoanIntrestRate = loan.LoanIntrestRate;
                    Loandata.InstallmentDate = loan.InstallmentDate;
                    Loandata.LastInstallmentDate = loan.InstallmentDate;
                    Loandata.ConsumerProductName = loan.ConsumerProductName;
                    //Loandata.ProcessingCharge = loan.ProcessingCharge;
                    //Loandata.ServiceTax = loan.ServiceTax;
                    Loandata.Term = loan.Term;
                    if (loan.Term == null)
                        Loandata.Term = "1";

                    Loandata.LastTenure = Convert.ToInt32(loan.Term);
                    if (Loandata.TotalCharges.HasValue)
                    {
                        Loandata.DisbursementAmount = loan.LoanAmount - Loandata.TotalCharges;
                        Loandata.LastPrincipalAmount = loan.LoanAmount + Loandata.TotalCharges;
                    }
                    else
                    {
                        Loandata.DisbursementAmount = loan.LoanAmount;
                        Loandata.LastPrincipalAmount = loan.LoanAmount;
                    }

                    Loandata.ModifiedBy = user.UserId;
                    Loandata.ModifiedDate = DateTime.Now;
                }

                if (!string.IsNullOrWhiteSpace(loan.Term) && loan.InstallmentDate != null && !loan.IsDisbursed)
                {
                    var customerProduct = new CustomerProduct()
                    {
                        CustomerProductId = loan.CustomerProductId,
                        NextInstallmentDate = loan.InstallmentDate,
                        DueDate = loan.InstallmentDate,
                        NoOfMonthsORYears = Convert.ToInt32(loan.Term),
                        TimePeriod = TimePeriod.Months,
                        MaturityDate = loan.InstallmentDate.Value.AddMonths(Convert.ToInt32(loan.Term) - 1),
                        TotalInstallment = Convert.ToInt32(loan.Term),
                        InterestRate = loan.LoanIntrestRate,
                        TotalDays = Convert.ToInt32((loan.InstallmentDate.Value - loan.InstallmentDate.Value.AddMonths(Convert.ToInt32(loan.Term) - 1)).TotalDays - 1),
                    };

                    db.CustomerProduct.Attach(customerProduct);
                    db.Entry(customerProduct).Property(s => s.NextInstallmentDate).IsModified = true;
                    db.Entry(customerProduct).Property(s => s.DueDate).IsModified = true;
                    db.Entry(customerProduct).Property(s => s.NoOfMonthsORYears).IsModified = true;
                    db.Entry(customerProduct).Property(s => s.TimePeriod).IsModified = true;
                    db.Entry(customerProduct).Property(s => s.TotalInstallment).IsModified = true;
                    db.Entry(customerProduct).Property(s => s.MaturityDate).IsModified = true;
                    db.Entry(customerProduct).Property(s => s.InterestRate).IsModified = true;
                    db.Entry(customerProduct).Property(s => s.TotalDays).IsModified = true;
                }
                db.SaveChanges();
                return loan.LoanId;
                //var result = SaveBorrower(loan.LoanId);
                //return result;
            }
        }

        public object SaveBorrower(List<Borrower> borrowers, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                foreach (var borrower in borrowers)
                {
                    if (borrower.BorrowerId != Guid.Empty)
                    {
                        var Loandata = db.Borrower.Where(a => a.BorrowerId == borrower.BorrowerId).FirstOrDefault();

                        Loandata.ModifiedBy = user.UserId;
                        Loandata.ModifiedDate = DateTime.Now;
                        Loandata.MartialStatus = borrower.MartialStatus;
                        Loandata.EducationDetail = borrower.EducationDetail;
                        Loandata.EmployementType = borrower.EmployementType;
                        Loandata.IncomeSource = borrower.IncomeSource;
                        Loandata.IfSalaried = borrower.IfSalaried;
                        Loandata.OrganisationNature = borrower.OrganisationNature;
                        Loandata.Proffesion = borrower.Proffesion;
                        Loandata.AccomodationType = borrower.AccomodationType;
                        Loandata.BusinessType = borrower.BusinessType;
                        Loandata.AccountType = borrower.AccountType;
                        Loandata.Caste = borrower.Caste;
                        Loandata.Category = borrower.Category;
                        Loandata.Former_OtherName = borrower.Former_OtherName;
                        Loandata.AnnualIncome = borrower.AnnualIncome;
                        Loandata.FamilyMember = borrower.FamilyMember;
                        Loandata.Employeeid = borrower.Employeeid;
                        Loandata.Department = borrower.Department;
                        Loandata.PrimaryPostOfficeAddress = borrower.PrimaryPostOfficeAddress;
                        Loandata.PersentAddressMonthYear = borrower.PersentAddressMonthYear;
                        Loandata.YearinCurrentCity = borrower.YearinCurrentCity;
                        Loandata.Country = borrower.Country;
                        Loandata.OfficePrimaryPostOfficeAddress = borrower.OfficePrimaryPostOfficeAddress;
                        Loandata.OfficePersentAddressMonthYear = borrower.OfficePersentAddressMonthYear;
                        Loandata.YearofExperience = borrower.YearofExperience;
                        Loandata.Company_firmName = borrower.Company_firmName;
                        Loandata.Position = borrower.Position;
                        Loandata.OfficeAddress1 = borrower.OfficeAddress1;
                        Loandata.OfficeAddress2 = borrower.OfficeAddress2;
                        Loandata.OfficeLandmark = borrower.OfficeLandmark;
                        Loandata.OfficeCity = borrower.OfficeCity;
                        Loandata.OfficeState = borrower.OfficeState;
                        Loandata.OfficePinCode = borrower.OfficePinCode;
                        Loandata.OfficeCountry = borrower.OfficeCountry;
                        Loandata.AccCountryCode = borrower.AccCountryCode;
                        Loandata.AccStdCode = borrower.AccStdCode;
                        Loandata.AccPhoneNum = borrower.AccPhoneNum;
                        Loandata.AccExtension = borrower.AccExtension;
                        Loandata.AccEmail = borrower.AccEmail;
                        Loandata.AccMobileNum = borrower.AccMobileNum;
                        Loandata.CompanyfirmAccDetail = borrower.CompanyfirmAccDetail;
                        Loandata.FirmBranch = borrower.FirmBranch;
                        Loandata.OrganizationAccNum = borrower.OrganizationAccNum;
                        Loandata.AccountOpenyear = borrower.AccountOpenyear;
                        Loandata.Accountodcclimit = borrower.Accountodcclimit;
                        Loandata.IssuerName1 = borrower.IssuerName1;
                        Loandata.CardNumber1 = borrower.CardNumber1;
                        Loandata.CreditLimit1 = borrower.CreditLimit1;
                        Loandata.IssuerName2 = borrower.IssuerName2;
                        Loandata.CradNum2 = borrower.CradNum2;
                        Loandata.CreditLimit2 = borrower.CreditLimit2;
                        Loandata.IsDelete = false;
                        db.SaveChanges();
                    }
                    else
                    {
                        var brwr = db.Borrower.Where(a => a.PersonalDetailId == borrower.PersonalDetailId && a.LoanId == borrower.LoanId).FirstOrDefault();
                        if (brwr == null)
                        {
                            db.Borrower.Add(borrower);
                        }
                        else
                        {
                            brwr.IsDelete = false;
                        }
                        db.SaveChanges();
                    }
                    if (borrower.LoanDetails != null)
                    {
                        if (borrower.LoanDetails.Count() > 0)
                        {
                            foreach (BorrowerLoanDetails _loanDetails in borrower.LoanDetails)
                            {
                                if (_loanDetails.BorrowerLoanId == new Guid())
                                {
                                    _loanDetails.LoanId = borrower.LoanId;
                                    _loanDetails.BorrowerId = borrower.BorrowerId;
                                    _loanDetails.CreatedBy = borrower.CreatedBy;
                                    _loanDetails.CreatedDate = borrower.CreatedDate;
                                    _loanDetails.ModifiedDate = DateTime.Now;
                                    _loanDetails.ModifiedBy = borrower.ModifiedBy;

                                    db.BorrowerLoanDetails.Add(_loanDetails);
                                }
                                else
                                {
                                    var LDetails = db.BorrowerLoanDetails.Where(s => s.BorrowerLoanId == _loanDetails.BorrowerLoanId).FirstOrDefault();
                                    LDetails.LoanId = _loanDetails.LoanId;
                                    LDetails.BorrowerId = _loanDetails.BorrowerId;
                                    LDetails.CreatedBy = _loanDetails.CreatedBy;
                                    LDetails.CreatedDate = _loanDetails.CreatedDate;
                                    LDetails.ModifiedDate = DateTime.Now;
                                    LDetails.ModifiedBy = user.UserId;
                                    LDetails.OrganizationName = _loanDetails.OrganizationName;
                                    LDetails.LoanType = _loanDetails.LoanType;
                                    LDetails.LoanAmount = _loanDetails.LoanAmount;
                                    LDetails.LoanLimit = _loanDetails.LoanLimit;
                                    LDetails.InstallmentAmount = _loanDetails.InstallmentAmount;
                                    LDetails.PaidInstallment = _loanDetails.PaidInstallment;

                                    db.Entry<BorrowerLoanDetails>(LDetails).State = EntityState.Modified;
                                }
                            }
                            db.SaveChanges();
                        }
                    }
                }

                return borrowers;
            }
        }

        public object SaveReferencer(Reference reference, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                if (reference.ReferenceId == Guid.Empty && reference.LoanId != Guid.Empty)
                {
                    reference.CreatedBy = reference.CreatedBy;
                    reference.DOB = reference.DOB;
                    db.Reference.Add(reference);
                    db.SaveChanges();
                }
                else
                {
                    var Referencerdata = db.Reference.Where(a => a.ReferenceId == reference.ReferenceId).FirstOrDefault();

                    Referencerdata.Title = reference.Title;
                    Referencerdata.Category = reference.Category;
                    Referencerdata.EducationDetail = reference.EducationDetail;
                    Referencerdata.IncomeSource = reference.IncomeSource;
                    Referencerdata.IfSalaried = Referencerdata.IfSalaried;
                    Referencerdata.OrganisationNature = Referencerdata.IfSalaried;
                    Referencerdata.Proffesion = reference.Proffesion;
                    Referencerdata.AccomodationType = reference.AccomodationType;
                    Referencerdata.BusinessType = reference.BusinessType;
                    Referencerdata.Caste = reference.Caste;
                    Referencerdata.MartialStatus = reference.MartialStatus;
                    Referencerdata.Former_OtherName = reference.Former_OtherName;
                    Referencerdata.AnnualIncome = reference.AnnualIncome;
                    Referencerdata.FamilyMember = reference.FamilyMember;
                    Referencerdata.MotherName = reference.MotherName;
                    Referencerdata.Employeeid = reference.Employeeid;
                    Referencerdata.Department = reference.Department;
                    Referencerdata.AadharNumber = reference.AadharNumber;
                    Referencerdata.PancardNumber = reference.PancardNumber;
                    Referencerdata.PrimaryPostOfficeAddress = reference.PrimaryPostOfficeAddress;
                    Referencerdata.PersentAddressMonthYear = reference.PersentAddressMonthYear;
                    Referencerdata.YearinCurrentCity = reference.YearinCurrentCity;
                    Referencerdata.Address1 = reference.Address1;
                    Referencerdata.Address2 = reference.Address2;
                    Referencerdata.Landmark = reference.Landmark;
                    Referencerdata.City = reference.City;
                    Referencerdata.State = reference.State;
                    Referencerdata.Pincode = reference.Pincode;
                    Referencerdata.Country = reference.Country;
                    Referencerdata.CountryCode = reference.CountryCode;
                    Referencerdata.StdCode = reference.StdCode;
                    Referencerdata.Extension = reference.Extension;
                    Referencerdata.Email = reference.Email;
                    Referencerdata.MobileNumber = reference.MobileNumber;
                    Referencerdata.FullName = reference.FullName;
                    Referencerdata.Sex = reference.Sex;
                    Referencerdata.TelephoneNo = reference.TelephoneNo;
                    Referencerdata.DOB = reference.DOB;
                    Referencerdata.ModifiedBy = user.UserId;
                    Referencerdata.ModifiedDate = DateTime.Now;
                }
                db.SaveChanges();
                return reference.LoanId;
            }
        }

        public object LoanList(DataTableSearch Search)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetAllLoanList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@AccountNumber", SqlDbType.NVarChar);
                paramAgentName.Value = (object)Search.sSearch ?? DBNull.Value;


                if (Search.LoanStatus == 0)
                    Search.LoanStatus = null;

                SqlParameter paramLoanStatus = cmdTimesheet.Parameters.Add("@LoanStatus", SqlDbType.Int);
                paramLoanStatus.Value = (object)Search.LoanStatus ?? DBNull.Value;


                //var IsHO = db.Branch.Where(c => c.BranchId == Search.BranchId).Select(s => s.IsHO).FirstOrDefault();
                //if (IsHO)
                //{

                //}
                //else
                //{
                //    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                //    paramBranchId.Value = Search.BranchId;
                //}


                SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                paramBranchId.Value = (object)Search.BranchId ?? DBNull.Value;

                SqlParameter paramGroupLoanId = cmdTimesheet.Parameters.Add("@GroupLoanId", SqlDbType.UniqueIdentifier);
                paramGroupLoanId.Value = (object)Search.GroupLoanId ?? DBNull.Value;

                SqlParameter paramRole = cmdTimesheet.Parameters.Add("@Role", SqlDbType.Int);
                paramRole.Value = (object)Search.role ?? DBNull.Value;

                SqlParameter paramStart = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                paramStart.Value = Search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.iDisplayLength;

                SqlParameter paramConsumerProductName = cmdTimesheet.Parameters.Add("@ConsumerProductName", SqlDbType.NVarChar);
                paramConsumerProductName.Value = (object)Search.ConsumerProductName ?? DBNull.Value;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<ListLoanDetails> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<ListLoanDetails>(reader).ToList();
                reader.NextResult();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object GroupLoanList(DataTableSearch Search, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetAllGroupLoanList", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                SqlParameter paramAgentName = cmdTimesheet.Parameters.Add("@GroupLoanNumber", SqlDbType.NVarChar);
                paramAgentName.Value = (object)Search.sSearch ?? DBNull.Value;

                var IsHO = db.Branch.Where(c => c.BranchId == Search.BranchId).Select(s => s.IsHO).FirstOrDefault();
                if (IsHO && user.Role == Role.Admin)
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = DBNull.Value;
                }
                else
                {
                    SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier);
                    paramBranchId.Value = Search.BranchId;
                }

                SqlParameter paramStart = cmdTimesheet.Parameters.Add("@Start", SqlDbType.Int);
                paramStart.Value = Search.iDisplayStart;

                SqlParameter paramLength = cmdTimesheet.Parameters.Add("@Length", SqlDbType.Int);
                paramLength.Value = Search.iDisplayLength;

                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                List<ListLoanDetails> rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<ListLoanDetails>(reader).ToList();
                reader.NextResult();

                int Count = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();

                var data = new
                {
                    sEcho = Search.sEcho,
                    iTotalRecords = rptlist.Count(),
                    iTotalDisplayRecords = Count,
                    aaData = rptlist,
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object LoanApprovalList(DataTableSearch search)
        {
            using (var db = new BSCCSLEntity())
            {
                var loandetails = (from cp in db.CustomerProduct.Where(x => x.IsDelete == false && x.IsActive == true && x.ProductType == ProductType.Loan && x.BranchId == search.BranchId).AsEnumerable()
                                   join tc in db.Loan.Where(x => x.IsDelete == false && x.LoanStatus != LoanStatus.Draft) on cp.CustomerProductId equals tc.CustomerProductId
                                   join l in db.Lookup on tc.LoanType equals l.LookupId
                                   select new
                                   {
                                       AccountNumber = cp.AccountNumber,
                                       ProductName = Enum.GetName(typeof(ProductType), cp.ProductType),
                                       IntrestRate = cp.InterestRate,
                                       LoanId = tc.LoanId,
                                       CustomerId = cp.CustomerId,
                                       ProductId = cp.CustomerProductId,
                                       LoanStatus = Enum.GetName(typeof(LoanStatus), tc.LoanStatus),
                                       LoanType = l.Name
                                   }).AsQueryable();

                if (!string.IsNullOrEmpty(search.sSearch))
                {
                    loandetails = loandetails.Where(c => c.AccountNumber.Contains(search.sSearch.Trim()) || c.ProductName.Contains(search.sSearch.Trim()));
                }
                var Loandata = loandetails.OrderBy(c => c.AccountNumber).Skip(search.iDisplayStart).Take(search.iDisplayLength).ToList();

                var data = new
                {
                    sEcho = search.sEcho,
                    iTotalRecords = Loandata.Count(),
                    iTotalDisplayRecords = loandetails.Count(),
                    aaData = Loandata
                };
                return data;
            }

        }

        public object GetReferencerDetailById(Guid ReferenceId)
        {
            using (var db = new BSCCSLEntity())
            {

                var Referencer = db.Reference.Where(r => r.ReferenceId == ReferenceId).FirstOrDefault();
                return Referencer;
            }
        }

        public object GetCustomerPersonalDetailById(Guid id, Guid? LoanId)
        {
            using (var db = new BSCCSLEntity())
            {
                var loandetail = (from pc in db.CustomerProduct.Where(x => x.IsDelete == false && x.CustomerProductId == id && x.IsActive == true)
                                  join c in db.Customer.Where(a => a.IsDelete == false) on pc.CustomerId equals c.CustomerId
                                  join cl in db.Loan on pc.CustomerProductId equals cl.CustomerProductId into lc
                                  from ld in lc.DefaultIfEmpty()
                                  join ls in db.UpdateLoanStatus on ld.LoanId equals ls.LoanId into lslist
                                  from ls in lslist.DefaultIfEmpty()
                                  join l in db.Lookup on ld.LoanType equals l.LookupId into type
                                  from l in type.DefaultIfEmpty()
                                  group new { ls } by new { pc, ld, c, l } into grplist
                                  select new
                                  {
                                      grplist.Key.pc.CustomerProductId,
                                      grplist.Key.pc.CustomerId,
                                      grplist.Key.c.ClienId,
                                      LoanIntrestRate = grplist.Key.ld != null ? grplist.Key.ld.LoanIntrestRate : grplist.Key.pc.InterestRate,
                                      LoanId = grplist.Key.ld != null ? grplist.Key.ld.LoanId : Guid.Empty,
                                      LoanType = grplist.Key.ld != null ? grplist.Key.ld.LoanType : Guid.Empty,
                                      LoanTypeName = grplist.Key.ld != null ? grplist.Key.l.Name : string.Empty,
                                      ReasonForUse = grplist.Key.l != null ? grplist.Key.ld.ReasonForUse : Guid.Empty,
                                      CustomerType = grplist.Key.ld != null ? grplist.Key.ld.CustomerType : Guid.Empty,
                                      PresentcustomerBank = grplist.Key.ld.PresentcustomerBank,
                                      Place = grplist.Key.ld.Place,
                                      LoanAmount = grplist.Key.ld != null ? grplist.Key.ld.LoanAmount : 0,
                                      Term = grplist.Key.ld.Term,
                                      DisbursementAmount = grplist.Key.ld != null ? grplist.Key.ld.DisbursementAmount : null,
                                      InstallmentDate = grplist.Key.ld != null ? grplist.Key.ld.InstallmentDate : null,
                                      TotalDisbursementAmount = grplist.Key.ld != null ? grplist.Key.ld.TotalDisbursementAmount : null,
                                      DisburseThrough = grplist.Key.ld != null ? grplist.Key.ld.DisburseThrough : null,
                                      ChequeNo = grplist.Key.ld != null ? grplist.Key.ld.ChequeNo : 0,
                                      ChequeDate = grplist.Key.ld != null ? grplist.Key.ld.ChequeDate : null,
                                      BankName = grplist.Key.ld != null ? grplist.Key.ld.BankName : string.Empty,
                                      IsDisbursed = grplist.Key.ld != null ? grplist.Key.ld.IsDisbursed : false,
                                      DateofApplication = grplist.Key.ld != null ? grplist.Key.ld.DateofApplication : null,
                                      LoanStatus = grplist.Key.ld != null ? grplist.Key.ld.LoanStatus : LoanStatus.Draft,
                                      Comment = grplist.Key.ld != null ? grplist.OrderByDescending(s => s.ls.UpdatedDate).Select(s => s.ls.Comment).FirstOrDefault() : null,
                                      LastPrincipalAmount = grplist.Key.ld.LastPrincipalAmount,
                                      LastTenure = grplist.Key.ld.LastTenure,
                                      MonthlyInstallmentAmount = grplist.Key.ld.MonthlyInstallmentAmount,
                                      LastInstallmentDate = grplist.Key.ld.LastInstallmentDate,
                                      ConsumerProductName = grplist.Key.ld.ConsumerProductName,
                                  }).FirstOrDefault();

                var CustomerName = string.Join(",", (from p in db.CustomerPersonalDetail.Where(a => a.CustomerId == loandetail.CustomerId && a.IsDelete == false)
                                                     select p.FirstName + " " + p.MiddleName + " " + p.LastName).ToArray());

                VehicleLoan VehicleLoan = new VehicleLoan();

                BusinessLoan BusinessLoan = new BusinessLoan();

                GoldLoan GoldLoan = new GoldLoan();

                EducationLoan EducationLoan = new EducationLoan();

                MortgageLoan MortgageLoan = new MortgageLoan();

                List<BusinessEconomicDetails> EconomicDetails = new List<BusinessEconomicDetails>();

                List<JewelleryInformation> JewelleryInfo = new List<JewelleryInformation>();

                List<AcademicDetails> EducationInfo = new List<AcademicDetails>();

                List<PurposeofEducationLoan> EducationLoanPurpose = new List<PurposeofEducationLoan>();

                List<ReferencerDetail> details = new List<ReferencerDetail>();

                List<LoanDocuments> loanDocuments = new List<LoanDocuments>();

                List<LoanCharges> loanCharges = new List<LoanCharges>();

                List<UpdateLoanStatus> loanStatus = new List<UpdateLoanStatus>();

                List<MortgageItemInformation> MortgageItemInfo = new List<MortgageItemInformation>();

                LoanId = loandetail.LoanId;

                loanCharges = GetChargesList(LoanId);

                if (loandetail.LoanId != Guid.Empty && !string.IsNullOrWhiteSpace(loandetail.LoanTypeName))
                {
                    loanDocuments = GetLoanDocumentList(LoanId.Value);

                    if (loandetail.LoanTypeName == "Education Loan")
                    {
                        EducationLoan = db.EducationLoan.Where(x => x.LoanId == LoanId).FirstOrDefault();

                        if (EducationLoan != null)
                        {
                            EducationInfo = db.AcademicDetails.Where(x => x.EducationLoanId == EducationLoan.EducationLoanId && !x.IsDelete).ToList();
                            EducationLoanPurpose = db.PurposeofEducationLoan.Where(x => x.EducationLoanId == EducationLoan.EducationLoanId).ToList();
                        }
                    }
                    else if (loandetail.LoanTypeName == "Vehicle Loan")
                    {
                        VehicleLoan = db.VehicleLoan.Where(x => x.LoanId == LoanId).FirstOrDefault();
                    }
                    else if (loandetail.LoanTypeName == "Business Loan")
                    {
                        BusinessLoan = db.BusinessLoan.Where(x => x.LoanId == LoanId).FirstOrDefault();

                        if (BusinessLoan != null)
                        {
                            EconomicDetails = db.BusinessEconomicDetails.Where(a => a.BusinessLoanId == BusinessLoan.BusinessLoanId && !a.IsDeleted).ToList();
                        }
                    }
                    else if (loandetail.LoanTypeName == "Gold Loan")
                    {
                        GoldLoan = db.GoldLoan.Where(x => x.LoanId == LoanId).FirstOrDefault();

                        if (GoldLoan != null)
                        {
                            JewelleryInfo = db.JewelleryInformation.Where(x => x.GoldLoanId == GoldLoan.GoldLoanId && !x.IsDelete).ToList();
                        }
                    }
                    else if (loandetail.LoanTypeName == "Mortgage Loan")
                    {
                        MortgageLoan = db.MortgageLoan.Where(x => x.LoanId == LoanId).FirstOrDefault();

                        if (MortgageLoan != null)
                        {
                            MortgageItemInfo = db.MortgageItemInformation.Where(x => x.MortgageLoanId == MortgageLoan.MortgageLoanId && !x.IsDelete).ToList();
                        }
                    }

                    loanStatus = GetLoanStatusList(LoanId.Value);

                    details = db.Database.SqlQuery<ReferencerDetail>("GetBorrowerDetailById @CustomerProductId, @LoanId",
                     new SqlParameter("CustomerProductId", id),
                     new SqlParameter("LoanId", (object)LoanId ?? DBNull.Value)).ToList();
                }
                else
                {
                    details = db.Database.SqlQuery<ReferencerDetail>("GetAccountHolderDetailsById @CustomerProductId", new SqlParameter("CustomerProductId", id)).ToList();
                }


                var data = new
                {
                    LoanDetail = loandetail,
                    Details = details,
                    VehicleLoan = VehicleLoan,
                    BusinessLoan = BusinessLoan,
                    EconomicDetails = EconomicDetails,
                    GoldLoan = GoldLoan,
                    MortgageLoan = MortgageLoan,
                    JewelleryInfo = JewelleryInfo,
                    MortgageItemInfo = MortgageItemInfo,
                    EducationLoan = EducationLoan,
                    EducationInfo = EducationInfo,
                    EducationLoanPurpose = EducationLoanPurpose,
                    LoanDocuments = loanDocuments,
                    LoanCharges = loanCharges,
                    CustomerList = CustomerName,
                    LoanStatusList = loanStatus
                };


                return data;
            }
        }

        public object GetCustomerDetailByPersonalId(Guid id, Guid? BorrowerId)
        {
            using (var db = new BSCCSLEntity())
            {
                var details = (from c in db.Customer.Where(i => i.IsDelete == false)
                               join cp in db.CustomerProduct.Where(a => a.IsDelete == false & a.IsActive == true) on c.CustomerId equals cp.CustomerId
                               join cpd in db.CustomerPersonalDetail.Where(a => a.IsDelete == false && a.PersonalDetailId == id) on c.CustomerId equals cpd.CustomerId
                               join a in db.CustomerAddress.Where(a => a.IsDelete == false) on cpd.PersonalDetailId equals a.PersonalDetailId
                               //join d in db.CustomerProofDocument on cpd.PersonalDetailId equals d.PersonalDetailId
                               join b in db.Borrower.Include("LoanDetails").Where(b => b.IsDelete == false && b.BorrowerId == BorrowerId) on cpd.PersonalDetailId equals b.PersonalDetailId into tmp
                               from em in tmp.DefaultIfEmpty()
                               group new { cpd, a, c, em } by new { cpd, a, c } into g
                               select new DisplayBorrower
                               {
                                   Customer = g.Key.c,
                                   Personal = g.Key.cpd,
                                   Address = g.Key.a,
                                   Borrower = g.Select(a => a.em).FirstOrDefault()
                               }).FirstOrDefault();

                if (BorrowerId != null)
                {
                    details.LoanDetails = db.BorrowerLoanDetails.Where(s => s.IsDelete == false && s.BorrowerId == BorrowerId && !s.IsDelete).ToList();
                }

                return details;
            }
        }

        public object GetCustomerPersonalDetailByAccountId(string Account)
        {
            using (var db = new BSCCSLEntity())
            {

                var details = (from c in db.Customer.Where(i => i.IsDelete == false)
                               from cp in db.CustomerProduct.Where(x => x.IsDelete == false && x.IsActive == true && x.AccountNumber == Account && x.ProductType == ProductType.Saving_Account)
                               join tc in db.Customer.Where(a => a.IsDelete == false) on cp.CustomerId equals tc.CustomerId
                               join cpd in db.CustomerPersonalDetail.Where(a => a.IsDelete == false) on cp.CustomerId equals cpd.CustomerId
                               group new { cpd, tc } by new { cpd, tc } into g
                               select new
                               {
                                   ClientId = g.Key.tc.ClienId,
                                   CustomerName = g.Key.cpd.FirstName + " " + g.Key.cpd.MiddleName + " " + g.Key.cpd.LastName,
                                   Sex = g.Key.cpd.Sex,
                                   PersonalId = g.Key.cpd.PersonalDetailId
                               }).ToList();

                return details;
            }
        }

        public object GetCustomerPersonalDetailByAccountIdForGroupLoan(string Account)
        {
            using (var db = new BSCCSLEntity())
            {

                var details = (from cp in db.CustomerProduct.Where(x => x.IsDelete == false && x.IsActive == true && x.AccountNumber == Account && x.ProductType == ProductType.Saving_Account)
                               join tc in db.Customer.Where(a => a.IsDelete == false) on cp.CustomerId equals tc.CustomerId
                               join cpd in db.CustomerPersonalDetail.Where(a => a.IsDelete == false) on cp.CustomerId equals cpd.CustomerId
                               group new { cpd } by new { tc, cp } into g
                               select new
                               {
                                   ClientId = g.Key.tc.ClienId,
                                   CustomerId = g.Key.cp.CustomerId,
                                   AccountNumber = g.Key.cp.AccountNumber,
                                   CustomerName = g.Select(s => s.cpd.FirstName + " " + s.cpd.MiddleName + " " + s.cpd.LastName),
                                   PersonalId = g.Select(s => s.cpd.PersonalDetailId)
                               }).FirstOrDefault();

                return details;
            }
        }

        public object CustomerProductId(Guid id)
        {
            using (var db = new BSCCSLEntity())
            {
                var query = db.CustomerProduct.Where(cp => cp.CustomerId == id).Select(c => c.CustomerProductId).FirstOrDefault();
                return query;
            }
        }

        public bool DeleteLoan(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var loan = db.Loan.Where(x => x.LoanId == Id).FirstOrDefault();
                    loan.IsDelete = true;
                    db.Entry(loan).State = EntityState.Modified;

                    db.Borrower.Where(l => l.LoanId == Id).ToList().ForEach(a => a.IsDelete = true);

                    db.Reference.Where(l => l.LoanId == Id).ToList().ForEach(a => a.IsDelete = true);

                    var customerProduct = new CustomerProduct() { CustomerProductId = loan.CustomerProductId, IsDelete = true };
                    db.CustomerProduct.Attach(customerProduct);
                    db.Entry(customerProduct).Property(s => s.IsDelete).IsModified = true;

                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public bool DeleteRefById(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var Ref = db.Reference.Where(x => x.ReferenceId == Id).FirstOrDefault();
                    Ref.IsDelete = true;
                    db.Entry(Ref).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public bool DeleteborrowById(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var borrow = db.Borrower.Where(x => x.BorrowerId == Id).FirstOrDefault();
                    borrow.IsDelete = true;
                    db.Entry(borrow).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public bool DeleteMorgageItem(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var MItem = new MortgageItemInformation() { MortgageItemInformationId = Id, IsDelete = true };
                    db.MortgageItemInformation.Attach(MItem);
                    db.Entry(MItem).Property(s => s.IsDelete).IsModified = true;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public GoldLoan SaveGoldLoan(GoldLoan goldloan, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (goldloan.GoldLoanId == Guid.Empty)
                {
                    db.Entry(goldloan).State = EntityState.Added;
                }
                else
                {
                    var goldloandata = db.GoldLoan.Where(x => x.GoldLoanId == goldloan.GoldLoanId).FirstOrDefault();
                    //goldloandata.DateofApplication = goldloan.DateofApplication;
                    //goldloandata.GoldApplicationNo = goldloan.GoldApplicationNo;
                    goldloandata.GoldloanType = goldloan.GoldloanType;
                    goldloandata.GoldType = goldloan.GoldType;
                    goldloandata.InterestRate = goldloan.InterestRate;
                    goldloandata.JewelleryDate = goldloan.JewelleryDate;
                    goldloandata.JewelleryDatePrice = goldloan.JewelleryDatePrice;
                    goldloandata.ValuationDate = goldloan.ValuationDate;
                    goldloandata.ValuationPrice = goldloan.ValuationPrice;
                    goldloandata.TotalPrice = goldloan.TotalPrice;
                    goldloandata.TotalWeight = goldloan.TotalWeight;
                    goldloandata.ModifiedBy = user.UserId;
                    goldloandata.ModifiedDate = DateTime.Now;
                }
                db.SaveChanges();
            }

            return goldloan;
        }

        public List<JewelleryInformation> SaveJewelleryInformation(List<JewelleryInformation> jewelleryInformation)
        {
            using (var db = new BSCCSLEntity())
            {

                foreach (var info in jewelleryInformation)
                {
                    if (info.JewelleryInformationId == Guid.Empty)
                    {
                        db.Entry(info).State = EntityState.Added;
                    }
                    else
                    {
                        var jewelleryinfo = db.JewelleryInformation.Where(x => x.JewelleryInformationId == info.JewelleryInformationId).FirstOrDefault();
                        jewelleryinfo.Item = info.Item;
                        jewelleryinfo.ItemWeight = info.ItemWeight;
                        jewelleryinfo.NetWeight = info.NetWeight;
                        jewelleryinfo.ItemPrice = info.ItemPrice;
                        jewelleryinfo.Type = info.Type;
                        jewelleryinfo.ModifiedBy = info.ModifiedBy;
                        jewelleryinfo.ModifiedDate = DateTime.Now;
                    }
                }

                db.SaveChanges();
            }

            return jewelleryInformation;
        }

        public MortgageLoan SaveMortgageLoan(MortgageLoan mortgageLoan, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (mortgageLoan.MortgageLoanId == Guid.Empty)
                {
                    db.Entry(mortgageLoan).State = EntityState.Added;
                }
                else
                {
                    var mortgageloandata = db.MortgageLoan.Where(x => x.MortgageLoanId == mortgageLoan.MortgageLoanId).FirstOrDefault();
                    //goldloandata.DateofApplication = goldloan.DateofApplication;
                    //goldloandata.GoldApplicationNo = goldloan.GoldApplicationNo;
                    mortgageloandata.ItemDate = mortgageLoan.ItemDate;
                    mortgageloandata.ItemDatePrice = mortgageLoan.ItemDatePrice;
                    mortgageloandata.ValuationDate = mortgageLoan.ValuationDate;
                    mortgageloandata.ValuationPrice = mortgageLoan.ValuationPrice;
                    mortgageloandata.TotalPrice = mortgageLoan.TotalPrice;
                    mortgageloandata.ModifiedBy = user.UserId;
                    mortgageloandata.ModifiedDate = DateTime.Now;
                }
                db.SaveChanges();
            }

            return mortgageLoan;
        }

        public List<MortgageItemInformation> SaveMortgageItemInfo(List<MortgageItemInformation> MortgageInfo, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                foreach (var info in MortgageInfo)
                {
                    if (info.MortgageItemInformationId == Guid.Empty)
                    {
                        db.Entry(info).State = EntityState.Added;
                    }
                    else
                    {
                        var mortitemInfo = db.MortgageItemInformation.Where(x => x.MortgageItemInformationId == info.MortgageItemInformationId).FirstOrDefault();
                        mortitemInfo.Item = info.Item;
                        mortitemInfo.ItemPrice = info.ItemPrice;
                        mortitemInfo.Type = info.Type;
                        mortitemInfo.ModifiedBy = user.UserId;
                        mortitemInfo.ModifiedDate = DateTime.Now;
                    }
                }
                db.SaveChanges();
            }
            return MortgageInfo;
        }

        public EducationLoan SaveEducationLoan(EducationLoan educationLoan, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                if (educationLoan.EducationLoanId == Guid.Empty)
                {
                    db.Entry(educationLoan).State = EntityState.Added;
                }
                else
                {
                    var educationLoandata = db.EducationLoan.Where(x => x.EducationLoanId == educationLoan.EducationLoanId).FirstOrDefault();
                    //educationLoandata.DateofApplication = educationLoan.DateofApplication;
                    //educationLoandata.EduApplicationNo = educationLoan.EduApplicationNo;
                    educationLoandata.CourseEndingDate = educationLoan.CourseEndingDate;
                    educationLoandata.CourseStartingDate = educationLoan.CourseStartingDate;
                    educationLoandata.EduAddressofCollege = educationLoan.EduAddressofCollege;
                    educationLoandata.EducationCity = educationLoan.EducationCity;
                    educationLoandata.EducationFaxNum = educationLoan.EducationFaxNum;
                    educationLoandata.EducationPhoneNum = educationLoan.EducationPhoneNum;
                    educationLoandata.EducationPincode = educationLoan.EducationPincode;
                    educationLoandata.Educationstate = educationLoan.Educationstate;
                    educationLoandata.EduCourseName = educationLoan.EduCourseName;
                    educationLoandata.EduCoursePlace = educationLoan.EduCoursePlace;
                    educationLoandata.EduExpenditures = educationLoan.EduExpenditures;
                    educationLoandata.EduFamilyFunded = educationLoan.EduFamilyFunded;
                    educationLoandata.EduLoanAmount = educationLoan.EduLoanAmount;
                    educationLoandata.EduNameofCollege = educationLoan.EduNameofCollege;
                    educationLoandata.EduNonRePayableScholarship = educationLoan.EduNonRePayableScholarship;
                    educationLoandata.EduReferenceNumber = educationLoan.EduReferenceNumber;
                    educationLoandata.EduReferenceNumber2 = educationLoan.EduReferenceNumber2;
                    educationLoandata.EduRePayableScholarship = educationLoan.EduRePayableScholarship;
                    educationLoandata.EduSecurityAmount = educationLoan.EduSecurityAmount;
                    educationLoandata.EduSecurityAmount2 = educationLoan.EduSecurityAmount2;
                    educationLoandata.EduSecurityInformation = educationLoan.EduSecurityInformation;
                    educationLoandata.EduSecurityInformation2 = educationLoan.EduSecurityInformation2;
                    educationLoandata.EduSecurityoffer = educationLoan.EduSecurityoffer;
                    educationLoandata.EduSecurityType = educationLoan.EduSecurityType;
                    educationLoandata.EduSecurityType2 = educationLoan.EduSecurityType2;
                    educationLoandata.EduTotalYearofCourse = educationLoan.EduTotalYearofCourse;
                    educationLoandata.LoanId = educationLoan.LoanId;
                    educationLoandata.ModifiedBy = user.UserId;
                    educationLoandata.ModifiedDate = DateTime.Now;
                }
                db.SaveChanges();
            }

            return educationLoan;
        }

        public SaveEducationInfo SaveEducationInfoDetails(SaveEducationInfo EduInfo, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                foreach (var info in EduInfo.EducationInfo)
                {
                    if (info.AcademicDetailsId == Guid.Empty)
                    {
                        db.AcademicDetails.Add(info);
                    }
                    else
                    {
                        var eduinfo = db.AcademicDetails.Where(x => x.AcademicDetailsId == info.AcademicDetailsId).FirstOrDefault();
                        eduinfo.EducationLoanId = info.EducationLoanId;
                        eduinfo.ExamQualified = info.ExamQualified;
                        eduinfo.University_Institution = info.University_Institution;
                        eduinfo.EducationMedium = info.EducationMedium;
                        eduinfo.YearofQualifing = info.YearofQualifing;
                        eduinfo.Qualifiedtrial = info.Qualifiedtrial;
                        eduinfo.MarksinFirstTrial = info.MarksinFirstTrial;
                        eduinfo.MarksPercentage = info.MarksPercentage;
                        eduinfo.Class_Grade = info.Class_Grade;
                        eduinfo.ModifiedBy = user.UserId;
                        eduinfo.ModifiedDate = DateTime.Now;
                    }
                }

                foreach (var info in EduInfo.EducationLoanPurpose)
                {
                    if (info.PurposeofEducationLoanId == Guid.Empty)
                    {
                        db.PurposeofEducationLoan.Add(info);
                    }
                    else
                    {
                        var purposeinfo = db.PurposeofEducationLoan.Where(x => x.PurposeofEducationLoanId == info.PurposeofEducationLoanId).FirstOrDefault();
                        purposeinfo.EducationLoanId = info.EducationLoanId;
                        purposeinfo.LookupId = info.LookupId;
                        purposeinfo.TutionFees = info.TutionFees;
                        purposeinfo.ExamFees = info.ExamFees;
                        purposeinfo.BookFees = info.BookFees;
                        purposeinfo.Rent = info.Rent;
                        purposeinfo.Board = info.Board;
                        purposeinfo.Clothe = info.Clothe;
                        purposeinfo.Casual = info.Casual;
                        purposeinfo.InsurancePremium = info.InsurancePremium;
                        purposeinfo.ModifiedBy = user.UserId;
                        purposeinfo.ModifiedDate = DateTime.Now;
                    }
                }

                db.SaveChanges();
            }

            return EduInfo;
        }

        public LoanStatus SaveUpdatedLoanStatus(UpdateLoanStatus updateLoanStatus)
        {
            using (var db = new BSCCSLEntity())
            {
                var StatusData = db.UpdateLoanStatus.Where(s => s.LoanId == updateLoanStatus.LoanId && s.Comment == updateLoanStatus.Comment && s.UpdatedBy == updateLoanStatus.UpdatedBy && s.LoanStatus == updateLoanStatus.LoanStatus).OrderByDescending(s => s.UpdatedDate).FirstOrDefault();
                if (StatusData == null)
                {
                    updateLoanStatus.UpdatedDate = DateTime.Now;
                    db.Entry(updateLoanStatus).State = EntityState.Added;

                    var loanData = new Loan() { LoanId = updateLoanStatus.LoanId, LoanStatus = updateLoanStatus.LoanStatus };
                    db.Loan.Attach(loanData);
                    db.Entry(loanData).Property(s => s.LoanStatus).IsModified = true;

                    if (updateLoanStatus.LoanStatus == LoanStatus.Disbursed || updateLoanStatus.LoanStatus == LoanStatus.Cancelled || updateLoanStatus.LoanStatus == LoanStatus.Rejected)
                    {
                        Guid CustomerProductId = db.Loan.Where(a => a.LoanId == updateLoanStatus.LoanId).Select(a => a.CustomerProductId).FirstOrDefault();
                        var customerProduct = new CustomerProduct() { CustomerProductId = CustomerProductId, Status = updateLoanStatus.LoanStatus == LoanStatus.Disbursed ? CustomerProductStatus.Approved : (updateLoanStatus.LoanStatus == LoanStatus.Cancelled ? CustomerProductStatus.Cancelled : CustomerProductStatus.Rejected) };
                        db.CustomerProduct.Attach(customerProduct);
                        db.Entry(customerProduct).Property(s => s.Status).IsModified = true;
                    }

                    if (updateLoanStatus.LoanStatus == LoanStatus.Disbursed)
                    {
                        var loanCharges = db.LoanCharges.Where(s => s.LoanId == updateLoanStatus.LoanId && s.Name == "Share").FirstOrDefault();


                        if (loanCharges != null)
                        {
                            Guid CustomerId = db.Loan.Where(s => s.LoanId == updateLoanStatus.LoanId).Select(s => s.CustomerId).FirstOrDefault();
                            CustomerShare _customerShare = new CustomerShare();
                            _customerShare.CustomerId = CustomerId;
                            _customerShare.CertificateNumber = loanCharges.CertificateNo;
                            _customerShare.StartDate = DateTime.Now;
                            _customerShare.Share = loanCharges.NoOfItem ?? 0;
                            _customerShare.ShareAmount = Convert.ToDecimal(100);
                            _customerShare.Total = loanCharges.Value ?? 0;
                            _customerShare.CreatedBy = updateLoanStatus.UpdatedBy;
                            _customerShare.ModifiedBy = updateLoanStatus.UpdatedBy;
                            _customerShare.CreatedDate = DateTime.Now;
                            _customerShare.ModifiedDate = DateTime.Now;
                            _customerShare.Maturity = Maturity.Nominal;

                            string shareno = _customerShare.Share.ToString();
                            var setting = db.Setting.Where(s => s.SettingName == "ShareNumber").FirstOrDefault();

                            int totalfrom = Convert.ToInt32(setting.Value) + Convert.ToInt32(1);
                            _customerShare.FromNumber = totalfrom.ToString();

                            int sharenum = Convert.ToInt32(setting.Value) + Convert.ToInt32(_customerShare.Share);
                            setting.Value = sharenum.ToString();
                            db.SaveChanges();

                            _customerShare.ToNumber = sharenum.ToString();
                            db.CustomerShare.Add(_customerShare);

                        }
                    }
                    db.SaveChanges();
                    SMSService.SendLoanStatusSMS(updateLoanStatus.LoanId);
                }
                return updateLoanStatus.LoanStatus;
            }
        }

        public List<UpdateLoanStatus> GetLoanStatusList(Guid LoanId)
        {
            using (var db = new BSCCSLEntity())
            {
                List<UpdateLoanStatus> loanStatus = (from u in db.UpdateLoanStatus.AsEnumerable().Where(a => a.LoanId == LoanId)
                                                     join a in db.User.AsEnumerable() on u.UpdatedBy equals a.UserId
                                                     select new UpdateLoanStatus
                                                     {
                                                         LoanId = u.LoanId,
                                                         Comment = u.Comment,
                                                         UpdatedByName = a.FirstName + " " + a.LastName,
                                                         LoanStatus = u.LoanStatus,
                                                         UpdatedBy = u.UpdatedBy,
                                                         UpdatedDate = u.UpdatedDate,
                                                         UpdateLoanStatusId = u.UpdateLoanStatusId
                                                     }).ToList();

                return loanStatus;
            }
        }

        public bool DeleteBorrowerLoan(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var Ref = new BorrowerLoanDetails() { BorrowerLoanId = Id, IsDelete = true };
                    db.BorrowerLoanDetails.Attach(Ref);
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

        public object SaveGroupLoan(GroupLoanData loan)
        {
            using (var db = new BSCCSLEntity())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var branchcode = db.Branch.Where(x => x.BranchId == loan.GroupLoan.BranchId).Select(x => x.BranchCode).FirstOrDefault();
                        if (loan.GroupLoan.GroupLoanId == Guid.Empty)
                        {
                            string productcode = db.Product.Where(a => a.ProductId == loan.GroupLoan.ProductId).Select(a => a.ProductCode).FirstOrDefault();
                            Setting setting = db.Setting.Where(a => a.SettingName == "GroupLoanNo").FirstOrDefault();
                            loan.GroupLoan.GroupLoanNumber = branchcode + productcode + setting.Value.ToString();
                            int Prodcutnum = Convert.ToInt32(setting.Value) + 1;
                            setting.Value = Prodcutnum.ToString().PadLeft(6, '0');
                            db.GroupLoan.Add(loan.GroupLoan);
                            db.SaveChanges();
                        }
                        else
                        {
                            var Loandata = db.GroupLoan.Where(a => a.GroupLoanId == loan.GroupLoan.GroupLoanId).FirstOrDefault();
                            Loandata.GroupLoanNumber = loan.GroupLoan.GroupLoanNumber;
                            Loandata.AgentId = loan.GroupLoan.AgentId;
                            Loandata.ProductId = loan.GroupLoan.ProductId;
                            Loandata.EmployeeId = loan.GroupLoan.EmployeeId;
                            Loandata.DateofApplication = loan.GroupLoan.DateofApplication;
                            Loandata.OpeningDate = loan.GroupLoan.OpeningDate;
                            Loandata.InstallmentDate = loan.GroupLoan.InstallmentDate;
                            Loandata.CreatedBy = loan.GroupLoan.CreatedBy;
                            Loandata.CreatedDate = loan.GroupLoan.CreatedDate;
                            Loandata.CreditCheque = loan.GroupLoan.CreditCheque;
                            Loandata.DateOfCredit = loan.GroupLoan.DateOfCredit;
                            Loandata.DistanceFromBranch = loan.GroupLoan.DistanceFromBranch;
                            Loandata.GroupName = loan.GroupLoan.GroupName;
                            Loandata.IDNO = loan.GroupLoan.IDNO;
                            Loandata.InstallmentDuration = loan.GroupLoan.InstallmentDuration;
                            Loandata.InterestRate = loan.GroupLoan.InterestRate;
                            Loandata.GroupLoanAmount = loan.GroupLoan.GroupLoanAmount;
                            Loandata.NeedOfLoan = loan.GroupLoan.NeedOfLoan;
                            Loandata.PreviousLoanAmount = loan.GroupLoan.PreviousLoanAmount;
                            Loandata.PreviousLoanCompleted = loan.GroupLoan.PreviousLoanCompleted;
                            Loandata.PreviouslyBorrowed = loan.GroupLoan.PreviouslyBorrowed;
                            Loandata.ModifiedBy = loan.GroupLoan.ModifiedBy;
                            Loandata.ModifiedDate = DateTime.Now;
                            db.SaveChanges();
                        }

                        foreach (Loan _loan in loan.Loan)
                        {
                            if (_loan.CustomerProductId != new Guid())
                            {
                                var ProductData = db.CustomerProduct.Where(a => a.IsActive == true && a.IsDelete == false && a.CustomerId == _loan.CustomerId && a.ProductId == loan.GroupLoan.ProductId).FirstOrDefault();
                                ProductData.ProductId = loan.GroupLoan.ProductId;
                                ProductData.OpeningDate = loan.GroupLoan.OpeningDate;
                                ProductData.DueDate = loan.GroupLoan.InstallmentDate;
                                ProductData.NextInstallmentDate = loan.GroupLoan.InstallmentDate;
                                db.SaveChanges();
                                _loan.CustomerProductId = ProductData.CustomerProductId;
                            }
                            else
                            {
                                CustomerProduct _customerProduct = new CustomerProduct();
                                Setting setting = db.Setting.Where(a => a.SettingName == "LoanNo").FirstOrDefault();
                                var product = db.Product.Where(a => a.ProductName.Contains("Personal Loan")).FirstOrDefault();
                                _customerProduct.AccountNumber = branchcode + product.ProductCode + setting.Value.ToString();
                                int Prodcutnum = Convert.ToInt32(setting.Value) + 1;
                                setting.Value = Prodcutnum.ToString().PadLeft(6, '0');
                                _customerProduct.Balance = 0;
                                _customerProduct.UpdatedBalance = 0;
                                _customerProduct.BranchId = loan.GroupLoan.BranchId;
                                _customerProduct.LatePaymentFees = product.LatePaymentFees;
                                _customerProduct.OpeningDate = loan.GroupLoan.OpeningDate;
                                _customerProduct.NextInstallmentDate = loan.GroupLoan.InstallmentDate;
                                _customerProduct.DueDate = loan.GroupLoan.InstallmentDate;
                                _customerProduct.CustomerId = _loan.CustomerId;
                                _customerProduct.PaymentType = Frequency.Monthly;
                                _customerProduct.ProductId = loan.GroupLoan.ProductId;
                                _customerProduct.EmployeeId = loan.GroupLoan.EmployeeId;
                                _customerProduct.AgentId = loan.GroupLoan.AgentId;
                                _customerProduct.ProductType = ProductType.Loan;
                                _customerProduct.InterestRate = loan.GroupLoan.InterestRate;
                                _customerProduct.LIType = 0;
                                db.CustomerProduct.Add(_customerProduct);
                                db.SaveChanges();
                                SMSService.SendNewAccountOpenSMS(_customerProduct.CustomerProductId);
                                _loan.CustomerProductId = _customerProduct.CustomerProductId;
                            }
                            _loan.GroupLoanId = loan.GroupLoan.GroupLoanId;

                            if (_loan.LoanId == Guid.Empty && _loan.CustomerId != Guid.Empty)
                            {
                                _loan.LoanStatus = LoanStatus.Draft;
                                _loan.LoanIntrestRate = loan.GroupLoan.InterestRate;
                                _loan.InstallmentDate = loan.GroupLoan.InstallmentDate;
                                db.Loan.Add(_loan);
                                db.SaveChanges();

                                UpdateLoanStatus _loanStaus = new UpdateLoanStatus();
                                _loanStaus.LoanId = _loan.LoanId;
                                _loanStaus.LoanStatus = LoanStatus.Draft;
                                _loanStaus.UpdatedBy = _loan.CreatedBy ?? new Guid();
                                _loanStaus.Comment = null;
                                _loanStaus.UpdatedDate = DateTime.Now;
                                db.Entry(_loanStaus).State = EntityState.Added;
                            }
                            else
                            {
                                var Loandata = db.Loan.Where(a => a.LoanId == _loan.LoanId).FirstOrDefault();
                                Loandata.DateofApplication = _loan.DateofApplication;
                                Loandata.InstallmentDate = loan.GroupLoan.InstallmentDate;
                                Loandata.LoanType = _loan.LoanType;
                                Loandata.GroupLoanId = _loan.GroupLoanId;
                                Loandata.LoanAmount = _loan.LoanAmount;
                                Loandata.LoanIntrestRate = _loan.LoanIntrestRate;
                                Loandata.ModifiedBy = _loan.ModifiedBy;
                                Loandata.ModifiedDate = DateTime.Now;
                                db.SaveChanges();
                            }

                            var PersonalIds = db.CustomerPersonalDetail.Where(s => s.IsDelete == false && s.CustomerId == _loan.CustomerId).Select(s => s.PersonalDetailId).AsEnumerable();

                            foreach (Guid _borrowerId in PersonalIds)
                            {
                                var _borrowerDetails = loan.Borrower.Where(s => s.IsDelete == false && s.PersonalDetailId == _borrowerId).FirstOrDefault();
                                if (_borrowerDetails != null)
                                {
                                    _borrowerDetails.Referencertype = Referencertype.Owner;
                                    _borrowerDetails.LoanId = _loan.LoanId;
                                    db.Borrower.Add(_borrowerDetails);
                                }
                            }
                            db.SaveChanges();
                        }
                        trans.Commit();
                        return loan.GroupLoan.GroupLoanId;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool DeleteGroupLoan(Guid Id)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var Ref = new GroupLoan() { GroupLoanId = Id, IsDelete = true };
                    db.GroupLoan.Attach(Ref);
                    db.Entry(Ref).Property(s => s.IsDelete).IsModified = true;

                    var LoanList = db.Loan.Where(s => s.GroupLoanId == Id).ToList();
                    if (LoanList.Count != 0)
                    {
                        var custIds = LoanList.Select(s => s.CustomerProductId).ToList();
                        var CustProductIds = db.CustomerProduct.Where(s => s.IsDelete == false && s.IsActive == true && custIds.Contains(s.CustomerProductId)).Select(s => s.CustomerProductId).ToList();
                        foreach (Guid PrId in CustProductIds)
                        {
                            var ref2 = new CustomerProduct() { CustomerProductId = PrId, IsActive = false, IsDelete = true };
                            db.CustomerProduct.Attach(ref2);
                            db.Entry(ref2).Property(s => s.IsDelete).IsModified = true;
                            db.Entry(ref2).Property(s => s.IsActive).IsModified = true;
                        }

                        foreach (Loan PrId in LoanList)
                        {
                            PrId.IsDelete = true;
                            db.Entry(PrId).State = EntityState.Modified;
                        }
                    }

                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public bool DeleteMember(MemberData MemData)
        {
            try
            {
                using (var db = new BSCCSLEntity())
                {
                    var Ref = new Loan() { LoanId = MemData.LoanId, IsDelete = true };
                    db.Loan.Attach(Ref);
                    db.Entry(Ref).Property(s => s.IsDelete).IsModified = true;
                    var Ref1 = new CustomerProduct() { CustomerProductId = MemData.CustomerProductId, IsDelete = true, IsActive = false };
                    db.CustomerProduct.Attach(Ref1);
                    db.Entry(Ref1).Property(s => s.IsDelete).IsModified = true;
                    db.Entry(Ref1).Property(s => s.IsActive).IsModified = true;

                    var borrower = db.Borrower.Where(s => s.LoanId == MemData.LoanId).ToList();
                    foreach (Borrower br in borrower)
                    {
                        br.IsDelete = true;
                        db.Entry(br).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public object GetGroupLoandetail(Guid id)
        {
            using (var db = new BSCCSLEntity())
            {

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetGroupLoanByID", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                //Create a parameter using the new SQL DB type viz. Structured to pass as table value parameter

                SqlParameter paramBranchId = cmdTimesheet.Parameters.Add("@GroupLoanId", SqlDbType.UniqueIdentifier);
                paramBranchId.Value = id;
                //Execute the query
                sql.Open();
                //int result = cmdTimesheet.ExecuteNonQuery();
                var reader = cmdTimesheet.ExecuteReader();

                // Read Blogs from the first result set
                var rptlist = ((IObjectContextAdapter)db).ObjectContext.Translate<GroupLoanDetails>(reader).ToList();

                var loandetail = (from r in rptlist
                                  group new { r } by new { r } into rList
                                  select new
                                  {
                                      rList.Key.r.GroupLoanId,
                                      rList.Key.r.GroupLoanNumber,
                                      rList.Key.r.ProductId,
                                      rList.Key.r.DateofApplication,
                                      rList.Key.r.IDNO,
                                      rList.Key.r.GroupName,
                                      rList.Key.r.DistanceFromBranch,
                                      rList.Key.r.InterestRate,
                                      rList.Key.r.GroupLoanAmount,
                                      rList.Key.r.CreditCheque,
                                      rList.Key.r.DateOfCredit,
                                      rList.Key.r.OpeningDate,
                                      rList.Key.r.InstallmentDate,
                                      rList.Key.r.InstallmentDuration,
                                      rList.Key.r.NeedOfLoan,
                                      rList.Key.r.PreviouslyBorrowed,
                                      rList.Key.r.PreviousLoanAmount,
                                      rList.Key.r.PreviousLoanCompleted,
                                      rList.Key.r.EmployeeId,
                                      rList.Key.r.AgentId,
                                      rList.Key.r.CreatedBy,
                                      rList.Key.r.CreatedDate,
                                      rList.Key.r.ModifiedBy,
                                      rList.Key.r.ModifiedDate,
                                      rList.Key.r.IsDelete,
                                      rList.Key.r.AgentCode,
                                      rList.Key.r.AgentName,
                                      rList.Key.r.EmpCode,
                                      rList.Key.r.EmpName,
                                  }).FirstOrDefault();

                var details = (from r in rptlist
                               group new { r } by new { r.LoanId, r.CustomerId, r.CustomerProductId, r.LoanAmount, r.LoanIntrestRate, r.LoanStatus, r.ClienId, r.AccountNumber } into rList
                               select new
                               {
                                   LoanId = rList.Key.LoanId,
                                   CustomerId = rList.Key.CustomerId,
                                   CustomerProductId = rList.Key.CustomerProductId,
                                   LoanAmount = rList.Key.LoanAmount,
                                   LoanIntrestRate = rList.Key.LoanIntrestRate,
                                   //ProcessingCharge = rList.Key.ProcessingCharge,
                                   //ServiceTax = rList.Key.ServiceTax,
                                   LoanStatus = rList.Key.LoanStatus,
                                   ClientId = rList.Key.ClienId,
                                   AccountNumber = rList.Key.AccountNumber,
                                   BorrowerId = string.Join(",", rList.Select(s => s.r.BorrowerId)),
                                   PersonalId = string.Join(",", rList.Select(s => s.r.PersonalDetailId)),
                                   CustomerName = string.Join(", ", rList.Select(s => s.r.FirstName + " " + s.r.MiddleName + " " + s.r.LastName)),
                               }).ToList();
                var data = new
                {
                    LoanDetail = loandetail,
                    Details = details,
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public object UploadLoanDocuments(List<LoanDocuments> LoanDocuments)
        {
            using (var db = new BSCCSLEntity())
            {
                foreach (var info in LoanDocuments)
                {
                    if (info.LoanDocumentId == Guid.Empty)
                    {
                        db.Entry(info).State = EntityState.Added;

                    }
                    else
                    {
                        var Ref = new LoanDocuments() { LoanDocumentId = info.LoanDocumentId, IsDelete = true };
                        db.LoanDocuments.Attach(Ref);
                        db.Entry(Ref).Property(s => s.IsDelete).IsModified = true;
                    }
                }
                db.SaveChanges();

                Guid LoanId = LoanDocuments[0].LoanId;
                var list = GetLoanDocumentList(LoanId);

                return list;
            }
        }

        public List<LoanDocuments> GetLoanDocumentList(Guid LoanId)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = db.LoanDocuments.Where(a => a.IsDelete == false && a.LoanId == LoanId).ToList();
                return list;
            }
        }

        public object DeleteDocuments(Guid LoanDocumentsId)
        {
            using (var db = new BSCCSLEntity())
            {
                var Ref = new LoanDocuments() { LoanDocumentId = LoanDocumentsId, IsDelete = true };
                db.LoanDocuments.Attach(Ref);
                db.Entry(Ref).Property(s => s.IsDelete).IsModified = true;
                db.SaveChanges();
                return true;
            }
        }

        public List<LoanCharges> GetChargesList(Guid? LoanId)
        {
            using (var db = new BSCCSLEntity())
            {
                var list = (from c in db.Charges.AsEnumerable().Where(a => a.IsDelete == false)
                            join l in db.LoanCharges.AsEnumerable().Where(a => (LoanId.HasValue ? a.LoanId == LoanId : false) && a.IsDelete == false)
                            on c.ChargesId equals l.ChargesId into t
                            from a in t.DefaultIfEmpty()
                            select new LoanCharges
                            {
                                Name = a != null ? a.Name : c.Name,
                                Value = a != null ? a.Value : c.Value,
                                ChargesId = a != null ? a.ChargesId : c.ChargesId,
                                LoanId = a == null ? Guid.Empty : a.LoanId,
                                LoanChargesId = a == null ? Guid.Empty : a.LoanChargesId,
                                NoOfItem = a == null ? 0 : a.NoOfItem,
                                IsDelete = false,
                                CertificateNo = a == null ? "" : a.CertificateNo,
                            }).ToList();

                if (LoanId.HasValue)
                {
                    List<Guid> LoanchargeIds = list.Select(d => d.LoanChargesId).ToList();
                    var list2 = (from l in db.LoanCharges.AsEnumerable().Where(a => a.LoanId == LoanId && a.IsDelete == false && !LoanchargeIds.Contains(a.LoanChargesId))
                                 select new LoanCharges
                                 {
                                     Name = l.Name,
                                     Value = l.Value,
                                     ChargesId = l.ChargesId,
                                     LoanId = l.LoanId,
                                     LoanChargesId = l.LoanChargesId,
                                     IsDelete = l.IsDelete,
                                     NoOfItem = l.NoOfItem,
                                     CertificateNo = l.CertificateNo
                                 }).ToList();


                    list = list.Union(list2).ToList();
                }

                return list;
            }
        }

        public object SaveLoanCharges(List<LoanCharges> loanCharges)
        {
            using (var db = new BSCCSLEntity())
            {
                TransactionService transactionService = new TransactionService();

                Guid LoanId = loanCharges[0].LoanId;
                decimal totalcharges = 0;
                Guid LoanCustomerProductId = db.Loan.Where(x => x.LoanId == LoanId).FirstOrDefault().CustomerProductId;
                CustomerProduct objCustomerProduct = db.CustomerProduct.Where(x => x.CustomerProductId == LoanCustomerProductId).FirstOrDefault();
                foreach (var info in loanCharges)
                {
                    if (info.Name != "Share")
                    {
                        var loanCharge = db.LoanCharges.Where(a => a.LoanId == info.LoanId && a.ChargesId == info.ChargesId).FirstOrDefault();
                        if (info.Value != null && info.Value > 0 && info.LoanChargesId == Guid.Empty && loanCharge == null)
                        {
                            totalcharges = totalcharges + info.Value.Value;
                            db.Entry(info).State = EntityState.Added;

                            //if (objCustomerProduct != null)
                            //{

                            //    //Withdraw Loan Charges in Loan Account
                            //    Transactions transaction1 = new Transactions();
                            //    transaction1.BranchId = objCustomerProduct.BranchId;
                            //    transaction1.CustomerId = objCustomerProduct.CustomerId;
                            //    transaction1.CustomerProductId = objCustomerProduct.CustomerProductId;
                            //    transaction1.Amount = info.Value.Value;
                            //    transaction1.Status = Status.Clear;
                            //    transaction1.TransactionType = TransactionType.BankTransfer;
                            //    transaction1.Type = TypeCRDR.DR;
                            //    transaction1.TransactionTime = DateTime.Now;
                            //    transaction1.CreatedDate = DateTime.Now;
                            //    transaction1.CreatedBy = objCustomerProduct.CreatedBy;
                            //    transaction1.RefCustomerProductId = null;
                            //    transaction1.DescIndentify = DescIndentify.Loan_Charges;
                            //    transaction1.TransactionId = transactionService.InsertTransctionUsingSP(transaction1);

                            //}

                        }
                        else if (loanCharge != null)
                        {
                            if (info.Value.HasValue)
                            {
                                totalcharges = totalcharges + info.Value.Value;
                            }
                            loanCharge.IsDelete = false;
                            loanCharge.Value = info.Value;
                            loanCharge.Name = info.Name;
                        }
                    }
                    else
                    {
                        var loanCharge = db.LoanCharges.Where(a => a.LoanId == info.LoanId && a.Name == "Share").FirstOrDefault();
                        if (loanCharge == null)
                        {
                            if (info.Value.HasValue)
                            {
                                totalcharges = totalcharges + info.Value.Value;
                            }
                            db.Entry(info).State = EntityState.Added;

                            //if (objCustomerProduct != null)
                            //{

                            //    //Withdraw Loan Share in Loan Account
                            //    Transactions transaction1 = new Transactions();
                            //    transaction1.BranchId = objCustomerProduct.BranchId;
                            //    transaction1.CustomerId = objCustomerProduct.CustomerId;
                            //    transaction1.CustomerProductId = objCustomerProduct.CustomerProductId;
                            //    transaction1.Amount = info.Value.Value;
                            //    transaction1.Status = Status.Clear;
                            //    transaction1.TransactionType = TransactionType.BankTransfer;
                            //    transaction1.Type = TypeCRDR.DR;
                            //    transaction1.TransactionTime = DateTime.Now;
                            //    transaction1.CreatedDate = DateTime.Now;
                            //    transaction1.CreatedBy = objCustomerProduct.CreatedBy;
                            //    transaction1.RefCustomerProductId = null;
                            //    transaction1.DescIndentify = DescIndentify.Share;
                            //    transaction1.TransactionId = transactionService.InsertTransctionUsingSP(transaction1);

                            //}

                        }
                        else if (loanCharge != null)
                        {
                            if (info.Value.HasValue)
                            {
                                totalcharges = totalcharges + info.Value.Value;
                            }
                            loanCharge.IsDelete = false;
                            loanCharge.Value = info.Value;
                            loanCharge.NoOfItem = info.NoOfItem;
                            loanCharge.Name = info.Name;
                            loanCharge.CertificateNo = info.CertificateNo;
                        }
                    }
                }
                db.SaveChanges();


                decimal LoanAmount = db.Loan.Where(a => a.LoanId == LoanId).Select(a => a.LoanAmount).FirstOrDefault();

                //var Ref = new Loan() { LoanId = LoanId, TotalCharges = totalcharges, DisbursementAmount = LoanAmount - totalcharges, LastPrincipalAmount = LoanAmount };
                //db.Loan.Attach(Ref);
                //db.Entry(Ref).Property(s => s.TotalCharges).IsModified = true;
                //db.Entry(Ref).Property(s => s.LastPrincipalAmount).IsModified = true;
                //db.Entry(Ref).Property(s => s.DisbursementAmount).IsModified = true;
                //db.SaveChanges();
                //db.Entry(Ref).State = EntityState.Detached;
                Loan ln = db.Loan.Where(x => x.LoanId == LoanId).FirstOrDefault();
                if (ln != null)
                {
                    ln.TotalCharges = totalcharges;
                    ln.DisbursementAmount = LoanAmount - totalcharges;
                    ln.LastPrincipalAmount = LoanAmount;
                }
                db.SaveChanges();

                var data = new
                {
                    LoanCharges = loanCharges,
                    DisbursementAmount = LoanAmount - totalcharges
                };

                return data;
            }
        }

        public bool DeleteCharges(Guid LoanChargesId)
        {
            using (var db = new BSCCSLEntity())
            {

                var loanCharges = db.LoanCharges.Where(a => a.LoanChargesId == LoanChargesId).FirstOrDefault();
                loanCharges.IsDelete = true;
                db.SaveChanges();

                decimal totalCharges = db.LoanCharges.Where(a => a.LoanId == loanCharges.LoanId && a.IsDelete == false).Select(a => a.Value.Value).DefaultIfEmpty(0).Sum();

                decimal LoanAmount = db.Loan.Where(a => a.LoanId == loanCharges.LoanId).Select(a => a.LoanAmount).FirstOrDefault();

                var Ref = new Loan() { LoanId = loanCharges.LoanId, TotalCharges = totalCharges, DisbursementAmount = LoanAmount - totalCharges, LastPrincipalAmount = LoanAmount + totalCharges };
                db.Loan.Attach(Ref);
                db.Entry(Ref).Property(s => s.TotalCharges).IsModified = true;
                db.Entry(Ref).Property(s => s.DisbursementAmount).IsModified = true;
                db.Entry(Ref).Property(s => s.LastPrincipalAmount).IsModified = true;
                db.SaveChanges();
                return true;
            }
        }

        public Amountisation LoanAmountisation(LoanAmountisation loanAmountisation)
        {
            using (var db = new BSCCSLEntity())
            {
                //List<ListLoanAmountisation> listLoanAmountisation = db.Database.SqlQuery<ListLoanAmountisation>("LoanAmountisation @PrincipalAmount, @InterestRate, @TotalMonth",
                //         new SqlParameter("PrincipalAmount", loanAmountisation.PrincipalAmount),
                //         new SqlParameter("InterestRate", loanAmountisation.LoanIntrestRate),
                //         new SqlParameter("TotalMonth", loanAmountisation.Term)).ToList();

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("LoanAmountisation", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                SqlParameter PrincipalAmount = cmdTimesheet.Parameters.Add("@PrincipalAmount", SqlDbType.Decimal);
                PrincipalAmount.Value = loanAmountisation.PrincipalAmount;
                SqlParameter InterestRate = cmdTimesheet.Parameters.Add("@InterestRate", SqlDbType.Decimal);
                InterestRate.Value = loanAmountisation.LoanIntrestRate;
                SqlParameter TotalMonth = cmdTimesheet.Parameters.Add("@TotalMonth", SqlDbType.Int);
                TotalMonth.Value = (loanAmountisation.Term != 0 ? loanAmountisation.Term : 1);

                SqlParameter InstallmentDate = cmdTimesheet.Parameters.Add("@InstallmentDate", SqlDbType.DateTime);
                InstallmentDate.Value = loanAmountisation.InstallmentDate;

                SqlParameter LoanId = cmdTimesheet.Parameters.Add("@LoanId", SqlDbType.UniqueIdentifier);
                LoanId.Value = loanAmountisation.LoanId;

                SqlParameter IsPrePayment = cmdTimesheet.Parameters.Add("@IsPrePayment", SqlDbType.Int);
                IsPrePayment.Value = 0;

                sql.Open();
                var reader = cmdTimesheet.ExecuteReader();

                List<ListLoanAmountisation> listLoanAmountisation = ((IObjectContextAdapter)db).ObjectContext.Translate<ListLoanAmountisation>(reader).ToList();
                reader.NextResult();

                decimal monthlyInstallment = ((IObjectContextAdapter)db).ObjectContext.Translate<decimal>(reader).FirstOrDefault();
                if (loanAmountisation.LoanId != null)
                {
                    var IsDisbursed = db.Loan.Where(a => a.LoanId == loanAmountisation.LoanId).Select(a => a.IsDisbursed).FirstOrDefault();

                    var LoanType = db.Loan.Where(a => a.LoanId == loanAmountisation.LoanId).Select(a => a.LoanType).FirstOrDefault();

                    var LoanAmount = db.Loan.Where(a => a.LoanId == loanAmountisation.LoanId).Select(a => a.LoanAmount).FirstOrDefault();



                    if (!IsDisbursed)
                    {
                        var Ref = new Loan() { LoanId = loanAmountisation.LoanId.Value, MonthlyInstallmentAmount = monthlyInstallment, TotalAmountToPay = listLoanAmountisation.Sum(a => a.MonthlyEMI) };
                        db.Loan.Attach(Ref);
                        db.Entry(Ref).Property(s => s.MonthlyInstallmentAmount).IsModified = true;
                        db.Entry(Ref).Property(s => s.TotalAmountToPay).IsModified = true;
                        db.SaveChanges();

                        Lookup objLookup = db.Lookup.Where(x => x.LookupId == LoanType).FirstOrDefault();
                        if (objLookup != null)
                        {
                            if (objLookup.Name == "Flexi Loan")
                            {
                                decimal TotalAmountTopay = 0;
                                TotalAmountTopay = LoanAmount;
                                Loan objLoan = db.Loan.Where(x => x.LoanId == loanAmountisation.LoanId).FirstOrDefault();
                                objLoan.TotalAmountToPay = TotalAmountTopay;
                                db.SaveChanges();
                            }
                        }

                    }
                }

                Amountisation data = new Amountisation();
                data.ListLoanAmountisation = listLoanAmountisation;
                data.MonthlyInstallmentAmount = monthlyInstallment;
                data.RemainingPrincipleAmt = listLoanAmountisation.Sum(a => a.PrincipalAmt);


                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public Amountisation LoanAmountisationforPrePayment(LoanAmountisation loanAmountisation)
        {
            using (var db = new BSCCSLEntity())
            {
                //List<ListLoanAmountisation> listLoanAmountisation = db.Database.SqlQuery<ListLoanAmountisation>("LoanAmountisation @PrincipalAmount, @InterestRate, @TotalMonth",
                //         new SqlParameter("PrincipalAmount", loanAmountisation.PrincipalAmount),
                //         new SqlParameter("InterestRate", loanAmountisation.LoanIntrestRate),
                //         new SqlParameter("TotalMonth", loanAmountisation.Term)).ToList();

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("LoanAmountisation", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                SqlParameter PrincipalAmount = cmdTimesheet.Parameters.Add("@PrincipalAmount", SqlDbType.Decimal);
                PrincipalAmount.Value = loanAmountisation.PrincipalAmount;
                SqlParameter InterestRate = cmdTimesheet.Parameters.Add("@InterestRate", SqlDbType.Decimal);
                InterestRate.Value = loanAmountisation.LoanIntrestRate;
                SqlParameter TotalMonth = cmdTimesheet.Parameters.Add("@TotalMonth", SqlDbType.Int);
                TotalMonth.Value = (loanAmountisation.Term != 0 ? loanAmountisation.Term : 1);

                SqlParameter InstallmentDate = cmdTimesheet.Parameters.Add("@InstallmentDate", SqlDbType.DateTime);
                InstallmentDate.Value = loanAmountisation.InstallmentDate;

                SqlParameter LoanId = cmdTimesheet.Parameters.Add("@LoanId", SqlDbType.UniqueIdentifier);
                LoanId.Value = loanAmountisation.LoanId;

                SqlParameter IsPrePayment = cmdTimesheet.Parameters.Add("@IsPrePayment", SqlDbType.Int);
                IsPrePayment.Value = 1;

                sql.Open();
                var reader = cmdTimesheet.ExecuteReader();

                List<ListLoanAmountisation> listLoanAmountisation = ((IObjectContextAdapter)db).ObjectContext.Translate<ListLoanAmountisation>(reader).ToList();
                reader.NextResult();

                decimal monthlyInstallment = ((IObjectContextAdapter)db).ObjectContext.Translate<decimal>(reader).FirstOrDefault();
                if (loanAmountisation.LoanId != null)
                {
                    var IsDisbursed = db.Loan.Where(a => a.LoanId == loanAmountisation.LoanId).Select(a => a.IsDisbursed).FirstOrDefault();

                    var LoanType = db.Loan.Where(a => a.LoanId == loanAmountisation.LoanId).Select(a => a.LoanType).FirstOrDefault();

                    var LoanAmount = db.Loan.Where(a => a.LoanId == loanAmountisation.LoanId).Select(a => a.LoanAmount).FirstOrDefault();



                    if (!IsDisbursed)
                    {
                        var Ref = new Loan() { LoanId = loanAmountisation.LoanId.Value, MonthlyInstallmentAmount = monthlyInstallment, TotalAmountToPay = listLoanAmountisation.Sum(a => a.MonthlyEMI) };
                        db.Loan.Attach(Ref);
                        db.Entry(Ref).Property(s => s.MonthlyInstallmentAmount).IsModified = true;
                        db.Entry(Ref).Property(s => s.TotalAmountToPay).IsModified = true;
                        db.SaveChanges();

                        Lookup objLookup = db.Lookup.Where(x => x.LookupId == LoanType).FirstOrDefault();
                        if (objLookup != null)
                        {
                            if (objLookup.Name == "Flexi Loan")
                            {
                                decimal TotalAmountTopay = 0;
                                TotalAmountTopay = LoanAmount;
                                Loan objLoan = db.Loan.Where(x => x.LoanId == loanAmountisation.LoanId).FirstOrDefault();
                                objLoan.TotalAmountToPay = TotalAmountTopay;
                                db.SaveChanges();
                            }
                        }

                    }
                }

                Amountisation data = new Amountisation();
                data.ListLoanAmountisation = listLoanAmountisation;
                data.MonthlyInstallmentAmount = monthlyInstallment;


                sql.Close();
                db.Dispose();
                return data;
            }
        }


        public Amountisation DisplayAmountisation(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                //List<ListLoanAmountisation> listLoanAmountisation = db.Database.SqlQuery<ListLoanAmountisation>("LoanAmountisation @PrincipalAmount, @InterestRate, @TotalMonth",
                //         new SqlParameter("PrincipalAmount", loanAmountisation.PrincipalAmount),
                //         new SqlParameter("InterestRate", loanAmountisation.LoanIntrestRate),
                //         new SqlParameter("TotalMonth", loanAmountisation.Term)).ToList();

                LoanAmountisation loanAmountisation = (from l in db.Loan.Where(a => a.LoanId == Id)
                                                       select new LoanAmountisation
                                                       {
                                                           PrincipalAmount = l.LastPrincipalAmount.Value,
                                                           InstallmentDate = l.LastInstallmentDate.Value,
                                                           LoanIntrestRate = l.LoanIntrestRate,
                                                           LoanId = l.LoanId,
                                                           Term = l.LastTenure.Value,
                                                           CustomerProductId = l.CustomerProductId,
                                                           LoanStatus = l.LoanStatus
                                                       }).FirstOrDefault();

                List<ListLoanAmountisation> Installments = new List<ListLoanAmountisation>();

                var PaidInstallments = (from r in db.RDPayment.Where(a => a.CustomerProductId == loanAmountisation.CustomerProductId && a.RDPaymentType == RDPaymentType.Installment)
                                        select new ListLoanAmountisation
                                        {
                                            MonthlyEMI = r.Amount,
                                            PrincipalAmt = r.PrincipalAmount == null ? 0 : r.PrincipalAmount.Value,
                                            Interest = r.InterestAmount == null ? 0 : r.InterestAmount.Value,
                                            Installmentdate = r.CreatedDate,
                                            IsPaid = r.IsPaid
                                        }).ToList();



                DateTime lastEMI;

                if (PaidInstallments.Count() > 0)
                {
                    lastEMI = PaidInstallments.Max(a => a.Installmentdate);
                }
                else
                {
                    lastEMI = DateTime.Now.Date;
                }

                //double TotalDaysTillToday = 0;
                //double TotalDaysbetweenCurrentMonthinstallment = 0;
                //decimal LoanClosingAmount = 0;
                //Decimal? RemainingPrincipalAmount = 0;
                //Decimal? CurrentMonthsInterestAmount = 0;
                //DateTime LastPaidDate = new DateTime();
                //LastPaidDate = lastEMI;
                //var TotalUnpaidInstallmetsInterst = PaidInstallments.Where(x => x.IsPaid == false).Sum(x => x.Interest);


                DateTime today = DateTime.Now.Date;
                //TotalDaysTillToday = Math.Ceiling((today - LastPaidDate).TotalDays);


                decimal MonthlyInstallmentAmount = 0;
                decimal RemainingPrincipleAmt = 0;

                if (loanAmountisation.LoanStatus != LoanStatus.Completed && loanAmountisation.PrincipalAmount > 0)
                {
                    Amountisation amountisation = LoanAmountisation(loanAmountisation);

                    //Change this Because when the Installment are Unpaid the Extra Installment are display
                    //var EMIAmountisation = amountisation.ListLoanAmountisation.Where(a => a.Installmentdate >= lastEMI).ToList();
                    var EMIAmountisation = amountisation.ListLoanAmountisation.Where(a => a.Installmentdate > lastEMI).ToList();

                    //RemainingPrincipalAmount = EMIAmountisation.Sum(x => x.PrincipalAmt);
                    //CurrentMonthsInterestAmount = EMIAmountisation.Select(x => x.Interest).FirstOrDefault();
                    //TotalDaysbetweenCurrentMonthinstallment = Math.Ceiling((EMIAmountisation.Select(x => x.Installmentdate).FirstOrDefault() - lastEMI).TotalDays);
                    //double dailyinterest = ((double)CurrentMonthsInterestAmount / (double)TotalDaysbetweenCurrentMonthinstallment);
                    //double totalremaininginteresttilldate = dailyinterest * TotalDaysTillToday;
                    //LoanClosingAmount = (((decimal)RemainingPrincipalAmount) + TotalUnpaidInstallmetsInterst + ((decimal)totalremaininginteresttilldate));

                    Installments = PaidInstallments.Union(EMIAmountisation).OrderBy(a => a.Installmentdate).ToList();
                    MonthlyInstallmentAmount = amountisation.MonthlyInstallmentAmount;
                    RemainingPrincipleAmt = amountisation.RemainingPrincipleAmt;
                }
                else
                {
                    Installments = PaidInstallments;
                }
                Amountisation data = new Amountisation();
                data.ListLoanAmountisation = Installments;
                data.MonthlyInstallmentAmount = MonthlyInstallmentAmount;
                data.RemainingPrincipleAmt = RemainingPrincipleAmt;
                return data;
            }
        }

        public object DisbursementLetter(Guid LoanId)
        {
            using (var db = new BSCCSLEntity())
            {
                //var list = (from l in db.Loan.Where(a => a.LoanId == LoanId)
                //            join c in db.CustomerProduct on l.CustomerProductId equals c.CustomerProductId
                //            join b in db.Branch on c.BranchId equals b.BranchId
                //            join p in db.CustomerPersonalDetail.Where(a => a.IsDelete == false) on c.CustomerId equals p.CustomerId
                //            join a in db.CustomerAddress.Where(a => a.IsDelete == false) on c.CustomerId equals a.CustomerId
                //            join n in db.Nominee on c.CustomerId equals n.CustomerId into cn
                //            from n in cn.DefaultIfEmpty()
                //            join h in db.LoanCharges.Where(a => a.IsDelete == false) on l.LoanId equals h.LoanId
                //            into lh
                //            from h in lh.DefaultIfEmpty()
                //                //group new { p } by new { l.LoanAmount, l.DisbursementAmount, l.DateofApplication, l.LoanIntrestRate, l.Term, b.BranchName, b.BranchAddress, b.BranchPhone, n.Name, n.NomineeDOB, n.PlaceofBirth } into grplist
                //            group new { p, a, h } by new { l.LoanAmount, l.DisbursementAmount, l.DateofApplication, l.LoanIntrestRate, l.Term, b.BranchName, b.BranchAddress, b.BranchPhone, n.Name, n.NomineeDOB, n.PlaceofBirth } into grplist
                //            select new
                //            {


                //                grplist.Key.LoanAmount,
                //                grplist.Key.DisbursementAmount,
                //                grplist.Key.DateofApplication,
                //                grplist.Key.LoanIntrestRate,
                //                grplist.Key.Term,
                //                grplist.Key.BranchName,
                //                grplist.Key.BranchAddress,
                //                grplist.Key.BranchPhone,
                //                grplist.Key.NomineeDOB,
                //                NomineeName = grplist.Key.Name,
                //                grplist.Key.PlaceofBirth,
                //                CustomerList = grplist.Select(a => new
                //                {
                //                    a.p.PersonalDetailId,
                //                    CustomerName = a.p.FirstName + " " + a.p.MiddleName + " " + a.p.LastName,
                //                    a.p.DOB
                //                }).ToList(),
                //                CustomerAddress = grplist.Select(a => new
                //                {
                //                    a.p.PersonalDetailId,
                //                    Address = (!string.IsNullOrEmpty(a.a.DoorNo) ? a.a.DoorNo + "," : "") + (!string.IsNullOrEmpty(a.a.BuildingName) ? a.a.BuildingName + ", " : "") + (!string.IsNullOrEmpty(a.a.PlotNo_Street) ? a.a.PlotNo_Street + ", " : "") + (!string.IsNullOrEmpty(a.a.Landmark) ? a.a.Landmark + ", " : "") + (!string.IsNullOrEmpty(a.a.Area) ? a.a.Area + ", " : "") + (!string.IsNullOrEmpty(a.a.City) ? a.a.City + ", " : "") + (!string.IsNullOrEmpty(a.a.District) ? a.a.District + ", " : "") + (!string.IsNullOrEmpty(a.a.State) ? a.a.State + ", " : "") + (!string.IsNullOrEmpty(a.a.Pincode) ? a.a.Pincode : "")
                //                }).FirstOrDefault(),
                //                LoanCharges = grplist.Select(a => new
                //                {
                //                    a.h.Name,
                //                    a.h.Value,
                //                }).Distinct().ToList(),
                //            }).FirstOrDefault();

                //var borrower = (from b in db.Borrower.Where(a => a.LoanId == LoanId && a.PersonalDetailId != list.CustomerList.PersonalDetailId)
                //                join p in db.CustomerPersonalDetail on b.PersonalDetailId equals p.PersonalDetailId
                //                join a in db.CustomerAddress on p.PersonalDetailId equals a.PersonalDetailId
                //                select new
                //                {
                //                    CustomerName = p.FirstName + " " + p.MiddleName + " " + p.LastName,
                //                    Address = (!string.IsNullOrEmpty(a.DoorNo) ? a.DoorNo + "," : "") + (!string.IsNullOrEmpty(a.BuildingName) ? a.BuildingName + ", " : "") + (!string.IsNullOrEmpty(a.PlotNo_Street) ? a.PlotNo_Street + ", " : "") + (!string.IsNullOrEmpty(a.Landmark) ? a.Landmark + ", " : "") + (!string.IsNullOrEmpty(a.Area) ? a.Area + ", " : "") + (!string.IsNullOrEmpty(a.City) ? a.City + ", " : "") + (!string.IsNullOrEmpty(a.District) ? a.District + ", " : "") + (!string.IsNullOrEmpty(a.State) ? a.State + ", " : "") + (!string.IsNullOrEmpty(a.Pincode) ? a.Pincode : "")
                //                }).ToList();

                string connectionstring = db.Database.Connection.ConnectionString;

                SqlConnection sql = new SqlConnection(connectionstring);
                SqlCommand cmdTimesheet = new SqlCommand("GetLoanDetailforPrint", sql);
                cmdTimesheet.CommandType = CommandType.StoredProcedure;
                SqlParameter paramLoanId = cmdTimesheet.Parameters.Add("@LoanId", SqlDbType.UniqueIdentifier);
                paramLoanId.Value = LoanId;
                sql.Open();
                var reader = cmdTimesheet.ExecuteReader();

                LoanDetail loanDetail = ((IObjectContextAdapter)db).ObjectContext.Translate<LoanDetail>(reader).FirstOrDefault();
                reader.NextResult();

                List<HolderDetail> LoanHolder = ((IObjectContextAdapter)db).ObjectContext.Translate<HolderDetail>(reader).ToList();
                reader.NextResult();

                List<ListLoanCharges> Loancharges = ((IObjectContextAdapter)db).ObjectContext.Translate<ListLoanCharges>(reader).ToList();
                reader.NextResult();

                List<HolderDetail> borrower = ((IObjectContextAdapter)db).ObjectContext.Translate<HolderDetail>(reader).ToList();

                var data = new
                {
                    LoanDetail = loanDetail,
                    LoanHolder = LoanHolder,
                    Loancharges = Loancharges,
                    BorrowerDetail = borrower,
                };

                sql.Close();
                db.Dispose();
                return data;
            }
        }

        public List<Charges> GetAllCharges()
        {
            using (var db = new BSCCSLEntity())
            {
                return db.Charges.Where(s => !s.IsDelete).ToList();
            }
        }

        public bool SaveDisbursementAmount(Loan loan, User user)
        {
            using (var db = new BSCCSLEntity())
            {

                bool isdisburs = db.Loan.Where(a => a.LoanId == loan.LoanId).Select(a => a.IsDisbursed).FirstOrDefault();

                if (!isdisburs)
                {
                    TransactionService transactionService = new TransactionService();

                    var Ref = new Loan()
                    {
                        LoanId = loan.LoanId,
                        TotalDisbursementAmount = loan.TotalDisbursementAmount,
                        DisburseThrough = loan.DisburseThrough,
                        ChequeDate = loan.ChequeDate,
                        ChequeNo = loan.ChequeNo,
                        BankName = loan.BankName,
                        DisbursementBy = loan.DisbursementBy,
                        IsDisbursed = true,
                        LoanStatus = LoanStatus.Disbursed
                    };
                    db.Loan.Attach(Ref);
                    db.Entry(Ref).Property(s => s.TotalDisbursementAmount).IsModified = true;
                    db.Entry(Ref).Property(s => s.DisburseThrough).IsModified = true;
                    db.Entry(Ref).Property(s => s.ChequeDate).IsModified = true;
                    db.Entry(Ref).Property(s => s.ChequeNo).IsModified = true;
                    db.Entry(Ref).Property(s => s.BankName).IsModified = true;
                    db.Entry(Ref).Property(s => s.DisbursementBy).IsModified = true;
                    db.Entry(Ref).Property(s => s.IsDisbursed).IsModified = true;
                    db.Entry(Ref).Property(s => s.LoanStatus).IsModified = true;
                    db.SaveChanges();


                    if (loan.DisburseThrough == DisburseThrough.Saving_Account)
                    {
                        CustomerProduct cp = db.CustomerProduct.Where(a => a.IsDelete == false && a.IsActive == true && a.CustomerId == loan.CustomerId && a.ProductType == ProductType.Saving_Account).FirstOrDefault();
                        if (cp != null)
                        {
                            //Add Disbursement Amount in Saving Account
                            Transactions transaction = new Transactions();
                            transaction.BranchId = cp.BranchId;
                            transaction.CustomerId = loan.CustomerId;
                            transaction.CustomerProductId = cp.CustomerProductId;
                            transaction.Amount = loan.TotalDisbursementAmount.Value;
                            transaction.Balance = cp.UpdatedBalance.Value + loan.TotalDisbursementAmount.Value;
                            transaction.Status = Status.Clear;
                            transaction.TransactionType = TransactionType.BankTransfer;
                            transaction.Type = TypeCRDR.CR;
                            transaction.TransactionTime = DateTime.Now;
                            transaction.CreatedDate = DateTime.Now;
                            transaction.CreatedBy = loan.DisbursementBy;
                            transaction.RefCustomerProductId = loan.CustomerProductId;
                            transaction.DescIndentify = DescIndentify.Transfer;
                            transaction.TransactionId = transactionService.InsertTransctionUsingSP(transaction);

                            //cp.UpdatedBalance = cp.UpdatedBalance + loan.TotalDisbursementAmount.Value;
                            //cp.Balance = cp.Balance + loan.TotalDisbursementAmount.Value;
                            //db.SaveChanges();


                            ////Withdraw Disbursement amount from Loan Account
                            //Transactions transaction1 = new Transactions();
                            //transaction1.BranchId = cp.BranchId;
                            //transaction1.CustomerId = loan.CustomerId;
                            //transaction1.CustomerProductId = loan.CustomerProductId;
                            //transaction1.Amount = loan.TotalDisbursementAmount.Value;
                            //transaction1.Balance = cp.UpdatedBalance.Value + loan.TotalDisbursementAmount.Value;
                            //transaction1.Status = Status.Clear;
                            //transaction1.TransactionType = TransactionType.BankTransfer;
                            //transaction1.Type = TypeCRDR.DR;
                            //transaction1.TransactionTime = DateTime.Now;
                            //transaction1.CreatedDate = DateTime.Now;
                            //transaction1.CreatedBy = loan.DisbursementBy;
                            //transaction1.RefCustomerProductId = null;
                            //transaction1.DescIndentify = DescIndentify.Loan_disbursement;
                            //transaction1.TransactionId = transactionService.InsertTransctionUsingSP(transaction1);


                            ////Withdraw Total Charges from Loan Account
                            //decimal TotalLoanCharges = 0;
                            //TotalLoanCharges = loan.LoanAmount - loan.TotalDisbursementAmount.Value;
                            //if (TotalLoanCharges > 0)
                            //{
                            //    Transactions transaction2 = new Transactions();
                            //    transaction2.BranchId = cp.BranchId;
                            //    transaction2.CustomerId = loan.CustomerId;
                            //    transaction2.CustomerProductId = loan.CustomerProductId;
                            //    transaction2.Amount = loan.LoanAmount - loan.TotalDisbursementAmount.Value;
                            //    transaction2.Balance = cp.UpdatedBalance.Value + loan.TotalDisbursementAmount.Value;
                            //    transaction2.Status = Status.Clear;
                            //    transaction2.TransactionType = TransactionType.BankTransfer;
                            //    transaction2.Type = TypeCRDR.DR;
                            //    transaction2.TransactionTime = DateTime.Now;
                            //    transaction2.CreatedDate = DateTime.Now;
                            //    transaction2.CreatedBy = loan.DisbursementBy;
                            //    transaction2.RefCustomerProductId = null;
                            //    transaction2.DescIndentify = DescIndentify.Loan_Charges;
                            //    transaction2.TransactionId = transactionService.InsertTransctionUsingSP(transaction2);

                            //}
                        }
                    }



                    CustomerProduct LoanProduct = db.CustomerProduct.Where(a => a.IsDelete == false && a.IsActive == true && a.CustomerProductId == loan.CustomerProductId).FirstOrDefault();


                    //Withdraw Disbursement amount from Loan Account
                    Transactions transaction1 = new Transactions
                    {
                        BranchId = LoanProduct.BranchId,
                        CustomerId = loan.CustomerId,
                        CustomerProductId = loan.CustomerProductId,
                        Amount = loan.TotalDisbursementAmount.Value,
                        Status = Status.Clear,
                        TransactionType = TransactionType.BankTransfer,
                        Type = TypeCRDR.DR,
                        TransactionTime = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        CreatedBy = loan.DisbursementBy,
                        RefCustomerProductId = null,
                        DescIndentify = DescIndentify.Loan_disbursement
                    };
                    transaction1.TransactionId = transactionService.InsertTransctionUsingSP(transaction1);


                    //Withdraw Total Charges from Loan Account
                    decimal TotalLoanCharges = 0;
                    TotalLoanCharges = loan.LoanAmount - loan.TotalDisbursementAmount.Value;
                    if (TotalLoanCharges > 0)
                    {
                        Transactions transaction2 = new Transactions();
                        transaction2.BranchId = LoanProduct.BranchId;
                        transaction2.CustomerId = loan.CustomerId;
                        transaction2.CustomerProductId = loan.CustomerProductId;
                        transaction2.Amount = loan.LoanAmount - loan.TotalDisbursementAmount.Value;
                        transaction2.Status = Status.Clear;
                        transaction2.TransactionType = TransactionType.BankTransfer;
                        transaction2.Type = TypeCRDR.DR;
                        transaction2.TransactionTime = DateTime.Now;
                        transaction2.CreatedDate = DateTime.Now;
                        transaction2.CreatedBy = loan.DisbursementBy;
                        transaction2.RefCustomerProductId = null;
                        transaction2.DescIndentify = DescIndentify.Loan_Charges;
                        transaction2.TransactionId = transactionService.InsertTransctionUsingSP(transaction2);
                    }




                    LoanPrePayment loanPrePayment = new LoanPrePayment();
                    loanPrePayment.InstallmentDate = loan.InstallmentDate.Value;
                    loanPrePayment.LoanAmount = loan.LastPrincipalAmount.Value;
                    loanPrePayment.MonthlyInstallmentAmount = loan.MonthlyInstallmentAmount.Value;
                    loanPrePayment.LoanId = loan.LoanId;
                    loanPrePayment.InterestRate = loan.LoanIntrestRate;
                    loanPrePayment.Term = Convert.ToInt32(loan.Term);
                    loanPrePayment.CreatedBy = user.UserId;
                    db.LoanPrePayment.Add(loanPrePayment);
                    db.SaveChanges();


                    UpdateLoanStatus _loanStaus = new UpdateLoanStatus();
                    _loanStaus.LoanId = loan.LoanId;
                    _loanStaus.LoanStatus = LoanStatus.Disbursed;
                    _loanStaus.UpdatedBy = user.UserId;
                    _loanStaus.Comment = null;
                    LoanStatus status = SaveUpdatedLoanStatus(_loanStaus);

                    SMSService.SendLoanStatusSMS(loan.LoanId);
                    return true;
                }
                else
                {
                    return false;
                }
            }



        }

        public bool ApproveGroupLoan(ApproveGroupLoan approveGroupLoan, User user)
        {
            using (var db = new BSCCSLEntity())
            {
                foreach (var loanid in approveGroupLoan.LoanId)
                {
                    var Ref = new Loan()
                    {
                        LoanId = loanid,
                        LoanStatus = LoanStatus.Approved
                    };


                    UpdateLoanStatus _loanStaus = new UpdateLoanStatus();
                    _loanStaus.LoanId = loanid;
                    _loanStaus.LoanStatus = LoanStatus.Approved;
                    _loanStaus.UpdatedBy = user.UserId;
                    _loanStaus.Comment = approveGroupLoan.Comment;
                    LoanStatus status = SaveUpdatedLoanStatus(_loanStaus);
                }
                return true;
            }
        }

        public object GetLoanDetailByIdForPrePayment(Guid Id)
        {
            using (var db = new BSCCSLEntity())
            {
                DateTime today = DateTime.Now.Date;

                var loan = (from l in db.Loan.Where(a => a.CustomerProductId == Id)
                            join c in db.CustomerProduct on l.CustomerProductId equals c.CustomerProductId
                            join lp in db.Lookup on l.LoanType equals lp.LookupId
                            join r in db.RDPayment on l.CustomerProductId equals r.CustomerProductId into pay
                            from rdpay in pay.DefaultIfEmpty()
                            group new { rdpay } by new { l.LoanId, l.CustomerProductId, l.MonthlyInstallmentAmount, l.Term, l.LoanIntrestRate, l.InstallmentDate, l.TotalDisbursementAmount, l.TotalCharges, c.NextInstallmentDate, c.LastInstallmentDate, c.MaturityDate, l.TotalAmountToPay, lp.Name } into grp
                            select new
                            {
                                MonthlyInstallmentAmount = grp.Key.MonthlyInstallmentAmount,
                                Term = grp.Key.Term,
                                InstallmentDate = grp.Key.InstallmentDate,
                                TotalLoanAmount = grp.Key.TotalDisbursementAmount + grp.Key.TotalCharges,
                                NextInstallmentDate = grp.Key.NextInstallmentDate,
                                LoanIntrestRate = grp.Key.LoanIntrestRate,
                                CustomerProductId = grp.Key.CustomerProductId,
                                PaidPrincipalAmount = grp.Select(a => a.rdpay != null ? a.rdpay.PrincipalAmount.Value : 0).Sum(),
                                TotalPaidAmount = grp.Select(a => a.rdpay != null ? a.rdpay.Amount : 0).Sum(),
                                RemainingMonth = DbFunctions.DiffMonths(grp.Key.LastInstallmentDate, grp.Key.MaturityDate),
                                TotalAmountToPay = grp.Key.TotalAmountToPay,
                                LoanId = grp.Key.LoanId,
                                LoanTypeName = grp.Key.LoanId != null ? grp.Key.Name : string.Empty,
                            }).FirstOrDefault();

                List<RDPayment> objRDPayment = db.RDPayment.Where(a => a.IsPaid == false && a.CustomerProductId == Id && a.RDPaymentType == RDPaymentType.Installment).ToList();
                decimal TotalPendingInstallmentAmount = 0;
                if (objRDPayment.Count() != 0)
                {
                    TotalPendingInstallmentAmount = objRDPayment.Select(a => a.Amount != null ? a.Amount : 0).Sum();
                }

                double TotalDaysTillToday = 0;
                double TotalDaysbetweenCurrentMonthinstallment = 0;
                Decimal? RemainingPrincipalAmount = 0;
                Decimal? CurrentMonthsInterestAmount = 0;
                DateTime LastPaidDate = new DateTime();


                List<RDPayment> objRDPaymentnew = db.RDPayment.Where(a => a.CustomerProductId == Id && a.RDPaymentType == RDPaymentType.Installment).OrderByDescending(x => x.CreatedDate).ToList();
                if (objRDPaymentnew.Count() > 0)
                {
                    LastPaidDate = objRDPaymentnew.FirstOrDefault().CreatedDate;
                    DateTime todayDate = DateTime.Now.Date;
                    TotalDaysTillToday = Math.Ceiling((todayDate - LastPaidDate).TotalDays);
                }
                LoanAmountisation loanAmountisation = new Models.LoanAmountisation();

                loanAmountisation = (from l in db.Loan.Where(a => a.CustomerProductId == Id)
                                     select new LoanAmountisation
                                     {
                                         PrincipalAmount = l.LastPrincipalAmount.Value,
                                         InstallmentDate = l.LastInstallmentDate.Value,
                                         LoanIntrestRate = l.LoanIntrestRate,
                                         LoanId = l.LoanId,
                                         Term = l.LastTenure.Value,
                                         CustomerProductId = l.CustomerProductId,
                                         LoanStatus = l.LoanStatus
                                     }).FirstOrDefault();

                Amountisation amountisation = LoanAmountisation(loanAmountisation);

                var EMIAmountisation = amountisation.ListLoanAmountisation.Where(a => a.Installmentdate > LastPaidDate).ToList();

                RemainingPrincipalAmount = EMIAmountisation.Sum(x => x.PrincipalAmt);
                CurrentMonthsInterestAmount = EMIAmountisation.Select(x => x.Interest).FirstOrDefault();
                TotalDaysbetweenCurrentMonthinstallment = Math.Ceiling((EMIAmountisation.Select(x => x.Installmentdate).FirstOrDefault() - LastPaidDate).TotalDays);
                double dailyinterest = ((double)CurrentMonthsInterestAmount / (double)TotalDaysbetweenCurrentMonthinstallment);
                double totalremaininginteresttilldate = dailyinterest * TotalDaysTillToday;
                if (totalremaininginteresttilldate < 0)
                    totalremaininginteresttilldate = 0;
                var data = new
                {
                    Loan = loan,
                    Totalremaininginteresttilldate = totalremaininginteresttilldate,
                    TotalPendingInstallmentAmount = TotalPendingInstallmentAmount
                };


                return data;
            }
        }
    }
}

