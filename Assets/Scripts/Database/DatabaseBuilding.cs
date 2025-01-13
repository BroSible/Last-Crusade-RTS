using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine;

public class DatabaseBuilding : MonoBehaviour
{
    private string dbPath;

    public ObjectsDatabseSO objectsDatabase;

    private void Awake()
    {
        dbPath = Path.Combine(Application.dataPath, "Buildings.db");
        CreateDatabase();
        InsertObjectsData();
    }

    private void CreateDatabase()
    {
        using (var connection = new SqliteConnection("URI=file:" + dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS ObjectData (
                        ID INTEGER PRIMARY KEY,
                        Name TEXT,
                        Description TEXT,
                        SizeX INTEGER,
                        SizeY INTEGER,
                        Cost INTEGER
                    );
                ";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    private void InsertObjectsData()
    {
        using (var connection = new SqliteConnection("URI=file:" + dbPath))
        {
            connection.Open();

            foreach (var obj in objectsDatabase.objectsData)
            {
                int totalCost = 0;
                foreach (var req in obj.requirements)
                {
                    totalCost += req.amount;
                }

                Debug.Log($"Inserting Object: ID={obj.ID}, Name={obj.Name}, SizeX={obj.Size.x}, SizeY={obj.Size.y}, Cost={totalCost}");

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT OR REPLACE INTO ObjectData (ID, Name, Description, SizeX, SizeY, Cost)
                        VALUES (@ID, @Name, @Description, @SizeX, @SizeY, @Cost);
                    ";
                    command.Parameters.Add(new SqliteParameter("@ID", obj.ID));
                    command.Parameters.Add(new SqliteParameter("@Name", obj.Name));
                    command.Parameters.Add(new SqliteParameter("@Description", obj.description));
                    command.Parameters.Add(new SqliteParameter("@SizeX", obj.Size.x));
                    command.Parameters.Add(new SqliteParameter("@SizeY", obj.Size.y));
                    command.Parameters.Add(new SqliteParameter("@Cost", totalCost));
                    command.ExecuteNonQuery();
                }
            }

            connection.Close();
        }
    }
}
