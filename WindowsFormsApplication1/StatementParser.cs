using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SqlParser
{
    class StatementParser
    {
        string separator = "::=";

        public string Name { get; set; }
        public string Body { get; set; }

        public StatementParser(string statement)
        {
            int indexOf = statement.IndexOf(separator);
            int lastIndexOf = statement.LastIndexOf(separator);

            if (indexOf > -1 && lastIndexOf > -1 && indexOf == lastIndexOf)
            {
                this.Name = statement.Substring(0, indexOf).Trim();
                this.Body = statement.Substring(indexOf + separator.Length).Trim();
            }
            else
            {
                throw new ArgumentException("Invalid statement", statement);
            }
        }

        public XmlDocument Parse()
        {
            XmlDocument statementDocument = new XmlDocument();
            StatementDeclarationElement statement = new StatementDeclarationElement(this.Name.Trim(), statementDocument);
            statementDocument.AppendChild(statement);

            StatementElement currentToken = statement;
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < this.Body.Length; i++)
            {
                char currentChar = this.Body[i];

                switch (currentChar)
                {
                    case '[':
                        if (stringBuilder.Length > 0)
                        {
                            currentToken.SetAttribute("Text", stringBuilder.ToString().Trim());
                            stringBuilder.Clear();
                        }
                        OptionalElement o = new OptionalElement(statementDocument);
                        currentToken.AppendChild(o);
                        currentToken = o;
                        break;

                    case ']':
                        if (stringBuilder.Length > 0)
                        {
                            if (currentToken.HasAttribute("Text"))
                            {
                                stringBuilder.Insert(0, currentToken.Attributes["Text"].Value);
                            }
                            currentToken.SetAttribute("Text", stringBuilder.ToString().Trim());
                            stringBuilder.Clear();
                        }
                        while (!(currentToken is OptionalElement))
                        {
                            currentToken = (StatementElement)currentToken.ParentNode;
                        }
                        currentToken = (StatementElement)currentToken.ParentNode;
                        break;

                    case '{':
                        if (stringBuilder.Length > 0)
                        {
                            currentToken.SetAttribute("Text", stringBuilder.ToString().Trim());
                            stringBuilder.Clear();
                        }
                        RequiredElement r = new RequiredElement(statementDocument);
                        currentToken.AppendChild(r);
                        currentToken = r;
                        break;

                    case '}':
                        if (stringBuilder.Length > 0)
                        {
                            currentToken.SetAttribute("Text", stringBuilder.ToString().Trim());
                            stringBuilder.Clear();
                        }
                        while (!(currentToken is RequiredElement))
                        {
                            currentToken = (StatementElement)currentToken.ParentNode;
                        }
                        currentToken = (StatementElement)currentToken.ParentNode;
                        break;

                    case '<':
                        if (stringBuilder.Length > 0)
                        {
                            currentToken.SetAttribute("Text", stringBuilder.ToString().Trim());
                            stringBuilder.Clear();
                        }
                        LabelElement l = new LabelElement(statementDocument);
                        currentToken.AppendChild(l);
                        currentToken = l;
                        break;

                    case '>':
                        if (stringBuilder.Length > 0)
                        {
                            currentToken.SetAttribute("Text", stringBuilder.ToString().Trim());
                            stringBuilder.Clear();
                        }
                        currentToken = (StatementElement)currentToken.ParentNode;
                        break;

                    case ' ':
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(currentChar);
                        }
                        break;

                    case '(':
                        break;

                    case ')':
                        currentToken.SetAttribute("EnclosedByBrackets", Boolean.TrueString);
                        break;

                    case '|':
                        if (stringBuilder.Length > 0)
                        {
                            currentToken.SetAttribute("Text", stringBuilder.ToString().Trim());
                            stringBuilder.Clear();
                        }

                        SeparatorElement separatorElement = new SeparatorElement(statementDocument);
                        currentToken.AppendChild(separatorElement);
                        currentToken = separatorElement;
                        break;

                    default:
                        StatementElement se = null;
                        bool keywordStarted = currentToken is KeywordElement;
                        bool labelStarted = currentToken is LabelElement;
                        bool userSuplpiedParameterStarted = currentToken is UserSuppliedParameterElement;
                        if (!keywordStarted && !labelStarted && !userSuplpiedParameterStarted)
                        {
                            if (Char.IsLower(currentChar))
                            {
                                se = new UserSuppliedParameterElement(statementDocument);
                            }
                            else
                            {
                                se = new KeywordElement(statementDocument);
                            }
                            currentToken.AppendChild(se);
                            currentToken = se;
                        }
                        stringBuilder.Append(currentChar);
                        break;
                }
            }
            return statementDocument;
        }
    }
}