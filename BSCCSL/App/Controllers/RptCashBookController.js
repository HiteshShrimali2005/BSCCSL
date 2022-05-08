angular.module("BSCCL").controller('RptCashBookController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')


    $scope.BranchHO = $filter('filter')($scope.BranchList, { BranchId: $cookies.get('Branch') })[0].IsHO

    var table;

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
        //defaultDate: new Date("04/21/2017")
        defaultDate: new Date()
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $scope.BranchHO = $filter('filter')($scope.BranchList, { BranchId: $cookies.get('Branch') })[0].IsHO;
        GetCashBookDetails();
    }

    $scope.SearchCRDR = function () {
        GetCashBookDetails()
        $("#divDuration").show();
    }

    $scope.ClearSearch = function () {
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $("#txtStartDateforSearch").data("DateTimePicker").date(null);
        $("#txtEndDateforSearch").data("DateTimePicker").date(null);

        GetCashBookDetails()
        $("#divDuration").hide();
    }

    GetCashBookDetails()

    function GetCashBookDetails() {
        let startDate = $("#txtStartDateforSearch").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtEndDateforSearch").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }
        var getdata = AppService.GetDataByQrystring("Report", "RptCashBook", $scope.UserBranch.BranchId, startDate, endDate)
        getdata.then(function (p1) {
            if (p1.data) {
                $scope.CashBookData = p1.data.rptlist1;
                $scope.TotalCredit = p1.data.TotalCredit;
                $scope.TotalDebit = p1.data.TotalDebit;
                $scope.FirstDate = p1.data.FirstDate;
                $scope.LastDate = p1.data.LastDate;
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

    $scope.PrintCashBookReport = function () {
        $("#HeaderDiv").show();
        $("#divCashBook").print();
        $("#HeaderDiv").hide();
    };


});