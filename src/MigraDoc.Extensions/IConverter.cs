using MigraDoc.DocumentObjectModel;
using System;

namespace MigraDoc.Extensions
{
    public interface IConverter
    {
        Action<Section> Convert(string contents);
    }
}
