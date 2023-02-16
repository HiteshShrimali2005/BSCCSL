var app = angular.module("BSCCL", ["oc.lazyLoad", "ui.router", "ngCookies", "ngSanitize", 'ngResource', 'ngComboDatePicker', 'ngIdle', 'angular.filter'])

app.constant('USER_ROLES', {
    all: '*',
    admin: 'Admin',
    manager: 'Manager',
    cashier: 'Cashier',
    agent: 'Agent',
    clerk: 'Clerk',
    cashierplusclerk: 'CashierPlusClerk',
    scree_sales: 'Scree-Sales',
    sales_manager: 'Sales-Manager',
})
app.constant('AUTH_EVENTS', {
    loginSuccess: 'auth-login-success',
    loginFailed: 'auth-login-failed',
    logoutSuccess: 'auth-logout-success',
    sessionTimeout: 'auth-session-timeout',
    notAuthenticated: 'auth-not-authenticated',
    notAuthorized: 'auth-not-authorized'
})

var urlpath = '/api/'

app.factory('httpInterceptor', function ($q, $rootScope, $log, $cookies, $injector) {
    //Loader config
    var numLoadings = 0;

    return {
        request: function (config) {
            if ($cookies.get('User') != undefined) {
                config.headers['Authorization'] = 'Bearer ' + $cookies.getObject('User').access_token
            }

            numLoadings++;

            // Show loader
            $("#overlay").show();
            $rootScope.$broadcast("loader_show");
            return config || $q.when(config)

        },
        response: function (response) {

            if ((--numLoadings) === 0) {
                // Hide loader
                $rootScope.$broadcast("loader_hide");
                $("#overlay").hide();
            }

            return response || $q.when(response);

        },
        responseError: function (response) {
            if (response.status == -1 || response.status == 401) {
                $injector.get('$state').transitionTo('Login');
            }

            if (!(--numLoadings)) {
                // Hide loader
                $rootScope.$broadcast("loader_hide");
                $("#overlay").hide();
            }

            return $q.reject(response);
        }
    }
});


app.factory('AuthService', function ($resource, $http, $rootScope, $window, AUTH_EVENTS, $cookies, $q, $state) {
    var authService = {};

    authService.refreshToken = function () {
        var deferred = $q.defer();

        if ($cookies.getObject('User') != undefined) {
            var data = $.param({ grant_type: 'refresh_token', refresh_token: $cookies.getObject('User').refresh_token, client_id: null })


            $http.post("/token", data, {
                headers:
                    { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).then(function (response) {
                if (response.data.access_token != undefined && response.data.access_token != "" && response.data.access_token != null) {
                    var o = response;
                    var minutes = 30;
                    var now = new Date();
                    var thirtyminutes = new Date(now.getTime() + (minutes * 60 * 1000));
                    $cookies.remove("User", { path: '/' }, { domain: window.location.protocol + '//' + window.location.host });
                    $cookies.putObject('User', response.data, { 'expires': thirtyminutes }, { 'path': '/' });
                    deferred.resolve(null);
                }
                else {
                    $state.go('Login')
                }


            });
            return deferred.promise;
        }
        else {
            $state.go('Login')
        }
    }

    //check if the user is authenticated
    authService.isAuthenticated = function () {
        return !!$cookies.getObject('User');
    };

    //check if the user is authorized to access the next route
    //this function can be also used on element level
    //e.g. <p ng-if="isAuthorized(authorizedRoles)">show this only to admins</p>
    authService.isAuthorized = function (authorizedRoles) {

        if (!angular.isArray(authorizedRoles)) {
            authorizedRoles = [authorizedRoles];
        }
        return (authService.isAuthenticated() && authorizedRoles.indexOf($cookies.getObject('User').RoleName) !== -1);
    };

    //log out the user and broadcast the logoutSuccess event
    //authService.logout = function () {
    //    Session.destroy();
    //    $window.sessionStorage.removeItem("userInfo");
    //    $rootScope.$broadcast(AUTH_EVENTS.logoutSuccess);
    //}

    return authService;
});

app.factory('AuthInterceptor', function ($rootScope, $q, AUTH_EVENTS) {
    return {
        responseError: function (response) {
            $rootScope.$broadcast({
                401: AUTH_EVENTS.notAuthenticated,
                403: AUTH_EVENTS.notAuthorized,
                419: AUTH_EVENTS.sessionTimeout,
                440: AUTH_EVENTS.sessionTimeout
            }[response.status], response);
            return $q.reject(response);
        }
    };
});

app.config(function ($stateProvider, $urlRouterProvider, $httpProvider, $ocLazyLoadProvider, $locationProvider, $provide, IdleProvider, KeepaliveProvider, USER_ROLES) {

    $httpProvider.interceptors.push('httpInterceptor');

    $httpProvider.interceptors.push(['$injector', function ($injector) {
        return $injector.get('AuthInterceptor');
    }]);

    var spinnerFunction = function spinnerFunction(data, headersGetter) {
        return data;
    };

    $httpProvider.defaults.transformRequest.push(spinnerFunction);

    KeepaliveProvider.interval(10);
    //IdleProvider.windowInterrupt('focus');

    $ocLazyLoadProvider.config({
        debug: true,
        serie: true,
        cache: true,
        modules: [
            {
                name: 'moment',
                files: [
                    'plugins/moment/moment.min.js'
                ],
                serie: true
            },
            {
                name: 'datetimepicker',
                files: [
                    'plugins/datetimepicker/bootstrap-datetimepicker.css',
                    'plugins/datetimepicker/bootstrap-datetimepicker.min.js'
                ],
                serie: true
            },
            {
                name: 'multiselect',
                files: [
                    'Content/bootstrap-multiselect.css',
                    'Scripts/bootstrap-multiselect.js',
                ],
                serie: true
            },
            {
                name: 'Typeahead',
                files: ['Scripts/bootstrap-typeahead.js',
                ],
                serie: true
            },
            {
                name: 'bootbox',
                files: [
                    'plugins/bootbox/bootbox.min.js'
                ],
                serie: true
            },
            {
                name: 'print',
                files: [
                    'Scripts/jquery.print.js'
                ],
                serie: true
            },
            {
                name: 'iCheck',
                files: [
                    'plugins/iCheck/all.css',
                    'plugins/iCheck/icheck.min.js'
                ],
                serie: true
            },
            {
                name: 'angucomplete',
                files: [
                    'plugins/angucomplete/angucomplete-alt.css',
                    'plugins/angucomplete/angucomplete-alt.js'
                ],
                serie: true
            },

            {
                name: 'bootstrap-select',
                files: [
                    'plugins/bootstrap-select/css/bootstrap-select.min.css',
                    'plugins/bootstrap-select/js/bootstrap-select.min.js'
                ],
                serie: true
            },
            {
                name: 'datepicker',
                files: [
                    'plugins/datepicker/datepicker3.css',
                    'plugins/datepicker/bootstrap-datepicker.js'
                ],
                serie: true
            },
            {
                name: 'datatable',
                files: [
                    'plugins/datatables/dataTables.bootstrap.css',
                    'plugins/datatables/jquery.dataTables.min.js',
                    'plugins/datatables/dataTables.bootstrap.min.js',
                ],
                serie: true
            },
            {
                name: 'datatableexport',
                files: [
                    'plugins/datatables/buttons.dataTables.min.css',
                    'plugins/datatables/dataTables.buttons.min.js',
                    'plugins/datatables/buttons.flash.min.js',
                    'plugins/datatables/jszip.min.js',
                    'plugins/datatables/pdfmake.min.js',
                    'plugins/datatables/vfs_fonts.js',
                    'plugins/datatables/buttons.html5.min.js',
                    'plugins/datatables/buttons.print.min.js',
                ],
                cache: true,
                serie: true
            },
            {
                name: 'lightbox',
                files: [
                    'plugins/lightbox/lightbox.min.css',
                    'plugins/lightbox/lightbox.min.js',
                ],
                serie: true
            },
            {
                name: 'Typeahead',
                files: ['Scripts/bootstrap-typeahead.js',
                ],
                serie: true
            },
        ]
    });

    $stateProvider
        .state('Login', {
            url: '/Login',
            templateUrl: "/Views/Login.html",
            controller: 'LoginController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['App/Controllers/LoginController.js', 'App/Services/LoginService.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: false,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.agent, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.scree_sales, USER_ROLES.sales_manager]
            }
        })
        .state('App', {
            abstract: true,
            url: '/App',
            templateUrl: "/Views/Layout.html",
            controller: 'LayoutController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['App/Controllers/LayoutController.js'], { serie: true }, { cache: false });
                }],
                BranchList: function (AppService, $cookies) {

                    return AppService.GetDetailsById("Branch", "GetAllBranch", $cookies.getObject('User').UserId)
                        .then(function (p1) {
                            return p1.data;
                        }, function () {
                            return [];
                        })
                },
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.agent, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.scree_sales, USER_ROLES.sales_manager]
            }
        })
        .state("App.Dashboard", {
            parent: 'App',
            url: "/Dashboard",
            templateUrl: "/Views/Dashboard.html",
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['https://cdnjs.cloudflare.com/ajax/libs/raphael/2.1.0/raphael-min.js',
                        'plugins/morris/morris.css',
                        'plugins/morris/morris.min.js',
                        'plugins/sparkline/jquery.sparkline.min.js',
                        'plugins/jvectormap/jquery-jvectormap-1.2.2.css',
                        'plugins/jvectormap/jquery-jvectormap-1.2.2.min.js',
                        'plugins/jvectormap/jquery-jvectormap-world-mill-en.js',
                        'plugins/knob/jquery.knob.js',
                        'plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css',
                        'plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js',
                        'dist/js/pages/dashboard.js'], { serie: true }, { cache: false });
                }]
            },
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.agent, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.scree_sales]
            }

        })
        .state("App.User", {
            parent: 'App',
            url: "/User",
            templateUrl: "/Views/User.html",
            controller: 'UserController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'bootbox', 'iCheck', 'App/Controllers/UserController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager]
            }
        })
        .state("App.Customer", {
            parent: 'App',
            url: "/Customer",
            templateUrl: "/Views/Customer.html",
            controller: 'CustomerController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'iCheck', 'App/Controllers/CustomerController.js', 'App/Controllers/AgentListController.js', 'App/Controllers/EmployeeListController.js', 'App/Controllers/UserController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.CustomerList", {
            parent: 'App',
            url: "/CustomerList",
            templateUrl: "/Views/CustomerList.html",
            controller: 'CustomerListController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'App/Controllers/CustomerListController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.CustomerProduct", {
            parent: 'App',
            url: "/CustomerProduct",
            templateUrl: "/Views/CustomerProduct.html",
            controller: 'CustomerProductController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['iCheck', 'bootbox', 'App/Controllers/CustomerProductController.js', 'App/Controllers/AgentListController.js', 'App/Controllers/EmployeeListController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.Branch", {
            parent: 'App',
            url: "/Branch",
            templateUrl: "/Views/Branch.html",
            controller: 'BranchController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'iCheck', 'App/Controllers/BranchController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager]
            }
        })
        .state("App.Product", {
            parent: 'App',
            url: "/Product",
            templateUrl: "/Views/Product.html",
            controller: 'ProductController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'iCheck', 'App/Controllers/ProductController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager]
            }
        })
        .state("App.Transaction", {
            parent: 'App',
            url: "/Transaction",
            templateUrl: "/Views/Transaction.html",
            controller: 'TransactionController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'iCheck', 'bootbox', 'App/Controllers/TransactionController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.UnclearChequeList", {
            parent: 'App',
            url: "/UnclearCheque",
            templateUrl: "/Views/UnclearCheque.html",
            controller: 'UnclearChequeController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'iCheck', 'App/Controllers/UnclearChequeController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.ClearChequeList", {
            parent: 'App',
            url: "/ClearCheque",
            templateUrl: "/Views/ClearCheque.html",
            controller: 'ClearChequeController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'iCheck', 'App/Controllers/ClearChequeController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.Profile", {
            parent: 'App',
            url: "/Profile",
            templateUrl: "/Views/Profile.html",
            controller: 'ProfileController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'App/Controllers/ProfileController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.CProfile", {
            parent: 'App',
            url: "/CProfile",
            templateUrl: "/Views/CProfile.html",
            controller: 'CProfileController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'App/Controllers/CProfileController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.CustomerLoan", {
            parent: 'App',
            url: "/Loan",
            templateUrl: "/Views/LoanApplication.html",
            controller: 'LoanController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'iCheck', 'App/Controllers/LoanController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.GroupLoan", {
            parent: 'App',
            url: "/GroupLoan",
            templateUrl: "/Views/GroupLoan.html",
            controller: 'GroupLoanController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'iCheck', 'App/Controllers/GroupLoanController.js', 'App/Controllers/AgentListController.js', 'App/Controllers/EmployeeListController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.LoanList", {
            parent: 'App',
            url: "/Loanlist",
            templateUrl: "/Views/LoanList.html",
            controller: 'LoanListController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'iCheck', 'App/Controllers/LoanListController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.GroupLoanList", {
            parent: 'App',
            url: "/GroupLoanList",
            templateUrl: "/Views/GroupLoanList.html",
            controller: 'GroupLoanListController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'iCheck', 'App/Controllers/GroupLoanListController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.LoanApproval", {
            parent: 'App',
            url: "/LoanApproval",
            templateUrl: "/Views/LoanApproval.html",
            controller: 'LoanApprovalController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'App/Controllers/LoanApprovalController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager]
            }
        })
        .state("App.BankMaster", {
            parent: 'App',
            url: "/BankMaster",
            templateUrl: "/Views/BankMaster.html",
            controller: 'BankMasterController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'multiselect', 'iCheck', 'bootbox', 'App/Controllers/BankMasterController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager]
            }
        })
        .state("App.CommissionPayment", {
            parent: 'App',
            url: "/CommissionPayment",
            templateUrl: "/Views/AgentCommissionPayment.html",
            controller: 'AgentCommissionPaymentController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'datatableexport', 'iCheck', 'Typeahead', 'App/Controllers/AgentCommissionPaymentController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager]
            }
        })
        .state("App.BankTransaction", {
            parent: 'App',
            url: "/BankTransaction",
            templateUrl: "/Views/BankTransaction.html",
            controller: 'BankTransactionController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'iCheck', 'bootbox', 'App/Controllers/BankTransactionController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager]
            }
        })

        .state("App.Reports", {
            parent: 'App',
            url: "/Reports",
            templateUrl: "/Views/Reports.html",
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            },
            controller: function ($scope) {
                $scope.UserBranch.ShowBranch = false;
                $scope.UserBranch.Enabled = false;
            }

        })
        .state("App.RptTransaction", {
            parent: 'App',
            url: "/RptTransaction",
            templateUrl: "/Views/RptTransaction.html",
            controller: 'RptTransactionController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'bootbox', 'App/Controllers/RptTransactionController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.AccountList", {
            parent: 'App',
            url: "/AccountList",
            templateUrl: "/Views/AccountList.html",
            controller: 'AccountListController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/AccountListController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.RptDayScroll", {
            parent: 'App',
            url: "/RptDayScroll",
            templateUrl: "/Views/RptDayScroll.html",
            controller: 'RptDayScrollController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptDayScrollController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.RptDayScrollStats", {
            parent: 'App',
            url: "/RptDayScrollStats",
            templateUrl: "/Views/RptDayScrollStates.html",
            controller: 'RptDayScrollStatesController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptDayScrollStatesController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.RptDayScrollSummary", {
            parent: 'App',
            url: "/RptDayScrollSummary",
            templateUrl: "/Views/RptDayScrollSummary.html",
            controller: 'RptDayScrollSummaryController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptDayScrollSummaryController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.PassbookPrint", {
            parent: 'App',
            url: "/PassbookPrint",
            templateUrl: "/Views/PassbookPrint.html",
            controller: 'PassbookPrintController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['App/Controllers/PassbookPrintController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk]
            }
        })
        .state("App.RptBalanceSheet", {
            parent: 'App',
            url: "/RptBalanceSheet",
            templateUrl: "/Views/RptBalanceSheet.html",
            controller: 'RptBalanceSheetController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptBalanceSheetController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.RptAgentCustomer", {
            parent: 'App',
            url: "/RptAgentCustomer",
            templateUrl: "/Views/RptAgentCustomer.html",
            controller: 'RptAgentCustomerController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptAgentCustomerController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.RptDueInstallmentList", {
            parent: 'App',
            url: "/RptDueInstallmentList",
            templateUrl: "/Views/RptRDFDDueInstallment.html",
            controller: 'RptRDFDDueInstallmentController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'Typeahead', 'App/Controllers/RptRDFDDueInstallmentController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.RptAgentCommission", {
            parent: 'App',
            url: "/RptAgentCommission",
            templateUrl: "/Views/RptAgentCommission.html",
            controller: 'RptAgentCommissionController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'iCheck', 'Typeahead', 'App/Controllers/RptAgentCommissionController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.RptCommissionPayment", {
            parent: 'App',
            url: "/RptCommissionPayment",
            templateUrl: "/Views/RptCommissionPayment.html",
            controller: 'RptCommissionPaymentController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'iCheck', 'Typeahead', 'App/Controllers/RptCommissionPaymentController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.RptRDFDPendingInstallments", {
            parent: 'App',
            url: "/RptRDFDPendingInstallments",
            templateUrl: "/Views/RptRDFDPendingInstallments.html",
            controller: 'RptRDFDPendingInstallmentsController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'Typeahead', 'App/Controllers/RptRDFDPendingInstallmentsController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.RptProductInstallment", {
            parent: 'App',
            url: "/RptProductInstallment",
            templateUrl: "/Views/RptProductInstallment.html",
            controller: 'RptProductInstallmentController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptProductInstallmentController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })

        .state("App.RptProductList", {
            parent: 'App',
            url: "/RptProductList",
            templateUrl: "/Views/RptProductList.html",
            controller: 'RptProductListController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptProductListController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })


        .state("App.RptMaturity", {
            parent: 'App',
            url: "/RptMaturity",
            templateUrl: "/Views/RptMaturity.html",
            controller: 'RptMaturityController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptMaturityController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })


        .state("App.RptLoanStatement", {
            parent: 'App',
            url: "/RptLoanInstallmentDetails",
            templateUrl: "/Views/RptLoanStatement.html",
            controller: 'RptLoanStatementController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptLoanStatementController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })

        .state("App.RptPrematureProductList", {
            parent: 'App',
            url: "/RptPrematureProductList",
            templateUrl: "/Views/RptPrematureProductList.html",
            controller: 'RptPrematureProductListController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptPrematureProductListController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })


        .state("App.RptInterestDepositList", {
            parent: 'App',
            url: "/RptInterestDepositList",
            templateUrl: "/Views/RptInterestDepositList.html",
            controller: 'RptInterestDepositListController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptInterestDepositListController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })




        .state("App.RptLoanStatementDetails", {
            parent: 'App',
            url: "/RptLoanStatementDetails",
            templateUrl: "/Views/RptLoanStatementDetails.html",
            controller: 'RptLoanStatementDetailsController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'print', 'App/Controllers/RptLoanStatementDetailsController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })



        .state("App.RptEmployeeProduct", {
            parent: 'App',
            url: "/RptEmployeeProduct",
            templateUrl: "/Views/RptEmployeeProduct.html",
            controller: 'RptEmployeeProductController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptEmployeeProductController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })

        .state("App.RptEmployeePerfomance", {
            parent: 'App',
            url: "/RptEmployeePerfomance",
            templateUrl: "/Views/RptEmployeePerfomance.html",
            controller: 'RptEmployeePerfomanceController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'Typeahead', 'App/Controllers/RptEmployeePerfomanceController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })


        .state("App.RptAgentPerfomance", {
            parent: 'App',
            url: "/RptAgentPerfomance",
            templateUrl: "/Views/RptAgentPerfomance.html",
            controller: 'RptAgentPerfomanceController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'Typeahead', 'App/Controllers/RptAgentPerfomanceController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })

        .state("App.RptCustomerShare", {
            parent: 'App',
            url: "/RptCustomerShare",
            templateUrl: "/Views/RptCustomerShares.html",
            controller: 'RptCustomerSharesController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'App/Controllers/RptCustomerSharesController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.RptAccountsCRDR", {
            parent: 'App',
            url: "/RptAccountsCRDR",
            templateUrl: "/Views/RptAccountsCRDR.html",
            controller: 'RptAccountsCRDRController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'App/Controllers/RptAccountsCRDRController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })

        .state("App.RptProfitandLoss", {
            parent: 'App',
            url: "/RptProfitandLoss",
            templateUrl: "/Views/RptProfitandLoss.html",
            controller: 'RptProfitandLossController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'iCheck', 'print', 'App/Controllers/RptProfitandLossController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })


        .state("App.RptCashBook", {
            parent: 'App',
            url: "/RptCashBook",
            templateUrl: "/Views/RptCashBook.html",
            controller: 'RptCashBookController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'print', 'App/Controllers/RptCashBookController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })


        .state("App.RptBankBook", {
            parent: 'App',
            url: "/RptBankBook",
            templateUrl: "/Views/RptBankBook.html",
            controller: 'RptBankBookController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'print', 'App/Controllers/RptBankBookController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })



        .state("App.RptTrailBalance", {
            parent: 'App',
            url: "/RptTrailBalance",
            templateUrl: "/Views/RptTrailBalance.html",
            controller: 'RptTrailBalanceController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'print', 'App/Controllers/RptTrailBalanceController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })

        .state("App.ProductEnquiry", {
            parent: 'App',
            url: "/ProductEnquiry",
            templateUrl: "/Views/ProductEnquiry.html",
            controller: 'ProductEnquiryController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['App/Controllers/ProductEnquiryController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })
        .state("LockScreen", {
            url: "/LockScreen",
            templateUrl: "/Views/LockScreen.html",
            controller: 'LockScreenController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['App/Controllers/LockScreenController.js', 'App/Services/LoginService.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: false,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.agent, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.scree_sales, USER_ROLES.sales_manager]
            }
        })
        .state("App.Calculator", {
            parent: 'App',
            url: "/Calculator",
            templateUrl: "/Views/Calculator.html",
            controller: 'CalculatorController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['App/Controllers/CalculatorController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })
        .state("App.AccountsHead", {
            parent: 'App',
            url: "/AccountsHead",
            templateUrl: "/Views/AccountsHead.html",
            controller: 'AccountsHeadController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'App/Controllers/AccountsHeadController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager]
            }
        })
        .state("App.Expense", {
            parent: 'App',
            url: "/Expense",
            templateUrl: "/Views/Expense.html",
            controller: 'ExpenseController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'App/Controllers/ExpenseController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager]
            }
        })

        .state("App.Income", {
            parent: 'App',
            url: "/Income",
            templateUrl: "/Views/Income.html",
            controller: 'IncomeController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'App/Controllers/IncomeController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager]
            }
        })


        .state("App.RptAgentHierarchy", {
            parent: 'App',
            url: "/RptAgentHierarchy",
            templateUrl: "/Views/RptAgentHierarchy.html",
            controller: 'RptAgentHierarchyController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['bootbox', 'App/Controllers/RptAgentHierarchyController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.RptAgentHierarchyCommission", {
            parent: 'App',
            url: "/RptAgentHierarchyCommission",
            templateUrl: "/Views/RptAgentHierarchyCommission.html",
            controller: 'RptAgentHierarchyCommissionController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'iCheck', 'Typeahead', 'App/Controllers/RptAgentHierarchyCommissionController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.AgentHierarchyCommissionPayment", {
            parent: 'App',
            url: "/RptHierarchyCommissionPayment",
            templateUrl: "/Views/AgentHierarchyCommissionPayment.html",
            controller: 'AgentHierarchyCommissionPaymentController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'iCheck', 'bootbox', 'Typeahead', 'App/Controllers/AgentHierarchyCommissionPaymentController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            }
        })
        .state("App.JournalVoucher", {
            parent: 'App',
            url: "/JournalVoucher",
            templateUrl: "/Views/JournalVoucher.html",
            controller: 'JournalVoucherController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'iCheck', 'bootbox', 'Typeahead', 'App/Controllers/JournalVoucherController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })
        .state("App.DDSPayment", {
            parent: 'App',
            url: "/DDSPayment",
            templateUrl: "/Views/DDSPayment.html",
            controller: 'DDSPaymentController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'iCheck', 'bootbox', 'Typeahead', 'App/Controllers/DDSPaymentController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })
        .state("App.ActivateRD", {
            parent: 'App',
            url: "/ActivateRD",
            templateUrl: "/Views/ActivateRD.html",
            controller: 'ActivateRDController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'bootbox', 'Typeahead', 'App/Controllers/ActivateRDController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })


        .state("App.Accounts", {
            parent: 'App',
            url: "/Accounts",
            templateUrl: "/App/Accounting/Views/Accounting.html",
            authorize: true,
            data: {
                authorizedRoles:
                    [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager, USER_ROLES.scree_sales]
            },
            controller: function ($scope) {
                $scope.UserBranch.ShowBranch = false;
                $scope.UserBranch.Enabled = false;
            }
        })



        .state("App.ChartsofAccounts", {
            url: "/ChartsofAccount",
            templateUrl: "/App/Accounting/Views/ChartsofAccount.html",
            controller: 'ChartsofAccountsController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'iCheck', 'bootbox', 'Typeahead', 'App/Accounting/Controllers/ChartsofAccountsController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })
        .state("App.JournalEntry", {
            url: "/JournalEntry",
            templateUrl: "/App/Accounting/Views/JournalEntry.html",
            controller: 'JournalEntryController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'angucomplete', 'iCheck', 'bootbox', 'Typeahead', 'App/Accounting/Controllers/JournalEntryController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })
        .state("App.GeneralLedger", {
            url: "/GeneralLedger",
            templateUrl: "/App/Accounting/Views/GeneralLedger.html",
            controller: 'GeneralLedgerController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'angucomplete', 'iCheck', 'bootbox', 'Typeahead', 'App/Accounting/Controllers/GeneralLedgerController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })

        .state("App.GeneralLedgerDetails", {
            url: "/GeneralLedgerDetails",
            templateUrl: "/App/Accounting/Views/GeneralLedgerDetails.html",
            controller: 'GeneralLedgerDetailsController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'angucomplete', 'iCheck', 'bootbox', 'Typeahead', 'App/Accounting/Controllers/GeneralLedgerDetailsController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })

        .state("App.TrailBalance", {
            url: "/TrailBalance",
            templateUrl: "/App/Accounting/Views/TrailBalance.html",
            controller: 'TrialBalanceController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'angucomplete', 'iCheck', 'bootbox', 'Typeahead', 'App/Accounting/Controllers/TrialBalanceController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })


        .state("App.ProfitandLossStatement", {
            url: "/ProfitandLossStatement",
            templateUrl: "/App/Accounting/Views/ProfitandLossStatement.html",
            controller: 'ProfitandLossStatementController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'angucomplete', 'print', 'iCheck', 'bootbox', 'Typeahead', 'App/Accounting/Controllers/ProfitandLossStatementController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })

        .state("App.BalanceSheet", {
            url: "/BalanceSheet",
            templateUrl: "/App/Accounting/Views/BalanceSheet.html",
            controller: 'BalanceSheetController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'angucomplete', 'print', 'iCheck', 'bootbox', 'Typeahead', 'App/Accounting/Controllers/BalanceSheetController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })


        .state("App.CashFlow", {
            url: "/CashFlow",
            templateUrl: "/App/Accounting/Views/CashFlow.html",
            controller: 'CashFlowController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'angucomplete', 'print', 'iCheck', 'bootbox', 'Typeahead', 'App/Accounting/Controllers/CashFlowController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin, USER_ROLES.manager, USER_ROLES.cashier, USER_ROLES.clerk, USER_ROLES.cashierplusclerk, USER_ROLES.sales_manager]
            }
        })
        .state("App.DeleteTransactions", {
            parent: 'App',
            url: "/DeleteTransactions",
            templateUrl: "/Views/DeleteTransactions.html",
            controller: 'DeleteTransactionsController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'iCheck', 'bootbox', 'App/Controllers/DeleteTransactionsController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin]
            }
        })
        .state("App.CreditInterest", {
            parent: 'App',
            url: "/CreditInterest",
            templateUrl: "/Views/CreditInterest.html",
            controller: 'CreditInterestController',
            resolve: {
                loadMyCtrl: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load(['datatableexport', 'iCheck', 'bootbox', 'App/Controllers/CreditInterestController.js'], { serie: true }, { cache: false });
                }]
            },
            authorize: true,
            data: {
                authorizedRoles: [USER_ROLES.admin]
            }
        })

    $locationProvider.html5Mode(true).hashPrefix('*');
    $urlRouterProvider.otherwise('Login');
})

app.run(function ($rootScope, $state, $cookies, Idle, $location, AuthService, AUTH_EVENTS) {

    Idle.watch();
    $rootScope.$state = $state; // state to be accessed from view
    var User = $cookies.getObject('User');
    if (User === undefined) {
        //authService.authenticated = false;
        $state.go("Login", {}, { notify: false })
    } else {
        if (User !== undefined) {
            $rootScope.UserName = User.FirstName + " " + User.LastName;
        }
        var minutes = 30;
        var now = new Date();
        var thirtyminutes = new Date(now.getTime() + (minutes * 60 * 1000));

        $cookies.remove("User", { path: '/' }, { domain: window.location.protocol + '//' + window.location.host });
        $cookies.putObject('User', User, { 'expires': thirtyminutes }, { 'path': '/' });

        var branchId = $cookies.get('Branch');
        $cookies.remove("Branch", { path: '/' }, { domain: window.location.protocol + '//' + window.location.host });
        $cookies.put('branchId', branchId, { 'expires': thirtyminutes }, { 'path': '/' });

    }

    //$rootScope.$on('AUTH_EVENTS', function (event, data) {
    //    ;
    //    $rootScope.AuthEvent = data;
    //});


    $.ajaxSetup({
        beforeSend: function (xhr) {
            if ($cookies.get('User') != undefined) {
                xhr.setRequestHeader("Authorization", 'Bearer ' + $cookies.getObject('User').access_token);
            }
        }
    });

    $rootScope.$on('$stateChangeStart', function (event, next, current, toState, toParams, fromState, fromParams) {
        if ($cookies.get('User') != undefined) {
            if (!AuthService.isAuthenticated() && toState.name !== 'Login' && toState.name == "") {
                $rootScope.$broadcast(AUTH_EVENTS.notAuthenticated);
                //event.preventDefault();
                $state.go("Login", {}, { notify: false })
            }
            else if (AuthService.isAuthenticated()) {
                $("#overlay").show();
                var authorizedRoles = next.data.authorizedRoles;
                if (!AuthService.isAuthorized(authorizedRoles)) {
                    event.preventDefault();
                    $rootScope.$broadcast(AUTH_EVENTS.notAuthorized);
                }
            }
        }
        else {
            ;
            $state.go("Login", {}, { notify: false })
        }


        //var authorizedRoles = next.data.authorizedRoles;
        //if (!AuthService.isAuthorized(authorizedRoles)) {
        //    event.preventDefault();
        //    if (toState.authorize == false && AuthService.isAuthenticated()) {
        //        // user is not allowed
        //        $rootScope.$broadcast(AUTH_EVENTS.notAuthorized);

        //    } else {
        //        // user is not logged in

        //        $rootScope.$broadcast(AUTH_EVENTS.notAuthenticated);
        //    }
        //}

    });

    $rootScope.$on("$stateChangeError", function (event, toState, toParams, fromState, fromParams, error) {
        if (!AuthService.isAuthenticated()) {
            $rootScope.$broadcast(AUTH_EVENTS.notAuthenticated);
            $state.go("Login", {}, { notify: false })
        }
    });

    $rootScope.$on('$stateChangeSuccess', function (event, next, current, toState, toParams, fromState, fromParams) {
        if ($cookies.get('User') === undefined) {
            //authService.authenticated = false;
            ;
            $state.go("Login", {}, { notify: false })
        }
        else {
            var getUserdata = new Object();
            getUserdata = $cookies.getObject('User');
            $cookies.putObject('User', getUserdata, { path: '/' });
            if (User !== undefined) {
                $rootScope.UserName = User.FirstName + " " + User.LastName;
            }
        }
        $("#overlay").hide();
    });
});

app.directive('icheck', function ($timeout, $parse) {
    return {
        require: 'ngModel',
        link: function ($scope, element, $attrs, ngModel) {
            return $timeout(function () {
                var value;
                value = $attrs['value'];

                $scope.$watch($attrs['ngModel'], function (newValue) {
                    $(element).iCheck('update');
                });

                return $(element).iCheck({
                    checkboxClass: 'icheckbox_flat-blue',
                    radioClass: 'iradio_flat-blue checked'

                }).on('ifChanged', function (event) {
                    if ($(element).attr('type') === 'checkbox' && $attrs['ngModel']) {
                        $scope.$apply(function () {
                            return ngModel.$setViewValue(event.target.checked);
                        });
                    }
                    if ($(element).attr('type') === 'radio' && $attrs['ngModel']) {
                        return $scope.$apply(function () {
                            return ngModel.$setViewValue(value);
                        });
                    }
                });
            });
        }
    };
})

app.directive('onFinishRender', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            if (scope.$last === true) {
                $timeout(function () {
                    scope.$emit(attr.onFinishRender);
                });
            }
        }
    };
})

app.directive('validNumber', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }

            ngModelCtrl.$parsers.push(function (val) {
                if (angular.isUndefined(val)) {
                    val = '';
                }

                var clean = val.toString().replace(/[^0-9\.]/g, '');
                //var negativeCheck = clean.split('-');
                var decimalCheck = clean.split('.');
                //if (!angular.isUndefined(negativeCheck[1])) {
                //    negativeCheck[1] = negativeCheck[1].slice(0, negativeCheck[1].length);
                //    clean = negativeCheck[0] + '-' + negativeCheck[1];
                //    if (negativeCheck[0].length > 0) {
                //        clean = negativeCheck[0];
                //    }
                //}

                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 2);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }

                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });

            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
        }
    };
})

app.directive('numbersOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
            function fromUser(text) {
                if (text) {
                    var transformedInput = text.replace(/[^0-9]/g, '');

                    if (transformedInput !== text) {
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                    }
                    return transformedInput;
                }
                return undefined;
            }
            ngModelCtrl.$parsers.push(fromUser);
        }
    };
})

app.directive('toString', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$formatters.push(function (val) {
                if (val !== undefined && val !== null)
                    return '' + val;
            });
        }
    };
})

app.filter('orderObjectBy', function () {
    return function (input, attribute) {
        if (!angular.isObject(input)) return input;

        var array = [];
        for (var objectKey in input) {
            array.push(input[objectKey]);
        }

        array.sort(function (a, b) {
            if (a[attribute]) {
                return 1;
            }
            a = parseInt(a[attribute]);
            b = parseInt(b[attribute]);
            return a - b;
        });
        return array;
    };
});

app.directive('ngFiles', ['$parse', function ($parse) {

    function fn_link(scope, element, attrs) {
        var onChange = $parse(attrs.ngFiles);
        element.on('change', function (event) {
            onChange(scope, { $files: event.target.files });
        });
    };

    return {
        link: fn_link
    }
}])

app.directive('percentageField', ['$filter', function ($filter) {
    return {
        restrict: 'A',
        require: 'ngModel',
        scope: {
            // currencyIncludeDecimals: '&',

        },
        link: function (scope, element, attr, ngModel) {

            attr['percentageMaxValue'] = attr['percentageMaxValue'] || 100;
            attr['percentageMaxDecimals'] = attr['percentageMaxDecimals'] || 2;

            //  $(element).css({ 'text-align': 'right' });

            // function called when parsing the inputted url
            // this validation may not be rfc compliant, but is more
            // designed to catch common url input issues.
            function into(input) {

                var valid;

                if (input == '') {
                    ngModel.$setValidity('valid', true);
                    return '';
                }

                // if the user enters something that's not even remotely a number, reject it
                if (!input.match(/^\d+(\.\d+){0,1}%{0,1}$/gi)) {
                    ngModel.$setValidity('valid', false);
                    return '';
                }

                // strip everything but numbers from the input
                input = input.replace(/[^0-9\.]/gi, '');

                input = parseFloat(input);

                var power = Math.pow(10, attr['percentageMaxDecimals']);

                input = Math.round(input * power) / power;

                if (input > attr['percentageMaxValue']) input = attr['percentageMaxValue'];

                // valid!
                ngModel.$setValidity('valid', true);

                return input;
            }

            ngModel.$parsers.push(into);

            function out(input) {
                if (ngModel.$valid && input !== undefined && input > '') {
                    //return input + '%';
                    return input;
                }

                return '';
            }

            ngModel.$formatters.push(out);

            $(element).bind('click', function () {
                //$( element ).val( ngModel.$modelValue );
                $(element).select();
            });

            $(element).bind('blur', function () {
                $(element).val(out(ngModel.$modelValue));
            });
        }
    };
}]);

app.directive('toString', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$formatters.push(function (val) {
                if (val !== undefined && val !== null)
                    return '' + val;
            });
        }
    };
})

app.filter('words', function () {
    function isInteger(x) {
        return x % 1 === 0;
    }

    return function (value) {
        if (value && isInteger(value))

            var th = ['', 'thousand', 'million', 'billion', 'trillion'];
        var dg = ['zero', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine'];
        var tn = ['ten', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen'];
        var tw = ['twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];

        var s = value;
        s = s.toString();
        s = s.replace(/[\, ]/g, '');
        if (s != parseFloat(s)) return 'not a number';
        var x = s.indexOf('.');
        if (x == -1) x = s.length;
        if (x > 15) return 'too big';
        var n = s.split('');
        var str = '';
        var sk = 0;
        for (var i = 0; i < x; i++) {
            if ((x - i) % 3 == 2) {
                if (n[i] == '1') {
                    str += tn[Number(n[i + 1])] + ' ';
                    i++;
                    sk = 1;
                }
                else if (n[i] != 0) {
                    str += tw[n[i] - 2] + ' ';
                    sk = 1;
                }
            }
            else if (n[i] != 0) {
                str += dg[n[i]] + ' ';
                if ((x - i) % 3 == 0) str += 'hundred ';
                sk = 1;
            }


            if ((x - i) % 3 == 1) {
                if (sk) str += th[(x - i - 1) / 3] + ' ';
                sk = 0;
            }
        }
        if (x != s.length) {
            var y = s.length;
            str += 'point ';
            for (var i = x + 1; i < y; i++) str += dg[n[i]] + ' ';
        }
        return str.replace(/\s+/g, ' ');
    };

});

app.filter('sumByKey', function () {
    return function (data, key) {
        if (typeof (data) === 'undefined') {
            return 0;
        }
        var sum = 0;
        for (var i = data.length - 1; i >= 0; i--) {
            sum += parseFloat(data[i][key] != null ? data[i][key] : 0);
        }
        return sum;
    };
});