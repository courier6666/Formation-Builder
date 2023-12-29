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
    /// Логика взаимодействия для CreateEquipmentWindow.xaml
    /// </summary>
    public partial class CreateEquipmentWindow : Window
    {
        Image downloadedImage;
        MainWindow mainWindowPointer;
        Equipment equipmentToEdit;
        public CreateEquipmentWindow(MainWindow mainWindow, Equipment equipment = null)
        {
            InitializeComponent();
            this.mainWindowPointer = mainWindow;
            this.equipmentToEdit = equipment;
            if(this.equipmentToEdit != null)
            {
                if(this.equipmentToEdit.ImagePath!=null&& this.equipmentToEdit.ImagePath !="")
                {
                    this.downloadedImage = new Image();
                    this.downloadedImage.Source = new BitmapImage(new Uri(equipment.ImagePath));
                    this.noImageFoundLabel.Visibility = Visibility.Hidden;
                }

                this.equipmentTypeTextBox.Text = equipment.EquipmentType;
                this.equipmentModelTextBox.Text = equipment.EquipmentModel;
                this.imageEquipmentPanel.Children.Add(this.downloadedImage);
                this.createEquipmentButton.Content = "Edit";
            }
        }

        private bool CheckInputData()
        {
            if (this.equipmentModelTextBox.Text.Trim() == "")
                return false;

            if (this.equipmentTypeTextBox.Text.Trim() == "")
                return false;

            return true;
        }

        private void createEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CheckInputData())
                {
                    MessageBox.Show("Equipment is not created! Wrong input!");
                    return;
                }
                string imageEquipmentPath = this.downloadedImage.Source.ToString();
                string equipmentModel = this.equipmentModelTextBox.Text.Trim();
                string equipmentType = this.equipmentTypeTextBox.Text.Trim();
                if (this.equipmentToEdit == null)
                {
                    Equipment equipment = Equipment.CreateEquipment(equipmentType, equipmentModel, imageEquipmentPath);
                    MilitaryApp.Instance.AddEquipment(equipment);
                    MessageBox.Show("Equipment created!");
                }
                else
                {
                    this.equipmentToEdit.EquipmentModel = this.equipmentModelTextBox.Text;
                    this.equipmentToEdit.EquipmentType = this.equipmentTypeTextBox.Text;

                    if(this.downloadedImage!=null)
                        this.equipmentToEdit.ImagePath = this.downloadedImage.Source.ToString().Remove(0, 8);

                    MessageBox.Show("Equipment edited!");
                }
                MilitaryApp.Instance.DisplayAllEquipment();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void resetEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            this.noImageFoundLabel.Visibility = Visibility.Visible;
            this.imageEquipmentPanel.Children.Remove(this.downloadedImage);
            this.downloadedImage = null;
            this.equipmentModelTextBox.Text = "";
            this.equipmentTypeTextBox.Text = "";
        }

        private void imageEquipmentPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "d:\\";
            openFileDialog.Filter = "Image files|*.bmp;*.jpg;*.png";
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    if (imageEquipmentPanel.Children.Contains(this.downloadedImage))
                    {
                        imageEquipmentPanel.Children.Remove(this.downloadedImage);
                    }
                    downloadedImage = new Image();
                    downloadedImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    downloadedImage.Width = 125;
                    imageEquipmentPanel.Children.Add(downloadedImage);
                    this.noImageFoundLabel.Visibility = Visibility.Hidden;
                }
            }
            catch
            {
                MessageBox.Show("Error reading image file!");
            }
        }
    }
}
