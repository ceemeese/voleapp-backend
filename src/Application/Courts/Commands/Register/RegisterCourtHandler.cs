using Application.Abstractions.Interfaces;
using Domain.Court;
using Domain.Court.Enum;
using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Register;

internal sealed class RegisterCourtHandler : IRequestHandler<RegisterCourt, Result<Guid>>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCourtHandler(ICourtRepository courtRepository, IUnitOfWork unitOfWork)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RegisterCourt request, CancellationToken cancellationToken)
    {
        var isDuplicate = await _courtRepository.ExistsByNameInClub(request.ClubId, request.Name, cancellationToken);
        if (isDuplicate)
        {
            return Result.Failure<Guid>(CourtErrors.DuplicateName(request.Name));
        }
        
        if (!Enum.TryParse<CourtType>(request.Type, ignoreCase: true, out var courtType))
        {
            return Result.Failure<Guid>(CourtErrors.InvalidType);
        }
        
        var courtResult = Court.Create(request.ClubId, request.Name, courtType, request.BasePrice, request.IsActive);

        if (courtResult.IsFailure)
        {
            return Result.Failure<Guid>(courtResult.Error);
        }
     
        _courtRepository.Add(courtResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success<Guid>(courtResult.Value.Id);
    }
}