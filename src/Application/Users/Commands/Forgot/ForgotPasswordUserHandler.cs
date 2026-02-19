using Application.Abstractions.DTO;
using Application.Abstractions.Interfaces;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Forgot;

internal sealed class ForgotPasswordUserHandler : IRequestHandler<ForgotPasswordUser, Result<ForgotResponse>>
{
    private readonly IIdentityService _identityService;

    public ForgotPasswordUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<ForgotResponse>> Handle(ForgotPasswordUser request, CancellationToken cancellationToken)
    {
        var identityResult = await _identityService.ForgotPasswordAsync(request.Email);

        if (identityResult.IsFailure)
        {
            return Result.Failure<ForgotResponse>(identityResult.Error);
        }
        
        var (token, email) = identityResult.Value;
        
        //envio de mail
        return Result.Success(new ForgotResponse(token, email));
    }
}