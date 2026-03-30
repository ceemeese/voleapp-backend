using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Common.ValueObjects;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Register;

internal sealed class RegisterClubHandler : IRequestHandler<RegisterClub, Result<Guid>>
{
    private readonly IClubRepository _clubRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public RegisterClubHandler(IClubRepository clubRepository, IUnitOfWork unitOfWork,  IUserContext userContext)
    {
        _clubRepository = clubRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<Guid>> Handle(RegisterClub request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            return Result.Failure<Guid>(ClubErrors.Forbidden);     
        }
        
        var addressResult = Address.Create(request.Street, request.City, request.ZipCode, request.Country);
        if (addressResult.IsFailure)
        {
            return Result.Failure<Guid>(addressResult.Error);
        }

        var club = Club.Create(
            request.Name,
            request.Cif,
            addressResult.Value,
            request.PhoneNumber,
            request.Email
        );
        
        _clubRepository.Add(club);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success<Guid>(club.Id);
    }
}
