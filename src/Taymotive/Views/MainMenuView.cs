
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Taymotive.Views
{
    public class MainMenuView : Form
    {
        private readonly Label lblTitle = new Label();
        private readonly Label lblWelcome = new Label();

        private readonly Button btnUsers = new Button();
        private readonly Button btnTrainers = new Button();
        private readonly Button btnRoutines = new Button();
        private readonly Button btnMeasurements = new Button();
        private readonly Button btnVisits = new Button();
        private readonly Button btnReports = new Button();
        private readonly Button btnLogout = new Button();

        public MainMenuView()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Menú principal";
            Size = new Size(650, 550);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "TAYMOTIVE";
            lblTitle.Font = new Font(
                "Arial",
                24,
                FontStyle.Bold
            );
            lblTitle.Location = new Point(205, 30);
            lblTitle.AutoSize = true;

            lblWelcome.Text = "Sistema de gestión fitness";
            lblWelcome.Font = new Font("Arial", 12);
            lblWelcome.Location = new Point(205, 85);
            lblWelcome.AutoSize = true;

            ConfigureButton(btnUsers, "Usuarios", 100, 150);
            ConfigureButton(btnTrainers, "Entrenadores", 350, 150);
            ConfigureButton(btnRoutines, "Rutinas", 100, 230);
            ConfigureButton(btnMeasurements, "Mediciones", 350, 230);
            ConfigureButton(btnVisits, "Visitas y clases", 100, 310);
            ConfigureButton(btnReports, "Reportes", 350, 310);
            ConfigureButton(btnLogout, "Cerrar sesión", 225, 410);

            btnUsers.Click += BtnUsers_Click;
            btnTrainers.Click += BtnTrainers_Click;
            btnRoutines.Click += BtnRoutines_Click;
            btnMeasurements.Click += BtnMeasurements_Click;
            btnVisits.Click += BtnVisits_Click;
            btnReports.Click += BtnReports_Click;
            btnLogout.Click += BtnLogout_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblWelcome);
            Controls.Add(btnUsers);
            Controls.Add(btnTrainers);
            Controls.Add(btnRoutines);
            Controls.Add(btnMeasurements);
            Controls.Add(btnVisits);
            Controls.Add(btnReports);
            Controls.Add(btnLogout);
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

        private void BtnUsers_Click(object? sender, EventArgs e)
        {
            using (UsersView view = new UsersView())
            {
                view.ShowDialog(this);
            }
        }

        private void BtnTrainers_Click(object? sender, EventArgs e)
        {
            using (TrainersView view = new TrainersView())
            {
                view.ShowDialog(this);
            }
        }

        private void BtnRoutines_Click(object? sender, EventArgs e)
        {
            using (RoutinesView view = new RoutinesView())
            {
                view.ShowDialog(this);
            }
        }

        private void BtnMeasurements_Click(object? sender, EventArgs e)
        {
            using (MeasurementsView view = new MeasurementsView())
            {
                view.ShowDialog(this);
            }
        }

        private void BtnVisits_Click(object? sender, EventArgs e)
        {
            using (VisitsView view = new VisitsView())
            {
                view.ShowDialog(this);
            }
        }

        private void BtnReports_Click(object? sender, EventArgs e)
        {
            using (ReportsView view = new ReportsView())
            {
                view.ShowDialog(this);
            }
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
