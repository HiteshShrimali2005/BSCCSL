angular.module("BSCCL").service('UserService', function ($http) {
    this.Login = function (username, password) {
        var request = $http({
            method: "GET",
            url: urlpath + "User/Register"

        });
        return request;
      
       
    };
    this.SaveFormData = function (data) {

        var request = $http({
            url: urlpath + 'User/UserRegister',
            method: 'POST',
            data: data,

        });
        return request;
    };
   
});