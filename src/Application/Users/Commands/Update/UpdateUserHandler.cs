using Application.Abstractions.Errors;
using Application.Abstractions.Interfaces;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Update;

internal sealed class UpdateUserHandler : IRequestHandler<UpdateUser, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public UpdateUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<Result<Unit>> Handle(UpdateUser request, CancellationToken cancellationToken)
    {
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
        return Result.Success<Unit>(Unit.Value);
    }
}