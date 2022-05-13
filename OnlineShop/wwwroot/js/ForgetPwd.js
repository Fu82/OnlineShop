$(document).ready(function (data) {
    $("#btnSendCode").click(function () {
        if ($("#txtAccount").val() == "") {
            alert("帳號必填");
        }

        else {
            $.ajax({
                url: "/api/Member/PostForgetPwd",
                type: "post",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "Account": $("#txtAccount").val()
                }),
                success: function (result) {
                    if (String.prototype.indexOf("帳號正確  ")) {
                        location.href = "/Member/VerifyMember";
                    } else {
                        alert(result);
                    }
                },
                error: function (error) {
                    alert(error);
                }
            });
        }
    });
});