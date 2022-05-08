angular.module("BSCCL").controller('TrialBalanceController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $location, $q) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')


    $("#txtFromDate").datetimepicker({
        format: 'DD/MM/YYYY',
        defaultDate: new Date()
    });
    $("#txtToDate").datetimepicker({
        format: 'DD/MM/YYYY',
        defaultDate: new Date()
    });


    $scope.GoBack = function () {
        $state.go('App.Accounts');
    }
    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');
    $scope.Role = getUserdata.Role;

    $scope.ParentAccountData;
    $scope.TotalCreditAmount = 0;
    $scope.TotalDebitAmount = 0;
    GetTrialBalanceListforParentAccount();

    var TrialBalanceParentAccountList;
    var startDate = new Date();
    var endDate = new Date();

    $scope.ChangeFinancialYear = function (value) {
        if (value != "") {
            var startYear = value.split('-')[0];
            var endYear = value.split('-')[1];
            startDate = new Date("04/01/" + startYear);
            endDate = new Date("03/31/" + endYear);
        }
    }


    function GetTrialBalanceListforParentAccount() {
        //startDate = $("#txtFromDate").data("DateTimePicker").date();
        //if (startDate !== null) {
        //    startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        //}
        //endDate = $("#txtToDate").data("DateTimePicker").date();
        //if (endDate !== null) {
        //    endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        //}
        var currentYear = (new Date()).getFullYear();
        var NextYearDate = new Date(currentYear + 1, 01, 01)
        var NextYear = (NextYearDate).getFullYear();
        $("#ddlFinyear").val(currentYear + '-' + NextYear);
        $scope.FinYear = currentYear + '-' + NextYear;
        var Financialyear = $("#ddlFinyear").val();
        if (Financialyear != "") {
            var startYear = Financialyear.split('-')[0];
            var endYear = Financialyear.split('-')[1];

            startDate = new Date("04/01/" + startYear);
            endDate = new Date("03/31/" + endYear);
            TrialBalanceParentAccountList = $('#tblTrialBalanceParentAccount').DataTable({
                "searching": false,
                //"processing": false,
                //"bInfo": true,
                "serverSide": true,
                "bLengthChange": true,
                "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
                "ordering": false,
                //"destroy": true,
                //dom: 'l<"floatRight"B>frtip',
                "dom": '<"pull-right"B><"clear">ltip',
                "buttons": [
                    {
                        "extend": 'excelHtml5',
                        "title": 'Trial Balance',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        "extend": 'pdfHtml5',
                        "title": 'Trial Balance',
                        exportOptions: {
                            columns: ':visible'
                        }
                    }
                ],

                "ajax": function (data, callback, settings) {
                    data.BranchId = $scope.UserBranch.BranchId;
                    data.fromDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
                    data.toDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
                    $.ajax({
                        "dataType": 'json',
                        "type": "POST",
                        "url": urlpath + "/TrailBalance/GetTrialBalanceListforParentAccount",
                        "data": data,
                        "success": function (json) {
                            $scope.TotalCreditAmount = json.TotalCreditAmount;
                            $scope.TotalDebitAmount = json.TotalDebitAmount;
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }
                            $scope.ParentAccountData = json.data;
                            callback(json);
                        }
                    });
                },
                "columnDefs": [
                    {
                        "targets": [0],
                        "visible": false
                    }
                ],

                "columns": [
                    {
                        "data": "AccountId"
                    },
                    {
                        "className": 'details-control',
                        "data": null,
                        "defaultContent": '',
                        "sWidth": "1%"
                    },

                    {
                        "title": "Account Name",
                        "data": "Name",
                        "sWidth": "10%"
                    },
                    {
                        "title": "Opening (Dr)",
                        "render": function (data, type, full) {
                            return $filter('currency')(full.OpeningDR, '₹ ', 2);
                        },
                        "sClass": "text-right"
                    },
                    {
                        "title": "Opening (Cr)",
                        "render": function (data, type, full) {
                            return $filter('currency')(full.OpeningCR, '₹ ', 2);
                        },
                        "sClass": "text-right"
                    },
                    {
                        "title": "Debit",
                        "render": function (data, type, full) {
                            return $filter('currency')(full.Debit, '₹ ', 2);
                        },
                        "sClass": "text-right"
                    },
                    {
                        "title": "Credit",
                        "render": function (data, type, full) {
                            return $filter('currency')(full.Credit, '₹ ', 2);
                        },
                        "sClass": "text-right"
                    },
                    {
                        "title": "Closing (Dr)",
                        "render": function (data, type, full) {
                            return $filter('currency')(full.ClosingDR, '₹ ', 2);
                        },
                        "sClass": "text-right"
                    },
                    {
                        "title": "Closing (Cr)",
                        "render": function (data, type, full) {
                            return $filter('currency')(full.ClosingCR, '₹ ', 2);
                        },
                        "sClass": "text-right"
                    },

                ],
            });
        }

    }

    var SubAccountData;
    $('#tblTrialBalanceParentAccount tbody').on('click', 'td.details-control', function () {
        SubAccountData = null;
        var tr = $(this).closest('tr');
        var row = TrialBalanceParentAccountList.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            // Open this row
            var rowData = row.data();
            var Financialyear = $("#ddlFinyear").val();
            if (Financialyear != "") {
                var startYear = Financialyear.split('-')[0];
                var endYear = Financialyear.split('-')[1];

                startDate = new Date("04/01/" + startYear);
                endDate = new Date("03/31/" + endYear);
            }
            var NewObj = new Object();
            NewObj.AccountId = rowData.AccountId;
            NewObj.fromDate = startDate;
            NewObj.toDate = endDate;
            NewObj.Branchid = $scope.UserBranch.BranchId;
            AppService.SaveData("TrailBalance", "GetTrialBalanceListforSubAccount", NewObj).then(function (p1) {
                if (p1.data) {
                    SubAccountData = p1.data;
                    var detail = '';
                    if (SubAccountData.length > 0)
                        detail += '<table class="table  m-l-40" id="tblTrialBalanceSubAccount_' + NewObj.AccountId + '"> ' +
                            '<tr>' +
                            '<th></th>' +
                            '<th>Account Name</th>' +
                            '<th class="text-right">Opening (Dr)</th>' +
                            '<th class="text-right">Opening (Cr)</th>' +
                            '<th class="text-right">Debit</th>' +
                            '<th class="text-right">Credit</th>' +
                            '<th class="text-right">Closing (Dr)</th>' +
                            '<th class="text-right">Closing (Cr)</th>' +
                            '</tr>';

                    for (var i = 0; i < SubAccountData.length; i++) {
                        detail += '<tr>' +
                            '<td Width"="1%" class="subdetails-control" id=' + SubAccountData[i].SubAccountId + '></td>' +
                            '<td>' + SubAccountData[i].Name + '</td>' +
                            '<td class="text-right">' + ($filter('currency')(SubAccountData[i].OpeningDR, '₹ ', 2)) + '</td>' +
                            '<td class="text-right">' + ($filter('currency')(SubAccountData[i].OpeningCR, '₹ ', 2)) + '</td>' +
                            '<td class="text-right">' + ($filter('currency')(SubAccountData[i].Debit, '₹ ', 2)) + '</td>' +
                            '<td class="text-right">' + ($filter('currency')(SubAccountData[i].Credit, '₹ ', 2)) + '</td>' +
                            '<td class="text-right">' + ($filter('currency')(SubAccountData[i].ClosingDR, '₹ ', 2)) + '</td>' +
                            '<td class="text-right">' + ($filter('currency')(SubAccountData[i].ClosingCR, '₹ ', 2)) + '</td>' +
                            '</tr>';
                    }

                    detail += '</table>';
                    row.child(detail).show();
                    tr.addClass('shown');
                    //var DataTableobj = $('#tblTrialBalanceParentAccount').DataTable();

                    $('#tblTrialBalanceSubAccount_' + NewObj.AccountId + ' tbody').on('click', 'td.subdetails-control', function () {
                        var subtr = $(this).closest('tr');
                        var AcId = $(this).attr('Id');
                        //var subrow = DataTableobj.row(subtr);
                        if (subtr.hasClass('shown')) {
                            // This row is already open - close it
                            $(subtr).next().hide();
                            subtr.removeClass('shown');
                        }
                        else {
                            // Open this row
                            //var subrowData = subrow.data();
                            var SubNewObj = new Object();
                            SubNewObj.AccountId = AcId;
                            SubNewObj.fromDate = startDate;
                            SubNewObj.toDate = endDate;
                            SubNewObj.Branchid = $scope.UserBranch.BranchId;

                            AppService.SaveData("TrailBalance", "GetTrialBalanceListforChildAccount", SubNewObj).then(function (p1) {
                                if (p1.data) {
                                    subtr = subtr;
                                    var ChildAccountData = p1.data;
                                    var subdetail = '';
                                    if (ChildAccountData.length > 0) {

                                        subdetail += '<table class="table m-l-40" id="tblTrialBalanceChildAccount">' +
                                            '<tr>' +
                                            '<th></th>' +
                                            '<th>#</th>' +
                                            '<th>Account Name</th>' +
                                            '<th class="text-right">Opening (Dr)</th>' +
                                            '<th class="text-right">Opening (Cr)</th>' +
                                            '<th class="text-right">Debit</th>' +
                                            '<th class="text-right">Credit</th>' +
                                            '<th class="text-right">Closing (Dr)</th>' +
                                            '<th class="text-right">Closing (Cr)</th>' +
                                            '</tr>';

                                        for (var i = 0; i < ChildAccountData.length; i++) {
                                            subdetail += '<tr>' +
                                                '<td></td>' +
                                                '<td>' + (i + 1) + '</td>' +
                                                '<td>' + ChildAccountData[i].Name + '</td>' +
                                                '<td class="text-right">' + ($filter('currency')(ChildAccountData[i].OpeningDR, '₹ ', 2)) + '</td>' +
                                                '<td class="text-right">' + ($filter('currency')(ChildAccountData[i].OpeningCR, '₹ ', 2)) + '</td>' +
                                                '<td class="text-right">' + ($filter('currency')(ChildAccountData[i].Debit, '₹ ', 2)) + '</td>' +
                                                '<td class="text-right">' + ($filter('currency')(ChildAccountData[i].Credit, '₹ ', 2)) + '</td>' +
                                                '<td class="text-right">' + ($filter('currency')(ChildAccountData[i].ClosingDR, '₹ ', 2)) + '</td>' +
                                                '<td class="text-right">' + ($filter('currency')(ChildAccountData[i].ClosingCR, '₹ ', 2)) + '</td>' +
                                                '</tr>';
                                        }
                                        subdetail += '</table>';
                                    }
                                    else {
                                        subdetail += '<table class="table table-bordered m-l-40" id="tblTrialBalanceChildAccount">' +
                                            '<tr>' +
                                            '<td> No Records Found</th>' +
                                            '</tr>';
                                        subdetail += '</table>';
                                    }
                                    //subrow.child(subdetail).show();
                                    $(subtr).after('<tr><td colspan="8">' + subdetail + '</td></tr>');
                                    subtr.addClass('shown');
                                }
                            })
                        }
                    });
                }
            })
        }
    });




    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        //startDate = $("#txtFromDate").data("DateTimePicker").date();
        //if (startDate !== null) {
        //    startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        //}
        //endDate = $("#txtToDate").data("DateTimePicker").date();
        //if (endDate !== null) {
        //    endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        //}

        //TrialBalanceParentAccountList.draw();
        var Financialyear = $("#ddlFinyear").val();
        if (Financialyear != "") {
            var startYear = Financialyear.split('-')[0];
            var endYear = Financialyear.split('-')[1];

            startDate = new Date("04/01/" + startYear);
            endDate = new Date("03/31/" + endYear);

            TrialBalanceParentAccountList.draw();
        }
        else {
            showToastMsg(3, 'Please select any Financial Year')
        }
    };


    $scope.SearchData = function () {
        //startDate = $("#txtFromDate").data("DateTimePicker").date();
        //if (startDate !== null) {
        //    startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        //}
        //endDate = $("#txtToDate").data("DateTimePicker").date();
        //if (endDate !== null) {
        //    endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        //}

        //TrialBalanceParentAccountList.draw();
        var Financialyear = $("#ddlFinyear").val();
        if (Financialyear != "") {
            var startYear = Financialyear.split('-')[0];
            var endYear = Financialyear.split('-')[1];

            startDate = new Date("04/01/" + startYear);
            endDate = new Date("03/31/" + endYear);

            TrialBalanceParentAccountList.draw();
        }
        else {
            showToastMsg(3, 'Please select any Financial Year')

        }

    }

    $scope.SearchClearData = function () {
        //$('#txtFromDate').data("DateTimePicker").date(new Date());
        //$('#txtToDate').data("DateTimePicker").date(new Date());
        //startDate = $("#txtFromDate").data("DateTimePicker").date();
        //if (startDate !== null) {
        //    startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        //}
        //endDate = $("#txtToDate").data("DateTimePicker").date();
        //if (endDate !== null) {
        //    endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        //}
        var currentYear = (new Date()).getFullYear();
        var NextYearDate = new Date(currentYear + 1, 01, 01)
        var NextYear = (NextYearDate).getFullYear();
        $("#ddlFinyear").val(currentYear + '-' + NextYear);
        $scope.FinYear = currentYear + '-' + NextYear;

        var Financialyear = $("#ddlFinyear").val();
        if (Financialyear != "") {
            var startYear = Financialyear.split('-')[0];
            var endYear = Financialyear.split('-')[1];

            startDate = new Date("04/01/" + startYear);
            endDate = new Date("03/31/" + endYear);

            TrialBalanceParentAccountList.draw();
        }
        else {

        }
    }

    $scope.CloseFinancialYear = function () {
        $scope.FinancialYearModel = new Object();

        var Financialyear = $("#ddlFinyear").val();
        //if (Financialyear == "") {
        //    var currentYear = (new Date()).getFullYear();
        //    var NextYearDate = new Date(currentYear + 1, 01, 01)
        //    var NextYear = (NextYearDate).getFullYear();
        //    $("#ddlFinyear").val(currentYear + '-' + NextYear);
        //    $scope.FinYear = currentYear + '-' + NextYear;
        //}

        if (Financialyear != "") {
            var startYear = Financialyear.split('-')[0];
            var endYear = Financialyear.split('-')[1];

            startDate = new Date("04/01/" + startYear);
            endDate = new Date("03/31/" + endYear);

            $scope.FinancialYearModel.Financialyear = Financialyear;
            $scope.FinancialYearModel.BranchId = $scope.UserBranch.BranchId;
            $scope.FinancialYearModel.FromDate = startDate;
            $scope.FinancialYearModel.ToDate = endDate;
            $scope.FinancialYearModel.ParentAccountTrialBalanceViewModel = $scope.ParentAccountData;

            bootbox.dialog({
                message: "Are you sure want to close the Financial Year " + $scope.FinancialYearModel.Financialyear + "?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            if ($scope.ParentAccountData.length > 0) {
                                var savedata = AppService.SaveData("TrailBalance", "SaveFinancialYearClosingBalance", $scope.FinancialYearModel)
                                savedata.then(function (p1) {
                                    if (p1.data != null) {
                                        showToastMsg(1, "Data Saved Successfully");
                                    }
                                    else {
                                        showToastMsg(3, 'Error in saving data')
                                    }
                                }, function (err) {
                                    showToastMsg(3, 'Error in saving data')
                                });
                            }
                            else {
                                showToastMsg(3, 'Data not Available')
                            }
                        }
                    },
                    danger: {
                        label: "No",
                        className: "btn-danger btn-flat"
                    }
                }
            });


        }
        else {
            showToastMsg(3, 'Please select any Financial Year')
        }

    }

})