$(document).ready(function (data) {
    $("#btnCheckCode").click(function () {
        if ($("#txtAccount").val() == "" && $("#newPassWord").val() == "" && $("#checkPassWord").val() == "" && $("#txtCode").val() == "") {
            alert("必填");
        }

        else if ($("#txtAccount").val() == "") {
            alert("帳號必填");
        }

        else if ($("#newPassWord").val() == "") {
            alert("新密碼必填");
        }

        else if ($("#checkPassWord").val() == "") {
            alert("確認密碼必填");
        }

        else if ($("#txtCode").val() == "") {
            alert("驗證碼必填");
        }

        else {
            $.ajax({
                url: "/api/Member/VerifyForgetPwd",
                type: "put",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "f_acc": $("#txtAccount").val(),
                    "newPwd": $("#newPassWord").val(),
                    "cfmNewPwd": $("#checkPassWord").val(),
                    "Code": $("#txtCode").val()
                }),
                success: function (result) {
                    if (result == "密碼修改成功") {
                        alert(result);
                        location.href = "/Index";
                    }
                    else {
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