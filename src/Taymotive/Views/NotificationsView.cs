
using System;
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

        private readonly Label lblTitle = new Label();
        private readonly Label lblUser = new Label();
        private readonly Label lblTotal = new Label();

        private readonly ListView lstNotifications =
            new ListView();

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        private readonly NotificationsController
            notificationsController;

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
            ClientSize = new Size(850, 560);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "MIS NOTIFICACIONES";
            lblTitle.Font = new Font(
                "Arial", 20, FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(245, 25);

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
                new Size(780, 310);

            lstNotifications.View = View.Details;
            lstNotifications.FullRowSelect = true;
            lstNotifications.GridLines = true;
            lstNotifications.MultiSelect = false;

            lstNotifications.Columns.Add(
                "N.º", 60
            );

            lstNotifications.Columns.Add(
                "Notificación", 690
            );

            btnRefresh.Text = "Actualizar";
            btnRefresh.Location = new Point(230, 495);
            btnRefresh.Size = new Size(170, 40);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose.Text = "Cerrar";
            btnClose.Location = new Point(450, 495);
            btnClose.Size = new Size(170, 40);
            btnClose.Click += BtnClose_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblUser);
            Controls.Add(lblTotal);
            Controls.Add(lstNotifications);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);
        }

        private void LoadNotifications()
        {
            try
            {
                lstNotifications.Items.Clear();

                var notifications =
                    notificationsController
                        .GetUserNotifications(
                            currentUser.Id
                        );

                int number = 1;

                foreach (string notification in notifications)
                {
                    ListViewItem item =
                        new ListViewItem(
                            number.ToString()
                        );

                    item.SubItems.Add(notification);

                    lstNotifications.Items.Add(item);

                    number++;
                }

                lblTotal.Text =
                    "Total: " + notifications.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar notificaciones: " +
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
            LoadNotifications();
        }

        private void BtnClose_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }
    }
}
