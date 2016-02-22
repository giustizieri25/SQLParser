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
