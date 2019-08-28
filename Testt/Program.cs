using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using System.Net.Http;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SQLite;

namespace Testt
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:12345/";
            //CreateWaitingRoomFile();

            CreateLangWarDataBase();
            CreateWaitingRoomTable();
            CreateBattleInfoTable();

            ValuesController.DeleteAllPlayersFromWaitingRoomTable();
            ValuesController.DeleteAllFromBattleInfoTable();

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                while (true)
                {
                    Thread.Sleep(2);
                }
            }
        }
        
        private static void CreateBattleInfoTable()
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
            {
                dbConnection.Open();
                bool battleInfoTableTableExist = CheckIfTableExists(dbConnection, "battleInfo");

                if (!battleInfoTableTableExist)
                {
                    string sqliteQuery = "CREATE TABLE battleInfo (infoId INT, playerId INT, health INT, firstCardId INT, firstCardLangVersion VARCHAR(20), secondCardId INT, secondCardLangVersion VARCHAR(20))";
                    SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);
                    sqliteCommand.ExecuteNonQuery();
                }
            }
        }

        private static void CreateLangWarDataBase()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var pathToFile = Path.Combine(currentDirectory, "LangWarDataBase.sqlite");
            if (!File.Exists(pathToFile))
            {
                SQLiteConnection.CreateFile("LangWarDataBase.sqlite");
            }
        }

        private static void CreateWaitingRoomTable()
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
            {
                dbConnection.Open();
                bool waitingRoomTableExist = CheckIfTableExists(dbConnection, "waitingRoom");

                if (!waitingRoomTableExist)
                {
                    string sqliteQuery = "CREATE TABLE waitingRoom (playerId INT, playerName VARCHAR(20))";
                    SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);
                    sqliteCommand.ExecuteNonQuery();
                }
            }
        }

        private static bool CheckIfTableExists(SQLiteConnection dbConnection,string tableName)
        {
            string sqliteQuery = "SELECT * FROM "+ tableName;
            SQLiteCommand command = new SQLiteCommand(dbConnection);
            command.CommandText = sqliteQuery;

            try
            {
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
