using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Activate;

internal sealed class ActivateUserHandler : IRequestHandler<ActivateUser, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IIdentityService _identityService;


    public ActivateUserHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IUserContext userContext,  IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _userContext = userContext;
        _identityService = identityService;
    }

    public async Task<Result> Handle(ActivateUser request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            return Result.Failure<Unit>(ClubErrors.Forbidden);     
        }
        
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Unit>(UserErrors.NotFound(request.UserId));
        }
        
        user.Activate();
        
        var identityResult = await _identityService.UpdateUserStatusAsync(request.UserId, true);

        if (identityResult.IsFailure)
        {
            return Result.Failure<Unit>(identityResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}