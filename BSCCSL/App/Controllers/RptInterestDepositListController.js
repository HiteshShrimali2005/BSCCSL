﻿angular.module("BSCCL").controller('RptInterestDepositListController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblInterestDepositist').dataTable().fnDraw();
    }

    $scope.SearchInterestDepositList = function () {
        $('#tblInterestDepositist').dataTable().fnDraw();

    }
    var TotalInterestAmount = 0;
    GetInterestDepositList()
    function GetInterestDepositList() {

        $('#tblInterestDepositist').dataTable({
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
            "sAjaxSource": urlpath + "/Report/RptInterestDepositList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $scope.CustomerName });
                aoData.push({ "name": "ProductName", "value": $scope.ProductName });
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
                        TotalInterestAmount = json.TotalInterest;
                        fnCallback(json);
                    }
                });
            },
            "aoColumns": [

                {
                    "mDataProp": "ClienId",
                },
                {
                    "mDataProp": "CustomerName",
                },
                {
                    "mDataProp": "AccountNumber",
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(full.TotalInterest, ' ', 2);
                    },
                    "sClass": "text-center"

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
                    "sClass": "text-center"
                },

            ],
            "footerCallback": function (row, aoData, start, end, display) {
                if (aoData.length > 0) {
                    //// Total over this page

                    //// Update footer
                    $("#labelTotal").show();
                    $("#lblTotalAmount").show();
                    $("#lblTotalAmount").html($filter('currency')(TotalInterestAmount, '₹', 2))
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
        $scope.ProductName = '';
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $('#tblInterestDepositist').dataTable().fnDraw();
    }
});

