angular.module("BSCCL").controller('RptAgentHierarchyController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    var AgentHierarchyReport;

    var currentTime = new Date();
    var Eventmindata = parseInt(currentTime.getFullYear()) - 20;
    var Eventmaxdata = parseInt(currentTime.getFullYear());
    $scope.min = new Date(Eventmindata + '-01-01');
    $scope.max = new Date(Eventmaxdata + '-01-01');
    $scope.SearchYear = '';

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        AgentHierarchyReport.draw();
    }

    $scope.SearchAgentList = function () {
        AgentHierarchyReport.draw();
    }

    GetAgentHierarchyList()

   // GetAllAgentList();

    //var flag1 = false;
    //function GetAllAgentHirarchyList() {
    //    AppService.GetDetailsById("User", "RptAgentHierarchy", $scope.UserBranch.BranchId).then(function (p1) {
    //        if (p1.data) {
    //            $scope.AgentList = p1.data;

    //            if (!flag1) {
    //                $('#txtAccountlistsearch').typeahead({
    //                    source: $scope.AgentList,
    //                    display: 'AgentName',
    //                    val: 'AgentId'
    //                });
    //                flag1 = true;
    //            } else {
    //                $("#txtAccountlistsearch").data('typeahead').source = $scope.AgentList;
    //            }
    //        }
    //    })
    //}


    function GetAgentHierarchyList() {

        AgentHierarchyReport = $('#tblAgentHierarchyList').DataTable({
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
            "sAjaxSource": urlpath + "/AgentCommission/RptAgentHierarchy",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "AgentName", "value": $("#txtAgentName").val() });
                //aoData.push({ "name": "sSearch", "value": $("#txtCustomerNameSearch").val() });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                //aoData.push({ "name": "fromDate", "value": $scope.WeekStartDate });
                //aoData.push({ "name": "toDate", "value": $scope.WeekEndDate });
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
                     "title": "Agent Name",
                     "data": "AgentName"
                 },
                
            {
                "title": "SR1",
                "data": "SR1",
            },
            {
                "title": "SR2",
                "data": "SR2",
            },
            {
                "title": "SR3",
                "data": "SR3",
            }
            ]
        });
    }



    $scope.SearchClearData = function () {
        $("#txtAgentName").val("");
        AgentCommissionReport.draw();
    }

});