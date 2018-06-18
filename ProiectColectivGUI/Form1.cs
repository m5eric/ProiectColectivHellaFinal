using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProiectColectivGUI
{

    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }

        static string tipVarDarian = "TIP_VAR_NOT_GIVEN", tipVar2Darian = "TIP_VAR_2_NOT_GIVEN", functieReadHoza = "FUNCTIE_READ_NOT_GIVEN";
        static string[] parametriiFaraValori = new string[10];
        static string[] valoriParametriiFaraValori = new string[10];
        static int indexValoriParametriiFaraValori = 0;
        static int indexParametriiFaraValori = 0;
        static string filePath, fileName, DBCFile, RteVdaFile;
        OpenFileDialog ofd = new OpenFileDialog();
        bool DBCOpen = false, RteOpen = false, inputOpen = false;

        public static class InvalidDataError
        {
            public static void InterfataInvalidDataError(ArrayList define, ArrayList lista, String conditie_din_functie, String outputPath)
            {
                Form InvalidDataError = new Form()
                {
                    Width = 500,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = "InvalidDataError",
                    StartPosition = FormStartPosition.CenterParent,
                    AutoSize = true,
                };

                Label evLabel = new Label() { Left = 20, Text = "Ev", AutoSize = true };
                TextBox evTextBox = new TextBox() { Left = 20, Top = 20, Width = 400 };

                Label preconditionLabel = new Label() { Left = 20, Top = 50 ,Text = "Precondition", AutoSize = true };
                TextBox preconditionTextBox = new TextBox() { Left = 20, Top = 70, Width = 400 };

                Label variablesLabel = new Label() { Left = 20, Top = 100, Text = "Tipul Variabilelor: ", AutoSize = true };
                RichTextBox richTextBox = new RichTextBox() { Left = 20, Top = 130, Width = 400 };

                Button generare = new Button() { Left = 20, Top = 250, Text = "OK" , Width = 100};

                InvalidDataError.Controls.Add(evLabel);
                InvalidDataError.Controls.Add(evTextBox);

                InvalidDataError.Controls.Add(preconditionLabel);
                InvalidDataError.Controls.Add(preconditionTextBox);

                InvalidDataError.Controls.Add(variablesLabel);
                InvalidDataError.Controls.Add(richTextBox);

                foreach(String it in lista)
                {
                    richTextBox.AppendText(it);

                }

                InvalidDataError.Controls.Add(generare);

                InvalidDataError.AcceptButton = generare;

                generare.Click += (sender, e) =>
                {
                    String ev = evTextBox.Text;
                    String precondition = preconditionTextBox.Text;

                    String richTextBoxParametrii = "";
                    richTextBoxParametrii += richTextBox.Lines[0];
                    for (int i = 1; i < define.Count; i++)
                    {
                        richTextBoxParametrii += ", " + richTextBox.Lines[i];
                    }
                    CreateFileOutput(outputPath, ev, precondition, define, richTextBoxParametrii, conditie_din_functie);
                    InvalidDataError.Close();
                };


                InvalidDataError.Show();
            }
        }

        public static class ParametriiPrompt
        {
            public static string ShowDialog(string text, string caption, string[] parameters)
            {
                Form ParametriiForm = new Form()
                {
                    Width = 500,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterParent,
                    AutoSize = true,
                };
                Label label = new Label() {Text = text, AutoSize= true };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
                Button confirm = new Button() { Text = "Confirm", Left = 350, Width = 100, Top = 100, DialogResult = DialogResult.OK };
                ListBox listBox = new ListBox() { Left = 50, Top = 70, AutoSize = true};

                for(int i = 0; i < parameters.Length ; i++)
                {
                    if(parameters[i] != null)
                    {
                        listBox.Items.Add(parameters[i]);
                    }
                    
                }


                listBox.MouseDoubleClick += (sender2, e2) =>
                {
                    if (listBox.SelectedItem != null)
                    {
                        Form inserareValoareParametru = new Form()
                        {
                            AutoSize = true
                        };
                        TextBox textBox1 = new TextBox() {Top = 10 };
                        Button buttonAddVal = new Button() {Top = 30 ,Text = "AddVal", DialogResult = DialogResult.OK};
                        
                        
                        buttonAddVal.Click += (sender1, e1) => {
                            Console.WriteLine("++++++++++++++++++=>" + textBox1.Text);
                            valoriParametriiFaraValori[indexValoriParametriiFaraValori++] = textBox1.Text;
                            inserareValoareParametru.Close();
                        };

                        inserareValoareParametru.Controls.Add(textBox1);
                        inserareValoareParametru.Controls.Add(buttonAddVal);
                        inserareValoareParametru.AcceptButton = buttonAddVal;

                        inserareValoareParametru.ShowDialog();
                    }
                };


                confirm.Click += (sender, e) => { ParametriiForm.Close();
                    Console.WriteLine("+++++++++++++=> valoriParamFaraVal");
                    for (int i = 0; i < valoriParametriiFaraValori.Length; i++)
                    {
                        if (valoriParametriiFaraValori[i] != null)
                        {
                            Console.WriteLine(i + " " + valoriParametriiFaraValori[i]);
                        }
                    }
                };
                ParametriiForm.Controls.Add(label);
                ParametriiForm.Controls.Add(confirm);
                ParametriiForm.Controls.Add(listBox);
                ParametriiForm.AcceptButton = confirm;

                

                return ParametriiForm.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }

        private void DBCButton_Click(object sender, EventArgs e)
        {
            String openFile;
            ofd.Filter = "dbcFile|*.dbc";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                openFile = ofd.FileName;
                DBCLabel.Text = ofd.SafeFileName;
                DBCOpen = true;
                DBCFile = ofd.FileName;
            }
        }

        private void Rte_VdaButton_Click(object sender, EventArgs e)
        {
            String openFile;
            ofd.Filter = "vdaFile|*.h";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                openFile = ofd.FileName;
                Rte_VdaLabel.Text = ofd.SafeFileName;
                RteOpen = true;
                RteVdaFile = ofd.FileName;
            }
        }

        private void FileInputButton_Click(object sender, EventArgs e)
        {
            String openFile;
            ofd.Filter = "txtFile|*.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                openFile = ofd.FileName;
                FileInputLabel.Text = ofd.SafeFileName;

                filePath = ofd.FileName;
                fileName = ofd.SafeFileName;
                inputOpen = true;
            }
        }


        ///Darian
      //the code for XML File
        static public string xmlVariableCheck(string fileToRead, string cuvantCautat)
        {
            try
            {
                string ok = "null";
                string textFromTheFile = System.IO.File.ReadAllText(fileToRead); //variabila care stocheaza continutul fisierului 

                if (textFromTheFile.Contains(cuvantCautat))
                {
                    Console.WriteLine("Stringul cautat de dvs. ( \"{0}\" ) a fost gasit! (return true)", cuvantCautat);
                    ok = regexFunctieXML(fileToRead, cuvantCautat);
                }
                else
                {
                    Console.WriteLine("Stringul cautat de dvs. ( \"{0}\" ) nu a fost gasit! (return false)", cuvantCautat);
                }
                return ok;
                //Console.WriteLine("Fisierul contine : {0}", textFromTheFile);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private void infoButton_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Pentru File input exista doua tipuri de fisiere de input: pentru Error handling si Signal Processing. \n     ►Pentru Error handling:se va introduce doar pentru o singura eroare. \n     ►Pentru Signal processing:se va introduce doar pentru un singur semnal.\n\n !!! In cazul in care ati intampinat o eroare va trebui sa introduceti in campul Ev numele erorii.\nIn campul Precondition, daca vor fi preconditii de introdus, veti introduce preconditiile respective, altfel veti introduce tot timpul true.", "Ghid de utilizare");
        }

        static public string regexFunctieXML(string fileToRead, string cuvantCautat)
        {
            string linie;
            var gasit = false;
            Regex expresieRegulata = new Regex(@"DataTypes\/([a-zA-Z]*[0-9]*)\<");

            System.IO.StreamReader file = new System.IO.StreamReader(fileToRead);

            while ((linie = file.ReadLine()) != null)
            {
                if (linie.Contains(cuvantCautat))
                {
                    gasit = true;
                }

                if (gasit == true)
                {
                    Match match = expresieRegulata.Match(linie);
                    if (match.Success)
                    {
                        Console.WriteLine(match.Groups[1].Value);
                        return match.Groups[1].Value;
                    }
                }
            }
            file.Close();
            return null;
        }







        //the code for DBC 
        static public string dbcVariableCheck(string fileToRead, string cuvantCautat)
        {
            try
            {
                string ok = "null";
                string textFromTheFile = System.IO.File.ReadAllText(fileToRead); //variabila care stocheaza continutul fisierului 

                if (textFromTheFile.Contains(cuvantCautat))
                {
                    Console.WriteLine("Stringul cautat de dvs. ( \"{0}\" ) a fost gasit! (return true)", cuvantCautat);
                    ok = regexFunctieDBC(fileToRead, cuvantCautat);
                }
                else
                {
                    Console.WriteLine("Stringul cautat de dvs. ( \"{0}\" ) nu a fost gasit! (return false)", cuvantCautat);

                }
                return ok;

                //Console.WriteLine("Fisierul contine : {0}", textFromTheFile);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("return false");
                return null;
            }
        }

        static public string regexFunctieDBC(string fileToRead, string cuvantDeCautat)
        {
            string textFromTheFile = System.IO.File.ReadAllText(fileToRead);

            //stringul format din cuvantul pe care trebuie sa il cautam si expresia regulata
            string cuvantDeCautatPlusRegex = cuvantDeCautat + @"\s:\s[0-9]+\W([0-9]+)@";

            var match = Regex.Match(textFromTheFile, cuvantDeCautatPlusRegex);

            if (match.Success)
            {
                Console.WriteLine("Numarul de biți este {0}", match.Groups[1].Value);

                int nrBiti = Convert.ToInt32(match.Groups[1].Value);

                switch (nrBiti)
                {
                    case int n when (n >= 1 && n <= 8):
                        Console.WriteLine("Tipul de date este unsigned char.");
                        return "uint8";
                    case int n when (n >= 9 && n <= 16):
                        Console.WriteLine("Tipul de date este unsigned short.");
                        return "uint16";
                    case int n when (n >= 17 && n <= 32):
                        Console.WriteLine("Tipul de date este unsigned int");
                        return "uint32";
                    default:
                        Console.WriteLine("Stringul \"{0}\" nu a fost gasit in fisier.", cuvantDeCautat);
                        return null;
                }
            }

            return "NOT FOUND";

        }

        //the code for the final function
        public static string typeOfInputFile(string path, string searchedWord)
        {
            string extensia = Path.GetExtension(path);
            switch (extensia)
            {
                case ".dbc":
                    return dbcVariableCheck(path, searchedWord);
                case ".xml":
                    return xmlVariableCheck(path, searchedWord);
                default:
                    return null;
            }
        }

        ///End Darian


        ///Hoza
        public static String Cautare(string adresaFisier, string stringul)
        {
            String[] cuv = System.IO.File.ReadAllLines(adresaFisier);

            String pattern = @"\s*#\s*define\s*(\w*)\s*(\w*)\s*";

            foreach (string it in cuv)
            {
                Match match = Regex.Match(it, pattern, RegexOptions.IgnoreCase);
                if (it.Contains(stringul) && it.Contains("Rte_Read") && match.Success)
                {
                    String rezultat = match.Groups[1].Value;
                    //Console.WriteLine("." + rezultat + ".");
                    return rezultat;
                }
            }
            return "STRING NOT FOUND";
        }


        ///EndHoza


        //Functia de generare a fisierului de output pt InvalidDataError
        public static void CreateFileOutput(string outputPath, string Ev, string precondition, ArrayList define, String richTextBoxParametrii, String conditia_din_functie)
        {

            String outputFile = outputPath + "\\output1.c";
            //System.IO.File.WriteAllText(outputFile, outputFile);
            File.WriteAllText(outputFile, "");
            foreach (String it in define)
            {
                System.IO.File.AppendAllText(outputFile, it);
            }

            File.AppendAllText(outputFile, "\n");


            String functie1 =
                        "static void Vda_Ist_CheckInvalidDataError_ESP_21(" + richTextBoxParametrii + ")\n" +
                        "{\n" +
                            "boolean isInvalidData;\n" +
                            "if (" + conditia_din_functie + ")\n" +
                            "{\n" +
                                "isInvalidData = TRUE;\n" +
                            "}\n" +
                            "else\n" +
                            "{\n" +
                                "isInvalidData = FALSE;\n" +
                            "}\n" +
                            "Vda_Ist_ProcessInvalidDataError(" + Ev + ", " + precondition + ", isInvalidData);\n" +
                        "}\n\n";

            File.AppendAllText(outputFile, functie1);

            String functie2 =
                        "static void Vda_Ist_ProcessInvalidDataError(EvHnd_Event_t EvHnd_Event_en, boolean isPreconditionFulfilledAndSignalUpdated, boolean isInvalidData)\n" +
                        "{\n" +
                            "if (TRUE == isPreconditionFulfilledAndSignalUpdated)\n" +
                            "{\n" +
                                "if (TRUE == isInvalidData)\n" +
                                "{\n" +
                                    "(void)Rte_Call_EvHnd_SetEventStatus(EvHnd_Event_en, EVHND_STATUS_FAILED, 0u, 0u);\n" +
                                "}\n" +
                                "else\n" +
                                "{\n" +
                                    "(void)Rte_Call_EvHnd_SetEventStatus(EvHnd_Event_en, EVHND_STATUS_PASSED, 0u, 0u);\n" +
                                "}\n" +
                            "}\n" +
                            "else\n" +
                            "{\n" +
                                "/* Do nothing since the precondition is not OK or/and the signals is not updated */\n" +
                            "}\n" +
                        "}\n";

            File.AppendAllText(outputFile, functie2);

            MessageBox.Show("FileOutput a fost generat!");
        }
        //End functie generare

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            int nrAnds = 0, nrOrs = 0, nrElementDeCautat = 0;
            string toSearch = "Empty", elementDeCautat = "";
            string[] vectorElementDeCautat = new string[10];
            string functionName = "Vda_Calculate", variable, valoare, outputPath, functionType = "void";
            string[] vectorValori = new string[10];
            try
            {
                toSearch = File.ReadAllText(filePath);
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.ToString());
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine(ane.ToString());
            }

            Console.WriteLine(toSearch);

            


            //Daca fisierul contine "InvalidDataError" se va deschide interfata de la Hoza
            if (toSearch.Contains("InvalidDataError"))
            {
                //Parametrii InvalidDataError++++++++++++++++++++++++++++++++++
                String[] liniiFis;

                ArrayList define = new ArrayList();
                ArrayList richTextBox = new ArrayList();
                String conditia_din_functie = "";


                
                ArrayList operatoriLogici = new ArrayList();
                ArrayList stanga = new ArrayList();
                ArrayList semn = new ArrayList();
                ArrayList dreapta = new ArrayList();

                //End Parametrii

                if (!File.ReadAllText(filePath).Contains("Preconditions:"))
                {
                    liniiFis = File.ReadAllLines(filePath);

                    foreach (String it in liniiFis)
                    {
                        //Operatorii logici
                        if (it.Equals("AND"))
                        {
                            operatoriLogici.Add("&&");
                        }
                        else if (it.Equals("OR"))
                        {
                            operatoriLogici.Add("||");
                        }
                    }
                }
                else
                {

                    liniiFis = File.ReadAllLines(filePath);
                    foreach (String it in liniiFis)
                    {
                        //Operatorii logici
                        operatoriLogici.Add("&&");

                    }
                }

                foreach (String it in liniiFis)
                {
                    //Operatorii logici
                    if (it.Equals("AND"))
                    {
                        operatoriLogici.Add("&&");
                    }
                    else if (it.Equals("OR"))
                    {
                        operatoriLogici.Add("||");
                    }

                    //Preconditii && Define
                    string pattern1 = @"""(\w*)""\s*(\S*)\s*'(\w*=\w*)'";
                    string pattern2 = @"""(\w*)""\s*(\S*)\s*'(\w*)'";
                    Match match1 = Regex.Match(it, pattern1, RegexOptions.IgnoreCase);
                    Match match2 = Regex.Match(it, pattern2, RegexOptions.IgnoreCase);
                       
                    if (match1.Success && !it.Contains("InvalidDataError"))
                    {
                        if(match1.Groups[2].Value.Equals("<>"))
                        {
                            semn.Add("!=");
                        }
                        else
                        {
                            semn.Add(match1.Groups[2].Value);
                        }
                        richTextBox.Add("type " + match1.Groups[1].Value + "\n");
                        stanga.Add(match1.Groups[1].Value);
                        Match aux = Regex.Match(match1.Groups[3].Value, @"(\w*)=(\w*)", RegexOptions.IgnoreCase);
                        dreapta.Add("VDA_IST_" + aux.Groups[2].Value.ToUpper());
                        define.Add("#define " + "VDA_IST_" + aux.Groups[2].Value.ToUpper() + " " + aux.Groups[1].Value + "\n");
                    }
                    if (match2.Success && !it.Contains("InvalidDataError"))
                    {
                        if(match2.Groups[2].Value.Equals("<>"))
                        {
                            semn.Add("!=");
                        }
                        else
                        {
                            semn.Add(match2.Groups[2].Value);
                        }
                        richTextBox.Add("type " + match2.Groups[1].Value + "\n");
                        stanga.Add(match2.Groups[1].Value);
                        dreapta.Add("VDA_IST_" + match2.Groups[3].Value.ToUpper());
                        define.Add("#define " + "VDA_IST_" + match2.Groups[3].Value.ToUpper() + "\n");
                    }


                }

                for (int i = 0; i < dreapta.Count - 1; i++)
                {
                    conditia_din_functie += "(" + stanga[i] + semn[i] + dreapta[i] + ")" + operatoriLogici[i];
                    //File.AppendAllText(outputFile, "(" + stanga[i] + semn[i] + dreapta[i] + ")\n");
                    //File.AppendAllText(outputFile, "" + operatoriLogici[i]);
                }
                conditia_din_functie += "(" + stanga[dreapta.Count - 1] + semn[dreapta.Count - 1] + dreapta[dreapta.Count - 1] + ")";

                outputPath = filePath.Substring(0, filePath.Length - ((fileName.Length) + 1));
                InvalidDataError.InterfataInvalidDataError(define, richTextBox, conditia_din_functie, outputPath);
            }

            //Altfel se va deschide interfata de la Eric
            else
            {

                Match matchElemDeCautat = Regex.Match(toSearch, "IF\\s*\"([a-zA-z0-9]+)\"\\s*");
                if (matchElemDeCautat.Success)
                {
                    elementDeCautat = matchElemDeCautat.Groups[1].Value;
                    Console.WriteLine("elem de cautat---------------->" + elementDeCautat);
                    while (matchElemDeCautat.Success)
                    {
                        vectorElementDeCautat[nrElementDeCautat++] = matchElemDeCautat.Groups[1].Value;
                        matchElemDeCautat = matchElemDeCautat.NextMatch();
                    }

                }
                else
                {
                    Console.WriteLine("elem de cautat----------------------------------> NOT FOUND");
                }

                Match matchElemDeCautatAnd = Regex.Match(toSearch, "AND\\s*\"([a-zA-z0-9]+)\"\\s*");
                if (matchElemDeCautatAnd.Success)
                {
                    elementDeCautat = matchElemDeCautatAnd.Groups[1].Value;
                    Console.WriteLine("elem de cautat---------------->" + elementDeCautat);
                    while (matchElemDeCautatAnd.Success)
                    {
                        vectorElementDeCautat[nrElementDeCautat++] = matchElemDeCautatAnd.Groups[1].Value;
                        matchElemDeCautatAnd = matchElemDeCautatAnd.NextMatch();
                    }

                }

                Match matchElemDeCautatOr = Regex.Match(toSearch, "OR\\s*\"([a-zA-z0-9]+)\"\\s*");
                if (matchElemDeCautatOr.Success)
                {
                    elementDeCautat = matchElemDeCautatOr.Groups[1].Value;
                    Console.WriteLine("elem de cautat---------------->" + elementDeCautat);
                    while (matchElemDeCautatOr.Success)
                    {
                        vectorElementDeCautat[nrElementDeCautat++] = matchElemDeCautatOr.Groups[1].Value;
                        matchElemDeCautatOr = matchElemDeCautatOr.NextMatch();
                    }

                }




                Match matchFunctionName = Regex.Match(toSearch, "THEN:\\s*\"\\w*_([a-zA-z0-9]+)\"\\s*");
                if (matchFunctionName.Success)
                {
                    functionName = functionName + matchFunctionName.Groups[1].Value;
                    Console.WriteLine("function name-------------->" + functionName);
                }
                else
                    Console.WriteLine("function name-------------------------> NOT FOUND");

                Match matchVariable = Regex.Match(toSearch, "THEN:\\s*\"(\\w+)\"\\s*");
                if (matchVariable.Success)
                {
                    variable = matchVariable.Groups[1].Value;
                    Console.WriteLine("variable-------------->" + variable);
                }
                else
                {
                    variable = "NOT FOUND";
                    Console.WriteLine("variable------------------------->" + variable);
                }


                Match matchValoare = Regex.Match(toSearch, @"\'([0-9])");
                if (matchValoare.Success)
                {

                    int i = 0;
                    //valoare = matchValoare.Groups[1].Value;
                    while (matchValoare.Success)
                    {
                        vectorValori[i] = matchValoare.Groups[1].Value;
                        matchValoare = matchValoare.NextMatch();
                        i++;
                    }


                    /* Console.WriteLine("valoare-------------->" + valoare);
                     Console.WriteLine("valoare-------------->" + matchValoare.NextMatch().Groups[1].Value);
                     Console.WriteLine("valoare-------------->" + matchValoare.NextMatch().NextMatch().Groups[1].Value);
                    */
                    for (int j = 0; j < vectorValori.Length; j++)
                    {
                        if (vectorValori[j] != null)
                            Console.WriteLine("valoare-------------->" + vectorValori[j]);
                    }
                }
                else
                {
                    valoare = "NOT FOUND";
                    Console.WriteLine("valoare------------------------->" + valoare);
                }

                Match numarAnd = Regex.Match(toSearch, "AND");
                if (numarAnd.Success)
                {
                    while (numarAnd.Success)
                    {
                        nrAnds++;
                        numarAnd = numarAnd.NextMatch();
                    }
                }
                Console.WriteLine("nrAnds------------------------->" + nrAnds);

                Match numarOr = Regex.Match(toSearch, "OR");
                if (numarOr.Success)
                {
                    while (numarOr.Success)
                    {
                        nrOrs++;
                        numarOr = numarOr.NextMatch();
                    }
                }
                Console.WriteLine("nrOrs------------------------->" + nrOrs);

                int ElseIfCondition = 0;
                Match okElseIf = Regex.Match(toSearch, "ELSE:\\s*IF");
                if (okElseIf.Success)
                {
                    ElseIfCondition = 1;
                }
                Console.WriteLine("ELSEIFCONDITION------------------------->" + ElseIfCondition);


                Console.WriteLine("file path ------------->" + filePath);

                void orCondition()
                {
                    int k = 0;
                    StreamWriter streamWriter = new StreamWriter(outputPath + "\\output1.c", false, Encoding.ASCII);

                    functieReadHoza = Cautare(RteVdaFile, vectorElementDeCautat[0]);
                    tipVarDarian = regexFunctieDBC(DBCFile, vectorElementDeCautat[0]);
                    tipVar2Darian = regexFunctieDBC(DBCFile, vectorElementDeCautat[0]);

                    int i = 0;
                    streamWriter.Write(
                    functionType + " " + functionName + "(" + functionType + ")" + "\r\n" +
                    "{" + "\r\n" +
                    tipVarDarian + " " + variable + ";" + "\r\n"
                    );

                    for (i = 0; i < vectorElementDeCautat.Length; i++)
                    {
                        if (vectorElementDeCautat[i] != null)
                        {
                            streamWriter.Write(
                                    tipVar2Darian + " " + vectorElementDeCautat[i] + ";" + "\r\n" +
                                    functieReadHoza + "(&" + vectorElementDeCautat[i] + ");" + "\r\n"
                                );
                        }
                    }

                    i = 0;
                    streamWriter.Write(
                    "if(" + vectorElementDeCautat[i] + " == " + vectorValori[k++] + ")" 
                    );

                    for (i = 1; i < vectorElementDeCautat.Length; i++)
                    {
                        if (vectorElementDeCautat[i] != null)
                        {
                            streamWriter.Write(
                                " || (" + vectorElementDeCautat[i] + " == " + vectorValori[k++] + ")" + "\r\n"
                                );
                        }
                    }

                    streamWriter.Write(
                   
                                "{" + "\r\n" +
                                "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                                "}" + "\r\n" +
                                "else" + "\r\n" +
                                "{" + "\r\n" +
                                "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                                "}" + "\r\n" +
                                "}" + "\r\n" + "\r\n");

                    streamWriter.Close();
                    MessageBox.Show("FileOutput a fost generat !!!");
                    this.Close();
                }


                void andCondition()
                {
                    int k = 0;
                    StreamWriter streamWriter = new StreamWriter(outputPath + "\\output1.c", false, Encoding.ASCII);

                    functieReadHoza = Cautare(RteVdaFile, vectorElementDeCautat[0]);
                    tipVarDarian = regexFunctieDBC(DBCFile, vectorElementDeCautat[0]);
                    tipVar2Darian = regexFunctieDBC(DBCFile, vectorElementDeCautat[0]);

                    int i = 0;
                    streamWriter.Write(
                    functionType + " " + functionName + "(" + functionType + ")" + "\r\n" +
                    "{" + "\r\n" +
                    tipVarDarian + " " + variable + ";" + "\r\n"
                    );

                    for (i = 0; i < vectorElementDeCautat.Length; i++)
                    {
                        if (vectorElementDeCautat[i] != null)
                        {
                            functieReadHoza = Cautare(RteVdaFile, vectorElementDeCautat[i]);
                            streamWriter.Write(
                                    tipVar2Darian + " " + vectorElementDeCautat[i] + ";" + "\r\n" +
                                    functieReadHoza + "(&" + vectorElementDeCautat[i] + ");" + "\r\n"
                                );
                        }
                    }

                    i = 0;
                    streamWriter.Write(
                    "if(" + vectorElementDeCautat[i] + " == " + vectorValori[k++] + ")" 
                    );

                    for (i=1; i< vectorElementDeCautat.Length; i++)
                    {
                        if (vectorElementDeCautat[i] != null)
                        {
                            streamWriter.Write(
                                " && (" + vectorElementDeCautat[i] + " == " + vectorValori[k++] + ")" + "\r\n"
                                );
                        }
                    }

                    streamWriter.Write(

                                "{" + "\r\n" +
                                "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                                "}" + "\r\n" +
                                "else" + "\r\n" +
                                "{" + "\r\n" +
                                "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                                "}" + "\r\n" +
                                "}" + "\r\n" + "\r\n");

                    streamWriter.Close();
                    MessageBox.Show("FileOutput a fost generat !!!");
                    this.Close();
                }

                void singleCondition()
                {
                    int k = 0;

                    StreamWriter streamWriter = new StreamWriter(outputPath + "\\output1.c", false, Encoding.ASCII);

                    if(ElseIfCondition == 0)
                    {
                        for (int i = 0; i <= vectorElementDeCautat.Length - 1; i++)
                        {
                            if (vectorElementDeCautat[i] != null)
                            {
                                functieReadHoza = Cautare(RteVdaFile, vectorElementDeCautat[i]);
                                tipVarDarian = regexFunctieDBC(DBCFile, vectorElementDeCautat[i]);
                                tipVar2Darian = regexFunctieDBC(DBCFile, vectorElementDeCautat[i]);


                                streamWriter.Write(
                                functionType + " " + functionName + "(" + functionType + ")" + "\r\n" +
                                "{" + "\r\n" +
                                tipVarDarian + " " + variable + ";" + "\r\n" +
                                tipVar2Darian + " " + vectorElementDeCautat[i] + ";" + "\r\n" +
                                functieReadHoza + "(&" + vectorElementDeCautat[i] + ");" + "\r\n" +
                                "if(" + vectorElementDeCautat[i] + " == " + vectorValori[k++] + ")" + "\r\n" +
                                "{" + "\r\n" +
                                "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                                "}" + "\r\n" +
                                "else" + "\r\n" +
                                "{" + "\r\n" +
                                "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                                "}" + "\r\n" +
                                "}" + "\r\n" + "\r\n"
                                );
                            }
                        }
                    }
                    else
                    {
                        functieReadHoza = Cautare(RteVdaFile, vectorElementDeCautat[0]);
                        tipVarDarian = regexFunctieDBC(DBCFile, vectorElementDeCautat[0]);
                        tipVar2Darian = regexFunctieDBC(DBCFile, vectorElementDeCautat[0]);

                        int i = 0;
                        streamWriter.Write(
                        functionType + " " + functionName + "(" + functionType + ")" + "\r\n" +
                        "{" + "\r\n" +
                        tipVarDarian + " " + variable + ";" + "\r\n"
                        );

                        for(i=0; i<vectorElementDeCautat.Length; i++)
                        {
                            if (vectorElementDeCautat[i] != null)
                            {
                                streamWriter.Write(
                                        tipVar2Darian + " " + vectorElementDeCautat[i] + ";" + "\r\n" +
                                        functieReadHoza + "(&" + vectorElementDeCautat[i] + ");" + "\r\n"
                                    );
                            }
                        }

                        i = 0;
                        streamWriter.Write(
                        "if(" + vectorElementDeCautat[i] + " == " + vectorValori[k++] + ")" + "\r\n" +
                        "{" + "\r\n" +
                        "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                        "}" + "\r\n" +
                        "else" + "\r\n" +
                        "if" + "(" + vectorElementDeCautat[++i] + "==" + vectorValori[k++] + ")" + "\r\n" +
                        "{" + "\r\n" +
                        "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                        "}" + "\r\n" +
                        "else" +  "\r\n" +
                        "{" + "\r\n" +
                        variable + " = " + vectorValori[k++] + ";" + "\r\n" + 
                        "}" + "\r\n" +
                        "}" + "\r\n" + "\r\n"
                        );
                    }
                    
                    MessageBox.Show("FileOutput a fost generat !!!");
                    streamWriter.Close();
                    this.Close();
                }

                void multipleConditions(string[] vectorTipuriParametrii, string[] vectorParametriiFaraValori)
                {

                    int k = 0;

                    tipVarDarian = valoriParametriiFaraValori[0];
                    tipVar2Darian = valoriParametriiFaraValori[0];
                    functieReadHoza = Cautare(RteVdaFile, vectorElementDeCautat[0]);


                    StreamWriter streamWriter = new StreamWriter(outputPath + "\\output1.c", false, Encoding.ASCII);

                    streamWriter.Write(
                       functionType + " " + functionName + "(");

                    int i = 0;
                    for (i = 0; i < vectorTipuriParametrii.Length - 1; i++)
                    {
                        if (vectorTipuriParametrii[i + 1] != null)
                        {
                            streamWriter.Write(vectorTipuriParametrii[i] + " " + vectorParametriiFaraValori[i] + ",");
                        }
                    }

                    i = 0;
                    while (vectorTipuriParametrii[i + 1] != null)
                        i++;

                    streamWriter.Write(vectorTipuriParametrii[i] + " " + vectorParametriiFaraValori[i] + ")" + "\r\n");

                    streamWriter.Write(
                        
                        "{" + "\r\n" +
                        tipVarDarian + " " + variable + ";" + "\r\n" +
                        "if(" + vectorElementDeCautat[i] + " == " + vectorValori[k++] + ")"
                        );

                    if(nrAnds > 0)
                    {
                       for (int j = 1; j < vectorElementDeCautat.Length; j++)
                       {
                           if (vectorElementDeCautat[j] != null)
                           {
                               streamWriter.Write(
                                   " && (" + vectorElementDeCautat[j] + " == " + vectorValori[k++] + ")" + "\r\n"
                                   );
                           }
                       }
                    }

                    if (nrOrs > 0)
                    {
                        for (int j = 1; j < vectorElementDeCautat.Length; j++)
                        {
                            if (vectorElementDeCautat[j] != null)
                            {
                                streamWriter.Write(
                                    " || (" + vectorElementDeCautat[j] + " == " + vectorValori[k++] + ")" + "\r\n"
                                    );
                            }
                        }
                    }

                    streamWriter.Write(
                        "{" + "\r\n" +
                        "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                        "}" + "\r\n" +
                        "else" + "\r\n" +
                        "{" + "\r\n" +
                        "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                        "}" + "\r\n" +
                        "}"
                        );
                    streamWriter.Close();
                    this.Close();
                }


              
                if (DBCOpen == true && RteOpen == true && inputOpen == true)
                {
                    outputPath = filePath.Substring(0, filePath.Length - ((fileName.Length) + 1));
                    
                        try
                        {
                            Console.WriteLine("output path --------->" + outputPath);
                            try
                            {
                                //daca nu se gaseste vreun parametru in dbc sau vda_rte (folosind functiile de la Hoza si Darian)
                                if ((typeOfInputFile(DBCFile, elementDeCautat).Equals("null")))
                                {

                                    parametriiFaraValori[indexParametriiFaraValori++] = elementDeCautat;

                                    Console.WriteLine("++++++++++++=>ParamFaraVal");

                                    for (int i = 0; i < parametriiFaraValori.Length; i++)
                                    {
                                        if (parametriiFaraValori[i] != null)
                                            Console.WriteLine(i + " " + parametriiFaraValori[i]);
                                    }

                                    string promptValue = ParametriiPrompt.ShowDialog("Multiple unidentified variables have been found, please insert their values here",
                                        "Variable names needed",
                                        parametriiFaraValori);

                                    try
                                    {

                                        multipleConditions(valoriParametriiFaraValori, parametriiFaraValori);

                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine("Exception" + exc.Message);
                                    }

                                   


                                }
                                else
                                {
                                    //daca parametrii sunt gasiti in dbc sau rte_vda (folosind functiile de la Hoza si Darian)

                                    if(nrAnds > 0)
                                    {
                                        try
                                        {
                                            andCondition();
                                        }
                                        catch (Exception exc)
                                        {
                                            Console.WriteLine("Exception" + exc.Message);
                                        }
                                        
                                    }
                                    else if (nrOrs > 0)
                                {
                                    try
                                    {
                                        orCondition();
                                    }
                                    catch(Exception exc)
                                    {
                                        Console.WriteLine("Exception" + exc.Message);
                                    }
                                }
                                    else
                                    {
                                        try
                                        {
                                            singleCondition();
                                        }
                                        catch (Exception exc)
                                        {
                                            Console.WriteLine("Exception" + exc.Message);
                                        }
                                    }
                                    
                                }
                            }
                            catch (FileNotFoundException fnfe)
                            {
                                Console.WriteLine(fnfe.ToString());
                                MessageBox.Show(
                                "File not found",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            }
                            catch (IOException ioe)
                            {
                                MessageBox.Show(
                                "Something went wrong",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                                Console.WriteLine(ioe.ToString());
                            }
                        }
                        catch (NullReferenceException nre)
                        {
                            MessageBox.Show(
                                "Something went wrong",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            Console.WriteLine(nre.ToString());
                        }
                    
                }
                else
                {
                    MessageBox.Show(
                            "You must select all 3 files",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                }
            }

        }
    }
}
