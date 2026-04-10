using System;
using Entities;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class UserTests
    {
        [Test]
        public void Name_ValidValue_IsAccepted()
        {
            var user = new User();

            user.Name = "Patrik";

            Assert.That(user.Name, Is.EqualTo("Patrik"));
        }

        [Test]
        public void Name_Empty_ThrowsArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.Name = "");

            Assert.That(ex!.Message, Is.EqualTo("Name cannot be empty."));
        }

        [Test]
        public void Name_Whitespace_ThrowsArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.Name = "   ");

            Assert.That(ex!.Message, Is.EqualTo("Name cannot be empty."));
        }

        [Test]
        public void Name_LessThan3Characters_ThrowsArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.Name = "Pa");

            Assert.That(ex!.Message, Is.EqualTo("Name must be at least 3 characters."));
        }

        [Test]
        public void Name_Exactly3Characters_IsAccepted()
        {
            var user = new User();

            user.Name = "Pat";

            Assert.That(user.Name, Is.EqualTo("Pat"));
        }

        [Test]
        public void Name_Exactly64Characters_IsAccepted()
        {
            var user = new User();
            var value = new string('a', 64);

            user.Name = value;

            Assert.That(user.Name, Is.EqualTo(value));
        }

        [Test]
        public void Name_MoreThan64Characters_ThrowsArgumentException()
        {
            var user = new User();
            var value = new string('a', 65);

            var ex = Assert.Throws<ArgumentException>(() => user.Name = value);

            Assert.That(ex!.Message, Is.EqualTo("Name cannot exceed 64 characters."));
        }

        [Test]
        public void Username_ValidValue_IsAccepted()
        {
            var user = new User();

            user.Username = "Username123";

            Assert.That(user.Username, Is.EqualTo("Username123"));
        }

        [Test]
        public void Username_Empty_ThrowsArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.Username = "");

            Assert.That(ex!.Message, Is.EqualTo("Username cannot be empty."));
        }

        [Test]
        public void Username_Whitespace_ThrowsArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.Username = "   ");

            Assert.That(ex!.Message, Is.EqualTo("Username cannot be empty."));
        }

        [Test]
        public void Username_LessThan3Characters_ThrowsArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.Username = "ab");

            Assert.That(ex!.Message, Is.EqualTo("Username must be at least 3 characters."));
        }

        [Test]
        public void Username_Exactly3Characters_IsAccepted()
        {
            var user = new User();

            user.Username = "Use";

            Assert.That(user.Username, Is.EqualTo("Use"));
        }

        [Test]
        public void Username_Exactly20Characters_IsAccepted()
        {
            var user = new User();
            var value = new string('a', 20);

            user.Username = value;

            Assert.That(user.Username, Is.EqualTo(value));
        }

        [Test]
        public void Username_MoreThan20Characters_ThrowsArgumentException()
        {
            var user = new User();
            var value = new string('a', 21);

            var ex = Assert.Throws<ArgumentException>(() => user.Username = value);

            Assert.That(ex!.Message, Is.EqualTo("Username cannot exceed 20 characters."));
        }

        [Test]
        public void Username_WithSpace_ThrowsArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.Username = "user name");

            Assert.That(ex!.Message, Is.EqualTo("Username cannot contain spaces."));
        }

        [Test]
        public void Email_ValidValue_IsAccepted()
        {
            var user = new User();

            user.Email = "email123@gmail.com";

            Assert.That(user.Email, Is.EqualTo("email123@gmail.com"));
        }

        [Test]
        public void Email_Empty_ThrowsArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.Email = "");

            Assert.That(ex!.Message, Is.EqualTo("Email cannot be empty."));
        }

        [Test]
        public void Email_Whitespace_ThrowsArgumentException()
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.Email = "   ");

            Assert.That(ex!.Message, Is.EqualTo("Email cannot be empty."));
        }

        [TestCase("d")]
        [TestCase("Email.gmail.com")]
        [TestCase("user@")]
        [TestCase("@domain.com")]
        [TestCase("user domain.com")]
        [TestCase("user@domain")]
        public void Email_InvalidFormat_ThrowsArgumentException(string invalidEmail)
        {
            var user = new User();

            var ex = Assert.Throws<ArgumentException>(() => user.Email = invalidEmail);

            Assert.That(ex!.Message, Is.EqualTo("Email is invalid. Format: user@host.domain"));
        }

        [TestCase("user@test.com")]
        [TestCase("first.last@domain.org")]
        [TestCase("name123@email.co.uk")]
        public void Email_ValidFormats_AreAccepted(string validEmail)
        {
            var user = new User();

            user.Email = validEmail;

            Assert.That(user.Email, Is.EqualTo(validEmail));
        }

        [Test]
        public void Password_AnyValue_IsStored()
        {
            var user = new User();

            user.Password = "Password123";

            Assert.That(user.Password, Is.EqualTo("Password123"));
        }

        [Test]
        public void Password_EmptyValue_IsStored()
        {
            var user = new User();

            user.Password = "";

            Assert.That(user.Password, Is.EqualTo(string.Empty));
        }

        [Test]
        public void User_AllValidProperties_CanBeAssigned()
        {
            var user = new User
            {
                Name = "Patrik",
                Username = "Patrik123",
                Email = "patrik@example.com",
                Password = "Password123"
            };

            Assert.That(user.Name, Is.EqualTo("Patrik"));
            Assert.That(user.Username, Is.EqualTo("Patrik123"));
            Assert.That(user.Email, Is.EqualTo("patrik@example.com"));
            Assert.That(user.Password, Is.EqualTo("Password123"));
        }
    }
}