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
    /// Логика взаимодействия для CreateRankWindow.xaml
    /// </summary>
    public partial class CreateRankWindow : Window
    {
        public MainWindow mainWindowPointer;
        public Image downloadedImage;
        public Rank rankToEdit;
        public CreateRankWindow(MainWindow mainWindow, Rank rank = null)
        {
            InitializeComponent();
            this.mainWindowPointer = mainWindow;
            this.rankToEdit = rank;
            if(this.rankToEdit != null)
            {
                if (this.rankToEdit.ImagePath != null && this.rankToEdit.ImagePath != "")
                {
                    this.downloadedImage = new Image();
                    this.downloadedImage.Source = new BitmapImage(new Uri(this.rankToEdit.ImagePath));
                    this.downloadedImage.Width = 125;
                    this.imageRankPanel.Children.Add(this.downloadedImage);
                    this.noImageFoundLabel.Visibility = Visibility.Hidden;
                }
                this.rankTypeComboBox.SelectedItem = this.rankTypeComboBox.Items[((int)rank.Type)];
                this.rankNameTextBox.Text = rank.RankName;
                this.rankValueTextBox.Text = rank.RankValue.ToString();
                this.createRankButton.Content = "Edit";
            }
            
        }

        private void imageRankPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "d:\\";
            openFileDialog.Filter = "Image files|*.bmp;*.jpg;*.png";
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    if (this.imageRankPanel.Children.Contains(this.downloadedImage))
                    {
                        this.imageRankPanel.Children.Remove(this.downloadedImage);
                    }
                    downloadedImage = new Image();
                    downloadedImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    downloadedImage.Width = 125;
                    imageRankPanel.Children.Add(downloadedImage);
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
            if (this.rankNameTextBox.Text.Trim() == "")
                return false;

            if (this.rankTypeComboBox.SelectedItem == null)
                return false;

            int value;
            if (this.rankValueTextBox.Text.Trim() == "" || int.TryParse(this.rankValueTextBox.Text.Trim(), out value))
                return false;

            return true;
        }
        private void createRankButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(CheckInputData())
                {
                    MessageBox.Show("Rank is not created! Wrong input!");
                    return;
                }
                string rankName = this.rankNameTextBox.Text.Trim();

                RankType rankType;
                switch((this.rankTypeComboBox.SelectedItem as ComboBoxItem).Content.ToString())
                {
                    case "Enlisted": rankType = RankType.Enlisted; break;
                    case "Non Commissioned": rankType = RankType.NonCommisioned;break;
                    case "Warrant": rankType = RankType.Warrant;break;
                    case "Officer": rankType = RankType.Officer;break;
                    default: throw new NullReferenceException("Selected rank is not present in combo box!");
                }

                int rankValue = int.Parse(this.rankValueTextBox.Text.Trim());
                string imageSource = null;
                if (this.downloadedImage != null)
                    imageSource = this.downloadedImage.Source.ToString().Remove(0, 8);
                if (this.rankToEdit == null)
                {
                    Rank newRank = Rank.CreateRank(rankType, rankName, rankValue, imageSource);
                    MilitaryApp.Instance.AddRank(newRank);
                    MessageBox.Show("Rank created!");
                }
                else
                {
                    this.rankToEdit.Type = rankType;
                    this.rankToEdit.RankName = rankName;
                    this.rankToEdit.RankValue = rankValue;
                    this.rankToEdit.ImagePath = imageSource;
                    MessageBox.Show("Rank edited!");
                }
                MilitaryApp.Instance.DisplayAllRanks();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void resetRankButton_Click(object sender, RoutedEventArgs e)
        {
            this.noImageFoundLabel.Visibility = Visibility.Visible;
            this.imageRankPanel.Children.Remove(this.downloadedImage);
            this.downloadedImage = null;
            this.rankNameTextBox.Text = "";
            this.rankTypeComboBox.SelectedItem = null;
            this.rankValueTextBox.Text = "";

        }
    }
}
