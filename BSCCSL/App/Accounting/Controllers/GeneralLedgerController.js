angular.module("BSCCL").controller('GeneralLedgerController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $location, $q) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')


    $("#txtFromDate").datetimepicker({
        format: 'DD/MM/YYYY',
        defaultDate: new Date()
    });
    $("#txtToDate").datetimepicker({
        format: 'DD/MM/YYYY',
        defaultDate: new Date()
    });

    $scope.GoBack = function () {
        $state.go('App.Accounts');
    }


    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');
    $scope.Role = getUserdata.Role;

    $scope.GeneralLedgerList = {};
    $scope.TotalCreditAmount = 0;
    $scope.TotalDebitAmount = 0;
    $scope.Openingbalance = 0;
    $scope.ClosingCreditAmount = 0;
    $scope.ClosingDebitAmount = 0;
    GetGeneralLedgerList();

    function GetGeneralLedgerList() {
        let startDate = $("#txtFromDate").data("DateTimePicker").date();
        if (startDate !== null) {
            startDate = $filter("date")(new Date(startDate), "yyyy-MM-dd");
        }
        let endDate = $("#txtToDate").data("DateTimePicker").date();
        if (endDate !== null) {
            endDate = $filter("date")(new Date(endDate), "yyyy-MM-dd");
        }
        var getdata = AppService.GetDataByQrystring("GeneralLedger", "GetGeneralLedgerList", startDate, endDate, $scope.UserBranch.BranchId);

        getdata.then(function (p1) {
            if (p1.data != null) {
                $scope.GeneralLedgerList = p1.data.aaData;
                $scope.TotalCreditAmount = p1.data.TotalCreditAmount;
                $scope.TotalDebitAmount = p1.data.TotalDebitAmount;
                $scope.Openingbalance = p1.data.Openingbalance;
                $scope.ClosingCreditAmount = p1.data.ClosingCreditAmount;
                $scope.ClosingDebitAmount = p1.data.ClosingDebitAmount;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    //function GeneralLedgerList() {

    //    $('#tblGeneralLedger').dataTable({
    //        "bFilter": false,
    //        "processing": false,
    //        "bInfo": true,
    //        "bServerSide": true,
    //        pageLength: 25,
    //        //"bLengthChange": true,
    //        "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
    //        "bSort": false,
    //        "bDestroy": true,
    //        searching: false,
    //        dom: 'l<"floatRight"B>frtip',
    //        buttons: [
    //            {
    //                extend: 'pdf',
    //            },
    //            {
    //                extend: 'print',
    //            }
    //        ],
    //        "sAjaxSource": urlpath + "GeneralLedger/GetGeneralLedgerList",
    //        "fnServerData": function (sSource, aoData, fnCallback) {
    //            aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
    //            $.ajax({
    //                "dataType": 'json',
    //                "type": "POST",
    //                headers: { 'Authorization': 'Bearer ' + $cookies.getObject('User').access_token },
    //                "url": sSource,
    //                "data": aoData,
    //                "success": function (json) {
    //                    fnCallback(json);
    //                }
    //            });
    //        },
    //        "aoColumns": [{
    //            "mDataProp": "BranchName",
    //        },
    //        {
    //            "mDataProp": "EntryTypeName",
    //        },
    //        {
    //            "mDataProp": "PostingDate",
    //        },
    //        {
    //            "mDataProp": "VoucherNumber",
    //        },
    //        {
    //            "mDataProp": "Credit",
    //        },
    //        {
    //            "mDataProp": "Debit",
    //        },

    //        ]

    //    });
    //}



    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        GetGeneralLedgerList();
    };


    $scope.SearchData = function () {
        GetGeneralLedgerList();
    }

    $scope.SearchClearData = function () {
        $('#txtFromDate').data("DateTimePicker").date(new Date());
        $('#txtToDate').data("DateTimePicker").date(new Date());
        GetGeneralLedgerList();
    }

})