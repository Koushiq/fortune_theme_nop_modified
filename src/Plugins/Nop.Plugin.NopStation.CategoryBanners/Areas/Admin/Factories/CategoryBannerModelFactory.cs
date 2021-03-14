using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Models;
using Nop.Plugin.NopStation.CategoryBanners.Services;
using Nop.Services.Media;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Factories
{
    public class CategoryBannerModelFactory : ICategoryBannerModelFactory
    {
        private readonly ICategoryBannerService _categoryBannerService;
        private readonly IPictureService _pictureService;

        public CategoryBannerModelFactory(ICategoryBannerService categoryBannerService,
            IPictureService pictureService)
        {
            _categoryBannerService = categoryBannerService;
            _pictureService = pictureService;
        }

        public virtual CategoryBannerListModel PrepareProductPictureListModel(CategoryBannerSearchModel searchModel, Category category)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            //get product pictures
            var productPictures = _categoryBannerService.GetCategoryBannersByCategoryId(category.Id).ToPagedList(searchModel);

            //prepare grid model
            var model = new CategoryBannerListModel().PrepareToGrid(searchModel, productPictures, () =>
            {
                return productPictures.Select(banner =>
                {
                    //fill in model values from the entity
                    var categoryBannerModel = new CategoryBannerModel()
                    {
                        DisplayOrder = banner.DisplayOrder,
                        CategoryId = banner.CategoryId,
                        ForMobile = banner.ForMobile,
                        PictureId = banner.PictureId,
                        Id = banner.Id
                    };

                    //fill in additional values (not existing in the entity)
                    var picture = _pictureService.GetPictureById(banner.PictureId)
                                  ?? throw new Exception("Picture cannot be loaded");

                    categoryBannerModel.PictureUrl = _pictureService.GetPictureUrl(picture.Id);
                    categoryBannerModel.OverrideAltAttribute = picture.AltAttribute;
                    categoryBannerModel.OverrideTitleAttribute = picture.TitleAttribute;

                    return categoryBannerModel;
                });
            });

            return model;
        }
    }
}
