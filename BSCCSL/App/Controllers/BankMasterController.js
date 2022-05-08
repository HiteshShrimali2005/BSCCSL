angular.module("BSCCL").controller('BankMasterController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')
    $scope.ShowBankMaster = false;
    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');

    if ($scope.UserBranch.ShowBranch == true && getUserdata.Role == "1") {
        $scope.ShowBankMaster = true;
    }

    $('#ddlBranchList').multiselect({
        enableFiltering: true,
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true,
        buttonWidth: '270px'
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblBankMasterList').dataTable().fnDraw();
    }

    $scope.SearchData = function () {
        if ($scope.AccountNum != null && $scope.AccountNum != undefined && $scope.AccountNum != "") {
            $('#tblBankMasterList').dataTable().fnDraw();
        }
        else {
            showToastMsg(3, 'Enter Account Number');
        }
    }

    $scope.SaveBankDetails = function () {

        var flag = true;
        flag = ValidateBankDetails();

        if (flag) {
            $(':focus').blur();
            if ($scope.bank.BankId == '0000000-0000-0000-0000-000000000000' || $scope.bank.BankId != undefined || $scope.bank.BankId == "") {
                $scope.bank.CreatedBy = $cookies.getObject('User').UserId;
            }
            else {
                $scope.bank.ModifiedBy = $cookies.getObject('User').UserId;
            }

            var selected = $("#ddlBranchList option:selected");
            if (selected.length != 0) {
                $scope.BranchIds = [];
                selected.each(function () {
                    if ($(this).val() != "multiselect-all") {
                        $scope.BranchIds.push($(this).val());
                    }
                });
            } else {
                $scope.BranchIds = [];
            }

            var NewObj = new Object();
            NewObj.Bank = $scope.bank;
            NewObj.BranchIds = $scope.BranchIds;

            AppService.SaveData("BankMaster", "SaveBankDetails", NewObj).then(function (p1) {
                if (p1.data) {
                    $scope.ClearForm();
                    $("#BankMasterModal").modal("hide");
                    showToastMsg(1, "Bank details saved Successfully");
                    $('#tblBankMasterList').dataTable().fnDraw();
                }
                else {
                    showToastMsg(3, 'Error in Saving Data')
                }
            });
        }
        else {
            return false;
        }
    }

    $scope.ClearForm = function () {
        $scope.bank = { IsActive: false };
        var selected = $("#ddlBranchList option:selected");
        selected.each(function () {
            if ($scope.BranchHO == true) {
                if ($(this).val() != $scope.UserBranch.BranchId) {
                    $('#ddlBranchList').multiselect('deselect', $(this).val())
                }
            } else {
                $('#ddlBranchList').multiselect('deselect', $(this).val())
            }
        });
        $('#ddlBranchList').multiselect('destroy');
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    function ValidateBankDetails() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtBankName"), 'Bank name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtAccountNumber"), 'Account number required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlAccountType"), 'Account type required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtbalance"), 'Balance required', 'after')) {
            flag = false;
        }

        var select = [];
        select = $("#ddlBranchList option:selected")
        if (select.length == 0) {
            $("#ModeDiv").closest('.form-group').addClass('has-error');
            PrintMessage($("#ModeDiv"), 'Branch required', 'after');
            flag = false;
        }

        return flag;
    }

    GetAllBankList()

    function GetAllBankList() {

        $('#tblBankMasterList').dataTable({
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
            "sAjaxSource": urlpath + "/BankMaster/GetAllBankList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $scope.AccountNum });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                        IntialPageControlBankList();
                    }
                });
            },
            "aoColumnDefs": [
                {
                    "bVisible": $scope.ShowBankMaster == true, "aTargets": [3]
                }
            ],
            "aoColumns": [
                {
                    "mDataProp": "AccountNumber",

                },
                {
                    "mDataProp": "BankName",

                },
                {
                    "mDataProp": "AccountType",
                    "mRender": function (data, type, full) {
                        if (data == 1) {
                            return "Saving Account";
                        } else {
                            return "Current Account";
                        }
                    },
                },
                {
                    "mDataProp": "Balance",
                    "sClass": "text-right",
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(data, '', 2);
                    },
                },
                {
                    "mDataProp": "IsActive",
                    "sClass": "text-center",
                    "mRender": function (data, type, full) {
                        if (data == true) {
                            return '<span class="label label-success">Active</span>'
                        } else {
                            return '<span class="label label-danger">Deactive</span>'
                        }
                    },
                },
                {
                    "mDataProp": "BankId",
                    "mRender": function (data, type, full) {
                        var str = "";
                        if ($scope.ShowBankMaster == true) {
                            str += '<a class="btn btn-success btn-xs btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</a>&nbsp;&nbsp;'
                        }
                        var acNo = btoa(full.BankId);
                        str += ' <a href="/App/BankTransaction?BankId=' + acNo + '" class="btn btn-primary btn-xs btnTransaction" title="Transaction"><span class="glyphicon glyphicon-money"></span> <i class="glyphicon glyphicon-plus"> </i> Transaction</a>&nbsp;&nbsp;'
                        if ($scope.ShowBankMaster == true) {
                            str += '<button class="btn btn-danger btn-xs btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> '
                        }
                        return str;
                    },
                    "sClass": "text-center"
                }
            ]
        });
    }

    function IntialPageControlBankList() {

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
                            var promiseDelete = AppService.DeleteData("BankMaster", "DeleteBank", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {
                                    RefreshDataTablefnDestroy();
                                    toastr.remove();
                                    showToastMsg(1, "Bank Deleted Successfully");
                                    GetAllBankList();
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

        $(".btnEdit").click(function () {
            var ID = $(this).attr("Id");
            AppService.GetDetailsById("BankMaster", "GetBankDetailById", ID).then(function (p1) {
                if (p1.data) {
                    $("#BankMasterModal").modal("show");
                    $scope.bank = p1.data;
                    $('#ddlBranchList').multiselect('select', p1.data.BranchMapping);
                    $('#ddlBranchList').multiselect('refresh');
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                }
            })
        });
    }

    $scope.SearchClearData = function () {
        $scope.AccountNum = '';
        // $("#txtAccountlistsearch").val('');
        $('#tblBankMasterList').dataTable().fnDraw();
    }

    function RefreshDataTablefnDestroy() {
        $("#tblBankMasterList").dataTable().fnDestroy();
    }

    $scope.Add = function () {
        $('#ddlBranchList').multiselect({
            enableFiltering: true,
            includeSelectAllOption: true,
            enableCaseInsensitiveFiltering: true,
            buttonWidth: '270px'
        });
    }

    $scope.$on('ngRepeatFinished', function (ngRepeatFinishedEvent) {
        $('#ddlBranchList').multiselect('destroy')
        $('#ddlBranchList').multiselect({
            enableFiltering: true,
            includeSelectAllOption: true,
            enableCaseInsensitiveFiltering: true,
            buttonWidth: '270px'
        });
    });
});