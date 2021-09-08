using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using titaniumpassback.DataAccess;
using titaniumpassback.Models;
using titaniumpassback.Services;

namespace titaniumpassback.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserService _userService;
        private readonly CompanyService _companyService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UserController(UserService userService,
            CompanyService companyService, IConfiguration config,
            SignInManager<User> signInManager, UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _context = context;
            _userService = userService;
            _companyService = companyService;
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register(UserVM userVM)
        {
            if (userVM.CompanyName == null || userVM.UserName == null || userVM.Password == null || userVM.Email == null)
            {
                return BadRequest(new { message = "Faltan datos." });
            }
            if (_userService.UserExist(userVM, false))
            {
                return BadRequest(new { message = "El usuario ya existe." });
            }
            if (_companyService.CompanyExist(userVM.CompanyName))
            {
                return BadRequest(new { message = "La compañia ya existe." });
            }
            using (IDbContextTransaction dbTran = _context.Database.BeginTransaction())
            {
                Company newCompany = new Company
                {
                    Name = userVM.CompanyName
                };
                Company company = _companyService.CreateCompany(newCompany);
                if (company != null)
                {
                    User newUser = new User
                    {
                        Email = userVM.Email,
                        EmailConfirmed = true,
                        UserName = userVM.UserName,
                        CompanyID = company.CompanyID
                    };
                    ResponseUserVM res = await _userService.RegisterUser(newUser, userVM.Password, userVM.Role);
                    if (res.user == null)
                    {
                        dbTran.Rollback();
                        return BadRequest(res.Message);
                    }
                    userVM.User = res.user.Id;
                    dbTran.Commit();
                    return Ok(userVM);
                }
            }
            return BadRequest("No fue posible crear al usuario.");
        }
    }
}
