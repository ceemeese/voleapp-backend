using Application.Abstractions.Interfaces;
using Domain.Court;
using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Delete;

internal sealed class DeleteCourtHandler : IRequestHandler<DeleteCourt, Result>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCourtHandler(ICourtRepository courtRepository, IUnitOfWork unitOfWork)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCourt request, CancellationToken cancellationToken)
    {
        // TODO: validacion si es superadmin o el propio dueno del club
        var court = await _courtRepository.GetCourtById(request.CourtId, cancellationToken);
        
        if (court is null)
        {
            return Result.Failure(CourtErrors.NotFound(request.CourtId));    
        }
        
        court.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}