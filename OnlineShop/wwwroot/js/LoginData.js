$(document).ready(function () {
    $("#btnLogout").click(function () {
        $.ajax({
            url: "api/Login/Logout",
            type: "Delete",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "Account": $("#Account").val(),
                "Pwd": $("#passWord").val()
            }),
            success: function (result) {
                location.href = "/Index";
            },
            error: function (error) {
            }
        })
    });
});