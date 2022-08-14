using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TemplateFiller.Core.Models;

namespace TemplateFiller.Core
{
    public class LocalFileProcessor : IFileProcessor
    {
        private static Regex _regex = new Regex(@"<<(.*?)>>");

        private Document _document;

        public LocalFileProcessor()
        {
            _document = new Document();
        }

        public void LoadFile(string fileName)
        {
            try
            {
                _document.LoadFromFile(fileName);
            }
            catch (Exception)
            {
                throw new Exception("Error opening file.");
            }
        }

        public List<DataField> GetDataFields()
        {
            return _document
                .FindAllPattern(_regex)
                .Select(ts => string.Concat(ts.SelectedText.Skip(2).Take(ts.SelectedText.Length - 4)).Split(';'))
                .Where(s => s.Length == 2)
                .Select(s => new DataField
                (
                    name: s[0],
                    description: s[1].Trim()
                ))
                .DistinctBy(df => df.Name)
                .ToList();
        }

        private void PopulateDataFields(List<DataField> dataFields)
        {
            foreach (var dataField in dataFields)
            {
                var regex = new Regex($@"<<{dataField.Name};(.*?)>>");
                _document.Replace(regex, dataField.Value);
            }
        }

        public void Save(List<DataField> dataFields, string fileName)
        {
            PopulateDataFields(dataFields);
            _document.SaveToFile(fileName);
        }
    }
}