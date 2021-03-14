using FluentValidation;
using Nop.Plugin.NopStation.BlogNews.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.BlogNews.Areas.Admin.Validators
{
    public class BlogNewsValidator : BaseNopValidator<ConfigurationModel>
    {
        public BlogNewsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.WidgetZone).NotEmpty().WithMessage(localizationService.GetResource("NopStation.BlogNews.Configuration.Fields.WidgetZone.Required"));
        }
    }
}
