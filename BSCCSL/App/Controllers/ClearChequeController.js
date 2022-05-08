angular.module("BSCCL").controller('ClearChequeController', function ($scope, $state, $cookies, $filter, AppService, $http, $location, $rootScope) {


    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblcheque').dataTable().fnDraw();
    }

    $("#txtChequeCleanceDate").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    var getUserdata = new Object();
    $scope.TransactionData = {};

    getUserdata = $cookies.getObject('User');

    $scope.AccountNo = $("#txtAccNumber").val()
    $scope.SectionHide = true
    $scope.HolderData = {}

    $scope.Acnumber = '';

    function GetHolderDetail() {
        var getholder = AppService.GetDetailsById("Customer", "GetHolderData", $scope.AccountNo);
        getholder.then(function (p1) {
            if (p1.data.AccountDetail != null) {
                $scope.SectionHide = false
                $scope.HolderData = p1.data.AccountDetail;
                $scope.HolderData.details = p1.data.details
                if ($scope.HolderData.details.length > 0) {
                    $scope.show = 1;
                    GetUnclearCheckList();
                }
            }
            else {
                $scope.SectionHide = true
                showToastMsg(3, "No Account found. Please enter correct Account No for transaction details")
                GetUnclearCheckList();
            }
        });
    }

    GetClearCheckList();

    function GetClearCheckList() {

        $('#tblcheque').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/Transaction/GetClearCheckList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });
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
                    "mDataProp": "AccountNo",
                },
                {
                    "mDataProp": "Name"
                },
                {
                    "mDataProp": "ProductTypeName",

                },
                {
                    "mDataProp": "Balance",
                    "mRender": function (data, type, full) {
                        return $filter('currency')(full.Balance, '', 2);
                    },
                },
                {
                    "mDataProp": "TransactionDate",
                    "mRender": function (data, type, full) {
                        return $filter('date')(data, 'dd/MM/yyyy');
                    },
                },
                {
                    "mDataProp": "ChequeAmount",
                    "mRender": function (data, type, full) {
                        return $filter('currency')(full.ChequeAmount, '', 2);
                    },
                },
                {
                    "mDataProp": "ChequeNo",
                },
                {
                    "mDataProp": "ChequeDate",
                    "mRender": function (data, type, full) {
                        return $filter('date')(data, 'dd/MM/yyyy');
                    },
                },
                 {
                     "mDataProp": "BankName",
                 },
                   {
                       "mDataProp": "ChequeClearDate",
                       "mRender": function (data, type, full) {
                           return $filter('date')(data, 'dd/MM/yyyy');
                       },
                   },
                ]
        });
    }

    function gettransactionDetails(ID) {

        var Promis = AppService.GetDetailsById("Customer", "gettransactionDetails", ID);
        Promis.then(function (p1) {
            if (p1.data != null) {
                $scope.TransactionData = p1.data.transactionData;
                $scope.TransactionId = p1.data.TransactionId;
                $scope.TransactionType = p1.data.TransactionType;
            }
            else {
                showToastMsg(3, 'Error in Getting Data');
            }
        });
    }

    $scope.SearchAccountNumber = function () {

        var flag = true;
        if ($scope.IsSameBranch == true) {
            if (!ValidateRequiredField($("#txtAccNumber"), 'Please enter Account number of holder of account', 'after')) {
                flag = false;
            }
        }

        if (flag) {
            GetCustomerDataByProductId();
        }

    }

    $scope.stateChanged = function () {
        if ($scope.IsSameBranch != true) { //If it is not checked
            $scope.CustomerData = "";
            $scope.AccountNo = "";
            $scope.AccBalance = "";
            if (!$('#chksamebank').attr('checked')) {
                $('#txtAccNumber').closest('.form-group').removeClass('has-error');
                $('#txtAccNumber').next('.help-block').remove();
                $('#txtAccNumber').prev('.help-block').remove();
            }

        }
    }

    $scope.SearchCheque = function () {

        $('#tblcheque').dataTable().fnDraw();
    }

    $scope.ClearChequeData = function () {
        $('#txtSearch').val('');
        $('#tblcheque').dataTable().fnDraw();
    }

    function RefreshDataTablefnDestroy() {
        $('#tblcheque').dataTable().fnDraw();
    }
});