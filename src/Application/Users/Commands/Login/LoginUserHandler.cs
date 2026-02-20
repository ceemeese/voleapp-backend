using Application.Abstractions.DTO;
using Application.Abstractions.Interfaces;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Login;

internal sealed class LoginUserHandler : IRequestHandler<LoginUser, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identity;
    private readonly ITokenProvider _tokenProvider;
    
    public LoginUserHandler(IUserRepository userRepository, IIdentityService identity, ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _identity = identity;
        _tokenProvider = tokenProvider;
    }

    public async Task<Result<LoginResponse>> Handle(LoginUser request, CancellationToken cancellationToken)
    {
        var identityResult = await _identity.LoginAsync(request.Username, request.Password);

        if (identityResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(identityResult.Error);
        }
        
        var authUser = identityResult.Value;
        
        var token = _tokenProvider.Create(authUser.UserId, authUser.Email, authUser.Role);
        var refreshToken = Guid.NewGuid();
        
        var identityRefreshResult = await _identity.SetRefreshTokenAsync(authUser.UserId, refreshToken.ToString());
        if (identityRefreshResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(identityRefreshResult.Error);
        }
        
        return Result.Success(new LoginResponse(token, refreshToken));
    }
}