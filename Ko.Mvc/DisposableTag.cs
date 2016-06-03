using System;
using System.IO;
using System.Web.Mvc;

namespace Ko.Mvc
{
    /// <summary>
    /// Writes down the end tag of the given tag with using statement
    /// </summary>
    public class DisposableTag : IDisposable
    {
        private bool disposed;
        private readonly TextWriter writer;
        private readonly TagBuilder tagBuilder;
        
        /// <summary>
        /// Inits the inner members
        /// </summary>
        /// <param name="writer">
        /// The writer
        /// </param>
        /// <param name="tagBuilder">
        /// Given tag builder
        /// </param>
        public DisposableTag(TextWriter writer, TagBuilder tagBuilder)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (tagBuilder == null)
            {
                throw new ArgumentNullException("tagBuilder");
            }
            this.writer = writer;
            writer.WriteLine(tagBuilder.ToString(TagRenderMode.StartTag));
            this.tagBuilder = tagBuilder;
        }

        public void Dispose()
        {
            Dispose(true /* disposing */);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                writer.Write(tagBuilder.ToString(TagRenderMode.EndTag));
            }
        }

        /// <summary>
        /// Forces to dispose
        /// </summary>
        public void EndTag()
        {
            Dispose(true);
        }

    }
}