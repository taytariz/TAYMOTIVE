
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
    public class MyMeasurementsView : Form
    {
        private readonly User currentUser;
        private readonly MeasurementsController measurementsController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblUsername = new Label();
        private readonly Label lblTotal = new Label();

        private readonly DataGridView dgvMeasurements =
            new DataGridView();

        private readonly GroupBox grpWeight = new GroupBox();
        private readonly GroupBox grpBMI = new GroupBox();

        private readonly Panel pnlWeight = new Panel();
        private readonly Panel pnlBMI = new Panel();

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        private List<Measurement> measurements =
            new List<Measurement>();

        public MyMeasurementsView(User currentUser)
        {
            this.currentUser = currentUser ??
                throw new ArgumentNullException(
                    nameof(currentUser)
                );

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "measurements.json"
            );

            measurementsController =
                new MeasurementsController(filePath);

            InitializeComponents();
            LoadMeasurements();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Mi progreso físico";
            ClientSize = new Size(1050, 850);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "MI PROGRESO FÍSICO";
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

            lblTotal.Text = "Total de mediciones: 0";
            lblTotal.Location = new Point(35, 130);
            lblTotal.AutoSize = true;

            // TABLA DE MEDICIONES

            dgvMeasurements.Location = new Point(35, 165);
            dgvMeasurements.Size = new Size(980, 220);

            dgvMeasurements.ReadOnly = true;
            dgvMeasurements.AllowUserToAddRows = false;
            dgvMeasurements.AllowUserToDeleteRows = false;
            dgvMeasurements.MultiSelect = false;
            dgvMeasurements.RowHeadersVisible = false;

            dgvMeasurements.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvMeasurements.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvMeasurements.Columns.Add("Id", "ID");
            dgvMeasurements.Columns.Add("Date", "Fecha");
            dgvMeasurements.Columns.Add("Weight", "Peso (kg)");
            dgvMeasurements.Columns.Add("Height", "Estatura (m)");
            dgvMeasurements.Columns.Add("BMI", "IMC");

            dgvMeasurements.Columns["Id"]!.FillWeight = 40;
            dgvMeasurements.Columns["Date"]!.FillWeight = 120;
            dgvMeasurements.Columns["Weight"]!.FillWeight = 100;
            dgvMeasurements.Columns["Height"]!.FillWeight = 100;
            dgvMeasurements.Columns["BMI"]!.FillWeight = 100;

            // GRÁFICO DE PESO

            grpWeight.Text = "Evolución del peso (kg)";
            grpWeight.Font = new Font(
                "Arial",
                11,
                FontStyle.Bold
            );
            grpWeight.Location = new Point(35, 405);
            grpWeight.Size = new Size(475, 340);

            pnlWeight.Location = new Point(15, 30);
            pnlWeight.Size = new Size(445, 290);
            pnlWeight.BackColor = Color.White;

            pnlWeight.Paint += PnlWeight_Paint;

            grpWeight.Controls.Add(pnlWeight);

            // GRÁFICO DE IMC

            grpBMI.Text = "Evolución del IMC";
            grpBMI.Font = new Font(
                "Arial",
                11,
                FontStyle.Bold
            );
            grpBMI.Location = new Point(540, 405);
            grpBMI.Size = new Size(475, 340);

            pnlBMI.Location = new Point(15, 30);
            pnlBMI.Size = new Size(445, 290);
            pnlBMI.BackColor = Color.White;

            pnlBMI.Paint += PnlBMI_Paint;

            grpBMI.Controls.Add(pnlBMI);

            // BOTONES

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(370, 775);
            btnRefresh.Size = new Size(140, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(540, 775);
            btnClose.Size = new Size(140, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(lblTotal);
            Controls.Add(dgvMeasurements);
            Controls.Add(grpWeight);
            Controls.Add(grpBMI);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();

            ClientSizeChanged += (sender, e) =>
            {
                CenterTitle();
            };
        }

        // Cargar mediciones del usuario
        private void LoadMeasurements()
        {
            try
            {
                dgvMeasurements.Rows.Clear();

                measurements =
                    measurementsController
                        .GetMeasurementsByUser(currentUser.Id)
                        .OrderBy(m => m.MeasurementDate)
                        .ThenBy(m => m.Id)
                        .ToList();

                foreach (Measurement measurement in measurements)
                {
                    dgvMeasurements.Rows.Add(
                        measurement.Id,
                        measurement.MeasurementDate.ToString(
                            "dd/MM/yyyy"
                        ),
                        measurement.Weight.ToString("F2"),
                        measurement.Height.ToString("F2"),
                        measurement.CalculateBMI().ToString("F2")
                    );
                }

                lblTotal.Text =
                    "Total de mediciones: " +
                    measurements.Count;

                dgvMeasurements.ClearSelection();

                // Actualizar ambos gráficos
                pnlWeight.Invalidate();
                pnlBMI.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar las mediciones:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Dibujar gráfico de peso
        private void PnlWeight_Paint(
            object? sender,
            PaintEventArgs e)
        {
            DrawChart(
                e.Graphics,
                pnlWeight.ClientRectangle,
                measurements,
                m => m.Weight,
                "kg",
                Color.RoyalBlue
            );
        }

        // Dibujar gráfico de IMC
        private void PnlBMI_Paint(
            object? sender,
            PaintEventArgs e)
        {
            DrawChart(
                e.Graphics,
                pnlBMI.ClientRectangle,
                measurements,
                m => m.CalculateBMI(),
                "IMC",
                Color.ForestGreen
            );
        }

        // Dibujar gráfico de evolución
        private void DrawChart(
            Graphics graphics,
            Rectangle area,
            List<Measurement> data,
            Func<Measurement, double> getValue,
            string unit,
            Color lineColor)
        {
            graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            graphics.Clear(Color.White);

            if (data.Count == 0)
            {
                using (Font font = new Font("Arial", 10))
                {
                    graphics.DrawString(
                        "No hay mediciones registradas.",
                        font,
                        Brushes.Gray,
                        60,
                        120
                    );
                }

                return;
            }

            // Espacio disponible para el gráfico
            int left = 60;
            int top = 25;
            int right = area.Width - 20;
            int bottom = area.Height - 55;

            int chartWidth = right - left;
            int chartHeight = bottom - top;

            List<double> values = data
                .Select(getValue)
                .ToList();

            double minValue = values.Min();
            double maxValue = values.Max();

            // Agregar margen vertical
            double difference = maxValue - minValue;

            double margin = difference > 0
                ? difference * 0.15
                : Math.Max(Math.Abs(maxValue) * 0.05, 1);

            minValue -= margin;
            maxValue += margin;

            // Dibujar ejes
            using (Pen axisPen = new Pen(
                Color.Gray,
                1))
            {
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
            }

            using (Font font = new Font("Arial", 8))
            {
                // Líneas horizontales y valores
                for (int i = 0; i <= 4; i++)
                {
                    float y = top +
                        (chartHeight * i / 4f);

                    double value = maxValue -
                        ((maxValue - minValue) * i / 4.0);

                    using (Pen gridPen = new Pen(
                        Color.LightGray,
                        1))
                    {
                        gridPen.DashStyle =
                            DashStyle.Dash;

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
                        font,
                        Brushes.DimGray,
                        5,
                        y - 7
                    );
                }

                graphics.DrawString(
                    unit,
                    font,
                    Brushes.DimGray,
                    5,
                    5
                );
            }

            // Calcular puntos del gráfico
            List<PointF> points = new List<PointF>();

            for (int i = 0; i < data.Count; i++)
            {
                float x;

                if (data.Count == 1)
                {
                    x = left + chartWidth / 2f;
                }
                else
                {
                    x = left +
                        (chartWidth * i /
                        (float)(data.Count - 1));
                }

                float y = bottom -
                    (float)(
                        (values[i] - minValue) /
                        (maxValue - minValue) *
                        chartHeight
                    );

                points.Add(new PointF(x, y));
            }

            // Dibujar línea de evolución
            if (points.Count >= 2)
            {
                using (Pen linePen = new Pen(
                    lineColor,
                    2.5f))
                {
                    graphics.DrawLines(
                        linePen,
                        points.ToArray()
                    );
                }
            }

            // Dibujar puntos y etiquetas
            using (Brush pointBrush =
                new SolidBrush(lineColor))
            using (Font font = new Font("Arial", 8))
            {
                for (int i = 0; i < points.Count; i++)
                {
                    PointF point = points[i];

                    graphics.FillEllipse(
                        pointBrush,
                        point.X - 4,
                        point.Y - 4,
                        8,
                        8
                    );

                    // Mostrar fechas en puntos representativos
                    if (i == 0 ||
                        i == points.Count - 1 ||
                        (points.Count <= 5))
                    {
                        string date =
                            data[i].MeasurementDate
                                .ToString("dd/MM");

                        graphics.DrawString(
                            date,
                            font,
                            Brushes.DimGray,
                            point.X - 17,
                            bottom + 10
                        );
                    }
                }
            }
        }

        // Actualizar mediciones y gráficos
        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            LoadMeasurements();
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
