﻿using Microsoft.AspNetCore.Identity;

namespace ChocoLuxAPI.Models
{
    public class UserModel:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
