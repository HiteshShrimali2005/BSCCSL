angular.module("BSCCL").controller('CashFlowController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $location, $q) {
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

    $scope.CashFlowData = {};
    $scope.TotalCreditAmount = 0;
    $scope.TotalDebitAmount = 0;
    $scope.OpeningBalance = 0;
    $scope.ClosingBalance = 0;
    GetGetCashFlowData();

    function GetGetCashFlowData() {
        let startDate = $("#txtFromDate").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtToDate").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }
        var getdata = AppService.GetDataByQrystring("CashFlow", "GetCashFlowData", startDate, endDate, $scope.UserBranch.BranchId);
        getdata.then(function (p1) {
            if (p1.data != null) {
                $scope.CashFlowData = p1.data.CashFlowData;
                $scope.TotalCreditAmount = p1.data.TotalCreditAmount;
                $scope.TotalDebitAmount = p1.data.TotalDebitAmount;
                $scope.OpeningBalance = p1.data.OpeningBalance;
                $scope.ClosingBalance = p1.data.ClosingBalance;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    $scope.PrintCashFlowData = function () {
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
        $("#divCashFlow").print();
        $("#HeaderDiv").hide();
    };


    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        GetGetCashFlowData();
    };


    $scope.SearchData = function () {
        GetGetCashFlowData();
    }

    $scope.SearchClearData = function () {
        $('#txtFromDate').data("DateTimePicker").date(new Date());
        $('#txtToDate').data("DateTimePicker").date(new Date());
        GetGetCashFlowData();
    }

})