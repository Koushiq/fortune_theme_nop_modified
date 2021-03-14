using Nop.Core.Configuration;

namespace Nop.Plugin.NopStation.CategoryBanners
{
    public class CategoryBannerSettings : ISettings
    {
        public bool HideInPublicStore { get; set; }

        public bool Loop { get; set; }

        public bool Nav { get; set; }

        public bool AutoPlay { get; set; }

        public int AutoPlayTimeout { get; set; }

        public int BannerPictureSize { get; set; }

        public bool AutoPlayHoverPause { get; set; }
    }
}