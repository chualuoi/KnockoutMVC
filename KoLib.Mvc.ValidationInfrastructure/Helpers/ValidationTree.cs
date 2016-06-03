using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using Ko.Utils.Extensions;
using KoLib.Mvc.ValidationInfrastructure.Attributes;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public class ValidationTree<TModel> : ValidationNode<TModel>, IEnumerable<ValidationNode<TModel>> where TModel : class 
    {
        private readonly List<ValidationNode<TModel>> children = new List<ValidationNode<TModel>>();
        private ValidationNode<TModel> lastChild=null;


        #region Constructors
        public ValidationTree()
            : this(null)
        {
        }

        public ValidationTree(ValidationTree<TModel> parent)
            : base(parent)
        {
        }
        #endregion

        #region Properties

        public List<ValidationNode<TModel>> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Implement index for this
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ValidationLeaf<TModel> this[string propertyName]
        {
            get
            {
                //TODO: Cache this
                var foundNodes =
                    this.Where(
                        node =>
                        node.IsLeaf &&
                        String.Equals(propertyName,
                                      ExpressionHelper.GetExpressionText(((ValidationLeaf<TModel>)node).PropertyExpression.LambdaExpression),
                                      StringComparison.Ordinal));

                return foundNodes.FirstOrDefault() as ValidationLeaf<TModel>;
            }
        }

        public ValidationLeaf<TModel> this[LambdaExpression pExpression]
        {
            get { return this[ExpressionHelper.GetExpressionText(pExpression)]; }
        }

        public ValidationLeaf<TModel> this[Expression<Func<TModel, object>> pExpression]
        {
            get { return this[pExpression.GetExpressionText()]; }
        }

        #endregion

        #region IEnumerable<ViewTree<TModel>> Members

        public IEnumerator<ValidationNode<TModel>> GetEnumerator()
        {
            return GetDepthFirstEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Methods

        public bool IsApplicable(TModel model, string propertyName)
        {
            ValidationNode<TModel> node = this[propertyName];

            while (node != null)
            {
                if (node.ApplicableRule != null)
                {
                    if (!node.ApplicableRule.IsTrue(model))
                        return false;
                }
                node = node.Parent;
            }
            return true;
        }

        public bool IsReadonly(TModel model, string propertyName)
        {
            ValidationNode<TModel> node = this[propertyName];

            while (node != null)
            {
                if (node.ReadOnlyRule != null)
                {
                    if (node.ReadOnlyRule.IsTrue(model))
                        return true;
                }
                node = node.Parent;
            }
            return false;
        }

        #region For

        /// <summary>
        /// Add a field (property) with no specific applicable rule
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ValidationTree<TModel> For<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            //TODO: Check duplication of property
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            var newItem = new ValidationLeaf<TModel>(this, new PropertyExpression(expression));

            children.Add(newItem);

            lastChild = newItem;

            return this;
        }

        #endregion

        #region ForGroup

        /// <summary>
        /// Adding a group with no specific applicable rule to this tree
        /// </summary>
        /// <param name="subTree">The sub tree.</param>
        /// <returns></returns>
        public ValidationTree<TModel> ForGroup(ValidationTree<TModel> subTree)
        {
            if (subTree == null)
            {
                throw new ArgumentNullException("subTree");
            }
            subTree.Parent = this;
            Children.Add(subTree);
            lastChild = subTree;

            return this;
        }
        #endregion

        #region ForEach

        public ForEachLeaf<TModel, TItem> ForEach<TItem>(Expression<Func<TModel, List<TItem>>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            var newGroup = new ForEachLeaf<TModel, TItem>(this, new PropertyExpression(expression));
            Children.Add(newGroup);
            return newGroup;
        }

        #endregion

        public ValidationTree<TModel> Applicable(Expression<Func<TModel, bool>> expression)
        {
            if (lastChild == null)
            {
                ApplicableRule = expression.GetActualExpression();
            }
            else
            {
                lastChild.ApplicableRule = expression;
            }
            return this;
        }

        public ValidationTree<TModel> ReadOnly(Expression<Func<TModel, bool>> conditionExpression)
        {
            if (lastChild == null)
            {
                ReadOnlyRule = conditionExpression.GetActualExpression();
            }
            else
            {
                lastChild.ReadOnlyRule = conditionExpression;
            }
            return this;
        }
        
        public ValidationTree<TModel> ReadOnly<TProperty>(Expression<Func<TModel, bool>> readOnlyRule, Expression<Func<TModel, TProperty>> valueExpression)
        {
            if (valueExpression == null)
                throw new ArgumentNullException("valueExpression");
            if (lastChild != null)
            {
                lastChild.ReadOnlyRule = readOnlyRule.GetActualExpression();
                var xlastChild = lastChild as ValidationLeaf<TModel>;
                if (xlastChild != null)
                {
                    xlastChild.ReadOnlyValueExpression = new PropertyExpression(valueExpression);
                }
            }

            return this;
        }

        public override IEnumerator<ValidationNode<TModel>> GetDepthFirstEnumerator()
        {
            yield return this;
            foreach (var kid in children)
            {
                IEnumerator<ValidationNode<TModel>> kidenumerator = kid.GetDepthFirstEnumerator();
                while (kidenumerator != null && kidenumerator.MoveNext())
                    yield return kidenumerator.Current;
            }
        }

        private void ProcessRuleForProperties(TModel model, List<PropertyExpression> inapplicableProperties)
        {
            List<String> inAppProperties =inapplicableProperties.Select(p => ExpressionHelper.GetExpressionText(p.LambdaExpression)).ToList();
            IDictionary<string, List<PropertyInfo>> propertiesMapper = GetPropertiesMapper(model, inAppProperties);
            foreach (KeyValuePair<string, List<PropertyInfo>> keyValuePair in propertiesMapper)
            {
                foreach (PropertyInfo propertyInfo in keyValuePair.Value)
                {
                    if (!inapplicableProperties.Exists(p => ExpressionHelper.GetExpressionText(p.LambdaExpression) == propertyInfo.Name))
                    {
                        ParameterExpression param = Expression.Parameter(propertyInfo.ReflectedType, "x");
                        LambdaExpression lambda = Expression.Lambda(Expression.Property(param, propertyInfo), param);
                        inapplicableProperties.Add(new PropertyExpression(lambda));
                    }
                }
            }
        }

        private IDictionary<string, List<PropertyInfo>> GetPropertiesMapper(TModel model, List<String> inAppProperties)
        {
            // Should consider to cache propertiesMapper if has performance issue
            IDictionary<string, List<PropertyInfo>> propertiesMapper = new Dictionary<string, List<PropertyInfo>>();

            // Scan RuleFor attributes
            model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => p.GetCustomAttributes(typeof(RuleForAttribute), false).Any())
                .ToList().ForEach(p =>
                    {
                        var ruleName = p.GetCustomAttributes(typeof(RuleForAttribute), false).Cast<RuleForAttribute>().First().Property;
                        if (!propertiesMapper.ContainsKey(ruleName))
                        {
                            propertiesMapper[ruleName] = new List<PropertyInfo>();
                        }
                        propertiesMapper[ruleName].Add(p);
                    });


            // Scan RuleForMultiple attributes
            model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => p.GetCustomAttributes(typeof(RuleForMultipleAttribute), false).Any())
                .ToList()
                .ForEach(p =>
                    {
                        var ruleNames =
                            p.GetCustomAttributes(typeof(RuleForMultipleAttribute), false).Cast
                                <RuleForMultipleAttribute>().First().Properties;
                        foreach (string ruleName in ruleNames)
                        {
                            if (!propertiesMapper.ContainsKey(ruleName))
                            {
                                propertiesMapper[ruleName] = new List<PropertyInfo>();
                            }
                            propertiesMapper[ruleName].Add(p);
                        }
                    });

            IDictionary<string, List<PropertyInfo>> mapperResult = new Dictionary<string, List<PropertyInfo>>();
            foreach (string inAppProperty in inAppProperties)
            {
                if(propertiesMapper.ContainsKey(inAppProperty))
                {
                    mapperResult[inAppProperty] = propertiesMapper[inAppProperty];
                }
            }

            return mapperResult;
        }

        
        /// <summary>
        /// This function will return list of all properties which will always be readonly on the screen
        /// This list will not contains reference data and computed properties
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<PropertyExpression> GetLockedReadOnlyProperties(TModel model)
        {
            var staticReadOnlyProperties = new HashSet<PropertyExpression>();
            foreach (var node in this)
            {
                if (node.IsLeaf)
                {
                    var tmp = (ValidationLeaf<TModel>)node;
                    if (tmp.ReadOnlyRule != null && IsStaticReadOnlyRule(tmp.ReadOnlyRule, model) && tmp.ReadOnlyRule.IsTrue(model) && tmp.ReadOnlyValueExpression == null)
                    {
                        staticReadOnlyProperties.Add(tmp.PropertyExpression);
                    }
                }
                else
                {
                    var childTree = (ValidationTree<TModel>)node;
                    if (childTree.ReadOnlyRule != null && IsStaticReadOnlyRule(childTree.ReadOnlyRule, model) &&
                        childTree.ReadOnlyRule.IsTrue(model))
                    {
                        foreach (var childNode in childTree)
                        {
                            if (childNode.IsLeaf)
                                staticReadOnlyProperties.Add(((ValidationLeaf<TModel>)childNode).PropertyExpression);
                        }
                    }
                }
            }
            return staticReadOnlyProperties.ToList();
        }

        public List<PropertyExpression> GetInapplicableProperties(TModel model)
        {
            //TODO add parent node only
            //TODO check if optimization needed
            var inapplicableProperties = new List<PropertyExpression>();
            foreach (var node in this)
            {
                if (node.IsLeaf)
                {
                    ((ValidationLeaf<TModel>)node).ExecuteForEachRule(model);
                    if (
                        !IsApplicable(model,
                                      ExpressionHelper.GetExpressionText(((ValidationLeaf<TModel>)node).PropertyExpression.LambdaExpression)))
                    {
                        var leaf = (ValidationLeaf<TModel>)node;

                        //Check whether parent of this property already in the list
                        var leafPath = ExpressionHelper.GetExpressionText(leaf.PropertyExpression.LambdaExpression);
                        if (!inapplicableProperties.Any(x => leafPath.Contains(ExpressionHelper.GetExpressionText(x.LambdaExpression) + ".") || leafPath.Contains(ExpressionHelper.GetExpressionText(x.LambdaExpression) + "[")))
                        {
                            inapplicableProperties.Add(leaf.PropertyExpression);
                        }
                    }
                }
            }

            ProcessRuleForProperties(model, inapplicableProperties);

            return inapplicableProperties;
        }


        public Dictionary<PropertyExpression, PropertyExpression> GetComputedReadOnlyProperties(TModel model)
        {
            var computedReadOnlyProperties = new Dictionary<PropertyExpression, PropertyExpression>();
            foreach (var node in this)
            {
                if (node.IsLeaf)
                {
                    var tmp = (ValidationLeaf<TModel>)node;
                    if (tmp.ReadOnlyRule != null && tmp.ReadOnlyRule.IsTrue(model) && (!IsStaticReadOnlyRule(tmp.ReadOnlyRule, model) || tmp.ReadOnlyValueExpression != null) && IsApplicable(model, ExpressionHelper.GetExpressionText(((ValidationLeaf<TModel>)node).PropertyExpression.LambdaExpression)))
                    {
                        computedReadOnlyProperties.Add(tmp.PropertyExpression, tmp.ReadOnlyValueExpression);
                    }
                }
            }
            return computedReadOnlyProperties;
        }

        #endregion

        public override bool IsLeaf
        {
            get { return false; }
        }


        /// <summary>
        /// A rule is static if it depends on non user-editable fields only
        /// so its result can be determined at the screen loading time and will not be changed by user
        /// Non user-editable fields may be:
        ///     A property decorated with [ReadOnlyAttribute(true)]
        ///     A property with readonly status determined by another static readonly rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsStaticReadOnlyRule(RuleExpression<TModel> rule, TModel model)
        {
            //Get all properties that this rule depends on
            //Check if all dependency properties are reference data (read-only)
            var dependProperties = FactorVisitor.GetFactors(rule);
            foreach (var dependProperty in dependProperties)
            {
                var lamda = dependProperty as MemberExpression;
                if (lamda != null)
                {
                    //reference to another static rule to support rule like this:
                    //public static Rule PageReadOnly = new Rule(m => m.IsSessionReadOnly || R.ReadOnly.IsTrue(m));
                    if (lamda.Expression == null) 
                    {
                        var ruleType = lamda.Member.DeclaringType;
                        var ruleField = ruleType.GetField(lamda.Member.Name, BindingFlags.Public | BindingFlags.Static);
                        if (ruleField == null)
                        {
                            throw new ArgumentException("Rule not found", lamda.Member.Name);
                        }
                        var ruleExpression = ruleField.GetValue(null) as RuleExpression<TModel>;
                        if (IsStaticReadOnlyRule(ruleExpression, model))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        var propertyInfo = lamda.Member as PropertyInfo;
                        //if property is readonly, its value wil be pre-determined
                        //TODO : check HiddenInputAttribute
                        if (propertyInfo != null &&
                            (propertyInfo.HasAttribute(typeof (ReadOnlyAttribute)) ||
                             propertyInfo.HasAttribute(typeof (HiddenInputAttribute))))
                        {
                            continue;
                        }

                        var node = this[lamda.GetExpressionText()];
                        if (node != null
                            && node.ReadOnlyRule.IsTrue(model) //this node is readonly
                            && IsStaticReadOnlyRule(node.ReadOnlyRule, model))
                            //and the rule governs it is static readonly rule also
                        {
                            continue;
                        }
                    }
                    return false;
                }
            }
            return true;
        }

    }
}