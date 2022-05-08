angular.module("BSCCL").controller('RptTrailBalanceController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')


    $scope.BranchHO = $filter('filter')($scope.BranchList, { BranchId: $cookies.get('Branch') })[0].IsHO

    var table;

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
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
        GetTrailBalanceDetails();
    }

    $scope.Search = function () {
        GetTrailBalanceDetails()
        $("#divDuration").show();
    }

    $scope.ClearSearch = function () {
        $scope.FinYear = '';
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $("#txtStartDateforSearch").data("DateTimePicker").date(null);
        $("#txtEndDateforSearch").data("DateTimePicker").date(null);
        GetTrailBalanceDetails()
        $("#divDuration").hide();
    }

    //GetTrailBalanceDetails()

    function GetTrailBalanceDetails() {
        let startDate = $("#txtStartDateforSearch").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtEndDateforSearch").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }

        var getdata = AppService.GetDataByQrysr("Report", "RptTrailBalance", $scope.UserBranch.BranchId, ($scope.FinYear ? $scope.FinYear : ""), startDate, endDate)
        getdata.then(function (p1) {
            if (p1.data) {
                $scope.TrailBalanceData = p1.data.rptlist1;
                $scope.TotalCredit = p1.data.TotalCredit;
                $scope.TotalDebit = p1.data.TotalDebit;
                $scope.FirstDate = p1.data.FirstDate;
                $scope.LastDate = p1.data.LastDate;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }
    
    $scope.PrintTrailBalanceReport = function () {
        $("#HeaderDiv").show();
        $("#divTrailBalance").print();
        $("#HeaderDiv").hide();
    };

});