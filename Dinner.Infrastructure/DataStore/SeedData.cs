using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dinner.Infrastructure.DataStore
{
    public class SeedData
    {
        public static async Task InitializeLunch(IServiceProvider provider)
        {
            ApplicationDbContext context = provider.GetService<ApplicationDbContext>();
        }
    }
}
