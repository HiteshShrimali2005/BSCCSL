angular.module("BSCCL").controller('LoanListController', function ($scope, $state, $cookies, $filter, AppService, $http, $location, $rootScope) {

    var getUserdata = new Object();

    getUserdata = $cookies.getObject('User');
    $scope.GroupLoanId = null;

    if ($location.search().GroupLoanId != undefined) {
        $scope.GroupLoanId = $location.search().GroupLoanId;
        $location.search('GroupLoanId', null);
    }
    
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

    GetLoanList();
    function GetLoanList() {

        $('#tblLoan').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/Loan/LoanList",

            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "GroupLoanId", "value": $scope.GroupLoanId });
                aoData.push({ "name": "role", "value": getUserdata.Role });
                aoData.push({ "name": "LoanStatus", "value": $("#ddlLoanStatus").val() });
                aoData.push({ "name": "ConsumerProductName", "value": $("#txtConsumerProductName").val() });

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
                //"mDataProp": "CustomerName"
                "mDataProp": "CustomerName",
                "mRender": function (data, type, full) {
                    return '<a href="/App/CustomerProduct?CustomerId=' + full.CustomerId + '" title="View" target="_blank">' + full.CustomerName + '</a>'
                },

            },
             {
                 "mDataProp": "GroupLoanNumber",
             },
            {
                "mDataProp": "ProductName",
            },
             {
                 "mDataProp": "LoanTypeName",

             },
            {
                "mRender": function (data, type, full) {
                    return $filter('currency')(full.LoanAmount,'',2)
                },
                "sClass": "text-right"
            },
            {
                "mDataProp": "InterestRate",
            },
            {
                 "mDataProp": "LoanStatusName",
            },
            {
                "mDataProp": "LoanId",
                "mRender": function (data, type, full) {
                    var str = ''

                    if (full.GroupLoanId != null) {
                        str = str +'<a href="/App/Loan?CustomerProductId=' + full.CustomerProductId + '&LoanId=' + data + '&GroupLoanId=' + full.GroupLoanId + '" class="btn btn-success btn-xs btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</a> ';
                    } else {
                        str = str + '<a href="/App/Loan?CustomerProductId=' + full.CustomerProductId + '&LoanId=' + data + '" class="btn btn-success btn-xs btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</a>';
                    }
                    str += '<button class="btn btn-danger btn-xs btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button>'

                    return str;
                },
                "sClass": "text-center"
            }]
        });
    }

    $scope.SearchData = function () {

        $('#tblLoan').dataTable().fnDraw();
    }
    $scope.SearchClearData = function () {
        $('#txtSearch').val('');
        $('#txtConsumerProductName').val('');
        $("#ddlLoanStatus").val("");
        $('#tblLoan').dataTable().fnDraw();
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
                                    showToastMsg(1, "Loan Deleted Successfully");
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