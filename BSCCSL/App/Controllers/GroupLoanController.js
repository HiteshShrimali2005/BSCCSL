angular.module("BSCCL").controller('GroupLoanController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $location) {

    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;

    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');

    $(".datepicker").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });
    $scope.GrpLoan = {};
    $scope.AccountHolderList = [];
    if ($location.search().GroupLoanId != undefined) {
        $scope.GroupLoanId = $location.search().GroupLoanId;
        GetAllLookup();
        GetGroupLoandetail($location.search().GroupLoanId, $location.search().LoanId);
    }

    GetProductDetail();
    $scope.LoanCharges = [];
    $scope.LoanDocuments = [];
    $scope.LoanAmountisation = [];
    $scope.AllLoanCharges = [];

    GetAllLookup();

    function GetAllLookup() {
        var getbranch = AppService.GetDetailsWithoutId("Loan", "GetAllLookup");
        getbranch.then(function (p1) {
            if (p1.data != null) {
                $scope.LookUp = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function GetGroupLoandetail(GroupLoanId) {

        $scope.AccountHolderList = []
        var getcustomerdetail = AppService.GetDetailsById("Loan", "GetGroupLoandetail", GroupLoanId);
        getcustomerdetail.then(function (p1) {
            if (p1.data != null) {
                $scope.AccountHolderList = p1.data.Details;
                $scope.GrpLoan = p1.data.LoanDetail;
                $("#txtGrpdateofapplication").data("DateTimePicker").date($filter('date')(p1.data.LoanDetail.DateofApplication, 'dd/MM/yyyy'));
                //$("#txtGrpdateofInstallment").data("DateTimePicker").date($filter('date')(p1.data.LoanDetail.InstallmentDate, 'dd/MM/yyyy'));
                $("#txtGrpdateofOpening").data("DateTimePicker").date($filter('date')(p1.data.LoanDetail.OpeningDate, 'dd/MM/yyyy'));
                if (p1.data.LoanDetail.DateOfCredit != null) {
                    $("#txtGrpdateofCreditapplication").data("DateTimePicker").date($filter('date')(p1.data.LoanDetail.DateOfCredit, 'dd/MM/yyyy'));
                }
                if (p1.data.LoanDetail.PreviousLoanCompleted != null) {
                    $("#txtGrpPreviousLoanCompleted").data("DateTimePicker").date($filter('date')(p1.data.LoanDetail.PreviousLoanCompleted, 'dd/MM/yyyy'));
                }

                //angular.forEach($scope.AccountHolderList, function (value, index) {
                //    value.ServiceAmount = parseFloat(value.ProcessingCharge) * parseFloat(value.ServiceTax) / 100;
                //    value.TotalLoanAmount = parseFloat(value.LoanAmount) + parseFloat(value.ProcessingCharge) + parseFloat(value.ServiceAmount);
                //})

                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function GetProductDetail() {
        var Promis = AppService.GetDetailsWithoutId("CustomerProduct", "GetProductNameOfGroupLoan");
        Promis.then(function (p1) {
            if (p1.data != null) {
                $scope.Productlist = [];
                $scope.Productlist = p1.data;
            }
            else {
                showToastMsg(3, 'No Product Avilable')
            }
        })
    }

    $scope.GetAllUserById = function (Id) {
        var getemployeedetail = AppService.GetDetailsById("User", "GetUserDataById", Id)
        getemployeedetail.then(function (p1) {
            if (p1.data != null) {
                if ($scope.Agent == true) {
                    $scope.GrpLoan.AgentName = p1.data.FirstName;
                    $scope.GrpLoan.AgentCode = p1.data.UserCode;
                    $scope.GrpLoan.AgentId = p1.data.UserId;
                }
                else {
                    $scope.GrpLoan.EmpName = p1.data.FirstName;
                    $scope.GrpLoan.EmpCode = p1.data.UserCode;
                    $scope.GrpLoan.EmployeeId = p1.data.UserId;
                }
            }
            else {
                showToastMsg(3, "Error in Getting Data");
            }
        })
    }

    $scope.GetLoanDetailsByLoanId = function (data) {
        $scope.PersonalLoan = {};
        $scope.UpdateStatus = {};
        $scope.ShareCertificateNo = "";
        $scope.TotalValue = "";
        $scope.NoOfShares = ""
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var getcustomerdetail = AppService.GetDataByQuerystring("Loan", "GetCustomerPersonalDetailById", data.CustomerProductId, data.LoanId);
        getcustomerdetail.then(function (p1) {
            if (p1.data != null) {
                $scope.PersonalLoan = p1.data.LoanDetail;

                if ($scope.PersonalLoan.DisbursementAmount == null || $scope.PersonalLoan.DisbursementAmount == undefined || $scope.PersonalLoan.DisbursementAmount == "") {
                    $scope.PersonalLoan.DisbursementAmount = $scope.PersonalLoan.LoanAmount;
                    $scope.PersonalLoan.TotalDisbursementAmount = $scope.PersonalLoan.LoanAmount;
                }
                else {
                    $scope.PersonalLoan.TotalDisbursementAmount = $scope.PersonalLoan.DisbursementAmount;
                }
                $scope.PersonalLoan.ClientId = data.ClientId;
                $scope.CustomerList = p1.data.CustomerList;
                $scope.LoanDocuments = p1.data.LoanDocuments;
                $scope.LoanCharges = p1.data.LoanCharges;
                $scope.LoanStatusList = p1.data.LoanStatusList;
                var newObj = new Object();
                newObj.LoanStatus = $scope.PersonalLoan.LoanStatus == 0 ? "" : $scope.PersonalLoan.LoanStatus.toString();
                //newObj.Comment = $scope.PersonalLoan.Comment;
                $scope.UpdateStatus = newObj;

                if ($scope.PersonalLoan.DisburseThrough == null) {
                    $scope.PersonalLoan.DisburseThrough = '';
                }
                else {
                    $scope.PersonalLoan.DisburseThrough = $scope.PersonalLoan.DisburseThrough + '';
                }

                $("#txtLoandateofapplication").data("DateTimePicker").date($filter('date')(p1.data.LoanDetail.DateofApplication, 'dd/MM/yyyy'));
                $("#txtInstallmentDate").data("DateTimePicker").date($filter('date')(p1.data.LoanDetail.InstallmentDate, 'dd/MM/yyyy'));

                if ($scope.LoanCharges.length > 0) {
                    var chargedata = $filter('filter')($scope.LoanCharges, { Name: 'Share' })[0];
                    if (chargedata != undefined) {
                        $scope.NoOfShares = chargedata.NoOfItem;
                        $scope.TotalValue = chargedata.Value;
                        $scope.ShareCertificateNo = chargedata.CertificateNo;
                    }
                }

                LoanAmountisation();
                //$("#tab7 a").click();

                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.DocumentShown = 0;
    $scope.Disbursement = 0;

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if (e.target.id === 'btntab10') {
            $scope.DocumentShown = 1;
            $scope.Disbursement = 0;
        } else if (e.target.id === 'btntab8') {
            $scope.DocumentShown = 0;
            $scope.Disbursement = 1;
        }
        else {
            $scope.DocumentShown = 0;
            $scope.Disbursement = 0;
        }
        if (!$scope.$$phase) {
            $scope.$apply();
        }
    });

    $scope.ChangeShareValue = function (value) {
        $scope.TotalValue = 0;
        $scope.TotalValue = value * 100;
        if (!$scope.$$phase) {
            $scope.$apply();
        }
    }

    $scope.SaveLoanDetails = function (string) {
        $scope.IsDisbursement = false;
        var flag = true;
        flag = ValidateCustomerLoan();

        if (flag) {


            if ($("#txtInstallmentDate").val() != "") {

                var a = moment(new Date($("#txtInstallmentDate").data("DateTimePicker").date())).format('YYYY-MM-DD').match(/(\d+)/g);
                var b = moment(new Date()).format('YYYY-MM-DD').match(/(\d+)/g);;

                var firstDate = new Date(parseInt(a[0]), parseInt(a[1]) - 1, parseInt(a[2]))
                var secondDate = new Date(parseInt(b[0]), parseInt(b[1]) - 1, parseInt(b[2]))

                var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds

                c = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));

                if (c < 30 && !$scope.PersonalLoan.IsDisbursed && $scope.PersonalLoan.LoanStatus == 4 && (!$scope.InstallmentDateflag || $scope.InstallmentDateflag == undefined)) {
                    $scope.InstallmentDateflag = false
                    $("#txtInstallmentDate").closest('.form-sgroup').removeClass('has-warning');
                    $("#txtInstallmentDate").closest('.form-group').removeClass('has-error');
                    $("#txtInstallmentDate").next('.help-block').remove();
                    $("#txtInstallmentDate").prev('.help-block').remove();
                    $("#txtInstallmentDate").closest('.form-group').addClass('has-warning');
                    $('<span class="help-block help-block-warning"> This duration is less than 30 days</span>').insertAfter($("#txtInstallmentDate"));
                }
                else {
                    $scope.InstallmentDateflag = true
                }

            }

            if (!$scope.InstallmentDateflag && $scope.PersonalLoan.LoanStatus == 4) {
                ValidateInstallmentDate();
            }
            else {
                $scope.InstallmentDateflag = true
            }

            if ($scope.InstallmentDateflag) {

                $scope.PersonalLoan.CustomerId = $scope.PersonalLoan.CustomerId;
                //$scope.PersonalLoan.CustomerProductId = $scope.CustomerProductId;
                $scope.PersonalLoan.DateofApplication = moment(new Date($("#txtLoandateofapplication").data("DateTimePicker").date())).format('YYYY-MM-DD');
                if ($("#txtInstallmentDate").val() != "") {
                    $scope.PersonalLoan.InstallmentDate = moment(new Date($("#txtInstallmentDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                }

                if ($scope.PersonalLoan.LoanId == '0000000-0000-0000-0000-000000000000' || $scope.PersonalLoan.LoanId != undefined || $scope.PersonalLoan.LoanId == "") {
                    $scope.PersonalLoan.CreatedBy = $cookies.getObject('User').UserId;
                }
                else {
                    $scope.PersonalLoan.ModifiedBy = $cookies.getObject('User').UserId;
                }

                if ($scope.NoOfShares != undefined) {
                    var sharedata = $filter('filter')($scope.LoanCharges, { Name: 'Share' })[0];
                    if (sharedata == undefined) {
                        var NewObj = new Object();
                        NewObj.Name = "Share";
                        NewObj.Value = $scope.TotalValue;
                        NewObj.NoOfItem = $scope.NoOfShares;
                        NewObj.CertificateNo = $scope.ShareCertificateNo;
                        $scope.LoanCharges.push(NewObj);
                    }
                    else {
                        sharedata.Value = $scope.TotalValue;
                        sharedata.NoOfItem = $scope.NoOfShares;
                        sharedata.CertificateNo = $scope.ShareCertificateNo;
                    }
                }

                $scope.UpdateStatus.UpdatedBy = getUserdata.UserId;
                $scope.UpdateStatus.LoanId = $scope.PersonalLoan.LoanId;

                var NewObj = new Object();
                NewObj.Loan = $scope.PersonalLoan;
                if ($scope.UpdateStatus.LoanStatus != 7) {
                    NewObj.LoanStatus = $scope.UpdateStatus;
                }
                NewObj.LoanCharges = $scope.LoanCharges;

                var saveloan = AppService.SaveData("Loan", "SaveLoanFromGroupLoan", NewObj)
                saveloan.then(function (p1) {
                    if (p1.data != null) {
                        $scope.InstallmentDateflag = false;
                        $scope.PersonalLoan.LoanId = p1.data;
                        GetGroupLoandetail($scope.GrpLoan.GroupLoanId)
                        showToastMsg(1, "Loan Saved Successfully");
                        if (string == "Close") {
                            $("#GroupPersonalLoan").modal('hide');
                            $("#tab7 a").click();
                        } else {
                            $scope.GetLoanDetailsByLoanId($scope.PersonalLoan);
                            GetChargesList();
                            LoanAmountisation();
                        }
                    }
                    else {
                        showToastMsg(3, 'Error in Saving Data')
                    }
                });
            }
        }
        else {
            return false;
        }
    }

    function ValidateInstallmentDate() {


        bootbox.dialog({
            message: "You have selected less than 30 days Duration date(Installment Date). Are you sure want to continue?",
            title: "Confirmation !",
            size: 'small',
            buttons: {
                success: {
                    label: "Yes",
                    className: "btn-success btn-flat",
                    callback: function () {
                        $scope.InstallmentDateflag = true;
                        $scope.SaveLoanDetails();
                    }
                },
                danger: {
                    label: "No",
                    className: "btn-danger btn-flat",
                    callback: function () {
                        $("#disbursementAmount").modal('hide');
                    }
                }
            }
        });

    }

    function ValidateCustomerLoan() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtLoandateofapplication"), 'Date of Application required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtamount"), 'Amountrequired', 'after')) {
            $("#btntab7").click();
            flag = false;
        }
        if (!ValidateRequiredField($("#txtplace"), 'Place required', 'after')) {
            $("#btntab7").click();
            flag = false;
        }

        if (!ValidateRequiredField($("#txtterm"), 'Loan Term required', 'after')) {
            flag = false;
        }

        if ($scope.UpdateStatus.LoanStatus == 4 || $scope.PersonalLoan.LoanStatus == 4) {
            if (!ValidateRequiredField($("#txtDisbursementAmt"), 'Amount required', 'after')) {
                flag = false;
            }


            if ($("#txtInstallmentDate").val() != "" && $("#txtInstallmentDate").val() != undefined && $("#txtInstallmentDate").val() != null) {
                var EMI = moment(new Date($("#txtInstallmentDate").data("DateTimePicker").date())).format('YYYY-MM-DD')
                var today = moment(new Date()).format('YYYY-MM-DD')

                if (EMI < today) {
                    flag = false;
                    $("#txtInstallmentDate").closest('.form-group').removeClass('has-warning');
                    $("#txtInstallmentDate").closest('.form-group').removeClass('has-error');
                    $("#txtInstallmentDate").next('.help-block').remove();
                    $("#txtInstallmentDate").prev('.help-block').remove();
                    $("#txtInstallmentDate").closest('.form-group').addClass('has-error');
                    $('<span class="help-block help-block-error">  Installment date should be future date</span>').insertAfter($("#txtInstallmentDate"));
                }
            }
            else if ($scope.PersonalLoan.LoanStatus == 4) {
                if (!ValidateRequiredField($("#txtInstallmentDate"), 'InstallmentDate required', 'after')) {
                    flag = false;
                }
            }

            if ($scope.NoOfShares != undefined && $scope.PersonalLoan.LoanStatus == 4 && $scope.IsDisbursement) {
                if (!ValidateRequiredField($("#txtShareCertificateNo"), 'Certificate No. required', 'after')) {
                    flag = false;
                }
                if (!ValidateRequiredField($("#txtInstallmentDate"), 'InstallmentDate required', 'after')) {
                    flag = false;
                }
                else {
                    var a = moment(new Date($("#txtInstallmentDate").data("DateTimePicker").date())).format('YYYY-MM-DD').match(/(\d+)/g);
                    var b = moment(new Date()).format('YYYY-MM-DD').match(/(\d+)/g);;

                    var firstDate = new Date(parseInt(a[0]), parseInt(a[1]) - 1, parseInt(a[2]))
                    var secondDate = new Date(parseInt(b[0]), parseInt(b[1]) - 1, parseInt(b[2]))

                    var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds

                    c = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));

                    if (c < 30 && $scope.PersonalLoan.LoanStatus == 4 && ($scope.InstallmentDateflag == false || $scope.InstallmentDateflag == undefined)) {
                        $("#btntab7").click();
                        $("#txtInstallmentDate").closest('.form-group').removeClass('has-error');
                        $("#txtInstallmentDate").next('.help-block').remove();
                        $("#txtInstallmentDate").prev('.help-block').remove();
                        $("#txtInstallmentDate").closest('.form-group').addClass('has-warning');
                        $('<span class="help-block help-block-warning"> You have selected less than 30 days installment date</span>').insertAfter($("#txtInstallmentDate"));
                    }
                }
            }

            if ($scope.NoOfShares != undefined && $scope.PersonalLoan.LoanStatus == 4 && $scope.IsDisbursement && $("#txtShareCertificateNo").val() == "") {
                $("#btntab9").click();
            } else if (flag == false) {
                $("#btntab7").click();
            }
        }

        return flag;
    }

    $scope.SearchCustomer = function () {
        if ($scope.SearchAccountNo != undefined && $scope.SearchAccountNo != "") {
            var getcustomerdetail = AppService.GetDetailsById("Loan", "GetCustomerPersonalDetailByAccountIdForGroupLoan", $scope.SearchAccountNo);
            getcustomerdetail.then(function (p1) {
                if (p1.data) {
                    var isnotExist = true;
                    for (var i = 0; i < $scope.AccountHolderList.length; i++) {
                        if ($scope.AccountHolderList[i].AccountNumber === p1.data.AccountNumber) {
                            isnotExist = false;
                        }
                    }
                    if (isnotExist) {
                        var IndLoanAmount = 0;
                        if ($scope.GrpLoan.GroupLoanAmount != undefined && $scope.GrpLoan.GroupLoanAmount != "") {
                            var TotalLen = $scope.AccountHolderList.length + 1;
                            IndLoanAmount = $scope.GrpLoan.GroupLoanAmount / TotalLen;
                        }
                        angular.forEach($scope.AccountHolderList, function (value, index) {
                            value.LoanAmount = IndLoanAmount.toFixed();
                        });

                        var obj = new Object();
                        obj.ClientId = p1.data.ClientId;
                        obj.CustomerId = p1.data.CustomerId;
                        obj.CustomerName = p1.data.CustomerName.join(", ");
                        obj.Referencertype = 4;
                        obj.AccountNumber = p1.data.AccountNumber;
                        obj.PersonalId = p1.data.PersonalId.join();
                        obj.LoanAmount = IndLoanAmount.toFixed();
                        obj.LoanStatus = 1;
                        $scope.AccountHolderList.push(obj);
                        $("#btnAllLoanChagr").addClass("disabled");
                    }
                    else {
                        showToastMsg(3, 'Sorry !!! But Member Already Exist');
                    }
                }
                else {
                    showToastMsg(3, 'Enter Saving Account Number')
                }
            }, function (err) {
                showToastMsg(3, 'Error in getting data')
            });
        }
        else {
            showToastMsg(3, 'Please enter saving account number.');
        }
    }

    $scope.GetInterestRate = function (productId) {
        if (productId != "") {
            $scope.GrpLoan.InterestRate = $filter('filter')($scope.Productlist, { ProductId: productId })[0].InterestRate;
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        }
    }

    //$scope.ChangeServiceTaxAmount = function (data) {
    //    var HolderData = $filter('filter')($scope.AccountHolderList, { ClientId: data.ClientId })[0];
    //    if (HolderData.ProcessingCharge != "" && HolderData.ProcessingCharge != "") {
    //        HolderData.ServiceAmount = parseFloat(HolderData.ProcessingCharge) * parseFloat(data.ServiceTax) / 100;
    //    }
    //    if (HolderData.LoanAmount != undefined && HolderData.LoanAmount != "") {
    //        HolderData.TotalLoanAmount = parseFloat(HolderData.LoanAmount) + parseFloat(HolderData.ProcessingCharge) + parseFloat(HolderData.ServiceAmount);
    //    }

    //    if (!$scope.$$phase) {
    //        $scope.$apply();
    //    }
    //}

    //$scope.ChangeServiceProcessingAmount = function (data) {
    //    var HolderData = $filter('filter')($scope.AccountHolderList, { ClientId: data.ClientId })[0];
    //    if (HolderData.ServiceTax != "" && HolderData.ServiceTax != "") {
    //        HolderData.ServiceAmount = parseFloat(data.ProcessingCharge) * parseFloat(HolderData.ServiceTax) / 100;
    //    }
    //    if (HolderData.LoanAmount != undefined && HolderData.LoanAmount != "") {
    //        HolderData.TotalLoanAmount = parseFloat(HolderData.LoanAmount) + parseFloat(HolderData.ProcessingCharge) + parseFloat(HolderData.ServiceAmount);
    //    }

    //    if (!$scope.$$phase) {
    //        $scope.$apply();
    //    }
    //}

    $scope.SaveGroupLoanDetails = function () {
        var flag = true;
        flag = ValidateGroupLoan();

        if (flag) {
            if ($scope.AccountHolderList.length != 0) {
                var TotalLoanAmount = 0;
                angular.forEach($scope.AccountHolderList, function (value, index) {
                    TotalLoanAmount += parseFloat(value.LoanAmount);
                })

                if (TotalLoanAmount == parseFloat($scope.GrpLoan.GroupLoanAmount)) {
                    $scope.GrpLoan.DateofApplication = moment(new Date($("#txtGrpdateofapplication").data("DateTimePicker").date())).format('YYYY-MM-DD');
                    $scope.GrpLoan.DateOfCredit = moment(new Date($("#txtGrpdateofCreditapplication").data("DateTimePicker").date())).format('YYYY-MM-DD');
                    $scope.GrpLoan.PreviousLoanCompleted = moment(new Date($("#txtGrpPreviousLoanCompleted").data("DateTimePicker").date())).format('YYYY-MM-DD');
                    $scope.GrpLoan.OpeningDate = moment(new Date($("#txtGrpdateofOpening").data("DateTimePicker").date())).format('YYYY-MM-DD');
                    //$scope.GrpLoan.InstallmentDate = moment(new Date($("#txtGrpdateofInstallment").data("DateTimePicker").date())).format('YYYY-MM-DD');
                    $scope.GrpLoan.BranchId = $scope.UserBranch.BranchId;
                    if ($scope.GrpLoan.GroupLoanId == '0000000-0000-0000-0000-000000000000' || $scope.GrpLoan.GroupLoanId == undefined || $scope.GrpLoan.GroupLoanId == "") {
                        $scope.GrpLoan.CreatedBy = $cookies.getObject('User').UserId;
                    }
                    else {
                        $scope.GrpLoan.ModifiedBy = $cookies.getObject('User').UserId;
                    }

                    $scope.IndividualLoan = [];
                    $scope.BorrowerList = [];

                    angular.forEach($scope.AccountHolderList, function (value, index) {
                        if (value.PersonalId.indexOf(",") !== -1) {
                            var PerIds = value.PersonalId.split(",");
                            var BorId = "";
                            if (value.BorrowerId != undefined) {
                                BorId = value.BorrowerId.split(",");
                            }
                            angular.forEach(PerIds, function (value, i) {
                                var obj = new Object();
                                obj.PersonalDetailId = value;
                                if (BorId != "") {
                                    obj.BorrowerId = BorId[i];
                                } else {
                                    obj.BorrowerId = "";
                                }
                                if (obj.BorrowerId == '0000000-0000-0000-0000-000000000000' || obj.BorrowerId == undefined || obj.BorrowerId == "") {
                                    obj.CreatedBy = $cookies.getObject('User').UserId;
                                    obj.ModifiedBy = $cookies.getObject('User').UserId;
                                    $scope.BorrowerList.push(obj);
                                }
                                else {
                                    obj.ModifiedBy = $cookies.getObject('User').UserId;
                                }
                            })
                        } else {
                            var obj = new Object();
                            obj.PersonalDetailId = value.PersonalId;
                            obj.BorrowerId = value.BorrowerId;
                            if (obj.BorrowerId == '0000000-0000-0000-0000-000000000000' || obj.BorrowerId == undefined) {
                                obj.CreatedBy = $cookies.getObject('User').UserId;
                                obj.ModifiedBy = $cookies.getObject('User').UserId;
                                $scope.BorrowerList.push(obj);
                            }
                            else {
                                obj.ModifiedBy = $cookies.getObject('User').UserId;
                            }
                        }

                        var LoanObj = new Object();
                        LoanObj.LoanId = value.LoanId;
                        LoanObj.CustomerId = value.CustomerId;
                        LoanObj.CustomerProductId = value.CustomerProductId;
                        LoanObj.LoanType = "1245cb34-310e-e711-a94b-a65d96eb6839";
                        if (value.LoanStatus == undefined) {
                            LoanObj.LoanStatus = 1;
                        }
                        LoanObj.DateofApplication = moment(new Date($("#txtGrpdateofapplication").data("DateTimePicker").date())).format('YYYY-MM-DD');
                        LoanObj.LoanAmount = value.LoanAmount;
                        LoanObj.LoanIntrestRate = $scope.GrpLoan.InterestRate;
                        //LoanObj.ProcessingCharge = value.ProcessingCharge;
                        //LoanObj.ServiceTax = value.ServiceTax;
                        LoanObj.LoanId = value.LoanId;
                        if (LoanObj.LoanId == '0000000-0000-0000-0000-000000000000' || LoanObj.LoanId == undefined) {
                            LoanObj.CreatedBy = $cookies.getObject('User').UserId;
                            LoanObj.ModifiedBy = $cookies.getObject('User').UserId;
                            $scope.IndividualLoan.push(LoanObj);
                        }
                        else {
                            LoanObj.ModifiedBy = $cookies.getObject('User').UserId;
                            $scope.IndividualLoan.push(LoanObj);
                        }
                    });

                    var NewObj = new Object();
                    NewObj.GroupLoan = $scope.GrpLoan;
                    NewObj.Loan = $scope.IndividualLoan;
                    NewObj.Borrower = $scope.BorrowerList;

                    var saveloan = AppService.SaveData("Loan", "SaveGroupLoan", NewObj)
                    saveloan.then(function (p1) {
                        if (p1.data != null) {
                            $scope.GrpLoan.GroupLoanId = p1.data;
                            showToastMsg(1, "Loan Application Saved Successfully");
                            if ($location.search().GroupLoanId == undefined) {
                                window.location = "/App/GroupLoan?GroupLoanId=" + $scope.GrpLoan.GroupLoanId;
                            } else {
                                GetGroupLoandetail($scope.GrpLoan.GroupLoanId);
                            }
                        }
                        else {
                            showToastMsg(3, 'Error in Saving Data')
                        }
                    });
                } else {
                    showToastMsg(3, "Distributed amount is not equal to necessary loan amount.");
                }
            } else {
                showToastMsg(3, "Please add member.");
            }
        }
        else {
            return false;
        }
    }

    $scope.RemoveMember = function (data, index) {
        if (data.LoanId != undefined) {
            var LnObj = new Object();
            LnObj.LoanId = data.LoanId;
            LnObj.CustomerProductId = data.CustomerProductId;
            var deleteborrow = AppService.DeleteMultipleData("Loan", "DeleteMember", LnObj);
            deleteborrow.then(function (p1) {
                if (p1.data != null) {
                    $scope.AccountHolderList.splice(index, 1);
                    showToastMsg(1, "Member Delete Successfully");
                }
                else {
                    showToastMsg(3, "Error in Delete Data");
                }
            });
        } else {
            $scope.AccountHolderList.splice(index, 1);
        }
    }

    function ValidateGroupLoan() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtGrpdateofapplication"), 'Date of Application required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtGrpGroupName"), 'Group name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlProductNamelist"), 'Product name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredFieldInputField($("#txtempname"), 'Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtempcode"), 'Code required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredFieldInputField($("#txtAgentname"), 'Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtagentcode"), 'Code required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtGrpdateofOpening"), 'Opening date required', 'after')) {
            flag = false;
        }
        //if (!ValidateRequiredField($("#txtGrpdateofInstallment"), 'Installment date required', 'after')) {
        //    flag = false;
        //}
        if (!ValidateRequiredField($("#txtGrpInterestRate"), 'Interest rate required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtamount"), 'Amount required', 'after')) {
            flag = false;
        }
        if ($scope.GrpLoan.PreviouslyBorrowed == true) {
            if (!ValidateRequiredField($("#txtGrpPreviousLoanAmount"), 'Previous loan amount required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtGrpPreviousLoanCompleted"), 'Loan comleted date required', 'after')) {
                flag = false;
            }
        }

        angular.forEach($scope.AccountHolderList, function (data) {
            if (!ValidateRequiredField($("#Amt_" + data.ClientId), 'Amount required', 'after')) {
                flag = false;
            } else if ($("#Amt_" + data.ClientId).val() == 0) {
                $("#Amt_" + data.ClientId).closest('.form-group').addClass('has-error');
                PrintMessage($("#Amt_" + data.ClientId), 'Amount is not 0', 'after');
            }
        })

        return flag;
    }

    $scope.Files = [];

    //Document Upload Function
    $scope.getTheFiles = function ($files) {

        angular.forEach($files, function (value, key) {
            var sizeInMB = (value.size / (1024 * 1024)).toFixed(2);
            if (sizeInMB < 10) {
                var obj = new Object();
                obj.DocumentName = value.name;
                obj.File = value;
                obj.LoanId = $scope.PersonalLoan.LoanId;
                obj.IsDelete = false;
                $scope.Files.push(obj);
            }
            else {
                showToastMsg("Error", 3, "File size should be less than 10 MB.");
                return false;
            }

        });
    };

    $scope.UploadDocuments = function () {
        if ($scope.Files.length > 0) {
            var fd = new FormData();
            angular.forEach($scope.Files, function (value, index) {
                fd.append("file", value.File);
            });
            fd.append("data", JSON.stringify($scope.Files));

            var uploadDocs = AppService.UploadDocumentwithData("Loan", "UploadLoanDocuments", fd)
            uploadDocs.then(function (p1) {
                if (p1.data != null) {
                    showToastMsg(1, "Document Uploaded Successfully")
                    $("#loanDocs").val("");
                    $scope.Files = [];
                    GetLoanDocumentList();
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving Data')
            });
        } else {
            showToastMsg(3, 'Please select document for upload.')
        }
    }

    $scope.DeleteDocuments = function (loanDocumentId) {
        var deletedoc = AppService.DeleteData("Loan", "DeleteDocuments", loanDocumentId)
        deletedoc.then(function (p1) {
            if (p1.data != null) {
                GetLoanDocumentList();
            }
            else {
                showToastMsg(3, 'Error in saving data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in Saving Data')
        });
    }

    $scope.LoanDocuments = [];
    function GetLoanDocumentList() {
        var getdoc = AppService.GetDetailsById("Loan", "GetLoanDocumentList", $scope.PersonalLoan.LoanId)
        getdoc.then(function (p1) {
            if (p1.data != null) {
                $scope.LoanDocuments = p1.data;
            }
            else {
                showToastMsg(3, 'Error in saving data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in Saving Data')
        });
    }

    function GetChargesList() {
        var getcharges = AppService.GetDetailsById("Loan", "GetChargesList", $scope.PersonalLoan.LoanId)
        getcharges.then(function (p1) {
            if (p1.data != null) {
                $scope.LoanCharges = p1.data;
            }
            else {
                showToastMsg(3, 'Error in saving data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in Saving Data')
        });
    }

    //$scope.SaveLoanCharges = function () {

    //    angular.forEach($scope.LoanCharges, function (value, index) {
    //        value.LoanId = $scope.PersonalLoan.LoanId
    //    });

    //    var saveLoanCharges = AppService.SaveData("Loan", "SaveLoanCharges", $scope.LoanCharges)
    //    saveLoanCharges.then(function (p1) {
    //        if (p1.data != null) {
    //            $scope.LoanCharges = p1.data;
    //            LoanAmountisation();
    //        }
    //        else {
    //            showToastMsg(3, 'Error in saving data')
    //        }
    //    }, function (err) {
    //        showToastMsg(3, 'Error in Saving Data')
    //    });

    //}

    $scope.AddCharges = function () {
        var abc = { Name: "", Value: "", LoanId: '00000000-0000-0000-0000-000000000000', ChargesId: '00000000-0000-0000-0000-000000000000', LoanChargesId: '00000000-0000-0000-0000-000000000000', IsDelete: false }
        $scope.LoanCharges.push(abc);
    }

    $scope.RemoveCharges = function ($index) {

        var data = $scope.LoanCharges[$index];
        if (data.LoanChargesId != '00000000-0000-0000-0000-000000000000') {
            $scope.LoanCharges[$index].IsDelete = true;

            var deletecharges = AppService.DeleteData("Loan", "DeleteCharges", $scope.LoanCharges[$index].LoanChargesId);
            deletecharges.then(function (p1) {
                if (p1.data != null) {
                    GetChargesList();
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving Data')
            });
        }
        else {
            $scope.LoanCharges.splice($index, 1);
        }
    }

    function LoanAmountisation() {

        $scope.TotalCharges = 0;
        angular.forEach($scope.LoanCharges, function (item) {
            if (!item.IsDelete && item.Value != null && item.Value != 0) {
                $scope.TotalCharges += item.Value;
            }
        });

        $scope.PrincipalAmount = parseFloat($scope.PersonalLoan.DisbursementAmount) + parseFloat($scope.TotalCharges);

        if (!$scope.PersonalLoan.IsDisbursed) {
            if ($scope.PersonalLoan.LoanIntrestRate != "" && $scope.PersonalLoan.LoanIntrestRate != undefined && $scope.PersonalLoan.DisbursementAmount != "" &&
                $scope.PersonalLoan.DisbursementAmount != undefined && $scope.PersonalLoan.Term != "" && $scope.PersonalLoan.Term != undefined && $("#txtInstallmentDate").val() != "" && $("#txtInstallmentDate").val() != undefined) {

                var obj = new Object()
                obj.PrincipalAmount = $scope.PrincipalAmount;
                obj.LoanIntrestRate = $scope.PersonalLoan.LoanIntrestRate;
                obj.Term = $scope.PersonalLoan.Term;
                obj.LoanId = $scope.PersonalLoan.LoanId;
                obj.InstallmentDate = moment(new Date($("#txtInstallmentDate").data("DateTimePicker").date())).format('YYYY-MM-DD');

                var amountisation = AppService.SaveData("Loan", "LoanAmountisation", obj);
                amountisation.then(function (p1) {
                    if (p1.data != null) {
                        $scope.LoanAmountisation = p1.data.ListLoanAmountisation;;
                    }
                    else {
                        showToastMsg(3, 'Error in saving data')
                    }
                }, function (err) {
                    showToastMsg(3, 'Error in Saving Data')
                });
            }
        }
        else {
            var displayAmountisation = AppService.GetDetailsById("Loan", "DisplayAmountisation", $scope.PersonalLoan.LoanId);
            displayAmountisation.then(function (p1) {
                if (p1.data != null) {
                    $scope.LoanAmountisation = p1.data.ListLoanAmountisation;
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving Data')
            });
        }
    }

    $scope.PrintAmountisation = function () {
        var printContent = document.getElementById('printAmountisation');
        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
        //WinPrint.document.write("<style>");
        //WinPrint.document.write("#tblholderdetail {padding-top:0px;padding-bottom:100px;} </style>");

        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        WinPrint.close();
    };

    $scope.PrintAmountisation = function () {
        var printContent = document.getElementById('printAmountisation');
        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        WinPrint.close();
    };

    $scope.OpenModalDisbursementLetter = function () {
        var datadisbursement = AppService.GetDetailsById("Loan", "DisbursementLetter", $scope.PersonalLoan.LoanId);
        datadisbursement.then(function (p1) {
            if (p1.data != null) {
                $scope.DisbursementLetter = p1.data;
                $scope.DisbursementLetter.LoanDetail.AmountInWords = inWords($scope.DisbursementLetter.LoanDetail.TotalLoanAmount)

                $("#divDisbursementLetter").modal('show')
            }
            else {
                showToastMsg(3, 'Error while getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in Saving Data')
        });
    }

    $scope.PrintDisbursementLetter = function () {
        var printContent = document.getElementById('DisbursementLetter');

        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
        WinPrint.document.write("<style>");
        WinPrint.document.write("#tbldetail td {border: 1px solid black;}#borrowerdetail td {border: none;}#loancharges td {border: 1px solid black;} </style>");
        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        WinPrint.close();
    }

    var a = ['', 'one ', 'two ', 'three ', 'four ', 'five ', 'six ', 'seven ', 'eight ', 'nine ', 'ten ', 'eleven ', 'twelve ', 'thirteen ', 'fourteen ', 'fifteen ', 'sixteen ', 'seventeen ', 'eighteen ', 'nineteen '];
    var b = ['', '', 'twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];

    function inWords(num) {
        if ((num = num.toString()).length > 9) return 'overflow';
        n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
        if (!n) return; var str = '';
        str += (n[1] != 0) ? (a[Number(n[1])] || b[n[1][0]] + ' ' + a[n[1][1]]) + 'crore ' : '';
        str += (n[2] != 0) ? (a[Number(n[2])] || b[n[2][0]] + ' ' + a[n[2][1]]) + 'lakh ' : '';
        str += (n[3] != 0) ? (a[Number(n[3])] || b[n[3][0]] + ' ' + a[n[3][1]]) + 'thousand ' : '';
        str += (n[4] != 0) ? (a[Number(n[4])] || b[n[4][0]] + ' ' + a[n[4][1]]) + 'hundred ' : '';
        str += (n[5] != 0) ? ((str != '') ? 'and ' : '') + (a[Number(n[5])] || b[n[5][0]] + ' ' + a[n[5][1]]) + 'only ' : '';
        return str;
    }

    $scope.OpenModalDisbursementAmount = function () {
        $scope.IsDisbursement = true;
        var flag = true;
        flag = ValidateCustomerLoan();
        if (flag) {
            $scope.SaveLoanDetails();
            $scope.PersonalLoan.TotalDisbursementAmount = $scope.PersonalLoan.DisbursementAmount;
            $("#DisbursementAmount .help-block").remove();
            $('#DisbursementAmount .form-group').removeClass('has-error');
            $('#DisbursementAmount .form-group').removeClass('help-block-error');

            $("#DisbursementAmount").modal('show')
        }
    }

    function ValidateDisbursementAmountForm() {

        var flag = true;

        $("#DisbursementAmount .help-block").remove();
        $('#DisbursementAmount .form-group').removeClass('has-error');
        $('#DisbursementAmount .form-group').removeClass('help-block-error');


        if (!ValidateRequiredField($("#ddlThrough"), 'please select the type', 'after')) {
            flag = false;
        }

        if ($scope.PersonalLoan.DisburseThrough == 2) {
            if (!ValidateRequiredField($("#txtChequeDate"), 'Cheque Date required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtBankName"), 'Bank Name required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtChequeNo"), 'Cheque No required', 'after')) {
                flag = false;
            }
        }

        return flag;
    }

    $scope.SaveDisbursementAmount = function () {
        var flag = true;
        flag = ValidateDisbursementAmountForm();
        if (flag) {
            $(':focus').blur();
            $scope.PersonalLoan.DisbursementBy = getUserdata.UserId;
            if ($scope.PersonalLoan.DisburseThrough == 2) {
                $scope.PersonalLoan.ChequeDate = moment(new Date($("#txtChequeDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            else {
                $scope.PersonalLoan.ChequeDate = null
                $scope.PersonalLoan.BankName = null;
                $scope.PersonalLoan.ChequeNo = null;
            }

            var datadisbursement = AppService.SaveData("Loan", "SaveDisbursementAmount", $scope.PersonalLoan);
            datadisbursement.then(function (p1) {
                if (p1.data) {
                    $scope.PersonalLoan.IsDisbursed = true;
                    $scope.UpdateStatus.LoanStatus = "7";
                    $scope.PersonalLoan.LoanStatus = 7
                    GetLoanStatusList($scope.PersonalLoan.LoanId)
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                    showToastMsg(1, 'You have successfully disbursement of loan.')
                    $("#DisbursementAmount").modal('hide')
                }
                else {
                    showToastMsg(3, 'Error while saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving Data')
            });
        }
    }

    function GetLoanStatusList(loanId) {
        var statuslist = AppService.GetDetailsById("Loan", "GetLoanStatusList", loanId);
        statuslist.then(function (p2) {
            if (p2.data) {
                $scope.LoanStatusList = p2.data;
                showToastMsg(1, 'Status Updated successfully.')
            }
        }, function (err) {
            showToastMsg(3, 'Error in Saving data')
        });
    }

    $scope.AddLoanCharges = function () {
        var abc = { Name: "", Value: "", LoanId: '00000000-0000-0000-0000-000000000000', ChargesId: '00000000-0000-0000-0000-000000000000', LoanChargesId: '00000000-0000-0000-0000-000000000000', IsDelete: false }
        $scope.AllLoanCharges.push(abc);
    }

    $scope.OpenLoanChargesModal = function () {
        $("#LoanCharges").modal("show");
        $scope.AllLoanCharges = [];
        var getbranch = AppService.GetDetailsWithoutId("Loan", "GetAllCharges");
        getbranch.then(function (p1) {
            if (p1.data != null) {
                $scope.AllLoanCharges = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.SaveAllLoanCharges = function () {
        var LoanIds = [];
        angular.forEach($scope.AccountHolderList, function (value) {
            if (value.LoanId != undefined) {
                LoanIds.push(value.LoanId);
            }
        })
        $(':focus').blur();

        if ($scope.NoOfSharesAll != undefined) {
            var sharedata = $filter('filter')($scope.AllLoanCharges, { Name: 'Share' })[0];
            if (sharedata == undefined) {
                var NewObj = new Object();
                NewObj.Name = "Share";
                NewObj.Value = $scope.TotalValueAll;
                NewObj.NoOfItem = $scope.NoOfSharesAll;
                $scope.AllLoanCharges.push(NewObj);
            }
            else {
                sharedata.Value = $scope.TotalValueAll;
                sharedata.NoOfItem = $scope.NoOfSharesAll;
            }
        }

        var NewObj = new Object();
        NewObj.LoanIds = LoanIds;
        NewObj.Charges = $scope.AllLoanCharges;

        AppService.SaveData("Loan", "SaveAllLoanCharges", NewObj).then(function (p1) {
            if (p1.data) {
                $scope.AllLoanCharges = [];
                $("#LoanCharges").modal("hide");
                showToastMsg(1, 'Charges saved successfully.')
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });

    }

    $scope.ChangeShareValueAll = function (value) {
        $scope.TotalValueAll = 0;
        $scope.TotalValueAll = value * 100;
        if (!$scope.$$phase) {
            $scope.$apply();
        }
    }

    $scope.RemoveAllCharges = function ($index) {
        $scope.AllLoanCharges.splice($index, 1);
    }

    $scope.ClearLoanModal = function () {
        $scope.PersonalLoan = {};
        $scope.UpdateStatus = {};
        $scope.DocumentShown = 0;
        $scope.Files = [];
        $scope.LoanCharges = [];
        $scope.LoanDocuments = [];
        $scope.LoanAmountisation = [];
        $scope.CustomerList = {};
        $scope.TotalCharges = {};
        $scope.PrincipalAmount = {};
        $("#tab7 a").click();

        if (!$scope.$$phase) {
            $scope.$apply();
        }
    }

    $scope.CheckAllCheckbox = function () {
        //$('#tblCustomerpersonal tbody tr td input[type="checkbox"]').not('[disabled=disabled]').prop('checked', $scope.CheckAll).iCheck('update');
        angular.forEach($scope.AccountHolderList, function (value, index) {
            if (value.LoanStatus == 2 || value.LoanStatus == 3) {
                value.Check = $scope.CheckAll
            }

        })

        if (!$scope.$$phase) {
            $scope.$apply();
        }
    }

    $scope.OpenApproveGroupLoanModal = function () {
        $("#ApproveGroupLoan").modal('show');
    }

    $scope.ApproveGroupLoan = function () {
        var approveLoanId = []

        angular.forEach($scope.AccountHolderList, function (value, index) {
            if (value.Check) {
                approveLoanId.push(value.LoanId);
            }
        })

        var obj = new Object()

        obj.LoanId = approveLoanId;
        obj.Comment = $scope.ApproveComment;

        if (approveLoanId.length > 0) {
            AppService.SaveData("Loan", "ApproveGroupLoan", obj).then(function (p1) {
                if (p1.data) {
                    $("#ApproveGroupLoan").modal('hide');
                    showToastMsg(1, 'Loans are Approved.')
                    GetGroupLoandetail($scope.GrpLoan.GroupLoanId)
                    $scope.ApproveComment = "";
                    $scope.ApproveStatus = "";
                    approveLoanId = []
                }
                else {
                    showToastMsg(3, 'Error in getting data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in getting data')
            });
        }
    }
});