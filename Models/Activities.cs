using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Belt1.Models
{
    public class Activities
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; }
        public string coordinator { get; set; }

        public DateTime date { get; set; }
        public DateTime datetime { get; set; }
        public int duration{ get; set; }
        public string duration2 { get; set; }
        public string description { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public int usersid { get; set; }
        public Users Users { get; set; }
        public Activities()
        {
            Participant = new List<Participants>();
        }
        public List<Participants> Participant { get; set;}
    }
}