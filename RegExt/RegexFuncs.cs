using System;
using System.Collections.Generic;
using System.Text;

namespace RegExt
{
    public delegate bool CharCondition(char ch);

    // Need to figure out how to implement optional, countable, and optionalcountable properly
    // To implement them, I will have to reorder them in the actual list.
    // To implement optionalCountable (ie. "_*", convert it to "?_*" first, then implement)
    internal static class RegexFuncs
    {
        // Need to implement null PatternFunc cases in all of these.
        internal static PatternFunc AnyChar =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return true; };

        /// <summary>
        /// Tests whether the next condition applies. (ie. Place this func BEFORE the func being applied to in the list)
        /// </summary>
        /// <returns></returns>
        /// <remarks>The reason for placing it before the applied func is so the func doesn't get tested before this is applied.</remarks>
        internal static PatternFunc Optional =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { if (next(s, ref i)) { i++; } return true; };

        internal static PatternFunc StartOfString =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return i == 0; };

        internal static PatternFunc EndOfString =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return i == s.Length; };

        private static bool IsWordChar(char ch) => char.IsDigit(ch) || char.IsLetter(ch) || ch == '_';

        internal static PatternFunc WordStart =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return (i == 0 || !IsWordChar(s[i - 1])) && IsWordChar(s[i]); };

        internal static PatternFunc WordEnd =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return IsWordChar(s[i - 1]) && (i == s.Length || !IsWordChar(s[i])); };

        internal static PatternFunc WordBoundary =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return WordStart(s, ref i, prev, next) || WordEnd(s, ref i, prev, next); };

        internal static PatternFunc LineStart =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return i == 0 || "\r\n".Contains(s[i - 1].ToString()); };

        internal static PatternFunc LineEnd =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return i == s.Length || "\r\n".Contains(s[i].ToString()); };

        internal static PatternFunc LineOrWordEnd =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return WordEnd(s, ref i, prev, next) || LineEnd(s, ref i, prev, next); };

        internal static PatternFunc Countable
            => delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { int j = i; while (prev(s, ref i) && !next(s, ref i)) i++; return j > i; };

        internal static PatternFunc ExactChar(char ch)
            => delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return s[i] == ch; };

        internal static PatternFunc FromCharCondition(CharCondition cond)
            => delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return cond(s[i]); };

        internal static PatternFunc FromOctalUnicode
            => delegate (in string s, ref int i, PatternFunc prev, PatternFunc next)
            {
                throw new NotImplementedException();
            };

        internal static PatternFunc FromHexUnicode
            => delegate (in string s, ref int i, PatternFunc prev, PatternFunc next)
            {
                throw new NotImplementedException();
            };

        internal static PatternFunc FromAscii
            => delegate (in string s, ref int i, PatternFunc prev, PatternFunc next)
            {
                throw new NotImplementedException();
            };

        internal static PatternFunc WithRange(PatternFunc func, Range range)
            => delegate (in string s, ref int i, PatternFunc prev, PatternFunc next)
            {
                throw new NotImplementedException();
            };

        internal static Dictionary<char, PatternFunc> EscapedChar => new Dictionary<char, PatternFunc>
        {
            {'0', FromOctalUnicode},
            {'1', FromOctalUnicode},
            {'2', FromOctalUnicode},
            {'3', FromOctalUnicode},
            {'4', FromOctalUnicode},
            {'5', FromOctalUnicode},
            {'6', FromOctalUnicode},
            {'7', FromOctalUnicode},
            {'8', FromOctalUnicode},
            {'9', FromOctalUnicode},
            {'A', WordStart},
            {'a', ExactChar('\a')},
            {'b', WordBoundary},
            {'n', ExactChar('\n')},
            {'f', ExactChar('\f')},
            {'r', ExactChar('\r')},
            {'t', ExactChar('\t')},
            {'v', ExactChar('\v')},
            {'e', ExactChar('\u001b')},
            {'d', FromCharCondition(char.IsDigit)},
            {'D', FromCharCondition((ch) => !char.IsDigit(ch))},
            {'w', FromCharCondition(char.IsLetter)},
            {'W', FromCharCondition((ch) => !char.IsLetter(ch))},
            {'s', FromCharCondition(char.IsWhiteSpace)},
            {'S', FromCharCondition((ch) => !char.IsWhiteSpace(ch))},
            {'u', FromHexUnicode},
            {'x', FromAscii},
            {'z', WordEnd},
            {'Z', LineOrWordEnd}
        };

        internal static PatternFunc Escaped(char ch) => EscapedChar.TryGetValue(ch, out PatternFunc func) ? func : ExactChar(ch);

        internal static Dictionary<char, PatternFunc> SpecialChars => new Dictionary<char, PatternFunc>
        {
            {'.', AnyChar},
            {'+', Countable},
            {'?', Optional},
            {'*', Countable},
            {'^', LineStart},
            {'$', LineEnd}
            // Add other special characters.
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="funcPrev">If char is '*', funcPrev is Optional. Otherwise, it's null.</param>
        /// <returns></returns>
        internal static PatternFunc Special(char ch, out PatternFunc funcPrev)
        {
            funcPrev = ch == '*' ? Optional : null;

            return SpecialChars.TryGetValue(ch, out PatternFunc @return) ? @return : ExactChar(ch);
        }
    }
}
