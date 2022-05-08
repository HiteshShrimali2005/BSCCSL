angular.module("BSCCL").controller('RptLoanStatementController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblLoanStatement').dataTable().fnDraw();
    }

    $scope.SearchLoanStatement = function () {
        $('#tblLoanStatement').dataTable().fnDraw();
    }

    GetLoanStatement()
    function GetLoanStatement() {
        $('#tblLoanStatement').dataTable({
            "bFilter": false,
            "processing": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": true,
            "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
            "bSort": false,
            "bDestroy": true,
            searching: false,
            dom: 'l<"floatRight"B>frtip',
            buttons: [
                {
                    extend: 'pdf',
                    //footer: true
                },
                {
                    extend: 'print',
                    //footer: true
                }
            ],
            "sAjaxSource": urlpath + "/Report/RptLoanStatement",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $scope.CustomerName });
                aoData.push({ "name": "AccountNumber", "value": $scope.AccountNumber });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {


                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                        fnCallback(json);
                    }
                });
            },
            "aoColumns": [
                {
                    "mDataProp": "ClienId",
                },
                {
                    "mDataProp": "CustomerName",
                },

                {
                    "mDataProp": "AccountNumber",

                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(full.TotalLoanAmount, '₹ ', 2);
                    },
                    "sClass": "text-right"
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(full.PaidAmount, '₹ ', 2);
                    },
                    "sClass": "text-right"
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(full.PendingAmount, '₹ ', 2);
                    },
                    "sClass": "text-right"
                },
                {
                    "mRender": function (data, type, full) {
                        if (full.LoanTypeName != 'Flexi Loan') {
                            return data = full.TotalInstallment;
                        }
                        else {
                            return data = '0';
                        }
                    },
                },
                {
                    "mRender": function (data, type, full) {
                        if (full.LoanTypeName != 'Flexi Loan') {
                            return data = full.TotalPaidInstallment;
                        }
                        else {
                            return data = '0';
                        }
                    },
                },
                {
                    "mRender": function (data, type, full) {
                        if (full.LoanTypeName != 'Flexi Loan') {
                            return data = full.TotalUnPaidInstallment;
                        }
                        else {
                            return data = '0';
                        }
                    },
                },
                {
                    "mRender": function (data, type, full) {
                        if (full.LoanTypeName != 'Flexi Loan') {
                            return data = full.TotalPendingInstallment;
                        }
                        else {
                            return data = '0';
                        }
                    },
                },



            ]
        });
    }

    $scope.SearchClearData = function () {
        $scope.CustomerName = '';
        $scope.AccountNumber = '';
        $('#tblLoanStatement').dataTable().fnDraw();
    }


});

