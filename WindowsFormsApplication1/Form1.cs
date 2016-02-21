using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string separator = "::=";
            XmlDocument document = new XmlDocument();
            XmlElement rootElement = document.CreateElement("Statements");
            document.AppendChild(rootElement);

            string[] lines = File.ReadAllLines("Statements.txt");

            foreach (string line in lines)
            {
                int indexOf = line.IndexOf(separator);
                int lastIndexOf = line.LastIndexOf(separator);

                if (indexOf > -1 && indexOf == lastIndexOf)
                {
                    string leftPart = line.Substring(0, indexOf).Trim();
                    string rightPart = line.Substring(indexOf + separator.Length).Trim();

                    StatementDeclarationElement statement = new StatementDeclarationElement(leftPart.Trim(), document);
                    StatementElement currentToken = statement;
                    StringBuilder stringBuilder = new StringBuilder();

                    for (int i = 0; i < rightPart.Length; i++)
                    {
                        char currentChar = rightPart[i];

                        switch (currentChar)
                        {
                            case '[':
                                if (stringBuilder.Length > 0)
                                {
                                    currentToken.SetAttribute("Text", stringBuilder.ToString().Trim());
                                    stringBuilder.Clear();
                                }
                                OptionalElement o = new OptionalElement(document);
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
                                RequiredElement r = new RequiredElement(document);
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
                                LabelElement l = new LabelElement(document);
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

                                SeparatorElement separatorElement = new SeparatorElement(document);
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
                                        se = new UserSuppliedParameterElement(document);
                                    }
                                    else
                                    {
                                        se = new KeywordElement(document);
                                    }
                                    currentToken.AppendChild(se);
                                    currentToken = se;
                                }
                                stringBuilder.Append(currentChar);
                                break;
                        }
                    }
                    rootElement.AppendChild(statement);
                }
            }
            document.Save("statements.xml");
        }
    }
}
