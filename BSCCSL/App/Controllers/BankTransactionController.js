angular.module("BSCCL").controller('BankTransactionController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = false;
    $scope.UserBranch.Enabled = false;
    $scope.UserBranch.BranchId = $cookies.get('Branch');
    $scope.show = 0;
    $scope.ShowBankMaster = false;
    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');

    if ($scope.BranchList.length > 1 && getUserdata.Role == "1") {
        $scope.ShowBankMaster = true;
    }

    $scope.today = new Date();

    $("#txtserchStartDate").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $("#txtserchEndDate,#txtChequeDate,#txtWithdrawCheckDate,#txtChequeCleanceDate").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $("#txtWithdrawTransactionTime,#txtTransactionTime,#txtinterAcTransactionTime").datetimepicker({
        format: 'DD/MM/YYYY hh:mm:ss a',
        useCurrent: true,
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

    $scope.TransactionType = "";
    $scope.WithdrawTransactionType = "";
    $scope.BankDetails = {}
    $scope.UpdateRDStatus = [];

    if ($location.search().BankId != undefined) {
        var acNo = atob($location.search().BankId);
        $scope.BankId = acNo;
        $location.search('BankId', null);
        GetBankDetails();
    } else {
        $state.go('App.BankMaster')
    }

    if ($scope.Role != 1) {
        $("#txtTransactionTime").attr("disabled", "disabled");
        $("#txtWithdrawTransactionTime").attr("disabled", "disabled");
    }

    $scope.RemoveErrorMessageinUncheck = function () {
        if (!$('#chksamebank').attr('checked')) {
            $('#txtAccNumber').closest('.form-group').removeClass('has-error');
            $('#txtAccNumber').next('.help-block').remove();
            $('#txtAccNumber').prev('.help-block').remove();
        }
    }

    $scope.ClearRecordOnCLose = function () {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        $('#txtChequeCleanceDate').val('');
    }

    function GetBankDetails() {
        var getholder = AppService.GetDetailsById("BankMaster", "GetBankData", $scope.BankId);
        getholder.then(function (p1) {
            if (p1.data != null) {
                $scope.BankDetails = p1.data;
                $scope.show = 1;
                GetTransactionList();
            }
            else {
                $scope.BankDetails = {}
                showToastMsg(3, "Error while retrieving data")
            }

        });
    }

    $scope.OpenDepositModal = function () {

        $('#Deposit').modal('show');
        $scope.DepositAmount = null;
        $scope.TransactionData = null;
        $scope.ClearForm();
        if ($scope.BankDetails.ProductType == 4) {
            $scope.PendingRDInstallments($scope.BankDetails.BankId);
        }
    }

    $scope.OpenWithdrawModal = function () {

        $('#Withdraw').modal('show');
        $scope.ClearForm();
        $scope.WithdrawAmount = null;
        $scope.TransactionData = null;
    }

    $scope.OpenInterAccTransferModal = function () {
        $('#InterAccountTransfer').modal('show');
        $scope.ClearForm();
        $scope.Amount = null;
        $scope.TransactionData = null;
        $scope.PendingInstallments = [];
    }

    $('#Deposit').on('shown.bs.modal', function () {
        $("#txtTransactionTime").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy hh:mm:ss a'));
    })

    $('#Withdraw').on('shown.bs.modal', function () {
        $("#txtWithdrawTransactionTime").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy hh:mm:ss a'));
    })

    $('#InterAccountTransfer').on('shown.bs.modal', function () {

        $("#txtinterAcTransactionTime").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy hh:mm:ss a'));
    })

    $scope.Deposit = function () {
        var flag = true;
        flag = ValidateDepositForm();

        if ($scope.BankDetails.AccountType == 4) {
            $scope.SelectInstallments();
        }

        if (flag) {

            $(':focus').blur();
            if ($scope.DepositAmount == 0 || $scope.DepositAmount == undefined) {
                return showToastMsg(3, 'Please Enter valid amount');
            }

            $scope.TransactionData = new Object();
            $scope.TransactionData.BranchId = $scope.UserBranch.BranchId;
            $scope.TransactionData.TransactionType = $scope.TransactionType;
            $scope.TransactionData.Balance = 0;
            $scope.TransactionData.BankId = $scope.BankDetails.BankId;
            if ($scope.TransactionType == 1 || $scope.TransactionType == 6) {
                $scope.TransactionData.Status = 0;
            }
            else {
                $scope.TransactionData.Status = 1;
            }
            $scope.TransactionData.Balance = parseFloat($scope.BankDetails.Balance) + parseFloat($scope.DepositAmount) + parseFloat($scope.BankDetails.UnclearBalance);
            $scope.TransactionData.Amount = $scope.DepositAmount;
            $scope.TransactionData.Type = 1;
            $scope.UnclearBalance = $scope.DepositAmount;
            var date = document.getElementById('txtChequeDate').value;
            if (date !== "") {
                $scope.TransactionData.ChequeDate = moment(new Date($("#txtChequeDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            else {
                $scope.TransactionData.ChequeDate = null;
            }
            $scope.TransactionData.CheckNumber = $scope.CheckNumber;
            $scope.TransactionData.CreatedBy = $cookies.getObject('User').UserId;
            $scope.TransactionData.BranchId = $cookies.get('Branch');

            $scope.TransactionData.BankName = $scope.BankName;
            var TransactionTime = moment(new Date($("#txtTransactionTime").data("DateTimePicker").date())).format('YYYY-MM-DD hh:mm:ss a');
            if ($scope.TransactionData.TransactionTime != "") {
                $scope.TransactionData.TransactionTime = TransactionTime;
            }
            else {
                $scope.TransactionData.TransactionTime = null
            }
            $scope.TransactionData.Description = $scope.Description;

            if ($scope.UpdateRDStatus.length > 0 && $scope.BankDetails.ProductType == 4) {
                var obj = new Object();
                obj.rdPaymentList = $scope.UpdateRDStatus;
                obj.transaction = $scope.TransactionData;
                RDManualPaymentofIsntallment(obj);
            }
            else {
                var saveTransaction = AppService.SaveData("BankMaster", "SaveBankTransaction", $scope.TransactionData);
                saveTransaction.then(function (p3) {
                    if (p3.data != null) {
                        showToastMsg(1, "Transaction Saved Successfully");
                        GetTransactionList();
                        GetBankDetails();
                        $('#Deposit').modal('hide');
                        $scope.DepositAmount = null;
                        $scope.TransactionData = null;
                        $scope.ClearForm();
                    }
                    else {
                        showToastMsg(3, 'Error in saving data');
                    }
                });
            }
        }
    }

    var WA = null;

    $scope.Withdraw = function () {
        var flag = true;
        flag = ValidateWithdrawForm();
        if (flag) {

            $(':focus').blur();
            if ($scope.WithdrawAmount == 0 || $scope.WithdrawAmount == undefined) {
                return showToastMsg(3, 'Please Enter valid amount');
            }

            $scope.TransactionData = new Object();
            $scope.TransactionData.BranchId = $scope.UserBranch.BranchId;
            $scope.TransactionData.Balance = 0;
            $scope.TransactionData.BankId = $scope.BankDetails.BankId;

            $scope.TransactionData.Status = 0;

            if ($scope.BankDetails.Balance >= parseFloat($scope.WithdrawAmount)) {
                $scope.TransactionData.Balance = $scope.BankDetails.Balance;
                $scope.TransactionData.ProductwiseBalance = (parseFloat($scope.BankDetails.Balance) + parseFloat($scope.BankDetails.UnclearBalance)) - parseFloat($scope.WithdrawAmount);
            }
            else {
                return showToastMsg(3, 'Cannot withdraw amount due to Insufficient balance');
            }

            $scope.TransactionData.Amount = $scope.WithdrawAmount;
            WA = $scope.WithdrawAmount;
            $scope.TransactionData.Type = 2;
            $scope.TransactionData.CreatedBy = $cookies.getObject('User').UserId;
            $scope.TransactionData.BranchId = $cookies.get('Branch');
            var date = document.getElementById('txtWithdrawCheckDate').value;
            if (date !== "") {
                $scope.TransactionData.ChequeDate = moment(new Date($("#txtWithdrawCheckDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            else {
                $scope.TransactionData.ChequeDate = null;
            }

            $scope.TransactionData.CheckNumber = $scope.WithdrawCheckNumber;
            $scope.TransactionData.TransactionType = $scope.WithdrawTransactionType;
            $scope.TransactionData.Description = $scope.Description;

            $scope.TransactionData.TransactionTime = moment(new Date($("#txtWithdrawTransactionTime").data("DateTimePicker").date())).format('YYYY-MM-DD hh:mm:ss a');
            var saveTransaction = AppService.SaveData("BankMaster", "SaveBankTransaction", $scope.TransactionData);
            saveTransaction.then(function (p3) {
                if (p3.data != null) {
                    showToastMsg(1, "Transaction Saved Successfully");
                    GetTransactionList();
                    GetBankDetails();
                    $('#Withdraw').modal('hide');
                    $scope.ClearForm();
                    $scope.WithdrawAmount = null;
                    $scope.TransactionData = null;
                }
                else {
                    showToastMsg(3, 'Error in saving data');
                }
            });
        }
    }

    $scope.ClearFormpenalty = function () {
        $('#ddlBounceReson').val("");
        $("#txtPenalty").val("");
    }

    $scope.ClearForm = function () {
        //$("#txtWithdrawTransactionTime").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy hh:mm a'));
        //$("#txtTransactionTime").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy hh:mm a'));
        $scope.DepositAmount = null;
        $scope.BankName = null;
        $scope.CheckNumber = null;
        $scope.TransactionType = "";
        $("#txtChequeDate").val("");
        $scope.WithdrawTransactionType = "";
        $scope.WithdrawAmount = null;
        $scope.WithdrawCheckNumber = null;
        $("#txtWithdrawCheckDate").val("");
        $('#ddlBounceReson').val("");
        $("#txtPenalty").val("");
        $("#txtChequeCleanceDate").val("");
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        $scope.Amount = null;
        $scope.Amount = null;
        $("#txtinterAcTransactionTime").val("");
        $scope.PendingInstallments = [];
    }


    function GetTransactionList() {
        var LastTransId = null;
        var CurrentDate = new Date();

        $('#tblBankTransaction').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            //"bLengthChange": false,
            "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
            "iDisplayLength": 25,
            "bSort": false,
            "bDestroy": true,
            dom: 'l<"floatRight"B>frtip',
            buttons: [
                 {
                     extend: 'excel',
                     footer: true
                 },
                  {
                      extend: 'pdf',
                      footer: true
                  },
                     {
                         extend: 'csv',
                         footer: true
                     }
            ],
            "sAjaxSource": urlpath + "/BankMaster/GetBankTransactionData",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "id", "value": $scope.BankDetails.BankId });
                aoData.push({ "name": "Type", "value": $scope.SearchType });
                aoData.push({ "name": "fromDate", "value": ddmmyyTommdddyy($("#txtserchStartDate").val()) });
                aoData.push({ "name": "toDate", "value": ddmmyyTommdddyy($("#txtserchEndDate").val()) });
                aoData.push({ "name": "BranchId", "value": $scope.ShowBankMaster == true ? $("#ddlsearchBranch").val() : $scope.UserBranch.BranchId });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        LastTransId = json.LastTransactionId;
                        CurrentDate = json.CurrentDate;
                        fnCallback(json);
                        IntialPageControlShare();
                    }
                });
            },
            "aoColumnDefs": [
              {
                  "bVisible": $scope.ShowBankMaster == true, "aTargets": [2]
              }
            ],
            "aoColumns": [
              {
                  "mDataProp": "TransactionTime",
                  "mRender": function (data, type, full) {
                      return $filter('date')(data, 'dd/MM/yyyy');
                  },
                  "sClass": "text-center"
              },
              {
                  "mDataProp": "Amount",
                  "mRender": function (data, type, full) {
                      return $filter('currency')(data, '', 2);
                  },
                  columnDefs: [
                           { type: 'formatted-num', targets: 0 }
                  ],
                  "sClass": "text-right"
              },
              {
                  "mDataProp": "Balance",
                  "mRender": function (data, type, full) {
                      return $filter('currency')(data, '', 2);
                  },
                  "sType": "numeric-comma",
                  "sClass": "text-right"
              },
              {
                  "mDataProp": "TypeName",
                  "sClass": "text-center"
              },
              {
                  "mDataProp": "Description",
                  "sClass": "text-left"
              },
              {
                  "mDataProp": "BankTransactionId",
                  "mRender": function (data, type, full) {
                      if (full.TransactionType !== 1 && full.TransactionType !== 0 && full.Status == 1) {
                          return '<button class="btn btn-success btn-xs btnClearCheck" Id="' + data + '" title="ClearCheque"><span class="glyphicon glyphicon-edit"></span> Clear Cheque </button>'
                      }
                      else {
                          if (LastTransId == data) {
                              if ($scope.Role != "1") {
                                  if (full.Status == 0 && full.TransactionTime.split('T')[0] == CurrentDate.split('T')[0]) {
                                      return '<button class="btn btn-danger btn-xs btnDeleteTransaction" Id="' + data + '" title="DeleteTransaction"><span class="glyphicon glyphicon-trash"></span> Delete </button>'
                                  }
                                  else {
                                      return '';
                                  }
                              }
                              else {
                                  if (full.Status == 0) {
                                      return '<button class="btn btn-danger btn-xs btnDeleteTransaction" Id="' + data + '" title="DeleteTransaction"><span class="glyphicon glyphicon-trash"></span> Delete </button>'
                                  }
                                  else {
                                      return '';
                                  }
                              }
                          }
                          else {
                              return '';
                          }
                      }
                  },
              }
            ]
        });
    }

    function IntialPageControlShare() {
        $(".btnClearCheck").click(function () {
            var ID = $(this).attr("Id");
            $scope.TrnsactuionclearID = ID;
            $("#ClearChequePopup").modal('show');
            $("#txtChequeCleanceDate").val('');

            gettransactionDetails()
            function gettransactionDetails() {
                var Promis = AppService.GetDetailsById("BankMaster", "gettransactionDetails", ID);
                Promis.then(function (p1) {
                    if (p1.data != null) {
                        $scope.TransactionData = p1.data.transactionData;
                        $scope.BankTransactionId = p1.data.BankTransactionId;
                        $scope.TransactionTypeName = p1.data.TransactionType;
                    }
                });
            }
        });

        $(".btnDeleteTransaction").click(function () {
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
                            var promiseDelete = AppService.DeleteData("BankMaster", "DeleteLastTransaction", ID);
                            promiseDelete.then(function (p1) {
                                if (p1.data != null) {
                                    $scope.BankDetails.Balance = p1.data.Balance;
                                    toastr.remove();
                                    showToastMsg(1, "Transaction Deleted Successfully");
                                    GetTransactionList();
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

    $scope.Clicked = false;

    $scope.ClearCheque = function () {
        var flag = true;
        flag = ValidateClearChequeForm();
        if (flag) {
            $scope.ClearBalance = new Object();
            $scope.ClearBalance.BankTransactionId = $scope.TrnsactuionclearID;
            $scope.ClearBalance.ChequeClearDate = moment(new Date($("#txtChequeCleanceDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            $scope.UnclearBalance = $scope.UnclearBalance - $scope.DepositAmount;
            $scope.ClearBalance.Type = 1;
            $scope.ClearBalance.Status = 0;
            $scope.ClearBalance.BranchId = $scope.UserBranch.BranchId;

            UpdateTransactionOnClear($scope.ClearBalance);
        }
    }

    function UpdateTransactionOnClear(ClearBalance) {
        var Newtransactionlist = AppService.SaveData("BankMaster", "UpdateTransactionData", ClearBalance)
        Newtransactionlist.then(function (p3) {
            if (p3.data != 0) {
                $scope.BankDetails.Balance = p3.data;
                GetTransactionList();
                GetBankDetails();
                $("#ClearChequePopup").modal('hide');
                showToastMsg(1, "Your Cheque has been cleared Successfully");
            }
            else {
                showToastMsg(3, "Sorry Can't clear cheque , Due to insufficient Amount");
            }
        });
    }

    $scope.BounceCheque = function () {
        var flag = true;
        falg = ValidateClearChequeForm();
        if (falg) {
            ID = $(this).attr("Id");
            $("#ClearChequePopup").modal('hide');
            $("#CheckBounce").modal('show');
            $scope.UnclearBalance = $scope.UnclearBalance - $scope.DepositAmount;
            $scope.Clicked = false
        }
    }

    $scope.SavePenalty = function () {
        var flag = ValidatePenaltyForm();
        if (flag) {
            $(':focus').blur();
            $scope.TransactionData = new Object();
            $scope.TransactionData.BranchId = $scope.UserBranch.BranchId;
            $scope.TransactionData.BounceReason = $('#ddlBounceReson').val();
            $scope.TransactionData.Penalty = $("#txtPenalty").val();
            $scope.TransactionData.BankTransactionId = $scope.TrnsactuionclearID;
            $scope.TransactionData.Type = 2;
            $scope.TransactionData.ChequeClearDate = moment(new Date($("#txtChequeCleanceDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            $scope.TransactionData.BankId = $scope.BankDetails.BankId;

            var Newtransactionlist = AppService.SaveData("BankMaster", "UpdateTransactionDataForBounce", $scope.TransactionData)
            Newtransactionlist.then(function (p3) {
                if (p3.data != null) {
                    $scope.BankDetails.Balance = p3.data;
                    $("#CheckBounce").modal('hide');
                    showToastMsg(3, 'Sorry, Your Cheque is bounced');
                    GetTransactionList();
                    GetBankDetails();
                    $scope.ClearForm();
                    $scope.AccBalance = 0.0;
                }
                else {
                    showToastMsg(3, 'Error in saving data');
                }
            });
        }
    }

    function ValidateClearChequeForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtChequeCleanceDate"), 'Please select cheque clear date', 'after')) {
            flag = false;
        }
        if ($scope.IsSameBranch == true) {
            if (!ValidateRequiredField($("#txtAccNumber"), 'Please enter Account number of holder of account', 'after')) {
                flag = false;
            }
        }
        return flag;
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

    $scope.SearchData = function () {
        $('#tblBankTransaction').dataTable().fnDraw();
    }

    $scope.SearchClearData = function () {
        $('#txtserchStartDate').val('');
        $('#txtserchEndDate').val('');
        $scope.SearchType = null;
        $("#ddlsearchBranch").val('');
        $('#tblBankTransaction').dataTable().fnDraw();
    }

    function ValidateWithdrawForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtWithdrawamount"), 'Withdraw Amount required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlWithdrawTransactionType"), 'Transaction type required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtWithdrawTransactionTime"), 'Enter Transaction Time', 'after')) {
            flag = false;
        }

        if ($scope.WithdrawTransactionType !== "1" && $scope.WithdrawTransactionType != 1 && $scope.WithdrawTransactionType !== "6" && $scope.WithdrawTransactionType != 6 && $scope.WithdrawTransactionType !== "" && $scope.WithdrawTransactionType !== undefined && $scope.WithdrawTransactionType !== null) {

            if (!ValidateRequiredField($("#txtWithdrawChequeNumber"), ' Cheque Number required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtWithdrawCheckDate"), 'Cheque Date required', 'after')) {
                flag = false;
            }
        }
        return flag;
    }

    function ValidateDepositForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtDepositamount"), ' Amount required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlTransactionType"), ' Transaction Type required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtTransactionTime"), 'Enter Transaction Time', 'after')) {
            flag = false;
        }
        if ($scope.TransactionType !== "1" && $scope.TransactionType !== '6' && $scope.TransactionType !== 'undefined' && $scope.TransactionType !== null) {
            if (!ValidateRequiredField($("#txtChequeDate"), 'Cheque Date required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtCheckNumber"), ' Cheque Number required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtBankName"), 'Bank Name required', 'after')) {
                flag = false;
            }
        }

        return flag;
    }

    $scope.Reset = function () {
        $scope.Accountno = '';
        $scope.BankDetails = {};
    }

    $scope.ClearRecordOnCLose = function () {

        $("#ClearChequePopup").modal('hide');
        $scope.IsSameBranch = false
        $scope.CustomerData = [];
        $scope.AccountNo = '';
        $("#txtChequeCleanceDate").val('');
        $scope.AccBalance = 0.0;
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $scope.ClearOnBounceClose = function () {

        $scope.AccBalance = 0.0;
        $scope.IsSameBranch = false
        $scope.CustomerData = [];
        $scope.AccountNo = '';
        $("#CheckBounce").modal('hide');
        // $("#DivCheckBounce").modal('hide');
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $scope.PendingRDInstallments = function (Id) {
        var getpendingInstallment = AppService.GetDetailsById("CustomerProduct", "PendingRDInstallments", Id)
        getpendingInstallment.then(function (p3) {
            if (p3.data != null) {
                $scope.PendingInstallments = p3.data;
            }
            else {
                showToastMsg(3, 'Error in saving data');
            }
        });
    }


    $scope.SelectInstallments = function () {
        var toalpay = 0;
        $scope.UpdateRDStatus = [];
        var midinstallment = 0
        angular.forEach($scope.PendingInstallments, function (value, index) {
            if (value.Check == true) {
                if (midinstallment > 0) {
                    $('#DisplayPendingInstallment').modal('hide');
                    showToastMsg(3, 'Installment selection should be in sequence.');
                }
                else {
                    midinstallment = 0
                    toalpay += (value.Amount + value.LatePaymentCharges);
                    $scope.UpdateRDStatus.push(value);
                }
            }
            else {
                midinstallment = midinstallment + 1;
            }
        })
        $('#DisplayPendingInstallment').modal('hide');
        $scope.Amount = toalpay;
        $scope.DepositAmount = toalpay;
    }

    function RDManualPaymentofIsntallment(obj) {
        var RDMaualTransaction = AppService.SaveData("BankMaster", "UpdateRDPaymentStatus", obj);
        RDMaualTransaction.then(function (p4) {
            if (p4.data != null) {
                $scope.BankDetails.Balance = p4.data;
                $scope.UpdateRDStatus = [];
                GetTransactionList();
                $('#Deposit').modal('hide');
                $scope.DepositAmount = null;
                $scope.TransactionData = null;
                $scope.ClearForm();
                $("#InterAccountTransfer").modal('hide');
                //$scope.PendingRDInstallments($scope.HolderData.CustomerProductId);
            }
            else {
                showToastMsg(3, 'Error in saving data');
            }
        });
    }



    function ValidateInterAccountTransfer() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#ddlToAccount"), 'Please Select Account', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtinterAcTransactionTime"), 'Transaction Time required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtAmount"), 'Amount Transaction Time', 'after')) {
            flag = false;
        }


        return flag;
    }

    $scope.SaveInterAccountTransfer = function () {
        var flag = ValidateInterAccountTransfer();
        $scope.SelectInstallments();
        if (flag) {
            $(':focus').blur();
            if ($scope.UpdateRDStatus.length > 0) {
                $scope.TransactionData = new Object();
                $scope.TransactionData.BranchId = $scope.UserBranch.BranchId;
                $scope.TransactionData.BankId = $scope.BankDetails.BankId
                $scope.TransactionData.Status = 0;
                if ($scope.BankDetails.Balance >= parseFloat($scope.Amount)) {
                    $scope.TransactionData.Balance = $scope.BankDetails.Balance;
                    $scope.TransactionData.ProductwiseBalance = (parseFloat($scope.BankDetails.Balance) + parseFloat($scope.BankDetails.UnclearBalance)) - parseFloat($scope.WithdrawAmount);
                }
                else {
                    return showToastMsg(3, 'Cannot transfer amount due to Insufficient balance');
                }

                $scope.TransactionData.Amount = $scope.Amount
                $scope.TransactionData.Type = 2;
                $scope.TransactionData.TransactionType = 4;
                $scope.TransactionData.CreatedBy = $cookies.getObject('User').UserId;
                $scope.TransactionData.BranchId = $cookies.get('Branch');
                $scope.TransactionData.TransactionTime = moment(new Date($("#txtinterAcTransactionTime").data("DateTimePicker").date())).format('YYYY-MM-DD hh:mm:ss a');

                var obj = new Object();
                obj.rdPaymentList = $scope.UpdateRDStatus;
                obj.transaction = $scope.TransactionData;

                RDManualPaymentofIsntallment(obj);
            }
        }
    }
})