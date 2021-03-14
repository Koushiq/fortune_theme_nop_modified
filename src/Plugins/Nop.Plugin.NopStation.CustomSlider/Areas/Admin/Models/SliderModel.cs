using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Models
{
    public class SliderModel : BaseNopEntityModel, IStoreMappingSupportedModel
    {
        public SliderModel()
        {
            AvailableWidgetZones = new List<SelectListItem>();
            AvailableAnimationTypes = new List<SelectListItem>();
            SliderItemSearchModel = new SliderItemSearchModel();
            SelectedStoreIds = new List<int>();
        }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.WidgetZone")]
        public int WidgetZoneId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.WidgetZone")]
        public string WidgetZoneStr { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.ShowBackgroundPicture")]
        public bool ShowBackgroundPicture { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.BackgroundPicture")]
        public int BackgroundPictureId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.Nav")]
        public bool Nav { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.AutoPlay")]
        public bool AutoPlay { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.AutoPlayTimeout")]
        public int AutoPlayTimeout { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.AutoPlayHoverPause")]
        public bool AutoPlayHoverPause { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.StartPosition")]
        public int StartPosition { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.LazyLoad")]
        public bool LazyLoad { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.LazyLoadEager")]
        public int LazyLoadEager { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.Video")]
        public bool Video { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.Loop")]
        public bool Loop { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.AnimateOut")]
        public string AnimateOut { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.AnimateIn")]
        public string AnimateIn { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Sliders.Fields.SelectedStoreIds")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
         
        public IList<SelectListItem> AvailableWidgetZones { get; set; }

        public IList<SelectListItem> AvailableAnimationTypes { get; set; }

        public SliderItemSearchModel SliderItemSearchModel { get; set; }
    }
}
