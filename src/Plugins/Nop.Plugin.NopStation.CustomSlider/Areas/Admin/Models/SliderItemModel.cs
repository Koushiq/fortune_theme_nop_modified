using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Models
{
    public class SliderItemModel : BaseNopEntityModel, ILocalizedModel<SliderItemLocalizedModel>
    {
        public SliderItemModel()
        {
            Locales = new List<SliderItemLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.ShortDescription")]
        public string ShortDescription { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.Picture")]
        public int PictureId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.MobilePicture")]
        public int MobilePictureId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.ImageAltText")]
        public string ImageAltText { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.Link")]
        public string Link { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.Picture")]
        public string PictureUrl { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.MobilePicture")]
        public string MobilePictureUrl { get; set; }

        public string FullPictureUrl { get; set; }

        public string MobileFullPictureUrl { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public int SliderId { get; set; }

        public IList<SliderItemLocalizedModel> Locales { get; set; }
    }

    public class SliderItemLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.ShortDescription")]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.ImageAltText")]
        public string ImageAltText { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.SliderItems.Fields.Link")]
        public string Link { get; set; }
    }
}
