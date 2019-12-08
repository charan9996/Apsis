using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.Constants
{
    public sealed class RequestResultCodes
    { 
        public static Guid ERROR = Guid.Parse("4B25F660-5751-4361-BEF6-19ED2C15A6D7");
        public static Guid Result_Awaited = Guid.Parse("7002ED9E-47D4-4C68-86FF-39EEE95ACEB4");
        public static Guid Project_Cleared = Guid.Parse("6BE2995B-4E48-4F5A-B281-52759F284462");
        public static Guid Project_Not_Cleared = Guid.Parse("F935792C-E7BB-49B6-9E60-9C8D39E43667");

        public string GetStatusName(Guid statusGuid)
        {
            string statusName = "";
            switch (statusGuid.ToString().ToUpper())
            {
                case "4B25F660-5751-4361-BEF6-19ED2C15A6D7":
                    statusName = "Error";
                    break;
                case "7002ED9E-47D4-4C68-86FF-39EEE95ACEB4":
                    statusName = "Yet To Evaluate";
                    break;
                case "6BE2995B-4E48-4F5A-B281-52759F284462":
                    statusName = "Cleared";
                    break;
                case "F935792C-E7BB-49B6-9E60-9C8D39E43667":
                    statusName = "Not Cleared";
                    break;
            }
            return statusName;
        }
    }
}
