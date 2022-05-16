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
                        location.href = "/Index";
                    }
                    else if (result === "重複登入") {
                        if (window.confirm("有使用者正在連線,要繼續登入嗎?")) {
                            location.href = "/Index"
                        }
                    }
                    else {
                        alert("登入失敗");
                    }
                },
                error: function (error) {
                    alert(error);
                }
            });
        }
    });
});