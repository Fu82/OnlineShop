$(document).ready(function (data) {
    $("#btnCheckCode").click(function () {
        if ($("#txtAccount").val() == "" && $("#passWord").val() == "") {
            alert("帳號密碼必填");
        }

        else if ($("#txtAccount").val() == "") {
            alert("帳號必填");
        }

        else if ($("#passWord").val() == "") {
            alert("密碼必填");
        }

        else {
            $.ajax({
                url: "/api/Member/VerifyMember",
                type: "put",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "Account": $("#txtAccount").val(),
                    "Pwd": $("#passWord").val()
                }),
                success: function (result) {
                    if (result == "驗證成功") {
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