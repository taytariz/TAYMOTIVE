
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class RegisterRoutineView : Form
    {
        private readonly Label lblTitle = new Label();

        private readonly ComboBox cmbUsers = new ComboBox();
        private readonly TextBox txtName = new TextBox();
        private readonly TextBox txtDescription = new TextBox();
        private readonly NumericUpDown numDuration = new NumericUpDown();
        private readonly DateTimePicker dtpStartDate = new DateTimePicker();
        private readonly DateTimePicker dtpEndDate = new DateTimePicker();

        private readonly Button btnSave = new Button();
        private readonly Button btnCancel = new Button();

        private readonly RoutinesController routinesController;

        public RegisterRoutineView()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            routinesController = new RoutinesController(filePath);

            InitializeComponents();
            LoadUsers();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Registrar rutina";
            this.Size = new Size(550, 570);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "REGISTRAR RUTINA";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(125, 25);

            AddLabel("Usuario:", 55, 105);
            cmbUsers.Location = new Point(220, 100);
            cmbUsers.Width = 250;
            cmbUsers.DropDownStyle = ComboBoxStyle.DropDownList;

            AddLabel("Nombre:", 55, 155);
            txtName.Location = new Point(220, 150);
            txtName.Width = 250;

            AddLabel("Descripción:", 55, 205);
            txtDescription.Location = new Point(220, 200);
            txtDescription.Size = new Size(250, 65);
            txtDescription.Multiline = true;

            AddLabel("Duración (min):", 55, 290);
            numDuration.Location = new Point(220, 285);
            numDuration.Width = 250;
            numDuration.Minimum = 1;
            numDuration.Maximum = 600;
            numDuration.Value = 45;

            AddLabel("Fecha inicio:", 55, 340);
            dtpStartDate.Location = new Point(220, 335);
            dtpStartDate.Width = 250;
            dtpStartDate.Format = DateTimePickerFormat.Short;
            dtpStartDate.Value = DateTime.Today;

            AddLabel("Fecha fin:", 55, 390);
            dtpEndDate.Location = new Point(220, 385);
            dtpEndDate.Width = 250;
            dtpEndDate.Format = DateTimePickerFormat.Short;
            dtpEndDate.Value = DateTime.Today.AddDays(30);

            btnSave.Text = "Guardar";
            btnSave.Location = new Point(125, 455);
            btnSave.Size = new Size(130, 40);
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancelar";
            btnCancel.Location = new Point(285, 455);
            btnCancel.Size = new Size(130, 40);
            btnCancel.Click += BtnCancel_Click;

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(cmbUsers);
            this.Controls.Add(txtName);
            this.Controls.Add(txtDescription);
            this.Controls.Add(numDuration);
            this.Controls.Add(dtpStartDate);
            this.Controls.Add(dtpEndDate);
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

                var users = routinesController.GetUsers();

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

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el nombre de la rutina.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtName.Focus();
                return;
            }

            if (dtpEndDate.Value.Date < dtpStartDate.Value.Date)
            {
                MessageBox.Show(
                    "La fecha de finalización no puede ser " +
                    "anterior a la fecha de inicio.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                bool registered = routinesController.RegisterRoutine(
                    selectedUser.Id,
                    txtName.Text,
                    txtDescription.Text,
                    (int)numDuration.Value,
                    dtpStartDate.Value.Date,
                    dtpEndDate.Value.Date
                );

                if (registered)
                {
                    MessageBox.Show(
                        "Rutina registrada correctamente.",
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
                        "No se pudo registrar la rutina.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al registrar rutina: " + ex.Message,
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

        private class UserOption
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
