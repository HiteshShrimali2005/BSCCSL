angular.module("BSCCL").controller('ProductController', function ($scope, AppService, $state, $cookies, $filter, $rootScope) {

    //$scope.percent = 5.45;
    //$scope.maxValue = 50;
    //$scope.maxDecimals = 2;
    //// $scope.dollarValue = '';
    //$scope.currencyIncludeDecimals = true;

    $("#txtStart").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $("#txtEnd").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $('#Product').modal({
        backdrop: 'static',
        show: false,
        keyboard: false
    });

    $scope.onlyNumbers = /^[1-9]+[0-9]*$/;

    GetProductList();
    GetLoanType();

    $scope.Add = function () {
        $scope.Product = {};
        $scope.Term = {};
        $("#txtStart").val('');
        $("#txtEnd").val('');
        $("#ProductType").val('');
        $("#InterestType").val('');
        $("#Frequency").val('');
        $("#PaymentType").val('');
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $rootScope.CountValue = 1;

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

    function GetProductDataById(Id) {

        var getproductdata = AppService.GetDetailsById("Product", "GetProductDataById", Id);
        getproductdata.then(function (p1) {
            if (p1.data != null) {
                $scope.Product = p1.data;
                $scope.Product.ProductType = p1.data.ProductType + "";

                if ($scope.Product.StartDate !== '') {
                    $("#txtStart").data("DateTimePicker").date($filter('date')($scope.Product.StartDate, 'dd/MM/yyyy'));
                    $("#txtEnd").data("DateTimePicker").date($filter('date')($scope.Product.EndDate, 'dd/MM/yyyy'));
                }
                else {
                    $("#txtStart").data("DateTimePicker").date($filter('date')(''));
                    $("#txtEnd").data("DateTimePicker").date($filter('date')(''));
                }

                $scope.Product.InterestType = p1.data.InterestType + "";
                $scope.Product.Frequency = p1.data.Frequency + "";
                $scope.Product.PaymentType = p1.data.PaymentType + "";
                $scope.Product.LoanTypeId = p1.data.LoanTypeId + "";
                //$("#ProductType").val($scope.Product.ProductType);
                //$("#InterestType").val($scope.Product.InterestType + "");
                //$("#Frequency").val($scope.Product.Frequency + "");
                //$("#PaymentType").val($scope.Product.PaymentType + "");

                //$scope.Product.TimePeriod = p1.data.TimePeriod + "";

            }
            else {
                showToastMsg(3, 'Error in Getting data 123')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data from database')
        });
    }

    $scope.SaveProduct = function () {
        var flag = true;

        flag = ValidateCustomerProduct();

        if (flag) {
            //if (document.getElementById('txtStart').value != "" && document.getElementById('txtEnd').value != '') {

            if ($("#txtStart").val() != "" && $("#txtStart").val() != '' && $("#txtStart").val() != undefined) {
                $scope.Product.StartDate = moment(new Date($("#txtStart").data("DateTimePicker").date())).format('YYYY-MM-DD');
                $scope.Product.EndDate = moment(new Date($("#txtEnd").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            else {
                $scope.Product.StartDate = null
                $scope.Product.EndDate = null
            }

            //$scope.Product.ProductType = $("#ProductType").val();
            //$scope.Product.InterestType = $("#InterestType").val();
            //$scope.Product.Frequency = $("#Frequency").val();
            //$scope.Product.PaymentType = $("#PaymentType").val();

            //if ($scope.Product.ProductType == 3) {
            //    $scope.Product.TimePeriod = $("#ddlTimePeriod").val();
            //}
            //else {
            //    $scope.Product.TimePeriod = 0
            //}

            //   $scope.Product.ProductType = $("#ProductType").val();
            if ($scope.Product.ProductId == '0000000-0000-0000-0000-000000000000' && $scope.Product.ProductId != undefined) {
                $scope.Product.CreatedBy = $cookies.getObject('User').UserId;
            }
            else {
                $scope.Product.ModifiedBy = $cookies.getObject('User').UserId;
            }

            var saveproduct = AppService.SaveData("Product", "ProductRegister", $scope.Product)

            saveproduct.then(function (p1) {
                if (p1.data != null) {
                    $('#Product').modal('hide');
                    showToastMsg(1, "Product Saved Successfully")
                    GetProductList();
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
        $scope.Product = {};
        $("#txtStart").val('');
        $("#txtEnd").val('');
        $("#ProductType").val('');
        $("#InterestType").val('');
        $("#Frequency").val('');
        $("#PaymentType").val('');
        $scope.Term.From = '';
        $scope.Term.To = '';
        $scope.Term.TimePeriod = '';
        $scope.Term.InterestRate = '';
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
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/Product/GetAllProductData",
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
                "mDataProp": "ProductName",
            },
            {
                "mDataProp": "ProductTypeName",
                "sWidth": "100px"
            },
             {
                 "mDataProp": "ProductCode",

             },
            {
                "mDataProp": "InterestRate",
            },
            {
                "mDataProp": "InterestName",
            },
            {
                "mDataProp": "FrequencyName",
            },
            {
                "mDataProp": "PaymentName",
            },
            {
                "mDataProp": "StartDate",
                "mRender": function (data, type, full) {
                    return $filter('date')(data, 'dd/MM/yyyy');
                },
                "sWidth": "50px"
            },
            {
                "mDataProp": "EndDate",
                "mRender": function (data, type, full) {
                    return $filter('date')(data, 'dd/MM/yyyy');
                },
                "sWidth": "50px"
            },
            {
                "mDataProp": "IsActive",
                "mRender": function (data, type, full) {
                    if (data == true) {
                        return '<span class="label" style="background-color:green">Active</span>';
                    }
                    else {
                        return '<span class="label" style="background-color:red">Deactive</span>';
                    }
                }
            },
            {
                "mDataProp": "ProductId",
                "mRender": function (data, type, full) {

                    return '<button class="btn btn-success btn-xs btnEdit btn-flat" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>  <button class="btn btn-danger btn-flat btn-xs btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> ';
                },
                "sClass": "text-center",
                "sWidth": "100px"
            }]

        });
    }

    $scope.SearchDataProduct = function () {

        $('#tblProduct').dataTable().fnDraw();
    }

    $scope.SearchClearProductData = function () {
        $('#txtSearch').val('');
        $('#tblProduct').dataTable().fnDraw();
    }

    function IntialPageControlUser() {
        $(".btnEdit").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $('#btntab1').click();
            $("#Product").modal('show');
            GetProductDataById(ID)
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
                            var promiseDelete = AppService.DeleteData("Product", "DeleteProduct", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {

                                    RefreshDataTablefnDestroy();
                                    toastr.remove();
                                    showToastMsg(1, "Product Deleted Successfully");
                                    GetProductList();
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
        $("#tblProduct").dataTable().fnDestroy();
    }

    function ValidateCustomerProduct() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#txtProductName"), 'Product Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ProductType"), 'Product Type required', 'after')) {
            flag = false;
        }
        if ($scope.Product.ProductType == "1" || $scope.Product.ProductType == "5") {
            if (!ValidateRequiredField($("#txtInterestRate"), 'Interest Rate required', 'after')) {
                flag = false;
            }
        }
        if ($scope.Product.ProductType == 3 || $scope.Product.ProductType == 4 || $scope.Product.ProductType == 5) {
            if (!ValidateRequiredField($("#InterestType"), 'Interest Type required', 'after')) {
                flag = false;
            }

            if (!ValidateRequiredField($("#Frequency"), 'Frequency Type required', 'after')) {
                flag = false;
            }
        }
        if ($scope.Product.ProductType == 5 || $scope.Product.ProductType == "5" || $scope.Product.ProductType == 4 || $scope.Product.ProductType == "4") {
            if (!ValidateRequiredField($("#txtLatePaymentFees"), 'Late Payment charges Required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#PaymentType"), 'Payment Type Required', 'after')) {
                flag = false;
            }
            if ($scope.Product.ProductType == 5 || $scope.Product.ProductType == "5") {
                if (!ValidateRequiredField($("#ddlLoanType"), 'Loan Type Required', 'after')) {
                    flag = false;
                }
            }
        }

        //if ($scope.Product.ProductType == 3 || $scope.Product.ProductType == "3") {
        //    if (!ValidateRequiredField($("#ddlTimePeriod"), 'Time Period Required', 'after')) {
        //        flag = false;
        //    }
        //    if (!ValidateRequiredField($("#txtNoOfMonthsORYears"), 'Enter Months/Years', 'after')) {
        //        flag = false;
        //    }
        //}
        return flag;
    }

    $scope.SaveTerm = function () {

        var flag = true;
        flag = ValidateTermData();

        if (flag) {

            if ($scope.Term.TermId == '0000000-0000-0000-0000-000000000000' || $scope.Term.TermId == undefined || $scope.Term.TermId == "") {
                $scope.Term.CreatedBy = $cookies.getObject('User').UserId;
            }
            else {
                $scope.Term.ModifiedBy = $cookies.getObject('User').UserId;
            }

            if (($("#txtfrom").val('') != null && $("#txtfrom").val('') != undefined && $("#txtfrom").val('') != "") || ($("#txtTo").val('') != null && $("#txtTo").val('') != undefined && $("#txtTo").val('') != "")) {
                if ($scope.Term.TimePeriod == "1") {
                    $scope.Term.TotalFrom = ($scope.Term.From) * 30;
                    $scope.Term.TotalTo = ($scope.Term.To) * 30;

                }
                else if ($scope.Term.TimePeriod == "2") {
                    $scope.Term.TotalFrom = ($scope.Term.From) * 365;
                    $scope.Term.TotalTo = ($scope.Term.To) * 365;
                }
            }

            var SaveTerm = AppService.SaveData("Product", "SaveTerm", $scope.Term);
            SaveTerm.then(function (p1) {
                if (p1.data != null) {
                    $scope.Term = p1.data;
                    showToastMsg(1, "Term Details Saved Successfully")
                    RefreshDataTablefnDestroyTerm();
                    TermList();
                    $scope.ClearTerm();

                }
                else {
                    showToastMsg(3, 'Error in Saving Data')
                }
            })
        }
    }

    function ValidateTermData() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#txtfrom"), 'From required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtTo"), 'To required', 'after')) {
            flag = false;
        }
        if (($("#txtfrom").val() != null || $("#txtfrom").val() != undefined || $("#txtfrom").val() != "") && ($("#txtTo").val() != null || $("#txtTo").val() != undefined || $("#txtTo").val() != "")) {
            if (!CheckGreterNumber($("#txtfrom"), $("#txtTo"), 'To is Greater Then From', 'after')) {
                flag = false;
            }
        }
        if (!ValidateRequiredField($("#ddlTimePeriod"), 'TimePeriod required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#ddlTimePeriod"), 'TimePeriod required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtinterest"), 'InterestRate required', 'after')) {
            flag = false;
        }
        return flag;
    }

    $scope.ClearTerm = function () {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        $scope.Term = {};
    }

    TermList();

    function TermList() {

        $('#tblterm').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "Product/GetTermList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearchTerm").val() });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                        IntialPageControlTerm();
                    }
                });
            },
            "aoColumns": [{
                "mDataProp": "From",

            },
             {
                 "mDataProp": "To",
             },
             {
                 "mDataProp": "Time",
             },
             {
                 "mDataProp": "InterestRate",
             },
             {
                 "mDataProp": "TermId",
                 "mRender": function (data, type, full) {

                     return '<button class="btn btn-success btn-xs btn-flat btnedit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>  <button class="btn btn-danger btn-xs btn-flat btndelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> ';
                 },
                 "sClass": "text-center"

             }]

        });
    }

    $scope.SearchData = function () {
        $('#tblterm').dataTable().fnDraw();
    }

    $scope.SearchClearData = function () {
        $('#txtSearchTerm').val('');
        $('#tblterm').dataTable().fnDraw();
    }

    function GetTermDataById(Id) {

        var GetTermdata = AppService.GetDetailsById("Product", "GetTermDataById", Id);
        GetTermdata.then(function (p1) {
            if (p1.data != null) {
                $scope.Term = p1.data;
                $scope.Term.TimePeriod = p1.data.TimePeriod + "";
                // $("#ddlTimePeriod").val($scope.Term.TimePeriod)
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function IntialPageControlTerm() {

        $(".btnedit").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            //$("#Term").modal('show');
            $("#btntab2").click();
            GetTermDataById(ID)
        });

        $(".btndelete").click(function () {
            var ID = $(this).attr("Id");
            bootbox.dialog({
                message: "Are You Sure Want to Delete Term?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            var promiseDelete = AppService.DeleteData("Product", "DeleteTerm", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {

                                    RefreshDataTablefnDestroyTerm();
                                    toastr.remove();
                                    showToastMsg(1, "Term Deleted Successfully");
                                    $("#txtfrom").val('');
                                    $("#txtTo").val('');
                                    $("#ddlTimePeriod").val('');
                                    $("#txtinterest").val('');
                                    TermList();
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

    function RefreshDataTablefnDestroyTerm() {
        $("#tblterm").dataTable().fnDestroy();
    }
})