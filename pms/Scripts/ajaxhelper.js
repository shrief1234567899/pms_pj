function AjaxRequest(method, action, controller, data = null, handledata = null, viewobj = null) {
    $('.fa-spinner').show();

    var urlAction = '@Url.Action("action", "controller", new { id = "idvalue" })';
    urlAction = urlAction.replace("action", action);
    urlAction = urlAction.replace("controller", controller);
    urlAction = urlAction.replace("idvalue", data);

    $.ajax({
        type: method,
        url: urlAction,
        contentType: "application/json; charset=utf-8",
        data: data,
        dataType: "json",
        success: function (data) {
            OnSuccessCall(data, handledata, viewobj);
        },
        error: OnErrorCall
    });
}

function OnSuccessCall(response, handledata, viewobj) {
    /*response (status , data , displaySweetAlert , message)*/
    if (handledata) {
        handledata(response.data, viewobj)
    }
    if (response.displaySweetAlert) {
        swal("Good job!", response.message, "success")
    }
    if (response.status != "200") {
        swal("Error Occured!", response.message, "error")
    }
    $('.fa-spinner').hide();
}

function OnErrorCall(response) {
    /*response (status , data , displaySweetAlert , message)*/
    if (response.displaySweetAlert) {
        swal("Error Occured!", response.message, "error")
    }
    $('.fa-spinner').hide();
}