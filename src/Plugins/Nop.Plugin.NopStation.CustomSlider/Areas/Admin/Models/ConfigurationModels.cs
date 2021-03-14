using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Models
{
    public partial class ConfigurationModel : BaseNopModel, ISettingsModel
    {
        [NopResourceDisplayName("Admin.NopStation.CustomSlider.Configuration.Fields.EnableSlider")]
        public bool EnableSlider { get; set; }
        public bool EnableSlider_OverrideForStore { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }
    }
}