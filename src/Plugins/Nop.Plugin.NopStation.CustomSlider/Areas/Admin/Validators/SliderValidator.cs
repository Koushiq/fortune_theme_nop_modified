using FluentValidation;
using Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Models;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Validators
{
    public class SliderValidator : BaseNopValidator<SliderModel>
    {
        public SliderValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Admin.NopStation.CustomSlider.Sliders.Fields.Name.Required");
            RuleFor(x => x.BackgroundPictureId).GreaterThan(0).When(x=> x.ShowBackgroundPicture).WithMessage("Admin.NopStation.CustomSlider.Sliders.Fields.BackgroundPicture.Required");
        }
    }
}
