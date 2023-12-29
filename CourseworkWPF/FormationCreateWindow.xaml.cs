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
    /// Логика взаимодействия для FormationCreateWindow.xaml
    /// </summary>
    public partial class FormationCreateWindow : Window
    {
        public MainWindow mainWindowPointer;
        public Image downloadedImage;
        public Image imageToEdit;
        public Director builderDirector;
        public IFormation formationToEdit = null;
        public class Director
        {
            FormationCreateWindow formationCreateWindowPointer;
            public void BuildFormation(IBuilderFormation builderFormation)
            {

                if (this.formationCreateWindowPointer.downloadedImage != null)
                {
                    if (this.formationCreateWindowPointer.imageToEdit!=null)
                    {
                        string imagePathToEdit = formationCreateWindowPointer.imageToEdit.Source.ToString().Remove(0, 8);
                        string imagePathDownloaded = formationCreateWindowPointer.downloadedImage.Source.ToString().Remove(0, 8);

                        builderFormation.SetImage(imagePathToEdit, imagePathDownloaded);
                    }
                    else
                    {
                        string imagePathDownloaded = formationCreateWindowPointer.downloadedImage.Source.ToString().Remove(0, 8);
                        builderFormation.SetImage(imagePathDownloaded);
                    }
                }



                builderFormation.SetFormationName(this.formationCreateWindowPointer.formationNameTextBox.Text.Trim());
                builderFormation.SetHierarchyName(this.formationCreateWindowPointer.hierarchyNameTextBox.Text.Trim());
                builderFormation.SetCommanderRank(MilitaryApp.Instance.GetRank(this.formationCreateWindowPointer.commanderRankTextBox.Text.Trim()));

                string[] heavyEquipmentStringList = this.formationCreateWindowPointer.heavyEquipmentAsignedTextBox.Text.Split(',');
                List<Equipment> heavyEquipment = new List<Equipment>();

                for (int i = 0; i < heavyEquipmentStringList.Length; ++i)
                    heavyEquipmentStringList[i] = heavyEquipmentStringList[i].Trim();

                for (int i = 0; i < heavyEquipmentStringList.Length; ++i)
                {
                    heavyEquipment.Add(MilitaryApp.Instance.GetEquipment(heavyEquipmentStringList[i]));
                }

                string[] additionalEquipmentStringList = this.formationCreateWindowPointer.AdditionalEquipmentAssignedTextBox.Text.Split(',');
                List<Equipment> additionalEquipment = new List<Equipment>();

                for (int i = 0; i < additionalEquipmentStringList.Length; ++i)
                    additionalEquipmentStringList[i] = additionalEquipmentStringList[i].Trim();

                for (int i = 0; i < additionalEquipmentStringList.Length; ++i)
                {
                    additionalEquipment.Add(MilitaryApp.Instance.GetEquipment(additionalEquipmentStringList[i]));
                }

            }
            public Director(FormationCreateWindow formationCreateWindow)
            {
                this.formationCreateWindowPointer = formationCreateWindow;
            }
        }
        public FormationCreateWindow(MainWindow mainWindow, IFormation formation = null)
        {
            InitializeComponent();
            builderDirector = new Director(this);
            this.mainWindowPointer = mainWindow;
            this.formationToEdit = formation;

            if(this.formationToEdit != null)
            {
                IFormation original = this.formationToEdit;
                while(original is FormationDecorator)
                {
                    if(original is FormationImageDecorator)
                    {
                        this.downloadedImage = new Image();
                        this.imageToEdit = new Image();
                        this.downloadedImage.Source = new BitmapImage(new Uri((original as FormationImageDecorator).GetImageFilePath()));
                        this.imageToEdit.Source = new BitmapImage(new Uri((original as FormationImageDecorator).GetImageFilePath()));
                        this.noImageFoundLabel.Visibility = Visibility.Hidden;

                        this.downloadedImage.Width = 125;

                        this.imagePanel.Children.Add(downloadedImage);
                    }
                    original = (original as FormationDecorator).GetWrapee();
                }

                if(original is Formation)
                {
                    this.formationTypeComboBox.SelectedItem = this.formationTypeComboBox.Items[0];
                }
                else if(original is LowestUnit)
                {
                    this.formationTypeComboBox.SelectedItem = this.formationTypeComboBox.Items[1];
                }

                this.formationNameTextBox.Text = original.FormationName;
                this.hierarchyNameTextBox.Text = original.HierarchyName;
                this.commanderRankTextBox.Text = original.CommanderNeededRank.RankName;

                List<string> additionalEquipmentListString = new List<string>();
                foreach(var equipment in original.AdditionalEquipment)
                {
                    additionalEquipmentListString.Add(equipment.EquipmentModel);
                }

                if (additionalEquipmentListString.Count > 0)
                {
                    this.AdditionalEquipmentAssignedTextBox.Text += additionalEquipmentListString[0];
                    for(int i = 1;i<additionalEquipmentListString.Count;++i)
                    {
                        this.AdditionalEquipmentAssignedTextBox.Text += ", " + additionalEquipmentListString[i];
                    }
                    
                }

                List<string> heavyEquipmentListString = new List<string>();
                foreach (var equipment in original.HeavyEquipmentAssigned)
                {
                    heavyEquipmentListString.Add(equipment.EquipmentModel);
                }

                if (heavyEquipmentListString.Count > 0)
                {
                    this.heavyEquipmentAsignedTextBox.Text += heavyEquipmentListString[0];
                    for (int i = 1; i < heavyEquipmentListString.Count; ++i)
                    {
                        this.heavyEquipmentAsignedTextBox.Text += ", " + heavyEquipmentListString[i];
                    }

                }
                this.createFormationButton.Content = "Edit";
            }
        }

        private void imagePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "d:\\";
            openFileDialog.Filter = "Image files|*.bmp;*.jpg;*.png";
            try
            {
                if(openFileDialog.ShowDialog() == true)
                {
                    if(imagePanel.Children.Contains(this.downloadedImage))
                    {
                        imagePanel.Children.Remove(this.downloadedImage);
                    }
                    downloadedImage = new Image();
                    downloadedImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    downloadedImage.Width = 125;
                    imagePanel.Children.Add(downloadedImage);
                    this.noImageFoundLabel.Visibility = Visibility.Hidden;
                }
            }
            catch
            {
                MessageBox.Show("Error reading image file!");
            }

        }
        private bool CheckInputData()
        {
            if (this.formationTypeComboBox.SelectedItem == null)
                return false;

            if (this.formationNameTextBox.Text.Trim() == "") 
                return false;

            if (this.hierarchyNameTextBox.Text.Trim() == "") 
                return false;

            if (MilitaryApp.Instance.GetRank(this.commanderRankTextBox.Text.Trim()) == null) 
                return false;

            string[] heavyEquipmentList = this.heavyEquipmentAsignedTextBox.Text.Split(',');
            
            for (int i = 0; i < heavyEquipmentList.Length; ++i)
                heavyEquipmentList[i] = heavyEquipmentList[i].Trim();

            for(int i = 0; i < heavyEquipmentList.Length; ++i)
            {
                if (heavyEquipmentList[i] == "") continue;
                if (MilitaryApp.Instance.GetEquipment(heavyEquipmentList[i]) == null) return false;
            }

            string[] additionalEquipmentList = this.AdditionalEquipmentAssignedTextBox.Text.Split(',');

            for (int i = 0; i < additionalEquipmentList.Length; ++i)
                additionalEquipmentList[i] = additionalEquipmentList[i].Trim();

            for(int i = 0;i<additionalEquipmentList.Length;++i)
            {
                if (additionalEquipmentList[i] == "") continue;
                if (MilitaryApp.Instance.GetEquipment(additionalEquipmentList[i]) == null) return false;
            }

            return true;
        }
        private void createFormationButton_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                if (!CheckInputData())
                {
                    MessageBox.Show("Formation is not created! Wrong input!");
                    return;
                }
                IBuilderFormation builderFormation;

                if ((this.formationTypeComboBox.SelectedItem as ComboBoxItem).Content.ToString() == "Formation") builderFormation = new FormationBuilder();
                else if ((this.formationTypeComboBox.SelectedItem as ComboBoxItem).Content.ToString() == "Lowest Unit") builderFormation = new LowestUnitBuilder();
                else throw new NullReferenceException("Selected formation type is not present in combo box!");

                if(formationToEdit!=null)
                {
                    if(!builderFormation.SetFormation(this.formationToEdit))
                    {
                        MessageBox.Show("Wrong selected formation type");
                        return;
                    }
                }

                this.builderDirector.BuildFormation(builderFormation);
                if(this.formationToEdit!=null)
                {
                    this.formationToEdit = builderFormation.GetFormation();
                }
                else MilitaryApp.Instance.AddFormation(builderFormation.GetFormation());
                MessageBox.Show("Formation created!");
                MilitaryApp.Instance.DisplayAllFormations();
                MilitaryApp.Instance.DisplayAllSavedFormations();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void resetFormationButton_Click(object sender, RoutedEventArgs e)
        {
            this.noImageFoundLabel.Visibility = Visibility.Visible;
            this.imagePanel.Children.Remove(this.downloadedImage);
            this.downloadedImage = null;
            this.formationTypeComboBox.SelectedItem = null;
            this.formationNameTextBox.Text = "";
            this.hierarchyNameTextBox.Text = "";
            this.commanderRankTextBox.Text = "";
            this.heavyEquipmentAsignedTextBox.Text = "";
            this.AdditionalEquipmentAssignedTextBox.Text = "";

        }
    }
}
