using MongoDB.Driver;
using System.Collections.Generic;

public class UserService
{
    private readonly IMongoCollection<User> _users;

    public UserService(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("users");
    }

    public User Register(string email, string password, string firstName, string lastName, List<string> interests)
    {
        if (_users.Find(u => u.Email == email).FirstOrDefault() != null)
        {
            return null;
        }

        var user = new User
        {
            Email = email,
            Password = password,
            FirstName = firstName,
            LastName = lastName,
            Interests = interests
        };
        _users.InsertOne(user);
        return user;
    }

    public User Login(string email, string password)
    {
        return _users.Find(u => u.Email == email && u.Password == password).FirstOrDefault();
    }

    public List<User> SearchUsers(string query)
    {
        return _users.Find(u => u.FirstName.Contains(query) || u.LastName.Contains(query)).ToList();
    }
}
