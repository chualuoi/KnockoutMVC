using System;
using System.Web.Mvc;

namespace Ko.Mvc
{
    public class DisposableKnockoutBuilder<TContext, TParent, TModel> : KnockoutBuilder<TContext, TParent, TModel>, IDisposable
    {
        #region Ctors

        public DisposableKnockoutBuilder(HtmlHelper<TModel> htmlHelper, KoBindings<TModel> bindings)
            : base(htmlHelper)
        {
            if (bindings == null)
            {
                throw new ArgumentNullException("bindings");
            }
            if (!bindings.VirtualElementAllowed)
            {
                throw new ArgumentException("The given binding is not allowed in virtual element");
            }

            Html.ViewContext.Writer.Write("<!-- ko {0} -->", bindings.Value);
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true /* disposing */);
            GC.SuppressFinalize(this);
        }

        #endregion

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                Html.ViewContext.Writer.Write("<!-- /ko -->");
            }
        }

        /// <summary>
        /// Forces to dispose
        /// </summary>
        public void EndElement()
        {
            Dispose(true);
        }
    }
}