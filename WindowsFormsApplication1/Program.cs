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
            document.Save("statements.xml");
        }
    }
}
