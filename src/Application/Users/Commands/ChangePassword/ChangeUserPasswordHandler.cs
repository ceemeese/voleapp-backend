using Application.Abstractions.Interfaces;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.ChangePassword;

internal sealed class ChangeUserPasswordHandler : IRequestHandler<ChangeUserPassword, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IUserContext _userContext;
    
    public ChangeUserPasswordHandler(IIdentityService identityService, IUserContext userContext)
    {
        _identityService = identityService;
        _userContext = userContext;
    }

    public async Task<Result> Handle(ChangeUserPassword request, CancellationToken cancellationToken)
    {
        Guid currentUserId = _userContext.UserId;
        
        var identityResult = await _identityService.ChangePasswordAsync(currentUserId, request.OldPassword, request.NewPassword);

        if (identityResult.IsFailure)
        {
            return Result.Failure(identityResult.Error);
        }
        
        return Result.Success();
    }
}