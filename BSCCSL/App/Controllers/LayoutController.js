angular.module("BSCCL").controller('LayoutController', function ($scope, AppService, $state, $cookies, $location, $rootScope, BranchList, Idle, AuthService, USER_ROLES, AUTH_EVENTS, $interval) {

    
    $scope.userRoles = USER_ROLES;
    $scope.isAuthorized = AuthService.isAuthorized;

    $interval(AuthService.refreshToken, 1500000);

    //$scope.refreshToken = function()
    //{
    //    AuthService.refreshToken();
    //}
    $scope.CurrentDate = new Date();

    $scope.events = [];
    $scope.idle = 1200;
    $scope.timeout = 60;

    $scope.$on('IdleStart', function () {
        addEvent({ event: 'IdleStart', date: new Date() });
    });

    $scope.$on('IdleEnd', function () {
        addEvent({ event: 'IdleEnd', date: new Date() });
    });

    $scope.$on('IdleWarn', function (e, countdown) {
        addEvent({ event: 'IdleWarn', date: new Date(), countdown: countdown });
    });

    $scope.$on('IdleTimeout', function () {

        //var User = $cookies.getObject('User');

        addEvent({ event: 'IdleTimeout', date: new Date() });

        $cookies.remove("User", { path: '/' }, { domain: window.location.protocol + '//' + window.location.host });
        $cookies.remove("Branch", { path: '/' }, { domain: window.location.protocol + '//' + window.location.host });
        $rootScope.$broadcast(AUTH_EVENTS.sessionTimeout);
        $state.go('Login')

        //if (User != undefined) {
        //    User.access_token = ''
        //    $cookies.remove("User", { path: '/' }, { domain: window.location.protocol + '//' + window.location.host });
        //    $cookies.putObject("User", User)
        //    $rootScope.$broadcast('AUTH_EVENTS', AUTH_EVENTS.sessionTimeout);
        //    $state.go('LockScreen')
        //}
        //else {
        //    $cookies.remove("User", { path: '/' }, { domain: window.location.protocol + '//' + window.location.host });
        //    $cookies.remove("Branch", { path: '/' }, { domain: window.location.protocol + '//' + window.location.host });
        //    $rootScope.$broadcast('AUTH_EVENTS', AUTH_EVENTS.sessionTimeout);
        //    $state.go('Login')
        //}

    });

    $scope.$on('Keepalive', function () {
        addEvent({ event: 'Keepalive', date: new Date() });
    });

    function addEvent(evt) {
        $scope.$evalAsync(function () {
            $scope.events.push(evt);
        })
    }

    $scope.reset = function () {
        Idle.watch();
    }

    $scope.$watch('idle', function (value) {
        if (value !== null) Idle.setIdle(value);
    });

    $scope.$watch('timeout', function (value) {
        if (value !== null) Idle.setTimeout(value);
    });



    $scope.UserBranch = {};

    $scope.UserBranch = { Enabled: true, ShowBranch: true };
    $scope.Settings = {};
    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');
    $scope.UserName = getUserdata.FirstName;

    $scope.BranchList = BranchList;

    if ($scope.Role == 1) {
        $scope.UserBranch.BranchId = $scope.BranchList[0].BranchId;
    }
    else {
        $scope.UserBranch.BranchId = getUserdata.BranchId;
        $scope.ShowCollege = false;
    }

    $cookies.put('Branch', $scope.UserBranch.BranchId)

    $scope.Role = getUserdata.Role;

    $rootScope.LoginBranchId = getUserdata.BranchId;

    $scope.Logout = function () {
        $rootScope.$broadcast(AUTH_EVENTS.logoutSuccess);
        $cookies.remove("User", { path: '/' }, { domain: window.location.protocol + '//' + window.location.host });
        $cookies.remove("Branch", { path: '/' }, { domain: window.location.protocol + '//' + window.location.host });
        $state.go('Login')

    }


    function GetBranchCode() {
        var getbranchcode = AppService.GetDetailsById("Customer", "GetBranchCode", $scope.BranchId);
        getbranchcode.then(function (p1) {
            if (p1.data != null) {
                $rootScope.BranchCode = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }
});