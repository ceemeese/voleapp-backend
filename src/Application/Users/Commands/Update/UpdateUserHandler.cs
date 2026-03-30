using Application.Abstractions.Errors;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Update;

internal sealed class UpdateUserHandler : IRequestHandler<UpdateUser, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IUserContext _userContext;

    public UpdateUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IIdentityService identityService, IUserContext userContext)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _userContext = userContext;
        
    }

    public async Task<Result> Handle(UpdateUser request, CancellationToken cancellationToken)
    {
        
        if (!_userContext.IsOwnerOrSuperadmin(request.Id))
        {
            return Result.Failure(UserErrors.Forbidden);
        }
        
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Unit>(UserErrors.NotFound(request.Id));
        }
        
        if (user.Username == "superadmin")
        {
            return Result.Failure<Unit>(IdentityErrors.CannotUpdateSuperAdmin);
        }
        
        var oldUserName = user.Username;
        
        var identityResult = await _identityService.UpdateUserProfileAsync(request.Id, oldUserName, request.Username, request.Email);

        if (identityResult.IsFailure)
        {
            return Result.Failure<Unit>(identityResult.Error);
        }
        
        user.UpdateProfile(request.Username, request.PhoneNumber, request.Email);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}