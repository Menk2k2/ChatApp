"use strict";

(function ($) {
    var formatter = new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    });

    $.fn.toBenPrice = function (d) {
        var s = formatter.format(d);
        this.html(s);
        this.removeClass();
        if (d < 1000000.0) {
            this.addClass("price-k");
        } else if (d < 10000000.0) {
            this.addClass("price-m");
        } else if (d < 30000000.0) {
            this.addClass("price-xm");
        } else {
            this.addClass("price-xxm");
        }
        return this;
    };

}(jQuery));

(function ($) {
    //Header trôi theo cuộn
    var wrapper = $("#headerPage");
    $(window).scroll(function () {
        if ($(window).scrollTop() > 160) {
            wrapper.addClass("fix-header");
            $("#fix-tabheader").addClass("fix-tabheader");
            $("#fillter-box .card-default").addClass("fixed");
        } else {
            wrapper.removeClass("fix-header");
            $("#fix-tabheader").removeClass("fix-tabheader");
            $("#fillter-box .card-default").removeClass("fixed");

        }

        if ($(window).scrollTop() > $(document).height() - 690 - $(window).height()) {
            if ($(window).width() >= 1200) {
                $("#fillter-box .card-default").removeClass("fixed");
                $("#fillter-box .card-default").css("position", "absolute");
                $("#fillter-box .card-default").css("bottom", "0px");
            }
        } else {
            if ($(window).width() >= 1200) {
                $("#fillter-box .card-default").attr("style", "");
                if ($(window).scrollTop() > 160) {
                    $("#fillter-box .card-default").addClass("fixed");
                } else {
                    wrapper.removeClass("fix-header");
                    $("#fillter-box .card-default").removeClass("fixed");
                }
            }
        }
    });
    //Animate khi cuộn trang
    var contentWayPoint = function () {
        var i = 0;
        var page = 1;
        $('.ben-animate').waypoint(function (direction) {
            if (direction === 'down' && $(this.element).hasClass('ben-animate')) {
                i++;
                $(this.element).addClass('item-animate');
                setTimeout(function () {
                    $('body .ben-animate.item-animate').each(function (k) {
                        var el = $(this);
                        setTimeout(function () {
                            var effect = el.data('animate-effect');
                            if (effect === 'fadeIn') {
                                el.addClass('fadeIn ben-animated');
                            } else if (effect === 'fadeInLeft') {
                                el.addClass('fadeInLeft ben-animated');
                            } else if (effect === 'fadeInRight') {
                                el.addClass('fadeInRight ben-animated');
                            } else {
                                el.addClass('fadeInUp ben-animated');
                            }
                            el.removeClass('item-animate');
                        }, k * 50, 'easeInOutExpo');
                    });

                }, 100);

            }
            if (direction === 'down' && $(this.element).hasClass('ben-loading')) {
                //scroll for list sp
                page++;
                console.log('Load page: ' + page);

            }
        }, { offset: '95%' });
    };
    contentWayPoint();
    //Một số chuyển đổi cho Responsive
    var isPcMode = $(window).width() >= 1200;
    if (isPcMode) {
        $("#fillter-box").append($("#fillterWindow"));
        //Detail page:
        //$("#forDetails").append($("#tab-details").addClass("col-8"));
        //$("#forDetails").append($("#tab-ThuocTinh").addClass(" col-4 border-left"));

        //$("#forRating").append($("#tabRating").removeClass("tab-pane"));
        //$("#forQandAns").append($("#tabQandAns").removeClass("tab-pane"));
    } else {
        $("#fillter-mobile-box").append($("#fillterWindow"));
        //Home
        $(".scroll-mobile-container").each(function() { $(this).css("width", `calc(40vw * ${$(this).data("items")} + 1px)`); });
        $(".slide-mobile .row .col-ben-2").remove();
        $(".slide-mobile .row").owlCarousel({
            autoHeight: true,
            autoplay: false,
            loop: true,
            margin: 0,
            center: true,
            animateOut: "slideOutLeft",
            animateIn: "slideInRight",
            nav: false,
            dots: false,
            autoplayHoverPause: false,
            navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
            responsive:{
                0 : { items : 2},
                768:{ items:3 },
                992:{ items:4 },
                1200:{ items:5 }
            }
        });
        //Detail
        $("#tabTongQuan").prepend($("#detailTongQuan"));
        $("#subtabChiTiet").append($("#detailChiTiet"));
        $("#subtabThongSo").append($("#detailThongSo"));
        $("#tabDanhGia").append($("#detailRating"));
        //Shopconfirm:
        $("#shopConfirmContainer").append($("#shopCart"));
        $("#shopConfirmContainer").append($("#shopPayMethod"));

        //footer:
        $("footer ul").addClass("collapse");
        $("footer h5").addClass("collapsed");
    }
    $(window).resize(function () {
        var width = $(window).outerWidth();
        //console.log(width, $(window).outerWidth());
        if (width < 1200) {
            if (isPcMode) { //nhẩy mobile mode:
                $("#fillter-mobile-box").append($("#fillterWindow"));
                isPcMode = false;
                $("#tabTongQuan").prepend($("#detailTongQuan"));
                $("#subtabChiTiet").append($("#detailChiTiet"));
                $("#subtabThongSo").append($("#detailThongSo"));
                $("#tabDanhGia").append($("#detailRating"));
                //Shopconfirm:
                $("#shopConfirmContainer").append($("#shopCart"));
                $("#shopConfirmContainer").append($("#shopPayMethod"));
                $("footer ul").addClass("collapse");
            }
        } else {
            if (!isPcMode) {
                $("#fillter-box").append($("#fillterWindow"));
                isPcMode = true;
                $("#detailBlocks").append($("#detailTongQuan"));
                $("#detailBlocks").append($("#sliderSPTuongTu"));
                $("#tabDetails .col-8").append($("#detailChiTiet"));
                $("#tabDetails .col-4").append($("#detailThongSo"));
                $("#detailBlocks").append($("#tabDetails"));
                $("#detailBlocks").append($("#detailRating"));
                $("#detailBlocks").append($("#detailQandAns"));
                //Shopconfirm:
                $("#shopConfirmContainer").append($("#shopPayMethod"));
                $("#shopConfirmContainer").append($("#shopCart"));
                $("footer ul").attr("class", "");
            }
        }
    });
    window.dispatchEvent(new Event('resize'));
    //Tạo menu của hộp searbox
    $(".main-menu").smartmenus({
        mainMenuSubOffsetX: 0,
        mainMenuSubOffsetY: 0,
        subMenusSubOffsetX: 0,
        subMenusSubOffsetY: 0
    });
    
    //$("#popupMainMenu").click(function () {
    //    $('#popup-nav').css('position', 'fixed').css('left', $(this).offset().left).css('top', '54px').addClass("x-menu").collapse('toggle');
    //});

    //$("#btnDanhmuc").click(function () {
    //    $('#popup-nav').css('position', 'absolute').css('left', '0').css('top', '100%').removeClass("x-menu").collapse('toggle');
    //});
    $(".main-menu").on("beforeshow.smapi", function(e, item) {
        const pop = $(item);
        if (pop.parent("li").hasClass("block-title")) {
            return false;
        }
        return true;
    });
    $(".main-menu").on("beforehide.smapi", function(e, item) {
        const pop = $(item);
        if (pop.parent("li").hasClass("block-title")) {
            return false;
        }
        return true;
    });
    var laspopup = null;
    $(".main-menu").on("show.smapi", function(e, menu) {
        const block = $(menu);
        const rootm = $(menu).parents("nav");
        if (block.hasClass("block-menu") && rootm.hasClass("x-menu")) {
            $(menu).parent().css("position", "static");
            const height = rootm.height();
            block.css("height", height + "px");
            //for popup child:
            block.find("a").each(function(l) {
                 var link = $(this);
                 var pop = link.next("ul").first();
                 if (pop.is("ul")) {
                     link.addClass("has-submenu").append('<span class="sub-arrow"></span>');
                     link.mouseenter(function() {
                         if (laspopup != null) {
                             laspopup.css("display", "none");
                         }
                         pop.css("display", "block").attr("class", "block-menu-popup");
                         laspopup = pop;
                     });
                     pop.mouseleave(function() {
                         pop.css("display", "none");
                     });
                     
                     //link.css("color", "#009900");
                     //pop.css("background", "#99ddff");
                 }
            });
            
        }
    });
    //$('.main-menu').on('click.smapi', function(e, item) {
    //    // check namespace if you need to differentiate from a regular DOM event fired inside the menu tree
    //    console.log($(e.target).is('.sub-arrow'));
    //    if (e.namespace === 'smapi') {
    //        // your handler code
    //        console.log(e, item);
    //    }
    //});

    function timkiem(s) {
        var url = $("#btnTimKiem").data("url");
        var seo = $("#btnTimKiem").data("cat");
        if (seo == '') {

        }
    }
    $("#btnTimKiem").click(function (e) {
        const cat = $("#btnDanhmuc").data("cat");
        var href = $(this).data("url") + "?search=" + $("#search-sanpham-pc").val().toLowerCase();
        console.log(href);
        location.href = href;
        //location.href = $(this).data("url") + (cat.length === 0 ? "" : `/${cat}`) + "?search=" + $("#search-sanpham-pc").val().toLowerCase();
    });
    $("#btnTimKiemMobile").click(function (e) {
        const cat = $("#btnDanhmuc").data("cat");
        var href = $("#btnTimKiem").data("url") + "?search=" + $("#search-sanpham-mobile").val().toLowerCase();
        console.log(href);
        location.href = href;
        //location.href = $("#btnTimKiem").data("url") + (cat.length === 0 ? "" : `/${cat}`) +  "?search=" + $("#search-sanpham-mobile").val().toLowerCase();
    });
    $("#search-sanpham-pc").keypress(function (event) {
        if (event.keyCode === 13) {
            $("#btnTimKiem").click();
        }
    });
    $("#search-sanpham-mobile").on("onsearch", function (event) {
        $("#btnTimKiem").click();
    });
    //Close popup menu
    $("body").click(function (e) {
        //if (e.target.id === "btnDanhmuc" || e.target.id === "popupMainMenu") return;
        if (e.target.id !== "popupMainMenu" && $("#popup-nav").hasClass("show")) {
            $("#popup-nav").removeClass("show");
            $("#popupMainMenu").addClass("collapsed");
        }

        if (e.target.id !== "btnDanhmuc" && $("#popup-nav-search").hasClass("show")) {
            $("#popup-nav-search").removeClass("show");
            $("#btnDanhmuc").addClass("collapsed");
        }
    });
    $.scrollUp({ scrollText: "", zIndex: 15 });

    $(".search-suggest").each(function () {
        $(this).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "/Products/SearchSuggest/",
                    dataType: "json",
                    cache: false,
                    data: {
                        term: request.term,
                        cat: $("#catSearch").val()
                    },
                    success: function (data) {
                        response(data.result);
                    }
                });
            },
            scroll: true,
            minLength: 2,
            open: function () {
                if (!isPcMode) {
                    $(this).autocomplete("widget").css("width", "100%");
                    $(this).autocomplete("widget").css("left", "0");
                }

            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            console.log(item);
            return $(`<li>${item["value"]}</li>`).appendTo(ul);
        };
    });

    function delay(callback, ms) {
        var timer = 0;
        return function () {
            var context = this, args = arguments;
            clearTimeout(timer);
            timer = setTimeout(function () {
                callback.apply(context, args);
            }, ms || 0);
        };
    };
    
    function onSearchSPPC() {
        $(".sanpham-pc").empty();
        $(".suggest-pc").empty();

        console.log('term: ' + $(".search-sanpham-pc").val());
        console.log('catSearch: ' + $("#catSearch").val());
      
        if ($(".search-sanpham-pc").val().length > 1) {
            $(".searchdiv").css("display", "block");
            $.ajax({
                url: "/Products/SearchSP/",
                dataType: "json",
                cache: false,
                data: {
                    term: $(".search-sanpham-pc").val(),
                    cat: $("#catSearch").val()
                },
                success: function (data) {

                    if (data != null && data.result != null) {
                        console.log(data.result);
                        console.log(data.result.length);
                        if (data.result.length > 0) {
                            for (var i = 0; i < data.result.length; i++) {
                                const link = `${data.result[i]["Link"]}`;
                                console.log(link);
                                $(`<li onclick="window.location='${link}'" style="width:100%;cursor: pointer;"> `).addClass("liSP")
                                    .append(`<div class="media">
                                    <img src="${data.result[i]["Image"]}" alt="" class="img-size-50 mr-3">
                                    <div class="media-body">
                                        <div class="dropdown-item-title text-muted">${data.result[i]["Name"]} <br />${data.result[i]["Price"]}</div>
                                    </div>
                                    </div>`)
                                    .appendTo($(".sanpham-pc"));
                            }
                        } else {
                            $(".sanpham-pc").empty();
                            $("<p>Không tìm thấy sản phẩm</p>").appendTo($(".sanpham-pc"));
                            console.log($(".sanpham-pc"));
                        }
                        $(".kwspan").on("click", function () {
                            $(".search-sanpham-pc").val($(this).attr("keyword")).trigger("keyup");
                        });

                    }
                }
            });
            $.ajax({
                url: "/Products/SearchSuggest/",
                dataType: "json",
                cache: false,
                data: {
                    term: $(".search-sanpham-pc").val(),
                    cat: $("#catSearch").val()
                },
                success: function (data) {
                    $(".searchdiv").css("display", "block");
                    //console.log(data.result);
                    if (data != null && data.result != null) {
                        if (data.result.length > 0) {
                            for (var i = 0; i < data.result.length; i++) {
                                console.log()
                                $(`<span class='kwspan' keyword="${data.result[i]}");'>${data.result[i]}</span><br/>`)
                                    .appendTo($(".suggest-pc"));
                            }
                        } else {
                            $(".suggest-pc").empty();
                        }
                    }
                    $(".kwspan").on("click", function () {
                        $(".search-sanpham-pc").val($(this).attr("keyword")).trigger("keyup");
                    });
                }
            });
        } else {
            $(".searchdiv").css("display", "none");
        }
    }

    function onSearchSPMobile() {
        $(".sanpham-mobile").empty();
        $(".suggest-mobile").empty();
      
        if ($(".search-sanpham-mobile").val().length > 1) {
            $(".searchdiv").css("display", "block");
            $.ajax({
                url: "/Products/SearchSP/",
                dataType: "json",
                cache: false,
                data: {
                    term: $(".search-sanpham-mobile").val(),
                    cat: $("#catSearch").val()
                },
                success: function (data) {

                    if (data != null && data.result != null) {
                        console.log(data.result);
                        console.log(data.result.length);
                        if (data.result.length > 0) {
                            for (var i = 0; i < data.result.length; i++) {
                                const link = `${data.result[i]["Link"]}`;
                                console.log(link);
                                $(`<li class='searchLink' onclick="window.location='${link}'" style="width:100%;cursor: pointer;"> `).append(`<div class="media">
                                    <img src="${data.result[i]["Image"]}" alt="" class="img-size-50">
                                    <div class="media-body">
                                        <div class="dropdown-item-title text-muted">${data.result[i]["Name"]} <br /> ${data.result[i]["Price"]}</div>
                                    </div>
                                    </div>`).appendTo($(".sanpham-mobile"));
                            }
                        } else {
                            $(".sanpham-mobile").empty();
                            $("<p>Không tìm thấy sản phẩm</p>").appendTo($(".sanpham-mobile"));
                            console.log($(".sanpham-mobile"));
                        }
                        $(".kwspan-mobile").on("click", function () {
                            $(".search-sanpham-mobile").val($(this).attr("keyword")).trigger("keyup");
                        });

                    }
                }
            });
            $.ajax({
                url: "/Products/SearchSuggest/",
                dataType: "json",
                cache: false,
                data: {
                    term: $(".search-sanpham-mobile").val(),
                    cat: $("#catSearch").val()
                },
                success: function (data) {
                    $(".searchdiv").css("display", "block");
                    console.log(data.result);
                    if (data != null && data.result != null) {
                        if (data.result.length > 0) {
                            for (var i = 0; i < data.result.length; i++) {
                                console.log()
                                $(`<span class='kwspan-mobile' keyword="${data.result[i]}");'>${data.result[i]}</span><br/>`)
                                    .appendTo($(".suggest-mobile"));
                            }
                        } else {
                            $(".suggest-mobile").empty();
                        }
                    }
                    $(".kwspan-mobile").on("click", function () {
                        $(".search-sanpham-mobile").val($(this).attr("keyword")).trigger("change");
                    });
                }
            });
        } else {
            $(".searchdiv").css("display", "none");
        }
    }

    $(".search-sanpham-pc").on("keyup", delay(function (e) {
        onSearchSPPC();
    }, 500));

    $(".search-sanpham-mobile").on("keyup", delay(function () {
        onSearchSPMobile();
    }, 500));

    //$(".search-sanpham-pc").on("change", function () {
    //    onSearchSPPC();
    //});    

    //Search autocomplete sản phẩm
    $(".search-sanpham").each(function () {    

        $(this).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "/Products/SearchSP/",
                    dataType: "json",
                    cache: false,
                    data: {
                        term: request.term,
                        cat: $("#catSearch").val()
                    },
                    success: function (data) {
                        response(data.result);
                        $(".search-suggest").val(request.term);
                        $(".search-suggest").each(function () {
                            $(this).autocomplete("search");
                        });
                    }
                });
            },
            scroll: true,
            minLength: 2,
            open: function () {
                if (!isPcMode) {
                    $(this).autocomplete("widget").css("width", "100%");
                    $(this).autocomplete("widget").css("left", "0");
                }

            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            ul.addClass("ui-autocomplete-sp");
            const link = `${item["Link"]}`;
            console.log(link);
            return $(`<li onclick="window.location='${link}'" style="width:100%">`)
                .append(`<div class="media">
                            <img src="${item["Image"]}" alt="" class="img-size-50 mr-3">
                            <div class="media-body">
                                <div class="dropdown-item-title text-muted">${item["Name"]}</div>
                                <div>${item["Price"]}</div>
                            </div>
                         </div>`)
                .appendTo(ul);
        };
    });
    //Subcrible email
    $("#btnSubscribeEmail").on("click", function (event) {
        $.ajax({
            url: "/Home/SubmitSubscribeEmail/",
            dataType: "json",
            type: "post",
            cache: false,
            data: {
                email: $("#subscribeEmail").val()
            },
            success: function (data) {
                if (data === "OK") {
                    alert("Quý khách đã đăng ký thành công!");
                } else if (data === "Exist") {
                    alert("Email của quý khách đã tồn tại.");
                }
            },
            error: function (err) {
                alert("Đã có lỗi xảy ra, xin vui lòng thử lại.")
            }
        });
    });



})(jQuery);


