using System.Collections.Generic;
using TemplateFiller.Core.Models;

namespace TemplateFiller.Core
{
    public interface IFileProcessor
    {
        List<DataField> GetDataFields();
        void LoadFile(string fileName);
        void Save(List<DataField> dataFields, string fileName);
    }
}