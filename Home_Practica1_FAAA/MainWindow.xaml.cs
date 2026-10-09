using System.Windows;

namespace Home_Practica1_FAAA
{
    public partial class MainWindow : Window
    {
        // Constructor por defecto
        public MainWindow()
        {
            InitializeComponent();
        }

        // Constructor personalizado que recibe el usuario autenticado
        public MainWindow(string usuario) : this()
        {
            txtBienvenida.Text = $"Bienvenido, {usuario}";
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            // Reabrir la ventana de Login y cerrar la ventana actual
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}