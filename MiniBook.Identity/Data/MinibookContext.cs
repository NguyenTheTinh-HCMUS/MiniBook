using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniBook.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniBook.Identity.Data
{
    public class MinibookContext: IdentityDbContext<IdentityUser>
    {
        public MinibookContext(DbContextOptions<MinibookContext> options): base(options)
        {

        }
        
    }
}
