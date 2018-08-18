using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Belt1.Models
{
    public class Users
    {
        [Key]
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public Users()
        {
            Activity = new List<Activities>();
            Participant = new List<Participants>();
        }
        public List<Activities> Activity { get; set; }
        public List<Participants> Participant { get; set; }
    }
}