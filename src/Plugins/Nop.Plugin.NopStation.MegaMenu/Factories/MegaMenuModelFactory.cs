using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using System;
using Nop.Plugin.NopStation.MegaMenu.Infrastructure.Cache;
using Nop.Plugin.NopStation.MegaMenu.Models;
using Nop.Plugin.NopStation.MegaMenu.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Services.Caching;
using Nop.Services.Customers;

namespace Nop.Plugin.NopStation.MegaMenu.Factories
{
    public class MegaMenuModelFactory : IMegaMenuModelFactory
    {
        #region Fields

        private readonly BlogSettings _blogSettings;
        private readonly ICategoryIconService _categoryIconService;
        private readonly CatalogSettings _catalogSettings;
        private readonly DisplayDefaultMenuItemSettings _displayDefaultMenuItemSettings;
        private readonly ForumSettings _forumSettings;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ITopicService _topicService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly MegaMenuSettings _megaMenuSettings;
        private readonly IMegaMenuCoreService _megaMenuCoreService;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public MegaMenuModelFactory(BlogSettings blogSettings,
            ICategoryIconService categoryIconService,
            CatalogSettings catalogSettings,
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings,
            ForumSettings forumSettings,
            ICategoryService categoryService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IProductService productService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            MegaMenuSettings megaMenuSettings,
            IMegaMenuCoreService megaMenuCoreService,
            ICacheKeyService cacheKeyService,
            ICustomerService customerService
            )
        {
            _blogSettings = blogSettings;
            _categoryIconService = categoryIconService;
            _catalogSettings = catalogSettings;
            _displayDefaultMenuItemSettings = displayDefaultMenuItemSettings;
            _forumSettings = forumSettings;
            _categoryService = categoryService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _productService = productService;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _megaMenuSettings = megaMenuSettings;
            _megaMenuCoreService = megaMenuCoreService;
            _cacheKeyService = cacheKeyService;
            _customerService = customerService;
        }

        #endregion

        protected virtual List<MegaMenuModel.TopicModel> PrepareTopicMenuModels()
        {
            //top menu topics
            var topicCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(MegaMenuModelCacheEventConsumer.MEGAMENU_TOPICS_MODEL_KEY,
                _workContext.WorkingLanguage,
                _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
                _storeContext.CurrentStore);
            var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
                _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
                .Where(t => t.IncludeInTopMenu)
                .Select(t => new MegaMenuModel.TopicModel
                {
                    Id = t.Id,
                    Name = _localizationService.GetLocalized(t, x => x.Title),
                    SeName = _urlRecordService.GetSeName(t)
                })
                .ToList()
            );

            return cachedTopicModel;
        }

        protected virtual List<CategoryMenuModel> PrepareCategoryMenuModels()
        {
            var loadPicture = _megaMenuSettings.ShowCategoryPicture;
            //load and cache them
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(MegaMenuModelCacheEventConsumer.MEGAMENU_CATEGORIES_MODEL_KEY,
                _workContext.WorkingLanguage,
                _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
                _storeContext.CurrentStore);
            return _cacheManager.Get(cacheKey, () =>
            {
                var ids = new List<int>();
                if (!string.IsNullOrWhiteSpace(_megaMenuSettings.SelectedCategoryIds))
                    ids = _megaMenuSettings.SelectedCategoryIds.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                var allCategories = _megaMenuCoreService.GetCategoriesByIds(_storeContext.CurrentStore.Id, ids);
                return PrepareCategoryMenuModels(allCategories, 0, loadPicture, skipItems: true);
            });
        }

        protected virtual List<CategoryMenuModel> PrepareCategoryMenuModels(IList<Category> allCategories,
            int rootCategoryId, bool loadPicture = true, bool loadSubCategories = true, bool skipItems = false)
        {
            var result = new List<CategoryMenuModel>();
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;
           
            var parentCategories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();

            foreach (var category in parentCategories)
            {
                var categoryModel = new CategoryMenuModel
                {
                    Id = category.Id,
                    Name = _localizationService.GetLocalized(category, x => x.Name),
                    SeName = _urlRecordService.GetSeName(category),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };
                if (loadPicture)
                {
                    //prepare picture model
                    var categoryIconPictureId = 0;
                    var categoryIcon = _categoryIconService.GetCategoryIconByCategoryId(category.Id);
                    if(categoryIcon != null)
                    {
                        categoryIconPictureId = categoryIcon.PictureId;
                    }
                    else
                    {
                        categoryIconPictureId = _megaMenuSettings.DefaultCategoryIconId;
                    }
                    var categoryPictureCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryPictureModelKey, 
                        categoryIconPictureId, 
                        pictureSize, 
                        true, 
                        _workContext.WorkingLanguage, 
                        _webHelper.IsCurrentConnectionSecured(), 
                        _storeContext.CurrentStore);
                    categoryModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(categoryIconPictureId);
                        if(picture == null)
                        {
                            picture = _pictureService.GetPictureById(_megaMenuSettings.DefaultCategoryIconId);
                        }
                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                            ImageUrl = _pictureService.GetPictureUrl(ref picture, pictureSize),
                            Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), categoryModel.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), categoryModel.Name)
                        };
                        return pictureModel;
                    });
                }

                //number of products in each category
                if (_megaMenuSettings.ShowNumberOfCategoryProducts)
                {
                    var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductAttributePictureModelKey,
                        _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
                        _storeContext.CurrentStore,
                        category);
                    categoryModel.NumberOfProducts = _cacheManager.Get(cacheKey, () =>
                    {
                        var categoryIds = new List<int>
                        {
                            category.Id
                        };
                        //include subcategories
                        if (_megaMenuSettings.ShowNumberOfCategoryProductsIncludeSubcategories)
                            categoryIds.AddRange(_categoryService.GetChildCategoryIds(category.Id, _storeContext.CurrentStore.Id));
                        return _productService.GetNumberOfProductsInCategory(categoryIds, _storeContext.CurrentStore.Id);
                    });
                }

                if (loadSubCategories)
                {
                    var subCategories = PrepareCategoryMenuModels(allCategories, category.Id, _megaMenuSettings.ShowSubcategoryPicture, loadSubCategories);
                    categoryModel.SubCategories.AddRange(subCategories);
                }
                result.Add(categoryModel);
            }

            return result;
        }

        protected virtual List<ManufacturerMenuModel> PrepareManufactureMenuModels(List<int> selectedManufactureIds)
        {
            var pictureSize = _mediaSettings.ManufacturerThumbPictureSize;
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(MegaMenuModelCacheEventConsumer.MEGAMENU_MANUFACTURERS_MODEL_KEY,
               _workContext.WorkingLanguage,
               _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
               _storeContext.CurrentStore);
            
            var manufacturers = _cacheManager.Get(cacheKey, () =>
            _megaMenuCoreService.GetManufacturersByIds(_storeContext.CurrentStore.Id, selectedManufactureIds)
            .Select(manufacturer =>
            {
                var model = new ManufacturerMenuModel
                {
                    Id = manufacturer.Id,
                    Name = _localizationService.GetLocalized(manufacturer, x => x.Name),
                    SeName = _urlRecordService.GetSeName(manufacturer)
                };

                if (_megaMenuSettings.ShowManufacturerPicture)
                {
                    var manufacturerPictureCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.ManufacturerPictureModelKey, 
                        manufacturer, 
                        pictureSize, 
                        true, 
                        _workContext.WorkingLanguage,
                        _webHelper.IsCurrentConnectionSecured(), 
                        _storeContext.CurrentStore);
                    model.PictureModel = _cacheManager.Get(manufacturerPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(manufacturer.PictureId);
                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                            ImageUrl = _pictureService.GetPictureUrl(ref picture, pictureSize),
                            Title = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageLinkTitleFormat"), model.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageAlternateTextFormat"), model.Name)
                        };
                        return pictureModel;
                    });
                }

                return model;
            }).ToList());

            return manufacturers;
        }

        public virtual MegaMenuModel PrepareMegaMenuModel()
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(MegaMenuModelCacheEventConsumer.MEGAMENU_MODEL_KEY,
                    _workContext.WorkingLanguage,
                   _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
                   _storeContext.CurrentStore);
            return _cacheManager.Get(cacheKey, () =>
            {
                var model = new MegaMenuModel()
                {
                    NewProductsEnabled = _catalogSettings.NewProductsEnabled,
                    BlogEnabled = _blogSettings.Enabled,
                    ForumEnabled = _forumSettings.ForumsEnabled,
                    DisplayHomePageMenuItem = _displayDefaultMenuItemSettings.DisplayHomepageMenuItem,
                    DisplayNewProductsMenuItem = _displayDefaultMenuItemSettings.DisplayNewProductsMenuItem,
                    DisplayProductSearchMenuItem = _displayDefaultMenuItemSettings.DisplayProductSearchMenuItem,
                    DisplayCustomerInfoMenuItem = _displayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem,
                    DisplayBlogMenuItem = _displayDefaultMenuItemSettings.DisplayBlogMenuItem,
                    DisplayForumsMenuItem = _displayDefaultMenuItemSettings.DisplayForumsMenuItem,
                    DisplayContactUsMenuItem = _displayDefaultMenuItemSettings.DisplayContactUsMenuItem,
                    MaxCategoryLevelsToShow = _megaMenuSettings.MaxCategoryLevelsToShow,
                    HideManufacturers = _megaMenuSettings.HideManufacturers
                };

                model.Topics = PrepareTopicMenuModels();
                model.Categories = PrepareCategoryMenuModels();

                if (!_megaMenuSettings.HideManufacturers)
                {
                    var selectedManufacturerIds = new List<int>();
                    if (!string.IsNullOrWhiteSpace(_megaMenuSettings.SelectedManufacturerIds))
                        selectedManufacturerIds = _megaMenuSettings.SelectedManufacturerIds.Split(',').Select(int.Parse).ToList();

                    model.Manufacturers = PrepareManufactureMenuModels(selectedManufacturerIds);
                }

                return model;
            });
        }
    }
}
