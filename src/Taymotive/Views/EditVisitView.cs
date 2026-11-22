
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class EditVisitView : Form
    {
        private readonly Label lblTitle = new Label();

        private readonly ComboBox cmbUsers = new ComboBox();
        private readonly ComboBox cmbClasses = new ComboBox();
        private readonly DateTimePicker dtpDate = new DateTimePicker();

        private readonly Button btnSave = new Button();
        private readonly Button btnCancel = new Button();

        private readonly VisitsController visitsController;
        private readonly UsersController usersController;

        private readonly int visitId;

        public EditVisitView(int id)
        {
            visitId = id;

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
            LoadUsers();
            LoadVisit();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Modificar visita";
            this.Size = new Size(550, 390);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "MODIFICAR VISITA";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(135, 30);

            AddLabel("Usuario:", 65, 110);

            cmbUsers.Location = new Point(220, 105);
            cmbUsers.Width = 250;
            cmbUsers.DropDownStyle = ComboBoxStyle.DropDownList;

            AddLabel("Fecha de visita:", 65, 160);

            dtpDate.Location = new Point(220, 155);
            dtpDate.Width = 250;
            dtpDate.Format = DateTimePickerFormat.Short;

            AddLabel("Clase:", 65, 210);

            cmbClasses.Location = new Point(220, 205);
            cmbClasses.Width = 250;
            cmbClasses.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbClasses.Items.AddRange(new object[]
            {
                "Yoga",
                "Spinning",
                "CrossFit",
                "Funcional",
                "Musculacion"
            });

            btnSave.Text = "Guardar cambios";
            btnSave.Location = new Point(115, 275);
            btnSave.Size = new Size(140, 40);
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancelar";
            btnCancel.Location = new Point(285, 275);
            btnCancel.Size = new Size(140, 40);
            btnCancel.Click += BtnCancel_Click;

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(cmbUsers);
            this.Controls.Add(dtpDate);
            this.Controls.Add(cmbClasses);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label label = new Label();

            label.Text = text;
            label.Location = new Point(x, y);
            label.AutoSize = true;

            this.Controls.Add(label);
        }

        private void LoadUsers()
        {
            try
            {
                cmbUsers.Items.Clear();

                var users = usersController.GetUsers();

                foreach (var user in users)
                {
                    cmbUsers.Items.Add(
                        new UserOption(user.Id, user.Name)
                    );
                }
            }
            catch (Exception ex)
            {
                btnSave.Enabled = false;

                MessageBox.Show(
                    "Error al cargar usuarios: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadVisit()
        {
            try
            {
                var visit = visitsController.GetVisitById(visitId);

                if (visit == null)
                {
                    btnSave.Enabled = false;

                    MessageBox.Show(
                        "No se encontró la visita.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                dtpDate.Value = visit.VisitDate;

                bool userFound = false;

                for (int i = 0; i < cmbUsers.Items.Count; i++)
                {
                    if (cmbUsers.Items[i] is UserOption user &&
                        user.Id == visit.UserId)
                    {
                        cmbUsers.SelectedIndex = i;
                        userFound = true;
                        break;
                    }
                }

                if (!userFound)
                {
                    btnSave.Enabled = false;

                    MessageBox.Show(
                        "El usuario de esta visita no existe.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                int classIndex = cmbClasses.FindStringExact(
                    visit.ClassName
                );

                if (classIndex >= 0)
                {
                    cmbClasses.SelectedIndex = classIndex;
                }
                else
                {
                    cmbClasses.Items.Add(visit.ClassName);
                    cmbClasses.SelectedItem = visit.ClassName;
                }
            }
            catch (Exception ex)
            {
                btnSave.Enabled = false;

                MessageBox.Show(
                    "Error al cargar visita: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (cmbUsers.SelectedItem is not UserOption selectedUser)
            {
                MessageBox.Show(
                    "Debe seleccionar un usuario.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (cmbClasses.SelectedItem == null)
            {
                MessageBox.Show(
                    "Debe seleccionar una clase.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                bool updated = visitsController.UpdateVisit(
                    visitId,
                    selectedUser.Id,
                    dtpDate.Value.Date,
                    cmbClasses.SelectedItem.ToString() ?? ""
                );

                if (updated)
                {
                    MessageBox.Show(
                        "Visita modificada correctamente.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo modificar la visita.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al modificar visita: " + ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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
                return $"{Id} - {Name}";
            }
        }
    }
}
