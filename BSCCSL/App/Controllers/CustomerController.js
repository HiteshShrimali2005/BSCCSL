angular.module("BSCCL").controller('CustomerController', function ($scope, $state, $cookies, $filter, AppService, $http, $location, $rootScope, $q, $timeout) {

    $scope.UserBranch.ShowBranch = $scope.BranchList.length > 1 ? true : false;
    $scope.UserBranch.Enabled = true;
    $scope.UserBranch.BranchId = $cookies.get('Branch')
    $scope.BranchCode = $filter('filter')($scope.BranchList, { BranchId: $scope.UserBranch.BranchId })[0].BranchCode;
    $rootScope.ChangeBranch = function () {
        $scope.BranchCode = $filter('filter')($scope.BranchList, { BranchId: $scope.UserBranch.BranchId })[0].BranchCode;
        $cookies.put('Branch', $scope.UserBranch.BranchId)
    }
    $("#txtstartdate,#txtdob,#txtdobminor,#txtenddate,#txtnomineedob,#txtdobappointee").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });


    //this function call when Add New Share button clicked
    $scope.Add = function () {
        CheckCustomerAccountExist();
        $scope.customershare = { ShareAmount: 100 };
        $("#maturity").val("");
        $("#txtstartdate").val("");
        $("#txtenddate").val("");
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $scope.CustomerDetails = new Object();
    $scope.CustomerDetails.Customer = {};
    $scope.customershare = new Object();
    $scope.Nominee = new Object();
    $scope.HolderImage = new Object();

    //Editing Customer Details and Find CustomerId
    if ($location.search().CustomerId != undefined) { //$location.search() is for to find out CustomerId when it passed in querystring
        $scope.UserBranch.ShowBranch = false;
        $scope.UserBranch.Enabled = false;
        var result = AppService.GetDetailsById("Customer", "GetCustomerDetailsbyId", $location.search().CustomerId)
        result.then(function (p1) {
            if (p1.data != null) {
                var data = p1.data;
                setData(data);
            }
            else {
                showToastMsg(3, 'Error in getting data from Database')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });


        if (!$scope.$$phase) {
            $scope.$apply()
        }
    }

    $scope.GetCustomerAgentId = function (Id) {
        $scope.CustomerDetails.Customer.UserAgent = Id;

        if (!$scope.$$phase) {
            $scope.$apply();
        }
    }

    //Set Customer data when page is load
    function setData(data, personId) {

        $scope.CustomerDetails.Customer = data.Customer;
        $scope.BranchCode = data.Customer.BranchCode;

        $scope.Nominee = data.Nominee;
        $scope.customershare = data.customershare;
        // $scope.Customer.Personal.Sex = data.Details[0].Personal.Sex;
        //json.stringify($scope.Customer.Personal);

        if ($scope.Nominee != null) {
            if ($scope.Nominee.NomineeDOB != undefined && $scope.Nominee.NomineeDOB != null && $scope.Nominee.NomineeDOB != "") {
                $("#txtnomineedob").data("DateTimePicker").date($filter('date')($scope.Nominee.NomineeDOB, 'dd/MM/yyyy'));
            }
            if ($scope.Nominee.AppointeeName != null && $scope.Nominee.AppointeeName != "" && $scope.Nominee.AppointeeName != undefined) {
                $scope.Nominee.minornominee1 = true;
            }
            if ($scope.Nominee.AppointeeDOB != undefined && $scope.Nominee.AppointeeDOB != null && $scope.Nominee.AppointeeDOB != "") {
                $("#txtdobappointee").data("DateTimePicker").date($filter('date')($scope.Nominee.AppointeeDOB, 'dd/MM/yyyy'));
            }
        }

        $scope.Customers = [];

        angular.forEach(data.Details, function (value, index) {
            var obj = new Object();
            obj.id = index + 1;
            obj.name = "Customer" + obj.id;
            obj.Address = value.Address;
            obj.Personal = value.Personal;
            obj.Document = value.Documents;
            obj.Personal.BirthCertificate1 = false;
            obj.Personal.Adharcard1 = false;
            obj.Personal.DrivingLicence1 = false;
            obj.Personal.Passport1 = false;
            obj.Personal.Pancard1 = false;
            obj.Personal.Other1 = false;
            obj.Personal.IdentityProof1 = false;

            obj.Address.AddressProof1 = false;
            obj.Address.PerAddressProof1 = false;
            $scope.Customers.push(obj);
        })

        $scope.CustomerDocumentData = [];


        angular.forEach(data.CustomerDocuments, function (value, index) {
            var obj1 = new Object();
            obj1.DocumentId = value.DocumentId;
            obj1.DocumentName = value.DocumentName;
            obj1.Path = value.Path;
            $scope.CustomerDocumentData.push(obj1);
        })


        angular.forEach($scope.Customers, function (newval, index) {

            if (newval.Personal.BirthCertificate != undefined && newval.Personal.BirthCertificate != null && newval.Personal.BirthCertificate != "") {
                newval.Personal.BirthCertificate1 = true;
            }
            if (newval.Personal.Adharcard != undefined && newval.Personal.Adharcard != null && newval.Personal.Adharcard != "") {
                newval.Personal.Adharcard1 = true;
            }
            if (newval.Personal.DrivingLicence != undefined && newval.Personal.DrivingLicence != null && newval.Personal.DrivingLicence != "") {
                newval.Personal.DrivingLicence1 = true;
            }
            if (newval.Personal.Passport != undefined && newval.Personal.Passport != null && newval.Personal.Passport != "") {
                newval.Personal.Passport1 = true;
            }
            if (newval.Personal.PanCard != undefined && newval.Personal.PanCard != null && newval.Personal.PanCard != "") {
                newval.Personal.Pancard1 = true;
            }
            if (newval.Personal.Other != undefined && newval.Personal.Other != null && newval.Personal.Other != "") {
                newval.Personal.Other1 = true;
            }
            if (newval.Personal.IdentityProof != undefined && newval.Personal.IdentityProof != null && newval.Personal.IdentityProof != "") {
                newval.Personal.IdentityProof1 = true;
            }

            if (newval.Address.AddressProof != undefined && newval.Address.AddressProof != null && newval.Address.AddressProof != "") {
                newval.Address.AddressProof1 = true;
            }
            if (newval.Address.PerAddressProof != undefined && newval.Address.PerAddressProof != null && newval.Address.PerAddressProof != "") {
                newval.Address.PerAddressProof1 = true;
            }
        })

        //console.log(JSON.stringify($scope.Customers))
        $scope.Photographs = [];
        $scope.Signatures = [];

        angular.forEach($scope.Customers, function (value, index) {
            var obj = new Object();
            obj.Id = value.Personal.PersonalDetailId;
            obj.PersonId = index + 1;
            obj.DocumentName = value.name;
            obj.NewDocument = '';
            obj.File = value;
            //alert(JSON.stringify(value.Personal.HolderPhotograph));
            if (value.Personal.HolderPhotograph != "" && value.Personal.HolderPhotograph != null) {
                obj.Path = value.Personal.HolderPhotograph;
            }
            else {
                obj.Path = '../Documents/no_image.png';
            }
            $scope.Photographs.push(obj);
        });

        angular.forEach($scope.Customers, function (value, index) {
            var obj = new Object();
            obj.Id = value.Personal.PersonalDetailId;
            obj.PersonId = index + 1;
            obj.DocumentName = value.name;
            obj.NewDocument = '';
            obj.File = value;
            //alert(JSON.stringify(value.Personal.HolderPhotograph));
            if (value.Personal.HolderSign != "" && value.Personal.HolderSign != null) {
                obj.Path = value.Personal.HolderSign;
            }
            else {
                obj.Path = '../Documents/no_image.png';
            }
            $scope.Signatures.push(obj);
        });

        if (!$scope.$$phase) {
            $scope.$apply();
        }
        // alert(JSON.stringify($scope.CustomerDetails.Customer.CustomerId))
        if ($scope.CustomerDetails.Customer.CustomerId != '00000000-0000-0000-0000-000000000000' && $scope.CustomerDetails.Customer.CustomerId != undefined && $scope.CustomerDetails.Customer.CustomerId != '') {
            GetShareList();
            GetBalanceById();
        }
    }

    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');//create cookie object and store  data in it

    //GetAllUser();
    $scope.CopyAddress = function (sameAddress, index) {
        if (sameAddress) {
            $scope.Customers[index].Address.PerDoorNo = $scope.Customers[index].Address.DoorNo;
            $scope.Customers[index].Address.PerBuildingName = $scope.Customers[index].Address.BuildingName;
            $scope.Customers[index].Address.PerPlotNo_Street = $scope.Customers[index].Address.PlotNo_Street;
            $scope.Customers[index].Address.PerCustomerName = $scope.Customers[index].Address.CustomerName;
            $scope.Customers[index].Address.PerLandmark = $scope.Customers[index].Address.Landmark;
            $scope.Customers[index].Address.PerArea = $scope.Customers[index].Address.Area;
            $scope.Customers[index].Address.PerDistrict = $scope.Customers[index].Address.District;
            $scope.Customers[index].Address.PerPlace = $scope.Customers[index].Address.Place;
            $scope.Customers[index].Address.PerCity = $scope.Customers[index].Address.City;
            $scope.Customers[index].Address.PerPincode = $scope.Customers[index].Address.Pincode;
            $scope.Customers[index].Address.PerState = $scope.Customers[index].Address.State;
            $scope.Customers[index].Address.PerTelephoneNo = $scope.Customers[index].Address.TelephoneNo;
            $scope.Customers[index].Address.PerMobileNo = $scope.Customers[index].Address.MobileNo;
            $scope.Customers[index].Address.PerEmail = $scope.Customers[index].Address.Email;
            $scope.Customers[index].Address.PerAddressProof = $scope.Customers[index].Address.AddressProof;
            $scope.Customers[index].Address.PerAddressProof1 = $scope.Customers[index].Address.AddressProof1;
        }
    };

    $scope.GetAllUserById = function (Id) {
        var getemployeedetail = AppService.GetDetailsById("User", "GetUserDataById", Id)
        getemployeedetail.then(function (p1) {
            if (p1.data != null) {
                if ($scope.Agent == true) {
                    $scope.CustomerDetails.Customer.AgentName = p1.data.FirstName;
                    $scope.CustomerDetails.Customer.AgentCode = p1.data.UserCode;
                    $scope.CustomerDetails.Customer.AgentId = p1.data.UserId;
                }
                else {
                    $scope.CustomerDetails.Customer.EmpName = p1.data.FirstName;
                    $scope.CustomerDetails.Customer.EmpCode = p1.data.UserCode;
                    $scope.CustomerDetails.Customer.EmployeeId = p1.data.UserId;
                }
            }
            else {
                showToastMsg(3, "Error in Getting Data");
            }
        })

    }

    $scope.Customers = [{ id: 1, name: 'Customer1', Personal: { CustomerId: '00000000-0000-0000-0000-000000000000', FirstName: '', MiddleName: '', LastName: '', FatherorSpouseName: '', MotherName: '', DOB: '', Nationality: 'Indian', Sex: 'Male', Age: '', PlaceOfBirth: '', Occupation: '', BirthCertificate: '', BirthCertificatePath: '', DrivingLicence: '', DrivingLicencePath: '', Passport: '', PassportPath: '', PanCard: '', PanCardPath: '', Other: '', OtherPath: '', IdentityProof: '', IdentityProofPath: '', Adharcard: '', HolderPhotograph: '', HolderSign: '', IsDelete: false }, Address: { CustomerId: '00000000-0000-0000-0000-000000000000', DoorNo: '', PlotNo_Street: '', CustomerName: '', Landmark: '', Area: '', District: '', Place: '', City: '', State: '', Pincode: '', TelephoneNo: '', MobileNo: '', Email: '', AddressProof: '', AddressProofPath: '', BuildingName: '', PerDoorNo: '', PerPlotNo_Street: '', PerCustomerName: '', PerLandmark: '', PerArea: '', PerDistrict: '', PerPlace: '', PerCity: '', PerState: '', PerPincode: '', PerTelephoneNo: '', PerMobileNo: '', PerEmail: '', PerAddressProof: '', PerBuildingName: '', SameAddress: false, IsDelete: false } }];

    $scope.HolderPhoto = [{ id: 1, name: 'Holder1', Photo: { CustomerId: '00000000-0000-0000-0000-000000000000', HolderPhotograph: '', } }]

    $scope.Holdersign = [{ id: 1, name: 'holdersign1', signature: { CustomerId: '00000000-0000-0000-0000-000000000000', HolderSign: '', } }]

    $scope.CustomersDocuments = [{ id: 1, name: 'customerdocument1', signature: { CustomerId: '00000000-0000-0000-0000-000000000000', HolderSign: '', } }]


    //Tab change function of next button click
    $scope.Changetab = function (next) {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        if (!ValidateAgent_Employee(next)) {
            return;
        }
        $('#tab1').attr('class', '');
        $('#tab2').attr('class', 'active');
        $('#tab_1').attr('class', 'tab-pane fade');
        $('#tab_2').attr('class', 'tab-pane fade  active in ');
    }

    $('#btntab2').bind('click', function () {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        //$(".nav nav-tabs").hide();
        //$(".active:first").show();
        //$(".nav nav-tabs li:first").addClass("active");
        if (!ValidateAgent_Employee()) {
            //Tab1 is active 
            $(".nav nav-tabs li").removeClass("active");
            $("#btntab1").parent().addClass("active");
            $(".active:first").show();
            return false;
        }
    });

    $(document).ready(function () {

        $('#myModal').on('show.bs.modal', function (e) {
            var image = $(e.relatedTarget).attr('src');
            $(".img-responsive").attr("src", image);

        });

    });

    $scope.RemoveErrorMsg = function (index, name) {

        if (!$('#chk' + name + '_' + index).attr('checked')) {
            $('#chk' + name + '_' + index).closest('.form-group').removeClass('has-error');
            $('#chk' + name + '_' + index).next('.help-block').remove();
            $('#chk' + name + '_' + index).prev('.help-block').remove();
            $('#txt' + name + '_' + +index).closest('.form-group').removeClass('has-error');
            $('#txt' + name + '_' + +index).next('.help-block').remove();
            $('#txt' + name + '_' + +index).prev('.help-block').remove();
        }

    }

    function ValidateAgent_Employee() {

        var flag = true;

        //if (!ValidateRequiredFieldInputField($("#txtagentname"), 'Agent Name required', 'after')) {
        //    flag = false;
        //}
        //if (!ValidateRequiredField($("#txtagentcode"), 'Agent Code required', 'after')) {
        //    flag = false;
        //}
        if (!ValidateRequiredFieldInputField($("#txtempname"), 'Employee Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtempcode"), 'Employee Code required', 'after')) {
            flag = false;
        }
        if (!CheckRadioChecked($("#Radiourban_"), $("#Radiorural_"), 'Sector Required', 'after')) {
            flag = false;
        }
        if (!CheckRadioChecked($("#RadioG_"), $("#RadioH_"), 'Form Type Required', 'after')) {
            flag = false;
        }
        return flag;
    }

    //this is for to add new holder in customer form
    $scope.addNewChoice = function (lim) {

        $scope.limit1 = 3;
        var newItemNo = $scope.Customers.length + 1;
        $scope.Customers.push({ 'id': newItemNo, 'name': 'Customer' + newItemNo, Personal: { CustomerId: '00000000-0000-0000-0000-000000000000', FirstName: '', MiddleName: '', LastName: '', FatherorSpouseName: '', MotherName: '', DOB: '', Nationality: 'Indian', Sex: 'male', Age: '', PlaceOfBirth: '', Occupation: '', BirthCertificate: '', BirthCertificatePath: '', DrivingLicence: '', DrivingLicencePath: '', Passport: '', PassportPath: '', PanCard: '', PanCardPath: '', Other: '', OtherPath: '', IdentityProof: '', IdentityProofPath: '', Adharcard: '', HolderPhotograph: '', HolderSign: '', IsDelete: false }, Address: { CustomerId: '00000000-0000-0000-0000-000000000000', DoorNo: '', PlotNo_Street: '', CustomerName: '', Landmark: '', Area: '', District: '', Place: '', City: '', State: '', Pincode: '', TelephoneNo: '', MobileNo: '', Email: '', AddressProof: '', AddressProofPath: '', BuildingName: '', PerDoorNo: '', PerPlotNo_Street: '', PerCustomerName: '', PerLandmark: '', PerArea: '', PerDistrict: '', PerPlace: '', PerCity: '', PerState: '', PerPincode: '', PerTelephoneNo: '', PerMobileNo: '', PerEmail: '', PerAddressProof: '', PerBuildingName: '', SameAddress: false, IsDelete: false } });
        $scope.limit1 = (lim <= 0) ? $scope.Customers.length : lim;
    };

    $scope.removeNewChoice = function (lim) {
        $scope.limit2 = 1;
        var newItemNo = $scope.Customers.length - 1;
        if (newItemNo !== 0) {
            $scope.limit2 = (lim <= 1) ? $scope.Customers.length : lim;
            $scope.Customers.length
            $scope.Customers.pop();

        }
    };

    $scope.showAddChoice = function (choice) {
        return Customers.id === $scope.Customers[$scope.Customers.length - 1].id;
    };

    $scope.AssignUserCode = function (user) {
        var users = $filter('filter')($scope.User, { UserId: user })[0];
        $scope.CustomerDetails.Customer.UserCode = users.UserCode;
    }

    $scope.$on('ngRepeatFinished', function (ngRepeatFinishedEvent) {
        $(".datepicker").datetimepicker({
            format: 'DD/MM/YYYY',
            useCurrent: false,
        });

        $('.datepicker').on('dp.change', function (e) {
            var index = $(this).attr('index');
            index = parseInt(index);
            if ($("#txtDOB_" + index).val() != undefined && $("#txtDOB_" + index).val() != '') {
                var date = moment(new Date($("#txtDOB_" + index).data("DateTimePicker").date())).format('YYYY-MM-DD')
                var currentday = new Date();
                var bDate = new Date(date);
                var age = currentday.getFullYear() - bDate.getFullYear();
                var mt = currentday.getMonth() - bDate.getMonth();
                if (mt < 0 || (mt === 0 && currentday.getDate() < bDate.getDate())) {
                    age--;
                }
                $scope.Customers[index].Personal.Age = age

                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }
        })

        angular.forEach($scope.Customers, function (value, index) {

            $("#txtDOB_" + index).data("DateTimePicker").date($filter('date')(value.Personal.DOB, 'dd/MM/yyyy'));
        });
    });

    $scope.SaveCustomerDetails = function (next) {

        var flag = true
        flag = Validatecustomeradress();

        if (flag) {

            var fd = new FormData();

            if ($scope.Files.length > 0) {
                angular.forEach($scope.Files, function (value, index) {
                    fd.append("file", value.File);
                });
            }
            $scope.CustomerDetails.Personal = [];
            $scope.CustomerDetails.Address = [];
            if ($scope.CustomerDetails.Customer.CustomerId != '00000000-0000-0000-0000-000000000000' && $scope.CustomerDetails.Customer.CustomerId != undefined && $scope.CustomerDetails.Customer.CustomerId != '') {
                $scope.CustomerDetails.Customer.ModifiedBy = getUserdata.UserId;
            }
            else {
                $scope.CustomerDetails.Customer.CreatedBy = getUserdata.UserId;
                $scope.CustomerDetails.Customer.BranchId = $scope.UserBranch.BranchId;
                $scope.CustomerDetails.Customer.BranchCode = $scope.BranchCode;
            }

            angular.forEach($scope.Customers, function (Customer, index) {
                Customer.Personal.DOB = moment(new Date($("#txtDOB_" + index).data("DateTimePicker").date())).format('YYYY-MM-DD');
                var docs = $filter('filter')($scope.Files, { PersonId: Customer.id });
                var Personal = new Object();
                Personal.Personal = Customer.Personal
                Personal.Address = Customer.Address
                Personal.Documents = docs;
                $scope.CustomerDetails.Personal.push(Personal);
            });

            fd.append("data", JSON.stringify($scope.CustomerDetails));

            // alert(JSON.stringify($scope.CustomerDetails))
            var savecustomer = AppService.UploadDocumentwithData("Customer", "CustomerRegister", fd)
            savecustomer.then(function (p1) {
                if (p1.data != null) {
                    if (next == true) {
                        $("#btntab4").click();
                    }
                    setData(p1.data);
                    showToastMsg(1, "Customer Saved Successfully")
                    $rootScope.CountValue = 0;

                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving Data')
            });

        }
        else {
            return false;
        }
    }

    $scope.SaveCustomer = function (next) {
        var flag = true
        flag = Validatecustomeradress();

        if ($scope.Customers[0].Personal.FirstName != "" && $scope.Customers[0].Personal.MiddleName != "" && $scope.Customers[0].Personal.LastName != "" &&
            $("#txtDOB_0").val() != null && $("#txtDOB_0").val() != "" && $("#txtDOB_0").val() != undefined &&
            ($scope.CustomerDetails.Customer.CustomerId == '00000000-0000-0000-0000-000000000000' || $scope.CustomerDetails.Customer.CustomerId == undefined ||
                $scope.CustomerDetails.Customer.CustomerId == '')) {

            var name = $scope.Customers[0].Personal.FirstName + " " + $scope.Customers[0].Personal.MiddleName + " " + $scope.Customers[0].Personal.LastName;
            var dob = moment(new Date($("#txtDOB_0").data("DateTimePicker").date())).format('YYYY-MM-DD');;

            // var q = $q.defer();
            var customerId = AppService.GetDataByQuerystring("Customer", "CheckCustomerExist", name, dob);
            customerId.then(function (p1) {
                //q.resolve(p1.data);

                if (p1.data) {
                    bootbox.dialog({
                        message: "Customer Already Exist? Are you sure want to continue?",
                        title: "Confirmation !",
                        size: 'small',
                        buttons: {
                            success: {
                                label: "Yes",
                                className: "btn-success btn-flat",
                                callback: function () {
                                    $scope.SaveCustomerDetails(next)
                                }
                            },
                            danger: {
                                label: "No",
                                className: "btn-danger btn-flat",
                                callback: function () {
                                    flag1 = false;
                                }
                            }
                        }
                    });
                }
                else {
                    $scope.SaveCustomerDetails(next)
                }
            });
            // return q.promise;
        }
        else {
            $scope.SaveCustomerDetails(next)
        }

    }

    $scope.AccountNumber = '';

    function CheckCustomerAccountExist() {
        var customerId = AppService.GetDetailsById("CustomerProduct", "GetSavingAccountNo", $scope.CustomerDetails.Customer.CustomerId);
        customerId.then(function (p1) {
            if (p1.data != null) {
                $scope.AccountNumber = p1.data;
            }
        });
    }

    //validate CustomerData and Documents
    function Validatecustomeradress() {

        var flag = true;

        angular.forEach($scope.Customers, function (value, index) {

            if (!ValidateRequiredField($("#txtplotnostreet_" + index), 'PlotNo. or Street required', 'after')) {
                flag = false;
            }
            //if (!ValidateRequiredField($("#txtCustomerName_" + index), 'Name required', 'after')) {
            //    flag = false;
            //}
            //if (!ValidateRequiredField($("#txtLandmark_" + index), 'Landmark required', 'after')) {
            //    flag = false;
            //}
            if (!ValidateRequiredField($("#txtArea_" + index), 'Area required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtDistrict_" + index), 'District required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtPinCode_" + index), 'PinCode required', 'after')) {
                flag = false;
            }
            //if (!ValidateRequiredField($("#txtmothername_" + index), 'Mother Name required', 'after')) {
            //    flag = false;
            //}
            if (!CheckpinCode($("#txtPinCode_" + index), 'Pincode Is Not Valid', 'after')) {
                flag = false;
            }

            if (!ValidateRequiredField($("#txtperplotnostreet_" + index), 'PlotNo. or Street required', 'after')) {
                flag = false;
            }
            //if (!ValidateRequiredField($("#txtperCustomerName_" + index), 'Name required', 'after')) {
            //    flag = false;
            //}
            //if (!ValidateRequiredField($("#txtPerLandmark_" + index), 'Landmark required', 'after')) {
            //    flag = false;
            //}
            if (!ValidateRequiredField($("#txtperArea_" + index), 'Area required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtperDistrict_" + index), 'District required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtperPinCode_" + index), 'PinCode required', 'after')) {
                flag = false;
            }
            if (!CheckpinCode($("#txtperPinCode_" + index), 'PinCode Not Valid', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtPerMobileNo_" + index), 'Mobile required', 'after')) {
                flag = false;
            }
            if (!CheckMobileNumandphnnum($("#txtPerMobileNo_" + index), 'MobileNumber Not Valid', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtMobileNo_" + index), 'Mobile required', 'after')) {
                flag = false;
            }
            if (!CheckMobileNumandphnnum($("#txtMobileNo_" + index), 'MobileNumber Not Valid', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtFirstName_" + index), 'First Name required', 'after')) {
                flag = false;
            }
            if (!CheckOnlyText($("#txtFirstName_" + index), 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtmidlname_" + index), 'Middle Name required', 'after')) {
                flag = false;
            }
            if (!CheckOnlyText($("#txtmidlname_" + index), 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtLastName_" + index), 'Last Name required', 'after')) {
                flag = false;
            }
            if (!CheckOnlyText($("#txtLastName_" + index), 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtDOB_" + index), 'Date Of Birth required', 'after')) {
                flag = false;
            }
            if (value.Personal.BirthCertificate1 == true) {
                if (!ValidateRequiredField($("#txtBirthCertificate_" + index), 'Birth Certificate Number required', 'after')) {
                    flag = false;
                }

                var doc1 = $filter('filter')($scope.Files, { PersonId: value.id, ProofTypeId: 1 });
                if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                    if (doc1.length == 0) {
                        flag = false;
                        showToastMsg(3, 'Select Birth Certificate for Account Holder ' + value.id);
                    }
                }
                else {
                    if (doc1.length == 0) {
                        var docbirth = $filter('filter')(value.Document, { ProofTypeId: 1 });
                        if (docbirth.length == 0) {
                            flag = false;
                            showToastMsg(3, 'Select Birth Certificate for Account Holder ' + value.id);
                        }
                    }
                }
            }
            if (value.Personal.Adharcard1 == true) {

                if (!ValidateRequiredField($("#txtAdharcard_" + index), 'Adhar Card Number required', 'after')) {
                    flag = false;
                }

                var doc2 = $filter('filter')($scope.Files, { PersonId: value.id, ProofTypeId: 8 });
                if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                    if (doc2.length == 0) {
                        flag = false;
                        showToastMsg(3, 'Select Adhar Card for Account Holder ' + value.id);
                    }
                }
                else {
                    if (doc2.length == 0) {
                        var docadhar = $filter('filter')(value.Document, { ProofTypeId: 8 });
                        if (docadhar.length == 0) {
                            flag = false;
                            showToastMsg(3, 'Select Adhar Card for Account Holder ' + value.id);
                        }
                    }
                }
            }
            if (value.Personal.DrivingLicence1 == true) {
                if (!ValidateRequiredField($("#txtDrivingLicence_" + index), 'Driving Licence Number required', 'after')) {
                    flag = false;
                }
                var doc3 = $filter('filter')($scope.Files, { PersonId: value.id, ProofTypeId: 2 });
                if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                    if (doc3.length == 0) {
                        flag = false;
                        showToastMsg(3, 'Select Driving Licence for Account Holder ' + value.id);
                    }
                }
                else {
                    if (doc3.length == 0) {
                        var docdriving = $filter('filter')(value.Document, { ProofTypeId: 2 });
                        if (docdriving.length == 0) {
                            flag = false;
                            showToastMsg(3, 'Select Driving Licence for Account Holder ' + value.id);
                        }
                    }
                }
            }
            if (value.Personal.Passport1 == true) {
                if (!ValidateRequiredField($("#txtPassport_" + index), 'Passport Number required', 'after')) {
                    flag = false;
                }
                var doc4 = $filter('filter')($scope.Files, { PersonId: value.id, ProofTypeId: 3 });
                if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                    if (doc4.length == 0) {
                        flag = false;
                        showToastMsg(3, 'Select Passport for Account Holder ' + value.id);
                    }
                }
                else {
                    if (doc4.length == 0) {
                        var docpassport = $filter('filter')(value.Document, { ProofTypeId: 3 });
                        if (docpassport.length == 0) {
                            flag = false;
                            showToastMsg(3, 'Select Passport for Account Holder ' + value.id);
                        }
                    }
                }
            }
            if (value.Personal.Pancard1 == true) {
                if (!ValidateRequiredField($("#txtPancard_" + index), 'Pancard Number required', 'after')) {
                    flag = false;
                }

                var doc5 = $filter('filter')($scope.Files, { PersonId: value.id, ProofTypeId: 4 });
                if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                    if (doc5.length == 0) {
                        flag = false;
                        showToastMsg(3, 'Select Pancard for Account Holder ' + value.id);
                    }
                }
                else {
                    if (doc5.length == 0) {
                        var docpan = $filter('filter')(value.Document, { ProofTypeId: 4 });
                        if (docpan.length == 0) {
                            flag = false;
                            showToastMsg(3, 'Select Pancard for Account Holder ' + value.id);
                        }
                    }
                }
            }
            if (value.Personal.Other1 == true) {

                if (!ValidateRequiredField($("#txtOther_" + index), 'Other Document Number required', 'after')) {
                    flag = false;
                }

                var doc6 = $filter('filter')($scope.Files, { PersonId: value.id, ProofTypeId: 5 });
                if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                    if (doc6.length == 0) {
                        flag = false;
                        showToastMsg(3, 'Select Other Document for Account Holder ' + value.id);
                    }
                }
                else {
                    if (doc6.length == 0) {
                        var docother = $filter('filter')(value.Document, { ProofTypeId: 5 });
                        if (docother.length == 0) {
                            flag = false;
                            showToastMsg(3, 'Select Other Document for Account Holder ' + value.id);
                        }
                    }
                }

            }
            if (value.Personal.IdentityProof1 == true) {
                if (!ValidateRequiredField($("#txtidentityproof_" + index), 'Identity Proof Number required', 'after')) {
                    flag = false;
                }
                var doc7 = $filter('filter')($scope.Files, { PersonId: value.id, ProofTypeId: 6 });
                if (doc7.length == 0) {
                    if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                        flag = false;
                        showToastMsg(3, 'Select Identity Proof for Account Holder ' + value.id);
                    }
                }
                else {
                    if (doc7.length == 0) {
                        var docidentity = $filter('filter')(value.Document, { ProofTypeId: 6 });
                        if (docidentity.length == 0) {
                            flag = false;
                            showToastMsg(3, 'Select Identity Proof for Account Holder ' + value.id);
                        }
                    }
                }
            }
            if (value.Address.AddressProof1 == true) {
                if (!ValidateRequiredField($("#txtaddressproof_" + index), 'Address Proof Number required', 'after')) {
                    flag = false;
                }
            }
            if (value.Address.PerAddressProof1 == true) {
                if (!ValidateRequiredField($("#txtperaddressproof_" + index), 'Permanent Address Proof Number required', 'after')) {
                    flag = false;
                }
            }
            if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                if (value.Address.AddressProof1 != true) {
                    flag = false;
                    showToastMsg(3, 'Address proof Required for Account Holder' + value.id)
                }
            }
            if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                if (!value.Address.SameAddress) {
                    if (value.Address.PerAddressProof1 != true) {
                        flag = false;
                        showToastMsg(3, 'Permanent Address proof Required for Account Holder' + value.id)
                    }
                }
            }
            if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                var docs = $filter('filter')($scope.Files, { PersonId: value.id, ProofTypeId: '!7' });

                if (!value.Address.SameAddress) {
                    var docs1 = $filter('filter')(docs, { ProofTypeId: '!9' });
                    if (docs1.length == 0) {
                        flag = false;
                        showToastMsg(3, 'Select Any one Proof Document for Account Holder ' + value.id);
                    }
                }
                else {
                    if (docs.length == 0) {
                        flag = false;
                        showToastMsg(3, 'Select Any one Proof Document for Account Holder ' + value.id);
                    }
                }
            }
            if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                var doc = $filter('filter')($scope.Files, { PersonId: value.id, ProofTypeId: 7 });
                if (doc.length == 0) {
                    flag = false;
                    showToastMsg(3, 'Select Address Proof for Account Holder ' + value.id);
                }

            }
            if (value.Personal.PersonalDetailId == '00000000-0000-0000-0000-000000000000' || value.Personal.PersonalDetailId == undefined) {
                if (!value.Address.SameAddress) {
                    var docs = $filter('filter')($scope.Files, { PersonId: value.id, ProofTypeId: 9 });
                    if (docs.length == 0) {
                        flag = false;
                        showToastMsg(3, 'Select Permanent Address Proof for Account Holder ' + value.id);
                    }
                }
            }


        })
        return flag;
    }

    $scope.Files = [];

    //Document Upload Function
    $scope.getTheFiles = function ($files, name, personId) {

        angular.forEach($files, function (value, key) {

            if (value.type == "image/png" || value.type == "image/jpg" || value.type == "image/jpeg") {
                var sizeInMB = (value.size / (1024 * 1024)).toFixed(2);
                if (sizeInMB < 10) {
                    var obj = new Object();
                    obj.ProofTypeId = name
                    obj.DocumentName = value.name;
                    obj.File = value;
                    obj.PersonId = personId;
                    //obj.DocumentId = value.DocumentId;
                    obj.IsDelete = false;

                    var docs = $filter('filter')($scope.Files, { PersonId: personId, ProofTypeId: name })[0];

                    if (docs != undefined) {

                        angular.forEach($scope.Files, function (value, index) {
                            //var index = index + 1;
                            if (value.PersonId == personId && value.ProofTypeId == name) {
                                $scope.Files[index] = obj
                                //value = obj;
                            }
                        });
                    }
                    else {
                        $scope.Files.push(obj);
                    }
                }
                else {
                    showToastMsg("Error", 3, "File size should be less than 10 MB.");
                    return false;
                }

            }
            else {
                showToastMsg(3, "Only Image format like .jpg .jpeg .png is Allowed");
                //$("#birthdocument_" + personId).val("");
                //$("#adhar_" + personId).val("");
                //$("#drivinglicencefile_" + personId).val("");
                //$("#passportfile_" + personId).val("");

                //$("#pancard_" + personId).val("");
                //$("#OtherFile_" + personId).val("");
                //$("#IdentityProof_" + personId).val("");
                //$("#addressProof_" + personId).val("");
                //$("#peraddressProof_" + personId).val("");
                return false;
            }

        });
    };

    $scope.ClearForm = function () {
        $scope.CustomerDetails.Customer = {};
        $scope.CustomerDetails.Personal = [];
        $scope.CustomerDetails.Address = [];
        $scope.Customers = [];
        $scope.Customers = [{ id: 1, name: 'Customer1', Personal: { CustomerId: '00000000-0000-0000-0000-000000000000', FirstName: '', MiddleName: '', LastName: '', FatherorSpouseName: '', MotherName: '', DOB: '', Nationality: '', Sex: '', Age: '', PlaceOfBirth: '', Occupation: '', BirthCertificate: '', BirthCertificatePath: '', DrivingLicence: '', DrivingLicencePath: '', Passport: '', PassportPath: '', PanCard: '', PanCardPath: '', Other: '', OtherPath: '', IdentityProof: '', IdentityProofPath: '', Adharcard: '', HolderPhotograph: '', HolderSign: '' }, Address: { CustomerId: '00000000-0000-0000-0000-000000000000', DoorNo: '', PlotNo_Street: '', CustomerName: '', Landmark: '', Area: '', District: '', Place: '', City: '', State: '', Pincode: '', TelephoneNo: '', MobileNo: '', Email: '', AddressProof: '', AddressProofPath: '', BuildingName: '', PerDoorNo: '', PerPlotNo_Street: '', PerCustomerName: '', PerLandmark: '', PerArea: '', PerDistrict: '', PerPlace: '', PerCity: '', PerState: '', PerPincode: '', PerTelephoneNo: '', PerMobileNo: '', PerEmail: '', PerAddressProof: '', PerBuildingName: '' } }];
        $scope.customershare = null;
    }

    $scope.Balance = "";
    function GetBalanceById() {
        var getBalance = AppService.GetDetailsById("Customer", "GetBalanceById", $scope.CustomerDetails.Customer.CustomerId);
        getBalance.then(function (p1) {
            if (p1.data != null) {
                $scope.Balance = p1.data.Balance;

            }
        })

    }

    $scope.SaveCustomerShare = function () {
        var flag = true;
        flag = Validatecustomershare();

        if (flag) {
            $(':focus').blur();
            $scope.customershare.CustomerId = $scope.CustomerDetails.Customer.CustomerId;
            // $scope.customershare.CreatedBy = $cookies.getObject('User').UserId;
            $scope.customershare.StartDate = moment(new Date($("#txtstartdate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            //$scope.customershare.Maturity = $("#maturity").val();
            //if ($("#txtenddate").val() != "" && $("#txtenddate").val() != null && $("#txtenddate").val() != undefined) {
            //    $scope.customershare.EndDate = moment(new Date($("#txtenddate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            //}
            //else {
            //    $scope.customershare.EndDate = null;
            //}

            if ($scope.customershare.ShareId == '0000000-0000-0000-0000-000000000000' || $scope.customershare.ShareId == undefined || $scope.customershare.ShareId == null) {
                $scope.customershare.CreatedBy = $cookies.getObject('User').UserId;
            }
            else {
                $scope.customershare.ModifiedBy = $cookies.getObject('User').UserId;
            }

            if ($scope.AccountNumber != null && $scope.AccountNumber != undefined && $scope.AccountNumber != "") {
                if (($scope.Balance < $scope.customershare.Total) && $scope.customershare.Maturity == 2) {
                    showToastMsg(3, 'Insufficient Balance');
                }
                else {
                    //var bal = $scope.Balance - $scope.customershare.Total;
                    var customershare = AppService.SaveData("Customer", "SaveCustomerShare", $scope.customershare)
                    customershare.then(function (p1) {
                        if (p1.data != null) {
                            $scope.customershare = p1.data;
                            //CheckCustomerAccountExist();
                            $('#Share').modal('hide');
                            showToastMsg(1, "Share Saved Successfully")

                            RefreshDataTablefnDestroy();
                            GetShareList();
                            $scope.ClearFormShare();
                        }

                        else {
                            showToastMsg(3, 'Error In Saving Data')
                        }
                    });
                }

            }
            else {
                showToastMsg(3, 'Please Create Saving Account.')
            }
        }
    }

    function GetShareDataById(Id) {
        var getuserdata = AppService.GetDetailsById("Customer", "GetShareDataById", Id);
        getuserdata.then(function (p1) {
            if (p1.data != null) {
                $scope.customershare = p1.data;
                $scope.customershare.Maturity = p1.data.Maturity + ''
                $("#txtstartdate").data("DateTimePicker").date($filter('date')($scope.customershare.StartDate, 'dd/MM/yyyy'));
                $("#txtenddate").data("DateTimePicker").date($filter('date')($scope.customershare.EndDate, 'dd/MM/yyyy'));
                $("#maturity").val($scope.customershare.Maturity)
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.customershare.Share = 0;
    $scope.isTotalCalculated = true;
    $scope.customershare.Total = 0;
    updateTotal();//this function for calculating the Number of share and their total

    $scope.updateTotal = updateTotal;

    function updateTotal() {
        if (!$scope.isTotalCalculated) return;
        $scope.customershare.Total = ($scope.customershare.Share * $scope.customershare.ShareAmount) || 0;
    }

    function Validatecustomershare() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtshare"), 'Share required', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#txtstartdate"), 'Start Date required', 'after')) {
            flag = false;
        }
        //if (!ValidateRequiredField($("#txtenddate"), 'End Date required', 'after')) {
        //    flag = false;
        //}
        if (!ValidateRequiredField($("#maturity"), 'Share Type required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtcertynum"), 'Certificate Number required', 'after')) {
            flag = false;
        }

        if ($scope.CustomerDetails.Customer.OldClientId != null && $scope.CustomerDetails.Customer.OldClientId != "" && $scope.CustomerDetails.Customer.OldClientId != undefined) {
            if (!ValidateRequiredField($("#txtOldfromnum"), 'Old From No. required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtOldtonum"), 'To Old To No. required', 'after')) {
                flag = false;
            }
        }

        if ($scope.customershare.DeductAdmissionFee) {
            if (!$scope.customershare.DeductShareAmount) {
                if ($scope.customershare.DeductShareAmount == undefined) {
                    $("#DeductShareAmountCheckBox").val('')
                    $scope.customershare.DeductShareAmount = false;
                }
                if (!ValidateRequiredFieldCheckBox($("#DeductShareAmountCheckBox"), 'Select Deduct Share Amount', 'after')) {
                    flag = false;
                }
            }
        }




        return flag;
    }

    $scope.ClearFormShare = function () {

        $scope.customershare = {};
        $scope.customershare.ShareAmount = 100
        $('#Share').modal('hide');
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    function GetShareList() {

        $('#tblCustomer').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/Customer/GetShareList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });
                aoData.push({ "name": "id", "value": $scope.CustomerDetails.Customer.CustomerId });

                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {

                        fnCallback(json);
                        IntialPageControlShare();
                    }
                });
            },
            "aoColumns": [{
                "mDataProp": "Share",
                "mRender": function (data, type, full) {
                    return full.Share
                },
            },
            {
                "mDataProp": "ShareAmount",
            },
            {
                "mDataProp": "Total",
            },
            {
                "mDataProp": "StartDate",
                "mRender": function (data, type, full) {
                    return $filter('date')(data, 'dd/MM/yyyy');
                },
            },
            //{
            //    "mDataProp": "EndDate",
            //    "mRender": function (data, type, full) {
            //        return $filter('date')(data, 'dd-MM-yyyy');
            //    },
            //},
            {
                "mDataProp": "MaturityName",
            },
            {
                "mDataProp": "FromNumber",
            },
            {
                "mDataProp": "ToNumber",
            },
            {
                "mDataProp": "IsReverted",
                "mRender": function (data, type, full) {
                    if (full.IsReverted == 1) {
                        return '<span class="label" style="background-color:green">Refunded</span>';
                    }
                    else {
                        return "";
                    }

                }
            },

            {
                "mDataProp": "ShareId",
                "mRender": function (data, type, full) {
                    var str = '';
                    if (full.IsReverted == 0 || full.IsReverted == null) {
                        str = '<button class="btn btn-primary btn-xs btnPrint " Id="' + data + '" title="Print"><span class="glyphicon glyphicon-print"></span> Print </button>';
                        str += ' <button class="btn btn-success btn-xs btnRefundShare" Id="' + data + '" title="Refund Share Amount"><span class="fa fa-money"></span>  Refund </button>';

                    }
                    return str;
                },
                "sClass": "text-center"
            },
                //{
                //    "mDataProp": "ShareId",
                //    "mRender": function (data, type, full) {
                //        return '<button class="btn btn-primary btn-xs btnRevertShare" Id="' + data + '" title="Revert Share Amount"><span class="glyphicon glyphicon-print"></span> Pay </button>';
                //    },
                //    "sClass": "text-center"
                //},

            ]
        });
    }

    function IntialPageControlShare() {
        $(".btnEdit").click(function () {
            var ID = $(this).attr("Id");
            $("#Share").modal('show');
            GetShareDataById(ID)
            CheckCustomerAccountExist();
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
                            var promiseDelete = AppService.DeleteData("Customer", "DeleteShare", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {
                                    RefreshDataTablefnDestroy();
                                    toastr.remove();
                                    showToastMsg(1, "Share Deleted Successfully");
                                    GetShareList();
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

        $(".btnPrint").click(function () {
            var ID = $(this).attr("Id");
            $("#ShareCerty").modal('show');
            GetShareDetailForPrint(ID)
        });

        $(".btnRefundShare").click(function () {
            var ID = $(this).attr("Id");
            bootbox.hideAll();
            bootbox.dialog({
                message: "Are you sure want to Refund Share amount?",
                title: "Confirmation !",
                size: 'small',
                buttons: {
                    success: {
                        label: "Yes",
                        className: "btn-success btn-flat",
                        callback: function () {
                            var promiseDelete = AppService.DeleteData("Customer", "RefundShareAmount", ID);
                            promiseDelete.then(function (p1) {
                                if (p1.data == 1) {
                                    GetShareList();
                                    showToastMsg(1, "Share Refunded Successfully");

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

    function GetShareDetailForPrint(ID) {

        var getuserdata = AppService.GetDetailsById("Customer", "GetShareDetailForPrint", ID);

        getuserdata.then(function (p1) {
            if (p1.data != null) {
                $scope.customershare = p1.data;
                $scope.Customername = p1.data.trimmedString;
                //$scope.Customername = p1.data.trimmedString.substring(0, p1.data.trimmedString.length - 1);
                $scope.NumofShare = p1.data.ShareDetails.Share;
                $scope.TotalAmount = p1.data.ShareDetails.Total;
                $scope.Customeaddress = p1.data.Address.address;
                $scope.NomineeName = p1.data.nominee
                $scope.ShareFromNumber = p1.data.ShareDetails.FromNumber;
                $scope.ShareToNumber = p1.data.ShareDetails.ToNumber;
                $scope.CertificateNumber = p1.data.ShareDetails.CertificateNumber;
                $scope.MembershipNo = p1.data.Address.MembershipNo;
                //$("#txtstartdate").data("DateTimePicker").date($filter('date')($scope.customershare.StartDate, 'dd/MM/yyyy'));
                //$("#txtenddate").data("DateTimePicker").date($filter('date')($scope.customershare.EndDate, 'dd/MM/yyyy'));
                //$("#maturity").val($scope.customershare.Maturity)
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.printPageArea = function (areaID) {
        var printContent = document.getElementById(areaID);
        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
        WinPrint.document.write("<style>");
        WinPrint.document.write("#tblholderdetail {padding-top:0px;padding-bottom:100px;} </style>");
        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        //WinPrint.close();
        $timeout(function () { WinPrint.close(); }, 2000);
    };

    function RefreshDataTablefnDestroy() {
        $("#tblCustomer").dataTable().fnDestroy();
    }

    $scope.SaveNominee = function (next) {

        var flag = true;
        flag = ValidateNomineeDetail();
        if (flag) {

            $scope.Nominee.CustomerId = $scope.CustomerDetails.Customer.CustomerId;
            $scope.Nominee.CreatedBy = $cookies.getObject('User').UserId;
            if ($("#txtnomineedob").val() != "" && $("#txtnomineedob").val() != null && $("#txtnomineedob").val() != undefined) {
                $scope.Nominee.NomineeDOB = moment(new Date($("#txtnomineedob").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            else {
                $scope.Nominee.NomineeDOB = null;
            }
            if ($("#txtdobappointee").val() != "" && $("#txtdobappointee").val() != null && $("#txtdobappointee").val() != undefined) {
                $scope.Nominee.AppointeeDOB = moment(new Date($("#txtdobappointee").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            else {
                $scope.Nominee.AppointeeDOB = null;
            }

            if ($scope.Nominee.NomineeId == '0000000-0000-0000-0000-000000000000' || $scope.Nominee.NomineeId == undefined) {
                $scope.Nominee.CreatedBy = $cookies.getObject('User').UserId;
            }
            else {
                $scope.Nominee.ModifiedBy = $cookies.getObject('User').UserId;
            }

            var savenominee = AppService.SaveData("Customer", "SaveNominee", $scope.Nominee)
            savenominee.then(function (p1) {
                if (p1.data != null) {
                    $scope.Nominee = p1.data;

                    if ($scope.Nominee != null) {
                        if ($scope.Nominee.NomineeDOB != undefined && $scope.Nominee.NomineeDOB != null && $scope.Nominee.NomineeDOB != "") {
                            $("#txtnomineedob").data("DateTimePicker").date($filter('date')($scope.Nominee.NomineeDOB, 'dd/MM/yyyy'));
                        }
                        if ($scope.Nominee.AppointeeName != null && $scope.Nominee.AppointeeName != "" && $scope.Nominee.AppointeeName != undefined) {
                            $scope.Nominee.minornominee1 = true;
                        }

                        if ($scope.Nominee.AppointeeDOB != undefined && $scope.Nominee.AppointeeDOB != null && $scope.Nominee.AppointeeDOB != "") {
                            $("#txtdobappointee").data("DateTimePicker").date($filter('date')($scope.Nominee.AppointeeDOB, 'dd/MM/yyyy'));
                        }
                    }

                    if (next == true) {
                        $("#btntab5").click();
                    }
                    showToastMsg(1, "Nominee Saved Successfully")

                }
                else {
                    showToastMsg(3, 'Error in Saving Data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in saving data')
            });
        }
    }

    function ValidateNomineeDetail() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtName"), 'Name & Surname required', 'after')) {
            flag = false;
        }
        if (!CheckOnlyText($("#txtName"), 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtbirth"), 'Place of Birth required', 'after')) {
            flag = false;
        }
        if (!CheckOnlyText($("#txtbirth"), 'after')) {
            flag = false;
        }
        //if (!ValidateRequiredField($("#txtnomineedob"), ' DateofBirth required', 'after')) {
        //    flag = false;
        //}
        if (!ValidateRequiredField($("#txtaccountholderrelation"), 'Relationship to Account Holder required', 'after')) {
            flag = false;
        }
        if (!CheckOnlyText($("#txtaccountholderrelation"), 'after')) {
            flag = false;
        }
        if ($scope.Nominee.minornominee1 == true) {
            if (!ValidateRequiredField($("#txtnamesurname"), 'Appointee Name & Surname required', 'after')) {
                flag = false;
            }
            if (!CheckOnlyText($("#txtnamesurname"), 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtbirthplaceappointee"), 'Appointee Birth Place required', 'after')) {
                flag = false;
            }
            if (!CheckOnlyText($("#txtbirthplaceappointee"), 'after')) {
                flag = false;
            }
            //if (!ValidateRequiredField($("#txtdobappointee"), 'Appointee DateofBirth required', 'after')) {
            //    flag = false;
            //}
            if (!ValidateRequiredField($("#appointeerelation"), 'Relationship to Account Holder required', 'after')) {
                flag = false;
            }
            if (!CheckOnlyText($("#appointeerelation"), 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtnomineerelation"), 'Relationship to Nominee required', 'after')) {
                flag = false;
            }
            if (!CheckOnlyText($("#txtnomineerelation"), 'after')) {
                flag = false;
            }
        }
        return flag;
    }

    $scope.ClearFormNominee = function () {
        $scope.Nominee = {};
        $("#txtName").val('');
        $("#txtbirth").val('');
        $("#txtnomineedob").val('');
        $("#txtaccountholderrelation").val('');
        $("#txtnamesurname").val('');
        $("#txtbirthplace").val('');
        $("#txtdobappointee").val('');
        $("#txtaccountholderrelation").val('');
        $("#txtnomineerelation").val('');
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $scope.Image = [];

    $scope.SignImage = [];

    //fuction which check holder photograph is valid and accept
    $scope.getHolderphoto = function ($files, personId, files) {

        var Increment = $scope.HolderPhoto.length + 1;
        $scope.HolderPhoto.push({ 'id': Increment, 'name': 'Holder' + Increment, Photo: { HolderPhotograph: '' } })

        angular.forEach($files, function (value, key) {
            if (value.type == "image/png" || value.type == "image/jpg" || value.type == "image/jpeg") {
                var sizeInMB = (value.size / (1024 * 1024)).toFixed(2);
                if (sizeInMB < 10) {

                    var obj = new Object();
                    obj.DocumentName = value.name;
                    obj.File = value;
                    obj.PersonId = personId;
                    //obj.DocumentId = value.DocumentId;
                    obj.IsDelete = false;

                    var docs = $filter('filter')($scope.Image, { PersonId: personId })[0];

                    var name = $filter('filter')($scope.Photographs, { PersonId: personId })[0];

                    if (name != undefined) {
                        angular.forEach($scope.Photographs, function (newval, index) {

                            if (newval.PersonId == personId) {
                                newval.NewDocument = value.name;
                                //value = obj;
                            }
                        });
                    }

                    if (docs != undefined) {
                        angular.forEach($scope.Image, function (value, index) {
                            //var index = index + 1;
                            if (value.PersonId == personId) {
                                $scope.Image[index] = obj
                                //value = obj;
                            }
                        });
                    }
                    else {
                        $scope.Image.push(obj);
                    }
                }
                else {
                    showToastMsg("Error", 3, "File size should be less than 10 MB.");
                }
            }
            else {
                showToastMsg(3, "Only Image format like .jpg .jpeg .png is Allowed");
            }
        });
    };

    //save holder photo when uplod button clicked
    $scope.SaveHolderphotograph = function () {

        //angular.forEach($scope.Photographs, function (value, index) {
        //    var index = index + 1;
        //    var docs = $filter('filter')($scope.Image, { PersonId: index })[0];
        //    value.Document = docs.DocumentName;
        //});
        if ($scope.Image.length > 0) {
            var imagedata = new FormData();
            if ($scope.Image.length > 0) {
                angular.forEach($scope.Image, function (value, index) {
                    imagedata.append("file", value.File);
                });
            }

            imagedata.append("data", JSON.stringify($scope.Photographs));
            // alert(JSON.stringify($scope.Photographs));
            var uploadphoto = AppService.UploadDocumentwithData("Customer", "UploadHolderPhotograph", imagedata)
            uploadphoto.then(function (p1) {
                if (p1.data != null) {
                    $scope.Photographs = [];
                    $scope.Photographs = p1.data;
                    showToastMsg(1, "Photograph Saved Successfully")
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                    //$scope.ClearForm();
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in saving data')
            });
        }
        else {
            showToastMsg(3, 'Please Upload photograph')
        }
    }

    //fuction which check holder sign is valid and accept
    $scope.getHolderSign = function ($files, personId, files) {

        var Increment = $scope.Holdersign.length + 1;
        $scope.Holdersign.push({ 'id': Increment, 'name': 'Holder' + Increment, Photo: { HolderSign: '' } })

        angular.forEach($files, function (value, key) {
            if (value.type == "image/png" || value.type == "image/jpg" || value.type == "image/jpeg") {
                var sizeInMB = (value.size / (1024 * 1024)).toFixed(2);
                if (sizeInMB < 10) {

                    var obj = new Object();
                    obj.DocumentName = value.name;
                    obj.File = value;
                    obj.PersonId = personId;
                    //obj.DocumentId = value.DocumentId;
                    obj.IsDelete = false;

                    var docs = $filter('filter')($scope.SignImage, { PersonId: personId, DocumentName: value.name })[0];

                    var name = $filter('filter')($scope.Signatures, { PersonId: personId })[0];

                    if (name != undefined) {
                        angular.forEach($scope.Signatures, function (newval, index) {

                            if (newval.PersonId == personId) {
                                newval.NewDocument = value.name;
                                //value = obj;
                            }
                        });
                    }
                    //var name = $filter('filter')($scope.Image, { DocumentName: name })[0];
                    if (docs != undefined) {
                        angular.forEach($scope.SignImage, function (value, index) {

                            if (value.PersonId == personId) {
                                $scope.SignImage[index] = obj
                                //value = obj;
                            }
                        });
                    }
                    else {
                        $scope.SignImage.push(obj);
                    }
                }
                else {
                    showToastMsg("Error", 3, "File size should be less than 10 MB.");
                }
            }
            else {
                showToastMsg(3, "Only Image format like .jpg .jpeg .png is Allowed");
            }
        });
    };

    //save holder sign when uplod button clicked
    $scope.SaveHolderSign = function () {

        if ($scope.SignImage.length > 0) {

            var imagedata = new FormData();
            if ($scope.SignImage.length > 0) {
                angular.forEach($scope.SignImage, function (value, index) {
                    imagedata.append("file", value.File);
                });
            }
            imagedata.append("data", JSON.stringify($scope.Signatures));

            var uploadsign = AppService.UploadDocumentwithData("Customer", "UploadHolderSign", imagedata)
            uploadsign.then(function (p1) {
                if (p1.data != null) {
                    $scope.Signatures = [];
                    $scope.Signatures = p1.data;
                    showToastMsg(1, "Signature Saved Successfully")
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                    //$scope.ClearForm();
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in saving data')
            });
        }
        else {
            showToastMsg(3, 'Please Upload Sign')
        }
    }

    function GetCustomerDocumentList() {

        var getdocument = AppService.GetDetailsById("Customer", "GetCustomerDocument", $scope.CustomerDetails.Customer.CustomerId)
        getdocument.then(function (p1) {
            if (p1.data != null) {
                $scope.CustomerDocument = p1.data;

            }
        })
    }


    $scope.SaveCustomerDocuments = function () {
        var DocumentName = $scope.DocumentName;
        var CustomerId = $scope.CustomerDetails.Customer.CustomerId;
        if (DocumentName != undefined && DocumentName != "") {
            if ($scope.Image.length > 0) {
                var imagedata = new FormData();
                if ($scope.Image.length > 0) {
                    angular.forEach($scope.Image, function (value, index) {
                        imagedata.append("file", value.File);
                    });
                }

                imagedata.append("data", JSON.stringify($scope.CustomersDocuments));
                imagedata.append("documentname", JSON.stringify($scope.DocumentName));
                imagedata.append("CustomerId", JSON.stringify($scope.CustomerDetails.Customer.CustomerId));
                // alert(JSON.stringify($scope.Photographs));
                var uploadphoto = AppService.UploadDocumentwithData("Customer", "UploadCustomerDocument", imagedata)
                uploadphoto.then(function (p1) {
                    if (p1.data != null) {
                        $scope.CustomerDocumentData = [];
                        $scope.CustomerDocumentData = p1.data;
                        $scope.DocumentName = '';
                        $('#customerdocuments').val('');

                        showToastMsg(1, "Documents Saved Successfully")
                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                        //$scope.ClearForm();
                    }
                    else {
                        showToastMsg(3, 'Error in saving data')
                    }
                }, function (err) {
                    showToastMsg(3, 'Error in saving data')
                });
            }
            else {
                showToastMsg(3, 'Please Upload any Documents')
            }
        }
        else {
            showToastMsg(3, 'Please Enter Document Name')

        }
    }

    $scope.getCustomerDocument = function ($files, personId, files) {

        var ext = $('#customerdocuments').val().split('.').pop().toLowerCase();
        var Increment = $scope.HolderPhoto.length + 1;
        $scope.CustomersDocuments.push({ 'id': Increment, 'name': 'Holder' + Increment, Photo: { HolderPhotograph: '' } })
        angular.forEach($files, function (value, key) {

            if ($.inArray(ext, ["txt", "doc", "docx", "pdf", "xls", "xlsx", "jpg", "jpeg", "png"]) != -1) {
                var sizeInMB = (value.size / (1024 * 1024)).toFixed(2);
                if (sizeInMB < 10) {

                    var obj = new Object();
                    obj.DocumentName = value.name;
                    obj.File = value;
                    obj.PersonId = personId;
                    //obj.DocumentId = value.DocumentId;
                    obj.IsDelete = false;

                    var docs = $filter('filter')($scope.Image, { PersonId: personId })[0];

                    var name = $filter('filter')($scope.Photographs, { PersonId: personId })[0];

                    if (name != undefined) {
                        angular.forEach($scope.Photographs, function (newval, index) {

                            if (newval.PersonId == personId) {
                                newval.NewDocument = value.name;
                                //value = obj;
                            }
                        });
                    }
                    if (docs != undefined) {
                        angular.forEach($scope.Image, function (value, index) {
                            //var index = index + 1;
                            if (value.PersonId == personId) {
                                $scope.Image[index] = obj
                                //value = obj;
                            }
                        });
                    }
                    else {
                        $scope.Image.push(obj);
                    }
                }
                else {
                    showToastMsg(3, "File size should be less than 10 MB.");
                }
            }
            else {
                $('#customerdocuments').val('');
                showToastMsg(3, 'Please upload only text,documents,image,pdf or excel files.')
            }
        });
    };
    $scope.btnDeleteCustDocument = function (DocumentId) {
        bootbox.dialog({
            message: "Are you sure want to delete this Document?",
            title: "Confirmation !",
            size: 'small',
            buttons: {
                success: {
                    label: "Yes",
                    className: "btn-success btn-flat",
                    callback: function () {
                        var DeleteCustDoc = AppService.GetDetailsById("Customer", "DeleteCustomerDocument", DocumentId);
                        DeleteCustDoc.then(function (P1) {
                            if (P1.data) {
                                showToastMsg(1, 'Document Deleted Successfully.')
                                $scope.CustomerDocumentData = [];
                                $scope.CustomerDocumentData = P1.data;
                            }
                            else {
                                showToastMsg(3, 'You can not delete this Document.')
                            }
                        }, function (err) {
                            showToastMsg(3, 'Error in getting data')
                        });

                    }
                },
                danger: {
                    label: "No",
                    className: "btn-danger btn-flat"
                }
            }
        });


    }


    $("#maturity").change(function () {
        var value = $(this).val();
        if (value == 1 || value == 3) {
            $("#DeductShareAmountDiv").show();
            $("#DeductAdmissionFeeDiv").show();
        }
        else {
            $("#DeductShareAmountDiv").hide();
            $("#DeductAdmissionFeeDiv").hide();
        }

        //if (value == "3") {
        //$scope.customershare.ShareAmount = 2500;
        //}
        //else {
        //$scope.customershare.ShareAmount = 100;
        //}
        //updateTotal();
    });


})

