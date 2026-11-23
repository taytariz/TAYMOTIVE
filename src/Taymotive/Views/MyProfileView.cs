
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class MyProfileView : Form
    {
        private readonly User currentUser;
        private readonly ProfileController profileController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblName = new Label();
        private readonly Label lblUsername = new Label();
        private readonly Label lblAge = new Label();
        private readonly Label lblWeight = new Label();
        private readonly Label lblHeight = new Label();
        private readonly Label lblGoal = new Label();

        private readonly GroupBox grpInformation = new GroupBox();
        private readonly GroupBox grpPassword = new GroupBox();

        private readonly TextBox txtCurrentPassword = new TextBox();
        private readonly TextBox txtNewPassword = new TextBox();
        private readonly TextBox txtConfirmPassword = new TextBox();

        private readonly Button btnChangePassword = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        public MyProfileView(User currentUser)
        {
            this.currentUser = currentUser ??
                throw new ArgumentNullException(nameof(currentUser));

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            profileController = new ProfileController(filePath);

            InitializeComponents();
            LoadProfile();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Mi perfil";
            ClientSize = new Size(650, 690);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "MI PERFIL";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Top = 25;

            // Información personal
            grpInformation.Text = "Información personal";
            grpInformation.Font = new Font(
                "Arial",
                11,
                FontStyle.Bold
            );
            grpInformation.Location = new Point(35, 85);
            grpInformation.Size = new Size(580, 240);

            ConfigureInformationLabel(lblName, 35);
            ConfigureInformationLabel(lblUsername, 65);
            ConfigureInformationLabel(lblAge, 95);
            ConfigureInformationLabel(lblWeight, 125);
            ConfigureInformationLabel(lblHeight, 155);
            ConfigureInformationLabel(lblGoal, 185);

            grpInformation.Controls.Add(lblName);
            grpInformation.Controls.Add(lblUsername);
            grpInformation.Controls.Add(lblAge);
            grpInformation.Controls.Add(lblWeight);
            grpInformation.Controls.Add(lblHeight);
            grpInformation.Controls.Add(lblGoal);

            // Cambio de contraseña
            grpPassword.Text = "Cambiar contraseña";
            grpPassword.Font = new Font(
                "Arial",
                11,
                FontStyle.Bold
            );
            grpPassword.Location = new Point(35, 345);
            grpPassword.Size = new Size(580, 235);

            Label lblCurrent = new Label();
            lblCurrent.Text = "Contraseña actual:";
            lblCurrent.Location = new Point(25, 40);
            lblCurrent.AutoSize = true;

            txtCurrentPassword.Location = new Point(250, 35);
            txtCurrentPassword.Size = new Size(280, 25);
            txtCurrentPassword.UseSystemPasswordChar = true;

            Label lblNew = new Label();
            lblNew.Text = "Nueva contraseña:";
            lblNew.Location = new Point(25, 85);
            lblNew.AutoSize = true;

            txtNewPassword.Location = new Point(250, 80);
            txtNewPassword.Size = new Size(280, 25);
            txtNewPassword.UseSystemPasswordChar = true;

            Label lblConfirm = new Label();
            lblConfirm.Text = "Confirmar contraseña:";
            lblConfirm.Location = new Point(25, 130);
            lblConfirm.AutoSize = true;

            txtConfirmPassword.Location = new Point(250, 125);
            txtConfirmPassword.Size = new Size(280, 25);
            txtConfirmPassword.UseSystemPasswordChar = true;

            btnChangePassword.Text = "Cambiar contraseña";
            btnChangePassword.Location = new Point(190, 175);
            btnChangePassword.Size = new Size(200, 40);
            btnChangePassword.Click += BtnChangePassword_Click;

            grpPassword.Controls.Add(lblCurrent);
            grpPassword.Controls.Add(txtCurrentPassword);
            grpPassword.Controls.Add(lblNew);
            grpPassword.Controls.Add(txtNewPassword);
            grpPassword.Controls.Add(lblConfirm);
            grpPassword.Controls.Add(txtConfirmPassword);
            grpPassword.Controls.Add(btnChangePassword);

            // Botones inferiores
            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(175, 610);
            btnRefresh.Size = new Size(140, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(335, 610);
            btnClose.Size = new Size(140, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(grpInformation);
            Controls.Add(grpPassword);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();
        }

        private void ConfigureInformationLabel(
            Label label,
            int top)
        {
            label.Location = new Point(25, top);
            label.AutoSize = true;
            label.Font = new Font("Arial", 10);
        }

        // Cargar datos del usuario
        private void LoadProfile()
        {
            try
            {
                User? user = profileController.GetUserProfile(
                    currentUser.Id
                );

                if (user == null)
                {
                    MessageBox.Show(
                        "No se encontró el perfil del usuario.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    Close();
                    return;
                }

                lblName.Text = "Nombre: " + user.Name;
                lblUsername.Text = "Usuario: " + user.Username;
                lblAge.Text = "Edad: " + user.Age;
                lblWeight.Text =
                    "Peso: " + user.Weight.ToString("F2") + " kg";
                lblHeight.Text =
                    "Estatura: " + user.Height.ToString("F2") + " m";
                lblGoal.Text = "Objetivo fitness: " + user.Goal;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar el perfil:\n" + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Cambiar contraseña
        private void BtnChangePassword_Click(
            object? sender,
            EventArgs e)
        {
            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show(
                    "Complete los tres campos de contraseña.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (newPassword.Length < 12)
            {
                MessageBox.Show(
                    "La nueva contraseña debe tener al menos 12 caracteres.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show(
                    "La confirmación no coincide con la nueva contraseña.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            try
            {
                bool changed = profileController.ChangePassword(
                    currentUser,
                    currentPassword,
                    newPassword
                );

                if (changed)
                {
                    MessageBox.Show(
                        "Contraseña actualizada correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    txtCurrentPassword.Clear();
                    txtNewPassword.Clear();
                    txtConfirmPassword.Clear();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo cambiar la contraseña.\n" +
                        "Verifique la contraseña actual y que " +
                        "la nueva sea diferente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cambiar la contraseña:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            LoadProfile();
        }

        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }

        private void CenterTitle()
        {
            lblTitle.Left =
                (ClientSize.Width - lblTitle.Width) / 2;
        }
    }
}
