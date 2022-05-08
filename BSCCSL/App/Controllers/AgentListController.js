angular.module("BSCCL").controller('AgentListController', function ($scope, $state, $cookies, $filter, AppService) {


    GetAgentList();

    function GetAgentList() {
        $('#tblAgent').dataTable({
            "processing": false,
            "bFilter": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": false,
            "bSort": false,
            "bDestroy": true,
            "sAjaxSource": urlpath + "/User/GetAgentList",

            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "sSearch", "value": $("#txtSearchAgent").val() });
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

                     return '<a class="btn btn-warning btn-xs btnSearch" Id="' + data + '" title="Select"><span class="fa fa-share-square-o"></span>&nbsp;Select</a>';
                 },

             }]

        });

    }

    function IntialPageControlAgentList() {
        $(".btnSearch").click(function () {
            var ID = $(this).attr("Id");
            $('#Agent').modal('hide');
            $scope.GetAllUserById(ID)
            $('#txtSearchAgent').val('');
            $('#tblAgent').dataTable().fnDraw();
        });
    }

    $scope.SearchAgent = function () {
        $('#tblAgent').dataTable().fnDraw();
    }

    $scope.ClearAgent = function () {
        $('#txtSearchAgent').val('');
        $('#tblAgent').dataTable().fnDraw();
    }


    $("#Agent").on('hidden.bs.modal', function (event) {
        if ($('.modal:visible').length) //check if any modal is open
        {
            $('body').addClass('modal-open');//add class to body
        }
    });

    //function GetAllUserById(Id) {
    //    var getemployeedetail = AppService.GetDetailsById("User", "GetUserDataById", Id)
    //    getemployeedetail.then(function (p1) {
    //        if (p1.data != null) {
    //            if ($scope.Agent == true) {
    //                $scope.CustomerDetails.Customer.AgentName = p1.data.FirstName;
    //                $scope.CustomerDetails.Customer.AgentCode = p1.data.UserCode;
    //                $scope.CustomerDetails.Customer.AgentId = p1.data.UserId;
    //            }
    //            else {
    //                $scope.CustomerDetails.Customer.EmpName = p1.data.FirstName;
    //                $scope.CustomerDetails.Customer.EmpCode = p1.data.UserCode;
    //                $scope.CustomerDetails.Customer.EmployeeId = p1.data.UserId;
    //            }
    //        }
    //        else {
    //            showToastMsg(3, "Error in Getting Data");
    //        }
    //    })

    //}
});