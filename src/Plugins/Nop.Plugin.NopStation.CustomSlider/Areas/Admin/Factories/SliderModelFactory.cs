using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Models;
using Nop.Plugin.NopStation.CustomSlider.Domains;
using Nop.Plugin.NopStation.CustomSlider.Helpers;
using Nop.Plugin.NopStation.CustomSlider.Services;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Factories
{
    public partial class SliderModelFactory : ISliderModelFactory
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ISliderService _sliderService;

        #endregion

        #region Ctor

        public SliderModelFactory(IStoreContext storeContext,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            ILocalizedModelFactory localizedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ISettingService settingService,
            IDateTimeHelper dateTimeHelper,
            ISliderService sliderService)
        {
            _storeContext = storeContext;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _localizedModelFactory = localizedModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _settingService = settingService;
            _dateTimeHelper = dateTimeHelper;
            _sliderService = sliderService;
        }

        #endregion

        #region Utilities

        protected void PrepareCustomWidgetZones(IList<SelectListItem> items, bool withSpecialDefaultItem = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available activity log types
            var availableWidgetZones = SliderHelper.GetCustomWidgetZoneSelectList();
            foreach (var zone in availableWidgetZones)
            {
                items.Add(zone);
            }

            if (withSpecialDefaultItem)
                items.Insert(0, new SelectListItem()
                {
                    Text = _localizationService.GetResource("Admin.Common.All"),
                    Value = "0"
                });
        }
        
        protected void PrepareActiveOptions(IList<SelectListItem> items, bool withSpecialDefaultItem = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            items.Add(new SelectListItem()
            {
                Text = _localizationService.GetResource("Admin.NopStation.CustomSlider.Sliders.List.SearchActive.Active"),
                Value = "1"
            });
            items.Add(new SelectListItem()
            {
                Text = _localizationService.GetResource("Admin.NopStation.CustomSlider.Sliders.List.SearchActive.Inactive"),
                Value = "2"
            });

            if (withSpecialDefaultItem)
                items.Insert(0, new SelectListItem()
                {
                    Text = _localizationService.GetResource("Admin.Common.All"),
                    Value = "0"
                });
        }

        #endregion

        #region Methods

        public ConfigurationModel PrepareConfigurationModel()
        {
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var sliderSettings = _settingService.LoadSetting<CustomSliderSettings>(storeId);

            var model = sliderSettings.ToSettingsModel<ConfigurationModel>();

            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            model.EnableSlider_OverrideForStore = _settingService.SettingExists(sliderSettings, x => x.EnableSlider, storeId);

            return model;
        }

        public virtual SliderSearchModel PrepareSliderSearchModel(SliderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            PrepareCustomWidgetZones(searchModel.AvailableWidgetZones, true);
            PrepareActiveOptions(searchModel.AvailableActiveOptions, true);

            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            return searchModel;
        }

        public virtual SliderListModel PrepareSliderListModel(SliderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var widgetZoneIds = searchModel.SearchWidgetZones?.Contains(0) ?? true ? null : searchModel.SearchWidgetZones.ToList();

            bool? active = null;
            if (searchModel.SearchActiveId == 1)
                active = true;
            else if (searchModel.SearchActiveId == 2)
                active = false;

            //get carousels
            var sliders = _sliderService.GetAllSliders(widgetZoneIds, searchModel.SearchStoreId,
                active, searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new SliderListModel().PrepareToGrid(searchModel, sliders, () =>
            {
                return sliders.Select(slider =>
                {
                    return PrepareSliderModel(null, slider, true);
                });
            });

            return model;
        }

        public SliderModel PrepareSliderModel(SliderModel model, Slider slider,
            bool excludeProperties = false)
        {
            if (slider != null)
            {
                if (model == null)
                {
                    model = slider.ToModel<SliderModel>();
                    model.WidgetZoneStr = SliderHelper.GetCustomWidgetZone(slider.WidgetZoneId);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(slider.CreatedOnUtc, DateTimeKind.Utc);
                    model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(slider.UpdatedOnUtc, DateTimeKind.Utc);
                }
            }

            if (!excludeProperties)
            {
                model.AvailableWidgetZones = SliderHelper.GetCustomWidgetZoneSelectList();
                model.AvailableAnimationTypes = SliderHelper.GetSliderAnimationTypesSelectList();

                _storeMappingSupportedModelFactory.PrepareModelStores(model, slider, excludeProperties);
            }

            return model;
        }

        public SliderItemListModel PrepareSliderItemListModel(SliderItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get sliders
            var sliderItems = _sliderService.GetSliderItemsBySliderId(searchModel.SliderId, searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new SliderItemListModel().PrepareToGrid(searchModel, sliderItems, () =>
            {
                return sliderItems.Select(sliderItem =>
                {
                    var slider = _sliderService.GetSliderById(sliderItem.SliderId);
                    return PrepareSliderItemModel(null, slider , sliderItem);
                });
            });

            return model;
        }

        public SliderItemModel PrepareSliderItemModel(SliderItemModel model, Slider slider, 
            SliderItem sliderItem, bool excludeProperties = false)
        {
            Action<SliderItemLocalizedModel, int> localizedModelConfiguration = null;

            if (sliderItem != null)
            {
                if (model == null)
                {
                    model = sliderItem.ToModel<SliderItemModel>();
                    model.PictureUrl = _pictureService.GetPictureUrl(sliderItem.PictureId, 200);
                    model.FullPictureUrl = _pictureService.GetPictureUrl(sliderItem.PictureId);
                    model.MobilePictureUrl = _pictureService.GetPictureUrl(sliderItem.MobilePictureId, 200);
                    model.MobileFullPictureUrl = _pictureService.GetPictureUrl(sliderItem.MobilePictureId);
                }

                if (!excludeProperties)
                {
                    localizedModelConfiguration = (locale, languageId) =>
                    {
                        locale.Title = _localizationService.GetLocalized(sliderItem, entity => entity.Title, languageId, false, false);
                        locale.ShortDescription = _localizationService.GetLocalized(sliderItem, entity => entity.ShortDescription, languageId, false, false);
                        locale.ImageAltText = _localizationService.GetLocalized(sliderItem, entity => entity.ImageAltText, languageId, false, false);
                        locale.Link = _localizationService.GetLocalized(sliderItem, entity => entity.Link, languageId, false, false);
                    };
                }
            }

            if (!excludeProperties)
            {
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);;
            }

            model.SliderId = slider.Id;

            return model;
        }

        #endregion
    }
}
