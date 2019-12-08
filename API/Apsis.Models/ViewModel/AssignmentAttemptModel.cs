using Apsis.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.ViewModel
{
    public class AssignmentAttemptModel : AssignmentAttempt
    {
        public string Mid { get; set; }
        public string YorbitRequestId { get; set; }
    }
}
