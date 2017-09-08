using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterSheet
{
    public class Templates
    {
        private string directoryPath;
        public string DirectoryPath
        {
            get { return directoryPath; }
        }
        public Templates()
        {
            directoryPath = @"CharacterFormats\";
        }
        public List<string> getTemplatePaths()
        {
            List<string> list = new List<string>();
            foreach (string d in Directory.GetFiles(directoryPath))
            {
                Console.WriteLine(d);
                if (d != directoryPath + "charformat.xml") list.Add(d);
            }
            return list;
        }
    }
}
