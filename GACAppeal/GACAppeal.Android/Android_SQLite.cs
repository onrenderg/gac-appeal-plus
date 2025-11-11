using GACAppeal.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(Android_SQLite))]
namespace GACAppeal.Droid
{
    public class Android_SQLite : ISQLite
    {
        public SQLite.SQLiteConnection GetConnection()
        {
            var dbName = "GACAppeal.sqlite";
            var dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            var path = System.IO.Path.Combine(dbPath, dbName);
            var conn = new SQLite.SQLiteConnection(path);
            return conn;
        }
    }
}
