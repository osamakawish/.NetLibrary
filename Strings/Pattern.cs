using System.Collections.Generic;

namespace Strings
{
    public class Pattern
    {
        private List<RegexAtom> atoms;

        public string String { get; private set; }

        public Pattern(string s="")
        {
            String = s;
        }


    }
}