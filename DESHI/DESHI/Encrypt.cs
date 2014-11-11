using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESHI
{
    class Encrypt
    {
        //FIELDS
        #region info tables
        private List<string> LRelements;

        
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
            public static readonly int[] left_shifts ={ 1, 1, 2, 2,
                                                        2, 2, 2, 2,
                                                        1, 2, 2, 2,
                                                        2, 2, 2, 1};
            public static List<int[,]> sBoxes = new List<int[,]>();
        
            static Encrypt()
            {
                sBoxes.Add(s1);
                sBoxes.Add(s2);
                
            }

            public static readonly int[,] s1 ={ { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 }, 
                                         { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 }, 
                                         { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 }, 
                                         { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 } };
            public static readonly int[,] s2 ={ { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 }, 
                                         { 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 }, 
                                         { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 }, 
                                         { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 } };
        #endregion
            public string Permutate(string text, int[] order)
            {
                string PermutatedText = "";

                for (int i = 0; i < order.Length; i++)
                {
                    PermutatedText += (text[order[i] - 1]);
                }

                return PermutatedText;
            }

            public string[] StrToBinary(string str)
            {
                string[] binary = new string[16];
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


            public string BinaryToStr(string str)
            {
                string[] Decim = new string[16];
                byte[] arrayOfBinary = new byte[16];
                for (int i = 0; i < arrayOfBinary.Length; i++)
                {
                    arrayOfBinary[i] = Convert.ToByte(str.Substring(0 + i * 8, 8),2);
                }
                
                var text = Encoding.ASCII.GetString(arrayOfBinary);
                return text;
            }

            public string[] To16bitChunks(string binarystring)
            {
                string [] PartsOf16Bits = new string[binarystring.Length/16];
                for (int i = 0; i < PartsOf16Bits.Length; i++)
			    {
                    PartsOf16Bits[i] = binarystring.Substring(0 + i * 16, 16);
			    }
                return PartsOf16Bits;
            }



            public List<string> SplitInTwoParts(string iptext)
            {
                string leftpart = iptext.Substring(0, 8);
                string rightpart = iptext.Substring(8, 8);
                List<string> splitParts = new List<string>();
                splitParts.Add(leftpart);
                splitParts.Add(rightpart);
                return splitParts;
            }
            
            public string Xpand(string rightpart)
            {
                string expandedtext = "";
                for (int i = 0; i < pc_e.Length; i++)
                {
                    expandedtext += (rightpart[pc_e[i] - 1]);
                }
                return expandedtext;
            }


            public string Xor(string xpandedtext,string subkey)
            {
                string result = "";
                for (int i = 0; i < xpandedtext.Length; i++)
                {
                    if (xpandedtext[i] != subkey[i])
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



            public string shiftLeft(string keytoshift)
            {
                string shiftedkey = "";
                shiftedkey += keytoshift.Substring(1, keytoshift.Length - 1);
                shiftedkey += keytoshift.Substring(0, 1);
                return shiftedkey;
            }

    }
}
