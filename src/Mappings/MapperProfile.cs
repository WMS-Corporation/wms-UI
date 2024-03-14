using AutoMapper;
using src.Models;
using src.Models.Product;
using src.ViewModel;

namespace src.Mappings
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserModel, UserViewModel>();
            CreateMap<ProductModel, ProductViewModel>();
            CreateMap<TaskModel, TaskViewModel>().ForMember(dest => dest.ProductList, opt => opt.MapFrom(src => src.ProductList)); ;
        }
    }
}
