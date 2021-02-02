using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniBook.Identity.Models;
using MiniBook.Identity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniBook.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {

        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerViewModel )
        {
            var user = new User
            {
                Email=registerViewModel.Email,
                UserName=registerViewModel.Email
            };
            var result =await  _userManager.CreateAsync(user,registerViewModel.Password);


          
            if (result.Succeeded)
            {
                return this.OkResult();
            }
            return this.ErrorResult(ErrorCode.BAD_REQUEST);

        }
    }
}
