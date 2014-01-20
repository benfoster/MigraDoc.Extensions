using MarkdownSharp;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Extensions.Html;
using System;

namespace MigraDoc.Extensions.Markdown
{
    public class MarkdownConverter : IConverter
    {
        private readonly MarkdownOptions _options;
        private readonly double _nestedListStartingLeftIndent = 1.0;

        public MarkdownConverter(double nestedListStartingLeftIndent)
        {
            _options = new MarkdownOptions
            {
                LinkEmails = true
            };
            _nestedListStartingLeftIndent = nestedListStartingLeftIndent;
        }

        public MarkdownConverter(MarkdownOptions options, double nestedListStartingLeftIndent)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this._options = options;
            _nestedListStartingLeftIndent = nestedListStartingLeftIndent;
        }
        
        public Action<Section> Convert(string contents)
        {
            var converter = new MarkdownSharp.Markdown(_options);
            var html = converter.Transform(contents);

            var htmlConverter = new HtmlConverter(_nestedListStartingLeftIndent);
            return htmlConverter.Convert(html);
        }
    }
}
