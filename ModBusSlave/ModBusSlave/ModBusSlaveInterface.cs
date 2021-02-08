using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Collections;


namespace ModBusSlave
{
    public partial class ModBusSlaveInterface : Form
    {
        public SerialPort serial = new SerialPort();

        int data,n;
        Queue datas = new Queue();
        long start_time = 0, current_time = 0;
        byte current_byte_number;

        byte[] ids = { 0, 0, 0, 0 };
        byte[] tests = { 0, 0, 0, 0 };
        byte[] crc = new byte[2];

        CheckBox[] coils1 = new CheckBox[16];
        CheckBox[] coils2 = new CheckBox[16];
        CheckBox[] coils3 = new CheckBox[16];
        CheckBox[] coils4 = new CheckBox[16];

        TextBox[] registers1 = new TextBox[10];
        TextBox[] registers2 = new TextBox[10];
        TextBox[] registers3 = new TextBox[10];
        TextBox[] registers4 = new TextBox[10];

        int[] coils1num = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        int[] coils2num = { 7321, 7322, 7323, 7324, 7325, 7326, 7327, 7328, 7329, 7330, 7331, 7332, 7333, 7334, 7335, 7336 };
        int[] coils3num = { 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68 };
        int[] coils4num = { 65520, 65521, 65522, 65523, 65524, 65525, 65526, 65527, 65528, 65529, 65530, 65531, 65532, 65533, 65534, 65535 };

        int[] registers1num = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        int[] registers2num = { 7999, 8000, 8001, 8002, 8003, 8004, 8005, 8006, 8007, 8008 };
        int[] registers3num = { 97, 98, 99, 100, 101, 102, 103, 104, 105, 106 };
        int[] registers4num = { 61972, 61973, 61974, 61975, 61976, 61977, 61978, 61979, 61980, 61981 };
        

        public ModBusSlaveInterface()
        {
            InitializeComponent();
            LoadListboxes();
            LoadCoilsAndRegisters();
        }

        private void LoadListboxes()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                serialPorts.Items.Add(port);
            }
            serialPorts.SelectedIndex = 0;

            string[] baudrates = { "115200", "19200", "9600" };
            foreach (string baudrate in baudrates)
            {
                baudRates.Items.Add(baudrate);
            }
            baudRates.SelectedIndex = 2;

            label_open.Text = "Port is not opened";
            label_open.ForeColor = Color.Red;
            label_connect.Text = "Master is not connected.";
            label_connect.ForeColor = Color.Red;

        }

        private void LoadCoilsAndRegisters()
        {
            coils1[0] = a0;
            coils1[1] = a1;
            coils1[2] = a2;
            coils1[3] = a3;
            coils1[4] = a4;
            coils1[5] = a5;
            coils1[6] = a6;
            coils1[7] = a7;
            coils1[8] = a8;
            coils1[9] = a9;
            coils1[10] = a10;
            coils1[11] = a11;
            coils1[12] = a12;
            coils1[13] = a13;
            coils1[14] = a14;
            coils1[15] = a15;

            registers1[0] = x0;
            registers1[1] = x1;
            registers1[2] = x2;
            registers1[3] = x3;
            registers1[4] = x4;
            registers1[5] = x5;
            registers1[6] = x6;
            registers1[7] = x7;
            registers1[8] = x8;
            registers1[9] = x9;

            coils2[0] = c0;
            coils2[1] = c1;
            coils2[2] = c2;
            coils2[3] = c3;
            coils2[4] = c4;
            coils2[5] = c5;
            coils2[6] = c6;
            coils2[7] = c7;
            coils2[8] = c8;
            coils2[9] = c9;
            coils2[10] = c10;
            coils2[11] = c11;
            coils2[12] = c12;
            coils2[13] = c13;
            coils2[14] = c14;
            coils2[15] = c15;

            registers2[0] = z0;
            registers2[1] = z1;
            registers2[2] = z2;
            registers2[3] = z3;
            registers2[4] = z4;
            registers2[5] = z5;
            registers2[6] = z6;
            registers2[7] = z7;
            registers2[8] = z8;
            registers2[9] = z9;

            coils3[0] = b0;
            coils3[1] = b1;
            coils3[2] = b2;
            coils3[3] = b3;
            coils3[4] = b4;
            coils3[5] = b5;
            coils3[6] = b6;
            coils3[7] = b7;
            coils3[8] = b8;
            coils3[9] = b9;
            coils3[10] = b10;
            coils3[11] = b11;
            coils3[12] = b12;
            coils3[13] = b13;
            coils3[14] = b14;
            coils3[15] = b15;

            registers3[0] = y0;
            registers3[1] = y1;
            registers3[2] = y2;
            registers3[3] = y3;
            registers3[4] = y4;
            registers3[5] = y5;
            registers3[6] = y6;
            registers3[7] = y7;
            registers3[8] = y8;
            registers3[9] = y9;

            coils4[0] = d0;
            coils4[1] = d1;
            coils4[2] = d2;
            coils4[3] = d3;
            coils4[4] = d4;
            coils4[5] = d5;
            coils4[6] = d6;
            coils4[7] = d7;
            coils4[8] = d8;
            coils4[9] = d9;
            coils4[10] = d10;
            coils4[11] = d11;
            coils4[12] = d12;
            coils4[13] = d13;
            coils4[14] = d14;
            coils4[15] = d15;

            registers4[0] = w0;
            registers4[1] = w1;
            registers4[2] = w2;
            registers4[3] = w3;
            registers4[4] = w4;
            registers4[5] = w5;
            registers4[6] = w6;
            registers4[7] = w7;
            registers4[8] = w8;
            registers4[9] = w9;
        }

        public bool open(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits)
        {
            if (!serial.IsOpen)
            {
                serial.PortName = portName;
                serial.BaudRate = baudRate;
                serial.DataBits = dataBits;
                serial.Parity = parity;
                serial.StopBits = stopBits;
                serial.ReadTimeout = 1000;
                serial.WriteTimeout = 1000;

                try
                {
                    serial.Open();
                }
                catch (Exception err)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        public bool close()
        {
            if (serial.IsOpen)
            {
                try
                {
                    serial.Close();
                }
                catch (Exception err)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void createCRC16(byte[] message, ref byte[] CRC)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF;
            byte CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }

        public bool checkCRC(byte[] message)
        {
            int n = message.Length;
            byte[] CRC = new byte[2];
            createCRC16(message, ref CRC);

            if (CRC[0] == message[n - 2] && CRC[1] == message[n - 1])
            {
                return true;
            }

            return false;
        }

        private void button_connect_Click_1(object sender, EventArgs e)
        {
            if (open(serialPorts.SelectedItem.ToString(), Convert.ToInt32(baudRates.SelectedItem.ToString()), 8, Parity.Odd, StopBits.One))
            {
                label_open.Text = "Port is opened";
                label_open.ForeColor = Color.Green;
                label_connect.Text = "Master is connected to " + serialPorts.SelectedItem.ToString() + ".";
                label_connect.ForeColor = Color.Green;

                serial.DataReceived += serial_DataReceived;
                current_byte_number = 0;
                label_byte_nr.Text = "0";
                label_slave_id.Text = "";
                label_fun_nr.Text = "";
            }
            else
            {
                label_open.Text = "Port is not opened";
                label_open.ForeColor = Color.Red;
                label_connect.Text = "Master is not connected.";
                label_connect.ForeColor = Color.Red;
            }
        }

        void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while(serial.BytesToRead > 0)
            {
                data = serial.ReadByte();
                this.Invoke(new EventHandler(displaydata_event));
            }
            checkFrame();
        }

        private void displaydata_event(object sender, EventArgs e)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            current_time = now.ToUnixTimeMilliseconds();

            if (current_time - start_time > 25)
            {
                current_byte_number = 0;
                start_time = current_time;
                datas.Clear();
            }

            datas.Enqueue((byte)data);

            label_byte_nr.Text = Convert.ToString(current_byte_number);
            current_byte_number += 1;
        }

        private void checkFrame()
        {
            n = datas.Count;
            byte[] mess = new byte[n];
            byte[] CRC = new byte[2];
            datas.CopyTo(mess, 0);
           createCRC16(mess, ref CRC);

            // symulacja błędu na łączu
            Random rand = new Random();
            if (rand.Next(0, 101) == 47)
                mess[0]++;
            // koniec symulacji błędu na łączu
            try
            {
                if (CRC[0] == mess[n - 2] && CRC[1] == mess[n - 1])
                {

                    label_byte_nr.Invoke(new Action(delegate ()
                    {
                        label_byte_nr.Text = "OK";
                        label_byte_nr.ForeColor = Color.Green;
                        label_slave_id.Text = mess[0].ToString();
                        label_fun_nr.Text = mess[1].ToString();
                    }));

                }
                else
                {
                    label_byte_nr.Invoke(new Action(delegate ()
                    {
                        label_byte_nr.Text = "ERROR";
                        label_byte_nr.ForeColor = Color.Red;
                    }));
                }
            }
            catch
            {

            }
            

            try
            {
                if (id_1.Text != "")
                    ids[0] = Convert.ToByte(id_1.Text);
                else
                    ids[0] = 0;

                if (id_2.Text != "")
                    ids[1] = Convert.ToByte(id_2.Text);
                else
                    ids[1] = 0;

                if (id_3.Text != "")
                    ids[2] = Convert.ToByte(id_3.Text);
                else
                    ids[2] = 0;

                if (id_4.Text != "")
                    ids[3] = Convert.ToByte(id_4.Text);
                else
                    ids[3] = 0;
            }
            catch
            {
                MessageBox.Show("Addresses of slaves must be numbers between 0 and 255.", "Bad slave address!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (test_1.Text != "")
                    tests[0] = Convert.ToByte(test_1.Text);
                else
                    tests[0] = 0;

                if (test_2.Text != "")
                    tests[1] = Convert.ToByte(test_2.Text);
                else
                    tests[1] = 0;

                if (test_3.Text != "")
                    tests[2] = Convert.ToByte(test_3.Text);
                else
                    tests[2] = 0;

                if (test_4.Text != "")
                    tests[3] = Convert.ToByte(test_4.Text);
                else
                    tests[3] = 0;
            }
            catch
            {
                MessageBox.Show("Tests numbers of slaves must be numbers between 0 and 255.", "Bad test number!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (mess[0] == 0x0)
                    Broadcast(ref mess);
            }
            catch
            {
                MessageBox.Show("Unknown error.", "Erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Slave1(ref mess);
            Slave2(ref mess);
            Slave3(ref mess);
            Slave4(ref mess);
        }

        public void Broadcast(ref byte[] message)
        {
            byte function = message[1];
            Int32 data1 = message[2]<<8 | message[3];
            Int32 data2 = message[4]<<8 | message[5];

            MessageBox.Show("Master sent a broadcast. Function " + function.ToString() + " with params: " + data1.ToString() + " and " + data2.ToString() + ".", "Broadcast!", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public void Slave1(ref byte[] message)
        {
            if (! checkCRC(message))
                return;

            if (message[0] != ids[0])
                return;

            switch (message[1])
            {
                case 1:
                    byte[] respons1 = new byte[7];
                    respons1[0] = ids[0];
                    respons1[1] = 1;
                    readCoils(message[2] << 8 | message[3], message[4], ref respons1[2], ref respons1[3], ref respons1[4], ref coils1, ref coils1num);
                     createCRC16(respons1, ref crc);
                    respons1[5] = crc[0];
                    respons1[6] = crc[1];
                     serial.Write(respons1, 0, 7);
                    break;
                case 4:
                    byte[] respons4 = new byte[5+2*message[4]];
                    respons4[0] = ids[0];
                    respons4[1] = 4;
                    readRegisters(message[2] << 8 | message[3], message[4], ref respons4, ref registers1, ref registers1num);
                     createCRC16(respons4, ref crc);
                    respons4[3 + 2 * message[4]] = crc[0];
                    respons4[4 + 2 * message[4]] = crc[1];
                     serial.Write(respons4, 0, 5 + 2 * message[4]);
                    break;
                case 8:
                    byte [] respons8 = new byte[6];
                    respons8[0] = ids[0];
                    respons8[1] = 8;
                    respons8[2] = 5;
                    respons8[3] = tests[0];
                     createCRC16(respons8, ref crc);
                    respons8[4] = crc[0];
                    respons8[5] = crc[1];
                     serial.Write(respons8, 0, 6);
                    break;
                case 15:
                    byte[] respons15 = new byte[5];
                    respons15[0] = ids[0];
                    respons15[1] = 15;
                    respons15[2] = writeCoils(message[2] << 8 | message[3], message[4] << 8 | message[5], message[6] << 8 | message[7], ref coils1, ref coils1num);
                     createCRC16(respons15, ref crc);
                    respons15[3] = crc[0];
                    respons15[4] = crc[1];
                     serial.Write(respons15, 0, 5);
                    break;
                case 16:
                    int[] data = new int[message[4] << 8 | message[5]];
                    for(int i = 0; i< (message[4] << 8 | message[5]); i++)
                    {
                        data[i] = message[6+2*i] << 8 | message[7+2*i];
                    }
                    byte[] respons16 = new byte[5];
                    respons16[0] = ids[0];
                    respons16[1] = 16;
                    respons16[2] = writeRegisters(message[2] << 8 | message[3], message[4] << 8 | message[5], data, ref registers1, ref registers1num);
                     createCRC16(respons16, ref crc);
                    respons16[3] = crc[0];
                    respons16[4] = crc[1];
                     serial.Write(respons16, 0, 5);
                    break;
                default:
                    break;
            }

        }

        public void Slave2(ref byte[] message)
        {
            if (! checkCRC(message))
                return;

            if (message[0] != ids[1])
                return;

            switch (message[1])
            {
                case 1:
                    byte[] respons1 = new byte[7];
                    respons1[0] = ids[1];
                    respons1[1] = 1;
                    readCoils(message[2] << 8 | message[3], message[4], ref respons1[2], ref respons1[3], ref respons1[4], ref coils2, ref coils2num);
                     createCRC16(respons1, ref crc);
                    respons1[5] = crc[0];
                    respons1[6] = crc[1];
                     serial.Write(respons1, 0, 7);
                    break;
                case 4:
                    byte[] respons4 = new byte[5 + 2 * message[4]];
                    respons4[0] = ids[1];
                    respons4[1] = 4;
                    readRegisters(message[2] << 8 | message[3], message[4], ref respons4, ref registers2, ref registers2num);
                     createCRC16(respons4, ref crc);
                    respons4[3 + 2 * message[4]] = crc[0];
                    respons4[4 + 2 * message[4]] = crc[1];
                     serial.Write(respons4, 0, 5 + 2 * message[4]);
                    break;
                case 8:
                    byte[] respons = new byte[6];
                    respons[0] = ids[1];
                    respons[1] = 8;
                    respons[2] = 5;
                    respons[3] = tests[1];
                     createCRC16(respons, ref crc);
                    respons[4] = crc[0];
                    respons[5] = crc[1];
                     serial.Write(respons, 0, 6);
                    break;
                case 15:
                    byte[] respons15 = new byte[5];
                    respons15[0] = ids[1];
                    respons15[1] = 15;
                    respons15[2] = writeCoils(message[2] << 8 | message[3], message[4] << 8 | message[5], message[6] << 8 | message[7], ref coils2, ref coils2num);
                     createCRC16(respons15, ref crc);
                    respons15[3] = crc[0];
                    respons15[4] = crc[1];
                     serial.Write(respons15, 0, 5);
                    break;
                case 16:
                    int[] data = new int[message[4] << 8 | message[5]];
                    for (int i = 0; i < (message[4] << 8 | message[5]); i++)
                    {
                        data[i] = message[6 + 2 * i] << 8 | message[7 + 2 * i];
                    }
                    byte[] respons16 = new byte[5];
                    respons16[0] = ids[1];
                    respons16[1] = 16;
                    respons16[2] = writeRegisters(message[2] << 8 | message[3], message[4] << 8 | message[5], data, ref registers2, ref registers2num);
                     createCRC16(respons16, ref crc);
                    respons16[3] = crc[0];
                    respons16[4] = crc[1];
                     serial.Write(respons16, 0, 5);
                    break;
                default:
                    break;
            }
        }

        public void Slave3(ref byte[] message)
        {
            if (! checkCRC(message))
                return;

            if (message[0] != ids[2])
                return;

            switch (message[1])
            {
                case 1:
                    byte[] respons1 = new byte[7];
                    respons1[0] = ids[2];
                    respons1[1] = 1;
                    readCoils(message[2] << 8 | message[3], message[4], ref respons1[2], ref respons1[3], ref respons1[4], ref coils3, ref coils3num);
                     createCRC16(respons1, ref crc);
                    respons1[5] = crc[0];
                    respons1[6] = crc[1];
                     serial.Write(respons1, 0, 7);
                    break;
                case 4:
                    byte[] respons4 = new byte[5 + 2 * message[4]];
                    respons4[0] = ids[2];
                    respons4[1] = 4;
                    readRegisters(message[2] << 8 | message[3], message[4], ref respons4, ref registers3, ref registers3num);
                     createCRC16(respons4, ref crc);
                    respons4[3 + 2 * message[4]] = crc[0];
                    respons4[4 + 2 * message[4]] = crc[1];
                     serial.Write(respons4, 0, 5 + 2 * message[4]);
                    break;
                case 8:
                    byte[] respons = new byte[6];
                    respons[0] = ids[2];
                    respons[1] = 8;
                    respons[2] = 5;
                    respons[3] = tests[2];
                     createCRC16(respons, ref crc);
                    respons[4] = crc[0];
                    respons[5] = crc[1];
                     serial.Write(respons, 0, 6);
                    break;
                case 15:
                    byte[] respons15 = new byte[5];
                    respons15[0] = ids[2];
                    respons15[1] = 15;
                    respons15[2] = writeCoils(message[2] << 8 | message[3], message[4] << 8 | message[5], message[6] << 8 | message[7], ref coils3, ref coils3num);
                     createCRC16(respons15, ref crc);
                    respons15[3] = crc[0];
                    respons15[4] = crc[1];
                     serial.Write(respons15, 0, 5);
                    break;
                case 16:
                    int[] data = new int[message[4] << 8 | message[5]];
                    for (int i = 0; i < (message[4] << 8 | message[5]); i++)
                    {
                        data[i] = message[6 + 2 * i] << 8 | message[7 + 2 * i];
                    }
                    byte[] respons16 = new byte[5];
                    respons16[0] = ids[2];
                    respons16[1] = 16;
                    respons16[2] = writeRegisters(message[2] << 8 | message[3], message[4] << 8 | message[5], data, ref registers3, ref registers3num);
                     createCRC16(respons16, ref crc);
                    respons16[3] = crc[0];
                    respons16[4] = crc[1];
                     serial.Write(respons16, 0, 5);
                    break;
                default:
                    break;
            }
        }

        public void Slave4(ref byte[] message)
        {
            if (! checkCRC(message))
                return;

            if (message[0] != ids[3])
                return;

            switch (message[1])
            {
                case 1:
                    byte[] respons1 = new byte[7];
                    respons1[0] = ids[3];
                    respons1[1] = 1;
                    readCoils(message[2] << 8 | message[3], message[4], ref respons1[2], ref respons1[3], ref respons1[4], ref coils4, ref coils4num);
                     createCRC16(respons1, ref crc);
                    respons1[5] = crc[0];
                    respons1[6] = crc[1];
                     serial.Write(respons1, 0, 7);
                    break;
                case 4:
                    byte[] respons4 = new byte[5 + 2 * message[4]];
                    respons4[0] = ids[3];
                    respons4[1] = 4;
                    readRegisters(message[2] << 8 | message[3], message[4], ref respons4, ref registers4, ref registers4num);
                     createCRC16(respons4, ref crc);
                    respons4[3 + 2 * message[4]] = crc[0];
                    respons4[4 + 2 * message[4]] = crc[1];
                     serial.Write(respons4, 0, 5 + 2 * message[4]);
                    break;
                case 8:
                    byte[] respons = new byte[6];
                    respons[0] = ids[3];
                    respons[1] = 8;
                    respons[2] = 5;
                    respons[3] = tests[3];
                     createCRC16(respons, ref crc);
                    respons[4] = crc[0];
                    respons[5] = crc[1];
                     serial.Write(respons, 0, 6);
                    break;
                case 15:
                    byte[] respons15 = new byte[5];
                    respons15[0] = ids[3];
                    respons15[1] = 15;
                    respons15[2] = writeCoils(message[2] << 8 | message[3], message[4] << 8 | message[5], message[6] << 8 | message[7], ref coils4, ref coils4num);
                     createCRC16(respons15, ref crc);
                    respons15[3] = crc[0];
                    respons15[4] = crc[1];
                     serial.Write(respons15, 0, 5);
                    break;
                case 16:
                    int[] data = new int[message[4] << 8 | message[5]];
                    for (int i = 0; i < (message[4] << 8 | message[5]); i++)
                    {
                        data[i] = message[6 + 2 * i] << 8 | message[7 + 2 * i];
                    }
                    byte[] respons16 = new byte[5];
                    respons16[0] = ids[3];
                    respons16[1] = 16;
                    respons16[2] = writeRegisters(message[2] << 8 | message[3], message[4] << 8 | message[5], data, ref registers4, ref registers4num);
                     createCRC16(respons16, ref crc);
                    respons16[3] = crc[0];
                    respons16[4] = crc[1];
                     serial.Write(respons16, 0, 5);
                    break;
                default:
                    break;
            }
        }

        private void readCoils(int first, int number, ref byte ans, ref byte ret1, ref byte ret2, ref CheckBox[] coils, ref int[] coilsnum)
        {
            int ret = 0;
            if (first < coilsnum[0] || first + number > coilsnum[15] + 1)
            {
                ans = 2;
                return;
            }

            int mask = 1;
            for (int i = 0; i < 16; i++)
            {
                if (coilsnum[i] >= first && coilsnum[i] < first + number)
                {
                    CheckBox cb = coils[i];
                    if (cb.Checked == true)
                    {
                        ret |= mask;
                    }
                    mask <<= 1;
                }
            }
            ans = 5;
            ret1 = (byte)(ret >> 8);
            ret2 = (byte)ret;
        }

        private void readRegisters(int first, int number, ref byte[] ans, ref TextBox[] regs, ref int[] regsnum)
        {
            if (first < regsnum[0] || first + number > regsnum[9] + 1)
            {
                ans[2] = 2;
                return;
            }

            int j = 0;
            for(int i =  0; i<10; i++)
            {
                if (regsnum[i] >= first && regsnum[i] < first + number)
                {
                    try
                    {
                        Int32 d = Convert.ToInt32(regs[i].Text);
                        if (d < 0)
                            throw new IndexOutOfRangeException();
                        ans[3 + 2 * j] = (byte)(Convert.ToInt64(regs[i].Text) >> 8);
                        ans[4 + 2 * j] = (byte)(Convert.ToInt64(regs[i].Text));
                    }
                    catch
                    {
                        ans[3 + 2 * j] = 0;
                        ans[4 + 2 * j] = 0;
                    }
                    
                    j++;
                }

            }

            ans[2] = 5;
        }

        private byte writeCoils(int first, int number, int data, ref CheckBox [] coils, ref int [] coilsnum)
        {
            int mask = data;
            if (first < coilsnum[0] || first + number > coilsnum[15]+1)
                return 2;

            for (int i = 0; i < 16; i++)
            {
                if (coilsnum[i] >= first && coilsnum[i] < first + number)
                {
                    CheckBox cb = coils[i];
                    if (mask%2 == 1)
                    {
                        cb.Invoke(new Action(delegate ()
                        {
                            cb.Checked = true;
                        }));
                    }
                    else
                    {
                        cb.Invoke(new Action(delegate ()
                        {
                            cb.Checked = false;
                        }));
                    }
                    mask /= 2;
                }
            }
            return 5;
        }

        private byte writeRegisters(int first, int number, int [] data, ref TextBox [] registers, ref int [] registersnum)
        {
            if (first < registersnum[0] || first + number > registersnum[9] + 1)
                return 2;

            var que = new Queue(data);

            for (int i = 0; i < 10; i++)
            {
                if (registersnum[i] >= first && registersnum[i] < first + number)
                {
                    TextBox tb = registers[i];
                
                    tb.Invoke(new Action(delegate ()
                    {
                        tb.Text = que.Dequeue().ToString();
                        
                    }));
                }
            }
            return 5;

        }

        private void ModBusSlaveInterface_Load(object sender, EventArgs e) { }

        private void groupBox5_Enter(object sender, EventArgs e) { }

        private void textBox13_TextChanged(object sender, EventArgs e) { }

        private void a0_CheckedChanged(object sender, EventArgs e) { }

        private void label2_Click(object sender, EventArgs e) { }

        private void label11_Click(object sender, EventArgs e) { }
    }
}
