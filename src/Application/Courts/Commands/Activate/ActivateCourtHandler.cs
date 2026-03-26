using Application.Abstractions.Interfaces;
using Domain.Court;
using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Activate;

internal sealed class ActivateCourtHandler : IRequestHandler<ActivateCourt, Result>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public ActivateCourtHandler(ICourtRepository courtRepository, IUnitOfWork unitOfWork)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ActivateCourt request, CancellationToken cancellationToken)
    {
        // TODO: validacion si es superadmin o el propio dueno del club
        var court = await _courtRepository.GetCourtById(request.CourtId, cancellationToken);

        if (court is null)
        {
            return Result.Failure(CourtErrors.NotFound(request.CourtId));
        }
        
        court.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}