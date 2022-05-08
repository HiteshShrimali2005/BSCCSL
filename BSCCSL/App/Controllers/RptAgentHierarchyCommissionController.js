angular.module("BSCCL").controller('RptAgentHierarchyCommissionController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    var AgentCommissionReport;

    var currentTime = new Date();
    var Eventmindata = parseInt(currentTime.getFullYear()) - 20;
    var Eventmaxdata = parseInt(currentTime.getFullYear());
    $scope.min = new Date(Eventmindata + '-01-01');
    $scope.max = new Date(Eventmaxdata + '-01-01');
    $scope.SearchYear = '';

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        AgentCommissionReport.draw();
    }

    $scope.SearchAgentCommissionList = function () {
        AgentCommissionReport.draw();
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

    GetAgentCommissionList()
    function GetAgentCommissionList() {
        AgentCommissionReport = $('#tblAgentCommission').DataTable({
            "searching": false,
            //"processing": false,
            //"bInfo": true,
            "serverSide": true,
            "bLengthChange": true,
            "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
            "ordering": false,
            //"destroy": true,
            //dom: 'l<"floatRight"B>frtip',
            "dom": '<"pull-right"B><"clear">ltip',
            "buttons": [
                {
                    "extend": 'excelHtml5',
                    "title": 'Agent Commission',
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    "extend": 'pdfHtml5',
                    "title": 'Agent Commission',
                    exportOptions: {
                        columns: ':visible'
                    }
                }
            ],

            "ajax": function (data, callback, settings) {
                data.AgentName = $("#txtAccountlistsearch").val();
                data.BranchId = $scope.UserBranch.BranchId;
                data.Month = $("#ddlMonthSearch").val();
                data.Year = $("#ddlSearchYear").val().replace("number:", "");

                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": urlpath + "/Report/RptAgentHierarchyCommission",
                    "data": data,
                    "success": function (json) {
                        callback(json);
                    }
                });
            },
            "columnDefs": [
                {
                    "targets": [0],
                    "visible": false
                }
            ],

            "columns": [
                {
                    "data": "UserId"
                },
                {
                    "className": 'details-control',
                    "data": null,
                    "defaultContent": '',
                    "sWidth": "1%"
                },
                {
                    "title": "Agent Code",
                    "data": "UserCode",
                    "sClass": "text-center"
                },
                {
                    "title": "Agent Name",
                    "data": "AgentName",
                },
                {
                    "title": "Mobile No",
                    "data": "PhoneNumber",
                },
                {
                    "title": "Month",
                    "render": function (data, type, full) {
                        var M = full.Month - 1;
                        return moment().month(M).format("MMMM") + " " + full.Year;
                    },
                },
                {
                    "title": "Commission",
                    "render": function (data, type, full) {
                        return $filter('currency')(full.Commission, '₹ ', 2);
                    },
                    "sClass": "text-right"
                },
                //{
                //    "title": "Action",
                //    "render": function (data, type, full) {
                //        return '<a class="btn btn-xs btn-primary btnPayment" Id="' + full.UserId + '"><i class="fa fa-money"></i>&nbsp;Payment</a>'
                //    },
                //    "sClass": "text-center"
                //},
            ]
        });
    }

    $('#tblAgentCommission tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = AgentCommissionReport.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            // Open this row
            var rowData = row.data();

            var NewObj = new Object();
            NewObj.Month = rowData.Month;
            NewObj.Year = rowData.Year;
            NewObj.UserId = rowData.UserId;
            NewObj.Branchid = $scope.UserBranch.BranchId;

            AppService.SaveData("Report", "RptAgentHierarchyCommissionByMonth", NewObj).then(function (p1) {
                if (p1.data) {
                    var ComData = p1.data;
                    var detail = '';
                    if (ComData.length > 0)
                        detail += '<table class="table m-l-40">' +
                            '<tr>' +
                            '<th>#</th>' +
                            '<th>Account Number</th>' +
                            '<th>Customer Name</th>' +
                            '<th>MobileNo</th>' +
                            '<th>Product Name</th>' +
                            '<th>Date</th>' +
                            '<th class="text-right">Amount</th>' +
                            '<th class="text-center">Commission(%)</th>' +
                            '<th class="text-right">Commission Amount</th>' +
                            '<th class="text-right">Rank</th>' +
                            '</tr>';

                    for (var i = 0; i < ComData.length; i++) {
                        detail += '<tr>' +
                            '<td>' + (i + 1) + '</td>' +
                            '<td>' + ComData[i].AccountNumber + '</td>' +
                            '<td>' + (ComData[i].CustomerName === null ? '' : ComData[i].CustomerName) + '</td>' +
                            '<td>' + (ComData[i].MobileNo === null ? '' : ComData[i].MobileNo) + '</td>' +
                            '<td>' + (ComData[i].ProductName === null ? '' : ComData[i].ProductName) + '</td>' +
                            '<td>' + (ComData[i].Date === null ? '' : $filter('date')(ComData[i].Date, "dd/MM/yyyy")) + '</td>' +
                            '<td class="text-right">' + ($filter('currency')(ComData[i].Amount, '₹ ', 2)) + '</td>' +
                            '<td class="text-center">' + ((ComData[i].agentCommission * 100) / ComData[i].Amount).toFixed(2) + '</td>' +
                            '<td class="text-right">' + ($filter('currency')(ComData[i].agentCommission, '₹ ', 2)) + '</td>' +
                            '<td class="text-right">' + ComData[i].Rank + '</td>' +
                            '</tr>';
                    }

                    detail += '</table>';
                    row.child(detail).show();
                    tr.addClass('shown');
                }
            })
        }
    });


    $scope.SearchClearData = function () {
        $("#txtAccountlistsearch").val("");
        $("#ddlMonthSearch").val("");
        $("#ddlSearchYear").val("");
        $scope.SearchYear = '';
        // $("#txtAccountlistsearch").val('');
        AgentCommissionReport.draw();
    }

});