angular.module("BSCCL").controller('RptDayScrollController', function ($scope, $state, $cookies, $location, AppService, $filter, $rootScope) {

    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblTransactionforall').dataTable().fnDraw();
    }

    $scope.SearchTransactionListAll = function () {
        $('#tblTransactionforall').dataTable().fnDraw();
    }

    $scope.ClearAll = function () {
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $('#tblTransactionforall').dataTable().fnDraw();
    }
    var TotalDeposite = 0;
    var TotalWithDraw = 0;

    GetTransactionListForAllCustomer()
    function GetTransactionListForAllCustomer() {

        $('#tblTransactionforall').dataTable({
            "bFilter": false,
            "processing": true,
            "bInfo": true,
            "bServerSide": true,
            pageLength: 25,
            //"bLengthChange": true,
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
            "sAjaxSource": urlpath + "/Report/RptDayScrollList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "fromDate", "value": $("#txtStartDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtStartDateforSearch").val()) : new Date().toDateString() });
                aoData.push({ "name": "toDate", "value": $("#txtEndDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtEndDateforSearch").val()) : new Date().toDateString() });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        // DepositSum = json.DepositSum
                        //WithdrawSum = json.WithdrawSum
                        TotalWithDraw = $filter('currency')(json.TotalWithdrawal, '₹', 2);
                        TotalDeposite = $filter('currency')(json.TotalDeposite, '₹', 2);;
                        fnCallback(json);
                    }
                });
            },
            //"footerCallback": function (row, data, start, end, display) {
            //    var api = this.api(), data;
            //    $(api.column(4).footer()).html(
            //    $filter('currency')(DepositSum, '', 2));
            //    $(api.column(5).footer()).html(
            //         $filter('currency')(WithdrawSum, '', 2));
            //},

            "aoColumns": [
                {
                    "mDataProp": "TransactionTime",
                    "mRender": function (data) {
                        return $filter('date')(data, 'dd/MM/yyyy');
                    },
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
                    "mDataProp": "Description",
                },
                {
                    "mDataProp": "Amount",
                    "sClass": "text-right",
                    "mRender": function (data, type, full) {
                        if (full.Type == 1) {
                            return data = $filter('currency')(data, '', 2);
                        }
                        else {
                            return ''
                        }
                    },
                },
                {
                    "mDataProp": "Amount",
                    "sClass": "text-right",
                    "mRender": function (data, type, full) {
                        if (full.Type == 2) {
                            return data = $filter('currency')(data, '', 2);
                        }
                        else {
                            return ''
                        }
                    },
                },
                //{
                //    "mDataProp": "Balance",
                //    "mRender": function (data, type, full) {
                //        return $filter('currency')(data, '', 2);
                //    },
                //    "sType": "numeric-comma",
                //    "sClass": "right"
                //}
            ],
            "footerCallback": function (row, aoData, start, end, display) {
                if (aoData.length > 0) {
                    //// Total over this page

                    //// Update footer



                    $("#lblTotalDepositeAmount").html(TotalDeposite);
                    $("#lblTotalWithDrawAmount").html(TotalWithDraw);

                    $("#lblTotalDepositeAmount").show();
                    $("#lblTotalWithDrawAmount").show();
                    $("#labelTotal").show();


                }
                else {
                    $("#lblTotalDepositeAmount").hide();
                    $("#lblTotalWithDrawAmount").hide();
                    $("#labelTotal").hide();
                }

            },

        });
    }


});