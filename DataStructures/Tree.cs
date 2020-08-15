using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Thorns
{
    /// <summary>
    /// A tree data structure.
    /// </summary>
    /// <typeparam name="T">The type for the elements in the tree.</typeparam>
    public class Tree<T> where T : IEquatable<T>
    {
        /// <summary>
        /// The value of the tree at this node.
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// The parent of this node of the tree.
        /// </summary>
        public Tree<T> Parent { get; private set; }
        internal readonly HashSet<Tree<T>> children;

        /// <summary>
        /// Creates a new tree with this instance at its root. Each instance is a node of the tree.
        /// </summary>
        /// <param name="val">The value of the node associated with the tree.</param>
        public Tree(T val) { Value = val; Parent = null; children = new HashSet<Tree<T>>(); }

        private Tree(T val, Tree<T> parent) { Value = val; Parent = parent; parent.children.Add(this); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The child nodes of this instance.</returns>
        public HashSet<Tree<T>> Children() => children;

        /// <summary>
        /// Adds a child to this tree node.
        /// </summary>
        /// <param name="val">The value at the child tree node.</param>
        /// <returns>The child tree node created.</returns>
        public Tree<T> AddChild(T val) => new Tree<T>(val, this); // Constructor adds child.

        /// <summary>
        /// Adds all the values as this tree node's children.
        /// </summary>
        /// <param name="values">The values for the children tree nodes.</param>
        /// <returns>The children nodes created.</returns>
        public IEnumerable<Tree<T>> AddChildren(params T[] values) => from T val in values select new Tree<T>(val, this);

        /// <summary>
        /// Adds all the values as this tree node's children.
        /// </summary>
        /// <param name="values">The values for the children tree nodes.</param>
        /// <returns>The children nodes created.</returns>
        public IEnumerable<Tree<T>> AddChildren(IEnumerable<T> values) => from T val in values select new Tree<T>(val, this);

        /// <summary>
        /// Removes the child element.
        /// </summary>
        /// <param name="child">The child element to be removed.</param>
        /// <returns>True if the child parameter was in fact a child of this node, false otherwise.</returns>
        public bool RemoveChild(Tree<T> child)
        {
            if (children.Contains(child)) { children.Remove(child); child.Parent = null; return true; }
            return false;
        }

        /// <summary>
        /// Checks if the child parameter is a child of this tree node.
        /// </summary>
        /// <param name="child">The potential child node.</param>
        /// <returns></returns>
        public bool HasChild(Tree<T> child) => children.Contains(child);

        /// <summary>
        /// Checks if the given iteration over this tree contains the provided tree.
        /// </summary>
        /// <param name="tree">The potentially contained tree.</param>
        /// <param name="iter">The iteration applied.</param>
        /// <returns></returns>
        public bool Has(Tree<T> tree, Func<Tree<T>, IEnumerable<Tree<T>>> iter=null)
        {
            if (iter == null) iter = TreeIter.BreadthFirst;
            return iter(this).Contains(tree);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">The value to test for the containment of.</param>
        /// <returns>True if the tree has given value as a descendant, false otherwise.</returns>
        public bool HasValue(T value) => FirstByValue(value) != null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cond">The condition to test the iteration for.</param>
        /// <param name="iter">The iteration to be applied on this tree. Applies breadth-first by default.</param>
        /// <returns>An iteration over all instances in the provided iteration that satisfy the given condition.</returns>
        public IEnumerable<Tree<T>> Where(Func<Tree<T>, bool> cond, Func<Tree<T>, IEnumerable<Tree<T>>> iter = null)
        {
            if (iter == null) iter = TreeIter.BreadthFirst; 
            return iter(this).Where(cond);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">The value to test fror the containment of.</param>
        /// <param name="iter">The iteration to implement. Breadth-first by default.</param>
        /// <returns>An iteration of all descendants with the given value.</returns>
        public IEnumerable<Tree<T>> WhereByValue(T value, Func<Tree<T>, IEnumerable<Tree<T>>> iter = null) 
            => Where((tr) => tr.Value.Equals(value), iter);

        //// <summary>
        /// 
        /// </summary>
        /// <param name="cond">The condition to test the iteration for.</param>
        /// <param name="iter">The iteration to be applied on this tree. Applies breadth-first by default.</param>
        /// <returns>An iteration over all instances in the provided iteration that satisfy the given condition.</returns>
        public IEnumerable<Tree<T>> WhereByValue(Func<T, bool> cond, Func<Tree<T>, IEnumerable<Tree<T>>> iter = null) 
            => Where((tr) => cond(tr.Value), iter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">The value to test fror the containment of.</param>
        /// <param name="iter">The iteration to implement. Breadth-first by default.</param>
        /// <returns>An iteration of the first descendant with the given value.</returns>
        public Tree<T> FirstByValue(T value, Func<Tree<T>, IEnumerable<Tree<T>>> iter = null)
        {
            StreamReader file = new StreamReader("");
            if (iter == null) iter = TreeIter.BreadthFirst;
            return iter(this).FirstOrDefault((tr) => tr.Value.Equals(value));
        }

        /// <summary>
        /// The root of the tree.
        /// </summary>
        public Tree<T> Root
        {
            get
            {
                Tree<T> root = this;
                while (root.Parent != null) root = root.Parent;
                
                return root;
            }
        }

        /// <summary>
        /// The sibling instances of this tree node.
        /// </summary>
        public HashSet<Tree<T>> Siblings => (Parent == null) ? new HashSet<Tree<T>>() : Parent.children;
    }
}
