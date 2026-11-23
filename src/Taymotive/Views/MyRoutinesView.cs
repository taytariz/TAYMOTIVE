
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class MyRoutinesView : Form
    {
        private readonly User currentUser;
        private readonly RoutinesController routinesController;
        private readonly TrainersController trainersController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblUsername = new Label();
        private readonly Label lblTotal = new Label();

        private readonly TabControl tabRoutines =
            new TabControl();

        private readonly DataGridView dgvRoutines =
            new DataGridView();

        private readonly DataGridView dgvLastFive =
            new DataGridView();

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        public MyRoutinesView(User currentUser)
        {
            this.currentUser = currentUser ??
                throw new ArgumentNullException(
                    nameof(currentUser)
                );

            string filesPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files"
            );

            routinesController = new RoutinesController(
                Path.Combine(filesPath, "users.json")
            );

            trainersController = new TrainersController(
                Path.Combine(filesPath, "trainers.json")
            );

            InitializeComponents();
            LoadRoutines();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Mis rutinas";
            ClientSize = new Size(1050, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "MIS RUTINAS";
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
            lblUsername.Location = new Point(35, 95);
            lblUsername.AutoSize = true;

            lblTotal.Text = "Total de rutinas: 0";
            lblTotal.Location = new Point(35, 130);
            lblTotal.AutoSize = true;

            tabRoutines.Location = new Point(35, 165);
            tabRoutines.Size = new Size(980, 340);

            TabPage tabAll = new TabPage(
                "Todas mis rutinas"
            );

            TabPage tabLastFive = new TabPage(
                "Últimas 5 rutinas"
            );

            ConfigureGrid(dgvRoutines);
            ConfigureGrid(dgvLastFive);

            tabAll.Controls.Add(dgvRoutines);
            tabLastFive.Controls.Add(dgvLastFive);

            tabRoutines.TabPages.Add(tabAll);
            tabRoutines.TabPages.Add(tabLastFive);

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(365, 530);
            btnRefresh.Size = new Size(140, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(545, 530);
            btnClose.Size = new Size(140, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(lblTotal);
            Controls.Add(tabRoutines);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();
        }

        // Configurar las columnas de las tablas
        private static void ConfigureGrid(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;

            grid.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            grid.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Name", "Rutina");
            grid.Columns.Add("Description", "Descripción");
            grid.Columns.Add("Duration", "Duración (min)");
            grid.Columns.Add("Trainer", "Entrenador");
            grid.Columns.Add("StartDate", "Fecha inicio");
            grid.Columns.Add("EndDate", "Fecha fin");
        }

        // Cargar las rutinas del usuario
        private void LoadRoutines()
        {
            try
            {
                dgvRoutines.Rows.Clear();
                dgvLastFive.Rows.Clear();

                List<Routine> routines =
                    routinesController.GetRoutinesByUser(
                        currentUser.Id
                    );

                List<Trainer> trainers =
                    trainersController.GetTrainers();

                // Mostrar todas las rutinas
                foreach (Routine routine in routines)
                {
                    AddRoutineRow(
                        dgvRoutines,
                        routine,
                        trainers
                    );
                }

                // Obtener las últimas cinco rutinas
                List<Routine> lastFive = routines
                    .OrderByDescending(
                        routine => routine.StartDate
                    )
                    .ThenByDescending(
                        routine => routine.Id
                    )
                    .Take(5)
                    .ToList();

                foreach (Routine routine in lastFive)
                {
                    AddRoutineRow(
                        dgvLastFive,
                        routine,
                        trainers
                    );
                }

                lblTotal.Text =
                    "Total de rutinas: " + routines.Count;

                dgvRoutines.ClearSelection();
                dgvLastFive.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar las rutinas:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Agregar una rutina a la tabla
        private static void AddRoutineRow(
            DataGridView grid,
            Routine routine,
            List<Trainer> trainers)
        {
            Trainer? trainer = trainers.FirstOrDefault(
                item => item.Id == routine.TrainerId
            );

            string trainerName = trainer == null
                ? "Sin asignar"
                : trainer.Name;

            grid.Rows.Add(
                routine.Id,
                routine.Name,
                routine.Description,
                routine.Duration,
                trainerName,
                routine.StartDate.ToString("dd/MM/yyyy"),
                routine.EndDate.ToString("dd/MM/yyyy")
            );
        }

        // Actualizar las rutinas
        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            LoadRoutines();
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
