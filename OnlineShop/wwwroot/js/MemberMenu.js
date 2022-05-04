$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/api/member/getmember?id=" + 18,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var rows = rows + "<tr>" +
                "<td id='Name'>" + data[0].f_name + "</td>" +
                "<td id='address'>" + data[0].f_address + "</td>" +
                "<td id='shopGold'>" + data[0].f_shopGold + "</td>" +
                "<td align='center'> <input type='button' class='EditBtn'  name='EditBtn'   id = '" + data[0].f_id + "'  value='編輯'/ ></td>" +
                "</tr>";
            $('#Table').append(rows);
        },

        failure: function (data) {
        },
        error: function (data) {
        }

    });
})