using HtmlAgilityPack;
using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MigraDoc.Extensions.Html
{
    public class HtmlConverter : IConverter
    {
        private IDictionary<string, Func<HtmlNode, DocumentObject, DocumentObject>> nodeHandlers
            = new Dictionary<string, Func<HtmlNode, DocumentObject, DocumentObject>>();
           
        public HtmlConverter()
        {
            AddDefaultNodeHandlers();
        }

        public IDictionary<string, Func<HtmlNode, DocumentObject, DocumentObject>> NodeHandlers
        {
            get
            {
                return nodeHandlers;
            }
        }
        
        public Action<Section> Convert(string contents)
        {
            return section => ConvertHtml(contents, section);
        }

        private void ConvertHtml(string html, Section section)
        {
            if (string.IsNullOrEmpty(html))
            {
                throw new ArgumentNullException("html");
            }

            if (section == null)
            {
                throw new ArgumentNullException("section");
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            ConvertHtmlNodes(doc.DocumentNode.ChildNodes, section);
        }

        private void ConvertHtmlNodes(HtmlNodeCollection nodes, DocumentObject section, DocumentObject current = null)
        {
            foreach (var node in nodes)
            {
                Func<HtmlNode, DocumentObject, DocumentObject> nodeHandler;
                if (nodeHandlers.TryGetValue(node.Name, out nodeHandler))
                {
                    // pass the current container or section
                    var result = nodeHandler(node, current ?? section);
                    
                    if (node.HasChildNodes)
                    {
                        ConvertHtmlNodes(node.ChildNodes, section, result);
                    }
                }
                else
                {
                    if (node.HasChildNodes)
                    {
                        ConvertHtmlNodes(node.ChildNodes, section, current);
                    }
                }
            }
        }
        
        private void AddDefaultNodeHandlers()
        {
            // Block Elements
            
            // could do with a predicate/regex matcher so we could just use one handler for all headings
            nodeHandlers.Add("h1", AddHeading);
            nodeHandlers.Add("h2", AddHeading);
            nodeHandlers.Add("h3", AddHeading);
            nodeHandlers.Add("h4", AddHeading);
            nodeHandlers.Add("h5", AddHeading);
            nodeHandlers.Add("h6", AddHeading);

            nodeHandlers.Add("p", (node, parent) =>
            {
                return ((Section)parent).AddParagraph();
            });

            // Inline Elements

            nodeHandlers.Add("strong", (node, parent) => AddFormattedText(node, parent, TextFormat.Bold));
            nodeHandlers.Add("i", (node, parent) => AddFormattedText(node, parent, TextFormat.Italic));
            nodeHandlers.Add("em", (node, parent) => AddFormattedText(node, parent, TextFormat.Italic));
            nodeHandlers.Add("u", (node, parent) => AddFormattedText(node, parent, TextFormat.Underline));
            nodeHandlers.Add("a", (node, parent) =>
            {
                return GetParagraph(parent).AddHyperlink(node.GetAttributeValue("href", ""), HyperlinkType.Web);
            });
            nodeHandlers.Add("hr", (node, parent) => GetParagraph(parent).SetStyle("HorizontalRule"));
            nodeHandlers.Add("br", (node, parent) => {
                if (parent is FormattedText)
                {
                    // inline elements can contain line breaks
                    ((FormattedText)parent).AddLineBreak();
                    return parent;
                }
                                
                var paragraph = GetParagraph(parent);
                paragraph.AddLineBreak();
                return paragraph;
            });

            nodeHandlers.Add("li", (node, parent) =>
            {
                var listStyle = node.ParentNode.Name == "ul"
                    ? "UnorderedList"
                    : "OrderedList";

                var section = (Section)parent;
                var isFirst = node.ParentNode.Elements("li").First() == node;
                var isLast = node.ParentNode.Elements("li").Last() == node; 

                // if this is the first item add the ListStart paragraph
                if (isFirst)
                {
                    section.AddParagraph().SetStyle("ListStart");
                }

                var listItem = section.AddParagraph().SetStyle(listStyle);

                // disable continuation if this is the first list item
                listItem.Format.ListInfo.ContinuePreviousList = !isFirst;

                // if the this is the last item add the ListEnd paragraph
                if (isLast)
                {
                    section.AddParagraph().SetStyle("ListEnd");
                }

                return listItem;
            });

            nodeHandlers.Add("#text", (node, parent) =>
            {
                // remove line breaks
                var innerText = node.InnerText.Replace("\r", "").Replace("\n", "");

                if (string.IsNullOrWhiteSpace(innerText))
                {
                    return parent;
                }

                // decode escaped HTML
                innerText = WebUtility.HtmlDecode(innerText);
                
                // text elements must be wrapped in a paragraph but this could also be FormattedText or a Hyperlink!!
                // this needs some work
                if (parent is FormattedText)
                {
                    return ((FormattedText)parent).AddText(innerText);
                }
                if (parent is Hyperlink)
                {
                    return ((Hyperlink)parent).AddText(innerText);
                }

                // otherwise a section or paragraph
                return GetParagraph(parent).AddText(innerText);
            });
        }

        private static DocumentObject AddFormattedText(HtmlNode node, DocumentObject parent, TextFormat format)
        {
            var formattedText = parent as FormattedText;
            if (formattedText != null)
            {
                return formattedText.Format(format);
            }

            // otherwise parent is paragraph or section
            return GetParagraph(parent).AddFormattedText(format);
        }

        private static DocumentObject AddHeading(HtmlNode node, DocumentObject parent)
        {
            return ((Section)parent).AddParagraph().SetStyle("Heading" + node.Name[1]);
        }

        private static Paragraph GetParagraph(DocumentObject parent)
        {
            return parent as Paragraph ?? ((Section)parent).AddParagraph();
        }

        private static Paragraph AddParagraphWithStyle(DocumentObject parent, string style) 
        {
            return ((Section)parent).AddParagraph().SetStyle(style);
        }
    }
}
