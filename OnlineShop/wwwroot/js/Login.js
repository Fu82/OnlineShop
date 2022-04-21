$(document).ready(function (data) {
    $("#btnPost").click(function () {


        //if(xxxxx)


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
        }).done(function (res) {
            $("#dvMsg").text(res);
        });


        //xxx


    });
});