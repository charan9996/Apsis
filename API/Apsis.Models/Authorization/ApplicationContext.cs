using Apsis.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.Authorization
{
    public class ApplicationContext
    {
        public User CurrentUser { get; set; }
    }
}
