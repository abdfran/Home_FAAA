using MySql.Data.MySqlClient;

namespace Home_Practica1_FAAA
{
    public class DatabaseHelper
    {
        // Escribe tu contraseña de MySQL después de Pwd=
        private static readonly string connectionString = "Server=localhost;Database=catalogo_videojuegos;Uid=root;Pwd=Al24aCUSH5;";

        public static MySqlConnection ObtenerConexion()
        {
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();
            return conexion;
        }

        public static void CerrarConexion(MySqlConnection conexion)
        {
            if (conexion != null && conexion.State == System.Data.ConnectionState.Open)
            {
                conexion.Close();
            }
        }
    }
}