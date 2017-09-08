using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterSheet
{
    /// <summary>
    /// Container to pass information to generate the needed controls on the Character Window.
    /// </summary>
    public class Control
    {
        public string Name { get; set; }
        public string Type;
        public string Value;
        public XElement Node;

        private string controlID;
        private Reader reader;
        public Reader.NodeType NodeType;
        public Control(Reader r, string n, string t, string v, XElement node, Reader.NodeType nodety )
        {
            reader = r;
            Name = n;
            Type = t;
            Value = v;
            Node = node;
            NodeType = nodety;
        }

        public Reader Reader
        {
            get { return reader; }
        }

        public string ControlID
        {
            get { return controlID; }
            set { controlID = value; }
        }
        public void ReadControl()
        {
            Console.WriteLine(Name + ": " + Type + " " + Value);
        }
    }
}
