angular.module("BSCCL").controller('TransactionListController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {

    $scope.SearchTransactionList = function () {

        if ($scope.AccountnoTransactionList != undefined && $scope.AccountnoTransactionList != "") {
            GetHolderDetail()
            //    GetTransactionList();
        }
        else {
            // $scope.SectionHide = true
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

    function GetHolderDetail() {
        var getholder = AppService.GetDetailsById("Customer", "GetHolderData", $scope.AccountnoTransactionList);

        getholder.then(function (p1) {
            if (p1.data.AccountDetail != null) {
                $scope.SectionHide = false
                $scope.HolderData = p1.data.AccountDetail;
                $scope.HolderData.details = p1.data.details
                if ($scope.HolderData.details.length > 0) {
                    $scope.show = 1;
                    GetTransactionList();
                }
            }
            else {
                $scope.SectionHide = true
                showToastMsg(3, "No Account found. Please enter correct Account No for transaction details")
                GetTransactionList();
            }
        });
    }

    function GetTransactionList() {
        $('#tblTransaction').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "iDisplayLength": 20,
            "bSort": false,
            "bDestroy": true,
            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'copyHtml5',
                    exportOptions: {
                        columns: ':contains("Office")'
                    }
                },
                'excelHtml5',
                'csvHtml5',
                'pdfHtml5'
            ],
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
                    "sClass": "align-center"
                },
                {
                    "mDataProp": "Amount",
                    "mRender": function (data, type, full) {
                        return $filter('currency')(data, ' ', 2);
                    },
                    "sClass": "right"
                },
                {
                    "mDataProp": "Balance",
                    "mRender": function (data, type, full) {
                        return $filter('currency')(data, ' ', 2);
                    },
                    "sClass": "right"
                },
                {
                    "mDataProp": "TypeName",
                    "sClass": "align-center"
                },
                {
                    "mDataProp": "TransactionId",
                    "mRender": function (data, type, full) {
                        return '<button class="btn btn-success btn-xs btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span>Edit </button>'
                    },
                }
            ]
        });
    }

    function InitPageLoad() {
        $("#btnEdit").click(function () {
            var ID = $(this).attr("Id");
        })
    }


})