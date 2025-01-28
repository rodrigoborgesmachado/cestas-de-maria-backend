using Main = CestasDeMaria.Domain.Entities.Mailmessage;
using MainDto = CestasDeMaria.Application.DTO.MailmessageDTO;

namespace CestasDeMaria.Application.Profiles
{
    public class MailmessageProfile : AutoMapper.Profile
    {
        public MailmessageProfile()
        {
            CreateMap<Main, MainDto>().PreserveReferences();
            CreateMap<MainDto, Main>().PreserveReferences();
        }
    }
}
