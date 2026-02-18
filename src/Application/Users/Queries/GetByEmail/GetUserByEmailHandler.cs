using Application.Abstractions.DTO;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetByEmail;

internal sealed class GetUserByEmailHandler : IRequestHandler<GetUserByEmail, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserResponse>> Handle(GetUserByEmail request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFoundByEmail);
        }
        
        var userResponse = new UserResponse(
            user.Id,
            user.Dni,
            user.Name,
            user.LastName,
            user.Username,
            user.PhoneNumber,
            user.Email,
            user.IsActive);
        
        return Result.Success(userResponse);
    }
}