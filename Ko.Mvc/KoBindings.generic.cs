using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Ko.Core;
using Ko.Core.BindingAttributes;
using System.Linq;

namespace Ko.Mvc
{
    /// <summary>
    /// Models for Knockout data-bind attribute
    /// </summary>
    /// <typeparam name="TContext">
    /// The C# type faking the Knockout binding context
    /// </typeparam>
    public class KoBindings<TContext> : ComplexBinding, IHtmlString
    {
        #region Overrides

        /// <summary>
        /// Returns "data-bind" as the binding name.
        /// Here it turns into the HTML attribute name.
        /// </summary>
        public override string Name
        {
            get { return "data-bind"; }
        }

        /// <summary>
        /// Checks if all added bindings are allowed to be in virtual element
        /// </summary>
        public override bool VirtualElementAllowed
        {
            get
            {
                var result = Children.Count > 0 && Children.All(x => x.VirtualElementAllowed);
                return result;
            }
        }

        /// <summary>
        /// Gets the data-bind value. Setting value is forbidden.
        /// </summary>
        public override string Value
        {
            get
            {
                var childList = GetChildren();
                if (childList.Count > 0)
                {
                    return string.Join(",", childList);
                }
                return string.Empty;
            }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Formats the object as format of htmlm element attribute
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var result = string.Empty;
            var value = Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                result = string.Format(@"{0}=""{1}""", Name, value);
            }
            return result;
        }

        /// <summary>
        /// To HTML string
        /// </summary>
        /// <returns></returns>
        public string ToHtmlString()
        {
            return ToString();
        }

        #endregion

        #region Non-binding methods

        /// <summary>
        /// Removes all added bindings. 
        /// Should be the first in method calling chain for adding bindings
        /// </summary>
        /// <returns></returns>
        public new KoBindings<TContext> Clear()
        {
            base.Clear();
            return this;
        }

        public new KoBindings<TContext> AddChildren(IEnumerable<KnockoutBinding> children)
        {
            if (children!=null)
            {
                base.AddChildren(children);
            }
            return this;
        }

        public new KoBindings<TContext> AddChild(KnockoutBinding child)
        {
            base.AddChild(child);
            return this;
        }

        #endregion

        //
        //NOTE: Each binding is added only once. 
        //Bindings are identified by their names.
        //

        #region Generic Bindings

        /// <summary>
        /// Adds an event binding
        /// </summary>
        /// <param name="expression">
        /// Value for binding expression
        /// </param>
        /// <param name="firstParamereter">
        /// Value for first parameter
        /// </param>
        /// <param name="additionalParameters">
        /// Other parameters
        /// </param>
        /// <returns>
        /// The current instance
        /// </returns>
        /// <typeparam name="TEventBinding">
        /// Type drived from IEventBinding and KnockoutBinding
        /// </typeparam>
        /// <returns></returns>
        public KoBindings<TContext> Add<TEventBinding>(Expression<Func<TContext, object>> expression, Expression<Func<TContext, object>> firstParamereter, params Expression<Func<TContext, object>>[] additionalParameters) where TEventBinding : KnockoutBinding, IEventBinding, new()
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (firstParamereter == null)
            {
                throw new ArgumentNullException("firstParamereter");
            }

            var eventBinding = Get<EventBinding>();
            if (eventBinding == null)
            {
                eventBinding = new EventBinding();
                AddChild(eventBinding);
            }

            var value = KnockoutHelper.ExtractExpression(expression);
            if (additionalParameters != null && additionalParameters.Length > 0)
            {
                var @params = additionalParameters.Where(x => x != null).Select(KnockoutHelper.ExtractExpression).ToList();
                @params.Insert(0, KnockoutHelper.ExtractExpression(firstParamereter));

                value = string.Format("{0}.bind($data,{1})", value, string.Join(",", @params));
            }
            else
            {
                value = string.Format("{0}.bind($data,{1})", value, KnockoutHelper.ExtractExpression(firstParamereter));
            }

            eventBinding.AddChild(new TEventBinding { Value = value });
            return this;
        }

        /// <summary>
        /// Adds a complex binding
        /// </summary>
        /// <param name="dataBind">
        /// Databind attribute
        /// </param>
        /// <returns>
        /// The current instance
        /// </returns>
        /// <typeparam name="TComplexBinding">
        /// Type drived from ComplexBinding
        /// </typeparam>
        /// <returns></returns>
        public KoBindings<TContext> Add<TComplexBinding>(KoBindings<TContext> dataBind) where TComplexBinding : ComplexBinding, new()
        {
            if (dataBind == null)
            {
                throw new ArgumentNullException("dataBind");
            }
            AddChild(new TComplexBinding().AddChildren(dataBind.Children));
            return this;
        }

        /// <summary>
        /// Adds a simple binding
        /// </summary>
        /// <param name="expression">
        /// Expression
        /// </param>
        /// <returns>
        /// The current instance
        /// </returns>
        public KoBindings<TContext> Add<TBinding>(Expression<Func<TContext, object>> expression) where TBinding : KnockoutBinding, new()
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            var bindingValue = KnockoutHelper.ExtractExpression(expression);
            var binding = new TBinding { Value = bindingValue };
            AddChild(binding);
            return this;
        }

        /// <summary>
        /// Remove an existing binding
        /// </summary>
        /// <returns>
        /// The current instance
        /// </returns>
        public KoBindings<TContext> Remove<TBinding>() where TBinding : KnockoutBinding, new()
        {
            var binding = new TBinding();
            RemoveChild(binding.Name);
            return this;
        }

        /// <summary>
        /// Gets existing bind by type
        /// </summary>
        /// <returns>
        /// A binding object if existing
        /// </returns>
        public TBinding Get<TBinding>() where TBinding : KnockoutBinding
        {
            var found = (TBinding)Children.FirstOrDefault(x => x is TBinding);
            return found;
        }

        /// <summary>
        /// Gets existing bind by name
        /// </summary>
        /// <returns>
        /// A binding object if existing
        /// </returns>
        public KnockoutBinding Get(string name)
        {
            var found = Children.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            return found;
        }

        #endregion

        #region LazyBinding

        /// <summary>
        /// Add a new binding without knowing about its corresponding class.
        /// Note: It should be used if and only if the corresponding class is unnecessary to be defined.
        /// </summary>
        /// <param name="name">
        /// Binding name
        /// </param>
        /// <param name="expression">
        /// Expression
        /// </param>
        /// <param name="virtualElementAllowed">
        /// Indicates that the binding is allowed to use in virtual element
        /// </param>
        /// <returns></returns>
        public KoBindings<TContext> AddLazy(string name, Expression<Func<TContext, object>> expression, bool virtualElementAllowed = false)
        {
            var lazyBinding = new LazyBinding(name, virtualElementAllowed);

            var lazyValue = KnockoutHelper.ExtractExpression(expression);
            lazyBinding.Value = lazyValue;

            AddChild(lazyBinding);
            return this;
        }

        /// <summary>
        /// Add a new binding without knowing about its corresponding class.
        /// Note: It should be used if and only if the corresponding class is unnecessary to be defined.
        /// </summary>
        /// <param name="name">
        /// Binding name
        /// </param>
        /// <param name="lazyValue">
        /// Binding Expression
        /// </param>
        /// <param name="virtualElementAllowed">
        /// Indicates that the binding is allowed to use in virtual element
        /// </param>
        /// <returns></returns>
        public KoBindings<TContext> AddLazy(string name, string lazyValue, bool virtualElementAllowed = false)
        {
            var lazyBinding = new LazyBinding(name, virtualElementAllowed) { Value = lazyValue };

            AddChild(lazyBinding);
            return this;
        }

        #endregion

        #region Create context from current context

        /// <summary>
        /// Creates new KoBindings instance based on TContext of current instance
        /// </summary>
        /// <returns></returns>
        public KoBindings<TContext> CloneContext()
        {
            return new KoBindings<TContext>();
        }

        #endregion

    }
}
