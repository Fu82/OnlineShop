$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/api/Product/GetProduct",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            ProdJson = data;

            var rows = "";
            /*var imgTag = ""*/
            for (var i in data) {
                //判断图片是否存在(還須修正)
                //if (data[i].f_img === "") {
                //    imgTag = "<a class='prodImg'>" + "無圖片" + "</a>"
                //} else {
                //    imgTag = "<a class='prodImg'>" + "<img src='" + data[i].f_img + "'/>" + "</a>"
                //}
                rows += "<div>" +
                    /*imgTag +*/ "<a class='prodImg'>" + "<img src='" + data[i].f_img + "'/>" + "</a>" +
                    "<div class='prodName'>" +
                    "<h5 class='prodNick'>" + "<a>" +
                    "<span class='prodExtra'>" + data[i].f_name + "</span>" +
                    "</a>" + "</h5>" + "</div>" +
                    "<div class='prodPrice'>" +
                    "<span class='prodDollar'>" + "$" + "<span class='prodValue'>" + data[i].f_price + "</span>" + "</span>" +
                    "</div>" +
                    "<div class='prodJoin'>" +
                    "<input type='Button' value='加入購物車' />" +
                    "<input type='Button' value='加入喜好' />" +
                    "</div>" +
                    "</div>";
            }
            $('#prodBody').html(rows);
        },

        failure: function (data) {
        },
        error: function (data) {
        }
    });
})

var ProdMenufun = {
    DrawProductList: function (prodArray) {
        var htmlText = "";

        for (var i = 0; i < prodArray.length; i++) {
            htmlText += "<div>" +
                "<a class='prodImg'>" + "<img src='" + prodArray[i].f_img + "'/>" + "</a>" +
                "<div class='prodName'>" +
                "<h5 class='prodNick'>" + "<a>" +
                "<span class='prodExtra'>" + prodArray[i].f_name + "</span>" +
                "</a>" + "</h5>" + "</div>" +
                "<div class='prodPrice'>" +
                "<span class='prodDollar'>" + "$" + "<span class='prodValue'>" + prodArray[i].f_price + "</span>" + "</span>" +
                "</div>" +
                "<div class='prodJoin'>" +
                "<input type='Button' value='加入購物車' />" +
                "<input type='Button' value='加入喜好' />" +
                "</div>" +
                "</div>";
        }

        $('#prodBody').html(htmlText);
    },
    ProdListSearch: function () {
        var tempTable = $.extend(true, [], ProdJson);
        var StrClassArr = "f_name"; //商品名稱為搜尋對象
        var searchvalue = $("#Search").val(); //輸入值

        if (searchvalue === "") {

            //組HTML,覆蓋
            ProdMenufun.DrawProductList(ProdJson);

        } else {

            //字串搜尋
            var searchStr = function (Search) {
                tempTable = tempTable.filter(function (item) {//filter搜尋json
                    if (item[StrClassArr].indexOf(Search) >= 0) {//indexOf -> 有找到所鍵入文字則回傳 >=0
                        return item //大於等於0則 return item
                    }
                })
            }

            //字串搜尋
            searchStr($("#Search").val());

            //組HTML,覆蓋
            ProdMenufun.DrawProductList(tempTable);

        }
    }
};
