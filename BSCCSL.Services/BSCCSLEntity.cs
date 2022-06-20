using BSCCSL.Models;
using BSCCSL.Models.Accounting;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Services
{
    public class BSCCSLEntity : DbContext
    {

        public BSCCSLEntity()
            : base("BSCCSLConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer<BSCCSLEntity>(new DatabaseContextSeedInitializer());
        }
        public DbSet<User> User { get; set; }

        public DbSet<Branch> Branch { get; set; }

        public DbSet<ErrorLog> ErrorLog { get; set; }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<CustomerPersonalDetail> CustomerPersonalDetail { get; set; }

        public DbSet<CustomerAddress> CustomerAddress { get; set; }

        public DbSet<CustomerProofDocument> CustomerProofDocument { get; set; }

        public DbSet<Setting> Setting { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<Transactions> Transaction { get; set; }

        public DbSet<CustomerShare> CustomerShare { get; set; }

        public DbSet<Nominee> Nominee { get; set; }

        public DbSet<CustomerProduct> CustomerProduct { get; set; }

        public DbSet<LookupCategory> LookupCategory { get; set; }

        public DbSet<Lookup> Lookup { get; set; }

        public DbSet<Loan> Loan { get; set; }

        public DbSet<UpdateLoanStatus> UpdateLoanStatus { get; set; }

        public DbSet<Borrower> Borrower { get; set; }

        public DbSet<Reference> Reference { get; set; }

        public DbSet<BusinessLoan> BusinessLoan { get; set; }

        public DbSet<BusinessEconomicDetails> BusinessEconomicDetails { get; set; }

        public DbSet<VehicleLoan> VehicleLoan { get; set; }

        public DbSet<DailyInterest> DailyInterest { get; set; }

        public DbSet<RDPayment> RDPayment { get; set; }

        public DbSet<Term> Term { get; set; }

        public DbSet<PassbookPrint> PassbookPrint { get; set; }

        public DbSet<Messages> Messages { get; set; }

        public DbSet<SMSLog> SMSLog { get; set; }

        public DbSet<GoldLoan> GoldLoan { get; set; }

        public DbSet<JewelleryInformation> JewelleryInformation { get; set; }

        public DbSet<EducationLoan> EducationLoan { get; set; }

        public DbSet<AcademicDetails> AcademicDetails { get; set; }

        public DbSet<PurposeofEducationLoan> PurposeofEducationLoan { get; set; }

        public DbSet<BorrowerLoanDetails> BorrowerLoanDetails { get; set; }

        public DbSet<GroupLoan> GroupLoan { get; set; }

        public DbSet<LoanDocuments> LoanDocuments { get; set; }

        public DbSet<Charges> Charges { get; set; }

        public DbSet<LoanCharges> LoanCharges { get; set; }

        public DbSet<ProductCommission> ProductCommission { get; set; }

        public DbSet<AgentRank> AgentRank { get; set; }

        public DbSet<BankMaster> BankMaster { get; set; }

        public DbSet<BankBranchMapping> BankBranchMapping { get; set; }

        public DbSet<BankTransaction> BankTransaction { get; set; }

        public DbSet<PrematureRDFDPercentage> PrematureRDFDPercentage { get; set; }

        public DbSet<RDFDPrematurePercentage> RDFDPrematurePercentage { get; set; }

        public DbSet<ProductEnquiry> ProductEnquiry { get; set; }

        public DbSet<UserTokens> UserTokens { get; set; }

        public DbSet<CustomerNotification> CustomerNotification { get; set; }

        public DbSet<AgentCommissionHistory> AgentCommissionHistory { get; set; }

        public DbSet<MortgageLoan> MortgageLoan { get; set; }

        public DbSet<MortgageItemInformation> MortgageItemInformation { get; set; }

        public DbSet<Application> Application { get; set; }

        public DbSet<AuditLog> AuditLog { get; set; }

        public DbSet<PrintCertificateHistory> PrintCertificateHistory { get; set; }

        public DbSet<AccountsHead> AccountsHead { get; set; }

        public DbSet<Expense> Expense { get; set; }

        public DbSet<AgentHierarchy> AgentHierarchy { get; set; }

        public DbSet<AgentCommission> AgentCommission { get; set; }

        public DbSet<LoanPrePayment> LoanPrePayment { get; set; }

        public DbSet<CustomerDocuments> CustomerDocuments { get; set; }

        public DbSet<JournalVoucher> JournalVoucher { get; set; }

        public DbSet<Accounts> Accounts { get; set; }

        public DbSet<JournalEntry> JournalEntry { get; set; }

        public DbSet<JournalEntryTransactions> JournalEntryTransactions { get; set; }

        public DbSet<FinancialYearClosingBalance> FinancialYearClosingBalance { get; set; }

        public DbSet<PandLFinancialYearClosingBalance> PandLFinancialYearClosingBalance { get; set; }

        public DbSet<Income> Income { get; set; }

        public DbSet<AccountingEntries> AccountingEntries { get; set; }

        //public DbSet<DailyProcessConsoleDates> DailyProcessConsoleDates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }


        public System.Data.Linq.DataLoadOptions LoadOptions { get; set; }
        //public object CustomerProofDocument { get; internal set; }
    }
    public class DatabaseContextSeedInitializer : CreateDatabaseIfNotExists<BSCCSLEntity>
    {
        protected override void Seed(BSCCSLEntity context)
        {
            Branch branch = new Branch();
            branch.BranchName = "HO";
            branch.BranchCode = "HO";
            branch.IsActive = true;
            branch.BranchAddress = "Bhavnagar";
            context.Branch.Add(branch);
            context.SaveChanges();

            User user = new User();
            user.FirstName = "Admin";
            user.LastName = "Admin";
            user.UserName = "admin@bsccsl.com";
            user.Password = UserService.Encrypt("Admin");
            user.IsActive = true;
            user.BranchId = branch.BranchId;
            user.Role = BSCCSL.Models.Role.Admin;
            context.User.Add(user);
            context.SaveChanges();
        }
    }
    public class DailyProcess
    {
        public int id { get; set; }
        public string DailyProcessName { get; set; }
        public string DailyProcessCode { get; set; }
        public DateTime DailyProcessDate { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }

}
