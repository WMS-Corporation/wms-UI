using AutoMapper;
using src.Models;
using src.ViewModel;

namespace src.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserModel, UserViewModel>();
        }
    }
}
