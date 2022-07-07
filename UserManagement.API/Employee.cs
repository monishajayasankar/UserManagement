using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.API
{
    public class Employee 
    {
        public int Id { get; set; }
        public AppUser AppUser { get; set; }


    }
}
