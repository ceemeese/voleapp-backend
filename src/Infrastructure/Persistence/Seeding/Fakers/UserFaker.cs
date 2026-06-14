using Bogus;
using Domain.User;

namespace Infrastructure.Persistence.Seeding.Fakers;

public static class UserFaker
{
    public static User Generate(Guid identityId, string firstName, string lastName, string username, string email)
    {
        var faker = new Faker("es");

        return User.Create(
            identityId: identityId,
            dni: faker.Random.Replace("########?").ToUpper(),
            name: firstName,
            lastName: lastName,
            username: username,
            email: email,
            phoneNumber: faker.Phone.PhoneNumber("6########")
        );
    }
}