
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class RegisterUserView : Form
    {
        private readonly Label lblTitle = new Label();

        private readonly TextBox txtName = new TextBox();
        private readonly TextBox txtUsername = new TextBox();
        private readonly TextBox txtPassword = new TextBox();
        private readonly TextBox txtGoal = new TextBox();

        private readonly NumericUpDown numAge = new NumericUpDown();
        private readonly NumericUpDown numWeight = new NumericUpDown();
        private readonly NumericUpDown numHeight = new NumericUpDown();

        private readonly Button btnSave = new Button();
        private readonly Button btnCancel = new Button();

        private readonly UsersController usersController;

        public RegisterUserView()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            usersController = new UsersController(filePath);

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Registrar usuario";
            this.Size = new Size(520, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "REGISTRAR USUARIO";
            lblTitle.Font = new Font(
                "Arial", 18, FontStyle.Bold
            );
            lblTitle.Location = new Point(105, 25);
            lblTitle.AutoSize = true;

            AddLabel("Nombre:", 60, 100);
            txtName.Location = new Point(220, 95);
            txtName.Width = 220;

            AddLabel("Edad:", 60, 145);
            numAge.Location = new Point(220, 140);
            numAge.Width = 220;
            numAge.Minimum = 1;
            numAge.Maximum = 120;
            numAge.Value = 18;

            AddLabel("Peso (kg):", 60, 190);
            numWeight.Location = new Point(220, 185);
            numWeight.Width = 220;
            numWeight.Minimum = 1;
            numWeight.Maximum = 500;
            numWeight.DecimalPlaces = 2;
            numWeight.Value = 70;

            AddLabel("Estatura (m):", 60, 235);
            numHeight.Location = new Point(220, 230);
            numHeight.Width = 220;
            numHeight.Minimum = 0.50M;
            numHeight.Maximum = 2.50M;
            numHeight.DecimalPlaces = 2;
            numHeight.Increment = 0.01M;
            numHeight.Value = 1.70M;

            AddLabel("Usuario:", 60, 280);
            txtUsername.Location = new Point(220, 275);
            txtUsername.Width = 220;

            AddLabel("Contraseña:", 60, 325);
            txtPassword.Location = new Point(220, 320);
            txtPassword.Width = 220;
            txtPassword.PasswordChar = '*';

            AddLabel("Objetivo físico:", 60, 370);
            txtGoal.Location = new Point(220, 365);
            txtGoal.Width = 220;

            btnSave.Text = "Guardar";
            btnSave.Location = new Point(95, 440);
            btnSave.Size = new Size(140, 40);
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancelar";
            btnCancel.Location = new Point(265, 440);
            btnCancel.Size = new Size(140, 40);
            btnCancel.Click += BtnCancel_Click;

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtName);
            this.Controls.Add(numAge);
            this.Controls.Add(numWeight);
            this.Controls.Add(numHeight);
            this.Controls.Add(txtUsername);
            this.Controls.Add(txtPassword);
            this.Controls.Add(txtGoal);
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

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                bool registered = usersController.RegisterUser(
                    txtName.Text,
                    (int)numAge.Value,
                    (double)numWeight.Value,
                    (double)numHeight.Value,
                    txtUsername.Text,
                    txtPassword.Text,
                    txtGoal.Text
                );

                if (registered)
                {
                    MessageBox.Show(
                        "Usuario registrado correctamente.",
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
                        "Verifique los datos ingresados. " +
                        "El usuario no debe estar repetido " +
                        "y la contraseña debe tener al menos 12 caracteres.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al registrar usuario: " + ex.Message,
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
    }
}
