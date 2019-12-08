using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Models.Common
{
    /// <summary>
    /// Common Helper Functions
    /// </summary>
    public class CommonHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="score">Score</param>
        /// <returns></returns>
        public static bool IsValidScore(float score)
        {
            string[] arrScore = score.ToString().Split('.');
            bool validStatus = false;
            if(arrScore.Length > 2)
            {
                validStatus = false;
            } 
            else
            {
                var lengthBeforeDecimal = arrScore[0].Length;
                var lengthAfterDecimal = 0;
                if (arrScore.Length > 1)
                {
                    lengthAfterDecimal = arrScore[1].Length;
                }
                if((lengthBeforeDecimal <= 5) || (lengthBeforeDecimal <= 5 && lengthAfterDecimal <= 2))
                {
                    validStatus = true;
                }

            }
            return validStatus;
        }
    }
}
