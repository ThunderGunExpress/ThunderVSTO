using Microsoft.Office.Tools.Ribbon;

namespace ThunderPhish
{
    public partial class Ribbon1
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            Form1 test = new Form1();
            test.Show();
        }
    }
}
