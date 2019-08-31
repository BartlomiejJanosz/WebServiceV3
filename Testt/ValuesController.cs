using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Http;
using System.IO;
using System.Net.Http;
using System.Data;
using System.Data.SQLite;

namespace Testt
{
    public class ValuesController : ApiController
    {         
        //HTTP METHOD    
        public string Get()
        {
            //Returns for example: 1Bartek2Kordian //number on left side of the playerName is id related to that word in waitingRoom table
            return ReturnChainOfPlayersNamesAndPlayerIdsFromWaitingRoomTable();
        }

        //HTTP METHOD
        public string Post([FromBody] Player player)
        {
            return AddPlayerToWaitingRoomTableReturnNameAndIdOfFounderIfExists(player);
        }

        public void Post([FromBody] BattleInfo battleInfo)
        {
            AddBattleInfoToTable(battleInfo);
        }

        private void AddBattleInfoToTable(BattleInfo battleInfo)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
            {
                dbConnection.Open();
                string battleInfoId = CreateIdForSpecyficTable(dbConnection, "battleInfo");

                string sqliteQuery = "INSERT INTO battleInfo (infoId, playerId, health, firstCardId, firstCardLangVersion, secondCardId, secondCardLangVersion) values ({0},{1},{2},{3},'{4}',{5},'{6}')";
                sqliteQuery = String.Format(sqliteQuery, battleInfoId, battleInfo.PlayerId, battleInfo.Health, battleInfo.FirstCard.CardId, battleInfo.FirstCard.LangVersion, battleInfo.SecondCard.CardId, battleInfo.SecondCard.LangVersion);
                SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);
                sqliteCommand.ExecuteNonQuery();
            }
        }

        public void Delete([FromBody]DeleteInfo deleteInfo)
        {
            if(deleteInfo.TableName == "waitingRoom")
            {
                DeleteAllRecordsFromWaitingRoomTable();
            }
            else if(deleteInfo.TableName == "battleInfo")
            {
                DeleteAllRecordsFromBattleInfoTable();
            }            
        }

        //HTTP METHOD
        public void Put(int id, [FromBody]string value)
        {
        }

        private string AddPlayerToWaitingRoomTableReturnNameAndIdOfFounderIfExists(Player player)
        {
            if (player.JoinToExistingGame)
            {
                using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
                {
                    dbConnection.Open();
                    if (HowManyRecordsExistInSpecyficTable(dbConnection, "waitingRoom") == 0)
                    {
                        return "";
                    }
                    else
                    {
                        AddPlayerToWaitingRoomTable(player.PlayerName);
                        var founderOfGame = ReturnFounderOfGame();
                        return "1" + founderOfGame;
                    }
                }
            }
            else
            {
                AddPlayerToWaitingRoomTable(player.PlayerName);
                return "";
            }
        }

        public static void DeleteAllRecordsFromBattleInfoTable()
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
            {
                dbConnection.Open();
                string sqliteQuery = "DELETE FROM battleInfo";
                SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);
                sqliteCommand.ExecuteNonQuery();
            }
        }

        public static void DeleteAllRecordsFromWaitingRoomTable()
        {           
                using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
                {
                    dbConnection.Open();
                    string sqliteQuery = "DELETE FROM waitingRoom";
                    SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);
                    sqliteCommand.ExecuteNonQuery();
                }            
        }

        private string ReturnChainOfPlayersNamesAndPlayerIdsFromWaitingRoomTable()
        {           
                using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
                {
                    dbConnection.Open();
                    string sqliteQuery = "SELECT * FROM waitingRoom";
                    SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);

                    if (HowManyRecordsExistInSpecyficTable(dbConnection, "waitingRoom") != 0)
                    {
                        SQLiteDataReader reader = sqliteCommand.ExecuteReader();

                        int counter = 1;
                        var returnString = "";
                        while (reader.Read())
                        {
                            var founderName = reader["playerName"].ToString();
                            returnString += counter.ToString() + founderName;
                            counter += 1;
                        }
                        return returnString;
                    }
                    else
                    {
                        return "";
                    }
                }            
        }

        //private void MakeCopyOfLangWarDataBaseFile()
        //{
        //    var currentDirectory = Directory.GetCurrentDirectory();
        //    var pathToFile = Path.Combine(currentDirectory, "LangWarDataBase.sqlite");
        //    var pathToCopy = Path.Combine(currentDirectory, "LangWarDataBaseCopy.sqlite");
        //    File.Copy(pathToFile, pathToCopy,true);
        //}

        //private void RemoveCopyOfLangWarDatabase()
        //{
        //    var currentDirectory = Directory.GetCurrentDirectory();
        //    var pathToCopy = Path.Combine(currentDirectory, "LangWarDataBaseCopy.sqlite");
        //    if(File.Exists(pathToCopy))
        //    {
        //        File.Delete(pathToCopy);
        //    }          
        //}

        private string ReturnFounderOfGame()
        {            
                using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
                {
                    dbConnection.Open();
                    string sqliteQuery = "SELECT * FROM waitingRoom WHERE playerId = 1";
                    SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);
                    SQLiteDataReader reader = sqliteCommand.ExecuteReader();
                    reader.Read();
                    var founderName = reader["playerName"].ToString();
                    return founderName;
                }           
        }

        private void AddPlayerToWaitingRoomTable(string playerName)
        {
                using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=LangWarDataBase.sqlite;Version=3;"))
                {
                    dbConnection.Open();
                    string playerId = CreateIdForSpecyficTable(dbConnection, "waitingRoom");

                    string sqliteQuery = "INSERT INTO waitingRoom (playerId, playerName) values (" + playerId + ", '" + playerName + "')";
                    SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);
                    sqliteCommand.ExecuteNonQuery();
                }
        }

        private string CreateIdForSpecyficTable(SQLiteConnection dbConnection, string tableName)
        {
            var id = HowManyRecordsExistInSpecyficTable(dbConnection, tableName) + 1;
            return id.ToString();
        }
        private int HowManyRecordsExistInSpecyficTable(SQLiteConnection dbConnection, string tableName)
        {
                string sqliteQuery = "SELECT * FROM " + tableName;
                SQLiteCommand sqliteCommand = new SQLiteCommand(sqliteQuery, dbConnection);
                SQLiteDataReader sqliteReader = sqliteCommand.ExecuteReader();

                if (sqliteReader.HasRows == true)
                {
                    int counter = 0;
                    while (sqliteReader.Read())
                    {
                        counter += 1;
                    }
                    return counter;
                }
                else
                {
                    return 0;
                }
        }
    }
}
