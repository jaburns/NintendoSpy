using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TotalPhase;

namespace NintendoSpy.Readers
{
    sealed public class XboxReader : IControllerReader
    {
        public class USBPacket
        {
            public USBPacket()
            {
                Length = 0;
                Packet = new byte[1024];
            }

            public int Length;
            public byte[] Packet;
        }

        public event StateEventHandler ControllerStateChanged;
        public event EventHandler ControllerDisconnected;

        private readonly int hBeagle;
        private bool isOpened = false;
        private bool isEnabled = false;
        private ConcurrentQueue<USBPacket> packetsToBeProcessed = new ConcurrentQueue<USBPacket>();
        private USBPacket[] packetPool;
        private int currentPacketInPool = 0;
        private bool inShutdown = false;

        static float ReadTrigger(byte input)
        {
            return (float)(input) / 256;
        }

        static float ReadStick(short input)
        {
            return (float)input / short.MaxValue;
        }

        public static List<uint> GetDevices()
        {
            var result = new List<uint>();
            ushort[] ports = new ushort[16];
            uint[] unique_ids = new uint[16];
            int nelem = 16;

            if (Environment.Is64BitProcess)
            {
                int count = BeagleApi.bg_find_devices_ext(nelem, ports, nelem, unique_ids);
                for (uint i = 0; i < count; i++)
                {
                    result.Add(ports[i]);
                }
            }

            return result;
        }

        public XboxReader(int controllerId)
        {
            if (!Environment.Is64BitProcess)
                throw new IOException("XboxReader is only support in 64 bit RetroSpy!");

            // Setup the Beagle
            hBeagle = BeagleApi.bg_open(controllerId);
            if (hBeagle <= 0)
            {
                throw new IOException(String.Format("XboxReader could not find Beagle USB Protocol Analyzer on port {0}.", controllerId));
            }

            isOpened = true;

            int samplerate = 0;
            samplerate = BeagleApi.bg_samplerate(hBeagle, samplerate);
            if (samplerate < 0)
            {
                Finish();
                throw new IOException(String.Format("XboxReader error: {0:s}\n", BeagleApi.bg_status_string(samplerate)));
            }

            BeagleApi.bg_timeout(hBeagle, milliseconds: 500);  
            BeagleApi.bg_latency(hBeagle, milliseconds: 0);  
            BeagleApi.bg_target_power(hBeagle, BeagleApi.BG_TARGET_POWER_OFF);

            if (BeagleApi.bg_enable(hBeagle, BeagleProtocol.BG_PROTOCOL_USB) != (int)BeagleStatus.BG_OK)
            {
                Finish();
                throw new IOException("XboxReader error: could not enable USB capture\n");
            }
            isEnabled = true;

            packetPool = new USBPacket[100];

            for (int i = 0; i < 100; ++i)
                packetPool[i] = new USBPacket();

            Thread usbParsingThread = new Thread(ProcessPacketWorker);
            Thread usbReadingThread = new Thread(ReadUSBWorker);
            usbParsingThread.Start(this);
            usbReadingThread.Start(this);
        }

        static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "start", "back", "l3", "r3"
        };

        static readonly string[] ANALOG_BUTTONS = {
            "a", "b", "x", "y", "black", "white", "trig_l", "trig_r"
        };

        static readonly string[] STICKS = {
            "lstick_x", "lstick_y", "rstick_x", "rstick_y"
        };

        public static void ReadUSBWorker(object data)
        {
            XboxReader reader = ((XboxReader)data); 

            USBPacket packet = ((XboxReader)data).packetPool[((XboxReader)data).currentPacketInPool];
            ((XboxReader)data).currentPacketInPool += ((XboxReader)data).currentPacketInPool;
            ((XboxReader)data).currentPacketInPool %= 100;

            while (!reader.inShutdown)
            {
                uint status = 0;
                uint events = 0;
                ulong timeSop = 0;
                ulong timeDuration = 0;
                uint timeDataOffset = 0;

                packet.Length = BeagleApi.bg_usb2_read(
                       ((XboxReader)data).hBeagle, ref status, ref events, ref timeSop,
                       ref timeDuration, ref timeDataOffset, 1024, packet.Packet);

                if (status != BeagleApi.BG_READ_OK)
                {
                    reader.ControllerDisconnected?.Invoke(reader, EventArgs.Empty);
                }
                else
                {
                    if (packet.Length == 23 
                        && (packet.Packet[0] == BeagleApi.BG_USB_PID_DATA1 || packet.Packet[0] == BeagleApi.BG_USB_PID_DATA0) 
                        && packet.Packet[1] == 0x00 
                        && packet.Packet[2] == 0x14)
                    {
                        reader.packetsToBeProcessed.Enqueue(packet);
                    }
                }
            }
        }

        public static void ProcessPacketWorker(object data)
        {
            XboxReader reader = ((XboxReader)data);

            while (!reader.inShutdown)
            {
                if (reader.packetsToBeProcessed.Count > 0)
                {
                    reader.packetsToBeProcessed.TryDequeue(out USBPacket packet);

                    var outState = new ControllerStateBuilder();

                    for (int i = 0; i < 8; ++i)
                    {
                        outState.SetButton(BUTTONS[i], (packet.Packet[3] & (1 << i)) != 0);
                    }

                    for (int i = 5; i < 13; ++i)
                    {
                        outState.SetButton(ANALOG_BUTTONS[i - 5], packet.Packet[i] > 0);
                        outState.SetAnalog(ANALOG_BUTTONS[i - 5], ReadTrigger(packet.Packet[i]));
                    }

                    int j = 0;
                    for (int i = 0; i < 4; ++i)
                    {
                        short val = packet.Packet[13 + j];
                        val += (short)(packet.Packet[14 + j] << 8);
                        outState.SetAnalog(STICKS[i], ReadStick(val));
                        j += 2;
                    }

                    reader.ControllerStateChanged?.Invoke(reader, outState.Build());
                }
            }
        }

        public void Finish()
        {
            inShutdown = true;
            Thread.Sleep(1000);

            if (isEnabled)
                BeagleApi.bg_disable(hBeagle);

            if (isOpened)
                BeagleApi.bg_close(hBeagle);
        }
    }
}
