angular.module("BSCCL").controller('CustomerListController', function ($scope, $state, $cookies, $filter, AppService, $http, $location, $rootScope) {

    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');

    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblCustomer').dataTable().fnDraw();
    }

    GetCustomerList();

    function GetCustomerList() {

        $('#tblCustomer').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/Customer/GetCustomerList",

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
                        IntialPageControlCustomer();
                    }
                });
            },
            "aoColumns": [{
                "mDataProp": "ClientId"
            },
            {
                "mDataProp": "CustomerName",
                "mRender": function (data, type, full) {
                    return '<a href="/App/CustomerProduct?CustomerId=' + full.CustomerId + '" class="btnCust_ProductEdit" Id="' + full.CustomerId + '" title="Edit">' + full.CustomerName + '</a>'
                },
            },
            {
                "mDataProp": "BranchName",
            },
            {
                "mDataProp": "CustomerId",
                "mRender": function (data, type, full) {
                    var str = ' <a href="/App/Customer?CustomerId=' + data + '" class="btn btn-success btn-xs btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</a>'
                    //if (getUserdata.Role==1)
                    //{
                    //    str += ' <button class="btn btn-danger btn-xs btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button>'
                    //}
                    return str;
                },
                "sClass": "text-center"
            }]
        });
    }

    $scope.SearchData = function () {
        $('#tblCustomer').dataTable().fnDraw();
    }

    $scope.SearchClearData = function () {
        $('#txtSearch').val('');
        $('#tblCustomer').dataTable().fnDraw();
    }

    function IntialPageControlCustomer() {

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
                            var promiseDelete = AppService.DeleteData("Customer", "DeleteCustomer", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {
                                    RefreshDataTablefnDestroy();
                                    toastr.remove();
                                    showToastMsg(1, "Customer Deleted Successfully");
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
        $("#tblCustomer").dataTable().fnDestroy();
    }
});