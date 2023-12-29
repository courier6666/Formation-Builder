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
using CourseworkWPF.MilitaryFolder;

namespace CourseworkWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MilitaryApp.Instance.TemplatesDisplayPanel = this.TemplatesDisplayGrid;
            MilitaryApp.Instance.DisplayHeaderPanel = this.SelectedFormationDisplayGrid;
            MilitaryApp.Instance.DisplayMainPanel = this.FormationsDisplayGrid;
            MilitaryApp.Instance.SavedTemplatesPanel = this.SavedFormationGrid;
            MilitaryApp.Instance.SearchBarPanel = this.templatesSearchGrid;
            MilitaryApp.Instance.MainWindowPointer = this;
            
            MilitaryApp.Instance.LoadData();
            MilitaryApp.Instance.DisplayAllFormations();
        }

        private void formationsTabButton_Click(object sender, RoutedEventArgs e)
        {
            MilitaryApp.Instance.DisplayAllFormations();
        }

        private void PersonnelTabButon_Click(object sender, RoutedEventArgs e)
        {
            MilitaryApp.Instance.DisplayAllPersonnelRoles();
        }

        private void equipmentTabButton_Click(object sender, RoutedEventArgs e)
        {
            MilitaryApp.Instance.DisplayAllEquipment();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MilitaryApp.Instance.DisplayAllRanks();
        }

        private void changeViewButton_Click(object sender, RoutedEventArgs e)
        {
            MilitaryApp.Instance.ChangeView();
        }

        private void saveAllDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (MilitaryApp.Instance.SaveData())
                MessageBox.Show("Data has been saved!");

        }

        private void loadDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (MilitaryApp.Instance.LoadData())
                MessageBox.Show("Data has been loaded!");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you like to save changes?", "Saving changes.", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

            switch(result)
            {
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
                case MessageBoxResult.Yes:
                    MilitaryApp.Instance.SaveData();
                    break;
                case MessageBoxResult.No:
                    
                    break;
            }
        }
    }
}
