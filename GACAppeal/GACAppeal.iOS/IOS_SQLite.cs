using System;
using System.IO;
using Xamarin.Forms;
using SQLite;
using GACAppeal.iOS;

[assembly: Dependency(typeof(IOS_SQLite))]
namespace GACAppeal.iOS
{
    public class IOS_SQLite : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var dbName = "GACApeal.sqlite";
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder  
            string libraryPath = Path.Combine(dbPath, "..", "Library"); // Library folder  
            var path = Path.Combine(libraryPath, dbName);
            var conn = new SQLite.SQLiteConnection(path);
            return conn;
        }
    }
}
