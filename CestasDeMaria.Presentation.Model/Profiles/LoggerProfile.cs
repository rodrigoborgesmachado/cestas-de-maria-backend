using MainDto = CestasDeMaria.Application.DTO.LoggerDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.LoggerViewModel;

namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class LoggerProfile : AutoMapper.Profile
    {
        public LoggerProfile()
        {
            CreateMap<MainDto, MainViewModel>().PreserveReferences();
            CreateMap<MainViewModel, MainDto>().PreserveReferences();
        }
    }
}
