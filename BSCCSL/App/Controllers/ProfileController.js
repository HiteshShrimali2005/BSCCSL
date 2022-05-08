angular.module("BSCCL").controller('ProfileController', function ($scope, $state, $cookies, $location, AppService) {
   
    $scope.User = new Object();
    $scope.User.UserId = $cookies.getObject('User').UserId;
    $scope.ChangePasswsord = function () {
     
        var flag = true;
        flag = ValidateForm();
        if (flag) {
            $scope.User.OldPassword = $scope.OldPassword;
            $scope.User.NewPassword = $scope.NewPassword;
            $scope.ConfirmPassword;
            var promise = AppService.SaveData("User", "UserChangePassword", $scope.User)

            promise.then(function (p1) {
                if (p1.data != null) {
                    showToastMsg(1, 'Your Password has been changed successfully')
                    $("#txtConfirmPassword").val() = '';
                    $("#txtNewPassword").val() = '';
                    $("#txtOldPassword").val() = '';
                }
                else {
                    showToastMsg(3, 'Enter currect Old Password')
                }
            }, function (err) {
                showToastMsg(3, 'Error in saving data')
            });
        }
    }
    function ValidateForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if ($scope.User.UserId != "00000000-0000-0000-0000-000000000000" && $scope.User.UserId != undefined) {
            if (!ValidateRequiredField($("#txtOldPassword"), 'Enter Old Password ', 'after')) {
                flag = false;
            }

            if (!ValidateRequiredField($("#txtNewPassword"), 'Enter New Password ', 'after')) {
                flag = false;
            }
            //else {
            //    if (!CheckPassword($("#txtNewPassword"), 'after')) {
            //        flag = false;
            //    }
            //}

            if (!ValidateRequiredField($("#txtConfirmPassword"), 'Re-Enter your Password', 'after')) {
                flag = false;
            }

            if ($("#txtConfirmPassword").val() != '' && $("#txtConfirmPassword").val() != undefined && $("#txtConfirmPassword").val() != null) {
                if ($("#txtNewPassword").val() != $("#txtConfirmPassword").val()) {
                    flag = false;
                    ClearMessage($("#txtConfirmPassword"));
                    $("#txtConfirmPassword").closest('.form-group').addClass('has-error');
                    PrintMessage("#txtConfirmPassword", "Password does not match.", 'after');
                }
                else {
                    ClearMessage($("#txtConfirmPassword"));
                }
            }
            return flag;
        }
    }
});