angular.module("BSCCL").controller('RptTransactionController', function ($scope, AppService, $state, $cookies, $filter, $location, $rootScope) {


    $("#txtStartDateforSearch,#txtEndDateforSearch").datetimepicker({
        format: 'DD/MM/YYYY',
        useCurrent: false,
    });

    $scope.SearchTransactionList = function () {

        if ($scope.TransactionrptAccountNumber != null && $scope.TransactionrptAccountNumber != undefined && $scope.TransactionrptAccountNumber != "") {
            $('#tblTransaction').dataTable().fnDraw();
            GetTransactionList()
        }
        else {
            showToastMsg(3, 'Enter Account Number');
        }
    }

    GetTransactionList()
    function GetTransactionList() {

        $('#tblTransaction').dataTable({
            "bFilter": false,
            "processing": false,
            "bInfo": true,
            "bServerSide": true,
            "bLengthChange": true,
            "lengthMenu": [25, 50, 100, 500, 1000, 5000, 10000],
            "bSort": false,
            "bDestroy": true,
            searching: false,
            dom: 'l<"floatRight"B>frtip',
            buttons: [
                 {
                     extend: 'pdf',
                     //footer: true
                 },
                  {
                      extend: 'print',
                      //footer: true
                  }
            ],
            "sAjaxSource": urlpath + "/Transaction/GetTransactionRptData",
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "AccountNumber", "value": $scope.TransactionrptAccountNumber });
                aoData.push({ "name": "fromDate", "value": ddmmyyTommdddyy($("#txtStartDateforSearch").val()) });
                aoData.push({ "name": "toDate", "value": ddmmyyTommdddyy($("#txtEndDateforSearch").val()) });
                $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": function (json) {
                        fnCallback(json);
                    }
                });
            },
            "aoColumns": [
              {
                  "mDataProp": "TransactionTime",
                  "mRender": function (data) {
                      return $filter('date')(data, 'dd/MM/yyyy');
                  },
              },
              {
                  "mDataProp": "CheckNumber",
              },
              {
                  "mDataProp": "Type",
                  "sClass": "right",
                  "mRender": function (data, type, full) {
                      if (data == 1) {
                          return $filter('currency')(full.Amount, '', 2);


                      }
                      else {
                          return ''
                      }
                  },

              },
              {
                  "mDataProp": "Type",
                  "sClass": "right",
                  "mRender": function (data, type, full) {
                      if (data == 2) {
                          return $filter('currency')(full.Amount, '', 2);

                      }
                      else {
                          return ''
                      }
                  },

              },
              {
                  "mDataProp": "Balance",
                  "mRender": function (data, type, full) {
                      return $filter('currency')(data, '', 2);
                  },
                  "sClass": "right"
              }
            ]
        });
    }

    $scope.Reset = function () {
        $scope.TransactionrptAccountNumber = " ";
        $("#txtStartDateforSearch").val('');
        $("#txtEndDateforSearch").val('');
        $scope.SearchTransactionList();
    }
});