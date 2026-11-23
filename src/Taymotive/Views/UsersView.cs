
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class UsersView : Form
    {
        private readonly User currentUser;
        private readonly UsersController usersController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblTotal = new Label();

        private readonly DataGridView dgvUsers = new DataGridView();

        private readonly Button btnRegister = new Button();
        private readonly Button btnEdit = new Button();
        private readonly Button btnDelete = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();
        private readonly Button btnActivation = new Button();

        public UsersView(User currentUser)
        {
            this.currentUser = currentUser ??
                throw new ArgumentNullException(
                    nameof(currentUser)
                );

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
            this.ClientSize = new Size(900, 590);
            this.StartPosition =
                FormStartPosition.CenterParent;
            this.FormBorderStyle =
                FormBorderStyle.FixedSingle;
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

            ConfigureButton(
                btnRegister,
                "Registrar usuario",
                30,
                445
            );

            ConfigureButton(
                btnEdit,
                "Modificar usuario",
                195,
                445
            );

            ConfigureButton(
                btnDelete,
                "Eliminar usuario",
                360,
                445
            );

            ConfigureButton(
                btnRefresh,
                "Actualizar",
                525,
                445
            );

            ConfigureButton(
                btnClose,
                "Cerrar",
                690,
                445
            );

            btnActivation.Text =
                "Generar código de activación";

            btnActivation.Location =
                new Point(325, 505);

            btnActivation.Size =
                new Size(250, 40);

            btnActivation.Visible =
                currentUser.IsAdmin;

            btnRegister.Click += BtnRegister_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnClose.Click += BtnClose_Click;

            btnActivation.Click +=
                BtnActivation_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblTotal);
            this.Controls.Add(dgvUsers);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClose);
            this.Controls.Add(btnActivation);
        }

        private void ConfigureButton(
            Button button,
            string text,
            int x,
            int y)
        {
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(155, 40);
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
                    "Error al cargar usuarios: " +
                    ex.Message,
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
                dgvUsers.SelectedRows[0]
                    .Cells["Id"].Value
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
                dgvUsers.SelectedRows[0]
                    .Cells["Id"].Value
            );

            string username = Convert.ToString(
                dgvUsers.SelectedRows[0]
                    .Cells["Username"].Value
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

            DialogResult confirmation =
                MessageBox.Show(
                    "¿Está seguro de eliminar al usuario " +
                    username +
                    "?\nEsta acción no se puede deshacer.",
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
                bool deleted =
                    usersController.DeleteUser(userId);

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
                    "Error al eliminar usuario: " +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Generar código de activación
        private void BtnActivation_Click(
            object? sender,
            EventArgs e)
        {
            if (!currentUser.IsAdmin)
            {
                MessageBox.Show(
                    "Solo el Administrador puede " +
                    "generar códigos de activación.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un usuario para activar.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            int userId = Convert.ToInt32(
                dgvUsers.SelectedRows[0]
                    .Cells["Id"].Value
            );

            string username = Convert.ToString(
                dgvUsers.SelectedRows[0]
                    .Cells["Username"].Value
            ) ?? "";

            DialogResult confirmation =
                MessageBox.Show(
                    "¿Desea generar un código de activación " +
                    "para el usuario " + username + "?",
                    "TAYMOTIVE - Activación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            try
            {
                string? code =
                    usersController.GenerateActivationCode(
                        currentUser.Id,
                        userId
                    );

                if (code == null)
                {
                    MessageBox.Show(
                        "No se pudo generar el código.\n" +
                        "Verifique que el usuario exista " +
                        "y todavía no tenga contraseña.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                // Mostrar el código solo una vez
                using (Form codeForm = new Form())
                {
                    codeForm.Text =
                        "TAYMOTIVE - Código de activación";

                    codeForm.ClientSize =
                        new Size(480, 220);

                    codeForm.StartPosition =
                        FormStartPosition.CenterParent;

                    codeForm.FormBorderStyle =
                        FormBorderStyle.FixedDialog;

                    codeForm.MaximizeBox = false;
                    codeForm.MinimizeBox = false;

                    Label lblMessage = new Label();
                    lblMessage.Text =
                        "Código de activación para " +
                        username + ":";

                    lblMessage.Location =
                        new Point(35, 30);

                    lblMessage.AutoSize = true;

                    TextBox txtCode = new TextBox();
                    txtCode.Text = code;
                    txtCode.ReadOnly = true;
                    txtCode.TextAlign =
                        HorizontalAlignment.Center;

                    txtCode.Font = new Font(
                        "Consolas",
                        14,
                        FontStyle.Bold
                    );

                    txtCode.Location =
                        new Point(35, 75);

                    txtCode.Size =
                        new Size(410, 30);

                    Label lblExpiration = new Label();
                    lblExpiration.Text =
                        "Válido por 24 horas. " +
                        "Se puede utilizar una sola vez.";

                    lblExpiration.Location =
                        new Point(35, 120);

                    lblExpiration.AutoSize = true;

                    Button btnOk = new Button();
                    btnOk.Text = "Cerrar";
                    btnOk.Location =
                        new Point(180, 160);

                    btnOk.Size =
                        new Size(120, 35);

                    btnOk.DialogResult =
                        DialogResult.OK;

                    codeForm.Controls.Add(lblMessage);
                    codeForm.Controls.Add(txtCode);
                    codeForm.Controls.Add(lblExpiration);
                    codeForm.Controls.Add(btnOk);

                    codeForm.AcceptButton = btnOk;

                    codeForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al generar código: " +
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
