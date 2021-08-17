using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MvcExam
{
    public class Hobby
    {
        [Key]
        [Required]
        public int HobbyId { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Name { get; set; }
         public int UserId {get;set;}
        public User Creator {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        public List<Association> UsersForThisHobby { get; set; }
    }
}