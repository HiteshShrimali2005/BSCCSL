angular.module("BSCCL").controller('IncomeController', function ($scope, AppService, $state, $cookies, $filter, $rootScope) {
    $scope.UserBranch.ShowBranch = true;
    $scope.UserBranch.Enabled = true;

    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');


    $(".datepicker").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });


    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblIncome').dataTable().fnDraw();
    }


    GetIncomeList();
    GetSubHead()
    $scope.AddIncome = function () {
        $("#txtIncomeDate").data("DateTimePicker").date($filter('date')(new Date(), 'dd/MM/yyyy'));
        $scope.AccountsHead = {};
        $scope.Income = {};
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $('#Income').modal({
        backdrop: 'static',
        show: false,
        keyboard: false  // to prevent closing with Esc button (if you want this too)
    });

    $rootScope.CountValue = 0;

    function GetSubHead() {
        var getheaddata = AppService.GetDetailsById("AccountsHead", "GetSubHead", 1);

        getheaddata.then(function (p1) {
            if (p1.data != null) {
                $scope.AccountsHeadList = p1.data
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function ValidateIncomeForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#txtIncomeName"), 'Enter Income Name', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#dpdAccountHead"), 'Select Accounts Head', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtAmount"), 'Enter Amount', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtIncomeDate"), 'Select Date', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#TransactionMode"), 'Select Transaction Mode', 'after')) {
            flag = false;
        }

        return flag;
    }


    $scope.SaveIncome = function () {
        var flag = true;
        flag = ValidateIncomeForm();
        if (flag) {

            $scope.Income.BranchId = $scope.UserBranch.BranchId;
            $scope.Income.IncomeDate = moment(new Date($("#txtIncomeDate").data("DateTimePicker").date())).format('YYYY-MM-DD');

            var savehead = AppService.SaveData("Income", "SaveIncome", $scope.Income)
            savehead.then(function (p1) {
                if (p1.data != false) {
                    $('#Income').modal('hide');
                    showToastMsg(1, "Income Saved Successfully")
                    GetIncomeList()
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in saving data')
            });
        }
    }


    $scope.ClearForm = function () {
        $scope.Income = {};
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    function GetIncomeList() {

        $('#tblIncome').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            'bAutoWidth': false,
            //"scrollX": true,
            "sAjaxSource": urlpath + "/Income/GetIncomeList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "fromDate", "value": ddmmyyTommdddyy($("#txtsearchStartDate").val()) });
                aoData.push({ "name": "toDate", "value": ddmmyyTommdddyy($("#txtsearchEndDate").val()) });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                        IntialPageControlIncome();
                    }
                });
            },
            "aoColumns": [{
                "sTitle": "Income Name",
                "mDataProp": "IncomeName",
            },
            {
                "sTitle": "Amount",
                "mDataProp": "Amount",
            },
            {
                "sTitle": "Head Name",
                "mDataProp": "HeadName",
            },
            {
                "sTitle": "Income Date",
                "mDataProp": "IncomeDate",
                "mRender": function (data, type, full) {
                    return $filter('date')(data, 'dd/MM/yyyy');
                },
                "sClass": "text-center"
            },
            {
                "sTitle": "Requested By",
                "mDataProp": "RequestedBy",
            },
            {
                "sTitle": "Action",
                "mDataProp": "IncomeId",
                "mRender": function (data, type, full) {

                    var str = ''
                    str += '<span style="cursor:pointer" class="btnEdit" href="#" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span></span> <span style="cursor:pointer" class="btnDelete" Id="' + data + '" href="#" title="Delete"><span class="glyphicon glyphicon-remove"></span></span>'
                    return str;
                },
                "sClass": "text-center",
                "sWidth": "150px"
            }]

        });
    }
    $scope.SearchData = function () {
        $('#tblIncome').dataTable().fnDraw();
    }
    $scope.SearchClearData = function () {
        $('#txtSearch').val('');
        $('#txtsearchStartDate').val('');
        $('#txtsearchEndDate').val('');
        $('#tblIncome').dataTable().fnDraw();
    }

    function IntialPageControlIncome() {
        $(".btnEdit").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $("#Income").modal('show');
            GetIncomeDataById(ID)
        });

        $(".btnDelete").click(function () {
            var ID = $(this).attr("Id");
            bootbox.hideAll();

            bootbox.dialog({
                message: "Are you sure want to delete?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            var promiseDelete = AppService.DeleteData("Income", "DeleteIncomeById", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {
                                    toastr.remove();
                                    showToastMsg(1, "Income Deleted Successfully");
                                    $("#tblIncome").dataTable().fnDraw();
                                }
                                else {
                                    showToastMsg(3, "Error Occured While Deleting");
                                }
                            }, function (err) {
                                showToastMsg(3, "Error Occured");
                            });
                        }
                    },
                    danger: {
                        label: "No",
                        className: "btn-danger btn-flat"
                    }
                }
            });

        });


    }

    function GetIncomeDataById(Id) {
        var getincome = AppService.GetDetailsById("Income", "GetIncomeById", Id);

        getincome.then(function (p1) {
            if (p1.data != null) {
                $scope.Income = p1.data
                $scope.Income.AccountsHeadId = $scope.Income.AccountsHeadId + ""
                $("#txtIncomeDate").data("DateTimePicker").date($filter('date')($scope.Income.IncomeDate, 'dd/MM/yyyy'));
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

})