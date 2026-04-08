using Application.Abstractions.DTO.CourtEvent;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Court;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Commands.Register;

internal sealed class RegisterCourtEventHandler : IRequestHandler<RegisterCourtEvent, Result<CourtEventResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly ICourtRepository _courtRepository;
    private readonly IMapper _mapper;
    
    public RegisterCourtEventHandler(IUnitOfWork unitOfWork, IUserContext userContext,  IClubMemberQueries clubMemberQueries,  ICourtRepository courtRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _courtRepository = courtRepository;
        _mapper = mapper;
    }

    public async Task<Result<CourtEventResponse>> Handle(RegisterCourtEvent request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<CourtEventResponse>(UserErrors.Forbidden);
        }
        
        var court = await _courtRepository.GetCourtWithEventsByDateRangeAsync(request.CourtId,request.StartTime, cancellationToken);
        if (court is null)
        {
            return Result.Failure<CourtEventResponse>(CourtErrors.NotFound(request.CourtId));
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(court.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<CourtEventResponse>(ClubErrors.Forbidden);
            }
        }
        
        var courtEventResult = court.AddEvent(request.StartTime, request.EndTime, request.EventName, request.Description);
        if (courtEventResult.IsFailure)
        {
            return Result.Failure<CourtEventResponse>(courtEventResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var courtEventMapped = _mapper.Map<CourtEventResponse>(courtEventResult.Value);
        return Result.Success(courtEventMapped);
    }
}