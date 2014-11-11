using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESHI
{
    class Encrypt
    {
       /// <summary>
       /// These arrays are used by the Permutate() method.
       /// </summary>
        #region Permutation Tables
        public static readonly int[] pc_1 ={ 11, 15, 4, 13, 7, 9, 3,
                                                  2,  5, 14, 6, 10, 12, 1};
        public static readonly int[] pc_2 ={ 6, 11, 4, 8, 13, 3,
                                                12, 5, 1, 10, 2, 9};
        public static readonly int[] ip ={ 4, 12, 6, 10,
                                                14, 8, 16,2,
                                                5, 15, 1, 7,
                                                11, 3, 13,9};
        public static readonly int[] ip_1 ={ 11, 8, 14, 1,
                                                9, 3, 12,6,
                                                16, 4, 13, 2,
                                                15, 5, 10, 7};
        public static readonly int[] pc_e ={ 8, 1, 2, 3, 4, 5,
                                                 4, 5, 6, 7, 8, 1};
        public static readonly int[] pc_p ={ 6, 4, 7, 3,
                                                 5, 1, 8, 2 };
        //Permutation table for the 
        //public static readonly int[] left_shifts ={ 1, 1, 2, 2,
        //                                                2, 2, 2, 2,
        //                                                1, 2, 2, 2,
        //                                                2, 2, 2, 1};
        public static List<int[,]> sBoxes = new List<int[,]>();
        //S-box 1
        public static readonly int[,] s1 ={ { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 }, 
                                         { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 }, 
                                         { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 }, 
                                         { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 } };
        //S-box 2
        public static readonly int[,] s2 ={ { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 }, 
                                         { 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 }, 
                                         { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 }, 
                                         { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 } };
        static Encrypt()
        {
            sBoxes.Add(s1);
            sBoxes.Add(s2);

        }

        
        #endregion
        public string getBinaryString(string plaintext)
        {
            string binarystring = "";
            for (int i = 0; i < this.StrToBinary(plaintext).Length; i++)
            {
                binarystring += this.StrToBinary(plaintext).ElementAt(i);
            }
            return binarystring;
        }
        public List<string>[] getChunks(string plaintext, string inputType)
        {
            //FIELDS
            string binarystr = "";
            string[] inputcontains = new string[2];
            inputcontains[0]="dec";
            inputcontains[1]="bin";
            if (inputType.ToLower().Contains(inputcontains[0]))//If the inputmethod is decimal.
            {
                 binarystr = this.getBinaryString(plaintext);
            }
            else if (inputType.ToLower().Contains(inputcontains[1]))
            {

                binarystr = plaintext;
            }
            
            string[] chunks = this.To16bitChunks(binarystr);
            string[] permutatedChunks = new string[chunks.Length];
            List<string>[] splittedChunks = new List<string>[permutatedChunks.Length];

            //loops
            for (int i = 0; i < chunks.Length; i++)
            {
               permutatedChunks[i]= this.Permutate(chunks[i], ip);
            }
            for (int i = 0; i < permutatedChunks.Length; i++)
            {
                splittedChunks[i] = this.SplitInTwoParts(permutatedChunks[i]);
            }
            return splittedChunks;
        }
        public string[] generateKeys(string masterkey)
        {
           
            string[] keys = new string[2];
            string nextLeftPartShift = this.shiftLeft(Permutate(masterkey, pc_1).Substring(0,7)); 
            string nextRightPartShift = this.shiftLeft(Permutate(masterkey, pc_1).Substring(7,7)); 
            for (int i = 0; i < keys.Length; i++)
			{
			 keys[i] = this.Permutate(masterkey, pc_1);
             string leftPartShifted = nextLeftPartShift;
             nextLeftPartShift = this.shiftLeft(leftPartShifted);
             string rightPartShifted = nextRightPartShift;
             nextRightPartShift = this.shiftLeft(rightPartShifted);
             keys[i] = leftPartShifted + rightPartShifted;
             keys[i] = this.Permutate(keys[i], Encrypt.pc_2);
			}
            return keys;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rightpart0"></param>
        /// <param name="masterkey"></param>
        /// <param name="encryptOrDecrypt"></param>
        /// <returns></returns>
        public List<string> FunctionF(string rightpart0, string masterkey, char encryptOrDecrypt)
        {
            string rightpart = this.Xpand(rightpart0);
            List<string> FboxResults = new List<string>();
            string currentBinaryResults = "";
            if (encryptOrDecrypt == 'e')
            {
                
                for (int i = 0; i < 2; i++)
                {
                    //COLUMN
                    string xoredRightPart = this.Xor(rightpart, this.generateKeys(masterkey)[i]);
                    string rightpart6bits = SplitKeyIn6Bits(xoredRightPart).ElementAt(1);

                    string binaryRow = rightpart6bits.Substring(0, 1) + rightpart6bits.Substring(5, 1);
                    int intBinaryRow = Convert.ToInt16(binaryRow, 2);
                    if (intBinaryRow == 0)
                    {
                        intBinaryRow = 0;
                    }
                    else
                    {
                        intBinaryRow--;
                    }
                    string decimalRow = Convert.ToString(intBinaryRow);
                    //ROW//////////////////////////////////////////////
                    string binaryCol = rightpart6bits.Substring(1, 4);
                    int intBinaryCol = Convert.ToInt16(binaryCol, 2);
                    if (intBinaryCol == 0)
                    {
                        intBinaryCol = 0;
                    }
                    else
                    {
                        intBinaryCol--;
                    }
                    string decimalCol = Convert.ToString(intBinaryCol);
                    string tempResult = sBoxes[i][intBinaryRow, intBinaryCol].ToString();
                    string tempResultBinary = Convert.ToString(Convert.ToInt16(tempResult), 2);
                    if (tempResultBinary.Length < 4)
                    {
                        string zeros = "";
                        for (int c = 1; c <= 4 - tempResultBinary.Length; c++)
                        {
                            zeros += "0";
                        }
                        tempResultBinary = zeros + tempResultBinary;
                    }

                    currentBinaryResults += tempResultBinary;

                }
                currentBinaryResults = this.Permutate(currentBinaryResults, pc_p);
                FboxResults.Add(currentBinaryResults);
                return FboxResults;
            }
            else if (encryptOrDecrypt == 'd')
            {
                //Inverted loop so that way the keys are used in the opposite way
                for (int i = 1; i >= 0; i--)
                {
                    //COLUMN
                    string xoredRightPart = this.Xor(rightpart, this.generateKeys(masterkey)[i]);
                    string rightpart6bits = SplitKeyIn6Bits(xoredRightPart).ElementAt(1);
                        
                    string binaryRow = rightpart6bits.Substring(0, 1) + rightpart6bits.Substring(5, 1);
                    int intBinaryRow = Convert.ToInt16(binaryRow, 2);
                    if (intBinaryRow == 0)
                    {
                        intBinaryRow = 0;
                    }
                    else
                    {
                        intBinaryRow--;
                    }
                    string decimalRow = Convert.ToString(intBinaryRow);
                    //ROW//////////////////////////////////////////////
                    string binaryCol = rightpart6bits.Substring(1, 4);
                    int intBinaryCol = Convert.ToInt16(binaryCol, 2);
                    if (intBinaryCol == 0)
                    {
                        intBinaryCol = 0;
                    }
                    else
                    {
                        intBinaryCol--;
                    }
                    string decimalCol = Convert.ToString(intBinaryCol);
                    string tempResult = sBoxes[Convert.ToInt16((i==0))][intBinaryRow, intBinaryCol].ToString();
                    string tempResultBinary = Convert.ToString(Convert.ToInt16(tempResult), 2);
                    if (tempResultBinary.Length < 4)
                    {
                        string zeros = "";
                        for (int c = 1; c <= 4 - tempResultBinary.Length; c++)
                        {
                            zeros += "0";
                        }
                        tempResultBinary = zeros + tempResultBinary;
                    }

                    currentBinaryResults += tempResultBinary;

                }
                currentBinaryResults = this.Permutate(currentBinaryResults, pc_p);
                FboxResults.Add(currentBinaryResults);
                return FboxResults;
            }
            return FboxResults;
        }
        /// <summary> EncryptOneChunk COMMENTS
        /// Encrypts one part of 16bits (2 alphanum characters) using the masterkey.
        /// The method works both for encrypting and decrypting.
        /// </summary>
        /// <param name="chunk">16bit binary chunk, that will be encrypted</param>
        /// <param name="masterkey">The 16 bit key used for DESHI. Input through tbKey</param>
        /// <param name="encryptOrDecrypt">char 'e' or encrypting; 'd' for decrypting</param>
        /// <returns></returns>
        public string EncryptOneChunk(List<string> chunk, string masterkey, char encryptOrDecrypt)
        {
            //INITIALIZE Left & Right strings
            string left = chunk.ElementAt(0);
            string right = chunk.ElementAt(1);

            //listOfTwoParts will contain 3 couples of strings (L0+R0,L1+R1,L2+R2)
            List<string> listOfTwoParts = new List<string>();
            //Readylist is the L2 and R2 values, meaning the last two values from listOfTwoParts, after it's filled.
            List<string> readyList = new List<string>();

            //Add initial values to the list
            listOfTwoParts.Add(left);
            listOfTwoParts.Add(right);
            
            for (int i = 0; i < 2; i++)
            {
                string permRight = this.FunctionF(right, masterkey, 'e').ElementAt(0);
                string xored = this.Xor(left, permRight);
                left = right;
                right = xored;
                listOfTwoParts.Add(left);
                listOfTwoParts.Add(right);
            }
            //Set final values using the last 2 strings from ListOfTwoParts
            readyList = listOfTwoParts.GetRange(listOfTwoParts.Count - 2, 2);
            string concatString16ForIP = readyList.ElementAt(1) + readyList.ElementAt(0);
            string finalStringDec = this.Permutate(concatString16ForIP, ip_1);
            return finalStringDec;
        }
        /// <summary> EncryptText COMMENTS
        /// Complex method, looping through all the 16bit chunks, encrypting them.
        /// </summary>
        /// <param name="plaintext">String used from tbPlainText</param>
        /// <param name="inputType">Input value type (decimal or binary)</param>
        /// <param name="masterkey">Input encryption key (16bit size). Uses tbKey</param>
        /// <param name="encryptOrDecrypt">char 'd' for decrypt or 'e' for encrypt</param>
        /// <returns>Final binary string with the encrypted plaintext</returns>
        public string EncryptText(string plaintext, string inputType, string masterkey, char encryptOrDecrypt)
        {
            string finalResult = "";
            for (int i = 0; i < getChunks(plaintext, inputType).Length; i++)
            {
                finalResult += EncryptOneChunk(getChunks(plaintext, inputType)[i], masterkey, encryptOrDecrypt);
            }
            return finalResult;
        }
        /// <summary> Permutate COMMENTS
        /// Used for making various permutations such as 
        /// Initial Permutation, Expanding, PC1,PC2(Key generating), Permutation in Fbox, etc
        /// </summary>
        /// <param name="text">string value, which will be permutated</param>
        /// <param name="order">the permutation array used</param>
        /// <returns> Permutated text</returns>
        public string Permutate(string text, int[] order)
        {
            string PermutatedText = "";

            for (int i = 0; i < order.Length; i++)
            {
                PermutatedText += (text[order[i] - 1]);
            }

            return PermutatedText;
        }
        /// <summary> StrToBinary COMMENTS
        /// Used to convert the ASCII representation of the plaintext into binary.
        /// </summary>
        /// <param name="str">string input of the value, that will be converted</param>
        /// <returns>Array containing 8bit size binary elements</returns>
        public string[] StrToBinary(string str)
        {
            int length = str.Length;
            string[] binary = new string[length];
            byte[] arr = System.Text.Encoding.ASCII.GetBytes(str);
            for (int i = 0; i < arr.Length; i++)
            {
                binary[i] = Convert.ToString(arr[i], 2);
                if (binary[i].Length < 8)
                {
                    string zeros = "";
                    for (int j = 0; j < (8 - binary[i].Length); j++)
                    {
                        zeros += "0";
                    }
                    binary[i] = zeros + binary[i];
                }
            }
            return binary;
        }
        /// <summary> BinaryToStr COMMENTS
        /// This method is used whenever we convert a binary text into string using ASCII code.
        /// It is used on decrypting or representing encrypted bites as ASCII presentation.
        /// </summary>
        /// <param name="str">string input of the value, that will be converted</param>
        /// <returns>ASCII representation of the binary string</returns>
        public string BinaryToStr(string str)
        {
            int length = str.Length/8;
            string[] Decim = new string[length];
            byte[] arrayOfBinary = new byte[length];
            for (int i = 0; i < arrayOfBinary.Length; i++)
            {
                arrayOfBinary[i] = Convert.ToByte(str.Substring(i * 8, 8), 2);
            }

            var text = Encoding.ASCII.GetString(arrayOfBinary);
            return text;
        }
        /// <summary> To16bitChunks COMMENTS
        /// After the alphanumeric text is converted to binary, 
        /// this method is used to make an array of 16bit parts (2 plaintext characters each part)
        /// because every character is exactly 8 bits.
        /// </summary>
        /// <param name="binarystring">The binary representation of the plaintext</param>
        /// <returns></returns>
        public string[] To16bitChunks(string binarystring)
        {
            
            if (binarystring.Length % 16 != 0)
                binarystring += "00000000";
            string[] PartsOf16Bits = new string[binarystring.Length / 16];
            for (int i = 0; i < PartsOf16Bits.Length; i++)
            {
                PartsOf16Bits[i] = binarystring.Substring(i * 16, 16);
            }
            return PartsOf16Bits;
        }
        /// <summary> SplitInTwoParts COMMENTS
        /// Used to make list of Left and Right part - 8 bits each.
        /// 
        /// </summary>
        /// <param name="iptext">Initial Permutated 16 bit binary chunk</param>
        /// <returns></returns>
        public List<string> SplitInTwoParts(string iptext)
        {
            string leftpart = iptext.Substring(0, 8);
            string rightpart = iptext.Substring(8, 8);
            List<string> splitParts = new List<string>();
            splitParts.Add(leftpart);
            splitParts.Add(rightpart);
            return splitParts;
        }
        /// <summary> SplitKeyIn6Bits COMMENTS
        /// Used after an expanded right part is XORed with subkey. 
        /// This will return list of two 6bit parts, used in Sboxes.
        /// </summary>
        /// <param name="xoredText">string input of the expanded and xored right part</param>
        /// <returns></returns>
        public List<string> SplitKeyIn6Bits(string xoredText)
        {
            string left = xoredText.Substring(0, 6);
            string right = xoredText.Substring(6, 6);
            List<string> splittedParts = new List<string>();
            splittedParts.Add(left);
            splittedParts.Add(right);
            return splittedParts;
        }
        /// <summary> Xpand COMMENTS
        /// Using the expanding permutation table, this method expands the right part before xoring with subkey.
        /// </summary>
        /// <param name="rightpart">string input for 8bit right part, which will become 12bit after the expand</param>
        /// <returns></returns>
        public string Xpand(string rightpart)
        {
            string expandedtext = "";
            for (int i = 0; i < pc_e.Length; i++)
            {
                expandedtext += (rightpart[pc_e[i] - 1]);
            }
            return expandedtext;
        }
        /// <summary> XOR COMMENTS
        /// Mathematical XOR. 
        /// Assuming that : 
        /// 0+1 = 1
        /// 1+1 = 0
        /// 0+0 = 0
        /// We can say that if the two numbers xored are different, the result is 1, otherwise it's 0.
        /// USAGE:
        /// - for expanded right part xor subkey
        /// - for xoring left and right parts in the iterations
        /// </summary>
        /// <param name="inputText">string input for the text we want to xor (right part or expanded right part)  </param>
        /// <param name="secondInputText">second input (left part or key)</param>
        /// <returns></returns>
        public string Xor(string inputText, string secondInputText)
        {
            string result = "";
            for (int i = 0; i < inputText.Length; i++)
            {
                if (inputText[i] != secondInputText[i])
                {
                    result += "1";
                }
                else
                {
                    result += "0";
                }
            }
            return result;
        }
        /// <summary> SHIFT LEFT COMMENTS
        /// Used for left shifting, when a subkeys are generated.
        /// This method is called after the PC1 permutation of the masterkey and the splitting of two parts.
        /// </summary>
        /// <param name="keytoshift"> string input for the left or right part of the masterkey. </param>
        /// <returns></returns>
        public string shiftLeft(string keytoshift)
        {
            string shiftedkey = "";
            shiftedkey += keytoshift.Substring(1, keytoshift.Length - 1);
            shiftedkey += keytoshift.Substring(0, 1);
            return shiftedkey;
        }
        /// <summary> RandomString COMMENTS
        /// Generate random String using a list of characters (a-z) and (0-9)
        /// </summary>
        /// <param name="Size" >The length of the string generated. </param>
        /// <returns>returns random string</returns>
        public string RandomString(int Size)
        {
            Random rnd = new Random();
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < Size; i++)
            {
                ch = input[rnd.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
