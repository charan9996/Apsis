using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.Entities
{
    /// <summary>
    /// Uploads Table Entity
    /// </summary>
    public class Upload : BaseEntity
    {
        /// <summary>
        /// FileName
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// FilePath
        /// </summary>
        public string FilePath { get; set; }
    }
}
