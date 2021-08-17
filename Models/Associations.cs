using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace MvcExam
{
    public class Association
    {
        [Key]
        public int AssociationId { get; set; }
        [Required]
        public int HobbyId { get; set; }
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        public Hobby Hobby { get; set; }
    }
}