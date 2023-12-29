using System;
using System.Collections.Generic;
using System.Text;
using CourseworkWPF.MilitaryFolder;

namespace CourseworkWPF.MilitaryFolder
{
    [Serializable]
        public class Personnel
        {
            private Rank m_rank;
            private List<Equipment> m_equipment = new List<Equipment>();
            private string s_role;
            public Rank PersonnelRank
            {
                get
                {
                    return this.m_rank;
                }
                set
                {
                    this.m_rank = value;
                }
            }
            public List<Equipment> Equipment 
            {
                get
                {
                    return this.m_equipment;
                }
                set
                {
                    this.m_equipment = value;
                }
            }
            public string Role
            {
                get
                {
                    return this.s_role;
                }
                set
                {
                    this.s_role = value;
                }
            }
            public Personnel(Personnel personnel)
            {
                this.m_rank = personnel.m_rank;
                this.m_equipment = new List<Equipment>(personnel.m_equipment);
                this.s_role = personnel.s_role;
            }
            public Personnel()
            {

            }
            public Personnel Clone()
            {
                return new Personnel(this);
            }

        }
}
