using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SqlParser
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            XmlDocument document = new XmlDocument();
            XmlElement rootElement = document.CreateElement("Statements");
            document.AppendChild(rootElement);

            string[] lines = File.ReadAllLines("Statements.txt");

            foreach (string line in lines)
            {
                XmlDocument xd = new StatementParser(line).Parse();
                XmlNode newChild = document.ImportNode(xd.FirstChild, true);
                rootElement.AppendChild(newChild);
            }

            traverseNodes(rootElement);
            document.Save("statements.xml");
        }

        private static void traverseNodes(XmlElement element)
        {
            if (element.ChildNodes.Count > 0)
            {
                List<XmlNode> childrenNodes = new List<XmlNode>();
                foreach (XmlNode childNode in element.ChildNodes)
                {
                    childrenNodes.Add(childNode);
                }
                foreach (XmlNode childNode in childrenNodes)
                {
                    if (childNode.Name.Equals("Optional"))
                    {
                        List<XmlNode> grandChildrenNodes = new List<XmlNode>();
                        foreach (XmlNode grandChildNode in childNode.ChildNodes)
                        {
                            grandChildrenNodes.Add(grandChildNode);
                        }
                        foreach (XmlNode grandChildNode in grandChildrenNodes)
                        {
                            (grandChildNode as XmlElement).SetAttribute("Optional", Boolean.TrueString);
                            element.InsertBefore(grandChildNode, childNode);
                        }
                        element.RemoveChild(childNode);
                    }
                }
                foreach (XmlNode childNode in element.ChildNodes)
                {
                    traverseNodes((XmlElement)childNode);
                }
            }
        }
    }
}
