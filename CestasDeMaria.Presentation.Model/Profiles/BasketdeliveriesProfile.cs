using MainDto = CestasDeMaria.Application.DTO.BasketdeliveriesDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.BasketdeliveriesViewModel;

namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class BasketdeliveriesProfile : AutoMapper.Profile
    {
        public BasketdeliveriesProfile()
        {
            CreateMap<MainDto, MainViewModel>().PreserveReferences();
            CreateMap<MainViewModel, MainDto>().PreserveReferences();
        }
    }
}
