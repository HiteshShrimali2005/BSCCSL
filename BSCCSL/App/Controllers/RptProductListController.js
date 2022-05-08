angular.module("BSCCL").controller('RptProductListController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblProductList').dataTable().fnDraw();
    }

    $scope.Search = function () {
        $('#tblProductList').dataTable().fnDraw();

    }

    GetProductList()
    function GetProductList() {

        $('#tblProductList').dataTable({
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
            "sAjaxSource": urlpath + "/Report/GetProductList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $scope.CustomerName });
                aoData.push({ "name": "AgentName", "value": $scope.AgentName });
                aoData.push({ "name": "ProductName", "value": $scope.ProductName });
                aoData.push({ "name": "Status", "value": $("#ddlsearchStatus").val() });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "fromDate", "value": $("#txtStartDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtStartDateforSearch").val()) : "" });
                aoData.push({ "name": "toDate", "value": $("#txtEndDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtEndDateforSearch").val()) : "" });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
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
                    "mDataProp": "Amount",
                    "sClass": "text-right"
                },
                {
                    "mDataProp": "Balance",
                    "sClass": "text-right"
                },


            ],
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
                        .column(7, { page: 'current' })
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(b);
                        });

                    //// Update footer
                    $("#lblTotalAmount").show();
                    $("#labelTotal").show();

                    $(api.column(7).footer()).html(
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
        $('#tblProductList').dataTable().fnDraw();
    }
});