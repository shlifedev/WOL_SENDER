using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApp6
{
    class Program
    {

        //패킷 스케일
        const int WOL_PACKET_SCALE = 17;
        //패킷 단위 길이 (6바이트)
        const int WOL_PACKET_UNIT_LENGTH = 6; 
        //패킷 총 사이즈.
        const int WOL_PACKET_SIZE = WOL_PACKET_SCALE * WOL_PACKET_UNIT_LENGTH;


        /// <summary>
        /// 내가 보내야할 Wol패킷을 만든다. (보낼 피시에서 실행)
        /// </summary> 
        private static byte[] MakeWol(byte[] targetMacAdress)
        {
            var packet = new byte[WOL_PACKET_SIZE];

            //현재 내가 타겟으로 잡은 배열 인덱스 인덱스
            int curIndex;
            
            //첫 0~ 6배열은 0xff로 채운다 
            for(int i = 0; i < WOL_PACKET_UNIT_LENGTH; i++)
                packet[i] = 0xff;


            //그 뒤 내 6바이트짜리 맥 주소를 16번 뒤에 반복해서 채운다. 
            for (int cnt = WOL_PACKET_UNIT_LENGTH; cnt < WOL_PACKET_SIZE; cnt += WOL_PACKET_UNIT_LENGTH)
            {
                for (curIndex = 0; curIndex < WOL_PACKET_UNIT_LENGTH; ++curIndex)
                    packet[curIndex + cnt] = targetMacAdress[curIndex];
            }
            return packet;
        }


        private static void Print(byte[] wolPacket)
        {
            Console.WriteLine();

            for (int index = 0; index <= WOL_PACKET_SIZE; index++)
            {
                if(index > (WOL_PACKET_UNIT_LENGTH-1) &&  index % WOL_PACKET_UNIT_LENGTH == 0)
                {
                    Console.Write(String.Format("{0} - {1} 까지의 패킷 :: ", index - WOL_PACKET_UNIT_LENGTH, index)); 
                    for (int readIndex = index - WOL_PACKET_UNIT_LENGTH; readIndex < index; readIndex++)
                    { 
                        Console.Write(String.Format("{0:x2} ", wolPacket[readIndex]));
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine();

        }


        public static bool Verify(byte[] wolPacket)
        {
            for (int i = 0; i < WOL_PACKET_UNIT_LENGTH; i++)
                if (wolPacket[i] != 0xFF)
                {
                    return false;
                }

            if(wolPacket.Length == WOL_PACKET_SIZE) return true;

            return true;
        }



        private static void SendWOLPacket(IPEndPoint target, byte[] wolPacket)
        {
            var cl = new UdpClient();
            cl.Send(wolPacket, wolPacket.Length, target);
        }


        static void Main(string[] args)
        {
            //내 맥주소를 넣어서 WolPacket을 받아옴
            var byteData = MakeWol(new byte[] { 0x70, 0x85, 0xc2 ,0x0a, 0xab ,0x3c });


             
            Print(byteData);  


            Console.WriteLine("정상적인 WOL 패킷인가요 ? => " + Verify(byteData));
        }
    }
}
