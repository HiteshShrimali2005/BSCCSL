function SetSidebarHeight() {
    // $('#sidebar-wrapper').css("min-height", 0 + 'px');
    var minheight = $(window).height();
    if ($('#wrapper').height() > minheight) {
        minheight = $('#wrapper').height()
    }
    $('#sidebar-wrapper').css("min-height", (minheight - 55) + 'px').css("height", "0px");

}

function PostData(url, jsondata, callbackfunction) {
    if (jsondata == '') {

        $.ajax({
            type: "POST",
            async: "true",
            contentType: "application/json",
            dataType: "json",
            url: url,
            success: function (html) {
                if (html != null) {
                    if (html.IsSessionExpired != null) {
                        if (html.IsSessionExpired == true) {
                            window.location.href = "/Employee/Login";
                            return;
                        }
                    }
                }
                eval(callbackfunction + "(" + JSON.stringify(html) + ")");
            },
            error: function (request, status, error) {
                //alert('Error: ' + error);
            }

        });
    }
    else {

        $.ajax({
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            url: url,
            data: JSON.stringify(jsondata),
            success: function (html) {
                if (html != null) {
                    if (html.IsSessionExpired != null) {
                        if (html.IsSessionExpired == true) {
                            window.location.href = "/Employee/Login";
                            return;
                        }
                    }
                }
                eval(callbackfunction + "(" + JSON.stringify(html) + ")");
            },
            error: function (request, status, error) {
                //alert('Error: ' + JSON.stringify(error));
            }

        });
    }
}

//ctl - class or id name of element
// validationMessage- to display message if valude is not provided
// default value if textbox or option has

function ValidateRequiredField(ctrl, ValidationMessage, ValidationPosition, defValue) {
    try {
        ClearMessage(ctrl);
        if ($(ctrl).val().trim() == '' || $(ctrl).val() == defValue) {
            $(ctrl).closest('.form-group').addClass('has-error');
            
            PrintMessage(ctrl, ValidationMessage, ValidationPosition);
            return false;
        }
        return true;

    } catch (e) {
        //alert(e);
    }
    return true;
}

function ValidateRequiredFieldCheckBox(ctrl, ValidationMessage, ValidationPosition, defValue) {
    try {
        ClearMessage(ctrl);
        if ($(ctrl).val().trim() == '' || $(ctrl).val() == defValue) {
            $(ctrl).closest('.form-group').addClass('has-error');
            PrintMessageforCheckBox(ctrl, ValidationMessage, ValidationPosition);
            return false;
        }
        return true;

    } catch (e) {
        //alert(e);
    }
    return true;
}

function ValidateRequiredFieldInputField(ctrl, ValidationMessage, ValidationPosition, defValue) {
    try {

        $(ctrl).closest('.form-group').removeClass('has-error');
        $(ctrl).closest('.input-group').removeClass('has-error');
        $(ctrl).closest('.input-group').prev('.help-block').remove();
        $(ctrl).closest('.input-group').next('.help-block').remove();
        if ($(ctrl).val() == '' || $(ctrl).val() == defValue) {
            $(ctrl).closest('.form-group').addClass('has-error');
            errorctrl = $(ctrl).closest(".input-group");
            PrintMessage(errorctrl, ValidationMessage, ValidationPosition);
            return false;
        }
        return true;

    } catch (e) {
        //alert(e);
    }
    return true;
}

function ValidateRequiredField2(ctrl, vctrl, ValidationMessage, ValidationPosition, defValue) {
    try {
        ClearMessage(vctrl);
        if ($(ctrl).val() == '' || $(ctrl).val() == defValue) {
            PrintMessage(vctrl, ValidationMessage, ValidationPosition);
            return false;
        }

        return true;

    } catch (e) {
        //alert(e);
    }
    return true;
}

function CheckNumber(ctrl,ValidationMessage, ValidationPosition) {
    if ($(ctrl).val() != '') {
        ClearMessage(ctrl);
        var regex = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"
        if (!regex.test($(ctrl).val())) {
            $(ctrl).closest('.form-group').addClass('has-error');
            PrintMessage(ctrl, ValidationMessage, ValidationPosition);
            return false;
        }
    }
    return true;
}

function CheckpinCode(ctrl, ValidationMessage, ValidationPosition)
{
    if ($(ctrl).val() != '') {
        ClearMessage(ctrl);
        var regex = /(^\d{6}$)/;
        if (!regex.test($(ctrl).val())) {
            $(ctrl).closest('.form-group').addClass('has-error');
            PrintMessage(ctrl, ValidationMessage, ValidationPosition);
            return false;

        }
    }
    return true;
}

function CheckMobileNum(ctrl, ValidationMessage, ValidationPosition) {
    if ($(ctrl).val() != '') {
        ClearMessage(ctrl);
        var regex = /^[6789]\d{9}$/;
        if (!regex.test($(ctrl).val())) {
            $(ctrl).closest('.form-group').addClass('has-error');
            PrintMessage(ctrl, ValidationMessage, ValidationPosition);
            return false;

        }
    }
    return true;
}

function CheckMobileNumandphnnum(ctrl, ValidationMessage, ValidationPosition) {
    
    if ($(ctrl).val() != '') {
        ClearMessage(ctrl);
        var regex1 = /^[0-9]\d{1,4}-\d{6,8}$/;
        var regex = /^[6789]\d{9}$/;
        if ((!regex1.test($(ctrl).val())) && (!regex.test($(ctrl).val()))) {
            $(ctrl).closest('.form-group').addClass('has-error');
            PrintMessage(ctrl, ValidationMessage, ValidationPosition);
            return false;

        }
    }
    return true;
}

function CheckOnlyText(ctrl, ValidationPosition) {
    
    if ($(ctrl).val() != '') {
        ClearMessage(ctrl);
        var regex = /^[a-z A-Z]+$/;
        if (!regex.test($(ctrl).val())) {
            $(ctrl).closest('.form-group').addClass('has-error');
            PrintMessage(ctrl,'Only Alphabet Allowed' , ValidationPosition);
            return false;

        }
    }
    return true;
}

function CheckGreterNumber(ctrl1, ctrl2, ValidationMessage, ValidationPosition) {
    
    if ($(ctrl1).val() != '' && $(ctrl2).val() != '') {
        ClearMessage(ctrl1);
        ClearMessage(ctrl2);
        if (parseFloat($(ctrl1).val()) >= parseFloat($(ctrl2).val())) {
            $(ctrl2).closest('.form-group').addClass('has-error');
            PrintMessage(ctrl2, ValidationMessage, ValidationPosition);
            return false;
        }
    }
    return true;
}

function CheckRadioChecked(ctrl1, ctrl2, Message, ValidationPosition) {
    if ($(ctrl1).prop("checked") === false && $(ctrl2).prop("checked") === false) {
        ClearMessage(ctrl1);
        ClearMessage(ctrl2);
        $(ctrl1).closest('.form-group').addClass('has-error');
        PrintMessage($(ctrl2).parents("label"), Message, ValidationPosition);
        return false;
    }
    ClearMessage(ctrl1);
    ClearMessage(ctrl2);
    return true;
}

function PrintMessage(ctrl, Message, ValidationPosition) {
    if (ValidationPosition == "after") {
        $('<span class="help-block help-block-error">' + Message + '</span>').insertAfter($(ctrl));
    }
    else {
        $('<span class="help-block help-block-error">' + Message + '</span>').insertBefore($(ctrl));
    }
}

function PrintMessageforCheckBox(ctrl, Message, ValidationPosition) {
    if (ValidationPosition == "after") {
        $('<span class="help-block help-block-error">' + Message + '</span>').insertAfter($(ctrl).closest('.icheckbox_flat-blue'));
    }
    else {
        $('<span class="help-block help-block-error">' + Message + '</span>').insertBefore($(ctrl).closest('.icheckbox_flat-blue'));
    }
}


function ClearMessage(ctrl) {
    $(ctrl).closest('.form-group').removeClass('has-error');
    $(ctrl).next('.help-block').remove();
    $(ctrl).prev('.help-block').remove();
}

function showmsg(ctrl, type, msg2) {
    $($(ctrl)).html('');
    if (type == 1) {
        cls = 'alert-success';

    }
    if (type == 2) {
        cls = 'alert-info';
    }
    else if (type == 3) {
        cls = 'alert bg-danger';
    }

    var msg = '<div class="' + cls + '">'
    msg += msg2 + '<a href="#" class="pull-right"><span class="glyphicon glyphicon-remove"></span></a></div>';

    $(ctrl).append(msg);
}

function showToastMsg(type, msg) {
    var msgType;
    if (type == 1) {
        msgType = 'success';
        title = "Success";
    }
    if (type == 2) {
        msgType = 'info';
        title = "Info";
    }
    else if (type == 3) {
        msgType = 'error';
        title = "Error";
    }

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    var $toast = toastr[msgType](msg, title);
}

function SetTimeOut(Id) {
    window.setTimeout(function () {
        $(".alert").slideUp(500, function () {
            $(this).hide();
        });
    }, 2000);
}

function CommonDialog() {
    $("#DeleteDialog").dialog({
        autoOpen: false,
        width: 300,
        open: function (event, ui) {
            $(this).find('#lblName').text($(this).data("name"))
        }
    });

    CommonDialogButtons();
}

function CommonDialogButtons() {
    $('#btnDelCancel').click(function () {
        $("#DeleteDialog").dialog('close');
    })
}

function DeletePopup(control) {

    var idname = $(control).attr("IDName");
    var idname = idname.split(':');
    $("#deleteModal").data("id", parseInt(idname[0])).data("name", idname[1]);
    $("#lblName").text(idname[1]);

    $("#deleteModal").modal();
}

function CheckConfirmPassword(ctrl1, ctrl2, ValidationPosition) {
    if ($(ctrl1).val() != '' && $(ctrl2).val() != '') {
        ClearMessage(ctrl1);
        ClearMessage(ctrl2);
        if ($(ctrl1).val() != $(ctrl2).val()) {
            //PrintMessage(ctrl2, 'The password you have entered is not valid!', ValidationPosition);
            PrintMessage(ctrl2, _validate_Password_ConfirmPassword, ValidationPosition);
            return false;
        }

    }
    return true;
}

function CheckEmail(ctrl, ValidationPosition) {

    if ($(ctrl).val() != '') {
        ClearMessage(ctrl);
        var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        if (!regex.test($(ctrl).val())) {
            $(ctrl).closest('.form-group').addClass('has-error');
            PrintMessage(ctrl, "Invalid Email Address", ValidationPosition);
            return false;
        }
    }
    return true;
}

function CheckGender(ctrl1, ctrl2, ValidationPosition) {
    if ($(ctrl1).prop("checked") == false && $(ctrl2).prop("checked") == false) {
        ClearMessage(ctrl1);
        ClearMessage(ctrl2);
        PrintMessage(ctrl2, 'Gender Required', ValidationPosition);
        return false;
    }
    ClearMessage(ctrl1);
    ClearMessage(ctrl2);
    return true;
}

function TimeTrace(value) {
    if (value != '' && value != null) {
        var data = value;
        var DatabaseTime = data.split("T");
        var CleanTime = DatabaseTime[1].split(".");
        return CleanTime[0];
    }
}

function ddmmyyTommdddyy(value) {
    if (value != '' && value != null) {
        var data = value;
        var m = data.split('/');
        var formatedDate = m[1] + '/' + m[0] + '/' + m[2];
        return formatedDate;
    }
}

function mmdddyyToddmmyy(value) {
    if (value != '' && value != null) {
        var data = value;
        var m = data.split('/');
        var formatedDate = m[1] + '/' + m[0] + '/' + m[2];
        return formatedDate;
    }
}

function ddmmyyToyyyymmdd(value) {
    if (value != '' && value != null) {
        var d = new Date(value.split("/").reverse().join("-"));
        var dd=d.getDate();
        var mm=d.getMonth()+1;
        var yy=d.getFullYear();
       return yy+"/"+mm+"/"+dd;
    }
}




function DateFormatter(value) {

    if (value != '' && value != null) {
        var data = value;
        var m = data.split(/[T-]/);
        var d = new Date(parseInt(m[0]), parseInt(m[1]) - 1, parseInt(m[2]));

        var curr_date = d.getDate();
        var curr_month = d.getMonth() + 1
        var curr_year = d.getFullYear();

        if (curr_month < 10) {
            curr_month = "0" + curr_month;
        }

        if (curr_date < 10) {
            curr_date = "0" + curr_date;
        }

        var formatedDate = curr_date + '/' + curr_month + '/' + curr_year;

        return formatedDate;
    }


}

function capitalize(str) {
    str = str.toLowerCase().replace(/\b[a-z]/g, function (letter) {
        return letter.toUpperCase();
    });
    return str;
}

function getAge(dateString) {
    var today = new Date();
    var birthDate = new Date(dateString);
    var age = today.getFullYear() - birthDate.getFullYear();
    var m = today.getMonth() - birthDate.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
        age--;
    }
    if (age < 0)
        age = -(age);
    return age;
}

function ConcateContact(value) {
    if (value.countryCode != undefined && value.countryCode != "")
    { value.Phone1 = "+" + value.countryCode + "_" + value.Phone1; }
    else
    { value.Phone1 = value.Phone1; }
    return value;
}

function ReplaceContact(value) {
    if (value != '' && value != null) {
        var data = value;
        var k = data.replace(/_/g, "-");
        return k;
    }
}

function SplitContact(value) {
    if (value != '' && value != null) {
        var k = new Object();
        var data = value;
        if (data.indexOf('_') === -1) {
            k.Phone = data;
        }
        else {
            arr = data.split("_");
            mystring1 = arr[0];
            mystring2 = arr[1];
            var arr1 = mystring1.split("+");
            mystring3 = arr1[0];
            mystring4 = arr1[1];
            k.countrycode = mystring4;
            k.Phone = mystring2;
        }
        return k;
    }
}

function DateFormatterMMDDYYYY(value) {
    if (value.length < 11) {
        return value;
    }
    else {
        if (value != '' && value != null) {
            var data = value;
            var m = data.split(/[T-]/);
            var d = new Date(parseInt(m[0]), parseInt(m[1]) - 1, parseInt(m[2]));
            var curr_date;
            var curr_month;
            var curr_year;
            var DateSplitNew = data.split('/');
            curr_date = d.getDate();
            curr_month = d.getMonth() + 1
            curr_year = d.getFullYear();
            var formatedDate = curr_month + '/' + curr_date + '/' + curr_year;
            return formatedDate;
        }
    }
}


function formatAMPMUTC(date) {

    date = new Date(date);

    var hours = date.getUTCHours();
    var minutes = date.getUTCMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

function SetTimeOut(Id) {
    window.setTimeout(function () {
        $(".alert").slideUp(500, function () {
            $(this).hide();
        });
    }, 2000);
}

function ValidateRequiredFieldLogin(ctrl, ValidationMessage, ValidationPosition, defValue) {
    try {

        $(ctrl).closest('.form-group').removeClass('has-error');
        $(ctrl).closest('.input-group').removeClass('has-error');
        $(ctrl).closest('.input-group').prev('.help-block').remove();
        $(ctrl).closest('.input-group').next('.help-block').remove();
        if ($(ctrl).val() == '' || $(ctrl).val() == defValue) {
            $(ctrl).closest('.form-group').addClass('has-error');
            errorctrl = $(ctrl).closest(".input-group");
            PrintMessage(errorctrl, ValidationMessage, ValidationPosition);
            return false;
        }
        return true;

    } catch (e) {
        //alert(e);
    }
    return true;
}

function CheckPassword(ctrl, ValidationPosition) {
    if ($(ctrl).val() != '') {
        ClearMessage(ctrl);
        var regex = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,}$/;
        if (!regex.test($(ctrl).val())) {
            $(ctrl).closest('.form-group').addClass('has-error');
            //Password must contain least 1 Alphabet, 1 Number and 1 Special Character and Minimum 8 characters
            PrintMessage(ctrl, 'Password must contain at least 8 characters, including Number, Letters and Special Character.', ValidationPosition);
            return false;
        }
    }
    return true;
}

function ThisWeekStart_Enddate() {
    var curr = new Date; // get current date    
    var first = curr.getDate() - curr.getDay(); // First day is the day of the month - the day of the week
    var last = first + 6; // last day is the first day + 6
    var firstday = new Date(curr.setDate(first));
    var lastday = new Date(curr.setDate(last));
    if (firstday.getDate() > lastday.getDate()) {
        if (firstday.getMonth() == lastday.getMonth()) {
            var month = lastday.getMonth();
            lastday = new Date(curr.setMonth(month + 1))
        }
    }
    var dates = new Object();
    dates.firstday = firstday.toUTCString();
    dates.lastday = lastday.toUTCString();
    return dates;
}