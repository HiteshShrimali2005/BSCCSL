angular.module("BSCCL").controller('LockScreenController', function ($scope, LoginService, $state, $cookies, $location, $rootScope, AuthService, AUTH_EVENTS) {


    $rootScope.$broadcast(AUTH_EVENTS.sessionTimeout);


    $scope.Login = function () {

        if ($cookies.get('User') != undefined) {

            var flag;
            flag = ValidateForm();

            if (flag) {

                var UserLogin = LoginService.Login($cookies.getObject('User').UserCode, encodeURIComponent($scope.password));

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
        else {
            $state.go("Login");

        }
    };
    function ValidateForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        ;
        var flag = true;
        //if (!ValidateRequiredField($("#txtUserName"), 'User Name required', 'after')) {
        //    flag = false;
        //}
        if (!ValidateRequiredField($("#txtPassword"), 'Password  required', 'after')) {
            flag = false;
        }
        return flag;
    }

});