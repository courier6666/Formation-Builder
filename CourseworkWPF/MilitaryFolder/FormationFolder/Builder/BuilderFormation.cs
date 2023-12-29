using System;
using System.Collections.Generic;
using System.Text;
using CourseworkWPF.MilitaryFolder;


namespace CourseworkWPF.MilitaryFolder
{
        public interface IBuilderFormation
        {
            public IFormation GetFormation();
            public bool SetFormation(IFormation formation);
            public void SetCommanderRank(Rank commanderRank);
            public void SetHierarchyName(string hierarchyName);
            public void SetFormationName(string formationName);
            public void SetHeavyEquipment(List<Equipment> heavyEquipment);
            public void SetAdditionalEquipment(List<Equipment> additionalEquipment);
            public bool SetImage(string imagePath, string imagePathToReplace = null);
            public void Reset();
        }
        public class FormationBuilder : IBuilderFormation
        {
            FormationSocket formation = new FormationSocket(new Formation());
            public IFormation GetFormation()
            {
                IFormation returnResult = formation;
                Reset();
                return returnResult;
            }
            public bool SetFormation(IFormation formation)
            {
                IFormation original = formation;

                while(original is FormationDecorator)
                {
                    original = (original as FormationDecorator).GetWrapee();
                }

                if (!(original is Formation))
                    return false;

                if (!(formation is FormationSocket))
                    this.formation = new FormationSocket(formation);

                this.formation = formation as FormationSocket;
                return true;
            }
            public bool SetImage(string imagePath, string imagePathToReplaceWith = null)
            {
                if (imagePathToReplaceWith == null)
                {
                    formation.SetFormation(new FormationImageDecorator(this.formation.GetWrapee(), imagePath));
                    return true;
                }

                string[] request = { "replace", "image", imagePath, imagePathToReplaceWith };
                return formation.HandleRequest(request);
            }
            public void Reset()
            {
                this.formation = new FormationSocket(new Formation());
            }
            public void SetCommanderRank(Rank commanderRank)
            {
                this.formation.CommanderNeededRank = commanderRank;
            }
            public void SetHierarchyName(string hierarchyName)
            {
                this.formation.HierarchyName = hierarchyName;
            }
            public void SetFormationName(string formationName)
            {
                this.formation.FormationName = formationName;
            }

            public void SetHeavyEquipment(List<Equipment> heavyEquipment)
            {
                this.formation.HeavyEquipmentAssigned = heavyEquipment;
            }
            public void SetAdditionalEquipment(List<Equipment> additionalEquipment)
            {
                this.formation.AdditionalEquipment = additionalEquipment;
            }
        }
        public class LowestUnitBuilder : IBuilderFormation
        {
            FormationSocket lowestUnit = new FormationSocket(new LowestUnit());
            public IFormation GetFormation()
            {
                IFormation returnResult = lowestUnit;
                Reset();
                return returnResult;
            }
            public bool SetFormation(IFormation formation)
            {
                IFormation original = formation;

                while (original is FormationDecorator)
                {
                    original = (original as FormationDecorator).GetWrapee();
                }

                if (!(original is LowestUnit))
                    return false;

                if (!(formation is FormationSocket))
                    this.lowestUnit = new FormationSocket(formation);

                this.lowestUnit = formation as FormationSocket;
                return true;
            }
            public bool SetImage(string imagePath, string imagePathToReplace = null)
            {
                if (imagePathToReplace == null)
                {
                    this.lowestUnit.SetFormation(new FormationImageDecorator(this.lowestUnit.GetWrapee(), imagePath));
                    return true;
                }

                string[] request = { "replace", "image", imagePath, imagePathToReplace };
                return lowestUnit.HandleRequest(request);
            }
            public void Reset()
            {
                this.lowestUnit = new FormationSocket(new LowestUnit());
            }
            public void SetCommanderRank(Rank commanderRank)
            {
                this.lowestUnit.CommanderNeededRank = commanderRank;
            }
            public void SetHierarchyName(string hierarchyName)
            {
                this.lowestUnit.HierarchyName = hierarchyName;
            }
            public void SetFormationName(string formationName)
            {
                this.lowestUnit.FormationName = formationName;
            }
            public void SetHeavyEquipment(List<Equipment> heavyEquipment)
            {
                this.lowestUnit.HeavyEquipmentAssigned = heavyEquipment;
            }
            public void SetAdditionalEquipment(List<Equipment> additionalEquipment)
            {
                this.lowestUnit.AdditionalEquipment = additionalEquipment;
            }
        }
}
