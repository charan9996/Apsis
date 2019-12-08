using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.Entities
{
    /// <summary>
    /// User Table Entity
    /// </summary>
    public class User :BaseEntity
    {
        /// <summary>
        /// Mid
        /// </summary>
        public string Mid { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Role Id
        /// </summary>
        public Guid RoleId { get; set; }
        
        /// <summary>
        /// Vendor
        /// </summary>
        public string Vendor { get; set; }

        /// <summary>
        /// IsExternal
        /// </summary>
        public bool IsExternal { get; set; }
    }
}
