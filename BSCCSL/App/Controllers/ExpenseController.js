angular.module("BSCCL").controller('ExpenseController', function ($scope, AppService, $state, $cookies, $filter, $rootScope) {
    $scope.UserBranch.ShowBranch = true;
    $scope.UserBranch.Enabled = true;

    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');


    $scope.Files = [];

    $(".datepicker").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });


    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblExpense').dataTable().fnDraw();
    }


    GetExpenseList();
    GetSubHead()
    $scope.AddExpense = function () {
        $scope.Expense = {}
        $("#divTransactionMode").hide();
        $("#txtExpenseDate").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy'));
        $scope.AccountsHead = {};
        $("#fileUploadBill").val(null);
        $("#FileName").hide();
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $('#Expense').modal({
        backdrop: 'static',
        show: false,
        keyboard: false  // to prevent closing with Esc button (if you want this too)
    });

    //$('#Expense').on('shown.bs.modal', function () {
    //    $("#txtExpenseDate").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy'));
    //})

    $rootScope.CountValue = 0;

    function GetSubHead() {
        var getheaddata = AppService.GetDetailsById("AccountsHead", "GetSubHead", 2);

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

    function ValidateExpenseForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#txtExpenseName"), 'Expense Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#dpdAccountHead"), 'Accounts Head required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtAmount"), 'Amount required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtExpenseDate"), 'Date required', 'after')) {
            flag = false;
        }
        //if (!ValidateRequiredField($("#txtBillDate"), 'Bill Date required', 'after')) {
        //    flag = false;
        //}

        //if ($scope.Expense.ApprovedAmount != null && $scope.Expense.ApprovedAmount > 0 && $scope.Expense.Amount > $scope.Expense.ApprovedAmount) {
        //    flag = false;
        //    $("#txtAmount").closest('.form-group').removeClass('has-error');
        //    $("#txtAmount").next('.help-block').remove();
        //    $("#txtAmount").prev('.help-block').remove();
        //    $("#txtAmount").closest('.form-group').addClass('has-error');
        //    $('<span class="help-block help-block-error"> Please enter amount less than approved amount.</span>').insertAfter($("#txtAmount"));
        //}

        if ($scope.IsApprove == 1) {

            if (!ValidateRequiredField($("#txtApproveAmount"), 'Approve amount required', 'after')) {
                flag = false;
            }

            //if (parseInt($("#txtApproveAmount").val()) == 0) {
            //    if (!ValidateRequiredField($("#txtApproveAmount"), 'Enter Valid Amount', 'after', $("#txtApproveAmount").val())) {
            //        flag = false;
            //    }
            //}

            if (!ValidateRequiredField($("#TransactionMode"), 'Transaction Mode is required', 'after')) {
                flag = false;
            }
            if ($("#TransactionMode").val() == "Cheque") {
                if (!ValidateRequiredField($("#txtReferenceNumber"), 'Reference Number is required', 'after')) {
                    flag = false;
                }

            }
        }

        if ($scope.Expense.ApprovedAmount != null && $scope.Expense.ApprovedAmount > 0 && $scope.Expense.ApprovedAmount > $scope.Expense.Amount) {
            flag = false;
            $("#txtApproveAmount").closest('.form-group').addClass('has-error');
            $('<span class="help-block help-block-error"> Please enter amount less than total amount.</span>').insertAfter($("#txtApproveAmount"));

        }


        return flag;
    }

    $scope.getTheFiles = function ($files) {

        angular.forEach($files, function (value, key) {
            var sizeInMB = (value.size / (1024 * 1024)).toFixed(2);
            if (sizeInMB < 10) {
                $scope.Files.push(value);
            }
            else {
                showToastMsg("Error", 3, "File size should be less than 10 MB.");
                return false;
            }

        });
    };

    $scope.SaveExpense = function () {
        var flag = true;
        flag = ValidateExpenseForm();
        if (flag) {

            $scope.Expense.BranchId = $scope.UserBranch.BranchId;
            $scope.Expense.ExpenseDate = moment(new Date($("#txtExpenseDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            $scope.Expense.BillDate = moment(new Date($("#txtBillDate").data("DateTimePicker").date())).format('YYYY-MM-DD');

            if ($scope.IsApprove != null) {
                bootbox.hideAll();

                bootbox.dialog({
                    message: "Are you sure want to Approve? Once you approve it will not be change.",
                    title: "Confirmation !",
                    size: 'small',
                    buttons: {
                        success: {
                            label: "Yes",
                            className: "btn-success btn-flat",
                            callback: function () {
                                AddExpense()
                            }
                        },
                        danger: {
                            label: "No",
                            className: "btn-danger btn-flat"
                        }
                    }
                });
            }
            else {
                AddExpense()
            }
        }
    }

    function AddExpense() {

        var fd = new FormData();
        angular.forEach($scope.Files, function (value, index) {
            fd.append("file", value);
        });

        fd.append("data", JSON.stringify($scope.Expense));

        var savehead = AppService.UploadDocumentwithData("Expense", "SaveExpense", fd)
        savehead.then(function (p1) {
            if (p1.data != false) {
                $('#Expense').modal('hide');
                showToastMsg(1, "Expense Saved Successfully")
                GetExpenseList()
            }
            else {
                showToastMsg(3, 'Error in saving data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in saving data')
        });
    }

    $scope.ClearForm = function () {
        $scope.Expense = {};
        $scope.IsApprove = null;
        $scope.Files = [];
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    function GetExpenseList() {

        $('#tblExpense').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            'bAutoWidth': false,
            //"scrollX": true,
            "sAjaxSource": urlpath + "/Expense/GetExpenseList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "fromDate", "value": ddmmyyTommdddyy($("#txtsearchStartDate").val()) });
                aoData.push({ "name": "toDate", "value": ddmmyyTommdddyy($("#txtsearchEndDate").val()) });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                        IntialPageControlExpense();
                    }
                });
            },
            "aoColumns": [{
                "sTitle": "Expense Name",
                "mDataProp": "ExpenseName",
            },
            //{
            //    "sTitle": "Description",
            //    "mDataProp": "Description",
            //},
            {
                "sTitle": "Amount",
                "mDataProp": "Amount",
            },
            {
                "sTitle": "HeadName",
                "mDataProp": "HeadName",
            },
            {
                "sTitle": "Expense Date",
                "mDataProp": "ExpenseDate",
                "mRender": function (data, type, full) {
                    return $filter('date')(data, 'dd/MM/yyyy');
                },
                "sClass": "text-center"
            },
            {
                "sTitle": "Requested By",
                "mDataProp": "RequestedBy",
            },
            {
                "sTitle": "Approved Amount",
                "mDataProp": "ApprovedAmount",
                "sClass": "text-right"
            },
            {
                "sTitle": "Approved By",
                "mDataProp": "ApprovedByName",
            },
            {
                "sTitle": "Approved Date",
                "mDataProp": "ApprovedDate",
                "mRender": function (data, type, full) {
                    return $filter('date')(data, 'dd/MM/yyyy');
                },
                "sClass": "text-center"
            },
            {
                "sTitle": "Paid Date",
                "mDataProp": "ModifiedDate",
                "mRender": function (data, type, full) {
                    if (full.IsPaid == 1) {
                        return $filter('date')(data, 'dd/MM/yyyy');
                    }
                    else {
                        return '';
                    }
                },
                "sClass": "text-center"
            },

            {
                "sTitle": "Status",
                "mDataProp": "Status",
                "mRender": function (data, type, full) {
                    if (data == 1 && full.IsPaid == 1) {
                        return '<span class="label label-info">Paid</span>';
                    }
                    else if (data == 0) {
                        return '<span class="label label-danger">Rejected</span>';
                    }
                    else if (data == 1) {
                        return '<span class="label label-success">Approved</span>';
                    }
                    else {
                        return ''
                    }


                },
            },
            {
                "sTitle": "Action",
                "mDataProp": "ExpenseId",
                "mRender": function (data, type, full) {

                    var str = ''
                    //if (getUserdata.Role == 1 && full.Status == null) {
                    //    str = '<button class="btn btn-info btn-xs btn-flat btnApprove" Id="' + data + '" title="Approve"><span class="glyphicon glyphicon-ok"></span> Approve</button>  <button class="btn btn-warning btn-xs btn-flat btnReject" Id="' + data + '" title="Reject"><span class="glyphicon glyphicon-remove-circle"></span> Reject </button>'
                    //}
                    //if (full.Status == 1 && full.IsPaid == null) {
                    //    str += '<button class="btn btn-info btn-xs btn-flat btnPaid" Id="' + data + '" title="Paid"><span class="glyphicon glyphicon-ok"></span> Paid</button>'
                    //}

                    //if (full.FileName != null) {
                    //    str += '<a class="btn btn-default btn-xs btn-flat btnDownload" href="../ExpenseDocuments/' + full.FileName + '" download="' + full.OrgFileName + '" title="Download" target="_blank"><span class="glyphicon glyphicon-download"></span> Download</a>'
                    //}

                    //str += '  <button class="btn btn-success btn-xs btn-flat btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>  <button class="btn btn-danger btn-xs btn-flat btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> '

                    str += '<div class="dropdown">';
                    str += '<button class="btn btn-primary dropdown-toggle" type="button" data-toggle="dropdown">Actions<span class="caret" ></span ></button >';
                    str += '<ul class="dropdown-menu">';


                    if (getUserdata.Role == 1 && full.Status == null) {
                        str += '<li><a class="btnApprove" href="#" Id="' + data + '" title="Approve"><span class="glyphicon glyphicon-ok"></span>Approve</a></li> <li><a class="btnReject" href="#" Id="' + data + '" title="Reject"><span class="glyphicon glyphicon-remove-circle"></span> Reject </a></li>'
                    }

                    if (full.Status == 1 && full.IsPaid == null) {
                        str += '<li><a class="btnPaid" href="#" Id="' + data + '" title="Paid"><span class="glyphicon glyphicon-ok"></span> Paid</a></li>'
                    }

                    if (full.FileName != null) {
                        str += '<li><a class="btnDownload" href="../ExpenseDocuments/' + full.FileName + '" download="' + full.OrgFileName + '" title="Download" target="_blank"><span class="glyphicon glyphicon-download"></span> Download</a></li>'
                    }

                    str += '<li><a class="btnEdit" href="#" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</a></li>  <li><a class="btnDelete" Id="' + data + '" href="#" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</a></li>'


                    str += '</ul>';
                    str += '</button>';

                    return str;
                },
                "sClass": "text-center",
                "sWidth": "150px"
            }]

        });
    }
    $scope.SearchData = function () {
        $('#tblExpense').dataTable().fnDraw();
    }
    $scope.SearchClearData = function () {
        $('#txtSearch').val('');
        $('#txtsearchStartDate').val('');
        $('#txtsearchEndDate').val('');
        $('#tblExpense').dataTable().fnDraw();
    }

    function IntialPageControlExpense() {


        $(".btnEdit").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $("#Expense").modal('show');
            $scope.Files = [];
            GetExpenseDataById(ID)
        });

        $(".btnApprove").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $scope.IsApprove = 1;
            $scope.Files = [];
            $("#Expense").modal('show');
            GetExpenseDataById(ID)
        });

        $(".btnReject").click(function () {
            var ID = $(this).attr("Id");
            GetExpenseDataById(ID)
            bootbox.hideAll();
            bootbox.dialog({
                message: "Are you sure want to Reject?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            $scope.Expense.Status = 0
                            AddExpense();
                        }
                    },
                    danger: {
                        label: "No",
                        className: "btn-danger btn-flat"
                    }
                }
            });

        });

        $(".btnDelete").click(function () {
            var ID = $(this).attr("Id");
            bootbox.hideAll();

            bootbox.dialog({
                message: "Are you sure want to delete?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            var promiseDelete = AppService.DeleteData("Expense", "DeleteExpenseById", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {
                                    toastr.remove();
                                    showToastMsg(1, "Expense Deleted Successfully");
                                    $("#tblExpense").dataTable().fnDraw();
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

        $(".btnPaid").click(function () {
            $scope.CustomerId = null;
            $scope.CustomerName = '';
            $scope.CustomerAccountNumber = '';
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $("#ExpensePaid").modal('show');
            GetExpenseDataById(ID)
        });

    }

    function GetExpenseDataById(Id) {
        var getexpense = AppService.GetDetailsById("Expense", "GetExpenseById", Id);

        getexpense.then(function (p1) {
            if (p1.data != null) {
                $("#fileUploadBill").val(null);
                if (p1.data.OrgFileName != null) {
                    $("#FileName").show();
                    $("#FileName").text(p1.data.OrgFileName);
                }
                else {
                    $("#FileName").hide();
                    $("#FileName").text('');
                }
                if (p1.data.Status == 1) {
                    $scope.IsApprove = 0;
                }
                $scope.Expense = p1.data
                $scope.Expense.AccountsHeadId = $scope.Expense.AccountsHeadId + ""
                $("#txtExpenseDate").data("DateTimePicker").date($filter('date')($scope.Expense.ExpenseDate, 'dd/MM/yyyy'));
                $("#txtBillDate").data("DateTimePicker").date($filter('date')($scope.Expense.BillDate, 'dd/MM/yyyy'));
                if ($scope.IsApprove) {
                    $scope.Expense.Status = 1
                    $scope.Expense.ApprovedAmount = $scope.Expense.Amount;
                }
                if ($scope.Expense.Status) {
                    $("#divTransactionMode").show();
                    $("#lblTransactionModeData").text($scope.Expense.TransactionMode);
                    $("#lblTransactionModeData").val($scope.Expense.TransactionMode);
                }
                else {
                    $("#divTransactionMode").hide();
                }
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function RefreshDataTablefnDestroy() {
        $("#tblExpense").dataTable().fnDestroy();
    }


    $scope.GetCustomerDataFromAccountNumber = function (acno) {

        var Promis = AppService.GetDetailsById("Expense", "GetCustomerDataByProductId", acno);
        Promis.then(function (p1) {

            if (p1.data.CustomerProductDetails.length > 0) {
                $scope.CustomerName = p1.data.CustomerProductDetails[0].CustomerName;
                $scope.CustomerId = p1.data.CustomerProductDetails[0].CustomerId;

            }
            else {
                $scope.CustomerName = '';
                $scope.CustomerAccountNumber = '';
                $scope.CustomerId = '';
                showToastMsg(3, 'No Customer Exist with this Accountnumber. Please enter valid account number.');
            }
        });
    }


    $scope.PaidExpense = function () {
        $scope.Expense.PaidTo = $scope.CustomerId;
        bootbox.hideAll();

        bootbox.dialog({
            message: "Are you sure want to Paid Expense Amount?",
            title: "Confirmation !",
            size: 'small',
            buttons: {
                success: {
                    label: "Yes",
                    className: "btn-success btn-flat",
                    callback: function () {
                        var promiseDelete = AppService.SaveData("Expense", "PaidExpense", $scope.Expense);
                        promiseDelete.then(function (p1) {
                            var status = p1.data;
                            if (status == true) {
                                toastr.remove();
                                showToastMsg(1, "Expense Paid Successfully");
                                $("#ExpensePaid").modal('hide');
                                $("#tblExpense").dataTable().fnDraw();
                            }
                            else {
                                showToastMsg(3, "Error Occured While Saving data");
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

    }

})