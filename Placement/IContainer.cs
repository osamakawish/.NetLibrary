using System;
using System.Collections.Generic;
using Concision.Thorns;

namespace Concision.Placement
{
    /// <summary>
    /// Contains UI with Children
    /// </summary>
    internal class UiElement : Tree<UiElement>, IEquatable<UiElement>
    {
        // For ensuring uniqueness.
        private static int Id = 0;
        private int id;

        internal static Dictionary<int, UiElement> ElementIds = new Dictionary<int, UiElement>();

        public UiElement(UiElement val) : base(val)
        {
            id = Id++; ElementIds.Add(id, this);
        }

        public override bool Equals(object obj) => id == ((UiElement)obj).id;

        public bool Equals(UiElement other) => id == other.id;

        public override int GetHashCode() => id;

        public override string ToString()
        {
            return base.ToString();
        }
    }
}