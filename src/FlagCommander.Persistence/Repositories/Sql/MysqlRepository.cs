using System.Data.Common;
using MySql.Data.MySqlClient;

namespace FlagCommander.Persistence.Repositories.Sql;

public class MysqlRepository : SqlRepositoryBase
{
    public MysqlRepository(string connectionString) : base(connectionString)
    {
    }

    protected override DbConnection DbConnection => new MySqlConnection(ConnectionString);

    protected override async Task Init()
    {
        await using var connection = DbConnection;
        await connection.OpenAsync();
        var scriptContent = @"
DROP PROCEDURE IF EXISTS `create_index`;

DELIMITER |

CREATE PROCEDURE `create_index`
(
    given_table    VARCHAR(64),
    given_index    VARCHAR(64),
    given_columns  VARCHAR(64)
)
BEGIN

    DECLARE IndexIsThere INTEGER;

    SELECT COUNT(1) INTO IndexIsThere
    FROM INFORMATION_SCHEMA.STATISTICS
    WHERE table_name   = given_table
    AND   index_name   = given_index;

    IF IndexIsThere = 0 THEN
        SET @statement = CONCAT('CREATE INDEX ',given_index,' ON ',given_table,' (',given_columns,')');
        PREPARE st FROM @statement;
        EXECUTE st;
        DEALLOCATE PREPARE st;
    END IF;

END |

DELIMITER ;

CREATE TABLE IF NOT EXISTS __flag_commander_flags 
(name VARCHAR(255) PRIMARY KEY, percentage_of_time INTEGER DEFAULT 100, percentage_of_actors INTEGER DEFAULT 100, enabled INTEGER DEFAULT 1);

CALL create_index('__flag_commander_flags', '__flag_commander_flags_enabled_index', 'enabled');
CALL create_index('__flag_commander_flags', '__flag_commander_flags_name_index', 'name');
CALL create_index('__flag_commander_flags', '__flag_commander_flags_name_and_enabled_index', 'name, enabled');

CREATE TABLE IF NOT EXISTS __flag_commander_actors 
(flag_name VARCHAR(255), actor_id VARCHAR(255), PRIMARY KEY (flag_name, actor_id), FOREIGN KEY (flag_name) REFERENCES __flag_commander_flags(name) ON DELETE CASCADE);

CALL create_index('__flag_commander_actors', '__flag_commander_actors_flag_name_and_actor_id_index', 'flag_name, actor_id');
";
        var mysqlScript = new MySqlScript((MySqlConnection)connection, scriptContent);
        await mysqlScript.ExecuteAsync();
    }
}