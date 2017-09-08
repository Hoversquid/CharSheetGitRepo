using System;
using System.Collections.Generic;
using System.Xml.Linq;
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
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
        private Control control;
        private CharacterScreen screen;

        public Control Control
        {
            get { return control; }
        }
        public ControlWindow(CharacterScreen cs)
        {
            InitializeComponent();
            screen = cs;
            XElement newNode = new XElement("Field_Name", "Value");
            control = new Control(cs.Reader, "Name", "text", "Value", newNode, Reader.NodeType.CustElement);
        }

        private void typeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string type = typeBox.SelectedValue.ToString();
            switch (type)
            {
                case "Text":
                    control.Type = "text"; break;
                case "Number":
                    control.Type = "num"; break;
                default:
                    break;
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            XElement newEle = new XElement(name.Text, value.Text);
            XAttribute newAttrib = new XAttribute("type", Control.Type);
            control.Reader.AddCustom(newEle);
            screen.ResetControls();
            this.Close();
        }
    }
}
