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
using Microsoft.Win32;
using CourseworkWPF.MilitaryFolder;

namespace CourseworkWPF
{
    /// <summary>
    /// Логика взаимодействия для AddEquipmentToEquipmentListWindow.xaml
    /// </summary>
    public partial class AddEquipmentToEquipmentListWindow : Window
    {
        Equipment selectedEquipment;
        Grid clickedGrid;
        List<Equipment> equipmentList;
        MainWindow mainWindowPointer;
        public AddEquipmentToEquipmentListWindow(List<Equipment> equipmentListToAdd, MainWindow mainWindow, IDisplayerStrategy displayerStrategy)
        {
            InitializeComponent();
            selectedEquipment = null;
            this.equipmentList = equipmentListToAdd;
            this.mainWindowPointer = mainWindow;
            StackPanel stackPanel = new StackPanel();
            foreach(var equipment in MilitaryApp.Instance.GetAllEquipment())
            {
                Grid equipmentGrid = displayerStrategy.CreateDisplayPanelEquipment(equipment.Value);
                equipmentGrid.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => {

                    if(this.clickedGrid!=null)
                    this.clickedGrid.Background = Brushes.Aquamarine;

                    this.clickedGrid = equipmentGrid;
                    this.clickedGrid.Background = new SolidColorBrush(Color.FromRgb(15, 125, 119));
                    selectedEquipment = equipment.Value; 
                });
                stackPanel.Children.Add(equipmentGrid);
            }
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            this.DisplayEquipmentPanel.Children.Add(scrollViewer);
        }

        private void cancelEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.clickedGrid != null)
                this.clickedGrid.Background = Brushes.Aquamarine;
            selectedEquipment = null;
        }

        private void addEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.selectedEquipment==null)
            {
                MessageBox.Show("No equipment selected!");
                return;
            }
            this.equipmentList.Add(this.selectedEquipment);
            MessageBox.Show("Equipment Added!");
        }
    }
}
