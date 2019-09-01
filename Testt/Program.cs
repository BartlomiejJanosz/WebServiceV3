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

            CreateLangWarDataBase();
        
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

            CreateWaitingRoomTable();
            CreateBattleInfoTable();
            CreateCardsTable();

            PrepareTablesBeforeGame();            
        }

        private static void PrepareTablesBeforeGame()
        {            
            ValuesController.DeleteAllRecordsFromWaitingRoomTable();
            ValuesController.DeleteAllRecordsFromBattleInfoTable();
            DeleteAllRecordsFromCardsTable();
            FillCardsTable();
        }

        private static void FillCardsTable() //Takes data from CardsCSV.csv and put into table
        {            
            var currentDirectory = Directory.GetCurrentDirectory();
            var pathToFile = Path.Combine(currentDirectory, "CardsCSV.csv");
            File.Delete(pathToFile);
            try
            {
                using (var reader = new StreamReader(pathToFile))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        string[] elements = line.Split(',');

                        AddRecordToCardsTable(elements[0], elements[1], elements[2], elements[3]);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Brak pliku .csv zawierającego kolekcje słów. Aplikacja zakończy działanie za 4 sekundy");
                Thread.Sleep(4000);
                Environment.Exit(0);
            }            
        }

        private static void AddRecordToCardsTable(string cardId, string cardLvl, string angWord, string plWord)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
            {
                dbConnection.Open();
                string temp = "INSERT INTO cards (cardId, cardlvl, plWord, angWord) values ({0},{1},'{2}','{3}')";
                string sqliteQuery = String.Format(temp,cardId,cardLvl,angWord,plWord);
                SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);
                sqliteCommand.ExecuteNonQuery();
            }
        }

        private static void DeleteAllRecordsFromCardsTable()
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
            {
                dbConnection.Open();
                string sqliteQuery = "DELETE FROM cards";
                SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);
                sqliteCommand.ExecuteNonQuery();
            }
        }

        private static void CreateCardsTable()
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
            {
                dbConnection.Open();
                bool cardsTableExist = CheckIfTableExists(dbConnection, "cards");

                if (!cardsTableExist)
                {
                    string sqliteQuery = "CREATE TABLE cards (cardId INT, cardlvl INT, plWord VARCHAR(20), angWord VARCHAR(20))";
                    SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);
                    sqliteCommand.ExecuteNonQuery();
                }
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
