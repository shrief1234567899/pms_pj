//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace pms.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    public partial class User
    {
        public int Id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string type { get; set; }
        public string jop_description { get; set; }
        public string photo { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        
    }
}
