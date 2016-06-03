using System;
using System.IO;
using System.Web.Mvc;

namespace Ko.Mvc
{
    /// <summary>
    /// Uses to enclose the end tag of Knockout Virtual element with using statement
    /// </summary>
    public class VirtualElement : IDisposable
    {
        private bool disposed;
        private readonly TextWriter writer;

        public VirtualElement(ViewContext viewContext)
        {
            if (viewContext == null) {
                throw new ArgumentNullException("viewContext");
            }
            writer = viewContext.Writer;
        }

        public void Dispose() {
            Dispose(true /* disposing */);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                disposed = true;
                writer.Write("<!-- /ko -->");
            }
        }

        /// <summary>
        /// Forces to dispose
        /// </summary>
        public void EndElement() {
            Dispose(true);
        }

    }
}