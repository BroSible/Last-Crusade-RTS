using UnityEditor;
using Mono.Data.Sqlite;
using UnityEngine;

public class UnitDatabaseEditorWindow : EditorWindow
{
    private string dbName = "URI=file:Units.db";  // Путь к вашей базе данных
    private Vector2 scrollPos;
    private string[] unitNames;
    private string[] unitDescriptions;
    private int[] unitMaxHealth;
    private int[] unitUnitDamage;
    private int[] unitSpeedUnit;
    private float[] unitAttackingDistance;
    private float[] unitStopAttackingDistance;

    [MenuItem("Window/Unit Database Viewer")]
    public static void ShowWindow()
    {
        GetWindow<UnitDatabaseEditorWindow>("Unit Database Viewer");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Load Units From Database"))
        {
            LoadUnitsFromDatabase();
        }

        if (unitNames != null)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();

            // Заголовок таблицы
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.Width(150));
            EditorGUILayout.LabelField("Description", GUILayout.Width(200));
            EditorGUILayout.LabelField("Max Health", GUILayout.Width(100));
            EditorGUILayout.LabelField("Damage", GUILayout.Width(100));
            EditorGUILayout.LabelField("Speed", GUILayout.Width(100));
            EditorGUILayout.LabelField("Attacking Distance", GUILayout.Width(150));
            EditorGUILayout.LabelField("Stop Distance", GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            // Таблица с данными
            for (int i = 0; i < unitNames.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(unitNames[i], GUILayout.Width(150));
                EditorGUILayout.LabelField(unitDescriptions[i], GUILayout.Width(200));
                EditorGUILayout.LabelField(unitMaxHealth[i].ToString(), GUILayout.Width(100));
                EditorGUILayout.LabelField(unitUnitDamage[i].ToString(), GUILayout.Width(100));
                EditorGUILayout.LabelField(unitSpeedUnit[i].ToString(), GUILayout.Width(100));
                EditorGUILayout.LabelField(unitAttackingDistance[i].ToString(), GUILayout.Width(150));
                EditorGUILayout.LabelField(unitStopAttackingDistance[i].ToString(), GUILayout.Width(150));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
    }

    private void LoadUnitsFromDatabase()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            // Запрос для получения всех юнитов
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM units;";
                SqliteDataReader reader = command.ExecuteReader();

                var tempUnitNames = new System.Collections.Generic.List<string>();
                var tempDescriptions = new System.Collections.Generic.List<string>();
                var tempMaxHealth = new System.Collections.Generic.List<int>();
                var tempUnitDamage = new System.Collections.Generic.List<int>();
                var tempSpeedUnit = new System.Collections.Generic.List<int>();
                var tempAttackingDistance = new System.Collections.Generic.List<float>();
                var tempStopAttackingDistance = new System.Collections.Generic.List<float>();

                while (reader.Read())
                {
                    tempUnitNames.Add(reader["name"].ToString());
                    tempDescriptions.Add(reader["description"].ToString());
                    tempMaxHealth.Add(int.Parse(reader["maxHealth"].ToString()));
                    tempUnitDamage.Add(int.Parse(reader["unitDamage"].ToString()));
                    tempSpeedUnit.Add(int.Parse(reader["speedUnit"].ToString()));
                    tempAttackingDistance.Add(float.Parse(reader["attackingDistance"].ToString()));
                    tempStopAttackingDistance.Add(float.Parse(reader["stopAttackingDistance"].ToString()));
                }

                unitNames = tempUnitNames.ToArray();
                unitDescriptions = tempDescriptions.ToArray();
                unitMaxHealth = tempMaxHealth.ToArray();
                unitUnitDamage = tempUnitDamage.ToArray();
                unitSpeedUnit = tempSpeedUnit.ToArray();
                unitAttackingDistance = tempAttackingDistance.ToArray();
                unitStopAttackingDistance = tempStopAttackingDistance.ToArray();
            }
        }
    }
}
