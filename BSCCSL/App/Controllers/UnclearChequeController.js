angular.module("BSCCL").controller('UnclearChequeController', function ($scope, $state, $cookies, $filter, AppService, $http, $location, $rootScope) {


    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblcheque').dataTable().fnDraw();
    }

    $("#txtChequeCleanceDate").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $(document).ready(function () {
        $('#myModal').on('show.bs.modal', function (e) {
            var image = $(e.relatedTarget).attr('src');
            $(".img-responsive").attr("src", image);
        });
    });

    $("#myModal").on('hidden.bs.modal', function (event) {
        if ($('.modal:visible').length) //check if any modal is open
        {
            $('body').addClass('modal-open');//add class to body
        }
    });


    var getUserdata = new Object();
    $scope.TransactionData = {};

    getUserdata = $cookies.getObject('User');

    $scope.AccountNo = $("#txtAccNumber").val()
    $scope.SectionHide = true
    $scope.HolderData = {}

    $scope.Acnumber = '';

    function GetHolderDetail() {
        var getholder = AppService.GetDetailsById("Customer", "GetHolderData", $scope.AccountNo);
        getholder.then(function (p1) {
            if (p1.data.AccountDetail != null) {
                $scope.SectionHide = false
                $scope.HolderData = p1.data.AccountDetail;
                $scope.HolderData.details = p1.data.details
                if ($scope.HolderData.details.length > 0) {
                    $scope.show = 1;
                    GetUnclearCheckList();
                }
            }
            else {
                $scope.SectionHide = true
                showToastMsg(3, "No Account found. Please enter correct Account No for transaction details")
                GetUnclearCheckList();
            }
        });
    }

    GetUnclearCheckList();

    function GetUnclearCheckList() {

        $('#tblcheque').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/Transaction/GetUnclearChequeList",
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
                        IntialPageControlUnclearCheque();
                    }
                });
            },
            "aoColumns": [
                {
                    "mDataProp": "AccountNo",
                },
                {
                    "mDataProp": "Name"
                },
                {
                    "mDataProp": "ProductTypeName",

                },
                {
                    "mDataProp": "Balance",
                    "mRender": function (data, type, full) {
                        return $filter('currency')(full.Balance, '', 2);
                    },
                },
                {
                    "mDataProp": "TransactionDate",
                    "mRender": function (data, type, full) {
                        return $filter('date')(data, 'dd/MM/yyyy');
                    },
                },
                {
                    "mDataProp": "ChequeAmount",
                    "mRender": function (data, type, full) {
                        return $filter('currency')(full.ChequeAmount, '', 2);
                    },
                },
                {
                    "mDataProp": "ChequeNo",
                },
                {
                    "mDataProp": "ChequeDate",
                    "mRender": function (data, type, full) {
                        return $filter('date')(data, 'dd/MM/yyyy');
                    },
                },
                 {
                     "mDataProp": "BankName",
                 },
                {
                    "mDataProp": "TransactionId",
                    "mRender": function (data, type, full) {
                        return '<a class="btn btn-success btn-xs btnUnclearCheque" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span>Clear Cheque</a>';
                    },
                    "sclass": "text-center"
                }]
        });
    }

    function IntialPageControlUnclearCheque() {
        $(".btnUnclearCheque").click(function () {
            var ID = $(this).attr("Id");
            $scope.TransactionClearId = ID;
            $("#ClearChequePopup").modal('show');
            gettransactionDetails(ID)
        });
    }

    function gettransactionDetails(ID) {

        var Promis = AppService.GetDetailsById("Customer", "gettransactionDetails", ID);
        Promis.then(function (p1) {
            if (p1.data != null) {
                $scope.TransactionData = p1.data.transactionData;
                $scope.TransactionId = p1.data.TransactionId;
                $scope.TransactionType = p1.data.TransactionType;
            }
            else {
                showToastMsg(3, 'Error in Getting Data');
            }
        });
    }

    function ValidateClearChequeForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtChequeCleanceDate"), 'Please Select Cheque Clear Date', 'after')) {
            flag = false;
        }
        if ($scope.IsSameBranch == true) {
            if (!ValidateRequiredField($("#txtAccNumber"), 'Please Enter Account Number of Holder Account', 'after')) {
                flag = false;
            }
        }
        return flag;
    }

    $scope.SearchAccountNumber = function () {

        var flag = true;
        if ($scope.IsSameBranch == true) {
            if (!ValidateRequiredField($("#txtAccNumber"), 'Please enter Account number of holder of account', 'after')) {
                flag = false;
            }
        }

        if (flag) {
            GetCustomerDataByProductId();
        }

    }

    $scope.stateChanged = function () {
        if ($scope.IsSameBranch != true) { //If it is not checked
            $scope.CustomerData = "";
            $scope.AccountNo = "";
            $scope.AccBalance = "";
            if (!$('#chksamebank').attr('checked')) {
                $('#txtAccNumber').closest('.form-group').removeClass('has-error');
                $('#txtAccNumber').next('.help-block').remove();
                $('#txtAccNumber').prev('.help-block').remove();
            }

        }
    }

    function GetCustomerDataByProductId() {
        if ($scope.TransactionData.AccountNumber != $("#txtAccNumber").val()) {
            //$scope.TData = new Object()
            //$scope.TData.AccountNumber = $("#txtAccNumber").val()
            // $scope.TData.TransactionId = $scope.TrnsactuionclearID
            if ($("#txtAccNumber").val() != "" && $("#txtAccNumber").val() != undefined && $("#txtAccNumber").val() != null) {
                var Promis = AppService.GetDetailsById("Transaction", "GetCustomerDataByProductId", $("#txtAccNumber").val());
                Promis.then(function (p1) {

                    if (p1.data.CustomerProductDetails.length > 0) {
                        $scope.CustomerData = p1.data.CustomerProductDetails;
                        $scope.AccBalance = p1.data.Balance;

                        for (var i = 0; i < $scope.CustomerData.length ; i++) {
                            if ($scope.CustomerData[i].Holdersign == "" || $scope.CustomerData[i].Holdersign == undefined) {
                                //$scope.CustomerData[i].HolderPhoto = 'no_image.png';
                                $scope.CustomerData[i].Holdersign = 'no_image.png';
                            }
                        }
                    }
                    else {
                        $scope.CustomerData = [];
                        $scope.AccBalance = 0;
                        showToastMsg(3, 'No Customer Exist with this Accountnumber. Please search valid account number.');
                    }
                });
            }
        }
        else {
            showToastMsg(3, 'You Can Not Deposite Money by Cheque In your Own Account Enter Valid AccountNumber');
        }
    }

    $scope.ClearCheque = function () {

        var flag = true;
        flag = ValidateClearChequeForm();
        if (flag) {

            bootbox.dialog({
                message: "Are you sure want to clear this cheque?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            if ($scope.AccountNo == $("#txtAccNumber").val()) {
                                // $scope.ClearBalance = new Object();
                                $scope.TransactionData.BranchId = $scope.UserBranch.BranchId;
                                $scope.TransactionData.TransactionId = $scope.TransactionClearId;
                                $scope.TransactionData.ChequeClearDate = moment(new Date($("#txtChequeCleanceDate").data("DateTimePicker").date())).format('YYYY-MM-DD');

                                $scope.TransactionData.Type = 1
                                $scope.TransactionData.Status = 0;
                                if ($scope.IsSameBranch) {
                                    if ($("#txtAccNumber").val() != "" && $("#txtAccNumber").val() != undefined && $("#txtAccNumber").val() != null) {
                                        $scope.TransactionData.TempThirdPartyAccNo = $("#txtAccNumber").val()
                                    }
                                    else {
                                        $scope.TransactionData.TempThirdPartyAccNo = null
                                    }
                                }
                                UpdateTransactionOnClear($scope.TransactionData);
                            }
                            else {
                                showToastMsg(3, 'You Can Not Deposite Money by Cheque In your Own Account Enter Valid AccountNumber');
                            }
                        }
                    },
                    danger: {
                        label: "No",
                        className: "btn-danger btn-flat"
                    }
                }
            });
        }
    }

    function UpdateTransactionOnClear(ID) {

        var Newtransactionlist = AppService.SaveData("Transaction", "UpdateTransactionData", $scope.TransactionData)
        Newtransactionlist.then(function (p3) {
            if (p3.data != 0) {
                $scope.HolderData.Balance = p3.data;
                GetUnclearCheckList();
                //$scope.SearchAccount();
                $("#ClearChequePopup").modal('hide');
                showToastMsg(1, "Your Cheque has been cleared Successfully");
                RefreshDataTablefnDestroy();
                $scope.TransactionData.TempThirdPartyAccNo = ''
                $scope.IsSameBranch = false
                $("#txtAccNumber").val('');
                $scope.CustomerData = null
            }
            else {
                showToastMsg(3, "Sorry Can't clear cheque , Due to insufficient Amount");
            }
        });
    }

    $scope.SearchCheque = function () {

        $('#tblcheque').dataTable().fnDraw();
    }

    $scope.ClearChequeData = function () {
        $('#txtSearch').val('');
        $('#tblcheque').dataTable().fnDraw();
    }

    $scope.BounceCheque = function () {
        var flag = true;
        falg = ValidateClearChequeForm();
        if (falg) {
            ID = $(this).attr("Id");
            $("#ClearChequePopup").modal('hide');
            $("#CheckBounce").modal('show');
            $scope.UnclearBalance = $scope.UnclearBalance - $scope.DepositAmount;
            $("#txtAccNumber").val('');
            $scope.Clicked = false
        }
    }

    $scope.ClearRecordOnCLose = function () {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        $('#txtChequeCleanceDate').val('');

    }

    $scope.ClearOnBounceClose = function () {

        $scope.AccBalance = 0.0;
        $scope.IsSameBranch = false
        $scope.CustomerData = [];
        $scope.AccountNo = '';
        $("#CheckBounce").modal('hide');
        $("#txtChequeCleanceDate").val('');
        // $("#DivCheckBounce").modal('hide');
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $scope.SavePenalty = function () {

        var flag = ValidatePenaltyForm();
        if (flag) {


            bootbox.dialog({
                message: "Are you sure want to bounce this cheque?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            //$scope.TransactionData = new Object();
                            $scope.TransactionData.BranchId = $scope.UserBranch.BranchId;
                            $scope.TransactionData.BounceReason = $('#ddlBounceReson').val();
                            $scope.TransactionData.Penalty = $("#txtPenalty").val();
                            $scope.TransactionData.TransactionId = $scope.TransactionClearId;
                            $scope.TransactionData.Type = 2;
                            $scope.TransactionData.Status = 1;
                            $scope.TransactionData.ChequeClearDate = moment(new Date($("#txtChequeCleanceDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                            $scope.TransactionData.CustomerProductId = $scope.TransactionData.CustomerProductId

                            if ($scope.IsSameBranch == true) {
                                if ($scope.AccountNo != "" && $scope.AccountNo != '' && $scope.AccountNo != undefined && $scope.AccountNo != null) {
                                    $scope.TransactionData.TempThirdPartyAccNo = $scope.AccountNo
                                }
                            }
                            else {
                                $scope.TransactionData.TempThirdPartyAccNo = null
                            }
                            var Newtransactionlist = AppService.SaveData("Transaction", "UpdateTransactionDataForBounce", $scope.TransactionData)
                            Newtransactionlist.then(function (p3) {
                                if (p3.data != null) {
                                    $scope.HolderData.Balance = p3.data;
                                    RefreshDataTablefnDestroy();
                                    $("#CheckBounce").modal('hide');
                                    showToastMsg(3, 'Sorry, Your Cheque is bounced');
                                    $scope.ClearForm();
                                    $scope.IsSameBranch = false
                                    $("#txtAccNumber").val('');
                                    $scope.CustomerData = null
                                }
                                else {
                                    showToastMsg(3, 'Error in saving data');
                                }
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
    }

    function ValidatePenaltyForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#ddlBounceReson"), 'Please select Date of checque to be cleared', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtPenalty"), 'Please enter Account Number', 'after')) {
            flag = false;
        }
        return flag;
    }

    function RefreshDataTablefnDestroy() {
        $('#tblcheque').dataTable().fnDraw();
    }
});