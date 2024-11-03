using WorkPulseAPI.Models;
using System;
using System.Linq;
using System.Collections.Generic;

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

    // Método para actualizar el usuario (por ejemplo, después de cambiar el refresh token)
    public void UpdateUser(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            // Actualizar las propiedades relevantes del usuario
            existingUser.RefreshToken = user.RefreshToken;
            existingUser.RefreshTokenExpiration = user.RefreshTokenExpiration;
            existingUser.Email = user.Email;
            existingUser.Username = user.Username;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.Role = user.Role;
        }
    }

    // Método para obtener un usuario a partir del refresh token
    public User? GetUserByRefreshToken(string refreshToken)
    {
        return _users.FirstOrDefault(u => u.RefreshToken == refreshToken);
    }
}

