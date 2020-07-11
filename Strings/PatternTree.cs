using System;
using System.Collections.Generic;
using System.Text;

namespace Strings
{
    class PatternTree
    {
        /// <summary>
        /// Every odd indexed element must be a capture. If capture starts from first character, start the list with empty string.
        /// </summary>
        public List<Pattern> Patterns { get; private set; }
        public PatternTree Tree { get; private set; }

        public PatternTree(string regexPattern)
        {

        }
    }
}
