angular.module("BSCCL").controller('ProfitandLossStatementController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $location, $q) {
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

    $scope.ProfitandLossStatementforParentExpenseList = {};
    $scope.ProfitandLossStatementforParentIncomeList = {};
    $scope.FinalIncomeTotalAmount = 0;
    $scope.FinalExpenseTotalAmount = 0;
    $scope.GrossLoss = 0;
    $scope.GrossProfit = 0;
    $scope.isLoss = false;
    $scope.isProfit = false;
    GetProfitandLossStatement();

    function GetProfitandLossStatement() {
        let startDate = $("#txtFromDate").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtToDate").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }
        var getdata = AppService.GetDataByQrystring("ProfitandLossStatement", "GetProfitandLossStatement", startDate, endDate, $scope.UserBranch.BranchId);
        getdata.then(function (p1) {
            if (p1.data != null) {
                $scope.ProfitandLossStatementforParentExpenseList = p1.data.ProfitandLossStatementforParentExpenseList;
                $scope.ProfitandLossStatementforParentIncomeList = p1.data.ProfitandLossStatementforParentIncomeList;
                $scope.FinalIncomeTotalAmount = p1.data.FinalIncomeTotalAmount;
                $scope.FinalExpenseTotalAmount = p1.data.FinalExpenseTotalAmount;
                $scope.GrossLoss = p1.data.GrossLoss;
                $scope.isLoss = p1.data.isLoss;
                $scope.isProfit = p1.data.isProfit;
                $scope.GrossProfit = p1.data.GrossProfit;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    $scope.PrintProfitandLossStatement = function () {
        let startDate = $("#txtFromDate").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "dd-MM-yyyy");
        }
        let endDate = $("#txtToDate").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "dd-MM-yyyy");
        }
        var duration = startDate + ' to ' + endDate;
        $("#lblDuration").text(duration);
        $("#HeaderDiv").show();
        $("#divRptProfitandLossStatement").print();
        $("#HeaderDiv").hide();
    };


    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        GetProfitandLossStatement();
    };


    $scope.SearchData = function () {
        GetProfitandLossStatement();
    }

    $scope.SearchClearData = function () {
        $('#txtFromDate').data("DateTimePicker").date(new Date());
        $('#txtToDate').data("DateTimePicker").date(new Date());
        GetProfitandLossStatement();
    }


    $scope.CloseFinancialYear = function () {
        let startDate = $("#txtFromDate").data("DateTimePicker").date();
        let endDate = $("#txtToDate").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        endDate = $("#txtToDate").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }

        $scope.PandLClosingYearModel = new Object();

        if (startDate !== null && endDate != null) {
            var currentDay = startDate.split('-')[2];
            var currentMonth = startDate.split('-')[1];
            var currentYear = startDate.split('-')[0];

            var CloseFinDay = endDate.split('-')[2];
            var CloseFinMonth = endDate.split('-')[1];
            var CloseFinYear = endDate.split('-')[0];
            var FinancialYear = "";
            if (currentDay == "01" && currentMonth == "04" && CloseFinDay == "31" && CloseFinMonth == "03" && CloseFinYear == (parseInt(currentYear) + 1)) {
                FinancialYear = currentYear + '-' + (parseInt(currentYear) + 1);
                if (FinancialYear != "") {
                    $scope.PandLClosingYearModel.Financialyear = FinancialYear;
                    $scope.PandLClosingYearModel.BranchId = $scope.UserBranch.BranchId;
                    $scope.PandLClosingYearModel.isLoss = $scope.isLoss;
                    $scope.PandLClosingYearModel.isProfit = $scope.isProfit;
                    $scope.PandLClosingYearModel.FinalExpenseTotalAmount = $scope.FinalExpenseTotalAmount;
                    $scope.PandLClosingYearModel.FinalIncomeTotalAmount = $scope.FinalIncomeTotalAmount;

                    if ($scope.ProfitandLossStatementforParentExpenseList.length != 0 || $scope.ProfitandLossStatementforParentIncomeList.length != 0) {
                        bootbox.dialog({
                            message: "Are you sure want to close the Financial Year " + FinancialYear + "?",
                            title: "Confirmation !",
                            size: 'small',
                            buttons: {
                                success: {
                                    label: "Yes",
                                    className: "btn-success btn-flat",
                                    callback: function () {
                                        var savedata = AppService.SaveData("ProfitandLossStatement", "ClosePandLFinancialYear", $scope.PandLClosingYearModel)
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
                                },
                                danger: {
                                    label: "No",
                                    className: "btn-danger btn-flat"
                                }
                            }
                        });

                    }
                    else {
                        showToastMsg(3, 'Data Not Available')
                    }
                }
                else {
                    showToastMsg(3, 'Please select any Financial Year')
                }

            }
            else {
                showToastMsg(3, 'Please select Proper Date according to financial Year')
            }
        }
    }

})