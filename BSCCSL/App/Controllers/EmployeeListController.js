angular.module("BSCCL").controller('EmployeeListController', function ($scope, $state, $cookies, $filter) {

    GetEmployeeList();

    function GetEmployeeList() {
        $('#tblEmployee').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/User/GetScreeSalesList",

            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtsearchEmployee").val() });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {

                        fnCallback(json);
                        IntialPageControlAgentList();
                    }
                });
            },
            "aoColumns": [{
                "mDataProp": "Name",
                "mRender": function (data, type, full) {
                    return full.FirstName + ' ' + full.LastName
                },
            },

           {
               "mDataProp": "Gender",
           },
            {
                "mDataProp": "PhoneNumber",
            },
            {
                "mDataProp": "DateOfBirth",
                "mRender": function (data, type, full) {
                    return $filter('date')(data, 'dd/MM/yyyy');
                },
            },
            {
                "mDataProp": "Branch.BranchName",
            },
            {
                "mDataProp": "UserCode",
            },
            {
                "mDataProp": "RoleName",
            },
            {
                "mDataProp": "UserId",
                "mRender": function (data, type, full) {

                    return '<a class="btn btn-warning btn-xs btnEmpSelect" Id="' + data + '" title="Select"><span class="fa fa-share-square-o"></span>&nbsp;Select</a>';
                },

            }]

        });

    }

    function IntialPageControlAgentList() {
        $(".btnEmpSelect").click(function () {
            var ID = $(this).attr("Id");
            $('#Employee').modal('hide');
            $scope.GetAllUserById(ID)
            $('#txtsearchEmployee').val('');
            $('#tblEmployee').dataTable().fnDraw();
        });
    }

    $scope.SearchEmployee = function () {
        $('#tblEmployee').dataTable().fnDraw();
    }

    $scope.ClearSearchEmployee = function () {
        $('#txtsearchEmployee').val('');
        $('#tblEmployee').dataTable().fnDraw();
    }

    $("#Employee").on('hidden.bs.modal', function (event) {
        if ($('.modal:visible').length) //check if any modal is open
        {
            $('body').addClass('modal-open');//add class to body
        }
    });
});