angular.module("BSCCL").controller('CalculatorController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $timeout) {

    $(".datepicker").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
        //minDate: new Date()
    });

    GetProductListForLoan();

    function GetProductListForLoan() {
        var Promis = AppService.GetDetailsById("CustomerProduct", "GetProductNameAsSelectedType", 5);
        Promis.then(function (p1) {
            if (p1.data != null) {
                $scope.LoanProductlist = [];
                $scope.LoanProductlist = p1.data;
            }
            else {
                showToastMsg(3, 'No Product Avilable')
            }
        })
    }

    $scope.GetProductListByType = function (ProducttypeId) {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var Promis = AppService.GetDetailsById("CustomerProduct", "GetProductNameAsSelectedType", ProducttypeId);
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

    $scope.GetProductDetailsAsSelectedName = function (ProductNameId) {
        var Promis = AppService.GetDetailsById("CustomerProduct", "GetProductDetailsAsSelectedName", ProductNameId);
        Promis.then(function (p1) {
            if (p1.data != null) {
                $scope.Calculator.InterestRate = p1.data.InterestRate;
                $scope.Calculator.InterestType = p1.data.InterestType + "";
                $scope.Calculator.PaymentType = p1.data.PaymentType + "";
                $scope.Calculator.FrequencyType = p1.data.Frequency + "";
                $scope.Calculator.LatePaymentFees = p1.data.LatePaymentFees;

            } else {
                showToastMsg(3, 'No Product Avilable')
            }
        });
    }

    $scope.TimePeriodChange = function () {

        if ($scope.Calculator.TimePeriod != null && $scope.Calculator.TimePeriod != undefined && $scope.Calculator.TimePeriod != ""
            && $scope.Calculator.NoOfMonthsORYears != "" && $scope.Calculator.NoOfMonthsORYears != undefined && $scope.Calculator.NoOfMonthsORYears != null) {

            if ($scope.Calculator.TimePeriod == "1") {
                var OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
                var date1 = new Date(OpeningDate);
                date1.setMonth(date1.getMonth() + parseInt($("#txtNoOfMonthsORYears").val()));

                $("#txtMaturityDate").data("DateTimePicker").date($filter('date')(date1, 'dd/MM/yyyy'))
            }
            else if ($scope.Calculator.TimePeriod == "3") {
                var OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
                var date1 = new Date(OpeningDate);
                date1.setDate(date1.getDate() + (parseInt($("#txtNoOfMonthsORYears").val())))

                $("#txtMaturityDate").data("DateTimePicker").date($filter('date')(date1, 'dd/MM/yyyy'))

            }
            else {
                var OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
                var date = new Date(OpeningDate).setFullYear(new Date(OpeningDate).getFullYear() + parseInt($("#txtNoOfMonthsORYears").val()));
                if (date !== NaN) {
                    $("#txtMaturityDate").data("DateTimePicker").date($filter('date')(new Date(date), 'dd/MM/yyyy'))
                }
            }
        }
    }

    $scope.CheckInterstrateinTerm = function () {

        if ($scope.Calculator.TimePeriod != null && $scope.Calculator.TimePeriod != undefined && $scope.Calculator.TimePeriod != ""
            && $scope.Calculator.NoOfMonthsORYears != "" && $scope.Calculator.NoOfMonthsORYears != undefined && $scope.Calculator.NoOfMonthsORYears != null) {

            var Id1 = $scope.Calculator.NoOfMonthsORYears;
            var Id2 = $scope.Calculator.TimePeriod;

            if ((Id1 != null && Id1 != undefined && Id1 != "") || (Id2 != null && Id2 != undefined && Id2 != "")) {
                if ($scope.Calculator.TimePeriod == "1") {
                    Id1 = (Id1) * 30;

                }
                else if ($scope.Calculator.TimePeriod == "2") {
                    Id1 = (Id1) * 365;
                }
            }

            var CheckInterest = AppService.GetDataByQuerystring("CustomerProduct", "GetInterestRate", Id1, Id2)
            CheckInterest.then(function (p1) {

                if (p1.data != 0) {
                    $scope.Calculator.InterestRate = p1.data;
                    if ($scope.Calculator.InterestRate != "" && $scope.Calculator.InterestRate != 0 && $scope.Calculator.InterestRate != undefined && $scope.Calculator.InterestRate != null && $scope.Calculator.InterestRate != "0") {
                        $scope.CalculateMaturityAmount();
                    }
                    else {
                        $scope.Calculator.MaturityAmount = "";
                    }
                }
                else {
                    $scope.Calculator.InterestRate = "";
                    showToastMsg(3, 'No Interest Rate Found For This Term');
                }
            })
        }
    }

    $scope.CalculateMaturityAmount = function () {
        if ($scope.Calculator.ProductType == "3" || $scope.Calculator.ProductType == "4" || $scope.Calculator.ProductType == "6") {

            if ($scope.Calculator.InterestRate != "" && $scope.Calculator.InterestRate != undefined && $scope.Calculator.Amount != "" &&
                $scope.Calculator.NoOfMonthsORYears != "" && $scope.Calculator.NoOfMonthsORYears != undefined && $scope.Calculator.TimePeriod != "" &&
                $scope.Calculator.TimePeriod != undefined && $("#txtOpeningDateCP").val() != "" && $("#txtOpeningDateCP").val() != undefined
                && $("#txtMaturityDate").val() != "" && $("#txtMaturityDate").val() != undefined) {


                var obj = new Object()
                obj.ProductType = $scope.Calculator.ProductType;
                // GetProductDetail(obj.ProductType);
                if ($("#txtOpeningDateCP").val() != null && $("#txtOpeningDateCP").val() != "") {
                    obj.OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
                }
                if ($("#txtMaturityDate").val() != null && $("#txtMaturityDate").val() != "") {
                    obj.MaturityDate = moment(new Date($("#txtMaturityDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                }

                obj.InterestType = $filter('filter')($scope.Productlist, { ProductId: $scope.Calculator.ProductId })[0].Frequency;
                obj.InterestRate = $scope.Calculator.InterestRate;
                if ($("#txtDueDate").val() != null && $("#txtDueDate").val() != "") {
                    obj.DueDate = moment(new Date($("#txtDueDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                }
                obj.PaymentType = $scope.Calculator.PaymentType;
                obj.Amount = $scope.Calculator.Amount;
                obj.TimePeriod = $scope.Calculator.TimePeriod;
                obj.NoOfMonthsORYears = $scope.Calculator.NoOfMonthsORYears;

                if ($("#txtOpeningDateCP").val() != null && $("#txtOpeningDateCP").val() != "" && $("#txtMaturityDate").val() != null && $("#txtMaturityDate").val() != "" &&
                    $scope.Calculator.InterestRate != "" && $scope.Calculator.InterestRate != 0 && $scope.Calculator.Amount != "" && $scope.Calculator.Amount != 0) {

                    var calculateMaturityAmount = AppService.SaveData("CustomerProduct", "CalculateMaturityAmount", obj)
                    calculateMaturityAmount.then(function (p1) {
                        if (p1.data != null) {
                            $scope.Calculator.MaturityAmount = p1.data;
                        }
                        else {
                            showToastMsg(3, 'Error in Saving Data')
                        }
                    }, function (err) {
                        showToastMsg(3, 'Error while calculation maturity amount.')
                    });
                }
            }
        }
        else if ($scope.Calculator.ProductType == "7") {
            $scope.Calculator.MaturityAmount = $scope.Calculator.Amount;
        }
        var PaymentType = $("#ddlPaymentTypeCP").val()
        var OpeningDate = new Date($("#txtOpeningDateCP").data("DateTimePicker").date());


        if (!$scope.IsDueDateChanged) {
            if (($("#txtOpeningDateCP").val() != "")) {
                if (PaymentType == "1") {
                    OpeningDate.setDate(OpeningDate.getDate() + 1);
                    //var date = moment(new Date(OpeningDate)).format('DD/MM/YYYY');
                    //$("#txtDueDate").val(date);
                    $("#txtDueDate").data("DateTimePicker").date($filter('date')(OpeningDate, 'dd/MM/yyyy'))


                }
                else if (PaymentType == "2") {
                    OpeningDate.setMonth(OpeningDate.getMonth() + 1);
                    //var date = moment(new Date(OpeningDate)).format('DD/MM/YYYY');
                    //$("#txtDueDate").val(date);
                    $("#txtDueDate").data("DateTimePicker").date($filter('date')(OpeningDate, 'dd/MM/yyyy'))


                }
                else if (PaymentType == "3") {
                    OpeningDate.setMonth(OpeningDate.getMonth() + 3);
                    //var date = moment(new Date(OpeningDate)).format('DD/MM/YYYY');
                    //$("#txtDueDate").val(date);
                    $("#txtDueDate").data("DateTimePicker").date($filter('date')(OpeningDate, 'dd/MM/yyyy'))

                }
                else if (PaymentType == "4") {
                    OpeningDate.setMonth(OpeningDate.getMonth() + 6);
                    var date = moment(new Date(OpeningDate)).format('DD/MM/YYYY');
                    $("#txtDueDate").val(date);

                }
                else if (PaymentType == "5") {
                    OpeningDate.setMonth(OpeningDate.getMonth() + 12);
                    //var date = moment(new Date(OpeningDate)).format('DD/MM/YYYY');
                    //$("#txtDueDate").val(date);
                    $("#txtDueDate").data("DateTimePicker").date($filter('date')(OpeningDate, 'dd/MM/yyyy'))

                }
            }

        }


    }

    $("#txtOpeningDateCP").on("dp.change", function () {
        $(this).trigger('change');
        $scope.TimePeriodChange();

        if ($scope.Calculator.ProductType == "3" || $scope.Calculator.ProductType == "4" || $scope.Calculator.ProductType == "6") {
            $scope.CalculateMaturityAmount();
        }
        else if ($scope.Calculator.ProductType == "7") {
            $scope.Calculator.MaturityAmount = $scope.Calculator.Amount;
        }
    });

    $scope.IsDueDateChanged = false;
    $("#txtDueDate").on("dp.change", function () {
        $scope.IsDueDateChanged = true;
        $(this).trigger('change');
        $scope.TimePeriodChange();

        if ($scope.Calculator.ProductType == "3" || $scope.Calculator.ProductType == "4" || $scope.Calculator.ProductType == "6") {
            $scope.CalculateMaturityAmount();
        }
        else if ($scope.Calculator.ProductType == "7") {
            $scope.Calculator.MaturityAmount = $scope.Calculator.Amount;
        }
    });

    $scope.Calculate = function () {
        var flag = true;

        flag = ValidateCalculateForm();
        if (flag) {
            if ($scope.Calculator.ProductType == "3" || $scope.Calculator.ProductType == "4" || $scope.Calculator.ProductType == "6") {
                $scope.CalculateMaturityAmount();
            }
            else if ($scope.Calculator.ProductType == "7") {
                $scope.Calculator.MaturityAmount = $scope.Calculator.Amount;
            }
        }
    }

    $scope.ClearCalculateForm = function () {
        $scope.Calculator = {};
    }

    $scope.CalculateLoanAmountisation = function () {

        var flag = true;

        flag = ValidateLoanCalculatorForm();
        if (flag) {

            if ($scope.LoanCalculator.LoanIntrestRate != "" && $scope.LoanCalculator.LoanIntrestRate != undefined && $scope.LoanCalculator.PrincipalAmount != "" &&
                $scope.LoanCalculator.Term != "" && $scope.LoanCalculator.Term != undefined && $("#txtLoanInstallmentDate").val() != "" && $("#txtLoanInstallmentDate").val() != undefined) {

                var obj = new Object()
                obj.PrincipalAmount = $scope.LoanCalculator.PrincipalAmount;
                obj.LoanIntrestRate = $scope.LoanCalculator.LoanIntrestRate;
                obj.Term = $scope.LoanCalculator.Term;
                obj.InstallmentDate = moment(new Date($("#txtLoanInstallmentDate").data("DateTimePicker").date())).format('YYYY-MM-DD');

                var amountisation = AppService.SaveData("Loan", "LoanAmountisation", obj);
                amountisation.then(function (p1) {
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
    }

    $scope.ClearLoanCalculateForm = function () {
        $scope.LoanAmountisation = [];
        $scope.LoanCalculator = {}
        $scope.RIPList = [];
    }

    function ValidateLoanCalculatorForm() {
        var flag = true
        if (!ValidateRequiredField($("#ddlLoanProductNamelist"), 'Product Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txLoantAmount"), 'Loan Amount Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtLoanInterestRate"), 'Interest Rate Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txLoanAmount"), 'Loan Amount Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtLoanInstallmentDate"), 'Installmentdate Required', 'after')) {
            flag = false;
        }
        return flag
    }

    $scope.GetLoanDetailsAsSelectedName = function (ProductNameId) {
        var Promis = AppService.GetDetailsById("CustomerProduct", "GetProductDetailsAsSelectedName", ProductNameId);
        Promis.then(function (p1) {
            if (p1.data != null) {
                if (p1.data.InterestRate != null && p1.data.InterestRate != "" && p1.data.InterestRate > 0 && p1.data.InterestRate != undefined)
                    $scope.LoanCalculator.InterestRate = p1.data.InterestRate;

            } else {
                showToastMsg(3, 'No Product Avilable')
            }
        });
    }

    function ValidateCalculateForm() {

        var flag = true;
        if (!ValidateRequiredField($("#ddlProductTypelist"), 'Product Type required', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#ddlProductNamelist"), 'Product Type Name required', 'after')) {
            flag = false;
        }

        if ($scope.Productlist != 0) {
            if (!ValidateRequiredField($("#txtInterestRateCP"), 'Interest Rate required', 'after')) {
                flag = false;
            }

            if (!ValidateRequiredField($("#txtOpeningDateCP"), 'Date Required', 'after')) {
                flag = false;
            }

            if (!ValidateRequiredField($("#ddlTimePeriod"), 'Enter Time Period ', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtNoOfMonthsORYears"), 'Enter Months/Years', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtMaturityDate"), 'Enter Maturity Date', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtAmount"), 'Enter Amount', 'after')) {
                flag = false;
            }

            if ($scope.Calculator.ProductType != undefined && $scope.Calculator.ProductType != "" && ($scope.Calculator.ProductType == "6" || $scope.Calculator.ProductType == "7")) {
                if (!ValidateRequiredField($("#txtDueDate"), 'Installment Date Required', 'after')) {
                    flag = false;
                }

            }

        }
        return flag;
    }

    $scope.OpenPrintModal = function () {
        var flag = true;

        flag = ValidateCalculateForm();
        if (flag) {
            if ($scope.Calculator.ProductType == "3" || $scope.Calculator.ProductType == "4" || $scope.Calculator.ProductType == "6") {
                $scope.CalculateMaturityAmount();
            }
            else if ($scope.Calculator.ProductType == "7") {
                $scope.Calculator.MaturityAmount = $scope.Calculator.Amount;
            }


            if ($("#txtOpeningDateCP").val() != null && $("#txtOpeningDateCP").val() != "") {
                $scope.Calculator.OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('DD-MM-YYYY');
            }

            if ($("#txtMaturityDate").val() != null && $("#txtMaturityDate").val() != "") {
                $scope.Calculator.MaturityDate = moment(new Date($("#txtMaturityDate").data("DateTimePicker").date())).format('DD-MM-YYYY');
            }

            if ($("#txtDueDate").val() != null && $("#txtDueDate").val() != "") {
                $scope.Calculator.DueDate = moment(new Date($("#txtDueDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }

            if ($scope.Calculator.ProductType != undefined && $scope.Calculator.ProductType != 3) {
                $scope.Calculator.PaymentTypeName = $("#ddlPaymentTypeCP option:selected").text();
            }


            $scope.Calculator.TimePeriodName = $("#ddlTimePeriod option:selected").text();

            if ($scope.Calculator.ProductType != undefined && ($scope.Calculator.ProductType == 6 || $scope.Calculator.ProductType == 7)) {

                var obj = new Object()
                obj.OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
                obj.MaturityDate = moment(new Date($("#txtMaturityDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                obj.PaymentType = $scope.Calculator.PaymentType
                obj.MaturityAmount = $scope.Calculator.MaturityAmount
                obj.Amount = $scope.Calculator.Amount
                obj.NoofMonthOrYear = $scope.Calculator.NoOfMonthsORYears
                obj.TimePeriod = $scope.Calculator.TimePeriod
                obj.InterestRate = $scope.Calculator.InterestRate
                obj.DueDate = moment(new Date($("#txtDueDate").data("DateTimePicker").date())).format('YYYY-MM-DD');

                var methodname = ''
                if ($scope.Calculator.ProductType == 6) {
                    methodname = 'RIPMaturityCalculation';
                }
                else {
                    methodname = 'MISMaturityCalculation';
                }


                var Promis = AppService.SaveData("CustomerProduct", methodname, obj);
                Promis.then(function (p1) {
                    if (p1.data != null) {
                        $scope.RIPList = p1.data;

                    } else {
                        showToastMsg(3, 'No Product Avilable')
                    }
                });
            }
            $("#PrintModal").modal('show');
        }


    }

    $scope.PrintRIP = function () {
        var printContent = document.getElementById('printProduct');
        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')

        if ($scope.Calculator.ProductType == 6) {
            WinPrint.document.write("<style>");
            WinPrint.document.write("#PlannerTable > tbody > tr > td {text-align:center;}</style>");
        }
        else {

            $('#PlannerTable').hide();
        }
        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        //WinPrint.close();
        $timeout(function () { WinPrint.close(); }, 2000);
    };

    $scope.PrintAmountisation = function () {
        var printContent = document.getElementById('printAmountisation');
        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        //WinPrint.close();
        $timeout(function () { WinPrint.close(); }, 2000);
    };
});