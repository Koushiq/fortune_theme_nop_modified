using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Plugin.NopStation.MegaMenu.Models
{
    public class ManufacturerMenuModel : BaseNopEntityModel
    {
        public ManufacturerMenuModel()
        {
            PictureModel = new PictureModel();
        }

        public string Name { get; set; }
        public string SeName { get; set; }
        public PictureModel PictureModel { get; set; }
    }
}
