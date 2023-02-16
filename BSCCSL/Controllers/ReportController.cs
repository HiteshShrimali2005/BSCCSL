using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BSCCSL.Models;
using BSCCSL.Services;
using BSCCSL.Extension;
using System.Web;
using ClosedXML.Excel;
using System.IO;

namespace BSCCSL.Controllers
{
    [Authorize]
    public class ReportController : ApiController
    {
        ReportService reportservice;

        public ReportController()
        {
            reportservice = new ReportService();
        }

        [HttpPost]
        public HttpResponseMessage GetAccountList(DataTableSearch Search)
        {
            try
            {
                var Accountlistdata = reportservice.GetAccountList(Search);
                if (Accountlistdata != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Accountlistdata);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetProductwiseBalanceList(DataTableSearch search)
        {
            try
            {
                var result = reportservice.GetProductwiseBalanceList(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage RptDayScrollList(DataTableSearch search)
        {
            try
            {
                var result = reportservice.RptDayScrollList(search, Request.GetCurrentUser());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetAllDayScrollTotal(DataTableSearch search)
        {
            try
            {
                var result = reportservice.GetAllDayScrollTotal(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage GetDayScrollDetails(DataTableSearch search)
        {
            try
            {
                var result = reportservice.GetDayScrollDetails(search);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }


        [HttpPost]
        public HttpResponseMessage GetAllAgentCustomerList(DataTableSearch Search)
        {
            try
            {
                var agentCustomerlist = reportservice.GetAllAgentCustomerList(Search, Request.GetCurrentUser());
                if (agentCustomerlist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, agentCustomerlist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetAllDueInstallmentList(DataTableSearch Search)
        {
            try
            {
                var DueInstallmentList = reportservice.GetAllDueInstallmentList(Search, Request.GetCurrentUser());
                if (DueInstallmentList != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DueInstallmentList);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetAllAgentCommissionList(ReportSearch Search)
        {
            try
            {
                var agentCustomerlist = reportservice.GetAllAgentCommissionList(Search, Request.GetCurrentUser());
                if (agentCustomerlist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, agentCustomerlist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetCommissionData(RptAgentCommission data)
        {
            try
            {
                var Accountlistdata = reportservice.GetCommissionData(data, Request.GetCurrentUser());
                if (Accountlistdata != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Accountlistdata);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetRDFDPendingInstallmentList(DataTableSearch Search)
        {
            try
            {
                var PendingInstallment = reportservice.GetRDFDPendingInstallmentList(Search, Request.GetCurrentUser());
                if (PendingInstallment != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, PendingInstallment);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetAllCommissionPaymentList(DataTableSearch Search)
        {
            try
            {
                var PendingInstallment = reportservice.GetAllCommissionPaymentList(Search, Request.GetCurrentUser());
                if (PendingInstallment != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, PendingInstallment);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetProductInstallmentList(DataTableSearch Search)
        {
            try
            {
                var PendingInstallment = reportservice.GetProductInstallmentList(Search, Request.GetCurrentUser());
                if (PendingInstallment != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, PendingInstallment);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetProductList(DataTableSearch Search)
        {
            try
            {
                var PendingInstallment = reportservice.GetProductList(Search, Request.GetCurrentUser());
                if (PendingInstallment != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, PendingInstallment);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpPost]
        public HttpResponseMessage GetMaturityList(DataTableSearch Search)
        {
            try
            {
                var MaturityList = reportservice.GetMaturityList(Search, Request.GetCurrentUser());
                if (MaturityList != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, MaturityList);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetEmployeeProductList(DataTableSearch Search)
        {
            try
            {
                var agentCustomerlist = reportservice.GetEmployeeProductList(Search, Request.GetCurrentUser());
                if (agentCustomerlist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, agentCustomerlist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetCustomerShares(DataTableSearch Search)
        {
            try
            {
                var agentCustomerlist = reportservice.GetCustomerShares(Search, Request.GetCurrentUser());
                if (agentCustomerlist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, agentCustomerlist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage RptAccountsCRDR(Guid Id1, string Id2, DateTime? Id3, DateTime? Id4)
        {
            try
            {
                var agentCustomerlist = reportservice.RptAccountsCRDR(Id1, Id2, Request.GetCurrentUser(), Id3, Id4);
                if (agentCustomerlist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, agentCustomerlist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpGet]
        public HttpResponseMessage RptSummaryCRDR(Guid Id1, string Id2, DateTime? Id3, DateTime? Id4)
        {
            try
            {
                var agentCustomerlist = reportservice.RptSummaryCRDR(Id1, Id2, Request.GetCurrentUser(), Id3, Id4);
                if (agentCustomerlist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, agentCustomerlist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpPost]
        public HttpResponseMessage RptAgentHierarchyCommission(ReportSearch data)
        {
            try
            {
                var Accountlistdata = reportservice.RptAgentHierarchyCommission(data, Request.GetCurrentUser());
                if (Accountlistdata != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Accountlistdata);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage RptAgentHierarchyCommissionByMonth(RptAgentCommission data)
        {
            try
            {
                var Accountlistdata = reportservice.RptAgentHierarchyCommissionByMonth(data, Request.GetCurrentUser());
                if (Accountlistdata != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Accountlistdata);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage RptProfitandLossforExpense(Guid Id1, string Id2, DateTime? Id3, DateTime? Id4)
        {
            try
            {
                var rptProfitLosslist = reportservice.RptProfitandLossforExpense(Id1, Id2, Request.GetCurrentUser(), Id3, Id4);
                if (rptProfitLosslist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, rptProfitLosslist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage RptProfitandLossforIncome(Guid Id1, string Id2, DateTime? Id3, DateTime? Id4)
        {
            try
            {
                var rptProfitLosslistforIncome = reportservice.RptProfitandLossforIncome(Id1, Id2, Request.GetCurrentUser(), Id3, Id4);
                if (rptProfitLosslistforIncome != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, rptProfitLosslistforIncome);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage RptCashBook(Guid Id1, DateTime? Id2, DateTime? Id3)
        {
            try
            {
                var rptCashboklist = reportservice.RptCashBook(Id1, Id2, Id3, Request.GetCurrentUser());
                if (rptCashboklist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, rptCashboklist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage RptBankBook(Guid Id1, DateTime? Id2, DateTime? Id3)
        {
            try
            {
                var rptBankBooklist = reportservice.RptBankBook(Id1, Id2, Id3, Request.GetCurrentUser());
                if (rptBankBooklist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, rptBankBooklist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage RptTrailBalance(Guid Id1, string Id2, DateTime? Id3, DateTime? Id4)
        {
            try
            {
                var rptTrailBalancelist = reportservice.RptTrailBalance(Id1, Id2, Request.GetCurrentUser(), Id3, Id4);
                if (rptTrailBalancelist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, rptTrailBalancelist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage RptProfitandLossDetails(Guid Id1, string Id2, string Id3, string Id4, List<int> productTypes,DateTime? Id6,DateTime? Id7)
        {
            try
            {

                var rptProfitLosslist = reportservice.RptProfitandLossDetails(Id1, Id2, Id3, Id4, productTypes, Request.GetCurrentUser(), Id6, Id7);
                if (rptProfitLosslist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, rptProfitLosslist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        //[HttpGet]
        //public HttpResponseMessage GetProfitandLossDetailsBasedonProductType(List<RptProfitLossDetails> objRptProfitLossDetails, List<ProductType> productTypes)
        //{
        //    try
        //    {

        //        //var rptProfitLosslist = reportservice.RptProfitandLossDetails(Id1, Id2, Id3, Id4, Request.GetCurrentUser());
        //        var rptProfitLosslistbasedonProduct = "";
        //        if (rptProfitLosslistbasedonProduct != null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, rptProfitLosslistbasedonProduct);
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogService.InsertLog(ex);
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
        //    }
        //}

        [HttpPost]
        public HttpResponseMessage RptLoanStatement(DataTableSearch Search)
        {
            try
            {
                var LoanStatementList = reportservice.RptLoanStatement(Search, Request.GetCurrentUser());
                if (LoanStatementList != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, LoanStatementList);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        //Get Data for Employee Perfomance report
        //[HttpPost]
        //public HttpResponseMessage RptEmployeePerfomanceList(DataTableSearch Search)
        //{
        //    try
        //    {
        //        var empPerfomancelist = reportservice.RptEmployeePerfomanceList(Search, Request.GetCurrentUser());
        //        if (empPerfomancelist != null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, empPerfomancelist);
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogService.InsertLog(ex);
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
        //    }
        //}


        [HttpPost]
        public HttpResponseMessage RptEmployeePerfomanceList(Guid Id1, string Id2, DateTime? Id3, DateTime? Id4)
        {
            try
            {

                var rptProfitLosslist = reportservice.RptEmployeePerfomanceList(Id1, Id2, Id3, Id4, Request.GetCurrentUser());
                if (rptProfitLosslist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, rptProfitLosslist);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage RptAgentPerfomanceList(Guid Id1, string Id2, DateTime? Id3, DateTime? Id4)
        {
            try
            {

                var rpt = reportservice.RptAgentPerfomanceList(Id1, Id2, Id3, Id4, Request.GetCurrentUser());
                if (rpt != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, rpt);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetEmployeeListByBranchId(Guid? Id)
        {
            try
            {
                var result = reportservice.GetEmployeeListByBranchId(Id);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage RptLoanStatementDetails(string Id1,Guid Id2)
        {
            try
            {
                var result = reportservice.RptLoanStatementDetails(Id1, Id2, Request.GetCurrentUser());
                if (result != null)
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpPost]
        public HttpResponseMessage RptPrematureProductList(DataTableSearch Search)
        {
            try
            {
                var PrematureProductList = reportservice.RptPrematureProductList(Search, Request.GetCurrentUser());
                if (PrematureProductList != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, PrematureProductList);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage RptInterestDepositList(DataTableSearch Search)
        {
            try
            {
                var InterestDepositList = reportservice.RptInterestDepositList(Search, Request.GetCurrentUser());
                if (InterestDepositList != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, InterestDepositList);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.InsertLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage ExportData(RptAccountsCRDRViewModel Export)
        {
            try
            {

                string FileName = "AccountCreditDebit_" + DateTime.Now.Ticks + ".xlsx";
                var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Export");


                worksheet.Cell(1, 1).Value = "Branch Name";
                worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                worksheet.Cell(1, 2).Value = "Product Date";
                worksheet.Cell(1, 2).Style.Font.SetBold();
                worksheet.Cell(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(1, 2).Style.Fill.BackgroundColor = XLColor.LightGray;

                worksheet.Cell(1, 3).Value = "Opening Balance";
                worksheet.Cell(1, 3).Style.Font.SetBold();
                worksheet.Cell(1, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Cell(1, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(1, 4).Value = "Credit";
                worksheet.Cell(1, 4).Style.Font.SetBold();
                worksheet.Cell(1, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Cell(1, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(1, 5).Value = "Debit";
                worksheet.Cell(1, 5).Style.Font.SetBold();
                worksheet.Cell(1, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Cell(1, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                int Count = 2;

                if (Export.Data.Count > 0)
                {

                    for (int i = 0; i < Export.Data.Count; i++)
                    {
                        if(Export.Data[i] != null)
                        {
                            if (Export.Data[i].BranchName != null)
                            {
                                worksheet.Cell(Count, 1).Value = Export.Data[i].BranchName;
                                worksheet.Cell(Count, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                worksheet.Column(1).Width = 15;
                            }

                            if (Export.Data[i].ProductName != null)
                            {
                                worksheet.Cell(Count, 2).Value = Export.Data[i].ProductName;
                                worksheet.Cell(Count, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                worksheet.Column(2).Width = 35;
                            }

                            worksheet.Cell(Count, 3).Value = Export.Data[i].OpeningBalance;
                            worksheet.Cell(Count, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            worksheet.Column(7).Width = 15;


                            worksheet.Cell(Count, 4).Value = Export.Data[i].Credit;
                            worksheet.Cell(Count, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            worksheet.Column(7).Width = 15;

                            worksheet.Cell(Count, 5).Value = Export.Data[i].Debit;
                            worksheet.Cell(Count, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            worksheet.Column(7).Width = 15;
                            Count = Count + 1;

                        }

                    }

                                       
                    worksheet.Cell(Count, 2).Value = "Total";
                    worksheet.Cell(Count, 2).Style.Font.SetBold();
                    worksheet.Cell(Count, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    worksheet.Cell(Count, 3).Value = Export.ExportData.TotalOpeningBalance;
                    worksheet.Cell(Count, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    worksheet.Cell(Count, 4).Value = Export.ExportData.TotalCredit;
                    worksheet.Cell(Count, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    worksheet.Cell(Count, 5).Value = Export.ExportData.TotalDebit;
                    worksheet.Cell(Count, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                }

                string contentType = System.Web.MimeMapping.GetMimeMapping(FileName);
                string temppath = System.IO.Path.GetTempPath();
                MemoryStream memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                byte[] array = memoryStream.ToArray();
                string strTemp = Convert.ToBase64String(array);
                memoryStream.Close();

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Content = strTemp,
                    MimeType = contentType,
                    FileName = FileName
                });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
