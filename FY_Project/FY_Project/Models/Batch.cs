//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FY_Project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Batch
    {
        public Batch()
        {
            this.Exams = new HashSet<Exam>();
            this.Users = new HashSet<User>();
        }
    
        public string Batch_Id { get; set; }
        public string Starting_Date { get; set; }
        public string Ending_Date { get; set; }
    
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
