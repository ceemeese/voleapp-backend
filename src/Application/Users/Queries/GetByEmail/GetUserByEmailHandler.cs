using Application.Abstractions.DTO;
using Application.Abstractions.DTO.User;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetByEmail;

internal sealed class GetUserByEmailHandler : IRequestHandler<GetUserByEmail, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public GetUserByEmailHandler(IUserRepository userRepository, IUserContext userContext)
    {
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<Result<UserResponse>> Handle(GetUserByEmail request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin)
        {
            return Result.Failure<UserResponse>(UserErrors.Forbidden);   
        }
        
        var user = _userContext.IsOnlySuperadmin()
            ? await _userRepository.GetByEmailActiveAsync(request.Email, cancellationToken)
            : await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFoundByEmail);
        }
        
        var userResponse = new UserResponse(
            user.Id,
            user.Name,
            user.LastName,
            user.Username,
            user.PhoneNumber,
            user.Email,
            user.IsActive,
            user.CreatedAt);
        
        return Result.Success(userResponse);
    }
}