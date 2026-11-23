
using System;
using System.Drawing;
using System.IO;
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

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

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
            ClientSize = new Size(900, 550);
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

            dgvMeasurements.Location = new Point(35, 165);
            dgvMeasurements.Size = new Size(830, 280);

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

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(295, 470);
            btnRefresh.Size = new Size(140, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(455, 470);
            btnClose.Size = new Size(140, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(lblTotal);
            Controls.Add(dgvMeasurements);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();

            ClientSizeChanged += (sender, e) =>
            {
                CenterTitle();
            };
        }

        // Cargar las mediciones del usuario
        private void LoadMeasurements()
        {
            try
            {
                dgvMeasurements.Rows.Clear();

                var measurements =
                    measurementsController.GetMeasurementsByUser(
                        currentUser.Id
                    );

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

        // Actualizar listado
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
