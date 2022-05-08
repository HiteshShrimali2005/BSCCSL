angular.module("BSCCL").controller('GeneralLedgerDetailsController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $location, $q) {
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

    $scope.GeneralLedgerList = new Object();
    $scope.TotalCreditAmount = 0;
    $scope.TotalDebitAmount = 0;
    $scope.Openingbalance = 0;
    $scope.ClosingCreditAmount = 0;
    $scope.ClosingDebitAmount = 0;
    $scope.AccountList = [];
    GetAccountList();
    function GetAccountList() {
        var getdata = AppService.GetDetailsWithoutId("GeneralLedgerDetails", "GetAccountList");

        getdata.then(function (p1) {
            if (p1.data != null) {
                $scope.AccountList = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    function GetGeneralLedgerList() {
        let startDate = $("#txtFromDate").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtToDate").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }
        var AccountId = $("#ddlAccount").val();
        var getdata = AppService.GetDataByQrysr("GeneralLedgerDetails", "GetGeneralLedgerList", startDate, endDate, $scope.UserBranch.BranchId, AccountId);

        getdata.then(function (p1) {
            if (p1.data != null) {
                $scope.GeneralLedgerList = p1.data.aaData;
                $scope.TotalCreditAmount = p1.data.TotalCreditAmount;
                $scope.TotalDebitAmount = p1.data.TotalDebitAmount;
                $scope.Openingbalance = p1.data.Openingbalance;
                $scope.ClosingCreditAmount = p1.data.ClosingCreditAmount;
                $scope.ClosingDebitAmount = p1.data.ClosingDebitAmount;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        if ($("#ddlAccount").val() != "") {
            GetGeneralLedgerList();
        }
        else {
            showToastMsg(3, 'Please select any account.')
        }
    };


    $scope.SearchData = function () {
        if ($("#ddlAccount").val() != "") {
            GetGeneralLedgerList();
        }
        else {
            showToastMsg(3, 'Please select any account.')
        }
    }

    $scope.SearchClearData = function () {
        $('#txtFromDate').data("DateTimePicker").date(new Date());
        $('#txtToDate').data("DateTimePicker").date(new Date());
        $("#ddlAccount").val("");
        $scope.GeneralLedgerList = {};
    }

})