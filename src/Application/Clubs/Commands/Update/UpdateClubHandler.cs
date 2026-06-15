using Application.Abstractions.DTO.Club;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Common.ValueObjects;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Update;

internal sealed class UpdateClubHandler : IRequestHandler<UpdateClub, Result<ClubResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IMapper _mapper;

    public UpdateClubHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository, IUserContext userContext,  IClubMemberQueries clubMemberQueries, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _mapper = mapper;
    }

    public async Task<Result<ClubResponse>> Handle(UpdateClub request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<ClubResponse>(UserErrors.Forbidden);
        }

        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.Id, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<ClubResponse>(ClubErrors.Forbidden);
            }
        }

        var club = await _clubRepository.GetClubByIdAsync(request.Id, cancellationToken);

        if (club is null)
        {
            return Result.Failure<ClubResponse>(ClubErrors.NotFound(request.Id));
        }
        
        var addressResult = Address.Create(request.Street, request.City, request.ZipCode, request.Country);
        if (addressResult.IsFailure)
        {
            return Result.Failure<ClubResponse>(addressResult.Error);
        }
        
        club.UpdateProfile(
            request.Name, 
            addressResult.Value,
            request.PhoneNumber,
            request.Email
            );
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var clubMapped = _mapper.Map<ClubResponse>(club);
        return Result.Success(clubMapped);
    }
}