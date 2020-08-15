using System.Collections.Generic;

namespace RegExt
{
    internal class TryMatch
    {
        internal IEnumerable<TryMatch> tries;

        internal TryMatch()
        {
        }

        public static implicit operator TryMatch(PatternMatch match)
        {
            
        }
    }
}