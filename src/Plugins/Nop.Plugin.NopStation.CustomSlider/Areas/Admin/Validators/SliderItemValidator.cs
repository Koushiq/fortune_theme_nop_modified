using FluentValidation;
using Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Models;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.CustomSlider.Areas.Admin.Validators
{
    public class SliderItemValidator : BaseNopValidator<SliderItemModel>
    {
        public SliderItemValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Admin.NopStation.CustomSlider.SliderItems.Fields.Title.Required");
            RuleFor(x => x.PictureId).GreaterThan(0).WithMessage("Admin.NopStation.CustomSlider.SliderItems.Fields.Picture.Required");
            RuleFor(x => x.MobilePictureId).GreaterThan(0).WithMessage("Admin.NopStation.CustomSlider.Sliders.Fields.MobilePicture.Required");
        }
    }
}
