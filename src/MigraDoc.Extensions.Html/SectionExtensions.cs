using MigraDoc.DocumentObjectModel;
using System;

namespace MigraDoc.Extensions.Html
{
    public static class SectionExtensions
    {
        public static void AddHtml(this Section section, string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                throw new ArgumentNullException("html");
            }

            var converter = new HtmlConverter();
            var conversion = converter.Convert(html);
            conversion(section);
        }
    }
}
