using MainDto = CestasDeMaria.Application.DTO.FamilystatusDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.FamilystatusViewModel;

namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class FamilystatusProfile : AutoMapper.Profile
    {
        public FamilystatusProfile()
        {
            CreateMap<MainDto, MainViewModel>().PreserveReferences();
            CreateMap<MainViewModel, MainDto>().PreserveReferences();
        }
    }
}
