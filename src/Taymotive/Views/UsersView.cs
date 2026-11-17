
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class UsersView : Form
    {
        private Label lblTitle = new Label();
        private Label lblTotal = new Label();

        private DataGridView dgvUsers = new DataGridView();

        private Button btnRegister = new Button();
        private Button btnRefresh = new Button();
        private Button btnClose = new Button();

        private readonly UsersController usersController;

        public UsersView()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            usersController = new UsersController(filePath);

            InitializeComponents();
            LoadUsers();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Gestión de usuarios";
            this.Size = new Size(850, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "GESTIÓN DE USUARIOS";
            lblTitle.Font = new Font(
                "Arial",
                18,
                FontStyle.Bold
            );
            lblTitle.Location = new Point(250, 25);
            lblTitle.AutoSize = true;

            lblTotal.Text = "Total de usuarios: 0";
            lblTotal.Location = new Point(30, 85);
            lblTotal.AutoSize = true;

            dgvUsers.Location = new Point(30, 120);
            dgvUsers.Size = new Size(770, 300);

            dgvUsers.ReadOnly = true;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.AllowUserToDeleteRows = false;
            dgvUsers.MultiSelect = false;

            dgvUsers.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvUsers.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvUsers.Columns.Add("Id", "ID");
            dgvUsers.Columns.Add("Name", "Nombre");
            dgvUsers.Columns.Add("Username", "Usuario");
            dgvUsers.Columns.Add("Age", "Edad");
            dgvUsers.Columns.Add("Weight", "Peso (kg)");
            dgvUsers.Columns.Add("Height", "Estatura (m)");

            btnRegister.Text = "Registrar usuario";
            btnRegister.Location = new Point(100, 445);
            btnRegister.Size = new Size(180, 40);
            btnRegister.Click += BtnRegister_Click;

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(325, 445);
            btnRefresh.Size = new Size(180, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(550, 445);
            btnClose.Size = new Size(180, 40);
            btnClose.Click += BtnClose_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblTotal);
            this.Controls.Add(dgvUsers);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClose);
        }

        private void LoadUsers()
        {
            try
            {
                dgvUsers.Rows.Clear();

                var users = usersController.GetUsers();

                foreach (var user in users)
                {
                    dgvUsers.Rows.Add(
                        user.Id,
                        user.Name,
                        user.Username,
                        user.Age,
                        user.Weight.ToString("F2"),
                        user.Height.ToString("F2")
                    );
                }

                lblTotal.Text =
                    "Total de usuarios: " + users.Count;
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

        private void BtnRegister_Click(
            object? sender,
            EventArgs e)
        {
            using (RegisterUserView registerView =
                new RegisterUserView())
            {
                if (registerView.ShowDialog(this) ==
                    DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            LoadUsers();
        }

        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            this.Close();
        }
    }
}
