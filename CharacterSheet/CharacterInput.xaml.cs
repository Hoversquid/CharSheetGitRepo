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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CharacterSheet
{
    /// <summary>
    /// Interaction logic for CharacterInput.xaml
    /// </summary>
    public partial class CharacterInput : UserControl
    {
        public string id;
        public Control item;
        public CharacterInput()
        {
            InitializeComponent();
        }

        public CharacterInput(Control ctrl) : this()
        {
            item = ctrl;
            label.Content = ctrl.Name;
            textBox.Text = ctrl.Value;
            id = this.Uid;
        }

        private void textBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            item.Node.Value = textBox.Text;
        }
    }
}
