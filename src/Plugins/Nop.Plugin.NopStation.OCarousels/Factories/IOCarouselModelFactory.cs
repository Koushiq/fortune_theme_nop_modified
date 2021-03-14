using System.Collections.Generic;
using Nop.Plugin.NopStation.OCarousels.Domains;
using Nop.Plugin.NopStation.OCarousels.Models;

namespace Nop.Plugin.NopStation.OCarousels.Factories
{
    public interface IOCarouselModelFactory
    {
        IList<OCarouselModel> PrepareCarouselListModel(IList<OCarousel> carousels);

        OCarouselModel PrepareCarouselModel(OCarousel carousel);
    }
}
