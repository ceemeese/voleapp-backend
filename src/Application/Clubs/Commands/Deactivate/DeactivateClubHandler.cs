using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Delete;

internal sealed class DeactivateClubHandler : IRequestHandler<DeactivateClub, Result>
{
     private readonly IUnitOfWork _unitOfWork;
     private readonly IClubRepository _clubRepository;
     private readonly IUserContext _userContext;

     public DeactivateClubHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository, IUserContext userContext)
     {
          _unitOfWork = unitOfWork;
          _clubRepository = clubRepository;
          _userContext = userContext;
     }

     public async Task<Result> Handle(DeactivateClub request, CancellationToken cancellationToken)
     {
          if (!_userContext.IsOnlySuperadmin())
          {
               return Result.Failure(ClubErrors.Forbidden);     
          }

          var club = await _clubRepository.GetClubById(request.ClubId, cancellationToken);
          
          if (club is null)
          {
               return Result.Failure(ClubErrors.NotFound(request.ClubId));
          }
          
          club.Deactivate();
          await _unitOfWork.SaveChangesAsync(cancellationToken);
          return Result.Success();
     }
}