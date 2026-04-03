using Application.Abstractions.Errors;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Deactivate;

internal sealed class DeactivateUserHandler : IRequestHandler<DeactivateUser, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    
    public DeactivateUserHandler(IIdentityService identityService, IUserRepository userRepository, IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(DeactivateUser request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            return Result.Failure<Unit>(UserErrors.Forbidden);     
        }
        
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Unit>(UserErrors.NotFound(request.UserId));
        }
        
        if (user.Username == "superadmin")
        {
            return Result.Failure<Unit>(IdentityErrors.CannotUpdateSuperAdmin);
        }
        
        user.Deactivate();
        
        var identityResult = await _identityService.UpdateUserStatusAsync(request.UserId, false);

        if (identityResult.IsFailure)
        {
            return Result.Failure<Unit>(identityResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}