using System;
using System.Collections.Generic;
using System.Linq;

namespace RegularExpressions
{
    public class Pattern
    {
        // Need to implement unicode characters: \000, \xFF, or \x{FF}, and boundaries "A, b, B, z, Z", and escape \e
        public enum Specialty { None, Character, Group, Unicode };
        public ParallelQuery<char> SpecialChars = "-,.|?*+^$[](){}\\/".AsParallel();
        public const string SpecialGroups = "ABDSWbdesw"; // For \w, \s, \d, etc. groups

        private int i;

        public string String { get; }

        private List<IRegexElement> regexElements;

        public Pattern(string s="")
        {
            String = s;
            GenerateAtomGroups(s);
        }

        /// <summary>
        /// Generates the atom groups based on the string provided.
        /// </summary>
        /// <param name="s"></param>
        private void GenerateAtomGroups(string s)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the character at index i is escaped appropriately. Also return false if inde
        /// </summary>
        /// <param name="s"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsLegallyEscaped(in string s, int i) => s[i - 1] == '\\' && Special(s[i]) == Specialty.None;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public Specialty Special(char ch)
        {
            String c = ch.ToString();
            if (RegexAtom.Contains(SpecialChars, ch)) return Specialty.Character;
            else if (SpecialGroups.Contains(c)) return Specialty.Group;
            else if (c == "x") return Specialty.Unicode;
            else return Specialty.None;
        }

        /// <summary>
        /// Deals with case where s[i-1] is '[' until string finishes or reaches ']'.
        /// </summary>
        /// <param name="s">Pattern string</param>
        /// <param name="i">Starting index</param>
        /// <returns></returns>
        public RegexAtom FromBracketed()
        {
            // Cases still to implement: ^ requests not to apply char.

            bool escaped = false;
            RegexAtom atom = new RegexAtom();

            int l = String.Length;

            while ((String[i] != ']' || !escaped) && i < l)
            {


                char ch = String[i];

                if (!escaped)
                {
                    if (ch == '\\') escaped = true;

                    // If the '-' is between two characters not at end of string or before a bracket
                    else if (ch == '-' && i > 0 && i + 1 < l && String[i + 1] != ']')
                    { atom |= RegexAtom.GetRange(String, i); i++; } // Additional increment to skip next char as it's part of range.

                    else atom |= new RegexAtom(ch);
                }
                else
                {
                    atom |= (Special(ch)) switch
                    {
                        Specialty.Group => RegexAtom.SpecialAtom(ch),
                        _ => new RegexAtom(ch)
                    };
                    escaped = false;
                }
                i++;
            }

            return atom;
        }

        /// <summary>
        /// Retreives the next atom in the string starting from the provided index. It updates the index while doing so.
        /// </summary>
        /// <param name="s">The string to retreive the regex atom from.</param>
        /// <param name="i">The starting index in the string.</param>
        /// <returns></returns>
        private IRegexElement GetNextRegexAtom()
        {
            IRegexElement atom = String[i] switch
            {
                '[' => FromBracketed(),
                '\\' => GetNextEscapeChar(),
                _ => new RegexAtom()
            };

            i++;

            return atom;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="i"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        private IRegexElement GetNextEscapeChar() => String[i] switch
        {
            'A' => RegexBoundary.WordStart(),
            'a' => new RegexAtom('\a'),
            'b' => BoundaryGroup.WordBoundary(),
            'n' => new RegexAtom('\n'),
            'f' => new RegexAtom('\f'),
            'r' => new RegexAtom('\r'),
            't' => new RegexAtom('\t'),
            'v' => new RegexAtom('\v'),
            'e' => new RegexAtom('\u001b'),
            'd' => new RegexAtom(char.IsDigit),
            'D' => new RegexAtom((ch) => !char.IsDigit(ch)),
            'w' => new RegexAtom(char.IsLetter),
            'W' => new RegexAtom((ch) => !char.IsLetter(ch)),
            's' => new RegexAtom(char.IsWhiteSpace),
            'S' => new RegexAtom((ch) => !char.IsWhiteSpace(ch)),
            'z' => RegexBoundary.WordEnd(),
            'Z' => BoundaryGroup.LineOrWordEnding(),
            _ => OtherEscapeChars()
        };

        private RegexAtom OtherEscapeChars()
        {
            char ch = String[i];

            ++i;
            if (IsSpecialChar(ch)) return ch;
            else if (ch == 'x') return GetFromEncoding(true);
            else if (char.IsDigit(ch)) return GetFromEncoding();
            else if (ch == 'u') return GetFromUnicode();
            else if (ch == 'c') return GetControlChar();
            else throw new NotEscapeException();
        }

        
        private RegexAtom GetFromUnicode()
        {
            int num = Convert.ToInt32(GetNum(4, IsHexChar),16);
            return char.ConvertFromUtf32(num);
        }

        private RegexAtom GetControlChar()
        {
            char ch = String[i++];
            if (char.IsLetter(ch)) return (char)(ch & 31);
            else throw new ImproperCharException();
        }

        private RegexAtom GetFromEncoding(bool isX = false)
        {
            return isX ? (char)Convert.ToInt32(GetNum(3, IsOctalChar), 8) 
                : (char)Convert.ToInt32(GetNum(2, IsHexChar), 16);
        }

        private static bool IsHexChar(char ch) => char.IsDigit(ch) || "ABCDEF".Contains(char.ToUpper(ch));

        private static bool IsOctalChar(char ch) => char.IsLetter(ch) && (ch & 31) < 8;

        private string GetNum(int maxdigits, Func<char, bool> charTest)
        {
            string num = "";
            int count = 0;
            char ch = String[i];
            while (charTest(ch) && count < maxdigits)
            {
                num.Append(ch);
                ch = String[++i]; count++;
            }
            return num;
        }

        // Use GetNextEscapeChar instead.
        internal bool IsSpecialChar(char ch) => RegexAtom.Contains(SpecialChars, ch);
    }
}