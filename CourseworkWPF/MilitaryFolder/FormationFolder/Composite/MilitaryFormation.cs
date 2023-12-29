using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using CourseworkWPF.MilitaryFolder;


namespace CourseworkWPF.MilitaryFolder
{
    
    public interface IFormation
    {
        public Rank CommanderNeededRank { get; set; }
        public Personnel Commander { get; set; }
        public string FormationName { get; set; }
        public string HierarchyName { get; set; }
        public List<Equipment> HeavyEquipmentAssigned { get; set; }
        public List<Equipment> AdditionalEquipment { get; set; }
        public List<Equipment> GetAllEquipment();
        public IFormation DeepClone();
        public IFormation ShallowClone();
        public List<string> GetFormationStringFormat(int tabSpace = 0);
    }
    [Serializable]
    public class Formation : IFormation
    {
        List<IFormation> formations = new List<IFormation>();
        IFormation headquarters;
        Rank commanderNeededRank;
        List<Equipment> heavyEquipmentAssigned = new List<Equipment>();
        List<Equipment> additionalEquipment = new List<Equipment>();
        public Formation()
        {

        }
        public List<string> GetFormationStringFormat(int tabSpace = 0)
        {
            string tab = "";
            for (int i = 0; i < tabSpace; ++i) tab += " ";

            List<string> stringFormat = new List<string>();
            stringFormat.Add(tab+"Formation: " + this.FormationName + " " + this.HierarchyName);
            stringFormat.Add(tab+"Headquarters");

            List<string> HQStringFormat = this.headquarters.GetFormationStringFormat();
            foreach(var s in HQStringFormat)
            {
                stringFormat.Add(s);
            }
            stringFormat.Add(tab+"Headquarters End");

            stringFormat.Add(tab+"Subordinate Formations");

            foreach(var subForm in this.formations)
            {
                foreach(var s in subForm.GetFormationStringFormat(tabSpace + 1))
                {
                    stringFormat.Add(s);
                }
            }

            stringFormat.Add(tab + "Subordinate Formations End");

            stringFormat.Add(tab + "HeavyEquipment");
            foreach(var equipment in this.HeavyEquipmentAssigned)
            {
                stringFormat.Add(equipment.EquipmentModel + " " + equipment.EquipmentType + " " + equipment.ImagePath);
            }
            stringFormat.Add(tab + "HeavyEquipment End");

            stringFormat.Add(tab + "AdditonalEquipment");
            foreach (var equipment in this.HeavyEquipmentAssigned)
            {
                stringFormat.Add(equipment.EquipmentModel + " " + equipment.EquipmentType + " " + equipment.ImagePath);
            }
            stringFormat.Add(tab + "AdditonalEquipment End");

            

            stringFormat.Add(tab+"Formation End");
            return stringFormat;
        }
        public Formation(IFormation headquarters)
        {
            this.headquarters = headquarters;
        }
        private Formation(Formation formation, bool b_DeepClone)
        {
            this.commanderNeededRank = formation.CommanderNeededRank;
            this.FormationName = formation.FormationName;
            this.HierarchyName = formation.HierarchyName;
            this.heavyEquipmentAssigned = new List<Equipment>(formation.heavyEquipmentAssigned);
            this.additionalEquipment = new List<Equipment>(formation.additionalEquipment);
            if (b_DeepClone)
            {

                this.headquarters = formation.headquarters.DeepClone();
                foreach (var element in formation.formations)
                {
                    this.formations.Add(element.DeepClone());
                }
            }
            else
            {
                this.headquarters = formation.headquarters;
                foreach (var element in formation.formations)
                {
                    this.formations.Add(element);
                }
            }
        }
        public IFormation DeepClone()
        {
            try
            {

                using (var stream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Seek(0, SeekOrigin.Begin);
                    object copy = formatter.Deserialize(stream);
                    return (Formation)copy;
                }
            } catch (Exception ex)
            {
                throw;
            }
        }
        public IFormation ShallowClone()
        {
            return new Formation(this, false);
        }
        public void AddFormation(IFormation formation)
        {
            if(formation!=this)
                this.formations.Add(formation);
        }
        public void RemoveFormation(IFormation formation)
        {
            this.formations.Remove(formation);
        }

        public List<Equipment> GetAllEquipment()
        {
            List<Equipment> equipmentPieces = new List<Equipment>();
            if (this.headquarters != null)
            {
                foreach (var equipment in this.headquarters.GetAllEquipment())
                {
                    equipmentPieces.Add(equipment);
                }
            }

            foreach (var equipment in this.heavyEquipmentAssigned)
            {
                equipmentPieces.Add(equipment);
            }
            foreach (var equipment in this.additionalEquipment)
            {
                equipmentPieces.Add(equipment);
            }

            foreach (var formation in this.formations)
            {
                foreach (var equipment in formation.GetAllEquipment())
                {
                    equipmentPieces.Add(equipment);
                }
            }

            return equipmentPieces;
        }
        public List<Equipment> HeavyEquipmentAssigned
        {
            get
            {
                return this.heavyEquipmentAssigned;
            }
            set
            {
                this.heavyEquipmentAssigned = value;
            }
        }
        public List<Equipment> AdditionalEquipment
        {
            get
            {
                return this.additionalEquipment;
            }
            set
            {
                this.additionalEquipment = value;
            }
        }
        public Rank CommanderNeededRank
        {
            get
            {
                return this.commanderNeededRank;
            }
            set
            {
                this.commanderNeededRank = value;
            }
        }
        public Personnel Commander
        {
            get
            {
                return headquarters.Commander;
            }
            set
            {
                if (value.PersonnelRank.RankValue < this.commanderNeededRank.RankValue)
                    this.headquarters.Commander = value;
            }
        }
        public string FormationName { get; set; }
        public string HierarchyName { get; set; }
        public IFormation HeadquartersFormation
        {
            get
            {
                return this.headquarters;
            }
            set
            {
                this.headquarters = value;
            }
        }
        public List<IFormation> SubordinateFormations
        {
            get
            {
                return this.formations;
            }
            set
            {

            }
        }
    }

    [Serializable]
    public class LowestUnit : IFormation
    {
        List<Personnel> soldiers = new List<Personnel>();
        List<Equipment> heavyEquipmentAssigned = new List<Equipment>();
        List<Equipment> additionalEquipment = new List<Equipment>();
        Personnel leader = null;
        Rank commanderNeededRank;
        public LowestUnit()
        {
            leader = null;
        }
        public LowestUnit(Personnel leader)
        {
            this.leader = leader;
        }
        private LowestUnit(LowestUnit lowestUnit, bool b_DeepClone)
        {
            this.commanderNeededRank = lowestUnit.commanderNeededRank;
            this.FormationName = lowestUnit.FormationName;
            this.HierarchyName = lowestUnit.HierarchyName;
            this.heavyEquipmentAssigned = new List<Equipment>(lowestUnit.heavyEquipmentAssigned);
            this.additionalEquipment = new List<Equipment>(lowestUnit.additionalEquipment);
            if (b_DeepClone)
            {
                this.leader = lowestUnit.leader.Clone();
                foreach (var element in lowestUnit.soldiers)
                {
                    this.soldiers.Add(element.Clone());
                }
            }
            else
            {
                this.leader = lowestUnit.leader;
                foreach (var element in lowestUnit.soldiers)
                {
                    this.soldiers.Add(element);
                }
            }
        }

        public IFormation SuperiorFormation { get; set; }
        public void AddPersonnel(Personnel personnel)
        {
            this.soldiers.Add(personnel);
        }
        public void RemovePersonnel(Personnel personnel)
        {
            this.soldiers.Remove(personnel);
        }
        public IFormation DeepClone()
        {
            return new LowestUnit(this, true);
        }
        public IFormation ShallowClone()
        {
            return new LowestUnit(this, false);
        }
        public List<Equipment> HeavyEquipmentAssigned
        {
            get
            {
                return this.heavyEquipmentAssigned;
            }
            set
            {
                this.heavyEquipmentAssigned = value;
            }
        }
        public List<Equipment> AdditionalEquipment
        {
            get
            {
                return this.additionalEquipment;
            }
            set
            {
                this.additionalEquipment = value;
            }
        }
        public List<Equipment> GetAllEquipment()
        {
            List<Equipment> equipmentPieces = new List<Equipment>();

            if (this.leader != null)
            {
                foreach (var equipment in this.leader.Equipment)
                {
                    equipmentPieces.Add(equipment);
                }
            }

            foreach (var equipment in this.heavyEquipmentAssigned)
            {
                equipmentPieces.Add(equipment);
            }
            foreach (var equipment in this.additionalEquipment)
            {
                equipmentPieces.Add(equipment);
            }

            foreach (var pesonnel in this.soldiers)
            {
                foreach (var equipment in pesonnel.Equipment)
                {
                    equipmentPieces.Add(equipment);
                }
            }
            return equipmentPieces;
        }
        public Rank CommanderNeededRank
        {
            get
            {
                return this.commanderNeededRank;
            }
            set
            {
                this.commanderNeededRank = value;
            }
        }
        public Personnel Commander
        {
            get
            {
                return this.leader;
            }
            set
            {
                this.leader = value;
            }
        }

        public string FormationName { get; set; }
        public string HierarchyName { get; set; }
        public List<Personnel> Personnels
        {
            get
            {
                return this.soldiers;
            }
        }

        public List<string> GetFormationStringFormat(int tabSpace = 0)
        {
            List<string> stringFormat = new List<string>();
            string tab = "";
            for (int i = 0; i < tabSpace; ++i) tab += " ";

            stringFormat.Add(tab+"Lowest Unit: " + this.FormationName + " " + this.HierarchyName);
            stringFormat.Add(tab+"Commander");

            stringFormat.Add(tab + this.leader.Role);

            stringFormat.Add(tab+"Commander End");

            stringFormat.Add(tab+"Subordinate Personnel");

            foreach (var personnel in this.soldiers)
            {
                stringFormat.Add(tab + personnel.Role);
            }

            stringFormat.Add(tab + "Subordinate Personnel End");

            stringFormat.Add(tab + "HeavyEquipment");
            foreach (var equipment in this.HeavyEquipmentAssigned)
            {
                stringFormat.Add(tab + equipment.EquipmentModel + " " + equipment.EquipmentType + " " + equipment.ImagePath);
            }
            stringFormat.Add(tab + "HeavyEquipment End");

            stringFormat.Add(tab + "AdditonalEquipment");
            foreach (var equipment in this.HeavyEquipmentAssigned)
            {
                stringFormat.Add(tab + equipment.EquipmentModel + " " + equipment.EquipmentType + " " + equipment.ImagePath);
            }
            stringFormat.Add(tab + "AdditonalEquipment End");



            stringFormat.Add(tab + "Lowest Unit End");
            return stringFormat;
        }

    }
}
