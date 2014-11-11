using System;
using System.Windows.Forms;

namespace DESHI
{
    public partial class Form1 : Form
    {
        #region FIELDS & InitializeComponent()
        //Object enc from the class Encrypt, which does all the functionality. 
        //You can also decrypt using the same methods, just changing one parameter. 
        Encrypt enc;
        //variables to store the encryted and decrypted values.
        //When it's decrypted, the "encrypted" value is used. 
        string encrypted, decrypted = "";
       
        public Form1()
        {
            InitializeComponent();
            enc = new Encrypt();
        }
        #endregion
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            #region Clear ListBox & Check if Texboxes are empty
            
            if ((tbPlainText.Text == "") || (tbKey.Text == ""))
                    goto Finish;//If key or plain text are not filled go to end of code 
            this.lbInfo.Items.Clear();
            #endregion 
            #region Display generated keys, using the MASTER key from tbKey
            for (int i = 0; i < enc.generateKeys(tbKey.Text).Length; i++)
            { 
                lbInfo.Items.Add("Subkey " + (i + 1));
                lbInfo.Items.Add(enc.generateKeys(tbKey.Text)[i]);
                lbInfo.Items.Add("");
            }
            #endregion

            //GET FINAL ENCRYPTED VALUE. The input type can be written in any way. It just needs to contain "dec".          
            encrypted = enc.EncryptText(tbPlainText.Text, "DECIMAL", tbKey.Text, 'e');
            #region Display Encypted value in bits & ASCII 
            lbInfo.Items.Add("Encrypted bits:");
            lbInfo.Items.Add("");
            for (int i = 0; i < encrypted.Length/16; i++)
            {
                lbInfo.Items.Add(encrypted.Substring(i * 16, 16) + "\n");
            }
            lbInfo.Items.Add("");
            lbInfo.Items.Add("Alphanumeric(ASCII) encrypted:");
            lbInfo.Items.Add(enc.BinaryToStr(encrypted));
            Finish:
            if ((tbPlainText.Text == "") || (tbKey.Text == ""))
                MessageBox.Show("You need to fill text & key in the fields!");
            #endregion
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            #region Clear ListBox & Check Fields
            this.lbInfo.Items.Clear();
            //In decrypting it doesn't matter if the plaintext is empty, because we use the encrypted generated with this key. 
            if (tbKey.Text == "")
                goto Finish;//Go to end of code and display mbox 
            #endregion
            #region Display generated keys, using the MASTER key from tbKey
            for (int i = 0; i < enc.generateKeys(tbKey.Text).Length; i++)
            {
                lbInfo.Items.Add("Subkey " + (i + 1));
                lbInfo.Items.Add(enc.generateKeys(tbKey.Text)[i]);
                lbInfo.Items.Add("");
            }
            #endregion
            //GET FINAL DECRYPTED VALUE. The input type can be written in any way. It just needs to contain "bin".
            //d for decrypt, e for encrypt
            decrypted =  enc.EncryptText(encrypted, "binary", tbKey.Text, 'd');
            #region ListBox strings & Finish:
            lbInfo.Items.Add("Decrypted bits:");
            lbInfo.Items.Add("");
            for (int i = 0; i < decrypted.Length/16; i++)
            {
                lbInfo.Items.Add(decrypted.Substring(i * 16, 16) +
               "\n");
            }
            lbInfo.Items.Add("");
            lbInfo.Items.Add("Alphanumeric(ASCII) decrypted: ");
            lbInfo.Items.Add(enc.BinaryToStr(decrypted));
            Finish:
            //When we decrypt, plaintext texbox doesn't matter, because we use the encrypted value stored.
            if (tbKey.Text == "")
                MessageBox.Show("You need to fill a key!");
            #endregion
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lbInfo.Items.Clear();
            tbCipherText.Clear();
            tbKey.Clear();
            tbPlainText.Clear();
        }
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            
            Random num = new Random();
            //Generate random alphanumeric text using RandomString, using a list of characters a-z & 0-9.
            tbPlainText.Text = enc.RandomString(25);
            string randomedKey = "";
            //DESHI encryption uses 16 bit key, so we generate random one with this length.
            for (int i = 1; i <= 16; i++)
            {
                randomedKey += Convert.ToString(num.Next(0, 2));
            }
            tbKey.Text = randomedKey;

        }
    }
}