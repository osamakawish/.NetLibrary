using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RegularExpressions
{
    /// <summary>
    /// A Regex Atom corresponds to a component of a pattern capturing a single character.
    /// </summary>
    /// <example>'.' corresponds to </example>
    public class RegexAtom : IRegexElement
    {
        internal Func<char, bool> Func { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public RegexAtom() => Func = (ch) => false;

        public RegexAtom(bool s) => Func = (ch) => s;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        public RegexAtom(char ch) => Func = (c) => c == ch;

        /// <summary>
        /// An atom containing on of the letters in the string.
        /// </summary>
        /// <param name="s"></param>
        public RegexAtom(string s)
        {
            ParallelQuery<char> query = s.AsParallel();
            // Isolate escape characters and group dashes
            Func = (ch) => Contains(query, ch);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static List<RegexAtom> AtomsMatchingString(string s)
        {
            List<RegexAtom> atoms = new List<RegexAtom>();
            foreach (var ch in s) { atoms.Add(new RegexAtom(ch)); }

            return atoms;
        }

        /// <summary>
        /// Assumes s[i] is '-' and is within range and is not escaped.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        internal static RegexAtom GetRange(in string s, int i) => new RegexAtom((char)(s[i - 1] + 1), s[i + 1]);

        public static implicit operator RegexAtom(bool ch) => new RegexAtom(ch);

        public static implicit operator RegexAtom(char ch) => new RegexAtom(ch);

        public static implicit operator RegexAtom(string s) => new RegexAtom(s);

        public static RegexAtom operator |(RegexAtom atom1, RegexAtom atom2) => new RegexAtom((ch) => atom1.Func(ch) || atom2.Func(ch));

        public static RegexAtom operator &(RegexAtom atom1, RegexAtom atom2) => new RegexAtom((ch) => atom1.Func(ch) && atom2.Func(ch));

        internal static RegexAtom SpecialAtom(char sp)
        {
            return sp switch
            {
                'w' => new RegexAtom(char.IsLetter),
                's' => new RegexAtom(char.IsWhiteSpace),
                'd' => new RegexAtom(char.IsDigit),
                '.' => new RegexAtom((ch) => true),
                _ => null,
            };
        }

        internal static bool Contains(in ParallelQuery<char> s, char ch) => s.Where((c) => c == ch).Count() != 0;
    }

    /// <summary>
    /// RegexAtom with an associated range for similarity.
    /// </summary>
    internal class AtomGroup
    {
        public RegexAtom RegexAtom { get; private set; }
        public Limits Limits { get; private set; }
        public int Min => Limits.Min;
        public int? Max => Limits.Max;

        public AtomGroup(RegexAtom atom, int min=1, int? max=1)
        {
            RegexAtom = atom; Limits = new Limits(min, max);
        }

        public static implicit operator AtomGroup(RegexAtom atom) => new AtomGroup(atom);
    }

    public class Limits
    {
        public static Limits RegexLimits(char c)
        {
            return c switch
            {
                '?' => new Limits(0),
                '*' => new Limits(0, null),
                '+' => new Limits(max: null),
                _ => new Limits()
            };
        }

        public int Min { get; private set; }
        public int? Max { get; private set; }

        public Limits(int min=1, int? max=1)
        {
            Min = min; Max = max;
        }
    }
}