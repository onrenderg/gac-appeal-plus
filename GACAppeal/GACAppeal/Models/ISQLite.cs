using System;
namespace GACAppeal
{
    public interface ISQLite
    {
        SQLite.SQLiteConnection GetConnection();
    }
}
