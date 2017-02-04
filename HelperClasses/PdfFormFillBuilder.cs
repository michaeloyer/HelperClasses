using System;
using System.Collections.Generic;

namespace HelperClasses
{
    /// <summary>
    /// Builder class that produces file content for an FDF file.
    /// </summary>
    public class PdfFormFillBuilder
    {
        /// <summary>
        /// Relative or Absoulte Path
        /// </summary>
        public string pdfPath { get; }
        private readonly HashSet<Field> fields;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdfPath">Relative or Absolute Path</param>
        public PdfFormFillBuilder(string pdfPath)
        {
            this.pdfPath = pdfPath.Replace(@"\", @"\\");
            fields = new HashSet<Field>();
        }

        public string Content =>
            string.Join(Environment.NewLine,
                "%FDF-1.2",
                "%âãÏÓ",
                $"1 0 obj<</FDF<</F({pdfPath})/Fields 2 0 R>>>>",
                "endobj",
                "2 0 obj[",
                string.Join(Environment.NewLine, fields),
                "]",
                "endobj",
                "trailer",
                "<</Root 1 0 R>>",
                "%%EO");

        /// <summary>
        /// Adds a field to the FDF document
        /// </summary>
        /// <param name="name">Name of the field (Case Sensitive)</param>
        /// <param name="value">Value of the field</param>
        public void AddField(string name, string value)
        {
            if (!fields.Add(new Field(name, value)))
                throw new ArgumentException($"FDF Error: Duplicate field attempting to be added: \n\nfield name:{name}, value: {value}");
        }

        private class Field : IEquatable<Field>
        {
            public string Name { get; }
            public string Value { get; }

            public string Content => $"<</T({Name})/V({Value})>>";

            public Field(string Name, string Value)
            {
                if (Name == null)
                    throw new ArgumentNullException("The field name cannot be null", "Name");

                if (Value == null)
                    throw new ArgumentNullException("The field value cannot be null", "Value");

                this.Name = Name;
                this.Value = Value;
            }

            public bool Equals(Field other)
            {
                if (other == null)
                    return false;
                else
                    return Name.Equals(other.Name, StringComparison.CurrentCulture);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                else if (ReferenceEquals(this, obj))
                    return true;
                else
                    return Equals(obj as Field);
            }

            public override string ToString() => Content;
            public override int GetHashCode() => Name.GetHashCode();
        }
    }
}
