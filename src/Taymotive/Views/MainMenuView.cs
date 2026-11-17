
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Taymotive.Views
{
    public class MainMenuView : Form
    {
        private Label lblTitle = new Label();
        private Label lblWelcome = new Label();

        private Button btnUsers = new Button();
        private Button btnTrainers = new Button();
        private Button btnRoutines = new Button();
        private Button btnMeasurements = new Button();
        private Button btnVisits = new Button();
        private Button btnReports = new Button();
        private Button btnLogout = new Button();

        public MainMenuView()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Menú principal";
            this.Size = new Size(650, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "TAYMOTIVE";
            lblTitle.Font = new Font(
                "Arial", 24, FontStyle.Bold
            );
            lblTitle.Location = new Point(205, 30);
            lblTitle.AutoSize = true;

            lblWelcome.Text = "Sistema de gestión fitness";
            lblWelcome.Font = new Font("Arial", 12);
            lblWelcome.Location = new Point(205, 85);
            lblWelcome.AutoSize = true;

            ConfigureButton(
                btnUsers, "Usuarios", 100, 150
            );

            ConfigureButton(
                btnTrainers, "Entrenadores", 350, 150
            );

            ConfigureButton(
                btnRoutines, "Rutinas", 100, 230
            );

            ConfigureButton(
                btnMeasurements, "Mediciones", 350, 230
            );

            ConfigureButton(
                btnVisits, "Visitas y clases", 100, 310
            );

            ConfigureButton(
                btnReports, "Reportes", 350, 310
            );

            ConfigureButton(
                btnLogout, "Cerrar sesión", 225, 410
            );

            btnUsers.Click += BtnUsers_Click;
            btnTrainers.Click += ShowPendingMessage;
            btnRoutines.Click += ShowPendingMessage;
            btnMeasurements.Click += ShowPendingMessage;
            btnVisits.Click += ShowPendingMessage;
            btnReports.Click += ShowPendingMessage;

            btnLogout.Click += BtnLogout_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblWelcome);
            this.Controls.Add(btnUsers);
            this.Controls.Add(btnTrainers);
            this.Controls.Add(btnRoutines);
            this.Controls.Add(btnMeasurements);
            this.Controls.Add(btnVisits);
            this.Controls.Add(btnReports);
            this.Controls.Add(btnLogout);
        }

        private void ConfigureButton(
            Button button,
            string text,
            int x,
            int y)
        {
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(200, 50);
        }

        private void ShowPendingMessage(
            object? sender,
            EventArgs e)
        {
            MessageBox.Show(
                "Esta funcionalidad se implementará próximamente.",
                "TAYMOTIVE",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnLogout_Click(
            object? sender,
            EventArgs e)
        {
            this.Close();
        }

        private void BtnUsers_Click(object? sender, EventArgs e)
        {           
        using (UsersView usersView = new UsersView())
        {
            usersView.ShowDialog();
            }
        }

    }
}
