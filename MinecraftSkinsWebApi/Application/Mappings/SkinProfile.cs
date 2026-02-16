// AutoMapper profile: SkinProfile
// Purpose: Define mappings from domain `Skin` entity to `SkinResponse` DTO.
// Note: FinalPriceUsd is ignored in mapping because it is computed separately via IPriceCalculator.

using Application.DTOs;
using AutoMapper;
using Domain.Models;

namespace MinecraftSkins.Application.Mappings;

public class SkinProfile : Profile
{
    public SkinProfile()
    {
        CreateMap<Skin, SkinResponse>()
            .ForMember(dest => dest.FinalPriceUsd, opt => opt.Ignore()); // заполняем отдельно
    }
}