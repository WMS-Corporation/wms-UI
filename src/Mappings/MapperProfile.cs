using AutoMapper;
using src.Models;
using src.Services;
using src.ViewModel;

namespace src.Mappings
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserModel, UserViewModel>();
            CreateMap<ProductModel, ProductViewModel>();
            CreateMap<OrderModel, OrderViewModel>().ForMember(dest => dest.ProductList, opt => opt.Ignore());
            CreateMap<TaskModel, TaskViewModel>().ForMember(dest => dest.ProductList, opt => opt.Ignore());


            CreateMap<ProductViewModel, ProductModel>();

        }
    }
}
