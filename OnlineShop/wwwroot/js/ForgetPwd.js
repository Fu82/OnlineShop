$(document).ready(function (data) {
    $("#btnSendCode").click(function () {
        if ($("#txtAccount").val() == "") {
            alert("帳號必填");
        }

        else {
            $.ajax({
                url: "/api/Member/GetForgetPwd",
                type: "get",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "Account": $("#txtAccount").val()
                }),
                success: function (result) {
                    alert(result);
                },
                error: function (error) {
                    alert(error);
                }
            });
        }
    });
});