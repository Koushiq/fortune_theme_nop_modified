using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.NopStation.BlogNews.Areas.Admin.Models
{
    public class BlogPictureModel : BaseNopEntityModel
    {
        [UIHint("Picture")]
        [NopResourceDisplayName("NopStation.BlogNews.Blogs.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("NopStation.BlogNews.Blogs.Fields.Picture")]
        public string PictureUrl { get; set; }

        [NopResourceDisplayName("NopStation.BlogNews.Blogs.Fields.OverrideAltAttribute")]
        public string OverrideAltAttribute { get; set; }

        [NopResourceDisplayName("NopStation.BlogNews.Blogs.Fields.OverrideTitleAttribute")]
        public string OverrideTitleAttribute { get; set; }

        [NopResourceDisplayName("NopStation.BlogNews.Blogs.Fields.ShowInStore")]
        public bool ShowInStore { get; set; }
    }
}
