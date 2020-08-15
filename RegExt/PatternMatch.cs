using System.Collections.Generic;

namespace RegExt
{
    public class PatternMatch
    {
        /// <summary>
        /// Internal captures of the match
        /// </summary>
        internal IEnumerable<TryMatch> Matches;

        internal PatternMatch()
        {

        }
    }
}