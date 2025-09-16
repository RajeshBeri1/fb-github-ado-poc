using Bogus;
using Lib.WebAPI.DbModels;

namespace Tests.WebAPI.Common
{
    internal static class FakeData
    {
        public static Faker<User> User = new Faker<User>()
            .RuleFor(x => x.DisplayName, x => x.Person.FullName)
            .RuleFor(x => x.Name, x => x.Person.Email);
    }
}