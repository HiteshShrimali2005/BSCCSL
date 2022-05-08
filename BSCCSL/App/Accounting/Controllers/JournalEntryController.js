angular.module("BSCCL").controller('JournalEntryController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $location, $q) {
    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')


    $("#txtPostingDate").datetimepicker({
        format: 'DD/MM/YYYY',
    });
    $("#txtReferenceDate").datetimepicker({
        format: 'DD/MM/YYYY',
    });
    $("#txtFromPostingDate").datetimepicker({
        format: 'DD/MM/YYYY',
    });
    $("#txtToPostingDate").datetimepicker({
        format: 'DD/MM/YYYY',
    });


    GetAccountList();
    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');
    $scope.Role = getUserdata.Role;

    $scope.JournalEntry = {};
    $scope.JournalEntry.EntryList = [];
    $scope.AccountList = [];

    var TotalCreditAmount = 0;
    var TotalDebitAmount = 0;
    var TotalDifferenceAmount = 0;
    function GetAccountList() {
        var getdata = AppService.GetDetailsWithoutId("JournalEntry", "GetAccountList");

        getdata.then(function (p1) {
            if (p1.data != null) {
                $scope.AccountList = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        GetJournalEntryList();
    };
    $("#btnAddNew").click(function () {
        $("#btnSaveJournal").show();
        $("#divAddEdit").show();
        $("#divListing").hide();
        $("#divSearching").hide();

        $(this).hide();
        $("#btnBack").show();
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        $scope.ISNew = true;


        $scope.JournalEntry = {};
        $scope.JournalEntry.EntryList = [];
        $scope.JournalEntry.EntryList.push({ 'Name': '', 'AccountId': '00000000-0000-0000-0000-000000000000', 'Party': '', 'Credit': 0, 'Debit': 0 });
        $scope.JournalEntry.EntryList.push({ 'Name': '', 'AccountId': '00000000-0000-0000-0000-000000000000', 'Party': '', 'Credit': 0, 'Debit': 0 });
        $("#txtAccount_" + 1 + "_value").val('');
        $("#txtAccount_" + 2 + "_value").val('');

        //GetAccountList();


    })

    $("#btnBack").click(function () {
        $(this).hide();
        $("#btnAddNew").show();

        $("#divListing").show();
        $("#divAddEdit").hide();
        $("#divSearching").show();
    })

    $scope.AddRow = function (index) {
        TotalDebitAmount = 0;
        TotalCreditAmount = 0;
        $scope.JournalEntry.EntryList.push({ 'AccountId': '00000000-0000-0000-0000-000000000000', 'Party': '', 'Credit': 0, 'Debit': 0 });
    }

    $scope.RemoveEntry = function (index) {
        TotalDebitAmount = 0;
        TotalCreditAmount = 0;

        if (index > 2) {
            $scope.JournalEntry.EntryList.pop();
        }
        else {
            showToastMsg(3, "You can not delete this Entry.");
        }
    }

    GetJournalEntryList();

    function GetJournalEntryList() {
        var fromDate = null;
        var toDate = null;
        if ($("#txtFromPostingDate").val() != "") {
            fromDate = moment(new Date($("#txtFromPostingDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
        }
        if ($("#txtToPostingDate").val() != "") {
            toDate = moment(new Date($("#txtToPostingDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
        }


        $('#tblJournalEntry').dataTable({
            "bFilter": false,
            "processing": false,
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
                },
                {
                    extend: 'print',
                }
            ],
            "sAjaxSource": urlpath + "JournalEntry/GetJournalEntryList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "EntryType", "value": $("#ddlSearchEntryType").val() });
                aoData.push({ "name": "FromPostingDate", "value": fromDate });
                aoData.push({ "name": "ToPostingDate", "value": toDate });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    headers: { 'Authorization': 'Bearer ' + $cookies.getObject('User').access_token },
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                        IntialPageControlUser();
                    }
                });
            },
            "aoColumns": [{
                "mDataProp": "VoucherNoString",
            },
            {
                "mDataProp": "EntryTypeName",
            },
            {
                "mDataProp": "PostingDate",
                "mRender": function (data, type, full) {
                    return $filter('date')(data, 'dd/MM/yyyy');
                },

            },
            {
                "mDataProp": "TotalAmount",
                "mRender": function (data, type, full) {
                    return $filter('currency')(data, '₹ ', 2);
                },
                "sClass": "right"
            },


            {
                "mDataProp": "Id",
                "mRender": function (data, type, full) {

                    return '<button class="btn btn-success btn-xs btn-flat btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>  <button class="btn btn-danger btn-xs btn-flat btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> ';
                },
                "sClass": "text-center"

            }]

        });
    }

    $scope.SearchData = function () {
        GetJournalEntryList();
    }

    $scope.SearchClearData = function () {
        $("#ddlSearchEntryType").val('');
        $("#txtFromPostingDate").val('');
        $("#txtToPostingDate").val('');
        $('#txtFromPostingDate').data("DateTimePicker").clear();
        $('#txtToPostingDate').data("DateTimePicker").clear();

        GetJournalEntryList();

    }

    $scope.ClearForm = function () {
        $scope.Accounts = null;
        $scope.SubAccountList = null;
        $("#txtSearch").val("");
        $("#ddlParentAccount").val("");
        $("#ddlRootAccount").val("");
        $("#ddlAccountType").val("");
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }


    function IntialPageControlUser() {

        $(".btnEdit").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");

            $("#divAddEdit").show();
            $("#divListing").hide();
            $("#btnAddNew").hide();
            $("#btnBack").show();
            $("#divSearching").hide();
            $scope.ISNew = false;
            GetJournalEntryDetailsById(ID)
        });

        $(".btnDelete").click(function () {
            var ID = $(this).attr("Id");
            bootbox.dialog({
                message: "Are you sure want to delete?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            var promiseDelete = AppService.DeleteData("JournalEntry", "DeleteJournalEntry", ID);
                            promiseDelete.then(function (p1) {
                                if (p1.data != null) {
                                    showToastMsg(1, "Entry Deleted Successfully");
                                    $("#txtFromPostingDate").val('');
                                    $("#txtToPostingDate").val('');
                                    GetJournalEntryList();
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


    function GetJournalEntryDetailsById(Id) {
        var getuserdata = AppService.GetDetailsById("JournalEntry", "GetJournalEntryDetailsById", Id);
        getuserdata.then(function (p1) {
            if (p1.data != null) {
                $scope.JournalEntry = p1.data;
                $("#btnSaveJournal").hide();

                if (p1.data.PostingDate != null) {
                    $scope.JournalEntry.PostingDate = p1.data.PostingDate;
                    $scope.JournalEntry.PostingDate = $filter("date")(new Date(p1.data.PostingDate), "dd/MM/yyyy");
                    $("#txtPostingDate").data("DateTimePicker").date($filter('date')($scope.JournalEntry.PostingDate, 'dd/MM/yyyy'))
                }
                if (p1.data.ReferenceDate != null) {
                    $scope.JournalEntry.ReferenceDate = p1.data.ReferenceDate;
                    $scope.JournalEntry.ReferenceDate = $filter("date")(new Date(p1.data.ReferenceDate), "dd/MM/yyyy");
                    $("#txtReferenceDate").data("DateTimePicker").date($filter('date')($scope.JournalEntry.ReferenceDate, 'dd/MM/yyyy'))
                }
                if (p1.data.EntryType != null) {
                    $scope.JournalEntry.EntryType = p1.data.EntryType + "";
                }
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    $scope.SaveJournalEntry = function () {
        TotalCreditAmount = 0;
        TotalDebitAmount = 0;
        var flag = true;
        flag = ValidateAccountForm();
        if (flag) {
            $(':focus').blur();
            $scope.JournalEntry.PostingDate = moment(new Date($("#txtPostingDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            if ($("#txtReferenceDate").val() != "") {
                $scope.JournalEntry.ReferenceDate = moment(new Date($("#txtReferenceDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            TotalDifferenceAmount = parseFloat(parseFloat(TotalCreditAmount) - parseFloat(TotalDebitAmount));
            $scope.JournalEntry.BranchId = $scope.UserBranch.BranchId;

            if (TotalDifferenceAmount == 0) {
                var savedata = AppService.SaveData("JournalEntry", "SaveJournalEntry", $scope.JournalEntry)

                savedata.then(function (p1) {
                    if (p1.data != null) {
                        $("#divListing").show();
                        $("#divAddEdit").hide();
                        $("#btnBack").hide();
                        $("#btnAddNew").show();
                        $("#divSearching").show();
                        $("#txtFromPostingDate").val('');
                        $("#txtToPostingDate").val('');
                        GetJournalEntryList();
                        showToastMsg(1, "Data Saved Successfully");

                    }
                    else {
                        showToastMsg(3, 'Error in saving data')
                    }
                }, function (err) {
                    showToastMsg(3, 'Error in saving data')
                });

            }
            else {
                showToastMsg(3, 'Credit and Debit Amount Should Match.')
            }

        }
    }


    $scope.SelectedAccount = function (selected) {
        if (selected) {
            var index = this.id.substring(11);
            $scope.JournalEntry.EntryList[index - 1].AccountId = selected.originalObject.AccountId;
        }
    }



    function ValidateAccountForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtPostingDate"), 'Date is required', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#ddlEntryType"), 'Entry Type is required', 'after')) {
            flag = false;
        }



        for (var i = 1; i <= $scope.JournalEntry.EntryList.length; i++) {
            TotalDebitAmount = parseFloat(parseFloat(TotalDebitAmount) + parseFloat($("#txtDebitAmount_" + i).val()));
            TotalCreditAmount = parseFloat(parseFloat(TotalCreditAmount) + parseFloat($("#txtCreditAmount_" + i).val()));

            if ($scope.JournalEntry.EntryList[1 - 1].AccountId == "00000000-0000-0000-0000-000000000000") {
                $("#txtAccount_" + i + "_value").val('');
                if (!ValidateRequiredField($("#txtAccount_" + i + "_value"), 'Account is required', 'after')) {
                    flag = false;
                }

            }

            if ($("#txtDebitAmount_" + i).val() == "0" && $("#txtCreditAmount_" + i).val() == "0") {
                if (!ValidateRequiredField($("#txtDebitAmount_" + i), 'Amount is required', 'after', 0)) {
                    flag = false;
                }
                if (!ValidateRequiredField($("#txtCreditAmount_" + i), 'Amount is required', 'after', 0)) {
                    flag = false;
                }
            }
        }

        return flag;
    }

    $scope.checkCreditDebitAmount = function (index) {
        var DebitValue = $("#txtDebitAmount_" + index).val();
        var CreditValue = $("#txtCreditAmount_" + index).val();

        if (DebitValue != 0) {
            $("#txtCreditAmount_" + index).val(0);
            $scope.JournalEntry.EntryList[index - 1].Credit = 0;
        }

        if (CreditValue != 0) {
            $("#txtDebitAmount_" + index).val(0);
            $scope.JournalEntry.EntryList[index - 1].Debit = 0;
        }

    }
})