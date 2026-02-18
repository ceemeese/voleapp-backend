using Domain.User;

namespace Application.Abstractions.Interfaces;

public interface ITokenProvider
{
    string Create(Guid userId, string email, string role);
}