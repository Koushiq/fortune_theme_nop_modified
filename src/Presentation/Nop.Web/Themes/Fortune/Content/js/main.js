(function ($) {
  "use strict";

  $(document).ready(function ($) {
    
    var homepage = $('html').hasClass("html-home-page");
    var headerAdmin = $(".admin-header-links").height();

    // Calculate header height
    function headerHeight() {      
      var headerNav = $(".header").height();
      if (headerAdmin) {
        var headerTotal = ((headerAdmin + headerNav) + 22);
      } else {
        var headerTotal = headerNav;
      }      
      return headerTotal;
    }
    
    // Megamenu height for larger device
    function megaMenuHeight() {
      setTimeout(function () {
        var sliderHeight = $(".anywhere-slider-container").height();
        var megaNavHeight = $(".mm-navbar-nav").height();

        if (sliderHeight && $(window).width() >= 1283) {
          $(".mm-nav-item .sublist.first-level").css("min-height", (sliderHeight - 3) + "px");
        } else {
          $(".mm-nav-item .sublist.first-level").css("min-height", (megaNavHeight + 15) + "px");
        }
        
        if ($(window).width() >= 1283) {
          $(".html-home-page .header-menu .mm-navbar-nav").css("height", (sliderHeight) + "px");
        }

        if (homepage && $(window).width() >= 1283) {
          var nav_items_height = 50;
          $(".not-mobile > .mm-navbar-nav > .mm-nav-item").each(function () {
            nav_items_height = nav_items_height + $(this).height();
            if (nav_items_height > sliderHeight || nav_items_height > 500) {
              $(this).addClass("go_others");
              if ($(this).hasClass("other_nav")) {
                $(".other_nav").removeClass("d-none")
              } else {
                $(this).appendTo(".other_nav .sublist");
                //$(this).addClass("d-inline-block");
              }
            }
          })
        }

      }, 0);
    }
    $(window).on('load', function () {
      megaMenuHeight();
    })

    // Calculate header height & assign to navmenu top    
    function menuTop(height) {
      if ($(window).width() < 975) {
        $(".mm-navbar.mobile, .header-menu .sublist").css("top", height + "px");

        if (headerAdmin) {
          $(".mm-navbar.mobile").css({ "height": "calc(100vh - " + height + "px)" });
        } else {
          $(".mm-navbar.mobile").css({ "height": "calc(100vh - " + height + "px)", "margin-top": "1px" });
        }
      } else {
        $(".mm-navbar, .header-menu .sublist").css("top", "23px");
      }
    }
    menuTop(headerHeight());

    // Delay loading
    setTimeout(function () {
      $(".mm-navbar.not-mobile, .product-tab-body .ui-tabs-nav").show();
    }, 500);

    // Assign height to flyout cart    
    function flyoutCartHeight(height) {      
      $(".mini-shopping-cart").css("height", "calc(100vh - " + height + "px)");
    }
    flyoutCartHeight(headerHeight());

    // Assign top & height to product filters 
    function filterSidebar(height) {
      $(".product-filters").css({ "top": height + "px", "height": "calc(100vh - " + height + "px)" });      
    }
    filterSidebar(headerHeight());

    // Initiate product details tabs
    $("#productTab").tabs();

    // Product filter sidebar
    $('.filter-toggler').click(function () {
      $('.product-filters').show("slide", { direction: "left" }, 300);
    });
    $('.fiters-flyout-title > span').click(function () {
      $('.product-filters').hide("slide", { direction: "left" }, 300)
    });
    $(".product-filter .filter-title").click(function () {
      $(".product-filter .filter-content").toggle("slow");
      $(".product-filter .filter-title").toggleClass("rotate-icon");
    });

    // 
    $('.remove-button').on('click', function () {
      $(this).siblings('label').trigger("click");
      $('.update-cart-button').trigger("click");
      $(".update-wishlist-button").trigger("click");
    });

    $(window).on("resize", function () {
      megaMenuHeight();
      menuTop(headerHeight());
      flyoutCartHeight(headerHeight());
      filterSidebar(headerHeight());
    });

    $(window).scroll(function () {
      var headerLower = $('.header-lower');
      var sliderHeight = $(".anywhere-slider-container").height();
      var homeHeader = (headerHeight() + sliderHeight);
      megaMenuHeight();

      function stickyAdd() {
        headerLower.addClass("sticky");
        flyoutCartHeight(headerLower.innerHeight());
        filterSidebar(headerLower.innerHeight());
        menuTop(headerLower.innerHeight());
      }

      function stickyRemove() {
        headerLower.removeClass("sticky");
        menuTop(headerHeight());
        flyoutCartHeight(headerHeight());
        filterSidebar(headerHeight());
      }

      if (homepage) {
        if ($(this).scrollTop() > homeHeader) {
          stickyAdd();
        }
        else {
          stickyRemove();
        }
      }
      else if ($(this).scrollTop() > headerHeight()) {
        stickyAdd();        
      }
      else {
        stickyRemove();
      }
    });

  });

}(jQuery));