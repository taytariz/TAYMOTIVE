
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class RoutineProgressView : Form
    {
        private readonly User currentUser;
        private readonly RoutinesController routinesController;
        private readonly RoutineProgressController progressController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblRoutine = new Label();
        private readonly Label lblExercise = new Label();
        private readonly Label lblWeight = new Label();
        private readonly Label lblDate = new Label();
        private readonly Label lblTotal = new Label();
        private readonly Label lblChartExercise = new Label();
        private readonly Label lblWeightChange = new Label();

        private readonly ComboBox cmbRoutines = new ComboBox();
        private readonly ComboBox cmbChartExercises = new ComboBox();

        private readonly TextBox txtExercise = new TextBox();
        private readonly NumericUpDown nudWeight = new NumericUpDown();
        private readonly DateTimePicker dtpDate = new DateTimePicker();

        private readonly TabControl tabProgress = new TabControl();

        private readonly DataGridView dgvProgress =
            new DataGridView();

        private readonly Panel pnlChart = new Panel();

        private readonly Button btnSave = new Button();
        private readonly Button btnDelete = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        private List<RoutineProgress> chartRecords =
            new List<RoutineProgress>();

        public RoutineProgressView(User currentUser)
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

            progressController = new RoutineProgressController(
                Path.Combine(filesPath, "routine_progress.json")
            );

            InitializeComponents();
            LoadRoutines();
            LoadProgress();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Progreso de rutinas";
            ClientSize = new Size(1000, 680);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "PROGRESO DE RUTINAS";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(300, 25);

            lblRoutine.Text = "Rutina:";
            lblRoutine.Location = new Point(40, 100);
            lblRoutine.AutoSize = true;

            cmbRoutines.Location = new Point(130, 95);
            cmbRoutines.Size = new Size(300, 25);
            cmbRoutines.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbRoutines.DisplayMember = "Name";
            cmbRoutines.ValueMember = "Id";

            cmbRoutines.SelectedIndexChanged +=
                CmbRoutines_SelectedIndexChanged;

            lblExercise.Text = "Ejercicio:";
            lblExercise.Location = new Point(40, 145);
            lblExercise.AutoSize = true;

            txtExercise.Location = new Point(130, 140);
            txtExercise.Size = new Size(300, 25);

            lblWeight.Text = "Peso (kg):";
            lblWeight.Location = new Point(470, 100);
            lblWeight.AutoSize = true;

            nudWeight.Location = new Point(565, 95);
            nudWeight.Size = new Size(150, 25);
            nudWeight.DecimalPlaces = 2;
            nudWeight.Minimum = 0;
            nudWeight.Maximum = 1000;
            nudWeight.Increment = 0.50M;

            lblDate.Text = "Fecha:";
            lblDate.Location = new Point(470, 145);
            lblDate.AutoSize = true;

            dtpDate.Location = new Point(565, 140);
            dtpDate.Size = new Size(200, 25);
            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.CustomFormat = "dd/MM/yyyy";
            dtpDate.MaxDate = DateTime.Today;
            dtpDate.Value = DateTime.Today;

            btnSave.Text = "Registrar progreso";
            btnSave.Location = new Point(790, 95);
            btnSave.Size = new Size(165, 35);
            btnSave.Click += BtnSave_Click;

            lblTotal.Text = "Registros: 0";
            lblTotal.Location = new Point(40, 195);
            lblTotal.AutoSize = true;

            ConfigureTabs();

            btnDelete.Text = "Eliminar registro";
            btnDelete.Location = new Point(220, 605);
            btnDelete.Size = new Size(165, 40);
            btnDelete.Click += BtnDelete_Click;

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(420, 605);
            btnRefresh.Size = new Size(165, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(620, 605);
            btnClose.Size = new Size(165, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblRoutine);
            Controls.Add(cmbRoutines);
            Controls.Add(lblExercise);
            Controls.Add(txtExercise);
            Controls.Add(lblWeight);
            Controls.Add(nudWeight);
            Controls.Add(lblDate);
            Controls.Add(dtpDate);
            Controls.Add(btnSave);
            Controls.Add(lblTotal);
            Controls.Add(tabProgress);
            Controls.Add(btnDelete);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);
        }

        // Configurar historial y gráfica
        private void ConfigureTabs()
        {
            tabProgress.Location = new Point(40, 225);
            tabProgress.Size = new Size(915, 350);

            TabPage tabHistory = new TabPage(
                "Historial de pesos"
            );

            TabPage tabChart = new TabPage(
                "Gráfica de progreso"
            );

            dgvProgress.Dock = DockStyle.Fill;
            dgvProgress.ReadOnly = true;
            dgvProgress.AllowUserToAddRows = false;
            dgvProgress.AllowUserToDeleteRows = false;
            dgvProgress.MultiSelect = false;
            dgvProgress.RowHeadersVisible = false;

            dgvProgress.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvProgress.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvProgress.Columns.Add("Id", "ID");
            dgvProgress.Columns.Add("Routine", "Rutina");
            dgvProgress.Columns.Add("Exercise", "Ejercicio");
            dgvProgress.Columns.Add("Weight", "Peso (kg)");
            dgvProgress.Columns.Add("Date", "Fecha");

            tabHistory.Controls.Add(dgvProgress);

            lblChartExercise.Text = "Seleccionar ejercicio:";
            lblChartExercise.Location = new Point(20, 20);
            lblChartExercise.AutoSize = true;

            cmbChartExercises.Location = new Point(165, 15);
            cmbChartExercises.Size = new Size(240, 25);
            cmbChartExercises.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbChartExercises.SelectedIndexChanged +=
                CmbChartExercises_SelectedIndexChanged;

            lblWeightChange.Text =
                "Variación de peso: --";

            lblWeightChange.Location = new Point(425, 18);
            lblWeightChange.Size = new Size(450, 30);
            lblWeightChange.Font = new Font(
                "Arial",
                10,
                FontStyle.Bold
            );

            pnlChart.Location = new Point(20, 55);
            pnlChart.Size = new Size(850, 245);
            pnlChart.BackColor = Color.White;
            pnlChart.BorderStyle = BorderStyle.FixedSingle;

            pnlChart.Paint += PnlChart_Paint;

            tabChart.Controls.Add(lblChartExercise);
            tabChart.Controls.Add(cmbChartExercises);
            tabChart.Controls.Add(lblWeightChange);
            tabChart.Controls.Add(pnlChart);

            tabProgress.TabPages.Add(tabHistory);
            tabProgress.TabPages.Add(tabChart);
        }

        // Cargar las rutinas del usuario
        private void LoadRoutines()
        {
            var routines = routinesController
                .GetRoutinesByUser(currentUser.Id);

            cmbRoutines.DataSource = null;
            cmbRoutines.DataSource = routines;

            btnSave.Enabled = routines.Count > 0;
        }

        // Cargar el historial de una rutina
        private void LoadProgress()
        {
            dgvProgress.Rows.Clear();

            if (cmbRoutines.SelectedItem is not Routine routine)
            {
                lblTotal.Text = "Registros: 0";
                chartRecords.Clear();
                cmbChartExercises.DataSource = null;
                lblWeightChange.Text = "Variación de peso: --";
                pnlChart.Invalidate();
                return;
            }

            List<RoutineProgress> records = progressController
                .GetProgressByRoutine(
                    currentUser.Id,
                    routine.Id
                );

            var orderedRecords = records
                .OrderByDescending(
                    item => item.ProgressDate
                )
                .ThenByDescending(item => item.Id)
                .ToList();

            foreach (RoutineProgress progress in orderedRecords)
            {
                dgvProgress.Rows.Add(
                    progress.Id,
                    routine.Name,
                    progress.ExerciseName,
                    progress.Weight.ToString("F2"),
                    progress.ProgressDate.ToString("dd/MM/yyyy")
                );
            }

            lblTotal.Text = "Registros: " + records.Count;

            dgvProgress.ClearSelection();

            LoadChartExercises(records);
        }

        // Cargar los ejercicios que tienen registros
        private void LoadChartExercises(
            List<RoutineProgress> records)
        {
            string? previousExercise =
                cmbChartExercises.SelectedItem as string;

            var exercises = records
                .Select(item => item.ExerciseName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .ToList();

            cmbChartExercises.DataSource = null;
            cmbChartExercises.DataSource = exercises;

            if (!string.IsNullOrEmpty(previousExercise))
            {
                int index = exercises.FindIndex(
                    name => string.Equals(
                        name,
                        previousExercise,
                        StringComparison.OrdinalIgnoreCase
                    )
                );

                if (index >= 0)
                {
                    cmbChartExercises.SelectedIndex = index;
                }
            }

            UpdateChart();
        }

        // Actualizar los datos de la gráfica
        private void UpdateChart()
        {
            chartRecords.Clear();

            if (cmbRoutines.SelectedItem is not Routine routine ||
                cmbChartExercises.SelectedItem is not string exercise)
            {
                lblWeightChange.Text =
                    "Variación de peso: --";

                pnlChart.Invalidate();
                return;
            }

            chartRecords = progressController
                .GetProgressByRoutine(
                    currentUser.Id,
                    routine.Id
                )
                .Where(item => string.Equals(
                    item.ExerciseName,
                    exercise,
                    StringComparison.OrdinalIgnoreCase
                ))
                .OrderBy(item => item.ProgressDate)
                .ThenBy(item => item.Id)
                .ToList();

            if (chartRecords.Count >= 2)
            {
                double initialWeight = chartRecords[0].Weight;
                double finalWeight = chartRecords[chartRecords.Count - 1].Weight;

                double change = finalWeight - initialWeight;

                if (initialWeight > 0)
                {
                    double percentage =
                        (change / initialWeight) * 100;

                    lblWeightChange.Text =
                        $"Variación: {change:+0.00;-0.00;0.00} kg " +
                        $"({percentage:+0.00;-0.00;0.00}%)";
                }
                else
                {
                    lblWeightChange.Text =
                        $"Variación: {change:+0.00;-0.00;0.00} kg";
                }
            }
            else
            {
                lblWeightChange.Text =
                    "Variación: se necesitan 2 registros";
            }

            pnlChart.Invalidate();
        }

        // Dibujar la gráfica de pesos
        private void PnlChart_Paint(
            object? sender,
            PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            if (chartRecords.Count == 0)
            {
                graphics.DrawString(
                    "No hay registros para mostrar.",
                    new Font("Arial", 11),
                    Brushes.Gray,
                    new PointF(260, 105)
                );

                return;
            }

            int left = 70;
            int top = 25;
            int right = pnlChart.ClientSize.Width - 35;
            int bottom = pnlChart.ClientSize.Height - 45;

            int chartWidth = right - left;
            int chartHeight = bottom - top;

            double minWeight = chartRecords.Min(
                item => item.Weight
            );

            double maxWeight = chartRecords.Max(
                item => item.Weight
            );

            double range = maxWeight - minWeight;

            if (range == 0)
            {
                range = 1;
            }

            double margin = range * 0.20;

            double chartMin = Math.Max(
                0,
                minWeight - margin
            );

            double chartMax = maxWeight + margin;

            if (chartMax <= chartMin)
            {
                chartMax = chartMin + 1;
            }

            using (Pen axisPen = new Pen(Color.Gray, 1))
            using (Pen linePen = new Pen(Color.RoyalBlue, 3))
            using (Brush pointBrush = new SolidBrush(
                Color.RoyalBlue))
            using (Font labelFont = new Font("Arial", 9))
            {
                // Ejes
                graphics.DrawLine(
                    axisPen,
                    left,
                    top,
                    left,
                    bottom
                );

                graphics.DrawLine(
                    axisPen,
                    left,
                    bottom,
                    right,
                    bottom
                );

                // Líneas horizontales y valores
                for (int i = 0; i <= 4; i++)
                {
                    float y = bottom -
                        (chartHeight * i / 4f);

                    double value = chartMin +
                        ((chartMax - chartMin) * i / 4.0);

                    using (Pen gridPen = new Pen(
                        Color.LightGray,
                        1))
                    {
                        graphics.DrawLine(
                            gridPen,
                            left,
                            y,
                            right,
                            y
                        );
                    }

                    graphics.DrawString(
                        value.ToString("F1"),
                        labelFont,
                        Brushes.Black,
                        15,
                        y - 8
                    );
                }

                List<PointF> points =
                    new List<PointF>();

                int count = chartRecords.Count;

                for (int i = 0; i < count; i++)
                {
                    RoutineProgress record = chartRecords[i];

                    float x = count == 1
                        ? left + chartWidth / 2f
                        : left + chartWidth * i / (count - 1f);

                    float y = bottom -
                        (float)(
                            (record.Weight - chartMin) /
                            (chartMax - chartMin)
                        ) * chartHeight;

                    points.Add(new PointF(x, y));
                }

                // Unir los registros
                if (points.Count >= 2)
                {
                    graphics.DrawLines(
                        linePen,
                        points.ToArray()
                    );
                }

                // Dibujar puntos y etiquetas
                for (int i = 0; i < points.Count; i++)
                {
                    PointF point = points[i];
                    RoutineProgress record = chartRecords[i];

                    graphics.FillEllipse(
                        pointBrush,
                        point.X - 5,
                        point.Y - 5,
                        10,
                        10
                    );

                    graphics.DrawString(
                        record.Weight.ToString("F2") + " kg",
                        labelFont,
                        Brushes.Black,
                        point.X - 20,
                        Math.Max(0, point.Y - 23)
                    );

                    // Etiqueta por número de registro
                    // para distinguir registros del mismo día.
                    graphics.DrawString(
                        (i + 1).ToString(),
                        labelFont,
                        Brushes.Black,
                        point.X - 4,
                        bottom + 8
                    );
                }

                graphics.DrawString(
                    "Registros en orden cronológico",
                    labelFont,
                    Brushes.Gray,
                    right - 210,
                    bottom + 25
                );
            }
        }

        // Registrar progreso
        private void BtnSave_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                if (cmbRoutines.SelectedItem is not Routine routine)
                {
                    MessageBox.Show(
                        "Debe seleccionar una rutina."
                    );
                    return;
                }

                RoutineProgress progress = new RoutineProgress
                {
                    UserId = currentUser.Id,
                    RoutineId = routine.Id,
                    ExerciseName = txtExercise.Text,
                    Weight = (double)nudWeight.Value,
                    ProgressDate = dtpDate.Value.Date
                };

                progressController.AddProgress(progress);

                MessageBox.Show(
                    "Progreso registrado correctamente.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                txtExercise.Clear();
                nudWeight.Value = 0;

                LoadProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al registrar progreso: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Eliminar registro
        private void BtnDelete_Click(
            object? sender,
            EventArgs e)
        {
            if (dgvProgress.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un registro del historial."
                );
                return;
            }

            int progressId = Convert.ToInt32(
                dgvProgress.SelectedRows[0].Cells["Id"].Value
            );

            DialogResult confirmation = MessageBox.Show(
                "¿Desea eliminar este registro?",
                "TAYMOTIVE",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool deleted = progressController.DeleteProgress(
                    progressId,
                    currentUser.Id
                );

                if (deleted)
                {
                    LoadProgress();
                }
                else
                {
                    MessageBox.Show(
                        "No se encontró el registro."
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar registro: " + ex.Message,
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
            try
            {
                LoadRoutines();
                LoadProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al actualizar: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Cambiar rutina
        private void CmbRoutines_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            LoadProgress();
        }

        // Cambiar ejercicio de la gráfica
        private void CmbChartExercises_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            UpdateChart();
        }

        // Cerrar pantalla
        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }
    }
}

