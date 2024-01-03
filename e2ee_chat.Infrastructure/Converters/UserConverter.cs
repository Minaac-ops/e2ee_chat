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