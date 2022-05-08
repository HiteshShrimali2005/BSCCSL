angular.module("BSCCL").controller('RptBalanceSheetController', function ($scope, $state, $cookies, $filter, AppService, $http, $location, $rootScope) {

    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    //$(".datepicker").datetimepicker({
    //    format: 'DD/MM/YYYY',
    //    useCurrent: true,
    //});

    var Total = 0;

    $rootScope.ChangeBranch = function () {
        $scope.BranchCode = $filter('filter')($scope.BranchList, { BranchId: $scope.UserBranch.BranchId })[0].BranchCode;
        $cookies.put('Branch', $scope.UserBranch.BranchId);
        $('#tblRptProductWiseBalance').dataTable().fnDraw();
    }

    //$("#txtStartDateforSearch").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy'));
    //$("#txtEndDateforSearch").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy'));

    //GetProductWiseBalance();

    function GetProductWiseBalance() {
        $('#tblRptProductWiseBalance').dataTable({
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
            "sAjaxSource": urlpath + "Report/GetProductwiseBalanceList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "fromDate", "value": ddmmyyTommdddyy($("#txtStartDateforSearch").val()) });
                aoData.push({ "name": "toDate", "value": ddmmyyTommdddyy($("#txtEndDateforSearch").val()) });
                aoData.push({ "name": "FinYear", "value": $scope.FinYear });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        Total = json.Total
                        fnCallback(json);
                    }
                });
            },
            "footerCallback": function (row, data, start, end, display) {
                var api = this.api(), data;
                $(api.column(2).footer()).html($filter('currency')(Total, '', 2));
            },
            "aoColumns": [
                {
                    "mDataProp": "ProductName",

                },
                {
                    "mDataProp": "ProductTypeName",
                    "mRender": function (data) {
                        if (data == 0) {
                            return '';
                        } else {
                            return data;
                        }
                    },
                },
                {
                    "mDataProp": "Balance",
                    "mRender": function (data, type, full) {
                        return $filter('currency')(data, '', 2);
                    },
                    "sClass": "right"
                }
            ]
        });
    }

    $scope.SearchRptProductwiseBalanceList = function () {
        if ($scope.FinYear != undefined && $scope.FinYear != '') {
            GetProductWiseBalance();
            //$('#tblRptProductWiseBalance').dataTable().fnDraw();
        }
        else {
            showToastMsg(3, 'Please select Financial Year')
        }
    }

    $scope.ResetRptProductwiseBalanceList = function () {
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $scope.FinYear = '';
        $('#tblRptProductWiseBalance').dataTable().fnDraw();
    }

});