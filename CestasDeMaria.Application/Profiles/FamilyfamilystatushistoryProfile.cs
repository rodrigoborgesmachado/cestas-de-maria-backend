using Main = CestasDeMaria.Domain.Entities.Familyfamilystatushistory;
using MainDto = CestasDeMaria.Application.DTO.FamilyfamilystatushistoryDTO;

namespace CestasDeMaria.Application.Profiles
{
    public class FamilyfamilystatushistoryProfile : AutoMapper.Profile
    {
        public FamilyfamilystatushistoryProfile()
        {
            CreateMap<Main, MainDto>().PreserveReferences();
            CreateMap<MainDto, Main>().PreserveReferences();
        }
    }
}
