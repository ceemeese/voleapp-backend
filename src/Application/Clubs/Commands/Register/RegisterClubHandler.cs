using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Common.ValueObjects;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Register;

internal sealed class RegisterClubHandler : IRequestHandler<RegisterClub, Result<Guid>>
{
    private readonly IClubRepository _clubRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterClubHandler(IClubRepository clubRepository, IUnitOfWork unitOfWork)
    {
        _clubRepository = clubRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RegisterClub request, CancellationToken cancellationToken)
    {
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
