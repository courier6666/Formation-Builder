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
    /// Логика взаимодействия для AddPersonnelWindow.xaml
    /// </summary>
    public partial class AddPersonnelWindow : Window
    {

        Personnel selectedPersonnel;
        MainWindow mainWindowPointer;
        Grid clickedGrid;

        public AddPersonnelWindow(List<Personnel> personnelsListToAdd, MainWindow mainWindow, IDisplayerStrategy displayerStrategy)
        {
            InitializeComponent();
            mainWindowPointer = mainWindow;

            StackPanel stackPanel = new StackPanel();

            foreach(var personnelRole in MilitaryApp.Instance.GetAllPersonnel())
            {
                Grid personnelGrid = displayerStrategy.CreateDisplayPanelPersonnel(personnelRole);
                personnelGrid.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) =>
                {
                    if (this.clickedGrid != null)
                        this.clickedGrid.Background = Brushes.Aquamarine;

                    this.clickedGrid = personnelGrid;
                    this.clickedGrid.Background = new SolidColorBrush(Color.FromRgb(15, 125, 119));
                    this.selectedPersonnel = personnelRole;
                });


                stackPanel.Children.Add(personnelGrid);
            }
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            this.addPersonnelButton.Click += new RoutedEventHandler((sender, r) => {
                if (this.selectedPersonnel == null)
                {
                    MessageBox.Show("No personnel selected!");
                    return;
                }
                personnelsListToAdd.Add(selectedPersonnel);
                MessageBox.Show("Personnel added!");
            });

            this.DisplayPersonnelPanel.Children.Add(scrollViewer);
        }
        public AddPersonnelWindow(LowestUnit lowestUnit, MainWindow mainWindow, IDisplayerStrategy displayerStrategy)
        {
            InitializeComponent();
            mainWindowPointer = mainWindow;

            StackPanel stackPanel = new StackPanel();

            foreach (var personnelRole in MilitaryApp.Instance.GetAllPersonnel())
            {
                Grid personnelGrid = displayerStrategy.CreateDisplayPanelPersonnel(personnelRole);
                personnelGrid.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) =>
                {
                    if (this.clickedGrid != null)
                        this.clickedGrid.Background = Brushes.Aquamarine;

                    this.clickedGrid = personnelGrid;
                    this.clickedGrid.Background = new SolidColorBrush(Color.FromRgb(15, 125, 119));
                    this.selectedPersonnel = personnelRole;
                });


                stackPanel.Children.Add(personnelGrid);
            }
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            this.addPersonnelButton.Click += new RoutedEventHandler((sender, e) => {
                if (this.selectedPersonnel == null)
                {
                    MessageBox.Show("No personnel selected!");
                    return;
                }

                if(lowestUnit.CommanderNeededRank.RankValue>this.selectedPersonnel.PersonnelRank.RankValue)
                {
                    MessageBox.Show("Rank of selected personnel is too low!");
                    return;
                }

                lowestUnit.Commander = this.selectedPersonnel;
                MessageBox.Show("Commander added!");
            });

            this.DisplayPersonnelPanel.Children.Add(scrollViewer);
        }

        private void cancelPersonnelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.clickedGrid != null)
                this.clickedGrid.Background = Brushes.Aquamarine;
            this.selectedPersonnel = null;
        }
    }
}
