using FluentValidation;
using Nop.Plugin.NopStation.ProductRibbon.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.ProductRibbon.Areas.Admin.Validators
{
    public class ProductRibbonValidator : BaseNopValidator<ConfigurationModel>
    {
        public ProductRibbonValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ProductDetailsPageWidgetZone).NotEmpty().WithMessage(localizationService.GetResource("Admin.NopStation.ProductRibbon.Configuration.Fields.ProductDetailsPageWidgetZone.Required"));
            RuleFor(x => x.ProductOverviewBoxWidgetZone).NotEmpty().WithMessage(localizationService.GetResource("Admin.NopStation.ProductRibbon.Configuration.Fields.ProductOverviewBoxWidgetZone.Required"));
        }
    }
}
