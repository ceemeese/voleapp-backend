using Application.Abstractions.Interfaces;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Reset;

internal sealed class ResetPasswordUserHandler : IRequestHandler<ResetPasswordUser, Result>
{
    private readonly IIdentityService _identityService;
    
    public ResetPasswordUserHandler(IIdentityService identityService, IUserRepository userRepository)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(ResetPasswordUser request, CancellationToken cancellationToken)
    {
        var identityResult = await _identityService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);

        if (identityResult.IsFailure)
        {
            return Result.Failure(identityResult.Error);
        }
        
        return Result.Success();
    }
}