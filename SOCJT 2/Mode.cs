﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Mode
    {
        #region properties
        private double nModeOmega;
        public double modeOmega
        {
            get { return nModeOmega; }
            set { nModeOmega = value; }
        }//end property modeOmega

        private double nD;
        public double D
        {
            get { return nD; }
            set { nD = value; }
        }//end property D

        private double nK;
        public double K
        {
            get { return nK; }
            set { nK = value; }
        }//end property K

        private double nwExe;
        public double wExe
        {
            get { return nwExe; }
            set { nwExe = value; }
        }//end property wExe

        private int nmodeVMax;
        public int modeVMax
        {
            get { return nmodeVMax; }
            set { nmodeVMax = value; }
        }//end property modeVMax

        private bool nfitOmega;
        public bool fitOmega
        {
            get { return nfitOmega; }
            set { nfitOmega = value; }
        }//end property nfitOmega

        private bool nfitD;
        public bool fitD
        {
            get { return nfitD; }
            set { nfitD = value; }
        }//end property nfitD

        private bool nfitK;
        public bool fitK
        {
            get { return nfitK; }
            set { nfitK = value; }
        }//end property nfitK

        private bool nfitWEXE;
        public bool fitWEXE
        {
            get { return nfitWEXE; }
            set { nfitWEXE = value; }
        }//end property nfitWEXE

        private bool nIsAType;
        public bool IsAType
        {
            get { return nIsAType; }
            set { nIsAType = value; }
        }//end property nIsAType

        private double nmodeAOmega;
        public double modeAOmega
        {
            get { return nmodeAOmega; }
            set { nmodeAOmega = value; }
        }//end property modeAOmega

        private double nmodeZeta;
        public double modeZeta
        {
            get { return nmodeZeta; }
            set { nmodeZeta = value; }
        }//end property modeZeta
        #endregion properties

        public void setMode(Mode thisMode, int modeN, string[] inputF)
        {
            int whatMode = -1;
            for (int i = 0; i < inputF.Length; i++)
            {
                if (inputF[i] == "&MODE_INFO")
                {
                    whatMode++;
                }//end check to see if it's the right mode
                if (whatMode == modeN)
                {
                    //moved this stuff to the initialization loop to see if that helps
                    /*
                    thisMode.fitOmega = false;
                    thisMode.fitD = false;
                    thisMode.fitK = false;
                    thisMode.fitWEXE = false;
                    thisMode.IsAType = false;
                    */

                    for (int u = i; ; u++)
                    {
                        if (inputF[u] == "MODEOMEGA")
                        {
                            thisMode.modeOmega = Convert.ToDouble(inputF[u + 1]);
                            continue;
                        }
                        if (inputF[u] == "MODED")
                        {
                            thisMode.D = Convert.ToDouble(inputF[u + 1]);
                            continue;
                        }
                        if (inputF[u] == "MODEK")
                        {
                            thisMode.K = Convert.ToDouble(inputF[u + 1]);
                            continue;
                        }
                        if (inputF[u] == "MODEWEXE")
                        {
                            thisMode.wExe = Convert.ToDouble(inputF[u + 1]);
                            continue;
                        }
                        if (inputF[u] == "MODEVMAX")
                        {
                            thisMode.modeVMax = Convert.ToInt32(inputF[u + 1]);
                            continue;
                        }
                        if (inputF[u] == "MODEA_OMEGA")
                        {
                            thisMode.modeAOmega = Convert.ToDouble(inputF[u + 1]);
                            continue;
                        }
                        if (inputF[u] == "MODEZETA")
                        {
                            thisMode.modeZeta = Convert.ToDouble(inputF[u + 1]);
                        }
                        if (inputF[u] == "FIT_OMEGA")
                        {
                            if (inputF[u + 1].ToUpper() == "T" || inputF[u + 1].ToUpper() == "TRUE")
                            {
                                thisMode.fitOmega = true;
                            }
                        }
                        if (inputF[u] == "FIT_D")
                        {
                            if (inputF[u + 1].ToUpper() == "T" || inputF[u + 1].ToUpper() == "TRUE")
                            {
                                thisMode.fitD = true;
                            }
                        }
                        if (inputF[u] == "FIT_K")
                        {
                            if (inputF[u + 1].ToUpper() == "T" || inputF[u + 1].ToUpper() == "TRUE")
                            {
                                thisMode.fitK = true;
                            }
                            continue;
                        }
                        if (inputF[u] == "FIT_WEXE")
                        {
                            if (inputF[u + 1].ToUpper() == "T" || inputF[u + 1].ToUpper() == "TRUE")
                            {
                                thisMode.fitWEXE = true;
                            }
                            continue;
                        }
                        if (inputF[u].ToUpper() == "ISATYPE")
                        {
                            if (inputF[u + 1].ToUpper() == "T" || inputF[u + 1].ToUpper() == "TRUE")
                            {
                                thisMode.IsAType = true;
                            }
                            continue;
                        }
                        if (inputF[u] == "/")
                        {
                            break;
                        }
                    }//end u for loop
                }//end if
            }//end for
        }//end method setMode
    }//end class Mode
}
