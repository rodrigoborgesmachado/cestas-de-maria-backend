using MainDto = CestasDeMaria.Application.DTO.BasketdeliverystatusDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.BasketdeliverystatusViewModel;

namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class BasketdeliverystatusProfile : AutoMapper.Profile
    {
        public BasketdeliverystatusProfile()
        {
            CreateMap<MainDto, MainViewModel>().PreserveReferences();
            CreateMap<MainViewModel, MainDto>().PreserveReferences();
        }
    }
}
