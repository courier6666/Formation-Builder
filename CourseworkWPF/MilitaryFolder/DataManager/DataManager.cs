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
using System.IO;
using System.Data.SqlClient;

namespace CourseworkWPF.MilitaryFolder
{
    public interface IDataManager
    {
        public bool Save();
        public bool Load();
        public void SetRanks(Dictionary<string, Rank> ranks);
        public void SetEquipment(Dictionary<string, Equipment> equipmentPieces);
        public void SetPersonnel(List<Personnel> personnels);
        public void SetSavedTemplates(List<IFormation> formations);
        public void SetTemplates(List<IFormation> formations);

        public Dictionary<string, Rank> GetLoadedRanks();
        public Dictionary<string, Equipment> GetLoadedEquipment();
        public List<Personnel> GetLoadedPersonnel();
        public List<IFormation> GetLoadedTemplates();
    }

    public class SqlDataManager : IDataManager
    {
        string connectionString;
        Dictionary<string, Rank> ranks;
        Dictionary<string, Equipment> equipmentPieces;
        List<Personnel> personnels;
        List<IFormation> savedFormations;
        List<IFormation> templates;
        
        public SqlDataManager(string connectionString)
        {
            //connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            this.connectionString = connectionString;

        }

        private void DeleteAllTableData(SqlConnection connection, SqlTransaction transaction)
        {
            string query = "";

            query += $"delete from FormationToFormation;\n";
            query += $"delete from lowestUnitToFormation;\n";
            query += $"delete from personnelToLowestUnit;\n";
            query += $"delete from EquipmentToPersonnel;\n";
            query += $"delete from additionalEquipmentToLowestUnit;\n";
            query += $"delete from additionalEquipmentToFormation;\n";
            query += $"delete from heavyEquipmentToFormation;\n";
            query += $"delete from heavyEquipmentToLowestUnit;\n";
            query += $"delete from lowestUnitHQToFormation;\n";
            query += $"delete from FormationHQToFormation;\n";

            query += $"delete from formation;\n";
            query += $"delete from lowestUnit;\n";
            query += $"delete from personnel;\n";
            query += $"delete from ranks;\n";
            query += "delete from equipmentTable;\n";


            using (SqlCommand sqlCommand = new SqlCommand(query, connection, transaction))
            {
                sqlCommand.ExecuteNonQuery();
            }
        }
        private void SaveRanks(SqlConnection connection, SqlTransaction transaction)
        {
            string query = "";
            int i = 1;
            foreach(var rank in this.ranks)
            {
                query += $"insert into ranks values ({i++}, '{rank.Value.RankName}', {((int)rank.Value.Type) + 1}, {rank.Value.RankValue}, '{rank.Value.ImagePath}');\n";
            }

            using (SqlCommand sqlCommand = new SqlCommand(query, connection, transaction))
            {
                sqlCommand.ExecuteNonQuery();
            }
        }
        private void SaveEquipment(SqlConnection connection, SqlTransaction transaction)
        {
            string query = "";
            int i = 1;
            foreach(var equipment in this.equipmentPieces)
            {
                query += $"insert into equipmentTable values({i++}, '{equipment.Value.EquipmentModel}', '{equipment.Value.EquipmentType}', '{equipment.Value.ImagePath}');\n";
            }

            using (SqlCommand sqlCommand = new SqlCommand(query, connection, transaction))
            {
                sqlCommand.ExecuteNonQuery();
            }
        }
        private void GetAllUniqueFormations(IFormation formation, List<IFormation> uniqueFormations)
        {
            IFormation original = formation;
            while(original is FormationDecorator)
            {
                original = (original as FormationDecorator).GetWrapee();
            }

            if (!uniqueFormations.Contains(formation))
                uniqueFormations.Add(formation);

            if(original is Formation)
            {

                if ((original as Formation).HeadquartersFormation != null)
                    GetAllUniqueFormations((original as Formation).HeadquartersFormation, uniqueFormations);

                foreach(var subFormation in (original as Formation).SubordinateFormations)
                {
                    GetAllUniqueFormations(subFormation, uniqueFormations);
                }

            }
        }
        private void SaveTemplateFormations(SqlConnection connection, SqlTransaction transaction)
        {
            string query = "";

            List<IFormation> allUniqueFormations = new List<IFormation>();

            Dictionary<IFormation, int> formationToID = new Dictionary<IFormation, int>();

            foreach (var formation in this.templates)
            {
                allUniqueFormations.Add(formation);

            }

            foreach(var formation in this.templates)
            {
                GetAllUniqueFormations(formation, allUniqueFormations);
            }

            int i = 1;
            foreach(var formation in allUniqueFormations)
            {
                formationToID.Add(formation, i++);
            }

            foreach(var formation in allUniqueFormations)
            {
                IFormation original = formation;
                string imagePath = "";
                while (original is FormationDecorator)
                {
                    if(original is FormationImageDecorator)
                    {
                        imagePath = (original as FormationImageDecorator).GetImageFilePath();
                    }
                    original = (original as FormationDecorator).GetWrapee();
                }

                string selectRankIDquery = $"select ID from ranks WHERE rankName = '{formation.CommanderNeededRank.RankName}';\n";
                int rankID = -1;

                using (SqlCommand sqlCommandGetRankID = new SqlCommand(selectRankIDquery, connection, transaction))
                {
                    using (SqlDataReader reader = sqlCommandGetRankID.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rankID = reader.GetInt32(0);
                        }
                    }
                }

                if (original is Formation)
                {
                    query += $"insert into formation values ({formationToID[formation]}, '{original.FormationName}', '{original.HierarchyName}', {rankID}, '{imagePath}');\n";

                    foreach(var addEquipment in original.AdditionalEquipment)
                    {
                        string selectEquipmentID = $"select ID from equipmentTable where equipmentModel = '{addEquipment.EquipmentModel}';\n";
                        int equipmentID = -1;
                        using (SqlCommand sqlCommandEquipmentID = new SqlCommand(selectEquipmentID, connection, transaction))
                        {
                            using (SqlDataReader reader = sqlCommandEquipmentID.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    equipmentID = reader.GetInt32(0);
                                }
                            }
                        }
                        query += $"insert into additionalEquipmentToFormation values ({formationToID[formation]}, {equipmentID});\n";
                    }
                    foreach(var heavyEquipment in original.HeavyEquipmentAssigned)
                    {
                        string selectEquipmentID = $"select ID from equipmentTable where equipmentModel = '{heavyEquipment.EquipmentModel}';\n";
                        int equipmentID = -1;
                        using (SqlCommand sqlCommandEquipmentID = new SqlCommand(selectEquipmentID, connection, transaction))
                        {
                            using (SqlDataReader reader = sqlCommandEquipmentID.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    equipmentID = reader.GetInt32(0);
                                }
                            }
                        }
                        query += $"insert into heavyEquipmentToFormation values ({formationToID[formation]}, {equipmentID});\n";
                    }
                }
                else if(original is LowestUnit)
                {
                    int commanderPersonnelID = -1;
                    string selectCommanderIDquery = $"select ID from personnel where personnelRole = '{(original as LowestUnit).Commander.Role}';\n";
                    using (SqlCommand sqlCommandGetRankID = new SqlCommand(selectCommanderIDquery, connection, transaction))
                    {
                        using (SqlDataReader reader = sqlCommandGetRankID.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                commanderPersonnelID = reader.GetInt32(0);
                            }
                        }
                    }
                    query += $"insert into lowestUnit values({formationToID[formation]}, {commanderPersonnelID}, '{original.FormationName}', '{original.HierarchyName}', {rankID}, '{imagePath}');\n";

                    foreach (var addEquipment in original.AdditionalEquipment)
                    {
                        string selectEquipmentID = $"select ID from equipmentTable where equipmentModel = '{addEquipment.EquipmentModel}';\n";
                        int equipmentID = -1;
                        using (SqlCommand sqlCommandEquipmentID = new SqlCommand(selectEquipmentID, connection, transaction))
                        {
                            using (SqlDataReader reader = sqlCommandEquipmentID.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    equipmentID = reader.GetInt32(0);
                                }
                            }
                        }
                        query += $"insert into additionalEquipmentToLowestUnit values ({formationToID[formation]}, {equipmentID});\n";
                    }

                    foreach (var heavyEquipment in original.HeavyEquipmentAssigned)
                    {
                        string selectEquipmentID = $"select ID from equipmentTable where equipmentModel = '{heavyEquipment.EquipmentModel}';\n";
                        int equipmentID = -1;
                        using (SqlCommand sqlCommandEquipmentID = new SqlCommand(selectEquipmentID, connection, transaction))
                        {
                            using (SqlDataReader reader = sqlCommandEquipmentID.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    equipmentID = reader.GetInt32(0);
                                }
                            }
                        }
                        query += $"insert into heavyEquipmentToLowestUnit values ({formationToID[formation]}, {equipmentID});\n";
                    }
                }
            }

            foreach(var formation in allUniqueFormations)
            {
                IFormation original = formation;
                while (original is FormationDecorator)
                {
                    original = (original as FormationDecorator).GetWrapee();
                }

                if(original is Formation)
                {
                    IFormation originalHQ = (original as Formation).HeadquartersFormation;
                    if (originalHQ != null)
                    {
                        while (originalHQ is FormationDecorator)
                        {
                            originalHQ = (originalHQ as FormationDecorator).GetWrapee();
                        }

                        if (originalHQ is Formation)
                        {
                            query += $"insert into FormationHQToFormation values({formationToID[formation]}, {formationToID[(original as Formation).HeadquartersFormation]});\n"; ;
                        }
                        else if (originalHQ is LowestUnit)
                        {
                            query += $"insert into lowestUnitHQToFormation values({formationToID[formation]}, {formationToID[(original as Formation).HeadquartersFormation]});\n";
                        }
                    }
                    foreach(var subFormation in (original as Formation).SubordinateFormations)
                    {
                        IFormation subOriginal = subFormation;
                        while(subOriginal is FormationDecorator)
                        {
                            subOriginal = (subOriginal as FormationDecorator).GetWrapee();
                        }
                        if(subOriginal is Formation)
                        {
                            query += $"insert into FormationToFormation values({formationToID[formation]}, {formationToID[subFormation]});\n";
                        }
                        else if(subOriginal is LowestUnit)
                        {
                            query += $"insert into lowestUnitToFormation values({formationToID[formation]}, {formationToID[subFormation]});\n";
                        }
                    }
                }
                else if(original is LowestUnit)
                {
                    foreach(var personnel in (original as LowestUnit).Personnels)
                    {
                        string selectedPersonnelID = $"select ID from personnel where personnelRole = '{personnel.Role}';\n";
                        int personnelID = -1;

                        using (SqlCommand sqlCommandEquipmentID = new SqlCommand(selectedPersonnelID, connection, transaction))
                        {
                            using (SqlDataReader reader = sqlCommandEquipmentID.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    personnelID = reader.GetInt32(0);
                                }
                            }
                        }

                        query += $"insert into personnelToLowestUnit values({formationToID[formation]}, {personnelID});\n";
                    }

                }
            }

            using (SqlCommand sqlCommand = new SqlCommand(query, connection, transaction))
            {
                sqlCommand.ExecuteNonQuery();
            }
        }
        private void SavePersonnel(SqlConnection connection, SqlTransaction transaction)
        {
            string query = "";
            int i = 1;
            foreach(var personnel in this.personnels)
            {
                string selectRankIDquery = $"select ID from ranks WHERE rankName = '{personnel.PersonnelRank.RankName}';\n";
                int rankID = -1;

                using (SqlCommand sqlCommandGetRankID = new SqlCommand(selectRankIDquery, connection, transaction))
                {
                    using(SqlDataReader reader = sqlCommandGetRankID.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            rankID = reader.GetInt32(0);
                        }
                    }
                }

                query += $"insert into personnel values({i}, '{personnel.Role}', {rankID});\n";
                foreach(var equipment in personnel.Equipment)
                {
                    string selectEquipmentIDquery = $"select ID from equipmentTable WHERE equipmentModel = '{equipment.EquipmentModel}';\n";
                    int equipmentID = -1;

                    using (SqlCommand sqlCommandGetEquipmentID = new SqlCommand(selectEquipmentIDquery, connection, transaction))
                    {
                        using (SqlDataReader reader = sqlCommandGetEquipmentID.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                equipmentID = reader.GetInt32(0);
                            }
                        }
                    }
                    query += $"insert into EquipmentToPersonnel values({i}, {equipmentID});\n";

                }
                ++i;
            }
            using (SqlCommand sqlCommand = new SqlCommand(query, connection, transaction))
            {
                sqlCommand.ExecuteNonQuery();
            }
        }
        public bool Save()
        {
            
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        DeleteAllTableData(connection, transaction);
                        SaveRanks(connection, transaction);
                        SaveEquipment(connection, transaction);
                        SavePersonnel(connection, transaction);
                        SaveTemplateFormations(connection, transaction);

                        transaction.Commit();
                        connection.Close();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Error saving data! Message: \n" + ex.Message);
                        transaction.Rollback();
                        connection.Close();
                        return false;
                    }
                }
                
            }
            return true;

        }

        public void LoadRanks(SqlConnection connection, SqlTransaction transaction)
        {
            this.ranks = new Dictionary<string, Rank>();
            string selectRanksQuery = "select * from ranks;\n";
            using (SqlCommand sqlCommand = new SqlCommand(selectRanksQuery, connection, transaction))
            {
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        Rank newRank = Rank.CreateRank((RankType)(reader.GetInt32(2) - 1), reader.GetString(1), reader.GetInt32(3), reader.GetString(4));
                        this.ranks.Add(newRank.RankName, newRank);
                    }
                }
            }
        }
        public void LoadEquipment(SqlConnection connection, SqlTransaction transaction)
        {
            this.equipmentPieces = new Dictionary<string, Equipment>();
            string selectEquipmentQuery = "select * from equipmentTable;\n";
            using (SqlCommand command = new SqlCommand(selectEquipmentQuery, connection, transaction))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        Equipment newEquipment = Equipment.CreateEquipment(reader.GetString(2), reader.GetString(1), reader.GetString(3));
                        this.equipmentPieces.Add(newEquipment.EquipmentModel, newEquipment);
                    }
                }
            }
        }
        public void LoadPersonnel(SqlConnection connection, SqlTransaction transaction)
        {
            Dictionary<Personnel, int> personnelToID = new Dictionary<Personnel, int>();
            this.personnels = new List<Personnel>();
            string selectPersonnelQuery = "select personnel.ID, personnel.personnelRole, ranks.rankName from personnel left join ranks on personnel.rankID = ranks.ID;\n";
            using (SqlCommand command = new SqlCommand(selectPersonnelQuery, connection, transaction))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Personnel newPersonnel = new Personnel();
                        newPersonnel.Role = reader.GetString(1);
                        newPersonnel.PersonnelRank = this.ranks[reader.GetString(2)];
                        personnelToID.Add(newPersonnel, reader.GetInt32(0));
                        this.personnels.Add(newPersonnel);
                    }
                }
            }

            string selectEquipmentAssignedToPersonnel = "select personnel.ID, equipmentTable.equipmentModel\n" +
            "from personnel\n" +
            "left JOIN EquipmentToPersonnel ON personnel.ID = EquipmentToPersonnel.personnelID\n" +
            "left JOIN equipmentTable ON EquipmentToPersonnel.equipmentID = equipmentTable.ID;\n";

            using (SqlCommand personnelEquipmentCommand = new SqlCommand(selectEquipmentAssignedToPersonnel, connection, transaction))
            {
                using (SqlDataReader equipmentReader = personnelEquipmentCommand.ExecuteReader())
                {
                    while (equipmentReader.Read())
                    {
                        this.personnels[equipmentReader.GetInt32(0) - 1].Equipment.Add(this.equipmentPieces[equipmentReader.GetString(1)]);
                    }
                }
            }

        }

        public void LoadTemplateFormations(SqlConnection connection, SqlTransaction transaction)
        {
            Dictionary<IFormation, int> formationToID = new Dictionary<IFormation, int>();
            string getFormationsCountQuery = "select count(*) from (select ID from formation UNION select ID from lowestUnit) as combined_formations;";

            using (SqlCommand command = new SqlCommand(getFormationsCountQuery, connection, transaction))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        int count = reader.GetInt32(0);
                        this.templates = new List<IFormation>(count);
                        while (count-- > 0)
                            this.templates.Add(null);
                    }
                }
            }

            string getLowestUnitsQuery = "select lowestUnit.ID, lowestUnit.commanderID, lowestUnit.formationName, lowestUnit.hierarchyName, ranks.rankName, lowestUnit.imagePath from lowestUnit left join ranks on lowestUnit.commanderRankID = ranks.ID;";

            using (SqlCommand command = new SqlCommand(getLowestUnitsQuery, connection, transaction))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LowestUnit lowestUnit = new LowestUnit();
                        lowestUnit.Commander = this.personnels[reader.GetInt32(1) - 1];
                        lowestUnit.FormationName = reader.GetString(2);
                        lowestUnit.HierarchyName = reader.GetString(3);
                        lowestUnit.CommanderNeededRank = this.ranks[reader.GetString(4)];

                        IFormation lowestUnitDecorateed = new FormationImageDecorator(lowestUnit, reader.GetString(5));
                        lowestUnitDecorateed = new FormationSocket(lowestUnitDecorateed);
                        this.templates[reader.GetInt32(0) - 1] = lowestUnitDecorateed;
                        formationToID.Add(lowestUnitDecorateed, reader.GetInt32(0));
                    }
                }
            }

            string getFormationsQuery = "select formation.ID, formation.formationName, formation.hierarchyName, ranks.rankName, formation.imagePath from formation left join ranks on formation.commanderRankID = ranks.ID;";

            using (SqlCommand command = new SqlCommand(getFormationsQuery, connection, transaction))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Formation formation = new Formation();
                        formation.FormationName = reader.GetString(1);
                        formation.HierarchyName = reader.GetString(2);
                        formation.CommanderNeededRank = this.ranks[reader.GetString(3)];

                        IFormation formationDecorated = new FormationImageDecorator(formation, reader.GetString(4));
                        formationDecorated = new FormationSocket(formationDecorated);
                        this.templates[reader.GetInt32(0) - 1] = formationDecorated;
                        formationToID.Add(formationDecorated, reader.GetInt32(0));
                    }
                }
            }

            foreach(var formation in this.templates)
            {
                IFormation original = formation;
                while(original is FormationDecorator)
                {
                    original = (original as FormationDecorator).GetWrapee();
                }

                if(original is LowestUnit)
                {
                    string selectHeavyEquipmentAssignedQuery = $"select equipmentTable.equipmentModel, heavyEquipmentToLowestUnit.lowestUnitID from EquipmentTable inner join heavyEquipmentToLowestUnit on equipmentTable.ID = heavyEquipmentToLowestUnit.equipmentID where heavyEquipmentToLowestUnit.lowestUnitID = {formationToID[formation]};";
                    using (SqlCommand command = new SqlCommand(selectHeavyEquipmentAssignedQuery, connection, transaction))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                formation.HeavyEquipmentAssigned.Add(this.equipmentPieces[reader.GetString(0)]);
                            }
                        }
                    }

                    string selectAdditionalEquipmentQuery = $"select equipmentTable.equipmentModel, additionalEquipmentToLowestUnit.lowestUnitID from EquipmentTable inner join additionalEquipmentToLowestUnit on equipmentTable.ID = additionalEquipmentToLowestUnit.equipmentID where additionalEquipmentToLowestUnit.lowestUnitID = {formationToID[formation]};";
                    using (SqlCommand command = new SqlCommand(selectAdditionalEquipmentQuery, connection, transaction))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                formation.AdditionalEquipment.Add(this.equipmentPieces[reader.GetString(0)]);
                            }
                        }
                    }

                    string selectAssignedPersonnelQuery = $"select lowestUnit.ID, personnelToLowestUnit.personnelID from lowestUnit inner join personnelToLowestUnit on personnelToLowestUnit.lowestUnitID = lowestUnit.ID where lowestUnit.ID = {formationToID[formation]};";
                    using (SqlCommand command = new SqlCommand(selectAssignedPersonnelQuery, connection, transaction))
                    {
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                (original as LowestUnit).AddPersonnel(this.personnels[reader.GetInt32(1) - 1]);
                            }
                        }
                    }
                }
                else if(original is Formation)
                {
                    string selectHeavyEquipmentAssignedQuery = $"select equipmentTable.equipmentModel, heavyEquipmentToFormation.formationID from EquipmentTable inner join heavyEquipmentToFormation on equipmentTable.ID = heavyEquipmentToFormation.equipmentID where heavyEquipmentToFormation.formationID = {formationToID[formation]};";
                    using (SqlCommand command = new SqlCommand(selectHeavyEquipmentAssignedQuery, connection, transaction))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                formation.HeavyEquipmentAssigned.Add(this.equipmentPieces[reader.GetString(0)]);
                            }
                        }
                    }

                    string selectAdditionalEquipmentQuery = $"select equipmentTable.equipmentModel, additionalEquipmentToFormation.formationID from EquipmentTable inner join additionalEquipmentToFormation on equipmentTable.ID = additionalEquipmentToFormation.equipmentID where additionalEquipmentToFormation.formationID = {formationToID[formation]};";
                    using (SqlCommand command = new SqlCommand(selectAdditionalEquipmentQuery, connection, transaction))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                formation.AdditionalEquipment.Add(this.equipmentPieces[reader.GetString(0)]);
                            }
                        }
                    }

                    string selectHQasLowestUnitQuery = $"select * from lowestUnitHQToFormation where formationID = {formationToID[formation]};";
                    using (SqlCommand command = new SqlCommand(selectHQasLowestUnitQuery, connection, transaction))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                (original as Formation).HeadquartersFormation = this.templates[reader.GetInt32(1) - 1];
                            }
                        }
                    }

                    string selectHQasFormationQuery = $"select * from FormationHQToFormation where formationID = {formationToID[formation]};";
                    using (SqlCommand command = new SqlCommand(selectHQasFormationQuery, connection, transaction))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                (original as Formation).HeadquartersFormation = this.templates[reader.GetInt32(1) - 1];
                            }
                        }
                    }

                    string selectLowestUnitsAssigned = $"select * from  lowestUnitToFormation where lowestUnitToFormation.mainFormationID = {formationToID[formation]};";
                    using (SqlCommand command = new SqlCommand(selectLowestUnitsAssigned, connection, transaction))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                (original as Formation).AddFormation(this.templates[reader.GetInt32(1) - 1]);
                            }
                        }
                    }

                    string selectFormationsAssigned = $"select * from  FormationToFormation where FormationToFormation.mainFormationID = {formationToID[formation]};";
                    using (SqlCommand command = new SqlCommand(selectFormationsAssigned, connection, transaction))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                (original as Formation).AddFormation(this.templates[reader.GetInt32(1) - 1]);
                            }
                        }
                    }
                }
            }
        }

        public bool Load()
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        LoadRanks(connection, transaction);
                        LoadEquipment(connection, transaction);
                        LoadPersonnel(connection, transaction);
                        LoadTemplateFormations(connection, transaction);

                        transaction.Commit();
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading data! Message: \n" + ex.Message);
                        transaction.Rollback();
                        connection.Close();
                        return false;
                    }
                }

            }

            return true;
        }
        public void SetRanks(Dictionary<string, Rank> ranks)
        {
            this.ranks = ranks;
        }
        public void SetEquipment(Dictionary<string, Equipment> equipmentPieces)
        {
            this.equipmentPieces = equipmentPieces;
        }
        public void SetPersonnel(List<Personnel> personnels)
        {
            this.personnels = personnels;
        }
        public void SetSavedTemplates(List<IFormation> formations)
        {
            this.savedFormations = formations;
        }
        public void SetTemplates(List<IFormation> formations)
        {
            this.templates = formations;
        }
        public Dictionary<string, Rank> GetLoadedRanks()
        {
            return this.ranks;
        }
        public Dictionary<string, Equipment> GetLoadedEquipment()
        {
            return this.equipmentPieces;
        }
        public List<Personnel> GetLoadedPersonnel()
        {
            return this.personnels;
        }
        public List<IFormation> GetLoadedTemplates()
        {
            return this.templates;
        }
    }

}
