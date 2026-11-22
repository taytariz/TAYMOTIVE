
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class MeasurementsView : Form
    {
        private readonly Label lblTitle = new Label();
        private readonly Label lblTotal = new Label();

        private readonly DataGridView dgvMeasurements = new DataGridView();

        private readonly Button btnRegister = new Button();
        private readonly Button btnEdit = new Button();
        private readonly Button btnDelete = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        private readonly MeasurementsController measurementsController;
        private readonly UsersController usersController;

        public MeasurementsView()
        {
            string measurementsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "measurements.json"
            );

            string usersPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            measurementsController =
                new MeasurementsController(measurementsPath);

            usersController = new UsersController(usersPath);

            InitializeComponents();
            LoadMeasurements();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Gestión de mediciones";
            this.Size = new Size(1050, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "GESTIÓN DE MEDICIONES";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(325, 30);

            lblTotal.Text = "Total de mediciones: 0";
            lblTotal.Location = new Point(35, 100);
            lblTotal.AutoSize = true;

            dgvMeasurements.Location = new Point(35, 135);
            dgvMeasurements.Size = new Size(960, 330);
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
            dgvMeasurements.Columns.Add("User", "Usuario");
            dgvMeasurements.Columns.Add("Date", "Fecha");
            dgvMeasurements.Columns.Add("Weight", "Peso (kg)");
            dgvMeasurements.Columns.Add("Height", "Estatura (m)");
            dgvMeasurements.Columns.Add("BMI", "IMC");

            btnRegister.Text = "Registrar medición";
            btnRegister.Location = new Point(35, 495);
            btnRegister.Size = new Size(180, 40);
            btnRegister.Click += BtnRegister_Click;

            btnEdit.Text = "Modificar medición";
            btnEdit.Location = new Point(230, 495);
            btnEdit.Size = new Size(180, 40);
            btnEdit.Click += BtnEdit_Click;

            btnDelete.Text = "Eliminar medición";
            btnDelete.Location = new Point(425, 495);
            btnDelete.Size = new Size(180, 40);
            btnDelete.Click += BtnDelete_Click;

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(620, 495);
            btnRefresh.Size = new Size(180, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(815, 495);
            btnClose.Size = new Size(180, 40);
            btnClose.Click += BtnClose_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblTotal);
            this.Controls.Add(dgvMeasurements);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClose);
        }

        private void LoadMeasurements()
        {
            try
            {
                dgvMeasurements.Rows.Clear();

                var measurements =
                    measurementsController.GetMeasurements();

                var users = usersController.GetUsers();

                foreach (var measurement in measurements)
                {
                    var user = users.Find(
                        u => u.Id == measurement.UserId
                    );

                    string userName = user != null
                        ? user.Name
                        : "Usuario no encontrado";

                    dgvMeasurements.Rows.Add(
                        measurement.Id,
                        userName,
                        measurement.MeasurementDate.ToString("dd/MM/yyyy"),
                        measurement.Weight.ToString("F2"),
                        measurement.Height.ToString("F2"),
                        measurement.CalculateBMI().ToString("F2")
                    );
                }

                lblTotal.Text =
                    "Total de mediciones: " + measurements.Count;

                dgvMeasurements.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar mediciones: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            using (RegisterMeasurementView registerView =
                new RegisterMeasurementView())
            {
                if (registerView.ShowDialog(this) == DialogResult.OK)
                {
                    LoadMeasurements();
                }
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvMeasurements.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione una medición para modificar.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            int measurementId = Convert.ToInt32(
                dgvMeasurements.SelectedRows[0].Cells["Id"].Value
            );

            using (EditMeasurementView editView =
                new EditMeasurementView(measurementId))
            {
                if (editView.ShowDialog(this) == DialogResult.OK)
                {
                    LoadMeasurements();
                }
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvMeasurements.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione una medición para eliminar.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            int measurementId = Convert.ToInt32(
                dgvMeasurements.SelectedRows[0].Cells["Id"].Value
            );

            string userName = Convert.ToString(
                dgvMeasurements.SelectedRows[0].Cells["User"].Value
            ) ?? "Usuario desconocido";

            DialogResult confirmation = MessageBox.Show(
                "¿Está seguro de que desea eliminar esta medición?\n\n" +
                "ID: " + measurementId + "\n" +
                "Usuario: " + userName + "\n\n" +
                "Esta acción no se puede deshacer.",
                "TAYMOTIVE - Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
            );

            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool deleted =
                    measurementsController.DeleteMeasurement(
                        measurementId
                    );

                if (deleted)
                {
                    MessageBox.Show(
                        "Medición eliminada correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadMeasurements();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo eliminar la medición.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar medición: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadMeasurements();
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}
