namespace Domain.Roles;

public interface IRole
{
    Guid Id { get; }
    string Name { get; }
}

public static class Role
{
    public class SuperAdmin : IRole
    {
        public static readonly Guid Id = new("7c9e66ab-7839-47e2-9383-718693c04291");
        public const string Name = "SuperAdmin";
        
        Guid IRole.Id => Id;
        string IRole.Name => Name;
    }
    
    public class Admin : IRole
    {
        public static readonly Guid Id = new("7c9e66ab-7839-47e2-9383-718693c04292");
        public const string Name = "Admin";
        
        Guid IRole.Id => Id;
        string IRole.Name => Name;
    }

    public class User : IRole
    {
        public static readonly Guid Id = new("7c9e66ab-7839-47e2-9383-718693c04293");
        public const string Name = "User";
        
        Guid IRole.Id => Id;
        string IRole.Name => Name;
    }
}