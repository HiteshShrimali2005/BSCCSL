angular.module("BSCCL").controller('AccountListController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {

    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')
    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });


    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblAccountList').dataTable().fnDraw();
    }

    $scope.SearchAccountList = function () {
        //if ($scope.AccountNum != null && $scope.AccountNum != undefined && $scope.AccountNum != "") {
        //    $('#tblAccountList').dataTable().fnDraw();
        //}
        //else {
        //    showToastMsg(3, 'Enter Account Number');
        //}
        $('#tblAccountList').dataTable().fnDraw();

    }

    GetAccountList()
    function GetAccountList() {

        $('#tblAccountList').dataTable({
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
            "sAjaxSource": urlpath + "/Report/GetAccountList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $scope.AccountNum });
                aoData.push({ "name": "ProductName", "value": $scope.ProductName });
                aoData.push({ "name": "InterestRate", "value": $scope.InterestRate });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "fromDate", "value": $("#txtStartDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtStartDateforSearch").val()) : "" });
                aoData.push({ "name": "toDate", "value": $("#txtEndDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtEndDateforSearch").val()) : "" });
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
            "aoColumns": [
                {
                    "mDataProp": "ClientId",
                },
                {
                    "mDataProp": "CustomerName",
                },
                {
                    "mDataProp": "MobileNo",
                },
                {
                    "mDataProp": "AccountNumber",
                },
                {
                    "mDataProp": "ProductTypeName",
                },
                {
                    "mDataProp": "InterestRate",
                },
                {
                    "mDataProp": "Balance",
                    "mRender": function (data, type, full) {
                        return $filter('currency')(full.Balance, '');
                    },
                    "sClass": "right"
                },
                {
                    "mDataProp": "Openingdate",
                    "mRender": function (data, type, full) {
                        return $filter('date')(data, 'dd/MM/yyyy');
                    },
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
                        .column(6, { page: 'current' })
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(b);
                        });

                    //// Update footer
                    $("#labelTotal").show();
                    $("#lblTotalAmount").show();
                    $("#lblTotalAmount").html($filter('currency')(pageTotal, '₹', 2))
                }
                else {
                    $("#lblTotalAmount").hide();
                    $("#labelTotal").hide();

                }
            }
        });
    }

    $scope.SearchClearData = function () {
        $scope.AccountNum = '';
        $scope.ProductName = '';
        $scope.InterestRate = '';
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');

        // $("#txtAccountlistsearch").val('');
        $('#tblAccountList').dataTable().fnDraw();


    }
});