using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Factories;
using Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Models;
using Nop.Plugin.NopStation.CategoryBanners.Domains;
using Nop.Plugin.NopStation.CategoryBanners.Infrastructure.Cache;
using Nop.Plugin.NopStation.CategoryBanners.Services;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class CategoryBannerController : BaseAdminController
    {
        #region Firlds

        private readonly ICategoryService _categoryService;
        private readonly IPermissionService _permissionService;
        private readonly ICategoryBannerService _categoryBannerService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ICategoryBannerModelFactory _categoryBannerModelFactory;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public CategoryBannerController(ICategoryService categoryService,
            IPermissionService permissionService,
            ICategoryBannerService categoryBannerService,
            IPictureService pictureService,
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            INotificationService notificationService,
            ICategoryBannerModelFactory categoryBannerModelFactory,
            IStaticCacheManager cacheManager)
        {
            _categoryService = categoryService;
            _permissionService = permissionService;
            _categoryBannerService = categoryBannerService;
            _pictureService = pictureService;
            _settingService = settingService;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _categoryBannerModelFactory = categoryBannerModelFactory;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Utilities


        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(CategoryBannerPermissionProvider.ManageCategoryBanner))
                return AccessDeniedView();

            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var categoryBannerSettings = _settingService.LoadSetting<CategoryBannerSettings>(storeId);
            
            var model = new ConfigurationModel()
            {
                HideInPublicStore = categoryBannerSettings.HideInPublicStore,
                AutoPlay = categoryBannerSettings.AutoPlay,
                Nav = categoryBannerSettings.Nav,
                Loop = categoryBannerSettings.Loop,
                AutoPlayHoverPause = categoryBannerSettings.AutoPlayHoverPause,
                AutoPlayTimeout = categoryBannerSettings.AutoPlayTimeout,
                BannerPictureSize = categoryBannerSettings.BannerPictureSize
            };

            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId > 0)
            {
                model.HideInPublicStore_OverrideForStore = _settingService.SettingExists(categoryBannerSettings, x => x.HideInPublicStore, storeId);
                model.Nav_OverrideForStore = _settingService.SettingExists(categoryBannerSettings, x => x.Nav, storeId);
                model.AutoPlay_OverrideForStore = _settingService.SettingExists(categoryBannerSettings, x => x.AutoPlay, storeId);
                model.Loop_OverrideForStore = _settingService.SettingExists(categoryBannerSettings, x => x.Loop, storeId);
                model.AutoPlayHoverPause_OverrideForStore = _settingService.SettingExists(categoryBannerSettings, x => x.AutoPlayHoverPause, storeId);
                model.AutoPlayTimeout_OverrideForStore = _settingService.SettingExists(categoryBannerSettings, x => x.AutoPlayTimeout, storeId);
                model.BannerPictureSize_OverrideForStore = _settingService.SettingExists(categoryBannerSettings, x => x.BannerPictureSize, storeId);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(CategoryBannerPermissionProvider.ManageCategoryBanner))
                return AccessDeniedView();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var categoryBannerSettings = _settingService.LoadSetting<CategoryBannerSettings>(storeScope);

            categoryBannerSettings.HideInPublicStore = model.HideInPublicStore;
            categoryBannerSettings.Nav = model.Nav;
            categoryBannerSettings.AutoPlay = model.AutoPlay;
            categoryBannerSettings.Loop = model.Loop;
            categoryBannerSettings.AutoPlayHoverPause = model.AutoPlayHoverPause;
            categoryBannerSettings.AutoPlayTimeout = model.AutoPlayTimeout;
            categoryBannerSettings.BannerPictureSize = model.BannerPictureSize;

            _settingService.SaveSettingOverridablePerStore(categoryBannerSettings, x => x.HideInPublicStore, model.HideInPublicStore_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryBannerSettings, x => x.Nav, model.Nav_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryBannerSettings, x => x.AutoPlay, model.AutoPlay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryBannerSettings, x => x.Loop, model.Loop_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryBannerSettings, x => x.AutoPlayHoverPause, model.AutoPlayHoverPause_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryBannerSettings, x => x.AutoPlayTimeout, model.AutoPlayTimeout_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryBannerSettings, x => x.BannerPictureSize, model.BannerPictureSize_OverrideForStore, storeScope, false);

            _settingService.ClearCache();

            _cacheManager.RemoveByPrefix(ModelCacheDefaults.CategoryBannerPicturePrefixCacheKey);
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        [HttpPost]
        public IActionResult CategoryBannerList(CategoryBannerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedDataTablesJson();

            var category = _categoryService.GetCategoryById(searchModel.CategoryId);
            if (category == null || category.Deleted)
                throw new ArgumentException();

            var model = _categoryBannerModelFactory.PrepareProductPictureListModel(searchModel, category);

            return Json(model);
        }

        [HttpPost]
        public IActionResult CategoryBannerUpdate(CategoryBannerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var banner = _categoryBannerService.GetCategoryBannerById(model.Id)
                ?? throw new ArgumentException("No category banner found with the specified id");

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(banner.PictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            var category = _categoryService.GetCategoryById(banner.CategoryId);
            if (category == null || category.Deleted)
                return Json(new { Result = false });

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            _pictureService.SetSeoFilename(banner.PictureId, _pictureService.GetPictureSeName(category.Name));

            banner.DisplayOrder = model.DisplayOrder;
            banner.ForMobile = model.ForMobile;

            _categoryBannerService.UpdateCategoryBanner(banner);

            return new NullJsonResult();
        }

        [HttpPost]
        public IActionResult CategoryBannerDelete(CategoryBannerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var banner = _categoryBannerService.GetCategoryBannerById(model.Id)
                ?? throw new ArgumentException("No category banner found with the specified id");
            
            _categoryBannerService.DeleteCategoryBanner(banner);

            return new NullJsonResult();
        }

        [HttpPost]
        public IActionResult CreateBanner(CategoryBannerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(model.CategoryId);
            if (category == null || category.Deleted)
                return Json(new { Result = false });

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(model.PictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            _pictureService.SetSeoFilename(model.PictureId, _pictureService.GetPictureSeName(category.Name));

            var banner = new CategoryBanner()
            {
                CategoryId = model.CategoryId,
                DisplayOrder = model.DisplayOrder,
                ForMobile = model.ForMobile,
                PictureId = model.PictureId
            };
            _categoryBannerService.InsertCategoryBanner(banner);

            return Json(new { Result = true });
        }

        #endregion
    }
}
