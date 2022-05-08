angular.module("BSCCL").controller('DDSPaymentController', function ($scope, AppService, $state, $cookies, $filter, $rootScope) {

    $scope.UserBranch.ShowBranch = true;
    $scope.UserBranch.Enabled = true;

    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');
    $scope.DDSPaymentListModel = new Object();

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblDDSPayment').dataTable().fnDraw();
    }

    $(".datepicker").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });


    GetDDSPaymentList();


    function ValidateDDSPayment() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#txtSavingAccountNumber"), 'Saving Account Number is required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtAmount"), 'Amount is required', 'after')) {
            flag = false;
        }



        return flag;
    }

    $scope.AddDDSPayment = function () {
        var flag = true;
        flag = ValidateDDSPayment();

        if (flag) {
            var SaveTerm = AppService.SaveData("DDSPayment", "AddPayment", $scope.DDSPaymentListModel);
            SaveTerm.then(function (p1) {
                if (p1.data != null) {
                    $('#AddDDSPayment').modal('hide');
                    GetDDSPaymentList();
                    showToastMsg(1, "Data Saved Successfully")
                    $scope.ClearForm();
                }
                else {
                    showToastMsg(3, 'Error in Saving Data')
                }
            })
        }
    }

    $scope.Search = function () {
        GetDDSPaymentList();
    }

    $scope.ClearForm = function () {
        $scope.DDSPaymentListModel = new Object();
        //$("#txtCustomerName").val('');
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $scope.ClearSearch = function () {
        $scope.DDSPaymentListModel = new Object();
        $("#txtCustomerName").val('');
        $("#txtAgentName").val('');
        GetDDSPaymentList();
    }



    function GetDDSPaymentList() {

        $('#tblDDSPayment').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            'bAutoWidth': false,
            //"scrollX": true,
            "sAjaxSource": urlpath + "/DDSPayment/GetDDSPaymentList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "CustomerName", "value": $("#txtCustomerName").val() });
                aoData.push({ "name": "AgentName", "value": $("#txtAgentName").val() });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                        IntialPageControlDDSPayment();
                    }
                });
            },
            "aoColumns": [
                {
                    "sTitle": "Client Id",
                    "mDataProp": "ClienId",
                },

                {
                    "sTitle": "Customer Name",
                    "mDataProp": "CustomerName",
                    "mRender": function (data, type, full) {
                        return '<a href="/App/CustomerProduct?CustomerId=' + full.CustomerId + '" title="View" target="_blank">' + full.CustomerName + '</a>'
                    },

                },
                {
                    "sTitle": "Agent Name",
                    "mDataProp": "AgentName",
                },

                {
                    "sTitle": "RD Account Number",
                    "mDataProp": "RDAccountNumber",
                },

                {
                    "sTitle": "Saving Account Number",
                    "mDataProp": "SavingAccountNumber",
                },

                {
                    "sTitle": "RDAmount",
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(full.RDAmount, '₹ ', 2);
                    },
                    "sClass": "text-right"
                },


                {
                    "sTitle": "Action",
                    "mDataProp": "SavingAccountCustomerProductId",
                    "mRender": function (data, type, full) {

                        var str = ''
                        str += ' <button class="btn btn-primary btn-xs btn-flat btnAddPayment" Id="' + data + '" SavingAccountNumber="' + full.SavingAccountNumber + '"  title="Pay"><span class="glyphicon glyphicon-ok"></span> PAY Amount</button> '
                        return str;
                    },
                    "sClass": "text-center",
                    "sWidth": "150px"
                }
            ]

        });
    }

    function IntialPageControlDDSPayment() {
        $scope.DDSPaymentListModel = new Object();
        $(".btnAddPayment").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $scope.DDSPaymentListModel.SavingAccountCustomerProductId = ID;
            var SavingAccountNumber = $(this).attr("SavingAccountNumber");
            $("#txtSavingAccountNumber").val(SavingAccountNumber);
            $scope.DDSPaymentListModel.SavingAccountNumber = $("#txtSavingAccountNumber").val();
            $("#AddDDSPayment").modal('show');
        });

    }


})