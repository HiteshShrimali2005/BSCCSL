angular.module("BSCCL").controller('LoanController', function ($scope, AppService, $state, $cookies, $filter, $rootScope, $location, $timeout) {
    var getUserdata = new Object();
    getUserdata = $cookies.getObject('User');

    $scope.UserBranch.ShowBranch = false;
    $scope.UserBranch.Enabled = false;

    $(".datepicker").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    //$('#txtInstallmentDate').datetimepicker({
    //    minDate: new Date()
    //});

    $scope.Loan = {};
    $scope.LookUp = [];
    $scope.CustomerPersonal = new Object();
    $scope.CustomerPersonal.Personal = {};

    $scope.CustomerAddress = new Object();
    $scope.CustomerAddress.Address = {};

    $scope.Borrower = {};
    $scope.Referencer = {};
    $scope.LoanDocuments = [];
    $scope.EducationLoanPurpose = [];
    $scope.LoanCharges = [];
    $scope.LoanAmountisation = []
    $scope.UpdateStatus = {};
    $scope.BorrowerList = [];
    $scope.CurrentLoanDetails = []
    $scope.AccountHolderList = []

    $scope.VehicleLoan = {};
    $scope.BusinessLoan = {};
    $scope.GoldLoan = {};
    $scope.EducationLoan = {};
    $scope.MortgageLoan = {};
    $scope.EducationInfo = []

    $scope.JewelleryInfo = [{ name: "JewelleryInfo1", SrNo: '', Item: '', Type: '', ItemWeight: '', NetWeight: '', ItemPrice: '' }]

    $scope.MortgageItemInfo = [{ name: "MortgageItemInfo1", SrNo: '', Item: '', Type: '', ItemPrice: '' }]

    $scope.EducationInfo = [{ name: "EducationInfo1", ExamQualified: '', University_Institution: '', EducationMedium: '', Qualifiedtrial: '', MarksinFirstTrial: '', MarksPercentage: '', Class_Grade: '' }]

    $scope.CurrentLoanDetails = [{ name: "CurrentLoan1", BorrowerLoanId: '00000000-0000-0000-0000-000000000000', OrganizationName: '', LoanType: '00000000-0000-0000-0000-000000000000', LoanAmount: '', LoanLimit: '', InstallmentAmount: '', PaidInstallment: '' }]

    $scope.EconomicDetails = [];
    var Economic1 = { BusinessLoanId: "", EconomicDetails: "Financial Year", FY1: "", FY2: "", FY3: "" }
    var Economic2 = { BusinessLoanId: "", EconomicDetails: "Selling", FY1: "", FY2: "", FY3: "" }
    var Economic3 = { BusinessLoanId: "", EconomicDetails: "Operating Profit", FY1: "", FY2: "", FY3: "" }
    var Economic4 = { BusinessLoanId: "", EconomicDetails: "After Taxation", FY1: "", FY2: "", FY3: "" }
    var Economic5 = { BusinessLoanId: "", EconomicDetails: "Net Worth", FY1: "", FY2: "", FY3: "" }

    $scope.EconomicDetails.push(Economic1);
    $scope.EconomicDetails.push(Economic2);
    $scope.EconomicDetails.push(Economic3);
    $scope.EconomicDetails.push(Economic4);
    $scope.EconomicDetails.push(Economic5);

    GetAllLookup();

    $scope.addNewChoiceinJewellery = function () {

        var newItemNo = $scope.JewelleryInfo.length + 1;
        $scope.JewelleryInfo.push({ 'name': 'JewelryInfo1' + newItemNo, SrNo: newItemNo, Item: '', Type: '', ItemWeight: '', NetWeight: '', ItemPrice: '' });
    };

    $scope.removeNewChoiceinJewellery = function () {

        var newItemNo = $scope.JewelleryInfo.length - 1;
        if (newItemNo !== 0) {
            $scope.JewelleryInfo.pop();
        }
    };

    $scope.addNewChoiceinMortgageItem = function () {

        var newItemNo = $scope.MortgageItemInfo.length + 1;
        $scope.MortgageItemInfo.push({ name: "MortgageItemInfo1", SrNo: '', Item: '', Type: '', ItemPrice: '' });
    };

    $scope.removeNewChoiceinMortgageItem = function () {
        var newItemNo = $scope.MortgageItemInfo.length - 1;
        if (newItemNo !== 0) {
            if ($scope.MortgageItemInfo[newItemNo].MortgageItemInformationId != undefined) {
                AppService.DeleteData("Loan", "DeleteMorgageItem", $scope.MortgageItemInfo[newItemNo].MortgageItemInformationId).then(function (p1) {
                    if (p1.data) {
                        $scope.MortgageItemInfo.pop();
                    }
                })
            } else {
                $scope.MortgageItemInfo.pop();
            }
        }
    };

    $scope.addNewChoiceinEducation = function (lim) {
        var newItemNo = $scope.EducationInfo.length + 1;
        $scope.EducationInfo.push({ 'name': 'EducationInfo1' + newItemNo, ExamQualified: '', University_Institution: '', EducationMedium: '', Qualifiedtrial: '', MarksinFirstTrial: '', MarksPercentage: '', Class_Grade: '' });
    };

    $scope.removeNewChoiceinEducation = function (lim) {
        var newItemNo = $scope.EducationInfo.length - 1;
        if (newItemNo !== 0) {
            $scope.EducationInfo.pop();
        }
    };

    $scope.AddNewChoiceInCurrentLoanDetails = function () {
        var newItemNo = $scope.CurrentLoanDetails.length + 1;
        $scope.CurrentLoanDetails.push({ 'name': 'CurrentLoan1' + newItemNo, BorrowerLoanId: '00000000-0000-0000-0000-000000000000', OrganizationName: '', LoanType: '00000000-0000-0000-0000-000000000000', LoanAmount: '', LoanLimit: '', InstallmentAmount: '', PaidInstallment: '' });
    };

    $scope.$on('ngRepeatFinished', function (ngRepeatFinishedEvent) {
        $(".datepicker").datetimepicker({
            format: 'DD/MM/YYYY',
            useCurrent: false,
        });
    });

    if ($location.search().CustomerProductId != undefined) {
        $scope.CustomerProductId = $location.search().CustomerProductId;
        $scope.LoanId = $location.search().LoanId;
        GetCustomerPersonaldetail($location.search().CustomerProductId, $location.search().LoanId);
    }

    if ($location.search().LoanId != undefined) {
        $scope.LoanId = $location.search().LoanId;
    }

    if ($location.search().GroupLoanId != undefined) {
        $scope.GroupLoanId = $location.search().GroupLoanId;
    }

    GetcustomerProductId();

    function setdata(data) {

        $scope.Borrower = data.Borrower;
        $scope.CurrentLoanDetails = data.LoanDetails;
        if (data.LoanDetails.length == 0) {
            $scope.CurrentLoanDetails = [{ name: "CurrentLoan1", BorrowerLoanId: '00000000-0000-0000-0000-000000000000', OrganizationName: '', LoanType: '00000000-0000-0000-0000-000000000000', LoanAmount: '', LoanLimit: '', InstallmentAmount: '', PaidInstallment: '' }]
        }
        // $scope.Referencer = data;
        function CheckNullReturnBlank(item) {
            return item = (item == null) ? ' ' : item;
        }

        $scope.CustomerPersonal = data;
        $scope.CustomerPersonal.FullName = data.Personal.FirstName + " " + data.Personal.MiddleName + " " + data.Personal.LastName;
        $scope.CustomerPersonal.DOB = $filter('date')(data.Personal.DOB, 'dd/MM/yyyy');
        $scope.CustomerPersonal.Sex = data.Personal.Sex;
        $scope.CustomerPersonal.MotherName = data.Personal.MotherName;
        $scope.CustomerPersonal.Adharcard = data.Personal.Adharcard;
        $scope.CustomerPersonal.PanCard = data.Personal.PanCard;

        $scope.CustomerAddress.FullAddress = CheckNullReturnBlank(data.Address.DoorNo) + " " + CheckNullReturnBlank(data.Address.BuildingName) + " " + data.Address.PlotNo_Street + " " + data.Address.Landmark + " " + data.Address.Area;
        $scope.CustomerAddress.FullAddress2 = CheckNullReturnBlank(data.Address.Place) + " " + CheckNullReturnBlank(data.Address.City) + " " + data.Address.Pincode + " " + data.Address.District + " " + CheckNullReturnBlank(data.Address.State);
        $scope.CustomerAddress.Landmark = data.Address.Landmark;
        $scope.CustomerAddress.City = data.Address.City;
        $scope.CustomerAddress.State = data.Address.State;
        $scope.CustomerAddress.Pincode = data.Address.Pincode;
        $scope.CustomerAddress.TelephoneNo = data.Address.TelephoneNo;
        $scope.CustomerAddress.MobileNo = data.Address.MobileNo;
        $scope.CustomerAddress.Email = data.Address.Email;

        $scope.CustomerAddress.PerFullAddress = CheckNullReturnBlank(data.Address.PerDoorNo) + " " + CheckNullReturnBlank(data.Address.PerBuildingName) + " " + data.Address.PerPlotNo_Street + " " + data.Address.PerLandmark + " " + data.Address.PerArea;
        $scope.CustomerAddress.PerFullAddress2 = CheckNullReturnBlank(data.Address.PerPlace) + " " + CheckNullReturnBlank(data.Address.PerCity) + " " + data.Address.PerPincode + " " + data.Address.PerDistrict + " " + CheckNullReturnBlank(data.Address.PerState);
        $scope.CustomerAddress.PerLandmark = data.Address.PerLandmark;
        $scope.CustomerAddress.PerCity = data.Address.PerCity;
        $scope.CustomerAddress.PerState = data.Address.PerState;
        $scope.CustomerAddress.PerPincode = data.Address.PerPincode;
    }

    function GetAllLookup() {

        var getbranch = AppService.GetDetailsWithoutId("Loan", "GetAllLookup");

        getbranch.then(function (p1) {
            if (p1.data != null) {
                $scope.LookUp = p1.data;
                var Expense = $filter('filter')($scope.LookUp, { CategoryName: 'EducationExpense' });
                $scope.EducationLoanPurpose = [];
                angular.forEach(Expense, function (value, key) {
                    $scope.EducationLoanPurpose.push({ EducationLoanId: null, LookupId: value.LookupId, Name: value.Name, TutionFees: '', ExamFees: '', BookFees: '', Rent: '', Board: '', Clothe: '', Casual: '', InsurancePremium: '' });
                })
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    function GetCustomerPersonaldetail(CustomerProductId, LoanId) {

        $scope.AccountHolderList = []
        var getcustomerdetail = AppService.GetDataByQuerystring("Loan", "GetCustomerPersonalDetailById", CustomerProductId, LoanId);
        getcustomerdetail.then(function (p1) {
            if (p1.data != null) {
                $scope.AccountHolderList = p1.data.Details;
                $scope.Loan = p1.data.LoanDetail;
                $scope.CustomerList = p1.data.CustomerList;
                var newObj = new Object();
                newObj.LoanStatus = $scope.Loan.LoanStatus == 0 ? "" : $scope.Loan.LoanStatus.toString();
                //newObj.Comment = $scope.Loan.Comment;
                $scope.UpdateStatus = newObj;
                if ($scope.Loan.DisburseThrough == null || $scope.Loan.DisburseThrough == 0) {
                    $scope.Loan.DisburseThrough = '';
                }
                else {
                    $scope.Loan.DisburseThrough = $scope.Loan.DisburseThrough + '';
                }

                $scope.LoanDocuments = p1.data.LoanDocuments;
                $scope.LoanCharges = p1.data.LoanCharges;
                $scope.LoanStatusList = p1.data.LoanStatusList;


                if (p1.data.LoanDetail.InstallmentDate != null && p1.data.LoanDetail.InstallmentDate != "" && p1.data.LoanDetail.InstallmentDate != undefined) {
                    $("#txtInstallmentDate").data("DateTimePicker").date($filter('date')(p1.data.LoanDetail.InstallmentDate, 'dd/MM/yyyy'));
                }

                $("#txtdateofapplication").data("DateTimePicker").date($filter('date')(p1.data.LoanDetail.DateofApplication, 'dd/MM/yyyy'));

                if ($scope.Loan.LoanTypeName == "Education Loan") {
                    $scope.EducationLoan = p1.data.EducationLoan;
                    if ($scope.EducationLoan != null) {

                        $scope.EducationInfo = p1.data.EducationInfo;
                        $scope.EducationLoanPurpose = p1.data.EducationLoanPurpose;
                        angular.forEach($scope.EducationLoanPurpose, function (value, key) {
                            value.Name = $filter('filter')($scope.LookUp, { LookupId: value.LookupId })[0].Name;
                        })

                        // $("#txteduapplicationdate").data("DateTimePicker").date($filter('date')($scope.EducationLoan.DateofApplication, 'dd/MM/yyyy'));
                        $("#txtcoursestartdate").data("DateTimePicker").date($filter('date')($scope.EducationLoan.CourseStartingDate, 'dd/MM/yyyy'));
                        if ($scope.EducationLoan.CourseEndingDate != undefined && $scope.EducationLoan.CourseEndingDate != null) {
                            $("#txtcourseenddate").data("DateTimePicker").date($filter('date')($scope.EducationLoan.CourseEndingDate, 'dd/MM/yyyy'));
                        }
                    }
                }
                else if ($scope.Loan.LoanTypeName == "Vehicle Loan") {
                    $scope.VehicleLoan = p1.data.VehicleLoan;
                }
                else if ($scope.Loan.LoanTypeName == "Business Loan") {
                    $scope.BusinessLoan = p1.data.BusinessLoan;
                    if ($scope.BusinessLoan != null) {
                        $scope.EconomicDetails = p1.data.EconomicDetails;
                    }

                    if ($scope.BusinessLoan != null) {
                        $("#txtEstablishedDate").data("DateTimePicker").date($filter('date')(p1.data.BusinessLoan.EstablishDate, 'dd/MM/yyyy'));
                    }
                }
                else if ($scope.Loan.LoanTypeName == "Gold Loan") {
                    $scope.GoldLoan = p1.data.GoldLoan;
                    if ($scope.GoldLoan != null) {

                        $scope.JewelleryInfo = p1.data.JewelleryInfo;
                        $scope.TotalNetWeight();
                        //$("#txtgoldapplicationdate").data("DateTimePicker").date($filter('date')($scope.GoldLoan.DateofApplication, 'dd/MM/yyyy'));
                        if ($scope.GoldLoan.JewelleryDate != null && $scope.GoldLoan.JewelleryDate != undefined) {
                            $("#txtbuyjewellerydate").data("DateTimePicker").date($filter('date')($scope.GoldLoan.JewelleryDate, 'dd/MM/yyyy'));
                        }
                        $("#txtvaluationdate").data("DateTimePicker").date($filter('date')($scope.GoldLoan.ValuationDate, 'dd/MM/yyyy'));
                    }
                }
                else if ($scope.Loan.LoanTypeName == "Mortgage Loan") {
                    $scope.MortgageLoan = p1.data.MortgageLoan;
                    if ($scope.MortgageLoan != null) {
                        $scope.MortgageItemInfo = p1.data.MortgageItemInfo;
                        //$("#txtgoldapplicationdate").data("DateTimePicker").date($filter('date')($scope.GoldLoan.DateofApplication, 'dd/MM/yyyy'));
                        if ($scope.MortgageLoan.ItemDate != null && $scope.MortgageLoan.ItemDate != undefined) {
                            $("#txtBuyMortgageItemDate").data("DateTimePicker").date($filter('date')($scope.MortgageLoan.ItemDate, 'dd/MM/yyyy'));
                        }
                        $("#txtMortgageItemvaluationdate").data("DateTimePicker").date($filter('date')($scope.MortgageLoan.ValuationDate, 'dd/MM/yyyy'));
                    }
                }

                if ($scope.LoanCharges.length > 0) {
                    var chargedata = $filter('filter')($scope.LoanCharges, { Name: 'Share' })[0];
                    if (chargedata != undefined) {
                        $scope.NoOfShares = chargedata.NoOfItem;
                        $scope.TotalValue = chargedata.Value;
                    }
                }

                //GetChargesList();
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
                LoanAmountisation();
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.GetReferencerDetail = function (RefId) {

        var getReferencerdetail = AppService.GetDetailsById("Loan", "GetReferencerDetailById", RefId);
        getReferencerdetail.then(function (p1) {
            if (p1.data != null) {
                $scope.Referencer = p1.data;
                if (p1.data.DOB != undefined && p1.data.DOB != null) {
                    $("#txtrefDOB").val($filter('date')(p1.data.DOB, 'dd/MM/yyyy'));
                }
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.GetCustomerDetailsByPersonalId = function (Id1, Id2) {

        $scope.Bid = Id2;
        var getcustomerdetail = AppService.GetDataByQuerystring("Loan", "GetCustomerDetailByPersonalId", Id1, Id2);
        getcustomerdetail.then(function (p1) {
            if (p1.data != null) {
                $scope.CustomerData = p1.data;
                setdata(p1.data);
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }

        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    //$scope.GetRefrencerDetail = function (ReferenceId) {
    //    var getrefrencerdetail = AppService.GetDetailsById("Loan", "GetReferencerDetailById", ReferenceId);
    //    getrefrencerdetail.then(function (p1) {
    //        if (p1.data != null) {
    //            $scope.Referencer = p1.data;
    //        }
    //        else {
    //            showToastMsg(3, 'Error in getting data')
    //        }
    //    });
    //}

    $scope.SearchCustomer = function () {
        var flag = true;
        flag = ValidateSearchBorrowerType()
        if (flag) {
            var getcustomerdetail = AppService.GetDetailsById("Loan", "GetCustomerPersonalDetailByAccountId", $scope.SearchAccountNo);
            getcustomerdetail.then(function (p1) {
                if (p1.data != 0) {

                    if ($scope.AccountHolderList.length > 0) {
                        angular.forEach(p1.data, function (value, index) {
                            var obj = new Object();
                            obj.ClientId = value.ClientId;
                            obj.CustomerName = value.CustomerName;
                            obj.Sex = value.Sex;
                            obj.Referencertype = $scope.Referencer.Referencertype;
                            //obj.CustomerPersonalId = value.CustomerPersonalId;
                            obj.PersonalId = value.PersonalId;

                            var isnotExist = true;
                            for (var i = 0; i < $scope.AccountHolderList.length; i++) {
                                if ($scope.AccountHolderList[i].PersonalId === obj.PersonalId) {
                                    isnotExist = false;
                                }
                            }
                            if (isnotExist) {
                                $scope.AccountHolderList.push(obj);
                            }
                            else {
                                showToastMsg(3, 'Sorry !!! But borrower Already Exist');
                            }
                        });
                    }
                }
                else {
                    showToastMsg(3, 'Enter Saving Account Number')
                }
            }, function (err) {
                showToastMsg(3, 'Error in getting data')
            });
        }
        else {
            return false;
        }
    }

    function ValidateSearchBorrowerType() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#ddlborrowertype"), 'Select borrower Type', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtaccountnum"), 'Account Number required', 'after')) {
            flag = false;
        }

        return flag;
    }

    $scope.SaveCustomerLoanDetailsandNext = function (next) {
        $scope.SaveCustomerLoanDetails(next)
    }

    $scope.SaveCustomerLoanDetails = function (next) {
        $scope.IsDisbursement = false
        var flag = true;
        var LoanTypeName = $filter('filter')($scope.LookUp, { LookupId: $scope.Loan.LoanType })[0].Name;
        flag = ValidateCustomerLoan();

        var flag1 = true;
        if (LoanTypeName == "Education Loan") {
            flag1 = ValidateEducationLoanForm();
        }
        else if (LoanTypeName == "Vehicle Loan") {
            flag1 = ValidateVehicleLoanForm();
        }
        else if (LoanTypeName == "Business Loan") {
            flag1 = ValidateBussinessLoanForm();
        }
        else if (LoanTypeName == "Gold Loan") {
            flag1 = ValidateGoldLoanForm();
        }
        else if (LoanTypeName == "Mortgage Loan") {
            flag1 = ValidateMortgageLoanForm();
        }


        if (flag && flag1) {

            if ($("#txtInstallmentDate").val() != "") {

                var a = moment(new Date($("#txtInstallmentDate").data("DateTimePicker").date())).format('YYYY-MM-DD').match(/(\d+)/g);
                var b = moment(new Date()).format('YYYY-MM-DD').match(/(\d+)/g);;

                var firstDate = new Date(parseInt(a[0]), parseInt(a[1]) - 1, parseInt(a[2]))
                var secondDate = new Date(parseInt(b[0]), parseInt(b[1]) - 1, parseInt(b[2]))

                var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds

                c = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));

                if (c < 30 && !$scope.Loan.IsDisbursed && $scope.Loan.LoanStatus == 4 && (!$scope.InstallmentDateflag || $scope.InstallmentDateflag == undefined)) {
                    $scope.InstallmentDateflag = false
                    $("#txtInstallmentDate").closest('.form-group').removeClass('has-error');
                    $("#txtInstallmentDate").next('.help-block').remove();
                    $("#txtInstallmentDate").prev('.help-block').remove();
                    $("#txtInstallmentDate").closest('.form-group').addClass('has-warning');
                    $('<span class="help-block help-block-warning"> This duration is less than 30 days</span>').insertAfter($("#txtInstallmentDate"));
                }
                else {
                    $scope.InstallmentDateflag = true
                }
            }

            if (!$scope.InstallmentDateflag && $scope.Loan.LoanStatus == 4) {
                ValidateInstallmentDate();
            }
            else {
                $scope.InstallmentDateflag = true
            }

            if ($scope.InstallmentDateflag) {
                $scope.Loan.CustomerId = $scope.Loan.CustomerId;
                $scope.Loan.CustomerProductId = $scope.CustomerProductId;
                $scope.Loan.DateofApplication = moment(new Date($("#txtdateofapplication").data("DateTimePicker").date())).format('YYYY-MM-DD');

                if ($("#txtInstallmentDate").val() != "") {
                    $scope.Loan.InstallmentDate = moment(new Date($("#txtInstallmentDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
                }

                if ($scope.Loan.LoanId == '0000000-0000-0000-0000-000000000000' || $scope.Loan.LoanId != undefined || $scope.Loan.LoanId == "") {
                    $scope.Loan.CreatedBy = $cookies.getObject('User').UserId;
                }
                else {
                    $scope.Loan.ModifiedBy = $cookies.getObject('User').UserId;
                }

                var saveloan = AppService.SaveData("Loan", "SaveLoan", $scope.Loan)
                saveloan.then(function (p1) {
                    if (p1.data != null) {
                        $scope.InstallmentDateflag = false;
                        $scope.Loan.LoanId = p1.data;

                        GetChargesList();
                        LoanAmountisation()
                        $scope.BorrowerList = []

                        angular.forEach($scope.AccountHolderList, function (value, index) {
                            var obj = new Object();

                            obj.LoanId = $scope.Loan.LoanId;
                            obj.PersonalDetailId = value.PersonalId;
                            obj.Referencertype = $scope.AccountHolderList[index].Referencertype;
                            obj.BorrowerId = value.BorrowerId;
                            if (obj.BorrowerId == '0000000-0000-0000-0000-000000000000' || obj.BorrowerId == undefined) {
                                obj.CreatedBy = $cookies.getObject('User').UserId;
                                if (obj.Referencertype == 1 || obj.Referencertype == 3 || obj.Referencertype == 4) {
                                    $scope.BorrowerList.push(obj);
                                }
                            }
                            else {
                                obj.ModifiedBy = $cookies.getObject('User').UserId;
                            }

                            $scope.InstallmentDateflag = false;
                        });

                        Saveborrower($scope.BorrowerList)

                        if (LoanTypeName == "Education Loan") {
                            $scope.SaveEducationLoan();
                        }
                        else if (LoanTypeName == "Vehicle Loan") {
                            $scope.SaveVehicleLoan();
                        }
                        else if (LoanTypeName == "Business Loan") {
                            $scope.SaveBusinessLoan();
                        }
                        else if (LoanTypeName == "Gold Loan") {
                            $scope.SaveGoldLoan();
                        }
                        else if (LoanTypeName == "Mortgage Loan") {
                            $scope.SaveMortgageLoan();
                        }

                        GetCustomerPersonaldetail($scope.CustomerProductId, $scope.Loan.LoanId)
                    }
                    else {
                        showToastMsg(3, 'Error in Saving Data')
                    }
                });

            }
        }
        else {
            return false;
        }
    }

    function ValidateInstallmentDate() {

        bootbox.dialog({
            message: "You have selected less than 30 days Duration date(Installment Date). Are you sure want to continue?",
            title: "Confirmation !",
            size: 'small',
            buttons: {
                success: {
                    label: "Yes",
                    className: "btn-success btn-flat",
                    callback: function () {
                        $scope.InstallmentDateflag = true;
                        $scope.SaveCustomerLoanDetails();
                        $("#DisbursementAmount").modal('hide')
                    }
                },
                danger: {
                    label: "No",
                    className: "btn-danger btn-flat",
                    callback: function () {
                        $scope.InstallmentDateflag = false;
                    }

                }
            }
        });
    }

    $scope.ClearForm = function () {
        $scope.VehicleLoan = {};
        $scope.BusinessLoan = {};
        $scope.VehicleLoan = {};
        $scope.GoldLoan = {};
        $scope.MortgageLoan = {};
        $scope.Loan = {};
        $scope.Borrower = {};
        $scope.Referencer = {};
        $("#txtEstablishedDate").val('');
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }

    $scope.NextTab = function () {
        //$('#tabloan > .active').next('li').find('a').trigger('click');
        $scope.IsDisbursement = false
        var flag = true;
        flag = ValidateCustomerLoan();

        if (flag) {
            var LoanTypeName = $filter('filter')($scope.LookUp, { LookupId: $scope.Loan.LoanType })[0].Name;

            if (LoanTypeName == "Education Loan") {
                $("#btntab5").click()
            }
            else if (LoanTypeName == "Vehicle Loan") {
                $("#btntab2").click()
            }
            else if (LoanTypeName == "Business Loan") {
                $("#btntab3").click()
            }
            else if (LoanTypeName == "Gold Loan") {
                $("#btntab4").click();
            }
            else if (LoanTypeName == "Mortgage Loan") {
                $("#btntab20").click();
            }
        }
        else {
            return false;
        }
    }

    $scope.SaveLoanBorrower = function () {
        var flag = true;
        if (flag) {
            $scope.Borrower.PersonalDetailId = $scope.CustomerPersonal.Personal.PersonalDetailId;
            $scope.Borrower.LoanId = $scope.Loan.LoanId;
            $scope.Borrower.BorrowerId = $scope.Bid;
            $scope.Borrower.LoanDetails = $scope.CurrentLoanDetails;
            $scope.SubmitBorrower = [];
            $scope.SubmitBorrower.push($scope.Borrower);
            Saveborrower($scope.SubmitBorrower, "borrower");
        }
    }

    function Saveborrower(borroweList, string) {
        var saveborrower = AppService.SaveData("Loan", "SaveBorrower", borroweList)
        saveborrower.then(function (p1) {
            if (p1.data != null) {

                if (string == "borrower") {
                    GetCustomerPersonaldetail($scope.CustomerProductId, $scope.Loan.LoanId);
                }
                $("#Loan").modal('hide');
                showToastMsg(1, "Details Saved Successfully");
            }
            else {
                showToastMsg(3, 'Error in Saving Data')
            }
        });
    }

    $scope.RemoveChoiceInCurrentLoanDetails = function (data, index) {
        if (data.BorrowerLoanId != '00000000-0000-0000-0000-000000000000') {
            AppService.DeleteData("Loan", "DeleteBorrowerLoan", data.BorrowerLoanId).then(function (p1) {
                if (p1.data) {
                    $scope.CurrentLoanDetails.splice(index, 1);
                    if ($scope.CurrentLoanDetails.length == 0) {
                        $scope.CurrentLoanDetails = [{ name: "CurrentLoan1", BorrowerLoanId: '00000000-0000-0000-0000-000000000000', OrganizationName: '', LoanType: '00000000-0000-0000-0000-000000000000', LoanAmount: '', LoanLimit: '', InstallmentAmount: '', PaidInstallment: '' }]
                    }
                    showToastMsg(1, "Data Deleted Successfully");
                }
            })
        } else {
            $scope.CurrentLoanDetails.splice(index, 1);
            if ($scope.CurrentLoanDetails.length == 0) {
                $scope.CurrentLoanDetails = [{ name: "CurrentLoan1", BorrowerLoanId: '00000000-0000-0000-0000-000000000000', OrganizationName: '', LoanType: '00000000-0000-0000-0000-000000000000', LoanAmount: '', LoanLimit: '', InstallmentAmount: '', PaidInstallment: '' }]
            }
        }
    };

    $scope.SaveReferencer = function () {

        var flag = true;
        if (flag) {
            $scope.Referencer.CreatedBy = $cookies.getObject('User').UserId;
            $scope.Referencer.DOB = moment(new Date($("#txtrefDOB").data("DateTimePicker").date())).format('YYYY-MM-DD');

            if ($scope.Loan.LoanId === "00000000-0000-0000-0000-000000000000" || $scope.Loan.LoanId === undefined) {
                $scope.IsDisbursement = false
                var flag = true;
                flag = ValidateCustomerLoan();

                if (flag) {
                    $scope.Loan.CustomerId = $scope.Loan.CustomerId;
                    $scope.Loan.CustomerProductId = $scope.CustomerProductId;
                    $scope.Loan.DateofApplication = moment(new Date($("#txtdateofapplication").data("DateTimePicker").date())).format('YYYY-MM-DD');
                    if ($scope.Loan.LoanId == '0000000-0000-0000-0000-000000000000' || $scope.Loan.LoanId != undefined || $scope.Loan.LoanId == "") {
                        $scope.Loan.CreatedBy = $cookies.getObject('User').UserId;
                    }
                    else {
                        $scope.Loan.ModifiedBy = $cookies.getObject('User').UserId;
                    }
                    var saveloan = AppService.SaveData("Loan", "SaveLoan", $scope.Loan)
                    saveloan.then(function (p1) {
                        if (p1.data != null) {

                            $scope.Loan.LoanId = p1.data;
                            showToastMsg(1, "LoanApplication Saved Successfully");

                            $scope.BorrowerList = []

                            angular.forEach($scope.AccountHolderList, function (value, index) {
                                var obj = new Object();
                                obj.LoanId = $scope.Loan.LoanId;
                                obj.PersonalDetailId = value.PersonalId;
                                obj.Referencertype = $scope.AccountHolderList[index].Referencertype;
                                obj.BorrowerId = value.BorrowerId;
                                if (obj.BorrowerId == '0000000-0000-0000-0000-000000000000' || obj.BorrowerId == undefined) {
                                    obj.CreatedBy = $cookies.getObject('User').UserId;
                                }
                                else {
                                    obj.ModifiedBy = $cookies.getObject('User').UserId;
                                }


                                if (obj.Referencertype == 1 || obj.Referencertype == 3 || obj.Referencertype == 4) {
                                    $scope.BorrowerList.push(obj);
                                    //Saveborrower(obj);
                                }
                            });
                            Saveborrower($scope.BorrowerList)

                        }
                        else {
                            showToastMsg(3, 'Error in Saving Data')
                        }
                    });
                }
                else {
                    return false;
                }
            }

            setTimeout(function () {
                var flag = true;
                $scope.IsDisbursement = false
                flag = ValidateCustomerLoan();
                if (flag) {
                    $scope.Referencer.LoanId = $scope.Loan.LoanId;
                    $scope.Referencertype
                    if ($scope.Referencer.UserId == '0000000-0000-0000-0000-000000000000' && $scope.Referencer.UserId != undefined) {
                        $scope.Referencer.CreatedBy = $cookies.getObject('User').UserId;
                    }
                    else {
                        $scope.Referencer.ModifiedBy = $cookies.getObject('User').UserId;
                    }

                    var saveReferencer = AppService.SaveData("Loan", "SaveReferencer", $scope.Referencer)
                    saveReferencer.then(function (p1) {
                        if (p1.data != null) {
                            $scope.Referencer = p1.data;
                            $("#Referencer").modal('hide');
                            showToastMsg(1, "Details Saved Successfully")
                            GetCustomerPersonaldetail($scope.CustomerProductId, $scope.Loan.LoanId);

                        }
                        else {
                            showToastMsg(3, 'Error in Saving Data')
                        }
                    });
                }
                else {
                    $("#Referencer").modal('hide');
                    showToastMsg(3, "Please Enter Loan Details")
                }

            }, 300);
        }
    }

    $scope.ProductId = "";

    function GetcustomerProductId() {
        var getbranch = AppService.GetDetailsById("Loan", "CustomerProductId", $scope.CustomerProductId);
        getbranch.then(function (p1) {
            if (p1.data != null) {
                $scope.ProductId = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.RemoveReferencer = function (refId) {

        var deleteref = AppService.DeleteData("Loan", "DeleteRefById", refId);
        deleteref.then(function (p1) {
            if (p1.data != null) {
                GetCustomerPersonaldetail($scope.CustomerProductId, $scope.Loan.LoanId);
                showToastMsg(1, "Referencer Delete Successfully");
            }
            else {
                showToastMsg(3, "Error in Delete Data");
            }
        });
    }

    $scope.Removeborrow = function (borrowId) {

        var deleteborrow = AppService.DeleteData("Loan", "DeleteborrowById", borrowId);
        deleteborrow.then(function (p1) {
            if (p1.data != null) {
                GetCustomerPersonaldetail($scope.CustomerProductId, $scope.Loan.LoanId);
                showToastMsg(1, "Borrower Delete Successfully");
            }
            else {
                showToastMsg(3, "Error in Delete Data");
            }
        });
    }

    $scope.RemoveItem = function (hashKey, sourceArray) {
        angular.forEach(sourceArray, function (obj, index) {
            if (obj.$$hashKey === hashKey) {
                sourceArray.splice(index, 1);
                return;
            };
        });
    }

    function ValidateCustomerLoan() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtdateofapplication"), 'Date of Application required', 'after')) {
            $("#tab_1").click()
            flag = false;
        }
        if (!ValidateRequiredField($("#txtplace"), 'Place required', 'after')) {
            $("#tab_1").click()
            flag = false;
        }

        if (!ValidateRequiredField($("#txtamount"), 'Amount required', 'after')) {
            flag = false;
        }
        else if ($scope.Loan.LoanAmount == "" || $scope.Loan.LoanAmount == 0 || $scope.Loan.LoanAmount == "0") {
            $("#txtamount").closest('.form-group').removeClass('has-error');
            $("#txtamount").next('.help-block').remove();
            $("#txtamount").prev('.help-block').remove();
            $("#txtamount").closest('.form-group').addClass('has-error');
            $('<span class="help-block help-block-error"> Loan Amount Required </span>').insertAfter($("#txtamount"));
        }

        var LoanTypeName = $filter('filter')($scope.LookUp, { LookupId: $scope.Loan.LoanType })[0].Name;
        if (LoanTypeName != "Flexi Loan") {
            if (!ValidateRequiredField($("#txtterm"), 'Loan Term required', 'after')) {
                flag = false;
            }
            else if ($scope.Loan.Term == "" || $scope.Loan.Term == 0 || $scope.Loan.Term == "0") {
                $("#txtterm").closest('.form-group').removeClass('has-error');
                $("#txtterm").next('.help-block').remove();
                $("#txtterm").prev('.help-block').remove();
                $("#txtterm").closest('.form-group').addClass('has-error');
                $('<span class="help-block help-block-error"> Loan Term required</span>').insertAfter($("#txtterm"));
            }
        }
        //    if (!ValidateRequiredField($("#txtterm"), 'Loan Term required', 'after')) {
        //        flag = false;
        //    }
        //    else if ($scope.Loan.Term == "" || $scope.Loan.Term == 0 || $scope.Loan.Term == "0") {
        //        $("#txtterm").closest('.form-group').removeClass('has-error');
        //        $("#txtterm").next('.help-block').remove();
        //        $("#txtterm").prev('.help-block').remove();
        //        $("#txtterm").closest('.form-group').addClass('has-error');
        //        $('<span class="help-block help-block-error"> Loan Term required</span>').insertAfter($("#txtterm"));
        //}

        if ($scope.Loan.LoanStatus == 4) {
            if (!ValidateRequiredField($("#txtDisbursementAmt"), 'Amount required', 'after')) {
                flag = false;
            }
            else if ($scope.Loan.DisbursementAmount == "" || $scope.Loan.DisbursementAmount == 0 || $scope.Loan.DisbursementAmount == "0") {
                $("#txtterm").closest('.form-group').removeClass('has-error');
                $("#txtterm").next('.help-block').remove();
                $("#txtterm").prev('.help-block').remove();
                $("#txtterm").closest('.form-group').addClass('has-error');
                $('<span class="help-block help-block-error"> Loan Term required</span>').insertAfter($("#txtterm"));
            }


            if ($("#txtInstallmentDate").val() != "" && $("#txtInstallmentDate").val() != undefined && $("#txtInstallmentDate").val() != null) {
                var EMI = moment(new Date($("#txtInstallmentDate").data("DateTimePicker").date())).format('YYYY-MM-DD')
                var today = moment(new Date()).format('YYYY-MM-DD')

                if (EMI < today) {
                    flag = false;
                    $("#txtInstallmentDate").closest('.form-group').removeClass('has-warning');
                    $("#txtInstallmentDate").closest('.form-group').removeClass('has-error');
                    $("#txtInstallmentDate").next('.help-block').remove();
                    $("#txtInstallmentDate").prev('.help-block').remove();
                    $("#txtInstallmentDate").closest('.form-group').addClass('has-error');
                    $('<span class="help-block help-block-error">  Installment date should be future date</span>').insertAfter($("#txtInstallmentDate"));
                }
            }
            else if ($scope.Loan.LoanStatus == 4) {
                if (!ValidateRequiredField($("#txtInstallmentDate"), 'InstallmentDate required', 'after')) {
                    flag = false;
                }
            }


            if ($scope.NoOfShares != undefined && $scope.Loan.LoanStatus == 4 && $scope.IsDisbursement) {
                if (!ValidateRequiredField($("#txtShareCertificateNo"), 'Certificate No. required', 'after')) {
                    flag = false;
                }

                $("#txtInstallmentDate").closest('.form-group').removeClass('has-error');
                $("#txtInstallmentDate").next('.help-block').remove();
                $("#txtInstallmentDate").prev('.help-block').remove();

                if (!ValidateRequiredField($("#txtInstallmentDate"), 'InstallmentDate required', 'after')) {
                    flag = false;
                }
                else {

                    var a = moment(new Date($("#txtInstallmentDate").data("DateTimePicker").date())).format('YYYY-MM-DD').match(/(\d+)/g);
                    var b = moment(new Date()).format('YYYY-MM-DD').match(/(\d+)/g);;

                    var firstDate = new Date(parseInt(a[0]), parseInt(a[1]) - 1, parseInt(a[2]))
                    var secondDate = new Date(parseInt(b[0]), parseInt(b[1]) - 1, parseInt(b[2]))

                    var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds

                    c = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));

                    if (c < 30 && $scope.Loan.LoanStatus == 4 && ($scope.InstallmentDateflag == false || $scope.InstallmentDateflag == undefined)) {
                        $("#btntab1").click();
                        $("#txtInstallmentDate").closest('.form-group').removeClass('has-warning');
                        $("#txtInstallmentDate").closest('.form-group').removeClass('has-error');
                        $("#txtInstallmentDate").next('.help-block').remove();
                        $("#txtInstallmentDate").prev('.help-block').remove();
                        $("#txtInstallmentDate").closest('.form-group').addClass('has-warning');
                        $('<span class="help-block help-block-warning"> You have selected less than 30 days installment date</span>').insertAfter($("#txtInstallmentDate"));
                    }
                }
            }

            if ($scope.NoOfShares != undefined && $scope.Loan.LoanStatus == 4 && $scope.IsDisbursement && $("#txtShareCertificateNo").val() == "") {
                $("#btntab18").click();
            } else if (flag == false) {
                $("#btntab1").click();
            }
        }

        return flag;
    }

    function validateborrowerdetail() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#txtannualincome"), 'Annual Income required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtfamilymember"), 'Family Member required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtcountrycode"), 'Family Member required', 'after')) {
            flag = false;
        }
        return flag;
    }
    //Save Vehicle Loan
    $scope.SaveVehicleLoan = function () {
        var flag = true;
        flag = ValidateVehicleLoanForm();
        if (flag) {
            $scope.VehicleLoan.LoanId = $scope.Loan.LoanId;
            $scope.VehicleLoan.CustomerId = $scope.Loan.CustomerId;
            if ($scope.VehicleLoan.VehicleLoanId == '00000000-0000-0000-0000-000000000000' || $scope.VehicleLoan.VehicleLoanId != undefined || $scope.VehicleLoan.VehicleLoanId != '') {
                $scope.VehicleLoan.CreatedBy = getUserdata.UserId;
            }
            else {
                $scope.VehicleLoan.ModifiedBy = getUserdata.UserId;
            }
            //$scope.VehicleLoan.ApplicationDate = moment(new Date($("#txtApplicationDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            var SaveVehicleLoanData = AppService.SaveData("Loan", "SaveVehicleLoanData", $scope.VehicleLoan)
            SaveVehicleLoanData.then(function (P1) {
                if (P1.data != null) {
                    $scope.VehicleLoan = P1.data;
                    showToastMsg(1, "VehicleLoan Saved Successfully");
                }
                else {
                    showToastMsg(3, 'Error in Saving data')
                }
            })
        }
        else {
            return false;
        }
    }

    function ValidateVehicleLoanForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        //if (!ValidateRequiredField($("#txtApplicationDate"), 'Application Date required', 'after')) {
        //    flag = false;
        //}
        //if (!ValidateRequiredField($("#txtApplicationNumber"), 'Application Number required', 'after')) {
        //    flag = false;
        //}
        if (!ValidateRequiredField($("#txtProducerName"), 'Producer Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtAccetMaquarian"), 'Asset Macquarient required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtDealerName"), ' Dealer Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtDealerCode"), 'Dealer Code Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtAssetLife"), 'Asset Life Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtX-showroomprice"), ' X-showroomprice Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtOnroadPrice"), 'Onroad Price Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtInsuuarnce"), 'Insurance Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtCategory"), 'Category Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtModel"), 'Model Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtOtherTax"), 'Other Tax Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtRegistrationCost"), 'Registration Cost Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtOnroadPrice"), 'Onroad Price Required', 'after')) {
            flag = false;
        }
        return flag;
    }

    $scope.ClearVehicleLoanForm = function () {
        $scope.VehicleLoan = {}
    }

    //BusinessLoan 
    $scope.SaveBusinessLoan = function () {
        var flag = true;
        flag = ValidateBussinessLoanForm();
        if (flag) {

            $scope.BusinessLoan.LoanId = $scope.Loan.LoanId;
            $scope.BusinessLoan.CustomerId = $scope.Loan.CustomerId;

            if ($scope.BusinessLoan.BusinessLoanId == '00000000-0000-0000-0000-000000000000' || $scope.BusinessLoan.BusinessLoanId != undefined || $scope.BusinessLoan.BusinessLoanId != '') {
                $scope.BusinessLoan.CreatedBy = getUserdata.UserId;
            }
            else {
                $scope.BusinessLoan.ModifiedBy = getUserdata.UserId;
            }

            $scope.BusinessLoan.EstablishDate = moment(new Date($("#txtEstablishedDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            var SaveBusinessLoan = AppService.SaveData("Loan", "SaveBusinessLoanData", $scope.BusinessLoan);
            SaveBusinessLoan.then(function (p1) {
                if (p1.data != null) {
                    //clearBusinessLoanForm();
                    $scope.BusinessLoan = p1.data;

                    angular.forEach($scope.EconomicDetails, function (value, key) {
                        value.BusinessLoanId = $scope.BusinessLoan.BusinessLoanId;
                        if (value.BusinessEconomicDetailsId == '00000000-0000-0000-0000-000000000000' || value.BusinessEconomicDetailsId != undefined || value.BusinessEconomicDetailsId != '') {
                            value.CreatedBy = getUserdata.UserId;
                        }
                        else {
                            value.ModifiedBy = getUserdata.UserId;
                        }
                    })

                    var saveEconomicDetails = AppService.SaveData("Loan", "SaveEconomicDetails", $scope.EconomicDetails);
                    saveEconomicDetails.then(function (p1) {
                        if (p1.data != null) {
                            $scope.EconomicDetails = p1.data;
                            showToastMsg(1, 'Data Saved Successfully')
                        }
                        else {
                            showToastMsg(3, 'Error in Saving data')
                        }
                    }, function (err) {
                        showToastMsg(3, 'Error in Saving data')
                    });
                }
                else {
                    showToastMsg(3, 'Error in Saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving data')
            });
        }
        else {
            showToastMsg(3, 'Please Fill Data Properly')
        }
    }

    function ValidateBussinessLoanForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;

        if (!ValidateRequiredField($("#txtPrimaryPostOfficeAddress"), 'Application Date required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtNumberOfYearsOfCurrentAddress"), 'Number of years opf current address required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtNumberOfYearsOfCurrentCity"), 'No of yeras in current city required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtcity"), 'City required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtState"), 'State required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtPincode"), 'Pincode required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtContry"), 'Country required', 'after')) {
            flag = false;
        }
        //if (!ValidateRequiredField($("#txtCountryCode"), 'Country Code required', 'after')) {
        //    flag = false;
        //}
        //if (!ValidateRequiredField($("#txtEmail"), 'Email required', 'after')) {
        //    flag = false;
        //}
        if (!ValidateRequiredField($("#txtMobileNo"), 'Mobile number required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtNumberOfYearsOfCurrentOrganization"), 'Number of yeras of organization required', 'after')) {
            flag = false;

        }
        if (!ValidateRequiredField($("#NumberOfYearsOfExperience"), 'Number of years of experience required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtCompanyName"), 'Company Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtPosition"), 'Position required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtOfficeAccomodationAddress1"), 'Address required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtOfficeLandMark"), 'LandMark required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtOfficeCity"), 'City required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtOfficeState"), 'State required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtOfficePincode"), 'Pincode required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtOfficeCountry"), 'Country required', 'after')) {
            flag = false;
        }
        //if (!ValidateRequiredField($("#txtOfficeCountryCode"), 'Country Code required', 'after')) {
        //    flag = false;
        //}
        //if (!ValidateRequiredField($("#txtOfficeSTDCode"), 'STD Code required', 'after')) {
        //    flag = false;
        //}
        //if (!ValidateRequiredField($("#txtOfficePhoneNo"), 'PhoneNumber required', 'after')) {
        //    flag = false;
        //}
        //if (!ValidateRequiredField($("#txtOfficeAccExtension"), 'AccesExtension required', 'after')) {
        //    flag = false;
        //}
        //if (!ValidateRequiredField($("#txtOfficeEmail"), 'Email required', 'after')) {
        //    flag = false;
        //}
        if (!ValidateRequiredField($("#txtOfficeMobileNo"), 'Mobile Number required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtLandMark"), 'LandMark required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtAccomodationAddress1"), 'Address required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtNumberOfYearsOfCurrentAddress"), 'Number Ofyears Of current Address required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtNumberOfYearsOfCurrentCity"), 'Number Of years Of current City Years required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtPrimaryPostOfficeAddress"), ' Primary Address required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtNumberOfYearsInBusiness"), 'Number Of Years of business Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtCustomerName"), 'Customer Name Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtOfficePrimaryPostOfficeAddress"), 'Primary address Required', 'after')) {
            flag = false;
        }
        //if (!ValidateRequiredField($("#txtAccetension"), 'Extension Required', 'after')) {
        //    flag = false;
        //}
        //if (!ValidateRequiredField($("#txtPhoneNo"), 'PhoneNo Required', 'after')) {
        //    flag = false;
        //}
        //if (!ValidateRequiredField($("#txtSTDCode"), 'STD Code Required', 'after')) {
        //    flag = false;
        //}
        if (!ValidateRequiredField($("#txtPANOrGRINumber"), 'PAN Number Required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtEstablishedDate"), 'EstablishDate Required', 'after')) {
            flag = false;
        }
        return flag;
    }

    $scope.ClearBusinessLoan = function () {
        $scope.BusinessLoan = {}
        $scope.EconomicDetails = [];
        var Economic1 = { BusinessLoanId: "", EconomicDetails: "Financial Year", FY1: "", FY2: "", FY3: "" }
        var Economic2 = { BusinessLoanId: "", EconomicDetails: "Selling", FY1: "", FY2: "", FY3: "" }
        var Economic3 = { BusinessLoanId: "", EconomicDetails: "Operating Profit", FY1: "", FY2: "", FY3: "" }
        var Economic4 = { BusinessLoanId: "", EconomicDetails: "After Taxation", FY1: "", FY2: "", FY3: "" }
        var Economic5 = { BusinessLoanId: "", EconomicDetails: "Net Worth", FY1: "", FY2: "", FY3: "" }

        $scope.EconomicDetails.push(Economic1);
        $scope.EconomicDetails.push(Economic2);
        $scope.EconomicDetails.push(Economic3);
        $scope.EconomicDetails.push(Economic4);
        $scope.EconomicDetails.push(Economic5);
    }

    //Gold Loan
    $scope.SaveGoldLoan = function () {
        var flag = true;
        flag = ValidateGoldLoanForm();
        if (flag) {

            $scope.GoldLoan.LoanId = $scope.Loan.LoanId;
            //$scope.GoldLoan.CustomerId = $scope.Loan.CustomerId;

            if ($scope.GoldLoan.GoldLoanId == '00000000-0000-0000-0000-000000000000' || $scope.GoldLoan.GoldLoanId != undefined || $scope.GoldLoan.GoldLoanId != '') {
                $scope.GoldLoan.CreatedBy = getUserdata.UserId;
            }
            else {
                $scope.GoldLoan.ModifiedBy = getUserdata.UserId;
            }

            // $scope.GoldLoan.DateofApplication = moment(new Date($("#txtgoldapplicationdate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            $scope.GoldLoan.JewelleryDate = moment(new Date($("#txtbuyjewellerydate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            $scope.GoldLoan.ValuationDate = moment(new Date($("#txtvaluationdate").data("DateTimePicker").date())).format('YYYY-MM-DD');

            var savegoldloan = AppService.SaveData("Loan", "SaveGoldLoan", $scope.GoldLoan);
            savegoldloan.then(function (p1) {
                if (p1.data != null) {
                    $scope.GoldLoan = p1.data;

                    angular.forEach($scope.JewelleryInfo, function (value, index) {
                        value.GoldLoanId = $scope.GoldLoan.GoldLoanId
                    });

                    var saveJewelleryInfo = AppService.SaveData("Loan", "SaveJewelleryInformation", $scope.JewelleryInfo);
                    saveJewelleryInfo.then(function (p2) {
                        $scope.JewelleryInfo = [];
                        $scope.JewelleryInfo = p2.data;
                        showToastMsg(1, 'Data Saved Successfully')
                    });
                    //clearBusinessLoanForm();
                }
                else {
                    showToastMsg(3, 'Error in Saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving data')
            });
        }
        else {
            showToastMsg(3, 'Please Fill Data Properly')
        }
    }

    function ValidateGoldLoanForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;

        if (!ValidateRequiredField($("#txtgoldloantype"), 'GoldLoan Type required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtgoldtype"), 'Gold Type required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtbuyjewellerydate"), 'Jewellery Date required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtjewelleryprice"), 'Jewellery Price required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtvaluationdate"), 'Valuation Date required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtvaluationprice"), 'Valuation Price required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtloanprice"), 'Total Loan Price required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtInterestrate"), 'Interest Rate required', 'after')) {
            flag = false;
        }


        angular.forEach($scope.JewelleryInfo, function (value, index) {


            if (!ValidateRequiredField($("#txtItem" + index), 'Item Name required', 'after')) {
                flag = false;
            }

            if (!ValidateRequiredField($("#txttype" + index), 'Type required', 'after')) {
                flag = false;
            }

            if (!ValidateRequiredField($("#txtItemWeight" + index), 'Item Weight required', 'after')) {
                flag = false;
            }

            if (!ValidateRequiredField($("#txtNetWeight" + index), 'Item Weight required', 'after')) {
                flag = false;
            }

            if (!ValidateRequiredField($("#txtItemPrice" + index), 'Price required', 'after')) {
                flag = false;
            }

        })

        return flag;
    }

    $scope.ClearGoldLoan = function () {
        $scope.GoldLoan = {}
        $scope.JewelleryInfo = [{ name: "JewelleryInfo1", SrNo: '', Item: '', Type: '', ItemWeight: '', NetWeight: '', ItemPrice: '' }]
    }

    $scope.SaveMortgageLoan = function () {
        var flag = true;
        flag = ValidateMortgageLoanForm();
        if (flag) {

            $scope.MortgageLoan.LoanId = $scope.Loan.LoanId;
            if ($scope.MortgageLoan.MortgageLoanId == '00000000-0000-0000-0000-000000000000' || $scope.MortgageLoan.MortgageLoanId != undefined || $scope.MortgageLoan.MortgageLoanId != '') {
                $scope.MortgageLoan.CreatedBy = getUserdata.UserId;
            }
            else {
                $scope.MortgageLoan.ModifiedBy = getUserdata.UserId;
            }
            $scope.MortgageLoan.ItemDate = moment(new Date($("#txtBuyMortgageItemDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            $scope.MortgageLoan.ValuationDate = moment(new Date($("#txtMortgageItemvaluationdate").data("DateTimePicker").date())).format('YYYY-MM-DD');

            var savemortgageloan = AppService.SaveData("Loan", "SaveMortgageLoan", $scope.MortgageLoan);
            savemortgageloan.then(function (p1) {
                if (p1.data != null) {
                    $scope.MortgageLoan = p1.data;

                    angular.forEach($scope.MortgageItemInfo, function (value, index) {
                        value.MortgageLoanId = $scope.MortgageLoan.MortgageLoanId;
                    });

                    var saveMortgageItemInfo = AppService.SaveData("Loan", "SaveMortgageItemInfo", $scope.MortgageItemInfo);
                    saveMortgageItemInfo.then(function (p2) {
                        $scope.MortgageItemInfo = [];
                        $scope.MortgageItemInfo = p2.data;
                        showToastMsg(1, 'Data Saved Successfully')
                    });
                    //clearBusinessLoanForm();
                }
                else {
                    showToastMsg(3, 'Error in Saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving data')
            });
        }
        else {
            showToastMsg(3, 'Please Fill Data Properly')
        }
    }

    function ValidateMortgageLoanForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;

        if (!ValidateRequiredField($("#txtBuyMortgageItemDate"), 'Item Date required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtMortgageItemPrice"), 'Item Price required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtMortgageItemvaluationdate"), 'Valuation Date required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtMortgageItemvaluationprice"), 'Valuation Price required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtMortgageloanprice"), 'Total Loan Price required', 'after')) {
            flag = false;
        }

        angular.forEach($scope.MortgageItemInfo, function (value, index) {
            if (!ValidateRequiredField($("#txtMItemItem" + index), 'Item Name required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtMItemtype" + index), 'Type required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtMItemPrice" + index), 'Price required', 'after')) {
                flag = false;
            }
        })

        return flag;
    }

    $scope.ClearMortgageLoan = function () {
        $scope.MortgageLoan = {}
        $scope.MortgageItemInfo = [{ name: "MortgageItemInfo1", SrNo: '', Item: '', Type: '', ItemPrice: '' }]
    }

    $scope.SaveEducationLoan = function () {
        var flag = true;
        falg = ValidateEducationLoanForm();
        if (flag) {

            $scope.EducationLoan.LoanId = $scope.Loan.LoanId;

            if ($scope.EducationLoan.EducationLoanId == '00000000-0000-0000-0000-000000000000' || $scope.EducationLoan.EducationLoanId != undefined || $scope.EducationLoan.EducationLoanId != '') {
                $scope.EducationLoan.CreatedBy = getUserdata.UserId;
            }
            else {
                $scope.EducationLoan.ModifiedBy = getUserdata.UserId;
            }

            //$scope.EducationLoan.DateofApplication = moment(new Date($("#txteduapplicationdate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            $scope.EducationLoan.CourseStartingDate = moment(new Date($("#txtcoursestartdate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            if ($("#txtcourseenddate").val() != "" && $("#txtcourseenddate").val() != null && $("#txtcourseenddate").val() != undefined) {
                $scope.EducationLoan.CourseEndingDate = moment(new Date($("#txtcourseenddate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }

            var saveEducationloan = AppService.SaveData("Loan", "SaveEducationLoan", $scope.EducationLoan);

            saveEducationloan.then(function (p1) {
                if (p1.data != null) {
                    $scope.EducationLoan = p1.data;

                    angular.forEach($scope.EducationInfo, function (value, index) {
                        value.EducationLoanId = $scope.EducationLoan.EducationLoanId;
                    });
                    angular.forEach($scope.EducationLoanPurpose, function (value, index) {
                        value.EducationLoanId = $scope.EducationLoan.EducationLoanId;
                    });

                    var NewObj = new Object();
                    NewObj.EducationInfo = $scope.EducationInfo;
                    NewObj.EducationLoanPurpose = $scope.EducationLoanPurpose;

                    var saveEducationInfo = AppService.SaveData("Loan", "SaveEducationInfoDetails", NewObj);
                    saveEducationloan.then(function (p2) {
                        $scope.EducationInfo = [];
                        $scope.EducationLoanPurpose = [];
                        $scope.EducationInfo = p2.data.EducationInfo;
                        $scope.EducationLoanPurpose = p2.data.EducationLoanPurpose;

                        showToastMsg(1, 'Data Saved Successfully')
                        if (!$scope.$$phase) {
                            $scope.$apply();
                        }
                    });
                    //clearBusinessLoanForm();
                }
                else {
                    showToastMsg(3, 'Error in Saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving data')
            });
        }
        else {
            showToastMsg(3, 'Please Fill Data Properly')
        }
    }

    function ValidateEducationLoanForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;

        if (!ValidateRequiredField($("#txtcoursename"), 'Course name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtttlcourseyear"), 'Course year required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtcoursestartdate"), 'Course Start Date required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtNameofCollege"), 'Name Of University required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtedupincode"), 'Pincode required', 'after')) {
            flag = false;
        }

        return flag;
    }

    $scope.ClearEducationLoan = function () {
        $scope.EducationLoan = {}
        $scope.EducationInfo = [{ name: "EducationInfo1", ExamQualified: '', University_Institution: '', EducationMedium: '', Qualifiedtrial: '', MarksinFirstTrial: '', MarksPercentage: '', Class_Grade: '' }]
        var Expense = $filter('filter')($scope.LookUp, { CategoryName: 'EducationExpense' });
        $scope.EducationLoanPurpose = [];
        angular.forEach(Expense, function (value, key) {
            $scope.EducationLoanPurpose.push({ EducationLoanId: null, LookupId: value.LookupId, Name: value.Name, TutionFees: '', ExamFees: '', BookFees: '', Rent: '', Board: '', Clothe: '', Casual: '', InsurancePremium: '' });
        })
    }

    $scope.TotalNetWeight = function () {
        $scope.GoldLoan.TotalWeight = 0;
        angular.forEach($scope.JewelleryInfo, function (value, index) {
            if (value.NetWeight != "" && value.NetWeight != 0 && value.NetWeight != "0") {
                $scope.GoldLoan.TotalWeight += parseFloat(value.NetWeight);
            }
        })
    }

    $scope.UpdateLoanStatus = function () {
        if ($scope.UpdateStatus.LoanStatus != 7) {
            var flag = true;
            $scope.IsDisbursement = false;
            flag = ValidateCustomerLoan();
            if (flag) {
                $scope.SaveLoanCharges("Status");
            }
            else {

            }
        }
    }

    $scope.Files = [];

    //Document Upload Function
    $scope.getTheFiles = function ($files) {

        angular.forEach($files, function (value, key) {
            var sizeInMB = (value.size / (1024 * 1024)).toFixed(2);
            if (sizeInMB < 10) {
                var obj = new Object();
                obj.DocumentName = value.name;
                obj.File = value;
                obj.LoanId = $scope.Loan.LoanId;
                obj.IsDelete = false;
                $scope.Files.push(obj);
            }
            else {
                showToastMsg("Error", 3, "File size should be less than 10 MB.");
                return false;
            }
        });
    };

    $scope.UploadDocuments = function () {
        if ($scope.Files.length > 0) {
            var fd = new FormData();
            angular.forEach($scope.Files, function (value, index) {
                fd.append("file", value.File);
            });
            fd.append("data", JSON.stringify($scope.Files));

            var uploadDocs = AppService.UploadDocumentwithData("Loan", "UploadLoanDocuments", fd)
            uploadDocs.then(function (p1) {
                if (p1.data != null) {
                    showToastMsg(1, "Document Uploaded Successfully")
                    $("#loanDocs").val("");
                    $scope.Files = [];
                    GetLoanDocumentList();
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving Data')
            });
        } else {
            showToastMsg(3, 'Please select document for upload.')
        }
    }

    $scope.DeleteDocuments = function (loanDocumentId) {
        var deletedoc = AppService.DeleteData("Loan", "DeleteDocuments", loanDocumentId)
        deletedoc.then(function (p1) {
            if (p1.data != null) {
                GetLoanDocumentList();
            }
            else {
                showToastMsg(3, 'Error in saving data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in Saving Data')
        });
    }

    function GetLoanDocumentList() {
        var getdoc = AppService.GetDetailsById("Loan", "GetLoanDocumentList", $scope.Loan.LoanId)
        getdoc.then(function (p1) {
            if (p1.data != null) {
                $scope.LoanDocuments = p1.data;
            }
            else {
                showToastMsg(3, 'Error in saving data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in Saving Data')
        });
    }

    function GetChargesList() {
        var getcharges = AppService.GetDetailsById("Loan", "GetChargesList", $scope.Loan.LoanId)
        getcharges.then(function (p1) {
            if (p1.data != null) {
                $scope.LoanCharges = p1.data;
            }
            else {
                showToastMsg(3, 'Error in saving data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in Saving Data')
        });
    }

    $scope.ChangeShareValue = function (value) {
        $scope.TotalValue = 0;
        $scope.TotalValue = value * 100;
        if (!$scope.$$phase) {
            $scope.$apply();
        }
    }

    $scope.SaveLoanCharges = function (string) {
        $(':focus').blur();
        if ($scope.NoOfShares != undefined) {
            var sharedata = $filter('filter')($scope.LoanCharges, { Name: 'Share' })[0];
            if (sharedata == undefined) {
                var NewObj = new Object();
                NewObj.Name = "Share";
                NewObj.Value = $scope.TotalValue;
                NewObj.NoOfItem = $scope.NoOfShares;
                NewObj.CertificateNo = $scope.ShareCertificateNo;
                $scope.LoanCharges.push(NewObj);
            }
            else {
                sharedata.Value = $scope.TotalValue;
                sharedata.NoOfItem = $scope.NoOfShares;
                sharedata.CertificateNo = $scope.ShareCertificateNo;
            }
        }

        angular.forEach($scope.LoanCharges, function (value, index) {
            value.LoanId = $scope.Loan.LoanId
        });

        var saveLoanCharges = AppService.SaveData("Loan", "SaveLoanCharges", $scope.LoanCharges)
        saveLoanCharges.then(function (p1) {
            if (p1.data != null) {
                $scope.LoanCharges = p1.data.LoanCharges;
                $scope.Loan.DisbursementAmount = p1.data.DisbursementAmount;
                $(".help-block").remove();
                $('.form-group').removeClass('has-error');
                $('.form-group').removeClass('help-block-error');
                if (string == "Status") {
                    $scope.SaveCustomerLoanDetails();
                    $scope.UpdateStatus.UpdatedBy = getUserdata.UserId;
                    $scope.UpdateStatus.LoanId = $scope.Loan.LoanId;
                    var saveUpdateLoanStatus = AppService.SaveData("Loan", "SaveUpdatedLoanStatus", $scope.UpdateStatus);
                    saveUpdateLoanStatus.then(function (p2) {
                        if (p2.data) {
                            $scope.Loan.LoanStatus = p2.data;
                            GetLoanStatusList($scope.Loan.LoanId)
                            showToastMsg(1, 'Status Updated successfully.')
                        }
                    }, function (err) {
                        showToastMsg(3, 'Error in Saving data')
                    });
                } else {
                    LoanAmountisation();
                }
            }
            else {
                showToastMsg(3, 'Error in saving data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in Saving Data')
        });
    }

    function GetLoanStatusList(loanId) {
        var statuslist = AppService.GetDetailsById("Loan", "GetLoanStatusList", loanId);
        statuslist.then(function (p2) {
            if (p2.data) {
                $scope.LoanStatusList = p2.data;
                showToastMsg(1, 'Status Updated successfully.')
            }
        }, function (err) {
            showToastMsg(3, 'Error in Saving data')
        });
    }

    $scope.AddCharges = function () {
        var abc = { Name: "", Value: "", LoanId: '00000000-0000-0000-0000-000000000000', ChargesId: '00000000-0000-0000-0000-000000000000', LoanChargesId: '00000000-0000-0000-0000-000000000000', IsDelete: false }
        $scope.LoanCharges.push(abc);
    }

    $scope.RemoveCharges = function ($index) {
        var data = $scope.LoanCharges[$index + 1];
        if (data != undefined) {
            if (data.LoanChargesId != '00000000-0000-0000-0000-000000000000') {
                $scope.LoanCharges[$index].IsDelete = true;
                var deleteloanCharges = AppService.DeleteData("Loan", "DeleteCharges", $scope.LoanCharges[$index + 1].LoanChargesId);
                deleteloanCharges.then(function (p1) {
                    if (p1.data != null) {
                        //GetChargesList();
                        GetCustomerPersonaldetail($scope.CustomerProductId, $scope.Loan.LoanId)
                    }
                    else {
                        showToastMsg(3, 'Error in saving data')
                    }
                }, function (err) {
                    showToastMsg(3, 'Error in Saving Data')
                });

            }
            else {
                $scope.LoanCharges.splice($index, 1);
            }

        }
        else {
            $scope.LoanCharges.splice($index, 1);
        }
    }

    function LoanAmountisation() {


        $scope.TotalCharges = 0;
        angular.forEach($scope.LoanCharges, function (item) {
            if (!item.IsDelete && item.Value != null && item.Value != 0) {
                $scope.TotalCharges += item.Value;
            }
        });

        $scope.PrincipalAmount = $scope.Loan.DisbursementAmount + $scope.TotalCharges;

        if ($scope.Loan.IsDisbursed == false) {
            if ($scope.Loan.LoanIntrestRate != "" && $scope.Loan.LoanIntrestRate != undefined && $scope.Loan.DisbursementAmount != "" &&
                $scope.Loan.DisbursementAmount != undefined && $scope.Loan.Term != "" && $scope.Loan.Term != undefined && $("#txtInstallmentDate").val() != "" && $("#txtInstallmentDate").val() != undefined) {

                var obj = new Object()
                obj.PrincipalAmount = $scope.PrincipalAmount;
                obj.LoanIntrestRate = $scope.Loan.LoanIntrestRate;
                obj.Term = $scope.Loan.Term;
                obj.LoanId = $scope.Loan.LoanId;
                obj.InstallmentDate = moment(new Date($("#txtInstallmentDate").data("DateTimePicker").date())).format('YYYY-MM-DD');

                var Amountisation = AppService.SaveData("Loan", "LoanAmountisation", obj);
                Amountisation.then(function (p1) {
                    if (p1.data != null) {
                        $scope.LoanAmountisation = p1.data.ListLoanAmountisation;
                        $scope.Loan.MonthlyInstallmentAmount = p1.data.MonthlyInstallmentAmount;
                    }
                    else {
                        showToastMsg(3, 'Error in saving data')
                    }
                }, function (err) {
                    showToastMsg(3, 'Error in Saving Data')
                });
            }
        }
        else {
            var displayAmountisation = AppService.GetDetailsById("Loan", "DisplayAmountisation", $scope.Loan.LoanId);
            displayAmountisation.then(function (p1) {
                if (p1.data != null) {
                    $scope.LoanAmountisation = p1.data.ListLoanAmountisation;
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving Data')
            });
        }
    }

    $scope.PrintAmountisation = function () {
        var printContent = document.getElementById('printAmountisation');
        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
        //WinPrint.document.write("<style>");
        //WinPrint.document.write("#tblholderdetail {padding-top:0px;padding-bottom:100px;} </style>");

        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        //WinPrint.close();
        $timeout(function () { WinPrint.close(); }, 2000);
    };

    $scope.PrintAmountisation = function () {
        var printContent = document.getElementById('printAmountisation');
        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        //WinPrint.close();
        $timeout(function () { WinPrint.close(); }, 2000);
    };

    $scope.OpenModalDisbursementLetter = function () {
        var datadisbursement = AppService.GetDetailsById("Loan", "DisbursementLetter", $scope.Loan.LoanId);
        datadisbursement.then(function (p1) {
            if (p1.data != null) {
                $scope.DisbursementLetter = p1.data;
                $scope.DisbursementLetter.LoanDetail.AmountInWords = inWords($scope.DisbursementLetter.LoanDetail.TotalLoanAmount)

                $("#divDisbursementLetter").modal('show')
            }
            else {
                showToastMsg(3, 'Error while getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in Saving Data')
        });
    }

    $scope.PrintDisbursementLetter = function () {
        var printContent = document.getElementById('DisbursementLetter');

        var WinPrint = window.open('', '', 'left=0,top=0,width=800,height=900,toolbar=0,scrollbars=0,status=0')
        WinPrint.document.write("<style>");
        WinPrint.document.write("#tbldetail td {border: 1px solid black;}#borrowerdetail td {border: none;}#loancharges td {border: 1px solid black;} </style>");
        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        //WinPrint.close();
        $timeout(function () { WinPrint.close(); }, 2000);
    }

    var a = ['', 'one ', 'two ', 'three ', 'four ', 'five ', 'six ', 'seven ', 'eight ', 'nine ', 'ten ', 'eleven ', 'twelve ', 'thirteen ', 'fourteen ', 'fifteen ', 'sixteen ', 'seventeen ', 'eighteen ', 'nineteen '];
    var b = ['', '', 'twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];

    function inWords(num) {
        if ((num = num.toString()).length > 9) return 'overflow';
        n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
        if (!n) return; var str = '';
        str += (n[1] != 0) ? (a[Number(n[1])] || b[n[1][0]] + ' ' + a[n[1][1]]) + 'crore ' : '';
        str += (n[2] != 0) ? (a[Number(n[2])] || b[n[2][0]] + ' ' + a[n[2][1]]) + 'lakh ' : '';
        str += (n[3] != 0) ? (a[Number(n[3])] || b[n[3][0]] + ' ' + a[n[3][1]]) + 'thousand ' : '';
        str += (n[4] != 0) ? (a[Number(n[4])] || b[n[4][0]] + ' ' + a[n[4][1]]) + 'hundred ' : '';
        str += (n[5] != 0) ? ((str != '') ? 'and ' : '') + (a[Number(n[5])] || b[n[5][0]] + ' ' + a[n[5][1]]) + 'only ' : '';
        return str;
    }

    $scope.OpenModalDisbursementAmount = function () {
        $scope.IsDisbursement = true;
        var flag = true;
        flag = ValidateCustomerLoan();
        if (flag) {
            LoanAmountisation()
            $scope.SaveLoanCharges();
            $scope.Loan.TotalDisbursementAmount = $scope.Loan.DisbursementAmount;
            $("#DisbursementAmount .help-block").remove();
            $('#DisbursementAmount .form-group').removeClass('has-error');
            $('#DisbursementAmount .form-group').removeClass('help-block-error');

            $("#DisbursementAmount").modal('show')
        }
    }

    function ValidateDisbursementAmountForm() {

        var flag = true;

        $("#DisbursementAmount .help-block").remove();
        $('#DisbursementAmount .form-group').removeClass('has-error');
        $('#DisbursementAmount .form-group').removeClass('help-block-error');


        if (!ValidateRequiredField($("#ddlThrough"), 'please select the type', 'after')) {
            flag = false;
        }

        if ($scope.Loan.DisburseThrough == 2) {
            if (!ValidateRequiredField($("#txtChequeDate"), 'Cheque Date required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtBankName"), 'Bank Name required', 'after')) {
                flag = false;
            }
            if (!ValidateRequiredField($("#txtChequeNo"), 'Cheque No required', 'after')) {
                flag = false;
            }
        }

        return flag;
    }

    $scope.SaveDisbursementAmount = function () {
        var flag = true;
        flag = ValidateDisbursementAmountForm();
        if (flag) {
            $(':focus').blur();
            $scope.Loan.DisbursementBy = getUserdata.UserId;
            if ($scope.Loan.DisburseThrough == 2) {
                $scope.Loan.ChequeDate = moment(new Date($("#txtChequeDate").data("DateTimePicker").date())).format('YYYY-MM-DD');
            }
            else {
                $scope.Loan.ChequeDate = null
                $scope.Loan.BankName = null;
                $scope.Loan.ChequeNo = null;
            }

            var datadisbursement = AppService.SaveData("Loan", "SaveDisbursementAmount", $scope.Loan);
            datadisbursement.then(function (p1) {
                if (p1.data) {
                    $scope.Loan.IsDisbursed = true;
                    $scope.Loan.LoanStatus = 7;
                    $scope.UpdateStatus.LoanStatus = "7";
                    GetLoanStatusList($scope.Loan.LoanId)
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                    showToastMsg(1, 'You have successfully disbursement of loan.')
                    $("#DisbursementAmount").modal('hide')
                }
                else {
                    showToastMsg(3, 'Error while saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in Saving Data')
            });
        }
    }
});