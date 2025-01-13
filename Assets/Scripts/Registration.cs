using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.Analytics;
using System;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class Registration : MonoBehaviour
{
    public InputField usernameField;
    public InputField passwordField;
    public Text errorText;
    public InputField doublePas1;
    public static bool Enter = false;

    private string dbName = "URI=file:Users.db";

    private void Start()
    {
        CreateDB();
    }

    private bool ValidatePassword(string password)
    {
        Regex regex = new Regex("^(?=.*[A-Z])(?=.*\\d)(?=.*[a-z]).{8,}$");
        return regex.IsMatch(password);
    }

    private bool ValidateUsername(string username)
    {
        Regex regex = new Regex("^[A-Z][a-z0-9]{2,}$");

        return regex.IsMatch(username);
    }

    private void CreateDB()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS users (username VARCHAR(20), password VARCHAR(20));";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public void ConfirmPas()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        string doublePas = doublePas1.text;
        if (string.IsNullOrEmpty(doublePas))
        {
            errorText.text = "CONFRIM YOUR PASSWORD";
        }
        else if (doublePas == password)
        {
            Register();
        }
        else
        {
            errorText.text = "You are input the wrong password";
        }
    }

    public void Login()
    {
        string username = usernameField.text;
        string password = passwordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorText.text = "INPUT SOME";
            return;
        }

        using (var connection = new SqliteConnection(dbName))
        {
            try
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                   command.CommandText = "SELECT * FROM users WHERE username = @username AND password = @password";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            errorText.text = "Your welcome";
                            Debug.Log("Login successful");
                            Enter = true;
                            SceneManager.LoadScene("Menu");
                        }
                        else
                        {
                            errorText.text = "Invalid username or password";
                            Debug.Log("Login failed");
                        }
                         reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during login: {ex.Message}");
                errorText.text = "Login failed. Please try again later.";
            }
            finally
            {
                connection.Close();
            }
        }
    }


 public void Register()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        string confirmPassword = doublePas1.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            errorText.text = "Please fill in all fields";
            return;
        }

        if (!ValidateUsername(username))
        {
            errorText.text = "Invalid username format. Must start with a capital letter, followed by 2 or more lowercase letters.";
            return;
        }

        if (!ValidatePassword(password))
        {
            errorText.text = "Invalid password format. Must contain at least 8 characters, one uppercase letter, one digit, and at least one lowercase letter.";
            return;
        }


        if (password != confirmPassword)
        {
            errorText.text = "Passwords do not match";
            return;
        }

        using (var connection = new SqliteConnection(dbName))
        {
             try
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM users WHERE username = @username";
                    command.Parameters.AddWithValue("@username", username);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    if (count > 0)
                    {
                        errorText.text = "User already exists";
                        Debug.Log("User already exists");
                         return;
                    }
                   
                  command.CommandText = "INSERT INTO users (username, password) VALUES (@username, @password)";
                    command.Parameters.AddWithValue("@username", username);
                     command.Parameters.AddWithValue("@password", password);
                    command.ExecuteNonQuery();
                 
                    errorText.text = "Registration successful!";
                   SceneManager.LoadScene("Menu");
                }
            }
             catch(Exception ex)
            {
                Debug.LogError($"Error during registration: {ex.Message}");
                errorText.text = "Registration failed. Please try again later.";
            }
              finally
            {
                connection.Close();
            }
        }
    }
}