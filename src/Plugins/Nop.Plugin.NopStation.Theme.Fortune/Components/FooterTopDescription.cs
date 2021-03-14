using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.NopStation.Theme.Fortune.Infrastructure.Cache;
using Nop.Plugin.NopStation.Theme.Fortune.Models;
using Nop.Services.Caching;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.Theme.Fortune.Components
{
    public class FooterTopDescriptionViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly FortuneSettings _fortuneSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;

        #endregion

        #region Ctor

        public FooterTopDescriptionViewComponent(
            IWorkContext workContext,
            ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            FortuneSettings fortuneSettings,
            ILocalizationService localizationService,
            IPictureService pictureService)
        {
            _workContext = workContext;
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _fortuneSettings = fortuneSettings;
            _localizationService = localizationService;
            _pictureService = pictureService;
        }

        #endregion

        #region Methods

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (
               !_fortuneSettings.EnableDescriptionBoxOne
            && !_fortuneSettings.EnableDescriptionBoxTwo
            && !_fortuneSettings.EnableDescriptionBoxThree
            && !_fortuneSettings.EnableDescriptionBoxFour)
            {
                return Content("");
            }

            string title;
            string text;
            string url;
            string pictureUrl;
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheEventConsumer.FOOTER_DESCRIPTION_MODEL_KEY, _storeContext.CurrentStore.Id, _workContext.WorkingLanguage.Id);
            var descriptionBoxes = _staticCacheManager.Get(cacheKey, () =>
            {
                var model = new List<FooterTopDescriptionModel>();
                if (_fortuneSettings.EnableDescriptionBoxOne)
                {
                    title = GetResourceForCurrentStore("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneTitleValue");
                    text = GetResourceForCurrentStore("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneTextValue");
                    url = _fortuneSettings.DescriptionBoxOneUrl;
                    pictureUrl = _pictureService.GetPictureUrl(_fortuneSettings.DescriptionBoxOnePictureId);
                    model.Add(new FooterTopDescriptionModel
                    {
                        Title = title,
                        Text = text,
                        Url = url,
                        PictureUrl = pictureUrl
                    });
                }
                if (_fortuneSettings.EnableDescriptionBoxTwo)
                {
                    title = GetResourceForCurrentStore("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoTitleValue");
                    text = GetResourceForCurrentStore("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoTextValue");
                    url = _fortuneSettings.DescriptionBoxTwoUrl;
                    pictureUrl = _pictureService.GetPictureUrl(_fortuneSettings.DescriptionBoxTwoPictureId);
                    model.Add(new FooterTopDescriptionModel
                    {
                        Title = title,
                        Text = text,
                        Url = url,
                        PictureUrl = pictureUrl
                    });
                }
                if (_fortuneSettings.EnableDescriptionBoxThree)
                {
                    title = GetResourceForCurrentStore("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeTitleValue");
                    text = GetResourceForCurrentStore("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeTextValue");
                    url = _fortuneSettings.DescriptionBoxThreeUrl;
                    pictureUrl = _pictureService.GetPictureUrl(_fortuneSettings.DescriptionBoxThreePictureId);
                    model.Add(new FooterTopDescriptionModel
                    {
                        Title = title,
                        Text = text,
                        Url = url,
                        PictureUrl = pictureUrl
                    });
                }
                if (_fortuneSettings.EnableDescriptionBoxFour)
                {
                    title = GetResourceForCurrentStore("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourTitleValue");
                    text = GetResourceForCurrentStore("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourTextValue");
                    url = _fortuneSettings.DescriptionBoxFourUrl;
                    pictureUrl = _pictureService.GetPictureUrl(_fortuneSettings.DescriptionBoxFourPictureId);
                    model.Add(new FooterTopDescriptionModel
                    {
                        Title = title,
                        Text = text,
                        Url = url,
                        PictureUrl = pictureUrl
                    });
                }

                return model;
            });

            return View("~/Plugins/NopStation.Theme.Fortune/Views/_FooterTopDescription.cshtml", descriptionBoxes);
        }

        private string GetResourceForCurrentStore(string resourceName)
        {
            var resourceKey = $"{resourceName}-{_storeContext.CurrentStore.Id}";
            var resourceValue = _localizationService.GetResource(resourceKey, _workContext.WorkingLanguage.Id, false, "", true);
            if (!string.IsNullOrEmpty(resourceValue))
            {
                return resourceValue;
            }
            return _localizationService.GetResource(resourceName, _workContext.WorkingLanguage.Id, false, "", true);
        }

        #endregion
    }
}
