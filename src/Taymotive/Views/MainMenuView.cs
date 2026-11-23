
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class MainMenuView : Form
    {
        private readonly User currentUser;
        private readonly UsersController usersController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblWelcome = new Label();
        private readonly Label lblUsername = new Label();

        // Botones del administrador
        private readonly Button btnUsers = new Button();
        private readonly Button btnTrainers = new Button();
        private readonly Button btnRoutines = new Button();
        private readonly Button btnMeasurements = new Button();
        private readonly Button btnVisits = new Button();
        private readonly Button btnGoals = new Button();
        private readonly Button btnBadges = new Button();
        private readonly Button btnReports = new Button();

        // Botones del usuario
        private readonly Button btnMySummary = new Button();
        private readonly Button btnMyRoutines = new Button();
        private readonly Button btnMyGoals = new Button();
        private readonly Button btnMyBadges = new Button();
        private readonly Button btnMyMeasurements = new Button();
        private readonly Button btnMyVisits = new Button();

        // Botones generales
        private readonly Button btnNotifications = new Button();
        private readonly Button btnLogout = new Button();

        public MainMenuView(User currentUser)
        {
            this.currentUser = currentUser ??
                throw new ArgumentNullException(
                    nameof(currentUser)
                );

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            usersController = new UsersController(filePath);

            InitializeComponents();
        }

        // Verificar si el usuario es administrador
        private bool IsAdministrator()
        {
            User? user = usersController.GetUserById(
                currentUser.Id
            );

            return user != null &&
                user.IsAdmin &&
                string.Equals(
                    user.Username,
                    currentUser.Username,
                    StringComparison.OrdinalIgnoreCase
                );
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Menú principal";

            bool isAdmin = IsAdministrator();

            ClientSize = isAdmin
                ? new Size(650, 720)
                : new Size(650, 790);

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "TAYMOTIVE";
            lblTitle.Font = new Font(
                "Arial",
                24,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Top = 25;

            lblWelcome.Text = "Sistema de gestión fitness";
            lblWelcome.Font = new Font("Arial", 12);
            lblWelcome.AutoSize = true;
            lblWelcome.Top = 80;

            lblUsername.Text =
                "Bienvenido(a), " + currentUser.Name;

            lblUsername.Font = new Font(
                "Arial",
                11,
                FontStyle.Bold
            );
            lblUsername.AutoSize = true;
            lblUsername.Top = 115;

            int buttonWidth = 200;
            int columnGap = 40;

            int totalWidth =
                (buttonWidth * 2) + columnGap;

            int leftColumn =
                (ClientSize.Width - totalWidth) / 2;

            int rightColumn =
                leftColumn + buttonWidth + columnGap;

            int centerColumn =
                (ClientSize.Width - buttonWidth) / 2;

            if (isAdmin)
            {
                // MENÚ DEL ADMINISTRADOR

                ConfigureButton(
                    btnUsers,
                    "Usuarios",
                    leftColumn,
                    170
                );

                ConfigureButton(
                    btnTrainers,
                    "Entrenadores",
                    rightColumn,
                    170
                );

                ConfigureButton(
                    btnRoutines,
                    "Rutinas",
                    leftColumn,
                    250
                );

                ConfigureButton(
                    btnMeasurements,
                    "Mediciones",
                    rightColumn,
                    250
                );

                ConfigureButton(
                    btnVisits,
                    "Visitas y clases",
                    leftColumn,
                    330
                );

                ConfigureButton(
                    btnGoals,
                    "Objetivos físicos",
                    rightColumn,
                    330
                );

                ConfigureButton(
                    btnBadges,
                    "Insignias y logros",
                    leftColumn,
                    410
                );

                ConfigureButton(
                    btnReports,
                    "Reportes y estadísticas",
                    rightColumn,
                    410
                );

                Controls.Add(btnUsers);
                Controls.Add(btnTrainers);
                Controls.Add(btnRoutines);
                Controls.Add(btnMeasurements);
                Controls.Add(btnVisits);
                Controls.Add(btnGoals);
                Controls.Add(btnBadges);
                Controls.Add(btnReports);

                btnUsers.Click += (sender, e) =>
                {
                    if (!IsAdministrator())
                    {
                        ShowAccessDenied();
                        return;
                    }

                    using (UsersView view =
                        new UsersView(currentUser))
                    {
                        view.ShowDialog(this);
                    }
                };

                btnTrainers.Click += (sender, e) =>
                {
                    if (!IsAdministrator())
                    {
                        ShowAccessDenied();
                        return;
                    }

                    using (TrainersView view =
                        new TrainersView())
                    {
                        view.ShowDialog(this);
                    }
                };

                btnRoutines.Click += (sender, e) =>
                {
                    if (!IsAdministrator())
                    {
                        ShowAccessDenied();
                        return;
                    }

                    using (RoutinesView view =
                        new RoutinesView())
                    {
                        view.ShowDialog(this);
                    }
                };

                btnMeasurements.Click += (sender, e) =>
                {
                    if (!IsAdministrator())
                    {
                        ShowAccessDenied();
                        return;
                    }

                    using (MeasurementsView view =
                        new MeasurementsView())
                    {
                        view.ShowDialog(this);
                    }
                };

                btnVisits.Click += (sender, e) =>
                {
                    if (!IsAdministrator())
                    {
                        ShowAccessDenied();
                        return;
                    }

                    using (VisitsView view =
                        new VisitsView())
                    {
                        view.ShowDialog(this);
                    }
                };

                btnGoals.Click += (sender, e) =>
                {
                    if (!IsAdministrator())
                    {
                        ShowAccessDenied();
                        return;
                    }

                    using (FitnessGoalsView view =
                        new FitnessGoalsView())
                    {
                        view.ShowDialog(this);
                    }
                };

                btnBadges.Click += (sender, e) =>
                {
                    if (!IsAdministrator())
                    {
                        ShowAccessDenied();
                        return;
                    }

                    using (BadgesView view =
                        new BadgesView())
                    {
                        view.ShowDialog(this);
                    }
                };

                btnReports.Click += (sender, e) =>
                {
                    if (!IsAdministrator())
                    {
                        ShowAccessDenied();
                        return;
                    }

                    using (ReportsView view =
                        new ReportsView())
                    {
                        view.ShowDialog(this);
                    }
                };
            }
            else
            {
                // MENÚ PERSONAL DEL USUARIO

                // Nuevo botón: Mi resumen fitness
                ConfigureButton(
                    btnMySummary,
                    "Mi resumen fitness",
                    centerColumn,
                    170
                );

                ConfigureButton(
                    btnMyRoutines,
                    "Mis rutinas",
                    centerColumn,
                    240
                );

                ConfigureButton(
                    btnMyGoals,
                    "Mis objetivos físicos",
                    centerColumn,
                    310
                );

                ConfigureButton(
                    btnMyBadges,
                    "Mis insignias y logros",
                    centerColumn,
                    380
                );

                ConfigureButton(
                    btnMyMeasurements,
                    "Mi progreso físico",
                    centerColumn,
                    450
                );

                ConfigureButton(
                    btnMyVisits,
                    "Mis visitas y clases",
                    centerColumn,
                    520
                );

                // Abrir resumen fitness
                btnMySummary.Click += (sender, e) =>
                {
                    using (MyFitnessSummaryView view =
                        new MyFitnessSummaryView(currentUser))
                    {
                        view.ShowDialog(this);
                    }
                };

                // Abrir rutinas
                btnMyRoutines.Click += (sender, e) =>
                {
                    using (MyRoutinesView view =
                        new MyRoutinesView(currentUser))
                    {
                        view.ShowDialog(this);
                    }
                };

                // Abrir objetivos físicos
                btnMyGoals.Click += (sender, e) =>
                {
                    using (MyFitnessGoalsView view =
                        new MyFitnessGoalsView(currentUser))
                    {
                        view.ShowDialog(this);
                    }
                };

                // Abrir insignias y logros
                btnMyBadges.Click += (sender, e) =>
                {
                    using (MyBadgesView view =
                        new MyBadgesView(currentUser))
                    {
                        view.ShowDialog(this);
                    }
                };

                // Abrir progreso físico
                btnMyMeasurements.Click += (sender, e) =>
                {
                    using (MyMeasurementsView view =
                        new MyMeasurementsView(currentUser))
                    {
                        view.ShowDialog(this);
                    }
                };

                // Abrir visitas y clases
                btnMyVisits.Click += (sender, e) =>
                {
                    using (MyVisitsView view =
                        new MyVisitsView(currentUser))
                    {
                        view.ShowDialog(this);
                    }
                };

                Controls.Add(btnMySummary);
                Controls.Add(btnMyRoutines);
                Controls.Add(btnMyGoals);
                Controls.Add(btnMyBadges);
                Controls.Add(btnMyMeasurements);
                Controls.Add(btnMyVisits);
            }

            // NOTIFICACIONES

            ConfigureButton(
                btnNotifications,
                "Mis notificaciones",
                centerColumn,
                isAdmin ? 505 : 590
            );

            btnNotifications.Paint +=
                BtnNotifications_Paint;

            btnNotifications.TextAlign =
                ContentAlignment.MiddleRight;

            btnNotifications.Padding =
                new Padding(0, 0, 12, 0);

            btnNotifications.Click += (sender, e) =>
            {
                using (NotificationsView view =
                    new NotificationsView(currentUser))
                {
                    view.ShowDialog(this);
                }
            };

            // CERRAR SESIÓN

            ConfigureButton(
                btnLogout,
                "Cerrar sesión",
                centerColumn,
                isAdmin ? 585 : 660
            );

            btnLogout.Click += (sender, e) =>
            {
                Close();
            };

            Controls.Add(lblTitle);
            Controls.Add(lblWelcome);
            Controls.Add(lblUsername);
            Controls.Add(btnNotifications);
            Controls.Add(btnLogout);

            CenterHeaderLabels();

            ClientSizeChanged += (sender, e) =>
            {
                CenterHeaderLabels();
            };
        }

        // Mostrar mensaje de acceso restringido
        private void ShowAccessDenied()
        {
            MessageBox.Show(
                "No tiene permisos para acceder " +
                "a este módulo.",
                "TAYMOTIVE",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        // Dibujar campana de notificaciones
        private void BtnNotifications_Paint(
            object? sender,
            PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(
                Color.Black,
                1.5f))
            {
                int x = 18;
                int y = 14;

                graphics.DrawArc(
                    pen,
                    x,
                    y,
                    16,
                    16,
                    180,
                    180
                );

                graphics.DrawLine(
                    pen,
                    x,
                    y + 8,
                    x,
                    y + 18
                );

                graphics.DrawLine(
                    pen,
                    x + 16,
                    y + 8,
                    x + 16,
                    y + 18
                );

                graphics.DrawLine(
                    pen,
                    x - 3,
                    y + 18,
                    x + 19,
                    y + 18
                );

                graphics.DrawArc(
                    pen,
                    x + 6,
                    y + 18,
                    5,
                    5,
                    0,
                    180
                );
            }
        }

        // Centrar los encabezados
        private void CenterHeaderLabels()
        {
            lblTitle.Left =
                (ClientSize.Width - lblTitle.Width) / 2;

            lblWelcome.Left =
                (ClientSize.Width - lblWelcome.Width) / 2;

            lblUsername.Left =
                (ClientSize.Width - lblUsername.Width) / 2;
        }

        // Configurar botones
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
    }
}
