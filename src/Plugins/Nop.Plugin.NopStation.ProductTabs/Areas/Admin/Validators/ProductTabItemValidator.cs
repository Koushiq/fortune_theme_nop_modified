using FluentValidation;
using Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Validators
{
    public class ProductTabItemValidator : BaseNopValidator<ProductTabItemModel>
    {
        public ProductTabItemValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("NopStation.ProductTabs.ProductTabItems.Fields.Name.Required"));
        }
    }
}
