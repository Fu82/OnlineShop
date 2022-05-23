$(document).ready(function () {
    //取得類型列表
    $.ajax({
        type: "GET",
        url: "/api/Product/GetCategory",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            Category10 = AddProductFun.MakeBoxHtml(data.filter(function (item) {
                if (item["f_categoryNum"].indexOf(10) >= 0) {
                    return item
                }
            }));
            Category20 = AddProductFun.MakeBoxHtml(data.filter(function (item) {
                if (item["f_categoryNum"].indexOf(20) >= 0) {
                    return item
                }
            }));
            Category30 = AddProductFun.MakeBoxHtml(data.filter(function (item) {
                if (item["f_categoryNum"].indexOf(30) >= 0) {
                    return item
                }
            }));
        },
        failure: function (data) {
            alert(data);
        },
        error: function (data) {
            alert(data);
        }
    });
    //商品清單
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
                rows += "<div>" +
                    "<a class='prodImg'>" + "<img id='join' src='" + data[i].f_img + "' onerror='noImg()' />" + "</a>" + 
                    "<div class='prodName'>" +
                    "<h5 class='prodNick'>" + "<a>" +
                    "<span class='prodExtra'>" + data[i].f_name + "</span>" +
                    "</a>" + "</h5>" + "</div>" +
                    "<div class='prodPrice prodDollar'>" +
                    "$" + "<span class='prodValue'>" + data[i].f_price + "</span>" +
                    "</div>" +
                    "<div style='display:none'>" + data[i].f_content + "</div>" + //商品內容隱藏
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
    //組合商品內頁
    $("#prodBody").on('click', '#join', function () {
        var ProdRow = $(this).closest("div");

        var ProdImg = ProdRow.find("a > img").attr('src');
        var ProdName = ProdRow.find("div:eq(0)").text();
        var ProdPrice = ProdRow.find("div > span").text();
        var ProdContent = ProdRow.find("div:eq(2)").text();

        if ($("#prodInside").css("display") == "none") {
            var InsideRow = "<div style='width: 100%;'>" +
                "<div style='width: 35%;float: left;'>" + "<div>" + "<img style='width: 400px;' src='" + ProdImg +"'/>" + "</div>" + "</div>" +
                "<div style='float: right;width: 55%;color: #666;height: 400px;margin: 50px;'>" +
                "<div style='margin-bottom: 20px;'>" + "<h5>" +
                "<span>" + ProdName + "</span>" +
                "</h5>" + "</div>" +
                "<div>" +
                "<span>" + ProdContent + "</span>" +
                "</div>" +
                "<div>" + "$" +
                "<span>" + ProdPrice + "</span>" +
                "</div>" + "</div>" +
                "</div>" +
                "<div>" +
                "<input type='Button' style='width:50%' value='加入購物車' />" +
                "<input type='Button' style='width:50%' value='加入喜好' />" +
                "</div>" +
                "<input type = 'Button'  onclick = 'InsideCancel_Click()' style='width:100%;color: #fff;background-color: #007bff;border-color: #007bff;margin-top: 30px;' value = '返回' />";
            $('#prodInsideBody').html(InsideRow);
            $("#prodInside").show();
            $("#prodList").addClass("ListSwitch");
            $("#prodSearch").addClass("ListSwitch");
        }
    });

    $("#Category10").click(function () {
        $('#SubCategory').html(Category10).toggle();
    });
    $("#Category20").click(function () {
        $('#SubCategory').html(Category20).toggle();
    });
    $("#Category30").click(function () {
        $('#SubCategory').html(Category30).toggle();
    });
})

//返回
function InsideCancel_Click() {
    if ($("#prodInside").css("display") !== "none") {
        $("#prodInside").hide();
        $("#prodList").removeClass("ListSwitch");
        $("#prodSearch").removeClass("ListSwitch");
        location.reload();
    }
}

//圖片不存在
function noImg() {
    var img = event.srcElement;
    img.src = "images/product/noImg.png";
    img.onerror = null;
}

var ProdMenufun = {
    //清單重組
    DrawProductList: function (prodArray) {
        var htmlText = "";

        for (var i = 0; i < prodArray.length; i++) {
            htmlText += "<div>" +
                "<a class='prodImg'>" + "<img id='join' src='" + prodArray[i].f_img + "' onerror='noImg()' />" + "</a>" +
                "<div class='prodName'>" +
                "<h5 class='prodNick'>" + "<a>" +
                "<span class='prodExtra'>" + prodArray[i].f_name + "</span>" +
                "</a>" + "</h5>" + "</div>" +
                "<div class='prodPrice prodDollar'>" +
                "$" + "<span class='prodValue'>" + prodArray[i].f_price + "</span>" +
                "</div>" +
                "<div style='display:none'>" + prodArray[i].f_content + "</div>" + //商品內容隱藏
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
        var searchvalue = $("#Search").val(); //輸入值

        if (searchvalue === "") {
            //組HTML,覆蓋
            ProdMenufun.DrawProductList(ProdJson);

        } else {
            //字串搜尋
            var searchvalue = function (Search) {
                tempTable = tempTable.filter(function (item) { //filter搜尋json
                    if (item["f_name"].indexOf(Search) >= 0) { //indexOf -> 有找到所鍵入文字則回傳 >=0
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

var AddProductFun = {
    //清單重組
    TopProductList: function (prodArray) {
        var htmlText = "";

        for (var i = 0; i < prodArray.length; i++) {
            htmlText += "<div>" +
                "<a class='prodImg'>" + "<img id='join' src='" + prodArray[i].f_img + "' onerror='noImg()' />" + "</a>" +
                "<div class='prodName'>" +
                "<h5 class='prodNick'>" + "<a>" +
                "<span class='prodExtra'>" + prodArray[i].f_name + "</span>" +
                "</a>" + "</h5>" + "</div>" +
                "<div class='prodPrice prodDollar'>" +
                "$" + "<span class='prodValue'>" + prodArray[i].f_price + "</span>" +
                "</div>" +
                "<div style='display:none'>" + prodArray[i].f_content + "</div>" + //商品內容隱藏
                "<div class='prodJoin'>" +
                "<input type='Button' value='加入購物車' />" +
                "<input type='Button' value='加入喜好' />" +
                "</div>" +
                "</div>";
        }

        $('#prodBody').html(htmlText);
    },
    //組子類別標籤
    MakeBoxHtml: function (CategoryJson) {
        var Rows = '';
        for (var i = 0; i < CategoryJson.length; i++) {
            Rows += "<li role='" + CategoryJson[i].f_categoryNum + "' value='" + CategoryJson[i].f_subCategoryNum + "' onclick='AddProductFun.TopProdList()'>" + CategoryJson[i].f_subCategoryName + "</li>";
        }
        return Rows
    },
    //
    TopProdList: function TopProdList() {
        var va = $(this).attr('value');
        return role;

        //extend  深複製暫存檔來操作;
        var tempTable = $.extend(true, [], ProdJson);

        return tempTable;

        //組HTML標籤
        /*AddProductFun.TopProdList(tempTable);*/

    }


    //map深複製 第一名
    //var array = [1, 2, 3, 4, 5];
    //var array_2 = array.map(function (item) {
    //    return { a: item, b: item.toString() };
    //});
    //var tempTable = $.extend(true, [], CategoryJson);//複製到暫存 //第二名
    //var tempTable = $.assign([], CategoryJson);//assign深複製  第三名
};
