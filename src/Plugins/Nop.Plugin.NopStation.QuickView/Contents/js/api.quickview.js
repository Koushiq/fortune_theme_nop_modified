
var quickView = function () {
};

quickView.prototype.viewProductDetails = function (options) {
  displayAjaxLoading(true);

  $.ajax({
    type: 'POST',
    data: options.data,
    url: '/quickview_product_details',
    contentType: options.contentType,
    success: function (reponse) {
      $("#qv-modal").html(reponse.html);
      $("#qv-button").magnificPopup({
        items: {
          src: '#qv-modal',
          type: 'inline'
        },
        callbacks: {

          open: function () {
            $(".thumbnails img").on("click", function () {
              $(".thumbnails img").removeClass("active");
              $(this).addClass("active");
              $(".largeImg").attr("src", $(this).attr("data-src"));
            });

            $(".tabs-product-details").tabs();
          },
        },
        preloader: true
      });

      $(".mfp-close").on("click", function () {
        $("#qv-modal").html("");
      });
      $("#qv-button").click();

      if (options.data.pictureZoom) {
        $('.src-zoom-anchor').on({
          'click': function () {
            var aSrc = $(this).attr('href');
            var imgSrc = $(this).attr('data-halfimgurl');
            $('.cloud-zoom-image').attr('src', imgSrc);
            $('.cloud-zoom').attr('href', aSrc);

            initZoom();
          }
        });
        initZoom();
      }

      $('#qv-modal .related-products-grid .item-grid,#qv-modal .also-purchased-products-grid .item-grid').addClass("owl-carousel");
      $('#qv-modal .related-products-grid .item-grid,#qv-modal .also-purchased-products-grid .item-grid').owlCarousel({
        items: 4,
        margin: 14,
        loop: true,
        autoplay: true,
        autoplayTimeout: 8000,
        nav: true,
        responsive: {
          0: {
            items: 2
          },
          576: {
            items: 2
          },
          768: {
            items: 2
          },
          1200: {
            items: 2
          },
          1300: {
            items: 3
          },
          1400: {
            items: 4
          }

        }
      });
    },
    error: function (result) {
      displayAjaxLoading(false);
    },
    complete: function () {
      displayAjaxLoading(false);
    }
  });
};

var api = new quickView();