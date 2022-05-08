angular.module("BSCCL").controller('BalanceSheetController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $location, $q) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    //$("#txtFromDate").datetimepicker({
    //    format: 'DD/MM/YYYY',
    //    defaultDate: new Date()
    //});
    //$("#txtToDate").datetimepicker({
    //    format: 'DD/MM/YYYY',
    //    defaultDate: new Date()
    //});


    $scope.GoBack = function () {
        $state.go('App.Accounts');
    }

    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');
    $scope.Role = getUserdata.Role;

    $scope.BalanceSheetParentLiabilitiesList = {};
    $scope.BalanceSheetforParentAssestsList = {};
    $scope.FinalLiabilitiesTotalAmount = 0;
    $scope.FinalAssestsTotalAmount = 0;
    GetBalanceSheetData();
    var startDate = new Date();
    var endDate = new Date();
    $scope.isLoss = false;
    $scope.isProfit = false;


    $scope.ChangeFinancialYear = function (value) {
        if (value != "") {
            var startYear = value.split('-')[0];
            var endYear = value.split('-')[1];
            startDate = new Date("04/01/" + startYear);
            endDate = new Date("03/31/" + endYear);
        }
    }

    function GetBalanceSheetData() {
        // startDate = $("#txtFromDate").data("DateTimePicker").date();
        //if (startDate !== null) {
        //    startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        //}
        // endDate = $("#txtToDate").data("DateTimePicker").date();
        //if (endDate !== null) {
        //    endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        //}
        var Financialyear = $("#ddlFinyear").val();
        if (Financialyear == "") {
            var currentYear = (new Date()).getFullYear();
            var NextYearDate = new Date(currentYear + 1, 01, 01)
            var NextYear = (NextYearDate).getFullYear();
            $("#ddlFinyear").val(currentYear + '-' + NextYear);
            $scope.FinYear = currentYear + '-' + NextYear;
            Financialyear = $("#ddlFinyear").val();
        }
        if (Financialyear != "") {
            var startYear = Financialyear.split('-')[0];
            var endYear = Financialyear.split('-')[1];

            startDate = new Date("04/01/" + startYear);
            endDate = new Date("03/31/" + endYear);

            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");

            var getdata = AppService.GetDataByQrystring("BalanceSheet", "GetBalanceSheetData", startDate, endDate, $scope.UserBranch.BranchId);
            getdata.then(function (p1) {
                if (p1.data != null) {
                    $scope.BalanceSheetforParentAssestsList = p1.data.BalanceSheetforParentAssestsList;
                    $scope.BalanceSheetParentLiabilitiesList = p1.data.BalanceSheetforParentLiabilitiesList;
                    $scope.FinalLiabilitiesTotalAmount = p1.data.FinalLiabilitiesTotalAmount;
                    $scope.FinalAssestsTotalAmount = p1.data.FinalAssestsTotalAmount;
                    $scope.isLoss = p1.data.isLoss;
                    $scope.isProfit = p1.data.isProfit;
                    $scope.PandLCurrentPeriodBalance = p1.data.PandLCurrentPeriodBalance;
                    $scope.PandLOpeningBalance = p1.data.PandLOpeningBalance;
                    $scope.PandLTotalAmount = p1.data.PandLTotalAmount;


                }
                else {
                    showToastMsg(3, 'Error in getting data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in getting data')
            });
        }
        else {
            showToastMsg(3, 'Please Select Financial Year')
        }

    }


    $scope.PrintBalanceSheetData = function () {
        //let startDate = $("#txtFromDate").data("DateTimePicker").date();
        //if (startDate !== null) {
        //    startDate = $filter("date")(new Date(startDate), "dd-MM-yyyy");
        //}
        //let endDate = $("#txtToDate").data("DateTimePicker").date();
        //if (endDate !== null) {
        //    endDate = $filter("date")(new Date(endDate), "dd-MM-yyyy");
        //}
        var Financialyear = $("#ddlFinyear").val();
        if (Financialyear != "") {
            var startYear = Financialyear.split('-')[0];
            var endYear = Financialyear.split('-')[1];

            startDate = new Date("04/01/" + startYear);
            endDate = new Date("03/31/" + endYear);
            startDate = $filter("date")(new Date(startDate), "dd-MM-yyyy");
            endDate = $filter("date")(new Date(endDate), "dd-MM-yyyy");
            var duration = startDate + ' to ' + endDate;
            $("#lblDuration").text(duration);
            $("#HeaderDiv").show();
            $("#divBalanceSheet").print();
            $("#HeaderDiv").hide();
        }
        else {
            showToastMsg(3, 'Please select any Financial Year')
        }
    };


    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        var Financialyear = $("#ddlFinyear").val();
        if (Financialyear != "") {
            var startYear = Financialyear.split('-')[0];
            var endYear = Financialyear.split('-')[1];

            startDate = new Date("04/01/" + startYear);
            endDate = new Date("03/31/" + endYear);

            GetBalanceSheetData();
        }
        else {
            showToastMsg(3, 'Please select any Financial Year')
        }
    };


    $scope.SearchData = function () {

        var Financialyear = $("#ddlFinyear").val();
        if (Financialyear != "") {
            var startYear = Financialyear.split('-')[0];
            var endYear = Financialyear.split('-')[1];

            startDate = new Date("04/01/" + startYear);
            endDate = new Date("03/31/" + endYear);

            GetBalanceSheetData();
        }
        else {
            showToastMsg(3, 'Please select any Financial Year')
        }
    }

    $scope.SearchClearData = function () {
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

            GetBalanceSheetData();
        }
        else {

        }
    }

})