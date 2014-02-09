﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    static class OutputFile
    {
        public static List<string> makeOutput(FileInfo input, List<double[,]> zMatrices, alglib.sparsematrix[] sHamMatrix, List<List<BasisFunction>> jBasisVecsByJ, List<double[]> eigenvalues, bool isQuad, Eigenvalue[] finalList, int[] IECODE, int[] ITER)
        {
            List<string> linesToWrite = new List<string>();
            StringBuilder file = new StringBuilder();
            decimal J = 0.5M;
            if (input.minJBool == true)
            {
                J = input.minJ;
            }                                                                                                                            
            decimal Sigma = input.S * -1M;
            file.AppendLine(" ");
            file.AppendLine("Time Report:");
            file.AppendLine("Matrix generation took " + String.Format("{0,10:0.00}", input.matGenTime) + " seconds.");
            file.AppendLine("The Lanczos routines took " + String.Format("{0,10:0.00}", input.diagTime) + " seconds.");
            file.AppendLine(" ");
            for (int i = 0; i < eigenvalues.Count; i++)
            {
                //add some asterisks here to make J and S stand out
                file.AppendLine("J = " + Convert.ToString(J));
                file.AppendLine("Sigma = " + String.Format("{0,3:0.0}", Sigma));// + Convert.ToString(Sigma));
                //add some asterisks


                file.AppendLine("  ");
                if (eigenvalues[i] == null || eigenvalues[i].Length == 0)
                {
                    file.AppendLine("Too small of a basis set to calculate these eigenvalues.");
                    linesToWrite.Add(file.ToString());
                    file.Clear();
                    J++;
                    continue;
                }
                double[] tempEvs = eigenvalues[i].ToArray();
                for (int j = 0; j < tempEvs.Length; j++)
                { 
                    file.AppendLine("\t" + String.Format("{0,10:0.0000}", tempEvs[j]));//  Convert.ToString(tempEvs[j]));
                }//this for loop writes the eigenvalues for each J level

                double[,] tempMat = zMatrices[i];

                int numRows = sHamMatrix[i].innerobj.m;
                file.AppendLine(" ");
                file.AppendLine("Number of basis functions: " + Convert.ToString(numRows));
                file.AppendLine("Number of non-zero matrix elements: " + Convert.ToString(sHamMatrix[i].innerobj.vals.Length));

                file.AppendLine(" ");
                switch (IECODE[i])
                { 
                    case 0:
                        file.AppendLine("Lanczos routine successfully completed");
                        break;
                    case 7:
                        file.AppendLine("Max number of iterations in Lanczos routine was exceeded.");
                        file.AppendLine("Not all eigenvalues have converged.");
                        break;
                    case 99:
                        break;
                }
                file.AppendLine("Lanczos routine took " + Convert.ToString(ITER[i]) + " iterations to complete.");
                file.AppendLine(" ");

                if (input.pVector == true)
                {
                    #region PrintVector
                    for (int j = 0; j < tempEvs.Length; j++)
                    {
                        file.AppendLine(" " + "\r");
                        file.AppendLine("Eigenvalue" + "\t" + Convert.ToString(j + 1) + " = " + String.Format("{0,10:0.0000}", tempEvs[j]));
                        file.AppendLine(" " + "\r");
                        file.AppendLine("Eigenvector: (Only vectors with coefficients larger than " + Convert.ToString(input.evMin) + " are shown)");
                        file.AppendLine(" ");

                        vecBuilder(input, jBasisVecsByJ[i], file, tempMat, j, input.evMin);
                    }
                    #endregion
                }

                if (input.printBasis == true)
                {
                    #region PrintBasis
                    file.AppendLine("\t" + "\r");
                    file.Append("Basis Fxn #" + "\t");
                    for (int h = 0; h < input.nModes; h++)
                    {
                        file.Append("v(" + Convert.ToString(h + 1) + ")" + "\t" + "l(" + Convert.ToString(h + 1) + ")" + "\t");
                    }
                    file.Append("lambda" + "\t" + "Sigma");
                    for (int j = 0; j < jBasisVecsByJ[i].Count; j++)//goes through basis vectors
                    {
                        file.AppendLine("\t");
                        file.Append("\t" + Convert.ToString(j + 1));
                        for(int m = 0; m < input.nModes; m++)//goes through each mode
                        {
                            file.Append("\t" + "  " + Convert.ToString(jBasisVecsByJ[i][j].modesInVec[m].v) + "\t" + String.Format("{0,3}", jBasisVecsByJ[i][j].modesInVec[m].l));
                        }
                        file.Append("\t" + String.Format("{0,4}", jBasisVecsByJ[i][j].Lambda) + "\t" + String.Format("{0,3:0.0}", Sigma));
                    }
                    #endregion
                }//end print Basis code

                if (input.pMatrix == true)
                {
                    #region PrintMatrixSparse
                    int nonZeroCount = 0;
                    //int upDiag = 0;
                    //int numRows = hamMatrix[i].GetLength(1);
                    //int numRows = sHamMatrix[i].innerobj.m;
                    file.AppendLine("\t");
                    file.AppendLine("\r");
                    file.AppendLine("\t" + "Hamiltonian Matrix");
                    file.AppendLine("\t" + "Only upper triangle given");
                    file.AppendLine("Row" + "\t" + "Column" + "\t" + "Value");
                    int T0 = 0;
                    int T1 = 0;
                    int sRow = 0;
                    int sCol = 0;
                    double V = 0.0;
                    for (; ; )
                    {
                        if (alglib.sparseenumerate(sHamMatrix[i], ref T0, ref T1, out sRow, out sCol, out V) == false)
                        {
                            break;
                        }
                        else 
                        {
                            file.AppendLine(Convert.ToString(sRow + 1) + "\t" + Convert.ToString(sCol + 1) + "\t" + String.Format("{0,10:0.0000}", V));
                            nonZeroCount++;
                        }
                    }
                    file.AppendLine("    " + Convert.ToString(nonZeroCount) + " non-zero matrix elements");
                    #endregion
                }

                file.AppendLine("**********************************************************************");
                file.AppendLine(" ");

                linesToWrite.Add(file.ToString());
                file.Clear();
                if (input.inclSO == true)
                {
                    if (Sigma < input.S)
                    {
                        Sigma++;
                    }
                    else
                    {
                        Sigma = input.S * -1M;
                        J++;
                    }
                }
                else
                {
                    J++;
                }
            }//writes all evs to the file object
            
            file.AppendLine("#" + "\t" + "Final results showing all eigenvalues found");
            file.AppendLine("\t" + "Eigenvalue" + "\t" + " j" + "\t" + "Sigma" + "\t" + "n_j" + "\t" + (input.blockLanczos ? "Symm" : input.pVector ? "Symm" : ""));
            int l = 0;
            for (int i = 0; i < finalList.Length; i++)
            {
                file.AppendLine(Convert.ToString(l + 1) + "\t" + String.Format("{0,9:0.0000}", finalList[i].Ev) + "\t" + Convert.ToString(finalList[i].pJ) + "\t" + String.Format("{0,3:0.0}", finalList[i].Sig) + "\t" + Convert.ToString(finalList[i].nJ) + "\t" + (input.blockLanczos ? (finalList[i].isa1 ? "1" : "2") : input.pVector ? (finalList[i].isa1 ? "1" : "2") : ""));
                l++;
            }
            linesToWrite.Add(file.ToString());
            
            return linesToWrite;
        }//end method makeOutput

        /// <summary>
        /// Appends eigenvector information to supplied StringBuilder for a given eigenvector.
        /// </summary>
        /// <param name="input">
        /// FileInfo object
        /// </param>
        /// <param name="jBasisVecsByJ">
        /// List of basis functions for this j block
        /// </param>
        /// <param name="file">
        /// Stringbuilder object to which eigenvector information will be appended
        /// </param>
        /// <param name="tempMat">
        /// Matrix containing the eigenvectors in the columns
        /// </param>
        /// <param name="j">
        /// Which eigenvector is being looked at
        /// </param>
        public static void vecBuilder(FileInfo input, List<BasisFunction> jBasisVecsByJ, StringBuilder file, double[,] tempMat, int j, double evMin, bool overRide = false)
        {
            bool a1 = SOCJT.isA(jBasisVecsByJ, tempMat, j, input, overRide);
            if (a1)
            {
                file.AppendLine("Vector is Type 1");
            }
            else
            {
                file.AppendLine("Vector is Type 2");
            }
            file.AppendLine(" ");

            file.Append("Coefficient" + "\t");
            for (int h = 0; h < input.nModes; h++)
            {
                file.Append("v(" + Convert.ToString(h + 1) + ")" + "\t" + "l(" + Convert.ToString(h + 1) + ")" + "\t");
            }
            file.Append("lambda");
            for (int h = 0; h < jBasisVecsByJ.Count; h++)//goes through basis vectors
            {
                if (tempMat[h, j] > evMin || tempMat[h, j] < -1.0 * evMin)
                {
                    SOCJT.writeVec(tempMat[h, j], jBasisVecsByJ[h], file);
                    /*
                    file.AppendLine("\t");
                    file.Append(String.Format("{0,10:0.000000}", tempMat[h, j]));
                    for (int m = 0; m < input.nModes; m++)//goes through each mode
                    {
                        file.Append("\t" + "  " + Convert.ToString(jBasisVecsByJ[h].modesInVec[m].v) + "\t" + String.Format("{0,3}", jBasisVecsByJ[h].modesInVec[m].l));//  "  " + Convert.ToString(jBasisVecsByJ[i][h].modesInVec[m].l));
                    }
                    file.Append("\t" + String.Format("{0,4}", jBasisVecsByJ[h].Lambda));
                    */
                }
            }
            file.AppendLine("\r");
        }

        /// <summary>
        /// Creates an input file to go at the start of the output files.
        /// </summary>
        /// <param name="input">
        /// FileInfo object
        /// </param>
        /// <param name="Modes">
        /// List of modes to be included in the input file generated.
        /// </param>
        /// <returns>
        /// List containing all lines of input file.
        /// </returns>
        public static List<string> inputFileMaker(FileInfo input, List<ModeInfo> Modes)
        {
            List<string> linesToWrite = new List<string>();
            StringBuilder file = new StringBuilder();
            file.AppendLine("&GENERAL");
            file.AppendLine("TITLE" + " = " + input.title);
            file.AppendLine("NMODES" + " = " + String.Format("{0,4}", input.nModes));
            file.AppendLine("S" + " = " + Convert.ToString(input.S));
            file.AppendLine("AZETA" + " = " + String.Format("{0,10:0.00000}", input.Azeta));
            file.AppendLine("FIT_AZETA" + " = " + Convert.ToString(input.fitAzeta));
            if (input.minJBool == true)
            {
                file.AppendLine("MINJ" + " = " + Convert.ToString(input.minJ));
            }
            file.AppendLine("MAXJ" + " = " + Convert.ToString(input.maxJ));
            file.AppendLine("FIT_ORIGIN = " + Convert.ToString(input.fitOrigin));
            file.AppendLine("ORIGIN = " + Convert.ToString(input.origin));
            file.AppendLine("CALC_DERIV" + " = " + Convert.ToString(input.calcDeriv));
            //file.AppendLine("ZETAE" + " = " + String.Format("{0,10:0.00000}", input.zetaE));
            file.AppendLine("USE_KAPPA_ETA = " + Convert.ToString(input.useKappaEta));
            file.AppendLine("S1" + " = " + Convert.ToString(input.S1));
            file.AppendLine("S2" + " = " + Convert.ToString(input.S2));
            file.AppendLine("/");
            file.AppendLine("  ");
                        
            for (int i = 0; i < Modes.Count; i++)
            {
                file.AppendLine("&MODE_INFO");
                file.AppendLine("MODEVMAX" + " = " + Convert.ToString(Modes[i].modeVMax));
                file.AppendLine("MODEOMEGA" + " = " + String.Format("{0,10:0.00000}", Modes[i].modeOmega));
                file.AppendLine("MODEWEXE" + " = " + String.Format("{0,10:0.00000}", Modes[i].wExe));
                if (input.useKappaEta)
                {
                    file.AppendLine("MODED" + " = " + String.Format("{0,10:0.00000}", Modes[i].D));
                    file.AppendLine("MODEK" + " = " + String.Format("{0,10:0.00000}", Modes[i].K));
                    file.AppendLine("KAPPA = " + String.Format("{0,10:0.00000}", Modes[i].kappa));                    
                    file.AppendLine("ETA = " + String.Format("{0,10:0.00000}", Modes[i].eta));
                    file.AppendLine("FIT_KAPPA = " + Convert.ToString(Modes[i].fitKappa));
                    file.AppendLine("FIT_ETA = " + Convert.ToString(Modes[i].fitEta));                    
                }
                else
                {
                    file.AppendLine("MODED" + " = " + String.Format("{0,10:0.00000}", Modes[i].D));                    
                    file.AppendLine("MODEK" + " = " + String.Format("{0,10:0.00000}", Modes[i].K));
                    file.AppendLine("FIT_D" + " = " + Convert.ToString(Modes[i].fitD));
                    file.AppendLine("FIT_K" + " = " + Convert.ToString(Modes[i].fitK));                    
                }
                file.AppendLine("FIT_OMEGA" + " = " + Convert.ToString(Modes[i].fitOmega));
                file.AppendLine("FIT_WEXE" + " = " + Convert.ToString(Modes[i].fitWEXE));   
                file.AppendLine("MODEZETA" + " = " + String.Format("{0,10:0.00000}", Modes[i].modeZeta));
                file.AppendLine("MODEA_OMEGA" + " = " + String.Format("{0,10:0.00000}", Modes[i].modeAOmega));
                file.AppendLine("ISATYPE" + " = " + Convert.ToString(Modes[i].IsAType));
                file.AppendLine("/");
                file.AppendLine("  ");
            }

            file.AppendLine("&SOLVE_INFO");
            file.AppendLine("BLOCK_LANCZOS = " + Convert.ToString(input.blockLanczos));
            file.AppendLine("M" + " = " + Convert.ToString(input.M));
            file.AppendLine("K_FACTOR" + " = " + Convert.ToString(input.kFactor));
            file.AppendLine("NOITS" + " = " + Convert.ToString(input.noIts));
            file.AppendLine("TOL" + " = " + Convert.ToString(input.tol));
            file.AppendLine("PARVEC = " + Convert.ToString(input.parVec));
            file.AppendLine("PARMAT = " + Convert.ToString(input.parMat));
            file.AppendLine("PARJ = " + Convert.ToString(input.parJ));
            file.AppendLine("/");
            file.AppendLine("  ");

            file.AppendLine("&IO_INFO");
            file.AppendLine("PRINT_BASIS" + " = " + Convert.ToString(input.printBasis));
            file.AppendLine("PRINT_MATRIX" + " = " + Convert.ToString(input.pMatrix));
            file.AppendLine("PRINT_VEC" + " = " + Convert.ToString(input.pVector));
            file.AppendLine("VEC_FILE = " + Convert.ToString(input.vecFile));
            file.AppendLine("VEC_FILE_COMPLETE = " + Convert.ToString(input.vecFileComplete));
            file.AppendLine("USE_MATRIX_FILE = " + Convert.ToString(input.useMatFile));
            file.AppendLine("MATRIX_FILE = " + input.matFile);
            file.AppendLine("EV_MIN = " + Convert.ToString(input.evMin));
            file.AppendLine("/");
            file.AppendLine("  ");

            file.AppendLine("&FIT_INFO");
            file.AppendLine("FITFILE" + " = " + input.fitFile);
            file.AppendLine("FTOL" + " = " + Convert.ToString(input.fTol));
            file.AppendLine("XTOL" + " = " + Convert.ToString(input.xTol));
            file.AppendLine("GTOL" + " = " + Convert.ToString(input.gTol));
            file.AppendLine("MAXFEV" + " = " + Convert.ToString(input.maxFev));
            file.AppendLine("FACTOR" + " = " + Convert.ToString(input.factor));
            //file.AppendLine("NPRINT" + " = " + Convert.ToString(input.print));
            file.AppendLine("/");
            file.AppendLine("  ");

            if (input.includeCrossTerms == true)
            {
                file.AppendLine("&CROSS_TERMS");
                //file.AppendLine("INCLUDE" + " = " + Convert.ToString(input.includeCrossTerms));
                for (int i = 0; i < input.nModes; i++)
                {
                    for (int j = 0; j < input.nModes; j++)
                    {
                        if (input.crossTermMatrix[i, j] != 0D || input.crossTermFit[i, j] == true)
                        {
                            if (i < j)
                            {
                                file.AppendLine("JT MODE " + Convert.ToString(i + 1) + " MODE " + Convert.ToString(j + 1) + " = " + String.Format("{0,10:0.00000}", input.crossTermMatrix[i, j]));
                                file.AppendLine("FIT" + " = " + Convert.ToString(input.crossTermFit[i, j]));
                            }
                        }
                    }
                }
                file.AppendLine("/");
                file.AppendLine("  ");
            }

            linesToWrite.Add(file.ToString());
            return linesToWrite;
        }

        public static List<string> ScanOutput(List<Eigenvalue[]> allOut)
        {
            List<string> linesToWrite = new List<string>();
            StringBuilder file = new StringBuilder();

            file.AppendLine("Final Scan Results:");
            file.AppendLine(" ");

            decimal jMax = 0.5M;
            for (int i = 0; i < allOut[0].Length; i++)
            {
                if (allOut[0][i].pJ > jMax)
                {
                    jMax = allOut[0][i].pJ;
                }//end if
            }            

            for (decimal i = 0.5M; i <= jMax; i++)
            {
                file.AppendLine("Results for j = " + Convert.ToString(i));
                file.AppendLine(" ");
                for (int j = 0; j < allOut.Count; j++)
                {
                    file.AppendLine(" ");
                    for (int k = 0; k < allOut[j].Length; k++)
                    {
                        if (allOut[j][k].pJ == i)
                        {
                            file.Append(Convert.ToString(allOut[j][k].Ev) + "\t");
                        }
                    }
                }
                file.AppendLine(" ");
                file.AppendLine(" ");
                //write out all eigenvalues for a given j
            }

            linesToWrite.Add(file.ToString());
            return linesToWrite;
        }//end method ScanOutput

        /// <summary>
        /// Writes all off-diagonal matrices to a matrix file for future use.
        /// </summary>
        /// <param name="input">
        /// FileInfo object containing matrix file name/path.
        /// </param>
        public static void writeMatFile(FileInfo input)
        { 
            //writes only the off-diagonal matrices to the file.\
            StringBuilder file = new StringBuilder();
            for (int i = 0; i < SOCJT.fitHamList.Count; i++)
            {
                file.AppendLine("List " + i);
                file.AppendLine(Convert.ToString(SOCJT.fitHamList[i][0].innerobj.m));
                file.AppendLine(" ");
                for (int j = 1; j < SOCJT.fitHamList[i].Count; j++)
                {
                    file.AppendLine("Matrix " + j);
                    file.AppendLine(" ");
                    int m;
                    int n;
                    double oldVal;
                    int t0 = 0;
                    int t1 = 0;
                    while (alglib.sparseenumerate(SOCJT.fitHamList[i][j], ref t0, ref t1, out m, out n, out oldVal))
                    {
                        file.AppendLine("\t" + Convert.ToString(n) + "\t" + Convert.ToString(m) + "\t" + Convert.ToString(oldVal));
                    }
                    file.AppendLine(" ");
                }//end loop j
            }//end loop i
            List<string> linesToWrite = new List<string>();
            linesToWrite.Add(file.ToString());
            File.WriteAllLines(input.matFilePath, linesToWrite);
        }//end method writeMatFile
    }//end class OutputFile    
}
