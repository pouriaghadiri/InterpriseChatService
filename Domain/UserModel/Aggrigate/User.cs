using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.Enums;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Guid? ActiveDepartmentId { get; set; }


        public virtual ICollection<UserRole> UserRoles { get; set; } = new Collection<UserRole>();
        public virtual Department ActiveDepartment { get; set; }
        private User()
        {
            
        }
        #region Users Methodes
        public static ResultDTO<User> RegisterUser(PersonFullName fullName, Email email, HashedPassword hashedPassword, PhoneNumber phone, string profilePicture, string bio, string Location)
        {
            if (string.IsNullOrWhiteSpace(fullName?.FirstName) && string.IsNullOrWhiteSpace(fullName?.LastName))
                return ResultDTO<User>.Failure("Empty Error",null,"FullName name cannot be empty");
            if (string.IsNullOrWhiteSpace(phone?.Value))
                return ResultDTO<User>.Failure("Empty Error", null, "Phone number cannot be empty");
            if (string.IsNullOrWhiteSpace(email?.Value))
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
            if (string.IsNullOrWhiteSpace(fullName?.FirstName) && string.IsNullOrWhiteSpace(fullName?.FirstName))
                return MessageDTO.Failure("Empty Error", null, "FullName name cannot be empty");
            if (string.IsNullOrWhiteSpace(phone?.Value))
                return MessageDTO.Failure("Empty Error", null, "Phone number cannot be empty");
            if (string.IsNullOrWhiteSpace(email?.Value))
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
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
                return MessageDTO.Failure("Empty Error", null, "New password cannot be empty or less than 6 charachters");
            if (!HashedPassword.Verify(currentPassword))
                return MessageDTO.Failure("Error", null, "Current password is incorrect");
            this.HashedPassword = HashedPassword.CreateFromPlain(newPassword);
            Update();
            return MessageDTO.Success("Updated", "Your Password Updated Successfully");

        }

        public MessageDTO SetActiveDepartment(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
                return MessageDTO.Failure("Invalid Department", null, "Department ID cannot be empty");

            ActiveDepartmentId = departmentId;
            Update();
            return MessageDTO.Success("Updated", "Active department updated successfully");
        }
        #endregion

        #region Roles Methods

        public MessageDTO AssignRoleToUser(Role role, Department department)
        {
            if (role == null)
            {
                return MessageDTO.Failure("Invalid Role", null, "Role is required!");
            }
            if (department == null)
            {
                return MessageDTO.Failure("Invalid department", null, "Department is required!");
            }
             

            if (UserRoles != null && UserRoles.Any(a => a.UserRoleInDepartments.Any()))
            {
                var alreadyAssigned = UserRoles.SelectMany(s => s.UserRoleInDepartments)
                                           .Where(x => x.Department.Id == department.Id && x.UserRole.Role.Id == role.Id)
                                           .Any();
                if (alreadyAssigned)
                {
                    return MessageDTO.Failure("Doublicate Role", null, "User already has this role in the specified department.");
                }
            }

            var userRole = UserRoles.FirstOrDefault(a => a.Role.Id == role.Id);
            if (userRole == null)
            {
                userRole = new UserRole()
                {
                    Role = role,
                    User = this
                };
                UserRoles.Add(userRole);
            }

            var userRoleInDepartment = new UserRoleInDepartment()
            {
                UserRole = userRole,
                Department = department,

            };
            userRole.UserRoleInDepartments.Add(userRoleInDepartment);
            return MessageDTO.Success("Assigned", "Role assigned to user in department.");

        }

        #endregion


    }
}
