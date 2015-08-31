﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CMPSAdvisingDB.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public new DbSet<ApplicationRole> Roles { get; set; }
        public ApplicationDbContext()
            : base("IdentityCMPSAdvising", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("ModelBuilder is NULL");
            }

            base.OnModelCreating(modelBuilder);

            //Defining the keys and relations
            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            modelBuilder.Entity<ApplicationRole>().HasKey<string>(r => r.Id).ToTable("AspNetRoles");
            modelBuilder.Entity<ApplicationUser>().HasMany<ApplicationUserRole>((ApplicationUser u) => u.UserRoles);
            modelBuilder.Entity<ApplicationUserRole>().HasKey(r => new { UserId = r.UserId, RoleId = r.RoleId }).ToTable("AspNetUserRoles");
        }

//        public bool Seed(ApplicationDbContext context)
//        {
//#if DEBUG
//            bool success = false;

//            ApplicationRoleManager _roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

//            success = this.CreateRole(_roleManager, "Admin", "Global Access");
//            if (!success == true) return success;

//            success = this.CreateRole(_roleManager, "Professor", "Edit students records");
//            if (!success == true) return success;

//            success = this.CreateRole(_roleManager, "Student", "Edit his/her own records ");
//            if (!success) return success;

//            // Create my debug objects here

//            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

//            ApplicationUser user = new ApplicationUser();
//            PasswordHasher passwordHasher = new PasswordHasher();

//            user.UserName = "lili.xu@selu.edu";
//            user.Email = "lili.xu@selu.edu";

//            IdentityResult result = userManager.Create(user, "Lily@123");

//            success = this.AddUserToRole(userManager, user.Id, "Admin");
//            if (!success) return success;

//            success = this.AddUserToRole(userManager, user.Id, "Professor");
//            if (!success) return success;

//            success = this.AddUserToRole(userManager, user.Id, "Student");
//            if (!success) return success;

//            return success;
//#endif
//        }

        public bool RoleExists(ApplicationRoleManager roleManager, string name)
        {
            return roleManager.RoleExists(name);
        }

        public bool CreateRole(ApplicationRoleManager _roleManager, string name, string description = "")
        {
            var idResult = _roleManager.Create<ApplicationRole, string>(new ApplicationRole(name, description));
            return idResult.Succeeded;
        }

        public bool AddUserToRole(ApplicationUserManager _userManager, string userId, string roleName)
        {
            var idResult = _userManager.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }

        public void ClearUserRoles(ApplicationUserManager userManager, string userId)
        {
            var user = userManager.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();

            currentRoles.AddRange(user.UserRoles);
            foreach (ApplicationUserRole role in currentRoles)
            {
                userManager.RemoveFromRole(userId, role.Role.Name);
            }
        }


        public void RemoveFromRole(ApplicationUserManager userManager, string userId, string roleName)
        {
            userManager.RemoveFromRole(userId, roleName);
        }

        public void DeleteRole(ApplicationDbContext context, ApplicationUserManager userManager, string roleId)
        {
            var roleUsers = context.Users.Where(u => u.UserRoles.Any(r => r.RoleId == roleId));
            var role = context.Roles.Find(roleId);

            foreach (var user in roleUsers)
            {
                this.RemoveFromRole(userManager, user.Id, role.Name);
            }
            context.Roles.Remove(role);
            context.SaveChanges();
        }

        /// <summary>
        /// Context Initializer
        /// </summary>
        //public class DropCreateAlwaysInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
        //{
        //    protected override void Seed(ApplicationDbContext context)
        //    {
        //        context.Seed(context);

        //        base.Seed(context);
        //    }
        //}

        public System.Data.Entity.DbSet<CMPSAdvisingDB.ViewModels.RoleViewModel> RoleViewModels { get; set; }
    } 
}