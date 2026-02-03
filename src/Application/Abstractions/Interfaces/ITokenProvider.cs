using Domain.User;

namespace Application.Abstractions.Interfaces;

public interface ITokenProvider
{
    string Create(User user, IList<string> roles);
}