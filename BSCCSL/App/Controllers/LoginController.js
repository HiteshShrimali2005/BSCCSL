//(function () {
//    'use strict';
angular.module("BSCCL").controller('LoginController', function ($scope, LoginService, $state, $cookies, $rootScope, AUTH_EVENTS, AuthService) {
    document.title = 'BSCCSL'
    $scope.forgot = null;
    $scope.OTP = null;
    $scope.reset = null;
    $scope.showforgot = function () {
        if ($scope.forgot == null && $scope.forgot == undefined) {
            $scope.forgot = 1;
        }
        else {
            $scope.forgot = null;
            $scope.reset = null;
            $scope.OTP = null;
        }
    }

    $scope.sendOTP = function () {
        $scope.OTP = 1;
    }
    $scope.verify = function () {
        $scope.reset = 1;
    }

    $scope.Login = function () {
        var flag;
        flag = ValidateForm();
        //console.log('scope is', $scope)
        console.log(flag);

        if (flag) {
           
            var UserLogin = LoginService.Login($scope.username, $scope.password);

            UserLogin.then(function (p1) {
                if (p1.data != null) {
                    $rootScope.$broadcast(AUTH_EVENTS.loginSuccess);
                    if ($cookies.get('User') != undefined) {
                        $cookies.remove("User")
                    }

                    $cookies.putObject("User", p1.data)

                    if (p1.data.Role == 1 || p1.data.Role == 2 || p1.data.Role == 4 || p1.data.Role == 1) {
                        $state.go("App.User");
                    }
                    else if (p1.data.Role == 3) {
                        $state.go("App.Transaction");
                    }
                    else if (p1.data.Role == 5 || p1.data.Role == 6) {
                        $state.go("App.CustomerList");
                    }
                    else if (p1.data.Role == 8 || p1.data.Role == 7) {
                        $state.go("App.Reports");
                    }
                }
                else {
                    $rootScope.$broadcast(AUTH_EVENTS.loginFailed);
                    showToastMsg(3, 'Invalid Username or Password.')
                }
            }, function (err) {
                $rootScope.$broadcast(AUTH_EVENTS.loginFailed);
                showToastMsg(3, 'Invalid Username or Password.')
            });
        }
    }


    function ValidateForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#txtUserName"), 'User Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtPassword"), 'Password  required', 'after')) {
            flag = false;
        }
        return flag;
    }
});

