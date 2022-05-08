angular.module("BSCCL").controller('RptLoanStatementDetailsController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $scope.AccountNumber = '';
        //GetLoanStatementDetails();
        $scope.isSearched = false;
    }

    $scope.isSearched = false;
    $scope.SearchLoanStatement = function () {
        if ($scope.AccountNumber != undefined && $scope.AccountNumber != '') {
            $scope.isSearched = true;
            GetLoanStatementDetails();
        }
        else {
            showToastMsg(3, 'Enter Loan Account Number')
        }
    }

    function GetLoanStatementDetails() {
        var getdata = AppService.GetDataByQuerystring("Report", "RptLoanStatementDetails", $scope.AccountNumber, $scope.UserBranch.BranchId)
        getdata.then(function (p1) {
            if (p1.data) {

                $scope.CustomerName = p1.data.CustomerName;
                $scope.LoanAmount = p1.data.LoanAmount;
                $scope.LoanTerm = p1.data.LoanTerm;
                $scope.InterestRate = p1.data.InterestRate;
                //$scope.MonthlyInstallmentAmount = p1.data.result.MonthlyInstallmentAmount;
                $scope.LoanAmountisationList = p1.data.result.LoanAmountisationList;
                $scope.TotalDebitAmount = p1.data.result.TotalDebitAmount;
                $scope.TotalCreditAmount = p1.data.result.TotalCreditAmount;
                $scope.TotalBalanceAmount = p1.data.result.TotalBalanceAmount;
                $scope.LoanTotalAmounttoPay = p1.data.LoanTotalAmounttoPay;
                $scope.LoanAccountNumber = p1.data.LoanAccountNumber;
                $scope.DisbursedDate = p1.data.DisbursedDate;

            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    $scope.SearchClearData = function () {
        $scope.AccountNumber = '';
        $scope.isSearched = false;
    }

    $scope.PrintLoanStatement = function () {
        $("#HeaderDiv").show();
        $("#divLoanStatement").print();
        $("#HeaderDiv").hide();
    };


});

