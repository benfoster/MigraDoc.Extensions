using MigraDoc.DocumentObjectModel;
using System;

namespace MigraDoc.Extensions
{
    public static class SectionExtensions
    {
        public static Section Add(this Section section, string contents, IConverter converter)
        {
            if (string.IsNullOrEmpty(contents))
            {
                throw new ArgumentNullException("contents");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            var addAction = converter.Convert(contents);
            addAction(section);
            return section;
        }
    }
}
