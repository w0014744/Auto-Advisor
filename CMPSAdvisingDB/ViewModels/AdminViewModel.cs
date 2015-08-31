using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CMPSAdvisingDB.Models;

namespace CMPSAdvisingDB.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string RoleName { get; set; }
        public string Description { get; set; }

        public RoleViewModel(ApplicationRole role)
        {
            this.Id = role.Id;
            this.RoleName = role.Name;
            this.Description = role.Description;
        }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        public string wNumber { get; set; }


        public IEnumerable<SelectListItem> RolesList { get; set; }

        public EditUserViewModel() { }

        public EditUserViewModel (string Id)
        {
            this.Id = Id;
        }
    }

    public class SelectRoleEditorViewModel
    {
        public SelectRoleEditorViewModel() { }

        // Update this to accept an argument of type ApplicationRole:
        public SelectRoleEditorViewModel(ApplicationRole role)
        {
            this.RoleName = role.Name;
            this.Description = role.Description;
        }

        public bool Selected { get; set; }

        [Required]
        public string RoleName { get; set; }

        // Add the new Description property:
        public string Description { get; set; }
    }

    public class EditRoleViewModel
    {
        public string Id { get; set; }
        public string OriginalRoleName { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public EditRoleViewModel() { }
        public EditRoleViewModel(ApplicationRole role)
        {
            this.Id = role.Id;
            this.OriginalRoleName = role.Name;
            this.RoleName = role.Name;
            this.Description = role.Description;
        }
    }
}