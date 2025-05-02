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
        public static ResultDTO<User> RegisterUser(PersonFullName fullName, Email email, HashedPassword hashedPassword, PhoneNumber phone, string profilePicture, string bio, string Location)
        {
            if (string.IsNullOrWhiteSpace(fullName.FirstName) && string.IsNullOrWhiteSpace(fullName.LastName))
                return ResultDTO<User>.Failure("Empty Error",null,"FullName name cannot be empty");
            if (string.IsNullOrWhiteSpace(phone.Value))
                return ResultDTO<User>.Failure("Empty Error", null, "Phone number cannot be empty");
            if (string.IsNullOrWhiteSpace(email.Value))
                return ResultDTO<User>.Failure("Empty Error", null, "Email cannot be empty");

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

            return ResultDTO<User>.Success("Created", newUser, "New User registered Successfully.");
        }

        public MessageDTO UpdateUserProfile(PersonFullName fullName, Email email, PhoneNumber phone, string bio, string location, string profilePicture)
        {
            if (string.IsNullOrWhiteSpace(fullName.FirstName) && string.IsNullOrWhiteSpace(fullName.FirstName))
                return MessageDTO.Failure("Empty Error", null, "FullName name cannot be empty");
            if (string.IsNullOrWhiteSpace(phone.Value))
                return MessageDTO.Failure("Empty Error", null, "Phone number cannot be empty");
            if (string.IsNullOrWhiteSpace(email.Value))
                return MessageDTO.Failure("Empty Error", null, "Email cannot be empty");
            FullName = fullName;
            Email = email;
            Phone = phone;
            Bio = bio;
            Location = location;
            ProfilePicture = profilePicture;
            Update();
            return MessageDTO.Success("Updated", "Your Profile Updated Successfully.");

        }
        public MessageDTO ChangePassword(string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                return MessageDTO.Failure("Empty Error", null, "New password cannot be empty or less than 6 charachters");
            if (!HashedPassword.Verify(newPassword))
                return MessageDTO.Failure("Error", null, "Current password is incorrect");
            this.HashedPassword = HashedPassword.CreateFromPlain(newPassword);

            return MessageDTO.Success("Updated", "Your Password Updated Successfully");

        }


    }
}
