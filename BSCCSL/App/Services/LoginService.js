angular.module("BSCCL").service('LoginService', function ($http) {
    this.Login = function (username, password) {

        var obj = new Object();
        obj.grant_type = 'password';
        obj.username = username,
        obj.password = password

        var request = $http({
            
            url: "/Token",
            //url: "http://localhost:50032/api/user/Token",
            method: "POST",
            //method: "GET",
            data: $.param({ grant_type: 'password', username: username, password: password }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            //url: urlpath + "user/Login?username=" + username + "&password=" + password
            //url: "http://localhost:50032/api/user/logintest"
        });

        return request;
    };
});


