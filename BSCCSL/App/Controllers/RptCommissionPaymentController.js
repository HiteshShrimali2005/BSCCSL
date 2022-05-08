angular.module("BSCCL").controller('RptCommissionPaymentController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblCommissionPayment').dataTable().fnDraw();
    }

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $scope.SearchCommissionList = function () {

        if ($scope.AgentName != null && $scope.AgentName != undefined && $scope.AgentName != "") {
            $('#tblCommissionPayment').dataTable().fnDraw();
        }
        else {
            showToastMsg(3, 'Enter Agent Name');
        }
    }

    GetAllAgentList();

    var flag1 = false;
    function GetAllAgentList() {
        AppService.GetDetailsById("User", "GetAgentListByBranchId", $scope.UserBranch.BranchId).then(function (p1) {
            if (p1.data) {
                $scope.AgentList = p1.data;

                if (!flag1) {
                    $('#txtAccountlistsearch').typeahead({
                        source: $scope.AgentList,
                        display: 'AgentName',
                        val: 'AgentId'
                    });
                    flag1 = true;
                } else {
                    $("#txtAccountlistsearch").data('typeahead').source = $scope.AgentList;
                }
            }
        })
    }

    $scope.TotalPaidCommission = 0;
    GetCommissionPaymentList()
    function GetCommissionPaymentList() {

        $('#tblCommissionPayment').dataTable({
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
            "sAjaxSource": urlpath + "/Report/GetAllCommissionPaymentList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "AgentName", "value": $scope.AgentName });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "fromDate", "value": ddmmyyTommdddyy($("#txtStartDateforSearch").val()) });
                aoData.push({ "name": "toDate", "value": ddmmyyTommdddyy($("#txtEndDateforSearch").val()) });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        $scope.TotalPaidCommission = json.PaidCommission;

                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                        fnCallback(json);
                    }
                });
            },
            "aoColumns": [
                 {
                     "mDataProp": "AgentName",
                 },
                  {
                      "mDataProp": "AgentMobileNo",
                  },
                  {
                      "mDataProp": "CustomerName",
                  },
                  {
                      "mDataProp": "MobileNo",
                  },
                  {
                      "mDataProp": "ProductTypeName",
                  },
              {
                  "mRender": function (data, type, full) {
                      return $filter('date')(full.PaidDate, 'dd/MM/yyyy');
                  },
              },
              {
                  "sClass": "right",
                  "mRender": function (data, type, full) {
                      return $filter('currency')(full.PaidAmount, '₹ ', 2);
                  },

              },
              {
                  "mDataProp": "PaidName",
              },
            ]
        });
    }

    $scope.Reset = function () {
        $scope.AgentName = null;
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $scope.TotalPaidCommission = 0;
        $('#tblCommissionPayment').dataTable().fnDraw();
    }
});