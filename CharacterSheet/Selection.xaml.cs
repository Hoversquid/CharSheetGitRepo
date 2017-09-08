using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Selection.xaml
    /// </summary>
    public partial class Selection : Window
    {
        public CharacterScreen parent;
        public enum SelectType { AddTemplate, DeleteTemplate, DeleteCustom };
        public SelectType Action;
        public Selection(CharacterScreen screen, SelectType action)
        {
            InitializeComponent();
            parent = screen;
            Action = action;
        }

        public void makeFullList(List<Control> controls)
        {
            DataContext = new ControlData(controls);
        }

        public void makeCustList(Reader r)
        {
            DataContext = new ControlData(r.getCustList());
        }

        public void makeTempList(Reader r)
        {
            DataContext = new ControlData(r.GetTemplateNodes());
        }

        public void makeTempPathList(Reader r, Templates temps)
        {
            DataContext = new ControlData(r.pathToControlList(temps.getTemplatePaths()));
        }

        private void selectionBut_Click(object sender, RoutedEventArgs e)
        {
            Control c = (Control)listBox.SelectedItem;

            // Will add templates to Reader or delete custom controls from it. Needs to be specific... fix later.
            if (c != null)
            {
                if (Action == SelectType.AddTemplate)
                    c.Reader.AddTemplate(c.Node);
                else if (Action == SelectType.DeleteTemplate)
                    c.Reader.DeleteTemplate(c.Name);
                else if (Action == SelectType.DeleteCustom)
                    c.Reader.DeleteCustomControl(c.Node);

                else
                    Console.WriteLine("Selection Action not set.");

                parent.ResetControls();
                this.Close();
            }
        }
    }
    public class ControlData
    {
        public ObservableCollection<Control> Controls { get; private set; }
        public ControlData(List<Control> ctrlList) 
        {
            Controls = new ObservableCollection<Control>();
            foreach (Control c in ctrlList)
            {
                Console.WriteLine(c.Name);
                Controls.Add(c);
            }
        }

        public ControlData()
        {
            Controls = new ObservableCollection<Control>();
        }
    }
}