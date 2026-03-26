using Application.Abstractions.DTO.Auth;
using Application.Abstractions.Interfaces;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetToken;

internal sealed class GetTokenHandler : IRequestHandler<GetToken, Result<LoginResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenProvider _tokenProvider;

    public GetTokenHandler(IIdentityService identityService, ITokenProvider tokenProvider)
    {
        _identityService = identityService;
        _tokenProvider = tokenProvider;
    }

    public async Task<Result<LoginResponse>> Handle(GetToken request, CancellationToken cancellationToken)
    {
        var identityResult = await _identityService.ValidateRefreshToken(request.RefreshToken.ToString());
        if (identityResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(identityResult.Error);
        }
        
        var authUser = identityResult.Value;

        var token = _tokenProvider.Create(authUser.UserId, authUser.Email, authUser.Role);
        var newRefreshToken = Guid.NewGuid();

        var refreshTokenResult = await _identityService.SetRefreshTokenAsync(authUser.UserId, newRefreshToken.ToString());

        if (refreshTokenResult.IsFailure)
        {
            Result.Failure<LoginResponse>(refreshTokenResult.Error);
        }

        return Result.Success(new LoginResponse(token, newRefreshToken));
    }
}