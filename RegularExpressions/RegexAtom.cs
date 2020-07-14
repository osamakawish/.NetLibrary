using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Strings
{
    /// <summary>
    /// A Regex Atom corresponds to the most basic element in a regex pattern, like a . or a [abce], which isolates a single character.
    /// Captures a function that represents what sort of characters are acceptable here.
    /// </summary>
    /// <example>'.' corresponds to </example>
    internal class RegexAtom
    {
        public enum Specialty { None, Character, Group };
        public const string SpecialChars = "-,.|?*+^$[](){}\\";
        public const string SpecialGroups = "wsd"; // For \w, \s, \d, etc. groups

        internal Func<char, bool> Func { get; private set; }

        /// <summary>
        /// Returns true if the character at index i is escaped appropriately. Also return false if inde
        /// </summary>
        /// <param name="s"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool IsLegallyEscaped(in string s, int i) => s[i - 1] == '\\' && Special(s[i]) == Specialty.None;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static Specialty Special(char ch)
        {
            String c = ch.ToString();
            if (SpecialChars.Contains(c)) return Specialty.Character;
            else if (SpecialGroups.Contains(c)) return Specialty.Group;
            else return Specialty.None;
        }

        /// <summary>
        /// 
        /// </summary>
        public RegexAtom() => Func = (ch) => false;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static List<RegexAtom> Atoms(string s)
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
        private static RegexAtom GetRange(in string s, int i) => new RegexAtom((char)(s[i - 1] + 1), s[i + 1]);

        /// <summary>
        /// Deals with case where s[i-1] is '[' until string finishes or reaches ']'.
        /// </summary>
        /// <param name="s">Pattern string</param>
        /// <param name="i">Starting index</param>
        /// <returns></returns>
        public static RegexAtom FromBracketed(in string s, ref int i)
        {
            // Note, only "\[]-" are special characters here. - only matter 

            bool escaped = false;
            RegexAtom atom = new RegexAtom();

            int l = s.Length;

            while ((s[i] != ']' || !escaped) && i < l)
            {
                char ch = s[i];

                if (!escaped)
                {
                    if (ch == '\\') escaped = true;

                    // If the '-' is between two characters not at end of string or before a bracket
                    else if (ch == '-' && i > 0 && i + 1 < l && s[i + 1] != ']')
                    {
                        atom |= GetRange(s, i);
                        i++; // Additional increment to skip next char as it's part of range.
                    }

                    else
                    {
                        atom |= new RegexAtom(ch);
                    }
                }
                else
                {
                    atom |= (Special(ch)) switch
                    {
                        Specialty.Group => SpecialAtom(ch),
                        _ => new RegexAtom(ch)
                    };
                    escaped = false;
                }

                i++;
            }

            return atom;
        }

        public static RegexAtom operator |(RegexAtom atom1, RegexAtom atom2) => new RegexAtom((ch) => atom1.Func(ch) || atom2.Func(ch));

        private static RegexAtom SpecialAtom(char sp)
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

        public static implicit operator AtomGroup(RegexAtom atom) => new AtomGroup(atom);
    }
}