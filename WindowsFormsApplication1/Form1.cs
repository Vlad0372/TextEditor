using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private bool czyPlikZmodyfikowany;
        private bool czyPlikZapisany;
        private bool czyPlikNowy;
        private string tekstFormy;
        private string nazwaPliku;
        private string katalogPoczatkowy;
        private string katalogTerazniejszy;

        public Form1()
        {
            InitializeComponent();

            czyPlikZmodyfikowany = false;
            czyPlikZapisany = false;
            czyPlikNowy = true;
            tekstFormy = "Bez tytułu - Notatnik";
            nazwaPliku = "Bez tytułu";
            katalogPoczatkowy = "C:\\Users\\student.IAII\\Desktop";
            katalogTerazniejszy = katalogPoczatkowy;
        }

        private void nowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (czyPlikZmodyfikowany == true)
            {
                DialogResult dialogResult = CzyChceszZapisacPlik(nazwaPliku);

                if (dialogResult == DialogResult.Yes)
                {
                    zapiszToolStripMenuItem_Click(sender, e);

                    if (czyPlikZapisany == true)
                    {
                        nowyToolStripMenuItem_Click(sender, e);
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    UtworzNowyPlik();
                }
            }
            else
            {
                UtworzNowyPlik();
            }
        }
        private void UtworzNowyPlik()
        {
            richTextBox1.Clear();

            tekstFormy = "Bez tytułu - Notatnik";
            this.Text = tekstFormy;

            nazwaPliku = "Bez tytułu";

            czyPlikZmodyfikowany = false;
            czyPlikZapisany = false;
            czyPlikNowy = true;
        }

        private void otworzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (czyPlikZmodyfikowany == true)
            {
                DialogResult dialogResult = CzyChceszZapisacPlik(nazwaPliku);

                if (dialogResult == DialogResult.Yes)
                {
                    zapiszToolStripMenuItem_Click(sender, e);

                    if (czyPlikZapisany == true)
                    {
                        otworzToolStripMenuItem_Click(sender, e);
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    SprobujOtworzycPlik();
                }
            }
            else
            {
                SprobujOtworzycPlik();
            }
        }
        private void SprobujOtworzycPlik()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = katalogPoczatkowy;
                openFileDialog.Filter = "Dokumenty tekstowe (*.txt)|*.txt";
                openFileDialog.RestoreDirectory = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader streamReader = new StreamReader(openFileDialog.OpenFile()))
                    {
                        richTextBox1.Text = streamReader.ReadToEnd();

                        katalogTerazniejszy = openFileDialog.FileName;
                        nazwaPliku = Path.GetFileName(katalogTerazniejszy);

                        tekstFormy = nazwaPliku + " - Notatnik";
                        this.Text = tekstFormy;

                        czyPlikZmodyfikowany = false;
                        czyPlikNowy = false;
                        czyPlikZapisany = true;

                        richTextBox1.TextChanged -= richTextBox1_TextChanged;

                        richTextBox1.Clear();
                        richTextBox1.LoadFile(katalogTerazniejszy, RichTextBoxStreamType.PlainText);

                        richTextBox1.TextChanged += richTextBox1_TextChanged;
                    }
                }
            }
        }


        private void zapiszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (czyPlikNowy == true)
            {
                SprobujZapisacPlik();
            }
            else if (czyPlikZmodyfikowany == true)
            {
                ZapiszIstniejacyPlik(katalogTerazniejszy);
            }
        }
        private void SprobujZapisacPlik()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                StreamWriter streamWriter;

                saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if ((streamWriter = new StreamWriter(saveFileDialog.OpenFile())) != null)
                    {
                        streamWriter.Write(richTextBox1.Text);

                        nazwaPliku = saveFileDialog.FileName;

                        tekstFormy = Path.GetFileName(nazwaPliku) + " - Notatnik";
                        this.Text = tekstFormy;

                        czyPlikNowy = false;
                        czyPlikZmodyfikowany = false;
                        czyPlikZapisany = true;

                        streamWriter.Close();
                    }
                }
            }
        }
        private void ZapiszIstniejacyPlik(string katalog)
        {
            StreamWriter streamWriter = new StreamWriter(katalog);

            streamWriter.Write(richTextBox1.Text);

            //======================
            katalogTerazniejszy = katalog;
            nazwaPliku = Path.GetFileName(katalogTerazniejszy);

            tekstFormy = nazwaPliku + " - Notatnik";
            this.Text = tekstFormy;

            czyPlikZmodyfikowany = false;
            czyPlikNowy = false;
            czyPlikZapisany = true;
            //======================

            streamWriter.Close();
        }


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (czyPlikZmodyfikowany == false)
            {
                czyPlikZmodyfikowany = true;
                this.Text = "*" + tekstFormy;
            }
            if (czyPlikNowy == true)
            {
                if (richTextBox1.TextLength == 0)
                {
                    this.Text = tekstFormy;
                    czyPlikZmodyfikowany = false;
                }
            }
        }

        private DialogResult CzyChceszZapisacPlik(string plik)
        {
            StringBuilder stringBuilder = new StringBuilder("Czy chcesz zapisać zmiany w pliku  ?");

            if (czyPlikNowy == true)
            {
                stringBuilder.Insert(stringBuilder.Length - 2, plik);
            }
            else
            {
                stringBuilder.Insert(stringBuilder.Length - 2, "\r\n" + katalogTerazniejszy);
            }

            return MessageBox.Show(stringBuilder.ToString(), "Notatnik", MessageBoxButtons.YesNoCancel);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            Size newSize = new Size(this.Size.Width, this.Size.Height - 75);

            richTextBox1.Size = newSize;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (czyPlikZmodyfikowany == true)
            {
                DialogResult dialogResult = CzyChceszZapisacPlik(nazwaPliku);

                if (dialogResult == DialogResult.Yes)
                {
                    zapiszToolStripMenuItem_Click(sender, e);

                    if (czyPlikZapisany == false)
                    {
                        e.Cancel = true;
                    }
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }             
            }
        }      
    }
}
