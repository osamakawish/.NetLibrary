using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Concision.Thorns
{
    public static class TreeIter
    {
        internal class BreadthFirstEnumerator<T> : IEnumerator<Tree<T>> where T : IEquatable<T>
        {
            int i = -1;
            readonly List<Tree<T>> descendants = new List<Tree<T>>();

            public Tree<T> Current => descendants[i];

            object IEnumerator.Current => Current;

            public BreadthFirstEnumerator(Tree<T> tree) => descendants.Add(tree);

            public bool MoveNext()
            {
                bool @continue = ++i < descendants.Count;

                if (@continue) descendants.AddRange(Current.children);

                return @continue;
            }

            public void Reset()
            {
                i = 0; Tree<T> tree = descendants[0]; 
                descendants.Clear(); descendants.Add(tree);
            }

            void IDisposable.Dispose() { }
        }

        internal class BreadthFirstEnumerable<T> : IEnumerable<Tree<T>> where T : IEquatable<T>
        {
            readonly Tree<T> _tree;

            public BreadthFirstEnumerable(Tree<T> tree) => _tree = tree;

            public IEnumerator<Tree<T>> GetEnumerator() => new BreadthFirstEnumerator<T>(_tree);

            IEnumerator IEnumerable.GetEnumerator() => new BreadthFirstEnumerator<T>(_tree);
        }

        internal class BreadthFirstOrderedEnumerator<T> : IEnumerator<OrderedTree<T>> where T : IEquatable<T>
        {
            int i = 0;
            readonly List<OrderedTree<T>> descendants = new List<OrderedTree<T>>();

            public OrderedTree<T> Current => descendants[i];

            object IEnumerator.Current => Current;

            public BreadthFirstOrderedEnumerator(OrderedTree<T> OrderedTree) => descendants.Add(OrderedTree);

            public bool MoveNext()
            {
                bool @continue = ++i < descendants.Count;

                if (@continue) descendants.AddRange(Current.children);

                return @continue;
            }

            public void Reset()
            {
                i = 0; OrderedTree<T> OrderedTree = descendants[0];
                descendants.Clear(); descendants.Add(OrderedTree);
            }

            void IDisposable.Dispose() { }
        }

        internal class BreadthFirstOrderedEnumerable<T> : IEnumerable<OrderedTree<T>> where T : IEquatable<T>
        {
            readonly OrderedTree<T> _orderedTree;

            public BreadthFirstOrderedEnumerable(OrderedTree<T> OrderedTree) => _orderedTree = OrderedTree;

            public IEnumerator<OrderedTree<T>> GetEnumerator() => new BreadthFirstOrderedEnumerator<T>(_orderedTree);

            IEnumerator IEnumerable.GetEnumerator() => new BreadthFirstOrderedEnumerator<T>(_orderedTree);
        }

        /// <summary>
        /// Breadth-first iteration of a tree.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the tree, breadth-first.</returns>
        public static IEnumerable<Tree<T>> BreadthFirst<T>(Tree<T> tree) where T : IEquatable<T> 
            => new BreadthFirstEnumerable<T>(tree);

        /// <summary>
        /// Breadth-first iteration of an ordered tree.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The ordered tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the tree, breadth-first.</returns>
        public static IEnumerable<OrderedTree<T>> BreadthFirst<T>(OrderedTree<T> tree) where T : IEquatable<T>
            => new BreadthFirstOrderedEnumerable<T>(tree);

        /// <summary>
        /// Breadth-first iteration of a tree by value.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the values of the tree, breadth-first.</returns>
        public static IEnumerable<T> BreadthFirstByValue<T>(Tree<T> tree) where T : IEquatable<T>
        {
            foreach (var tr in BreadthFirst(tree)) yield return tr.Value;
        }

        /// <summary>
        /// Breadth-first iteration of an ordered tree by value.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The ordered tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the values of the  tree, breadth-first.</returns>
        public static IEnumerable<T> BreadthFirstByValue<T>(OrderedTree<T> tree) where T : IEquatable<T>
        {
            foreach (var tr in BreadthFirst(tree)) yield return tr.Value;
        }

        /// <summary>
        /// Depth-first iteration of a tree.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the tree, depth-first.</returns>
        public static IEnumerable<Tree<T>> DepthFirst<T>(Tree<T> tree) where T : IEquatable<T>
        {
            yield return tree;
            foreach (Tree<T> child in tree.children) { foreach (Tree<T> ch in DepthFirst(child)) yield return ch; }
        }

        /// <summary>
        /// Depth-first iteration of an ordered tree.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The ordered tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the tree, depth-first.</returns>
        public static IEnumerable<OrderedTree<T>> DepthFirst<T>(OrderedTree<T> tree) where T : IEquatable<T>
        {
            yield return tree;
            foreach (OrderedTree<T> child in tree.children) { foreach (OrderedTree<T> ch in DepthFirst(child)) yield return ch; }
        }

        /// <summary>
        /// Depth-first iteration of a tree.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the tree, depth-first.</returns>
        public static IEnumerable<T> DepthFirstByValue<T>(Tree<T> tree) where T : IEquatable<T>
        {
            yield return tree.Value;
            foreach (Tree<T> child in tree.children) { foreach (Tree<T> ch in DepthFirst(child)) yield return ch.Value; }
        }

        /// <summary>
        /// Depth-first iteration of an ordered tree.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The ordered tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the tree, depth-first.</returns>
        public static IEnumerable<T> DepthFirstByValue<T>(OrderedTree<T> tree) where T : IEquatable<T>
        {
            yield return tree.Value;
            foreach (OrderedTree<T> child in tree.children) { foreach (OrderedTree<T> ch in DepthFirst(child)) yield return ch.Value; }
        }
    }
}
