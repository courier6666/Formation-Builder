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
    /// Логика взаимодействия для AddFormationWindow.xaml
    /// </summary>
    public partial class AddFormationWindow : Window
    {
        IFormation selectedFormation;
        Grid clickedGrid;
        MainWindow mainWindowPointer;
        public AddFormationWindow(List<IFormation> formationsListToAdd, MainWindow mainWindow, IDisplayerStrategy displayerStrategy)
        {
            InitializeComponent();
            selectedFormation = null;
            StackPanel stackPanel = new StackPanel();
            foreach(var formation in MilitaryApp.Instance.GetAllFormationTemplates())
            {
                Grid formationGrid;
                if (formation is FormationDecorator)
                    formationGrid = displayerStrategy.CreateDisplayPanelFormation(formation as FormationDecorator);
                else
                    formationGrid = displayerStrategy.CreateDisplayPanelFormation(formation);
                formationGrid.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => {
                    if (this.clickedGrid != null)
                        this.clickedGrid.Background = Brushes.Aquamarine;


                    this.clickedGrid = formationGrid;
                    this.clickedGrid.Background = new SolidColorBrush(Color.FromRgb(15, 125, 119));

                    this.selectedFormation = formation;
                });
                stackPanel.Children.Add(formationGrid);
            }
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            this.addFormationButton.Click += new RoutedEventHandler((sender, e) => {
                if (this.selectedFormation == null)
                {
                    MessageBox.Show("No formation selected!");
                    return;
                }
                formationsListToAdd.Add(this.selectedFormation);
                MessageBox.Show("Formation Added!");
            } );

            this.DisplayFormationsPanel.Children.Add(scrollViewer);
        }
        
        public AddFormationWindow(Formation formationToSetHQ, MainWindow mainWindow, IDisplayerStrategy displayerStrategy)
        {
            InitializeComponent();
            selectedFormation = null;
            StackPanel stackPanel = new StackPanel();
            foreach (var formation in MilitaryApp.Instance.GetAllFormationTemplates())
            {
                Grid formationGrid;
                if (formation is FormationDecorator)
                    formationGrid = displayerStrategy.CreateDisplayPanelFormation(formation as FormationDecorator);
                else
                    formationGrid = displayerStrategy.CreateDisplayPanelFormation(formation);
                formationGrid.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => {
                    if (this.clickedGrid != null)
                        this.clickedGrid.Background = Brushes.Aquamarine;


                    this.clickedGrid = formationGrid;
                    this.clickedGrid.Background = new SolidColorBrush(Color.FromRgb(15, 125, 119));

                    this.selectedFormation = formation;
                });
                stackPanel.Children.Add(formationGrid);
            }
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            this.addFormationButton.Click += new RoutedEventHandler((sender, e)=> {

                if (this.selectedFormation == null)
                {
                    MessageBox.Show("No formation selected!");
                    return;
                }
                if(this.selectedFormation.CommanderNeededRank.RankValue < formationToSetHQ.CommanderNeededRank.RankValue)
                {
                    MessageBox.Show("Rank of selected formation commander is too low!");
                    return;
                }

                formationToSetHQ.HeadquartersFormation = this.selectedFormation;

                MessageBox.Show("Headquarters of formations is set!");
            });

            this.DisplayFormationsPanel.Children.Add(scrollViewer);
        }


        private void cancelFormationButton_Click(object sender, RoutedEventArgs e)
        {
            this.selectedFormation = null;

            if (this.clickedGrid != null)
                this.clickedGrid.Background = Brushes.Aquamarine;
        }
    }
}
