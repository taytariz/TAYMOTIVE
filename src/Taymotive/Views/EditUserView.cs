
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class EditUserView : Form
    {
        private Label lblTitle = new Label();

        private TextBox txtName = new TextBox();
        private TextBox txtUsername = new TextBox();
        private TextBox txtGoal = new TextBox();

        private NumericUpDown numAge = new NumericUpDown();
        private NumericUpDown numWeight = new NumericUpDown();
        private NumericUpDown numHeight = new NumericUpDown();

        private Button btnSave = new Button();
        private Button btnCancel = new Button();

        private readonly UsersController usersController;
        private readonly int userId;

        public EditUserView(int id)
        {
            userId = id;

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            usersController = new UsersController(filePath);

            InitializeComponents();
            LoadUser();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Modificar usuario";
            this.Size = new Size(520, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "MODIFICAR USUARIO";
            lblTitle.Font = new Font(
                "Arial",
                18,
                FontStyle.Bold
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

            AddLabel("Peso (kg):", 60, 190);
            numWeight.Location = new Point(220, 185);
            numWeight.Width = 220;
            numWeight.Minimum = 1;
            numWeight.Maximum = 500;
            numWeight.DecimalPlaces = 2;

            AddLabel("Estatura (m):", 60, 235);
            numHeight.Location = new Point(220, 230);
            numHeight.Width = 220;
            numHeight.Minimum = 0.50M;
            numHeight.Maximum = 2.50M;
            numHeight.DecimalPlaces = 2;
            numHeight.Increment = 0.01M;

            AddLabel("Usuario:", 60, 280);
            txtUsername.Location = new Point(220, 275);
            txtUsername.Width = 220;

            AddLabel("Objetivo físico:", 60, 325);
            txtGoal.Location = new Point(220, 320);
            txtGoal.Width = 220;

            btnSave.Text = "Guardar cambios";
            btnSave.Location = new Point(95, 395);
            btnSave.Size = new Size(140, 40);
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancelar";
            btnCancel.Location = new Point(265, 395);
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

        private void LoadUser()
        {
            User? user = usersController.GetUserById(userId);

            if (user == null)
            {
                MessageBox.Show(
                    "No se encontró el usuario.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                this.BeginInvoke(new Action(this.Close));
                return;
            }

            txtName.Text = user.Name;
            txtUsername.Text = user.Username;
            txtGoal.Text = user.Goal;

            numAge.Value = Math.Clamp(
                user.Age,
                (int)numAge.Minimum,
                (int)numAge.Maximum
            );

            numWeight.Value = Math.Clamp(
                (decimal)user.Weight,
                numWeight.Minimum,
                numWeight.Maximum
            );

            numHeight.Value = Math.Clamp(
                (decimal)user.Height,
                numHeight.Minimum,
                numHeight.Maximum
            );
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                bool updated = usersController.UpdateUser(
                    userId,
                    txtName.Text,
                    (int)numAge.Value,
                    (double)numWeight.Value,
                    (double)numHeight.Value,
                    txtUsername.Text,
                    txtGoal.Text
                );

                if (updated)
                {
                    MessageBox.Show(
                        "Usuario modificado correctamente.",
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
                        "No se pudo modificar el usuario. " +
                        "Verifique los datos y que el nombre " +
                        "de usuario no esté repetido.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al modificar usuario: " + ex.Message,
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
