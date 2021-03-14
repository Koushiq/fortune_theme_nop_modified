using FluentValidation;
using Nop.Plugin.NopStation.AnywhereSlider.Areas.Admin.Models;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.AnywhereSlider.Areas.Admin.Validators
{
    public class SliderItemValidator : BaseNopValidator<SliderItemModel>
    {
        public SliderItemValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Admin.NopStation.AnywhereSlider.SliderItems.Fields.Title.Required");
            RuleFor(x => x.PictureId).GreaterThan(0).WithMessage("Admin.NopStation.AnywhereSlider.SliderItems.Fields.Picture.Required");
            RuleFor(x => x.MobilePictureId).GreaterThan(0).WithMessage("Admin.NopStation.AnywhereSlider.Sliders.Fields.MobilePicture.Required");
        }
    }
}
