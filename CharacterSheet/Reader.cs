using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterSheet
{
    /// <summary>
    /// Reads a formatted XML file containing character data and outputs parameters to generate a control.
    /// </summary>
    public class Reader
    {
        private string fileLoc = @"Characters\";
        private string fileName;
        public XElement file { get; set; }
        private Character chara;

        // Types are used to determine the kind of control that is added for each entry.
        private string[] types = { "text", "num" };
        private List<string> templates;

        public enum NodeType { Root, Standard, StdElement, Template, TempElement, Custom, CustElement, None }
        public string Name
        {
            get { return stdElement(file).Element("Name").Value; }
            set { stdElement(file).Element("Name").Value = value; }
        }

        // Names of the nodes to be selected.
        const string STD = "Standard";
        const string TEMP = "Template";
        const string CUST = "Custom";

        public Reader(Character c)
        {
            chara = c;
            fileName = c.Name + ".xml";
            file = XElement.Load(fileLoc + fileName);

            //file = fixedFile(XElement.Load(fileLoc + fileName));
            makeTemplates();
        }

        /// <summary>
        /// This adds a small string to each XElement that uses a name that the reader needs to work correctly.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public XElement fixedFile(XElement file)
        {
            foreach (XElement x in file.Descendants())
            {
                if (file.Name == "Standard" || file.Name == "Template" || file.Name == "Custom")
                {
                    file.Name = file.Name + "(user)";
                }
            }
            return file;
        }
        public Reader() : this(new Character("character"))
        { }

        /// <summary>
        /// Creates list of strings that name all given templates.
        /// </summary>
        private void makeTemplates()
        {
            IEnumerable<XElement> temps =
                from ele in file.Elements("Template")
                select ele;

            IEnumerable<string> atts =
                from att in temps.Attributes()
                select att.Value;

            templates = atts.ToList();
        }

        public void AddCustom(XElement x)
        {
            custElement(file).Add(x);
            file.Save(fileLoc + fileName);
        }
        public void AddTemplate(XElement x)
        {
            string templateName = x.Name.ToString();
            x.SetAttributeValue("name", x.Name);
            x.Name = "Template";
            Console.WriteLine(x);
            foreach (XElement temp in templateElements(file))
            {
                // If template node has no attributes, it is a blank node to be replaced.
                if (!temp.HasAttributes)
                {
                    temp.AddBeforeSelf(x);
                    temp.Remove();
                    file.Save(fileLoc + fileName);
                    chara.templates.Add(templateName);
                    templates.Add(templateName);
                    return;
                }
                else if (temp.Attribute("name").Value == x.Attribute("name").Value)
                {
                    Console.WriteLine("Template already applied.");
                    return;
                }
                else if (temp.ElementsAfterSelf("Template").Count() < 1)
                {
                    temp.AddAfterSelf(x);
                    file.Save(fileLoc + fileName);
                    chara.templates.Add(templateName);
                    templates.Add(templateName);
                    return;
                }
            }
            Console.WriteLine("Shouldn't reach here!!!");
            return;
        }
        public void DeleteTemplate(string templateName)
        {
            foreach (XElement x in templateElements(file))
            {
                if (x.HasAttributes && templates.Contains(templateName) && x.Attribute("name").Value == templateName)
                {
                    // Replaces the template node with a blank one if it has no more templates.
                    if (templateElements(file).Count() == 1)
                    {
                        x.AddBeforeSelf(new XElement("Template"));
                        x.Remove();
                    }
                    else
                    {
                        x.Remove();
                    }

                    // Replace this.
                    templates.Remove(templateName);
                    chara.templates.Remove(templateName);
                    file.Save(fileLoc + fileName);
                    return;
                }
            }
            Console.WriteLine("Template not found.");
            return;
        }
        /// <summary>
        /// Creates a list for the containers to be passed to the Main Window.
        /// </summary>
        /// <returns></returns>
        public List<Control> makeList()
        {
            List<Control> list = new List<Control>();

            // This is where the file is initially parsed.
            // It goes through each of the indicated tags given by the const strings.
            foreach (XElement x in stdElement(file).Elements())
            {
                list.Add(makeControl(x));
            }

            // Uses the list of template names to select each node by their attribute.
            foreach (string temp in templates)
            {
                foreach (XElement x in templateElement(temp).Elements())
                {
                    list.Add(makeControl(x));
                }
            }

            // Custom fields for notes and homebrew stats are parsed here, if they have any.
            foreach (XElement x in custElement(file).Elements())
            {
                list.Add(makeControl(x));
            }
            return list;
        }

        /// <summary>
        /// Uses all the methods below to pull the data from the formatted XML file.
        /// </summary>
        /// <param name="x">XML node that is currently being parsed.</param>
        /// <returns></returns>
        private Control makeControl(XElement x)
        {
            return new Control(this, getName(x), getType(x), getValue(x), x, getNodeType(x));
        }

        public List<Control> pathToControlList(List<string> list)
        {
            List<Control> newList = new List<Control>();
            foreach (string path in list)
            {
                newList.Add(makeControl(XElement.Load(path)));
            }
            return newList;
        }

        public void DeleteCustomControl(XElement x)
        {
            if (getNodeType(x) == NodeType.CustElement)
            {
                x.Remove();
                file.Save(fileLoc + fileName);
            }
        }
        public List<Control> getCustList()
        {
            List<Control> list = new List<Control>();
            foreach (XElement x in custElement(file).Elements())
            {
                list.Add(makeControl(x));
            }
            return list;
        }
        public List<Control> GetTemplateNodes()
        {
            List<Control> list = new List<Control>();
            foreach (XElement x in templateElements(file))
            {
                // Gets all the Template Nodes, and makes a control for each, returns empty list if only had default Template.
                Control c = makeControl(x);
                if (x.HasAttributes)
                {
                    c.Name = x.FirstAttribute.Value;
                    list.Add(c);
                }
            }
            return list;
        }
        private string getName(XElement x)
        {
            return x.Name.ToString();
        }
        private NodeType getNodeType(XElement x)
        {
            NodeType parentType;
            if (x.Parent == null) { parentType = NodeType.None; }
            else { parentType = getNodeType(x.Parent); }

            if (parentType == NodeType.Standard) { return NodeType.StdElement; }
            else if (parentType == NodeType.Template) { return NodeType.TempElement; }
            else if (parentType == NodeType.Custom) { return NodeType.CustElement; }
            else if (parentType == NodeType.None && x.Name == "Character") { return NodeType.Root; }
            else if (x.Name == "Standard") { return NodeType.Standard; }
            else if (x.Name == "Template") { return NodeType.Template; }
            else if (x.Name == "Custom") { return NodeType.Custom; }
            else
            {
                Console.WriteLine("Not a main node.");
                return NodeType.None;
            }
        }

        private string getType(XElement x)
        {
            // Returns the attribute that defines the Control that is made for it on the sheet.
            // Needs to return Type for template nodes as well. Lists need to not all be Controls... fix this.
            if (x.HasAttributes)
            {
                XAttribute attr = x.FirstAttribute;
                // Checks if Control is a field in a Node. Otherwise it's a Template Node.
                if (attr.Name == "type" && types.Contains(attr.Value))
                {
                    return attr.Value;
                }
                else if (attr.Name == "name" && templates.Contains(attr.Value))
                {
                    return "template";
                }
                else return "none";
            }
            return "none";
        }
        private string getValue(XElement ele)
        {
            return ele.Value;
        }

        /// <summary>
        /// Return XElement containing Character's standard fields.
        /// </summary>
        /// <param name="root">The file location.</param>
        /// <returns></returns>
        private XElement stdElement(XElement root)
        {
            return root.Element(STD);
        }
        /// <summary>
        /// Get collection of XElements used for templates
        /// </summary>
        /// <param name="root">The file location.</param>
        private IEnumerable<XElement> templateElements(XElement root)
        {
            return root.Elements(TEMP);
        }
        /// <summary>
        /// Return XElement in the template nodes that has the attribute given by value.
        /// </summary>
        /// <param name="value">The attribute value to match.</param>
        /// <returns></returns>
        private XElement templateElement(string value)
        {
            foreach (XAttribute att in templateElements(file).Attributes())
            {
                if (att.Value == value) return att.Parent;
            }
            Console.Write("Template " + value + " not found on character.");
            return null;
        }

        private XElement templateElement(XElement root)
        {
            foreach (XElement x in templateElements(file))
            {
                if (root.Equals(x)) return x;
            }

            Console.WriteLine("Item not found");
            return null;
        }

        /// <summary>
        /// Return XElement containing custom fields.
        /// </summary>
        /// <param name="root">The file location.</param>
        private XElement custElement(XElement root)
        {
            return root.Element(CUST);
        }
        private XElement custElement(string name)
        {
            foreach (XElement x in custElement(file).Elements())
            {
                if (x.Name == name) return x;
            }

            Console.WriteLine("Could not find " + name + "in the custom node.");
            return null;
        }
    }
}
