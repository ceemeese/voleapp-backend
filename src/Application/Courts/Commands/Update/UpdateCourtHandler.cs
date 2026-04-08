using Application.Abstractions.DTO.Court;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club.Entities;
using Domain.Court;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Update;

internal sealed class UpdateCourtHandler : IRequestHandler<UpdateCourt, Result<CourtResponse>>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IMapper _mapper;
    
    public UpdateCourtHandler(ICourtRepository courtRepository, IUnitOfWork unitOfWork, IUserContext userContext, IClubMemberQueries clubMemberQueries, IMapper mapper)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _mapper = mapper;
    }

    public async Task<Result<CourtResponse>> Handle(UpdateCourt request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<CourtResponse>(UserErrors.Forbidden);
        }
        
        var court = await _courtRepository.GetCourtById(request.Id, cancellationToken);
        if (court is null)
        {
            return Result.Failure<CourtResponse>(CourtErrors.NotFound(request.Id));
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(court.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<CourtResponse>(ClubMemberErrors.Forbidden);
            }
        }
        
        var isDuplicate = await _courtRepository.ExistsByNameInClubExcludeId(court.ClubId, request.Name, request.Id, cancellationToken);
        if (isDuplicate)
        {
            return Result.Failure<CourtResponse>(CourtErrors.DuplicateName(request.Name));
        }

        var courtResult = court.UpdateProfile(request.Name, request.BasePrice);
        if (courtResult.IsFailure)
        {
            return Result.Failure<CourtResponse>(courtResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var courtMapped = _mapper.Map<CourtResponse>(court);
        return Result.Success(courtMapped);
    }
}