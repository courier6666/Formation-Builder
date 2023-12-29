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
using System.Windows.Media.Effects;


namespace CourseworkWPF.MilitaryFolder
{
        public interface IDisplayerStrategy
        {
            public Grid CreateDisplayPanelEquipment(Equipment equipment, Grid prevGrid = null);
            public Grid CreateDisplayPanelPersonnel(Personnel personnel, Grid prevGrid = null);
            public Grid CreateDisplayPanelFormation(IFormation formation, Grid prevGrid = null);
            public Grid CreateDisplayPanelFormation(FormationDecorator formationDecorator, Grid prevGrid = null);
            public Grid CreateDisplayPanelFormation(FormationImageDecorator formationImageDecorator, Grid prevGrid = null);
            public Grid CreateDisplayPanelRank(Rank rank, Grid prevGrid = null);
        }
        public class ConcreteDisplayerStrategy : IDisplayerStrategy
        {
            public Grid CreateDisplayPanelEquipment(Equipment equipment, Grid prevGrid = null)
            {
                Grid equipmentGrid;
                if (prevGrid != null)
                    equipmentGrid = prevGrid;
                else
                    equipmentGrid = new Grid();

                Image equipmentImage = new Image();
                equipmentImage.Source = new BitmapImage(new Uri(equipment.ImagePath));
                equipmentImage.Width = 150;
                equipmentImage.HorizontalAlignment = HorizontalAlignment.Left;

                Grid labels = new Grid();
                labels.RowDefinitions.Add(new RowDefinition());
                labels.RowDefinitions.Add(new RowDefinition());

                Label labelEquipmentType = new Label();
                labelEquipmentType.Content = equipment.EquipmentType;
                labelEquipmentType.HorizontalAlignment = HorizontalAlignment.Right;

                Label labelEquipmentModel = new Label();
                labelEquipmentModel.Content = equipment.EquipmentModel;
                labelEquipmentModel.HorizontalAlignment = HorizontalAlignment.Right;

                Grid.SetRow(labelEquipmentType, 0);
                Grid.SetRow(labelEquipmentModel, 1);

                labels.Children.Add(labelEquipmentType);
                labels.Children.Add(labelEquipmentModel);

                equipmentGrid.Height = 55;
                equipmentGrid.Children.Add(equipmentImage);
                labels.HorizontalAlignment = HorizontalAlignment.Right;
                equipmentGrid.Children.Add(labels);

                DropShadowEffect dropShadow = new DropShadowEffect();
                dropShadow.BlurRadius = 10;
                dropShadow.Color = Colors.Black;
                dropShadow.Direction = 315;
                dropShadow.ShadowDepth = 1;

                equipmentGrid.Background = Brushes.Aquamarine;
                equipmentGrid.Effect = dropShadow;

                return equipmentGrid;
            }
            public Grid CreateDisplayPanelPersonnel(Personnel personnel, Grid prevGrid = null)
            {
                Grid personnelGrid;
                if (prevGrid != null)
                    personnelGrid = prevGrid;
                else
                    personnelGrid = new Grid();

                Image rankImage = new Image();
                rankImage.Margin = new Thickness(5);
                rankImage.Source = new BitmapImage(new Uri(personnel.PersonnelRank.ImagePath));
                rankImage.HorizontalAlignment = HorizontalAlignment.Left;
                personnelGrid.Children.Add(rankImage);

                if (personnel.Equipment != null && personnel.Equipment.Count > 0)
                {
                    Image mainWeapon = new Image();
                    mainWeapon.Margin = new Thickness(5);
                    mainWeapon.Source = new BitmapImage(new Uri(personnel.Equipment[0].ImagePath));
                    mainWeapon.HorizontalAlignment = HorizontalAlignment.Center;
                    personnelGrid.Children.Add(mainWeapon);
                }

                Grid labels = new Grid();
                labels.RowDefinitions.Add(new RowDefinition());
                labels.RowDefinitions.Add(new RowDefinition());

                Label roleLabel = new Label();
                roleLabel.Content = personnel.Role;
                roleLabel.HorizontalAlignment = HorizontalAlignment.Right;
                roleLabel.VerticalAlignment = VerticalAlignment.Center;

                Label rankLabel = new Label();
                rankLabel.Content = personnel.PersonnelRank.RankName;
                rankLabel.HorizontalAlignment = HorizontalAlignment.Right;

                Grid.SetRow(roleLabel, 0);
                Grid.SetRow(rankLabel, 1);

                labels.HorizontalAlignment = HorizontalAlignment.Right;
                labels.Children.Add(roleLabel);
                labels.Children.Add(rankLabel);

                personnelGrid.Height = 55;

                personnelGrid.Children.Add(labels);
                personnelGrid.Background = Brushes.Aquamarine;

                DropShadowEffect dropShadowRankImage = new DropShadowEffect();
                dropShadowRankImage.BlurRadius = 10;
                dropShadowRankImage.Color = Colors.Black;
                dropShadowRankImage.Direction = 315;
                dropShadowRankImage.ShadowDepth = 1;

                DropShadowEffect dropShadowPersonnelGrid = new DropShadowEffect();
                dropShadowPersonnelGrid.BlurRadius = 5;
                dropShadowPersonnelGrid.Color = Colors.Black;
                dropShadowPersonnelGrid.Direction = 315;
                dropShadowPersonnelGrid.ShadowDepth = 1;

                personnelGrid.Effect = dropShadowPersonnelGrid;
                rankImage.Effect = dropShadowRankImage;

                return personnelGrid;
            }
            public Grid CreateDisplayPanelFormation(IFormation formation, Grid prevGrid = null)
            {
                Grid formationGrid;
                if (prevGrid != null)
                    formationGrid = prevGrid;
                else
                    formationGrid = new Grid();

                Grid labels = new Grid();
                labels.RowDefinitions.Add(new RowDefinition());
                labels.RowDefinitions.Add(new RowDefinition());

                Label labelFormaionName = new Label();
                labelFormaionName.Content = formation.FormationName + " " + formation.HierarchyName;
                labelFormaionName.HorizontalAlignment = HorizontalAlignment.Right;


                Label labelRank = new Label();
                labelRank.Content = "Commander: " + formation.CommanderNeededRank.RankName;
                labelRank.HorizontalAlignment = HorizontalAlignment.Right;


                Grid.SetRow(labelFormaionName, 0);
                Grid.SetRow(labelRank, 1);

                labels.Children.Add(labelFormaionName);
                labels.Children.Add(labelRank);

                formationGrid.Height = 55;

                labels.HorizontalAlignment = HorizontalAlignment.Right;

                formationGrid.Children.Add(labels);

                DropShadowEffect dropShadow = new DropShadowEffect();
                dropShadow.BlurRadius = 5;
                dropShadow.Color = Colors.Black;
                dropShadow.Direction = 315;
                dropShadow.ShadowDepth = 1;

                formationGrid.Effect = dropShadow;
                formationGrid.Background = Brushes.Aquamarine;

                return formationGrid;
            }
            public Grid CreateDisplayPanelFormation(FormationDecorator formationDecorator, Grid prevGrid = null)
            {
                Grid grid;
                if (formationDecorator.GetWrapee() is FormationDecorator)
                    grid = (formationDecorator.GetWrapee() as FormationDecorator).CreateFormationPanel(this);
                else grid = this.CreateDisplayPanelFormation(formationDecorator.GetWrapee());

                return grid;
            }
            public Grid CreateDisplayPanelFormation(FormationImageDecorator formationImageDecorator, Grid prevGrid = null)
            {
                Grid gridReturned = null;
                if (formationImageDecorator.GetWrapee() is FormationDecorator)
                    gridReturned = (formationImageDecorator.GetWrapee() as FormationDecorator).CreateFormationPanel(this);
                else gridReturned = this.CreateDisplayPanelFormation(formationImageDecorator.GetWrapee());

                //---Adding image

                Image formationImage = new Image();
                formationImage.Margin = new Thickness(5);
                formationImage.Source = new BitmapImage(new Uri(formationImageDecorator.GetImageFilePath()));
                formationImage.HorizontalAlignment = HorizontalAlignment.Left;
                gridReturned.Children.Add(formationImage);

                DropShadowEffect dropShadow = new DropShadowEffect();
                dropShadow.BlurRadius = 10;
                dropShadow.Color = Colors.Black;
                dropShadow.Direction = 315;
                dropShadow.ShadowDepth = 2;
                formationImage.Effect = dropShadow;

                

                return gridReturned;
            }
            public Grid CreateDisplayPanelRank(Rank rank, Grid prevGrid = null)
            {
                Grid gridReturned = new Grid();

                if(rank.ImagePath != null && rank.ImagePath != "")
                {
                    Image rankImage = new Image();
                    rankImage.Margin = new Thickness(5);
                    rankImage.Source = new BitmapImage(new Uri(rank.ImagePath));
                    rankImage.HorizontalAlignment = HorizontalAlignment.Left;


                    DropShadowEffect dropShadowImage = new DropShadowEffect();
                    dropShadowImage.BlurRadius = 10;
                    dropShadowImage.Color = Colors.Black;
                    dropShadowImage.Direction = 315;
                    dropShadowImage.ShadowDepth = 1;
                    rankImage.Effect = dropShadowImage;
                    gridReturned.Children.Add(rankImage);
                }
                Label rankName = new Label();
                rankName.Content = rank.RankName;
                rankName.HorizontalAlignment = HorizontalAlignment.Right;

                gridReturned.Children.Add(rankName);

                DropShadowEffect dropShadow = new DropShadowEffect();
                dropShadow.BlurRadius = 5;
                dropShadow.Color = Colors.Black;
                dropShadow.Direction = 315;
                dropShadow.ShadowDepth = 1;

                gridReturned.Effect = dropShadow;
                gridReturned.Background = Brushes.Aquamarine;
                gridReturned.Height = 55;

                return gridReturned;
            }
        }

    public class ConcreteDisplayerStrategySecondView : IDisplayerStrategy
    {
        public Grid CreateDisplayPanelEquipment(Equipment equipment, Grid prevGrid = null)
        {
            Grid equipmentGrid;
            if (prevGrid != null)
                equipmentGrid = prevGrid;
            else
                equipmentGrid = new Grid();

            Image equipmentImage = new Image();
            equipmentImage.Source = new BitmapImage(new Uri(equipment.ImagePath));
            equipmentImage.Width = 150;
            equipmentImage.HorizontalAlignment = HorizontalAlignment.Left;

            Grid labels = new Grid();
            labels.RowDefinitions.Add(new RowDefinition());
            labels.RowDefinitions.Add(new RowDefinition());

            Label labelEquipmentType = new Label();
            labelEquipmentType.Content = equipment.EquipmentType;
            labelEquipmentType.HorizontalAlignment = HorizontalAlignment.Right;

            Label labelEquipmentModel = new Label();
            labelEquipmentModel.Content = equipment.EquipmentModel;
            labelEquipmentModel.HorizontalAlignment = HorizontalAlignment.Right;

            Grid.SetRow(labelEquipmentType, 0);
            Grid.SetRow(labelEquipmentModel, 1);

            labels.Children.Add(labelEquipmentType);
            labels.Children.Add(labelEquipmentModel);

            equipmentGrid.Height = 55;
            equipmentGrid.Children.Add(equipmentImage);
            labels.HorizontalAlignment = HorizontalAlignment.Right;
            equipmentGrid.Children.Add(labels);

            DropShadowEffect dropShadow = new DropShadowEffect();
            dropShadow.BlurRadius = 10;
            dropShadow.Color = Colors.Black;
            dropShadow.Direction = 315;
            dropShadow.ShadowDepth = 1;

            equipmentGrid.Background = Brushes.Aquamarine;
            equipmentGrid.Effect = dropShadow;

            return equipmentGrid;
        }
        public Grid CreateDisplayPanelPersonnel(Personnel personnel, Grid prevGrid = null)
        {
            Grid personnelGrid;
            if (prevGrid != null)
                personnelGrid = prevGrid;
            else
                personnelGrid = new Grid();

            Image rankImage = new Image();
            rankImage.Margin = new Thickness(5);
            rankImage.Source = new BitmapImage(new Uri(personnel.PersonnelRank.ImagePath));
            rankImage.HorizontalAlignment = HorizontalAlignment.Left;
            personnelGrid.Children.Add(rankImage);

            Grid labels = new Grid();
            labels.RowDefinitions.Add(new RowDefinition());
            labels.RowDefinitions.Add(new RowDefinition());

            Label roleLabel = new Label();
            roleLabel.Content = personnel.Role;
            roleLabel.HorizontalAlignment = HorizontalAlignment.Right;
            roleLabel.VerticalAlignment = VerticalAlignment.Center;

            Label rankLabel = new Label();
            rankLabel.Content = personnel.PersonnelRank.RankName;
            rankLabel.HorizontalAlignment = HorizontalAlignment.Right;

            Grid.SetRow(roleLabel, 0);
            Grid.SetRow(rankLabel, 1);

            labels.HorizontalAlignment = HorizontalAlignment.Right;
            labels.Children.Add(roleLabel);
            labels.Children.Add(rankLabel);

            personnelGrid.Height = 55;

            personnelGrid.Children.Add(labels);
            personnelGrid.Background = Brushes.Aquamarine;

            DropShadowEffect dropShadowRankImage = new DropShadowEffect();
            dropShadowRankImage.BlurRadius = 10;
            dropShadowRankImage.Color = Colors.Black;
            dropShadowRankImage.Direction = 315;
            dropShadowRankImage.ShadowDepth = 1;

            DropShadowEffect dropShadowPersonnelGrid = new DropShadowEffect();
            dropShadowPersonnelGrid.BlurRadius = 5;
            dropShadowPersonnelGrid.Color = Colors.Black;
            dropShadowPersonnelGrid.Direction = 315;
            dropShadowPersonnelGrid.ShadowDepth = 1;

            personnelGrid.Effect = dropShadowPersonnelGrid;
            rankImage.Effect = dropShadowRankImage;

            return personnelGrid;
        }
        public Grid CreateDisplayPanelFormation(IFormation formation, Grid prevGrid = null)
        {
            Grid formationGrid;
            if (prevGrid != null)
                formationGrid = prevGrid;
            else
                formationGrid = new Grid();

            Grid labels = new Grid();
            labels.RowDefinitions.Add(new RowDefinition());
            labels.RowDefinitions.Add(new RowDefinition());

            Label labelFormaionName = new Label();
            labelFormaionName.Content = formation.FormationName + " " + formation.HierarchyName;
            labelFormaionName.HorizontalAlignment = HorizontalAlignment.Right;


            Label labelRank = new Label();
            labelRank.Content = "Commander: " + formation.CommanderNeededRank.RankName;
            labelRank.HorizontalAlignment = HorizontalAlignment.Right;


            Grid.SetRow(labelFormaionName, 0);
            Grid.SetRow(labelRank, 1);

            labels.Children.Add(labelFormaionName);
            labels.Children.Add(labelRank);

            formationGrid.Height = 55;

            labels.HorizontalAlignment = HorizontalAlignment.Right;

            formationGrid.Children.Add(labels);

            DropShadowEffect dropShadow = new DropShadowEffect();
            dropShadow.BlurRadius = 5;
            dropShadow.Color = Colors.Black;
            dropShadow.Direction = 315;
            dropShadow.ShadowDepth = 1;

            formationGrid.Effect = dropShadow;
            formationGrid.Background = Brushes.Aquamarine;

            return formationGrid;
        }
        public Grid CreateDisplayPanelFormation(FormationDecorator formationDecorator, Grid prevGrid = null)
        {
            Grid grid;
            if (formationDecorator.GetWrapee() is FormationDecorator)
                grid = (formationDecorator.GetWrapee() as FormationDecorator).CreateFormationPanel(this);
            else grid = this.CreateDisplayPanelFormation(formationDecorator.GetWrapee());

            return grid;
        }
        public Grid CreateDisplayPanelFormation(FormationImageDecorator formationImageDecorator, Grid prevGrid = null)
        {
            Grid gridReturned = null;
            if (formationImageDecorator.GetWrapee() is FormationDecorator)
                gridReturned = (formationImageDecorator.GetWrapee() as FormationDecorator).CreateFormationPanel(this);
            else gridReturned = this.CreateDisplayPanelFormation(formationImageDecorator.GetWrapee());

            //---Adding image

            Image formationImage = new Image();
            formationImage.Margin = new Thickness(5);
            formationImage.Source = new BitmapImage(new Uri(formationImageDecorator.GetImageFilePath()));
            formationImage.HorizontalAlignment = HorizontalAlignment.Left;
            gridReturned.Children.Add(formationImage);

            DropShadowEffect dropShadow = new DropShadowEffect();
            dropShadow.BlurRadius = 10;
            dropShadow.Color = Colors.Black;
            dropShadow.Direction = 315;
            dropShadow.ShadowDepth = 2;
            formationImage.Effect = dropShadow;



            return gridReturned;
        }
        public Grid CreateDisplayPanelRank(Rank rank, Grid prevGrid = null)
        {
            Grid gridReturned = new Grid();

            if (rank.ImagePath != null && rank.ImagePath != "")
            {
                Image rankImage = new Image();
                rankImage.Margin = new Thickness(5);
                rankImage.Source = new BitmapImage(new Uri(rank.ImagePath));
                rankImage.HorizontalAlignment = HorizontalAlignment.Left;


                DropShadowEffect dropShadowImage = new DropShadowEffect();
                dropShadowImage.BlurRadius = 10;
                dropShadowImage.Color = Colors.Black;
                dropShadowImage.Direction = 315;
                dropShadowImage.ShadowDepth = 1;
                rankImage.Effect = dropShadowImage;
                gridReturned.Children.Add(rankImage);
            }
            Label rankName = new Label();
            rankName.Content = rank.RankName;
            rankName.HorizontalAlignment = HorizontalAlignment.Right;

            gridReturned.Children.Add(rankName);

            DropShadowEffect dropShadow = new DropShadowEffect();
            dropShadow.BlurRadius = 5;
            dropShadow.Color = Colors.Black;
            dropShadow.Direction = 315;
            dropShadow.ShadowDepth = 1;

            gridReturned.Effect = dropShadow;
            gridReturned.Background = Brushes.Aquamarine;
            gridReturned.Height = 55;

            return gridReturned;
        }
    }


}
