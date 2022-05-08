angular.module("BSCCL").controller('ChartsofAccountsController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $location, $q) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');
    $scope.Role = getUserdata.Role;

    $scope.GoBack = function () {
        $state.go('App.Accounts');
    }

    $scope.Accounts = {};
    $scope.ParentAccountList = {};
    $scope.SubAccountList = {};
    GetParentAccounts();
    function GetParentAccounts() {
        var getdata = AppService.GetDetailsWithoutId("ChartsofAccount", "GetParentAccounts");

        getdata.then(function (p1) {
            if (p1.data != null) {
                $scope.ParentAccountList = p1.data
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    //$rootScope.ChangeBranch = function () {
    //    $cookies.put('Branch', $scope.UserBranch.BranchId)
    //    $('#tblChartofAccount').dataTable().fnDraw();
    //};


    $scope.Add = function () {
        $scope.Accounts = null;
        $scope.SubAccountList = null;
        $("#txtAccountName").val('');
        $scope.ChartofAccount = {};
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    GetChartOfAccountList();



    function GetChartOfAccountList() {
        $scope.SearchRootAccount = $("#ddlSearchRootAccount").val();
        $scope.SearchParentAccount = $("#ddlSearchParentAccount").val();
        $('#tblChartofAccount').dataTable({
            "bFilter": false,
            "processing": false,
            "bInfo": true,
            "bServerSide": true,
            pageLength: 25,
            //"bLengthChange": true,
            "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
            "bSort": false,
            "bDestroy": true,
            searching: false,
            dom: 'l<"floatRight"B>frtip',
            buttons: [
                {
                    extend: 'pdf',
                },
                {
                    extend: 'print',
                }
            ],
            "sAjaxSource": urlpath + "ChartsofAccount/GetChartofAccountList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "ChartofAccountName", "value": $("#txtSearch").val() });
                aoData.push({ "name": "RootAccount", "value": $scope.SearchRootAccount });
                aoData.push({ "name": "ParentAccount", "value": $scope.SearchParentAccount });
                aoData.push({ "name": "AccountType", "value": $("#ddlSearchAccountType").val() });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    headers: { 'Authorization': 'Bearer ' + $cookies.getObject('User').access_token },
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                        IntialPageControlUser();
                    }
                });
            },
            "aoColumns": [{
                "mDataProp": "Name",
            },
            {
                "mDataProp": "ParentAccountName",
            },
            {
                "mDataProp": "AccountTypeName",
            },

            {
                "mDataProp": "Id",
                "mRender": function (data, type, full) {

                    return '<button class="btn btn-success btn-xs btn-flat btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>  <button class="btn btn-danger btn-xs btn-flat btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> ';
                },
                "sClass": "text-center"

            }]

        });
    }

    $scope.SearchData = function () {
        GetChartOfAccountList();
    }

    $scope.SearchClearData = function () {
        $('#txtSearch').val('');
        $("#ddlSearchParentAccount").val('');
        $("#ddlSearchRootAccount").val('');
        $("#ddlSearchAccountType").val('');
        GetChartOfAccountList();
        $scope.SubAccountList = null;
    }

    $scope.ClearForm = function () {
        $scope.Accounts = null;
        $scope.SubAccountList = null;
        $("#txtSearch").val("");
        $("#ddlParentAccount").val("");
        $("#ddlRootAccount").val("");
        $("#ddlAccountType").val("");
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }


    function IntialPageControlUser() {

        $(".btnEdit").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $("#divChartofAccount").modal('show');
            GetAccountDetailsById(ID)
        });

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
                            var promiseDelete = AppService.DeleteData("ChartsofAccount", "DeleteAccount", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {

                                    showToastMsg(1, "Account Deleted Successfully");
                                    GetChartOfAccountList();
                                }
                                else {
                                    showToastMsg(3, "You can not delete this Account.");
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
    }

    function RefreshDataTablefnDestroy() {
        $("#tblUser").dataTable().fnDestroy();
    }


    function GetAccountDetailsById(Id) {
        var getuserdata = AppService.GetDetailsById("ChartsofAccount", "GetAccountDetailsById", Id);

        getuserdata.then(function (p1) {
            if (p1.data != null) {
                $scope.Accounts = p1.data;
                $scope.SubAccountList = {};
                if (p1.data.RootId != null) {
                    $scope.SubAccountList = $scope.GetSubAccounts(p1.data.RootId);
                }
                if (p1.data.ParentId != null) {
                    $scope.SubAccountList = $scope.GetSubAccounts(p1.data.RootId);
                }

                if (p1.data.RootId != null) {
                    $scope.Accounts.RootId = p1.data.RootId + "";
                }
                else if (p1.data.RootId == null) {
                    $scope.Accounts.RootId = p1.data.ParentId + "";
                }
                else {
                    if (p1.data.ParentId != null) {
                        $scope.Accounts.RootId = p1.data.ParentId + "";
                    }
                }

                if (p1.data.ParentId == null && p1.data.RootId == null) {
                    $scope.SubAccountList = null;
                }
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    $scope.GetSubAccounts = function (RootId) {
        if (RootId != null) {
            var getdata = AppService.GetDetailsById("ChartsofAccount", "GetSubAccounts", RootId);

            getdata.then(function (p1) {
                if (p1.data != null) {
                    $scope.SubAccountList = p1.data
                }
                else {
                    showToastMsg(3, 'Error in getting data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in getting data')
            });

        }
        else {
            $scope.SubAccountList = null;
        }
    }


    $scope.SaveAccount = function () {

        var flag = true;
        flag = ValidateAccountForm();
        if (flag) {
            $(':focus').blur();

            if ($("#ddlAccountType").val() != "") {
                $scope.Accounts.AccountType = $("#ddlAccountType").val();
            }

            if ($scope.Accounts.ParentId == null) {
                if ($scope.Accounts.RootId != null) {
                    $scope.Accounts.ParentId = $scope.Accounts.RootId;
                }
            }
            $scope.Accounts.BranchId = $scope.UserBranch.BranchId;


            var savedata = AppService.SaveData("ChartsofAccount", "SaveAccount", $scope.Accounts)

            savedata.then(function (p1) {
                if (p1.data) {
                    $("#divChartofAccount").modal('hide');
                    GetChartOfAccountList();
                    showToastMsg(1, "Data Saved Successfully");
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in saving data')
            });
        }
    }



    function ValidateAccountForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtAccountName"), 'Account Name required', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#ddlRootAccount"), 'Root Account is required', 'after')) {
            flag = false;
        }

        return flag;
    }

})