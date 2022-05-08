angular.module("BSCCL").controller('RptProfitandLossController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
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
        GetProfitandLossListforExpense();
        GetProfitandLossListforIncome()
    }

    $scope.Search = function () {
        GetProfitandLossListforExpense()
        GetProfitandLossListforIncome()
    }

    $scope.ClearSearch = function () {
        $scope.FinYear = '';
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $("#txtStartDateforSearch").data("DateTimePicker").date(null);
        $("#txtEndDateforSearch").data("DateTimePicker").date(null);

        GetProfitandLossListforExpense()
        GetProfitandLossListforIncome()
    }

    $scope.GetDetailsforParticularItem = function (data, type) {

        HideRptProfiandLossDiv();
        ShowProfiandLossDetailsDiv();
        HideFilteringDiv();
        HideBranchDropDown();
        $scope.ParticularItem = data;
        $scope.ParticularType = type;
        var NewObj = new Object();
        NewObj.ProductTypes = null;

        let startDate = $("#txtStartDateforSearch").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtEndDateforSearch").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }


        var getdata = AppService.GetDataByQS("Report", "RptProfitandLossDetails", $scope.UserBranch.BranchId, $scope.ParticularItem, $scope.ParticularType, $scope.FinYear, NewObj.ProductTypes, startDate, endDate)
        getdata.then(function (p1) {
            if (p1.data) {
                $scope.ProfitandLossDetailsData = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });


    }

    $scope.BackToReport = function () {
        ShowRptProfiandLossDiv();
        HideProfiandLossDetailsDiv();
        ShowFilteringDiv();
        ShowBranchDropDown();
    }


    HideProfiandLossDetailsDiv();
    function GetProfitandLossListforExpense() {
        let startDate = $("#txtStartDateforSearch").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtEndDateforSearch").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }

        var getdata = AppService.GetDataByQrysr("Report", "RptProfitandLossforExpense", $scope.UserBranch.BranchId, ($scope.FinYear ? $scope.FinYear : ""), startDate, endDate)
        getdata.then(function (p1) {
            if (p1.data) {
                $scope.ProfitLoss = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function GetProfitandLossListforIncome() {
        let startDate = $("#txtStartDateforSearch").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtEndDateforSearch").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }
       
        var getdata = AppService.GetDataByQrysr("Report", "RptProfitandLossforIncome", $scope.UserBranch.BranchId, ($scope.FinYear ? $scope.FinYear : ""), startDate, endDate)
        getdata.then(function (p1) {
            if (p1.data) {
                $scope.ProfitLossforInCome = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    $scope.PrintProfitandLossReport = function () {
        $("#HeaderDiv").show();
        $("#divRptProfitandLoss").print();
        $("#HeaderDiv").hide();
    };

    $scope.PrintProfitandLossDetails = function () {
        $("#HeaderDivforDetails").show();
        $(".descriptionSpan").hide();
        $("#PrintProfitandLossDetailsDiv").print();
        $(".descriptionSpan").show();
        $("#HeaderDivforDetails").hide();
    };
    $scope.RecurringDeposit = false;
    $scope.FixedDeposit = false;
    $scope.RegularIncomePlanner = false;
    $scope.MonthlyIncomeScheme = false;



    $scope.ChangeProductType = function (Type, IsChecked) {
        let startDate = $("#txtStartDateforSearch").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtEndDateforSearch").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }

        var ProductType = []

        if (Type == 4 && IsChecked)
            $scope.RecurringDeposit = true;
        else if (Type == 4 && !IsChecked)
            $scope.RecurringDeposit = false;

        if (Type == 3 && IsChecked)
            $scope.FixedDeposit = true;
        else if (Type == 3 && !IsChecked)
            $scope.FixedDeposit = false;

        if (Type == 6 && IsChecked)
            $scope.RegularIncomePlanner = true;
        else if (Type == 6 && !IsChecked)
            $scope.RegularIncomePlanner = false;

        if (Type == 7 && IsChecked)
            $scope.MonthlyIncomeScheme = true;
        else if (Type == 7 && !IsChecked)
            $scope.MonthlyIncomeScheme = false;


        if ($scope.RecurringDeposit)
            ProductType.push(4)

        if ($scope.FixedDeposit)
            ProductType.push(3)

        if ($scope.RegularIncomePlanner)
            ProductType.push(6)

        if ($scope.MonthlyIncomeScheme)
            ProductType.push(7)

        var getdata = AppService.GetDataByQS("Report", "RptProfitandLossDetails", $scope.UserBranch.BranchId, $scope.ParticularItem, $scope.ParticularType, $scope.FinYear, ProductType, startDate, endDate)
        getdata.then(function (p1) {
            if (p1.data) {
                $scope.ProfitandLossDetailsData = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }



    function ShowRptProfiandLossDiv() {
        $("#divRptProfitandLoss").show();
    }
    function HideRptProfiandLossDiv() {
        $("#divRptProfitandLoss").hide();
    }
    function ShowProfiandLossDetailsDiv() {
        $("#divProfitandLossDetails").show();
    }
    function HideProfiandLossDetailsDiv() {
        $("#divProfitandLossDetails").hide();
    }
    function ShowFilteringDiv() {
        $("#divFiltering").show();
    }
    function HideFilteringDiv() {
        $("#divFiltering").hide();
    }
    function ShowBranchDropDown() {
        $("#ddlBranch").show();
    }
    function HideBranchDropDown() {
        $("#ddlBranch").hide();
    }



});