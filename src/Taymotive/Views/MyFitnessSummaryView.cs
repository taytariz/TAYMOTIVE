
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class MyFitnessSummaryView : Form
    {
        private readonly User currentUser;

        private readonly RoutinesController routinesController;
        private readonly VisitsController visitsController;
        private readonly FitnessGoalsController goalsController;
        private readonly BadgesController badgesController;
        private readonly MeasurementsController measurementsController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblUsername = new Label();

        private readonly Label lblRoutines = new Label();
        private readonly Label lblVisits = new Label();
        private readonly Label lblGoals = new Label();
        private readonly Label lblBadges = new Label();

        private readonly Label lblWeight = new Label();
        private readonly Label lblHeight = new Label();
        private readonly Label lblBMI = new Label();
        private readonly Label lblDate = new Label();

        private readonly GroupBox grpGeneral = new GroupBox();
        private readonly GroupBox grpMeasurements = new GroupBox();

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        public MyFitnessSummaryView(User currentUser)
        {
            this.currentUser = currentUser ??
                throw new ArgumentNullException(
                    nameof(currentUser)
                );

            string folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files"
            );

            routinesController = new RoutinesController(
                Path.Combine(folderPath, "users.json")
            );

            visitsController = new VisitsController(
                Path.Combine(folderPath, "visits.json")
            );

            goalsController = new FitnessGoalsController(
                Path.Combine(folderPath, "fitnessgoals.json")
            );

            badgesController = new BadgesController(
                Path.Combine(folderPath, "badges.json")
            );

            measurementsController = new MeasurementsController(
                Path.Combine(folderPath, "measurements.json")
            );

            InitializeComponents();
            LoadSummary();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Mi resumen fitness";
            ClientSize = new Size(750, 620);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "MI RESUMEN FITNESS";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Top = 25;

            lblUsername.Text =
                "Usuario: " + currentUser.Name;

            lblUsername.Font = new Font("Arial", 11);
            lblUsername.Location = new Point(40, 95);
            lblUsername.AutoSize = true;

            // Resumen general
            grpGeneral.Text = "Resumen general";
            grpGeneral.Font = new Font(
                "Arial",
                11,
                FontStyle.Bold
            );
            grpGeneral.Location = new Point(40, 140);
            grpGeneral.Size = new Size(670, 185);

            ConfigureLabel(
                lblRoutines,
                "Rutinas asignadas: 0",
                25,
                35
            );

            ConfigureLabel(
                lblVisits,
                "Visitas registradas: 0",
                25,
                70
            );

            ConfigureLabel(
                lblGoals,
                "Objetivos completados: 0 de 0",
                25,
                105
            );

            ConfigureLabel(
                lblBadges,
                "Insignias obtenidas: 0",
                25,
                140
            );

            grpGeneral.Controls.Add(lblRoutines);
            grpGeneral.Controls.Add(lblVisits);
            grpGeneral.Controls.Add(lblGoals);
            grpGeneral.Controls.Add(lblBadges);

            // Última medición física
            grpMeasurements.Text = "Última medición física";
            grpMeasurements.Font = new Font(
                "Arial",
                11,
                FontStyle.Bold
            );
            grpMeasurements.Location = new Point(40, 345);
            grpMeasurements.Size = new Size(670, 180);

            ConfigureLabel(
                lblWeight,
                "Peso: Sin registros",
                25,
                35
            );

            ConfigureLabel(
                lblHeight,
                "Estatura: Sin registros",
                25,
                70
            );

            ConfigureLabel(
                lblBMI,
                "IMC: Sin registros",
                25,
                105
            );

            ConfigureLabel(
                lblDate,
                "Fecha: Sin registros",
                340,
                35
            );

            grpMeasurements.Controls.Add(lblWeight);
            grpMeasurements.Controls.Add(lblHeight);
            grpMeasurements.Controls.Add(lblBMI);
            grpMeasurements.Controls.Add(lblDate);

            // Botones
            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(220, 550);
            btnRefresh.Size = new Size(140, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(390, 550);
            btnClose.Size = new Size(140, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(grpGeneral);
            Controls.Add(grpMeasurements);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();

            ClientSizeChanged += (sender, e) =>
            {
                CenterTitle();
            };
        }

        // Configurar etiquetas
        private void ConfigureLabel(
            Label label,
            string text,
            int x,
            int y)
        {
            label.Text = text;
            label.Font = new Font(
                "Arial",
                11,
                FontStyle.Regular
            );
            label.Location = new Point(x, y);
            label.AutoSize = true;
        }

        // Cargar el resumen del usuario
        private void LoadSummary()
        {
            try
            {
                int userId = currentUser.Id;

                // Rutinas asignadas
                int totalRoutines =
                    routinesController
                        .GetRoutinesByUser(userId)
                        .Count;

                lblRoutines.Text =
                    "Rutinas asignadas: " + totalRoutines;

                // Visitas registradas
                int totalVisits =
                    visitsController
                        .CountVisitsByUser(userId);

                lblVisits.Text =
                    "Visitas registradas: " + totalVisits;

                // Objetivos físicos
                var goals =
                    goalsController.GetGoalsByUser(userId);

                int completedGoals = goals.Count(
                    goal => goal.IsCompleted
                );

                lblGoals.Text =
                    "Objetivos completados: " +
                    completedGoals + " de " + goals.Count;

                // Insignias obtenidas
                int totalBadges =
                    badgesController
                        .GetBadgesByUser(userId)
                        .Count;

                lblBadges.Text =
                    "Insignias obtenidas: " + totalBadges;

                // Última medición física
                var measurements =
                    measurementsController
                        .GetMeasurementsByUser(userId);

                Measurement? latestMeasurement =
                    measurements
                        .OrderByDescending(
                            measurement =>
                                measurement.MeasurementDate
                        )
                        .FirstOrDefault();

                if (latestMeasurement != null)
                {
                    lblWeight.Text =
                        "Peso: " +
                        latestMeasurement.Weight.ToString("F2") +
                        " kg";

                    lblHeight.Text =
                        "Estatura: " +
                        latestMeasurement.Height.ToString("F2") +
                        " m";

                    lblBMI.Text =
                        "IMC: " +
                        latestMeasurement.CalculateBMI()
                            .ToString("F2");

                    lblDate.Text =
                        "Fecha: " +
                        latestMeasurement.MeasurementDate
                            .ToString("dd/MM/yyyy");
                }
                else
                {
                    lblWeight.Text = "Peso: Sin registros";
                    lblHeight.Text = "Estatura: Sin registros";
                    lblBMI.Text = "IMC: Sin registros";
                    lblDate.Text = "Fecha: Sin registros";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar el resumen fitness:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Actualizar información
        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            LoadSummary();
        }

        // Cerrar ventana
        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }

        // Centrar título
        private void CenterTitle()
        {
            lblTitle.Left =
                (ClientSize.Width - lblTitle.Width) / 2;
        }
    }
}
