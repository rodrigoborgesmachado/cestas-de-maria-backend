using Main = CestasDeMaria.Domain.Entities.Basketdeliverystatus;
using MainDto = CestasDeMaria.Application.DTO.BasketdeliverystatusDTO;

namespace CestasDeMaria.Application.Profiles
{
    public class BasketdeliverystatusProfile : AutoMapper.Profile
    {
        public BasketdeliverystatusProfile()
        {
            CreateMap<Main, MainDto>().PreserveReferences();
            CreateMap<MainDto, Main>().PreserveReferences();
        }
    }
}
