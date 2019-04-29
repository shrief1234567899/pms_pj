using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using pms.Models;

namespace pms.ViewModels
{
    public class ProjectUsers
    {
        public project Project { get; set; }
        public IEnumerable <User> users { get; set; }
    }
}