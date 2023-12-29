using System;
using System.Collections.Generic;
using System.Text;

namespace CourseworkWPF.MilitaryFolder
{
    [Serializable]
        public class Equipment
        {
            private string s_type;
            private string s_model;
            private string s_imagePath;
            public string EquipmentType
            {
                get
                {
                    return this.s_type;
                }
                set
                {
                    this.s_type = value;
                }
            }
            public string EquipmentModel
            {
                get
                {
                    return this.s_model;
                }
                set
                {
                    this.s_model= value;
                }
            }
            public string ImagePath
            {
                get
                {
                    return this.s_imagePath;
                }
                set
                {
                    this.s_imagePath = value;
                }
            }

            private Equipment()
            {

            }
            private Equipment(string s_type, string s_model, string s_imagePath)
            {
                this.s_type = s_type;
                this.s_model = s_model;
                this.s_imagePath = s_imagePath;
            }
            public static Equipment CreateEquipment(string s_type, string s_model, string s_imagePath)
            {
                return new Equipment(s_type, s_model, s_imagePath);
            }
        }
        public class EquipmentFactory
        {
            Dictionary<string, Equipment> equipmentPieces = new Dictionary<string, Equipment>();
            public void AddNewEquipment(Equipment equipment)
            {
                equipmentPieces.Add(equipment.EquipmentModel, equipment);
            }
            public Equipment GetEquipment(string s_model)
            {
                if (!equipmentPieces.ContainsKey(s_model))
                    return null;

                return equipmentPieces[s_model];
            }
            public void RemoveEquipment(Equipment equipment)
            {
           
                this.equipmentPieces.Remove(equipment.EquipmentModel);
                equipment = null;
            }
            public Dictionary<string, Equipment> EquipmentPieces
            {
                get
                {
                    return this.equipmentPieces;
                }
                set
                {
                    this.equipmentPieces = value;
                }
            }


        }
}
