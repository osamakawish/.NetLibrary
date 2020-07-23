using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegularExpressions
{
    public class RegexBoundary : IRegexElement
    {
        public Func<char, bool> Prev { get; private set; }
        public Func<char, bool> Next { get; private set; }

        private readonly bool AcceptsStartOfString;
        private readonly bool AcceptsEndOfString;

        public RegexAtom PrevAtom => new RegexAtom((ch) => ApplyOrDefault(Prev, ch, AcceptsStartOfString));

        public RegexAtom NextAtom => new RegexAtom((ch) => ApplyOrDefault(Next, ch, AcceptsEndOfString));

        private RegexBoundary(Func<char, bool> prev, Func<char, bool> next, bool acceptStartOfString=false, bool acceptEndOfString=false)
        { Prev = prev; Next = next; AcceptsStartOfString = acceptStartOfString; AcceptsEndOfString = acceptEndOfString; }

        public static RegexBoundary WordStart() => new RegexBoundary((ch) => !IsWordChar(ch), IsWordChar, true, false);

        public static RegexBoundary WordEnd() => new RegexBoundary(IsWordChar, (ch) => !IsWordChar(ch), false, true);

        public static RegexBoundary LineStart() => new RegexBoundary(IsLineEnding, (ch) => true, true, false);

        public static RegexBoundary LineEnd() => new RegexBoundary((ch) => true, IsLineEnding, false, true);

        internal static bool IsWordChar(char ch) => char.IsLetterOrDigit(ch) || ch == '_';

        internal static bool IsLineEnding(char ch) => ch == '\n' || ch == '\r';

        private static bool ApplyOrDefault(in Func<char, bool> func, char ch, bool @default)
        { try { return func(ch); } catch (IndexOutOfRangeException) { return @default; } }

        public bool this[char ch1, char ch2] => ApplyOrDefault(Prev, ch1, AcceptsStartOfString) && ApplyOrDefault(Next, ch2, AcceptsEndOfString);

        public static RegexBoundary LeftBoundary(Func<char, bool> func) => new RegexBoundary(func, (ch) => !func(ch), true, false);

        public static RegexBoundary RightBoundary(Func<char, bool> func) => new RegexBoundary((ch) => !func(ch), func, false, true);

        public static BoundaryGroup operator |(RegexBoundary boundary1, RegexBoundary boundary2) => new BoundaryGroup(boundary1, boundary2);
    }

    public class BoundaryGroup : IRegexElement, IEnumerable<RegexBoundary>
    {
        readonly RegexBoundary[] RegexBoundaries;

        public BoundaryGroup(params RegexBoundary[] boundaries) => RegexBoundaries = boundaries;

        public IEnumerator<RegexBoundary> GetEnumerator() => ((IEnumerable<RegexBoundary>)RegexBoundaries).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => RegexBoundaries.GetEnumerator();

        public static implicit operator BoundaryGroup(RegexBoundary[] boundaries) => new BoundaryGroup(boundaries);

        public static BoundaryGroup operator |(BoundaryGroup boundaryGroup, RegexBoundary boundary) 
            => boundaryGroup.RegexBoundaries.Append(boundary).ToArray();

        public static BoundaryGroup operator |(BoundaryGroup group1, BoundaryGroup group2) 
            => group1.RegexBoundaries.Concat(group2).ToArray();

        public IEnumerable<bool> Partition(char ch1, char ch2)
        { foreach (RegexBoundary bdy in RegexBoundaries) { yield return bdy[ch1, ch2]; } }

        public bool this[char ch1, char ch2]
        { get { foreach (RegexBoundary bdy in RegexBoundaries) { if (bdy[ch1, ch2]) return true; } return false; } }

        public static BoundaryGroup DualBoundary(Func<char, bool> func) => RegexBoundary.LeftBoundary(func) | RegexBoundary.RightBoundary(func);

        public static BoundaryGroup WordBoundary() => DualBoundary(RegexBoundary.IsWordChar);

        public static BoundaryGroup LineBoundary() => DualBoundary(RegexBoundary.IsLineEnding);

        public static BoundaryGroup LineOrWordEnding() => new BoundaryGroup(RegexBoundary.WordEnd(), RegexBoundary.LineEnd());
    }
}
