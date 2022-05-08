angular.module("BSCCL").controller('AccountsHeadController', function ($scope, AppService, $state, $cookies, $filter, $rootScope) {
    $scope.UserBranch.ShowBranch = false;
    $scope.UserBranch.Enabled = false;

    GetAccountsHeadList();
    GetAccountsHeadListParentHead()
    $scope.AddAccountHead = function () {
        $scope.AccountsHead = {};
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $('#AccountsHead').modal({
        backdrop: 'static',
        show: false,
        keyboard: false  // to prevent closing with Esc button (if you want this too)
    });

    $rootScope.CountValue = 0;

    function GetAccountsHeadDataById(Id) {
        var getheaddata = AppService.GetDetailsById("AccountsHead", "GetaccountsheadDataById", Id);

        getheaddata.then(function (p1) {
            if (p1.data != null) {

                $scope.AccountsHead = p1.data
                $scope.AccountsHead.HeadType = $scope.AccountsHead.HeadType + ""
                $scope.AccountsHead.ParentHead = $scope.AccountsHead.ParentHead + ""
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function GetAccountsHeadListParentHead() {
        var getheadlist = AppService.GetDetailsWithoutId("AccountsHead", "GetAccountListforParent");

        getheadlist.then(function (p1) {
            if (p1.data != null) {
                $scope.ParentHeadList = p1.data
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.SaveAccountsHead = function () {
        var flag = true;
        flag = ValidateAccountsHeadForm();
        if (flag) {

            var savehead = AppService.SaveData("AccountsHead", "SaveAccountsHead", $scope.AccountsHead)
            savehead.then(function (p1) {
                if (p1.data != false) {
                    $('#AccountsHead').modal('hide');
                    showToastMsg(1, "Accounts Head Saved Successfully")
                    GetAccountsHeadListParentHead()
                    GetAccountsHeadList()
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in saving data')
            });
        }
    }

    $scope.ClearForm = function () {
        $scope.AccountsHead = {};
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    function GetAccountsHeadList() {

        $('#tblAccountsHead').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/AccountsHead/GetAccountsHead",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {

                        fnCallback(json);
                        IntialPageControlUser();
                    }
                });
            },
            "aoColumns": [{
                "mDataProp": "HeadName",
            },
             {
                 "mDataProp": "Description",
             },
            {
                "mDataProp": "HeadTypeName",
            },
            {
                "mDataProp": "ParentHeadName",
            },
            {
                "mDataProp": "AccountsHeadId",
                "mRender": function (data, type, full) {

                    return '<button class="btn btn-success btn-xs btn-flat btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>  <button class="btn btn-danger btn-xs btn-flat btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> ';
                },
                "sClass": "text-center"

            }]

        });
    }
    $scope.SearchData = function () {
        $('#tblAccountsHead').dataTable().fnDraw();
    }
    $scope.SearchClearData = function () {
        $('#txtSearch').val('');
        $('#tblAccountsHead').dataTable().fnDraw();
    }

    function IntialPageControlUser() {

        $(".btnEdit").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $("#AccountsHead").modal('show');
            GetAccountsHeadDataById(ID)
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
                            var promiseDelete = AppService.GetDetailsById("AccountsHead", "DeleteAccountsHead", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {
                                    toastr.remove();
                                    showToastMsg(1, "Accounts Head Deleted Successfully");
                                    RefreshDataTablefnDestroy();
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
    }
    function RefreshDataTablefnDestroy() {
        $("#tblAccountsHead").dataTable().fnDraw();
    }
    function ValidateAccountsHeadForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#txtheadName"), 'Head Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#dpdheadType"), 'Type required', 'after')) {
            flag = false;
        }

        return flag;
    }

})