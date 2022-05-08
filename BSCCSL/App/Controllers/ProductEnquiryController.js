angular.module("BSCCL").controller('ProductEnquiryController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {


    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    GetProductEnquiryList();

    $rootScope.ChangeBranch = function () {
        $cookies.put('Branch', $scope.UserBranch.BranchId)
        $('#tblProductEnquiry').dataTable().fnDraw();
    }

    function GetProductEnquiryList() {
        $('#tblProductEnquiry').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/ProductEnquiry/GetProductEnquiryList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });
                aoData.push({ "name": "BranchId", "value": $scope.UserBranch.BranchId });
                aoData.push({ "name": "fromDate", "value": $("#txtStartDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtStartDateforSearch").val()) : "" });
                aoData.push({ "name": "toDate", "value": $("#txtEndDateforSearch").val() != "" ? ddmmyyTommdddyy($("#txtEndDateforSearch").val()) : "" });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {

                        fnCallback(json);
                        IntialPageControlProductEnquiry()
                    }
                });
            },
            "aoColumns": [{
                "mDataProp": "FirstName",
            }, {
                "mDataProp": "LastName",
            },
             {
                 "mDataProp": "ContactNumber",
             },
             {
                 "mDataProp": "ProductTypeName",
             },
             //{
             //    "mDataProp": "Comments",
             //},
             {
                 "mDataProp": "EnquiryBy",
                 "mRender": function (data, type, full) {
                     if (data != null && data != "") {
                         return data;
                     }
                     else {
                         return full.EnquiryByUser
                     }
                 },
             },
             {
                 "mDataProp": "EnquirySourceName",
             },
             {
                 "mDataProp": "EnquiryDate",
                 "mRender": function (data, type, full) {
                     return $filter('date')(data, 'dd/MM/yyyy');
                 },
                 "sWidth": "100px"
             },
             {
                 "mDataProp": "StatusName",
                 "sWidth": "100px"
             },
            {
                "mDataProp": "ProductEnquiryId",
                "mRender": function (data, type, full) {
                    var str = '<button class="btn btn-success btn-xs btnEdit btn-flat" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>';

                    return str
                },
                "sClass": "text-center",
                "sWidth": "100px"
            }
            ]
        });
    }

    function IntialPageControlProductEnquiry() {
        $(".btnEdit").click(function () {
            var PID = $(this).attr("Id");
            $scope.ProductId = PID;
            $("#ProductEnquiry").modal('show');
            GetProductEnquiryDataById(PID)
        });
    }

    $scope.Search = function () {
        $('#tblProductEnquiry').dataTable().fnDraw();
    }

    $scope.ClearSearch = function () {
        $("#txtStartDateforSearch").val('')
        $("#txtEndDateforSearch").val('')
        $("#txtSearch").val('')
        $('#tblProductEnquiry').dataTable().fnDraw();
    }


    function GetProductEnquiryDataById(PID) {
        var getproductdata = AppService.GetDetailsById("ProductEnquiry", "GetProductEnquiryById", PID);
        getproductdata.then(function (p1) {
            if (p1.data != null) {
                $scope.Enquiry = {};
                $scope.Enquiry = p1.data
                if ($scope.Enquiry.Status == 0) {
                    $scope.Enquiry.Status = "";
                }
                else {
                    $scope.Enquiry.Status = $scope.Enquiry.Status + ""
                }

                $scope.Enquiry.ProductType = $scope.Enquiry.ProductType + "";
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }


    $scope.SaveProductEnquiry = function () {

        var flag = true;
        flag = ValidateProductEnquiryForm()
        var Promis = AppService.SaveData("ProductEnquiry", "SaveProductEnquiry", $scope.Enquiry);
        Promis.then(function (p1) {
            if (p1.data != null) {
                $("#ProductEnquiry").modal('hide');
                $('#tblProductEnquiry').dataTable().fnDraw();
            }
            else {
                showToastMsg(3, 'Please Create Saving Account.')
            }
        })

    }

    function ValidateProductEnquiryForm() {

        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');

        var flag = true;
        if (!ValidateRequiredField($("#ddlProductTypelist"), 'Product Type required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtFirstName"), 'First Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtLastName"), 'Last Name required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtContactNo"), 'Contact No required', 'after')) {
            flag = false;
        }
        if (!ValidateRequiredField($("#txtComment"), 'Comment required', 'after')) {
            flag = false;
        }

        if ($scope.Enquiry.ProductEnquiryId != undefined && $scope.Enquiry.ProductEnquiryId != null && $scope.Enquiry.ProductEnquiryId != "") {
            if (!ValidateRequiredField($("#ddlStatus"), 'Comment required', 'after')) {
                flag = false;
            }
        }
        return flag;
    }

    $scope.ClearForm = function () {
        $scope.Enquiry = {};
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
    }


})