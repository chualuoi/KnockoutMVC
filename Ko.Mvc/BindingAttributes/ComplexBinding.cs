using System;
using System.Collections.Generic;
using System.Linq;

namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Base class for all complex Knockout bindings.
    /// Complex knock binding is the binding that can contain other ones
    /// </summary>
    public abstract class ComplexBinding : KnockoutBinding
    {
        protected ComplexBinding()
        {
            Children = new List<KnockoutBinding>();
        }

        /// <summary>
        /// All nested bindings
        /// </summary>
        public IList<KnockoutBinding> Children { get; private set; }
        
        /// <summary>
        /// Add new children to the complex binding.
        /// Each child are distinct by name
        /// </summary>
        /// <param name="children">
        /// New children
        /// </param>
        /// <returns>
        /// The current instance
        /// </returns>
        public virtual ComplexBinding AddChildren(IEnumerable<KnockoutBinding> children)
        {
            if (children == null)
            {
                throw new ArgumentNullException("children");
            }

            foreach (var child in children)
            {
                AddChild(child);
            }

            return this;
        }

        /// <summary>
        /// Add new child to the complex binding.
        /// Each child are distinct by name
        /// </summary>
        /// <param name="child">
        /// New child
        /// </param>
        /// <returns>
        /// The current instance
        /// </returns>
        public virtual ComplexBinding AddChild(KnockoutBinding child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }
            var found = Children.FirstOrDefault(x => string.Equals(x.Name, child.Name, StringComparison.OrdinalIgnoreCase));
            if (found == null && !string.Equals(child.Name, Name, StringComparison.OrdinalIgnoreCase))
            {
                Children.Add(child);
            }

            return this;
        }

        /// <summary>
        /// Removing an existing child from the complex binding if any
        /// </summary>
        /// <param name="child">
        /// Existing child
        /// </param>
        /// <returns>
        /// The current instance
        /// </returns>
        public ComplexBinding RemoveChild(KnockoutBinding child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }
            return RemoveChild(child.Name);
        }

        /// <summary>
        /// Removing an existing child from the complex binding if any
        /// </summary>
        /// <param name="name">
        /// Name of existing child
        /// </param>
        /// <returns>
        /// The current instance
        /// </returns>
        public ComplexBinding RemoveChild(string name)
        {
            var found = Children.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            if (found != null)
            {
                Children.Remove(found);
            }
            return this;
        }

        /// <summary>
        /// Get all children in string.
        /// All empty children are excluded.
        /// Empty child is the one that is empty in string.
        /// </summary>
        /// <returns></returns>
        public IList<string> GetChildren()
        {
            var childList = new List<string>();
            foreach (var child in Children)
            {
                var childString = child.ToString();
                if (!string.IsNullOrWhiteSpace(childString))
                {
                    childList.Add(childString);
                }
            }
            return childList;
        }

        /// <summary>
        /// Formats the binding in {binding-name}:{child1, child2}
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var childList = GetChildren();
            if (childList.Count > 0)
            {
                var children = string.Join(",", Children);
                var result = string.Format("{0}:{{{1}}}", Name, children);
                return result;   
            }
            return base.ToString();
        }

        /// <summary>
        /// Removes all added bindings. 
        /// Should be the first in method calling chain for adding bindings
        /// </summary>
        /// <returns></returns>
        public virtual ComplexBinding Clear()
        {
            Children.Clear();
            return this;
        }

    }
}