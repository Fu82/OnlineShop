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
                    /*imgTag +*/ "<a class='prodImg'>" + "<img id='join' src='" + data[i].f_img + "'/>" + "</a>" +
                    "<div class='prodName'>" +
                    "<h5 class='prodNick'>" + "<a>" +
                    "<span id='join' class='prodExtra'>" + data[i].f_name + "</span>" +
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

    $("#prodBody").on('click', '#join', function () {
        var currentRow1 = $(this).closest("div");
        var currentRow2 = $(this).closest("a");

        var col1 = currentRow2.find("a:eq(0)").text();
        var col2 = currentRow1.find("div:eq(0)").text();
        var col3 = currentRow1.find("div:eq(1)").text();

        if ($("#prodInside").css("display") == "none") {
            var InsideRow = "<div style='width: 100%;'>" +
                "<div style='width: 35%;float: left;'>" + "<div>" + "<img style='width: 480px;' src='" + col1 +"'/>" + "</div>" + "</div>" +
                "<div style='float: right;width: 55%;color: #666;height: 500px;margin: 50px;'>" +
                "<div style='margin-bottom: 20px;font: bold 20px / 27px Arial, Verdana, 'Yu Gothic', 'MS Gothic', 'Microsoft JhengHei', Helvetica, sans - serif;'>" + "<h5>" +
                "<span>" + col2 + "</span>" +
                "</h5>" + "</div>" +
                "<div>" +
                "<span>" + "主機板晶片組：B460 記憶體：16GB DDR4 = 原廠8G + 創見8G(需自行安裝) 硬碟：512GB M.2 SSD 光碟機：DVD writer 8X 螢幕輸出介面：D - Sub(VGA) ．HDMI．DP 電源供應: 500W power supply(80 + Bronze, peak 550W) 其他：Wi - Fi 5(802.11ac) + Bluetooth 4.2(Dual band) 1 * 1 作業系統：Windows 11 家用版(64 bit) 保固：三年保固 / 到府收送" + "</span>" +
                "</div>" +
                "<div>" +
                "<span>" + col3 + "</span>" +
                "</div>" + "</div>" +
                "</div>" +
                "<div>" +
                "<input type='Button' class='join' value='加入購物車' />" +
                "<input type='Button' value='加入喜好' />" +
                "</div>" +
                "<input type = 'Button'  onclick = 'InsideCancel_Click()' value = '返回' /></div > ";
            $('#prodInsideBody').html(InsideRow);
            $("#prodInside").show();
            $("#prodList").addClass("ListSwitch");
            $("#prodSearch").addClass("ListSwitch");
        }
    });
})

//返回
function InsideCancel_Click() {
    if ($("#prodInside").css("display") !== "none") {
        $("#prodInside").hide();
        $("#prodList").removeClass("ListSwitch");
        $("#prodSearch").removeClass("ListSwitch");
    }
}

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
    //排序下拉選單
    ProdListSort: function ProdListSort(SortSearch) {
        //extend  深複製暫存檔來操作;
        var tempTable = $.extend(true, [], ProdJson);

        //名稱
        if (SortSearch == "0" || SortSearch == "1") {
            tempTable.sort(function (a, b) {
                return a["f_name"].localeCompare(b["f_name"], "zh-hant"); //升序
            })
            if (SortSearch == "1") {
                tempTable.reverse(); //反序
            }

        }
        //價格
        else if (SortSearch == "2" || SortSearch == "3") {
            tempTable.sort(function (a, b) {
                return a["f_price"] - b["f_price"] //升序
            })
            if (SortSearch == "3") {
                tempTable.reverse(); //反序
            }

        }
        //熱門
        //else if (OrderClass == "4" || OrderClass == "5") {
        //    tempTable.sort(function (a, b) {
        //        return a["f_accPosition"].localeCompare(b["f_accPosition"], "zh-hant"); //升序
        //    })
        //    if (OrderClass == "5") {
        //        tempTable.reverse();//降序
        //    }

        //}
        //組HTML標籤
        ProdMenufun.DrawProductList(tempTable);

    },
    //搜尋
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
                tempTable = tempTable.filter(function (item) { //filter搜尋json
                    if (item[StrClassArr].indexOf(Search) >= 0) { //indexOf -> 有找到所鍵入文字則回傳 >=0
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
