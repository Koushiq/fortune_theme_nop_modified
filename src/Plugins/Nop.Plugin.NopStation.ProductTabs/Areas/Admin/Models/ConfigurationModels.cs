using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Models
{
    public partial class ConfigurationModel : BaseNopModel, ISettingsModel
    {
        [NopResourceDisplayName("NopStation.ProductTabs.Configuration.Fields.EnableProductTab")]
        public bool EnableProductTab { get; set; }
        public bool EnableProductTab_OverrideForStore { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }
    }
}