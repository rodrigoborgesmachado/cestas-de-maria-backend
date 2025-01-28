using MainDto = CestasDeMaria.Application.DTO.FamilyfamilystatushistoryDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.FamilyfamilystatushistoryViewModel;

namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class FamilyfamilystatushistoryProfile : AutoMapper.Profile
    {
        public FamilyfamilystatushistoryProfile()
        {
            CreateMap<MainDto, MainViewModel>().PreserveReferences();
            CreateMap<MainViewModel, MainDto>().PreserveReferences();
        }
    }
}
