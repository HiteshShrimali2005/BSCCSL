angular.module("BSCCL").controller('JournalVoucherController', function ($scope, AppService, $state, $cookies, $filter, $rootScope) {

    $scope.UserBranch.ShowBranch = true;
    $scope.UserBranch.Enabled = true;

    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblJournalVoucher').dataTable().fnDraw();
    }

    $(".datepicker").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });


    GetJournalVoucherList();

    $scope.Add = function () {
        $("#txtJVDate").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy'));

        $scope.JournalVoucher = {};
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        GetFromBranch();
        GetToBranch();
        $scope.CustomerAccountNumber = '';
        $scope.ProductType = '';
        $scope.CustomerName = '';
        $scope.Type = '' + '';
        $scope.JournalVoucher.JournalVoucherId = null;
    };

    function ValidateJournalVoucher() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#txtJournalVoucherName"), 'Name is required', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#txtJVDate"), 'Date required', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#ddlType"), 'Type is required', 'after')) {
            flag = false;
        }


        if (!ValidateRequiredField($("#txtJVNumber"), 'JV Number required', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#txtAccountNumber"), 'Account Number is required', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#txtAmount"), 'Amount is required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlToAccountHead"), 'Account Head is required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlFromAccountHead"), 'Account Head is required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlFromSubAccountHead"), 'Sub Head is required', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#ddlToSubAccountHead"), 'Sub Head is required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlTransactionMode"), 'Transaction Mode is required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlFromBranch"), 'From Branch is required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlToBranch"), 'To Branch is required', 'after')) {
            flag = false;
        }



        return flag;
    }

    $scope.SaveJournalVoucher = function () {
        var flag = true;
        flag = ValidateJournalVoucher();

        if (flag) {
            $scope.JournalVoucher.BranchId = $scope.UserBranch.BranchId;
            $scope.JournalVoucher.JVDate = moment(new Date($("#txtJVDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            $scope.JournalVoucher.ToAccount = $scope.CustomerAccountNumber;
            $scope.JournalVoucher.Type = $("#ddlType").val();

            var SaveTerm = AppService.SaveData("JournalVoucher", "SaveJournalVoucher", $scope.JournalVoucher);
            SaveTerm.then(function (p1) {
                if (p1.data != null) {
                    $('#JournalVoucher').modal('hide');
                    GetJournalVoucherList();
                    showToastMsg(1, "Data Saved Successfully")
                    $scope.ClearTerm();
                }
                else {
                    showToastMsg(3, 'Error in Saving Data')
                }
            })
        }
    }

    $("#ddlFromAccountHead").on("change", function () {
        var ParentHead = $(this).val();
        GetFromSubHead(ParentHead);
    });

    $("#ddlToAccountHead").on("change", function () {
        var ParentHead = $(this).val();
        GetToSubHead(ParentHead);
    });

    function GetFromSubHead(ParentHead) {
        var getheaddata = AppService.GetDetailsById("AccountsHead", "GetSubHead", ParentHead);

        getheaddata.then(function (p1) {
            if (p1.data != null) {
                $scope.AccountsHeadList = p1.data
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function GetToSubHead(ParentHead) {
        var getheaddata = AppService.GetDetailsById("AccountsHead", "GetSubHead", ParentHead);

        getheaddata.then(function (p1) {
            if (p1.data != null) {
                $scope.ToAccountsHeadList = p1.data
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function GetFromBranch() {
        var getheaddata = AppService.GetDetailsById("Branch", "GetAllBranch", getUserdata.UserId);

        getheaddata.then(function (p1) {
            if (p1.data != null) {
                $scope.FromBranchList = p1.data
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function GetToBranch() {
        var getheaddata = AppService.GetDetailsById("Branch", "GetAllBranch", getUserdata.UserId);

        getheaddata.then(function (p1) {
            if (p1.data != null) {
                $scope.ToBranchList = p1.data
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.ClearForm = function () {
        $scope.JournalVoucher = {};
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    function GetJournalVoucherList() {

        $('#tblJournalVoucher').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            'bAutoWidth': false,
            //"scrollX": true,
            "sAjaxSource": urlpath + "/JournalVoucher/GetJournalVoucherList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                        IntialPageControlJournalVoucher();
                    }
                });
            },
            "aoColumns": [
                {
                    "sTitle": "Customer Name",
                    "data": "LastName",
                    "className": "left",
                    "render": function (data, type, full, meta) {
                        return full.FirstName + ' ' + full.MiddleName + ' ' + full.LastName;
                    }

                },
                {
                    "sTitle": "Account Number",
                    "mDataProp": "ToAccount",
                },

                {
                    "sTitle": "JV Number",
                    "mDataProp": "JVNumber",
                },

                {
                    "sTitle": "Date",
                    "mRender": function (data, type, full) {
                        return data = $filter('date')(full.JVDate, 'dd/MM/yyyy');
                    },
                },
                {
                    "sTitle": "Type",
                    "mRender": function (data, type, full) {
                        if (full.Type == "1")
                            return "Credit"
                        else
                            return "Debit"
                    },
                },

                {
                    "sTitle": "Amount",
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(full.Amount, '₹ ', 2);
                    },
                    "sClass": "text-right"
                },

                {
                    "sTitle": "Action",
                    "mDataProp": "JournalVoucherId",
                    "mRender": function (data, type, full) {

                        var str = ''
                        str += ' <button class="btn btn-success btn-xs btn-flat btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>  <button class="btn btn-danger btn-xs btn-flat btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> '
                        return str;
                    },
                    "sClass": "text-center",
                    "sWidth": "150px"
                }
            ]

        });
    }

    function IntialPageControlJournalVoucher() {

        $(".btnEdit").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $("#JournalVoucher").modal('show');
            GetJournalVoucherDataById(ID)
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
                            var promiseDelete = AppService.DeleteData("JournalVoucher", "DeleteJournalVoucherById", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {
                                    toastr.remove();
                                    showToastMsg(1, "Data Deleted Successfully");
                                    $("#tblJournalVoucher").dataTable().fnDraw();
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

    function GetJournalVoucherDataById(Id) {
        var getjournalVoucher = AppService.GetDetailsById("JournalVoucher", "GetJournalVoucherById", Id);

        getjournalVoucher.then(function (p1) {
            if (p1.data != null) {
                $scope.JournalVoucher = p1.data
                $("#txtJVDate").data("DateTimePicker").date($filter('date')($scope.JournalVoucher.JVDate, 'dd/MM/yyyy'));
                //$scope.JournalVoucher.FromHeadType = p1.data.FromHeadType + ""
                //GetToSubHead($scope.JournalVoucher.ToHeadType);
                //GetFromSubHead($scope.JournalVoucher.FromHeadType);
                //GetFromBranch(getUserdata.UserId);
                //GetToBranch(getUserdata.UserId);
                //$scope.JournalVoucher.ToHeadType = p1.data.ToHeadType + ""
                //$scope.JournalVoucher.FromAccountHead = p1.data.FromAccountHead + ""
                //$scope.JournalVoucher.ToAccountHead = p1.data.ToAccountHead + ""
                //$scope.JournalVoucher.FromBranchId = p1.data.FromBranchId + ""
                //$scope.JournalVoucher.ToBranchId = p1.data.ToBranchId + ""
                //$scope.JournalVoucher.TransactionMode = p1.data.TransactionMode

                $scope.Type = p1.data.Type + ""
                $scope.CustomerAccountNumber = p1.data.ToAccount;
                $scope.GetCustomerDataFromAccountNumber($scope.CustomerAccountNumber);
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    $scope.GetCustomerDataFromAccountNumber = function (acno) {
        var Promis = AppService.GetDetailsById("JournalVoucher", "GetCustomerDataByProductId", acno);
        Promis.then(function (p1) {

            if (p1.data.CustomerProductDetails.length > 0) {
                $scope.CustomerName = p1.data.CustomerProductDetails[0].CustomerName;
                $scope.CustomerId = p1.data.CustomerProductDetails[0].CustomerId;
                $scope.ProductType = p1.data.CustomerProductDetails[0].productType.replace('_', ' ')
            }
            else {
                $scope.CustomerName = '';
                $scope.CustomerId = '';
                $scope.ProductType = '';
                $scope.CustomerAccountNumber = '';
                showToastMsg(3, 'No Customer Exist with this Accountnumber. Please enter valid account number.');
            }
        });
    }

})