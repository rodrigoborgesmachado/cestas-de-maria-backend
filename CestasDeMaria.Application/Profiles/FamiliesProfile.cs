using Main = CestasDeMaria.Domain.Entities.Families;
using MainDto = CestasDeMaria.Application.DTO.FamiliesDTO;

namespace CestasDeMaria.Application.Profiles
{
    public class FamiliesProfile : AutoMapper.Profile
    {
        public FamiliesProfile()
        {
            CreateMap<Main, MainDto>().PreserveReferences();
            CreateMap<MainDto, Main>().PreserveReferences();
        }
    }
}
