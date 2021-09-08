using titaniumpassback.DataAccess;
using titaniumpassback.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace titaniumpassback.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public UserService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public User GetUser(int userID)
        {
            return _context.User.Find(userID);
        }

        public async Task<ICollection<UserVM>> GetUsers(User user)
        {
            ICollection<UserVM> users = await (from User in _context.User
                                               join userRoles in _context.UserRoles on User.Id equals userRoles.UserId
                                               join role in _context.Roles on userRoles.RoleId equals role.Id
                                               where User.CompanyID == user.CompanyID
                                               orderby User.Id
                                               select new UserVM
                                               {
                                                   User = User.Id,
                                                   UserName = User.UserName,
                                                   Email = User.Email,
                                                   Role = role.Name,
                                               }).ToListAsync();
            return users;
        }

        public async Task<ResponseUserVM> RegisterUser(User user, string password, string role)
        {
            ResponseUserVM res = new ResponseUserVM();
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var resultRole = await _userManager.AddToRoleAsync(user, role);
                if (resultRole.Succeeded)
                {
                    Save();
                    res.user = user;
                }
                else
                {
                    res.Message = "No se pudo registrar este usuario.";
                }
            }
            else
            {
                res.Message = "No se pudo registrar este usuario.";
            }
            return res;
        }

        public User UpdateMyProfile(UserVM userVM, int userLoguedID)
        {
            User user = _context.User.Find(userLoguedID);
            user.UserName = userVM.UserName;
            user.Email = userVM.Email;
            _context.Entry(user).State = EntityState.Modified;
            Save();
            return user;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool UserExist(UserVM user, bool edit)
        {
            if (edit)
            {

                if (_context.User.Any(c => c.UserName == user.UserName || c.Email == user.Email))
                {

                    if (_context.User.Where(u => u.UserName == user.UserName).FirstOrDefault().UserName == user.UserName || _context.User.Where(u => u.Email == user.Email).FirstOrDefault().Email == user.Email)
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            else
            {
                if (_context.User.Any(u => u.UserName == user.UserName || u.Email == user.Email))
                {
                    return true;
                }
                return false;
            }
        }
        public bool DeleteUser(int userID)
        {
            try
            {
                User user = _context.User.Find(userID);
                if (user == null)
                {
                    return false;
                }
                user.IsActive = false;
                _context.Entry(user).State = EntityState.Modified;
                Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
