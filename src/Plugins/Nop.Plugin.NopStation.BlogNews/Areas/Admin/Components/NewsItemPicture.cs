using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.BlogNews.Services;
using Nop.Services.Media;
using Nop.Web.Framework.Components;
using Nop.Plugin.NopStation.BlogNews.Domains;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.BlogNews.Areas.Admin.Models;
using Nop.Web.Areas.Admin.Models.News;

namespace Nop.Plugin.NopStation.BlogNews.Areas.Admin.Components
{
    public partial class NewsItemPictureViewComponent : NopViewComponent
    {
        private readonly IPictureService _pictureService;
        private readonly IBlogNewsPictureService _blogNewsPictureService;
        private readonly INopStationLicenseService _licenseService;
        private readonly BlogNewsSettings _blogNewsSettings;

        public NewsItemPictureViewComponent(IPictureService pictureService,
            IBlogNewsPictureService blogNewsPictureService,
            INopStationLicenseService licenseService,
            BlogNewsSettings blogNewsSettings)
        {
            _blogNewsPictureService = blogNewsPictureService;
            _blogNewsSettings = blogNewsSettings;
            _pictureService = pictureService;
            _licenseService = licenseService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            var newsItemModel = (NewsItemModel)additionalData;
            var model = new NewsPictureModel();
            model.Id = newsItemModel.Id;

            var blogNewsPicture = _blogNewsPictureService.GetBlogNewsPictureByEntytiId(newsItemModel.Id, EntityType.News);
            if (blogNewsPicture != null)
            {
                model.PictureId = blogNewsPicture.PictureId;
                model.ShowInStore = blogNewsPicture.ShowInStore;

                var picture = _pictureService.GetPictureById(model.PictureId);
                if (picture != null)
                {
                    model.OverrideAltAttribute = picture.AltAttribute;
                    model.OverrideTitleAttribute = picture.TitleAttribute;
                }
            }

            return View(model);
        }
    }
}
