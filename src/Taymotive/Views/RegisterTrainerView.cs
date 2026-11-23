
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class RegisterTrainerView : Form
    {
        private readonly Label lblTitle = new Label();

        private readonly TextBox txtName = new TextBox();

        private readonly NumericUpDown numAge = new NumericUpDown();
        private readonly NumericUpDown numWeight = new NumericUpDown();
        private readonly NumericUpDown numHeight = new NumericUpDown();

        private readonly ComboBox cmbSpecialty = new ComboBox();
        private readonly CheckBox chkPersonalTrainer = new CheckBox();

        private readonly Button btnSave = new Button();
        private readonly Button btnCancel = new Button();

        private readonly TrainersController trainersController;

        public RegisterTrainerView()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "trainers.json"
            );

            trainersController = new TrainersController(filePath);

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Registrar entrenador";
            this.Size = new Size(520, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle.Text = "REGISTRAR ENTRENADOR";
            lblTitle.Font = new Font(
                "Arial",
                18,
                FontStyle.Bold
            );
            lblTitle.Location = new Point(85, 25);
            lblTitle.AutoSize = true;

            AddLabel("Nombre:", 60, 100);
            txtName.Location = new Point(220, 95);
            txtName.Width = 220;

            AddLabel("Edad:", 60, 145);
            numAge.Location = new Point(220, 140);
            numAge.Width = 220;
            numAge.Minimum = 18;
            numAge.Maximum = 100;
            numAge.Value = 25;

            AddLabel("Peso (kg):", 60, 190);
            numWeight.Location = new Point(220, 185);
            numWeight.Width = 220;
            numWeight.Minimum = 1;
            numWeight.Maximum = 500;
            numWeight.DecimalPlaces = 2;
            numWeight.Value = 70;

            AddLabel("Estatura (m):", 60, 235);
            numHeight.Location = new Point(220, 230);
            numHeight.Width = 220;
            numHeight.Minimum = 0.50M;
            numHeight.Maximum = 2.50M;
            numHeight.DecimalPlaces = 2;
            numHeight.Increment = 0.01M;
            numHeight.Value = 1.70M;

            AddLabel("Especialidad:", 60, 280);
            cmbSpecialty.Location = new Point(220, 275);
            cmbSpecialty.Width = 220;
            cmbSpecialty.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbSpecialty.Items.AddRange(new object[]
            {
                "Musculacion",
                "CrossFit",
                "Yoga",
                "Spinning",
                "Entrenamiento funcional"
            });

            cmbSpecialty.SelectedIndex = 0;

            chkPersonalTrainer.Text = "Entrenador personal";
            chkPersonalTrainer.Location = new Point(220, 320);
            chkPersonalTrainer.AutoSize = true;

            btnSave.Text = "Guardar";
            btnSave.Location = new Point(95, 380);
            btnSave.Size = new Size(140, 40);
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancelar";
            btnCancel.Location = new Point(265, 380);
            btnCancel.Size = new Size(140, 40);
            btnCancel.Click += BtnCancel_Click;

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtName);
            this.Controls.Add(numAge);
            this.Controls.Add(numWeight);
            this.Controls.Add(numHeight);
            this.Controls.Add(cmbSpecialty);
            this.Controls.Add(chkPersonalTrainer);
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

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                bool registered = trainersController.RegisterTrainer(
                    txtName.Text,
                    (int)numAge.Value,
                    (double)numWeight.Value,
                    (double)numHeight.Value,
                    cmbSpecialty.Text,
                    chkPersonalTrainer.Checked
                );

                if (registered)
                {
                    MessageBox.Show(
                        "Entrenador registrado correctamente.",
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
                        "Verifique los datos ingresados.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al registrar entrenador: " + ex.Message,
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
    }
}
