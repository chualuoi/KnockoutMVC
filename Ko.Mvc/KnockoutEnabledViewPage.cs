using System.Web.Mvc;
using Ko.Core;

namespace Ko.Mvc
{
    /// <summary>
    /// Uses when accessing level 0 context as the binding context for page
    /// </summary>
    /// <typeparam name="TContext">
    /// Context type for binding
    /// </typeparam>
    //public abstract class KnockoutEnabledViewPage<TContext> : WebViewPage<TContext>
    public abstract class KnockoutEnabledViewPage<TContext> : KnockoutEnabledViewPage<TContext, TContext, TContext>
    {
        ///// <summary>
        ///// Uses to apply bindings to a specific element
        ///// </summary>
        //public KnockoutBinder<TContext> Ko { get; private set; }

        //protected KnockoutEnabledViewPage()
        //{
        //    Ko = new KnockoutBinder<TContext>();
        //}

        public override abstract void Execute();
        
        //protected override void InitializePage()
        //{
        //    //TODO: Remove duplication
        //    if (!ViewData.ContainsKey("rootModel"))
        //    {
        //        ViewData["rootModel"] = Model;
        //        Html.ViewData["rootModel"] = Model;
        //    }
        //    if (!ViewData.ContainsKey("htmlViewContext"))
        //    {
        //        ViewData["htmlViewContext"] = Html.ViewContext;
        //        Html.ViewData["htmlViewContext"] = Html.ViewContext;
        //    }
        //    if (!ViewData.ContainsKey("htmlViewDataContainer"))
        //    {
        //        ViewData["htmlViewDataContainer"] = Html.ViewDataContainer;
        //        Html.ViewData["htmlViewDataContainer"] = Html.ViewDataContainer;
        //    }            

        //    base.InitializePage();
        //}
    }

    /// <summary>
    /// Uses when accessing level 1 context as the binding context for page
    /// </summary>
    /// <typeparam name="TParent">
    /// Root & Parent context type
    /// </typeparam>
    /// <typeparam name="TData">
    /// Current context type
    /// </typeparam>
    public abstract class KnockoutEnabledViewPage<TParent, TData> : KnockoutEnabledViewPage<TParent, TParent, TData>
    {
        public override abstract void Execute();
    }

    /// <summary>
    /// Uses when accessing level>1 context as the binding context for page
    /// </summary>
    /// <typeparam name="TRoot">
    /// Root context type
    /// </typeparam>
    /// <typeparam name="TParent">
    /// Parent context type
    /// </typeparam>
    /// <typeparam name="TData">
    /// Current context type
    /// </typeparam>
    public abstract class KnockoutEnabledViewPage<TRoot, TParent, TData> : WebViewPage<TData>
    {
        ///// <summary>
        ///// Uses to apply bindings to a specific element
        ///// </summary>
        //public KnockoutBinder<IKoContext<TRoot, TParent, TData>> Ko { get; private set; }

        /// <summary>
        /// Uses to apply bindings to a specific element
        /// </summary>
        public KnockoutBuilder<TRoot, TParent, TData> Ko { get; private set; }

        //protected KnockoutEnabledViewPage()
        //{
        //    Ko = new KnockoutBinder<IKoContext<TRoot, TParent, TData>>();
        //}

        public override abstract void Execute();

        protected override void InitializePage()
        {
            Ko = new KnockoutBuilder<TRoot, TParent, TData>(Html);

            //TODO: Remove duplication
            if (!ViewData.ContainsKey("rootModel"))
            {
                ViewData["rootModel"] = Model;
                Html.ViewData["rootModel"] = Model;
            }
            if (!ViewData.ContainsKey("htmlViewContext"))
            {
                ViewData["htmlViewContext"] = Html.ViewContext;
                Html.ViewData["htmlViewContext"] = Html.ViewContext;
            }
            if (!ViewData.ContainsKey("htmlViewDataContainer"))
            {
                ViewData["htmlViewDataContainer"] = Html.ViewDataContainer;
                Html.ViewData["htmlViewDataContainer"] = Html.ViewDataContainer;
            }            

            base.InitializePage();
        }
    }
}
