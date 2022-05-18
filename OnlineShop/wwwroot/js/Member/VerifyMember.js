$(document).ready(function (data) {
    $("#btnCheckCode").click(function () {
        if ($("#txtAccount").val() == "" && $("#passWord").val() == "" && $("#txtCode").val() == "") {
            alert("必填");
        }

        else if ($("#txtAccount").val() == "") {
            alert("帳號必填");
        }

        else if ($("#passWord").val() == "") {
            alert("密碼必填");
        }

        else if ($("#txtCode").val() == "") {
            alert("驗證碼必填");
        }

        else {
            $.ajax({
                url: "/api/Member/VerifyMember",
                type: "put",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "Account": $("#txtAccount").val(),
                    "Pwd": $("#passWord").val(),
                    "Code":  $("#txtCode").val()
                }),
                success: function (result) {
                    if (result == "驗證成功") {
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