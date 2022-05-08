angular.module("BSCCL").controller('RptAgentPerfomanceController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        GetAgentPerfomanceList();
    }

    $scope.SearchAgentPerfomanceList = function () {
        GetAgentPerfomanceList();
    }

    GetAllAgentList();
    var flag1 = false;
    function GetAllAgentList() {
        AppService.GetDetailsById("Report", "GetEmployeeListByBranchId", $scope.UserBranch.BranchId).then(function (p1) {
            if (p1.data) {
                $scope.AgentList = p1.data;

                if (!flag1) {
                    $('#txtAgentlistsearch').typeahead({
                        source: $scope.AgentList,
                        display: 'AgentName',
                        val: 'AgentId'
                    });
                    flag1 = true;
                } else {
                    $("#txtAgentlistsearch").data('typeahead').source = $scope.AgentList;
                }
            }
        })
    }

    GetAgentPerfomanceList();
    function GetAgentPerfomanceList() {
        let startDate = $("#txtStartDateforSearch").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtEndDateforSearch").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }


        var getdata = AppService.GetDataByQryStr("Report", "RptAgentPerfomanceList", $scope.UserBranch.BranchId, ($scope.AgentName ? $scope.AgentName : ""), startDate, endDate)
        getdata.then(function (p1) {
            if (p1.data) {
                $scope.AgentPerfomancedata = p1.data.rptlist;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }



    $scope.SearchClearData = function () {
        $scope.AgentName = '';
        $("#txtStartDateforSearch").val("");
        $("#txtEndDateforSearch").val("");
        $("#txtStartDateforSearch").data("DateTimePicker").date(null);
        $("#txtEndDateforSearch").data("DateTimePicker").date(null);
        GetAgentPerfomanceList();
    }
});