using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using MatchServer.FieldManagement;
using MatchServer.Players;
using NetworkShared.Data.Field;

namespace MatchServer.DatabaseManagement
{
    /// <summary>
    /// Controls database connection and data management
    /// </summary>
    public class DatabaseManager : IDisposable
    {
        private const string connectionStringSettingsKey = "connectionString";
        private string connectionString;
        private const string providerSettingsKey = "provider";
        private DbProviderFactory factory;
        private DbConnection connection;

        #region Initialization

        /// <summary>
        /// Connects manager to the database
        /// </summary>
        public void Connect()
        {
            connectionString = ConfigurationManager.AppSettings[connectionStringSettingsKey];
            string provider = ConfigurationManager.AppSettings[providerSettingsKey];
            factory = DbProviderFactories.GetFactory(provider);
            connection = factory.CreateConnection();
            if (connection == null)
            {
                Console.WriteLine("Connection error");
                throw new Exception("Connection error");
            }
            connection.ConnectionString = connectionString;
            connection.Open();
            Console.WriteLine($"Open connection to: {connectionString}");

            InitDatabaseIfEmpty();
        }

        private const string sqlInitDatabaseIfEmpty = @"
IF OBJECT_ID(N'dbo.Players', N'U') IS NULL BEGIN
    create table Players
    (
	    ID uniqueidentifier not null,
        PlayerID varchar(64) not null,
	    Name varchar(64),
	    UniqueBlockCollection uniqueidentifier not null,
	    ActiveHero varchar(64),
	    Currency int,
	    Rating int,
	    RegistrationDate DateTime,
	    primary key (ID),
    );
    create index Players_PlayerID
    on Players (PlayerID);
END;

IF OBJECT_ID(N'dbo.Collections', N'U') IS NULL BEGIN
    create table Collections
    (
	    ID uniqueidentifier not null,
        Level1Blocks varchar(max),
        Level2Blocks varchar(max),
        Level3Blocks varchar(max),
        Collection varchar(max),
	    primary key (ID),
    );
END;
";

        private void InitDatabaseIfEmpty()
        {
            DbCommand command = factory.CreateCommand();
            if (command == null)
            {
                Console.WriteLine("CreateCommand error");
                throw new Exception("CreateCommand error");
            }

            command.Connection = connection;
            command.CommandText = sqlInitDatabaseIfEmpty;
            command.ExecuteNonQuery();
        }

        #endregion

        #region Players

        private const string sqlAddPlayer = @"
insert into dbo.Players (ID, PlayerID, Name, UniqueBlockCollection, ActiveHero, Currency, Rating, RegistrationDate)
values (@id, @player_id, @name, @collection, @active_hero, @currency, @rating, @registration_date);
";

        /// <summary>
        /// Adds player
        /// </summary>
        public void AddPlayer(Player player)
        {
            DbCommand command = factory.CreateCommand();
            if (command == null)
            {
                Console.WriteLine("CreateCommand error");
                throw new Exception("CreateCommand error");
            }

            command.Connection = connection;
            command.CommandText = sqlAddPlayer;

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@id";
            param.Value = player.ID.ToString();
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@player_id";
            param.Value = player.PlayerID;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@name";
            param.Value = player.Name;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@collection";
            param.Value = player.UniqueBlockCollection.ID.ToString();
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@active_hero";
            param.Value = player.ActiveHero;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@currency";
            param.Value = player.Currency;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@rating";
            param.Value = player.Rating;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@registration_date";
            param.Value = DateTime.UtcNow.ToString("s"); ;
            command.Parameters.Add(param);

            command.ExecuteNonQuery();
        }

        private const string sqlUpdatePlayer = @"
update dbo.Players
set ID = @id, PlayerID = @player_id, Name = @name, UniqueBlockCollection = @collection, ActiveHero = @active_hero,
    Currency = @currency, Rating = @rating, RegistrationDate = @registration_date;
";

        /// <summary>
        /// Updates player data in database
        /// </summary>
        /// <param name="player"></param>
        public void UpdatePlayer(Player player)
        {
            DbCommand command = factory.CreateCommand();
            if (command == null)
            {
                Console.WriteLine("CreateCommand error");
                throw new Exception("CreateCommand error");
            }

            command.Connection = connection;
            command.CommandText = sqlUpdatePlayer;

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@id";
            param.Value = player.ID.ToString();
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@player_id";
            param.Value = player.PlayerID;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@name";
            param.Value = player.Name;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@collection";
            param.Value = player.UniqueBlockCollection.ID.ToString();
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@active_hero";
            param.Value = player.ActiveHero;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@currency";
            param.Value = player.Currency;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@rating";
            param.Value = player.Rating;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@registration_date";
            param.Value = DateTime.UtcNow.ToString("s"); ;
            command.Parameters.Add(param);

            command.ExecuteNonQuery();
        }

        private const string sqlGetPlayer = @"
select * from dbo.Players where PlayerID = @player_id;
";

        /// <summary>
        /// Returns player by its account id
        /// </summary>
        public Player GetPlayer(string playerID)
        {
            DbCommand command = factory.CreateCommand();
            if (command == null)
            {
                Console.WriteLine("CreateCommand error");
                throw new Exception("CreateCommand error");
            }

            command.Connection = connection;
            command.CommandText = sqlGetPlayer;

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@player_id";
            param.Value = playerID;
            command.Parameters.Add(param);

            using (DbDataReader dataReader = command.ExecuteReader())
            {
                if (!dataReader.Read())
                    return null;

                Player player = new Player();
                player.ID = (Guid)dataReader[nameof(Player.ID)];
                player.PlayerID = (string)dataReader[nameof(Player.PlayerID)];
                player.Name = dataReader[nameof(Player.Name)] == DBNull.Value ? string.Empty : (string)dataReader[nameof(Player.Name)];
                player.UniqueBlockCollection = new UniqueBlockCollection();
                player.UniqueBlockCollection.ID = (Guid)dataReader[nameof(Player.UniqueBlockCollection)];
                player.ActiveHero = dataReader[nameof(Player.ActiveHero)] == DBNull.Value ? string.Empty : (string)dataReader[nameof(Player.ActiveHero)];
                player.Currency = (int)dataReader[nameof(Player.Currency)];
                player.Rating = (int)dataReader[nameof(Player.Rating)];

                return player;
            }
        }

        #endregion

        #region Collection

        private string BlocksToString(IEnumerable<UniqueBlock> blocks)
        {
            return string.Join(",", blocks.Select(b => b.Name));
        }

        private IEnumerable<UniqueBlock> BlocksFromString(string data)
        {
            return data.Split(',').Select(k => BlockEffectsManager.UniqueBlocks[k]);
        }

        private string ActiveBlocksToString(Dictionary<BlockTypes, UniqueBlock> blocks)
        {
            return string.Join(";", blocks.Select(p => p.Key.ToString() + "," + p.Value.Name));
        }

        private Dictionary<BlockTypes, UniqueBlock> ActiveBlocksFromString(string data)
        {
            Dictionary<BlockTypes, UniqueBlock> dic = new Dictionary<BlockTypes, UniqueBlock>();
            foreach (string pair in data.Split(';'))
            {
                string[] parsedPair = pair.Split(',');
                Enum.TryParse(parsedPair[0], out BlockTypes type);
                dic.Add(type, BlockEffectsManager.UniqueBlocks[parsedPair[1]]);
            }
            return dic;
        }

        private const string sqlAddCollection = @"
insert into dbo.Collections (ID, Level1Blocks, Level2Blocks, Level3Blocks, Collection)
values (@id, @level1_blocks, @level2_blocks, @level3_blocks, @blocks);
";

        /// <summary>
        /// Add unique block collection to database
        /// </summary>
        /// <param name="collection"></param>
        public void AddCollection(UniqueBlockCollection collection)
        {
            DbCommand command = factory.CreateCommand();
            if (command == null)
            {
                Console.WriteLine("CreateCommand error");
                throw new Exception("CreateCommand error");
            }

            command.Connection = connection;
            command.CommandText = sqlAddCollection;

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@id";
            param.Value = collection.ID.ToString();
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@level1_blocks";
            param.Value = ActiveBlocksToString(collection.Level1Blocks);
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@level2_blocks";
            param.Value = ActiveBlocksToString(collection.Level2Blocks);
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@level3_blocks";
            param.Value = ActiveBlocksToString(collection.Level3Blocks);
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@blocks";
            param.Value = BlocksToString(collection.Collection);
            command.Parameters.Add(param);

            command.ExecuteNonQuery();
        }

        private const string sqlUpdateCollection = @"
update dbo.Collections
set ID = @id, Level1Blocks = @level1_blocks, Level2Blocks = @level2_blocks, Level3Blocks = @level3_blocks, Collection = @blocks;
";

        /// <summary>
        /// Update unique block collection in database
        /// </summary>
        /// <param name="collection"></param>
        public void UpdateCollection(UniqueBlockCollection collection)
        {
            DbCommand command = factory.CreateCommand();
            if (command == null)
            {
                Console.WriteLine("CreateCommand error");
                throw new Exception("CreateCommand error");
            }

            command.Connection = connection;
            command.CommandText = sqlUpdateCollection;

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@id";
            param.Value = collection.ID.ToString();
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@level1_blocks";
            param.Value = ActiveBlocksToString(collection.Level1Blocks);
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@level2_blocks";
            param.Value = ActiveBlocksToString(collection.Level2Blocks);
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@level3_blocks";
            param.Value = ActiveBlocksToString(collection.Level3Blocks);
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@blocks";
            param.Value = BlocksToString(collection.Collection);
            command.Parameters.Add(param);

            command.ExecuteNonQuery();
        }

        private const string sqlGetCollection = @"
select * from dbo.Collections where ID = @id;
";

        /// <summary>
        /// Returns collection by its id
        /// </summary>
        public UniqueBlockCollection GetCollection(Guid collectionID)
        {
            DbCommand command = factory.CreateCommand();
            if (command == null)
            {
                Console.WriteLine("CreateCommand error");
                throw new Exception("CreateCommand error");
            }

            command.Connection = connection;
            command.CommandText = sqlGetCollection;

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@id";
            param.Value = collectionID;
            command.Parameters.Add(param);

            using (DbDataReader dataReader = command.ExecuteReader())
            {
                if (!dataReader.Read())
                    return null;

                UniqueBlockCollection collection = new UniqueBlockCollection();
                collection.ID = (Guid)dataReader[nameof(Player.ID)];
                collection.Level1Blocks = ActiveBlocksFromString((string)dataReader[nameof(UniqueBlockCollection.Level1Blocks)]);
                collection.Level2Blocks = ActiveBlocksFromString((string)dataReader[nameof(UniqueBlockCollection.Level2Blocks)]);
                collection.Level3Blocks = ActiveBlocksFromString((string)dataReader[nameof(UniqueBlockCollection.Level3Blocks)]);
                collection.Collection = BlocksFromString((string)dataReader[nameof(UniqueBlockCollection.Collection)]).ToList();

                return collection;
            }
        }

        #endregion

        #region Dispose

        private void ReleaseUnmanagedResources()
        {
            connection?.Close();
            Console.WriteLine($"Connection closed");
            connection?.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~DatabaseManager()
        {
            ReleaseUnmanagedResources();
        }

        #endregion
    }
}
