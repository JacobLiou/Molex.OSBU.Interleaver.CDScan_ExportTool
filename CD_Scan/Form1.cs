using Agilent.LWD.Ag86038x;
using Agilent.LWD.Ag86038x.InstrumentObjects;
using InterleaverDateKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;
//using System.Threading.Tasks;
//using Excel = Microsoft.Office.Interop.Excel;

namespace CD_Scan
{
    public partial class Form1 : Form
    {
        double[] dblFreq1 = new double[20000];
        double[] dblFreq2 = new double[20000];
        double[] dblIL = new double[20000];
        double[] dblCD = new double[20000];
        double[] dblGD = new double[20000];
        //double[] dblDGD = new double[20000];
        double[] dblPhase = new double[20000];
        double[] dblPDL = new double[20000];
        double[] dblPMD = new double[20000];
        double[] dblprarmax = new double[30];
        inipara[] stpara = new inipara[2];
        mateinfo stmateinfo;
        int nuseini;
        bool m_btypefour;
        bool m_bfind;
        bool m_bPDL, breadfile;
        string m_strdata, m_strini;
        string m_strCDDriverIP, m_strscriptfile, m_strtestfile, m_strresultfile, m_strtempletfile, m_strinputfile, m_stroutputfile, m_strconfigpath;
        string m_strPN,m_strPNini;
        double dblstartw, dblstopw, dblstep, dblrf;
        int nIF, nchspace, nchstep;
        int m_nCDP;
        int m_nCalGhz;
        bool breadpara;
        bool bconnect;
        ArrayList alFindByCDFreq = new ArrayList();
        public RemoteClient.Communicator pdlaClient = new RemoteClient.Communicator();

        private ODARemoting.NewStatusDelegate NewStatusHandler;
        private ODARemoting.TriggerProgressDelegate TriggerHandler;



        public void TriggerProgessEvent(ODACommon.enumStatus status, ODACommon.enumAcquisitionMode acqMode)
        {
            try
            {
                if (acqMode == ODACommon.enumAcquisitionMode.eNormalization)
                {
                    if ((status == ODACommon.enumStatus.COMPLETE) || (status == ODACommon.enumStatus.COMPLETE_WARN))
                    {

                        MessageBox.Show("instrument normalize complete!");
                    }
                }

                if (acqMode == ODACommon.enumAcquisitionMode.eMeasurement)
                {
                    if ((status == ODACommon.enumStatus.COMPLETE) || (status == ODACommon.enumStatus.COMPLETE_WARN))
                    {
                        getResultValue(dblFreq1, dblFreq2, dblGD, dblIL, dblPhase, dblCD);
                        savedata();
                        label_result.Text = "扫描完成！";

                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }
        public Form1()
        {
            InitializeComponent();
            Readcfgfile();
            ReadCDfile();
            bconnect = false;
            m_nCDP = 0;
            nuseini = 0;
            breadfile = true;
            m_bfind = false;
            breadpara = false;


        }
        private void ReadCDfile()
        {
            string strfilename;
            strfilename = Application.StartupPath + "\\CD_IP.txt";
            StreamReader strcfg = new StreamReader(strfilename, System.Text.Encoding.GetEncoding("GBK"));
            string strcfgvalue = strcfg.ReadLine();
            textBox_IP.Text = strcfgvalue;
            strcfg.Close();
        }
        private void writeCDfile()
        {
            string strfilename;
            strfilename = Application.StartupPath + "\\CD_IP.txt";
            StreamWriter strcfg = new StreamWriter(strfilename);
            string strcfgvalue = textBox_IP.Text;
            strcfg.WriteLine(strcfgvalue);
            strcfg.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {

            openFileDialog1.ShowDialog();
            m_strtestfile = openFileDialog1.FileName;
            lab_data.Text = m_strtestfile;

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

            string strtemplet = m_strconfigpath + "\\CD_templet.csv";

            m_strresultfile = m_strtestfile.Substring(0, m_strtestfile.Length - 4) + "-导表.csv";

            if (System.IO.File.Exists(m_strresultfile))
            {
                // DialogResult r1 = MessageBox.Show("已经有该导表文件，是否覆盖", "标题", MessageBoxButtons.YesNoCancel);

                string strnewfilename = "";
                int nfileindex = 1;
                strnewfilename = string.Format("{0}_{1}.csv", m_strresultfile.PadLeft(m_strresultfile.Length - 4), nfileindex);
                while (System.IO.File.Exists(strnewfilename))
                {
                    nfileindex++;
                    strnewfilename = string.Format("{0}_{1}.csv", m_strresultfile.PadLeft(m_strresultfile.Length - 4), nfileindex);
                }
                System.IO.File.Copy(m_strresultfile, strnewfilename);
                //if (r1.ToString() == "Yes")

                //{ MessageBox.Show("Yes"); }

               // if (r1.ToString().Equals("No"))

             //   { return; }

                //if (r1.ToString().Equals("Cancel"))

                //{ MessageBox.Show("Cancel"); }


            }
            string strResFile = m_strresultfile;
            int nFirstLine, nFirstCol;
            double dblFirstCh, dblLastCh;
            ArrayList alTotalRes = new ArrayList();
            ArrayList alTitle = new ArrayList();

            ArrayList alTitleparameter = new ArrayList();
            ArrayList alITUCF = new ArrayList();
            calcResult(alTotalRes, alTitle, alTitleparameter, alITUCF, out nFirstLine, out nFirstCol, out dblFirstCh, out dblLastCh);

            //  Excel.Application excel = new Excel.ApplicationClass();
            try
            {
                string strTemplateFile = strtemplet;

                // initial data preparing
                int nRealLine = (int)(dblLastCh - dblFirstCh) + 1;
                if (nchspace < 100)
                {
                    nRealLine = nRealLine * 2;
                }                
                if (nchspace > 100)
                {
                    nRealLine = nRealLine*2/ 3+1;
                }
                if (nchspace > 150)
                {
                    nRealLine = nRealLine / 2 + 1;
                }
                double dblBaseFreqC = (m_nCalGhz / 100 + dblLastCh) * 100;

                //modify by chaoli;
                //double dblBaseFreqH = dblBaseFreqC + 50;
                double dblBaseFreqH = dblBaseFreqC + nchstep;

                // double dblBaseFreqH = dblBaseFreqC + double.Parse(textBox_chspace.Text);

                //excel.Workbooks.Open(strtemplet,
                //                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                //                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                //                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                //                                    Type.Missing, Type.Missing);
                //Excel.Worksheet curSheet = (Excel.Worksheet)excel.Worksheets[1];

                int nRealStartIndex = 0;
                for (nRealStartIndex = 0; nRealStartIndex < alITUCF.Count; nRealStartIndex++)
                {
                    double dblCurCF = (double)alITUCF[nRealStartIndex];
                    if ((Math.Abs(dblCurCF - dblBaseFreqC) > 20) && (Math.Abs(dblCurCF - dblBaseFreqH) > 20))
                        continue;

                    break;
                }
                StreamWriter strresultfile = new StreamWriter(strResFile);
                string strtempw = "", strtempwpara = "", strtempvalue = "";
                for (int i = 0; i < alTitle.Count; i++)
                {
                    strtempw += alTitle[i].ToString() + ",";
                    strtempwpara += alTitleparameter[i].ToString() + ",";


                }

                strresultfile.WriteLine(strtempw);
                strresultfile.WriteLine(strtempwpara);
                double minvalue = 1000;
                ArrayList alstrtempw = new ArrayList();
                for (int j = 0; j < ((double[])alTotalRes[0]).Length; j++)
                {

                    if (j < nRealStartIndex)
                        continue;
                    else if (j >= (nRealLine + nRealStartIndex))
                        break;
                    else
                    {
                        strtempw = "";
                        for (int i = 0; i < alTotalRes.Count; i++)
                        {

                            if (Math.Abs(((double[])alTotalRes[i])[j]) > Math.Abs(dblprarmax[i]))
                            {
                                dblprarmax[i] = ((double[])alTotalRes[i])[j];
                            }
                            strtempw += ((double[])alTotalRes[i])[j].ToString() + ",";
                            //curSheet.Cells[nFirstLine + j - nRealStartIndex, nFirstCol + i] = strCurVal;
                        }
                        alstrtempw.Add(strtempw);
                        //strresultfile.WriteLine(strtempw);
                    }

                }
                for (int i = 0; i < alTitle.Count; i++)
                {
                    minvalue = 1000;
                    if (alTitle[i].ToString() == "MIN IL")
                    {
                        for (int j = 0; j < ((double[])alTotalRes[i]).Length; j++)
                        {
                            if (j < nRealStartIndex)
                                continue;
                            else if (j >= (nRealLine + nRealStartIndex))
                                break;
                            if (Math.Abs(((double[])alTotalRes[i])[j]) < Math.Abs(minvalue))
                            {
                                minvalue = ((double[])alTotalRes[i])[j];
                            }

                        }
                        dblprarmax[i] = minvalue;
                    }
                    if (m_nCDP > 0)
                    {
                        if (alTitle[i].ToString() == "Worst CD")
                        {
                            double dblcdsum = 0;
                            int nindex = 0;
                            for (int j = 0; j < ((double[])alTotalRes[i]).Length; j++)
                            {
                                if (j < nRealStartIndex)
                                    continue;
                                else if (j >= (nRealLine + nRealStartIndex))
                                    break;
                                //if (Math.Abs(((double[])alTotalRes[i])[j]) < Math.Abs(minvalue))
                                //{
                                //    minvalue = ((double[])alTotalRes[i])[j];
                                //}
                                dblcdsum += Math.Abs(((double[])alTotalRes[i])[j]);
                                nindex++;
                            }
                            dblprarmax[i] = dblcdsum / nindex;
                        }
                    }
                    if (alTitle[i].ToString() == "Adj Iso")
                    {
                        for (int j = 0; j < ((double[])alTotalRes[i]).Length; j++)
                        {

                            if (j < nRealStartIndex)
                                continue;
                            else if (j >= (nRealLine + nRealStartIndex))
                                break;
                            if (Math.Abs(((double[])alTotalRes[i])[j]) < Math.Abs(minvalue))
                            {
                                minvalue = ((double[])alTotalRes[i])[j];
                            }

                        }
                        dblprarmax[i] = minvalue;
                    }

                }

                strtempvalue = "MAX-Value,";
                for (int i = 1; i < alTitle.Count; i++)
                {
                    strtempvalue += dblprarmax[i].ToString() + ",";
                }
                strresultfile.WriteLine(strtempvalue);
                strresultfile.WriteLine("");
                for (int i = 0; i < alstrtempw.Count; i++)
                {
                    strresultfile.WriteLine(alstrtempw[i]);
                }
                strresultfile.Close();

                MessageBox.Show("导表完成！");

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void button6_Click(object sender, EventArgs e)
        {
            if(!breadpara)
            {
                MessageBox.Show("未选择参数！");
                return;
            }
           
            if (!readtestfile())
            {
                return;
            }
          
            if (stmateinfo.dcmuse > 1)
            {
                ArrayList dcmsn1 = new ArrayList(stmateinfo.dcm1path);
                ArrayList dcmsn2 = new ArrayList(stmateinfo.dcm2path);
                ArrayList dcmsn1name = new ArrayList(stmateinfo.dcm1sn);
                ArrayList dcmsn2name = new ArrayList(stmateinfo.dcm2sn);
                ArrayList coersneven = new ArrayList();
                ArrayList coersnodd = new ArrayList();
                //ArrayList coersneven = new ArrayList(stmateinfo.coerpatheven);
                //ArrayList coersnodd = new ArrayList(stmateinfo.coerpathodd);
                string[] strcoerdcmsn = new string[stmateinfo.coersum];
                if (!sortcoer(coersneven, coersnodd))
                {
                    return;
                }
                // m_strscriptfile
                for (int ncoer = 0; ncoer < stmateinfo.coersum; ncoer++)
                {
                    Datanew[] datafile = new Datanew[3];
                    datafile[0] = new Datanew();
                    datafile[1] = new Datanew();
                    datafile[2] = new Datanew();
                    datafile[0].getvaluefromfile(coersneven[ncoer].ToString());

                    int i = 0;
                    bool bin = true;
                    while ((dcmsn1.Count > 0) && (bin))
                    {
                        datafile[1].getvaluefromfile(dcmsn1[i].ToString());
                        for (int n = 0; n < dcmsn2.Count; n++)
                        {
                            datafile[2].getvaluefromfile(dcmsn2[n].ToString());
                            m_strtestfile = coersneven[ncoer].ToString().Substring(0, coersneven[ncoer].ToString().Length - 4) + "_" + dcmsn1name[i] + "_" + dcmsn2name[n] + ".csv";
                            m_strscriptfile = stmateinfo.scriptfileeven;

                            if (fileamalgamate(datafile, 3))
                            {
                                dcmsn1.RemoveAt(i);
                                dcmsn2.RemoveAt(n);
                                dcmsn1name.RemoveAt(i);
                                dcmsn2name.RemoveAt(n);
                                bin = false;
                                break;
                            }
                        }
                        i++;
                    }



                }
            }
            else
            {
                ArrayList dcmsn1 = new ArrayList(stmateinfo.dcm1path);
                ArrayList dcmsn1name = new ArrayList(stmateinfo.dcm1sn);
                ArrayList coersneven = new ArrayList();
                ArrayList coersnodd = new ArrayList();
                //ArrayList coersneven = new ArrayList(stmateinfo.coerpatheven);
                //ArrayList coersnodd = new ArrayList(stmateinfo.coerpathodd);
                string[] strcoerdcmsn = new string[stmateinfo.coersum];
                if (!sortcoer(coersneven, coersnodd))
                {
                    return;
                }
                // m_strscriptfile
                for (int ncoer = 0; ncoer < stmateinfo.coersum; ncoer++)
                {
                    m_bfind = false;
                    Datanew[] datafile = new Datanew[2];
                    datafile[0] = new Datanew();
                    datafile[1] = new Datanew();

                    datafile[0].getvaluefromfile(coersneven[ncoer].ToString());

                    for (int n = 0; n < dcmsn1.Count; n++)
                    {
                        datafile[1].getvaluefromfile(dcmsn1[n].ToString());

                        m_strtestfile = coersneven[ncoer].ToString().Substring(0, coersneven[ncoer].ToString().Length - 4) + "_" + dcmsn1name[n] + ".csv";
                        m_strscriptfile = stmateinfo.scriptfileeven;

                        if (fileamalgamate(datafile, 2))
                        {
                            datafile[0].getvaluefromfile(coersnodd[ncoer].ToString());
                            m_strtestfile = coersnodd[ncoer].ToString().Substring(0, coersnodd[ncoer].ToString().Length - 4) + "_" + dcmsn1name[n] + ".csv";
                            m_strscriptfile = stmateinfo.scriptfileodd;
                            if (fileamalgamate(datafile, 2))
                            {
                                m_bfind = true;
                                dcmsn1.RemoveAt(n);
                                dcmsn1name.RemoveAt(n);
                                break;
                            }
                        }
                    }
                    if(!m_bfind)
                    {
                        string strtemp = "", strtempwrite="";
                    
                        string strinipath = "";
                        string strsection = "";

                        strinipath = Application.StartupPath + "\\NO_pair.ini";
                        Ini inifile = new Ini();                      
                        strtemp = inifile.ReadValue("info", "number", strinipath);
                        strtempwrite = (int.Parse(strtemp) + 1).ToString();
                        inifile.Writue("info", "number", strtempwrite, strinipath);
                        strsection = "SN_" + strtempwrite;
                        strtempwrite = coersneven[ncoer].ToString();                        
                        inifile.Writue("info", strsection, strtempwrite, strinipath);
                    }

                }
            }

            MessageBox.Show("匹配完成！");

        }
        private bool sortcoer(ArrayList coersneven, ArrayList coersnodd)
        {
            try
            {
                string filename = "";
                string strread = "";
                double dblevenCD = 0, dbloddCD = 0;
                Dictionary<int, double> dic = new Dictionary<int, double>();
                double dblCDoffset = 0;
                for (int n = 0; n < stmateinfo.coersum; n++)
                {
                    StreamReader readfile1 = new StreamReader(stmateinfo.coerpathevenresult[n]);
                    strread = readfile1.ReadLine();
                    strread = readfile1.ReadLine();
                    strread = readfile1.ReadLine();
                    string[] strtemp = strread.Split(',');
                    dblevenCD = double.Parse(strtemp[stpara[0].position[0]]);
                    readfile1.Close();

                    StreamReader readfile2 = new StreamReader(stmateinfo.coerpathoddresult[n]);
                    strread = readfile2.ReadLine();
                    strread = readfile2.ReadLine();
                    strread = readfile2.ReadLine();
                    strtemp = strread.Split(',');
                    dbloddCD = double.Parse(strtemp[stpara[0].position[0]]);
                    readfile2.Close();
                    if (dblevenCD > dbloddCD)
                    {
                        dblCDoffset = Math.Abs(stpara[0].checkCD - dblevenCD);
                    }
                    else
                    {
                        dblCDoffset = Math.Abs(stpara[0].checkCD - dbloddCD);
                    }
                    dic.Add(n, dblCDoffset);
                }

                var dicSort = from objDic in dic orderby objDic.Value descending select objDic;
                foreach (KeyValuePair<int, double> kvp in dicSort)
                {
                    coersneven.Add(stmateinfo.coerpatheven[kvp.Key]);
                    coersnodd.Add(stmateinfo.coerpathodd[kvp.Key]);

                }
            }
            catch (Exception es)
            {
                return false;

            }
            //    Console.Write(kvp.Key + "：" + kvp.Value + "  ");
            return true;
        }
        private bool fileamalgamate(Datanew[] datafile, int nfile)
        {

            Data.zerodata();
            for (int npoint = 0; npoint < datafile[0].m_aldblFreq1.Count - 1; npoint++)
            {
                Data.m_aldblFreq1.Add(datafile[0].m_aldblFreq1[npoint]);
                Data.m_aldblFreq2.Add(datafile[0].m_aldblFreq2[npoint]);

                if (nfile > 2)
                {
                    Data.m_aldblGD.Add((double)datafile[0].m_aldblGD[npoint] + (double)datafile[1].m_aldblGD[npoint] + (double)datafile[2].m_aldblGD[npoint]);

                    Data.m_aldblIL.Add((double)datafile[0].m_aldblIL[npoint] + (double)datafile[1].m_aldblIL[npoint] + (double)datafile[2].m_aldblIL[npoint]);

                    Data.m_aldblPhase.Add((double)datafile[0].m_aldblPhase[npoint] + (double)datafile[1].m_aldblPhase[npoint] + (double)datafile[2].m_aldblPhase[npoint]);

                    Data.m_aldblPDL.Add((double)datafile[0].m_aldblPDL[npoint] + (double)datafile[1].m_aldblPDL[npoint] + (double)datafile[2].m_aldblPDL[npoint]);

                    Data.m_aldblPMD.Add((double)datafile[0].m_aldblPMD[npoint] + (double)datafile[1].m_aldblPMD[npoint] + (double)datafile[2].m_aldblPMD[npoint]);

                    Data.m_aldblCD.Add((double)datafile[0].m_aldblCD[npoint] + (double)datafile[1].m_aldblCD[npoint] + (double)datafile[2].m_aldblCD[npoint]);


                }
                else
                {

                    Data.m_aldblGD.Add((double)datafile[0].m_aldblGD[npoint] + (double)datafile[1].m_aldblGD[npoint]);

                    Data.m_aldblIL.Add((double)datafile[0].m_aldblIL[npoint] + (double)datafile[1].m_aldblIL[npoint]);

                    Data.m_aldblPhase.Add((double)datafile[0].m_aldblPhase[npoint] + (double)datafile[1].m_aldblPhase[npoint]);

                    Data.m_aldblPDL.Add((double)datafile[0].m_aldblPDL[npoint] + (double)datafile[1].m_aldblPDL[npoint]);

                    Data.m_aldblPMD.Add((double)datafile[0].m_aldblPMD[npoint] + (double)datafile[1].m_aldblPMD[npoint]);

                    Data.m_aldblCD.Add((double)datafile[0].m_aldblCD[npoint] + (double)datafile[1].m_aldblCD[npoint]);

                }



            }

            if (anaylsedata())
            {
                StreamWriter strresultfile = new StreamWriter(m_strtestfile);
                string strtempw = "";
                strtempw = "Freq,GD,IL/Gain,Phase,PDL,PMD,Freq,CD";
                strresultfile.WriteLine(strtempw);
                for (int npoint = 0; npoint < datafile[0].m_aldblFreq1.Count - 1; npoint++)
                {
                    strtempw = Data.m_aldblFreq1[npoint].ToString() + "," + Data.m_aldblGD[npoint].ToString() + "," + Data.m_aldblIL[npoint].ToString() + "," + Data.m_aldblPhase[npoint].ToString() + "," + Data.m_aldblPDL[npoint].ToString() + "," + Data.m_aldblPMD[npoint].ToString() + "," + Data.m_aldblFreq2[npoint].ToString() + "," + Data.m_aldblCD[npoint].ToString();
                    strresultfile.WriteLine(strtempw);
                }

                strresultfile.Close();
                return true;
            }
            return false;
        }
        private bool readinifile(int npara)
        {
            //dblparametercheck

            string strtemp = "", strfilename = " ", strshow = "";
            string strpara = "", strposition = "";
            string strinipath = "";
            string strsection = "";

            strinipath = Application.StartupPath + "\\parameter.ini";
            Ini inifile = new Ini();
            if(npara<2)
            {
                strsection = "8048M" + "_1";
                strtemp = inifile.ReadValue(strsection, "Paraname", strinipath);
                stpara[0].paraname = strtemp;
                string[] strtempname = strtemp.Split(',');
                strtemp = inifile.ReadValue(strsection, "Parasum", strinipath);
                stpara[0].parasum = int.Parse(strtemp);
                stpara[0].position = new int[stpara[0].parasum + 1];
                stpara[0].paracheckvalue = new double[stpara[0].parasum + 1];
                strtemp = inifile.ReadValue(strsection, "CheckCD", strinipath);
                stpara[0].checkCD = double.Parse(strtemp);

                for (int n = 0; n < stpara[0].parasum; n++)
                {
                    strposition = string.Format("position_{0}", n + 1);
                    strtemp = inifile.ReadValue(strsection, strposition, strinipath);
                    stpara[0].position[n] = int.Parse(strtemp);
                    strpara = string.Format("paravalue_{0}", n + 1);
                    strtemp = inifile.ReadValue(strsection, strpara, strinipath);
                    stpara[0].paracheckvalue[n] = double.Parse(strtemp);
                }
                strpara = string.Format("paravalue_{0}", stpara[0].parasum + 1);
                strtemp = inifile.ReadValue(strsection, strpara, strinipath);
                stpara[0].paracheckvalue[stpara[0].parasum] = double.Parse(strtemp);
                paraname1.Text = strtempname[0] + ":" + stpara[0].paracheckvalue[0].ToString();
                paraname2.Text = strtempname[1] + ":" + stpara[0].paracheckvalue[1].ToString();
                paraname3.Text = strtempname[2] + ":" + stpara[0].paracheckvalue[2].ToString();
                paraname4.Text = strtempname[3] + ":" + stpara[0].paracheckvalue[3].ToString();
                paraname5.Text = strtempname[4] + ":" + stpara[0].paracheckvalue[4].ToString();
               
            }
            else
            {
                strsection = "8048M" + "_2";
                strtemp = inifile.ReadValue(strsection, "Paraname", strinipath);
                stpara[1].paraname = strtemp;
                string[] strtempname = strtemp.Split(',');
                strtemp = inifile.ReadValue(strsection, "Parasum", strinipath);
                stpara[1].parasum = int.Parse(strtemp);
                stpara[1].position = new int[stpara[1].parasum + 1];
                stpara[1].paracheckvalue = new double[stpara[1].parasum + 1];

                for (int n = 0; n < stpara[1].parasum; n++)
                {
                    strposition = string.Format("position_{0}", n + 1);
                    strtemp = inifile.ReadValue(strsection, strposition, strinipath);
                    stpara[1].position[n] = int.Parse(strtemp);
                    strpara = string.Format("paravalue_{0}", n + 1);
                    strtemp = inifile.ReadValue(strsection, strpara, strinipath);
                    stpara[1].paracheckvalue[n] = double.Parse(strtemp);
                }
                strpara = string.Format("paravalue_{0}", stpara[1].parasum + 1);
                strtemp = inifile.ReadValue(strsection, strpara, strinipath);
                stpara[1].paracheckvalue[stpara[1].parasum] = double.Parse(strtemp);
                paraname1.Text = strtempname[0] + ":" + stpara[1].paracheckvalue[0].ToString();
                paraname2.Text = strtempname[1] + ":" + stpara[1].paracheckvalue[1].ToString();
                paraname3.Text = strtempname[2] + ":" + stpara[1].paracheckvalue[2].ToString();
                paraname4.Text = strtempname[3] + ":" + stpara[1].paracheckvalue[3].ToString();
                paraname5.Text = strtempname[4] + ":" + stpara[1].paracheckvalue[4].ToString();
            }
           

            

            return true;
            /*
            strtemp =inifile.ReadValue("8048M", "MAX IL", strinipath);            
            stpara.dblmaxIL= double.Parse(strtemp);
          
            strtemp = inifile.ReadValue("8048M", "GD Ripple", strinipath);
            stpara.dblGDripple = double.Parse(strtemp);
            strtemp = inifile.ReadValue("8048M", "position_2", strinipath);
            stpara.position[1] = int.Parse(strtemp);
            strtemp = inifile.ReadValue("8048M", "GD Slope", strinipath);
            stpara.dblGDSlope = double.Parse(strtemp);
            strtemp = inifile.ReadValue("8048M", "position_3", strinipath);
            stpara.position[2] = int.Parse(strtemp);
            strtemp = inifile.ReadValue("8048M", "Worst CD", strinipath);
            stpara.dblCD = double.Parse(strtemp);
            strtemp = inifile.ReadValue("8048M", "position_4", strinipath);
            stpara.position[3] = int.Parse(strtemp);
            strtemp = inifile.ReadValue("8048M", "Ripple", strinipath);
            stpara.ripple = double.Parse(strtemp);
            */


        }
        private bool readtestfile()
        {
            //dblparametercheck

            string strtemp = "", strfilename = " ", strshow = "";
            string strinipath = "";
            Ini inifile = new Ini();
            strinipath = Application.StartupPath + "\\amalgamate.ini";
            strtemp = inifile.ReadValue("COER", "DCM_use", strinipath);
            stmateinfo.dcmuse = int.Parse(strtemp);
            strtemp = inifile.ReadValue("COER", "script_odd_path", strinipath);
            stmateinfo.scriptfileodd = strtemp;
            strtemp = inifile.ReadValue("COER", "script_even_path", strinipath);
            stmateinfo.scriptfileeven = strtemp;
            strtemp = inifile.ReadValue("COER", "coer_sum", strinipath);
            stmateinfo.coersum = int.Parse(strtemp);
            stmateinfo.coerpath = new string[stmateinfo.coersum];
            stmateinfo.coersn = new string[stmateinfo.coersum];
            stmateinfo.coerpathodd = new string[stmateinfo.coersum];
            stmateinfo.coerpatheven = new string[stmateinfo.coersum];
            stmateinfo.coerpathoddresult = new string[stmateinfo.coersum];
            stmateinfo.coerpathevenresult = new string[stmateinfo.coersum];
            string strsn = "";
            for (int n = 0; n < stmateinfo.coersum; n++)
            {
                strsn = string.Format("SN_{0}", n + 1);
                strtemp = inifile.ReadValue("COER", strsn, strinipath);
                stmateinfo.coersn[n] = strtemp;
                strsn = string.Format("PATH_{0}", n + 1);
                strtemp = inifile.ReadValue("COER", strsn, strinipath);
                stmateinfo.coerpath[n] = strtemp;
                strfilename = stmateinfo.coerpath[n] + "\\" + stmateinfo.coersn[n] + "=ODD.csv";
                stmateinfo.coerpathodd[n] = strfilename;
                if (!System.IO.File.Exists(strfilename))
                {
                    strshow = "未找到该文件:" + strfilename;
                    DialogResult r1 = MessageBox.Show(strshow, "标题", MessageBoxButtons.YesNoCancel);
                    return false;

                }
                strfilename = stmateinfo.coerpath[n] + "\\" + stmateinfo.coersn[n] + "=ODD-导表.csv";
                stmateinfo.coerpathoddresult[n] = strfilename;
                if (!System.IO.File.Exists(strfilename))
                {
                    strshow = "未找到该文件:" + strfilename;
                    DialogResult r1 = MessageBox.Show(strshow, "标题", MessageBoxButtons.YesNoCancel);
                    return false;

                }
                strfilename = stmateinfo.coerpath[n] + "\\" + stmateinfo.coersn[n] + "=EVEN.csv";
                stmateinfo.coerpatheven[n] = strfilename;
                if (!System.IO.File.Exists(strfilename))
                {
                    strshow = "未找到该文件:" + strfilename;
                    DialogResult r1 = MessageBox.Show(strshow, "标题", MessageBoxButtons.YesNoCancel);
                    return false;

                }
                strfilename = stmateinfo.coerpath[n] + "\\" + stmateinfo.coersn[n] + "=EVEN-导表.csv";
                stmateinfo.coerpathevenresult[n] = strfilename;
                if (!System.IO.File.Exists(strfilename))
                {
                    strshow = "未找到该文件:" + strfilename;
                    DialogResult r1 = MessageBox.Show(strshow, "标题", MessageBoxButtons.YesNoCancel);
                    return false;

                }
            }


            strtemp = inifile.ReadValue("DCM1", "dcm1_sum", strinipath);
            stmateinfo.dcm1sum = int.Parse(strtemp);
            stmateinfo.dcm1path = new string[stmateinfo.dcm1sum];
            stmateinfo.dcm1sn = new string[stmateinfo.dcm1sum];
            for (int n = 0; n < stmateinfo.coersum; n++)
            {
                strsn = string.Format("SN_{0}", n + 1);
                strtemp = inifile.ReadValue("DCM1", strsn, strinipath);
                stmateinfo.dcm1sn[n] = strtemp;
                strsn = string.Format("PATH_{0}", n + 1);
                strtemp = inifile.ReadValue("DCM1", strsn, strinipath);

                strfilename = strtemp + "\\" + stmateinfo.dcm1sn[n] + ".csv";
                stmateinfo.dcm1path[n] = strfilename;
                if (!System.IO.File.Exists(strfilename))
                {
                    strshow = "未找到该文件:" + strfilename;
                    DialogResult r1 = MessageBox.Show(strshow, "标题", MessageBoxButtons.YesNoCancel);
                    return false;

                }
            }

            strtemp = inifile.ReadValue("DCM2", "dcm2_sum", strinipath);
            stmateinfo.dcm2sum = int.Parse(strtemp);
            stmateinfo.dcm2path = new string[stmateinfo.dcm2sum];
            stmateinfo.dcm2sn = new string[stmateinfo.dcm2sum];
            for (int n = 0; n < stmateinfo.coersum; n++)
            {
                strsn = string.Format("SN_{0}", n + 1);
                strtemp = inifile.ReadValue("DCM2", strsn, strinipath);
                stmateinfo.dcm2sn[n] = strtemp;
                strsn = string.Format("PATH_{0}", n + 1);
                strtemp = inifile.ReadValue("DCM2", strsn, strinipath);

                strfilename = strtemp + "\\" + stmateinfo.dcm2sn[n] + ".csv";
                stmateinfo.dcm2path[n] = strfilename;
                if (!System.IO.File.Exists(strfilename))
                {
                    strshow = "未找到该文件:" + strfilename;
                    DialogResult r1 = MessageBox.Show(strshow, "标题", MessageBoxButtons.YesNoCancel);
                    return false;

                }
            }
            return true;



        }

        private void radioODD_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioEVEN_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

            if (!readinifile(1))
            {
                return;
            }
            breadpara = true;
        }

        private void para2_CheckedChanged(object sender, EventArgs e)
        {
            if (!readinifile(2))
            {
                return;
            }
            breadpara = true;
        }

        private bool anaylsedata()
        {
            breadfile = false;
            string strtemplet = m_strconfigpath + "\\CD_templet.csv";

            m_strresultfile = m_strtestfile.Substring(0, m_strtestfile.Length - 4) + "-导表.csv";

            if (System.IO.File.Exists(m_strresultfile))
            {
                // DialogResult r1 = MessageBox.Show("已经有该导表文件，是否覆盖", "标题", MessageBoxButtons.YesNoCancel);
                string strnewfilename = "";
                int nfileindex = 1;
                strnewfilename = string.Format("{0}_{1}.csv", m_strresultfile.PadLeft(m_strresultfile.Length - 4), nfileindex);
                while (System.IO.File.Exists(strnewfilename))
                {
                    nfileindex++;
                    strnewfilename = string.Format("{0}_{1}.csv", m_strresultfile.PadLeft(m_strresultfile.Length - 4), nfileindex);
                }
                System.IO.File.Copy(m_strresultfile, strnewfilename);
              //  if (r1.ToString().Equals("No"))
              //  {
              //      return false;
              //  }
            }
            string strResFile = m_strresultfile;
            int nFirstLine, nFirstCol;
            double dblFirstCh, dblLastCh;
            ArrayList alTotalRes = new ArrayList();
            ArrayList alTitle = new ArrayList();

            ArrayList alTitleparameter = new ArrayList();
            ArrayList alITUCF = new ArrayList();
            calcResult(alTotalRes, alTitle, alTitleparameter, alITUCF, out nFirstLine, out nFirstCol, out dblFirstCh, out dblLastCh);
            string strTemplateFile = strtemplet;
            int nRealLine = (int)(dblLastCh - dblFirstCh) + 1;
            if (nchspace < 100)
            {
                nRealLine = nRealLine * 2;
            }
            if (nchspace > 100)
            {
                nRealLine = nRealLine * 2 / 3 + 1;
            }
            if (nchspace > 150)
            {
                nRealLine = nRealLine / 2 + 1;
            }
            double dblBaseFreqC = (m_nCalGhz / 100 + dblLastCh) * 100;
            double dblBaseFreqH = dblBaseFreqC + nchstep;
            int nRealStartIndex = 0;
            for (nRealStartIndex = 0; nRealStartIndex < alITUCF.Count; nRealStartIndex++)
            {
                double dblCurCF = (double)alITUCF[nRealStartIndex];
                if ((Math.Abs(dblCurCF - dblBaseFreqC) > 20) && (Math.Abs(dblCurCF - dblBaseFreqH) > 20))
                    continue;

                break;
            }

            string strtempw = "", strtempwpara = "", strtempvalue = "";
            bool bwritefile = true;
            double minvalue = 1000;
            ArrayList alstrtempw = new ArrayList();
            for (int j = 0; j < ((double[])alTotalRes[0]).Length; j++)
            {

                if (j < nRealStartIndex)
                    continue;
                else if (j >= (nRealLine + nRealStartIndex))
                    break;
                else
                {
                    strtempw = "";
                    for (int i = 0; i < alTotalRes.Count; i++)
                    {

                        if (Math.Abs(((double[])alTotalRes[i])[j]) > Math.Abs(dblprarmax[i]))
                        {
                            dblprarmax[i] = ((double[])alTotalRes[i])[j];
                        }
                        strtempw += ((double[])alTotalRes[i])[j].ToString() + ",";
                        //curSheet.Cells[nFirstLine + j - nRealStartIndex, nFirstCol + i] = strCurVal;
                    }
                    alstrtempw.Add(strtempw);
                    //strresultfile.WriteLine(strtempw);
                }

            }
            for (int i = 0; i < alTitle.Count; i++)
            {
                minvalue = 1000;
                if (alTitle[i].ToString() == "MIN IL")
                {
                    for (int j = 0; j < ((double[])alTotalRes[i]).Length; j++)
                    {
                        if (j < nRealStartIndex)
                            continue;
                        else if (j >= (nRealLine + nRealStartIndex))
                            break;
                        if (Math.Abs(((double[])alTotalRes[i])[j]) < Math.Abs(minvalue))
                        {
                            minvalue = ((double[])alTotalRes[i])[j];
                        }

                    }
                    dblprarmax[i] = minvalue;
                }
                if (m_nCDP > 0)
                {
                    if (alTitle[i].ToString() == "Worst CD")
                    {
                        double dblcdsum = 0;
                        int nindex = 0;
                        for (int j = 0; j < ((double[])alTotalRes[i]).Length; j++)
                        {
                            if (j < nRealStartIndex)
                                continue;
                            else if (j >= (nRealLine + nRealStartIndex))
                                break;
                            //if (Math.Abs(((double[])alTotalRes[i])[j]) < Math.Abs(minvalue))
                            //{
                            //    minvalue = ((double[])alTotalRes[i])[j];
                            //}
                            dblcdsum += Math.Abs(((double[])alTotalRes[i])[j]);
                            nindex++;
                        }
                        dblprarmax[i] = dblcdsum / nindex;
                    }
                }
                if (alTitle[i].ToString() == "Adj Iso")
                {
                    for (int j = 0; j < ((double[])alTotalRes[i]).Length; j++)
                    {

                        if (j < nRealStartIndex)
                            continue;
                        else if (j >= (nRealLine + nRealStartIndex))
                            break;
                        if (Math.Abs(((double[])alTotalRes[i])[j]) < Math.Abs(minvalue))
                        {
                            minvalue = ((double[])alTotalRes[i])[j];
                        }

                    }
                    dblprarmax[i] = minvalue;
                }

            }

            strtempvalue = "MAX-Value,";
            for (int i = 1; i < alTitle.Count; i++)
            {
                strtempvalue += dblprarmax[i].ToString() + ",";
            }
            double dbltest = 0, dblcheck = 0;
            for (int n = 0; n < stpara[nuseini].parasum; n++)
            {
                dbltest = dblprarmax[stpara[nuseini].position[n]];
                dblcheck = stpara[nuseini].paracheckvalue[n];
                if (dbltest > dblcheck)
                {
                    bwritefile = false;
                }
            }
            //IL Ripple 为maxil-minil，maxil在第二个位置
            double dbilripple=0;
            dbilripple = dblprarmax[stpara[nuseini].position[1]] - dblprarmax[stpara[nuseini].position[1] + 1];
            dblcheck = stpara[nuseini].paracheckvalue[stpara[nuseini].parasum];
            if (dbilripple > dblcheck)
            {
                bwritefile = false;
            }

            if (bwritefile)
            {
                StreamWriter strresultfile = new StreamWriter(strResFile);
                strtempw = ""; strtempwpara = "";
                for (int i = 0; i < alTitle.Count; i++)
                {
                    strtempw += alTitle[i].ToString() + ",";
                    strtempwpara += alTitleparameter[i].ToString() + ",";
                }

                strresultfile.WriteLine(strtempw);
                strresultfile.WriteLine(strtempwpara);
                strresultfile.WriteLine(strtempvalue);
                strresultfile.WriteLine("");
                for (int i = 0; i < alstrtempw.Count; i++)
                {
                    strresultfile.WriteLine(alstrtempw[i]);
                }
                strresultfile.Close();
                return true;
            }

            return false;

        }
        private void button4_Click(object sender, EventArgs e)
        {

            openFileDialog2.ShowDialog();
            m_strscriptfile = openFileDialog2.FileName;
            lab_ini.Text = m_strscriptfile;
        }

        private void Readcfgfile()
        {
            string strfilename;
            strfilename = Application.StartupPath + "\\Temp_config.txt";
            StreamReader strcfg = new StreamReader(strfilename, System.Text.Encoding.GetEncoding("GBK"));

            string strcfgvalue = strcfg.ReadLine();
            string[] strtempcfg = strcfgvalue.Split(',');
            dblstartw = double.Parse(strtempcfg[1]);
            dblstopw = double.Parse(strtempcfg[2]);
            dblstep = double.Parse(strtempcfg[3]);
            dblrf = double.Parse(strtempcfg[4]);
            nIF = int.Parse(strtempcfg[5]);
            m_bPDL = Convert.ToBoolean(int.Parse(strtempcfg[6]));


            strcfgvalue = strcfg.ReadLine();
            strtempcfg = strcfgvalue.Split(',');
            m_strinputfile = strtempcfg[1];

            strcfgvalue = strcfg.ReadLine();
            strtempcfg = strcfgvalue.Split(',');
            m_stroutputfile = strtempcfg[1];
            m_strconfigpath = strtempcfg[2];
            int ntype = 0;
            strcfgvalue = strcfg.ReadLine();
            strtempcfg = strcfgvalue.Split(',');
            ntype = int.Parse(strtempcfg[1]);
            if (ntype > 2)
            {
                m_btypefour = true;

            }
            else
            {
                m_btypefour = false;
                radioMux.Enabled = false;
                radioDemux.Enabled = false;
            }

            //strcfgvalue = strcfg.ReadLine();
            //strtempcfg = strcfgvalue.Split(',');
            //m_strPN = strtempcfg[1];
            strcfg.Close();
        }
        private void initpara()
        {
            pdlaClient.MeasurementRange.XStart = dblstartw;
            pdlaClient.MeasurementRange.XStop = dblstopw;
            pdlaClient.Resolution.RFModulationFrequency = dblrf;
            pdlaClient.Resolution.Increment = dblstep;
            pdlaClient.Sensitivity.IFBandwidth = nIF;

            if (m_bPDL)
            {
                pdlaClient.DispersionMode = ODACommon.eDispersionMode.CD_PMD_Swept;
            }
            else
            {
                pdlaClient.DispersionMode = ODACommon.eDispersionMode.CD_Swept;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!bconnect)
                {
                    MessageBox.Show("未连接测试设备，请连接！");
                    return;
                }
                initpara();
                pdlaClient.GenerateMuellerAndPMDData = true;
                pdlaClient.Actions.Normalize(ODACommon.enumAcquisitionMode.eNormalization);
            }

            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }
        }
        private void Connect_Click()
        {
            try
            {
                pdlaClient.Connectivity.Connect(textBox_IP.Text);
                this.NewStatusHandler = new ODARemoting.NewStatusDelegate(this.NewStatusEvent);
                pdlaClient.NewStatus += this.NewStatusHandler;

                this.TriggerHandler = new ODARemoting.TriggerProgressDelegate(this.TriggerProgessEvent);
                pdlaClient.TriggerProgress += this.TriggerHandler;
            }

            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }
        }

        private void NewStatusEvent(string msg, ODACommon.eEventLogType e)
        {
            label_log.Text = msg;
            //MessageBox.Show("Instrument Connect Successful!");
        }
        public void getResultValue(double[] dblFreq1, double[] dblFreq2, double[] dblGD, double[] dblIL, double[] dblPhase, double[] dblCD)
        {
            Array.Clear(dblFreq1, 0, dblFreq1.Length);
            Array.Clear(dblFreq2, 0, dblFreq2.Length);
            Array.Clear(dblGD, 0, dblGD.Length);
            // Array.Clear(dblDGD, 0, dblDGD.Length);
            Array.Clear(dblIL, 0, dblIL.Length);
            Array.Clear(dblCD, 0, dblCD.Length);
            Array.Clear(dblPhase, 0, dblPhase.Length);
            Array.Clear(dblPMD, 0, dblPMD.Length);
            Array.Clear(dblPDL, 0, dblPDL.Length);

            double[] dblTempIL = pdlaClient.Results.YData(ODACommon.eMeasurementType.Gain, ODACommon.eODAPort.One);
            double[] dblTempGD = pdlaClient.Results.YData(ODACommon.eMeasurementType.GD, ODACommon.eODAPort.One);
            double[] dblTempPhase = pdlaClient.Results.YData(ODACommon.eMeasurementType.OptPhase, ODACommon.eODAPort.One);

            int nPoint1 = dblTempIL.Length;

            double[] dblTempCD = pdlaClient.Results.YData(ODACommon.eMeasurementType.CD, ODACommon.eODAPort.One);
            int nPoint2 = dblTempCD.Length;

            double xStart1 = pdlaClient.Results.XStart(ODACommon.eMeasurementType.Gain);
            double xStop1 = pdlaClient.Results.XStop(ODACommon.eMeasurementType.Gain);
            double xStart2 = pdlaClient.Results.XStart(ODACommon.eMeasurementType.CD);
            double xStop2 = pdlaClient.Results.XStop(ODACommon.eMeasurementType.CD);
            double xStep1 = (xStop1 - xStart1) / (nPoint1 - 1);
            double xStep2 = (xStop2 - xStart2) / (nPoint2 - 1);

            double dblCSpeed = 299792458.458;
            for (int i = 0; i < nPoint1; i++)
            {
                dblFreq1[i] = dblCSpeed / (xStart1 + i * xStep1);
                dblIL[i] = dblTempIL[i];
                dblGD[i] = dblTempGD[i];
                dblPhase[i] = dblTempPhase[i];
            }

            if (m_bPDL)
            {
                double[] dblTempPDL = pdlaClient.Results.YData(ODACommon.eMeasurementType.PDL, ODACommon.eODAPort.One);
                double[] dblTempPMD = pdlaClient.Results.YData(ODACommon.eMeasurementType.PMD, ODACommon.eODAPort.One);
                // double[] dblTempDGD = pdlaClient.Results.YData(ODACommon.eMeasurementType.DGD, ODACommon.eODAPort.One);

                for (int i = 0; i < nPoint1; i++)
                {
                    dblPDL[i] = dblTempPDL[i];
                    dblPMD[i] = dblTempPMD[i];
                    // dblDGD[i] = dblTempDGD[i];
                }
            }

            for (int i = 0; i < nPoint2; i++)
            {
                dblFreq2[i] = dblCSpeed / (xStart2 + i * xStep2);
                dblCD[i] = dblTempCD[i];
            }
        }
        private void savedata()
        {

            string filename = m_strtestfile;
            if (System.IO.File.Exists(m_strtestfile))
            {
                // DialogResult r1 = MessageBox.Show("已经有该文件，是否覆盖", "标题", MessageBoxButtons.YesNoCancel);
                string strnewfilename = "";
                int nfileindex = 1;
                strnewfilename = string.Format("{0}_{1}.csv", m_strtestfile.PadLeft(m_strtestfile.Length - 4), nfileindex);
                while (System.IO.File.Exists(strnewfilename))
                {
                    nfileindex++;
                    strnewfilename = string.Format("{0}_{1}.csv", m_strtestfile.PadLeft(m_strtestfile.Length - 4), nfileindex);
                }
                System.IO.File.Copy(m_strtestfile, strnewfilename);
                //if (r1.ToString() == "Yes")

                //{ MessageBox.Show("Yes"); }

               // if (r1.ToString().Equals("No"))

              //  { return; }

                //if (r1.ToString().Equals("Cancel"))

                //{ MessageBox.Show("Cancel"); }


            }

            StreamWriter swRes = new StreamWriter(filename);
            swRes.WriteLine("Freq,GD,IL/Gain,Phase,PDL,PMD,Freq,CD");

            int i = 0;
            for (i = 0; i < dblFreq2.Length; i++)
            {
                if (Math.Abs(dblFreq2[i]) < 100)
                    break;

                string strwrite = "";
                strwrite = dblFreq1[i].ToString() + ","
                + dblGD[i].ToString() + ","
                + dblIL[i].ToString() + ","
                + dblPhase[i].ToString() + ","
                + dblPDL[i].ToString() + ","
                + dblPMD[i].ToString() + ","
                + dblFreq2[i].ToString() + ","
                + dblCD[i].ToString();

                swRes.WriteLine(strwrite);
            }
            swRes.WriteLine(dblFreq1[i].ToString() + ","
                            + dblGD[i].ToString() + ","
                            + dblIL[i].ToString() + ","
                            + dblPhase[i].ToString() + ","
                            + dblPDL[i].ToString() + ","
                            + dblPMD[i].ToString() + ","
                            + ",");

            swRes.Close();
        }
        private void CDScan_Click(object sender, EventArgs e)
        {
            try
            {
                if (!bconnect)
                {
                    MessageBox.Show("未连接测试设备，请连接！");
                    return;
                }
                initpara();
                if (radioODD.Checked == true)
                {
                    if (m_btypefour)
                    {
                        if (radioDemux.Checked == true)
                        {
                            //m_strresultfile = m_strinputfile + "-Demux-ODD-导表.xls";
                            m_strtestfile = m_strinputfile + "-Demux-ODD.csv";
                        }
                        else
                        {
                            // m_strresultfile = m_strinputfile + "-Mux-ODD-导表.xls";
                            m_strtestfile = m_strinputfile + "-Mux-ODD.csv";
                        }
                    }
                    else
                    {
                        //m_strresultfile = m_strinputfile + "-ODD-导表.xls";
                        m_strtestfile = m_strinputfile + "-ODD.csv";
                    }
                    if (m_bPDL)
                    {
                        // m_strtempletfile = m_stroutputfile + "_templet_ODD_PDL.xls";
                        m_strscriptfile = m_stroutputfile + "_script_ODD_PDL.txt";
                    }
                    else
                    {
                        // m_strtempletfile = m_stroutputfile + "_templet_ODD_NOPDL.xls";
                        m_strscriptfile = m_stroutputfile + "_script_ODD_NOPDL.txt";
                    }
                }
                else
                {
                    if (m_btypefour)
                    {
                        if (radioDemux.Checked == true)
                        {
                            //m_strresultfile = m_strinputfile + "-Demux-EVEN-导表.xls";
                            m_strtestfile = m_strinputfile + "-Demux-EVEN.csv";
                        }
                        else
                        {
                            //m_strresultfile = m_strinputfile + "-Mux-EVEN-导表.xls";
                            m_strtestfile = m_strinputfile + "-Mux-EVEN.csv";
                        }
                    }
                    else
                    {
                        // m_strresultfile = m_strinputfile + "-EVEN-导表.xls";
                        m_strtestfile = m_strinputfile + "-EVEN.csv";
                    }
                    if (m_bPDL)
                    {
                        // m_strtempletfile = m_stroutputfile + "_templet_EVEN_PDL.xls";
                        m_strscriptfile = m_stroutputfile + "_script_EVEN_PDL.txt";
                    }
                    else
                    {
                        // m_strtempletfile = m_stroutputfile + "_templet_EVEN_NOPDL.xls";
                        m_strscriptfile = m_stroutputfile + "_script_EVEN_NOPDL.txt";
                    }
                }
                string strpath = Application.StartupPath + "\\Temp_testfile.txt";
                StreamWriter swRes = new StreamWriter(strpath, false, Encoding.GetEncoding("gb2312"));
                swRes.WriteLine(m_strtestfile);
                swRes.Close();


                if (m_bPDL)
                {
                    pdlaClient.DispersionMode = (ODACommon.eDispersionMode.CD_PMD_Swept);
                }
                else
                {
                    pdlaClient.DispersionMode = ODACommon.eDispersionMode.CD_Swept;
                }
                pdlaClient.Measure();
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void outputresult_Click(object sender, EventArgs e)
        {
            breadfile = true;
            for (int m = 0; m < 30; m++)
            {
                dblprarmax[m] = 0;
            }
           
            if (radioODD.Checked == true)
            {
                if (m_btypefour)
                {
                    if (radioDemux.Checked == true)
                    {
                        m_strresultfile = m_strinputfile + "-Demux-ODD-导表.csv";
                        m_strtestfile = m_strinputfile + "-Demux-ODD.csv";
                    }
                    else
                    {
                        m_strresultfile = m_strinputfile + "-Mux-ODD-导表.csv";
                        m_strtestfile = m_strinputfile + "-Mux-ODD.csv";
                    }
                }
                else
                {
                    m_strresultfile = m_strinputfile + "-ODD-导表.csv";
                    m_strtestfile = m_strinputfile + "-ODD.csv";
                }
                if (m_bPDL)
                {

                    m_strscriptfile = m_stroutputfile + "_script_ODD_PDL.txt";
                }
                else
                {

                    m_strscriptfile = m_stroutputfile + "_script_ODD_NOPDL.txt";
                }
            }
            else
            {
                if (m_btypefour)
                {
                    if (radioDemux.Checked == true)
                    {
                        m_strresultfile = m_strinputfile + "-Demux-EVEN-导表.csv";
                        m_strtestfile = m_strinputfile + "-Demux-EVEN.csv";
                    }
                    else
                    {
                        m_strresultfile = m_strinputfile + "-Mux-EVEN-导表.csv";
                        m_strtestfile = m_strinputfile + "-Mux-EVEN.csv";
                    }
                }
                else
                {
                    m_strresultfile = m_strinputfile + "-EVEN-导表.csv";
                    m_strtestfile = m_strinputfile + "-EVEN.csv";
                }
                if (m_bPDL)
                {

                    m_strscriptfile = m_stroutputfile + "_script_EVEN_PDL.txt";
                }
                else
                {

                    m_strscriptfile = m_stroutputfile + "_script_EVEN_NOPDL.txt";
                }
            }
            
            string strpath = Application.StartupPath + "\\Temp_testfile.txt";
            StreamWriter swRes = new StreamWriter(strpath, false, Encoding.GetEncoding("gb2312"));
            swRes.WriteLine(m_strtestfile);
            swRes.Close();
          
            string strtemplet = m_strconfigpath + "\\CD_templet.csv";
            if (System.IO.File.Exists(m_strscriptfile))
            {

            }
            else
            {
                MessageBox.Show("没有找到脚本文件，请找工程师处理！");
                return;
            }
          
            m_strPNini = m_stroutputfile + ".ini";
          
            if (System.IO.File.Exists(m_strPNini))
            {
                string strsection = "";
                string strtemp = "";
                Ini inifile = new Ini();
                //读取ini文件获取参数
                strsection = "CalGhz";
              
                strtemp = inifile.ReadValue(strsection, "Vaue", m_strPNini);
               
                if (int.TryParse(strtemp, out int calValue))
                {
                    m_nCalGhz = calValue;
                }
               
            }
            else
            {
                m_nCalGhz = 190000;

            }
            m_nCalGhz = 184000;

            if (System.IO.File.Exists(m_strtestfile))
            {

            }
            else
            {
                MessageBox.Show("没有找到测试的文件，请确定测试完成或找工程师处理！");
                return;
            }

            //if (System.IO.File.Exists(m_strresultfile))
            //{

            //    string strnewfilename="";
            //    int nfileindex =1;
            //    strnewfilename=string.Format("{0}_{1}.csv", m_strresultfile.PadLeft(m_strresultfile.Length - 4), nfileindex);
            //    while (System.IO.File.Exists(strnewfilename))
            //    {
            //        nfileindex++;
            //        strnewfilename=string.Format("{0}_{1}.csv", m_strresultfile.PadLeft(m_strresultfile.Length - 4), nfileindex);
            //    }
            //    System.IO.File.Copy(m_strresultfile, strnewfilename);




            //}
           // m_strresultfile = "C:\\Public-T\\GitLab\\WS02\\ITL\\sw2227_ITL_FTS_CD\\src\\CD_Scan\\result.csv";
            string strResFile = m_strresultfile;
            int nFirstLine, nFirstCol;
            double dblFirstCh, dblLastCh;
            ArrayList alTotalRes = new ArrayList();
            ArrayList alTitle = new ArrayList();

            ArrayList alTitleparameter = new ArrayList();
            ArrayList alITUCF = new ArrayList();
            calcResult(alTotalRes, alTitle, alTitleparameter, alITUCF, out nFirstLine, out nFirstCol, out dblFirstCh, out dblLastCh);

            //  Excel.Application excel = new Excel.ApplicationClass();
            try
            {
                string strTemplateFile = strtemplet;

                // initial data preparing
                int nRealLine = (int)(dblLastCh - dblFirstCh) + 1;
                if (nchspace < 100)
                {
                    nRealLine = nRealLine * 2;
                }
                if (nchspace > 100)
                {
                    nRealLine = nRealLine * 2 / 3 + 1;
                }
                if (nchspace > 150)
                {
                    nRealLine = nRealLine / 2 + 1;
                }
                //202606
               // double dblBaseFreqC = (1900 + dblLastCh) * 100;
                double dblBaseFreqC = (m_nCalGhz/100 + dblLastCh) * 100;
                //modify by chaoli;
                //double dblBaseFreqH = dblBaseFreqC + 50;
                double dblBaseFreqH = dblBaseFreqC + nchstep;

                // double dblBaseFreqH = dblBaseFreqC + double.Parse(textBox_chspace.Text);

               
                int nRealStartIndex = 0;
                for (nRealStartIndex = 0; nRealStartIndex < alITUCF.Count; nRealStartIndex++)
                {
                    double dblCurCF = (double)alITUCF[nRealStartIndex];
                    if ((Math.Abs(dblCurCF - dblBaseFreqC) > 20) && (Math.Abs(dblCurCF - dblBaseFreqH) > 20))
                        continue;

                    break;
                }
                StreamWriter strresultfile = new StreamWriter(strResFile);
                string strtempw = "", strtempwpara = "", strtempvalue = "";
                for (int i = 0; i < alTitle.Count; i++)
                {
                    strtempw += alTitle[i].ToString() + ",";
                    strtempwpara += alTitleparameter[i].ToString() + ",";


                }

                strresultfile.WriteLine(strtempw);
                strresultfile.WriteLine(strtempwpara);
                double minvalue = 1000;
                ArrayList alstrtempw = new ArrayList();
                for (int j = 0; j < ((double[])alTotalRes[0]).Length; j++)
                {

                    if (j < nRealStartIndex)
                        continue;
                    else if (j >= (nRealLine + nRealStartIndex))
                        break;
                    else
                    {
                        strtempw = "";
                        for (int i = 0; i < alTotalRes.Count; i++)
                        {

                            if (Math.Abs(((double[])alTotalRes[i])[j]) > Math.Abs(dblprarmax[i]))
                            {
                                dblprarmax[i] = ((double[])alTotalRes[i])[j];
                            }
                            strtempw += ((double[])alTotalRes[i])[j].ToString() + ",";
                            //curSheet.Cells[nFirstLine + j - nRealStartIndex, nFirstCol + i] = strCurVal;
                        }
                        alstrtempw.Add(strtempw);
                        //strresultfile.WriteLine(strtempw);
                    }

                }
                for (int i = 0; i < alTitle.Count; i++)
                {
                    minvalue = 1000;
                    if (alTitle[i].ToString() == "MIN IL")
                    {
                        for (int j = 0; j < ((double[])alTotalRes[i]).Length; j++)
                        {
                            if (j < nRealStartIndex)
                                continue;
                            else if (j >= (nRealLine + nRealStartIndex))
                                break;
                            if (Math.Abs(((double[])alTotalRes[i])[j]) < Math.Abs(minvalue))
                            {
                                minvalue = ((double[])alTotalRes[i])[j];
                            }

                        }
                        dblprarmax[i] = minvalue;
                    }
                    if (m_nCDP > 0)
                    {
                        if (alTitle[i].ToString() == "Worst CD")
                        {
                            double dblcdsum = 0;
                            int nindex = 0;
                            for (int j = 0; j < ((double[])alTotalRes[i]).Length; j++)
                            {
                                if (j < nRealStartIndex)
                                    continue;
                                else if (j >= (nRealLine + nRealStartIndex))
                                    break;
                              
                                dblcdsum += Math.Abs(((double[])alTotalRes[i])[j]);
                                nindex++;
                            }
                            dblprarmax[i] = dblcdsum / nindex;
                        }
                    }
                    if (alTitle[i].ToString() == "Adj Iso")
                    {
                        for (int j = 0; j < ((double[])alTotalRes[i]).Length; j++)
                        {

                            if (j < nRealStartIndex)
                                continue;
                            else if (j >= (nRealLine + nRealStartIndex))
                                break;
                            if (Math.Abs(((double[])alTotalRes[i])[j]) < Math.Abs(minvalue))
                            {
                                minvalue = ((double[])alTotalRes[i])[j];
                            }

                        }
                        dblprarmax[i] = minvalue;
                    }

                }

                strtempvalue = "MAX-Value,";
                for (int i = 1; i < alTitle.Count; i++)
                {
                    strtempvalue += dblprarmax[i].ToString() + ",";
                }
                strresultfile.WriteLine(strtempvalue);
                strresultfile.WriteLine("");
                for (int i = 0; i < alstrtempw.Count; i++)
                {
                    strresultfile.WriteLine(alstrtempw[i]);
                }
                strresultfile.Close();
       
                label_result.Text = "导表完成！";

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show("导表完成");

        }
        private void calcResult(ArrayList alRes, ArrayList alTitle, ArrayList alTitleparameter, ArrayList alITUCF, out int nLine, out int nCol, out double nFirst, out double nLast)
        {
            nLine = nCol = -1;
            nFirst = nLast = -1;
            try
            {
                // 1. get file data

                if (breadfile)
                {
                    Data.getvaluefromfile(m_strtestfile);
                }



                // 2. get script file

                ILCalc ResbyIL = new ILCalc();
                double[] dblITUCF;
                // 2.5 get ITU CF 


                // 3. parse script and calculate
                StreamReader swScript = new StreamReader(m_strscriptfile);

                int nLineIndex = 0;

                alRes.Clear();
                while (swScript.EndOfStream != true)
                {
                    string strCurLine = swScript.ReadLine();
                    string[] strArray = strCurLine.Split(',');

                    if (nLineIndex == 0)
                    {
                        if (strArray.Length < 2)
                        {
                            throw new Exception("脚本文件第" + nLineIndex.ToString() + "行不正确!");
                        }
                        nLine = int.Parse(strArray[0]);
                        nCol = int.Parse(strArray[1]);
                    }
                    else if (nLineIndex == 1)
                    {
                        if (strArray.Length < 2)
                        {
                            throw new Exception("脚本文件第" + nLineIndex.ToString() + "行不正确!");
                        }
                        else
                        {
                            nFirst = double.Parse(strArray[0]);
                            nLast = double.Parse(strArray[1]);
                            nchspace = int.Parse(strArray[2]);
                            nchstep = nchspace / 2;
                            int nmove = int.Parse(strArray[3]);
                            if (nmove > 0)
                            {
                                if (radioDemux.Checked == true)
                                {
                                    nFirst = double.Parse(strArray[4]);
                                    nLast = double.Parse(strArray[5]);
                                }
                            }
                            //202606
                            // ResbyIL.m_nChFirst = 190000 + nLast * 100;
                            ResbyIL.m_nChFirst = m_nCalGhz + nLast * 100;                 
                        }
                    }
                    else
                    {
                        ResbyIL.m_nChSpace = nchspace;

                        ResbyIL.CalcITUCF(out dblITUCF);
                        alITUCF.Clear();
                        for (int i = 0; i < dblITUCF.Length; i++)
                        {
                            //   alITUCF.Add(dblITUCF);
                            alITUCF.Add(dblITUCF[i]);
                        }
                        alTitle.Add(strArray[0]);
                        if (strArray.Length < 6)
                        {
                            throw new Exception("脚本文件第" + nLineIndex.ToString() + "行不正确!");
                        }

                        switch (strArray[0])
                        {
                            case "CCF":
                                double dblPassband = double.Parse(strArray[2]);
                                double dblDepth = double.Parse(strArray[3]);
                                int nFitOrder = -1;
                                if (strArray[5] != "")
                                {
                                    nFitOrder = int.Parse(strArray[5]);
                                    if (nFitOrder != 1)
                                    {
                                        MessageBox.Show("CCF只支持两点直线拟合!");
                                        break;
                                    }
                                }
                                double[] dblCCF;
                                ResbyIL.CalcCCF(dblPassband, dblDepth, nFitOrder, out dblCCF);
                                //for(int n=0;n<dblCCF.Length;n++)
                                //{
                                //    if (Math.Abs(dblCCF[n] )>Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblCCF[n];
                                //    }

                                //}
                                alRes.Add(dblCCF);
                                alTitleparameter.Add("GHz");
                                break;
                            case "ITU Shift":
                                dblPassband = double.Parse(strArray[2]);
                                dblDepth = double.Parse(strArray[3]);
                                nFitOrder = -1;
                                if (strArray[5] != "")
                                {
                                    nFitOrder = int.Parse(strArray[5]);
                                    if (nFitOrder != 1)
                                    {
                                        MessageBox.Show("ITU Shift只支持两点直线拟合!");
                                        break;
                                    }
                                }
                                ResbyIL.CalcCCF(dblPassband, dblDepth, nFitOrder, out dblCCF);
                                //double[] dblITUCF;
                                Array.Clear(dblITUCF, 0, dblITUCF.Length);
                                ResbyIL.CalcITUCF(out dblITUCF);
                                double[] dblShift = new double[dblCCF.Length];
                                for (int i = 0; i < dblShift.Length; i++)
                                {
                                    dblShift[i] = dblCCF[i] - dblITUCF[i];

                                }
                                //for (int n = 0; n < dblShift.Length; n++)
                                //{
                                //    if (Math.Abs(dblShift[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblShift[n];
                                //    }

                                //}
                                alRes.Add(dblShift);
                                alTitleparameter.Add("GHz");
                                break;
                            case "MAX IL":
                                double dblCFPb = -1, dblCFDep = -1;
                                if (strArray[1] != "ITU")
                                {
                                    string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                    if (strTemp.Length < 2)
                                    {
                                        MessageBox.Show("MAX IL 脚本段落有问题!");
                                        break;
                                    }

                                    dblCFPb = double.Parse(strTemp[1]);
                                    dblCFDep = double.Parse(strTemp[2]);
                                }
                                dblPassband = double.Parse(strArray[2]);

                                double[] dblMaxIL;
                                ResbyIL.CalcMaxIL(dblPassband, dblCFPb, dblCFDep, out dblMaxIL);
                                alRes.Add(dblMaxIL);
                                //for (int n = 0; n < dblMaxIL.Length; n++)
                                //{
                                //    if (Math.Abs(dblMaxIL[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblMaxIL[n];
                                //    }

                                //}
                                alTitleparameter.Add("dB");
                                break;
                            case "MIN IL":
                                dblCFPb = -1;
                                dblCFDep = -1;
                                if (strArray[1] != "ITU")
                                {
                                    string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                    if (strTemp.Length < 2)
                                    {
                                        MessageBox.Show("MIN IL 脚本段落有问题!");
                                        break;
                                    }

                                    dblCFPb = double.Parse(strTemp[1]);
                                    dblCFDep = double.Parse(strTemp[2]);
                                }
                                dblPassband = double.Parse(strArray[2]);

                                double[] dblMinIL;
                                ResbyIL.CalcMinIL(dblPassband, dblCFPb, dblCFDep, out dblMinIL);
                                alRes.Add(dblMinIL);
                                //for (int n = 0; n < dblMinIL.Length; n++)
                                //{
                                //    if (Math.Abs(dblMinIL[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblMinIL[n];
                                //    }

                                //}
                                alTitleparameter.Add("dB");
                                break;
                            case "Ripple":
                                dblCFPb = -1;
                                dblCFDep = -1;
                                if (strArray[1] != "ITU")
                                {
                                    string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                    if (strTemp.Length < 2)
                                    {
                                        MessageBox.Show("Ripple IL 脚本段落有问题!");
                                        break;
                                    }

                                    dblCFPb = double.Parse(strTemp[1]);
                                    dblCFDep = double.Parse(strTemp[2]);
                                }
                                dblPassband = double.Parse(strArray[2]);

                                ResbyIL.CalcMinIL(dblPassband, dblCFPb, dblCFDep, out dblMinIL);
                                ResbyIL.CalcMaxIL(dblPassband, dblCFPb, dblCFDep, out dblMaxIL);
                                double[] dblRipple = new double[dblMinIL.Length];
                                for (int i = 0; i < dblRipple.Length; i++)
                                {
                                    dblRipple[i] = dblMaxIL[i] - dblMinIL[i];
                                }
                                alRes.Add(dblRipple);
                                //for (int n = 0; n < dblRipple.Length; n++)
                                //{
                                //    if (Math.Abs(dblRipple[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblRipple[n];
                                //    }

                                //}
                                alTitleparameter.Add("dB");
                                break;
                            case "PDL":
                                dblCFPb = -1;
                                dblCFDep = -1;
                                if (strArray[1] != "ITU")
                                {
                                    string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                    if (strTemp.Length < 2)
                                    {
                                        MessageBox.Show("PDL 脚本段落有问题!");
                                        break;
                                    }

                                    dblCFPb = double.Parse(strTemp[1]);
                                    dblCFDep = double.Parse(strTemp[2]);
                                }
                                dblPassband = double.Parse(strArray[2]);

                                double[] dblMaxPDL;
                                ResbyIL.CalcPDL(dblPassband, dblCFPb, dblCFDep, out dblMaxPDL);
                                alRes.Add(dblMaxPDL);
                                //for (int n = 0; n < dblMaxPDL.Length; n++)
                                //{
                                //    if (Math.Abs(dblMaxPDL[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblMaxPDL[n];
                                //    }

                                //}
                                alTitleparameter.Add("dB");
                                break;
                            case "Adj Iso":
                                dblCFPb = -1;
                                dblCFDep = -1;
                                if (strArray[1] != "ITU")
                                {
                                    string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                    if (strTemp.Length < 2)
                                    {
                                        MessageBox.Show("Adj Iso 脚本段落有问题!");
                                        break;
                                    }

                                    dblCFPb = double.Parse(strTemp[1]);
                                    dblCFDep = double.Parse(strTemp[2]);
                                }

                                dblPassband = double.Parse(strArray[2]);
                                alTitleparameter.Add(dblPassband);
                                double[] dblIso;
                                ResbyIL.CalcIso(dblPassband, dblCFPb, dblCFDep, out dblIso);
                                alRes.Add(dblIso);
                                //for (int n = 0; n < dblIso.Length; n++)
                                //{
                                //    if (Math.Abs(dblIso[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblIso[n];
                                //    }

                                //}
                                break;
                            case "Worst CD":
                                dblCFPb = -1;
                                dblCFDep = -1;
                                if (strArray[1] != "ITU")
                                {
                                    string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                    if (strTemp.Length < 2)
                                    {
                                        MessageBox.Show("Worst CD 脚本段落有问题!");
                                        break;
                                    }

                                    dblCFPb = double.Parse(strTemp[1]);
                                    dblCFDep = double.Parse(strTemp[2]);
                                }

                                dblPassband = double.Parse(strArray[2]);
                                alTitleparameter.Add(dblPassband);
                                if (strArray[3] != "")
                                {
                                    m_nCDP = int.Parse(strArray[3]);
                                }

                                nFitOrder = -1;
                                if (strArray[5] != "")
                                {
                                    nFitOrder = int.Parse(strArray[5]);
                                    if (nFitOrder > 9)
                                    {
                                        MessageBox.Show("CD最大拟合阶数只能到九！");
                                        break;
                                    }
                                }

                                double[] dblWstCD;
                                ResbyIL.CalcWorstCD(dblPassband, dblCFPb, dblCFDep, nFitOrder, out dblWstCD);
                                alRes.Add(dblWstCD);
                                //for (int n = 0; n < dblWstCD.Length; n++)
                                //{
                                //    if (Math.Abs(dblWstCD[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblWstCD[n];
                                //    }

                                //}
                                break;
                            case "GD Ripple":
                                dblCFPb = -1;
                                dblCFDep = -1;
                                if (strArray[1] != "ITU")
                                {
                                    string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                    if (strTemp.Length < 2)
                                    {
                                        MessageBox.Show("Center GD 脚本段落有问题!");
                                        break;
                                    }

                                    dblCFPb = double.Parse(strTemp[1]);
                                    dblCFDep = double.Parse(strTemp[2]);
                                }

                                dblPassband = double.Parse(strArray[2]);
                                alTitleparameter.Add(dblPassband);
                                nFitOrder = -1;
                                if (strArray[5] != "")
                                {
                                    nFitOrder = int.Parse(strArray[5]);
                                    if (nFitOrder > 9)
                                    {
                                        MessageBox.Show("GD最大拟合阶数只能到九！");
                                        break;
                                    }
                                }

                                int nTypeIndex = int.Parse(strArray[6]);

                                double[] dblGDRipple;
                                ResbyIL.CalcGD(dblPassband, dblCFPb, dblCFDep, nFitOrder, nTypeIndex, out dblGDRipple);
                                alRes.Add(dblGDRipple);
                                //for (int n = 0; n < dblGDRipple.Length; n++)
                                //{
                                //    if (Math.Abs(dblGDRipple[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblGDRipple[n];
                                //    }

                                //}
                                break;
                            case "GD Slope":
                                dblCFPb = -1;
                                dblCFDep = -1;
                                if (strArray[1] != "ITU")
                                {
                                    string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                    if (strTemp.Length < 2)
                                    {
                                        MessageBox.Show("Center GD 脚本段落有问题!");
                                        break;
                                    }

                                    dblCFPb = double.Parse(strTemp[1]);
                                    dblCFDep = double.Parse(strTemp[2]);
                                }

                                dblPassband = double.Parse(strArray[2]);
                                alTitleparameter.Add(dblPassband);
                                double[] dblGDSlope;
                                ResbyIL.CalcGDSlope(dblPassband, dblCFPb, dblCFDep, out dblGDSlope);
                                alRes.Add(dblGDSlope);
                                //for (int n = 0; n < dblGDSlope.Length; n++)
                                //{
                                //    if (Math.Abs(dblGDSlope[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblGDSlope[n];
                                //    }

                                //}
                                break;
                            case "Passband":
                                dblCFPb = -1;
                                dblCFDep = -1;
                                if (strArray[1] != "ITU")
                                {
                                    string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                    if (strTemp.Length < 2)
                                    {
                                        MessageBox.Show("Center GD 脚本段落有问题!");
                                        break;
                                    }

                                    dblCFPb = double.Parse(strTemp[1]);
                                    dblCFDep = double.Parse(strTemp[2]);
                                }

                                dblPassband = double.Parse(strArray[2]);
                                alTitleparameter.Add(dblPassband);
                                dblDepth = double.Parse(strArray[3]);

                                bool bCleanPb = false;
                                if (strArray[4] == "clean")
                                    bCleanPb = true;

                                nFitOrder = -1;
                                if (strArray[5] != "")
                                {
                                    nFitOrder = int.Parse(strArray[5]);
                                    if (nFitOrder > 9)
                                    {
                                        MessageBox.Show("GD最大拟合阶数只能到九！");
                                        break;
                                    }
                                }

                                double[] dblPb;
                                ResbyIL.CalcPb(dblPassband, dblDepth, dblCFPb, dblCFDep, nFitOrder, bCleanPb, out dblPb);
                                alRes.Add(dblPb);
                                //for (int n = 0; n < dblPb.Length; n++)
                                //{
                                //    if (Math.Abs(dblPb[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblPb[n];
                                //    }

                                //}
                                break;
                            case "Stopband":
                                dblCFPb = -1;
                                dblCFDep = -1;
                                if (strArray[1] != "ITU")
                                {
                                    string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                    if (strTemp.Length < 2)
                                    {
                                        MessageBox.Show("Center GD 脚本段落有问题!");
                                        break;
                                    }

                                    dblCFPb = double.Parse(strTemp[1]);
                                    dblCFDep = double.Parse(strTemp[2]);
                                }

                                dblPassband = double.Parse(strArray[2]);
                                alTitleparameter.Add(dblPassband);
                                dblDepth = double.Parse(strArray[3]);

                                nFitOrder = -1;
                                if (strArray[5] != "")
                                {
                                    nFitOrder = int.Parse(strArray[5]);
                                    if (nFitOrder > 9)
                                    {
                                        MessageBox.Show("Stopband计算只支持直线拟合！");
                                        break;
                                    }
                                }

                                double[] dblStopband;
                                ResbyIL.CalcStopband(dblPassband, dblDepth, dblCFPb, dblCFDep, nFitOrder, out dblStopband);
                                alRes.Add(dblStopband);
                                //for (int n = 0; n < dblStopband.Length; n++)
                                //{
                                //    if (Math.Abs(dblStopband[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblStopband[n];
                                //    }

                                //}
                                break;
                            case "PMD":
                                dblCFPb = -1;
                                dblCFDep = -1;
                                if (strArray[1] != "ITU")
                                {
                                    string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                    if (strTemp.Length < 2)
                                    {
                                        MessageBox.Show("MAX IL 脚本段落有问题!");
                                        break;
                                    }

                                    dblCFPb = double.Parse(strTemp[1]);
                                    dblCFDep = double.Parse(strTemp[2]);
                                }
                                dblPassband = double.Parse(strArray[2]);
                                alTitleparameter.Add(dblPassband);
                                double[] dblMaxPMD;
                                ResbyIL.CalcMaxPMD(dblPassband, dblCFPb, dblCFDep, out dblMaxPMD);
                                alRes.Add(dblMaxPMD);
                                //for (int n = 0; n < dblMaxPMD.Length; n++)
                                //{
                                //    if (Math.Abs(dblMaxPMD[n]) > Math.Abs(dblprarmax[nLineIndex - 2]))
                                //    {
                                //        dblprarmax[nLineIndex - 2] = dblMaxPMD[n];
                                //    }

                                //}
                                break;
                                //case "DGD":
                                //    dblCFPb = -1;
                                //    dblCFDep = -1;
                                //    if (strArray[1] != "ITU")
                                //    {
                                //        string[] strTemp = strArray[1].Split(new char[3] { '(', ')', ':' });
                                //        if (strTemp.Length < 2)
                                //        {
                                //            MessageBox.Show("DGD 脚本段落有问题!");
                                //            break;
                                //        }

                                //        dblCFPb = double.Parse(strTemp[1]);
                                //        dblCFDep = double.Parse(strTemp[2]);
                                //    }
                                //    dblPassband = double.Parse(strArray[2]);

                                //    double[] dblMaxDGD;
                                //    ResbyIL.CalcDGD(dblPassband, dblCFPb, dblCFDep, out dblMaxDGD);
                                //    alRes.Add(dblMaxDGD);
                                //    break;
                        }
                    }

                    nLineIndex++;
                }

                swScript.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            writeCDfile();
            Connect_Click();

            bconnect = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
