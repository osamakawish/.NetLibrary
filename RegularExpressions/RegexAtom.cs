using System;

namespace Strings
{
    /// <summary>
    /// A Regex Atom corresponds to the most basic element in a regex pattern, like a . or a [abce], which isolates a single characte.
    /// </summary>
    internal class RegexAtom
    {
        internal Func<char, bool> Func { get; private set; }

        // An atom containing on of the letters in the string.
        public RegexAtom(string s)
        {
            // Isolate escape characters and group dashes
            Func = (ch) => s.Contains(ch.ToString());
        }

        /// <summary>
        /// An atom for all characters in the range of UTF-8 characters between chars a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public RegexAtom(char a, char b) => Func = (ch) => (a <= ch) && (ch <= b);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        public RegexAtom(Func<char, bool> func) => Func = func;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public bool this[char ch] => Func(ch);
    }

    /// <summary>
    /// A collection of identical RegexAtom patterns.
    /// </summary>
    internal class AtomGroup
    {
        public RegexAtom RegexAtom { get; private set; }
        public int MinCount { get; private set; }
        public int? MaxCount { get; private set; }

        public AtomGroup(RegexAtom atom, int min=0, int? max=1)
        {
            RegexAtom = atom; MinCount = min; MaxCount = max;
        }
    }
}