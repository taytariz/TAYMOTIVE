
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class ReportsView : Form
    {
        private readonly ReportsController reportsController;
        private readonly UsersController usersController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblSummary = new Label();
        private readonly Label lblStartDate = new Label();
        private readonly Label lblEndDate = new Label();

        private readonly DateTimePicker dtpStart = new DateTimePicker();
        private readonly DateTimePicker dtpEnd = new DateTimePicker();

        private readonly Button btnGenerate = new Button();
        private readonly Button btnClose = new Button();

        private readonly TabControl tabReports = new TabControl();

        private readonly DataGridView dgvTopVisitors =
            new DataGridView();

        private readonly DataGridView dgvLeastVisitors =
            new DataGridView();

        private readonly DataGridView dgvClasses =
            new DataGridView();

        private readonly DataGridView dgvProgress =
            new DataGridView();

        private readonly ComboBox cmbUsers = new ComboBox();

        private readonly Label lblSelectUser = new Label();
        private readonly Label lblWeightChange = new Label();
        private readonly Label lblBMIChange = new Label();

        public ReportsView()
        {
            string filesPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files"
            );

            reportsController = new ReportsController(
                Path.Combine(filesPath, "users.json"),
                Path.Combine(filesPath, "trainers.json"),
                Path.Combine(filesPath, "measurements.json"),
                Path.Combine(filesPath, "visits.json")
            );

            usersController = new UsersController(
                Path.Combine(filesPath, "users.json")
            );

            InitializeComponents();
            LoadUsers();
            GenerateReports();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Reportes y estadísticas";

            Size = new Size(1050, 700);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(900, 600);

            lblTitle.Text = "REPORTES Y ESTADÍSTICAS";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(300, 25);

            lblSummary.Location = new Point(35, 90);
            lblSummary.Size = new Size(950, 60);
            lblSummary.Font = new Font("Arial", 11);
            lblSummary.TextAlign = ContentAlignment.MiddleCenter;

            lblStartDate.Text = "Fecha inicio:";
            lblStartDate.Location = new Point(90, 170);
            lblStartDate.AutoSize = true;

            dtpStart.Location = new Point(185, 165);
            dtpStart.Size = new Size(200, 25);
            dtpStart.Format = DateTimePickerFormat.Short;
            dtpStart.Value = DateTime.Today.AddMonths(-1);

            lblEndDate.Text = "Fecha fin:";
            lblEndDate.Location = new Point(420, 170);
            lblEndDate.AutoSize = true;

            dtpEnd.Location = new Point(495, 165);
            dtpEnd.Size = new Size(200, 25);
            dtpEnd.Format = DateTimePickerFormat.Short;
            dtpEnd.Value = DateTime.Today;

            btnGenerate.Text = "Generar reportes";
            btnGenerate.Location = new Point(740, 160);
            btnGenerate.Size = new Size(180, 35);
            btnGenerate.Click += BtnGenerate_Click;

            tabReports.Location = new Point(35, 220);
            tabReports.Size = new Size(950, 350);

            tabReports.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            TabPage tabTop = new TabPage("Top 10 visitantes");
            TabPage tabLeast = new TabPage("Top 10 ausentes");
            TabPage tabClasses = new TabPage("Visitas por clase");
            TabPage tabProgress = new TabPage("Progreso físico");

            ConfigureGrid(dgvTopVisitors);
            ConfigureGrid(dgvLeastVisitors);
            ConfigureGrid(dgvClasses);
            ConfigureGrid(dgvProgress);

            dgvTopVisitors.Columns.Add("UserId", "ID");
            dgvTopVisitors.Columns.Add("UserName", "Usuario");
            dgvTopVisitors.Columns.Add("Total", "Total de visitas");

            dgvLeastVisitors.Columns.Add("UserId", "ID");
            dgvLeastVisitors.Columns.Add("UserName", "Usuario");
            dgvLeastVisitors.Columns.Add("Total", "Total de visitas");

            dgvClasses.Columns.Add("Class", "Clase");
            dgvClasses.Columns.Add("Total", "Total de visitas");

            dgvProgress.Columns.Add("Id", "ID");
            dgvProgress.Columns.Add("Date", "Fecha");
            dgvProgress.Columns.Add("Weight", "Peso (kg)");
            dgvProgress.Columns.Add("Height", "Estatura (m)");
            dgvProgress.Columns.Add("BMI", "IMC");

            tabTop.Controls.Add(dgvTopVisitors);
            tabLeast.Controls.Add(dgvLeastVisitors);
            tabClasses.Controls.Add(dgvClasses);

            ConfigureProgressTab(tabProgress);

            tabReports.TabPages.Add(tabTop);
            tabReports.TabPages.Add(tabLeast);
            tabReports.TabPages.Add(tabClasses);
            tabReports.TabPages.Add(tabProgress);

            btnClose.Text = "Cerrar";
            btnClose.Size = new Size(180, 40);
            btnClose.Location = new Point(420, 590);
            btnClose.Anchor =
                AnchorStyles.Bottom | AnchorStyles.Left;
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblSummary);
            Controls.Add(lblStartDate);
            Controls.Add(dtpStart);
            Controls.Add(lblEndDate);
            Controls.Add(dtpEnd);
            Controls.Add(btnGenerate);
            Controls.Add(tabReports);
            Controls.Add(btnClose);
        }

        private void ConfigureGrid(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;
            grid.MultiSelect = false;

            grid.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            grid.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Organizar correctamente los controles
        // de la pestaña Progreso físico.
        private void ConfigureProgressTab(TabPage tabProgress)
        {
            Panel panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 100;
            panelHeader.BackColor = Color.WhiteSmoke;

            lblSelectUser.Text = "Seleccionar usuario:";
            lblSelectUser.Location = new Point(15, 20);
            lblSelectUser.AutoSize = true;

            cmbUsers.Location = new Point(160, 15);
            cmbUsers.Size = new Size(230, 25);
            cmbUsers.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbUsers.DisplayMember = "Name";
            cmbUsers.ValueMember = "Id";

            cmbUsers.SelectedIndexChanged +=
                CmbUsers_SelectedIndexChanged;

            lblWeightChange.Text =
                "Variación de peso: --";

            lblWeightChange.Location =
                new Point(420, 15);

            lblWeightChange.Size =
                new Size(450, 30);

            lblWeightChange.Font = new Font(
                "Arial",
                10,
                FontStyle.Bold
            );

            lblBMIChange.Text =
                "Variación de IMC: --";

            lblBMIChange.Location =
                new Point(420, 50);

            lblBMIChange.Size =
                new Size(450, 30);

            lblBMIChange.Font = new Font(
                "Arial",
                10,
                FontStyle.Bold
            );

            panelHeader.Controls.Add(lblSelectUser);
            panelHeader.Controls.Add(cmbUsers);
            panelHeader.Controls.Add(lblWeightChange);
            panelHeader.Controls.Add(lblBMIChange);

            Panel panelGrid = new Panel();
            panelGrid.Dock = DockStyle.Fill;
            panelGrid.Padding = new Padding(10);

            panelGrid.Controls.Add(dgvProgress);

            // Primero el panel de datos.
            tabProgress.Controls.Add(panelGrid);

            // Después el encabezado.
            // Así ambos conservan su espacio.
            tabProgress.Controls.Add(panelHeader);
        }

        private void LoadUsers()
        {
            try
            {
                var users = usersController.GetUsers();

                cmbUsers.DataSource = null;
                cmbUsers.DataSource = users;

                if (users.Count > 0)
                {
                    cmbUsers.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar usuarios: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void GenerateReports()
        {
            DateTime startDate = dtpStart.Value.Date;
            DateTime endDate = dtpEnd.Value.Date;

            if (startDate > endDate)
            {
                MessageBox.Show(
                    "La fecha inicial no puede ser mayor que la final.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                GeneralReport summary =
                    reportsController.GetGeneralReport();

                lblSummary.Text =
                    $"Usuarios: {summary.TotalUsers}     " +
                    $"Entrenadores: {summary.TotalTrainers}     " +
                    $"Rutinas: {summary.TotalRoutines}\n" +
                    $"Mediciones: {summary.TotalMeasurements}     " +
                    $"Visitas: {summary.TotalVisits}";

                dgvTopVisitors.Rows.Clear();

                var topVisitors =
                    reportsController.GetTopVisitors(
                        startDate,
                        endDate
                    );

                foreach (var report in topVisitors)
                {
                    dgvTopVisitors.Rows.Add(
                        report.UserId,
                        report.UserName,
                        report.TotalVisits
                    );
                }

                dgvLeastVisitors.Rows.Clear();

                var leastVisitors =
                    reportsController.GetLeastVisitors(
                        startDate,
                        endDate
                    );

                foreach (var report in leastVisitors)
                {
                    dgvLeastVisitors.Rows.Add(
                        report.UserId,
                        report.UserName,
                        report.TotalVisits
                    );
                }

                dgvClasses.Rows.Clear();

                var classReports =
                    reportsController.GetVisitsByClass(
                        startDate,
                        endDate
                    );

                foreach (var report in classReports)
                {
                    dgvClasses.Rows.Add(
                        report.ClassName,
                        report.TotalVisits
                    );
                }

                LoadProgress();

                dgvTopVisitors.ClearSelection();
                dgvLeastVisitors.ClearSelection();
                dgvClasses.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al generar reportes: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadProgress()
        {
            dgvProgress.Rows.Clear();

            if (cmbUsers.SelectedItem is not User selectedUser)
            {
                lblWeightChange.Text =
                    "Variación de peso: --";

                lblBMIChange.Text =
                    "Variación de IMC: --";

                return;
            }

            DateTime startDate = dtpStart.Value.Date;
            DateTime endDate = dtpEnd.Value.Date;

            var measurements =
                reportsController.GetUserMeasurements(
                    selectedUser.Id,
                    startDate,
                    endDate
                )
                .OrderBy(m => m.MeasurementDate)
                .ThenBy(m => m.Id)
                .ToList();

            foreach (var measurement in measurements)
            {
                dgvProgress.Rows.Add(
                    measurement.Id,
                    measurement.MeasurementDate.ToString("dd/MM/yyyy"),
                    measurement.Weight.ToString("F2"),
                    measurement.Height.ToString("F2"),
                    measurement.CalculateBMI().ToString("F2")
                );
            }

            if (measurements.Count >= 2)
            {
                double initialWeight =
                    measurements[0].Weight;

                double finalWeight =
                    measurements[measurements.Count - 1].Weight;

                double weightChange =
                    finalWeight - initialWeight;

                double initialBMI =
                    measurements[0].CalculateBMI();

                double finalBMI =
                    measurements[measurements.Count - 1]
                        .CalculateBMI();

                double bmiChange =
                    finalBMI - initialBMI;

                lblWeightChange.Text =
                    $"Variación de peso: " +
                    $"{weightChange:+0.00;-0.00;0.00} kg";

                lblBMIChange.Text =
                    $"Variación de IMC: " +
                    $"{bmiChange:+0.00;-0.00;0.00}";
            }
            else
            {
                lblWeightChange.Text =
                    "Variación de peso: datos insuficientes";

                lblBMIChange.Text =
                    "Variación de IMC: datos insuficientes";
            }

            dgvProgress.ClearSelection();
        }

        private void CmbUsers_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            if (dtpStart.Value.Date <= dtpEnd.Value.Date)
            {
                try
                {
                    LoadProgress();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar progreso: " + ex.Message,
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void BtnGenerate_Click(
            object? sender,
            EventArgs e)
        {
            GenerateReports();
        }

        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }
    }
}
