using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.Enums;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : Entity, IAggrigate
    {
        public PersonFullName FullName { get; set; }
        public Email Email { get; set; }
        public HashedPassword HashedPassword { get; set; }
        public PhoneNumber Phone { get; set; }
        public string ProfilePicture { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }


        public virtual ICollection<UserRole> UserRoles { get; set; }

        private User()
        {
            
        }
        public static User RegisterUser(PersonFullName fullName, Email email, HashedPassword hashedPassword, PhoneNumber phone, string profilePicture, string bio, string Location)
        {
            if (string.IsNullOrWhiteSpace(fullName.FirstName) && string.IsNullOrWhiteSpace(fullName.FirstName))
                throw new DomainException("FullName name cannot be empty", (int)DomainExceptionEnums.CantBeNull);
            if (string.IsNullOrWhiteSpace(phone.Value))
                throw new DomainException("Phone number cannot be empty", (int)DomainExceptionEnums.CantBeNull);
            if (string.IsNullOrWhiteSpace(email.Value))
                throw new DomainException("Email cannot be empty", (int)DomainExceptionEnums.CantBeNull);

            User newUser = new User()
           {
               FullName = fullName,
               Email = email,
               HashedPassword = hashedPassword,
               Phone = phone,
               ProfilePicture = profilePicture,
               Bio = bio,
               Location = Location
           };

            return newUser;
        }

        public void UpdateUserProfile(PersonFullName fullName, Email email, PhoneNumber phone, string bio, string location)
        {
            if (string.IsNullOrWhiteSpace(fullName.FirstName) && string.IsNullOrWhiteSpace(fullName.FirstName))
                throw new DomainException("FullName name cannot be empty", (int)DomainExceptionEnums.CantBeNull);
            if (string.IsNullOrWhiteSpace(phone.Value))
                throw new DomainException("Phone number cannot be empty", (int)DomainExceptionEnums.CantBeNull);
            if (string.IsNullOrWhiteSpace(email.Value))
                throw new DomainException("Email cannot be empty", (int)DomainExceptionEnums.CantBeNull);
            FullName = fullName;
            Email = email;
            Phone = phone;
            Bio = bio;
            Location = location;
            Update();
        }
        public void ChangePassword(string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new DomainException("New password cannot be empty or less than 6 charachters", (int)DomainExceptionEnums.CantBeNull);
            if (!HashedPassword.Verify(newPassword))
                throw new DomainException("Current password is incorrect", (int)DomainExceptionEnums.Unauthorized);
            this.HashedPassword = HashedPassword.CreateFromPlain(newPassword);

        }


    }
}
