using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DESHI
{
    public partial class Form1 : Form
    {
        Encrypt enc;
        public Form1()
        {
            InitializeComponent();
            enc = new Encrypt();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            
            string InitialPermutated = enc.DoInitialPermutation(tbPlainText.Text, Encrypt.ip);
            tbCipherText.Text = InitialPermutated;
            lbInfo.Items.Add("Permutated: " + InitialPermutated);
            foreach (string parts in enc.SplitInTwoParts(InitialPermutated))
            {
                   lbInfo.Items.Add("Splitted *8bit* : " + parts + " --");
            }



        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            string InitialDePermutated = enc.DoInitialPermutation(tbCipherText.Text, Encrypt.ip_1);

            lbInfo.Items.Add("DePermutated: " + InitialDePermutated);

        }
    }
}
