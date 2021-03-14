using Nop.Web.Framework.Models;

namespace Nop.Plugin.NopStation.BlogNews.Models
{
    public class NewsItemModel : BaseNopEntityModel
    {
        public string Title { get; set; }
        public string CreatedOnUtcStr { get; set; }
        public string PictureUrl { get; set; }
        public string AltAttribute { get; set; }
        public string TitleAttribute { get; set; }
        public string SeName { get; set; }
        public string ShortDescription { get; set; }
        public bool AllowComments { get; set; }
        public int TotalComments { get; set; }
    }
}
