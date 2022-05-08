angular.module("BSCCL").controller('AgentHierarchyCommissionPaymentController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    var AgentCommissionReport;
    $scope.CommissionData = [];
    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        AgentCommissionReport.draw();
    }

    var currentTime = new Date();
    var Eventmindata = parseInt(currentTime.getFullYear()) - 20;
    var Eventmaxdata = parseInt(currentTime.getFullYear());
    $scope.min = new Date(Eventmindata + '-01-01');
    $scope.max = new Date(Eventmaxdata + '-01-01');
    $scope.SearchYear = '';

    $scope.RecurringDeposit = false;
    $scope.FixedDeposit = false;

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
                data.UserStatus = $("#ddlUserStatus").val();

                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": urlpath + "Report/RptAgentHierarchyCommission",
                    "data": data,
                    "success": function (json) {
                        $scope.CommissionData = [];
                        $scope.CommissionData = json.data;
                        callback(json);
                        InitCustomerCommissionModal();
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
                    "title": '<span style="margin-left:21px;"><input type="checkbox" class="selectall" style="height:18px;width:18px;"></span>',
                    "render": function (data, type, full) {
                        return '<input type="checkbox" id="' + full.UserId + '_' + full.Month + '_' + full.Year + '" style="height:18px;width:18px;">';
                    },
                    "sClass": "text-center"
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
                    "title": "Agent Status",
                    "data": "UserStatus",
                    "render": function (data, type, full) {
                        if (full.UserStatus) {
                            return '<span class="label label-success">Active</span>'
                        } else
                            return '<span class="label label-danger">Deactive</span>'
                    }
                },

                {
                    "title": "Mobile No",
                    "data": "PhoneNumber",
                },
                {
                    "title": "AccountNo",
                    "data": "AccountNumber",
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
                {
                    "title": "Paid Commission",
                    "render": function (data, type, full) {
                        return $filter('currency')(full.PaidCommission, '₹ ', 2);
                    },
                    "sClass": "text-right"
                },
                {
                    "title": "Pending Commission",
                    "render": function (data, type, full) {
                        return $filter('currency')(full.Commission - full.PaidCommission, '₹ ', 2);
                    },
                    "sClass": "text-right"
                },
                {
                    "title": "Status",
                    "render": function (data, type, full) {
                        if (full.Commission == full.PaidCommission) {
                            return '<span class="label label-success">Paid</span>'
                        } else if (full.PaidCommission == 0) {
                            return '<span class="label label-danger">Unpaid</span>'
                        } else if (full.PaidCommission != 0 && full.Commission != full.PaidCommission) {
                            return '<span class="label label-warning">Partially Paid</span>'
                        }
                    },
                    "sClass": "text-center"
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

    $scope.CommissionSubData = [];
    function InitCustomerCommissionModal() {
        $('.selectall').click(function () {
            $(this.form.elements).filter(':checkbox').prop('checked', this.checked);
        });
    }

    $scope.ViewCommissionDetails = function () {
        var UserIDs = [];
        angular.forEach($scope.CommissionData, function (data) {
            if ($('#' + data.UserId + '_' + data.Month + '_' + data.Year).prop('checked') == true) {
                UserIDs.push(data.UserId + '_' + data.Month + '_' + data.Year);
            }
        })

        var ProductType = []

        if ($scope.RecurringDeposit == true) {
            ProductType.push(4)
        }

        if ($scope.FixedDeposit == true) {
            ProductType.push(3)
        }

        if ($scope.RegularIncomePlanner == true) {
            ProductType.push(6)
        }
        if ($scope.MonthlyIncomeScheme == true) {
            ProductType.push(7)
        }


        var NewObj = new Object();
        NewObj.BranchId = $scope.UserBranch.BranchId;
        NewObj.UserIDs = UserIDs;
        NewObj.ProductTypes = ProductType;

        AppService.SaveData("AgentCommission", "AgentHierarchyCommissionByMonthAndYear", NewObj).then(function (p1) {
            if (p1.data) {
                $scope.CommissionSubData = p1.data;
                $("#AgentCommissionAmount").modal("show");
            }
        })
    }

    $scope.TotalPayableCommission = 0;
    $scope.SelectCommissionData = function (data) {
        if (data.Check == true) {
            $scope.TotalPayableCommission += data.agentCommission;
        } else {
            $scope.TotalPayableCommission -= data.agentCommission;
        }

        if ($scope.CheckAll == true) {
            var Flag = false;
            angular.forEach($scope.CommissionSubData, function (data) {
                if (data.Check == true) {
                    Flag = true;
                }
            })
            if (Flag == false) {
                $scope.CheckAll = false;
            }
        }
    }

    $scope.CheckDeposite = function () {

        //if ($scope.RecurringDeposit == true && $scope.FixedDeposit == true && $scope.RegularIncomePlanner == false) {
        //    $scope.CommissionSubData = $filter('filter')($scope.CommissionSubData, { ProductType: 4, ProductType: 3 }, true);
        //}
        //else if ($scope.RecurringDeposit == true && $scope.FixedDeposit == false && $scope.RegularIncomePlanner == false) {
        //    $scope.CommissionSubData = $filter('filter')($scope.CommissionSubData, { ProductType: 4 }, true);
        //}
        //else if ($scope.RecurringDeposit == true && $scope.FixedDeposit == false && $scope.RegularIncomePlanner == true) {
        //    $scope.CommissionSubData = $filter('filter')($scope.CommissionSubData, { ProductType: 4, ProductType: 6 }, true);
        //}
        //else if ($scope.RecurringDeposit == false && $scope.FixedDeposit == true && $scope.RegularIncomePlanner == true) {
        //    $scope.CommissionSubData = $filter('filter')($scope.CommissionSubData, { ProductType: 3, ProductType: 6 }, true);
        //}
        //else if ($scope.RecurringDeposit == false && $scope.FixedDeposit == false && $scope.RegularIncomePlanner == true) {
        //    $scope.CommissionSubData = $filter('filter')($scope.CommissionSubData, { ProductType: 6 }, true);
        //}
        //else if ($scope.RecurringDeposit == false && $scope.FixedDeposit == true && $scope.RegularIncomePlanner == false) {
        //    $scope.CommissionSubData = $filter('filter')($scope.CommissionSubData, { ProductType: 3 }, true);
        //}


        $scope.CheckAll = false;
        $scope.TotalPayableCommission = 0;
    }

    $scope.SaveAgentCommissionPayment = function () {
        var checkdata = $filter('filter')($scope.CommissionSubData, { Check: true });
        if (checkdata.length > 0) {
            bootbox.dialog({
                message: "Are you sure want to pay " + $filter('currency')($scope.TotalPayableCommission, '₹ ', 2) + " to selected agents?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            var NewObj = new Object();
                            NewObj.Commission = checkdata;
                            NewObj.PaidBy = $cookies.getObject('User').UserId;
                            NewObj.BranchId = $scope.UserBranch.BranchId;

                            AppService.SaveData("AgentCommission", "SaveAgentCommissionPayment", NewObj).then(function (result) {
                                if (result.data) {
                                    $scope.CloseCommissionModal();
                                    $("#AgentCommissionAmount").modal("hide");
                                    AgentCommissionReport.draw();
                                }
                            })
                        }
                    },
                    danger: {
                        label: "No",
                        className: "btn-danger btn-flat"
                    }
                }
            });

        } else {
            showToastMsg(3, 'Please select commission amount to pay.')
        }
    }

    $scope.CloseCommissionModal = function () {
        $scope.CommissionSubData = [];
        $scope.RecurringDeposit = false;
        $scope.FixedDeposit = false;
        $scope.CheckAll = false;
        $scope.TotalPayableCommission = 0;

        if (!$scope.$$phase) {
            $scope.$apply();
        }
    }

    $scope.SearchClearData = function () {
        $("#txtAccountlistsearch").val("");
        $("#ddlMonthSearch").val("");
        $("#ddlSearchYear").val("");
        $("#ddlUserStatus").val("");
        $scope.SearchYear = '';
        $(".selectall").attr("Checked", false);
        // $("#txtAccountlistsearch").val('');
        AgentCommissionReport.draw();
    }

    $scope.CheckAllCheckBoxes = function (check) {
        if (check == true) {
            $scope.TotalPayableCommission = 0;
            angular.forEach($scope.CommissionSubData, function (data) {
                if (data.CommissionPaid != true) {
                    data.Check = true;
                    $scope.TotalPayableCommission += data.agentCommission;
                }
            })
        } else {
            angular.forEach($scope.CommissionSubData, function (data) {
                data.Check = false;
                $scope.TotalPayableCommission = 0;
            })
        }

    }
});