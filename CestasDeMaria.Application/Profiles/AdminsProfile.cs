using Main = CestasDeMaria.Domain.Entities.Admins;
using MainDto = CestasDeMaria.Application.DTO.AdminsDTO;

namespace CestasDeMaria.Application.Profiles
{
    public class AdminsProfile : AutoMapper.Profile
    {
        public AdminsProfile()
        {
            CreateMap<Main, MainDto>().PreserveReferences();
            CreateMap<MainDto, Main>().PreserveReferences();
        }
    }
}
