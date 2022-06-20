angular.module("BSCCL").controller('BranchController', function ($scope, AppService, $state, $cookies, $filter, $rootScope) {


    $scope.UserBranch.ShowBranch = false;
    $scope.UserBranch.Enabled = false;

    GetBranchList();
    $scope.Add = function () {
        $scope.Branch = {};
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $('#Branch').modal({
        backdrop: 'static',
        show: false,
        keyboard: false  // to prevent closing with Esc button (if you want this too)
    });

    $rootScope.CountValue = 0;

    function GetBranchDataById(Id) {
        var getbranchdata = AppService.GetDetailsById("Branch", "GetBranchDataById", Id);

        getbranchdata.then(function (p1) {
            if (p1.data != null) {
                $scope.BranchData = p1.data;
                $scope.Branch = $scope.BranchData;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.SaveBranch = function () {
        var flag = true;
        flag = ValidateUserForm();
        if (flag) {
            if ($scope.Branch.BranchId == '0000000-0000-0000-0000-000000000000' && $scope.Branch.BranchId != undefined) {
                $scope.Branch.CreatedBy = $cookies.getObject('User').UserId;
                $scope.Branch.CreatedDate = Date.now();
            }
            else {
                $scope.Branch.ModifiedBy = $cookies.getObject('User').UserId;
            };
            var checkHO = AppService.SaveData("Branch", "CheckHO", $scope.Branch);
            checkHO.then(function (p1) {
                if (p1.data == true) {

                    var saveuser = AppService.SaveData("Branch", "BranchRegister", $scope.Branch)
                    saveuser.then(function (p1) {
                        if (p1.data != false) {
                            $('#Branch').modal('hide');
                            showToastMsg(1, "Branch Saved Successfully")
                            RefreshDataTablefnDestroy()
                            GetBranchList()
                            GetUserBranchList()



                            $('#tblBranch').dataTable().fnDraw();
                            $window.location.reload();
                            $state.go($state.current, {}, { reload: true });
                        }
                        else {
                            showToastMsg(3, 'BranchCode is already Exist Please Enter Unique BranchCode')
                        }
                    }, function (err) {
                        showToastMsg(3, 'Error in saving data')
                    });
                }
                else if (p1.data == false) {
                    showToastMsg(3, 'HO Already Exist.')
                }
            });
        }
    }


    function GetUserBranchList() {

        var getbranch = AppService.GetDetailsById("Branch", "GetAllBranch", $cookies.getObject('User').UserId);
        getbranch.then(function (p1) {
            if (p1.data != null) {
                $scope.BranchList = p1.data;
                if ($scope.Role == 1) {
                    $scope.UserBranch.BranchId = $cookies.get('Branch')
                    //$scope.UserBranch.BranchId = $scope.BranchList[0].BranchId;
                }
                else {
                    $scope.UserBranch.BranchId = $cookies.getObject('User').BranchId;
                }
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.ClearForm = function () {
        $scope.Branch = {};
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    function GetBranchList() {

        $('#tblBranch').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/Branch/GetAllBranchData",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {

                        fnCallback(json);
                        IntialPageControlUser();
                    }
                });
            },
            "aoColumns": [{
                "mDataProp": "BranchName",
            },
            {
                "mDataProp": "BranchAddress",
            },
            {
                "mDataProp": "BranchCode",
            },
            {
                "mDataProp": "RegistrationNo",
            },
            {
                "mDataProp": "BranchPhone",
            },
            {
                "mDataProp": "IsHO",
                "mRender": function (data, type, full) {
                    if (full.IsHO == true) {
                        return '<span class="label" style="background-color:green">Active</span>';
                    }
                    else {
                        return null;
                    }

                }
            },
            {
                "mDataProp": "BranchId",
                "mRender": function (data, type, full) {

                    return '<button class="btn btn-success btn-xs btn-flat btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>  <button class="btn btn-danger btn-xs btn-flat btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> ';
                },
                "sClass": "text-center"

            }]

        });
    }
    $scope.SearchData = function () {
        $('#tblBranch').dataTable().fnDraw();
    }
    $scope.SearchClearData = function () {
        $('#txtSearch').val('');
        $('#tblBranch').dataTable().fnDraw();
    }

    function IntialPageControlUser() {

        $(".btnEdit").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $("#Branch").modal('show');
            GetBranchDataById(ID)
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
                            var promiseDelete = AppService.DeleteData("Branch", "DeleteBranch", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {

                                    RefreshDataTablefnDestroy();
                                    toastr.remove();
                                    showToastMsg(1, "Branch Deleted Successfully");
                                    RefreshDataTablefnDestroy();
                                    GetBranchList();
                                    GetUserBranchList();
                                    //GetUserBranchList()
                                   // $('#tblBranch').dataTable().fnDraw();
                                    $('#tblBranch').dataTable().fnDraw();
                                    $window.location.reload();
                                    $state.go($state.current, {}, { reload: true });
                                    
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
    function RefreshDataTablefnDestroy() {
        $("#tblBranch").dataTable().fnDestroy();
    }
    function ValidateUserForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#txtBranchName"), 'Branch Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtBranchPhone"), 'Branch Phone Number required', 'after')) {
            flag = false;
        }
        if (!CheckMobileNumandphnnum($("#txtBranchPhone"), 'Not a Valid Phone Number', 'after')) {
            flag = false;
        }

        //if (!ValidateRequiredField($("#txtBranchCode"), 'Branch Code required', 'after')) {
        //    flag = false;
        //}
        //if (!ValidateRequiredField($("#txtRegistrationNo"), 'Registration No required', 'after')) {
        //    flag = false;
        //}
        if (!ValidateRequiredField($("#txtBranchAddress"), 'Branch Address Required', 'after')) {
            flag = false;
        }
        return flag;
    }

})