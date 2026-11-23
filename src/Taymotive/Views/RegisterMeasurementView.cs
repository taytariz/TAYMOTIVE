
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class RegisterMeasurementView : Form
    {
        private readonly Label lblTitle = new Label();
        private readonly Label lblBMI = new Label();

        private readonly ComboBox cmbUsers = new ComboBox();
        private readonly DateTimePicker dtpDate = new DateTimePicker();
        private readonly NumericUpDown numWeight = new NumericUpDown();
        private readonly NumericUpDown numHeight = new NumericUpDown();

        private readonly Button btnSave = new Button();
        private readonly Button btnCancel = new Button();

        private readonly MeasurementsController measurementsController;
        private readonly UsersController usersController;

        public RegisterMeasurementView()
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
            LoadUsers();
            CalculateBMI();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Registrar medición";
            this.Size = new Size(550, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "REGISTRAR MEDICIÓN";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(100, 30);

            AddLabel("Usuario:", 65, 110);

            cmbUsers.Location = new Point(220, 105);
            cmbUsers.Width = 250;
            cmbUsers.DropDownStyle = ComboBoxStyle.DropDownList;

            AddLabel("Fecha:", 65, 160);

            dtpDate.Location = new Point(220, 155);
            dtpDate.Width = 250;
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Value = DateTime.Today;

            AddLabel("Peso (kg):", 65, 210);

            numWeight.Location = new Point(220, 205);
            numWeight.Width = 250;
            numWeight.DecimalPlaces = 2;
            numWeight.Minimum = 1;
            numWeight.Maximum = 500;
            numWeight.Value = 70;

            AddLabel("Estatura (m):", 65, 260);

            numHeight.Location = new Point(220, 255);
            numHeight.Width = 250;
            numHeight.DecimalPlaces = 2;
            numHeight.Increment = 0.01M;
            numHeight.Minimum = 0.50M;
            numHeight.Maximum = 2.50M;
            numHeight.Value = 1.70M;

            AddLabel("IMC calculado:", 65, 310);

            lblBMI.Text = "0.00";
            lblBMI.Location = new Point(220, 310);
            lblBMI.AutoSize = true;
            lblBMI.Font = new Font(
                "Arial",
                12,
                FontStyle.Bold
            );

            numWeight.ValueChanged += Measurement_ValueChanged;
            numHeight.ValueChanged += Measurement_ValueChanged;

            btnSave.Text = "Guardar";
            btnSave.Location = new Point(115, 365);
            btnSave.Size = new Size(140, 40);
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancelar";
            btnCancel.Location = new Point(285, 365);
            btnCancel.Size = new Size(140, 40);
            btnCancel.Click += BtnCancel_Click;

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblBMI);
            this.Controls.Add(cmbUsers);
            this.Controls.Add(dtpDate);
            this.Controls.Add(numWeight);
            this.Controls.Add(numHeight);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label label = new Label();

            label.Text = text;
            label.Location = new Point(x, y);
            label.AutoSize = true;

            this.Controls.Add(label);
        }

        private void LoadUsers()
        {
            try
            {
                cmbUsers.Items.Clear();

                var users = usersController.GetUsers();

                foreach (var user in users)
                {
                    cmbUsers.Items.Add(
                        new UserOption(user.Id, user.Name)
                    );
                }

                if (cmbUsers.Items.Count > 0)
                {
                    cmbUsers.SelectedIndex = 0;
                }
                else
                {
                    btnSave.Enabled = false;

                    MessageBox.Show(
                        "No existen usuarios registrados.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                btnSave.Enabled = false;

                MessageBox.Show(
                    "Error al cargar usuarios: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Measurement_ValueChanged(
            object? sender,
            EventArgs e)
        {
            CalculateBMI();
        }

        private void CalculateBMI()
        {
            double weight = (double)numWeight.Value;
            double height = (double)numHeight.Value;

            if (height <= 0)
            {
                lblBMI.Text = "0.00";
                return;
            }

            double bmi = weight / (height * height);

            lblBMI.Text = bmi.ToString("F2");
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (cmbUsers.SelectedItem is not UserOption selectedUser)
            {
                MessageBox.Show(
                    "Debe seleccionar un usuario.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                bool registered =
                    measurementsController.RegisterMeasurement(
                        selectedUser.Id,
                        dtpDate.Value.Date,
                        (double)numWeight.Value,
                        (double)numHeight.Value
                    );

                if (registered)
                {
                    MessageBox.Show(
                        "Medición registrada correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo registrar la medición.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al registrar medición: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private sealed class UserOption
        {
            public int Id { get; }
            public string Name { get; }

            public UserOption(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public override string ToString()
            {
                return $"{Id} - {Name}";
            }
        }
    }
}
