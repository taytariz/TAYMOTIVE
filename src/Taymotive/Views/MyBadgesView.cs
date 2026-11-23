
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class MyBadgesView : Form
    {
        private readonly User currentUser;
        private readonly BadgesController badgesController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblUsername = new Label();
        private readonly Label lblTotal = new Label();

        private readonly DataGridView dgvBadges =
            new DataGridView();

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        public MyBadgesView(User currentUser)
        {
            this.currentUser = currentUser ??
                throw new ArgumentNullException(
                    nameof(currentUser)
                );

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "badges.json"
            );

            badgesController =
                new BadgesController(filePath);

            InitializeComponents();
            LoadBadges();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Mis insignias y logros";
            ClientSize = new Size(1000, 560);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "MIS INSIGNIAS Y LOGROS";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Top = 25;

            lblUsername.Text =
                "Usuario: " + currentUser.Name;

            lblUsername.Font = new Font("Arial", 11);
            lblUsername.Location = new Point(35, 95);
            lblUsername.AutoSize = true;

            lblTotal.Text = "Total de insignias: 0";
            lblTotal.Location = new Point(35, 130);
            lblTotal.AutoSize = true;

            dgvBadges.Location = new Point(35, 165);
            dgvBadges.Size = new Size(930, 290);

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
            dgvBadges.Columns.Add("Name", "Insignia");
            dgvBadges.Columns.Add(
                "Description",
                "Descripción"
            );
            dgvBadges.Columns.Add("Type", "Tipo");
            dgvBadges.Columns.Add(
                "AwardedDate",
                "Fecha de otorgamiento"
            );

            dgvBadges.Columns["Id"]!.FillWeight = 40;
            dgvBadges.Columns["Name"]!.FillWeight = 140;
            dgvBadges.Columns["Description"]!.FillWeight = 230;
            dgvBadges.Columns["Type"]!.FillWeight = 80;
            dgvBadges.Columns["AwardedDate"]!.FillWeight = 120;

            dgvBadges.ShowCellToolTips = true;

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(345, 480);
            btnRefresh.Size = new Size(140, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(505, 480);
            btnClose.Size = new Size(140, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(lblTotal);
            Controls.Add(dgvBadges);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();

            ClientSizeChanged += (sender, e) =>
            {
                CenterTitle();
            };
        }

        // Cargar las insignias del usuario
        private void LoadBadges()
        {
            try
            {
                dgvBadges.Rows.Clear();

                var badges =
                    badgesController.GetBadgesByUser(
                        currentUser.Id
                    );

                foreach (Badge badge in badges)
                {
                    int rowIndex = dgvBadges.Rows.Add(
                        badge.Id,
                        badge.Name,
                        badge.Description,
                        badge.BadgeType,
                        badge.AwardedDate.ToString(
                            "dd/MM/yyyy"
                        )
                    );

                    dgvBadges.Rows[rowIndex]
                        .Cells["Description"].ToolTipText =
                        badge.Description;
                }

                lblTotal.Text =
                    "Total de insignias: " +
                    badges.Count;

                dgvBadges.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar las insignias:\n" +
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
            LoadBadges();
        }

        // Cerrar ventana
        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }

        // Centrar título
        private void CenterTitle()
        {
            lblTitle.Left =
                (ClientSize.Width - lblTitle.Width) / 2;
        }
    }
}
