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
        public interface IHandler
        {
            public abstract bool HandleRequest(string[] request);
        }
        [Serializable]
        public abstract class FormationDecorator : IFormation, IHandler
        {
            protected IFormation wrapee;
            public FormationDecorator(IFormation formation)
            {
                this.wrapee = formation;
            }
            public FormationDecorator()
            {

            }
            public IFormation GetWrapee()
            {
                return this.wrapee;
            }
            public Rank CommanderNeededRank
            {
                get
                {
                    return wrapee.CommanderNeededRank;
                }
                set
                {
                    wrapee.CommanderNeededRank = value;
                }
            }
            public Personnel Commander
            {
                get
                {
                    return this.wrapee.Commander;
                }
                set
                {
                    this.wrapee.Commander = value;
                }
            }
            public string FormationName
            {
                get
                {
                    return this.wrapee.FormationName;
                }
                set
                {
                    this.wrapee.FormationName = value;
                }
            }
            public string HierarchyName
            {
                get
                {
                    return this.wrapee.HierarchyName;
                }
                set
                {
                    this.wrapee.HierarchyName = value;
                }
            }

            public virtual IFormation DeepClone()
            {
                return wrapee.DeepClone();
            }
            public virtual IFormation ShallowClone()
            {
                return wrapee.ShallowClone();
            }

            public List<Equipment> HeavyEquipmentAssigned
            {
                get
                {
                    return wrapee.HeavyEquipmentAssigned;
                }
                set
                {
                    wrapee.HeavyEquipmentAssigned = value;
                }
            }
            public List<Equipment> AdditionalEquipment
            {
                get
                {
                    return this.wrapee.AdditionalEquipment;
                }
                set
                {
                    this.wrapee.AdditionalEquipment = value;
                }
            }
            public List<Equipment> GetAllEquipment()
            {
                return this.wrapee.GetAllEquipment();
            }
            public abstract Grid CreateFormationPanel(IDisplayerStrategy displayerStrategy, Grid prevGrid = null);
            public abstract bool HandleRequest(string[] request);
            public abstract List<string> GetFormationStringFormat(int tabSpace = 0);
    }
    [Serializable]
    public class FormationImageDecorator : FormationDecorator
    {
        string imageFilePath = null;
        public FormationImageDecorator(IFormation formation, string imageFilePath)
        {
            this.wrapee = formation;
            this.imageFilePath = imageFilePath;
        }
        public string GetImageFilePath()
        {
            return this.imageFilePath;
        }
        public void ChangeImageFilePath(string imageFilePath)
        {
            this.imageFilePath = imageFilePath;
        }
        public override IFormation DeepClone()
        {
            IFormation formationCopy = this.wrapee.DeepClone();
            return new FormationImageDecorator(formationCopy, this.imageFilePath);
        }
        public override IFormation ShallowClone()
        {
            IFormation formationCopy = this.wrapee.ShallowClone();
            return new FormationImageDecorator(formationCopy, this.imageFilePath);
        }
        public override Grid CreateFormationPanel(IDisplayerStrategy displayerStrategy, Grid prevGrid = null)
        {
            return displayerStrategy.CreateDisplayPanelFormation(this, prevGrid);
        }
        public override bool HandleRequest(string[] request)
        {

            if (request.Length <= 0)
            {
                if (this.wrapee is FormationDecorator)
                {
                    return (this.wrapee as FormationDecorator).HandleRequest(request);
                }
                return false;
            }

            if (request[1] != "image")
            {
                if (this.wrapee is FormationDecorator)
                {
                    return (this.wrapee as FormationDecorator).HandleRequest(request);
                }
                return false;
            }
            if (request[0] == "replace")
            {
                if (request[2] == this.imageFilePath)
                {
                    this.imageFilePath = request[3];
                    return true;
                }
                if (this.wrapee is FormationDecorator)
                {
                    return (this.wrapee as FormationDecorator).HandleRequest(request);
                }
                return false;
            }
            return false;
        }
        public override List<string> GetFormationStringFormat(int tabSpace = 0)
        {
            string tab = "";
            for (int i = 0; i < tabSpace; ++i) tab += " ";
            List<string> stringFormat = new List<string>();
            stringFormat.Add(tab + "Image - " + this.imageFilePath);
            foreach(var s in this.wrapee.GetFormationStringFormat(tabSpace + 1))
            {
                stringFormat.Add(s);
            }
            return stringFormat;
        }
    }
    [Serializable]
    public class FormationSocket : FormationDecorator
    {
        public FormationSocket(IFormation formation) : base(formation)
        {

        }
        public void SetFormation(IFormation formation)
        {
            this.wrapee = formation;
        }
        public override IFormation DeepClone()
        {
            IFormation formationCopy = this.wrapee.DeepClone();
            return new FormationSocket(formationCopy);
        }
        public override IFormation ShallowClone()
        {
            IFormation formationCopy = this.wrapee.ShallowClone();
            return new FormationSocket(formationCopy);
        }
        public override Grid CreateFormationPanel(IDisplayerStrategy displayerStrategy, Grid prevGrid = null)
        {
            return displayerStrategy.CreateDisplayPanelFormation(this, prevGrid);
        }
        public override bool HandleRequest(string[] request)
        {
            if (this.wrapee is FormationDecorator)
                return (this.wrapee as FormationDecorator).HandleRequest(request);
            return false;
        }
        public override List<string> GetFormationStringFormat(int tabSpace = 0)
        {
            return this.GetWrapee().GetFormationStringFormat(tabSpace);
        }
    }
}
