angular.module("BSCCL").controller('RptEmployeeProductController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblEmployeeProductList').dataTable().fnDraw();
    }

    $scope.SearchEmployeeProductList = function () {
        $('#tblEmployeeProductList').dataTable().fnDraw();
    }

    var TotalAmount = 0;
    GetEmployeeProductList()
    function GetEmployeeProductList() {

        $('#tblEmployeeProductList').dataTable({
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
            "sAjaxSource": urlpath + "/Report/GetEmployeeProductList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "EmployeeName", "value": $scope.EmployeeName });
                aoData.push({ "name": "ProductName", "value": $scope.ProductName });
                aoData.push({ "name": "fromDate", "value": $("#txtStartDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtStartDateforSearch").val()) : "" });
                aoData.push({ "name": "toDate", "value": $("#txtEndDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtEndDateforSearch").val()) : "" });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        TotalAmount = json.TotalAmount;
                        fnCallback(json);
                    }
                });
            },
            "aoColumns": [
                {
                    "mDataProp": "EmployeeName",
                },
                {
                    "mDataProp": "PhoneNumber",
                },
                {
                    "mDataProp": "ClienId",
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
                    "mDataProp": "ProductTypeName",
                },

                {
                    "mRender": function (data, type, full) {
                        return data = $filter('date')(full.OpeningDate, 'dd/MM/yyyy');
                    },
                },
                {
                    "mRender": function (data, type, full) {
                        if (full.ProductType == 1) {
                            return data = $filter('currency')(full.Balance, '₹ ', 2);
                        }
                        else {
                            return data = $filter('currency')(full.Amount, '₹ ', 2);
                        }
                    },
                    "sClass": "text-right"
                },

            ],
            "footerCallback": function (row, aoData, start, end, display) {
                if (aoData.length > 0) {
                    //// Total over this page

                    //// Update footer
                    $("#labelTotal").show();
                    $("#lblTotalAmount").show();

                    $("#lblTotalAmount").html($filter('currency')(TotalAmount, '₹', 2))
                }
                else {
                    $("#lblTotalAmount").hide();
                    $("#labelTotal").hide();

                }
            }

        });
    }

    $scope.SearchClearData = function () {
        $scope.EmployeeName = '';
        $scope.ProductName = '';
        $("#txtStartDateforSearch").val("");
        $("#txtEndDateforSearch").val("");
        // $("#txtAccountlistsearch").val('');
        $('#tblEmployeeProductList').dataTable().fnDraw();


    }
});