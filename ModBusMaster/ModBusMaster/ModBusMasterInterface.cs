using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace ModBusMaster
{
    public partial class ModBusMasterInterface : Form
    {
        ModBusMaster modbus = new ModBusMaster();
        string data;

        CheckBox[] coils = new CheckBox[16];
        TextBox[] registers = new TextBox[10];

        public ModBusMasterInterface()
        {
            InitializeComponent();
            LoadListboxes();
            LoadCoilsAndRegs();
        }

        private void LoadCoilsAndRegs()
        {
            coils[0] = x0;
            coils[1] = x1;
            coils[2] = x2;
            coils[3] = x3;
            coils[4] = x4;
            coils[5] = x5;
            coils[6] = x6;
            coils[7] = x7;
            coils[8] = x8;
            coils[9] = x9;
            coils[10] = x10;
            coils[11] = x11;
            coils[12] = x12;
            coils[13] = x13;
            coils[14] = x14;
            coils[15] = x15;

            registers[0] = y0;
            registers[1] = y1;
            registers[2] = y2;
            registers[3] = y3;
            registers[4] = y4;
            registers[5] = y5;
            registers[6] = y6;
            registers[7] = y7;
            registers[8] = y8;
            registers[9] = y9;

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

            label_test.Text = "";
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            if (modbus.open(serialPorts.SelectedItem.ToString(), Convert.ToInt32(baudRates.SelectedItem.ToString()), 8, Parity.Odd, StopBits.One))
            {
                label_open.Text = "Port is opened";
                label_open.ForeColor = Color.Green;
                label_connect.Text = "Master is connected to " + serialPorts.SelectedItem.ToString() + ".";
                label_connect.ForeColor = Color.Green;

                // modbus.serial.DataReceived += serial_DataReceived;
            }
            else
            {
                label_open.Text = "Port is not opened";
                label_open.ForeColor = Color.Red;
                label_connect.Text = "Master is not connected.";
                label_connect.ForeColor = Color.Red;
            }

            label_test.Text = "";
        }

        // read self test
        private void button_test_Click(object sender, EventArgs e)
        {
            byte id = 0, number = 0, result = 0;

            try
            {
                id = Convert.ToByte(text_box_id.Text);
                if (id < 1 || id > 255)
                {
                    MessageBox.Show("Slave ID must be number between 1 and 255.", "Bad slave ID!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Slave ID must be number between 1 and 255.", "Bad slave ID!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                number = Convert.ToByte(text_box_test_nr.Text);
                if (number < 0 || number > 255)
                {
                    MessageBox.Show("Number test must be number between 0 and 255.", "Bad number test!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Number test must be number between 0 and 255.", "Bad number test!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (modbus.Fun08(id, number, ref result))
                {
                    if (result == 0)
                    {
                        label_test.Text = "Slave " + id.ToString() + " works correctly.";
                        label_test.ForeColor = Color.Green;
                    }
                    else if (result == 1)
                    {
                        label_test.Text = "Slave " + id.ToString() + " not configured.";
                        label_test.ForeColor = Color.Orange;
                    }
                    else if (result == 2)
                    {
                        label_test.Text = "Slave " + id.ToString() + " sends wrong answear.";
                        label_test.ForeColor = Color.Orange;
                    }
                    else
                    {
                        label_test.Text = "Slave " + id.ToString() + " reports error: " + result.ToString() + ".";
                        label_test.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


        /*
                void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
                {
                    data = modbus.serial.ReadExisting();

                    this.Invoke(new EventHandler(displaydata_event));
                }

                private void displaydata_event(object sender, EventArgs e)
                {
                    label_test.Text = data;
                }
        */


        // read coils
        private void button_read_c_Click(object sender, EventArgs e)
        {
            byte id = 0;
            int result = 0;
            Int32 first = Convert.ToInt32(0), numbers = Convert.ToInt32(0);

            try
            {
                id = Convert.ToByte(text_box_id.Text);
                if (id < 1 || id > 255)
                {
                    MessageBox.Show("Slave ID must be number between 1 and 255.", "Bad slave ID!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Slave ID must be number between 1 and 255.", "Bad slave ID!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                first = Convert.ToInt32(text_box_f_coil.Text);
                if (first < 0 || first > 65535)
                {
                    MessageBox.Show("First coil address must be number between 0 and 65535.", "Bad coils read params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("First coil address must be number between 0 and 65535.", "Bad coils read params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                numbers = Convert.ToByte(text_box_coils_n.Text);
                if (numbers < 0 || numbers > 16)
                {
                    MessageBox.Show("Number of coils must be number between 0 and 16.", "Bad coils read params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Number of coils must be number between 0 and 16.", "Bad coils read params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (modbus.Fun01(id, (short)first, (byte)numbers, ref result))
                {
                    int data = result;
                    for(int i = 0; i<numbers; i++)
                    {
                        if(result%2 == 1)
                        {
                            coils[i].Checked = true;
                        }
                        else
                        {
                            coils[i].Checked = false;
                        }
                        result /= 2;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        // read registers
        private void button_read_r_Click(object sender, EventArgs e)
        {
            byte id = 0;
           
            Int32 first = Convert.ToInt32(0), numbers = Convert.ToInt32(0);

            try
            {
                id = Convert.ToByte(text_box_id.Text);
                if (id < 1 || id > 255)
                {
                    MessageBox.Show("Slave ID must be number between 1 and 255.", "Bad slave ID!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Slave ID must be number between 1 and 255.", "Bad slave ID!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                first = Convert.ToInt32(text_box_f_register.Text);
                if (first < 0 || first > 65535)
                {
                    MessageBox.Show("First register address must be number between 0 and 65535.", "Bad register read params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("First register address must be number between 0 and 65535.", "Bad register read params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                numbers = Convert.ToInt32(text_box_registers_n.Text);
                if (numbers < 0 || numbers > 10)
                {
                    MessageBox.Show("Number of registers must be number between 0 and 10.", "Bad coils read params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Number of registers must be number between 0 and 10.", "Bad coils read params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte[] result = new byte[2 * numbers];
            try
            {
                if (modbus.Fun04(id, (short)first, (byte)numbers, ref result))
                {
                    for(int i = 0; i<numbers; i++)
                    {
                        registers[i].Text = (result[2 * i] << 8 | result[2 * i + 1]).ToString();
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        // write coils
        private void button_write_c_Click(object sender, EventArgs e)
        {
            byte id = 0, result = 0, number = 0;
            Int32 first = Convert.ToInt32(0), numbers = Convert.ToInt32(0);

            try
            {
                id = Convert.ToByte(text_box_id.Text);
                if (id < 1 || id > 255)
                {
                    MessageBox.Show("Slave ID must be number between 1 and 255.", "Bad slave ID!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Slave ID must be number between 1 and 255.", "Bad slave ID!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                first = Convert.ToInt32(text_box_nc_to_w.Text);
                if (first < 0 || first > 65535)
                {
                    MessageBox.Show("First coil address must be number between 0 and 65535.", "Bad coils write params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("First coil address must be number between 0 and 65535.", "Bad coils write params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                number = Convert.ToByte(text_box_num_c.Text);
                if (number < 0 || number > 255)
                {
                    MessageBox.Show("Number of coils to write must be number between 0 and 255.", "Bad number!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Number of coils to write must be number between 0 and 255.", "Bad number!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Int16 status = 0;
            if (w0.Checked)
                status |= 1;
            if (w1.Checked)
                status |= 2;
            if (w2.Checked)
                status |= 4;
            if (w3.Checked)
                status |= 8;
            if (w4.Checked)
                status |= 16;
            if (w5.Checked)
                status |= 32;
            if (w6.Checked)
                status |= 64;
            if (w7.Checked)
                status |= 128;
            if (w8.Checked)
                status |= 256;
            if (w9.Checked)
                status |= 512;

            try
            {
                modbus.Fun15(id, (short)first, number, status, ref result);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }

        }

        // write registers
        private void button4_Click(object sender, EventArgs e)
        {
            byte id = 0, result = 0;
            Int32 first = Convert.ToInt32(0), numbers = Convert.ToInt32(0);
            Int32 number = 0, n;

            try
            {
                id = Convert.ToByte(text_box_id.Text);
                if (id < 1 || id > 255)
                {
                    MessageBox.Show("Slave ID must be number between 1 and 255.", "Bad slave ID!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Slave ID must be number between 1 and 255.", "Bad slave ID!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                first = Convert.ToInt32(text_box_nr_to_w.Text);
                if (first < 0 || first > 65535)
                {
                    MessageBox.Show("First register address must be number between 0 and 65535.", "Bad registers write params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("First register address must be number between 0 and 65535.", "Bad registers write params!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Int32[] data = new Int32[6];

            try
            {
                n = Convert.ToInt32(z0.Text);
                if (n < 0 || n > 65535)
                {
                    MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    data[0] = n;
                    number++;
                }
            }
            catch
            {
                MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (z1.Text != "")
                {
                    n = Convert.ToInt32(z1.Text);
                    if (n < 0 || n > 65535)
                    {
                        MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        data[1] = n;
                        number++;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (z2.Text != "")
                {
                    n = Convert.ToInt32(z2.Text);
                    if (n < 0 || n > 65535)
                    {
                        MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        data[2] = n;
                        number++;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (z3.Text != "")
                {
                    n = Convert.ToInt32(z3.Text);
                    if (n < 0 || n > 65535)
                    {
                        MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        data[3] = n;
                        number++;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (z4.Text != "")
                {
                    n = Convert.ToInt32(z4.Text);
                    if (n < 0 || n > 65535)
                    {
                        MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        data[4] = n;
                        number++;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (z5.Text != "")
                {
                    n = Convert.ToInt32(z5.Text);
                    if (n < 0 || n > 65535)
                    {
                        MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        data[5] = n;
                        number++;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Data to write to register must be number between 0 an 65535.", "Bad registers value!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                modbus.Fun16(id, (short)first, (byte)number, data, ref result);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        // Broadcast
        private void bc_button_Click(object sender, EventArgs e)
        {
            byte fun;
            Int32 data1, data2;

            try
            {
                fun = Convert.ToByte(bc_f.Text);
            }
            catch
            {
                MessageBox.Show("Number of function must be number between 0 and 255.", "Bad number of function!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                data1 = Convert.ToInt32(bc_d1.Text);
                if (data1 < 0 || data1 > 65535)
                {
                    MessageBox.Show("Data 1 must be number between 0 and 65535.", "Bad data number!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Data 1 must be number between 0 and 65535.", "Bad data number!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                data2 = Convert.ToInt32(bc_d2.Text);
                if (data2 < 0 || data2 > 65535)
                {
                    MessageBox.Show("Data 1 must be number between 0 and 65535.", "Bad data number!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Data 2 must be number between 0 and 65535.", "Bad data number!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                modbus.Broadcast(fun, data1, data2);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        private void button_reset_c_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                coils[i].Checked = false;
            }
            for (int i = 0; i < 10; i++)
            {
                registers[i].Text = "";
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {


        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {


        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void portLabel_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_2(object sender, EventArgs e)
        {

        }



        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void x0_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void w0_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void table_w_Paint(object sender, PaintEventArgs e)
        {

        }

        private void x12_CheckedChanged(object sender, EventArgs e)
        {

        }

        
    }
}
