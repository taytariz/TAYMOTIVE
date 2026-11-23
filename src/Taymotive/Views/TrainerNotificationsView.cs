
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class TrainerNotificationsView : Form
    {
        private readonly Trainer currentTrainer;

        private readonly NotificationsController
            notificationsController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblTrainer = new Label();
        private readonly Label lblTotal = new Label();

        private readonly ListView lstNotifications =
            new ListView();

        private readonly Button btnRefresh = new Button();
        private readonly Button btnClose = new Button();

        public TrainerNotificationsView(Trainer currentTrainer)
        {
            this.currentTrainer = currentTrainer ??
                throw new ArgumentNullException(
                    nameof(currentTrainer)
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

            MeasurementsController measurementsController =
                new MeasurementsController(
                    Path.Combine(filesPath, "measurements.json")
                );

            notificationsController =
                new NotificationsController(
                    routinesController,
                    visitsController,
                    badgesController,
                    measurementsController
                );

            InitializeComponents();
            LoadNotifications();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Notificaciones del entrenador";
            ClientSize = new Size(1000, 580);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "NOTIFICACIONES DEL ENTRENADOR";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Top = 25;

            lblTrainer.Text =
                "Entrenador: " + currentTrainer.Name;

            lblTrainer.Font = new Font("Arial", 11);
            lblTrainer.AutoSize = true;
            lblTrainer.Location = new Point(35, 95);

            lblTotal.Text = "Total: 0";
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(35, 130);

            lstNotifications.Location = new Point(35, 165);
            lstNotifications.Size = new Size(930, 320);

            lstNotifications.View = View.Details;
            lstNotifications.FullRowSelect = true;
            lstNotifications.GridLines = true;
            lstNotifications.MultiSelect = false;
            lstNotifications.HideSelection = false;

            lstNotifications.Columns.Add(
                "Fecha",
                170
            );

            lstNotifications.Columns.Add(
                "Tipo",
                130
            );

            lstNotifications.Columns.Add(
                "Notificación",
                600
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
            Controls.Add(lblTrainer);
            Controls.Add(lblTotal);
            Controls.Add(lstNotifications);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);

            CenterTitle();
        }

        // Cargar notificaciones del entrenador
        private void LoadNotifications()
        {
            try
            {
                lstNotifications.Items.Clear();

                List<Notification> notifications =
                    notificationsController
                        .GetTrainerTimeline(
                            currentTrainer.Id
                        );

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

                    // Colores según el tipo de notificación
                    if (notification.Type.Equals(
                        "Vencimiento",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        item.ForeColor = Color.DarkOrange;
                    }
                    else if (notification.Type.Equals(
                        "Medición",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        item.ForeColor = Color.DarkBlue;
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

        // Centrar título
        private void CenterTitle()
        {
            lblTitle.Left =
                (ClientSize.Width - lblTitle.Width) / 2;
        }
    }
}
