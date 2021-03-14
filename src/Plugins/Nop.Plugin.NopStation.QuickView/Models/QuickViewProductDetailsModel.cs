using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.NopStation.QuickView.Models
{
    public class QuickViewProductDetailsModel : BaseNopEntityModel
    {
        public QuickViewProductDetailsModel()
        {
            ProductDetailsModel = new ProductDetailsModel();
        }

        public ProductDetailsModel ProductDetailsModel { get; set; }

        public bool ShowRelatedProducts { get; set; }
        
        public bool ShowAlsoPurchasedProducts { get; set; }

        public bool ShowShortDescription { get; set; }

        public bool ShowFullDescription { get; set; }

        public bool ShowAddToWishlistButton { get; set; }

        public bool ShowCompareProductsButton { get; set; }

        public bool ShowProductEmailAFriendButton { get; set; }

        public bool ShowProductReviewOverview { get; set; }

        public bool ShowProductManufacturers { get; set; }

        public bool ShowAvailability { get; set; }

        public bool ShowDeliveryInfo { get; set; }

        public bool ShowProductSpecifications { get; set; }

        public bool ShowProductTags { get; set; }

        public bool PictureZoomEnabled { get; set; }
    }
}
