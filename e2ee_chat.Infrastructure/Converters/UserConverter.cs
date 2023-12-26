using e2ee_chat.Core.Models;
using e2ee_chat.Infrastructure.Schemas;

namespace e2ee_chat.Infrastructure.Converters;

public class UserConverter
{
    public User Convert(UserModel userModel)
    {
        return new User
        {
            Email = userModel.Email,
            Username = userModel.Username,
            EncryptedRandom = userModel.EncryptedRandom,
            RandomString = userModel.Random,
        };
    }

    public IEnumerable<UserModel> Convert(List<User> users, List<UserKeys> keys)
    {
        var combinedUsers = users.Select(user => new UserModel
        {
            Id = user.Id.ToString()!,
            Email = user.Email,
            Username = user.Username,
            Random = user.RandomString,
            EncryptedRandom = user.EncryptedRandom,
            IV = keys.First(k => k.UserId == user.Id.ToString())!.IV,
            PasswordSalt = keys.First(k => k.UserId == user.Id.ToString())!.Passwordsalt
        });
        return combinedUsers;
    }

    public UserModel Convert(User userSchema, UserKeys keys)
    {
        var combinedUser = new UserModel
        {
            Id = userSchema.Id.ToString()!,
            Email = userSchema.Email,
            Username = userSchema.Username,
            Random = userSchema.RandomString,
            EncryptedRandom = userSchema.EncryptedRandom,
            IV = keys.IV,
            PasswordSalt = keys.Passwordsalt
        };
        return combinedUser;
    }
}