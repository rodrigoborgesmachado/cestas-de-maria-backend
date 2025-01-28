using MainDto = CestasDeMaria.Application.DTO.MailmessageDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.MailmessageViewModel;

namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class MailmessageProfile : AutoMapper.Profile
    {
        public MailmessageProfile()
        {
            CreateMap<MainDto, MainViewModel>().PreserveReferences();
            CreateMap<MainViewModel, MainDto>().PreserveReferences();
        }
    }
}
