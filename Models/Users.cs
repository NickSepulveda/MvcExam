using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MvcExam
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        [MinLength(2, ErrorMessage="Password must be 2 characters or longer!")]
        public string FirstName {get;set;}
        [Required]
        [MinLength(2, ErrorMessage="Password must be 2 characters or longer!")]
        public string LastName {get;set;}
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        [MinLength(3, ErrorMessage="Username must be 3 characters or longer!")]
        [MaxLength(15, ErrorMessage="Username must be 15 characters or shorter!")]

        [Required]
        public string Username {get;set;}
        [DataType(DataType.Password)]
        [Required]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        // Will not be mapped to your users table!
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
        public List<Association> HobbiesForThisUser { get; set; }
    }
    
}