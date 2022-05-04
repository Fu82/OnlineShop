$(document).ready(function (data) {
    $("#btnPost").click(function () {
        if ($("#txtAccount").val() == "" && $("#passWord").val() == "")
        {
            alert("帳號密碼必填");
        }

        else if ($("#txtAccount").val() == "")
        {
            alert("帳號必填");
        }

        else if ($("#passWord").val() == "")
        {
            alert("密碼必填");
        }

        else
        {
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
                    if (result == "登入成功") {
                        location.href = "/LoginData";
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