using System;
using System.Diagnostics;
using System.IO;

namespace DES_WPF
{
    public class FIPS46
    {
        public bool[] Key1;
        public bool[] Key2;
        public bool[] Key3;

        private int[] ipGuide = {
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6,
            64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17, 9,  1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7
        };
        private int[] ipInverseGuide = {
            40, 8, 48, 16, 56, 24, 64, 32,
            39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30,
            37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28,
            35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41, 9,  49, 17, 57, 25
        };
        private int[] e =
        {
            32, 1 , 2 , 3 , 4 , 5 ,
            4 , 5 , 6 , 7 , 8 , 9 ,
            8 , 9 , 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32, 1
        };
        private int[] p =
        {
            16, 7 , 20, 21,
            29, 12, 28, 17,
            1 , 15, 23, 26,
            5 , 18, 31, 10,
            2 , 8 , 24, 14,
            32, 27, 3 , 9 ,
            19, 13, 30, 6 ,
            22, 11, 4 , 25
        };
        private int[,] s =
        {
            {
                14, 4 , 13, 1 , 2 , 15, 11, 8 , 3 , 10, 6 , 12, 5 , 9 , 0 , 7 ,
                0 , 15, 7 , 4 , 14, 2 , 13, 1 , 10, 6 , 12, 11, 9 , 5 , 3 , 8 ,
                4 , 1 , 14, 8 , 13, 6 , 2 , 11, 15, 12, 9 , 7 , 3 ,10 , 5 , 0 ,
                15, 12, 8 , 2 , 4 , 9 , 1 , 7 , 5 , 11, 3 , 14, 10, 0 , 6 , 13
            },{
                15, 1 , 8 , 14, 6 , 11, 3 , 4 , 9 , 7 , 2 , 13, 12, 0 , 5 , 10,
                3 , 13, 4 , 7 , 15, 2 , 8 , 14, 12, 0 , 1 , 10, 6 , 9 , 11, 5 ,
                0 , 14, 7 , 11, 10, 4 , 13, 1 , 5 , 8 , 12, 6 , 9 , 3 , 2 , 15,
                13, 8 , 10, 1 , 3 , 15, 4 , 2 , 11, 6 , 7 , 12, 0 , 5 , 14, 9
            },{
                10, 0 , 9 , 14, 6 , 3 , 15, 5 , 1 , 13, 12, 7 , 11, 4 , 2 , 8 ,
                13, 7 , 0 , 9 , 3 , 4 , 6 , 10, 2 , 8 , 5 , 14, 12, 11, 15, 1 ,
                13, 6 , 4 , 9 , 8 , 15, 3 , 0 , 11, 1 , 2 , 12, 5 , 10, 14, 7 ,
                1 , 10, 13, 0 , 6 , 9 , 8 , 7 , 4 , 15, 14, 3 , 11, 5 , 2 , 12
            },{
                7 , 13, 14, 3 , 0 , 6 , 9 , 10, 1 , 2 , 8 , 5 , 11, 12, 4 , 15 ,
                13, 8 , 11, 5 , 6 , 15, 0 , 3 , 4 , 7 , 2 , 12, 1 , 10, 14, 9  ,
                10, 6 , 9 , 0 , 12, 11, 7 , 13, 15, 1 , 3 , 14, 5 , 2 , 8 , 4  ,
                3 , 15, 0 , 6 , 10, 1 , 13, 8 , 9 , 4 , 5 , 11, 12, 7 , 2 , 14
            },{
                2 , 12, 4 , 1 , 7 , 10, 11, 6 , 8 , 5 , 3 , 15, 13, 0 , 14, 9 ,
                14, 11, 2 , 12, 4 , 7 , 13, 1 , 5 , 0 , 15, 10, 3 , 9 , 8 , 6 ,
                4 , 2 , 1 , 11, 10, 13, 7 , 8 , 15, 9 , 12, 5 , 6 , 3 , 0 , 14,
                11, 8 , 12, 7 , 1 , 14, 2 , 13, 6 , 15, 0 , 9 , 10, 4 , 5 , 3
            },{
                12, 1 , 10, 15, 9 , 2 , 6 , 8 , 0 , 13, 3 , 4 , 14, 7 , 5 , 11,
                10, 15, 4 , 2 , 7 , 12, 9 , 5 , 6 , 1 , 13, 14, 0 , 11, 3 , 8 ,
                9 , 14, 15, 5 , 2 , 8 , 12, 3 , 7 , 0 , 4 , 10, 1 , 13, 11, 6 ,
                4 , 3 , 2 , 12, 9 , 5 , 15, 10, 11, 14, 1 , 7 , 6 , 0 , 8 , 13
            },{
                4 , 11, 2 , 14, 15, 0 , 8 , 13, 3 , 12, 9 , 7 , 5 , 10, 6 , 1 ,
                13, 0 , 11, 7 , 4 , 9 , 1 , 10, 14, 3 , 5 , 12, 2 , 15, 8 , 6 ,
                1 , 4 , 11, 13, 12, 3 , 7 , 14, 10, 15, 6 , 8 , 0 , 5 , 9 , 2 ,
                6 , 11, 13, 8 , 1 , 4 , 10, 7 , 9 , 5 , 0 , 15, 14, 2 , 3 , 12
            },{
                13, 2 , 8 , 4 , 6 , 15, 11, 1 , 10, 9 , 3 , 14, 5 , 0 , 12, 7 ,
                1 , 15, 13, 8 , 10, 3 , 7 , 4 , 12, 5 , 6 , 11, 0 , 14, 9 , 2 ,
                7 , 11, 4 , 1 , 9 , 12, 14, 2 , 0 , 6 , 10, 13, 15, 3 , 5 , 8 ,
                2 , 1 , 14, 7 , 4 , 10, 8 , 13, 15, 12, 9 , 0 , 3 , 5 , 6 , 11
            }
        };
        private int[] pc1 =
        {
            57, 49, 41, 33, 25, 17, 9 ,
            1 , 58, 50, 42, 34, 26, 18,
            10, 2 , 59, 51, 43, 35, 27,
            19, 11, 3 , 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
            7 , 62, 54, 46, 38, 30, 22,
            14, 6 , 61, 53, 45, 37, 29,
            21, 13, 5 , 28, 20, 12, 4
        };
        private int[] pc2 =
        {
            14, 17, 11, 24, 1 , 5 ,
            3 , 28, 15, 6 , 21, 10,
            23, 19, 12, 4 , 26, 8 ,
            16, 7 , 27, 20, 13, 2 ,
            41, 52, 31, 37, 47, 55,
            30, 40, 51, 45, 33, 48,
            44, 49, 39, 56, 34, 53,
            46, 42, 50, 36, 29, 32
        };
        private bool[][] kn = new bool[16][];

        private void InitialPermutation(bool[] input)
        {
            bool[] original = (bool[])input.Clone();
            for (int i = 0; i < 64; ++i)
            {
                input[i] = original[ipGuide[i] - 1];
            }
        }

        private void InitialPermutationInverse(bool[] input)
        {
            bool[] original = (bool[])input.Clone();
            for (int i = 0; i < 64; ++i)
            {
                input[i] = original[ipInverseGuide[i] - 1];
            }
        }

        private void ReverseInitialPermutation(bool[] input)
        {
            bool[] original = (bool[])input.Clone();
            for (int i = 0; i < 64; ++i)
            {
                input[ipGuide[i] - 1] = original[i];
            }
        }

        private void ReverseInitialPermutationInverse(bool[] input)
        {
            bool[] original = (bool[])input.Clone();
            for (int i = 0; i < 64; ++i)
            {
                input[ipInverseGuide[i] - 1] = original[i];
            }
        }

        private bool[] PermutedChoice1(bool[] input)
        {
            bool[] tmp = new bool[56];
            for (int i = 0; i < 56; ++i)
            {
                tmp[i] = input[pc1[i] - 1];
            }
            return tmp;
        }

        private bool[] PermutedChoice2(bool[] inputC, bool[] inputD)
        {
            bool[] k = new bool[48];
            for (int i = 0; i < 48; ++i)
            {
                if ((pc2[i] - 1) < 28)
                    k[i] = inputC[pc2[i] - 1];
                else
                    k[i] = inputD[pc2[i] - 29];
            }
            return k;
        }

        private void KeyGeneration(int KeyIndex)
        {
            bool[] p= PermutedChoice1(Key1);
            bool[] d = new bool[28];
            bool[] c = new bool[28];
            for (int i = 0; i < 56; ++i)
            {
                if (i < 28)
                {
                    c[i] = p[i];
                }
                else
                {
                    d[i % 28] = p[i];
                }
            }
            for (int i = 0; i < 16; ++i)
            {
                LeftShift(c);
                LeftShift(d);
                if (i != 0 && i != 1 && i != 15 && i != 8)
                {
                    LeftShift(c);
                    LeftShift(d);
                }
                kn[i] = PermutedChoice2(c, d);
            }
        }

        private void LeftShift(bool[] key)
        {
            bool tmp = key[0];
            for (int i = 1; i < 28; ++i)
            {
                key[i - 1] = key[i];
            }
            key[27] = tmp;
        }

        private bool[] Function(bool[] l, bool[] r, bool[] k)
        {
            bool[] tmp = new bool[48];
            for (int i = 0; i < 48; ++i)
            {
                tmp[i] = r[e[i] - 1];
                tmp[i] ^= k[i];
            }
            bool[] sr = new bool[32];
            for (int i = 0; i < 8; ++i)
            {
                bool[] b = new bool[6];
                for (int j = 0; j < 6; ++j)
                {
                    b[j] = tmp[i * 6 + j];
                }
                int row = Convert.ToInt32(b[0]) * 2 + Convert.ToInt32(b[5]);
                int col = Convert.ToInt32(b[1]) * 8 + Convert.ToInt32(b[2]) * 4 + Convert.ToInt32(b[3]) * 2 + Convert.ToInt32(b[4]);
                int x = s[i, row * 16 + col];

                sr[i * 4] = ((x / 8) >= 1);
                if((x / 8) >= 1) x -= 8;
                sr[i * 4 + 1] = ((x / 4) >= 1);
                if((x / 4) >= 1) x -= 4;
                sr[i * 4 + 2] = ((x / 2) >= 1);
                if((x / 2) >= 1) x -= 2;
                sr[i * 4 + 3] = ((x / 1) >= 1);

            }
            bool[] tmp1 = new bool[32];
            for (int i = 0; i < 32; ++i)
            {
                tmp1[i] = sr[p[i] - 1];
                tmp1[i] ^= l[i];

            }
            return tmp1;
        }

        public void Encrypt(string ins, string outs, int keyIndex)
        {
            using (BinaryReader input = new BinaryReader(
                File.Open(ins, FileMode.Open)))
            using (BinaryWriter output = new BinaryWriter(
                File.Open(outs, FileMode.Create)))
            {
                int pos = 0;
                int length = (int)input.BaseStream.Length;
                while (pos < length)
                {
                    byte[] bi = input.ReadBytes(8);
                    bool[] inn = new bool[64];
                    byte[] bo = new byte[8];
                    for (int i = 0; i < bi.Length; ++i)
                    {
                        for (int j = 7; j >= 0; --j)
                        {
                            if ((bi[i] & (1 << j)) != 0)
                            {
                                inn[i * 8 + 7-j] = true;
                            }
                        }
                    }
                    InitialPermutation(inn);

                    bool[] l = new bool[32];
                    bool[] r = new bool[32];
                    for (int i = 0; i < 64; ++i)
                    {
                        if (i < 32)
                        {
                            l[i] = inn[i];
                        }
                        else
                        {
                            r[i % 32] = inn[i];
                        }
                    }
                    bool[] tmp;
                    KeyGeneration(keyIndex);
  
                    for (int i = 0; i < 16; ++i)
                    {
                        if (i == 15)
                        {
                            l = Function(l, r, kn[i]);
                        }
                        else
                        {
                            tmp = r;
                            r = Function(l, r, kn[i]);
                            l = tmp;
                        }
                    }

                    for (int i = 0; i < 64; ++i)
                    {
                        if (i < 32)
                        {
                            inn[i] = l[i];
                        }
                        else
                        {
                            inn[i] = r[i % 32];
                        }
                    }
                            
                    InitialPermutationInverse(inn);
                            
                    for (int i = 0; i < 64; ++i)
                    {
                        if (inn[i])
                        {
                            bo[i / 8] ^= (byte)(1 << (7-(i % 8)));
                        }
                    }
                    pos += sizeof(byte) * 8;
                    output.Write(bo);
                }
            }
        }

        public void Decrypt(string ins, string outs, int keyIndex)
        {
            using (BinaryReader input = new BinaryReader(
                File.Open(ins, FileMode.Open)))
            using (BinaryWriter output = new BinaryWriter(
                File.Open(outs, FileMode.Create)))
            {
                int pos = 0;
                int length = (int)input.BaseStream.Length;
                while (pos < length)
                {
                    byte[] bi = input.ReadBytes(8);
                    bool[] inn = new bool[64];
                    byte[] bo = new byte[8];
                    for (int i = 0; i < bi.Length; ++i)
                    {
                        for (int j = 7; j >= 0; --j)
                        {
                            if ((bi[i] & (1 << j)) != 0)
                            {
                                inn[i * 8 + 7-j] = true;
                            }
                        }
                    }

                    InitialPermutation(inn);
                    bool[] l = new bool[32];
                    bool[] r = new bool[32];
                    for (int i = 0; i < 64; ++i)
                    {
                        if (i < 32)
                        {
                            l[i] = inn[i];
                        }
                        else
                        {
                            r[i % 32] = inn[i];
                        }
                    }

                    bool[] tmp;

                    KeyGeneration(keyIndex);
                    for (int i = 0; i < 16; ++i)
                    {
                        if (i == 15)
                        {
                            l = Function(l, r, kn[15 - i]);
                        }
                        else
                        {
                            tmp = r;
                            r = Function(l, r, kn[15 - i]);
                            l = tmp;
                        }
                    }
                    for (int i = 0; i < 64; ++i)
                    {
                        if (i < 32)
                        {
                            inn[i] = l[i];
                        }
                        else
                        {
                            inn[i] = r[i % 32];
                        }
                    }
                    InitialPermutationInverse(inn);

                    for (int i = 0; i < 64; ++i)
                    {
                        if (inn[i])
                        {
                            bo[i / 8] ^= (byte)(1 << (7-(i % 8)));
                        }
                    }
                    pos += sizeof(byte) * 8;
                    output.Write(bo);
                }
            }
        }
        public void TripleEncrypt(string ins, string outs)
        {
            Encrypt(ins, outs, 1);
        }
        public void TripleDecrypt(string ins, string outs)
        {
            Decrypt(ins, outs, 1);
        }
    }
}