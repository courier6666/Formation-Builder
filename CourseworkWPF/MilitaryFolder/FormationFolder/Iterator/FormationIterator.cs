using System;
using System.Collections.Generic;
using System.Text;
using CourseworkWPF.MilitaryFolder;
using CourseworkWPF.MilitaryFolder;

namespace CourseworkWPF.MilitaryFolder
{
        public interface IAbstractIterator<T>
        {
            public T First();
            public T Next();
            public bool IsDone();
            public T Current();
        }
        public class BreadthSearchFormationIterator : IAbstractIterator<IFormation>
        {
            IFormation startingFormation;
            List<IFormation> currentFormations;
            public BreadthSearchFormationIterator(IFormation formation)
            {
                startingFormation = formation;
                currentFormations = new List<IFormation>();
            }
            private BreadthSearchFormationIterator()
            {

            }
            public IFormation Next()
            {
                return null;
            }
            public IFormation First()
            {
                currentFormations.Clear();
                currentFormations.Add(startingFormation);
                return this.startingFormation;
            }
            public IFormation Current()
            {
                return this.currentFormations[0];
            }
            public bool IsDone()
            {
                return true;
            }

        }
        public class LowestUnitIterator : IAbstractIterator<Personnel>
        {
            LowestUnit startingUnit;
            List<Personnel> currentPersonnels;
            int currentItemIndex;
            public LowestUnitIterator(LowestUnit unit)
            {
                this.currentPersonnels = new List<Personnel>();
                this.startingUnit = unit;
                if (this.startingUnit.Commander != null)
                {
                    this.currentPersonnels.Add(this.startingUnit.Commander);
                }
                foreach (var soldier in this.startingUnit.Personnels)
                {
                    this.currentPersonnels.Add(soldier);
                }

                this.startingUnit = unit;
                this.currentItemIndex = 0;
            }
            public Personnel First()
            {
                this.currentItemIndex = 0;
                return this.currentPersonnels[this.currentItemIndex];
            }
            public Personnel Current()
            {
                return this.currentPersonnels[this.currentItemIndex];
            }
            public Personnel Next()
            {
                ++this.currentItemIndex;
                Personnel personnel = this.currentPersonnels[this.currentItemIndex];
                return personnel;
            }
            public bool IsDone()
            {
                return this.currentItemIndex >= this.currentPersonnels.Count - 1;
            }
        }
}
