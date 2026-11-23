using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class NotificationsView : Form
    {
        private readonly User currentUser;

        private readonly NotificationsController
            notificationsController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblUser = new Label();
        private readonly Label lblTotal = new Label();

        private readonly ListView lstNotifications =
            new ListView();

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        public NotificationsView(User currentUser)
        {
            this.currentUser = currentUser ??
                throw new ArgumentNullException(
                    nameof(currentUser)
                );

            string filesPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files"
            );

            RoutinesController routinesController =
                new RoutinesController(
                    Path.Combine(filesPath, "users.json")
                );

            VisitsController visitsController =
                new VisitsController(
                    Path.Combine(filesPath, "visits.json")
                );

            BadgesController badgesController =
                new BadgesController(
                    Path.Combine(filesPath, "badges.json")
                );

            notificationsController =
                new NotificationsController(
                    routinesController,
                    visitsController,
                    badgesController
                );

            InitializeComponents();
            LoadNotifications();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Mis notificaciones";
            ClientSize = new Size(1000, 580);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "MIS NOTIFICACIONES";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Top = 25;

            lblUser.Text =
                "Usuario: " + currentUser.Name;

            lblUser.Font = new Font("Arial", 11);
            lblUser.AutoSize = true;
            lblUser.Location = new Point(35, 95);

            lblTotal.Text = "Total: 0";
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(35, 130);

            lstNotifications.Location =
                new Point(35, 165);

            lstNotifications.Size =
                new Size(930, 320);

            lstNotifications.View = View.Details;
            lstNotifications.FullRowSelect = true;
            lstNotifications.GridLines = true;
            lstNotifications.MultiSelect = false;
            lstNotifications.HideSelection = false;

            lstNotifications.Columns.Add(
                "Fecha",
                150
            );

            lstNotifications.Columns.Add(
                "Tipo",
                130
            );

            lstNotifications.Columns.Add(
                "Notificación",
                620
            );

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(340, 510);
            btnRefresh.Size = new Size(140, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(520, 510);
            btnClose.Size = new Size(140, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblUser);
            Controls.Add(lblTotal);
            Controls.Add(lstNotifications);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();
        }

        // Cargar timeline del usuario
        private void LoadNotifications()
        {
            try
            {
                lstNotifications.Items.Clear();

                List<Notification> notifications =
                    notificationsController
                        .GetUserTimeline(currentUser.Id);

                foreach (Notification notification
                    in notifications)
                {
                    ListViewItem item =
                        new ListViewItem(
                            notification.Date.ToString(
                                "dd/MM/yyyy HH:mm"
                            )
                        );

                    item.SubItems.Add(
                        notification.Type
                    );

                    item.SubItems.Add(
                        notification.Message
                    );

                    // Diferenciar las notificaciones
                    if (notification.Type.Equals(
                        "Negativa",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        item.ForeColor = Color.DarkRed;
                    }
                    else if (notification.Type.Equals(
                        "Positiva",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        item.ForeColor = Color.DarkGreen;
                    }
                    else if (notification.Type.Equals(
                        "Descanso",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        item.ForeColor = Color.DarkOrange;
                    }

                    lstNotifications.Items.Add(item);
                }

                lblTotal.Text =
                    "Total: " + notifications.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar las notificaciones:\n" +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Actualizar notificaciones
        private void BtnRefresh_Click(
            object? sender,
            EventArgs e)
        {
            LoadNotifications();
        }

        // Cerrar ventana
        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }

        // Centrar el título
        private void CenterTitle()
        {
            lblTitle.Left =
                (ClientSize.Width - lblTitle.Width) / 2;
        }
    }
}
