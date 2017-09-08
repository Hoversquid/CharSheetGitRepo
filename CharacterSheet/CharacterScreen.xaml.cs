using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CharacterSheet
{
    /// <summary>
    /// Interaction logic for CharacterScreen.xaml
    /// </summary>
    public partial class CharacterScreen : Window
    {
        private Character chara;
        private List<Control> ctrlList;
        private Reader reader;
        private Templates templates;
        private string charLoc = @"Characters\";
        private string charName = "Tim";

        public Reader Reader
        {
            get { return reader; }
        }
        public CharacterScreen()
        {
            InitializeComponent();
            AddCharacter();
            AddControls();
        }
        public void AddCharacter()
        {
            chara = new Character(charLoc, charName);
            //chara = new Character("Tim");
            templates = new Templates();
            reader = chara.CharReader;
            //chara.addTemplate("DND35");
            //chara.addTemplate("Homebrew");
            //chara.MakeFile("Greg(5)", 0);
            ctrlList = reader.makeList();
        }
        /// <summary>
        /// Uses an XML reader to add controls from a formatted XML file.
        /// </summary>
        public void AddControls()
        {
            foreach (Control control in ctrlList)
            {
                CharacterInput charInput = new CharacterInput(control);
                Stack.Children.Add(charInput);
            }
        }

        public void ResetControls()
        {
            Stack.Children.Clear();
            ctrlList = reader.makeList();
            AddControls();
        }

        public void ReadInputsToFile()
        {
            foreach (UIElement ui in Stack.Children)
            {
                CharacterInput input = ui as CharacterInput;
                input.item.Node.Value = input.textBox.Text;
            }
        }

        private void Add_Custom_Click(object sender, RoutedEventArgs e)
        {
            ControlWindow window = new ControlWindow(this);
            window.Show();
        }
        private void Add_Template_Click(object sender, RoutedEventArgs e)
        {
            Selection selection = new Selection(this, Selection.SelectType.AddTemplate);
            selection.makeTempPathList(reader, templates);
            selection.Title = "Add Templates";
            selection.Show();
        }
        private void Del_Custom_Click(object sender, RoutedEventArgs e)
        {
            Selection selection = new Selection(this, Selection.SelectType.DeleteCustom);
            selection.makeCustList(reader);
            selection.Title = "Custom Elements: " + chara.Name;
            selection.Show();
        }
        private void Del_Template_Click(object sender, RoutedEventArgs e)
        {
            Selection selection = new Selection(this, Selection.SelectType.DeleteTemplate);
            selection.makeTempList(reader);
            selection.Title = "Templates: " + chara.Name;
            selection.Show();
        }
        private void Save_Character_Click(object sender, RoutedEventArgs e)
        {
            ReadInputsToFile();
            chara.Save();
        }
        private void Save_New_Character_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
