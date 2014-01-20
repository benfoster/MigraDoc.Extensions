using MigraDoc.DocumentObjectModel;

namespace MigraDoc.Extensions.Html
{
    public static class SectionExtensions
    {
        public static Section AddHtml(this Section section, string html, double nestedListStartingLeftIndent = 1.0)
        {
            return section.Add(html, new HtmlConverter(nestedListStartingLeftIndent));
        }
    }
}
