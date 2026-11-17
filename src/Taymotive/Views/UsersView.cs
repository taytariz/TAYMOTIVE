
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
        private Button btnEdit = new Button();
        private Button btnDelete = new Button();
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
            this.Size = new Size(900, 570);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "GESTIÓN DE USUARIOS";
            lblTitle.Font = new Font(
                "Arial",
                18,
                FontStyle.Bold
            );
            lblTitle.Location = new Point(270, 25);
            lblTitle.AutoSize = true;

            lblTotal.Text = "Total de usuarios: 0";
            lblTotal.Location = new Point(30, 85);
            lblTotal.AutoSize = true;

            dgvUsers.Location = new Point(30, 120);
            dgvUsers.Size = new Size(820, 300);

            dgvUsers.ReadOnly = true;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.AllowUserToDeleteRows = false;
            dgvUsers.MultiSelect = false;
            dgvUsers.RowHeadersVisible = false;

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
            btnRegister.Location = new Point(30, 445);
            btnRegister.Size = new Size(155, 40);
            btnRegister.Click += BtnRegister_Click;

            btnEdit.Text = "Modificar usuario";
            btnEdit.Location = new Point(195, 445);
            btnEdit.Size = new Size(155, 40);
            btnEdit.Click += BtnEdit_Click;

            btnDelete.Text = "Eliminar usuario";
            btnDelete.Location = new Point(360, 445);
            btnDelete.Size = new Size(155, 40);
            btnDelete.Click += BtnDelete_Click;

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(525, 445);
            btnRefresh.Size = new Size(155, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(690, 445);
            btnClose.Size = new Size(155, 40);
            btnClose.Click += BtnClose_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblTotal);
            this.Controls.Add(dgvUsers);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClose);
        }

        // Cargar usuarios en la tabla
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

                dgvUsers.ClearSelection();
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

        // Registrar usuario
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

        // Modificar usuario
        private void BtnEdit_Click(
            object? sender,
            EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un usuario para modificar.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            int userId = Convert.ToInt32(
                dgvUsers.SelectedRows[0].Cells["Id"].Value
            );

            using (EditUserView editView =
                new EditUserView(userId))
            {
                if (editView.ShowDialog(this) ==
                    DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        // Eliminar usuario
        private void BtnDelete_Click(
            object? sender,
            EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un usuario para eliminar.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            int userId = Convert.ToInt32(
                dgvUsers.SelectedRows[0].Cells["Id"].Value
            );

            string username = Convert.ToString(
                dgvUsers.SelectedRows[0].Cells["Username"].Value
            ) ?? "";

            if (string.Equals(
                username,
                "admin",
                StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "No se puede eliminar la cuenta administradora.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            DialogResult confirmation = MessageBox.Show(
                "¿Está seguro de eliminar al usuario " +
                username + "?\nEsta acción no se puede deshacer.",
                "TAYMOTIVE - Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
            );

            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool deleted = usersController.DeleteUser(userId);

                if (deleted)
                {
                    MessageBox.Show(
                        "Usuario eliminado correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadUsers();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo eliminar el usuario.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar usuario: " + ex.Message,
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
            LoadUsers();
        }

        // Cerrar ventana
        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            this.Close();
        }
    }
}
