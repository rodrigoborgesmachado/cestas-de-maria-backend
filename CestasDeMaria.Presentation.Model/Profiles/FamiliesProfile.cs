using MainDto = CestasDeMaria.Application.DTO.FamiliesDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.FamiliesViewModel;

namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class FamiliesProfile : AutoMapper.Profile
    {
        public FamiliesProfile()
        {
            CreateMap<MainDto, MainViewModel>().PreserveReferences();
            CreateMap<MainViewModel, MainDto>().PreserveReferences();
        }
    }
}
