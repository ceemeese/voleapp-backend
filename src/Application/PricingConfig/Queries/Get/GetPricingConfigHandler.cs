using Application.Abstractions.DTO.PricingConfig;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Club.Entities;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.PricingConfig.Queries.Get;

internal sealed class GetPricingConfigHandler : IRequestHandler<GetPricingConfig, Result<PricingConfigResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IClubRepository _clubRepository;
    private readonly IClubMemberQueries _clubMemberQueries;

    public GetPricingConfigHandler(IMapper mapper, IUserContext userContext, IClubRepository clubRepository,  IClubMemberQueries clubMemberQueries)
    {
        _mapper = mapper;
        _userContext = userContext;
        _clubRepository = clubRepository;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result<PricingConfigResponse>> Handle(GetPricingConfig request, CancellationToken cancellationToken)
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

        var pricingMapped = _mapper.Map<PricingConfigResponse>(club.PricingConfig);
        return Result.Success(pricingMapped);
    }
}