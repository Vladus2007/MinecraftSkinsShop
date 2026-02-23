// Application/Mappings/PurchaseMappingProfile.cs
using Application.DTOs;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    public class PurchaseMappingProfile : Profile
    {
        public PurchaseMappingProfile()
        {
            CreateMap<Purchase, PurchaseResponse>()
                .ForMember(
                    dest => dest.Id, 
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.SkinId, 
                    opt => opt.MapFrom(src => src.SkinId))
                .ForMember(
                    dest => dest.PaidAmountUsd, 
                    opt => opt.MapFrom(src => src.PaidAmountUsd))
                .ForMember(
                    dest => dest.BtcPriceAtMoment,  // Имя в DTO
                    opt => opt.MapFrom(src => src.BtcPriceAtMonent)) // Имя в модели (с опечаткой)
                .ForMember(
                    dest => dest.PurchasedAt,  // Имя в DTO
                    opt => opt.MapFrom(src => src.PurchaseAt)); // Имя в модели
        }
    }
}