using System;
using System.Collections.Generic;

namespace Thorns
{
    public static class TreeIter
    {
        /// <summary>
        /// Breadth-first iteration of a tree.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the tree, breadth-first.</returns>
        public static IEnumerable<Tree<T>> BreadthFirst<T>(Tree<T> tree) where T : IEquatable<T>
        {
            List<Tree<T>> trees = new List<Tree<T>> { tree };
            
            foreach (var child in trees) trees.AddRange(child.children); 

            return trees;
        }

        /// <summary>
        /// Breadth-first iteration of an ordered tree.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The ordered tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the tree, breadth-first.</returns>
        public static IEnumerable<OrderedTree<T>> BreadthFirst<T>(OrderedTree<T> tree) where T : IEquatable<T>
        {
            List<OrderedTree<T>> trees = new List<OrderedTree<T>> { tree };

            foreach (var child in trees) trees.AddRange(child.children);

            return trees;
        }

        /// <summary>
        /// Breadth-first iteration of a tree by value.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the values of the tree, breadth-first.</returns>
        public static IEnumerable<T> BreadthFirstByValue<T>(Tree<T> tree) where T : IEquatable<T>
        {
            List<Tree<T>> trees = new List<Tree<T>> { tree }; List<T> values = new List<T>();

            foreach (var child in trees) { values.Add(child.Value); trees.AddRange(child.children); }

            return values;
        }

        /// <summary>
        /// Breadth-first iteration of an ordered tree by value.
        /// </summary>
        /// <typeparam name="T">The value type for the tree.</typeparam>
        /// <param name="tree">The ordered tree to be iterated over.</param>
        /// <returns>An IEnumerable to iterate over the values of the  tree, breadth-first.</returns>
        public static IEnumerable<T> BreadthFirstByValue<T>(OrderedTree<T> tree) where T : IEquatable<T>
        {
            List<OrderedTree<T>> trees = new List<OrderedTree<T>> { tree }; List<T> values = new List<T>();

            foreach (var child in trees) { values.Add(child.Value); trees.AddRange(child.children); }

            return values;
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
