using Dapper;
using EngineeringProjectApp.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace EngineeringProjectApp
{
    public class SqliteDataAccess
    {
        public static List<UserModel> LoadAllUsers(){
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<UserModel>("select * from User", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveUser(UserModel user) {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into User (FirstName, LastName) values (@FirstName, @LastName)",user);
            }
        }

        public static void EditUser(UserModel user, int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string query = "update User set FirstName = @FirstName, LastName =  @LastName where id = @id";
                cnn.Execute(query,new { user.FirstName,user.LastName, id});
            }
        }


        public static void DeleteUser(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string query = "delete from User where id = @id";
                cnn.Execute(query, new { id });
            }
        }

        public static List<GameModel> LoadAllGames()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<GameModel>("select * from Game", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void DeleteAllGamesForUser(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string query = "delete from Game where UserId = @id";
                cnn.Execute(query, new { id });
            }
        }

        public static void SaveGame(GameModel game)
        {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                string query = "insert into Game (UserId, Date, Level, Returning, AmountOfButterflies, AmountOfBirds, Time, Velocity) " +
                    "values (@UserId, @Date, @Level, @Returning, @AmountOfButterflies, @AmountOfBirds, @Time, @Velocity)";
                cnn.Execute(query, new {
                    game.UserId, game.Date, game.Level, game.Returning, game.AmountOfButterflies, game.AmountOfBirds, game.Time, game.Velocity

                });
            }
        }

        public static List<ResultModel> LoadGamesForUser(string level, int userId, int returningFlag)
        {        

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string query = "select User.FirstName, User.LastName, Game.Date, Game.Level, Game.Returning, Game.AmountOfButterflies, Game.AmountOfBirds, Game.Time, Game.Velocity from User inner join Game on User.Id = Game.UserId where Game.Level like @value and  User.Id = @id and Game.Returning= @flag order by Game.Time ";
                var output = cnn.Query<ResultModel>(query, new { value = level, id = userId, flag = returningFlag});
                return output.ToList();
            }
        }

        private static string LoadConnectionString(string id="Default") {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;

        }
    }
}
