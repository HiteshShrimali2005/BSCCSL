angular.module("BSCCL").controller('RptAccountsCRDRController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')


    $scope.BranchHO = $filter('filter')($scope.BranchList, { BranchId: $cookies.get('Branch') })[0].IsHO

    var table;

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
        //defaultDate: new Date("04/21/2017")
    });


    $scope.ChangeFinancialYear = function (value) {
        if (value != "") {
            var startYear = value.split('-')[0];
            var endYear = value.split('-')[1];

            $("#txtStartDateforSearch").val('');
            $("#txtEndDateforSearch").val('');
            $("#txtStartDateforSearch").data("DateTimePicker").date(null);
            $("#txtEndDateforSearch").data("DateTimePicker").date(null);


            var startDate = new Date("04/01/" + startYear);
            var endDate = new Date("03/31/" + endYear);

            $("#txtStartDateforSearch").data("DateTimePicker").maxDate(endDate);
            $("#txtEndDateforSearch").data("DateTimePicker").maxDate(endDate);
            $("#txtStartDateforSearch").data("DateTimePicker").minDate(startDate);
            $("#txtEndDateforSearch").data("DateTimePicker").minDate(startDate);
        }
    }


    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $scope.BranchHO = $filter('filter')($scope.BranchList, { BranchId: $cookies.get('Branch') })[0].IsHO;
        //  $('#tblAccountsCRDR').dataTable().fnDestroy();
        GetAccountsCreditDebit();
    }

    $scope.SearchCRDR = function () {
        GetAccountsCreditDebit()
    }

    $scope.ClearSearch = function () {
        $scope.FinYear = '';
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $("#txtStartDateforSearch").data("DateTimePicker").date(null);
        $("#txtEndDateforSearch").data("DateTimePicker").date(null);

        GetAccountsCreditDebit()
    }

    GetAccountsCreditDebit()

    function GetAccountsCreditDebit() {

        let startDate = $("#txtStartDateforSearch").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtEndDateforSearch").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }


        var getdata = AppService.GetDataByQrysr("Report", "RptAccountsCRDR", $scope.UserBranch.BranchId, ($scope.FinYear ? $scope.FinYear : ""), startDate, endDate)
        getdata.then(function (p1) {
            if (p1.data) {
                $scope.AccountsCRDR = p1.data.rptlist;
                if ($scope.RptAccountsCRDRViewModel == undefined)
                    $scope.RptAccountsCRDRViewModel = new Object();

                if ($scope.RptAccountsCRDRViewModel.ExportData == undefined)
                    $scope.RptAccountsCRDRViewModel.ExportData = new Object();


                $scope.RptAccountsCRDRViewModel.Data = $scope.AccountsCRDR;
                $scope.RptAccountsCRDRViewModel.ExportData.IsHo = $scope.BranchHO;
                $scope.RptAccountsCRDRViewModel.ExportData.TotalCredit = p1.data.TotalCredit;
                $scope.RptAccountsCRDRViewModel.ExportData.TotalDebit = p1.data.TotalDebit;
                $scope.RptAccountsCRDRViewModel.ExportData.TotalOpeningBalance = p1.data.TotalOpeningBalance;
                if (p1.data.rptlist.length > 0) {
                    $("#lblTotalCreditAmount").show();
                    $("#lblTotalDebitAmount").show();
                    $("#lblTotalOpeningBalance").show();
                    $("#lblTotalFinalAmount").show();
                    $("#labelTotal").show();

                    $("#lblTotalCreditAmount").html($filter('currency')(p1.data.TotalCredit, '₹', 2))
                    $("#lblTotalDebitAmount").html($filter('currency')(p1.data.TotalDebit, '₹', 2))
                    $("#lblTotalOpeningBalance").html($filter('currency')(p1.data.TotalOpeningBalance, '₹', 2))
                    let FinalAmount = (p1.data.TotalOpeningBalance + p1.data.TotalCredit) - p1.data.TotalDebit;
                    $("#lblTotalFinalAmount").html($filter('currency')(FinalAmount, '₹', 2))

                }
                else {
                    $("#lblTotalCreditAmount").hide();
                    $("#lblTotalDebitAmount").hide();
                    $("#lblTotalOpeningBalance").hide();
                    $("#lblTotalFinalAmount").hide();
                    $("#labelTotal").hide();
                }
                //$('#AccountsHead').modal('hide');
                //showToastMsg(1, "Accounts Head Saved Successfully")
                //GetAccountsHeadListParentHead()
                //GetAccountsHeadList()
            }
            else {
                showToastMsg(3, 'Error in saving data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in saving data')
        });

        //table = $('#tblAccountsCRDR').DataTable({
        //    "bFilter": false,
        //    "processing": false,
        //    "bInfo": true,
        //    "bServerSide": true,
        //    "bLengthChange": true,
        //    "bPaginate": false,
        //    "bSort": false,
        //    "bDestroy": true,
        //    searching: false,
        //    dom: 'l<"floatRight"B>frtip',
        //    buttons: [
        //         {
        //             extend: 'pdf',
        //             footer: true
        //         },
        //          {
        //              extend: 'print',
        //              footer: true
        //          }
        //    ],
        //    "sAjaxSource": urlpath + "/Report/RptAccountsCRDR",
        //    "fnServerData": function (sSource, aoData, fnCallback) {
        //        aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
        //        $.ajax({
        //            "dataType": 'json',
        //            "type": "POST",
        //            "url": sSource,
        //            "data": aoData,
        //            "success": function (json) {
        //                fnCallback(json);
        //            }
        //        });
        //    },

        //    "aoColumns": [
        //           {
        //               "sTitle": "Branch Name",
        //               "mDataProp": "BranchName",
        //               "visible": $filter('filter')($scope.BranchList, { BranchId: $cookies.get('Branch') })[0].IsHO
        //           },
        //    {
        //        "sTitle": "Product Name",
        //        "mDataProp": "ProductName",
        //    },
        //    {
        //        "sTitle": "Credit",
        //        "mDataProp": "Credit",
        //        "sClass": "text-right",
        //        "render": function (data, type, full) {
        //            if (data != null) {
        //                return data.toFixed(2);
        //            }
        //            else {
        //                return '0.00'
        //            }
        //        }
        //    },
        //    {
        //        "sTitle": "Debit",
        //        "mDataProp": "Debit",
        //        "sClass": "text-right",
        //        "render": function (data, type, full) {
        //            if (data != null) {
        //                return data.toFixed(2);
        //            }
        //            else {
        //                return '0.00'
        //            }
        //        }
        //    }]
        //});
    }


    $scope.ExportCRDR = function () {
        var getdata = AppService.SaveData("Report", "ExportData", $scope.RptAccountsCRDRViewModel )
        getdata.then(function (p1) {
            if (p1.data) {
                DownloadFile(p1.data);
            }
            else {
                showToastMsg(3, 'Error in saving data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in saving data')
        });
    }

    function b64toBlob(b64Data, contentType, sliceSize) {
        contentType = contentType || '';
        sliceSize = sliceSize || 512;

        var byteCharacters = atob(b64Data);
        var byteArrays = [];

        for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            var slice = byteCharacters.slice(offset, offset + sliceSize);

            var byteNumbers = new Array(slice.length);
            for (var i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            var byteArray = new Uint8Array(byteNumbers);

            byteArrays.push(byteArray);
        }

        var blob = new Blob(byteArrays, { type: contentType });
        return blob;
    }

    function DownloadFile(FileContent) {
        var byteNumbers = new Array(FileContent.Content.length);
        for (var i = 0; i < FileContent.Content.length; i++) {
            byteNumbers[i] = FileContent.Content.charCodeAt(i);
        }
        var arrayBufferView = new Uint8Array(byteNumbers);
        var blob = b64toBlob(FileContent.Content, FileContent.ContentType);
        var a = document.createElement("a");
        document.body.appendChild(a);
        a.style = "display: none";
        url = window.URL.createObjectURL(blob);
        a.href = url;
        a.download = FileContent.FileName;
        a.isContentEditable = false;
        a.click();
        window.URL.revokeObjectURL(url);
    }


});