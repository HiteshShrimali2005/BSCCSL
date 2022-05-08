angular.module("BSCCL").controller('RptAgentCustomerController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblAgentCustomerList').dataTable().fnDraw();
    }
    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });


    $scope.SearchAgentCustomerList = function () {
        $('#tblAgentCustomerList').dataTable().fnDraw();

        //if ($scope.AgentName != null && $scope.AgentName != undefined && $scope.AgentName != "") {
        //    $('#tblAgentCustomerList').dataTable().fnDraw();

        //}
        //else {
        //    showToastMsg(3, 'Enter Agent Name');
        //}
    }

    GetAgentCustomerList()
    function GetAgentCustomerList() {

        $('#tblAgentCustomerList').dataTable({
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
            "sAjaxSource": urlpath + "/Report/GetAllAgentCustomerList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $scope.AgentName });
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
                        fnCallback(json);
                    }
                });
            },
            "aoColumns": [
            {
                "mDataProp": "ClienId",
            },
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
               "mDataProp": "CustomerAddress",
               "sWidth": '30%'
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

            ]
        });
    }

    $scope.SearchClearData = function () {
        $scope.AgentName = '';
        // $("#txtAccountlistsearch").val('');
        $scope.ProductName = '';
        $("#txtStartDateforSearch").val("");
        $("#txtEndDateforSearch").val("");
        $('#tblAgentCustomerList').dataTable().fnDraw();


    }
});