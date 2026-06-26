using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using LinearFit;
using System.Runtime.InteropServices;

namespace InterleaverDateKit
{
    public struct ScanParam816x
    {
        public uint dwSize;				// no use, default 0
        public double dblStartWL;
        public double dblStopWL;
        public double dblStepSize;
        public double dblTLSPower;
        public double dblPMPower;
        public uint dwScanTimes;		// specify the scan times of 816x, default 1
        public uint dwScannedChCount;	// how many channels for scan at the same time
        public uint dwScanHighChId;		// which channel(iChId) for scan in the range of 32~63, dwScanHighChId = (DWORD)pow(2, iChId % 31)
        public uint dwScanLowChId;		// which channel(iChId) for scan in the range of 0~31, dwScanLowChId = (DWORD)pow(2, iChId)
        public uint dwSampleCount;		// return from 816x function
    };
   
    public struct ScanParam
    {
        public bool bReference;
        public bool bDoPDL;

        public uint dwSize;           // no use here
        public uint dwSlotIndex;      // no use here
        public uint dwChannelIndex;   // no use here

        public double dblAlphaValue;   // alpha search value

        public ScanParam816x stOp816XScanParam;
    };
    public class Ini
    {
        // 声明INI文件的写操作函数 WritePrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);
       
      
        public void Writue(string section, string key, string value, string path)
        {
            // section=配置节，key=键名，value=键值，path=路径
            WritePrivateProfileString(section, key, value, path);
        }
        public string ReadValue(string section, string key, string path)
        {
            // 每次从ini中读取多少字节
            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
            // section=配置节，key=键名，temp=上面，path=路径
            GetPrivateProfileString(section, key, "", temp, 255, path);
            return temp.ToString();
        }
    
}
    class Data
    {
        static public ArrayList m_aldblFreq1 = new ArrayList();
        static public ArrayList m_aldblFreq2 = new ArrayList();
        static public ArrayList m_aldblIL = new ArrayList();
        static public ArrayList m_aldblCD = new ArrayList();
        static public ArrayList m_aldblGD = new ArrayList();
        static public ArrayList m_aldblPhase = new ArrayList();
        static public ArrayList m_aldblPDL = new ArrayList();
        //static public ArrayList m_aldblDGD = new ArrayList();
        static public ArrayList m_aldblPMD = new ArrayList();
        static public ArrayList m_aldblAverageCDZeroPoint = new ArrayList();
        static public ArrayList m_aldblFitCDZeroPoint = new ArrayList();
        static public ArrayList m_aldblWorstCDVal = new ArrayList();
        static public void zerodata()
        {
            m_aldblFreq1.Clear();
            m_aldblFreq2.Clear();
            m_aldblIL.Clear();
            m_aldblCD.Clear();
            m_aldblGD.Clear();
            m_aldblPhase.Clear();
            m_aldblPDL.Clear();
            m_aldblPMD.Clear();
            m_aldblFitCDZeroPoint.Clear();
        }
        static public void getvaluefromfinalScanfile(string strfilename)
        {
            m_aldblFreq1.Clear();
            m_aldblIL.Clear();
            m_aldblPDL.Clear();

            StreamReader readfile = new StreamReader(strfilename);
            try
            {

                for (int i = 0; i < 3; i++)
                {
                    string strread = readfile.ReadLine();
                }

                while (readfile.EndOfStream == false)
                {
                    string strread = readfile.ReadLine();

                    string[] strtemp = strread.Split(new char[]{'\t', ','});

                    if (strtemp[0].Length > 0)
                    {
                        m_aldblFreq1.Add(double.Parse(strtemp[0]));
                    }

                    if (strtemp[1].Length > 0)
                        m_aldblIL.Add(double.Parse(strtemp[1]));

                    if (strtemp[2].Length > 0)
                        m_aldblPDL.Add(double.Parse(strtemp[2]));
                }

                readfile.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                readfile.Close();
                throw ex;
            }
        }

        static public void getvaluefromfileLuna(string strfilename)
        {
            m_aldblFreq1.Clear();
            m_aldblFreq2.Clear();
            m_aldblIL.Clear();
            m_aldblCD.Clear();
            m_aldblGD.Clear();
            m_aldblPDL.Clear();
            m_aldblPMD.Clear();
           // m_aldblDGD.Clear();
            m_aldblPhase.Clear();
            m_aldblFitCDZeroPoint.Clear();

            StreamReader readfile = new StreamReader(strfilename);
            try
            {

                for (int i = 0; i < 9; i++)
                {
                    string strread = readfile.ReadLine();
                }

                while (readfile.EndOfStream == false)
                {
                    string strread = readfile.ReadLine();

                    string[] strtemp = strread.Split('\t');

                    if (strtemp[1].Length > 0)
                    {
                        m_aldblFreq1.Add(double.Parse(strtemp[1]));
                        m_aldblFreq2.Add(double.Parse(strtemp[1]));
                    }

                    if (strtemp[3].Length > 0)
                        m_aldblGD.Add(double.Parse(strtemp[3]));

                    if (strtemp[2].Length > 0)
                        m_aldblIL.Add(double.Parse(strtemp[2]));

                    if (strtemp[4].Length > 0)
                        m_aldblCD.Add(double.Parse(strtemp[4]));

                    if (strtemp[7].Length > 0)
                        m_aldblPhase.Add(double.Parse(strtemp[7]));

                    if (strtemp[5].Length > 0)
                        m_aldblPDL.Add(double.Parse(strtemp[5]));

                    if (strtemp[6].Length > 0)
                        m_aldblPMD.Add(double.Parse(strtemp[6]));
                }

                readfile.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                readfile.Close();
                throw ex;
            }
        }

        static public void getvaluefromfile(string strfilename)
        {
            m_aldblFreq1.Clear();
            m_aldblFreq2.Clear();
            m_aldblIL.Clear();
            m_aldblCD.Clear();
            m_aldblGD.Clear();
            m_aldblPhase.Clear();
            m_aldblPDL.Clear();
            m_aldblPMD.Clear();
            m_aldblFitCDZeroPoint.Clear();

            StreamReader readfile = new StreamReader(strfilename);
            try
            {
                string strread = readfile.ReadLine();

                while (readfile.EndOfStream == false)
                {
                    strread = readfile.ReadLine();

                    string[] strtemp = strread.Split(',');

                    if (strtemp.Length < 6)
                    {
                        throw new Exception("not enough element in file!");
                    }

                    if (strtemp[0].Length > 0)
                        m_aldblFreq1.Add(double.Parse(strtemp[0]));

                    if (strtemp[1].Length > 0)
                        m_aldblGD.Add(double.Parse(strtemp[1]));

                    if (strtemp[2].Length > 0)
                        m_aldblIL.Add(double.Parse(strtemp[2]));

                    if (strtemp[3].Length > 0)
                        m_aldblPhase.Add(double.Parse(strtemp[3]));

                    if (strtemp.Length == 6)
                    {
                        if (strtemp[4].Length > 0)
                            m_aldblFreq2.Add(double.Parse(strtemp[4]));

                        if (strtemp[5].Length > 0)
                            m_aldblCD.Add(double.Parse(strtemp[5]));
                    }
                    else if (strtemp.Length ==8)
                    {
                        if (strtemp[4].Length > 0)
                            m_aldblPDL.Add(double.Parse(strtemp[4]));

                        if (strtemp[5].Length > 0)
                            m_aldblPMD.Add(double.Parse(strtemp[5]));

                        if (strtemp[6].Length > 0)
                            m_aldblFreq2.Add(double.Parse(strtemp[6]));

                        if (strtemp[7].Length > 0)
                            m_aldblCD.Add(double.Parse(strtemp[7]));

                        //if (strtemp[8].Length > 0)
                        //    m_aldblDGD.Add(double.Parse(strtemp[8]));
                    }
                }
                readfile.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                readfile.Close();
                throw ex;
            }
        }

        static public void getValue(ArrayList alFreq1, ArrayList alFreq2, ArrayList alIL, ArrayList alCD, ArrayList alGD, ArrayList alPhase)
        {
            m_aldblFreq1 = alFreq1;
            m_aldblFreq2 = alFreq2;
            m_aldblCD = alCD;
            m_aldblGD = alGD;
            m_aldblIL = alIL;
            m_aldblPhase = alPhase;
        }

        static public bool valueIntegrated()
        {
            try
            {
                if (m_aldblFreq1.Count == 0)
                    return false;

                if (m_aldblFreq1.Count != m_aldblIL.Count)
                    return false;

                if (m_aldblFreq1.Count != m_aldblGD.Count)
                    return false;

                if (m_aldblFreq1.Count != m_aldblPhase.Count)
                    return false;

                if (m_aldblFreq2.Count == 0)
                    return false;

                if (m_aldblFreq2.Count != m_aldblCD.Count)
                    return false;

                return true;
            }

            catch 
            {
                return false;
            }
        }

        static public int GetIndexByValueFromArray(Object objValue, ArrayList alSourceArray)
        {
            for (int i = 0; i < alSourceArray.Count; i++)
            {
                if (objValue == alSourceArray[i])
                    return i;
            }
            return -1;
        }

        static public int GetIndexByValueFromArray(double dblValue, double[] dblSourceArray,bool bOut=true)
        {
            if (dblSourceArray[dblSourceArray.Length - 1] > dblSourceArray[0])
            {
                if (dblValue < dblSourceArray[0])
                    return 0;
                if (dblValue > dblSourceArray[dblSourceArray.Length - 1])
                    return dblSourceArray.Length - 1;
            }
            else
            {
                if (dblValue > dblSourceArray[0])
                    return 0;
                if (dblValue < dblSourceArray[dblSourceArray.Length - 1])
                    return dblSourceArray.Length - 1;
            }

            for (int i = 1; i < dblSourceArray.Length; i++)
            {
                if(bOut)
                {
                    if (((dblSourceArray[i] - dblValue) * (dblSourceArray[i - 1] - dblValue)) <= 0)
                    {
                        return i;
                    }
                }
                else
                {
                    if (((dblSourceArray[i] - dblValue) * (dblSourceArray[i + 1] - dblValue)) <= 0)
                    {
                        return i;
                    }
                }
               
            }

            return -1;
        }

        static public int FindFirstIndex(double dblVal, double[] dblSource, int nStart, int nEnd)
        {
            int nTotalCount = Math.Abs(nStart - nEnd) + 1;
            if (nTotalCount == 1)
                return nStart;

            double[] dblSecArray = new double[nTotalCount];

            int nMinIndex = 0, nMaxIndex = 0;
            FindMinMaxIndex(dblSource, nStart, nEnd, ref nMinIndex, ref nMaxIndex);
            if ((dblVal < dblSource[nMinIndex]) || (dblVal > dblSource[nMaxIndex]))
                return nStart;

            int nDir = Math.Abs(nStart - nEnd) / (nStart - nEnd);
            int nCurIndex = nStart;
            for (int i = 0; i < nTotalCount; i++, nCurIndex -= nDir)
            {
                if (nCurIndex == 0)
                    continue;
                if (((dblSource[nCurIndex] - dblVal) * (dblSource[nCurIndex - 1] - dblVal)) <= 0)
                {
                    return nCurIndex;
                }
                //nCurIndex -= nDir;
            }
            return nStart;
        }

        static public int GetIndexByValueFromArray(double dblValue, double[] dblSourceArray, int nStart, int nEnd)
        {
            //if (nStart > nEnd)
            //{
            //    nStart = nStart + nEnd;
            //    nEnd = nStart - nEnd;
            //    nStart = nStart - nEnd;
            //}

            int nTotalCount = Math.Abs(nStart - nEnd) + 1;
            double[] dblSecArray = new double[nTotalCount];

            if (nStart == nEnd)
                return nStart;

            int nDir = Math.Abs(nStart - nEnd) / (nStart - nEnd);
            int nCurIndex = nStart;        
            for (int i = 0; i < nTotalCount; i ++)
            {
                dblSecArray[i] = dblSourceArray[nCurIndex];
                nCurIndex -= nDir;
            }

            return (nStart - nDir * GetIndexByValueFromArray(dblValue, dblSecArray));
        }

        static public void FindMinMaxIndex(double[] dblSourceArray, ref int nMinIndex, ref int nMaxIndex)
        {
            double dblTempMin = dblSourceArray[0];
            double dblTempMax = dblSourceArray[0];
            nMinIndex = 0;
            nMaxIndex = 0;

            for (int i = 0; i < dblSourceArray.Length; i++)
            {
                if (dblSourceArray[i] < dblTempMin)
                {
                    dblTempMin = dblSourceArray[i];
                    nMinIndex = i;
                }

                if (dblSourceArray[i] > dblTempMax)
                {
                    dblTempMax = dblSourceArray[i];
                    nMaxIndex = i;
                }
            }
        }

        static public bool FindMinMaxIndex(double[] dblSourceArray, int nStart, int nEnd, ref int nMinIndex, ref int nMaxIndex)
        {
            if (nStart >= dblSourceArray.Length)
                return false;
            if (nEnd >= dblSourceArray.Length)
                return false;

            double[] dblSecArray = new double[Math.Abs(nEnd - nStart) + 1];
            if (nStart > nEnd)
            {
                nStart = nStart + nEnd ;
                nEnd = nStart - nEnd;
                nStart = nStart - nEnd;
            }
            for (int i = nStart; i <= nEnd; i++)
            {
                dblSecArray[i - nStart] = dblSourceArray[i];
            }

            FindMinMaxIndex(dblSecArray, ref nMinIndex, ref nMaxIndex);

            nMinIndex += nStart;
            nMaxIndex += nStart;

            return true;
        }

        static public void GetRangeIndexFromArray(double dblMin, double dblMax, double[] dblSourceArray, ArrayList nIndexArray)
        {
            nIndexArray.Clear();
            for (int i = 0; i < dblSourceArray.Length; i++)
            {
                if ((dblSourceArray[i] >= dblMin-0.001) && (dblSourceArray[i] <= dblMax+0.001))
                {
                    nIndexArray.Add(i);
                }
            }
        }
    }
    class Datanew
    {
         public ArrayList m_aldblFreq1 = new ArrayList();
         public ArrayList m_aldblFreq2 = new ArrayList();
         public ArrayList m_aldblIL = new ArrayList();
         public ArrayList m_aldblCD = new ArrayList();
         public ArrayList m_aldblGD = new ArrayList();
         public ArrayList m_aldblPhase = new ArrayList();
         public ArrayList m_aldblPDL = new ArrayList();
        //static public ArrayList m_aldblDGD = new ArrayList();
         public ArrayList m_aldblPMD = new ArrayList();
         public ArrayList m_aldblAverageCDZeroPoint = new ArrayList();
         public ArrayList m_aldblFitCDZeroPoint = new ArrayList();
         public ArrayList m_aldblWorstCDVal = new ArrayList();

         public void getvaluefromfinalScanfile(string strfilename)
        {
            m_aldblFreq1.Clear();
            m_aldblIL.Clear();
            m_aldblPDL.Clear();

            StreamReader readfile = new StreamReader(strfilename);
            try
            {

                for (int i = 0; i < 3; i++)
                {
                    string strread = readfile.ReadLine();
                }

                while (readfile.EndOfStream == false)
                {
                    string strread = readfile.ReadLine();

                    string[] strtemp = strread.Split(new char[] { '\t', ',' });

                    if (strtemp[0].Length > 0)
                    {
                        m_aldblFreq1.Add(double.Parse(strtemp[0]));
                    }

                    if (strtemp[1].Length > 0)
                        m_aldblIL.Add(double.Parse(strtemp[1]));

                    if (strtemp[2].Length > 0)
                        m_aldblPDL.Add(double.Parse(strtemp[2]));
                }

                readfile.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                readfile.Close();
                throw ex;
            }
        }

         public void getvaluefromfileLuna(string strfilename)
        {
            m_aldblFreq1.Clear();
            m_aldblFreq2.Clear();
            m_aldblIL.Clear();
            m_aldblCD.Clear();
            m_aldblGD.Clear();
            m_aldblPDL.Clear();
            m_aldblPMD.Clear();
            // m_aldblDGD.Clear();
            m_aldblPhase.Clear();
            m_aldblFitCDZeroPoint.Clear();

            StreamReader readfile = new StreamReader(strfilename);
            try
            {

                for (int i = 0; i < 9; i++)
                {
                    string strread = readfile.ReadLine();
                }

                while (readfile.EndOfStream == false)
                {
                    string strread = readfile.ReadLine();

                    string[] strtemp = strread.Split('\t');

                    if (strtemp[1].Length > 0)
                    {
                        m_aldblFreq1.Add(double.Parse(strtemp[1]));
                        m_aldblFreq2.Add(double.Parse(strtemp[1]));
                    }

                    if (strtemp[3].Length > 0)
                        m_aldblGD.Add(double.Parse(strtemp[3]));

                    if (strtemp[2].Length > 0)
                        m_aldblIL.Add(double.Parse(strtemp[2]));

                    if (strtemp[4].Length > 0)
                        m_aldblCD.Add(double.Parse(strtemp[4]));

                    if (strtemp[7].Length > 0)
                        m_aldblPhase.Add(double.Parse(strtemp[7]));

                    if (strtemp[5].Length > 0)
                        m_aldblPDL.Add(double.Parse(strtemp[5]));

                    if (strtemp[6].Length > 0)
                        m_aldblPMD.Add(double.Parse(strtemp[6]));
                }

                readfile.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                readfile.Close();
                throw ex;
            }
        }

         public void getvaluefromfile(string strfilename)
        {
            m_aldblFreq1.Clear();
            m_aldblFreq2.Clear();
            m_aldblIL.Clear();
            m_aldblCD.Clear();
            m_aldblGD.Clear();
            m_aldblPhase.Clear();
            m_aldblPDL.Clear();
            m_aldblPMD.Clear();
            m_aldblFitCDZeroPoint.Clear();

            StreamReader readfile = new StreamReader(strfilename);
            try
            {
                string strread = readfile.ReadLine();

                while (readfile.EndOfStream == false)
                {
                    strread = readfile.ReadLine();

                    string[] strtemp = strread.Split(',');

                    if (strtemp.Length < 6)
                    {
                        throw new Exception("not enough element in file!");
                    }

                    if (strtemp[0].Length > 0)
                        m_aldblFreq1.Add(double.Parse(strtemp[0]));

                    if (strtemp[1].Length > 0)
                        m_aldblGD.Add(double.Parse(strtemp[1]));

                    if (strtemp[2].Length > 0)
                        m_aldblIL.Add(double.Parse(strtemp[2]));

                    if (strtemp[3].Length > 0)
                        m_aldblPhase.Add(double.Parse(strtemp[3]));

                    if (strtemp.Length == 6)
                    {
                        if (strtemp[4].Length > 0)
                            m_aldblFreq2.Add(double.Parse(strtemp[4]));

                        if (strtemp[5].Length > 0)
                            m_aldblCD.Add(double.Parse(strtemp[5]));
                    }
                    else if (strtemp.Length == 8)
                    {
                        if (strtemp[4].Length > 0)
                            m_aldblPDL.Add(double.Parse(strtemp[4]));

                        if (strtemp[5].Length > 0)
                            m_aldblPMD.Add(double.Parse(strtemp[5]));

                        if (strtemp[6].Length > 0)
                            m_aldblFreq2.Add(double.Parse(strtemp[6]));

                        if (strtemp[7].Length > 0)
                            m_aldblCD.Add(double.Parse(strtemp[7]));

                        //if (strtemp[8].Length > 0)
                        //    m_aldblDGD.Add(double.Parse(strtemp[8]));
                    }
                }
                readfile.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                readfile.Close();
                throw ex;
            }
        }

         public void getValue(ArrayList alFreq1, ArrayList alFreq2, ArrayList alIL, ArrayList alCD, ArrayList alGD, ArrayList alPhase)
        {
            m_aldblFreq1 = alFreq1;
            m_aldblFreq2 = alFreq2;
            m_aldblCD = alCD;
            m_aldblGD = alGD;
            m_aldblIL = alIL;
            m_aldblPhase = alPhase;
        }

         public bool valueIntegrated()
        {
            try
            {
                if (m_aldblFreq1.Count == 0)
                    return false;

                if (m_aldblFreq1.Count != m_aldblIL.Count)
                    return false;

                if (m_aldblFreq1.Count != m_aldblGD.Count)
                    return false;

                if (m_aldblFreq1.Count != m_aldblPhase.Count)
                    return false;

                if (m_aldblFreq2.Count == 0)
                    return false;

                if (m_aldblFreq2.Count != m_aldblCD.Count)
                    return false;

                return true;
            }

            catch
            {
                return false;
            }
        }

         public int GetIndexByValueFromArray(Object objValue, ArrayList alSourceArray)
        {
            for (int i = 0; i < alSourceArray.Count; i++)
            {
                if (objValue == alSourceArray[i])
                    return i;
            }
            return -1;
        }

         public int GetIndexByValueFromArray(double dblValue, double[] dblSourceArray)
        {
            if (dblSourceArray[dblSourceArray.Length - 1] > dblSourceArray[0])
            {
                if (dblValue < dblSourceArray[0])
                    return 0;
                if (dblValue > dblSourceArray[dblSourceArray.Length - 1])
                    return dblSourceArray.Length - 1;
            }
            else
            {
                if (dblValue > dblSourceArray[0])
                    return 0;
                if (dblValue < dblSourceArray[dblSourceArray.Length - 1])
                    return dblSourceArray.Length - 1;
            }

            for (int i = 1; i < dblSourceArray.Length; i++)
            {
                if (((dblSourceArray[i] - dblValue) * (dblSourceArray[i - 1] - dblValue)) <= 0)
                {
                    return i;
                }
            }

            return -1;
        }

         public int FindFirstIndex(double dblVal, double[] dblSource, int nStart, int nEnd)
        {
            int nTotalCount = Math.Abs(nStart - nEnd) + 1;
            if (nTotalCount == 1)
                return nStart;

            double[] dblSecArray = new double[nTotalCount];

            int nMinIndex = 0, nMaxIndex = 0;
            FindMinMaxIndex(dblSource, nStart, nEnd, ref nMinIndex, ref nMaxIndex);
            if ((dblVal < dblSource[nMinIndex]) || (dblVal > dblSource[nMaxIndex]))
                return nStart;

            int nDir = Math.Abs(nStart - nEnd) / (nStart - nEnd);
            int nCurIndex = nStart;
            for (int i = 0; i < nTotalCount; i++, nCurIndex -= nDir)
            {
                if (nCurIndex == 0)
                    continue;
                if (((dblSource[nCurIndex] - dblVal) * (dblSource[nCurIndex - 1] - dblVal)) <= 0)
                {
                    return nCurIndex;
                }
                //nCurIndex -= nDir;
            }
            return nStart;
        }

         public int GetIndexByValueFromArray(double dblValue, double[] dblSourceArray, int nStart, int nEnd)
        {
            //if (nStart > nEnd)
            //{
            //    nStart = nStart + nEnd;
            //    nEnd = nStart - nEnd;
            //    nStart = nStart - nEnd;
            //}

            int nTotalCount = Math.Abs(nStart - nEnd) + 1;
            double[] dblSecArray = new double[nTotalCount];

            if (nStart == nEnd)
                return nStart;

            int nDir = Math.Abs(nStart - nEnd) / (nStart - nEnd);
            int nCurIndex = nStart;
            for (int i = 0; i < nTotalCount; i++)
            {
                dblSecArray[i] = dblSourceArray[nCurIndex];
                nCurIndex -= nDir;
            }

            return (nStart - nDir * GetIndexByValueFromArray(dblValue, dblSecArray));
        }

         public void FindMinMaxIndex(double[] dblSourceArray, ref int nMinIndex, ref int nMaxIndex)
        {
            double dblTempMin = dblSourceArray[0];
            double dblTempMax = dblSourceArray[0];
            nMinIndex = 0;
            nMaxIndex = 0;

            for (int i = 0; i < dblSourceArray.Length; i++)
            {
                if (dblSourceArray[i] < dblTempMin)
                {
                    dblTempMin = dblSourceArray[i];
                    nMinIndex = i;
                }

                if (dblSourceArray[i] > dblTempMax)
                {
                    dblTempMax = dblSourceArray[i];
                    nMaxIndex = i;
                }
            }
        }

         public bool FindMinMaxIndex(double[] dblSourceArray, int nStart, int nEnd, ref int nMinIndex, ref int nMaxIndex)
        {
            if (nStart >= dblSourceArray.Length)
                return false;
            if (nEnd >= dblSourceArray.Length)
                return false;

            double[] dblSecArray = new double[Math.Abs(nEnd - nStart) + 1];
            if (nStart > nEnd)
            {
                nStart = nStart + nEnd;
                nEnd = nStart - nEnd;
                nStart = nStart - nEnd;
            }
            for (int i = nStart; i <= nEnd; i++)
            {
                dblSecArray[i - nStart] = dblSourceArray[i];
            }

            FindMinMaxIndex(dblSecArray, ref nMinIndex, ref nMaxIndex);

            nMinIndex += nStart;
            nMaxIndex += nStart;

            return true;
        }

         public void GetRangeIndexFromArray(double dblMin, double dblMax, double[] dblSourceArray, ArrayList nIndexArray)
        {
            nIndexArray.Clear();
            for (int i = 0; i < dblSourceArray.Length; i++)
            {
                if ((dblSourceArray[i] >= dblMin) && (dblSourceArray[i] <= dblMax))
                {
                    nIndexArray.Add(i);
                }
            }
        }
    }
    public struct inipara
    {

        public double dblmaxIL;
        public double dblGDripple;
        public double dblGDSlope;
        public double dblCD;
        public double ripple;
        public int[] position;
        public double []paracheckvalue;
        public int parasum;
        public string paraname;
        public double checkCD;
    }
    public struct mateinfo
    {
        public string scriptfileodd;
        public string scriptfileeven;
        public string[] coerpath;
        public string[] coerpathodd;
        public string[] coerpatheven;
        public string[] coerpathoddresult;
        public string[] coerpathevenresult;
        public string[] dcm1path;
        public string[] dcm2path;
        public string[] coersn;
        public string[] dcm1sn;
        public string[] dcm2sn;
        public int coersum;
        public int dcm1sum;
        public int dcm2sum;
        public int dcmuse;
       
    }
    enum ILChannelType
    {
        ChannelTypeEven,
        ChannelTypeOdd
    };
    enum ILItemIndex
    {
        ITU_Shift,
        MAX_IL,
        MIN_IL,
        Ripple,
        ISO_firstPassband,
        ISO_10GHzPassband,
        ISO_125GHzPassband,
        MAX_CD_firstPassband,
        MAX_CD_10GHzPassband,
        MAX_CD_125GHzPassband,
        Passband_dot5dBDown,
        Passband_1dBDown,
        Passband_22dBDown,
        Stopband,
    };

    class ILCalc
    {
        public int m_nChSpace;
        public double m_nChFirst;
        
        public ILChannelType checkChannelType()
        {
            int nFirstChTotalCount = (int)(m_nChSpace / (Math.Abs((double)Data.m_aldblFreq1[1] - (double)Data.m_aldblFreq1[0])));
            double dblMinValue = 1e9;
            int    nMinValueIndex = 0;
            for (int i = 0; i < nFirstChTotalCount; i++)
            {
                if (Math.Abs((double)Data.m_aldblIL[i]) < dblMinValue)
                {
                    dblMinValue = Math.Abs((double)Data.m_aldblIL[i]);
                    nMinValueIndex = i;
                }
            }

            double dblModRes = ((double)Data.m_aldblFreq1[nMinValueIndex] % m_nChSpace);

            if ((dblModRes > 25) && (dblModRes < 75))
                return ILChannelType.ChannelTypeOdd;
            else
                return ILChannelType.ChannelTypeEven;
        }

        private void CreateITUGrid(ILChannelType type, ArrayList alITUGrid)
        {
            ArrayList aldblITUChGrid = new ArrayList();
            double[] dblFreq = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            double dblTempVal = 0;
            dblTempVal = m_nChFirst + m_nChSpace;
        
                while (dblTempVal >= dblFreq[dblFreq.Length - 1])
                {          
                    alITUGrid.Add(dblTempVal);
                    dblTempVal -= m_nChSpace;
                }
                /*
            if (type == ILChannelType.ChannelTypeEven)
            {
                dblTempVal = dblFreq[0] - dblFreq[0] % m_nChSpace;
                while (dblTempVal >= dblFreq[dblFreq.Length - 1])
                {
                    if (m_nChSpace == 75)
                    {
                        alITUGrid.Add(dblTempVal + 25);
                    }
                    else
                    {
                        alITUGrid.Add(dblTempVal);
                    }
                    
                    dblTempVal -= m_nChSpace;
                }
            }
            else
            {

                if ((dblFreq[0] % m_nChSpace) >= (m_nChSpace / 2))
                {
                    if (m_nChSpace == 75)
                    {
                        dblTempVal = dblFreq[0] - dblFreq[0] % (m_nChSpace / 2) + 28.5;
                    }
                    else
                    {
                        dblTempVal = dblFreq[0] - dblFreq[0] % (m_nChSpace / 2);
                    }
                    //上面的算法感觉有问题，直接用外部传入的起始频率计算
                    dblTempVal = m_nChFirst + m_nChSpace;
                }
                else
                {
                    if (m_nChSpace == 75)
                    {
                        dblTempVal = dblFreq[0] - dblFreq[0] % m_nChSpace - (m_nChSpace / 2) + 28.5;
                    }
                    else
                    {
                        dblTempVal = dblFreq[0] - dblFreq[0] % m_nChSpace - (m_nChSpace / 2);
                    }
                    //上面的算法感觉有问题，直接用外部传入的起始频率计算
                    dblTempVal = m_nChFirst + m_nChSpace;
                }
                while (dblTempVal >= dblFreq[dblFreq.Length - 1])
                {          
                    alITUGrid.Add(dblTempVal);
                    dblTempVal -= m_nChSpace;
                }
            }*/
        }

        private void CreateIndexGrid(ArrayList alBaseCenterGrid, double dblPassBand, ArrayList alIndexGrid,bool bCD=false)
        {
            double[] dblFreq;
            if (bCD)
            {
                dblFreq = (double[])Data.m_aldblFreq2.ToArray(typeof(double));
            }
            else
            {
                 dblFreq = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            }
            //double[] dblFreq = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            for (int i = 0; i < alBaseCenterGrid.Count; i++)
            {
                // for ITU self channel
                double dblCurFreq = (double)alBaseCenterGrid[i] - dblPassBand;
                int nTempIndex =  Data.GetIndexByValueFromArray(dblCurFreq, dblFreq,false);
                alIndexGrid.Add(nTempIndex);

                dblCurFreq = (double)alBaseCenterGrid[i];
                nTempIndex = Data.GetIndexByValueFromArray(dblCurFreq, dblFreq);
                alIndexGrid.Add(nTempIndex);

                dblCurFreq = (double)alBaseCenterGrid[i] + dblPassBand;
                nTempIndex = Data.GetIndexByValueFromArray(dblCurFreq, dblFreq);
                alIndexGrid.Add(nTempIndex);

                // last 50GHz channel, for ISO
                dblCurFreq = (double)alBaseCenterGrid[i] - dblPassBand - (m_nChSpace / 2);
                nTempIndex =  Data.GetIndexByValueFromArray(dblCurFreq, dblFreq);
                alIndexGrid.Add(nTempIndex);

                dblCurFreq = (double)alBaseCenterGrid[i] - (m_nChSpace / 2);
                nTempIndex = Data.GetIndexByValueFromArray(dblCurFreq, dblFreq);
                alIndexGrid.Add(nTempIndex);

                dblCurFreq = (double)alBaseCenterGrid[i] + dblPassBand - (m_nChSpace / 2);
                nTempIndex = Data.GetIndexByValueFromArray(dblCurFreq, dblFreq);
                alIndexGrid.Add(nTempIndex);

                // next 50GHz channel, for ISO
                dblCurFreq = (double)alBaseCenterGrid[i] - dblPassBand + (m_nChSpace / 2);
                nTempIndex =  Data.GetIndexByValueFromArray(dblCurFreq, dblFreq);
                alIndexGrid.Add(nTempIndex);

                dblCurFreq = (double)alBaseCenterGrid[i] + (m_nChSpace / 2);
                nTempIndex = Data.GetIndexByValueFromArray(dblCurFreq, dblFreq);
                alIndexGrid.Add(nTempIndex);

                dblCurFreq = (double)alBaseCenterGrid[i] + dblPassBand + (m_nChSpace / 2);
                nTempIndex = Data.GetIndexByValueFromArray(dblCurFreq, dblFreq);
                alIndexGrid.Add(nTempIndex);
            }
        }

        public void CalcITUParas(double dblDepth, double dblPassband, ArrayList alRes)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid);

            ArrayList alRes1 = new ArrayList();
            CalcParasPart1(dblDepth, alIndexGrid, alRes1);

            ArrayList alRes2 = new ArrayList();
            CalcParasPart2(dblPassband, alITUGrid, alRes2);

            ArrayList alRes3 = new ArrayList();
            CalcParasPart3(thisType, dblDepth, alIndexGrid, alRes3);

            ArrayList alTemp = (ArrayList)alRes1[0];
            alRes.Add(alTemp.ToArray(typeof(double)));

            double[] dblRealCenter = (double[])alTemp.ToArray(typeof(double));
            double[] dblITUCenter = (double[])alITUGrid.ToArray(typeof(double));
            alTemp.Clear();
            for (int i = 0; i < dblRealCenter.Length; i++)
            {
                double dblTempVal = Math.Abs(dblRealCenter[i] - dblITUCenter[i]);
                alTemp.Add(dblTempVal);
            }
            alRes.Add(alTemp.ToArray(typeof(double)));
            for (int i = 1; i < alRes1.Count; i++)
                alRes.Add(((ArrayList)alRes1[i]).ToArray(typeof(double)));

            for (int i = 0; i < alRes2.Count; i++)
                alRes.Add(((ArrayList)alRes2[i]).ToArray(typeof(double)));

            for (int i = 0; i < alRes3.Count; i++)
                alRes.Add(((ArrayList)alRes3[i]).ToArray(typeof(double)));
        }

        public void CalcRealParas(double dblDepth, double dblPassband, ArrayList alRes)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid);

            ArrayList alRes1 = new ArrayList();
            CalcParasPart1(dblDepth, alIndexGrid, alRes1);

            ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

            alIndexGrid.Clear();
            CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid);

            alRes1.Clear();
            CalcParasPart1(dblDepth, alIndexGrid, alRes1);

            ArrayList alRes2 = new ArrayList();
            CalcParasPart2(dblPassband, alITUGrid, alRes2);

            ArrayList alRes3 = new ArrayList();
            CalcParasPart3(thisType, dblDepth, alIndexGrid, alRes3);

            ArrayList alTemp = (ArrayList)alRes1[0];
            alRes.Add(alTemp.ToArray(typeof(double)));

            double[] dblRealCenter = (double[])alTemp.ToArray(typeof(double));
            double[] dblITUCenter = (double[])alITUGrid.ToArray(typeof(double));
            alTemp.Clear();
            for (int i = 0; i < dblRealCenter.Length; i++)
            {
                double dblTempVal = Math.Abs(dblRealCenter[i] - dblITUCenter[i]);
                alTemp.Add(dblTempVal);
            }
            alRes.Add(alTemp.ToArray(typeof(double)));
            for (int i = 1; i < alRes1.Count; i++)
                alRes.Add(((ArrayList)alRes1[i]).ToArray(typeof(double)));

            for (int i = 0; i < alRes2.Count; i++)
                alRes.Add(((ArrayList)alRes2[i]).ToArray(typeof(double)));

            for (int i = 0; i < alRes3.Count; i++)
                alRes.Add(((ArrayList)alRes3[i]).ToArray(typeof(double)));
        }

        //public void CalcRealParasByPassband(double dblDepth, double dblPassband, ArrayList alRes)
        //{
        //    ILChannelType thisType = checkChannelType();
        //    ArrayList alITUGrid = new ArrayList();
        //    CreateITUGrid(thisType, alITUGrid);

        //    // 1st loop: to get xdB down center from ITU grid
        //    ArrayList alIndexGrid1 = new ArrayList();
        //    CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid1);

        //    ArrayList alRes1 = new ArrayList();
        //    CalcParas(dblDepth, dblPassband, alIndexGrid, alRes1);
        //    // 2nd loop: use xdB down center to calc all parameters
        //    ArrayList alIndexGrid2 = new ArrayList();
        //    ArrayList alBaseGrid = (ArrayList)alRes1[10];

        //}

        /*private void CalcParas(double dblDepth, double dblPassband, ArrayList alIndexArray, ArrayList alRes)
        {
            double[] dblIL = (double[])Data.m_aldblIL.ToArray(typeof(double));
            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            double[] dblCD = (double[])Data.m_aldblCD.ToArray(typeof(double));
            double[] dblFreq2 = (double[])Data.m_aldblFreq2.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexGrid.ToArray(typeof(int));

            ArrayList alOffset = new ArrayList();
            ArrayList alMinIL = new ArrayList();
            ArrayList alMaxIL = new ArrayList();
            ArrayList alRipple = new ArrayList();
            ArrayList alIso1 = new ArrayList();
            ArrayList alIso2 = new ArrayList();
            ArrayList alIso3 = new ArrayList();
            ArrayList alMaxCD1 = new ArrayList();
            ArrayList alMaxCD2 = new ArrayList();
            ArrayList alMaxCD3 = new ArrayList();

            ArrayList alLeftBoundary = new ArrayList();
            ArrayList alRightBoundary = new ArrayList();

            int nMultiCount = 9;
            int nTotalChCount = alIndexArray.Count / nMultiCount;
            for (int i = 0; i < nTotalChCount; i++)
            {
                int nMinIndex = 0, nMaxIndex = 0;

                if (Data.FindMinMaxIndex(dblIL, nIndexListArray[i * nMultiCount],
                    nIndexListArray[i * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                double dblMinIL = dblIL[nMinIndex];
                double dblMaxIL = dblIL[nMaxIndex];
                alMinIL.Add(dblMinIL);
                alMaxIL.Add(dblMaxIL);
                alRipple.Add(Math.Abs(dblMinIL - dblMaxIL));

                double dblTempILVal = dblIL[nMaxIndex] - dblDepth;

                int nLeftBoundry = Data.GetIndexByValueFromArray(
                                    dblTempILVal,
                                    dblIL,
                                    nIndexListArray[i * nMultiCount + 4],
                                    nIndexListArray[i * nMultiCount + 1]);
                alLeftBoundary.Add(nLeftBoundry);

                int nRightBoundry = Data.GetIndexByValueFromArray(
                                    dblTempILVal,
                                    dblIL,
                                    nIndexListArray[i * nMultiCount + 1],
                                    nIndexListArray[i * nMultiCount + 7]);
                alRightBoundary.Add(nRightBoundry);

                double dblRealCenter = 0.5 * Math.Abs(dblFreq1[nLeftBoundry] + dblFreq1[nRightBoundry]);
                alCenter.Add(dblRealCenter);

                if (Data.FindMinMaxIndex(dblIL, nIndexListArray[i * nMultiCount + 3],
                    nIndexListArray[i * nMultiCount + 5], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                double dblLeftIso = dblMinIL - dblIL[nMaxIndex];

                if (Data.FindMinMaxIndex(dblIL, nIndexListArray[i * nMultiCount + 6],
                    nIndexListArray[i * nMultiCount + 8], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                double dblRightIso = dblMinIL - dblIL[nMaxIndex];

                double dblIso = (dblLeftIso < dblRightIso) ? dblLeftIso : dblRightIso;
                alIso.Add(dblIso);

                ArrayList alIndexArray = new ArrayList();
                double dblStartFreq = dblFreq1[nIndexListArray[i * nMultiCount]];
                double dblEndFreq = dblFreq1[nIndexListArray[i * nMultiCount + 2]];

                Data.GetRangeIndexFromArray(dblStartFreq, dblEndFreq, dblFreq2, alIndexArray);
                if (alIndexArray.Count == 0)
                {
                    throw new Exception("定义通带过小，无法查找相应CD数值点!");
                }
                else
                {
                    Data.FindMinMaxIndex(dblCD, (int)alIndexArray[0], (int)alIndexArray[alIndexArray.Count - 1], ref nMinIndex, ref nMaxIndex);
                    if ((dblCD[nMinIndex] + dblCD[nMaxIndex]) < 0)
                        alMaxCD.Add(dblCD[nMinIndex]);
                    else
                        alMaxCD.Add(dblCD[nMaxIndex]);
                }
            }

            alRes.Add(alCenter);
            alRes.Add(alMaxIL);
            alRes.Add(alMinIL);
            alRes.Add(alRipple);
            alRes.Add(alIso);
            alRes.Add(alMaxCD);
            alRes.Add(alLeftBoundary);
            alRes.Add(alRightBoundary);
        }*/

        // this part is for real center by xdB depth
        // min IL
        // max IL
        // ripple
        private void CalcParasPart1(double dblDepth, ArrayList alIndexArray, ArrayList alRes)
        {
            double[] dblIL = (double[])Data.m_aldblIL.ToArray(typeof(double));
            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexArray.ToArray(typeof(int));

            ArrayList alMinIL = new ArrayList();
            ArrayList alMaxIL = new ArrayList();
            ArrayList alRipple = new ArrayList();
            ArrayList alCenter = new ArrayList();

            int nMultiCount = 9;
            int nTotalChCount = alIndexArray.Count / nMultiCount;
            for (int i = 0; i < nTotalChCount; i++)
            {
                int nMinIndex = 0, nMaxIndex = 0;

                if (Data.FindMinMaxIndex(dblIL, nIndexListArray[i * nMultiCount],
                    nIndexListArray[i * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                double dblMinIL = dblIL[nMinIndex];
                double dblMaxIL = dblIL[nMaxIndex];
                alMinIL.Add(dblMinIL);
                alMaxIL.Add(dblMaxIL);
                alRipple.Add(Math.Abs(dblMinIL - dblMaxIL));

                double dblTempILVal = dblIL[nMaxIndex] - dblDepth;

                int nLeftBoundry = Data.GetIndexByValueFromArray(
                                    dblTempILVal,
                                    dblIL,
                                    nIndexListArray[i * nMultiCount + 4],
                                    nIndexListArray[i * nMultiCount + 1]);

                int nRightBoundry = Data.GetIndexByValueFromArray(
                                    dblTempILVal,
                                    dblIL,
                                    nIndexListArray[i * nMultiCount + 1],
                                    nIndexListArray[i * nMultiCount + 7]);

                double dblRealCenter = 0.5 * Math.Abs(dblFreq1[nLeftBoundry] + dblFreq1[nRightBoundry]);
                alCenter.Add(dblRealCenter);
            }

            alRes.Add(alCenter);
            alRes.Add(alMaxIL);
            alRes.Add(alMinIL);
            alRes.Add(alRipple);
        }

        // this part is for adj-isolation & max cd
        // 1st is value by input parameter
        // 2nd is +/-8GHz
        // 3rd is +/-12.5GHz
        private void CalcParasPart2(double dblPassband, ArrayList alBaseGrid, ArrayList alRes)
        {
            double[] dblIL = (double[])Data.m_aldblIL.ToArray(typeof(double));
            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            double[] dblCD = (double[])Data.m_aldblCD.ToArray(typeof(double));
            double[] dblFreq2 = (double[])Data.m_aldblFreq2.ToArray(typeof(double));

            ArrayList alIso1 = new ArrayList();
            ArrayList alIso2 = new ArrayList();
            ArrayList alIso3 = new ArrayList();
            ArrayList alMaxCD1 = new ArrayList();
            ArrayList alMaxCD2 = new ArrayList();
            ArrayList alMaxCD3 = new ArrayList();

            for (int i = 0; i < 3; i++)
            {
                double dblCurPassband = dblPassband;
                ArrayList alCurIso = alIso1;
                ArrayList alCurMaxCD = alMaxCD1;
                if (i == 1)
                {
                    dblCurPassband = 10;
                    alCurIso = alIso2;
                    alCurMaxCD = alMaxCD2;
                }
                else if (i == 2)
                {
                    dblCurPassband = 12.5;
                    alCurIso = alIso3;
                    alCurMaxCD = alMaxCD3;
                }

                ArrayList alIndexGrid = new ArrayList();
                CreateIndexGrid(alBaseGrid, dblCurPassband, alIndexGrid);
                int[] nIndexListArray = (int[])alIndexGrid.ToArray(typeof(int));

                int nMultiCount = 9;
                for (int j = 0; j < alBaseGrid.Count; j++)
                {
                    int nMinIndex = 0, nMaxIndex = 0;

                    if (Data.FindMinMaxIndex(dblIL, nIndexListArray[j * nMultiCount],
                        nIndexListArray[j * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                    {
                        throw new Exception("数据异常!");
                    }
                    double dblMinIL = dblIL[nMinIndex];
                    double dblMaxIL = dblIL[nMaxIndex];

                    if (Data.FindMinMaxIndex(dblIL, nIndexListArray[j * nMultiCount + 3],
                        nIndexListArray[j * nMultiCount + 5], ref nMinIndex, ref nMaxIndex) == false)
                    {
                        throw new Exception("数据异常!");
                    }
                    double dblLeftIso = dblMinIL - dblIL[nMaxIndex];

                    if (Data.FindMinMaxIndex(dblIL, nIndexListArray[j * nMultiCount + 6],
                        nIndexListArray[j * nMultiCount + 8], ref nMinIndex, ref nMaxIndex) == false)
                    {
                        throw new Exception("数据异常!");
                    }
                    double dblRightIso = dblMinIL - dblIL[nMaxIndex];

                    double dblIso = (dblLeftIso < dblRightIso) ? dblLeftIso : dblRightIso;
                    alCurIso.Add(dblIso);

                    ArrayList alIndexArray = new ArrayList();
                    double dblStartFreq = dblFreq1[nIndexListArray[j * nMultiCount]];
                    double dblEndFreq = dblFreq1[nIndexListArray[j * nMultiCount + 2]];

                    Data.GetRangeIndexFromArray(dblStartFreq, dblEndFreq, dblFreq2, alIndexArray);
                    if (alIndexArray.Count == 0)
                    {
                        throw new Exception("定义通带过小，无法查找相应CD数值点!");
                    }
                    else
                    {
                        Data.FindMinMaxIndex(dblCD, (int)alIndexArray[0], (int)alIndexArray[alIndexArray.Count - 1], ref nMinIndex, ref nMaxIndex);
                        if ((dblCD[nMinIndex] + dblCD[nMaxIndex]) < 0)
                            alCurMaxCD.Add(dblCD[nMinIndex]);
                        else
                            alCurMaxCD.Add(dblCD[nMaxIndex]);
                    }
                }
            }

            alRes.Add(alIso1);
            alRes.Add(alIso2);
            alRes.Add(alIso3);
            alRes.Add(alMaxCD1);
            alRes.Add(alMaxCD2);
            alRes.Add(alMaxCD3);
        }

        // this part is for pass band & stopband
        private void CalcParasPart3(ILChannelType type, double dblDepth, ArrayList alIndexArray, ArrayList alRes)
        {
            double[] dblIL = (double[])Data.m_aldblIL.ToArray(typeof(double));
            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexArray.ToArray(typeof(int));

            ArrayList alPb1 = new ArrayList();
            ArrayList alPb2 = new ArrayList();
            ArrayList alPb3 = new ArrayList();
            ArrayList alStopband = new ArrayList();

            ArrayList alLeftBoundary = new ArrayList();
            ArrayList alRightBoundary = new ArrayList();

            int nMultiCount = 9;
            int nTotalChCount = nIndexListArray.Length / nMultiCount;

            int nLastIndex = dblFreq1.Length - 1;
            for (int i = 0; i < 3; i++)
            {
                double dblCurDepth = dblDepth;
                ArrayList alCurPb = alPb1;
                if (i == 1)
                {
                    dblCurDepth = 1;
                    alCurPb = alPb2;
                }
                else if (i == 2)
                {
                    dblCurDepth = 22;
                    alCurPb = alPb3;
                }

                for (int j = 0; j < nTotalChCount; j++)
                {
                    int nMinIndex = 0, nMaxIndex = 0;

                    if (Data.FindMinMaxIndex(dblIL, nIndexListArray[j * nMultiCount],
                        nIndexListArray[j * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                    {
                        throw new Exception("数据异常!");
                    }
                    double dblMinIL = dblIL[nMinIndex];
                    double dblMaxIL = dblIL[nMaxIndex];

                    double dblTempILVal = dblIL[nMaxIndex] - dblCurDepth;

                    int nLeftBoundry = Data.GetIndexByValueFromArray(
                                        dblTempILVal,
                                        dblIL,
                                        nIndexListArray[j * nMultiCount + 4],
                                        nIndexListArray[j * nMultiCount + 1]);
                    double dblLeftband = Math.Abs(dblFreq1[nLeftBoundry] - dblFreq1[nIndexListArray[j * nMultiCount + 1]]);
                    if (i == 2)
                    {
                        int nAnotherFitPoint = 0;
                        if (nLeftBoundry == 0)
                            nAnotherFitPoint = 1;
                        else if (nLeftBoundry == nLastIndex)
                            nAnotherFitPoint = nLastIndex - 1;
                        else
                        {
                            nAnotherFitPoint = nLeftBoundry - 1;
                            if (((dblIL[nAnotherFitPoint] - dblTempILVal) * (dblIL[nLeftBoundry] - dblTempILVal)) > 0)
                                nAnotherFitPoint = nLeftBoundry + 1;
                        }
                        dblLeftband = LeastSquareFit.LinearFitByTwoPoint(dblIL[nAnotherFitPoint],
                                                                dblIL[nLeftBoundry],
                                                                dblFreq1[nAnotherFitPoint],
                                                                dblFreq1[nLeftBoundry],
                                                                dblTempILVal) - dblFreq1[nIndexListArray[j * nMultiCount + 1]];
                        dblLeftband = Math.Abs(dblLeftband);
                    }

                    int nRightBoundry = Data.GetIndexByValueFromArray(
                                        dblTempILVal,
                                        dblIL,
                                        nIndexListArray[j * nMultiCount + 1],
                                        nIndexListArray[j * nMultiCount + 7]);
                    double dblRightband = Math.Abs(dblFreq1[nRightBoundry] - dblFreq1[nIndexListArray[j * nMultiCount + 1]]);
                    if (i == 2)
                    {
                        int nAnotherFitPoint = 0;
                        if (nRightBoundry == 0)
                            nAnotherFitPoint = 1;
                        else if (nLeftBoundry == nLastIndex)
                            nAnotherFitPoint = nLastIndex - 1;
                        else
                        {
                            nAnotherFitPoint = nRightBoundry - 1;
                            if (((dblIL[nAnotherFitPoint] - dblTempILVal) * (dblIL[nLeftBoundry] - dblTempILVal)) > 0)
                                nAnotherFitPoint = nRightBoundry + 1;
                        }
                        dblRightband = LeastSquareFit.LinearFitByTwoPoint(dblIL[nAnotherFitPoint],
                                                                dblIL[nRightBoundry],
                                                                dblFreq1[nAnotherFitPoint],
                                                                dblFreq1[nRightBoundry],
                                                                dblTempILVal) - dblFreq1[nIndexListArray[j * nMultiCount + 1]];

                         dblRightband = Math.Abs(dblRightband);
                        
                    }
                    double dblCleanband = 2 * ((dblLeftband < dblRightband) ? dblLeftband : dblRightband);
                    alCurPb.Add(dblCleanband);
                }
            }

            // calc 22dB down stopband
            for (int i = 0; i < nTotalChCount; i++)
            {
                int nMinIndex = 0, nMaxIndex = 0;

                if (Data.FindMinMaxIndex(dblIL, nIndexListArray[i * nMultiCount],
                        nIndexListArray[i * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                double dblMinIL = dblIL[nMinIndex];
                double dblMaxIL = dblIL[nMaxIndex];

                double dblTempILVal = dblIL[nMaxIndex] - 22;

                int nPoint1 = 0, nPoint2 = 0;
                if (type == ILChannelType.ChannelTypeEven)
                {
                    int nKick0 = 0;
                    if (i > 0)
                        nKick0 = nIndexListArray[(i - 1) * nMultiCount + 1];
                    int nKick1 = nIndexListArray[i * nMultiCount + 1];
                    
                    nPoint1 = Data.FindFirstIndex(
                                        dblTempILVal,
                                        dblIL,
                                        nKick0,
                                        nKick1);

                    nPoint2 = Data.FindFirstIndex(
                                        dblTempILVal,
                                        dblIL,
                                        nKick1,
                                        nKick0);
                }
                else if (type == ILChannelType.ChannelTypeOdd)
                {
                    int nKick0 = nIndexListArray[i * nMultiCount + 1];
                    int nKick1 = dblIL.Length - 1;
                    if (i < nTotalChCount-1)
                        nKick1 = nIndexListArray[(i + 1) * nMultiCount + 1];

                    nPoint1 = Data.FindFirstIndex(
                                        dblTempILVal,
                                        dblIL,
                                        nKick0,
                                        nKick1);

                    nPoint2 = Data.FindFirstIndex(
                                        dblTempILVal,
                                        dblIL,
                                        nKick1,
                                        nKick0);
                }

                double dblBand1 = 0, dblBand2 = 0;
                int nAnotherFitPoint = 0;
                if (nPoint1 == 0)
                    nAnotherFitPoint = 1;
                else if (nPoint1 == nLastIndex)
                    nAnotherFitPoint = nLastIndex - 1;
                else
                {
                    nAnotherFitPoint = nPoint1 - 1;
                    if (((dblIL[nAnotherFitPoint] - dblTempILVal) * (dblIL[nPoint1] - dblTempILVal)) > 0)
                        nAnotherFitPoint = nPoint1 + 1;
                }
                dblBand1 = LeastSquareFit.LinearFitByTwoPoint(dblIL[nAnotherFitPoint],
                                                        dblIL[nPoint1],
                                                        dblFreq1[nAnotherFitPoint],
                                                        dblFreq1[nPoint1],
                                                        dblTempILVal);

                nAnotherFitPoint = 0;
                if (nPoint2 == 0)
                    nAnotherFitPoint = 1;
                else if (nPoint2 == nLastIndex)
                    nAnotherFitPoint = nLastIndex - 1;
                else
                {
                    nAnotherFitPoint = nPoint2 - 1;
                    if (((dblIL[nAnotherFitPoint] - dblTempILVal) * (dblIL[nPoint2] - dblTempILVal)) > 0)
                        nAnotherFitPoint = nPoint2 + 1;
                }
                dblBand2 = LeastSquareFit.LinearFitByTwoPoint(dblIL[nAnotherFitPoint],
                                                        dblIL[nPoint2],
                                                        dblFreq1[nAnotherFitPoint],
                                                        dblFreq1[nPoint2],
                                                        dblTempILVal);

                double dblStopband = Math.Abs(dblBand1 - dblBand2);
                alStopband.Add(dblStopband);
            }

            alRes.Add(alPb1);
            alRes.Add(alPb2);
            alRes.Add(alPb3);
            alRes.Add(alStopband);
        }

        private void CalcEdge(int nFitOrder, double dblDepth, ArrayList alIndexArray, ArrayList alLeft, ArrayList alRight)
        {
            double[] dblIL = (double[])Data.m_aldblIL.ToArray(typeof(double));
            for (int i = 0; i < dblIL.Length; i++)
            {
                dblIL[i] = dblIL[i] * -1;
            }
            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexArray.ToArray(typeof(int));

            ArrayList alPb = new ArrayList();

            ArrayList alLeftBoundary = new ArrayList();
            ArrayList alRightBoundary = new ArrayList();

            int nMultiCount = 9;
            int nTotalChCount = nIndexListArray.Length / nMultiCount;

            int nLastIndex = dblFreq1.Length - 1;

            for (int j = 0; j < nTotalChCount; j++)
            {
                int nMinIndex = 0, nMaxIndex = 0;

                if (Data.FindMinMaxIndex(dblIL, nIndexListArray[j * nMultiCount],
                    nIndexListArray[j * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                double dblMaxIL = dblIL[nMaxIndex];

                double dblTempILVal = dblMaxIL + dblDepth;

                int nLeftBoundryIndex = Data.GetIndexByValueFromArray(
                                    dblTempILVal,
                                    dblIL,
                                    nIndexListArray[j * nMultiCount + 4],
                                    nIndexListArray[j * nMultiCount + 1]);
                double dblLeftBoundry = dblFreq1[nLeftBoundryIndex];
  
                int nRightBoundryIndex = Data.GetIndexByValueFromArray(
                                    dblTempILVal,
                                    dblIL,
                                    nIndexListArray[j * nMultiCount + 1],
                                    nIndexListArray[j * nMultiCount + 7]);
                double dblRightBoundry = dblFreq1[nRightBoundryIndex];
             
                if (nFitOrder >= 1)
                {
                    int nAnotherFitPoint = 0;
                    if (nLeftBoundryIndex == 0)
                        nAnotherFitPoint = 1;
                    else if (nLeftBoundryIndex == nLastIndex)
                        nAnotherFitPoint = nLastIndex - 1;
                    else
                    {
                        nAnotherFitPoint = nLeftBoundryIndex - 1;
                        if (((dblIL[nAnotherFitPoint] - dblTempILVal) * (dblIL[nLeftBoundryIndex] - dblTempILVal)) > 0)
                            nAnotherFitPoint = nLeftBoundryIndex + 1;
                    }
                    dblLeftBoundry = LeastSquareFit.LinearFitByTwoPoint(dblIL[nAnotherFitPoint],
                                                            dblIL[nLeftBoundryIndex],
                                                            dblFreq1[nAnotherFitPoint],
                                                            dblFreq1[nLeftBoundryIndex],
                                                            dblTempILVal);

                    nAnotherFitPoint = 0;
                    if (nRightBoundryIndex == 0)
                        nAnotherFitPoint = 1;
                    else if (nRightBoundryIndex == nLastIndex)
                        nAnotherFitPoint = nLastIndex - 1;
                    else
                    {
                        nAnotherFitPoint = nRightBoundryIndex - 1;
                        if (((dblIL[nAnotherFitPoint] - dblTempILVal) * (dblIL[nRightBoundryIndex] - dblTempILVal)) > 0)
                            nAnotherFitPoint = nRightBoundryIndex + 1;
                    }
                    dblRightBoundry = LeastSquareFit.LinearFitByTwoPoint(dblIL[nAnotherFitPoint],
                                                            dblIL[nRightBoundryIndex],
                                                            dblFreq1[nAnotherFitPoint],
                                                            dblFreq1[nRightBoundryIndex],
                                                            dblTempILVal);
                }

                alLeft.Add(dblLeftBoundry);
                alRight.Add(dblRightBoundry);
            }
        }
        public void CalcCCF(double dblPassband, double dblDepth, int nFitOrder, out double[] dblCCF)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid);

            ArrayList alLeft = new ArrayList();
            ArrayList alRight = new ArrayList();
            CalcEdge(nFitOrder, dblDepth, alIndexGrid, alLeft, alRight);

            dblCCF = new double[alLeft.Count];
            for (int i = 0; i < dblCCF.Length; i++)
            {
                dblCCF[i] = ((double)alLeft[i] + (double)alRight[i]) / 2;
                
            }
        }

        public void CalcITUCF(out double[] dblITUCF)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            dblITUCF = new double[alITUGrid.Count];
            for (int i = 0; i < dblITUCF.Length; i++)
            {
                dblITUCF[i] = (double)alITUGrid[i];
            }
        }

        public void CalcMaxIL(double dblPassband, double dblCFPb, double dblCFDep, out double[] dblMaxIL)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid);

            if ((dblCFDep > 0) && (dblCFPb > 0))
            {
                ArrayList alRes1 = new ArrayList();
                CalcParasPart1(dblCFDep, alIndexGrid, alRes1);

                ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

                alIndexGrid.Clear();
                CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid);
            }

            double[] dblIL = (double[])Data.m_aldblIL.ToArray(typeof(double));
            for (int i = 0; i < dblIL.Length; i++)
            {
                dblIL[i] = dblIL[i] * -1;
            }
            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexGrid.ToArray(typeof(int));

            ArrayList alPb = new ArrayList();

            ArrayList alLeftBoundary = new ArrayList();
            ArrayList alRightBoundary = new ArrayList();

            int nMultiCount = 9;
            int nTotalChCount = nIndexListArray.Length / nMultiCount;

            dblMaxIL = new double[nTotalChCount];
            for (int i = 0; i < nTotalChCount; i++)
            {
                int nMinIndex = 0, nMaxIndex = 0;

                if (Data.FindMinMaxIndex(dblIL, nIndexListArray[i * nMultiCount],
                    nIndexListArray[i * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                dblMaxIL[i] = dblIL[nMaxIndex];
            }
        }

        public void CalcMaxPMD(double dblPassband, double dblCFPb, double dblCFDep, out double[] dblMaxPMD)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid);

            if ((dblCFDep > 0) && (dblCFPb > 0))
            {
                ArrayList alRes1 = new ArrayList();
                CalcParasPart1(dblCFDep, alIndexGrid, alRes1);

                ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

                alIndexGrid.Clear();
                CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid);
            }

            double[] dblPMD = (double[])Data.m_aldblPMD.ToArray(typeof(double));
            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexGrid.ToArray(typeof(int));

            ArrayList alPb = new ArrayList();

            ArrayList alLeftBoundary = new ArrayList();
            ArrayList alRightBoundary = new ArrayList();

            int nMultiCount = 9;
            int nTotalChCount = nIndexListArray.Length / nMultiCount;

            dblMaxPMD = new double[nTotalChCount];
            for (int i = 0; i < nTotalChCount; i++)
            {
                int nMinIndex = 0, nMaxIndex = 0;

                if (Data.FindMinMaxIndex(dblPMD, nIndexListArray[i * nMultiCount],
                    nIndexListArray[i * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                dblMaxPMD[i] = dblPMD[nMaxIndex];
            }
        }

        public void CalcMinIL(double dblPassband, double dblCFPb, double dblCFDep, out double[] dblMinIL)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid);

            if ((dblCFDep > 0) && (dblCFPb > 0))
            {
                ArrayList alRes1 = new ArrayList();
                CalcParasPart1(dblCFDep, alIndexGrid, alRes1);

                ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

                alIndexGrid.Clear();
                CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid);
            }

            double[] dblIL = (double[])Data.m_aldblIL.ToArray(typeof(double));
            for (int i = 0; i < dblIL.Length; i++)
            {
                dblIL[i] = dblIL[i] * -1;
            }
            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexGrid.ToArray(typeof(int));

            ArrayList alPb = new ArrayList();

            ArrayList alLeftBoundary = new ArrayList();
            ArrayList alRightBoundary = new ArrayList();

            int nMultiCount = 9;
            int nTotalChCount = nIndexListArray.Length / nMultiCount;

            dblMinIL = new double[nTotalChCount];
            for (int i = 0; i < nTotalChCount; i++)
            {
                int nMinIndex = 0, nMaxIndex = 0;

                if (Data.FindMinMaxIndex(dblIL, nIndexListArray[i * nMultiCount],
                    nIndexListArray[i * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                dblMinIL[i] = dblIL[nMinIndex];
            }
        }

        public void CalcPDL(double dblPassband, double dblCFPb, double dblCFDep, out double[] dblMaxPDL)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblCFPb, alIndexGrid);

            if ((dblCFDep > 0) && (dblCFPb > 0))
            {
                ArrayList alRes1 = new ArrayList();
                CalcParasPart1(dblCFDep, alIndexGrid, alRes1);

                ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

                alIndexGrid.Clear();
                CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid);
            }

            double[] dblPDL = (double[])Data.m_aldblPDL.ToArray(typeof(double));
            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexGrid.ToArray(typeof(int));

            ArrayList alPb = new ArrayList();

            ArrayList alLeftBoundary = new ArrayList();
            ArrayList alRightBoundary = new ArrayList();

            int nMultiCount = 9;
            int nTotalChCount = nIndexListArray.Length / nMultiCount;

            dblMaxPDL = new double[nTotalChCount];
            for (int i = 0; i < nTotalChCount; i++)
            {
                int nMinIndex = 0, nMaxIndex = 0;

                if (Data.FindMinMaxIndex(dblPDL, nIndexListArray[i * nMultiCount],
                    nIndexListArray[i * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                dblMaxPDL[i] = dblPDL[nMaxIndex];
            }
        }
        //public void CalcDGD(double dblPassband, double dblCFPb, double dblCFDep, out double[] dblMaxDGD)
        //{
        //    ILChannelType thisType = checkChannelType();
        //    ArrayList alITUGrid = new ArrayList();
        //    CreateITUGrid(thisType, alITUGrid);

        //    ArrayList alIndexGrid = new ArrayList();
        //    CreateIndexGrid(alITUGrid, dblCFPb, alIndexGrid);

        //    if ((dblCFDep > 0) && (dblCFPb > 0))
        //    {
        //        ArrayList alRes1 = new ArrayList();
        //        CalcParasPart1(dblCFDep, alIndexGrid, alRes1);

        //        ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

        //        alIndexGrid.Clear();
        //        CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid);
        //    }

        //    double[] dblDGD = (double[])Data.m_aldblDGD.ToArray(typeof(double));
        //    double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
        //    int[] nIndexListArray = (int[])alIndexGrid.ToArray(typeof(int));

        //    ArrayList alPb = new ArrayList();

        //    ArrayList alLeftBoundary = new ArrayList();
        //    ArrayList alRightBoundary = new ArrayList();

        //    int nMultiCount = 9;
        //    int nTotalChCount = nIndexListArray.Length / nMultiCount;

        //    dblMaxDGD = new double[nTotalChCount];
        //    for (int i = 0; i < nTotalChCount; i++)
        //    {
        //        int nMinIndex = 0, nMaxIndex = 0;

        //        if (Data.FindMinMaxIndex(dblDGD, nIndexListArray[i * nMultiCount],
        //            nIndexListArray[i * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
        //        {
        //            throw new Exception("数据异常!");
        //        }
        //        dblMaxDGD[i] = dblDGD[nMaxIndex];
        //    }
        //}

        public void CalcIso(double dblPassband, double dblCFPb, double dblCFDep, out double[] dblIso)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid);

            if ((dblCFDep > 0) && (dblCFPb > 0))
            {
                ArrayList alRes1 = new ArrayList();
                CalcParasPart1(dblCFDep, alIndexGrid, alRes1);

                ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

                alIndexGrid.Clear();
                CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid);
            }

            double[] dblIL = (double[])Data.m_aldblIL.ToArray(typeof(double));
            for (int i = 0; i < dblIL.Length; i++)
            {
                dblIL[i] = dblIL[i] * -1;
            }
            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexGrid.ToArray(typeof(int));

            ArrayList alPb = new ArrayList();

            ArrayList alLeftBoundary = new ArrayList();
            ArrayList alRightBoundary = new ArrayList();

            int nMultiCount = 9;
            int nTotalChCount = nIndexListArray.Length / nMultiCount;

            dblIso = new double[nTotalChCount];
            for (int i = 0; i < nTotalChCount; i++)
            {
                int nMinIndex = 0, nMaxIndex = 0;

                if (Data.FindMinMaxIndex(dblIL, nIndexListArray[i * nMultiCount],
                    nIndexListArray[i * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                double dblMaxIL = dblIL[nMaxIndex];

                if (Data.FindMinMaxIndex(dblIL, nIndexListArray[i * nMultiCount + 3],
                                        nIndexListArray[i * nMultiCount + 5], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                double dblLeftIso = dblIL[nMinIndex] - dblMaxIL;

                if (Data.FindMinMaxIndex(dblIL, nIndexListArray[i * nMultiCount + 6],
                    nIndexListArray[i * nMultiCount + 8], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                double dblRightIso = dblIL[nMinIndex] - dblMaxIL;

                dblIso[i] = (dblLeftIso < dblRightIso) ? dblLeftIso : dblRightIso;
            }
        }

        public void CalcWorstCD(double dblPassband, double dblCFPb, double dblCFDep, int nFitOrder, out double[] dblWstCD)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid,true);

            if ((dblCFDep > 0) && (dblCFPb > 0))
            {
                ArrayList alRes1 = new ArrayList();
                CalcParasPart1(dblCFDep, alIndexGrid, alRes1);

                ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

                alIndexGrid.Clear();
                CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid,true);
            }

            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            double[] dblCD = (double[])Data.m_aldblCD.ToArray(typeof(double));
            double[] dblFreq2 = (double[])Data.m_aldblFreq2.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexGrid.ToArray(typeof(int));

            int nMultiCount = 9;
            int nTotalChCount = nIndexListArray.Length / nMultiCount;

            dblWstCD = new double[nTotalChCount];
            for (int i = 0; i < nTotalChCount; i++)
            {
                ArrayList alIndexArray = new ArrayList();
                double dblStartFreq = dblFreq2[nIndexListArray[i * nMultiCount]];
                double dblEndFreq = dblFreq2[nIndexListArray[i * nMultiCount + 2]];

                if (dblStartFreq > dblEndFreq)
                {
                    dblStartFreq = dblStartFreq + dblEndFreq;
                    dblEndFreq = dblStartFreq - dblEndFreq;
                    dblStartFreq = dblStartFreq - dblEndFreq;
                }

                Data.GetRangeIndexFromArray(dblStartFreq, dblEndFreq, dblFreq2, alIndexArray);
                if (alIndexArray.Count == 0)
                {
                    //throw new Exception("定义通带过小，无法查找相应CD数值点!");
                    dblWstCD[i] = double.NaN;
                }
                else
                {
                    if (nFitOrder <= 0)
                    {
                        int nMinIndex = -1;
                        int nMaxIndex = -1;
                        Data.FindMinMaxIndex(dblCD, (int)alIndexArray[0], (int)alIndexArray[alIndexArray.Count - 1], ref nMinIndex, ref nMaxIndex);
                        if ((dblCD[nMinIndex] + dblCD[nMaxIndex]) < 0)
                            dblWstCD[i] = (dblCD[nMinIndex]);
                        else
                            dblWstCD[i] = (dblCD[nMaxIndex]);
                    }
                    else
                    {
                        double[] dblTempX = new double[alIndexArray.Count];
                        double[] dblTempY = new double[alIndexArray.Count];
                        for (int j = 0; j < alIndexArray.Count; j++)
                        {
                            dblTempX[j] = (double)Data.m_aldblFreq2[(int)alIndexArray[j]];
                            dblTempY[j] = (double)Data.m_aldblCD[(int)alIndexArray[j]];
                        }

                        int nEnlargeTimes = 100;
                        int nTotalCount = (int)Math.Abs(dblTempX[0] * nEnlargeTimes - dblTempX[dblTempX.Length - 1] * nEnlargeTimes);
                        double[] dblFitedValX = new double[nTotalCount];
                        for (int j = 0; j < nTotalCount; j++)
                        {
                            dblFitedValX[j] = dblTempX[0] - j / nEnlargeTimes;
                        }
                        // double[] dblCoeff = new double[10];
                        //LeastSquareFit.EMatrix(dblTempX, dblTempY, alIndexArray.Count, nFitOrder + 1, dblCoeff);

                        double[] dblFitedValY;
                        LeastSquareFit.ReturnFitedArray(dblTempX, dblTempY, alIndexArray.Count, nFitOrder + 1,
                                                dblFitedValX, out dblFitedValY);

                        int nMinIndex = -1, nMaxIndex = -1;
                        Data.FindMinMaxIndex(dblFitedValY, ref nMinIndex, ref nMaxIndex);

                        if ((dblFitedValY[nMinIndex] + dblFitedValY[nMaxIndex]) < 0)
                        {
                            dblWstCD[i] = dblFitedValY[nMinIndex];
                        }
                        else
                        {
                            dblWstCD[i] = dblFitedValY[nMaxIndex];
                        }
                    }
                }
            }
        }

        public void CalcGD(double dblPassband, double dblCFPb, double dblCFDep, int nFitOrder, int nTypeIndex, out double[] dblGDRipple)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid);

            if ((dblCFDep > 0) && (dblCFPb > 0))
            {
                ArrayList alRes1 = new ArrayList();
                CalcParasPart1(dblCFDep, alIndexGrid, alRes1);

                ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

                alIndexGrid.Clear();
                CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid);
            }

            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            double[] dblGD = (double[])Data.m_aldblGD.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexGrid.ToArray(typeof(int));

            int nMultiCount = 9;
            int nTotalChCount = nIndexListArray.Length / nMultiCount;

            dblGDRipple = new double[nTotalChCount];
            for (int i = 0; i < nTotalChCount; i++)
            {
                ArrayList alIndexArray = new ArrayList();

                if (nFitOrder <= 0)
                {
                    int nMinIndex = 0, nMaxIndex = 0;
                    Data.FindMinMaxIndex(dblGD, (int)alIndexGrid[i * 9], (int)alIndexGrid[i * 9 + 2], ref nMinIndex, ref nMaxIndex);
                    dblGDRipple[i] = dblGD[nMaxIndex] - dblGD[nMinIndex];
                }
                else
                {
                    int nStartIndex = (int)alIndexGrid[9 * i];
                    int nEndIndex = (int)alIndexGrid[9 * i + 2];

                    if (nStartIndex > nEndIndex)
                    {
                        nStartIndex = nStartIndex + nEndIndex;
                        nEndIndex = nStartIndex - nEndIndex;
                        nStartIndex = nStartIndex - nEndIndex;
                    }

                    int nTotalCount = Math.Abs(nEndIndex - nStartIndex) + 1;
                    double[] dblTempX = new double[nTotalCount];
                    double[] dblTempY = new double[nTotalCount];
                    for (int j = 0; j < nTotalCount; j++)
                    {
                        dblTempX[j] = (double)Data.m_aldblFreq1[nStartIndex + j];
                        dblTempY[j] = (double)Data.m_aldblGD[nStartIndex + j];
                    }

                    if (nTypeIndex == 1)
                    {
                        int nMinIndex = -1, nMaxIndex = -1;
                        Data.FindMinMaxIndex(dblTempY, ref nMinIndex, ref nMaxIndex);
                        dblGDRipple[i] = dblTempY[nMaxIndex] - dblTempY[nMinIndex];
                    }
                    else if (nTypeIndex == 2)
                    {
                        double[] dblFitY;
                        LinearFit.LeastSquareFit.ReturnFitedArray(dblTempX, dblTempY, nTotalCount,
                                                            nFitOrder + 1, dblTempX, out dblFitY);

                        double[] dblRes = new double[dblFitY.Length];
                        for (int j = 0; j < dblFitY.Length; j++)
                        {
                            dblRes[j] = Math.Abs(dblFitY[j] - dblTempY[j]);
                        }

                        int nMinIndex = -1, nMaxIndex = -1;
                        Data.FindMinMaxIndex(dblRes, ref nMinIndex, ref nMaxIndex);
                        dblGDRipple[i] = dblRes[nMaxIndex] - dblRes[nMinIndex];
                    }
                }
            }
        }

        public void CalcGDSlope(double dblPassband, double dblCFPb, double dblCFDep, out double[] dblGDSlope)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid);

            if ((dblCFDep > 0) && (dblCFPb > 0))
            {
                ArrayList alRes1 = new ArrayList();
                CalcParasPart1(dblCFDep, alIndexGrid, alRes1);

                ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

                alIndexGrid.Clear();
                CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid);
            }

            double[] dblFreq1 = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            double[] dblGD = (double[])Data.m_aldblGD.ToArray(typeof(double));
            int[] nIndexListArray = (int[])alIndexGrid.ToArray(typeof(int));

            int nMultiCount = 9;
            int nTotalChCount = nIndexListArray.Length / nMultiCount;

            dblGDSlope = new double[nTotalChCount];
            for (int i = 0; i < nTotalChCount; i++)
            {
                ArrayList alIndexArray = new ArrayList();

                int nStartIndex = (int)alIndexGrid[9 * i];
                int nEndIndex = (int)alIndexGrid[9 * i + 2];

                if (nStartIndex > nEndIndex)
                {
                    nStartIndex = nStartIndex + nEndIndex;
                    nEndIndex = nStartIndex - nEndIndex;
                    nStartIndex = nStartIndex - nEndIndex;
                }

                int nTotalCount = nEndIndex - nStartIndex + 1;
                double[] dblTempX = new double[nTotalCount];
                double[] dblTempY = new double[nTotalCount];
                for (int j = 0; j < nTotalCount; j++)
                {
                    dblTempX[j] = 299792458/(double)Data.m_aldblFreq1[nStartIndex + j];
                    dblTempY[j] = (double)Data.m_aldblGD[nStartIndex + j];
                }

                double[] dblCoeff = new double[10];
                LeastSquareFit.EMatrix(dblTempX, dblTempY, nTotalCount, 2, dblCoeff);
                dblGDSlope[i] = dblCoeff[2];
            }
        }

        public void CalcPb(double dblPassband, double dblDepth, double dblCFPb, double dblCFDep, int nFitOrder, bool bCleanBd, out double[] dblPb)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid);

            ArrayList alLeft = new ArrayList();
            ArrayList alRight = new ArrayList();
            CalcEdge(nFitOrder, dblDepth, alIndexGrid, alLeft, alRight);

            if ((dblCFDep > 0) && (dblCFPb > 0))
            {
                ArrayList alRes1 = new ArrayList();
                CalcParasPart1(dblCFDep, alIndexGrid, alRes1);

                ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

                alIndexGrid.Clear();
                CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid);
            }

            double[] dblFreq = (double[])Data.m_aldblFreq1.ToArray(typeof(double));

            dblPb = new double[alLeft.Count];
            for (int i = 0; i < dblPb.Length; i++)
            {
                if (bCleanBd == true)
                {
                    double dblITUCF = (double)alITUGrid[i];
                    double dblLeftLen = Math.Abs((double)alLeft[i] - dblITUCF);
                    double dblRightLen = Math.Abs((double)alRight[i] - dblITUCF);

                    dblPb[i] = 2 * ((dblLeftLen < dblRightLen) ? dblLeftLen : dblRightLen);
                }
                else
                {
                    dblPb[i] = Math.Abs((double)alLeft[i] - (double)alRight[i]);
                }
            }
        }

        public void CalcStopband(double dblPassband, double dblDepth, double dblCFPb, double dblCFDep, int nFitOrder, out double[] dblStopband)
        {
            ILChannelType thisType = checkChannelType();
            ArrayList alITUGrid = new ArrayList();
            CreateITUGrid(thisType, alITUGrid);

            ArrayList alIndexGrid = new ArrayList();
            CreateIndexGrid(alITUGrid, dblPassband, alIndexGrid);

            ArrayList alLeft = new ArrayList();
            ArrayList alRight = new ArrayList();
            CalcEdge(nFitOrder, dblDepth, alIndexGrid, alLeft, alRight);

            if ((dblCFDep > 0) && (dblCFPb > 0))
            {
                ArrayList alRes1 = new ArrayList();
                CalcParasPart1(dblCFDep, alIndexGrid, alRes1);

                ArrayList alRealBaseGrid = (ArrayList)alRes1[0];

                alIndexGrid.Clear();
                CreateIndexGrid(alRealBaseGrid, dblPassband, alIndexGrid);
            }

            double[] dblFreq = (double[])Data.m_aldblFreq1.ToArray(typeof(double));
            double[] dblIL = (double[])Data.m_aldblIL.ToArray(typeof(double));
            int nMultiCount = 9;
            int nTotalChCount = alIndexGrid.Count / nMultiCount;

            dblStopband = new double[nTotalChCount];
            for (int i = 0; i < nTotalChCount; i++)
            {
                int nMinIndex = 0, nMaxIndex = 0;

                if (Data.FindMinMaxIndex(dblIL, (int)alIndexGrid[i * nMultiCount],
                        (int)alIndexGrid[i * nMultiCount + 2], ref nMinIndex, ref nMaxIndex) == false)
                {
                    throw new Exception("数据异常!");
                }
                double dblMaxIL = dblIL[nMaxIndex];

                double dblTempILVal = dblMaxIL - dblDepth;

                int nPoint1 = 0, nPoint2 = 0;
                if (thisType == ILChannelType.ChannelTypeEven)
                {
                    int nKick0 = 0;
                    if (i > 0)
                        nKick0 = (int)alIndexGrid[(i - 1) * nMultiCount + 1];
                    int nKick1 = (int)alIndexGrid[i * nMultiCount + 1];

                    nPoint1 = Data.FindFirstIndex(
                                        dblTempILVal,
                                        dblIL,
                                        nKick0,
                                        nKick1);

                    nPoint2 = Data.FindFirstIndex(
                                        dblTempILVal,
                                        dblIL,
                                        nKick1,
                                        nKick0);
                }
                else if (thisType == ILChannelType.ChannelTypeOdd)
                {
                    int nKick0 = (int)alIndexGrid[i * nMultiCount + 1];
                    int nKick1 = dblIL.Length - 1;
                    if (i < nTotalChCount - 1)
                        nKick1 = (int)alIndexGrid[(i + 1) * nMultiCount + 1];

                    nPoint1 = Data.FindFirstIndex(
                                        dblTempILVal,
                                        dblIL,
                                        nKick0,
                                        nKick1);

                    nPoint2 = Data.FindFirstIndex(
                                        dblTempILVal,
                                        dblIL,
                                        nKick1,
                                        nKick0);
                }

                double dblBand1 = 0, dblBand2 = 0;
                int nAnotherFitPoint = 0;
                if (nPoint1 == 0)
                    nAnotherFitPoint = 1;
                else if (nPoint1 == dblFreq.Length - 1)
                    nAnotherFitPoint = dblFreq.Length - 2;
                else
                {
                    nAnotherFitPoint = nPoint1 - 1;
                    if (((dblIL[nAnotherFitPoint] - dblTempILVal) * (dblIL[nPoint1] - dblTempILVal)) > 0)
                        nAnotherFitPoint = nPoint1 + 1;
                }
                dblBand1 = LeastSquareFit.LinearFitByTwoPoint(dblIL[nAnotherFitPoint],
                                                        dblIL[nPoint1],
                                                        dblFreq[nAnotherFitPoint],
                                                        dblFreq[nPoint1],
                                                        dblTempILVal);

                nAnotherFitPoint = 0;
                if (nPoint2 == 0)
                    nAnotherFitPoint = 1;
                else if (nPoint2 == dblFreq.Length - 1)
                    nAnotherFitPoint = dblFreq.Length - 2;
                else
                {
                    nAnotherFitPoint = nPoint2 - 1;
                    if (((dblIL[nAnotherFitPoint] - dblTempILVal) * (dblIL[nPoint2] - dblTempILVal)) > 0)
                        nAnotherFitPoint = nPoint2 + 1;
                }
                dblBand2 = LeastSquareFit.LinearFitByTwoPoint(dblIL[nAnotherFitPoint],
                                                        dblIL[nPoint2],
                                                        dblFreq[nAnotherFitPoint],
                                                        dblFreq[nPoint2],
                                                        dblTempILVal);

                dblStopband[i] = Math.Abs(dblBand1 - dblBand2);
            }
        }
    }

    class CDCalc
    {
        public void analysisdata_byCD(double dblFitRange)
        {
            ArrayList alAllChTopPointIndex = new ArrayList();
            ArrayList alAllChBottomPointIndex = new ArrayList();
            //ArrayList alAllCrossZeroPoint = new ArrayList();
            Data.m_aldblFitCDZeroPoint.Clear();
            Data.m_aldblAverageCDZeroPoint.Clear();
            Data.m_aldblWorstCDVal.Clear();

            // find first point;
            double dblTempMin = 1, dblTempMax = -1;
            int nFirstMaxIndex = 0, nFirstMinIndex = 0;
            for (int i = 0; i < Data.m_aldblFreq2.Count; i++)
            {
                if (Math.Abs((double)Data.m_aldblFreq2[i] - (double)Data.m_aldblFreq2[0]) > 100)
                    break;

                if ((double)Data.m_aldblCD[i] > dblTempMax)
                {
                    dblTempMax = (double)Data.m_aldblCD[i];
                    nFirstMaxIndex = i;
                }
                if ((double)Data.m_aldblCD[i] < dblTempMin)
                {
                    dblTempMin = (double)Data.m_aldblCD[i];
                    nFirstMinIndex = i;
                }
            }

            // set next position in loop
            alAllChTopPointIndex.Add(nFirstMaxIndex);
            alAllChBottomPointIndex.Add(nFirstMinIndex);

            // this code segment is for get all bottom point
            int nTempBeginIndex = nFirstMinIndex;
            for (; ; )
            {
                double dblCurBaseFreq = (double)Data.m_aldblFreq2[nTempBeginIndex];
                double dblMaxFreqOffset = Math.Abs((double)Data.m_aldblFreq2[Data.m_aldblFreq2.Count - 1] - dblCurBaseFreq);
                if (dblMaxFreqOffset < 50)
                    break;

                double dblMinValue = 1e9;
                int nMinIndex = 0;
                for (int i = nTempBeginIndex; i < Data.m_aldblFreq2.Count; i++)
                {
                    double dblCurFreqOffset = Math.Abs((double)Data.m_aldblFreq2[i] - dblCurBaseFreq);

                    if (dblCurFreqOffset > 150)
                        break;

                    if (dblCurFreqOffset < 50)
                        continue;

                    if ((double)Data.m_aldblCD[i] < dblMinValue)
                    {
                        dblMinValue = (double)Data.m_aldblCD[i];
                        nMinIndex = i;
                    }
                }

                alAllChBottomPointIndex.Add(nMinIndex);
                nTempBeginIndex = nMinIndex;
            }
            // this code segment is for get all top point
            nTempBeginIndex = nFirstMaxIndex;
            for (; ; )
            {
                double dblCurBaseFreq = (double)Data.m_aldblFreq2[nTempBeginIndex];
                double dblMaxFreqOffset = Math.Abs((double)Data.m_aldblFreq2[Data.m_aldblFreq2.Count - 1] - dblCurBaseFreq);
                if (dblMaxFreqOffset < 50)
                    break;

                double dblMaxValue = -1e9;
                int nMaxIndex = 0;
                for (int i = nTempBeginIndex; i < Data.m_aldblFreq2.Count; i++)
                {
                    double dblCurFreqOffset = Math.Abs((double)Data.m_aldblFreq2[i] - dblCurBaseFreq);

                    if (dblCurFreqOffset > 150)
                        break;

                    if (dblCurFreqOffset < 50)
                        continue;

                    if ((double)Data.m_aldblCD[i] > dblMaxValue)
                    {
                        dblMaxValue = (double)Data.m_aldblCD[i];
                        nMaxIndex = i;
                    }
                }

                alAllChTopPointIndex.Add(nMaxIndex);
                nTempBeginIndex = nMaxIndex;
            }
            // here to sort all channel section
            // define one channel is from min to max
            // so min.count == max.count && min[i] < max[i]
            if ((int)alAllChBottomPointIndex[0] < (int)alAllChTopPointIndex[0])
                alAllChBottomPointIndex.RemoveAt(0);

            if (alAllChTopPointIndex.Count < alAllChBottomPointIndex.Count)
                alAllChBottomPointIndex.RemoveAt(alAllChBottomPointIndex.Count - 1);
            else if (alAllChTopPointIndex.Count > alAllChBottomPointIndex.Count)
                alAllChTopPointIndex.RemoveAt(alAllChTopPointIndex.Count - 1);

            //if ((int)alAllChTopPointIndex[0] == 0)
            //{
            //    alAllChTopPointIndex.RemoveAt(0);
            //    alAllChTopPointIndex.Insert(0, 1);
            //}

            // after get all channel sections
            // find any cross-zero point in each section
            for (int i = 0; i < alAllChTopPointIndex.Count; i++)
            {
                double dblTempSum = 0;
                int nTempCount = 0;
                for (int j = (int)alAllChTopPointIndex[i]; j < (int)alAllChBottomPointIndex[i]; j++)
                {
                    if (j == 0)
                        continue;

                    double dblCurCDState = (double)Data.m_aldblCD[j - 1] * (double)Data.m_aldblCD[j];
                    if (dblCurCDState < 0)
                    {
                        dblTempSum += (double)Data.m_aldblFreq2[j] - (double)Data.m_aldblCD[j] /
                            ((double)Data.m_aldblCD[j] - (double)Data.m_aldblCD[j - 1]) *
                            ((double)Data.m_aldblFreq2[j] - (double)Data.m_aldblFreq2[j - 1]);
                        nTempCount++;
                    }
                }

                if (nTempCount > 0)
                {
                    double dblTempCurCrossZeroPoint = dblTempSum / nTempCount;
                    Data.m_aldblAverageCDZeroPoint.Add(dblTempCurCrossZeroPoint);
                }
            }

            double[] dblValue = (double[])Data.m_aldblAverageCDZeroPoint.ToArray(typeof(double));
            for (int i = 0; i < dblValue.Length; i++)
            {
                if (double.IsNaN(dblValue[i]) == true)
                    continue;

                ArrayList alIndexArray = new ArrayList();
                double[] dblTotal = (double[])Data.m_aldblFreq2.ToArray(typeof(double));
                Data.GetRangeIndexFromArray(dblValue[i] - dblFitRange, dblValue[i] + dblFitRange, dblTotal, alIndexArray);

                int nFitArrayLen = alIndexArray.Count;
                double[] dblFitLineX = new double[nFitArrayLen];
                double[] dblFitLineY = new double[nFitArrayLen];
                for (int j = 0; j < nFitArrayLen; j++)
                {
                    dblFitLineX[j] = (double)Data.m_aldblCD[(int)alIndexArray[j]];
                    dblFitLineY[j] = (double)Data.m_aldblFreq2[(int)alIndexArray[j]];
                }

                double[] dblCoffe = new double[10];
                LeastSquareFit.EMatrix(dblFitLineX, dblFitLineY, nFitArrayLen, 5, dblCoffe);

                Data.m_aldblFitCDZeroPoint.Add(dblCoffe[1]);

                // find the worst CD value from fit center freq +/- 8GHz
                Data.GetRangeIndexFromArray(dblCoffe[1] - 8, dblCoffe[1] + 8, dblTotal, alIndexArray);
                dblTempMax = 0;
                dblTempMin = 0;
                for (int j = 0; j < alIndexArray.Count; j++)
                {
                    if (dblTempMax < (double)Data.m_aldblCD[(int)alIndexArray[j]])
                        dblTempMax = (double)Data.m_aldblCD[(int)alIndexArray[j]];

                    if (dblTempMin > (double)Data.m_aldblCD[(int)alIndexArray[j]])
                        dblTempMin = (double)Data.m_aldblCD[(int)alIndexArray[j]];
                }
                double dblWorst = ((dblTempMin + dblTempMax) > 0) ? dblTempMax : dblTempMin;
                Data.m_aldblWorstCDVal.Add(dblWorst);
            }
            return;
        }
    }

    class GDClac
    {

    }
}
