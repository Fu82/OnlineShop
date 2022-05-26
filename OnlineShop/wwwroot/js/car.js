$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/api/Product/GetCar",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            CarJson = data;

            var rows = "";
            for (var i in data) {
                rows += "<tr>" +
                    "<td>" + data[i].f_id + "</td>" +
                    "<td>" + "<img style='width: 120px;' src='" + data[i].f_img + "' onerror='noImg()' />" + "</td>" +
                    "<td>" + data[i].f_name + "</td>" +
                    "<td>" + data[i].f_price + "</td>" +
                    "<td>" + data[i].f_stock + "</td>" +
                    "</tr>";
            }
            $('#carTableBody').html(rows);
        },

        failure: function (data) {
        },
        error: function (data) {
        }
    });
})