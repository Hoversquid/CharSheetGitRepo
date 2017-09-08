using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CharacterSheet
{
    public class Character
    {
        private string fileDir = @"Characters\";
        private string charaPath;
        private string formatDir = @"CharacterFormats\";
        private string formatFile = "charformat.xml";
        private XElement root;

        public string FormatFilePath { get { return formatDir + formatFile; } }
        public string Name;
        public List<string> templates;
        public Reader CharReader;

        public Character() : this("Character") { }

        public Character(string name)
        {
            // Needs to just make name.
            Name = name;
            MakeFile();
            charaPath = fileDir + Name;

            // Format file creates root with given Name.
            loadFormatFile();
            Save(charaPath);
            CharReader = new Reader(this);
            templates = new List<string>();
        }

        public Character(string fileLoc, string fileName)
        {
            if (!File.Exists(fileLoc + fileName + ".xml"))
            {
                Console.WriteLine(fileName + ".xml doesn't exist.");
            }
            else
            {
                // Need better way of managing the root file... fix this.
                Name = fileName;
                CharReader = new Reader(this);
                root = CharReader.file;
                charaPath = fileLoc + fileName + ".xml";
                templates = getTemplates();
            }
        }


        private void loadFormatFile()
        {
            if (File.Exists(formatDir + formatFile))
            {
                root = XElement.Load(formatDir + formatFile);
                root.Element("Standard").Element("Name").Value = Name;
                return;
            }

            else Console.WriteLine("Invalid format file location");
            root = new XElement("New");
        }

        /// <summary>
        /// Creates file with character's name. Adds an index if file already exists with the same name.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="index"></param>
        public void MakeFile(string n, int index)
        {
            string[] list = Directory.GetFiles(fileDir);
            string fileN = fileDir + n;

            // If no files are in directory, save.
            if (list.Length < 1)
            {
                Name = n;
                return;
            }
            
            // This loop checks if the file with the same name exists. If it does, it saves it with an index in the name. If it doesn't, it saves normally.
            for (int i = 0; i < list.Length; i++)
            {
                if (index == 0)
                {
                    if (list[i] == (fileN + ".xml"))
                    {
                        index++;
                        MakeFile(n, index);
                        return;
                    }
                    else if
                        (list[i] != (fileN + ".xml")
                        && i == list.Length - 1)
                    {
                        return;
                    }
                }
                else
                {
                    if (list[i] == (fileN + "(" + index + ")" + ".xml"))
                    {
                        index++;
                        MakeFile(n, index);
                        return;
                    }
                    else if
                        (list[i] != (fileN + "(" + index + ")" + ".xml")
                        && i == list.Length - 1)
                    {
                        Name = Name + "(" + index + ")";
                        return;
                    }
                }
            }
            Console.WriteLine("Directories not found.");
        }

        public void MakeFile()
        {
            MakeFile(Name, 0);
        }

        private void Save(string fileN)
        {
            CharReader.file.Save(fileN + ".xml");
            Console.WriteLine(fileN + " saved.");
        }
        
        public void Save()
        {
            CharReader.file.Save(charaPath);
            Console.WriteLine(charaPath + " saved.");
        }

        /// <summary>
        /// Gets a list of attributes (Template names) on the nodes named "Template".
        /// </summary>
        /// <returns></returns>
        public List<string> getTemplates()
        {
            IEnumerable<XElement> temps =
                from ele in root.Elements("Template")
                select ele;

            IEnumerable<string> atts =
                from att in temps.Attributes()
                select att.Value;

            return atts.ToList();
        }

        /// <summary>
        /// Adds a Template node to the XML file or replaces the empty Template node if there are none.
        /// </summary>
        /// <param name="t">Name of the template.</param>
        public void addTemplate(string t)
        {
            XElement template;

            // Gets the file location of template's structure.
            if (File.Exists(formatDir + t + ".xml"))
            {
                template = XElement.Load(formatDir + t + ".xml");
            }
            else
            {
                Console.WriteLine("No template file " + t + ".xml.");
                return;
            }

            // Replace <Template> element if none are on list, append another one otherwise.
            if (templates.Count < 1)
            {
                templates.Add(t);
                root.Element("Template").Add(template.Elements());
                root.Element("Template").SetAttributeValue("name", t);
                root.Save(fileDir + Name + ".xml");
                Console.WriteLine(t + " template applied to " + fileDir + Name + ".xml");
                return;
            }
            else if (templates.Contains(t))
            {
                Console.WriteLine(t + " template already applied.");
                return;
            }
            else
            {
                // Find last Template node, add new node with copied info under a <Template> tag with attributes.
                foreach (XElement x in root.Elements("Template"))
                {
                    if (x.ElementsAfterSelf("Template").Count() < 1)
                    {
                        template.Name = "Template";
                        template.SetAttributeValue("name", t);
                        x.AddAfterSelf(template);
                        templates.Add(t);
                        root.Save(fileDir + Name + ".xml");
                        Console.WriteLine(t + " template applied.");
                        return;
                    }
                }
                Console.WriteLine("What happen");
            }
        }


    }
}
