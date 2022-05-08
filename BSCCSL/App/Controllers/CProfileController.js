angular.module("BSCCL").controller('CProfileController', function ($scope, $state, $cookies, $location, AppService, $filter) {
    $("#txtDOB").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });
    $scope.User = new Object();
    $scope.User.UserId = $cookies.getObject('User').UserId;
    GetUserDataById();

    function GetUserDataById() {
        var getuserdata = AppService.GetDetailsById("User", "GetUserDataById", $scope.User.UserId);

        getuserdata.then(function (p1) {
            if (p1.data != null) {
                $scope.User = p1.data;
                var getbranchdata = AppService.GetDetailsById("Branch", "GetBranchDataById", $scope.User.BranchId);
                getbranchdata.then(function (p2) {
                    if (p2.data != null) {
                        $scope.BranchName = p2.data.BranchName;
                    }
                });
                $("#txtDOB").data("DateTimePicker").date($filter('date')($scope.User.DateOfBirth, 'dd/MM/yyyy'));
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
            $scope.User.Password = "";
            $scope.User.DateOfBirth = moment(new Date($("#txtDOB").data("DateTimePicker").date())).format('YYYY-MM-DD');

            if ($scope.User.UserId == '0000000-0000-0000-0000-000000000000' && $scope.User.UserId != undefined)
                $scope.User.CreatedBy = $cookies.getObject('User').UserId;
            else
                $scope.User.ModifiedBy = $cookies.getObject('User').UserId;


            SaveUser();
            //var checkmail = AppService.SaveData("User", "GetUsersEmailId", $scope.User);
            //checkmail.then(function (p1) {
            //    if (p1.data == true) {
                    
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
                if (p1.data != false) {
                    showToastMsg(1, "User Saved Successfully");
                    GetUserDataById();
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in saving data')
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
        if (!CheckOnlyText($("#txtFirstName"),'after')) {
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
        if (!CheckMobileNumandphnnum($("#txtPhonenNo"), 'Not a Valid PhoneNumber', 'after')) {
            flag = false;
        }
     

        //if (!ValidateRequiredField($("#txtUserName"), 'Email Required', 'after')) {
        //    flag = false;
        //}

        if (!CheckEmail($("#txtUserName"), 'after')) {
            flag = false;
        }

        return flag;
    }

});