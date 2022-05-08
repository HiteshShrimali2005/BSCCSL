angular.module("BSCCL").controller('RptPrematureProductListController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblPrematureProductList').dataTable().fnDraw();
    }

    $scope.SearchPrematueProductList = function () {
        $('#tblPrematureProductList').dataTable().fnDraw();

    }

    GetPrematureProductList()
    function GetPrematureProductList() {

        $('#tblPrematureProductList').dataTable({
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
            "sAjaxSource": urlpath + "/Report/RptPrematureProductList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $scope.CustomerName });
                aoData.push({ "name": "PrematurePercentage", "value": $scope.PrematurePercentage });
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
                    "mDataProp": "CustomerName",
                },
                {
                    "mDataProp": "AccountNumber",
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(full.PrematurePercentage, ' ', 2);
                    },
                    "sClass": "text-center"

                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('date')(full.OpeningDate, 'dd/MM/yyyy');
                    },
                    "sClass": "text-center"
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('date')(full.PrematureDate, 'dd/MM/yyyy');
                    },
                    "sClass": "text-center"
                },

            ],
        });

    }

    $scope.SearchClearData = function () {
        $scope.CustomerName = '';
        $scope.PrematurePercentage = '';
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $('#tblPrematureProductList').dataTable().fnDraw();
    }




});

