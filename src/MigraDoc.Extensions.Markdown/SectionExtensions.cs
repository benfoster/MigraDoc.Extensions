using MigraDoc.DocumentObjectModel;

namespace MigraDoc.Extensions.Markdown
{
    public static class SectionExtensions
    {
        public static Section AddMarkdown(this Section section, string markdown, double nestedListStartingLeftIndent = 1.0)
        {
            return section.Add(markdown, new MarkdownConverter(nestedListStartingLeftIndent));
        }
    }
}
