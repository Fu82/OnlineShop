$(document).ready(function (data) {
    $("#btnPost").click(function () {
        $.ajax({
            url: "/api/Login",
            type: "post",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "Account": $("#txtAccount").val(),
                "Pwd": $("#passWord").val()
            }),
            success: function (result) {
                alert(result)
            },
            error: function (error) {
                alert(error);
            }
        });
    });
});