angular.module("BSCCL").service('AppService', function ($http) {

    this.GetDetailsWithoutId = function (Controller, Function) {
        return $http.get(urlpath + Controller + "/" + Function);
    };

    this.GetDetailsById = function (Controller, Function, Id) {
        return $http.get(urlpath + Controller + "/" + Function + "/" + Id);
    };

    this.UpdateTransactionData = function (Controller, Function, ID) {

        return $http.post(urlpath + Controller + "/" + Function + "/" + ID);
    }

    this.DeleteData = function (Controller, Function, Id) {
        //return $http.post(urlpath + Controller + "/" + Function + "/" + Id);

        var request = $http({
            method: "POST",
            url: urlpath + Controller + "/" + Function + "/" + Id,
        })
        return request;
    };

    this.DeleteMultipleData = function (Controller, Function, CustomerProductdata) {

        var request = $http({
            method: "POST",
            url: urlpath + Controller + "/" + Function,
            data: CustomerProductdata
        })
        return request;
    }

    this.SaveData = function (Controller, Function, CustomerProductdata) {

        var request = $http({
            method: "Post",
            url: urlpath + Controller + "/" + Function,
            data: CustomerProductdata
        })
        return request;
    }

    this.GetDataById = function (Email) {
        var request = $http({
            method: "GET",
            url: urlpath + "User/GetUsersEmailId?Email=" + Email
        });
        return request;
    };

    //this.GetLoanData = function (Id, LoanId) {
    //    var request = $http({
    //        method: "GET",
    //        url: urlpath + "Loan/GetCustomerPersonalDetailById?id=" + Id + "&LoanId=" + LoanId
    //    });
    //    return request;
    //};


    this.GetDataByQuerystring = function (Controller, Function, Id1, Id2) {
        var request = $http({
            method: "GET",
            url: urlpath + Controller + "/" + Function + "?Id1=" + Id1 + "&Id2=" + Id2
        });
        return request;
    };

    //With Three Parameter
    this.GetDataByQrystring = function (Controller, Function, Id1, Id2, Id3) {
        var request = $http({
            method: "GET",
            url: urlpath + Controller + "/" + Function + "?Id1=" + Id1 + "&Id2=" + Id2 + "&Id3=" + Id3
        });

        return request;
    };


    //With Four Parameter
    this.GetDataByQrysr = function (Controller, Function, Id1, Id2, Id3, Id4) {
        var request = $http({
            method: "GET",
            url: urlpath + Controller + "/" + Function + "?Id1=" + Id1 + "&Id2=" + Id2 + "&Id3=" + Id3 + "&Id4=" + Id4,
        });

        return request;
    };


    //With Five Parameter
    this.GetDataByQryStr = function (Controller, Function, Id1, Id2, Id3, Id4, Id5) {
        var request = $http({
            method: "POST",
            url: urlpath + Controller + "/" + Function + "?Id1=" + Id1 + "&Id2=" + encodeURIComponent(Id2) + "&Id3=" + Id3 + "&Id4=" + Id4,
            data: Id5

        });

        return request;
    };


    //With Seven Parameter
    this.GetDataByQS = function (Controller, Function, Id1, Id2, Id3, Id4, Id5, Id6, Id7) {
        var request = $http({
            method: "POST",
            url: urlpath + Controller + "/" + Function + "?Id1=" + Id1 + "&Id2=" + encodeURIComponent(Id2) + "&Id3=" + Id3 + "&Id4=" + Id4 + "&Id6=" + Id6 + "&Id7=" + Id7,
            data: Id5

        });

        return request;
    };

    this.GetDataBySearchFilter = function (Controller, Function, Data) {
        var request = $http({
            method: "Post",
            url: urlpath + Controller + "/" + Function,
            data: Data
        })
        return request;
    }


    this.UploadDocumentwithData = function (Controller, Function, Documents) {

        var request = $http.post(urlpath + Controller + "/" + Function, Documents, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
            //data: Data
        })

        return request;
    }


    //this.GetShareDetailForPrint = function (Controller, Function, ID) {

    //    return $http.get(urlpath + Controller + "/" + Function + "/" + ID);
    //}


    //this.UpdateTransactionDataForBounce = function (Controller, Function, Data) {
    //    var request = $http({
    //        method: "Post",
    //        url: urlpath + Controller + "/" + Function,
    //        data: Data
    //    })
    //    return request;
    //}
})