angular.module("BSCCL").controller('DeleteTransactionsController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {


    $scope.UserBranch.ShowBranch = false;
    $scope.UserBranch.Enabled = false;
    $scope.UserBranch.BranchId = $cookies.get('Branch');


    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');
    $scope.Role = getUserdata.Role;

    $scope.parseFloat = parseFloat;

    $scope.today = new Date();

    $("#txtserchStartDate").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $("#txtserchEndDate,#txtChequeDate,#txtWithdrawCheckDate,#txtChequeCleanceDate,#txtNEFTDate,#txtWithdrawNEFTDate").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    // $('.modal').modal({ backdrop: 'static', keyboard: false });


    //$("#txtWithdrawTransactionTime,#txtTransactionTime,#txtinterAcTransactionTime,#txtBankAcTransactionTime,#txtLoanPrePaymentTransactionTime").datetimepicker({
    //    format: 'DD/MM/YYYY hh:mm:ss a',
    //    useCurrent: true,
    //});

    //$('#Withdraw').modal({
    //    //backdrop: 'static',
    //    show: false,
    //    keyboard: false
    //});

    //$('#Deposit').modal({
    //    //backdrop: 'static',
    //    show: false,
    //    keyboard: false
    //});

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
    $scope.HolderData = {}
    $scope.UpdateRDStatus = [];

    if ($location.search().AccountNo != undefined) {
        var acNo = atob($location.search().AccountNo);
        $scope.Accountno = acNo;
        $location.search('AccountNo', null);
        GetHolderDetail();
    }

    //if ($scope.Role != 1) {
    //    $("#txtTransactionTime").attr("disabled", "disabled");
    //    $("#txtWithdrawTransactionTime").attr("disabled", "disabled");
    //}

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

    function GetHolderDetail() {

        var getholder = AppService.GetDetailsById("Customer", "GetHolderData", $scope.Accountno);
        getholder.then(function (p1) {
            if (p1.data.AccountDetail != null) {
                $scope.HolderData = p1.data.AccountDetail;
                $scope.HolderData.details = p1.data.details;
                $scope.HolderData.TotalLoanPendingAmount = p1.data.TotalLoanPendingAmount;

                if ($scope.HolderData.ProductType == 1) {
                    var customerRDList = AppService.GetDetailsById("CustomerProduct", "GetCustomerRDAccount", $scope.HolderData.CustomerId);
                    customerRDList.then(function (p2) {
                        if (p2.data != null) {
                            $scope.RDList = p2.data;
                        }
                        else {
                            showToastMsg(3, 'Error in saving data');
                        }
                    });
                }

                if ($scope.HolderData.details.length > 0) {
                    $scope.show = 1;

                    for (i = 0; i < $scope.HolderData.details.length; i++) {
                        if ($scope.HolderData.details[i].HolderPhoto == undefined || $scope.HolderData.details[i].HolderPhoto == "") {
                            $scope.HolderData.details[i].HolderPhoto = 'no_image.png';

                        }
                        if ($scope.HolderData.details[i].Holdersign == undefined || $scope.HolderData.details[i].Holdersign == "") {
                            $scope.HolderData.details[i].Holdersign = 'no_image.png';
                        }
                    }
                    GetTransactionList();
                }
                else {
                    $scope.HolderData = {}
                    showToastMsg(3, "No Account found. Please enter correct Account No or Saving Account for transaction details")
                }
            }
            else {
                $scope.HolderData = {}
                showToastMsg(3, "No Account found. Please enter correct Account No or Saving Account for transaction details")
            }

        });
    }

    $scope.CreateAutoFD = function () {
        if ($scope.HolderData.Balance > 2500) {
            var FDAmount = $scope.HolderData.Balance - 2500;
            var SavingCustomerProductId = $scope.HolderData.Id;
          
            bootbox.dialog({
                message: "Are you sure want to create FD of " + FDAmount.toFixed(2) + " Rs ?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            $scope.CustomerProduct = {}
                            $scope.CustomerProduct.CustomerId = $scope.HolderData.CustomerId;
                            $scope.CustomerProduct.CreatedBy = $cookies.getObject('User').UserId;
                            $scope.CustomerProduct.CustomerProductId = '0000000-0000-0000-0000-000000000000'
                            $scope.CustomerProduct.BranchId = $scope.UserBranch.BranchId;
                            $scope.CustomerProduct.CreatedBy = $cookies.getObject('User').UserId;
                            $scope.CustomerProduct.ProductType = 3;
                            $scope.CustomerProduct.Status = 1;
                            $scope.CustomerProduct.OpeningDate = moment(new Date()).format('YYYY-MM-DD');
                            $scope.CustomerProduct.InterestRate = 6.25;
                            $scope.CustomerProduct.InterestType = 3;
                            $scope.CustomerProduct.IsAutoFD = true;
                            var OpeningDate = $scope.CustomerProduct.OpeningDate;
                            var date = new Date(OpeningDate).setFullYear(new Date(OpeningDate).getFullYear() + parseInt("8"));
                            if (date !== NaN) {
                                $scope.CustomerProduct.MaturityDate = moment(new Date(date)).format('YYYY-MM-DD');
                            }



                            $scope.CustomerProduct.LIType = 0;
                            $scope.CustomerProduct.Amount = FDAmount.toFixed(2);
                            $scope.CustomerProduct.NoOfMonthsORYears = 8;
                            $scope.CustomerProduct.TimePeriod = 2;

                            var obj = new Object()
                            obj.ProductType = $scope.CustomerProduct.ProductType;
                            obj.OpeningDate = moment(new Date()).format('YYYY-MM-DD');

                            obj.MaturityDate = $scope.CustomerProduct.MaturityDate

                            //obj.InterestType = $filter('filter')($scope.Productlist, { ProductId: $scope.CustomerProduct.ProductId })[0].Frequency;
                            obj.InterestRate = $scope.CustomerProduct.InterestRate;
                            obj.InterestType = $scope.CustomerProduct.InterestType;
                            obj.DueDate = null;
                            obj.PaymentType = 0;
                            obj.Amount = $scope.CustomerProduct.Amount;
                            obj.TimePeriod = $scope.CustomerProduct.TimePeriod;
                            obj.NoOfMonthsORYears = $scope.CustomerProduct.NoOfMonthsORYears;
                            obj.OpeningBalance = $scope.CustomerProduct.OpeningBalance;
                            obj.SkipFirstInstallment = $scope.CustomerProduct.SkipFirstInstallment;

                            var calculateMaturityAmount = AppService.SaveData("CustomerProduct", "CalculateMaturityAmount", obj)
                            calculateMaturityAmount.then(function (p1) {
                                if (p1.data != null) {
                                    $scope.CustomerProduct.MaturityAmount = p1.data;
                                }
                                else {
                                    showToastMsg(3, 'Error in Saving Data')
                                }
                            }, function (err) {
                                showToastMsg(3, 'Error while calculation maturity amount.')
                            });


                            var customerproduct = AppService.SaveData("CustomerProduct", "SaveCustomerProductData", $scope.CustomerProduct)
                            customerproduct.then(function (p1) {
                                if (p1.data != null) {
                                    $scope.CustomerProduct = p1.data
                                    showToastMsg(1, "Product Saved Successfully")
                                }
                                else {
                                    showToastMsg(3, 'Error in Saving Data')
                                }
                            }, function (err) {
                                showToastMsg(3, 'Error in saving data')
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
        else {
            showToastMsg(3, 'You can not create FD. The account balance should be greater than 2500.');
        }

    }


    $scope.OpenDepositModal = function () {
        $('#Deposit').modal('show');
        $scope.DepositAmount = null;
        $scope.TransactionData = null;
        $scope.ClearForm();
        //if ($scope.HolderData.ProductType == 4) {
        //    $scope.PendingRDInstallments($scope.HolderData.CustomerProductId);
        //}
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
        $scope.RefCustomerProductId = null;
        $scope.TransactionData = null;
        $scope.PendingInstallments = [];
    }

    $scope.OpenBankTransfer = function () {
        $('#BankTransfer').modal('show');
        $scope.BankTransfer = {};
        $scope.BankTransfer.Accountno = $scope.Accountno;

        //$scope.BankTransfer.Accountno = $scope.Accountno;
        //$scope.Amount = null;
        //$scope.RefCustomerProductId = null;
        //$scope.TransactionData = null;
        //$scope.PendingInstallments = [];
    }

    $scope.OpenLoanPrePayment = function () {
        $('#LoanPrepayment').modal('show');
        $scope.PrePayment = {};
        $scope.Totalremaininginteresttilldate = 0;
        $scope.TotalBalancetillDate = 0;
        //$scope.Amount = null;
        //$scope.RefCustomerProductId = null;
        //$scope.TransactionData = null;
        //$scope.PendingInstallments = [];
    }

    //$('#Deposit').on('shown.bs.modal', function () {
    //    $("#txtTransactionTime").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy hh:mm:ss a'));
    //})

    //$('#Withdraw').on('shown.bs.modal', function () {
    //    $("#txtWithdrawTransactionTime").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy hh:mm:ss a'));
    //})

    //$('#InterAccountTransfer').on('shown.bs.modal', function () {
    //    $("#txtinterAcTransactionTime").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy hh:mm:ss a'));
    //})

    //$('#BankTransfer').on('shown.bs.modal', function () {
    //    $("#txtBankAcTransactionTime").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy hh:mm:ss a'));
    //})

    //$('#LoanPrepayment').on('shown.bs.modal', function () {
    //    $("#txtLoanPrePaymentTransactionTime").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy hh:mm:ss a'));
    //})

    $scope.Deposit = function () {
        var flag = true;
        flag = ValidateDepositForm();

        if ($scope.HolderData.ProductType == 4) {
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
            $scope.TransactionData.CustomerProductId = $scope.HolderData.CustomerProductId
            if ($scope.TransactionType == 1 || $scope.TransactionType == 5 || $scope.TransactionType == 7) {
                $scope.TransactionData.Status = 0;
            }
            else {
                $scope.TransactionData.Status = 1;
            }
            //$scope.TransactionData.Balance = parseFloat($scope.HolderData.Balance) + parseFloat($scope.DepositAmount) + parseFloat($scope.HolderData.UnclearBalance);
            $scope.TransactionData.CustomerId = $scope.HolderData.CustomerId;
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

            var NEFTdate = document.getElementById('txtNEFTDate').value;
            if (NEFTdate !== "") {
                $scope.TransactionData.NEFTDate = moment(new Date($("#txtNEFTDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            else {
                $scope.TransactionData.NEFTDate = null;
            }
            $scope.TransactionData.NEFTNumber = $scope.NEFTNumber;
            if ($scope.TransactionType != "7") {
                $scope.TransactionData.NEFTNumber = null;
                $scope.TransactionData.NEFTDate = null;
            }


            $scope.TransactionData.CreatedBy = $cookies.getObject('User').UserId;
            $scope.TransactionData.BranchId = $cookies.get('Branch');

            $scope.TransactionData.BankName = $scope.BankName;
            $scope.TransactionData.Description = $scope.Description;
            //var TransactionTime = moment(new Date($("#txtTransactionTime").data("DateTimePicker").date())).format('YYYY-MM-DD hh:mm:ss a');
            //if ($scope.TransactionData.TransactionTime != "") {
            //    $scope.TransactionData.TransactionTime = TransactionTime;
            //}
            //else {
            //    $scope.TransactionData.TransactionTime = null
            //}

            if ($scope.UpdateRDStatus.length > 0 && $scope.HolderData.ProductType == 4) {
                var obj = new Object();
                obj.rdPaymentList = $scope.UpdateRDStatus;
                obj.transaction = $scope.TransactionData;
                RDManualPaymentofIsntallment(obj);
            }
            else {
                var saveTransaction = AppService.SaveData("Transaction", "SaveTransaction", $scope.TransactionData);
                saveTransaction.then(function (p3) {
                    if (p3.data != null) {
                        showToastMsg(1, "Transaction Saved Successfully");
                        GetTransactionList();
                        $scope.SearchAccount();
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
            $scope.TransactionData.CustomerProductId = $scope.HolderData.CustomerProductId

            $scope.TransactionData.Status = 0;

            if ($scope.HolderData.Balance >= parseFloat($scope.WithdrawAmount)) {
                $scope.TransactionData.Balance = $scope.HolderData.Balance;
                $scope.TransactionData.ProductwiseBalance = (parseFloat($scope.HolderData.Balance) + parseFloat($scope.HolderData.UnclearBalance)) - parseFloat($scope.WithdrawAmount);
            }
            else {
                return showToastMsg(3, 'Cannot withdraw amount due to Insufficient balance');
            }
            if ($scope.HolderData.ProductCode == "SA04" || $scope.HolderData.ProductName == "Smart Saving Plus") {
                var MinumumBalance = $scope.HolderData.Balance - parseFloat($scope.WithdrawAmount);
                if (MinumumBalance < 2500) {
                    return showToastMsg(3, 'You can not withdraw this Amount. The minimun balance should be 2500.');
                }
            }


            $scope.TransactionData.CustomerId = $scope.HolderData.CustomerId;
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

            var NEFTdate = document.getElementById('txtWithdrawNEFTDate').value;
            if (NEFTdate !== "") {
                $scope.TransactionData.NEFTDate = moment(new Date($("#txtWithdrawNEFTDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            else {
                $scope.TransactionData.NEFTDate = null;
            }

            $scope.TransactionData.NEFTNumber = $scope.WithdrawNEFTNumber;
            if ($scope.WithdrawTransactionType != "7") {
                $scope.WithdrawNEFTNumber = null;
                $scope.WithdrawNEFTDate = null;
            }

            $scope.TransactionData.TransactionType = $scope.WithdrawTransactionType;

            $scope.TransactionData.Description = $scope.Description;

            // $scope.TransactionData.TransactionTime = moment(new Date($("#txtWithdrawTransactionTime").data("DateTimePicker").date())).format('YYYY-MM-DD hh:mm:ss a');
            var saveTransaction = AppService.SaveData("Transaction", "SaveTransaction", $scope.TransactionData);
            saveTransaction.then(function (p3) {
                if (p3.data) {
                    showToastMsg(1, "Transaction Saved Successfully");
                    GetTransactionList();
                    $scope.SearchAccount();
                    $('#Withdraw').modal('hide');
                    $scope.ClearForm();
                    $scope.WithdrawAmount = null;
                    $scope.TransactionData = null;
                }
                else {
                    showToastMsg(3, "You have Insufficient balance or error occurred while doing transaction.");
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
        $scope.RefCustomerProductId = null;
        $scope.Amount = null;
        $scope.Amount = null;
        $scope.Description = null;
        //$("#txtinterAcTransactionTime").val("");
        $scope.PendingInstallments = [];
        $("#txtWithdrawNEFTDate").val("");
        $("#txtWithdrawNEFTNumber").val("");
        $("#txtNEFTDate").val("");
        $("#txtNEFTNumber").val("");

    }

    function GetTransactionList() {
        var LastTransId = null;
        var CurrentDate = new Date();
        $('#tblTransaction').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            //"bLengthChange": false,
            "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
            "iDisplayLength": 25,
            "bSort": false,
            "bDestroy": true,
            //dom: 'l<"floatRight"B>frtip',
            //buttons: [
            //     {
            //         extend: 'excel',
            //         footer: true
            //     },
            //      {
            //          extend: 'pdf',
            //          footer: true
            //      },
            //         {
            //             extend: 'csv',
            //             footer: true
            //         }
            //],
            "sAjaxSource": urlpath + "/Transaction/GetTransactionData",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "id", "value": $scope.HolderData.CustomerProductId });
                aoData.push({ "name": "Type", "value": $scope.SearchType });
                aoData.push({ "name": "fromDate", "value": ddmmyyTommdddyy($("#txtserchStartDate").val()) });
                aoData.push({ "name": "toDate", "value": ddmmyyTommdddyy($("#txtserchEndDate").val()) });
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
                        if (full.Type == 2) {
                            return $filter('currency')(data, '', 2);
                        } else {
                            return $filter('currency')(0.00, '', 2);
                        }
                    },
                    columnDefs: [
                        { type: 'formatted-num', targets: 0 }
                    ],
                    "sClass": "text-right"
                },
                {
                    "mDataProp": "Amount",
                    "mRender": function (data, type, full) {
                        if (full.Type == 1) {
                            return $filter('currency')(data, '', 2);
                        } else {
                            return $filter('currency')(0.00, '', 2);
                        }
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
                    "mDataProp": "Balance",
                    "mRender": function (data, type, full) {
                        if (full.Balance >= 0) {
                            return "CR";
                        } else {
                            return "DR";
                        }
                    },
                    "sClass": "text-center"
                },
                //{
                //    "mDataProp": "TypeName",
                //    "sClass": "text-center"
                //},
                {
                    "mDataProp": "Description",
                    "sClass": "text-left"
                },
                {
                    "mDataProp": "TransactionId",
                    "mRender": function (data, type, full) {
                        if (full.Status == 0) {
                            return '<button class="btn btn-danger btn-xs btnDeleteTransaction" Id="' + data + '" title="DeleteTransaction"><span class="glyphicon glyphicon-trash"></span> Delete </button>'
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
                var Promis = AppService.GetDetailsById("Customer", "gettransactionDetails", ID);
                Promis.then(function (p1) {
                    if (p1.data != null) {
                        $scope.TransactionData = p1.data.transactionData;
                        $scope.TransactionId = p1.data.TransactionId;
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
                            var promiseDelete = AppService.DeleteData("Transaction", "DeleteLastTransaction", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (p1.data != null) {
                                    $scope.HolderData.Balance = p1.data.Balance;
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

    $scope.SearchAccountNumber = function () {

        $scope.Clicked = true;
        var flag = true;
        if ($scope.IsSameBranch == true) {
            if (!ValidateRequiredField($("#txtAccNumber"), 'Please enter Account number of holder of account', 'after')) {
                flag = false;
            }
        }
        if (flag) {
            $scope.GetCustomerDataFromAccountNumber($scope.SameBranchAccountNo);
        }
    }

    //function GetCustomerDataByProductId() {

    //if ($location.search().AccountNo != undefined) {
    //    $scope.Accountno = $location.search().AccountNo;
    //}
    //if ($scope.Accountno != $("#txtAccNumber").val()) {

    //    if ($("#txtAccNumber").val() != "" && $("#txtAccNumber").val() != undefined && $("#txtAccNumber").val() != null) {
    //        GetCustomerDataFromAccountNumber($("#txtAccNumber").val())
    //    }
    //}
    //else {
    //    showToastMsg(3, 'You Can Not Deposite Money by Cheque In your Own Account Enter Valid AccountNumber');
    //}
    //}

    $scope.GetCustomerDataFromAccountNumber = function (acno) {
        if ($scope.Accountno != acno) {

            var Promis = AppService.GetDetailsById("Transaction", "GetCustomerDataByProductId", acno);
            Promis.then(function (p1) {

                if (p1.data.CustomerProductDetails.length > 0) {
                    $scope.CustomerData = p1.data.CustomerProductDetails;
                    $scope.AccBalance = p1.data.Balance;

                    //$scope.TotalLoanPendingAmount = p1.data.TotalLoanPendingAmount;
                    // $scope.ProductType = p1.data.ProductType;

                    for (var i = 0; i < $scope.CustomerData.length; i++) {
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
        else {
            $scope.CustomerData = [];
            $scope.AccBalance = 0;
            showToastMsg(3, 'You Can Not Deposit / withdrawal in your own accountnumber');
        }

    }

    $scope.SearchAccount = function () {

        if ($scope.Accountno != undefined && $scope.Accountno != "") {
            GetHolderDetail()
        }
        else {
            showToastMsg(3, 'Account No is Required.');
            $scope.show = 0;
            $('#txtserchStartDate').val('');
            $('#txtserchEndDate').val('');
            $scope.SearchType = null;
            $scope.HolderData = null;
            $scope.DepositAmount = null;
            $scope.WithdrawAmount = null;
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            $scope.TransactionData = null;
        }
    }

    $scope.ClearCheque = function () {

        var flag = true;
        flag = ValidateClearChequeForm();
        if (flag) {
            $(':focus').blur();
            bootbox.dialog({
                message: "Are you sure want to clear this cheque?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            if ($scope.Accountno != $scope.SameBranchAccountNo) {
                                $scope.ClearBalance = new Object();
                                $scope.ClearBalance.TransactionId = $scope.TrnsactuionclearID;
                                $scope.ClearBalance.ChequeClearDate = moment(new Date($("#txtChequeCleanceDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                                $scope.UnclearBalance = $scope.UnclearBalance - $scope.DepositAmount;
                                $scope.ClearBalance.Type = 1
                                $scope.ClearBalance.Status = 0;
                                $scope.ClearBalance.BranchId = $scope.UserBranch.BranchId;
                                if ($scope.IsSameBranch) {
                                    if ($scope.SameBranchAccountNo != "" && $scope.SameBranchAccountNo != undefined && $scope.SameBranchAccountNo != null) {
                                        $scope.ClearBalance.TempThirdPartyAccNo = $scope.SameBranchAccountNo
                                    }
                                    else {
                                        $scope.ClearBalance.TempThirdPartyAccNo = null
                                    }
                                }
                                UpdateTransactionOnClear($scope.ClearBalance);
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

    function UpdateTransactionOnClear(ClearBalance) {
        var Newtransactionlist = AppService.SaveData("Transaction", "UpdateTransactionData", ClearBalance)
        Newtransactionlist.then(function (p3) {
            if (p3.data != 0) {
                $scope.HolderData.Balance = p3.data;
                GetTransactionList();
                $scope.SearchAccount();
                $("#ClearChequePopup").modal('hide');
                showToastMsg(1, "Your Cheque has been cleared Successfully");
                $scope.ClearBalance.TempThirdPartyAccNo = ''
                $scope.IsSameBranch = false
                $("#txtAccNumber").val('');
                $scope.CustomerData = [];
                $scope.AccBalance = 0.0;
            }
            else {
                showToastMsg(3, "Sorry Can't clear cheque , Due to Insufficient Balance");
            }
        });
    }

    $scope.BounceCheque = function () {

        var flag = true;
        falg = ValidateClearChequeForm();
        if (falg) {
            $(':focus').blur();
            ID = $(this).attr("Id");
            $("#ClearChequePopup").modal('hide');
            $("#CheckBounce").modal('show');
            $scope.UnclearBalance = $scope.UnclearBalance - $scope.DepositAmount;
            $("#txtAccNumber").val('');
            $scope.Clicked = false
        }
    }

    $scope.SavePenalty = function () {

        var flag = ValidatePenaltyForm();
        if (flag) {
            $(':focus').blur();
            bootbox.dialog({
                message: "Are you sure want to bounce this cheque?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            $scope.TransactionData = new Object();
                            $scope.TransactionData.BranchId = $scope.UserBranch.BranchId;
                            $scope.TransactionData.BounceReason = $('#ddlBounceReson').val();
                            $scope.TransactionData.Penalty = $("#txtPenalty").val();
                            $scope.TransactionData.TransactionId = $scope.TrnsactuionclearID;
                            $scope.TransactionData.Type = 2;
                            $scope.TransactionData.ChequeClearDate = moment(new Date($("#txtChequeCleanceDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                            $scope.TransactionData.CustomerProductId = $scope.HolderData.CustomerProductId;

                            if ($scope.IsSameBranch == true) {
                                if ($scope.SameBranchAccountNo != "" && $scope.SameBranchAccountNo != '' && $scope.SameBranchAccountNo != undefined && $scope.SameBranchAccountNo != null) {
                                    $scope.TransactionData.TempThirdPartyAccNo = $scope.SameBranchAccountNo
                                }
                            }
                            else {
                                $scope.TransactionData.TempThirdPartyAccNo = null
                            }
                            var Newtransactionlist = AppService.SaveData("Transaction", "UpdateTransactionDataForBounce", $scope.TransactionData)
                            Newtransactionlist.then(function (p3) {
                                if (p3.data != null) {
                                    $scope.HolderData.Balance = p3.data;
                                    $("#CheckBounce").modal('hide');
                                    showToastMsg(3, 'Sorry, Your Cheque is bounced');
                                    GetTransactionList();
                                    $scope.SearchAccount();
                                    $scope.ClearForm();
                                    $scope.IsSameBranch = false
                                    $("#txtAccNumber").val('');
                                    $scope.CustomerData = []
                                    $scope.AccBalance = 0.0;
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
        $('#tblTransaction').dataTable().fnDraw();
    }

    $scope.SearchClearData = function () {
        $('#txtserchStartDate').val('');
        $('#txtserchEndDate').val('');
        $scope.SearchType = null;
        $('#tblTransaction').dataTable().fnDraw();
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
        //if (!ValidateRequiredField($("#txtWithdrawTransactionTime"), 'Enter Transaction Time', 'after')) {
        //    flag = false;
        //}

        if ($scope.WithdrawTransactionType !== "1" && $scope.WithdrawTransactionType != 1 && $scope.WithdrawTransactionType !== "" && $scope.WithdrawTransactionType !== undefined && $scope.WithdrawTransactionType !== null) {

            if ($scope.WithdrawTransactionType == "7") {
                if (!ValidateRequiredField($("#txtWithdrawNEFTNumber"), ' NEFT Number required', 'after')) {
                    flag = false;
                }
                if (!ValidateRequiredField($("#txtWithdrawNEFTDate"), 'NEFT Date required', 'after')) {
                    flag = false;
                }

            }
            else {
                if (!ValidateRequiredField($("#txtWithdrawChequeNumber"), ' Cheque Number required', 'after')) {
                    flag = false;
                }
                if (!ValidateRequiredField($("#txtWithdrawCheckDate"), 'Cheque Date required', 'after')) {
                    flag = false;
                }

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
        //if (!ValidateRequiredField($("#txtTransactionTime"), 'Enter Transaction Time', 'after')) {
        //    flag = false;
        //}
        if ($scope.TransactionType !== "1" && $scope.TransactionType !== "5" && $scope.TransactionType !== 'undefined' && $scope.TransactionType !== null) {
            if ($scope.TransactionType == "7") {
                if (!ValidateRequiredField($("#txtNEFTDate"), 'NEFT Date required', 'after')) {
                    flag = false;
                }
                if (!ValidateRequiredField($("#txtNEFTNumber"), ' NEFT Number required', 'after')) {
                    flag = false;
                }

            }
            else {
                if (!ValidateRequiredField($("#txtChequeDate"), 'Cheque Date required', 'after')) {
                    flag = false;
                }
                if (!ValidateRequiredField($("#txtCheckNumber"), ' Cheque Number required', 'after')) {
                    flag = false;
                }

            }
            if (!ValidateRequiredField($("#txtBankName"), 'Bank Name required', 'after')) {
                flag = false;
            }
        }

        return flag;
    }

    $scope.Reset = function () {
        $scope.Accountno = '';
        $scope.HolderData = {};
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
        if (Id != null && Id != "") {
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
        else {
            $scope.PendingInstallments = []
        }
    }

    $scope.SelectInstallments = function () {

        var flag = true;

        var toalpay = 0;
        $scope.UpdateRDStatus = [];
        var midinstallment = 0
        angular.forEach($scope.PendingInstallments, function (value, index) {
            if (value.Check == true) {

                if (!ValidateRequiredField($("#txtLatePaymentCharges_" + index), 'Please Enter Late Payment Charges', 'after')) {
                    flag = false;
                }

                if (midinstallment > 0) {
                    $('#DisplayPendingInstallment').modal('hide');
                    showToastMsg(3, 'Installment selection should be in sequence.');
                }
                else {
                    midinstallment = 0
                    toalpay += (parseFloat(value.Amount) + parseFloat(value.LatePaymentCharges));
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

        return flag;
    }

    function RDManualPaymentofIsntallment(obj) {
        var RDMaualTransaction = AppService.SaveData("Transaction", "UpdateRDPaymentStatus", obj);
        RDMaualTransaction.then(function (p4) {
            if (p4.data != 0) {
                $scope.HolderData.Balance = p4.data;
                $scope.UpdateRDStatus = [];
                GetTransactionList();
                $scope.SearchAccount();
                $('#Deposit').modal('hide');
                $scope.DepositAmount = null;
                $scope.TransactionData = null;
                $scope.ClearForm();
                $("#InterAccountTransfer").modal('hide');
                //$scope.PendingRDInstallments($scope.HolderData.CustomerProductId);
            }
            else {
                showToastMsg(3, "You have Insufficient balance or Error in while submitting");
            }
        });
    }

    function ValidateInterAccountTransfer() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#ddlToAccount"), 'Account Required', 'after')) {
            flag = false;
        }
        //if (!ValidateRequiredField($("#txtinterAcTransactionTime"), 'Transaction Time required', 'after')) {
        //    flag = false;
        //}
        if (!ValidateRequiredField($("#txtAmount"), 'Amount Required', 'after')) {
            flag = false;
        }


        return flag;
    }

    $scope.SaveInterAccountTransfer = function () {

        var flag1 = true;
        var flag2 = true;

        flag1 = ValidateInterAccountTransfer();
        flag2 = $scope.SelectInstallments();
        if (flag1 && flag2) {
            $(':focus').blur();
            if ($scope.UpdateRDStatus.length > 0) {
                $scope.TransactionData = new Object();
                $scope.TransactionData.BranchId = $scope.UserBranch.BranchId;
                $scope.TransactionData.CustomerProductId = $scope.HolderData.CustomerProductId
                $scope.TransactionData.Status = 0;
                if ($scope.HolderData.Balance >= parseFloat($scope.Amount)) {
                    $scope.TransactionData.Balance = $scope.HolderData.Balance;
                    $scope.TransactionData.ProductwiseBalance = (parseFloat($scope.HolderData.Balance) + parseFloat($scope.HolderData.UnclearBalance)) - parseFloat($scope.WithdrawAmount);
                }
                else {
                    return showToastMsg(3, 'Cannot transfer amount due to Insufficient balance');
                }

                $scope.TransactionData.CustomerId = $scope.HolderData.CustomerId;
                $scope.TransactionData.Amount = $scope.Amount
                $scope.TransactionData.Type = 2;
                $scope.TransactionData.TransactionType = 4;
                $scope.TransactionData.RefCustomerProductId = $scope.RefCustomerProductId;
                $scope.TransactionData.CreatedBy = $cookies.getObject('User').UserId;
                $scope.TransactionData.BranchId = $cookies.get('Branch');
                //$scope.TransactionData.TransactionTime = moment(new Date($("#txtinterAcTransactionTime").data("DateTimePicker").date())).format('YYYY-MM-DD hh:mm:ss a');

                var obj = new Object();
                obj.rdPaymentList = $scope.UpdateRDStatus;
                obj.transaction = $scope.TransactionData;

                RDManualPaymentofIsntallment(obj);
            }
        }
    }

    function ValidateBankAccountTransfer() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtFromAccount"), 'Account Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtToAccount"), 'To Account required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtBankTrnAmount"), 'Amount Required', 'after')) {
            flag = false;
        }
        //if (!ValidateRequiredField($("#txtBankAcTransactionTime"), 'Time Required', 'after')) {
        //    flag = false;
        //}

        return flag;
    }

    $scope.SaveBankTransfer = function () {


        var flag = true;

        flag = ValidateBankAccountTransfer();

        if (flag) {
            $(':focus').blur();
            if ($scope.HolderData.Balance >= parseFloat($scope.BankTransfer.Amount)) {
                $scope.BankTransfer.BranchId = $scope.UserBranch.BranchId;
                $scope.BankTransfer.Balance = 0;
                $scope.BankTransfer.CustomerProductId = $scope.HolderData.CustomerProductId
                $scope.BankTransfer.Status = 0;
                $scope.BankTransfer.CustomerId = $scope.HolderData.CustomerId;
                $scope.BankTransfer.Type = 2;
                $scope.BankTransfer.TransactionType = 4;
                $scope.BankTransfer.RefCustomerProductId = $scope.CustomerData[0].CustomerProductId;
                $scope.BankTransfer.CreatedBy = $cookies.getObject('User').UserId;
                $scope.BankTransfer.DescIndentify = 18
                //$scope.BankTransfer.TransactionTime = moment(new Date($("#txtBankAcTransactionTime").data("DateTimePicker").date())).format('YYYY-MM-DD hh:mm:ss a');
                var saveTransaction = AppService.SaveData("Transaction", "SaveTransaction", $scope.BankTransfer);
                saveTransaction.then(function (p1) {
                    if (p1.data != null) {

                        $scope.BankTransfer2 = new Object();
                        $scope.BankTransfer2.BranchId = $scope.UserBranch.BranchId;
                        $scope.BankTransfer2.Balance = 0;
                        $scope.BankTransfer2.Amount = $scope.BankTransfer.Amount;
                        $scope.BankTransfer2.CustomerProductId = $scope.CustomerData[0].CustomerProductId;
                        $scope.BankTransfer2.Status = 0;
                        $scope.BankTransfer2.CustomerId = $scope.CustomerData[0].CustomerId;
                        $scope.BankTransfer2.Type = 1;
                        $scope.BankTransfer2.TransactionType = 4;
                        $scope.BankTransfer2.RefCustomerProductId = $scope.HolderData.CustomerProductId
                        $scope.BankTransfer2.CreatedBy = $cookies.getObject('User').UserId;
                        $scope.BankTransfer2.DescIndentify = 17
                        $scope.BankTransfer2.Description = $scope.BankTransfer.Description;
                        //$scope.BankTransfer2.TransactionTime = moment(new Date($("#txtBankAcTransactionTime").data("DateTimePicker").date())).format('YYYY-MM-DD hh:mm:ss a');
                        var saveTransaction2 = AppService.SaveData("Transaction", "SaveTransaction", $scope.BankTransfer2);

                        saveTransaction2.then(function (p2) {
                            if (p2.data != null) {
                                $scope.BankTransfer2 = {};
                                $scope.BankTransfer = {};
                                $scope.CustomerData = [];
                                showToastMsg(1, "Transaction Saved Successfully");
                                GetTransactionList();
                                $scope.SearchAccount();
                                $('#BankTransfer').modal('hide');
                                $scope.ClearForm();
                            }
                        });

                    }
                    else {
                        showToastMsg(3, 'Error in saving data');
                    }
                });

            }
            else {
                return showToastMsg(3, 'Cannot Transfer amount due to Insufficient balance');
            }
        }
    }

    $scope.ClearBankTransferForm = function () {
        $scope.BankTransfer2 = {};
        $scope.BankTransfer = {};
        $scope.CustomerData = [];
        GetTransactionList();
        $scope.SearchAccount();
        $('#BankTransfer').modal('hide');
        $scope.ClearForm();
    }

    $scope.DownloadPDF = function () {
        $(':focus').blur();
        var obj = new Object();
        obj.AccountNumber = $scope.Accountno
        obj.Type = $scope.SearchType;
        if ($("#txtserchStartDate").val() != null && $("#txtserchStartDate").val() != undefined) {
            obj.FromDate = ddmmyyTommdddyy($("#txtserchStartDate").val())
        }
        if ($("#txtserchEndDate").val() != null && $("#txtserchEndDate").val() != undefined) {
            obj.ToDate = ddmmyyTommdddyy($("#txtserchEndDate").val())
        }

        var downloadpdfstatement = AppService.SaveData("Transaction", "TransactionStatement", obj);

        downloadpdfstatement.then(function (p2) {
            if (p2.data != null) {

                var fileName = '../PDFStatement/' + p2.data;
                var a = document.createElement("a");
                a.href = fileName;
                a.download = p2.data;
                a.click();
            }
        });
    }

    $scope.Totalremaininginteresttilldate = 0;
    $scope.TotalBalancetillDate = 0;

    $scope.GetLoanDetail = function (Id) {
        var GetLoanDetailByIdForPrePayment = AppService.GetDetailsById("Loan", "GetLoanDetailByIdForPrePayment", Id);

        GetLoanDetailByIdForPrePayment.then(function (p2) {
            if (p2.data != null) {
                $scope.PrePayment.TotalPendingInstallmentAmount = p2.data.TotalPendingInstallmentAmount;


                $scope.PrePayment.PaidPrincipalAmount = p2.data.Loan.PaidPrincipalAmount
                $scope.PrePayment.MonthlyInstallmentAmount = p2.data.Loan.MonthlyInstallmentAmount;
                $scope.PrePayment.Term = p2.data.Loan.RemainingMonth;
                $scope.PrePayment.NextInstallmentDate = p2.data.Loan.NextInstallmentDate;
                $scope.PrePayment.LoanIntrestRate = p2.data.Loan.LoanIntrestRate;
                $scope.PrePayment.LoanTypeName = p2.data.Loan.LoanTypeName;
                if (p2.data.Loan.LoanTypeName != "Flexi Loan") {
                    $scope.PrePayment.TotalLoanAmount = p2.data.Loan.TotalLoanAmount;
                    $("#PrePaymentMonthlyDiv").show();
                    $("#PrePaymentTermDiv").show();
                    $("#PrePaymentClosingLoandiv").show();
                    $("#btnCloseLoan").attr("disabled", false);

                }
                else {
                    $scope.PrePayment.TotalLoanAmount = p2.data.Loan.TotalAmountToPay;
                    $("#PrePaymentMonthlyDiv").hide();
                    $("#PrePaymentTermDiv").hide();
                    $("#PrePaymentClosingLoandiv").hide();
                    $("#btnCloseLoan").attr("disabled", true);

                }
                $scope.PrePayment.LoanId = p2.data.Loan.LoanId;
                $scope.PrePayment.RemainingAmount = ($scope.PrePayment.TotalLoanAmount - $scope.PrePayment.PaidPrincipalAmount).toFixed(2);
                if (p2.data.LoanTypeName != "Flexi Loan") {
                    if (p2.data.Totalremaininginteresttilldate != 0 || p2.data.Totalremaininginteresttilldate != null) {
                        //$scope.PrePayment.RemainingAmount = (parseFloat($scope.PrePayment.RemainingAmount) + parseFloat(p2.data.Totalremaininginteresttilldate)).toFixed(2);
                        $scope.TotalBalancetillDate = (parseFloat($scope.PrePayment.RemainingAmount) + parseFloat(p2.data.Totalremaininginteresttilldate)).toFixed(2);

                        $scope.Totalremaininginteresttilldate = p2.data.Totalremaininginteresttilldate.toFixed(2);
                    }
                }
                if (parseFloat($scope.PrePayment.TotalPendingInstallmentAmount) > 0) {
                    $scope.TotalBalancetillDate = (parseFloat($scope.TotalBalancetillDate) + parseFloat($scope.PrePayment.TotalPendingInstallmentAmount.toFixed(2))).toFixed(2);

                }

                if (parseFloat($scope.PrePayment.RemainingAmount) < 0) {
                    $scope.PrePayment.RemainingAmount = 0;
                }
            }
        });
    }

    $scope.CalculateLoanAmountisation = function () {
        var obj = new Object();
        obj.LoanId = $scope.PrePayment.LoanId;
        if ($scope.PrePayment.PaymentAmount != null && $scope.PrePayment.PaymentAmount != "" && $scope.PrePayment.Term != "" && $scope.PrePayment.Term != undefined) {
            //obj.PrincipalAmount = $scope.PrePayment.TotalLoanAmount - $scope.PrePayment.PaidPrincipalAmount - $scope.PrePayment.PaymentAmount);
            var RemainingAmount = parseFloat($scope.PrePayment.TotalLoanAmount - $scope.PrePayment.PaidPrincipalAmount);
            if (parseFloat(RemainingAmount) < 0) {
                RemainingAmount = 0;
            }
            obj.PrincipalAmount = parseFloat(RemainingAmount.toFixed(2)) - parseFloat($scope.PrePayment.PaymentAmount);

            if (parseFloat($scope.PrePayment.TotalPendingInstallmentAmount) > 0) {
                obj.PrincipalAmount = parseFloat(obj.PrincipalAmount.toFixed(2)) + parseFloat($scope.PrePayment.TotalPendingInstallmentAmount.toFixed(2));
                obj.TotalPendingInstallmentAmount = parseFloat($scope.PrePayment.TotalPendingInstallmentAmount.toFixed(2));

            }


            if (obj.PrincipalAmount == 0) {
                $scope.PrePayment.Term = 0;
                $scope.PrePayment.MonthlyInstallmentAmount = 0;
            }
            else {
                if (parseFloat($scope.PrePayment.PaymentAmount) < parseFloat($scope.PrePayment.RemainingAmount) && ($scope.PrePayment.Term == 0)) {

                    $("#txtTerm").closest('.form-group').removeClass('has-error');
                    $("#txtTerm").next('.help-block').remove();
                    $("#txtTerm").prev('.help-block').remove();
                    $("#txtTerm").closest('.form-group').addClass('has-error');
                    $('<span class="help-block help-block-error">  Please Enter the Term Properly.</span>').insertAfter($("#txtTerm"));
                }
                else {
                    obj.LoanIntrestRate = $scope.PrePayment.LoanIntrestRate;
                    obj.Term = $scope.PrePayment.Term;
                    //obj.LoanId = $scope.PrePayment.LoanId;
                    obj.InstallmentDate = moment(new Date($scope.PrePayment.NextInstallmentDate)).format('YYYY-MM-DD');

                    var Amountisation = AppService.SaveData("Loan", "LoanAmountisationforPrePayment", obj);


                    Amountisation.then(function (p1) {
                        if (p1.data != null) {
                            $scope.LoanAmountisation = p1.data.ListLoanAmountisation;
                            $scope.PrePayment.MonthlyInstallmentAmount = p1.data.MonthlyInstallmentAmount;
                            if (parseFloat($scope.PrePayment.MonthlyInstallmentAmount) < 0) {
                                $scope.PrePayment.MonthlyInstallmentAmount = 0;
                                $("#tabAmountisation").hide();
                            }
                            else {
                                $("#tabAmountisation").show();
                            }

                        }
                        else {
                            showToastMsg(3, 'Error in saving data')
                        }
                    }, function (err) {
                        showToastMsg(3, 'Error in Saving Data')
                    });
                }
            }
        }

    }

    function ValidatePrePayment() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txPrePaymenttDepositamount"), 'Amount Required', 'after')) {
            flag = false;
        }
        else {
            var RemainingAmt = $scope.PrePayment.RemainingAmount;
            if ($scope.PrePayment.TotalPendingInstallmentAmount > 0) {
                RemainingAmt = parseFloat($scope.PrePayment.RemainingAmount) + parseFloat($scope.PrePayment.TotalPendingInstallmentAmount);
                if ($scope.PrePayment.RemainingAmount < 0) {
                    $scope.PrePayment.RemainingAmount = RemainingAmt.toFixed(2);
                }
            }
            if (parseFloat($scope.PrePayment.PaymentAmount) > parseFloat(RemainingAmt)) {
                flag = false
                $("#txPrePaymenttDepositamount").closest('.form-group').removeClass('has-error');
                $("#txPrePaymenttDepositamount").next('.help-block').remove();
                $("#txPrePaymenttDepositamount").prev('.help-block').remove();
                $("#txPrePaymenttDepositamount").closest('.form-group').addClass('has-error');
                $('<span class="help-block help-block-error">  Please add Payment amount less than or equal to Pending Principal.</span>').insertAfter($("#txPrePaymenttDepositamount"));
            }
        }
        if (!ValidateRequiredField($("#ddlLoanAccount"), 'Loan Account required', 'after')) {
            flag = false;
        }

        if ($scope.PrePayment.LoanTypeName != "Flexi Loan") {

            if (!ValidateRequiredField($("#txtTerm"), 'Term Required', 'after')) {
                flag = false;
            }
            else if (parseFloat($scope.PrePayment.PaymentAmount) < parseFloat($scope.PrePayment.RemainingAmount) && ($scope.PrePayment.Term == 0)) {
                $("#txtTerm").closest('.form-group').removeClass('has-error');
                $("#txtTerm").next('.help-block').remove();
                $("#txtTerm").prev('.help-block').remove();
                $("#txtTerm").closest('.form-group').addClass('has-error');
                $('<span class="help-block help-block-error">  Please Enter the Term Properly.</span>').insertAfter($("#txtTerm"));
                flag = false;
            }
        }
        //if (!ValidateRequiredField($("#txtLoanPrePaymentTransactionTime"), 'Time Required', 'after')) {
        //    flag = false;
        //}
        return flag;
    }

    function ValidateCloseLoan() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txPrePaymenttDepositamount"), 'Amount Required', 'after')) {
            flag = false;
        }
        else {
            var RemainingAmt = $scope.TotalBalancetillDate;
            if ($scope.PrePayment.TotalPendingInstallmentAmount > 0) {
                if ($scope.PrePayment.RemainingAmount < 0) {
                    $scope.PrePayment.RemainingAmount = RemainingAmt.toFixed(2);
                }
            }
            if (parseFloat($scope.PrePayment.PaymentAmount) > parseFloat(RemainingAmt)) {
                flag = false
                $("#txPrePaymenttDepositamount").closest('.form-group').removeClass('has-error');
                $("#txPrePaymenttDepositamount").next('.help-block').remove();
                $("#txPrePaymenttDepositamount").prev('.help-block').remove();
                $("#txPrePaymenttDepositamount").closest('.form-group').addClass('has-error');
                $('<span class="help-block help-block-error">  Please add Payment amount equal to Total Balance.</span>').insertAfter($("#txPrePaymenttDepositamount"));
            }
            if (parseFloat($scope.PrePayment.PaymentAmount) < parseFloat(RemainingAmt)) {
                flag = false
                $("#txPrePaymenttDepositamount").closest('.form-group').removeClass('has-error');
                $("#txPrePaymenttDepositamount").next('.help-block').remove();
                $("#txPrePaymenttDepositamount").prev('.help-block').remove();
                $("#txPrePaymenttDepositamount").closest('.form-group').addClass('has-error');
                $('<span class="help-block help-block-error">  Please add Payment amount equal to Total Balance.</span>').insertAfter($("#txPrePaymenttDepositamount"));
            }

        }
        if (!ValidateRequiredField($("#ddlLoanAccount"), 'Loan Account required', 'after')) {
            flag = false;
        }

        if ($scope.PrePayment.LoanTypeName != "Flexi Loan") {

            if (!ValidateRequiredField($("#txtTerm"), 'Term Required', 'after')) {
                flag = false;
            }
            else if (($scope.PrePayment.Term != 0)) {
                $("#txtTerm").closest('.form-group').removeClass('has-error');
                $("#txtTerm").next('.help-block').remove();
                $("#txtTerm").prev('.help-block').remove();
                $("#txtTerm").closest('.form-group').addClass('has-error');
                $('<span class="help-block help-block-error">  Term should be Zero(0).</span>').insertAfter($("#txtTerm"));
                flag = false;
            }
        }
        return flag;
    }


    $scope.AddPrePayment = function () {
        var flag = true;
        flag = ValidatePrePayment();
        var RemainingAmt = $scope.PrePayment.RemainingAmount;
        if ($scope.PrePayment.TotalPendingInstallmentAmount > 0) {
            RemainingAmt = $scope.PrePayment.TotalPendingInstallmentAmount;
            if (parseFloat($scope.PrePayment.RemainingAmount) <= 0) {
                $scope.PrePayment.RemainingAmount = RemainingAmt.toFixed(2);
            }
        }

        if (flag) {
            $(':focus').blur();
            if (parseFloat($scope.HolderData.Balance) >= parseFloat($scope.PrePayment.PaymentAmount)) {

                // $scope.PrePayment.TransactionTime = moment(new Date($("#txtLoanPrePaymentTransactionTime").data("DateTimePicker").date())).format('YYYY-MM-DD hh:mm:ss a');
                //$scope.PrePayment.LoanAmount = ($scope.PrePayment.DisbursementAmount - $scope.PrePayment.PaidPrincipalAmount - $scope.PrePayment.PaymentAmount);
                $scope.PrePayment.CustomerProductIdSaving = $scope.HolderData.CustomerProductId;
                $scope.PrePayment.InstallmentDate = moment(new Date($scope.PrePayment.NextInstallmentDate)).format('YYYY-MM-DD');
                $scope.PrePayment.LoanAmount = ($scope.PrePayment.TotalLoanAmount - $scope.PrePayment.PaidPrincipalAmount - $scope.PrePayment.PaymentAmount).toFixed(2);

                var AddLoanPrePayment = AppService.SaveData("Transaction", "AddPrePayment", $scope.PrePayment);

                AddLoanPrePayment.then(function (p2) {
                    if (p2.data != null) {
                        $scope.PrePayment = {};
                        showToastMsg(1, "Payment Added Successfully");
                        $('#LoanPrepayment').modal('hide');
                        $scope.ClearForm();
                        GetHolderDetail();

                    }
                });
            }
            else {
                return showToastMsg(3, 'Cannot Transfer amount due to Insufficient balance');
            }
        }
    }



    $scope.CloseLoan = function () {
        var flag = true;
        flag = ValidateCloseLoan();

        if (flag) {
            $(':focus').blur();
            var RemainingAmt = $scope.TotalBalancetillDate;
            $scope.PrePayment.RemainingAmount = $scope.TotalBalancetillDate;
            if (parseFloat($scope.PrePayment.RemainingAmount) <= 0) {
                $scope.PrePayment.RemainingAmount = RemainingAmt.toFixed(2);
            }
            $scope.PrePayment.TotalPendingInteresttillDate = $scope.Totalremaininginteresttilldate;
            if (parseFloat($scope.HolderData.Balance) >= parseFloat($scope.PrePayment.PaymentAmount)) {

                $scope.PrePayment.CustomerProductIdSaving = $scope.HolderData.CustomerProductId;
                $scope.PrePayment.InstallmentDate = moment(new Date($scope.PrePayment.NextInstallmentDate)).format('YYYY-MM-DD');
                $scope.PrePayment.LoanAmount = ($scope.PrePayment.TotalLoanAmount - $scope.PrePayment.PaidPrincipalAmount - $scope.PrePayment.PaymentAmount).toFixed(2);

                var AddLoanPrePayment = AppService.SaveData("Transaction", "AddPrePayment", $scope.PrePayment);

                AddLoanPrePayment.then(function (p2) {
                    if (p2.data != null) {
                        $scope.PrePayment = {};
                        showToastMsg(1, "Payment Added Successfully");
                        $('#LoanPrepayment').modal('hide');
                        $scope.ClearForm();
                        GetHolderDetail();

                    }
                });
            }
            else {
                return showToastMsg(3, 'Cannot Transfer amount due to Insufficient balance');
            }
        }
    }

})