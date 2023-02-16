angular.module("BSCCL").controller('UserController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $location, $q) {

    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')

    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');
    $scope.Role = getUserdata.Role;

    //var path = "/dist/img/fa-user.png";
    //$('#uploadedimage').attr("src", path);
    //var imageData;
    //function readImage(input) {
    //    if (input.files && input.files[0]) {
    //        var FR = new FileReader();
    //        FR.onload = function (e) {
    //            $('#uploadedimage').attr("src", e.target.result);
    //            imageData = e.target.result;
    //            $scope.ImageBlob = imageData;
    //        };
    //        FR.readAsDataURL(input.files[0]);
    //    }
    //}
    //$('input[type=file]').change(function () {
    //    readImage(this);
    //});

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblUser').dataTable().fnDraw();
    };

    //$rootScope.CountValue = 1;

    //$scope.$watch(
    //function () {
    //    return $scope.BranchId;
    //},
    //function () {
    //    console.log($scope.BranchId);
    //    console.log("changed");
    //}, true);


    $("#txtDOB").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $('#User').modal({
        backdrop: 'static',
        show: false,
        keyboard: false  // to prevent closing with Esc button (if you want this too)
    });

    $scope.Add = function () {
        $scope.User = {};
        $scope.AgentRankData = [];
        $("#btntabuser").click();
        $("#role").val("");
        $("#txtDOB").val("");
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $('#User').on('shown.bs.modal', function () {
        if ($location.search().CustomerId != undefined) {
            $scope.CustomerId = $location.search().CustomerId;
            AppService.GetDetailsById("Customer", "GetCustomerDetailsForAgentCreationbyId", $scope.CustomerId).then(function (p1) {
                if (p1.data) {
                    var NewObj = new Object();
                    NewObj.FirstName = p1.data.Personal.FirstName;
                    NewObj.LastName = p1.data.Personal.LastName;
                    NewObj.PhoneNumber = p1.data.Address.MobileNo;
                    NewObj.Gender = p1.data.Personal.Sex.toLowerCase();
                    NewObj.Address = (p1.data.Address.DoorNo == "" ? "" : p1.data.Address.DoorNo + ", ") + (p1.data.Address.BuildingName == "" ? "" : p1.data.Address.BuildingName + ", ") +
                        (p1.data.Address.PlotNo_Street == "" ? "" : p1.data.Address.PlotNo_Street + ", ") + (p1.data.Address.Landmark == "" ? "" : p1.data.Address.Landmark + ", ") +
                        (p1.data.Address.Area == "" ? "" : p1.data.Address.Area + ", ") + (p1.data.Address.District == "" ? "" : p1.data.Address.District + ", ") +
                        (p1.data.Address.Place == "" ? "" : p1.data.Address.Place + ", ") + (p1.data.Address.City == "" ? "" : p1.data.Address.City + ", ") +
                        (p1.data.Address.Pincode == "" ? "" : p1.data.Address.Pincode + ", ") + (p1.data.Address.State == "" ? "" : p1.data.Address.State + ".");
                    NewObj.UserName = p1.data.Address.Email;
                    NewObj.BranchId = $scope.UserBranch.BranchId;
                    NewObj.Role = "4";
                    $scope.User = NewObj;
                    $("#txtDOB").data("DateTimePicker").date($filter('date')(p1.data.Personal.DOB, 'dd/MM/yyyy'));

                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                }
            })
        }
    })

    // GetAllBranch();
    GetUserList();
    GetAllEmployee();


    function GetAllCustomer() {

        AppService.GetDetailsById("User", "GetAllCustomerByBranhId",$scope.UserBranch.BranchId).then(function (p1) {
            if (p1.data != null) {
                $scope.CustomerList = [];
                $scope.CustomerList = p1.data;
            }
        });
    }

    function GetAllEmployee() {

        var getemployeedata = AppService.GetDetailsWithoutId("User", "GetAllEmployee");
        getemployeedata.then(function (p1) {
            if (p1.data != null) {
                $scope.EmployeeData = [];
                $scope.EmployeeData = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    function GetAllAgentList() {
        AppService.GetDetailsById("User", "GetAgentListByBranchId", null).then(function (p1) {
            if (p1.data != null) {
                $scope.AgentList = p1.data;
            }
        })
    }


    function GetAgentRankList() {
        var Id = null;
        if ($scope.User != undefined) {
            Id = $scope.User.UserId
        }

        var getAgentRank = AppService.GetDetailsById("User", "GetAgentRankList", Id);
        getAgentRank.then(function (p1) {
            if (p1.data != null) {
                $scope.AgentRankData = [];
                $scope.AgentRankData = p1.data;
                //if ($scope.$$phase) {
                //    $scope.$apply();
                //}
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        });
    }

    //get use data by id
    function GetUserDataById(Id) {
        var getuserdata = AppService.GetDetailsById("User", "GetUserDataById", Id);

        getuserdata.then(function (p1) {
            if (p1.data != null) {

                $scope.User = p1.data;
                if ($scope.User.Role == 4) {
                    GetAllAgentList()
                    GetAgentRankList()
                    GetAllCustomer()
                }
                else {
                    $scope.AgentRankData = [];
                }
                $scope.User.Password = "";
                $("#txtDOB").data("DateTimePicker").date($filter('date')($scope.User.DateOfBirth, 'dd/MM/yyyy'));
                $scope.User.Role = p1.data.Role + "";



                // $("#role").val($scope.User.Role)
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.SaveUser = function () {

        var flag = true;
        flag = ValidateUserForm();
        if (flag) {
            $(':focus').blur();
            if ($("#txtDOB").val() != "" && $("#txtDOB").val() != null && $("#txtDOB").val() != undefined) {
                $scope.User.DateOfBirth = moment(new Date($("#txtDOB").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            else {
                $scope.User.DateOfBirth = null;
            }
            //$scope.User.DateOfBirth = moment(new Date($("#txtDOB").data("DateTimePicker").date())).format('YYYY-MM-DD');          

            if ($scope.User.UserId == '0000000-0000-0000-0000-000000000000' || $scope.User.UserId == undefined)
                $scope.User.CreatedBy = $cookies.getObject('User').UserId;
            else
                $scope.User.ModifiedBy = $cookies.getObject('User').UserId;

            if ($scope.CustomerId != undefined) {
                $scope.User.CustomerId = $scope.CustomerId;
            }

            SaveUser();

            //var checkmail = AppService.SaveData("User", "GetUsersEmailId", $scope.User);
            //checkmail.then(function (p1) {
            //    if (p1.data == true) {
            //        SaveUser();
            //    }
            //    else if (p1.data == false) {
            //        showToastMsg(3, 'Username Already Exist.')
            //    }
            //});
        }
    }


    function SaveUser() {
        var saveuser = AppService.SaveData("User", "UserRegister", $scope.User)

        saveuser.then(function (p1) {
            if (p1.data) {
                var UserId = p1.data;
                $scope.User.UserId = p1.data;
                if ($scope.User.Role != 4) {
                    $('#User').modal('hide');
                    showToastMsg(1, "User Saved Successfully");
                    $scope.User = {};
                    $("#role").val("");
                    $("#txtDOB").val("");
                    GetUserList();
                    GetAllEmployee();
                    if ($scope.CustomerId != undefined) {
                        $scope.GetCustomerAgentId(UserId);
                    }
                }
                else {
                    GetAllAgentList()
                    GetAgentRankList()
                    $("#btnagentcmsn").click();
                }
            }
            else {
                showToastMsg(3, 'Error in saving data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in saving data')
        });
    }


    $scope.SaveAgentHierarchy = function () {

        var flag = true;
        //  flag = ValidateAgentHierarchyForm();
        if (flag) {
            $(':focus').blur();
            angular.forEach($scope.AgentRankData, function (data, index) {
                data.UserId = $scope.User.UserId;
            })

            var saveagenthierarchy = AppService.SaveData("User", "SaveAgentHierarchy", $scope.AgentRankData)

            saveagenthierarchy.then(function (p1) {
                if (p1.data) {

                    $('#User').modal('hide');
                    showToastMsg(1, "User Saved Successfully");
                    $scope.User = {};
                    $("#role").val("");
                    $("#txtDOB").val("");
                    GetUserList();
                    GetAllAgentList();
                    GetAgentRankList()
                    if ($scope.CustomerId != undefined) {
                        $scope.GetCustomerAgentId(UserId);
                    }
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in saving data')
            });
        }
    }

    //function ValidateAgentHierarchyForm() {
    //    var flag = true;
    //    angular.forEach($scope.AgentRankData, function (data, index) {
    //        if (!ValidateRequiredField($("#ddlAgentRank" + index), data.Rank + ' required', 'after')) {
    //            flag = false;
    //        }
    //    })
    //    return flag;
    //}

    function GetAllBranch() {

        var getbranch = AppService.GetDetailsById("Branch", "GetAllBranch", getUserdata.UserId);

        getbranch.then(function (p1) {
            if (p1.data != null) {
                $scope.Branch = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function ValidateUserForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtFirstName"), 'First Name required', 'after')) {
            flag = false;
        }
        if (!CheckOnlyText($("#txtFirstName"), 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtLastName"), 'Last Name required', 'after')) {
            flag = false;
        }
        if (!CheckOnlyText($("#txtLastName"), 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtPhonenNo"), 'PhoneNumber required', 'after')) {
            flag = false;
        }
        if (!CheckMobileNumandphnnum($("#txtPhonenNo"), 'Not a Valid Phone Number', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#role"), 'Role required', 'after')) {
            flag = false;
        }
        //if (!ValidateRequiredField($("#txtUserCode"), 'UserCode required', 'after')) {
        //    flag = false;
        //}

        if (!ValidateRequiredField($("#ddlBranchId"), 'Branch required', 'after')) {
            flag = false;
        }

        //if (!ValidateRequiredField($("#txtUserName"), 'Email Required', 'after')) {
        //    flag = false;
        //}

        if (!CheckEmail($("#txtUserName"), 'after')) {
            flag = false;
        }

        if (!$scope.User || !$scope.User.UserId || $scope.User.UserId == "00000000-0000-0000-0000-000000000000") {

            if (!ValidateRequiredField($("#txtPassword"), 'Password Required', 'after')) {
                flag = false;
            }
            //else {
            //    if (!CheckPassword($("#txtPassword"), 'after')) {
            //        flag = false;
            //    }
            //}

            if (!ValidateRequiredField($("#txtCnfPassword"), 'Password Required', 'after')) {
                flag = false;
            }
            if ($("#txtCnfPassword").val() != '' && $("#txtCnfPassword").val() != undefined && $("#txtCnfPassword").val() != null) {
                if ($("#txtPassword").val() != $("#txtCnfPassword").val()) {
                    flag = false;
                    ClearMessage($("#txtCnfPassword"));
                    $("#txtCnfPassword").closest('.form-group').addClass('has-error');
                    PrintMessage("#txtCnfPassword", "Password does not match.", 'after');
                }
                else {
                    ClearMessage($("#txtCnfPassword"));
                }
            }
        }

        return flag;
    }

    $scope.ClearForm = function () {
        $scope.User = {};
        $("#role").val("");
        $("#txtDOB").val("");
        $("#btntabuser").click();
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    function GetUserList() {

        $('#tblUser').dataTable({
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
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6]
                    }
                },
                {
                    extend: 'print',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6]
                    }
                }
            ],
            "sAjaxSource": urlpath + "User/GetUserList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });
                aoData.push({ "name": "role", "value": $("#rolesearch").val() });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
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
                "mDataProp": "Name",
                "mRender": function (data, type, full) {
                    return full.FirstName + ' ' + full.LastName
                },
            },
            {
                "mDataProp": "Gender",
            },
            {
                "mDataProp": "PhoneNumber",
            },
            {
                "mDataProp": "DateOfBirth",
                "mRender": function (data, type, full) {
                    return $filter('date')(data, 'dd/MM/yyyy');
                },
            },
            {
                "mDataProp": "Branch.BranchName",
            },
            {
                "mDataProp": "UserCode",
            },
            {
                "mDataProp": "RoleName",
                "mRender": function (data, type, full) {
                    if (data == "CashierPlusClerk") {
                        return "Cashier + Clerk"
                    }
                    else {
                        return data;
                    }
                },
            },
            {
                "mDataProp": "UserId",
                "mRender": function (data, type, full) {

                    return '<button class="btn btn-success btn-xs btn-flat btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>  <button class="btn btn-danger btn-xs btn-flat btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> ';
                },
                "sClass": "text-center"

            }]

        });
    }

    $scope.SearchData = function () {
        $('#tblUser').dataTable().fnDraw();
    }

    $scope.SearchClearData = function () {
        $('#txtSearch').val('');
        $('#rolesearch').val('');
        $('#tblUser').dataTable().fnDraw();
    }

    function IntialPageControlUser() {

        $(".btnEdit").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $("#btntabuser").click();
            $("#User").modal('show');
            GetUserDataById(ID)
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
                            var promiseDelete = AppService.DeleteData("User", "DeleteUser", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {

                                    RefreshDataTablefnDestroy();
                                    toastr.remove();
                                    showToastMsg(1, "User Deleted Successfully");
                                    GetUserList();
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
        $("#tblUser").dataTable().fnDestroy();
    }

})