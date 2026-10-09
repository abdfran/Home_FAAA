using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Home_Practica1_FAAA
{
    /// <summary>
    /// Lógica de interacción para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        // 1. Almacén de credenciales en memoria (Punto 2)
        private string[,] usuariosRegistrados = new string[,]
        {
            { "admin", "1234" },
            { "franklin", "pws2026" },
            { "player1", "gamer" }
        };

        public LoginWindow()
        {
            InitializeComponent();
        }

        // 2. Evento Click del botón de Login (Punto 2 y 3)
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuarioIngresado = txtUsuario.Text.Trim();
            string passwordIngresado = txtPassword.Password;

            // Validación: Comprobar que los campos no estén vacíos mediante ventana emergente
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

            // Validación: Recorrer la matriz en memoria
            bool autenticado = false;
            for (int i = 0; i < usuariosRegistrados.GetLength(0); i++)
            {
                if (usuariosRegistrados[i, 0] == usuarioIngresado && usuariosRegistrados[i, 1] == passwordIngresado)
                {
                    autenticado = true;
                    break;
                }
            }

            // Emisión de mensajes por ventana y navegación (Punto 3)
            if (autenticado)
            {
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
                MessageBox.Show(
                    "El usuario o la contraseña introducidos no son correctos.",
                    "Error de Autenticación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
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