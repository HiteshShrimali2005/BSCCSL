angular.module("BSCCL").controller('RptDayScrollStatesController', function ($scope, $state, $cookies, $location, AppService, $filter, $rootScope) {

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });
    $scope.TotalSavingDeposite = 0;
    $scope.TotalSavingCashDeposite = 0;
    $scope.TotalSavingChequeDeposite = 0;
    $scope.NoOfCustomerSavingCashDeposite = 0;
    $scope.NoOfCustomerSavingChequeDeposite = 0;
    $scope.TotalSavingWithdraw = 0;
    $scope.TotalSavingCashWithdraw = 0;
    $scope.TotalSavingChequeWithdraw = 0;
    $scope.NoOfCustomerSavingCashWithdraw = 0;
    $scope.NoOfCustomerSavingChequeWithdraw = 0;
    $scope.TotalCurrentDeposite = 0;
    $scope.TotalCurrentWithdraw = 0;
    $scope.TotalRecuringDeposite = 0;
    $scope.TotalFixedDeposite = 0;
    $scope.TotalDhanVrudhiYojanaDeposite = 0;
    $scope.DayScrollDetailsList = new Object();

    //var DepositSum = 0;
    //var WithdrawSum = 0;

    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        GetAllDayScrollTotal();
    }

    $scope.SearchTransactionListAll = function () {
        GetAllDayScrollTotal();
    }

    $scope.ClearAll = function () {
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        GetAllDayScrollTotal();
    }

    GetAllDayScrollTotal();

    function GetAllDayScrollTotal() {
        var NewObj = new Object();
        NewObj.BranchId = $scope.UserBranch.BranchId;
        if ($("#txtStartDateforSearch").val() != "") {
            NewObj.fromDate = ddmmyyTommdddyy($("#txtStartDateforSearch").val());
        } else {
            NewObj.fromDate = new Date().toDateString();
        }
        if ($("#txtEndDateforSearch").val() != "") {
            NewObj.toDate = ddmmyyTommdddyy($("#txtEndDateforSearch").val());
        } else {
            NewObj.toDate = new Date().toDateString();
        }


        AppService.SaveData("Report", "GetAllDayScrollTotal", NewObj).then(function (p1) {
            if (p1.data) {
                $scope.TotalSavingDeposite = p1.data.SavingDepositSum;
                $scope.TotalSavingCashDeposite = p1.data.SavingCashDepositSum;
                $scope.TotalSavingChequeDeposite = p1.data.SavingChequeDepositSum;
                $scope.NoOfCustomerSavingCashDeposite = p1.data.NoOfCustomerSavingCashDeposit;
                $scope.NoOfCustomerSavingChequeDeposite = p1.data.NoOfCustomerSavingChequeDeposit;
                $scope.TotalSavingWithdraw = p1.data.SavingWithdrawSum;
                $scope.TotalSavingCashWithdraw = p1.data.SavingCashWithdrawSum;
                $scope.TotalSavingChequeWithdraw = p1.data.SavingChequeWithdrawSum;
                $scope.NoOfCustomerSavingCashWithdraw = p1.data.NoOfCustomerSavingCashWithdraw;
                $scope.NoOfCustomerSavingChequeWithdraw = p1.data.NoOfCustomerSavingChequeWithdraw;
                $scope.TotalCurrentDeposite = p1.data.CurrentDepositSum;
                $scope.TotalCurrentWithdraw = p1.data.CurrentWithdrawSum;
                $scope.TotalRecuringDeposite = p1.data.RecuringDepositeSum;
                $scope.TotalFixedDeposite = p1.data.FixDipositeSum;
                $scope.TotalDhanVrudhiYojanaDeposite = p1.data.TYPDipositeSum;
            }
        })
    }

    $("#DayScrollDetailsDiv").hide()

    $scope.GetDayScrollStatsDetails = function (type) {
        $("#DayScrollDiv").hide()
        $("#DayScrollDetailsDiv").show()
        GetDayScrollDetails(type);
    };
    $scope.GetDayScrollStats = function (type) {
        $("#DayScrollDetailsDiv").hide()
        $("#DayScrollDiv").show()
    };

    function GetDayScrollDetails(type) {
        var NewObj = new Object();
        NewObj.BranchId = $scope.UserBranch.BranchId;
        if ($("#txtStartDateforSearch").val() != "") {
            NewObj.fromDate = ddmmyyTommdddyy($("#txtStartDateforSearch").val());
        } else {
            NewObj.fromDate = new Date().toDateString();
        }
        if ($("#txtEndDateforSearch").val() != "") {
            NewObj.toDate = ddmmyyTommdddyy($("#txtEndDateforSearch").val());
        } else {
            NewObj.toDate = new Date().toDateString();
        }

        NewObj.DayScrollStatsType = type;

        AppService.SaveData("Report", "GetDayScrollDetails", NewObj).then(function (p1) {
            if (p1.data) {
                $scope.DayScrollDetailsList = p1.data.DayScrollDetailsList;
            }
        })
    }

});