"use strict";
(function ($) {
	$(window).stellar({
		responsive: true,
		parallaxBackgrounds: true,
		parallaxElements: true,
		horizontalScrolling: false,
		hideDistantElements: false,
		scrollProperty: 'scroll'
	});

	const carousel = function () {
		$(".slide-bar.owl-carousel").owlCarousel({
            autoHeight: true,
            loop: true,
            autoplay: true,
            margin: 0,
            animateOut: "fadeOut",
            animateIn: "fadeIn",
            nav: true,
            dots: false,
            autoplayHoverPause: false,
            items: 1,
			navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
            responsive: {
                0: {
                    items: 2,
                    center: false
                },
                1200: {
                    items: 3,
                    center: false
                }
            }
        });
        $(".list-products.owl-carousel").owlCarousel({
            autoHeight : true,
            loop : true,
            autoplay : true,
            margin : 0,
            center : true,
            animateOut : "fadeOut",
            animateIn : "fadeIn",
            nav : true,
            dots : false,
            autoplayHoverPause : false,
            items : 1,
            navText : [ "<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>" ],
            responsive : {
                0 : {
                    items : 2,
                    center : true
                },
                768 : {
                    items : 3,
                    center : false
                },
                992 : {
                    items : 4,
                    center : true
                },
                1200 : {
                    items : 5,
                    center : false
                }
            }
        });
        $(".mobile-banner").owlCarousel({
            autoHeight: true,
            autoplay: true,
            margin: 0,
            center: true,
            animateOut: "slideOutLeft",
            animateIn: "slideInRight",
            nav: false,
            dots: true,
            autoplayHoverPause: false,
            items: 1,
            navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
        });
        var desktopCarou = $("#desktop-slider");
        var items = desktopCarou.data("items");
        var loop = desktopCarou.data("loop");
        var dots = desktopCarou.data("dots");
        var nav = desktopCarou.data("nav");
        //var height = $(desktopCarou.data("height")).height();
        console.log(items > 1);
        desktopCarou.owlCarousel({
            loop: items > 1,
            items: 1,
            autoplay: items > 1,
            center: true,
            nav: nav,
            dots: dots,
            animateOut: "slideOutLeft",
            animateIn: "slideInRight",
            autoplayHoverPause: false,
            navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"]
        });

        //$("#desktop-slider .owl-item").css("height", height + 'px');
    };
	carousel();
    $(".nav-tabs a").on("shown.bs.tab", function (event) {
        window.dispatchEvent(new Event("resize"));
    });

})(jQuery);

