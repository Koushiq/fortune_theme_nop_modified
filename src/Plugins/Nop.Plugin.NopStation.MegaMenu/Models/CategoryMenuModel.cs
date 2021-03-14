using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Plugin.NopStation.MegaMenu.Models
{
    public class CategoryMenuModel : BaseNopEntityModel
    {
        public CategoryMenuModel()
        {
            SubCategories = new List<CategoryMenuModel>();
            PictureModel = new PictureModel();
        }

        public string Name { get; set; }
        public string SeName { get; set; }
        public int? NumberOfProducts { get; set; }
        public bool IncludeInTopMenu { get; set; }
        public PictureModel PictureModel { get; set; }
        public List<CategoryMenuModel> SubCategories { get; set; }
    }
}
