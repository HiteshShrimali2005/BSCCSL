angular.module("BSCCL").controller('RptEmployeePerfomanceController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        GetEmployeePerfomanceList();
    }

    $scope.SearchEmployeePerfomanceList = function () {
            GetEmployeePerfomanceList();
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

    GetEmployeePerfomanceList();
    function GetEmployeePerfomanceList() {
        let startDate = $("#txtStartDateforSearch").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtEndDateforSearch").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }


        var getdata = AppService.GetDataByQryStr("Report", "RptEmployeePerfomanceList", $scope.UserBranch.BranchId, ($scope.EmployeeName ? $scope.EmployeeName : ""), startDate, endDate)
        getdata.then(function (p1) {
            if (p1.data) {
                $scope.EmployeeProductListdata = p1.data.rptlist;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    //GetEmployeePerfomanceList()
    //function GetEmployeePerfomanceList() {

    //    $('#tblEmployeePerfomanceList').dataTable({
    //        "bFilter": false,
    //        "processing": false,
    //        "bInfo": true,
    //        "bServerSide": true,
    //        "bLengthChange": true,
    //        "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
    //        "bSort": false,
    //        "bDestroy": true,
    //        searching: false,
    //        dom: 'l<"floatRight"B>frtip',
    //        buttons: [
    //            {
    //                extend: 'pdf',
    //                //footer: true
    //            },
    //            {
    //                extend: 'print',
    //                //footer: true
    //            }
    //        ],
    //        "sAjaxSource": urlpath + "/Report/RptEmployeePerfomanceList",
    //        "fnServerData": function (sSource, aoData, fnCallback) {
    //            aoData.push({ "name": "EmployeeName", "value": $scope.EmployeeName });
    //            aoData.push({ "name": "fromDate", "value": $("#txtStartDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtStartDateforSearch").val()) : "" });
    //            aoData.push({ "name": "toDate", "value": $("#txtEndDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtEndDateforSearch").val()) : "" });
    //            aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
    //            $.ajax({
    //                "dataType": 'json',
    //                "type": "POST",
    //                "url": sSource,
    //                "data": aoData,
    //                "success": function (json) {
    //                    fnCallback(json);
    //                }
    //            });
    //        },
    //        "aoColumns": [
    //            {
    //                "mDataProp": "EmployeeCode"
    //            },
    //            {
    //                "mDataProp":"EmployeeName"
    //            },
    //            {
    //                "mDataProp": "ProductName",
    //            },
    //            {
    //                "mDataProp": "ProductTypeName",
    //            },
    //            {
    //                "mDataProp": "TotalCount",
    //            },

    //            {
    //                "mRender": function (data, type, full) {
    //                    if (full.ProductType == 1) {
    //                        return data = $filter('currency')(full.Balance, '₹ ', 2);
    //                    }
    //                    else {
    //                        return data = $filter('currency')(full.Amount, '₹ ', 2);
    //                    }
    //                },
    //                "sClass": "text-right"
    //            },

    //        ]
    //    });
    //}

    $scope.SearchClearData = function () {
        $scope.EmployeeName = '';
        $("#txtStartDateforSearch").val("");
        $("#txtEndDateforSearch").val("");
        $("#txtStartDateforSearch").data("DateTimePicker").date(null);
        $("#txtEndDateforSearch").data("DateTimePicker").date(null);

        //$('#tblEmployeePerfomanceList').dataTable().fnDraw();
        GetEmployeePerfomanceList();

    }
});