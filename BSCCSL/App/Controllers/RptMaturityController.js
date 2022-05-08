angular.module("BSCCL").controller('RptMaturityController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblMaturityList').dataTable().fnDraw();
    }

    $scope.SearchMaturutyList = function () {
        $('#tblMaturityList').dataTable().fnDraw();

    }
    var TotalMaturityAmount = 0;
    GetMaturityList()
    function GetMaturityList() {

        $('#tblMaturityList').dataTable({
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
            "sAjaxSource": urlpath + "/Report/GetMaturityList",
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
                        TotalMaturityAmount = json.TotalMaturityAmount;
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
                "mDataProp": "CustomerName",
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
                   return data = $filter('date')(full.MaturityDate, 'dd/MM/yyyy');
               },
               "sClass": "text-center"
           },

            {
                "mRender": function (data, type, full) {
                    return data = $filter('currency')(full.MaturityAmount, '₹ ', 2);
                },
                "sClass": "text-right"
            }
            //{
            //    "sTitle": "Action",
            //    "mRender": function (data, type, full) {
            //        var str = '';

            //        if (full.ProductType == 4) {
            //            var acNo = btoa(full.AccountNumber);
            //            str += '<button onclick="openMaturityTransactionModal(\'' + full.AccountNumber + '\',' + full.MaturityAmount + ',' + full.Balance + ')" class="btn btn-primary btn-xs btnTransaction  btn-flat" title="View Details"><span class="glyphicon glyphicon-money"></span> <i class="glyphicon glyphicon-th-list"> </i> View Details</button>'
            //        }
            //        return str
            //    },
            //    "sClass": "text-center",
            //    "sWidth": "150px"

            //}
            ],
            "footerCallback": function (row, aoData, start, end, display) {
                if (aoData.length > 0) {
                    //// Total over this page

                    //// Update footer
                    $("#labelTotal").show();
                    $("#lblTotalAmount").show();

                    $("#lblTotalAmount").html($filter('currency')(TotalMaturityAmount, '₹', 2))
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
        $('#tblMaturityList').dataTable().fnDraw();
    }


  

});

function openMaturityTransactionModal(AccountNumber, TotalMaturityAmount,TotalPaidBalance) {
    var floatTotalMaturityAmount = parseFloat(TotalMaturityAmount);
    var floatTotalPaidBalance = parseFloat(TotalPaidBalance);
    var PendingAmount = '';
    PendingAmount = parseFloat(floatTotalMaturityAmount - floatTotalPaidBalance);
    $('#AccountNumber').text(AccountNumber);
    $('#TotalMaturityAmount').text(floatTotalMaturityAmount.toFixed(2));
    $('#TotalPaidBalance').text(floatTotalPaidBalance.toFixed(2));
    $('#PendingAmount').text(PendingAmount.toFixed(2));
    $('#MaturityTransactionDetails').modal('show');

}
