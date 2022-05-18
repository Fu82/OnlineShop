$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/api/Product/GetProduct",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            for (var i in data) {
                var rows = rows + "<dd>" +
                    "<a class='prodImg'>" + data[i].f_img + "</a>" +
                    "<div class='prodInfo'>" +
                    "<h5 class='nick'>" + "<a>" +
                    "<span class='extra'>" + data[i].f_num + "</span>" +
                    "</a>" + "</h5>" + "</div>" +
                    "<ul class='priceBox'>" + "<li>" +
                    "<span class='priceDollar'>" + "$" + "<span class='priceValue'>" + data[i].f_price + "</span>" + "</span>" +
                    "</li>" + "</ul>" +
                    "<div class='prodJoin'>" +
                    "<input type='Button' value='加入購物車' />" +
                    "<input type='Button' value='加入喜好' />" +
                    "</div>" +
                    "</dd>";
            }
            $('#dlBody').append(rows);
        },

        failure: function (data) {
        },
        error: function (data) {
        }
    });
})
