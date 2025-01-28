using Main = CestasDeMaria.Domain.Entities.Basketdeliveries;
using MainDto = CestasDeMaria.Application.DTO.BasketdeliveriesDTO;

namespace CestasDeMaria.Application.Profiles
{
    public class BasketdeliveriesProfile : AutoMapper.Profile
    {
        public BasketdeliveriesProfile()
        {
            CreateMap<Main, MainDto>().PreserveReferences();
            CreateMap<MainDto, Main>().PreserveReferences();
        }
    }
}
