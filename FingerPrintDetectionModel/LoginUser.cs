﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FingerPrintDetectionModel
{
    public class LoginUser:IdentityUser<long, UserLogin, UserRole, UserClaim>
    {
        public virtual SoundTrack Sound { get; set; }
        public virtual Plan Plan { get; set; }
        public virtual ICollection<RealUser> RealUsers { get; set; } = new List<RealUser>();

        public LoginUser()
        {
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<LoginUser, long> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

    }
    
}