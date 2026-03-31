using Application.Abstractions.DTO.CourtEvent;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Court;
using Domain.Court.Entities;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Queries.GetEventById;

internal sealed class GetEventByIdHandler : IRequestHandler<GetEventById, Result<CourtEventCompleteResponse>>
{
   private readonly IClubMemberQueries _clubMemberQueries;
   private readonly IUserContext _userContext;
   private readonly ICourtRepository _courtRepository;
   private readonly ICourtEventQueries _courtEventQueries;
   
   public GetEventByIdHandler(IClubMemberQueries clubMemberQueries,  IUserContext userContext,  ICourtRepository courtRepository,  ICourtEventQueries courtEventQueries)
   {
      _clubMemberQueries = clubMemberQueries;
      _userContext = userContext;
      _courtRepository = courtRepository;
      _courtEventQueries = courtEventQueries;
   }

   public async Task<Result<CourtEventCompleteResponse>> Handle(GetEventById request, CancellationToken cancellationToken)
   {
      if (!_userContext.IsAnyAdmin())
      {
         return Result.Failure<CourtEventCompleteResponse>(UserErrors.Forbidden);
      }
      
      var court = await _courtRepository.GetCourtById(request.CourtId, cancellationToken);
      if (court is null)
      {
         return Result.Failure<CourtEventCompleteResponse>(CourtErrors.NotFound(request.CourtId));
      }
      
      if (!_userContext.IsOnlySuperadmin())
      {
         var hasPermission = await _clubMemberQueries.IsAdminInClub(court.ClubId, _userContext.UserId, cancellationToken);

         if (!hasPermission)
         {
            return Result.Failure<CourtEventCompleteResponse>(ClubErrors.Forbidden);
         }
      }

      var eventCourt = await _courtEventQueries.GetByIdAsync(court.Id, request.EventId, cancellationToken);
      if (eventCourt is null)
      {
         return Result.Failure<CourtEventCompleteResponse>(CourtEventErrors.NotFound(request.EventId));
      }

      return Result.Success(eventCourt);
   }
}