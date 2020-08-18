using System.Data.SQLite;
using System.IO;

namespace EngineeringProjectApp
{
    public class Database
    {
        public SQLiteConnection myConnection;

        public Database() {
            myConnection = new SQLiteConnection("Data Source = gamedatabase.sqlite3");
            if (!File.Exists("./gamedatabase.sqlite3"))
            {
                SQLiteConnection.CreateFile("gamedatabase.sqlite3");
                System.Console.WriteLine("DZIALA");
            }
        }
    }
}
