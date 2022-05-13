$(document).ready(function (data) {
    $("#btnAddMember").click(function () {
        if ($("#txtAccount").val() == "" && $("#passWord").val() == "" && $("#txtPhone").val() == "" && $("#txtMail").val() == "") {
            alert("必填");
        }
        else if ($("#txtAccount").val() == "") {
            alert("帳號必填");
        }
        else if ($("#passWord").val() == "") {
            alert("密碼必填");
        }
        else if ($("#txtPhone").val() == "") {
            alert("手機必填");
        }
        else if ($("#txtMail").val() == "") {
            alert("信箱必填");
        }
        else
        {
            $.ajax({
                url: "/api/Member/AddAcc",
                type: "post",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "Account": $("#txtAccount").val(),
                    "Pwd": $("#passWord").val(),
                    "Phone": $("#txtPhone").val(),
                    "Mail": $("#txtMail").val()
                }),
                success: function (result) {
                    alert(result);
                    let some = "帳號新增成功  ";
                    if (some.indexOf("帳號新增成功  ")){
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