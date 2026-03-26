using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Court;
using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Update;

internal sealed class UpdateCourtHandler : IRequestHandler<UpdateCourt, Result>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    
    public UpdateCourtHandler(ICourtRepository courtRepository, IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(UpdateCourt request, CancellationToken cancellationToken)
    {
        //TODO:Implementar permisos de admin de club para editar cuando esté tabla MemberClub
        var court = await _courtRepository.GetCourtById(request.Id, cancellationToken);
        if (court is null)
        {
            return Result.Failure<Unit>(CourtErrors.NotFound(request.Id));
        }
        
        var isDuplicate = await _courtRepository.ExistsByNameInClub(request.ClubId, request.Name, cancellationToken);
        if (isDuplicate)
        {
            return Result.Failure<Guid>(CourtErrors.DuplicateName(request.Name));
        }

        var courtResult = court.UpdateProfile(request.Name, request.BasePrice);
        if (courtResult.IsFailure)
        {
            return Result.Failure<Guid>(courtResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}