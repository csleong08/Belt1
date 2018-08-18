using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Belt1.Models
{
    public class Participants
    {
        [Key]
        public int id { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public int usersid { get; set; }
        public Users Users { get; set; }
        public int activitiesid { get; set; }
        public Activities Activities { get; set; }
    }
}