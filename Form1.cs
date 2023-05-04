using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AulaBosch2;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        this.WindowState = FormWindowState.Maximized;
        this.FormBorderStyle = FormBorderStyle.None;
        KeyDown += delegate
        {
            Application.Exit();
        };

        PictureBox pb = new PictureBox();
        pb.Dock = DockStyle.Left;
        pb.SizeMode = PictureBoxSizeMode.CenterImage;
        this.Controls.Add(pb);

        PictureBox pbresult = new PictureBox();
        pbresult.Dock = DockStyle.Right;
        pbresult.SizeMode = PictureBoxSizeMode.CenterImage;
        this.Controls.Add(pbresult);

        Bitmap bmp = Image.FromFile("bg.png") as Bitmap;
        Bitmap bmp2 = (Bitmap)bmp.Clone();
        process();
        pbresult.Image = bmp2;

        pb.Image = bmp;

        Load += delegate
        {
            pbresult.Width = Width / 2;
            pb.Width = Width / 2;
        };


        void process()
        {
            var data = bmp2.LockBits(
                new Rectangle(0, 0, bmp2.Width, bmp2.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);

            byte[] bytes = new byte[data.Stride * data.Height];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            bytes = compactAndDecompact(bytes);

            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);

            bmp2.UnlockBits(data);
        }

        byte[] compactAndDecompact(byte[] bytes)
        {
            var start = DateTime.Now;
            var compressed = compact(bytes);
            bytes = decompact(compressed);
            var end = DateTime.Now;
            MessageBox.Show((end - start).TotalMilliseconds.ToString() + " ms");
            return bytes;
        }

        // COLOQUE ABAIXO SUAS FUNÇÕES PARA COMPACT E DECOMPACT

        byte[] compact(byte[] vetor)
        {
            int count = 0;
            byte binary1, binary2, binary3;
            byte[] zipped = new byte[vetor.Length / 2];
            for (int i = 0; i < (vetor.Length); i = i + 2)
            {
                binary1 = (byte)((vetor[i] >> 4));
                binary2 = (byte)((vetor[i + 1] >> 4));
                binary3 = (byte)((binary1 << 4) | binary2);
                zipped[count] = binary3;
                count++;
                Console.WriteLine($"{i + 1}° zipped: {Convert.ToString(binary1, 2).PadLeft(4, '0')}");
                Console.WriteLine($"{i + 2}° zipped: {Convert.ToString(binary2, 2).PadLeft(4, '0')}");
            }
            return zipped;
        }

        byte[] decompact(byte[] zippedVetor)
        {
            byte fist4, last4;
            byte[] unzipped = new byte[zippedVetor.Length * 2];
            for (int i = 0; i < (zippedVetor.Length); i++)
            {
                fist4 = (byte)(zippedVetor[i] >> 4);
                last4 = (byte)(zippedVetor[i] & 0b00001111);
                fist4 <<= 4;
                last4 <<= 4;

                unzipped[i * 2] = fist4;
                unzipped[i * 2 + 1] = last4;

            }
            return unzipped;
        }

    }
}