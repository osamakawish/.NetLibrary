using System;
using System.Collections.Generic;
using System.Text;

namespace RegularExpressions
{
    /// <summary>
    /// Matches any given regex boundary or element.
    /// </summary>
    internal interface IRegexElement
    {
        
    }

    public sealed class RegexElement
    {
        internal IRegexElement Element;

        private RegexElement(IRegexElement element) => Element = element;

        public static implicit operator RegexElement(RegexAtom atom) => new RegexElement(atom);

        public static implicit operator RegexElement(RegexBoundary boundary) => new RegexElement(boundary);

        public static implicit operator RegexElement(BoundaryGroup boundaries) => new RegexElement(boundaries);
    }
}
