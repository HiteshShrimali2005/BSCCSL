angular.module("BSCCL").controller('CustomerProductController', function ($scope, AppService, $state, $cookies, $filter, $location, $timeout) {

    $scope.UserBranch.ShowBranch = false;
    $scope.UserBranch.Enabled = false;
    //$scope.UserBranch.BranchId = $cookies.get('Branch');

    $scope.today = new Date();

    $scope.CustomerData = {}
    $scope.CustomerProduct = {}
    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');
    $scope.AssigenProductClicked = false;

    var ID = $location.search().CustomerId;

    if ($location.search().CustomerId != undefined) {
        $scope.CustomerId = $location.search().CustomerId;
        GetCustomerByIdForProduct($location.search().CustomerId)
        GetProductList();
    }

    GetLoanType();
    //GetAllEmployeeforProduct();

    $(".datepicker").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
        //minDate: new Date()
    });

    jQuery('txtInterestRateCP').keyup(function () {
        this.value = this.value.replace(/[^0-9\.]/g, '');
    });

    $scope.AssignProduct = function () {
        $scope.AssigenProductClicked = true
    }

    $(document).ready(function () {
        $('#myModal').on('show.bs.modal', function (e) {
            var image = $(e.relatedTarget).attr('src');
            $(".img-responsive").attr("src", image);
        });
    });

    function GetCustomerByIdForProduct(ID) {

        var Promis = AppService.GetDetailsById("CustomerProduct", "GetCustomerByIdForProduct", ID);
        Promis.then(function (p1) {
            if (p1.data != null) {
                $scope.CustomerData = p1.data.CustomerDetails;
                $scope.Customer = p1.data.Customer;

                $scope.CustomerProduct = {};
                $scope.CustomerProduct.AgentName = $scope.Customer.AgentName;
                $scope.CustomerProduct.AgentCode = $scope.Customer.AgentCode;
                $scope.CustomerProduct.AgentId = $scope.Customer.AgentId;

                $scope.CustomerProduct.EmpName = $scope.Customer.EmpName;
                $scope.CustomerProduct.EmpCode = $scope.Customer.EmpCode;
                $scope.CustomerProduct.EmployeeId = $scope.Customer.EmployeeId;

                for (var i = 0; i < $scope.CustomerData.length; i++) {

                    if ($scope.CustomerData[i].HolderPhoto == "" || $scope.CustomerData[i].HolderPhoto == undefined) {
                        $scope.CustomerData[i].HolderPhoto = 'no_image.png';
                        //$scope.HolderData.details[0].Holdersign = 'no_image.png';
                    }
                    else {
                        $scope.CustomerData[i].HolderPhoto = $scope.CustomerData[i].HolderPhoto;
                    }
                }
            }
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
        });
    }

    //Check CustomerAccount Before issue F.D/R.D
    $scope.CheckCustomerAccountExist = function () {

        if ($scope.CustomerProduct.ProductType == 1 || $scope.CustomerProduct.ProductType == 2) {

            var obj = new Object();
            obj.CustomerId = $scope.CustomerId;
            obj.ProductType = $scope.CustomerProduct.ProductType
            obj.ProductId = $scope.CustomerProduct.ProductId

            var accountexist = AppService.SaveData("CustomerProduct", "CheckCustomerAccountExist", obj);
            accountexist.then(function (p1) {
                if (p1.data) {
                    $scope.GetProductDetailsAsSelectedName($scope.CustomerProduct.ProductId)
                }
                else {
                    $('#CustomerProduct').modal('hide');
                    $scope.ClearForm();
                    showToastMsg(3, 'Account Already Exist. You Cannot create  another Account')
                }
            });
        }
        else {
            $scope.GetProductDetailsAsSelectedName($scope.CustomerProduct.ProductId)
        }
    }

    $scope.GetProductNameAsSelectedType = function (ProducttypeId) {
        $scope.Productlist = [];
        GetProductDetail(ProducttypeId);
        if (ProducttypeId == 1 || ProducttypeId == 2) {
            $scope.CustomerProduct.LIType = 0;
        }
        else {
            ReturnSavingAccountNum(ProducttypeId)
        }
    }

    function ReturnSavingAccountNum(ProducttypeId) {

        var Promis = AppService.GetDetailsById("CustomerProduct", "GetSavingAccountNo", $scope.CustomerId);
        Promis.then(function (p1) {
            if (p1.data != null) {
                $scope.SavingAccountNo = p1.data;
            }
            else {
                showToastMsg(3, 'Please Create Saving Account.')
            }
        })
    }

    function GetProductDetail(ProducttypeId) {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var Promis = AppService.GetDetailsById("CustomerProduct", "GetProductNameAsSelectedType", ProducttypeId);
        Promis.then(function (p1) {
            if (p1.data != null) {
                $scope.Productlist = [];
                $scope.Productlist = p1.data;

                if ($scope.CustomerProduct.CustomerProductId != undefined && $scope.CustomerProduct.CustomerProductId != '0000000-0000-0000-0000-000000000000') {
                    $scope.SetDataForEdit();
                }
            }
            else {
                showToastMsg(3, 'No Product Avilable')
            }
        })
    }

    //$scope.GetLoanProductAsPerLoanTypeId = function () {
    //    if ($scope.CustomerProduct.LoanTypeId != null && $scope.CustomerProduct.LoanTypeId != undefined) {
    //        $scope.Productlist = $filter('filter')($scope.Productlist, { LoanTypeId: $scope.CustomerProduct.LoanTypeId })
    //    }
    //}

    $scope.GetProductDetailsAsSelectedName = function (ProductNameId) {

        if (ProductNameId != null) {
            var Promis = AppService.GetDetailsById("CustomerProduct", "GetProductDetailsAsSelectedName", ProductNameId);
            Promis.then(function (p1) {
                if (p1.data != null) {
                    $scope.CustomerProduct.InterestRate = p1.data.InterestRate;
                    $scope.CustomerProduct.InterestType = p1.data.InterestType + "";
                    $scope.CustomerProduct.PaymentType = p1.data.PaymentType + "";
                    $scope.CustomerProduct.FrequencyType = p1.data.Frequency + "";
                    $scope.CustomerProduct.LatePaymentFees = p1.data.LatePaymentFees;
                    //$scope.CustomerProduct.TimePeriod = p1.data.TimePeriod + "";
                    //$scope.CustomerProduct.NoOfMonthsORYears = p1.data.NoOfMonthsORYears;
                    //$("#txtStartDateCP").data("DateTimePicker").date($filter('date')(p1.data.StartDate, 'dd/MM/yyyy'))
                    //$("#txtEndDateCP").data("DateTimePicker").date($filter('date')(p1.data.EndDate, 'dd/MM/yyyy'))

                    if ($scope.CustomerProduct.ProductType == 8)// Set Fixed Term 3 Year
                    {
                        $scope.CustomerProduct.TimePeriod = 2 + "";
                        $("#ddlTimePeriod").attr("disabled", "true");
                        $("#txtNoOfMonthsORYears").attr("readonly", "true");
                        $scope.CustomerProduct.NoOfMonthsORYears = 3;
                        $scope.CustomerProduct.PaymentType = 5 + "";
                        $("#ddlPaymentTypeCP").attr("disabled", "true");
                    }
                    else if ($scope.CustomerProduct.ProductType == 9)
                    {
                        $scope.CustomerProduct.TimePeriod = 1 + "";
                        $("#ddlPaymentTypeCP option[value='1']").remove();
                        $("#ddlTimePeriod").attr("disabled", "true");
                        if ($("#ddlProductNamelist").find(":selected").text() == "Capital Builder 60") {
                            $("#txtNoOfMonthsORYears").attr("readonly", "true");
                            $scope.CustomerProduct.NoOfMonthsORYears = 60;
                        }
                        if ($("#ddlProductNamelist").find(":selected").text() == "Capital Builder 72") {
                            $("#txtNoOfMonthsORYears").attr("readonly", "true");
                            $scope.CustomerProduct.NoOfMonthsORYears = 72;
                        }
                        if ($("#ddlProductNamelist").find(":selected").text() == "Capital Builder 84") {
                            $("#txtNoOfMonthsORYears").attr("readonly", "true");
                            $scope.CustomerProduct.NoOfMonthsORYears = 84;
                        }
                    }
                    else if ($scope.CustomerProduct.ProductType == 10) {
                        $scope.CustomerProduct.TimePeriod = 1 + "";
                        $("#ddlPaymentTypeCP option[value='1']").remove();
                        $("#ddlTimePeriod").attr("disabled", "true");
                        if ($("#ddlProductNamelist").find(":selected").text() == "Wealth Creator 24") {
                            $("#txtNoOfMonthsORYears").attr("readonly", "true");
                            $scope.CustomerProduct.NoOfMonthsORYears = 24;
                        }
                        if ($("#ddlProductNamelist").find(":selected").text() == "Wealth Creator 30") {
                            $("#txtNoOfMonthsORYears").attr("readonly", "true");
                            $scope.CustomerProduct.NoOfMonthsORYears = 30;
                        }
                        if ($("#ddlProductNamelist").find(":selected").text() == "Wealth Creator 36") {
                            $("#txtNoOfMonthsORYears").attr("readonly", "true");
                            $scope.CustomerProduct.NoOfMonthsORYears = 36;
                        }
                        if ($("#ddlProductNamelist").find(":selected").text() == "Wealth Creator 48") {
                            $("#txtNoOfMonthsORYears").attr("readonly", "true");
                            $scope.CustomerProduct.NoOfMonthsORYears = 48;
                        }
                    }
                    else {
                        $scope.CustomerProduct.TimePeriod = "" + "";
                        $("#ddlTimePeriod").removeAttr("disabled");
                        $("#txtNoOfMonthsORYears").removeAttr("readonly");
                        $scope.CustomerProduct.NoOfMonthsORYears = "";
                        $scope.CustomerProduct.PaymentType = "" + "";
                        $("#ddlPaymentTypeCP").removeAttr("disabled");
                    }

                } else {
                    showToastMsg(3, 'No Product Avilable')
                }
            });
        }
    }

    function GetLoanType() {
        var loanType = AppService.GetDetailsWithoutId("Product", "GetLoanType");
        loanType.then(function (p1) {
            if (p1.data != null) {
                $scope.LoanType = p1.data;
            }
            else {
                showToastMsg(3, 'Error in Saving Data')
            }
        })
    }

    $("#txtPreviousOpeningDateCP").on("dp.change", function () {
        $(this).trigger('change');
        var date1 = new Date($("#txtPreviousOpeningDateCP").data("DateTimePicker").date());
        var date2 = new Date();
        var timeDiff = Math.abs(date2.getTime() - date1.getTime());
        var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
        $scope.CustomerProduct.DiffDays = diffDays;
    });

    $("#txtOpeningDateCP").on("dp.change", function () {
        $(this).trigger('change');

        $scope.TimePeriodChange();


        if ($scope.CustomerProduct.ProductType == "3" || $scope.CustomerProduct.ProductType == "4" || $scope.CustomerProduct.ProductType == "6" || $scope.CustomerProduct.ProductType == "8") {
            $scope.CalculateMaturityAmount();
        }
        else if ($scope.CustomerProduct.ProductType == "7") {
            $scope.CustomerProduct.MaturityAmount = $scope.CustomerProduct.Amount;
        }
    });


    $scope.IsDueDateChanged = false;
    $("#txtDueDate").on("dp.change", function () {
        $scope.IsDueDateChanged = true;

        $(this).trigger('change');
        $scope.TimePeriodChange();

        if ($scope.CustomerProduct.ProductType == "3" || $scope.CustomerProduct.ProductType == "4" || $scope.CustomerProduct.ProductType == "6" || $scope.CustomerProduct.ProductType == "8") {
            $scope.CalculateMaturityAmount();
        }
    });

    //jQuery('#txtNoOfMonthsORYears').on('input', function () {
    //    if ($scope.CustomerProduct.ProductType == "3" || $scope.CustomerProduct.ProductType == "4") {

    //        if ($scope.CustomerProduct.TimePeriod == "1") {
    //            var OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
    //            var date1 = new Date(OpeningDate);
    //            date1.setMonth(date1.getMonth() + parseInt($("#txtNoOfMonthsORYears").val()));
    //            $("#txtMaturityDate").data("DateTimePicker").date($filter('date')(date1, 'dd/MM/yyyy'))
    //        }
    //        else if ($scope.CustomerProduct.TimePeriod == "3") {
    //            var OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
    //            var date1 = new Date(OpeningDate);
    //            date1.setDate(date1.getDate() + (parseInt($("#txtNoOfMonthsORYears").val())))

    //            $("#txtMaturityDate").data("DateTimePicker").date($filter('date')(date1, 'dd/MM/yyyy'))
    //        }
    //        else {
    //            var OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
    //            var date = new Date(OpeningDate).setFullYear(new Date(OpeningDate).getFullYear() + parseInt($("#txtNoOfMonthsORYears").val()));
    //            if (date !== NaN) {
    //                $("#txtMaturityDate").data("DateTimePicker").date($filter('date')(new Date(date), 'dd/MM/yyyy'))
    //            }
    //        }
    //    }
    //});

    $scope.TimePeriodChange = function () {
        if (($scope.CustomerProduct.ProductType != "1" && $scope.CustomerProduct.ProductType != "2" || $scope.CustomerProduct.ProductType != "5") &&
            ($scope.CustomerProduct.TimePeriod != null && $scope.CustomerProduct.TimePeriod != undefined && $scope.CustomerProduct.TimePeriod != ""
                && $scope.CustomerProduct.NoOfMonthsORYears != "" && $scope.CustomerProduct.NoOfMonthsORYears != undefined && $scope.CustomerProduct.NoOfMonthsORYears != null)) {

            if ($scope.CustomerProduct.TimePeriod == "1") {
                var OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
                var date1 = new Date(OpeningDate);
                date1.setMonth(date1.getMonth() + parseInt($("#txtNoOfMonthsORYears").val()));
                //date1 = moment(date1).subtract(1, 'days');

                $("#txtMaturityDate").data("DateTimePicker").date($filter('date')(date1, 'dd/MM/yyyy'))
            }
            else if ($scope.CustomerProduct.TimePeriod == "3") {
                var OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
                var date1 = new Date(OpeningDate);
                date1.setDate(date1.getDate() + (parseInt($("#txtNoOfMonthsORYears").val())))
                //date1 = moment(date1).subtract(1, 'days');
                $("#txtMaturityDate").data("DateTimePicker").date($filter('date')(date1, 'dd/MM/yyyy'))
                //var OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
                //var date1 = new Date(OpeningDate);
                //date1.setDate(date1.getDate() + parseInt($("#txtNoOfMonthsORYears").val()));
            }
            else {
                var OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
                var date = new Date(OpeningDate).setFullYear(new Date(OpeningDate).getFullYear() + parseInt($("#txtNoOfMonthsORYears").val()));
                if ($scope.CustomerProduct.ProductType == "8") {
                    date = new Date(OpeningDate).setFullYear(new Date(OpeningDate).getFullYear() + parseInt("5"));
                }
                //date = moment(date1).subtract(1, 'days');
                if (date !== NaN) {
                    $("#txtMaturityDate").data("DateTimePicker").date($filter('date')(new Date(date), 'dd/MM/yyyy'))
                }
            }
        }
    }

    $("#txtGICommencementDate").on("dp.change", function () {
        if ($scope.CustomerProduct.ProductType == "1") {
            $(this).trigger('change');
            var InsuranceDate = moment(new Date($("#txtGICommencementDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            var date1 = new Date(InsuranceDate);
            date1.setYear(date1.getFullYear() + 1);
            $("#txtGIDueDate").data("DateTimePicker").date($filter('date')(date1, 'dd/MM/yyyy'))
        }
    });

    $("#txtLICommencementDate").on("dp.change", function () {
        if ($scope.CustomerProduct.ProductType == "1") {
            $(this).trigger('change');
            var InsuranceDate = moment(new Date($("#txtLICommencementDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            var date1 = new Date(InsuranceDate);
            date1.setYear(date1.getFullYear() + 1);
            $("#txtLIDueDate").data("DateTimePicker").date($filter('date')(date1, 'dd/MM/yyyy'))
        }
    });

    $scope.CheckInterstrateinTerm = function () {

        if ($scope.CustomerProduct.TimePeriod != null && $scope.CustomerProduct.TimePeriod != undefined && $scope.CustomerProduct.TimePeriod != ""
            && $scope.CustomerProduct.NoOfMonthsORYears != "" && $scope.CustomerProduct.NoOfMonthsORYears != undefined && $scope.CustomerProduct.NoOfMonthsORYears != null) {
            var Id1 = $scope.CustomerProduct.NoOfMonthsORYears;
            var Id2 = $scope.CustomerProduct.TimePeriod;

            if ((Id1 != null && Id1 != undefined && Id1 != "") || (Id2 != null && Id2 != undefined && Id2 != "")) {
                if ($scope.CustomerProduct.TimePeriod == "1") {
                    Id1 = (Id1) * 30;

                }
                else if ($scope.CustomerProduct.TimePeriod == "2") {
                    Id1 = (Id1) * 365;
                }
            }

            var CheckInterest = AppService.GetDataByQuerystring("CustomerProduct", "GetInterestRate", Id1, Id2)
            CheckInterest.then(function (p1) {

                if (p1.data != 0) {
                    $scope.CustomerProduct.InterestRate = p1.data;
                    if ($scope.CustomerProduct.InterestRate != "" && $scope.CustomerProduct.InterestRate != 0 && $scope.CustomerProduct.InterestRate != undefined &&
                        $scope.CustomerProduct.InterestRate != null && $scope.CustomerProduct.InterestRate != "0" && $("#txtOpeningDateCP").val() != "" && $("#txtMaturityDate").val() != "") {
                        $scope.CalculateMaturityAmount();
                    }
                    else {
                        $scope.CustomerProduct.MaturityAmount = "";
                    }
                }
                else {
                    //$scope.CustomerProduct.InterestRate = "";
                    showToastMsg(3, 'No Interest Rate Found For This Term');
                }
            })
        }
    }

    //function IntialPageControlforAllEmployee() {
    //    $(".btnSearch").click(function () {
    //        var ID = $(this).attr("Id");
    //        $('#Employee').modal('hide');
    //        GetAllUserById(ID)
    //        $('#txtsearch').val('');
    //        $('#tblemployee').dataTable().fnDraw();
    //    });
    //}

    $scope.GetAllUserById = function (Id) {
        var getemployeedetail = AppService.GetDetailsById("User", "GetUserDataById", Id)
        getemployeedetail.then(function (p1) {
            if (p1.data != null) {
                if ($scope.Agent == true) {
                    $scope.CustomerProduct.AgentName = p1.data.FirstName + " " + p1.data.LastName;
                    $scope.CustomerProduct.AgentCode = p1.data.UserCode;
                    $scope.CustomerProduct.AgentId = p1.data.UserId;
                }
                else {
                    $scope.CustomerProduct.EmpName = p1.data.FirstName + " " + p1.data.LastName;
                    $scope.CustomerProduct.EmpCode = p1.data.UserCode;
                    $scope.CustomerProduct.EmployeeId = p1.data.UserId;
                }
            }
            else {
                showToastMsg(3, "Error in Getting Data");
            }
        })
    }

    $scope.AllEmployeeReset_Product = function () {
        $('#txtsearch').val('');
        $('#tblproductemployee').dataTable().fnDraw();
    }

    function ValidateCustomerProductForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#ddlProductTypelist"), 'Product Type required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlProductNamelist"), 'Product Name required', 'after')) {
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
        if ($scope.Productlist != 0) {
            if (!ValidateRequiredField($("#txtInterestRateCP"), 'Interest Rate required', 'after')) {
                flag = false;
            }

            if (!ValidateRequiredField($("#txtOpeningDateCP"), 'Date Required', 'after')) {
                flag = false;
            }

            if ($scope.CustomerProduct.ProductType == "1") {

                if ($scope.CustomerProduct.InsuranceTypeGI == true) {
                    if (!ValidateRequiredField($("#txtGICommencementDate"), 'GI Date Required', 'after')) {
                        flag = false;
                    }
                    if (!ValidateRequiredField($("#txtGIDueDate"), 'GI Date Required', 'after')) {
                        flag = false;
                    }
                    if (!ValidateRequiredField($("#txtGIPremium"), 'GI Premium Required', 'after')) {
                        flag = false;
                    }
                }

                if ($scope.CustomerProduct.InsuranceTypeLI == true) {
                    if (!ValidateRequiredField($("#txtLICommencementDate"), 'LI  Date Required', 'after')) {
                        flag = false;
                    }
                    if (!ValidateRequiredField($("#txtLIDueDate"), 'LI Date Required', 'after')) {
                        flag = false;
                    }
                    if (!ValidateRequiredField($("#txtLIPremium"), 'LI Premium Required', 'after')) {
                        flag = false;
                    }

                    if (!CheckRadioChecked($("#radioregular"), $("#radiosingle"), 'LIType Required', 'after')) {
                        flag = false;
                    }
                }
            }

            if ($scope.CustomerProduct.ProductType != "1" && $scope.CustomerProduct.ProductType != "2" && $scope.CustomerProduct.ProductType != "5") {
                if (!ValidateRequiredField($("#ddlTimePeriod"), 'Time Period Required', 'after')) {
                    flag = false;
                }
                if (!ValidateRequiredField($("#txtNoOfMonthsORYears"), 'Enter Months/Years', 'after')) {
                    flag = false;
                }
                if (!ValidateRequiredField($("#txtMaturityDate"), 'Enter Months/Years', 'after')) {
                    flag = false;
                }
                if (!ValidateRequiredField($("#txtAmount"), 'Enter Amount', 'after')) {
                    flag = false;
                }
            }

            if ($scope.CustomerProduct.ProductType == "4" || $scope.CustomerProduct.ProductType == "6" || $scope.CustomerProduct.ProductType == "7") {
                //if (!ValidateRequiredField($("#ddlTimePeriod"), 'Time Period Required', 'after')) {
                //    flag = false;
                //}
                //if (!ValidateRequiredField($("#txtNoOfMonthsORYears"), 'Enter Months/Years', 'after')) {
                //    flag = false;
                //}

                //if (!ValidateRequiredField($("#txtMaturityDate"), 'Enter Months/Years', 'after')) {
                //    flag = false;
                //}
                //if (!ValidateRequiredField($("#txtAmount"), 'Enter Amount', 'after')) {
                //    flag = false;
                //}

                if (!ValidateRequiredField($("#ddlPaymentTypeCP"), 'Payment Type Required', 'after')) {
                    flag = false;
                }

                if (!ValidateRequiredField($("#txtDueDate"), 'Due Date Required', 'after')) {
                    flag = false;
                }
            }
            if ($scope.CustomerProduct.ProductType == "4" || $scope.CustomerProduct.ProductType == "5" || $scope.CustomerProduct.ProductType == "6") {

                if (!ValidateRequiredField($("#txtLatePaymentFeesCP"), 'Late Payment Required', 'after')) {
                    flag = false;
                }
            }
            if ($scope.CustomerProduct.ProductType == "9" || $scope.CustomerProduct.ProductType == "10") {
                if (!ValidateRequiredField($("#ddlPaymentTypeCP"), 'Payment Type Required', 'after')) {
                    flag = false;
                }
                if (!ValidateRequiredField($("#txtDueDate"), 'Next Installment Date Required', 'after')) {
                    flag = false;
                }
            }
        }
        return flag;
    }

    function SetCustomerProductData() {

        var flag = ValidateCustomerProductForm();
        if (flag) {
            var ID = $scope.CustomerId;
            $scope.CustomerProduct.CustomerId = ID;
            $scope.CustomerProduct.CreatedBy = $cookies.getObject('User').UserId;

            if ($scope.CustomerProduct.CustomerProductId == '0000000-0000-0000-0000-000000000000' || $scope.CustomerProduct.CustomerProductId == undefined || $scope.CustomerProduct.CustomerProductId == null) {
                // $scope.CustomerProduct.BranchId = $cookies.getObject('User').BranchId;
                $scope.CustomerProduct.BranchId = $scope.CustomerData[0].BranchId;
                $scope.CustomerProduct.CreatedBy = $cookies.getObject('User').UserId;
                if ($scope.CustomerProduct.ProductType == "3" || $scope.CustomerProduct.ProductType == "4" || $scope.CustomerProduct.ProductType == "5" ||
                    $scope.CustomerProduct.ProductType == "6" || $scope.CustomerProduct.ProductType == "7" || $scope.CustomerProduct.ProductType == "8" || $scope.CustomerProduct.ProductType == "9" || $scope.CustomerProduct.ProductType == "10" || $scope.CustomerProduct.ProductType == 3 || $scope.CustomerProduct.ProductType == 4 ||
                    $scope.CustomerProduct.ProductType == 5 || $scope.CustomerProduct.ProductType == 6 || $scope.CustomerProduct.ProductType == 7 || $scope.CustomerProduct.ProductType == 8 || $scope.CustomerProduct.ProductType == 9 || $scope.CustomerProduct.ProductType == 10) {
                    $scope.CustomerProduct.Status = 1;
                }
            }
            else {
                $scope.CustomerProduct.ModifyBy = $cookies.getObject('User').UserId;
            }
            //if ($scope.CustomerProduct.UserId != '0000000-0000-0000-0000-000000000000' && $scope.CustomerProduct.UserId != undefined) {

            //}
            //else {

            //}
            $scope.CustomerProduct.OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');

            if ($scope.CustomerProduct.ProductType == "1") {
                if ($scope.CustomerProduct.InsuranceTypeGI == true) {
                    $scope.CustomerProduct.GICommencementDate = moment(new Date($("#txtGICommencementDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                    if (($("#txtGIDueDate").val() != '' && $("#txtGIDueDate").val() != "") && $("#txtGIDueDate").val() != '' && $("#txtGIDueDate").val() != "") {
                        $scope.CustomerProduct.GIDueDate = moment(new Date($("#txtGIDueDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                    }
                }

                if ($scope.CustomerProduct.InsuranceTypeLI == true) {
                    $scope.CustomerProduct.LICommencementDate = moment(new Date($("#txtLICommencementDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                    if (($("#txtLIDueDate").val() != '' && $("#txtLIDueDate").val() != "") && $("#txtLIDueDate").val() != '' && $("#txtLIDueDate").val() != "") {
                        $scope.CustomerProduct.LIDueDate = moment(new Date($("#txtLIDueDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                    }
                }
            }
            else if ($scope.CustomerProduct.ProductType == "2") {
                $scope.CustomerProduct.LIType = 0;
            }
            else if ($scope.CustomerProduct.ProductType == "5") {
                $scope.CustomerProduct.PaymentType = 2;
            }

            else if ($scope.CustomerProduct.ProductType == "3") {
                $scope.CustomerProduct.MaturityDate = moment(new Date($("#txtMaturityDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                $scope.CustomerProduct.LIType = 0;

            }
            else if ($scope.CustomerProduct.ProductType == "4" || $scope.CustomerProduct.ProductType == "6" || $scope.CustomerProduct.ProductType == "7" || $scope.CustomerProduct.ProductType == "8" || $scope.CustomerProduct.ProductType == "9" || $scope.CustomerProduct.ProductType == "10") {
                $scope.CustomerProduct.DueDate = moment(new Date($("#txtDueDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                $scope.CustomerProduct.NextInstallmentDate = moment(new Date($("#txtnextInstallmentdate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                $scope.CustomerProduct.MaturityDate = moment(new Date($("#txtMaturityDate").data("DateTimePicker").date())).format('YYYY-MM-DD')
                $scope.CustomerProduct.LIType = 0;
                if ($scope.CustomerProduct.ProductType == "9" || $scope.CustomerProduct.ProductType == "10") {
                    
                }
            }
            else {
                $scope.CustomerProduct.MaturityDate = null;
            }
            return true;
        }
        else {
            return false;
        }
    }

    $scope.SaveCustomerProduct = function () {
        var res = SetCustomerProductData();
        if (res) {
            $(':focus').blur();
            var customerproduct = AppService.SaveData("CustomerProduct", "SaveCustomerProductData", $scope.CustomerProduct)
            customerproduct.then(function (p1) {
                if (p1.data != null) {
                    if ($scope.CustomerProduct.ProductType == "5") {
                        $('#CustomerProduct').modal('hide');
                        window.location = "/App/Loan?CustomerProductId=" + p1.data;
                    }

                    $scope.CustomerProduct = p1.data
                    $('#CustomerProduct').modal('hide');
                    GetProductList();
                    showToastMsg(1, "Product Saved Successfully")
                    $scope.ClearForm();
                }
                else {
                    showToastMsg(3, 'Error in Saving Data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in saving data')
            });
        }
    }

    $scope.SubmitCustomerProduct = function () {
        var res = SetCustomerProductData();
        if (res) {
            $(':focus').blur();
            if ($filter('date')($scope.CustomerProduct.OpeningDate, "yyyy-MM-dd") == $filter('date')($scope.CustomerProduct.DueDate, "yyyy-MM-dd")) {
                showToastMsg(3, "You cannot set installment date same as opening date.");
                return false;
            }

            //$scope.CustomerProduct.OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
            if ($filter('date')($scope.CustomerProduct.OpeningDate, "yyyy-MM-dd") == $filter('date')($scope.today, "yyyy-MM-dd") &&
                ($scope.CustomerProduct.OpeningBalance == "" || $scope.CustomerProduct.OpeningBalance == null)) {
                SubmitCustomerProductRDFDMIS()
            }
            else if ($scope.CustomerProduct.OpeningBalance != "" && $scope.CustomerProduct.OpeningBalance != null &&
                $scope.CustomerProduct.OldAccountNumber != "" && $scope.CustomerProduct.OldAccountNumber != null) {
                SubmitCustomerProductRDFDMIS()
            }
            else {
                showToastMsg(3, "You can not submit this Product. You have to submit this RD/FD on Opening date");
            }
        }
    }

    function SubmitCustomerProductRDFDMIS() {
        var SubmitCustomerFD = AppService.SaveData("CustomerProduct", "SubmitCustomerRDFD", $scope.CustomerProduct)
        SubmitCustomerFD.then(function (p1) {
            if (p1.data != false) {
                showToastMsg(1, "Data saved successfully");
                $('#CustomerProduct').modal('hide');
                $('#tblProduct').dataTable().fnDraw();
            }
            else {
                showToastMsg(3, "You don't have sufficient balance");
            }
        })
    }

    $scope.ClearForm = function () {

        $("#txtStartDateCP").val('');
        $("#txtEndDateCP").val('');
        $("#txtOpeningDateCP").val('');
        $("#txtPreviousOpeningDateCP").val('');
        $("#txtLatePaymentFeesCP").val('');
        $("#ddlProductNamelist").val('');
        $scope.CustomerProduct = {};
        $scope.Productlist = [];
        $('#txtPreviousOpeningDateCP').data("DateTimePicker").clear();
        $('#txtOpeningDateCP').data("DateTimePicker").clear();
        $('#txtDueDate').data("DateTimePicker").clear();
        $('#txtnextInstallmentdate').data("DateTimePicker").clear();        
        $('#txtLIDueDate').data("DateTimePicker").clear();
        $('#txtGIDueDate').data("DateTimePicker").clear();

        $scope.CustomerProduct.AgentName = $scope.Customer.AgentName;
        $scope.CustomerProduct.AgentCode = $scope.Customer.AgentCode;
        $scope.CustomerProduct.AgentId = $scope.Customer.AgentId;

        $scope.CustomerProduct.EmpName = $scope.Customer.EmpName;
        $scope.CustomerProduct.EmpCode = $scope.Customer.EmpCode;
        $scope.CustomerProduct.EmployeeId = $scope.Customer.EmployeeId;

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $scope.ClearonbtnClose = function () {
        $('#CustomerProduct').modal('hide');
        //$scope.Product = {};
        $("#txtStartDateCP").val('');
        $("#txtEndDateCP").val('');
        $("#txtOpeningDateCP").val('');
        $("#txtLatePaymentFeesCP").val('');
        $scope.CustomerProduct.CustomerProductId = '0000000-0000-0000-0000-000000000000';
        $("#ddlProductNamelist").val('');
        $scope.CustomerProduct.ProductType = null;
        $scope.CustomerProduct.ProductId = null;
        $scope.CustomerProduct.Amount = null
        // $scope.CustomerProduct.InsuranceLIGI = 0

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    function GetProductList() {
        $('#tblProduct').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            //'bAutoWidth': false,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            //"scrollX": true,
            "sAjaxSource": urlpath + "/CustomerProduct/GetAllProductDataByCustomerId",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });
                aoData.push({ "name": "id", "value": $scope.CustomerId });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {

                        fnCallback(json);
                        IntialPageControlCustomerProduct();
                    }
                });
            },
            "aoColumns": [{
                "sTitle": "Account No",
                "mDataProp": "AccountNumber",
            },
            {
                "sTitle": "Product Name",
                //"mDataProp": "Product.ProductName",
                "mRender": function (data, type, full) {
                    if (full.ReferenceCustomerProductId != null) {
                        return full.Product.ProductName + " (Akshaya Tritiya)"
                    }
                    else {
                        return full.Product.ProductName;
                    }

                },

            },
            {
                "sTitle": "Product Type",
                "mDataProp": "ProductTypeName",
            },
            {
                "sTitle": "Interest",
                "mDataProp": "InterestRate",
            },
            {
                "sTitle": "Payment Type",
                "mDataProp": "PaymentName",
            },
            {
                "sTitle": "Time Period",
                "mRender": function (data, type, full) {
                    if (full.NoOfMonthsORYears != null && full.TimePeriod != null) {
                        return full.NoOfMonthsORYears + " " + full.TimePeriodName
                    }
                    else {
                        return '';
                    }

                },
            },
            {
                "sTitle": "Opening Date",
                "mDataProp": "OpeningDate",
                "mRender": function (data, type, full) {
                    return $filter('date')(data, 'dd/MM/yyyy');
                },
            },
            //{
            //    "sTitle": "Insurance",
            //    "mDataProp": "InsuranceTypeLI",
            //    "mRender": function (data, type, full) {
            //        var str = ''

            //        if (full.InsuranceTypeLI != true && full.InsuranceTypeGI != true)
            //            str = "-";
            //        else if (full.InsuranceTypeLI == true && full.InsuranceTypeGI == true) {
            //            str = "LI, GI";
            //        }
            //        else if (full.InsuranceTypeLI == true)
            //            str = "LI";
            //        else if (full.InsuranceTypeGI == true)
            //            str = "GI";


            //        return str;
            //    },
            //    "sWidth": "100px"
            //},
            {
                "sTitle": "Status",
                "mDataProp": "Status",
                "mRender": function (data, type, full) {

                    if (full.Status == 2) {
                        if (full.ProductType != 5) {
                            return '<span class="label" style="background-color:green">Submitted</span>';
                        }
                        else {
                            return '<span class="label" style="background-color:green">Approved</span>';
                        }
                    }
                    else if (full.Status == 3) {
                        return '<span class="label" style="background-color:red">Rejected</span>';
                    }
                    else if (full.Status == 4) {
                        return '<span class="label" style="background-color:skyblue">Cancelled</span>';
                    }
                    else if (full.Status == 5) {
                        return '<span class="label" style="background-color:blue">Completed</span>';
                    }
                    else if (full.Status == 6) {
                        return '<span class="label" style="background-color:red">Closed</span>';
                    }
                    else {
                        return '';
                    }
                },
            },
            {
                "sTitle": "Active",
                "mDataProp": "IsActive",
                "mRender": function (data, type, full) {

                    if (full.IsActive == true) {
                        return '<span class="label" style="background-color:green">Active</span>';
                    }
                    else {
                        return '<span class="label" style="background-color:red">Deactive</span>';
                    }
                },
            },
            {
                "sTitle": "Action",
                "mDataProp": "CustomerProductId",
                "mRender": function (data, type, full) {
                    var str = '';
                    if (full.Status == null || full.Status == 1 || full.Status == 2) {

                        str = '<button class="btn btn-success btn-xs btnEdit btn-flat" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>';
                    }

                    if (full.Status == null || full.Status == 2 || full.Status == 5 || full.Status == 6) {
                        var acNo = btoa(full.AccountNumber);
                        str += ' <a href="/App/Transaction?AccountNo=' + acNo + '" class="btn btn-primary btn-xs btnTransaction  btn-flat" title="Transaction"><span class="glyphicon glyphicon-money"></span> <i class="glyphicon glyphicon-plus"> </i> Transaction</a>'
                    }

                    if (full.ProductType != 5 && full.ProductType != 2 && full.ProductType != 1 && full.Status == 2) {
                        if (full.IsCertificatePrinted == null) {
                            str += ' <button class="btn btn-warning btn-xs btnPrint btn-flat" Id="' + data + '" title="Print"><span class="glyphicon glyphicon-print"></span> Print</button> ';
                        }
                        else {
                            str += ' <button class="btn btn-warning btn-xs btnRePrint btn-flat" Id="' + data + '" title="RePrint"><span class="glyphicon glyphicon-print"></span> RePrint</button> ';
                        }
                    }

                    if (full.ProductType == 3 || full.ProductType == 4 || full.ProductType == 8) {
                        //  str += ' <button class="btn btn-warning btn-xs btnPrint btn-flat" Id="' + data + '" title="Print"><span class="glyphicon glyphicon-print"></span> Print</button> ';
                        if (full.Status == 2)
                            str += '<button class="btn btn-info btn-xs btnPremature btn-flat" Id="' + data + '" title="Premature"><span class="glyphicon glyphicon-stop"></span> Premature</button>';
                    }

                    if (full.ProductType == 6) {
                        if (full.Status != 5) {
                            str += ' <button class="btn btn-warning btn-xs btnRIPMISPrint btn-flat" Id="' + data + '" title="Print"><span class="glyphicon glyphicon-print"></span> Annexure</button> ';
                            if (full.Status == 2)
                                str += '<button class="btn btn-info btn-xs btnPrematureRIP btn-flat" Id="' + data + '" title="Premature"><span class="glyphicon glyphicon-stop"></span> Premature</button>';
                        }
                    }


                    if (full.ProductType == 7) {
                        if (full.Status != 5) {
                            // str += ' <button class="btn btn-warning btn-xs btnPrint btn-flat" Id="' + data + '" title="Print"><span class="glyphicon glyphicon-print"></span> Print</button> ';
                            str += ' <button class="btn btn-warning btn-xs btnRIPMISPrint btn-flat" Id="' + data + '" title="Print"><span class="glyphicon glyphicon-print"></span> Annexure</button> ';
                            if (full.Status == 2)
                                str += '<button class="btn btn-info btn-xs btnPremature btn-flat" Id="' + data + '" title="Premature"><span class="glyphicon glyphicon-stop"></span> Premature</button>';
                        }
                    }

                    if (full.ProductType == 4) {
                        if (full.Status != 5 && full.Product.ProductName == 'Akshaya Tritiya') {
                            str += ' <button class="btn btn-warning btn-xs btnAkshyaTrityaPrint btn-flat" Id="' + data + '" title="Print"><span class="glyphicon glyphicon-print"></span> Annexure</button> ';
                        }
                    }



                    if ((full.ProductType == 1 || full.ProductType == 2) && full.Status != 6) {
                        str += ' <button class="btn btn-danger btn-xs btnClose btn-flat" Id="' + data + '" title="Close"><span class="fa fa-window-close"></span> Close A/C</button>';
                    }
                    return str
                },
                "sClass": "text-center",
                "sWidth": "150px"

            }]
        });
    }

    function IntialPageControlCustomerProduct() {
        $(".btnEdit").click(function () {
            var PID = $(this).attr("Id");
            $scope.ProductId = PID;
            $("#CustomerProduct").modal('show');
            GetProductDataById(PID)

        });
        $(".btnPrint").click(function () {
            var PID = $(this).attr("Id");
            $scope.ProductId = PID;
            $("#CustomerDepositPrint").modal('show');
            GetProductPrintData(PID)

        });

        $(".btnRePrint").click(function () {
            var PID = $(this).attr("Id");
            $scope.Reprint = {};
            $scope.Reprint.CustomerProductId = PID;
            GetCustomerPrintHistory();
            $("#CertificateReprint").modal('show');

        });

        $(".btnPrematureRIP").click(function () {
            var PID = $(this).attr("Id");
            $scope.ProductId = PID;
            $("#PrematureRIPModal").modal('show');
            GetPremetureRIPData(PID);
        });

        $(".btnRIPMISPrint").click(function () {
            var PID = $(this).attr("Id");
            $scope.ProductId = PID;
            $("#PrintRIPMIS").modal('show');
            GetRIPPrintData(PID)

        });
        $(".btnAkshyaTrityaPrint").click(function () {
            var PID = $(this).attr("Id");
            $scope.ProductId = PID;
            $("#PrintAKSHYATRITYA").modal('show');
            GetAKSHYATRITYAPrintData(PID)
        });



        $(".btnPremature").click(function () {
            var PID = $(this).attr("Id");
            $scope.ProductId = PID;

            var premature = AppService.GetDetailsById("CustomerProduct", "CalculatePrematureWithdrawalRDFD", PID);
            premature.then(function (p1) {
                if (p1.data != null) {
                    $scope.Premature = p1.data;
                }
                else {
                    showToastMsg(3, 'Error in getting data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in getting data')
            });


            $("#PrematureRIPModal").modal('show');


        });


        $(".btnClose").click(function () {
            var PID = $(this).attr("Id");
            bootbox.dialog({
                message: "Are you sure want to close this Account?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            var closeAc = AppService.GetDetailsById("CustomerProduct", "CloseCustomerAccount", PID);
                            closeAc.then(function (p1) {
                                if (p1.data) {
                                    showToastMsg(1, 'your account is closed.')
                                    $('#tblProduct').dataTable().fnDraw();
                                }
                                else {
                                    showToastMsg(3, 'you can not close this account as you may withdraw the money.')
                                }
                            }, function (err) {
                                showToastMsg(3, 'Error in getting data')
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

    function GetProductDataById(PID) {
        var getproductdata = AppService.GetDetailsById("CustomerProduct", "GetCustomerProductById", PID);
        getproductdata.then(function (p1) {
            if (p1.data != null) {
                $scope.CustomerProduct = {};

                //$scope.CustomerProduct.CustomerProductId = p1.data.CustomerProductId;
                $scope.CustomerProduct = p1.data;
                GetProductDetail($scope.CustomerProduct.ProductType);
                ReturnSavingAccountNum(p1.data.ProductType)
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function GetPremetureRIPData(Id) {
        AppService.GetDetailsById("CustomerProduct", "GetPremetureRIPData", Id).then(function (p1) {
            if (p1) {
                $scope.Premature = p1.data;
            }
        });
    }

    $scope.PrematureRDFD = function () {
        var PID = $(this).attr("Id");
        $scope.ProductId = PID;
        bootbox.hideAll();
        bootbox.dialog({
            message: "Are you sure want to Premature Withdrawal?",
            title: "Confirmation !",
            size: 'small',
            buttons: {
                success: {
                    label: "Yes",
                    className: "btn-success btn-flat",
                    callback: function () {
                        $(':focus').blur();
                        var premature = AppService.SaveData("CustomerProduct", "PrematureWithDrawalRDFD", $scope.Premature);
                        premature.then(function (p1) {
                            if (p1.data != null) {
                                $("#PrematureRIPModal").modal('hide');
                                $('#tblProduct').dataTable().fnDraw();
                                showToastMsg(1, 'Successfully prematured withdrawal')
                            }
                            else {
                                showToastMsg(3, 'Error in getting data')
                            }
                        }, function (err) {
                            showToastMsg(3, 'Error in getting data')
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

    $scope.PrematureRIP = function () {
        bootbox.hideAll();
        bootbox.dialog({
            message: "Are you sure want to Premature Withdrawal?",
            title: "Confirmation !",
            size: 'small',
            buttons: {
                success: {
                    label: "Yes",
                    className: "btn-success btn-flat",
                    callback: function () {
                        $(':focus').blur();
                        var premature = AppService.SaveData("CustomerProduct", "PrematureWithDrawalRIP", $scope.Premature);
                        premature.then(function (p1) {
                            if (p1.data != null) {
                                $("#PrematureRIPModal").modal('hide');
                                $('#tblProduct').dataTable().fnDraw();
                                showToastMsg(1, 'Successfully prematured withdrawal')
                            }
                            else {
                                showToastMsg(3, 'Error in getting data')
                            }
                        }, function (err) {
                            showToastMsg(3, 'Error in getting data')
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

    $scope.SetDataForEdit = function () {

        $scope.CustomerProduct.AccountNumber = $scope.CustomerProduct.AccountNumber;
        //GetProductDetail($scope.CustomerProduct.ProductType);
        $scope.CustomerProduct.LatePaymentFees = $scope.CustomerProduct.LatePaymentFees;
        $scope.CustomerProduct.ProductId = $scope.CustomerProduct.ProductId + "";
        $scope.CustomerProduct.ProductType = $scope.CustomerProduct.ProductType + "";
        $scope.CustomerProduct.InterestRate = $scope.CustomerProduct.InterestRate;
        //$scope.CustomerProduct.InterestType = $scope.CustomerProduct.InterestType + "";
        $scope.CustomerProduct.InsuranceTypeLI = $scope.CustomerProduct.InsuranceTypeLI;
        $scope.CustomerProduct.InsuranceTypeGI = $scope.CustomerProduct.InsuranceTypeGI;

        $scope.CustomerProduct.FrequencyType = $scope.CustomerProduct.FrequencyType + "";
        $scope.CustomerProduct.PaymentType = $scope.CustomerProduct.PaymentType + "";
        $scope.CustomerProduct.IsActive = $scope.CustomerProduct.IsActive;
        $scope.CustomerProduct.ProductType = $scope.CustomerProduct.ProductType + "";
        $scope.CustomerProduct.EmpName = $scope.CustomerProduct.EmpName;
        $scope.CustomerProduct.EmpCode = $scope.CustomerProduct.EmpCode;
        $scope.CustomerProduct.EmployeeId = $scope.CustomerProduct.EmployeeId;
        $scope.CustomerProduct.AgentId = $scope.CustomerProduct.AgentId;
        $scope.CustomerProduct.AgentName = $scope.CustomerProduct.AgentName;
        $scope.CustomerProduct.AgentCode = $scope.CustomerProduct.AgentCode;
        $scope.CustomerProduct.TimePeriod = $scope.CustomerProduct.TimePeriod;
        $scope.CustomerProduct.MaturityAmount = $scope.CustomerProduct.MaturityAmount;
        $scope.CustomerProduct.OldAccountNumber = $scope.CustomerProduct.OldAccountNumber;
        $scope.CustomerProduct.OpeningBalance = $scope.CustomerProduct.OpeningBalance;


        $("#txtOpeningDateCP").data("DateTimePicker").date($filter('date')($scope.CustomerProduct.OpeningDate, 'dd/MM/yyyy'))

        if ($scope.CustomerProduct.ProductType == "1") {
            if ($scope.CustomerProduct.InsuranceTypeLI == true) {

                //alert(JSON.stringify($scope.CustomerProduct));
                $scope.CustomerProduct.LIType = $scope.CustomerProduct.LIType;
                $("#txtLICommencementDate").data("DateTimePicker").date($filter('date')($scope.CustomerProduct.LICommencementDate, 'dd/MM/yyyy'))
                $("#txtLIDueDate").data("DateTimePicker").date($filter('date')($scope.CustomerProduct.LIDueDate, 'dd/MM/yyyy'))
                $scope.CustomerProduct.LIPremium = $scope.CustomerProduct.LIPremium;
            }

            if ($scope.CustomerProduct.InsuranceTypeGI == true) {
                $scope.CustomerProduct.GIPremium = $scope.CustomerProduct.GIPremium;
                $("#txtGICommencementDate").data("DateTimePicker").date($filter('date')($scope.CustomerProduct.GICommencementDate, 'dd/MM/yyyy'))
                $("#txtGIDueDate").data("DateTimePicker").date($filter('date')($scope.CustomerProduct.GIDueDate, 'dd/MM/yyyy'))

            }
        }
        if ($scope.CustomerProduct.ProductType == "3") {
            $scope.CustomerProduct.Amount = $scope.CustomerProduct.Amount;
            $("#txtMaturityDate").data("DateTimePicker").date($filter('date')($scope.CustomerProduct.MaturityDate, 'dd/MM/yyyy'))
            $scope.CustomerProduct.Status = $scope.CustomerProduct.Status;
            $scope.CustomerProduct.CertificateNumber = $scope.CustomerProduct.CertificateNumber;
        }
        if ($scope.CustomerProduct.ProductType == "4" || $scope.CustomerProduct.ProductType == "6" || $scope.CustomerProduct.ProductType == "7" || $scope.CustomerProduct.ProductType == "8" || $scope.CustomerProduct.ProductType == "9" || $scope.CustomerProduct.ProductType == "10") {
            $scope.CustomerProduct.Amount = $scope.CustomerProduct.Amount;
            $scope.CustomerProduct.Status = $scope.CustomerProduct.Status;
            $("#txtDueDate").data("DateTimePicker").date($filter('date')($scope.CustomerProduct.DueDate, 'dd/MM/yyyy'))
            $("#txtnextInstallmentdate").data("DateTimePicker").date($filter('date')($scope.CustomerProduct.NextInstallmentDate, 'dd/MM/yyyy'))
            $("#txtMaturityDate").data("DateTimePicker").date($filter('date')($scope.CustomerProduct.MaturityDate, 'dd/MM/yyyy'))
            $scope.CustomerProduct.CertificateNumber = $scope.CustomerProduct.CertificateNumber;
        }
        if ($scope.CustomerProduct.ProductType == "9" || $scope.CustomerProduct.ProductType == "10") {
            $("#ddlPaymentTypeCP option[value='1']").remove()
        }

        $scope.CustomerProduct.TimePeriod = "" + $scope.CustomerProduct.TimePeriod;
        $scope.CustomerProduct.NoOfMonthsORYears = $scope.CustomerProduct.NoOfMonthsORYears;
    }

    $scope.CalculateMaturityAmount = function () {
        if (($scope.CustomerProduct.ProductType == "3" || $scope.CustomerProduct.ProductType == "4" || $scope.CustomerProduct.ProductType == "6" || $scope.CustomerProduct.ProductType == "8") &&
            $("#txtOpeningDateCP").val() != "" && $("#txtMaturityDate").val() != "" && $scope.CustomerProduct.InterestRate != "" && $scope.CustomerProduct.InterestRate != undefined && $scope.CustomerProduct.InterestRate
            && $scope.CustomerProduct.TimePeriod != undefined && $scope.CustomerProduct.TimePeriod != "" && $scope.CustomerProduct.NoOfMonthsORYears != undefined && $scope.CustomerProduct.NoOfMonthsORYears != "") {
            var obj = new Object()
            obj.ProductType = $scope.CustomerProduct.ProductType;
            // GetProductDetail(obj.ProductType);
            if ($("#txtOpeningDateCP").val() != null && $("#txtOpeningDateCP").val() != "") {
                obj.OpeningDate = moment(new Date($("#txtOpeningDateCP").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }

            if ($("#txtMaturityDate").val() != null && $("#txtMaturityDate").val() != "") {
                obj.MaturityDate = moment(new Date($("#txtMaturityDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }

            obj.InterestType = $filter('filter')($scope.Productlist, { ProductId: $scope.CustomerProduct.ProductId })[0].Frequency;
            obj.InterestRate = $scope.CustomerProduct.InterestRate;
            if ($("#txtDueDate").val() != null && $("#txtDueDate").val() != "") {
                obj.DueDate = moment(new Date($("#txtDueDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            if ($("#txtnextInstallmentdate").val() != null && $("#txtnextInstallmentdate").val() != "") {
                obj.NextInstallmentDate = moment(new Date($("#txtnextInstallmentdate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            obj.PaymentType = $scope.CustomerProduct.PaymentType;
            obj.Amount = $scope.CustomerProduct.Amount;
            obj.TimePeriod = $scope.CustomerProduct.TimePeriod;
            obj.NoOfMonthsORYears = $scope.CustomerProduct.NoOfMonthsORYears;
            obj.OpeningBalance = $scope.CustomerProduct.OpeningBalance;
            obj.SkipFirstInstallment = $scope.CustomerProduct.SkipFirstInstallment;
            angular.forEach($scope.Productlist, function (value) {
                if ($scope.CustomerProduct.ProductId == value.ProductId) {
                    obj.ProductName = value.ProductName;
                }
            });

            if ($("#txtOpeningDateCP").val() != null && $("#txtOpeningDateCP").val() != "" && $("#txtMaturityDate").val() != null && $("#txtMaturityDate").val() != "" &&
                $scope.CustomerProduct.InterestRate != "" && $scope.CustomerProduct.InterestRate != 0 && $scope.CustomerProduct.Amount != "" && $scope.CustomerProduct.Amount != 0) {

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
            }
        }
        else if ($scope.CustomerProduct.ProductType == "7") {
            $scope.CustomerProduct.MaturityAmount = $scope.CustomerProduct.Amount;
        }
        var PaymentType = $("#ddlPaymentTypeCP").val()
        var OpeningDate = new Date($("#txtOpeningDateCP").data("DateTimePicker").date());
        if ($scope.CustomerProduct.CustomerProductId == undefined) {
            if (!$scope.IsDueDateChanged) {
                if (($("#txtOpeningDateCP").val() != "")) {
                    if (PaymentType == "1") {
                        OpeningDate.setDate(OpeningDate.getDate() + 1);
                        //var date = moment(new Date(OpeningDate)).format('DD/MM/YYYY');
                        //$("#txtDueDate").val(date);
                        $("#txtDueDate").data("DateTimePicker").date($filter('date')(OpeningDate, 'dd/MM/yyyy'))
                        //alert(1)
                        //$("#txtDueDate").datepicker("option", "minDate", new Date(OpeningDate));


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
    }

    function GetProductPrintData(PID) {
        var getproductdata = AppService.GetDetailsById("CustomerProduct", "GetProductPrintData", PID);
        getproductdata.then(function (p1) {
            if (p1.data != null) {
                $scope.CustomerPrintdata = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function GetRIPPrintData(PID) {
        var getproductdata = AppService.GetDetailsById("CustomerProduct", "GetRIPPrintData", PID);
        getproductdata.then(function (p1) {
            if (p1.data != null) {

                $scope.CustomerRIPPrintdata = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function GetAKSHYATRITYAPrintData(PID) {
        var getproductdata = AppService.GetDetailsById("CustomerProduct", "GetAkshyaTrityaPrintData", PID);
        getproductdata.then(function (p1) {
            if (p1.data != null) {

                $scope.CustomerAKSHYATRITYAPrintdata = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.printPageArea = function (Id, IsCertificatePrinted) {
        if (IsCertificatePrinted == null) {
            var printflag = AppService.GetDetailsById("CustomerProduct", "UpdatePrintFlag", Id);
            printflag.then(function (p1) {
                if (p1.data != null) {
                    GetCustomerByIdForProduct($location.search().CustomerId)
                    PrintCertificate()
                }
                else {
                    showToastMsg(3, 'Error in getting data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in getting data')
            });
        }
        else {
            PrintCertificate();
        }
    };

    function PrintCertificate() {
        var printContent = document.getElementById('print');
        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
        //WinPrint.document.write("<style>");
        //WinPrint.document.write("#tblholderdetail{padding- top:50px;padding-bottom:100px;} </style>");
        //WinPrint.document.write("<style>");
        //WinPrint.document.write("@media print {body { font-size: 20pt}} </style>");
        // document.write(printContent.innerHTML);
        //  return;
        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        //WinPrint.close();
        $timeout(function () { WinPrint.close(); }, 2000);
    }

    $scope.PrintRIP = function (areaID) {
        var printContent = document.getElementById(areaID);
        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
        WinPrint.document.write("<style>");
        WinPrint.document.write("#PlannerTable > tbody > tr > td {text-align:center;}</style>");
        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        //WinPrint.close();
        $timeout(function () { WinPrint.close(); }, 2000);
    };

    function ValidateReprintForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#txtReason"), 'Reason required', 'after')) {
            flag = false;
        }

        return flag;
    }

    $scope.SaveRePrint = function () {
        var flag = true
        flag = ValidateReprintForm()

        if (flag) {
            $(':focus').blur();
            var printflag = AppService.SaveData("CustomerProduct", "SaveCertificatePrintHistory", $scope.Reprint);
            printflag.then(function (p1) {
                if (p1.data) {
                    $("#CertificateReprint").modal('hide');
                    $("#CustomerDepositPrint").modal('show');
                    GetProductPrintData($scope.Reprint.CustomerProductId);

                }
                else {
                    showToastMsg(3, 'Error in getting data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in getting data')
            });
        }
    }

    function GetCustomerPrintHistory() {
        $('#CertificatePrintHistory').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            //'bAutoWidth': false,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            //"scrollX": true,
            "sAjaxSource": urlpath + "/CustomerProduct/GetCertificatePrintHistory",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "id", "value": $scope.Reprint.CustomerProductId });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                    }
                });
            },
            "aoColumns": [{
                "mDataProp": "IsDuplicate",
            },
            {
                "mDataProp": "Reason",
            },
            {
                "mDataProp": "PrintedDate",
                "mRender": function (data, type, full) {
                    return $filter('date')(data, 'dd/MM/yyyy');
                },
            },
            {
                "mDataProp": "PrintBy",
            }

            ]
        });
    }

    $("#CertificateReprint").on('hidden.bs.modal', function (event) {
        if ($('.modal:visible').length) //check if any modal is open
        {
            $('body').addClass('modal-open');//add class to body
        }
    });
})