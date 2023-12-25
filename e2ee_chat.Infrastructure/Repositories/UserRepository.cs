using e2ee_chat.Core.Interfaces.Repositories;
using e2ee_chat.Core.Models;
using e2ee_chat.Infrastructure.Converters;
using e2ee_chat.Infrastructure.Schemas;
using MongoDB.Bson;
using MongoDB.Driver;

namespace e2ee_chat.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbContext _context;
    private readonly UserConverter _converter;

    public UserRepository(UserConverter converter)
    {
        _context = new DbContext("UserDb");
        _converter = converter;
    }

    public void CreateUser(UserModel user)
    {
        try
        {
            var userSchema = _converter.Convert(user);
            var userExists = _context.Users.Find(u => u.Email == user.Email).FirstOrDefault();
            if (userExists != null)
            {
                Console.WriteLine("Email is already in use.");
                return;
            }
            _context.Users.InsertOne(userSchema);
            _context.UserKeys.InsertOne(new UserKeys
            {
                UserId = userSchema.Id.ToString()!,
                IV = user.IV,
                Passwordsalt = user.PasswordSalt
            });
            Console.WriteLine("Your new account is set up with email: " + user.Email+ " and username: " + user.Username);
        }
        catch (Exception e)
        {
            Console.WriteLine("Contact support.");
        }
    }

    public UserModel GetUser(string email)
    {
        var user = _context.Users.Find(user => user.Email == email).First();
        var keys = _context.UserKeys.Find(key => key.UserId == user.Id.ToString()).First();
        if (user == null || keys == null)
        {
            throw new InvalidDataException("Wrong email or password");
        }

        return _converter.Convert(user, keys);
    }
}