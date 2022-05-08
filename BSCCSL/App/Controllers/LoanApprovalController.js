angular.module("BSCCL").controller('LoanApprovalController', function ($scope, $state, $cookies, $filter, AppService, $http, $location, $rootScope) {


    var getUserdata = new Object();

    getUserdata = $cookies.getObject('User');

    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')
    $scope.BranchCode = $filter('filter')($scope.BranchList, { BranchId: $scope.UserBranch.BranchId })[0].BranchCode;
    $rootScope.ChangeBranch = function () {
        $scope.BranchCode = $filter('filter')($scope.BranchList, { BranchId: $scope.UserBranch.BranchId })[0].BranchCode;
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblLoan').dataTable().fnDraw();
    }

    //$rootScope.CountValue = 1;

    GetLoanApprovalList();
    function GetLoanApprovalList() {

        $('#tblLoanApprovalList').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/Loan/LoanApprovalList",

            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {

                        fnCallback(json);
                        IntialPageControlLoanList();
                    }
                });
            },
            "aoColumns": [{
                "mDataProp": "AccountNumber"
            },
            {
                "mDataProp": "ProductName",

            },
             {
                 "mDataProp": "LoanType",

             },
            {
                "mDataProp": "IntrestRate",
            },
             {
                 "mDataProp": "LoanStatus",
             },
            {
                "mDataProp": "LoanId",
                "mRender": function (data, type, full) {
                    return '<a href="/App/Loan?CustomerProductId=' + full.ProductId + '&LoanId=' + data + '" class="btn btn-success btn-xs btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</a>';
                },
                "sClass": "text-center"
            }]
        });
    }
    $scope.SearchData = function () {

        $('#tblLoanApprovalList').dataTable().fnDraw();
    }
    $scope.SearchClearData = function () {
        $('#txtSearch').val('');
        $('#tblLoanApprovalList').dataTable().fnDraw();
    }

    function IntialPageControlLoanList() {

        $(".btnDelete").click(function () {

            var ID = $(this).attr("Id");
            bootbox.dialog({
                message: "Are you sure want to delete?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {

                            var promiseDelete = AppService.DeleteData("Loan", "DeleteLoan", ID);
                            promiseDelete.then(function (p1) {

                                var status = p1.data;
                                if (status == true) {
                                    RefreshDataTablefnDestroy();
                                    toastr.remove();
                                    showToastMsg(1, "Customer Deleted Successfully");
                                    GetLoanList();
                                    GetCustomerList();
                                }
                                else {
                                    showToastMsg(3, "Error Occured While Deleting");
                                }
                            }, function (err) {
                                showToastMsg(3, "Error Occured");
                            });

                        }
                    },
                    danger: {
                        label: "No",
                        className: "btn-danger btn-flat"
                    }
                }
            });
        });

        $(".btnCust_ProductEdit").click(function () {

            var ID = $(this).attr("Id");
            $rootScope.Cust_Id = ID;
        });
    }

    function RefreshDataTablefnDestroy() {
        $("#tblLoan").dataTable().fnDestroy();
    }
});