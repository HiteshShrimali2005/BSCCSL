angular.module("BSCCL").controller('ActivateRDController', function ($scope, AppService, $state, $cookies, $filter, $rootScope) {

    $scope.UserBranch.ShowBranch = true;
    $scope.UserBranch.Enabled = true;

    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblRDActivate').dataTable().fnDraw();
    }



    GetList();



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
        GetList();
    }

    $scope.ClearForm = function () {
        $scope.DDSPaymentListModel = new Object();
        //$("#txtCustomerName").val('');
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $scope.ClearSearch = function () {
        $("#txtCustomerName").val('');
        $("#txtAccountNumber").val('');
        GetList();
    }



    function GetList() {

        $('#tblRDActivate').dataTable({
            "bFilter": false,
            "processing": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": true,
            "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
            "bSort": false,
            "bDestroy": true,
            searching: false,
            //"scrollX": true,
            "sAjaxSource": urlpath + "ActivateRD/GetList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "CustomerName", "value": $("#txtCustomerName").val() });
                aoData.push({ "name": "AccountNumber", "value": $("#txtAccountNumber").val() });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                        IntialPageControl();
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
                    "sTitle": "Account Number",
                    "mDataProp": "AccountNumber",
                },

                {
                    "sTitle": "Action",
                    "mDataProp": "CustomerProductId",
                    "mRender": function (data, type, full) {

                        var str = ''
                        str += ' <button class="btn btn-primary btn-xs btn-flat btnActivateRD" Id="' + data + '" AccountNumber="' + full.AccountNumber + '"  title="Activate RD"><span class="glyphicon glyphicon-ok"></span> Activate RD</button> '
                        return str;
                    },
                    "sClass": "text-center",
                    "sWidth": "150px"
                }
            ]

        });
    }

    function IntialPageControl() {
        $(".btnActivateRD").click(function () {
            var AccountNumber = $(this).attr("AccountNumber");
            bootbox.dialog({
                message: "Are you sure want to Activate this RD?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            var details = AppService.GetDetailsById("ActivateRD", "UnFreezedRDAccount", AccountNumber);
                            details.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {
                                    showToastMsg(1, "RD Activated Successfully");
                                    GetList();
                                }
                                else {
                                    showToastMsg(3, "Error Occured Activating RD");
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


})