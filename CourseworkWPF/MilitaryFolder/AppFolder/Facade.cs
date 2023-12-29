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

namespace CourseworkWPF.MilitaryFolder
{
    public class FormationManager
    {
        private MainWindow mainWindowPointer;
        private List<IFormation> formationTemplates = new List<IFormation>();
        private List<IFormation> savedFormationTemplates = new List<IFormation>();

        private TextBox hierarchyNameSearchBox;
        private TextBox formationNameSearchBox;
        private TextBox commanderRankSearchBox;

        private StackPanel searchPanelPointer;

        public Dictionary<Equipment, int> GetAllEquipmentCount(IFormation formation)
        {
            Dictionary<Equipment, int> allEquipmentCount = new Dictionary<Equipment, int>();
            foreach (var equipment in formation.GetAllEquipment())
            {
                if (!allEquipmentCount.ContainsKey(equipment))
                    allEquipmentCount.Add(equipment, 0);

                ++allEquipmentCount[equipment];
            }
            return allEquipmentCount;
        }
        public FormationManager(MainWindow mainWindow)
        {
            this.mainWindowPointer = mainWindow;
            this.hierarchyNameSearchBox = new TextBox();
            this.formationNameSearchBox = new TextBox();
            this.commanderRankSearchBox = new TextBox();

            this.hierarchyNameSearchBox.TextChanged += (sender, e) => {
                this.DisplayAllFormations(MilitaryApp.Instance.GetCurrentView(), MilitaryApp.Instance.TemplatesDisplayPanel);
            };

            this.formationNameSearchBox.TextChanged += (sender, e) => {
                this.DisplayAllFormations(MilitaryApp.Instance.GetCurrentView(), MilitaryApp.Instance.TemplatesDisplayPanel);
            };

            this.commanderRankSearchBox.TextChanged += (sender, e) => {
                this.DisplayAllFormations(MilitaryApp.Instance.GetCurrentView(), MilitaryApp.Instance.TemplatesDisplayPanel);
            };

            this.hierarchyNameSearchBox.Height = 25;
            this.formationNameSearchBox.Height = 25;
            this.commanderRankSearchBox.Height = 25;

            this.hierarchyNameSearchBox.Margin = new Thickness(10, 5, 10, 5);
            this.formationNameSearchBox.Margin = new Thickness(10, 5, 10, 5);
            this.commanderRankSearchBox.Margin = new Thickness(10, 5, 10, 5);
        }
        public List<IFormation> FilterFormations(List<IFormation> formations)
        {
            ConcreteFilter<IFormation> filter = new ConcreteFilter<IFormation>();
            List<ISpecification<IFormation>> specifications = new List<ISpecification<IFormation>>();

            if(this.hierarchyNameSearchBox.Text.Trim() != "")
            {
                string[] criterias = this.hierarchyNameSearchBox.Text.Split(',');

                ChainSpecification<IFormation> chainSpecification = new ChainSpecificationOr<IFormation>(null);

                bool allEmptyCriterias = true;
                foreach(var criteria in criterias)
                {
                    if (criteria.Trim() != "")
                    {
                        chainSpecification.AddNextSpec(new HierarchyNameSpecification(criteria));
                        allEmptyCriterias = false;
                    }
                }

                if(!allEmptyCriterias)
                    specifications.Add(chainSpecification);
            }

            if(this.formationNameSearchBox.Text.Trim() != "")
            {
                string[] criterias = this.formationNameSearchBox.Text.Trim().Split(',');

                ChainSpecification<IFormation> chainSpecification = new ChainSpecificationOr<IFormation>(null);

                bool allEmptyCriterias = true;
                foreach (var criteria in criterias)
                {
                    if (criteria.Trim() != "")
                    {
                        chainSpecification.AddNextSpec(new FormationNameSpecification(criteria));
                        allEmptyCriterias = false;
                    }
                }

                if(!allEmptyCriterias)
                    specifications.Add(chainSpecification);
            }
            if(this.commanderRankSearchBox.Text.Trim() != "")
            {
                string[] criterias = this.commanderRankSearchBox.Text.Trim().Split(',');

                ChainSpecification<IFormation> chainSpecification = new ChainSpecificationOr<IFormation>(null);

                bool allEmptyCriterias = true;
                foreach (var criteria in criterias)
                {
                    if (criteria.Trim() != "")
                    {
                        chainSpecification.AddNextSpec(new CommanderRankSpecification(criteria));
                        allEmptyCriterias = false;
                    }
                }

                if(!allEmptyCriterias)
                    specifications.Add(chainSpecification);
            }

            if (specifications.Count < 1)
                return formations;

            ChainSpecification<IFormation> chainSpecificationAnd = new ChainSpecificationAnd<IFormation>(null);

            foreach(var spec in specifications)
            {
                chainSpecificationAnd.AddNextSpec(spec);
            }
            return filter.Filter(formations, chainSpecificationAnd).ToList();
        }
        public void DisplaySearchBar(Grid searchGrid)
        {
            this.searchPanelPointer?.Children.Clear();
            searchGrid.Children.Clear();
            StackPanel stackPanel = new StackPanel();


            Label hierarchyLabel = new Label();
            hierarchyLabel.Content = "Hierarchy Name";
            hierarchyLabel.Margin = new Thickness(10, 5, 10, 5);

            Label formationLabel = new Label();
            formationLabel.Content = "Formation Name";
            formationLabel.Margin = new Thickness(10, 5, 10, 5);

            Label commanderRankNameLabel = new Label();
            commanderRankNameLabel.Content = "Commander Rank";
            commanderRankNameLabel.Margin = new Thickness(10, 5, 10, 5);

            stackPanel.Children.Add(hierarchyLabel);
            stackPanel.Children.Add(this.hierarchyNameSearchBox);
            stackPanel.Children.Add(formationLabel);
            stackPanel.Children.Add(this.formationNameSearchBox);
            stackPanel.Children.Add(commanderRankNameLabel);
            stackPanel.Children.Add(this.commanderRankSearchBox);
            this.searchPanelPointer = stackPanel;

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            searchGrid.Children.Add(scrollViewer);
        }
        public void AddFormation(IFormation formation)
        {
            this.formationTemplates.Add(formation);
        }
        public void RemoveFormation(IFormation formation)
        {
            this.formationTemplates.Remove(formation);
        }
        public List<IFormation> GetAllFormationsTemplates()
        {
            return this.formationTemplates;
        }
        public List<IFormation> GetAllSavedFormationTemplates()
        {
            return this.savedFormationTemplates;
        }
        public void SetTempalateFormations(List<IFormation> templates)
        {
            this.formationTemplates = templates;
        }
        public void DisplayAllSavedFormations(IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            displayPanel.Children.Clear();
            StackPanel stackPanel = new StackPanel();
            foreach (var formation in this.savedFormationTemplates)
            {
                Grid formationGrid = null;
                if (formation is FormationDecorator)
                    formationGrid = displayerStrategy.CreateDisplayPanelFormation(formation as FormationDecorator);
                else formationGrid = displayerStrategy.CreateDisplayPanelFormation(formation);
                formationGrid.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => MilitaryApp.Instance.DisplayOnMain(formation));
                Grid buttonsManip = new Grid();
                buttonsManip.Height = 40;

                Button editButton = CreateEditFormationButton(formation, this.mainWindowPointer);
                editButton.HorizontalAlignment = HorizontalAlignment.Left;

                Button removeButton = CreateSavedFormationRemoveButton(formation, displayerStrategy, displayPanel);
                removeButton.HorizontalAlignment = HorizontalAlignment.Right;

                buttonsManip.Children.Add(editButton);
                buttonsManip.Children.Add(removeButton);

                stackPanel.Children.Add(formationGrid);
                stackPanel.Children.Add(buttonsManip);

                this.GetAllEquipmentCount(formation);
            }

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            displayPanel.Children.Add(scrollViewer);
        }
        public void DisplayAllFormations(IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            List<IFormation> formations = this.FilterFormations(this.formationTemplates);
            displayPanel.Children.Clear();
            StackPanel stackPanel = new StackPanel();
            foreach (var formation in formations)
            {
                Grid formationGrid = null;
                if (formation is FormationDecorator)
                    formationGrid = displayerStrategy.CreateDisplayPanelFormation(formation as FormationDecorator);
                else formationGrid = displayerStrategy.CreateDisplayPanelFormation(formation);
                formationGrid.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => MilitaryApp.Instance.DisplayOnMain(formation));
                Grid buttonsManip = new Grid();
                buttonsManip.Height = 40;

                Button editButton = CreateEditFormationButton(formation, this.mainWindowPointer);
                editButton.HorizontalAlignment = HorizontalAlignment.Left;

                Button removeButton = CreateFormationRemoveButton(formation, displayerStrategy, displayPanel);
                removeButton.HorizontalAlignment = HorizontalAlignment.Right;

                buttonsManip.Children.Add(editButton);
                buttonsManip.Children.Add(removeButton);

                stackPanel.Children.Add(formationGrid);
                stackPanel.Children.Add(buttonsManip);

                this.GetAllEquipmentCount(formation);
            }

            stackPanel.Children.Add(CreateFormationCreatePopupButton());
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            displayPanel.Children.Add(scrollViewer);

        }
        private Button CreateEditFormationButton(IFormation formationEdit, MainWindow mainWindow)
        {
            Button editFormationButton = new Button();
            editFormationButton.Margin = new Thickness(5);
            editFormationButton.Content = "Edit";
            editFormationButton.Width = 100;
            editFormationButton.HorizontalAlignment = HorizontalAlignment.Center;
            editFormationButton.Height = 20;
            editFormationButton.Click += (sender, e) => {
                FormationCreateWindow formationCreateWindow = new FormationCreateWindow(mainWindow, formationEdit);
                formationCreateWindow.Show();
            };

            return editFormationButton;
        }
        public Button CreateFormationCreatePopupButton()
        {
            Button createFormationButton = new Button();
            createFormationButton.Margin = new Thickness(5);
            createFormationButton.Content = "Create";
            createFormationButton.Width = 200;
            createFormationButton.HorizontalAlignment = HorizontalAlignment.Center;
            createFormationButton.Height = 30;
            createFormationButton.Click += (sender, e) => {
                FormationCreateWindow formationCreateWindow = new FormationCreateWindow(this.mainWindowPointer);
                formationCreateWindow.Show();
            };

            return createFormationButton;
        }
        public Button CreateFormationRemoveButton(IFormation formation, IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            Button removeFormationButton = new Button();
            removeFormationButton.Margin = new Thickness(5);
            removeFormationButton.Content = "Remove";
            removeFormationButton.Width = 100;
            removeFormationButton.HorizontalAlignment = HorizontalAlignment.Center;
            removeFormationButton.Height = 20;
            removeFormationButton.Click += (sender, e) => {
                this.formationTemplates.Remove(formation);
                this.DisplayAllFormations(displayerStrategy, displayPanel);
            };

            return removeFormationButton;
        }
        public Button CreateSavedFormationRemoveButton(IFormation formation, IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            Button removeFormationButton = new Button();
            removeFormationButton.Margin = new Thickness(5);
            removeFormationButton.Content = "Remove";
            removeFormationButton.Width = 100;
            removeFormationButton.HorizontalAlignment = HorizontalAlignment.Center;
            removeFormationButton.Height = 20;
            removeFormationButton.Click += (sender, e) => {
                this.savedFormationTemplates.Remove(formation);
                this.DisplayAllSavedFormations(displayerStrategy, displayPanel);
            };

            return removeFormationButton;
        }
        public Button CreateFormationSaveButton(IFormation formation, IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            Button saveFormationButton = new Button();
            saveFormationButton.Margin = new Thickness(5);
            saveFormationButton.Content = "Save";
            saveFormationButton.Width = 100;
            saveFormationButton.HorizontalAlignment = HorizontalAlignment.Left;
            saveFormationButton.VerticalAlignment = VerticalAlignment.Bottom;
            saveFormationButton.Height = 20;
            saveFormationButton.Click += (sender, e) => {
                this.savedFormationTemplates.Add(formation.DeepClone());
                this.DisplayAllSavedFormations(displayerStrategy, displayPanel);
            };

            return saveFormationButton;
        }
        public Button CreateFormationCopyButton(IFormation formation, IDisplayerStrategy displayerStrategy)
        {
            Button copyFormationButton = new Button();
            copyFormationButton.Margin = new Thickness(5);
            copyFormationButton.Content = "Copy";
            copyFormationButton.Width = 100;
            copyFormationButton.HorizontalAlignment = HorizontalAlignment.Right;
            copyFormationButton.VerticalAlignment = VerticalAlignment.Bottom;
            copyFormationButton.Height = 20;
            copyFormationButton.Click += (sender, e) => {
                this.formationTemplates.Add(formation.ShallowClone());
                MilitaryApp.Instance.DisplayAllFormations();
            };

            return copyFormationButton;
        }
        public void DisplayOnMain(IFormation formation, IDisplayerStrategy displayerStrategy, Grid displayPanelHeader, Grid displayPanelMain, Grid displayPanelSaved)
        {
            displayPanelHeader.Children.Clear();
            displayPanelMain.Children.Clear();
            if (formation is FormationDecorator)
                displayPanelHeader.Children.Add(displayerStrategy.CreateDisplayPanelFormation(formation as FormationDecorator));
            else
                displayPanelHeader.Children.Add(displayerStrategy.CreateDisplayPanelFormation(formation));
            IFormation original = formation;
            while (original is FormationDecorator)
            {
                original = (original as FormationDecorator).GetWrapee();
            }

            if (original is Formation) this.DisplayOnMainNoHeader(original as Formation, displayerStrategy, displayPanelMain);
            else if (original is LowestUnit) this.DisplayOnMainNoHeader(original as LowestUnit, displayerStrategy, displayPanelMain);

            displayPanelHeader.Children.Add(CreateFormationSaveButton(formation, displayerStrategy, displayPanelSaved));
            displayPanelHeader.Children.Add(CreateFormationCopyButton(formation, displayerStrategy));
        }

        private void DisplayOnMainNoHeader(Formation formation, IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            displayPanel.Children.Clear();
            StackPanel stackPanel = new StackPanel();

            Label HQLabel = new Label();
            HQLabel.Content = "Headquarters:";
            stackPanel.Children.Add(HQLabel);

            if (formation.HeadquartersFormation != null)
            {
                Grid formationHeadquartersGrid;
                if (formation.HeadquartersFormation is FormationDecorator)
                    formationHeadquartersGrid = displayerStrategy.CreateDisplayPanelFormation(formation.HeadquartersFormation as FormationDecorator);
                else
                    formationHeadquartersGrid = displayerStrategy.CreateDisplayPanelFormation(formation.HeadquartersFormation);

                formationHeadquartersGrid.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => MilitaryApp.Instance.DisplayOnMain(formation.HeadquartersFormation));
                stackPanel.Children.Add(formationHeadquartersGrid);
            }
            stackPanel.Children.Add(CreateSetFormationHeadquartersButton(formation, this.mainWindowPointer, displayerStrategy));
            Label formationsLabel = new Label();
            formationsLabel.Content = "Formations:";
            stackPanel.Children.Add(formationsLabel);
            foreach (var formationSub in formation.SubordinateFormations)
            {
                Grid subFormationGrid;
                if (formationSub is FormationDecorator)
                    subFormationGrid = displayerStrategy.CreateDisplayPanelFormation(formationSub as FormationDecorator);
                else subFormationGrid = displayerStrategy.CreateDisplayPanelFormation(formationSub);

                subFormationGrid.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => MilitaryApp.Instance.DisplayOnMain(formationSub));

                stackPanel.Children.Add(subFormationGrid);
                stackPanel.Children.Add(CreateRemoveFormationFromFormationButton(formation, formationSub));
            }


            stackPanel.Children.Add(CreateAddFormationButton(formation.SubordinateFormations, this.mainWindowPointer, displayerStrategy));


            Label heavyEquipmentLabel = new Label();
            heavyEquipmentLabel.Content = "Heavy Equipment:";
            stackPanel.Children.Add(heavyEquipmentLabel);

            if (formation.HeavyEquipmentAssigned != null)
            {
                foreach (var equipment in formation.HeavyEquipmentAssigned)
                {
                    stackPanel.Children.Add(displayerStrategy.CreateDisplayPanelEquipment(equipment));
                    stackPanel.Children.Add(CreateRemoveEquipmentButton(equipment, formation.AdditionalEquipment));
                }
                stackPanel.Children.Add(CreateAddEquipmentButton(formation.HeavyEquipmentAssigned, this.mainWindowPointer, displayerStrategy));
            }

            Label additionalEquipmentLabel = new Label();
            additionalEquipmentLabel.Content = "Additional Equipment:";
            stackPanel.Children.Add(additionalEquipmentLabel);

            if (formation.AdditionalEquipment != null)
            {
                foreach (var equipment in formation.AdditionalEquipment)
                {
                    stackPanel.Children.Add(displayerStrategy.CreateDisplayPanelEquipment(equipment));
                    stackPanel.Children.Add(CreateRemoveEquipmentButton(equipment, formation.AdditionalEquipment));
                }
                stackPanel.Children.Add(CreateAddEquipmentButton(formation.AdditionalEquipment, this.mainWindowPointer, displayerStrategy));
            }

            Label totalEquipmentLabel = new Label();
            totalEquipmentLabel.Content = "Total Equipment:";

            stackPanel.Children.Add(totalEquipmentLabel);

            foreach (var equipment in GetAllEquipmentCount(formation))
            {
                Label label = new Label();
                label.Content = equipment.Value.ToString() + " Of";
                stackPanel.Children.Add(label);
                Grid equipmentGrid = displayerStrategy.CreateDisplayPanelEquipment(equipment.Key);
                stackPanel.Children.Add(equipmentGrid);
            }

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            displayPanel.Children.Add(scrollViewer);
        }
        private Button CreateRemoveFormationFromFormationButton(Formation formation, IFormation formationToRemove)
        {
            Button removeFormationFromFormationButton = new Button();
            removeFormationFromFormationButton.Margin = new Thickness(5);
            removeFormationFromFormationButton.Content = "Remove";
            removeFormationFromFormationButton.Width = 100;
            removeFormationFromFormationButton.HorizontalAlignment = HorizontalAlignment.Center;
            removeFormationFromFormationButton.Height = 20;
            removeFormationFromFormationButton.Click += (sender, e) => {
                formation.RemoveFormation(formationToRemove);
                removeFormationFromFormationButton.IsEnabled = false;
            };
            return removeFormationFromFormationButton;
        }
        private Button CreateRemovePersonnelFromFormationButton(Personnel personnel, LowestUnit lowestUnit)
        {
            Button removePersonnelFromFormationButton = new Button();
            removePersonnelFromFormationButton.Margin = new Thickness(5);
            removePersonnelFromFormationButton.Content = "Remove";
            removePersonnelFromFormationButton.Width = 100;
            removePersonnelFromFormationButton.HorizontalAlignment = HorizontalAlignment.Center;
            removePersonnelFromFormationButton.Height = 20;
            removePersonnelFromFormationButton.Click += (sender, e) => {
                lowestUnit.RemovePersonnel(personnel);
                removePersonnelFromFormationButton.IsEnabled = false;
            };
            return removePersonnelFromFormationButton;
        }

        private Button CreateAddFormationButton(List<IFormation> formationsList, MainWindow mainWindow, IDisplayerStrategy displayerStrategy)
        {
            Button addFormationButton = new Button();
            addFormationButton.Margin = new Thickness(5);
            addFormationButton.Content = "Add Formation";
            addFormationButton.Width = 100;
            addFormationButton.HorizontalAlignment = HorizontalAlignment.Center;
            addFormationButton.Height = 20;
            addFormationButton.Click += (sender, e) => {
                AddFormationWindow addFormationWindow = new AddFormationWindow(formationsList, mainWindow, displayerStrategy);
                addFormationWindow.Show();
            };
            return addFormationButton;
        }
        private void DisplayOnMainNoHeader(LowestUnit lowestUnit, IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            displayPanel.Children.Clear();
            StackPanel stackPanel = new StackPanel();
            Label commanderLabel = new Label();
            commanderLabel.Content = "Commander: ";
            stackPanel.Children.Add(commanderLabel);

            if (lowestUnit.Commander != null)
            {
                Grid commanderGridElement = displayerStrategy.CreateDisplayPanelPersonnel(lowestUnit.Commander);
                commanderGridElement.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => MilitaryApp.Instance.DisplayOnMain(lowestUnit.Commander));
                stackPanel.Children.Add(commanderGridElement);
            }
            stackPanel.Children.Add(CreateSetUnitCommanderButton(lowestUnit, this.mainWindowPointer, displayerStrategy));
            Label personnelLabel = new Label();
            personnelLabel.Content = "Personnel:";
            stackPanel.Children.Add(personnelLabel);

            if (lowestUnit.Personnels != null)
            {
                foreach (var personnel in lowestUnit.Personnels)
                {
                    Grid personnelElement = displayerStrategy.CreateDisplayPanelPersonnel(personnel);
                    personnelElement.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => MilitaryApp.Instance.DisplayOnMain(personnel));
                    stackPanel.Children.Add(personnelElement);
                    stackPanel.Children.Add(CreateRemovePersonnelFromFormationButton(personnel, lowestUnit));
                }
                stackPanel.Children.Add(CreateAddPersonnelButton(lowestUnit.Personnels, this.mainWindowPointer, displayerStrategy));

            }

            Label heavyEquipmentLabel = new Label();
            heavyEquipmentLabel.Content = "Heavy Equipment:";
            stackPanel.Children.Add(heavyEquipmentLabel);

            if (lowestUnit.HeavyEquipmentAssigned != null)
            {
                foreach (var equipment in lowestUnit.HeavyEquipmentAssigned)
                {
                    stackPanel.Children.Add(displayerStrategy.CreateDisplayPanelEquipment(equipment));
                    stackPanel.Children.Add(CreateRemoveEquipmentButton(equipment, lowestUnit.HeavyEquipmentAssigned));
                }
                stackPanel.Children.Add(CreateAddEquipmentButton(lowestUnit.HeavyEquipmentAssigned, this.mainWindowPointer, displayerStrategy));
            }

            Label additionalEquipmentLabel = new Label();
            additionalEquipmentLabel.Content = "Additional Equipment:";
            stackPanel.Children.Add(additionalEquipmentLabel);

            if (lowestUnit.AdditionalEquipment != null)
            {
                foreach (var equipment in lowestUnit.AdditionalEquipment)
                {
                    stackPanel.Children.Add(displayerStrategy.CreateDisplayPanelEquipment(equipment));
                    stackPanel.Children.Add(CreateRemoveEquipmentButton(equipment, lowestUnit.AdditionalEquipment));
                }
                stackPanel.Children.Add(CreateAddEquipmentButton(lowestUnit.AdditionalEquipment, this.mainWindowPointer, displayerStrategy));
            }

            Label totalEquipmentLabel = new Label();
            totalEquipmentLabel.Content = "Total Equipment:";

            stackPanel.Children.Add(totalEquipmentLabel);

            foreach (var equipment in GetAllEquipmentCount(lowestUnit))
            {
                Label label = new Label();
                label.Content = equipment.Value.ToString() + " Of";
                stackPanel.Children.Add(label);
                stackPanel.Children.Add(displayerStrategy.CreateDisplayPanelEquipment(equipment.Key));
            }

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;


            displayPanel.Children.Add(scrollViewer);
        }
        private Button CreateSetUnitCommanderButton(LowestUnit lowestUnit, MainWindow mainWindow, IDisplayerStrategy displayerStrategy)
        {
            Button setCommanderButton = new Button();
            setCommanderButton.Margin = new Thickness(5);
            setCommanderButton.Content = "Set Commander";
            setCommanderButton.Width = 100;
            setCommanderButton.HorizontalAlignment = HorizontalAlignment.Center;
            setCommanderButton.Height = 20;
            setCommanderButton.Click += (sender, e) => {
                AddPersonnelWindow addPersonnelWindow = new AddPersonnelWindow(lowestUnit, mainWindow, displayerStrategy);
                addPersonnelWindow.Show();
            };
            return setCommanderButton;
        }
        private Button CreateSetFormationHeadquartersButton(Formation formation, MainWindow mainWindow, IDisplayerStrategy displayerStrategy)
        {
            Button setHeadquartersButton = new Button();
            setHeadquartersButton.Margin = new Thickness(5);
            setHeadquartersButton.Content = "Set Headquarters";
            setHeadquartersButton.Width = 100;
            setHeadquartersButton.HorizontalAlignment = HorizontalAlignment.Center;
            setHeadquartersButton.Height = 20;
            setHeadquartersButton.Click += (sender, e) => {
                AddFormationWindow addFormationWindow = new AddFormationWindow(formation, mainWindow, displayerStrategy);
                addFormationWindow.Show();
            };
            return setHeadquartersButton;
        }
        private Button CreateAddEquipmentButton(List<Equipment> equipmentList, MainWindow mainWindow, IDisplayerStrategy displayerStrategy)
        {
            Button addEquipmentButton = new Button();
            addEquipmentButton.Margin = new Thickness(5);
            addEquipmentButton.Content = "Add Equipment";
            addEquipmentButton.Width = 100;
            addEquipmentButton.HorizontalAlignment = HorizontalAlignment.Center;
            addEquipmentButton.Height = 20;
            addEquipmentButton.Click += (sender, e) => {
                AddEquipmentToEquipmentListWindow addEquipmentToEquipmentListWindow = new AddEquipmentToEquipmentListWindow(equipmentList, mainWindow, displayerStrategy);
                addEquipmentToEquipmentListWindow.Show();
            };

            return addEquipmentButton;
        }
        private Button CreateRemoveEquipmentButton(Equipment equipment, List<Equipment> equipmentList)
        {
            Button removeEquipmentButton = new Button();
            removeEquipmentButton.Margin = new Thickness(5);
            removeEquipmentButton.Content = "Remove";
            removeEquipmentButton.Width = 100;
            removeEquipmentButton.HorizontalAlignment = HorizontalAlignment.Left;
            removeEquipmentButton.Height = 20;
            removeEquipmentButton.Click += (sender, e) => {
                equipmentList.Remove(equipment);
                removeEquipmentButton.IsEnabled = false;
            };

            return removeEquipmentButton;
        }
        private Button CreateAddPersonnelButton(List<Personnel> personnelsList, MainWindow mainWindow, IDisplayerStrategy displayerStrategy)
        {
            Button addPersonnelButton = new Button();
            addPersonnelButton.Margin = new Thickness(5);
            addPersonnelButton.Content = "Add Personnel";
            addPersonnelButton.Width = 100;
            addPersonnelButton.HorizontalAlignment = HorizontalAlignment.Center;
            addPersonnelButton.Height = 20;
            addPersonnelButton.Click += (sender, e) => {
                AddPersonnelWindow addPersonnelWindow = new AddPersonnelWindow(personnelsList, this.mainWindowPointer, displayerStrategy);
                addPersonnelWindow.Show();
            };

            return addPersonnelButton;
        }
        //private Button CreateSetCommanderButton()
        //{

        //}
    }
    public class EquipmentManager
    {
        private MainWindow mainWindowPointer;
        private EquipmentFactory equipmentFactory = new EquipmentFactory();

        private TextBox equipmentModelSearchBox;
        private TextBox equipmentTypeSearchBox;
        private StackPanel searchPanelPointer;
        public EquipmentManager(MainWindow mainWindow)
        {
            this.mainWindowPointer = mainWindow;
            this.equipmentModelSearchBox = new TextBox();
            this.equipmentTypeSearchBox = new TextBox();

            this.equipmentModelSearchBox.TextChanged += (sender, e) => {
                this.DisplayAllEquipment(MilitaryApp.Instance.GetCurrentView(), MilitaryApp.Instance.TemplatesDisplayPanel);
            };

            this.equipmentTypeSearchBox.TextChanged += (sender, e) => {
                this.DisplayAllEquipment(MilitaryApp.Instance.GetCurrentView(), MilitaryApp.Instance.TemplatesDisplayPanel);
            };



            this.equipmentModelSearchBox.Height = 25;
            this.equipmentTypeSearchBox.Height = 25;

            this.equipmentModelSearchBox.Margin = new Thickness(10, 5, 10, 5);
            this.equipmentTypeSearchBox.Margin = new Thickness(10, 5, 10, 5);
        }

        public List<Equipment> FilterEquipment(List<Equipment> equipments)
        {
            ConcreteFilter<Equipment> filter = new ConcreteFilter<Equipment>();
            List<ISpecification<Equipment>> specifications = new List<ISpecification<Equipment>>();

            if(this.equipmentModelSearchBox.Text.Trim() != "")
            {
                string[] criterias = this.equipmentModelSearchBox.Text.Split(',');

                ChainSpecification<Equipment> chainSpecification = new ChainSpecificationOr<Equipment>(null);

                bool allEmptyCriterias = true;
                foreach (var criteria in criterias)
                {
                    if(criteria.Trim()!="")
                    {
                        chainSpecification.AddNextSpec(new EquipmentModelSpecification(criteria.Trim()));
                        allEmptyCriterias = false;
                    }
                }

                if (!allEmptyCriterias)
                    specifications.Add(chainSpecification);
            }

            if(this.equipmentTypeSearchBox.Text.Trim() != "")
            {
                string[] criterias = this.equipmentTypeSearchBox.Text.Split(',');

                ChainSpecification<Equipment> chainSpecification = new ChainSpecificationOr<Equipment>(null);

                bool allEmptyCriterias = true;
                foreach (var criteria in criterias)
                {
                    if (criteria.Trim() != "")
                    {
                        chainSpecification.AddNextSpec(new EquipmentTypeSpecification(criteria.Trim()));
                        allEmptyCriterias = false;
                    }
                }

                if (!allEmptyCriterias)
                    specifications.Add(chainSpecification);
            }

            if (specifications.Count < 1)
                return equipments;

            ChainSpecificationAnd<Equipment> chainSpecificationAnd = new ChainSpecificationAnd<Equipment>(null);

            foreach (var spec in specifications)
            {
                chainSpecificationAnd.AddNextSpec(spec);
            }

            return filter.Filter(equipments, chainSpecificationAnd).ToList();
        }

        public void DisplaySearchBar(Grid searchGrid)
        {
            this.searchPanelPointer?.Children.Clear();
            searchGrid.Children.Clear();
            StackPanel stackPanel = new StackPanel();


            Label modelLabel = new Label();
            modelLabel.Content = "Equipment Model";
            modelLabel.Margin = new Thickness(10, 5, 10, 5);

            Label typeLabel = new Label();
            typeLabel.Content = "Equipment Type";
            typeLabel.Margin = new Thickness(10, 5, 10, 5);


            stackPanel.Children.Add(modelLabel);
            stackPanel.Children.Add(this.equipmentModelSearchBox);
            stackPanel.Children.Add(typeLabel);
            stackPanel.Children.Add(this.equipmentTypeSearchBox);

            this.searchPanelPointer = stackPanel;

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            searchGrid.Children.Add(scrollViewer);
        }

        public Dictionary<string, Equipment> GetAllEquipment()
        {
            return this.equipmentFactory.EquipmentPieces;
        }
        public void AddEquipment(Equipment equipment)
        {
            this.equipmentFactory.AddNewEquipment(equipment);
        }
        public Equipment GetEquipment(string equipmentName)
        {
            return this.equipmentFactory.GetEquipment(equipmentName);
        }
        public void RemoveEquipment(Equipment equipment)
        {
            this.equipmentFactory.RemoveEquipment(equipment);
        }
        public void SetEquipment(Dictionary<string, Equipment> equipmentPieces)
        {
            this.equipmentFactory.EquipmentPieces = equipmentPieces;
        }
        public void DisplayAllEquipment(IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            displayPanel.Children.Clear();

            List<Equipment> equipmentListFormat = new List<Equipment>();

            foreach (var equipment in equipmentFactory.EquipmentPieces)
                equipmentListFormat.Add(equipment.Value);

            equipmentListFormat = this.FilterEquipment(equipmentListFormat);

            var equipmentPieces = new Dictionary<string, Equipment>();

            foreach (var equipment in equipmentListFormat)
                equipmentPieces.Add(equipment.EquipmentModel, equipment);

            StackPanel stackPanel = new StackPanel();
            foreach (var equipment in equipmentPieces)
            {

                stackPanel.Children.Add(displayerStrategy.CreateDisplayPanelEquipment(equipment.Value));

                Grid buttonsManip = new Grid();
                buttonsManip.Height = 40;

                Button editButton = CreateEditEquipmentButton(equipment.Value, this.mainWindowPointer);

                Button removeButton = CreateRemoveEquipmentButton(equipment.Value, displayerStrategy, displayPanel);

                buttonsManip.Children.Add(editButton);
                buttonsManip.Children.Add(removeButton);

                stackPanel.Children.Add(buttonsManip);
            }
            stackPanel.Children.Add(CreateEquipmentCreatePopupButton());
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            displayPanel.Children.Add(scrollViewer);
        }

        private Button CreateEditEquipmentButton(Equipment equipment, MainWindow mainWindow)
        {
            Button editEquipemntButton = new Button();
            editEquipemntButton.Margin = new Thickness(5);
            editEquipemntButton.Content = "Edit";
            editEquipemntButton.Width = 100;
            editEquipemntButton.HorizontalAlignment = HorizontalAlignment.Left;
            editEquipemntButton.Height = 20;
            editEquipemntButton.Click += (sender, e) => {
                CreateEquipmentWindow editEquipmentWindow = new CreateEquipmentWindow(mainWindow, equipment);
                editEquipmentWindow.Show();
            };

            return editEquipemntButton;
        }

        public Button CreateEquipmentCreatePopupButton()
        {
            Button createEquipmentButton = new Button();
            createEquipmentButton.Margin = new Thickness(5);
            createEquipmentButton.Content = "Create";
            createEquipmentButton.Width = 200;
            createEquipmentButton.HorizontalAlignment = HorizontalAlignment.Center;
            createEquipmentButton.Height = 30;
            createEquipmentButton.Click += (sender, e) => {
                CreateEquipmentWindow createEquipmentWindow = new CreateEquipmentWindow(this.mainWindowPointer);
                createEquipmentWindow.Show();
            };

            return createEquipmentButton;
        }
        public Button CreateRemoveEquipmentButton(Equipment equipment, IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            Button removeEquipmentButton = new Button();
            removeEquipmentButton.Margin = new Thickness(5);
            removeEquipmentButton.Content = "Remove";
            removeEquipmentButton.Width = 100;
            removeEquipmentButton.HorizontalAlignment = HorizontalAlignment.Right;
            removeEquipmentButton.Height = 20;
            removeEquipmentButton.Click += (sender, e) => {
                this.RemoveEquipment(equipment);
                this.DisplayAllEquipment(displayerStrategy, displayPanel);
            };

            return removeEquipmentButton;
        }
    }

    public class PersonnelManager
    {
        private MainWindow mainWindowPointer;
        private List<Personnel> personnelRoles = new List<Personnel>();

        private TextBox roleSearchBox;
        private TextBox rankNameSearchBox;
        private StackPanel searchPanelPointer;
        public PersonnelManager(MainWindow mainWindow)
        {
            this.mainWindowPointer = mainWindow;

            this.roleSearchBox = new TextBox();
            this.rankNameSearchBox = new TextBox();

            this.roleSearchBox.TextChanged += (sender, e) => {
                this.DisplayAllPersonnelRoles(MilitaryApp.Instance.GetCurrentView(), MilitaryApp.Instance.TemplatesDisplayPanel);
            };

            this.rankNameSearchBox.TextChanged += (sender, e) => {
                this.DisplayAllPersonnelRoles(MilitaryApp.Instance.GetCurrentView(), MilitaryApp.Instance.TemplatesDisplayPanel);
            };



            this.roleSearchBox.Height = 25;
            this.rankNameSearchBox.Height = 25;

            this.roleSearchBox.Margin = new Thickness(10, 5, 10, 5);
            this.rankNameSearchBox.Margin = new Thickness(10, 5, 10, 5);
        }
        public void DisplaySearchBar(Grid searchGrid)
        {
            this.searchPanelPointer?.Children.Clear();
            searchGrid.Children.Clear();
            StackPanel stackPanel = new StackPanel();

            Label roleLabel = new Label();
            roleLabel.Content = "Personnel Role";
            roleLabel.Margin = new Thickness(10, 5, 10, 5);

            Label rankNameLabel = new Label();
            rankNameLabel.Content = "Personnel Rank";
            rankNameLabel.Margin = new Thickness(10, 5, 10, 5);

            stackPanel.Children.Add(roleLabel);
            stackPanel.Children.Add(this.roleSearchBox);
            stackPanel.Children.Add(rankNameLabel);
            stackPanel.Children.Add(this.rankNameSearchBox);

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            this.searchPanelPointer = stackPanel;

            searchGrid.Children.Add(scrollViewer);
        }
        public List<Personnel> FilterPersonnel(List<Personnel> personnels)
        {
            ConcreteFilter<Personnel> filter = new ConcreteFilter<Personnel>();
            List<ISpecification<Personnel>> specifications = new List<ISpecification<Personnel>>();

            if (this.roleSearchBox.Text.Trim() != "")
            {
                string[] criterias = this.roleSearchBox.Text.Split(',');

                ChainSpecification<Personnel> chainSpecification = new ChainSpecificationOr<Personnel>(null);

                bool allEmptyCriterias = true;
                foreach (var criteria in criterias)
                {
                    if (criteria.Trim() != "")
                    {
                        chainSpecification.AddNextSpec(new PersonnelRoleSpecification(criteria));
                        allEmptyCriterias = false;
                    }
                }

                if (!allEmptyCriterias)
                    specifications.Add(chainSpecification);
            }

            if (this.rankNameSearchBox.Text.Trim() != "")
            {
                string[] criterias = this.rankNameSearchBox.Text.Split(',');

                ChainSpecification<Personnel> chainSpecification = new ChainSpecificationOr<Personnel>(null);

                bool allEmptyCriterias = true;
                foreach (var criteria in criterias)
                {
                    if (criteria.Trim() != "")
                    {
                        chainSpecification.AddNextSpec(new PersonnelRankNameSpecification(criteria));
                        allEmptyCriterias = false;
                    }
                }

                if (!allEmptyCriterias)
                    specifications.Add(chainSpecification);
            }

            if (specifications.Count < 1)
                return personnels;

            ChainSpecification<Personnel> chainSpecificationAnd = new ChainSpecificationAnd<Personnel>(null);

            foreach (var spec in specifications)
            {
                chainSpecificationAnd.AddNextSpec(spec);
            }
            return filter.Filter(personnels, chainSpecificationAnd).ToList();
        }
        public void AddPersonnel(Personnel personnel)
        {
            this.personnelRoles.Add(personnel);
        }
        public void RemovePersonnel(Personnel personnel)
        {
            this.personnelRoles.Remove(personnel);
        }
        public List<Personnel> GetAllPersonnel()
        {
            return this.personnelRoles;
        }
        public void SetPersonnel(List<Personnel> personnels)
        {
            this.personnelRoles = personnels;
        }
        public void DisplayAllPersonnelRoles(IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            displayPanel.Children.Clear();

            List<Personnel> personnels = this.FilterPersonnel(this.personnelRoles);

            StackPanel stackPanel = new StackPanel();
            foreach (var personnel in personnels)
            {
                Grid element = displayerStrategy.CreateDisplayPanelPersonnel(personnel);
                element.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => MilitaryApp.Instance.DisplayOnMain(personnel));

                stackPanel.Children.Add(element);

                Grid buttonsManip = new Grid();
                buttonsManip.Height = 40;

                Button editButton = CreateEditPersonnelButton(personnel, mainWindowPointer);
                editButton.HorizontalAlignment = HorizontalAlignment.Left;

                Button removeButton = CreateRemovePersonnelButton(personnel, displayerStrategy, displayPanel);
                removeButton.HorizontalAlignment = HorizontalAlignment.Right;

                buttonsManip.Children.Add(editButton);
                buttonsManip.Children.Add(removeButton);

                stackPanel.Children.Add(buttonsManip);
            }
            stackPanel.Children.Add(CreatePersonnelCreatePopupButton());
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            displayPanel.Children.Add(scrollViewer);
        }

        private Button CreateEditPersonnelButton(Personnel personnel, MainWindow mainWindow)
        {
            Button editPersonnelButton = new Button();
            editPersonnelButton.Margin = new Thickness(5);
            editPersonnelButton.Content = "Edit";
            editPersonnelButton.Width = 100;
            editPersonnelButton.HorizontalAlignment = HorizontalAlignment.Center;
            editPersonnelButton.Height = 20;
            editPersonnelButton.Click += (sender, e) => {
                PersonnelCreateWindow personnelEditWindow = new PersonnelCreateWindow(mainWindow, personnel);
                personnelEditWindow.Show();
            };

            return editPersonnelButton;
        }

        public void DisplayOnMain(Personnel role, IDisplayerStrategy displayerStrategy, Grid displayPanelHeader, Grid displayPanelMain)
        {
            displayPanelHeader.Children.Clear();
            displayPanelMain.Children.Clear();
            displayPanelHeader.Children.Add(displayerStrategy.CreateDisplayPanelPersonnel(role));
            StackPanel stackPanel = new StackPanel();
            foreach (var equipment in role.Equipment)
            {
                stackPanel.Children.Add(displayerStrategy.CreateDisplayPanelEquipment(equipment));
                stackPanel.Children.Add(CreateRemoveEquipmentButton(equipment, role.Equipment));
            }

            stackPanel.Children.Add(CreateAddEquipmentButton(role.Equipment, this.mainWindowPointer, displayerStrategy));

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            displayPanelMain.Children.Add(scrollViewer);
        }
        public Button CreatePersonnelCreatePopupButton()
        {
            Button createPersonnelButton = new Button();
            createPersonnelButton.Margin = new Thickness(5);
            createPersonnelButton.Content = "Create";
            createPersonnelButton.Width = 200;
            createPersonnelButton.HorizontalAlignment = HorizontalAlignment.Center;
            createPersonnelButton.Height = 30;
            createPersonnelButton.Click += (sender, e) => {
                PersonnelCreateWindow personnelCreateWindow = new PersonnelCreateWindow(this.mainWindowPointer);
                personnelCreateWindow.Show();
            };

            return createPersonnelButton;
        }
        public Button CreateRemovePersonnelButton(Personnel personnel, IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            Button removePersonnelButton = new Button();
            removePersonnelButton.Margin = new Thickness(5);
            removePersonnelButton.Content = "Remove";
            removePersonnelButton.Width = 100;
            removePersonnelButton.HorizontalAlignment = HorizontalAlignment.Center;
            removePersonnelButton.Height = 20;
            removePersonnelButton.Click += (sender, e) => {
                this.RemovePersonnel(personnel);
                this.DisplayAllPersonnelRoles(displayerStrategy, displayPanel);
            };

            return removePersonnelButton;
        }
        private Button CreateAddEquipmentButton(List<Equipment> equipmentList, MainWindow mainWindow, IDisplayerStrategy displayerStrategy)
        {
            Button addEquipmentButton = new Button();
            addEquipmentButton.Margin = new Thickness(5);
            addEquipmentButton.Content = "Add Equipment";
            addEquipmentButton.Width = 100;
            addEquipmentButton.HorizontalAlignment = HorizontalAlignment.Center;
            addEquipmentButton.Height = 20;
            addEquipmentButton.Click += (sender, e) => {
                AddEquipmentToEquipmentListWindow addEquipmentToEquipmentListWindow = new AddEquipmentToEquipmentListWindow(equipmentList, mainWindow, displayerStrategy);
                addEquipmentToEquipmentListWindow.Show();
            };

            return addEquipmentButton;
        }
        private Button CreateRemoveEquipmentButton(Equipment equipment, List<Equipment> equipmentList)
        {
            Button removeEquipmentButton = new Button();
            removeEquipmentButton.Margin = new Thickness(5);
            removeEquipmentButton.Content = "Remove";
            removeEquipmentButton.Width = 100;
            removeEquipmentButton.HorizontalAlignment = HorizontalAlignment.Left;
            removeEquipmentButton.Height = 20;
            removeEquipmentButton.Click += (sender, e) => {
                equipmentList.Remove(equipment);
                removeEquipmentButton.IsEnabled = false;
            };

            return removeEquipmentButton;
        }
    }
    public class RankManager
    {
        MainWindow mainWindowPointer;
        RankFactory rankFactory = new RankFactory();

        private TextBox rankNameSearchBox;
        private StackPanel searchPanelPointer;
        public RankManager(MainWindow mainWindow)
        {
            this.mainWindowPointer = mainWindow;

            this.rankNameSearchBox = new TextBox();
            this.rankNameSearchBox.TextChanged += (sender, e) =>
            {
                this.DisplayAllRanks(MilitaryApp.Instance.GetCurrentView(), MilitaryApp.Instance.TemplatesDisplayPanel);
            };

            this.rankNameSearchBox.Height = 25;
            this.rankNameSearchBox.Margin = new Thickness(10, 5, 10, 5);

        }

        public void DisplaySearchBar(Grid searchGrid)
        {
            this.searchPanelPointer?.Children.Clear();
            searchGrid.Children.Clear();
            StackPanel stackPanel = new StackPanel();


            Label rankNameLabel = new Label();
            rankNameLabel.Content = "Rank Name";
            rankNameLabel.Margin = new Thickness(10, 5, 10, 5);

            stackPanel.Children.Add(rankNameLabel);
            stackPanel.Children.Add(this.rankNameSearchBox);

            this.searchPanelPointer = stackPanel;

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            searchGrid.Children.Add(scrollViewer);
        }

        public List<Rank> FilterRanks(List<Rank> ranks)
        {
            ConcreteFilter<Rank> filter = new ConcreteFilter<Rank>();
            List<ISpecification<Rank>> specifications = new List<ISpecification<Rank>>();

            if(this.rankNameSearchBox.Text.Trim() != "")
            {
                string[] criterias = this.rankNameSearchBox.Text.Split(',');

                ChainSpecification<Rank> chainSpecification = new ChainSpecificationOr<Rank>(null);

                bool allEmptyCriterias = true;
                foreach (var criteria in criterias)
                {
                    if (criteria.Trim() != "")
                    {
                        chainSpecification.AddNextSpec(new RankNameSpecification(criteria));
                        allEmptyCriterias = false;
                    }
                }

                if (!allEmptyCriterias)
                    specifications.Add(chainSpecification);
            }

            if (specifications.Count < 1)
                return ranks;

            ChainSpecification<Rank> chainSpecificationAnd = new ChainSpecificationAnd<Rank>(null);

            foreach (var spec in specifications)
            {
                chainSpecificationAnd.AddNextSpec(spec);
            }
            return filter.Filter(ranks, chainSpecificationAnd).ToList();

        }

        public void AddRank(Rank rank)
        {
            this.rankFactory.AddNewRank(rank);
        }
        public Rank GetRank(string rankName)
        {
            return this.rankFactory.GetRank(rankName);
        }
        public Dictionary<string, Rank> GetAllRanks()
        {
            return this.rankFactory.AllRanks;
        }
        public void SetRanks(Dictionary<string, Rank> ranks)
        {
            this.rankFactory.AllRanks = ranks;
        }
        public void RemoveRank(Rank rank)
        {
            this.rankFactory.RemoveRank(rank);
        }
        public void DisplayAllRanks(IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            displayPanel.Children.Clear();
            StackPanel stackPanel = new StackPanel();
            List<Rank> ranksListFormat = new List<Rank>();
            foreach (var rank in this.rankFactory.AllRanks)
                ranksListFormat.Add(rank.Value);

            ranksListFormat = this.FilterRanks(ranksListFormat);

            var ranks = new Dictionary<string, Rank>();

            foreach (var rank in ranksListFormat)
                ranks.Add(rank.RankName, rank);

            foreach (var rank in ranks)
            {
                Grid rankGrid = displayerStrategy.CreateDisplayPanelRank(rank.Value);
                rankGrid.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => {
                    MilitaryApp.Instance.DisplayOnMain(rank.Value);
                });
                stackPanel.Children.Add(rankGrid);

                Grid buttonsManip = new Grid();
                buttonsManip.Height = 40;

                Button editButton = CreateEditRankButton(rank.Value, mainWindowPointer);

                Button removeButton = CreateRemoveRankButton(rank.Value, displayerStrategy, displayPanel);

                buttonsManip.Children.Add(editButton);
                buttonsManip.Children.Add(removeButton);

                stackPanel.Children.Add(buttonsManip);
            }
            stackPanel.Children.Add(CreateRankCreatePopupButton());
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = stackPanel;

            displayPanel.Children.Add(scrollViewer);
        }
        private Button CreateEditRankButton(Rank rank, MainWindow mainWindow)
        {
            Button editRankButton = new Button();
            editRankButton.Margin = new Thickness(5);
            editRankButton.Content = "Edit";
            editRankButton.Width = 100;
            editRankButton.HorizontalAlignment = HorizontalAlignment.Left;
            editRankButton.Height = 20;
            editRankButton.Click += (sender, e) => {
                CreateRankWindow editRankWindow = new CreateRankWindow(mainWindow, rank);
                editRankWindow.Show();
            };

            return editRankButton;
        }
        public Button CreateRankCreatePopupButton()
        {
            Button createRankButton = new Button();
            createRankButton.Margin = new Thickness(5);
            createRankButton.Content = "Create";
            createRankButton.Width = 200;
            createRankButton.HorizontalAlignment = HorizontalAlignment.Center;
            createRankButton.Height = 30;
            createRankButton.Click += (sender, e) => {
                CreateRankWindow createRankWindow = new CreateRankWindow(this.mainWindowPointer);
                createRankWindow.Show();
            };

            return createRankButton;
        }
        public Button CreateRemoveRankButton(Rank rank, IDisplayerStrategy displayerStrategy, Grid displayPanel)
        {
            Button removeRankButton = new Button();
            removeRankButton.Margin = new Thickness(5);
            removeRankButton.Content = "Remove";
            removeRankButton.Width = 100;
            removeRankButton.HorizontalAlignment = HorizontalAlignment.Right;
            removeRankButton.Height = 20;
            removeRankButton.Click += (sender, e) => {
                this.RemoveRank(rank);
                this.DisplayAllRanks(displayerStrategy, displayPanel);
            };

            return removeRankButton;
        }
        public void DisplayOnMain(Rank rank, IDisplayerStrategy displayerStrategy, Grid displayHeaderPanel, Grid displayMainPanel)
        {
            displayHeaderPanel.Children.Clear();
            displayMainPanel.Children.Clear();
            displayHeaderPanel.Children.Add(displayerStrategy.CreateDisplayPanelRank(rank));
        }
    }

    public class MilitaryApp
    {
        private MainWindow mainWindowPointer;
        private static MilitaryApp instance;
        private Grid templatesDisplayPanel;
        private Grid displayHeaderPanel;
        private Grid displayMainPanel;
        private Grid savedTemplatesPanel;
        private Grid searchBarGrid;

        public Grid TemplatesDisplayPanel { get => templatesDisplayPanel; set => templatesDisplayPanel = value; }
        public Grid DisplayHeaderPanel { get => displayHeaderPanel; set => displayHeaderPanel = value; }
        public Grid DisplayMainPanel { get => displayMainPanel; set => displayMainPanel = value; }
        public Grid SavedTemplatesPanel { get => savedTemplatesPanel; set => savedTemplatesPanel = value; }
        public Grid SearchBarPanel { get => searchBarGrid; set => searchBarGrid = value; }

        private MilitaryApp()
        {
            this.formationManager = new FormationManager(this.mainWindowPointer);
            this.equipmentManager = new EquipmentManager(this.mainWindowPointer);
            this.personnelManager = new PersonnelManager(this.mainWindowPointer);
            this.rankManager = new RankManager(this.mainWindowPointer);
            this.elementsDisplayer = new List<IDisplayerStrategy>();
            this.elementsDisplayer.Add(new ConcreteDisplayerStrategy());
            this.elementsDisplayer.Add(new ConcreteDisplayerStrategySecondView());
            this.dataManager = new SqlDataManager("Data Source = (local);Initial Catalog = MIlitaryApp;Integrated Security = True");
        }

        public static MilitaryApp Instance
        {
            get
            {
                if (MilitaryApp.instance == null)
                {
                    MilitaryApp.instance = new MilitaryApp();
                }
                return MilitaryApp.instance;
            }
        }

        public MainWindow MainWindowPointer
        {
            set
            {
                this.mainWindowPointer = value;
            }
            get
            {
                return this.mainWindowPointer;
            }
        }



        int currentView = 0;
        List<IDisplayerStrategy> elementsDisplayer;
        IDataManager dataManager;
        FormationManager formationManager;
        EquipmentManager equipmentManager;
        PersonnelManager personnelManager;
        RankManager rankManager;

        public bool SaveData()
        {
            this.dataManager.SetEquipment(this.equipmentManager.GetAllEquipment());
            this.dataManager.SetPersonnel(this.personnelManager.GetAllPersonnel());
            this.dataManager.SetRanks(this.rankManager.GetAllRanks());
            this.dataManager.SetTemplates(this.formationManager.GetAllFormationsTemplates());
            this.dataManager.SetSavedTemplates(this.formationManager.GetAllSavedFormationTemplates());
            return this.dataManager.Save();
        }
        public bool LoadData()
        {
            if(this.dataManager.Load())
            {
                this.rankManager.SetRanks(this.dataManager.GetLoadedRanks());
                this.equipmentManager.SetEquipment(this.dataManager.GetLoadedEquipment());
                this.personnelManager.SetPersonnel(this.dataManager.GetLoadedPersonnel());
                this.formationManager.SetTempalateFormations(this.dataManager.GetLoadedTemplates());
                return true;
            }
            return false;
        }
        public void ChangeView()
        {
            ++this.currentView;
            this.currentView %= this.elementsDisplayer.Count;
            this.DisplayAllSavedFormations();
            this.DisplayAllFormations();
            this.displayHeaderPanel.Children.Clear();
            this.displayMainPanel.Children.Clear();
        }
        public IDisplayerStrategy GetCurrentView()
        {
            return this.elementsDisplayer[currentView];
        }

        public void DisplayOnMain(Personnel role)
        {
            this.personnelManager.DisplayOnMain(role, this.elementsDisplayer[currentView], this.DisplayHeaderPanel, this.DisplayMainPanel);
        }


        public void DisplayOnMain(IFormation formation)
        {
            this.formationManager.DisplayOnMain(formation, this.elementsDisplayer[currentView], this.DisplayHeaderPanel, this.DisplayMainPanel, this.savedTemplatesPanel);
        }
        public void DisplayOnMain(Rank rank)
        {
            this.rankManager.DisplayOnMain(rank, this.elementsDisplayer[currentView], this.DisplayHeaderPanel, this.displayMainPanel);
        }
        public void DisplayAllEquipment()
        {
            this.equipmentManager.DisplaySearchBar(this.searchBarGrid);
            this.equipmentManager.DisplayAllEquipment(this.elementsDisplayer[currentView], this.TemplatesDisplayPanel);
        }

        public void DisplayAllFormations()
        {
            this.formationManager.DisplaySearchBar(this.SearchBarPanel);
            this.formationManager.DisplayAllFormations(this.elementsDisplayer[currentView], this.TemplatesDisplayPanel);
        }
        public void DisplayAllSavedFormations()
        {
            this.formationManager.DisplayAllSavedFormations(this.elementsDisplayer[currentView], this.savedTemplatesPanel);
        }
        public void DisplayAllRanks()
        {
            this.rankManager.DisplaySearchBar(this.SearchBarPanel);
            this.rankManager.DisplayAllRanks(this.elementsDisplayer[currentView], this.TemplatesDisplayPanel);
        }
        public void DisplayAllPersonnelRoles()
        {
            this.personnelManager.DisplaySearchBar(this.SearchBarPanel);
            this.personnelManager.DisplayAllPersonnelRoles(this.elementsDisplayer[currentView], this.TemplatesDisplayPanel);
        }
        public void AddFormation(IFormation formation)
        {
            this.formationManager.AddFormation(formation);
        }
        public void RemoveFormation(IFormation formation)
        {
            this.formationManager.RemoveFormation(formation);
        }
        public void AddEquipment(Equipment equipment)
        {
            this.equipmentManager.AddEquipment(equipment);
        }
        public void RemoveEquipment(Equipment equipment)
        {
            this.equipmentManager.RemoveEquipment(equipment);
        }
        public void AddRank(Rank rank)
        {
            this.rankManager.AddRank(rank);
        }
        public Rank GetRank(string rankName)
        {
            return this.rankManager.GetRank(rankName);
        }

        public void RemoveRank(Rank rank)
        {
            this.rankManager.RemoveRank(rank);
        }

        public void RemovePersonnel(Personnel personnel)
        {
            this.personnelManager.RemovePersonnel(personnel);
        }

        public Equipment GetEquipment(string equipmentName)
        {
            return this.equipmentManager.GetEquipment(equipmentName);
        }
        public Dictionary<string, Equipment> GetAllEquipment()
        {
            return this.equipmentManager.GetAllEquipment();
        }
        public List<IFormation> GetAllFormationTemplates()
        {
            return this.formationManager.GetAllFormationsTemplates();
        }
        public List<IFormation> GetAllSavedFormationTemplates()
        {
            return this.formationManager.GetAllSavedFormationTemplates();
        }
        public List<Personnel> GetAllPersonnel()
        {
            return this.personnelManager.GetAllPersonnel();
        }
        public void AddPersonnelRole(Personnel personnel)
        {
            this.personnelManager.AddPersonnel(personnel);
        }
    }
}
