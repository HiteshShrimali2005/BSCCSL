angular.module("BSCCL").controller('CustomerProductController', function ($scope, AppService, $state, $cookies, $filter, $location) {

    $scope.UserBranch.ShowBranch = false;
    $scope.UserBranch.Enabled = false;
    //$scope.UserBranch.BranchId = $cookies.get('Branch');

    $scope.GetProductNameAsSelectedType = function (ProducttypeId) {
        $scope.Productlist = [];
        var Promis = AppService.GetDetailsById("CustomerProduct", "GetProductNameAsSelectedType", ProducttypeId);
        Promis.then(function (p1) {
            if (p1.data != null) {
                $scope.Productlist = [];
                $scope.Productlist = p1.data;
            }
            else {
                showToastMsg(3, 'No Product Avilable')
            }
        })
    }

    $scope.SaveProductRank = function () {

        var flag = true;
        flag = ValidateProductRankForm();
        if (flag) {
            var saveuser = AppService.SaveData("ProductRank", "SaveProductRank", $scope.productRank)

            saveuser.then(function (p1) {
                if (p1.data) {

                    $('#ProductRank').modal('hide');
                    showToastMsg(1, "User Saved Successfully");
                    $scope.productRank = {};
                }
                else {
                    showToastMsg(3, 'Error in saving data')
                }
            }, function (err) {
                showToastMsg(3, 'Error in saving data')
            });
        }
    }

    function ValidateProductRankForm() {
        $(".help-block").remove();
        $('.form-group').removeClass('has-error');
        $('.form-group').removeClass('help-block-error');
        var flag = true;
        if (!ValidateRequiredField($("#ddlProductTypelist"), 'Product Type required', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#ddlProductNamelist"), 'Product required', 'after')) {
            flag = false;
        }

        if (!ValidateRequiredField($("#txtPercentage"), 'Percentage required', 'after')) {
            flag = false;
        }

        return flag;
    }

    function GetUserList() {

        $('#tblProductRank').dataTable({
            "bFilter": false,
            "processing": false,
            "bInfo": true,
            "bServerSide": true,
            pageLength: 25,
            //"bLengthChange": true,
            "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
            "bSort": false,
            "bDestroy": true,
            searching: false,
            dom: 'l<"floatRight"B>frtip',
            buttons: [
                 {
                     extend: 'pdf',
                     exportOptions: {
                         columns: [0, 1, 2, 3, 4, 5, 6]
                     }
                 },
                  {
                      extend: 'print',
                      exportOptions: {
                          columns: [0, 1, 2, 3, 4, 5, 6]
                      }
                  }
            ],
            "sAjaxSource": urlpath + "ProductRank/GetProductRankList",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearch").val() });

                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                        IntialPageControlProductRank();
                    }
                });

            },
            "aoColumns": [{
                "mDataProp": "ProductType",

            },
             {
                 "mDataProp": "ProductName",
             },
            {
                "mDataProp": "Percentage",
            },
            {
                "mDataProp": "ProductRankId",
                "mRender": function (data, type, full) {

                    return '<button class="btn btn-success btn-xs btn-flat btnEdit" Id="' + data + '" title="Edit"><span class="glyphicon glyphicon-edit"></span> Edit</button>  <button class="btn btn-danger btn-xs btn-flat btnDelete" Id="' + data + '" title="Delete"><span class="glyphicon glyphicon-remove"></span> Delete</button> ';
                },
                "sClass": "text-center"

            }]
        });
    }

    function IntialPageControlProductRank() {

        $(".btnEdit").click(function () {
            $(".help-block").remove();
            $('.form-group').removeClass('has-error');
            $('.form-group').removeClass('help-block-error');
            var ID = $(this).attr("Id");
            $("#ProductRank").modal('show');
            GetProductRankDataById(ID)
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
                            var promiseDelete = AppService.DeleteData("ProductRank", "DeleteProductRank", ID);
                            promiseDelete.then(function (p1) {
                                var status = p1.data;
                                if (status == true) {
                                    toastr.remove();
                                    showToastMsg(1, "User Deleted Successfully");
                                    $('#tblProductRank').dataTable().fnDraw();
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

    function GetProductRankDataById(Id) {
        var getProductRankData = AppService.GetDetailsById("ProductRank", "GetProductRankDataById", Id);

        getProductRankData.then(function (p1) {
            if (p1.data != null) {

                $scope.productRank = p1.data;
            }
            else {
                showToastMsg(3, 'Error in getting data')
            }
        }, function (err) {
            showToastMsg(3, 'Error in getting data')
        });
    }

    $scope.SearchData = function () {
        $('#tblProductRank').dataTable().fnDraw();
    }

    $scope.SearchClearData = function () {
        $('#txtSearch').val('');
        $('#tblProductRank').dataTable().fnDraw();
    }



})