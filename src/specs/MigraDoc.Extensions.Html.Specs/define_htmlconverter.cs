using MigraDoc.DocumentObjectModel;
using NSpec;

namespace MigraDoc.Extensions.Html.Specs
{
    class define_htmlconverter : nspec
    {
        HtmlConverter converter;
        Section pdf;

        void before_each()
        {
            converter = new HtmlConverter(1.0);
            pdf = new Section();
        }
        
        void adding_custom_handlers()
        {
            before = () => converter.NodeHandlers.Add(
                    "img", (node, parent, parentSection) => ((Section)parent).AddParagraph()
                );
            
            act = () => pdf.Add("<img/>", converter);

            it["adds the handler to the converter"] = ()
                => converter.NodeHandlers["img"].should_not_be_null();

            it["uses the handler when processing"] = ()
                => pdf.LastParagraph.should_not_be_null();
        }
    }
}
