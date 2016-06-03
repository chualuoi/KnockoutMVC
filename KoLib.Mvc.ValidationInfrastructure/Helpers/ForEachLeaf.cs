using System;
using System.Collections.Generic;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    /// <summary>
    /// A view tree for each item in the collection
    /// </summary>
    public class ForEachLeaf<TModel, TItem> : ValidationLeaf<TModel> where TModel : class 
    {
        private Action<TModel> applicableInvoker;
        private Action<TModel> readOnlyInvoker;

        public Action<TModel> ApplicableInvoker
        {
            get { return applicableInvoker; }
            set
            {
                if (applicableInvoker != null)
                {
                    throw new InvalidOperationException("ApplicableInvoker has been set");
                }
                applicableInvoker = value;
            }
        }

        public Action<TModel> ReadOnlyInvoker
        {
            get { return readOnlyInvoker; }
            set
            {
                if (readOnlyInvoker != null)
                {
                    throw new InvalidOperationException("ReadOnlyInvoker has been set");
                }
                readOnlyInvoker = value;
            }
        }
        public ForEachLeaf(ValidationTree<TModel> parent, PropertyExpression expression)
            : base(parent, expression)
        {
        }

        /// <summary>
        /// Execute applicable and readonly rule for all items in the list
        /// </summary>
        public override void ExecuteForEachRule(TModel model)
        {
            //Get value of the list
            var list = PropertyExpression.GetValue(model) as List<TItem>;
            if (list == null || list.Count == 0)
            {
                return;
            }
            if (applicableInvoker != null)
            {
                ApplicableInvoker.DynamicInvoke(model);
            }
            if (readOnlyInvoker != null)
            {
                ReadOnlyInvoker.DynamicInvoke(model);
            }
        }

        /// <summary>
        /// Add applicable for each item in collection
        /// </summary>
        /// <param name="applicable"></param>
        /// <returns></returns>
        public ForEachLeaf<TModel, TItem> ApplyApplicable(Action<TModel> applicable)
        {
            ApplicableInvoker = applicable;
            return this;
        }

        /// <summary>
        /// Add readonly rule for each item in collection
        /// </summary>
        /// <param name="readOnly">The read only.</param>
        /// <returns></returns>
        public ForEachLeaf<TModel, TItem> ApplyReadOnly(Action<TModel> readOnly)
        {
            ReadOnlyInvoker = readOnly;
            return this;
        }
    }    
}