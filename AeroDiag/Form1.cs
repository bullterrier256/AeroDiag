namespace AeroDiag
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void loadAeroDataButton_Click(object sender, EventArgs e)
        {
            string aeroData;
            DateTime date = dateComboBox.Value;
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            bool ok = WebGetter.TryGetAeroData(
                pointTextBox.Text
                , String.Format("{0:d4}", year) + String.Format("{0:d2}", month) + String.Format("{0:d2}", day) + timeComboBox.Text
                , out aeroData
            );
            aeroDataText.Text = ok ? aeroData.Replace("\n", "\r\n") : "нет данных. \r\n" + aeroData;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timeComboBox.SelectedItem = timeComboBox.Items[0];
        }
    }
}