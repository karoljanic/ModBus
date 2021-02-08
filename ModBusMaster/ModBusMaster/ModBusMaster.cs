using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;

namespace ModBusMaster
{
    class ModBusMaster
    {
        public SerialPort serial = new SerialPort();

        public ModBusMaster(){ }

        ~ModBusMaster(){ }
 
        public bool open(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits){
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
                catch (Exception e)
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
            if (serial.IsOpen){
                try
                {
                    serial.Close();
                }
                catch (Exception e)
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

        private void createCRC16(byte[] message, ref byte[] CRC)
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

        private bool checkResponse(byte[] receivedAnswer)
        {
            byte[] CRC = new byte[2];
            createCRC16(receivedAnswer, ref CRC);
            if (CRC[0] == receivedAnswer[receivedAnswer.Length - 2] && CRC[1] == receivedAnswer[receivedAnswer.Length - 1])
                return true;
            else
                return false;
        }

        private void receiveResponse(ref byte[] answer)
        {
            for (int i = 0; i < answer.Length; i++)
            {
                answer[i] = (byte)(serial.ReadByte());
            }
        }

        public bool Fun01(byte address, Int16 first, byte number, ref int result)
        {
            if (serial.IsOpen)
            {
                serial.DiscardOutBuffer();
                serial.DiscardInBuffer();
                byte[] message = new byte[7];
                // adres - 1 bajt
                // nr. funkcji - 1 bajt
                // pole danych( typ testu):
                    // pierwsza cewka - 2 bajty
                    // ile cewek - 1 bajt
                // CRC - 2 bajty
                message[0] = address;
                message[1] = 0x1;
                message[2] = (byte)(first >> 8);
                message[3] = (byte)(first);
                message[4] = number;

                byte[] CRC = new byte[2];
                createCRC16(message, ref CRC);
                message[5] = CRC[0];
                message[6] = CRC[1];

                byte[] response = new byte[7];
                try
                {
                    Thread.Sleep(35); // cisza na łączu 35ms
                    serial.Write(message, 0, message.Length);

                    Thread.Sleep(35);
                    receiveResponse(ref response);
                }
                catch
                {
                    MessageBox.Show("No answer received.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (checkResponse(response) && response[2] == 5)
                {
                    MessageBox.Show("Message successfully sent and received. Coils readed", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    result = response[3] << 8 | response[4];
                }
                else if(response[2] == 2)
                {
                    MessageBox.Show("Selected addresses for reading are blocked.", "Bad addresses.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    result = 0;
                    return false;
                }
                else
                {
                    MessageBox.Show("Unknown error.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    result = 0;
                    return false;
                }

                return true;
            }
            else
            {
                MessageBox.Show("No connection to any serial port .", "No connection!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
                
        }

        public bool Fun04(byte address, Int16 first, byte number, ref byte[] result)
        {
            if (serial.IsOpen)
            {
                serial.DiscardOutBuffer();
                serial.DiscardInBuffer();
                byte[] message = new byte[7];
                // adres - 1 bajt
                // nr. funkcji - 1 bajt
                // pole danych( typ testu):
                    // pierwszy rejest - 2 bajty
                    // ile rejestrow - 1 bajt
                // CRC - 2 bajty
                message[0] = address;
                message[1] = 0x4;
                message[2] = (byte)(first >> 8);
                message[3] = (byte)(first);
                message[4] = number;

                byte[] CRC = new byte[2];
                createCRC16(message, ref CRC);
                message[5] = CRC[0];
                message[6] = CRC[1];

                byte[] response = new byte[5+2* message[4]];
                try
                {
                    Thread.Sleep(35); // cisza na łączu 35ms
                    serial.Write(message, 0, message.Length);

                    Thread.Sleep(35);
                    receiveResponse(ref response);    
                }
                catch
                {
                    MessageBox.Show("No answer received.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (checkResponse(response) && response[2] == 5)
                {
                    MessageBox.Show("Message successfully sent and received. Registers readed", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    for(int i = 0; i<number; i++)
                    {
                        result[2 * i] = response[3 + 2 * i];
                        result[2 * i+1] = response[4 + 2 * i];

                    }
                }
                else if (response[2] == 2)
                {
                    MessageBox.Show("Selected addresses for reading are blocked.", "Bad addresses.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                else
                {
                    MessageBox.Show("Unknown error.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                return true;
            }
            else
            {
                MessageBox.Show("No connection to any serial port .", "No connection!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool Fun08(byte address, byte test_number, ref byte result)
        {
            if (serial.IsOpen)
            {
                serial.DiscardOutBuffer();
                serial.DiscardInBuffer();
                byte [] message = new byte[5];
                // adres - 1 bajt
                // nr. funkcji - 1 bajt
                // pole danych:
                    // typ testu - 1 bajt
                // CRC - 2 bajty
                message[0] = address;
                message[1] = 0x8;
                message[2] = test_number;

                byte[] CRC = new byte[2];
                createCRC16(message, ref CRC);
                message[3] = CRC[0];
                message[4] = CRC[1];

                byte[] response = new byte[6];
                try
                {
                    Thread.Sleep(35); // cisza na łączu 35ms
                    serial.Write(message, 0, message.Length);

                    Thread.Sleep(35);
                    receiveResponse(ref response);
                }
                catch
                {
                    MessageBox.Show("No answer received.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (checkResponse(response) && response[2] == 5)
                {
                    MessageBox.Show("Message successfully sent and received.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    result = response[3];
                }
                else
                {
                    MessageBox.Show("Unknown error.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    result = 2;
                }
                    
                result = response[3];
                

                return true;
            }
            else
            {
                MessageBox.Show("No connection to any serial port .", "No connection!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool Fun15(byte address, Int16 first, Int16 number, Int16 status, ref byte result)
        {
            if (serial.IsOpen)
            {
                serial.DiscardOutBuffer();
                serial.DiscardInBuffer();
                double n = Math.Ceiling((double)(number*0.125));
                byte[] message = new byte[10];
                // adres - 1 bajt
                // nr. funkcji - 1 bajt
                // pole danych:
                    // pierwszy rejest - 2 bajty
                    // ile rejestrow - 2 bajty
                    // status tych rejestrow - ceil(ile_rejestrow / 8) bajtow
                // CRC - 2 bajty
                message[0] = address;
                message[1] = 0xF;
                message[2] = (byte)(first >> 8);
                message[3] = (byte)(first);
                message[4] = (byte)(number >> 8);
                message[5] = (byte)(number);
                message[6] = (byte)(status >> 8);
                message[7] = (byte)(status);

                byte[] CRC = new byte[2];
                createCRC16(message, ref CRC);
                message[8] = CRC[0];
                message[9] = CRC[1];

                byte[] response = new byte[5];
                try
                {
                    Thread.Sleep(35); // cisza na łączu 35ms
                    serial.Write(message, 0, message.Length);

                    Thread.Sleep(35);
                    receiveResponse(ref response);
                }
                catch
                {
                    MessageBox.Show("No answer received.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (checkResponse(response))
                {
                    if (response[2] == 5)
                        MessageBox.Show("Message successfully sent and received. Coils written successfully.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (response[2] == 2)
                        MessageBox.Show("Selected addresses for writing are blocked.", "Bad addresses.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Unknow error.", "Eror!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("Unknown error response.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                result = 0;
                return true;
            }
            else
            {
                MessageBox.Show("No connection to any serial port .", "No connection!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool Fun16(byte address, Int16 first, byte number, Int32 [] data, ref byte result)
        {
            if (serial.IsOpen)
            {
                serial.DiscardOutBuffer();
                serial.DiscardInBuffer();
                byte[] message = new byte[number * 2 + 8];
                // adres - 1 bajt
                // nr. funkcji - 1 bajt
                // pole danych:
                // pierwszy rejest - 2 bajty
                // ile rejestrow - 2 bajty
                // dane do rejestrow - 2 * number bajtow
                // CRC - 2 bajty
                message[0] = address;
                message[1] = 0x10;
                message[2] = (byte)(first >> 8);
                message[3] = (byte)(first);
                message[4] = (byte)(number >> 8);
                message[5] = (byte)(number);
                for (int i = 0; i < number; i++)
                {
                    message[6 + 2 * i] = (byte)(data[i] >> 8);
                    message[7 + 2 * i] = (byte)(data[i]);
                }

                byte[] CRC = new byte[2];
                createCRC16(message, ref CRC);
                message[number * 2 + 6] = CRC[0];
                message[number * 2 + 7] = CRC[1];

                byte[] response = new byte[5];
                try
                {
                    Thread.Sleep(35); // cisza na łączu 35ms
                    serial.Write(message, 0, message.Length);

                    Thread.Sleep(35);
                    receiveResponse(ref response);
                }
                catch
                {
                    MessageBox.Show("No answer received.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (checkResponse(response))
                {
                    if (response[2] == 5)
                        MessageBox.Show("Message successfully sent and received. Registers written successfully.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (response[2] == 2)
                        MessageBox.Show("Selected addresses for writing are blocked.", "Bad addresses.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Unknow error.", "Eror!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Unknown error response.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                result = 0;
                return true;
            }
            else
            {
                MessageBox.Show("No connection to any serial port .", "No connection!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool Broadcast(byte fun, Int32 data1, Int32 data2)
        {
            if (serial.IsOpen)
            {
                serial.DiscardOutBuffer();
                serial.DiscardInBuffer();
                byte[] message = new byte[8];
                // adres - 1 bajt
                // nr. funkcji - 1 bajt
                // pole danych:
                    // pole danych 1 - 2 bajty
                    // pole danych 2 - 2 bajty
                // CRC - 2 bajty
                message[0] = 0x0;
                message[1] = fun;
                message[2] = (byte)(data1 >> 8);
                message[3] = (byte)(data1);
                message[4] = (byte)(data2 >> 8);
                message[5] = (byte)(data2);

                byte[] CRC = new byte[2];
                createCRC16(message, ref CRC);
                message[6] = CRC[0];
                message[7] = CRC[1];

                try
                {
                    Thread.Sleep(35); // cisza na łączu 35ms
                    serial.Write(message, 0, message.Length);
                }
                catch
                {
                    MessageBox.Show("Unknown error.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                return true;
            }
            else
            {
                MessageBox.Show("No connection to any serial port .", "No connection!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

    }
}
