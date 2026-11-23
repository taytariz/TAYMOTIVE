
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class BadgesView : Form
    {
        private readonly Label lblTitle = new Label();
        private readonly Label lblTotal = new Label();
        private readonly Label lblPositive = new Label();
        private readonly Label lblNegative = new Label();

        private readonly ComboBox cmbUsers = new ComboBox();
        private readonly ComboBox cmbTypes = new ComboBox();

        private readonly DataGridView dgvBadges =
            new DataGridView();

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        private readonly BadgesController badgesController;
        private readonly UsersController usersController;
        private readonly VisitsController visitsController;
        private readonly FitnessGoalsController goalsController;
        private readonly MeasurementsController measurementsController;
        private readonly BadgeRulesController badgeRulesController;

        public BadgesView()
        {
            string filesPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files"
            );

            string badgesPath = Path.Combine(
                filesPath,
                "badges.json"
            );

            string usersPath = Path.Combine(
                filesPath,
                "users.json"
            );

            string visitsPath = Path.Combine(
                filesPath,
                "visits.json"
            );

            string goalsPath = Path.Combine(
                filesPath,
                "fitnessgoals.json"
            );

            string measurementsPath = Path.Combine(
                filesPath,
                "measurements.json"
            );

            badgesController = new BadgesController(badgesPath);
            usersController = new UsersController(usersPath);
            visitsController = new VisitsController(visitsPath);
            goalsController = new FitnessGoalsController(goalsPath);

            measurementsController =
                new MeasurementsController(measurementsPath);

            badgeRulesController = new BadgeRulesController(
                badgesController,
                visitsController,
                goalsController,
                usersController,
                measurementsController
            );

            InitializeComponents();
            LoadUserFilter();
            RefreshBadges();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Insignias y logros";
            Size = new Size(1150, 700);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "INSIGNIAS Y LOGROS";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(390, 25);

            lblTotal.Text = "Total: 0";
            lblTotal.Location = new Point(50, 100);
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font(
                "Arial", 11, FontStyle.Bold
            );

            lblPositive.Text = "Positivas: 0";
            lblPositive.Location = new Point(250, 100);
            lblPositive.AutoSize = true;
            lblPositive.ForeColor = Color.DarkGreen;
            lblPositive.Font = new Font(
                "Arial", 11, FontStyle.Bold
            );

            lblNegative.Text = "Negativas: 0";
            lblNegative.Location = new Point(450, 100);
            lblNegative.AutoSize = true;
            lblNegative.ForeColor = Color.DarkRed;
            lblNegative.Font = new Font(
                "Arial", 11, FontStyle.Bold
            );

            Label lblUserFilter = new Label
            {
                Text = "Usuario:",
                Location = new Point(50, 150),
                AutoSize = true
            };

            cmbUsers.Location = new Point(115, 145);
            cmbUsers.Size = new Size(300, 30);
            cmbUsers.DropDownStyle =
                ComboBoxStyle.DropDownList;

            Label lblTypeFilter = new Label
            {
                Text = "Tipo:",
                Location = new Point(465, 150),
                AutoSize = true
            };

            cmbTypes.Location = new Point(515, 145);
            cmbTypes.Size = new Size(230, 30);
            cmbTypes.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbTypes.Items.AddRange(new object[]
            {
                "Todas",
                "Positivas",
                "Negativas"
            });

            cmbTypes.SelectedIndex = 0;

            cmbUsers.SelectedIndexChanged +=
                (sender, e) => LoadBadges();

            cmbTypes.SelectedIndexChanged +=
                (sender, e) => LoadBadges();

            dgvBadges.Location = new Point(50, 200);
            dgvBadges.Size = new Size(1030, 370);
            dgvBadges.ReadOnly = true;
            dgvBadges.AllowUserToAddRows = false;
            dgvBadges.AllowUserToDeleteRows = false;
            dgvBadges.MultiSelect = false;
            dgvBadges.RowHeadersVisible = false;

            dgvBadges.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvBadges.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvBadges.Columns.Add("Id", "ID");
            dgvBadges.Columns.Add("User", "Usuario");
            dgvBadges.Columns.Add("Name", "Insignia");
            dgvBadges.Columns.Add("Description", "Descripción");
            dgvBadges.Columns.Add("Type", "Tipo");
            dgvBadges.Columns.Add("Date", "Fecha otorgada");

            dgvBadges.Columns["Id"]!.FillWeight = 35;
            dgvBadges.Columns["User"]!.FillWeight = 90;
            dgvBadges.Columns["Name"]!.FillWeight = 110;
            dgvBadges.Columns["Description"]!.FillWeight = 230;
            dgvBadges.Columns["Type"]!.FillWeight = 70;
            dgvBadges.Columns["Date"]!.FillWeight = 90;

            dgvBadges.CellFormatting +=
                DgvBadges_CellFormatting;

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(365, 595);
            btnRefresh.Size = new Size(180, 40);
            btnRefresh.Click += (sender, e) =>
                RefreshBadges();

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(585, 595);
            btnClose.Size = new Size(180, 40);
            btnClose.Click += (sender, e) => Close();

            Controls.Add(lblTitle);
            Controls.Add(lblTotal);
            Controls.Add(lblPositive);
            Controls.Add(lblNegative);
            Controls.Add(lblUserFilter);
            Controls.Add(cmbUsers);
            Controls.Add(lblTypeFilter);
            Controls.Add(cmbTypes);
            Controls.Add(dgvBadges);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);
        }

        // Cargar usuarios para el filtro
        private void LoadUserFilter()
        {
            cmbUsers.Items.Clear();
            cmbUsers.Items.Add(
                new UserOption(0, "Todos los usuarios")
            );

            var users = usersController.GetUsers();

            foreach (var user in users.Where(
                user => !user.IsAdmin))
            {
                cmbUsers.Items.Add(
                    new UserOption(user.Id, user.Name)
                );
            }

            cmbUsers.SelectedIndex = 0;
        }

        // Sincronizar objetivos e insignias
        private void RefreshBadges()
        {
            try
            {
                goalsController.UpdateAllVisitGoals(
                    visitsController
                );

                var users = usersController.GetUsers();

                foreach (var user in users)
                {
                    badgeRulesController.EvaluateUserBadges(
                        user.Id
                    );
                }

                LoadBadges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al actualizar insignias: " +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Mostrar insignias según filtros
        private void LoadBadges()
        {
            try
            {
                dgvBadges.Rows.Clear();

                var badges = badgesController.GetBadges();
                var users = usersController.GetUsers();

                // Excluir insignias de administradores
                var filteredBadges = badges.Where(badge =>
                    users.Any(user =>
                        user.Id == badge.UserId &&
                        !user.IsAdmin
                    )
                ).ToList();

                // Filtrar por usuario
                if (cmbUsers.SelectedItem is UserOption option &&
                    option.Id != 0)
                {
                    filteredBadges = filteredBadges
                        .Where(badge =>
                            badge.UserId == option.Id)
                        .ToList();
                }

                // Filtrar por tipo
                string selectedType =
                    cmbTypes.SelectedItem?.ToString() ?? "Todas";

                if (selectedType == "Positivas")
                {
                    filteredBadges = filteredBadges
                        .Where(badge =>
                            badge.BadgeType == "Positiva")
                        .ToList();
                }
                else if (selectedType == "Negativas")
                {
                    filteredBadges = filteredBadges
                        .Where(badge =>
                            badge.BadgeType == "Negativa")
                        .ToList();
                }

                foreach (var badge in filteredBadges
                    .OrderByDescending(badge => badge.AwardedDate)
                    .ThenBy(badge => badge.UserId))
                {
                    var user = users.Find(
                        u => u.Id == badge.UserId
                    );

                    dgvBadges.Rows.Add(
                        badge.Id,
                        user?.Name ?? "Usuario no encontrado",
                        badge.Name,
                        badge.Description,
                        badge.BadgeType,
                        badge.AwardedDate.ToString("dd/MM/yyyy")
                    );
                }

                lblTotal.Text =
                    "Total: " + filteredBadges.Count;

                lblPositive.Text =
                    "Positivas: " +
                    filteredBadges.Count(badge =>
                        badge.BadgeType == "Positiva");

                lblNegative.Text =
                    "Negativas: " +
                    filteredBadges.Count(badge =>
                        badge.BadgeType == "Negativa");

                dgvBadges.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al mostrar insignias: " +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Colorear insignias
        private void DgvBadges_CellFormatting(
            object? sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            string type = Convert.ToString(
                dgvBadges.Rows[e.RowIndex]
                    .Cells["Type"].Value
            ) ?? "";

            if (type == "Positiva")
            {
                dgvBadges.Rows[e.RowIndex]
                    .DefaultCellStyle.BackColor =
                    Color.Honeydew;

                dgvBadges.Rows[e.RowIndex]
                    .DefaultCellStyle.ForeColor =
                    Color.DarkGreen;
            }
            else if (type == "Negativa")
            {
                dgvBadges.Rows[e.RowIndex]
                    .DefaultCellStyle.BackColor =
                    Color.MistyRose;

                dgvBadges.Rows[e.RowIndex]
                    .DefaultCellStyle.ForeColor =
                    Color.DarkRed;
            }
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
                return Name;
            }
        }
    }
}
