angular.module("BSCCL").controller('PassbookPrintController', function ($scope, AppService, $state, $cookies, $filter, $location, $timeout) {


    $scope.UserBranch.ShowBranch = false;
    $scope.UserBranch.Enabled = false;

    $scope.transactionList = [];
    $scope.passbookprint = {};

    $(".datepicker").datetimepicker({
        format: 'DD/MM/YYYY hh:mm a',
        useCurrent: false,
    });

    $scope.SearchTransactionList = function () {

        var flag = true;
        flag = ValidateSearchParams();
        if (flag) {
            var obj = new Object();
            obj.AccountNo = $("#txtSearchAccountNo").val();
            obj.FromDate = moment(new Date($("#txtSearchFromDate").data("DateTimePicker").date())).format('YYYY-MM-DD hh:mm a');
            obj.ToDate = moment(new Date($("#txtSearchToDate").data("DateTimePicker").date())).format('YYYY-MM-DD hh:mm a');

            var getdata = AppService.SaveData("Transaction", "PrintPassbookData", obj);
            getdata.then(function (p1) {
                if (p1.data != null) {
                    $scope.transactionList = [];
                    $scope.transactionList = p1.data.TransactionData;
                    $scope.passbookprint = p1.data.Passbookprint;
                }
                else {
                    showToastMsg(3, 'Error in getting data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in getting data')
            });
        }
    }

    function ValidateSearchParams() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#txtSearchFromDate"), 'Start Date required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtSearchToDate"), 'End Date required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtSearchAccountNo"), 'Account No required', 'after')) {
            flag = false;
        }


        return flag;
    }

    $scope.SearchClearData = function () {
        $("#txtSearchAccountNo").val('');
        $("#txtSearchFromDate").val('');
        $("#txtSearchToDate").val('');
        $scope.transactionList = [];
    }


    $scope.PrintTransaction = function () {
        //var printContent = document.getElementById("tabledata");
        //WinPrint.document.write("<style media='print'>");
        //WinPrint.document.write("table {padding-top:20px;}  .page-break { page-break-before: always;}</style>");

        //WinPrint.document.write(html);
        //WinPrint.document.write("<table style='width:100%'>");
        //WinPrint.document.write(printContent.getElementsByTagName("tbody")[0].innerHTML);
        //WinPrint.document.write(printContent.innerHTML);
        //WinPrint.document.write("</table>");

        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
        var row = ''
        var c = 0;
        var padding = 17
        var printed = 1
        var remaining = 28
        if ($scope.passbookprint != undefined) {
            if ($scope.passbookprint.LastPageCount != null && $scope.passbookprint.LastPageCount != undefined && $scope.passbookprint.LastPageCount != "0" && $scope.passbookprint.LastPageCount != 0 && $scope.passbookprint.LastPageCount != "") {
                if (parseFloat($scope.passbookprint.LastPageCount) <= 14) {
                    padding = ($scope.passbookprint.LastPageCount * 20) + 20
                }
                else {
                    padding = ($scope.passbookprint.LastPageCount * 20) + 35
                }

                remaining = 28 - $scope.passbookprint.LastPageCount;
                printed = parseFloat($scope.passbookprint.LastPageCount) + 1;
            }
        }

        row = "<style media='print'>";
        row = row + "@page {margin-left: 0;}</style>";
        angular.forEach($scope.transactionList, function (value, key) {

            if (c == 0) {
                row = row + '<table style="width:100%;line-height:17px;padding-top:' + padding + 'px;">'
            }

            var i = key + 1;

            c = i % remaining;

            var depositamt = ''
            var withdraw = ''
            if (value.Type == 1) {
                depositamt = value.Amount
                withdraw = 0
            }
            else {
                depositamt = 0;
                withdraw = value.Amount
            }

            var chq = ''
            if (value.CheckNumber) {
                chq = value.CheckNumber;
            }

            var style = ''
            if (printed == 14) {
                style = 'padding-top:15px';
            }

            row = row + '<tr>'
            row = row + '<td style="width:5%;' + style + '">' + printed + '</td>'
            row = row + '<td style="width:11%;' + style + '">' + $filter('date')(value.TransactionTime, 'dd/MM/yyyy') + '</td>'
            row = row + '<td style="width:10%;' + style + '">' + chq + '</td>'
            row = row + '<td style="width:23%;' + style + '">' + value.Description + '</td>'
            row = row + '<td style="width:15%;text-align:right;' + style + '">' + parseFloat(withdraw).toFixed(2) + '</td>'
            row = row + '<td style="width:14%;text-align:right;' + style + '">' + parseFloat(depositamt).toFixed(2) + '</td>'
            row = row + '<td style="width:14%;text-align:right;' + style + '">' + parseFloat(value.Balance).toFixed(2) + '</td>'
            row = row + '<td style="width:10%;' + style + '"></td>'
            row = row + '</tr>'
            printed = printed + 1;
            if (c == 0) {
                row = row + '</table>'
                row = row + '<div style="display: block; position: relative; page-break-after:always">&nbsp;&nbsp;&nbsp;&nbsp;</div>';
                printed = 1
                remaining = 28;
                padding = 20;
            }
        });

        if (c != 0) {
            row = row + '</table>'
        }

        //var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0');
        var obj = new Object()
        obj.AccountNo = $scope.AccountNo;
        obj.PrintedCount = $scope.transactionList.length;
        if ($scope.transactionList.length <= 28) {
            if ($scope.passbookprint != undefined) {
                if ($scope.passbookprint.LastPageCount != null && $scope.passbookprint.LastPageCount != undefined && $scope.passbookprint.LastPageCount != "0" && $scope.passbookprint.LastPageCount != 0 && $scope.passbookprint.LastPageCount != "") {
                    var lastpage = parseFloat($scope.passbookprint.LastPageCount) + c;

                    if (lastpage >= 28) {
                        obj.LastPageCount = lastpage - 28
                    }
                    else {
                        obj.LastPageCount = lastpage;
                    }
                }
                else {
                    obj.LastPageCount = c;
                }
            }
            else {
                obj.LastPageCount = c;
            }
        }
        else {
            obj.LastPageCount = c;
        }

        var getdata = AppService.SaveData("Transaction", "SavePrintPassbook", obj);
        getdata.then(function (p1) {
            if (p1.data != null) {
                showToastMsg(1, "Passbook Printed successfully")
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });


        WinPrint.document.write(row);
        //WinPrint.document.write('<script src="/Scripts/jquery-1.11.3.min.js"></script>');
        //WinPrint.document.write("<script type='text/javascript'> $(function () { var beforePrint = function () { console.log('Functionality to run before printing.'); };" +
        //    "var afterPrint = function () { console.log('Functionality to run after printing'); $.post('/api/Transaction/SavePrintPassbook','" + obj + "', function () { alert(" + JSON.stringify(JSON.stringify(obj)) + "); });  };" +
        //    "if (window.matchMedia)" +
        //    "{ var mediaQueryList = window.matchMedia('print'); mediaQueryList.addListener(function (mql) { if (mql.matches) { beforePrint(); } else { afterPrint(); } }); }" +
        //        "window.onbeforeprint = beforePrint;" +
        //        "window.onafterprint = afterPrint;"
        //+ "}())</script>");
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        //WinPrint.close();
        $timeout(function () { WinPrint.close(); }, 2000);

    };


    $scope.PrintAccountDetail = function () {

        var getdata = AppService.GetDetailsById("Transaction", "PrintAccountDetailOnPassbook", $scope.AccountNo);
        getdata.then(function (p1) {
            if (p1.data != null) {
                var printdetail = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
                var row = ''
                
                row = "<style media='print'>";
                row = row + "@page {margin-left: 5px;margin-right: 5px;}</style>";
                row = '<table style="width:100%;padding-top:360px;">'
                row = row + '<tr><td style="width:75%;"><table  cellpadding=7 >'
                row = row + '<tr><td style="vertical-align:top"> NAME :</td> <td>' + p1.data.CustomerName + ' </td></tr>'
                row = row + '<tr><td style="vertical-align:top"> ADDRESS: </td> <td>' + p1.data.Address + ' </td></tr>'
                row = row + '<tr><td style="vertical-align:top"> NOMINEE: </td> <td>' + p1.data.Nominee + ' </td></tr>'
                row = row + '<tr><td style="vertical-align:top"> MOBILE NO: </td> <td>' + p1.data.MobileNo + ' </td></tr>'
                row = row + '</table></td>'
                row = row + '<td style="width:25%;vertical-align:top"><table cellpadding=7>'
                row = row + '<tr><td> A/C NO: </td> <td>' + p1.data.AccountNo + ' </td></tr>'
                row = row + '<tr><td> OPENING DT: </td> <td>' + $filter('date')(p1.data.OpeningDate, 'dd/MM/yyyy') + ' </td></tr>'
                row = row + '</table></td></tr>'
                row = row + '</table>'
                console.log(row)
                printdetail.document.write(row);
                printdetail.document.close();
                printdetail.focus();
                printdetail.print();
                printdetail.close();
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }
});



