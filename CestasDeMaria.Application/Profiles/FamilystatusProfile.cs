using Main = CestasDeMaria.Domain.Entities.Familystatus;
using MainDto = CestasDeMaria.Application.DTO.FamilystatusDTO;

namespace CestasDeMaria.Application.Profiles
{
    public class FamilystatusProfile : AutoMapper.Profile
    {
        public FamilystatusProfile()
        {
            CreateMap<Main, MainDto>().PreserveReferences();
            CreateMap<MainDto, Main>().PreserveReferences();
        }
    }
}
