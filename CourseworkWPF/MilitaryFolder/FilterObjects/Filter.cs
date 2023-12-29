using System;
using System.Collections.Generic;
using System.Text;

namespace CourseworkWPF.MilitaryFolder
{
    public interface ISpecification<T>
    {
        public bool IsSatisfied(T item);
    }
    public abstract class ChainSpecification<T> : ISpecification<T>
    {
        protected ISpecification<T> spec;
        protected ChainSpecification<T> _next;
        public ChainSpecification(ISpecification<T> spec)
        {
            this.spec = spec;
        }
      
        protected ChainSpecification()
        {

        }
        public abstract void AddNextSpec(ISpecification<T> spec);

        public abstract bool IsSatisfied(T item);
    }
    public class ChainSpecificationAnd<T> : ChainSpecification<T>
    {
        public ChainSpecificationAnd(ISpecification<T> spec) : base(spec)
        {

        }
        private ChainSpecificationAnd()
        {

        }
        public override void AddNextSpec(ISpecification<T> spec)
        {
            if(this.spec == null)
            {
                this.spec = spec;
                return;
            }

            if (_next == null) _next = new ChainSpecificationAnd<T>(spec);
            else _next.AddNextSpec(spec);
        }
        public override bool IsSatisfied(T item)
        {
            if (this.spec == null)
                throw new ArgumentNullException("Specification is null, not defined!");

            if (_next == null)
                return spec.IsSatisfied(item);
            return spec.IsSatisfied(item) && _next.IsSatisfied(item);
        }
    }
    public class ChainSpecificationOr<T> : ChainSpecification<T>
    {
        public ChainSpecificationOr(ISpecification<T> spec) : base(spec)
        {

        }
        private ChainSpecificationOr()
        {

        }
        public override void AddNextSpec(ISpecification<T> spec)
        {
            if (this.spec == null)
            {
                this.spec = spec;
                return;
            }

            if (_next == null) _next = new ChainSpecificationOr<T>(spec);
            else _next.AddNextSpec(spec);
        }
        public override bool IsSatisfied(T item)
        {
            if (this.spec == null)
                throw new ArgumentNullException("Specification is null, not defined!");

            if (_next == null)
                return spec.IsSatisfied(item);
            return spec.IsSatisfied(item) || _next.IsSatisfied(item);
        }
    }
    public class HierarchyNameSpecification : ISpecification<IFormation>
    {
        string hierarchyName;
        public HierarchyNameSpecification(string hierarchyName)
        {
            this.hierarchyName = hierarchyName.Trim();
        }
        private HierarchyNameSpecification()
        {

        }
        public bool IsSatisfied(IFormation item)
        {
            return item.HierarchyName.Contains(this.hierarchyName);
        }
    }
    public class FormationNameSpecification : ISpecification<IFormation>
    {
        string formationName;
        public FormationNameSpecification(string formationName)
        {
            this.formationName = formationName.Trim();
        }
        private FormationNameSpecification()
        {

        }
        public bool IsSatisfied(IFormation item)
        {

            return item.FormationName.Contains(formationName);
        }
    }
    public class CommanderRankSpecification : ISpecification<IFormation>
    {
        string commanderRankName;
        public CommanderRankSpecification(string commanderRankName)
        {
            this.commanderRankName = commanderRankName.Trim();
        }
        private CommanderRankSpecification()
        {

        }
        public bool IsSatisfied(IFormation item)
        {

            if (item.CommanderNeededRank == null || this.commanderRankName == null)
                return false;

            return item.CommanderNeededRank.RankName.Contains(this.commanderRankName);
        }
    }
    public class EquipmentModelSpecification : ISpecification<Equipment>
    {
        string equipmentModel;
        public EquipmentModelSpecification(string equipmentModel)
        {
            this.equipmentModel = equipmentModel.Trim();
        }
        private EquipmentModelSpecification()
        {

        }
        public bool IsSatisfied(Equipment item)
        {
            return item.EquipmentModel.Contains(this.equipmentModel);
        }
    }

    public class EquipmentTypeSpecification : ISpecification<Equipment>
    {
        string equipmentType;
        public EquipmentTypeSpecification(string equipmentType)
        {
            this.equipmentType = equipmentType.Trim();
        }
        private EquipmentTypeSpecification()
        {

        }
        public bool IsSatisfied(Equipment item)
        {
            return item.EquipmentType.Contains(this.equipmentType);
        }
    }
    public class RankNameSpecification : ISpecification<Rank>
    {
        string rankName;
        public RankNameSpecification(string rankName)
        {
            this.rankName = rankName.Trim();
        }
        private RankNameSpecification()
        {

        }
        public bool IsSatisfied(Rank item)
        {
            return item.RankName.Contains(this.rankName);
        }
    }
    public class PersonnelRoleSpecification : ISpecification<Personnel>
    {
        string personnelRole;
        public PersonnelRoleSpecification(string personnelRole)
        {
            this.personnelRole = personnelRole.Trim();
        }
        private PersonnelRoleSpecification()
        {

        }
        public bool IsSatisfied(Personnel item)
        {
            return item.Role.Contains(this.personnelRole);
        }
    }
    public class PersonnelRankNameSpecification : ISpecification<Personnel>
    {
        string personneRankName;
        public PersonnelRankNameSpecification(string personneRankName)
        {
            this.personneRankName = personneRankName.Trim();
        }
        private PersonnelRankNameSpecification()
        {

        }
        public bool IsSatisfied(Personnel item)
        {
            return item.PersonnelRank.RankName.Contains(this.personneRankName);
        }
    }

    public interface IFilter<T>
    {
        public IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> specification);
    }
    public class ConcreteFilter<T> : IFilter<T>
    {
        public IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> specification)
        {
            foreach (var i in items)
                if (specification.IsSatisfied(i))
                    yield return i;

        }
    }

 
}
