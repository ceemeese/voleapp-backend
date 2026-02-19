using System.Security.Claims;
using Application.Abstractions.Interfaces;
using Domain.Roles;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
   private sealed class UserContextUnavailableException : Exception
   {
      public UserContextUnavailableException() : base("User context no disponible")
      {
      }
   }
   
   private readonly IHttpContextAccessor _httpContextAccessor;
   public UserContext(IHttpContextAccessor httpContextAccessor)
   {
      _httpContextAccessor = httpContextAccessor;
   }

   public Guid UserId => Guid.TryParse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),
      out var userId)
      ? userId
      : throw new UserContextUnavailableException();

   public bool IsAdmin => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) == Role.Admin.Name;
   public bool IsSuperAdmin => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) == Role.SuperAdmin.Name;
}