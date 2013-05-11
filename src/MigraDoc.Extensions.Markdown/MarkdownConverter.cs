using MarkdownSharp;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Extensions.Html;
using System;

namespace MigraDoc.Extensions.Markdown
{
    class MarkdownConverter : IConverter
    {
        private readonly MarkdownOptions options;

        public MarkdownConverter()
        {
            options = new MarkdownOptions
            {
                AutoNewLines = true,
                LinkEmails = true
            };
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
