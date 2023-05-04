using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ToDoApplication.Models
{
    public class Tasks
    {
        
        public Guid Id { get; set; }

        [BindProperty]
        [Required]
        public string Name { get; set; }

        [BindProperty]
        [Required]
        [Range(1, 10)]
        public int Priority { get; set; }

        [BindProperty]
        [Required]
        public string Status { get; set; }
    }
}
