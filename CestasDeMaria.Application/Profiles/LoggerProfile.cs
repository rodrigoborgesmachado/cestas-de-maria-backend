using Main = CestasDeMaria.Domain.Entities.Logger;
using MainDto = CestasDeMaria.Application.DTO.LoggerDTO;

namespace CestasDeMaria.Application.Profiles
{
    public class LoggerProfile : AutoMapper.Profile
    {
        public LoggerProfile()
        {
            CreateMap<Main, MainDto>().PreserveReferences();
            CreateMap<MainDto, Main>().PreserveReferences();
        }
    }
}
