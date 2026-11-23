
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

        private readonly DataGridView dgvVisits =
            new DataGridView();

        private readonly Button btnRegister = new Button();
        private readonly Button btnEdit = new Button();
        private readonly Button btnDelete = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        private readonly VisitsController visitsController;
        private readonly UsersController usersController;
        private readonly FitnessGoalsController goalsController;
        private readonly BadgesController badgesController;
        private readonly BadgeRulesController badgeRulesController;

        public VisitsView()
        {
            string filesPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files"
            );

            string visitsPath = Path.Combine(
                filesPath,
                "visits.json"
            );

            string usersPath = Path.Combine(
                filesPath,
                "users.json"
            );

            string goalsPath = Path.Combine(
                filesPath,
                "fitnessgoals.json"
            );

            string badgesPath = Path.Combine(
                filesPath,
                "badges.json"
            );

            visitsController = new VisitsController(visitsPath);
            usersController = new UsersController(usersPath);
            goalsController = new FitnessGoalsController(goalsPath);
            badgesController = new BadgesController(badgesPath);

            badgeRulesController = new BadgeRulesController(
                badgesController,
                visitsController,
                goalsController,
                usersController
            );

            InitializeComponents();
            LoadVisits();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Gestión de visitas y clases";
            Size = new Size(1050, 600);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

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

            Controls.Add(lblTitle);
            Controls.Add(lblTotal);
            Controls.Add(dgvVisits);
            Controls.Add(btnRegister);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);
        }

        // Mostrar visitas registradas
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

                lblTotal.Text =
                    "Total de visitas: " + visits.Count;

                dgvVisits.ClearSelection();
            }
            catch (Exception ex)
            {
                ShowError(
                    "Error al cargar visitas: " + ex.Message
                );
            }
        }

        // Actualizar objetivos de asistencia
        private void SynchronizeVisitGoals()
        {
            goalsController.UpdateAllVisitGoals(
                visitsController
            );
        }

        // Evaluar insignias de todos los usuarios
        private void EvaluateAllUserBadges()
        {
            var users = usersController.GetUsers();

            foreach (var user in users)
            {
                badgeRulesController.EvaluateUserBadges(
                    user.Id
                );
            }
        }

        // Sincronizar objetivos e insignias
        private void SynchronizeProgressAndBadges()
        {
            SynchronizeVisitGoals();
            EvaluateAllUserBadges();
        }

        // Registrar visita
        private void BtnRegister_Click(
            object? sender,
            EventArgs e)
        {
            using (RegisterVisitView registerView =
                new RegisterVisitView())
            {
                if (registerView.ShowDialog(this) ==
                    DialogResult.OK)
                {
                    try
                    {
                        SynchronizeProgressAndBadges();
                    }
                    catch (Exception ex)
                    {
                        ShowError(
                            "La visita fue registrada, pero " +
                            "no se pudieron actualizar los objetivos " +
                            "o las insignias: " + ex.Message
                        );
                    }

                    LoadVisits();
                }
            }
        }

        // Modificar visita
        private void BtnEdit_Click(
            object? sender,
            EventArgs e)
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

            using (EditVisitView editView =
                new EditVisitView(visitId))
            {
                if (editView.ShowDialog(this) ==
                    DialogResult.OK)
                {
                    try
                    {
                        SynchronizeProgressAndBadges();
                    }
                    catch (Exception ex)
                    {
                        ShowError(
                            "La visita fue modificada, pero " +
                            "no se pudieron actualizar los objetivos " +
                            "o las insignias: " + ex.Message
                        );
                    }

                    LoadVisits();
                }
            }
        }

        // Eliminar visita
        private void BtnDelete_Click(
            object? sender,
            EventArgs e)
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
                bool deleted =
                    visitsController.DeleteVisit(visitId);

                if (deleted)
                {
                    try
                    {
                        SynchronizeProgressAndBadges();
                    }
                    catch (Exception ex)
                    {
                        ShowError(
                            "La visita fue eliminada, pero " +
                            "no se pudieron actualizar los objetivos " +
                            "o las insignias: " + ex.Message
                        );
                    }

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
                ShowError(
                    "Error al eliminar visita: " + ex.Message
                );
            }
        }

        // Actualizar visitas, objetivos e insignias
        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                SynchronizeProgressAndBadges();
            }
            catch (Exception ex)
            {
                ShowError(
                    "No se pudieron sincronizar los objetivos " +
                    "o las insignias: " + ex.Message
                );
            }

            LoadVisits();
        }

        // Mostrar errores
        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "TAYMOTIVE",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        // Cerrar ventana
        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }
    }
}