using System;
using System.Windows;
using System.Windows.Threading;

namespace Home_Practica1_FAAA
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>  
    public partial class LoginWindow : Window
    {
        // Almacenamos credenciales de usuarios en memoria 
        private string[,] usuariosRegistrados = new string[,]
        {
            { "admin", "1234" },
            { "franklin", "pws2026" },
            { "player1", "gamer" }
        };

        // Variables para controlar los intentos y el bloqueo temporal
        private int intentosFallidos = 0;
        private const int MAX_INTENTOS = 3;
        private DispatcherTimer temporizadorBloqueo;
        private int segundosRestantes = 30; // Tiempo de bloqueo en segundos

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

            //  Comprueba que los campos no estén vacíos
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

            // Recorremos el array de usuarios registrados para verificar las credenciales
            bool autenticado = false;
            for (int i = 0; i < usuariosRegistrados.GetLength(0); i++)
            {
                if (usuariosRegistrados[i, 0] == usuarioIngresado && usuariosRegistrados[i, 1] == passwordIngresado)
                {
                    autenticado = true;
                    break;
                }
            }

            // Manejo del resultado de autenticación
            if (autenticado)
            {
                // Reiniciar contador al tener éxito
                intentosFallidos = 0;

                MessageBox.Show(
                    $"Acceso correcto. ¡Bienvenido {usuarioIngresado}!",
                    "Inicio de Sesión Exitoso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                // Abre la ventana principal pasándole el nombre del usuario
                MainWindow ventanaPrincipal = new MainWindow(usuarioIngresado);
                ventanaPrincipal.Show();

                // Cierra la ventana actual de login
                this.Close();
            }
            else
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
        }

        private void BloquearFormulario()
        {
            // Deshabilitar campos y botón de login
            txtUsuario.IsEnabled = false;
            txtPassword.IsEnabled = false;
            btnLogin.IsEnabled = false;

            // Iniciar cuenta atrás
            segundosRestantes = 30;
            temporizadorBloqueo.Start();
        }

        private void TemporizadorBloqueo_Tick(object sender, EventArgs e)
        {
            segundosRestantes--;

            if (segundosRestantes <= 0)
            {
                // Detener temporizador y desbloquear interfaz
                temporizadorBloqueo.Stop();
                txtUsuario.IsEnabled = true;
                txtPassword.IsEnabled = true;
                btnLogin.IsEnabled = true;

                // Limpiar campos y reiniciar contador
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

        // Evento Click del botón Salir
        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}