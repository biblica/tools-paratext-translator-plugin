using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.CheckManagement;

namespace TvpMain.Forms
{
    /// <summary>
    /// A form for choosing the repository where a check or group will be stored.
    /// </summary>
    public partial class ChooseRepositoryForm : Form
    {
        /// <summary>
        /// Constructor for creating a new form.
        /// </summary>
        /// <param name="repositories">A list of repository names to choose from.</param>
        public ChooseRepositoryForm(string[] repositories)
        {
            if (repositories is null) throw new ArgumentNullException(nameof(repositories));

            InitializeComponent();
            repositoryComboBox.Items.AddRange(repositories);
            promptLabel.Text = "Please choose a location:";
        }

        private void ChooseRepositoryForm_Load(object sender, EventArgs e)
        {
            repositoryComboBox.SelectedIndex = 0;
        }

        public string GetRepository()
        {
            return repositoryComboBox.SelectedItem as string;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
