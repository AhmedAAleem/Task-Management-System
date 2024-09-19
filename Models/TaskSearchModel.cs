using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementApp.Models
{
    public class TaskSearchModel
    {
        [DisplayName("Status")]
        public int? statusId { get; set; }
        [DisplayName("Team")]
        public int? TeamId { get; set; }
        public string title { get; set; }
        public IEnumerable<Data.Task>? Tasks { get; set; }
        public IEnumerable<SelectListItem>? statusList { get; set; }
        public IEnumerable<SelectListItem>? TeamsList { get; set; }
    }
}
