angular.module("BSCCL").controller('RptRDFDDueInstallmentController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')


    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    var dates = ThisWeekStart_Enddate();
    $scope.WeekStartDate = dates.firstday;
    $scope.WeekEndDate = dates.lastday;

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        GetAllAgentList();
        $('#tblDueInstallmentList').dataTable().fnDraw();
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

    $scope.SearchDueInstallmentList = function () {
        if ($("#txtStartDateforSearch").val() != "") {
            $scope.WeekStartDate = ddmmyyTommdddyy($("#txtStartDateforSearch").val())
        }
        if ($("#txtEndDateforSearch").val() != "") {
            $scope.WeekEndDate = ddmmyyTommdddyy($("#txtEndDateforSearch").val())
        }

        $('#tblDueInstallmentList').dataTable().fnDraw();
    }

    var TotalDueInst = 0;
    var TotalLatePayment = 0;
    var Amount = 0;
    var TotalAmount = 0;

    GetAllDueInstallmentList()
    function GetAllDueInstallmentList() {

        $('#tblDueInstallmentList').dataTable({
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
            "sAjaxSource": urlpath + "/Report/GetAllDueInstallmentList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "AgentName", "value": $("#txtAccountlistsearch").val() });
                aoData.push({ "name": "sSearch", "value": $("#txtCustomerNameSearch").val() });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "fromDate", "value": $scope.WeekStartDate });
                aoData.push({ "name": "toDate", "value": $scope.WeekEndDate });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        TotalDueInst = json.TotalDueInst;
                        TotalLatePayment = json.TotalLatePayment;
                        Amount = json.Amount;
                        TotalAmount = json.TotalAmount;
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
                    "mDataProp": "ClienId",
                },
                {
                    "mDataProp": "AccountNumber",
                },
                {
                    "mDataProp": "CustomerName",
                },
                {
                    "mDataProp": "MobileNo",
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('date')(full.InstallmentDate, 'dd/MM/yyyy');
                    },
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(full.PendingInstallment, '', 2);
                    },
                    "sClass": "text-right"
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(full.LatePayment, '', 2);
                    },
                    "sClass": "text-right"
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(full.Amount, '', 2);
                    },
                    "sClass": "text-right"
                },
                {
                    "mRender": function (data, type, full) {
                        return data = $filter('currency')(full.PendingInstallment + full.LatePayment + full.Amount, '', 2);
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
                    $("#lblTotalDueInstallmentAmount").show();
                    $("#lblTotalLatePaymentAmount").show();
                    $("#lblAmount").show();

                    $("#lblTotalDueInstallmentAmount").html($filter('currency')(TotalDueInst, '₹', 2))
                    $("#lblTotalLatePaymentAmount").html($filter('currency')(TotalLatePayment, '₹', 2))
                    $("#lblAmount").html($filter('currency')(Amount, '₹', 2))
                    $("#lblTotalAmount").html($filter('currency')(TotalAmount, '₹', 2))
                }
                else {
                    $("#lblTotalAmount").hide();
                    $("#lblTotalDueInstallmentAmount").hide();
                    $("#lblTotalLatePaymentAmount").hide();
                    $("#lblAmount").hide();
                    $("#labelTotal").hide();

                }
            }
        });
    }

    $scope.SearchClearData = function () {
        $("#txtAccountlistsearch").val("");
        $("#txtStartDateforSearch").val("");
        $("#txtEndDateforSearch").val("");
        $("#txtCustomerNameSearch").val("");
        var dates = ThisWeekStart_Enddate();
        $scope.WeekStartDate = dates.firstday;
        $scope.WeekEndDate = dates.lastday;
        $('#tblDueInstallmentList').dataTable().fnDraw();
    }
});