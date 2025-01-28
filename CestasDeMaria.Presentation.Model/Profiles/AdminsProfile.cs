using MainDto = CestasDeMaria.Application.DTO.AdminsDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.AdminsViewModel;

namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class AdminsProfile : AutoMapper.Profile
    {
        public AdminsProfile()
        {
            CreateMap<MainDto, MainViewModel>().PreserveReferences();
            CreateMap<MainViewModel, MainDto>().PreserveReferences();
        }
    }
}
