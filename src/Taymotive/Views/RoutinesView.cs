
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class RoutinesView : Form
    {
        private readonly Label lblTitle = new Label();
        private readonly Label lblTotal = new Label();

        private readonly DataGridView dgvRoutines = new DataGridView();

        private readonly Button btnRegister = new Button();
        private readonly Button btnEdit = new Button();
        private readonly Button btnDelete = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        private readonly RoutinesController routinesController;

        public RoutinesView()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            routinesController = new RoutinesController(filePath);

            InitializeComponents();
            LoadRoutines();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Gestión de rutinas";
            this.Size = new Size(1050, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "GESTIÓN DE RUTINAS";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(350, 25);

            lblTotal.Text = "Total de rutinas: 0";
            lblTotal.Location = new Point(30, 90);
            lblTotal.AutoSize = true;

            dgvRoutines.Location = new Point(30, 125);
            dgvRoutines.Size = new Size(970, 330);
            dgvRoutines.ReadOnly = true;
            dgvRoutines.AllowUserToAddRows = false;
            dgvRoutines.AllowUserToDeleteRows = false;
            dgvRoutines.MultiSelect = false;
            dgvRoutines.RowHeadersVisible = false;

            dgvRoutines.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvRoutines.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvRoutines.Columns.Add("Id", "ID");
            dgvRoutines.Columns.Add("User", "Usuario");
            dgvRoutines.Columns.Add("Name", "Rutina");
            dgvRoutines.Columns.Add("Description", "Descripción");
            dgvRoutines.Columns.Add("Duration", "Duración (min)");
            dgvRoutines.Columns.Add("StartDate", "Fecha inicio");
            dgvRoutines.Columns.Add("EndDate", "Fecha fin");

            btnRegister.Text = "Registrar rutina";
            btnRegister.Location = new Point(45, 485);
            btnRegister.Size = new Size(180, 40);
            btnRegister.Click += BtnRegister_Click;

            btnEdit.Text = "Modificar rutina";
            btnEdit.Location = new Point(235, 485);
            btnEdit.Size = new Size(180, 40);
            btnEdit.Click += BtnEdit_Click;

            btnDelete.Text = "Eliminar rutina";
            btnDelete.Location = new Point(425, 485);
            btnDelete.Size = new Size(180, 40);
            btnDelete.Click += BtnDelete_Click;

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(615, 485);
            btnRefresh.Size = new Size(160, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(785, 485);
            btnClose.Size = new Size(160, 40);
            btnClose.Click += BtnClose_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblTotal);
            this.Controls.Add(dgvRoutines);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClose);
        }

        // Cargar rutinas desde users.json
        private void LoadRoutines()
        {
            try
            {
                dgvRoutines.Rows.Clear();

                var users = routinesController.GetUsers();

                int totalRoutines = 0;

                foreach (var user in users)
                {
                    foreach (var routine in user.Routines)
                    {
                        dgvRoutines.Rows.Add(
                            routine.Id,
                            user.Name,
                            routine.Name,
                            routine.Description,
                            routine.Duration,
                            routine.StartDate.ToString("dd/MM/yyyy"),
                            routine.EndDate.ToString("dd/MM/yyyy")
                        );

                        totalRoutines++;
                    }
                }

                lblTotal.Text =
                    "Total de rutinas: " + totalRoutines;

                dgvRoutines.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar las rutinas: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Registrar rutina
        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            using (RegisterRoutineView registerView =
                new RegisterRoutineView())
            {
                if (registerView.ShowDialog(this) == DialogResult.OK)
                {
                    LoadRoutines();
                }
            }
        }

        // Modificar rutina
        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvRoutines.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione una rutina para modificar.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            int routineId = Convert.ToInt32(
                dgvRoutines.SelectedRows[0].Cells["Id"].Value
            );

            using (EditRoutineView editView =
                new EditRoutineView(routineId))
            {
                if (editView.ShowDialog(this) == DialogResult.OK)
                {
                    LoadRoutines();
                }
            }
        }

        // Eliminar rutina
        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvRoutines.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione una rutina para eliminar.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            int routineId = Convert.ToInt32(
                dgvRoutines.SelectedRows[0].Cells["Id"].Value
            );

            string routineName = Convert.ToString(
                dgvRoutines.SelectedRows[0].Cells["Name"].Value
            ) ?? string.Empty;

            DialogResult confirmation = MessageBox.Show(
                "¿Está seguro de eliminar la rutina " +
                routineName + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool deleted =
                    routinesController.DeleteRoutine(routineId);

                if (deleted)
                {
                    MessageBox.Show(
                        "Rutina eliminada correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadRoutines();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo eliminar la rutina.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar la rutina: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Actualizar tabla
        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadRoutines();
        }

        // Cerrar ventana
        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}
