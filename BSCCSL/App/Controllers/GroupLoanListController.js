angular.module("BSCCL").controller('GroupLoanListController', function ($scope, $state, $cookies, $filter, AppService, $http, $location, $rootScope) {

    var getUserdata = new Object();

    getUserdata = $cookies.getObject('User');

    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')
    $scope.BranchCode = $filter('filter')($scope.BranchList, { BranchId: $scope.UserBranch.BranchId })[0].BranchCode;
    $rootScope.ChangeBranch = function () {
        $scope.BranchCode = $filter('filter')($scope.BranchList, { BranchId: $scope.UserBranch.BranchId })[0].BranchCode;
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblGroupLoan').dataTable().fnDraw();
    }

    //$rootScope.CountValue = 1;

    GetGroupLoanList();
    function GetGroupLoanList() {

        $('#tblGroupLoan').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/Loan/GroupLoanList",

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
                "mDataProp": "GroupLoanNumber",
                "mRender": function (data, type, full) {
                    return '<a href="/App/Loanlist?GroupLoanId=' + full.GroupLoanId + '" class="btnRedirect" title="View">' + data + '</a>';
                },
            },
             {
                 "mDataProp": "GroupName",
             },
            {
                "mDataProp": "ProductName",
            },
              {
                  "mRender": function (data, type, full) {
                      return $filter('currency')(full.LoanAmount, '', 2)
                  },
                  "sClass": "text-right"
              },
            {
                "mDataProp": "InterestRate",
                "sClass": "text-center"
            },
             {
                 "mDataProp": "NoOfCustomer",
                 "sClass": "text-center"
             },
            {
                "mDataProp": "GroupLoanId",
                "mRender": function (data, type, full) {
                    return '<a href="/App/GroupLoan?GroupLoanId=' + full.GroupLoanId + '" class="btn btn-success btn-xs btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</a>  <button class="btn btn-danger btn-xs btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> ';
                },
                "sClass": "text-center"
            }]
        });
    }
    $scope.SearchData = function () {

        $('#tblGroupLoan').dataTable().fnDraw();
    }
    $scope.SearchClearData = function () {
        $('#txtSearch').val('');
        $('#tblGroupLoan').dataTable().fnDraw();
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

                            var promiseDelete = AppService.DeleteData("Loan", "DeleteGroupLoan", ID);
                            promiseDelete.then(function (p1) {

                                var status = p1.data;
                                if (status == true) {
                                    RefreshDataTablefnDestroy();
                                    toastr.remove();
                                    showToastMsg(1, "Loan Deleted Successfully");
                                    GetGroupLoanList();
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
        $("#tblGroupLoan").dataTable().fnDestroy();
    }
});