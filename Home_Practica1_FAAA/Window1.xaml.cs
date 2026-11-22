using System;
using System.Windows;
using System.Windows.Threading;
using MySql.Data.MySqlClient;

namespace Home_Practica1_FAAA
{
    public partial class LoginWindow : Window
    {
        // Variables para controlar los intentos y el bloqueo temporal
        private int intentosFallidos = 0;
        private const int MAX_INTENTOS = 3;
        private DispatcherTimer temporizadorBloqueo;
        private int segundosRestantes = 30;

        public LoginWindow()
        {
            InitializeComponent();
            ConfigurarTemporizador();
        }

        private void ConfigurarTemporizador()
        {
            temporizadorBloqueo = new DispatcherTimer();
            temporizadorBloqueo.Interval = TimeSpan.FromSeconds(1);
            temporizadorBloqueo.Tick += TemporizadorBloqueo_Tick;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuarioIngresado = txtUsuario.Text.Trim();
            string passwordIngresado = txtPassword.Password;

            // Validación de campos vacíos
            if (string.IsNullOrEmpty(usuarioIngresado) || string.IsNullOrEmpty(passwordIngresado))
            {
                MessageBox.Show(
                    "Por favor, rellene todos los campos antes de continuar.",
                    "Campos Vacíos",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            MySqlConnection conexion = null;

            try
            {
                conexion = DatabaseHelper.ObtenerConexion();

                // Consulta SQL parametrizada
                string query = "SELECT COUNT(*) FROM Usuarios WHERE nombre_usuario = @usuario AND password = @password";
                MySqlCommand cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@usuario", usuarioIngresado);
                cmd.Parameters.AddWithValue("@password", passwordIngresado);

                int coincidencias = Convert.ToInt32(cmd.ExecuteScalar());

                if (coincidencias > 0)
                {
                    intentosFallidos = 0;

                    MessageBox.Show(
                        $"Acceso correcto. ¡Bienvenido {usuarioIngresado}!",
                        "Inicio de Sesión Exitoso",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    MainWindow ventanaPrincipal = new MainWindow(usuarioIngresado);
                    ventanaPrincipal.Show();
                    this.Close();
                }
                else
                {
                    GestionarFalloAutenticacion();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    $"No se pudo conectar con la base de datos MySQL.\nComprueba que el servicio esté activo.\n\nDetalle: {ex.Message}",
                    "Error de Conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ha ocurrido un error inesperado: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                DatabaseHelper.CerrarConexion(conexion);
            }
        }

        private void btnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            string nuevoUsuario = txtUsuario.Text.Trim();
            string nuevaPassword = txtPassword.Password;

            if (string.IsNullOrEmpty(nuevoUsuario) || string.IsNullOrEmpty(nuevaPassword))
            {
                MessageBox.Show(
                    "Introduce un nombre de usuario y una contraseña en los campos para registrarte.",
                    "Campos Vacíos",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            MySqlConnection conexion = null;

            try
            {
                conexion = DatabaseHelper.ObtenerConexion();

                string query = "INSERT INTO Usuarios (nombre_usuario, password) VALUES (@usuario, @password)";
                MySqlCommand cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@usuario", nuevoUsuario);
                cmd.Parameters.AddWithValue("@password", nuevaPassword);

                int filasAfectadas = cmd.ExecuteNonQuery();

                if (filasAfectadas > 0)
                {
                    MessageBox.Show(
                        $"El usuario '{nuevoUsuario}' se ha registrado correctamente. Ya puedes iniciar sesión.",
                        "Usuario Creado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    txtPassword.Clear();
                }
            }
            catch (MySqlException ex)
            {
                // El código 1062 en MySQL corresponde a entrada duplicada (Duplicate entry for key UNIQUE)
                if (ex.Number == 1062)
                {
                    MessageBox.Show(
                        $"El nombre de usuario '{nuevoUsuario}' ya existe en la base de datos. Elige otro distinto.",
                        "Usuario Duplicado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
                else
                {
                    MessageBox.Show(
                        $"Error de base de datos al registrar el usuario.\n\nDetalle: {ex.Message}",
                        "Error de Conexión",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error inesperado: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                DatabaseHelper.CerrarConexion(conexion);
            }
        }

        private void GestionarFalloAutenticacion()
        {
            intentosFallidos++;

            if (intentosFallidos >= MAX_INTENTOS)
            {
                BloquearFormulario();
                MessageBox.Show(
                    $"Ha superado el límite de {MAX_INTENTOS} intentos fallidos.\nEl acceso se ha bloqueado temporalmente por {segundosRestantes} segundos.",
                    "Cuenta Bloqueada",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop
                );
            }
            else
            {
                int intentosRestantes = MAX_INTENTOS - intentosFallidos;
                MessageBox.Show(
                    $"El usuario o la contraseña introducidos no son correctos.\nIntentos restantes: {intentosRestantes}",
                    "Error de Autenticación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void BloquearFormulario()
        {
            txtUsuario.IsEnabled = false;
            txtPassword.IsEnabled = false;
            btnLogin.IsEnabled = false;
            btnNuevoUsuario.IsEnabled = false;

            segundosRestantes = 30;
            temporizadorBloqueo.Start();
        }

        private void TemporizadorBloqueo_Tick(object sender, EventArgs e)
        {
            segundosRestantes--;

            if (segundosRestantes <= 0)
            {
                temporizadorBloqueo.Stop();
                txtUsuario.IsEnabled = true;
                txtPassword.IsEnabled = true;
                btnLogin.IsEnabled = true;
                btnNuevoUsuario.IsEnabled = true;

                intentosFallidos = 0;
                txtPassword.Clear();
                txtUsuario.Focus();

                MessageBox.Show(
                    "El tiempo de bloqueo ha finalizado. Puede volver a intentarlo.",
                    "Acceso Desbloqueado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}