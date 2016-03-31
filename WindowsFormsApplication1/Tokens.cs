using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SqlParser
{
    class StatementElement : XmlElement
    {
        public StatementElement(XmlDocument xmlDocument) : this(string.Empty, xmlDocument)
        {

        }

        public StatementElement(string text, XmlDocument xmlDocument) : base(xmlDocument.Prefix, text, xmlDocument.NamespaceURI, xmlDocument)
        {

        }

    }

    class StatementDeclarationElement : StatementElement
    {
        public StatementDeclarationElement(string statement, XmlDocument xmlDocument) : base("Statement", xmlDocument)
        {
            this.SetAttribute("Text", statement);
        }
    }

    class OptionalElement : StatementElement
    {
        public OptionalElement(XmlDocument xmlDocument) : base("Optional", xmlDocument)
        {

        }
    }

    class KeywordElement : StatementElement
    {
        public KeywordElement(XmlDocument xmlDocument) : base("Keyword", xmlDocument)
        {

        }
    }

    class LabelElement : StatementElement
    {
        public LabelElement(XmlDocument xmlDocument) : base("Label", xmlDocument)
        {

        }
    }

    class RequiredElement : StatementElement
    {
        public RequiredElement(XmlDocument xmlDocument) : base("Required", xmlDocument)
        {
        }
    }

    class UserSuppliedParameterElement : StatementElement
    {
        public UserSuppliedParameterElement(XmlDocument xmlDocument) : base("UserSuppliedParameter", xmlDocument)
        {

        }
    }

    class SeparatorElement : StatementElement
    {
        public SeparatorElement(XmlDocument xmlDocument) : base("Separator", xmlDocument)
        {

        }
    }

}
