namespace Domain.User;

public interface IUserRepository
{
    Task<List<User>> GetAll(CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<bool> ExistByDniAsync(string dni, CancellationToken cancellationToken);
    Task<bool> ExistByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken);
    void Add(User user);
    void Delete(User user);
}