using Application.Abstractions.Errors;
using Application.Abstractions.Interfaces;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Delete;

internal sealed class DeleteUserHandler : IRequestHandler<DeleteUser, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteUserHandler(IIdentityService identityService, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUser request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }
        
        if (user.Username == "superadmin")
        {
            return Result.Failure(IdentityErrors.CannotUpdateSuperAdmin);
        }
        
        user.Deactivate();
        
        var identityResult = await _identityService.UpdateUserStatusAsync(request.UserId, false);

        if (identityResult.IsFailure)
        {
            return Result.Failure(identityResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}