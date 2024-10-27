using WorkPulseAPI.Models;
public class UserService
{
    private readonly List<User> _users = new();

    public bool UserExists(string email) => _users.Any(u => u.Email == email);

    public void CreateUser(User user)
    {
        user.Id = _users.Count + 1; // Generación de ID sencilla para ejemplificar
        _users.Add(user);
    }
    public User? GetUserByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.Username == username);
    }
}
