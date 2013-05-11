using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;

namespace MigraDoc.Extensions
{
    public static class FormattedTextExtensions
    {
        private static Dictionary<TextFormat, Action<FormattedText>> formats
            = new Dictionary<TextFormat, Action<FormattedText>>
            {
                { TextFormat.Bold, text => text.Bold = true },
                { TextFormat.NotBold, text => text.Bold = false },
                { TextFormat.Italic, text => text.Italic = true },
                { TextFormat.NotItalic, text => text.Italic = false },
                { TextFormat.Underline, text => text.Underline = Underline.Single },
                { TextFormat.NoUnderline, text => text.Underline = Underline.None }
            };

        public static FormattedText Format(this FormattedText formattedText, TextFormat textFormat)
        {
            if (formattedText == null)
            {
                throw new ArgumentNullException("formattedText");
            }

            Action<FormattedText> formatter;
            if (formats.TryGetValue(textFormat, out formatter))
            {
                formatter(formattedText);
            }

            return formattedText;
        }
    }
}
