using FluentValidation;
using Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Validators
{
    public class ProductTabValidator : BaseNopValidator<ProductTabModel>
    {
        public ProductTabValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("NopStation.ProductTabs.ProductTabs.Fields.Name.Required"));
            RuleFor(x => x.TabTitle).NotEmpty().WithMessage(localizationService.GetResource("NopStation.ProductTabs.ProductTabs.Fields.Title.Required"));
            RuleFor(x => x.PictureId).GreaterThan(0).WithMessage(localizationService.GetResource("NopStation.ProductTabs.ProductTabs.Fields.Picture.Required"));
        }
    }
}
