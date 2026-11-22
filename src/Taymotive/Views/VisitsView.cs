
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class VisitsView : Form
    {
        private readonly Label lblTitle = new Label();
        private readonly Label lblTotal = new Label();

        private readonly DataGridView dgvVisits = new DataGridView();

        private readonly Button btnRegister = new Button();
        private readonly Button btnEdit = new Button();
        private readonly Button btnDelete = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        private readonly VisitsController visitsController;
        private readonly UsersController usersController;

        public VisitsView()
        {
            string visitsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "visits.json"
            );

            string usersPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            visitsController = new VisitsController(visitsPath);
            usersController = new UsersController(usersPath);

            InitializeComponents();
            LoadVisits();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Gestión de visitas y clases";
            this.Size = new Size(1050, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "GESTIÓN DE VISITAS Y CLASES";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(280, 30);

            lblTotal.Text = "Total de visitas: 0";
            lblTotal.Location = new Point(35, 100);
            lblTotal.AutoSize = true;

            dgvVisits.Location = new Point(35, 135);
            dgvVisits.Size = new Size(960, 330);
            dgvVisits.ReadOnly = true;
            dgvVisits.AllowUserToAddRows = false;
            dgvVisits.AllowUserToDeleteRows = false;
            dgvVisits.MultiSelect = false;
            dgvVisits.RowHeadersVisible = false;

            dgvVisits.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvVisits.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvVisits.Columns.Add("Id", "ID");
            dgvVisits.Columns.Add("User", "Usuario");
            dgvVisits.Columns.Add("Date", "Fecha de visita");
            dgvVisits.Columns.Add("Class", "Clase");

            btnRegister.Text = "Registrar visita";
            btnRegister.Location = new Point(35, 495);
            btnRegister.Size = new Size(180, 40);
            btnRegister.Click += BtnRegister_Click;

            btnEdit.Text = "Modificar visita";
            btnEdit.Location = new Point(230, 495);
            btnEdit.Size = new Size(180, 40);
            btnEdit.Click += BtnEdit_Click;

            btnDelete.Text = "Eliminar visita";
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
            this.Controls.Add(dgvVisits);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClose);
        }

        private void LoadVisits()
        {
            try
            {
                dgvVisits.Rows.Clear();

                var visits = visitsController.GetVisits();
                var users = usersController.GetUsers();

                foreach (var visit in visits)
                {
                    var user = users.Find(
                        u => u.Id == visit.UserId
                    );

                    string userName = user != null
                        ? user.Name
                        : "Usuario no encontrado";

                    dgvVisits.Rows.Add(
                        visit.Id,
                        userName,
                        visit.VisitDate.ToString("dd/MM/yyyy"),
                        visit.ClassName
                    );
                }

                lblTotal.Text = "Total de visitas: " + visits.Count;

                dgvVisits.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar visitas: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            using (RegisterVisitView registerView =
                new RegisterVisitView())
            {
                if (registerView.ShowDialog(this) == DialogResult.OK)
                {
                    LoadVisits();
                }
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvVisits.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione una visita para modificar.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            int visitId = Convert.ToInt32(
                dgvVisits.SelectedRows[0].Cells["Id"].Value
            );

            using (EditVisitView editView = new EditVisitView(visitId))
            {
                if (editView.ShowDialog(this) == DialogResult.OK)
                {
                    LoadVisits();
                }
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvVisits.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione una visita para eliminar.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            int visitId = Convert.ToInt32(
                dgvVisits.SelectedRows[0].Cells["Id"].Value
            );

            string userName = Convert.ToString(
                dgvVisits.SelectedRows[0].Cells["User"].Value
            ) ?? "Usuario desconocido";

            string visitDate = Convert.ToString(
                dgvVisits.SelectedRows[0].Cells["Date"].Value
            ) ?? "";

            string className = Convert.ToString(
                dgvVisits.SelectedRows[0].Cells["Class"].Value
            ) ?? "";

            DialogResult confirmation = MessageBox.Show(
                "¿Está seguro de que desea eliminar esta visita?\n\n" +
                "ID: " + visitId + "\n" +
                "Usuario: " + userName + "\n" +
                "Fecha: " + visitDate + "\n" +
                "Clase: " + className + "\n\n" +
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
                bool deleted = visitsController.DeleteVisit(visitId);

                if (deleted)
                {
                    MessageBox.Show(
                        "Visita eliminada correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadVisits();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo eliminar la visita.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar visita: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadVisits();
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}