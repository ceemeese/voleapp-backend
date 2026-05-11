using Application.Abstractions.DTO.PricingConfig;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Club.Entities;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.PricingConfig.Commands.Update;

internal sealed class UpdatePricingConfigHandler : IRequestHandler<UpdatePricingConfig, Result<PricingConfigResponse>>
{
    private readonly IMapper _mapper;
    private readonly IClubRepository _clubRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;

    public UpdatePricingConfigHandler(IMapper mapper, IClubRepository clubRepository, IUnitOfWork unitOfWork, IUserContext userContext,  IClubMemberQueries clubMemberQueries)
    {
        _mapper = mapper;
        _clubRepository = clubRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result<PricingConfigResponse>> Handle(UpdatePricingConfig request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<PricingConfigResponse>(UserErrors.Forbidden);
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<PricingConfigResponse>(ClubErrors.Forbidden);
            }
        }

        var club = await _clubRepository.GetClubByIdAsync(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<PricingConfigResponse>(PricingConfigErrors.NotFound(request.ClubId));
        }
        
        var updatedPricingConfig = club.UpdatePricing(request.RainDiscountPercent, request.WindThreshold, request.WindDiscountPercent, request.HeatThreshold, request.HeatDiscountPercent, request.ColdThreshold, request.ColdDiscountPercent);
        if (updatedPricingConfig.IsFailure)
        {
            return Result.Failure<PricingConfigResponse>(updatedPricingConfig.Error);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var pricingMapped = _mapper.Map<PricingConfigResponse>(club.PricingConfig);
        return Result.Success(pricingMapped);
    }
}