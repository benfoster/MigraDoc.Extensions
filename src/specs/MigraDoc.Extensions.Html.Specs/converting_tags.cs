using MigraDoc.DocumentObjectModel;
using NSpec;

namespace MigraDoc.Extensions.Html.Specs
{
    class converting_tags : nspec
    {
        Section pdf;

        string html;

        void before_each()
        {
            pdf = new Section();
        }

        void act_each()
        {
            pdf.AddHtml(html);
        }

        void plain_text()
        {
            before = () => html = "test";
            it["adds a paragraph to the pdf containing the provided text"] = ()
                => pdf.LastParagraph.should_not_be_null()
                    .Elements[0].should_cast_to<Text>()
                        .Content.should_be("test");
        }

        void paragraph_tag()
        {
            context["when the tag is empty"] = () =>
            {
                before = () => html = "<p></p>";
                it["adds an empty paragraph to the pdf"] = ()
                    => pdf.LastParagraph.Elements.Count.should_be(0);
            };

            context["when the tag contains plain text"] = () =>
            {
                before = () => html = "<p>test</p>";
                it["adds a paragraph to the pdf containing the provided text"] = ()
                    => pdf.LastParagraph.Elements[0].should_cast_to<Text>()
                        .Content.should_be("test");
            };
        }

        void heading_tag()
        {
            before = () => html = "<h1></h1>";

            it["adds a paragraph with the relevant 'HeadingX' style"] = ()
                => pdf.LastParagraph.Style.should_be("Heading1");

            context["when the tag contains text"] = () =>
            {
                before = () => html = "<h1>test</h1>";
                it["the paragraph should contain the provided text"] = ()
                    => pdf.LastParagraph.Elements[0].should_cast_to<Text>()
                        .Content.should_be("test");
            };
        }

        void heading1_tag()
        {
            before = () => html = "<h1></h1>";
            it["adds a paragraph with style 'Heading1'"] = ()
                => pdf.LastParagraph.Style.should_be("Heading1");
        }

        void heading2_tag()
        {
            before = () => html = "<h2></h2>";
            it["adds a paragraph with style 'Heading2'"] = ()
                => pdf.LastParagraph.Style.should_be("Heading2");
        } 

        void heading3_tag()
        {
            before = () => html = "<h3></h3>";
            it["adds a paragraph with style 'Heading3'"] = ()
                => pdf.LastParagraph.Style.should_be("Heading3");
        }

        void heading4_tag()
        {
            before = () => html = "<h4></h4>";
            it["adds a paragraph with style 'Heading4'"] = ()
                => pdf.LastParagraph.Style.should_be("Heading4");
        }

        void heading5_tag()
        {
            before = () => html = "<h5></h5>";
            it["adds a paragraph with style 'Heading5'"] = ()
                => pdf.LastParagraph.Style.should_be("Heading5");
        }

        void heading6_tag()
        {
            before = () => html = "<h6></h6>";
            it["adds a paragraph with style 'Heading6'"] = ()
                => pdf.LastParagraph.Style.should_be("Heading6");
        }

        void strong_tag()
        {
            before = () => html = "<strong>test</strong>";
            it["adds a paragraph with bold text"] = ()
                => pdf.LastParagraph.should_not_be_null()
                    .Elements[0].should_cast_to<FormattedText>()
                        .Bold.should_be_true();
        }

        void italic_tag()
        {
            before = () => html = "<i>test</i>";
            it["adds a paragraph with italic text"] = ()
                => pdf.LastParagraph.should_not_be_null()
                    .Elements[0].should_cast_to<FormattedText>()
                        .Italic.should_be_true();
        }

        void underline_tag()
        {
            before = () => html = "<u>test</u>";
            it["adds a paragraph with underlined text"] = ()
                => pdf.LastParagraph.should_not_be_null()
                    .Elements[0].should_cast_to<FormattedText>()
                        .Underline.should_be(Underline.Single);
        }

        void nested_bold_italic_and_underline_tags()
        {
            FormattedText text = null;
            before = () => html = "<strong><i><u>test</ul></i></strong>";

            act = () =>
            {
                text = pdf.LastParagraph.Elements[0] as FormattedText;
            };

            it["adds a paragraph with formatted text"] = ()
                => text.should_not_be_null();

            it["the text is bold"] = () => text.Bold.should_be_true();
            it["the text is italic"] = () => text.Italic.should_be_true();
            it["the text is underlined"] = () => text.Underline.should_be(Underline.Single);
        }

        void anchor_tag()
        {
            Hyperlink link = null;

            before = () => html = "<a href='http://www.google.com'>test</a>";

            act = () => link = pdf.LastParagraph.Elements[0] as Hyperlink;

            it["adds a hyperlink"] = () => link.should_not_be_null();
            it["sets the link to the anchor's href"] = () => link.Name.should_be("http://www.google.com");
            it["contains the anchor text"] = () 
                => link.Elements[0].should_cast_to<Text>()
                    .Content.should_be("test");
        }

        void div_tag()
        {
            before = () => html = "<div><p><strong>test</strong></p></div>";

            it["processes the inner html as normal"] = ()
                => pdf.LastParagraph.Elements[0].should_cast_to<FormattedText>()
                    .Elements[0].should_cast_to<Text>()
                        .Content.should_be("test");
        }

        void heading_followed_by_a_paragraph()
        {
            before = () => html = "<h1>Heading</h1><p>Content</p>";

            it["adds a heading paragraph"] = ()
                => pdf.Elements[0].should_cast_to<Paragraph>()
                        .Style.should_be("Heading1");

            it["adds a text paragraph"] = ()
                => pdf.LastParagraph.Elements[0].should_cast_to<Text>()
                    .Content.should_be("Content");
        }

        void whitespace_between_tags()
        {
            before = () => html = @"
                <p>One</p>


                <p>Two</p>
            ";

            it["removes the whitespace"] = ()
                => pdf.Elements.Count.should_be(2);
        }

        void unordered_list()
        {
            before = () => html = @"
                <ul>
                    <li>Item 1</li>
                    <li>Item 2</li>
                    <li>Item 3</li>
                </ul>
            ";

            it["adds a paragraph for each list item with the style 'UnorderedList'"] = () 
                => {

                    pdf.Elements.Count.should_be(3);
                    for (int i = 0, j = pdf.Elements.Count; i < j; i++)
                    {
                        var li = pdf.Elements[i] as Paragraph;
                        li.Style.should_be("UnorderedList");
                    }
            };
          
        }
    }
}
