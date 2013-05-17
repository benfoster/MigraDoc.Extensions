using MarkdownSharp;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Extensions.Html;
using System;

namespace MigraDoc.Extensions.Markdown
{
    public class MarkdownConverter : IConverter
    {
        private readonly MarkdownOptions options;

        public MarkdownConverter()
        {
            options = new MarkdownOptions
            {
                LinkEmails = true
            };
        }

        public MarkdownConverter(MarkdownOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.options = options;
        }
        
        public Action<Section> Convert(string contents)
        {
            var converter = new MarkdownSharp.Markdown(options);
            var html = converter.Transform(contents);

            var htmlConverter = new HtmlConverter();
            return htmlConverter.Convert(html);
        }
    }
}
