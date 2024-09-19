using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TaskManagementApp.Areas.Identity.Data;

public class TMSUser : IdentityUser
{
    public TMSUser()
    {
        this.Id = base.Id;
    }
    public string Id { get; set; }
    [ForeignKey("Role")]
    public string? RoleId { get; set; }
    public virtual IdentityRole? Role { get; set; }
}

