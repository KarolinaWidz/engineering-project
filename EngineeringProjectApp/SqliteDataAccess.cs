using Dapper;
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
                string query = "update User set FirstName = @FirstName, LastName =  @LastName where id= @id";
                cnn.Execute(query,new { user.FirstName,user.LastName, id});
            }
        }


        public static void DeleteUser(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string query = @"DELETE FROM User WHERE id=@id";
                cnn.Execute(query, new { id });
            }
        }

        private static string LoadConnectionString(string id="Default") {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;

        }
    }
}
