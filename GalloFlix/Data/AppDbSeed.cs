using GalloFlix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GalloFlix.Data;

public class AppDbSeed
{
    public AppDbSeed(ModelBuilder builder)
    {
        #region  Populate Roles - Perfis de Usuário
        List<IdentityRole> roles = new()
        {
            new IdentityRole()
            {
               Id = Guid.NewGuid().ToString(),
               Name = "Administrador",
               NormalizedName = "ADMINISTRADOR" 
            },
            new IdentityRole()
            {
               Id = Guid.NewGuid().ToString(),
               Name = "Moderador",
               NormalizedName = "MODERADOR" 
            },
            new IdentityRole()
            {
               Id = Guid.NewGuid().ToString(),
               Name = "Usuário",
               NormalizedName = " USUÁRIO" 
            }
        };
        builder.Entity<IdentityRole>().HasData(roles);
        #endregion
        
        #region Populate AppUser - Usuários
        List<AppUser> users = new(){
            new AppUser(){
                Id = Guid.NewGuid().ToString(),
                Name = "Seu Nome Completo",
                DateOfBirth = DateTime.Parse("11/11/2005"),
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                LockoutEnabled = false,
                PhoneNumber = "14912345678",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                ProfilePicture = "/img/users/avatar.png"
            }
        };
        foreach (var user in users)
        {
            PasswordHasher<AppUser> pass = new();
            user.PasswordHash = pass.HashPassword(user, "@Etec123");
        }
        builder.Entity<AppUser>().HasData(users);
        #endregion
    
        #region Populate UserRole - Usuário com Perfil
        List<IdentityUserRole<string>> userRoles = new()
        {
            new IdentityUserRole<string>() {
                UserId = users[0].Id,
                RoleId = roles[0].Id
            }
        };
        builder.Entity<IdentityUserRole<string>>().HasData(userRoles);
        #endregion
    }     
}