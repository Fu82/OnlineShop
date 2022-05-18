$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/api/Member/GetMember",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var rows = rows + "<tr>" +
                "<td name='fid'>" + data[0].f_id + "</td>" +
                "<td name='fname' style='width: 20%;'>" + data[0].f_name + "</td>" +
                "<td name='faddress'>" + data[0].f_address + "</td>" +
                "<td name='fshopGold' style='width: 20%;'>" + data[0].f_shopGold + "</td>" +
                "<td align='center' style='width: 15%;'> <input type='button' class='EditAccBtn'  name='EditAccBtn'  value='編輯資料'/ ></td>" +
                "</tr>";
            $('#TableBody').append(rows);
        },

        failure: function (data) {
        },
        error: function (data) {
        }
    });

    //點擊編輯帳號按鈕
    $("#TableBody").on('click', '.EditAccBtn', function () {
        var currentRow = $(this).closest("tr");

        var col1 = currentRow.find("td:eq(0)").text();
        var col2 = currentRow.find("td:eq(1)").text();
        var col3 = currentRow.find("td:eq(2)").text();

        if ($("#EditBox").css("display") == "none") {
            var EditData =
                "<h5>編輯資料</h5>" +
                "<div><label> 姓名：</label><label id='Editfname'>" + col2 + "</label></div>" +
                "<div><label> 地址：</label><label id='Editfaddress'>" + col3 + "</label></div>" +
                "<div><label> 更新姓名：</label><input id='txtName' style='width: 10%;' maxlength='4' /></div>" +
                "<div><label> 更新地址：</label><input id='txtAddress' style='width: 40%;' /></div>" +
                "<div id='Editbutton'><input name='EditAcc' onclick ='EditAcc_Click(" + col1 + ")' type='Button' value='確認編輯' />" +
                "<input name='EditCancel' id = 'EditCancel' type = 'Button'  onclick = 'EditCancel_Click()' value = '取消編輯' /></div > "
            $('#Editform').append(EditData);
            $("#EditBox").show();
        }
    });
})

//確認編輯帳號
function EditAcc_Click(f_id) {
    $.ajax({
        url: "/api/Member/PutMember?id=" + f_id,
        type: "put",
        contentType: "application/json",
        dataType: "text",
        data: JSON.stringify({
            "Name": $("#txtName").val(),
            "Address": $("#txtAddress").val()
        }),
        success: function (result) {
            alert(result)

            if (result == "帳號更新成功") {
                location.reload(); //新增成功才更新頁面
            }
        },
        error: function (error) {
            alert(error);
        }
    })
}
//取消編輯
function EditCancel_Click() {
    if ($("#EditBox").css("display") !== "none") {
        $("#EditBox").hide();
        $("#Editform > div,h5").remove();
    }
}