using System;
using System.Linq.Expressions;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public class ValidationLeaf<TModel> : ValidationNode<TModel> where TModel : class 
    {
        public PropertyExpression PropertyExpression { get; set; }
        private PropertyExpression readOnlyValueExpression;

        public ValidationLeaf(ValidationTree<TModel> parent, PropertyExpression expression)
            : base(parent)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("parent");
            }
            PropertyExpression = expression;
        }

        public PropertyExpression ReadOnlyValueExpression
        {
            get { return readOnlyValueExpression; }
            set
            {
                if (readOnlyValueExpression != null)
                {
                    throw new InvalidOperationException("read only value expression has been set");
                }
                readOnlyValueExpression = value;
            }
        }

        public ValidationLeaf<TModel> Applicable(Expression<Func<TModel, bool>> applicable)
        {
            ApplicableRule = applicable.GetActualExpression();
            return this;
        }
        public ValidationLeaf<TModel> ReadOnly(Expression<Func<TModel, bool>> readOnly)
        {
            ReadOnlyRule = readOnly.GetActualExpression();
            return this;
        }
        public ValidationLeaf<TModel> ReadOnly<TProperty>(Expression<Func<TModel, bool>> readOnlyRule, Expression<Func<TModel, TProperty>> valueExpression)
        {
            if (valueExpression == null)
                throw new ArgumentNullException("valueExpression");
            readOnlyValueExpression = new PropertyExpression(valueExpression);
            ReadOnlyRule = readOnlyRule.GetActualExpression();
            return this;
        }
        public ValidationTree<TModel> ForGroup(ValidationTree<TModel> group)
        {
            Parent.Children.Add(group);
            return Parent;
        }
        public ValidationLeaf<TModel> For<TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            var leaf = new ValidationLeaf<TModel>(Parent, new PropertyExpression(property));
            Parent.Children.Add(leaf);
            return leaf;
        }

        public override bool IsLeaf
        {
            get { return true; }
        }
        public virtual void ExecuteForEachRule(TModel model)
        {
        }
    }
}