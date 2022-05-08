angular.module("BSCCL").controller('RptProductInstallmentController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblProductInstallmentList').dataTable().fnDraw();
    }

    $scope.SearchPendingInstallmentList = function () {
        $('#tblProductInstallmentList').dataTable().fnDraw();

    }

    $scope.PaidTotal = 0;
    $scope.PendingTotal = 0;
    GetProductInstallmentList()
    function GetProductInstallmentList() {

        $('#tblProductInstallmentList').dataTable({
            "bFilter": false,
            "processing": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": true,
            "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
            "bSort": false,
            "bDestroy": true,
            searching: false,
            dom: 'l<"floatRight"B>frtip',
            buttons: [
                {
                    extend: 'pdf',
                    //footer: true
                },
                {
                    extend: 'print',
                    //footer: true
                }
            ],
            "sAjaxSource": urlpath + "/Report/GetProductInstallmentList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $scope.CustomerName });
                aoData.push({ "name": "AgentName", "value": $scope.AgentName });
                aoData.push({ "name": "ProductName", "value": $scope.ProductName });
                aoData.push({ "name": "Status", "value": $("#ddlsearchStatus").val() });
                aoData.push({ "name": "ProductStatus", "value": $("#ddlsearchProductStatus").val() });
                aoData.push({ "name": "PaymentType", "value": $("#ddlPaymentType").val() });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "fromDate", "value": $("#txtStartDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtStartDateforSearch").val()) : "" });
                aoData.push({ "name": "toDate", "value": $("#txtEndDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtEndDateforSearch").val()) : "" });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        $scope.PaidTotal = json.PaidTotal;
                        $scope.PendingTotal = json.PendingTotal;
                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                        fnCallback(json);
                    }
                });
            },
            "aoColumns": [
                {
                    "mDataProp": "ClienId",

                },
                {
                    "mDataProp": "AccountNumber",

                },
                {
                    "mDataProp": "AgentName",
                },

                {
                    "mDataProp": "CustomerName",
                },
                {
                    "mDataProp": "MobileNo",
                },
                {
                    "mDataProp": "ProductName",
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('date')(full.OpeningDate, 'dd/MM/yyyy');
                    },
                    "sClass": "text-center"
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('date')(full.InstallmentDate, 'dd/MM/yyyy');
                    },
                    "sClass": "text-center"
                },
                //{
                //    "mRender": function (data, type, full) {
                //        return data = $filter('currency')(full.Amount, '', 2);
                //    },
                //    "sClass": "text-right"
                //},
                {
                    "mDataProp": "Amount",
                    "sClass": "text-right"
                },

                {
                    "mRender": function (data, type, full) {
                        if (full.IsPaid == true) {
                            return '<span class="label label-success">Paid</span>';
                        } else {
                            return '<span class="label label-danger">Pending</span>';
                        }
                    },
                    "sClass": "text-center"
                },
                {
                    "mRender": function (data, type, full) {
                        if (full.ProductCurrentStatus == "5") {
                            return '<span class="label" style="background-color:blue">Completed</span>';
                        } else {
                            return '<span class="label label-warning">Running</span>';
                        }
                    },
                    "sClass": "text-center"

                },

            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                if (aData.ProductCurrentStatus == "5" && aData.IsPaid == false) {
                    $('td', nRow).css('background-color', '#efbdb9');
                }
            },
            "footerCallback": function (row, aoData, start, end, display) {
                var api = this.api();
                var intVal = function (i) {
                    return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '') * 1 :
                        typeof i === 'number' ?
                            i : 0;
                };
                if (aoData.length > 0) {

                    //// Total over this page
                    var pageTotal = api
                        .column(8, { page: 'current' })
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(b);
                        });

                    //// Update footer
                    $("#lblTotalAmount").show();
                    $("#labelTotal").show();

                    $(api.column(8).footer()).html(
                        '₹' + $filter('currency')(pageTotal, '', 2) + ''
                    );

                }
                else {
                    $("#lblTotalAmount").hide();
                    $("#labelTotal").hide();
                }
            }

        });
    }
    
    $scope.SearchClearData = function () {
        $scope.CustomerName = '';
        $scope.AgentName = '';
        $scope.ProductName = '';
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $("#ddlsearchStatus").val('');
        $("#ddlsearchProductStatus").val('');
        $("#ddlPaymentType").val('');
        $('#tblProductInstallmentList').dataTable().fnDraw();
    }
});