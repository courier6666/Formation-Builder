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
    /// Логика взаимодействия для PersonnelCreateWindow.xaml
    /// </summary>
    public partial class PersonnelCreateWindow : Window
    {
        MainWindow mainWindowPointer;
        Personnel personnelToEdit;
        public PersonnelCreateWindow(MainWindow mainWindow, Personnel personnel = null)
        {
            InitializeComponent();
            mainWindowPointer = mainWindow;
            this.personnelToEdit = personnel;
            if (this.personnelToEdit != null)
            {
                this.personnelRankTextBox.Text = this.personnelToEdit.PersonnelRank.RankName;
                this.personnelRoleTextBox.Text = this.personnelToEdit.Role;

                List<string> equipmentListString = new List<string>();
                foreach (var equipment in this.personnelToEdit.Equipment)
                {
                    equipmentListString.Add(equipment.EquipmentModel);
                }

                if (equipmentListString.Count > 0)
                {
                    this.personnelEquipmentTextBox.Text += equipmentListString[0];
                    for (int i = 1; i < equipmentListString.Count; ++i)
                    {
                        this.personnelEquipmentTextBox.Text += ", " + equipmentListString[i];
                    }

                }
                this.createPersonnelButton.Content = "Edit";
            }

        }

        private bool CheckInputData()
        {
            string rankName = this.personnelRankTextBox.Text.Trim();
            if (MilitaryApp.Instance.GetRank(rankName) == null)
                return false;

            if (this.personnelRoleTextBox.Text.Trim() == "")
                return false;

            string[] equipmentList = this.personnelEquipmentTextBox.Text.Split(',');

            for (int i = 0; i < equipmentList.Length; ++i)
                equipmentList[i] = equipmentList[i].Trim();

            for (int i = 0; i < equipmentList.Length; ++i)
            {
                if (equipmentList[i] == "") continue;
                if (MilitaryApp.Instance.GetEquipment(equipmentList[i]) == null) return false;
            }

            return true;
        }

        private void createPersonnelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CheckInputData())
                {
                    MessageBox.Show("Personnel is not created! Wrong input!");
                    return;
                }

                string rankName = this.personnelRankTextBox.Text.Trim();
                string role = this.personnelRoleTextBox.Text.Trim();
                Rank rank = MilitaryApp.Instance.GetRank(rankName);
                List<Equipment> equipment = new List<Equipment>();

                string[] equipmentList = this.personnelEquipmentTextBox.Text.Split(',');




                for (int i = 0; i < equipmentList.Length; ++i)
                    equipmentList[i] = equipmentList[i].Trim();

                for (int i = 0; i < equipmentList.Length; ++i)
                {
                    if (equipmentList[i] == "") continue;
                    equipment.Add(MilitaryApp.Instance.GetEquipment(equipmentList[i]));
                }

                if (this.personnelToEdit == null)
                {
                    Personnel personnel = new Personnel();
                    personnel.PersonnelRank = rank;
                    personnel.Role = role;
                    personnel.Equipment = equipment;
                    MilitaryApp.Instance.AddPersonnelRole(personnel);
                    MessageBox.Show("Personnel created!");
                }
                else
                {
                    this.personnelToEdit.Equipment = equipment;
                    this.personnelToEdit.PersonnelRank = rank;
                    this.personnelToEdit.Role = role;
                    MessageBox.Show("Personnel edited!");
                }
                MilitaryApp.Instance.DisplayAllPersonnelRoles();


            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void resetPersonnelButton_Click(object sender, RoutedEventArgs e)
        {
            this.personnelRankTextBox.Text = "";
            this.personnelRoleTextBox.Text = "";
            this.personnelEquipmentTextBox.Text = "";
        }
    }
}
