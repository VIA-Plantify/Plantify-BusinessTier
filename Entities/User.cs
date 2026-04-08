using System.Text.RegularExpressions;

namespace Entities;

public class User
{
    private string _name = string.Empty;
    private string _username = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty.");

            if (value.Length < 3)
                throw new ArgumentException("Name must be at least 3 characters.");

            if (value.Length > 64)
                throw new ArgumentException("Name cannot exceed 64 characters.");

            _name = value;
        }
    }

    public string Username
    {
        get => _username;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Username cannot be empty.");

            if (value.Length < 3)
                throw new ArgumentException("Username must be at least 3 characters.");

            if (value.Length > 20)
                throw new ArgumentException("Username cannot exceed 20 characters.");

            if (value.Contains(" "))
                throw new ArgumentException("Username cannot contain spaces.");

            _username = value;
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            // [ALT3]
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty.");

            // we can also use this regex if we want to be fancy @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
            if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Email is invalid. Format: user@host.domain");

            _email = value;
        }
    }

    public string Password
    {
        get => _password;
        set => _password = value; //Password check inside webapi , here will be stored the hash
    }
}
