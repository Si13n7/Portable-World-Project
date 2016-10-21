namespace AppsDownloader.UI
{
    using System;
    using System.Media;
    using System.Windows.Forms;
    using SilDev;

    public partial class LangSelectionForm : Form
    {
        private readonly string _name;
        private readonly string _section;

        public LangSelectionForm(string name, string text, string[] langs)
        {
            InitializeComponent();
            _name = Lang.GetText($"{Name}Titel");
            _section = name;
            appNameLabel.Text = text;
            langBox.Items.AddRange((object[])langs.Clone());
            langBox.SelectedIndex = 0;
        }

        private void LangSelectionForm_Load(object sender, EventArgs e)
        {
            Lang.SetControlLang(this);
            Text = _name;
        }

        private void SetArchiveLangForm_Shown(object sender, EventArgs e) =>
            SystemSounds.Asterisk.Play();

        private void OKBtn_Click(object sender, EventArgs e)
        {
            Ini.Write(_section, "ArchiveLang", langBox.GetItemText(langBox.SelectedItem));
            if (rememberLangCheck.Checked)
                Ini.Write(_section, "ArchiveLangConfirmed", rememberLangCheck.Checked);
            DialogResult = DialogResult.OK;
        }

        private void CancelBtn_Click(object sender, EventArgs e) =>
            DialogResult = DialogResult.Cancel;
    }
}
