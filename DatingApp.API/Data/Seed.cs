using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(UserManager<User> usermanager, RoleManager<Role> roleManager)
        {
            if(!usermanager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                //Create some roles

                var roles = new List<Role>
                {
                    new Role{Name = "Member"},
                    new Role{Name = "Admin"},
                    new Role{Name = "Moderator"},
                    new Role{Name = "VIP"}
                };

                foreach(var r in roles)
                {
                    roleManager.CreateAsync(r).Wait();
                }
                
                foreach(var user in users)
                {
                    usermanager.CreateAsync(user, "password").Wait();
                    usermanager.AddToRoleAsync(user, "Member");
                }

                var adminUser = new User
                {
                    UserName = "Admin"
                };

                var result = usermanager.CreateAsync(adminUser, "password").Result;

                if(result.Succeeded)
                {
                    var admin = usermanager.FindByNameAsync("Admin").Result;
                    usermanager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
                }

            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //The salt here is like a key to the hash
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }            
        }
    }
}