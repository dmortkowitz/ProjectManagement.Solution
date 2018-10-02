using System.Collections.Generic;
using ProjectManagement;
using MySql.Data.MySqlClient;
using System;

namespace ProjectManagement.Models
{
  public class Project : ICRUDMethods<Project>
  {
    public string Name {get; set;}
    public string Content {get; set;}
    public DateTime DueDate {get; set;}
    public string Status {get; set;}
    public int Id {get; set;}

    public Project(string newName, string newContent, DateTime newDueDate, string newStatus, int id = 0)
    {
      this.Name = newName;
      this.Content = newContent;
      this.DueDate = newDueDate;
      this.Status = newStatus;
      this.Id = id;
    }

    public Project(string newName, int id = 0)
    {
      this.Name = newName;
      this.Id = id;
    }


    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO projects (name, content, dueDate, status) VALUES (@name, @content, @dueDate, @status);";

      cmd.Parameters.AddWithValue("@name", this.Name);
      cmd.Parameters.AddWithValue("@content", this.Content);
      cmd.Parameters.AddWithValue("@dueDate", this.DueDate);
      cmd.Parameters.AddWithValue("@status", this.Status);

      cmd.ExecuteNonQuery();
      this.Id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }


    public static List<Project> GetAll()
    {
      List<Project> allProjects = new List<Project> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM projects;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int ProjectId = rdr.GetInt32(0);
        string ProjectName = rdr.GetString(1);
        string ProjectContent = rdr.GetString(2);
        DateTime ProjectDueDate = rdr.GetDateTime(3);
        string ProjectStatus = rdr.GetString(4);
        Project newProject = new Project(ProjectName, ProjectContent,ProjectDueDate,ProjectStatus, ProjectId);
        allProjects.Add(newProject);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allProjects;
    }


    public static Project Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM projects WHERE id = @searchId;";

      cmd.Parameters.AddWithValue("@searchId", id);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      Project newProject = new Project("", "", DateTime.Now, "", id);

      while(rdr.Read())
      {
        newProject.Id = rdr.GetInt32(0);
        newProject.Name = rdr.GetString(1);
        newProject.Content = rdr.GetString(2);
        newProject.DueDate = rdr.GetDateTime(3);
        newProject.Status = rdr.GetString(4);
      }

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

      return newProject;
    }


    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM projects_users WHERE project_id = @ProjectId; DELETE FROM projects_tags WHERE project_id = @ProjectId; DELETE FROM projects_todos WHERE project_id = @ProjectId; DELETE FROM projects_forums WHERE project_id = @ProjectId; DELETE FROM projects WHERE id = @ProjectId;");

      cmd.Parameters.AddWithValue("@ProjectId", this.Id);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM projects;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }


    public void Update(Project newProject)
    {
      //string newName, string newContent, DateTime newDueDate, string newStatus
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE projects SET name = @newName, content = @newContent, duedate = @newDueDate, status = @newStatus WHERE id = @searchId;";

      cmd.Parameters.AddWithValue("@searchId", this.Id);
      cmd.Parameters.AddWithValue("@newName", newProject.Name);
      cmd.Parameters.AddWithValue("@newContent", newProject.Content);
      cmd.Parameters.AddWithValue("@newDueDate", newProject.DueDate);
      cmd.Parameters.AddWithValue("@newStatus", newProject.Status);

      cmd.ExecuteNonQuery();
      this.Name = newProject.Name;
      this.Content = newProject.Content;
      this.DueDate = newProject.DueDate;
      this.Status = newProject.Status;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public override bool Equals(System.Object otherProject)
    {
      if (!(otherProject is Project))
      {
        return false;
      }
      else
      {

        Project newProject = (Project) otherProject;
        bool idEquality = (this.Id == newProject.Id);
        bool nameEquality = (this.Name == newProject.Name);
        bool contentEquality = (this.Content == newProject.Content);
        bool duedateEquality = (this.DueDate == newProject.DueDate);
        bool statusEquality = (this.Status == newProject.Status);
        return (idEquality && nameEquality && contentEquality && duedateEquality && statusEquality);
      }
    }
    public override int GetHashCode()
    {
      return this.Name.GetHashCode();
    }
  }
}
