angular.module("BSCCL").controller('RptBankBookController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')


    $scope.BranchHO = $filter('filter')($scope.BranchList, { BranchId: $cookies.get('Branch') })[0].IsHO

    var table;

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
        defaultDate: new Date()
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $scope.BranchHO = $filter('filter')($scope.BranchList, { BranchId: $cookies.get('Branch') })[0].IsHO;
        GetBankBookDetails();
    }

    $scope.SearchCRDR = function () {
        GetBankBookDetails()
        $("#divDuration").show();
    }

    $scope.ClearSearch = function () {
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $("#txtStartDateforSearch").data("DateTimePicker").date(null);
        $("#txtEndDateforSearch").data("DateTimePicker").date(null);

        GetBankBookDetails()
        $("#divDuration").hide();
    }

    GetBankBookDetails()

    function GetBankBookDetails() {
        let startDate = $("#txtStartDateforSearch").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtEndDateforSearch").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }

        var getdata = AppService.GetDataByQrystring("Report", "RptBankBook", $scope.UserBranch.BranchId, startDate, endDate)
        getdata.then(function (p1) {
            if (p1.data) {
                $scope.BankBookData = p1.data.rptlist1;
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
    
    $scope.PrintBankBookReport = function () {
        $("#HeaderDiv").show();
        $("#divBankBook").print();
        $("#HeaderDiv").hide();
    };

});