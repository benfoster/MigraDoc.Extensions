using MigraDoc.DocumentObjectModel;
using System;

namespace MigraDoc.Extensions.Html
{
    public static class ParagraphExtensions
    {
        public static Paragraph AddStyle(this Paragraph paragraph, string style)
        {
            if (paragraph == null)
            {
                throw new ArgumentNullException("paragraph");
            }
            if (string.IsNullOrEmpty(style))
            {
                throw new ArgumentNullException("style");
            }

            paragraph.Style = style;
            return paragraph;
        }
    }
}
