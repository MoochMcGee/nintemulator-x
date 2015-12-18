using Nintemulator.Shared;
using System.Text;

namespace Nintemulator.GBA.CPU.Disassemble
{
    public static class ThumbDisassembler
    {
        private static string GetRegisterList(uint register)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < 8; i++, register >>= 1)
            {
                if ((register & 1U) != 0U)
                {
                    sb.AppendFormat("r{0},", i);
                }
            }

            sb.Length--;

            return sb.ToString();
        }

        private static string GetRegisterName(uint register)
        {
            switch (register)
            {
            default:
            case 0U: return "r0";
            case 1U: return "r1";
            case 2U: return "r2";
            case 3U: return "r3";
            case 4U: return "r4";
            case 5U: return "r5";
            case 6U: return "r6";
            case 7U: return "r7";
            case 8U: return "r8";
            case 9U: return "r9";
            case 10U: return "r10";
            case 11U: return "r11";
            case 12U: return "r12";
            case 13U: return "sp";
            case 14U: return "lr";
            case 15U: return "pc";
            }
        }
        private static string GetAddressName(uint address)
        {
            switch (address & 0x0F000000U)
            {
            case 0x00000000U:
            case 0x01000000U: return string.Format("bios[${0:X8}]", address & 0x3FFFU);
            case 0x02000000U: return string.Format("eram[${0:X8}]", address & 0x3FFFFU);
            case 0x03000000U: return string.Format("iram[${0:X8}]", address & 0x7FFFU);
            case 0x04000000U:
                switch (address & 0x3FFU)
                {
                case 0x000U: case 0x001U: return "dispcnt";
                case 0x002U: case 0x003U: return "greenswap";
                case 0x004U: case 0x005U: return "dispstat";
                case 0x006U: case 0x007U: return "vcount";
                case 0x008U: case 0x009U: return "bg0cnt";
                case 0x00AU: case 0x00BU: return "bg1cnt";
                case 0x00CU: case 0x00DU: return "bg2cnt";
                case 0x00EU: case 0x00FU: return "bg3cnt";
                case 0x010U: case 0x011U: return "bg0hofs";
                case 0x012U: case 0x013U: return "bg0vofs";
                case 0x014U: case 0x015U: return "bg1hofs";
                case 0x016U: case 0x017U: return "bg1vofs";
                case 0x018U: case 0x019U: return "bg2hofs";
                case 0x01AU: case 0x01BU: return "bg2vofs";
                case 0x01CU: case 0x01DU: return "bg3hofs";
                case 0x01EU: case 0x01FU: return "bg3vofs";
                case 0x020U: case 0x021U: return "bg2pa";
                case 0x022U: case 0x023U: return "bg2pb";
                case 0x024U: case 0x025U: return "bg2pc";
                case 0x026U: case 0x027U: return "bg2pd";
                case 0x028U: case 0x029U: case 0x02AU: case 0x02BU: return "return bg2x";
                case 0x02CU: case 0x02DU: case 0x02EU: case 0x02FU: return "return bg2y";
                case 0x030U: case 0x031U: return "bg3pa";
                case 0x032U: case 0x033U: return "bg3pb";
                case 0x034U: case 0x035U: return "bg3pc";
                case 0x036U: case 0x037U: return "bg3pd";
                case 0x038U: case 0x039U: case 0x03AU: case 0x03BU: return "return bg3x";
                case 0x03CU: case 0x03DU: case 0x03EU: case 0x03FU: return "return bg3y";
                case 0x040U: case 0x041U: return "win0h";
                case 0x042U: case 0x043U: return "win1h";
                case 0x044U: case 0x045U: return "win0v";
                case 0x046U: case 0x047U: return "win1v";
                case 0x048U: case 0x049U: return "winin";
                case 0x04AU: case 0x04BU: return "winout";
                case 0x04CU: case 0x04DU: return "mosaic";
                case 0x04EU: case 0x04FU: break;
                case 0x050U: case 0x051U: return "bldcnt";
                case 0x052U: case 0x053U: return "bldalpha";
                case 0x054U: case 0x055U: return "bldy";
                case 0x056U: case 0x057U: break;
                case 0x058U: case 0x059U: break;
                case 0x05AU: case 0x05BU: break;
                case 0x05CU: case 0x05DU: break;
                case 0x05EU: case 0x05FU: break;
                //Sound Registers
                case 0x060U: case 0x061U: return "sound1cnt_l";
                case 0x062U: case 0x063U: return "sound1cnt_h";
                case 0x064U: case 0x065U: return "sound1cnt_x";
                case 0x066U: case 0x067U: break;
                case 0x068U: case 0x069U: return "sound2cnt_l";
                case 0x06AU: case 0x06BU: break;
                case 0x06CU: case 0x06DU: return "sound2cnt_h";
                case 0x06EU: case 0x06FU: break;
                case 0x070U: case 0x071U: return "sound3cnt_l";
                case 0x072U: case 0x073U: return "sound3cnt_h";
                case 0x074U: case 0x075U: return "sound3cnt_x";
                case 0x076U: case 0x077U: break;
                case 0x078U: case 0x079U: return "sound4cnt_l";
                case 0x07AU: case 0x07BU: break;
                case 0x07CU: case 0x07DU: return "sound4cnt_h";
                case 0x07EU: case 0x07FU: break;
                case 0x080U: case 0x081U: return "soundcnt_l";
                case 0x082U: case 0x083U: return "soundcnt_h";
                case 0x084U: case 0x085U: return "soundcnt_x";
                case 0x086U: case 0x087U: break;
                case 0x088U: case 0x089U: return "soundbias";
                case 0x08AU: case 0x08BU: break;
                case 0x090U: case 0x091U: return "wave_ram";
                case 0x092U: case 0x093U: return "wave_ram";
                case 0x094U: case 0x095U: return "wave_ram";
                case 0x096U: case 0x097U: return "wave_ram";
                case 0x098U: case 0x099U: return "wave_ram";
                case 0x09AU: case 0x09BU: return "wave_ram";
                case 0x09CU: case 0x09DU: return "wave_ram";
                case 0x09EU: case 0x09FU: return "wave_ram";
                case 0x0A0U: case 0x0A1U: case 0x0A2U: case 0x0A3U: return "fifo_a"; //  40000A0h  4    W    FIFO_A    Channel A FIFO, Data 0-3
                case 0x0A4U: case 0x0A5U: case 0x0A6U: case 0x0A7U: return "fifo_b"; //  40000A4h  4    W    FIFO_B    Channel B FIFO, Data 0-3
                case 0x0A8U: case 0x0A9U: break;
                case 0x0AAU: case 0x0ABU: break;
                case 0x0ACU: case 0x0ADU: break;
                case 0x0AEU: case 0x0AFU: break;
                //DMA Transfer Channels
                case 0x0B0U: case 0x0B1U: case 0x0B2U: case 0x0B3U: return "dma0sad";
                case 0x0B4U: case 0x0B5U: case 0x0B6U: case 0x0B7U: return "dma0dad";
                case 0x0B8U: case 0x0B9U: return "dma0cnt_l";
                case 0x0BAU: case 0x0BBU: return "dma0cnt_h";
                case 0x0BCU: case 0x0BDU: case 0x0BEU: case 0x0BFU: return "dma1sad";
                case 0x0C0U: case 0x0C1U: case 0x0C2U: case 0x0C3U: return "dma1dad";
                case 0x0C4U: case 0x0C5U: return "dma1cnt_l";
                case 0x0C6U: case 0x0C7U: return "dma1cnt_h";
                case 0x0C8U: case 0x0C9U: case 0x0CAU: case 0x0CBU: return "dma2sad";
                case 0x0CCU: case 0x0CDU: case 0x0CEU: case 0x0CFU: return "dma2dad";
                case 0x0D0U: case 0x0D1U: return "dma2cnt_l";
                case 0x0D2U: case 0x0D3U: return "dma2cnt_h";
                case 0x0D4U: case 0x0D5U: case 0x0D6U: case 0x0D7U: return "dma3sad";
                case 0x0D8U: case 0x0D9U: case 0x0DAU: case 0x0DBU: return "dma3dad";
                case 0x0DCU: case 0x0DDU: return "dma3cnt_l";
                case 0x0DEU: case 0x0DFU: return "dma3cnt_h";
                case 0x0E0U: case 0x0E1U: break;
                case 0x0E2U: case 0x0E3U: break;
                case 0x0E4U: case 0x0E5U: break;
                case 0x0E6U: case 0x0E7U: break;
                case 0x0E8U: case 0x0E9U: break;
                case 0x0EAU: case 0x0EBU: break;
                case 0x0ECU: case 0x0EDU: break;
                case 0x0EEU: case 0x0EFU: break;
                //Timer Registers
                case 0x100U: case 0x101U: return "tm0cnt_l";
                case 0x102U: case 0x103U: return "tm0cnt_h";
                case 0x104U: case 0x105U: return "tm1cnt_l";
                case 0x106U: case 0x107U: return "tm1cnt_h";
                case 0x108U: case 0x109U: return "tm2cnt_l";
                case 0x10AU: case 0x10BU: return "tm2cnt_h";
                case 0x10CU: case 0x10DU: return "tm3cnt_l";
                case 0x10EU: case 0x10FU: return "tm3cnt_h";
                case 0x110U: case 0x111U: break;
                case 0x112U: case 0x113U: break;
                case 0x114U: case 0x115U: break;
                case 0x116U: case 0x117U: break;
                case 0x118U: case 0x119U: break;
                case 0x11AU: case 0x11BU: break;
                case 0x11CU: case 0x11DU: break;
                case 0x11EU: case 0x11FU: break;
                // todo: finish these...
                //Serial Communication (1)
                //  4000120h  4    R/W  SIODATA32 SIO Data (Normal-32bit Mode) (shared with below!)
                //  4000120h  2    R/W  SIOMULTI0 SIO Data 0 (Parent)    (Multi-Player Mode)
                //  4000122h  2    R/W  SIOMULTI1 SIO Data 1 (1st Child) (Multi-Player Mode)
                //  4000124h  2    R/W  SIOMULTI2 SIO Data 2 (2nd Child) (Multi-Player Mode)
                //  4000126h  2    R/W  SIOMULTI3 SIO Data 3 (3rd Child) (Multi-Player Mode)
                //  4000128h  2    R/W  SIOCNT    SIO Control Register
                //  400012Ah  2    R/W  SIOMLT_SEND SIO Data (Local of Multi-Player) (shared below)
                //  400012Ah  2    R/W  SIODATA8  SIO Data (Normal-8bit and UART Mode)
                //  400012Ch       -    -         Not used
                //Keypad Input
                case 0x130U: case 0x131U: return "keyinput"; //  4000130h  2    R    KEYINPUT  Key Status
                case 0x132U: case 0x133U: return "keycnt";   //  4000132h  2    R/W  KEYCNT    Key Interrupt Control
                //Serial Communication (2)
                //  4000134h  2    R/W  RCNT      SIO Mode Select/General Purpose Data
                //  4000136h  -    -    IR        Ancient - Infrared Register (Prototypes only)
                //  4000138h       -    -         Not used
                //  4000140h  2    R/W  JOYCNT    SIO JOY Bus Control
                //  4000142h       -    -         Not used
                //  4000150h  4    R/W  JOY_RECV  SIO JOY Bus Receive Data
                //  4000154h  4    R/W  JOY_TRANS SIO JOY Bus Transmit Data
                //  4000158h  2    R/?  JOYSTAT   SIO JOY Bus Receive Status
                //  400015Ah       -    -         Not used
                //Interrupt, Waitstate, and Power-Down Control
                //  4000200h  2    R/W  IE        Interrupt Enable Register
                //  4000202h  2    R/W  IF        Interrupt Request Flags / IRQ Acknowledge
                //  4000204h  2    R/W  WAITCNT   Game Pak Waitstate Control
                //  4000206h       -    -         Not used
                //  4000208h  2    R/W  IME       Interrupt Master Enable Register
                //  400020Ah       -    -         Not used
                //  4000300h  1    R/W  POSTFLG   Undocumented - Post Boot Flag
                //  4000301h  1    W    HALTCNT   Undocumented - Power Down Control
                //  4000302h       -    -         Not used
                //  4000410h  ?    ?    ?         Undocumented - Purpose Unknown / Bug ??? 0FFh
                //  4000411h       -    -         Not used
                //  4000800h  4    R/W  ?         Undocumented - Internal Memory Control (R/W)
                //  4000804h       -    -         Not used
                //  4xx0800h  4    R/W  ?         Mirrors of 4000800h (repeated each 64K)
                }
                break;
            case 0x05000000U: return string.Format("pram[${0:X8}]", address & 0x3FFU);
            case 0x07000000U: return string.Format("oram[${0:X8}]", address & 0x3FFU);
            }

            return address.ToString("X8");
        }

        public static string Disassemble(uint pc, uint code)
        {
            switch (code >> 8)
            {
            default:
            case 0x00U:
            case 0x01U:
            case 0x02U:
            case 0x03U:
            case 0x04U:
            case 0x05U:
            case 0x06U:
            case 0x07U: return string.Format("lsl   r{0},r{1},#{2}", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 31U);
            case 0x08U:
            case 0x09U:
            case 0x0AU:
            case 0x0BU:
            case 0x0CU:
            case 0x0DU:
            case 0x0EU:
            case 0x0FU: return string.Format("lsr   r{0},r{1},#{2}", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 31U);
            case 0x10U:
            case 0x11U:
            case 0x12U:
            case 0x13U:
            case 0x14U:
            case 0x15U:
            case 0x16U:
            case 0x17U: return string.Format("asr   r{0},r{1},#{2}", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 31U);
            case 0x18U:
            case 0x19U: return string.Format("add   r{0},r{1},r{2}", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x1AU:
            case 0x1BU: return string.Format("sub   r{0},r{1},r{2}", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x1CU:
            case 0x1DU: return string.Format("add   r{0},r{1},#{2}", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x1EU:
            case 0x1FU: return string.Format("sub   r{0},r{1},#{2}", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x20U:
            case 0x21U:
            case 0x22U:
            case 0x23U:
            case 0x24U:
            case 0x25U:
            case 0x26U:
            case 0x27U: return string.Format("mov   r{0},#{1}", (code >> 8) & 7U, (code >> 0) & 255U);
            case 0x28U:
            case 0x29U:
            case 0x2AU:
            case 0x2BU:
            case 0x2CU:
            case 0x2DU:
            case 0x2EU:
            case 0x2FU: return string.Format("cmp   r{0},#{1}", (code >> 8) & 7U, (code >> 0) & 255U);
            case 0x30U:
            case 0x31U:
            case 0x32U:
            case 0x33U:
            case 0x34U:
            case 0x35U:
            case 0x36U:
            case 0x37U: return string.Format("add   r{0},#{1}", (code >> 8) & 7U, (code >> 0) & 255U);
            case 0x38U:
            case 0x39U:
            case 0x3AU:
            case 0x3BU:
            case 0x3CU:
            case 0x3DU:
            case 0x3EU:
            case 0x3FU: return string.Format("sub   r{0},#{1}", (code >> 8) & 7U, (code >> 0) & 255U);
            case 0x40U:
            case 0x41U:
            case 0x42U:
            case 0x43U:
                switch ((code >> 6) & 15U)
                {
                default:
                case 0x0U: return string.Format("and   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0x1U: return string.Format("eor   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0x2U: return string.Format("lsl   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0x3U: return string.Format("lsr   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0x4U: return string.Format("asr   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0x5U: return string.Format("adc   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0x6U: return string.Format("sbc   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0x7U: return string.Format("ror   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0x8U: return string.Format("tst   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0x9U: return string.Format("neg   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0xAU: return string.Format("cmp   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0xBU: return string.Format("cmn   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0xCU: return string.Format("orr   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0xDU: return string.Format("mul   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0xEU: return string.Format("bic   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                case 0xFU: return string.Format("mvn   r{0},r{1}", (code >> 0) & 7U, (code >> 3) & 7U);
                }
            case 0x44U: return string.Format("add   {0},{1}", GetRegisterName((code & 7U) | ((code >> 4) & 8U)), GetRegisterName((code >> 3) & 15U));
            case 0x45U: return string.Format("cmp   {0},{1}", GetRegisterName((code & 7U) | ((code >> 4) & 8U)), GetRegisterName((code >> 3) & 15U));
            case 0x46U: return string.Format("mov   {0},{1}", GetRegisterName((code & 7U) | ((code >> 4) & 8U)), GetRegisterName((code >> 3) & 15U));
            case 0x47U: return string.Format("bx    {1}    ", GetRegisterName((code & 7U) | ((code >> 4) & 8U)), GetRegisterName((code >> 3) & 15U));
            case 0x48U:
            case 0x49U:
            case 0x4AU:
            case 0x4BU:
            case 0x4CU:
            case 0x4DU:
            case 0x4EU:
            case 0x4FU: return string.Format("ldr   r{0},[pc,#{1}]", (code >> 8) & 7U, (code >> 0) & 255U);
            case 0x50U:
            case 0x51U: return string.Format("str   r{0},[r{1},r{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x52U:
            case 0x53U: return string.Format("strh  r{0},[r{1},r{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x54U:
            case 0x55U: return string.Format("strb  r{0},[r{1},r{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x56U:
            case 0x57U: return string.Format("ldsb  r{0},[r{1},r{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x58U:
            case 0x59U: return string.Format("ldr   r{0},[r{1},r{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x5AU:
            case 0x5BU: return string.Format("ldrh  r{0},[r{1},r{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x5CU:
            case 0x5DU: return string.Format("ldrb  r{0},[r{1},r{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x5EU:
            case 0x5FU: return string.Format("ldsh  r{0},[r{1},r{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 7U);
            case 0x60U:
            case 0x61U:
            case 0x62U:
            case 0x63U:
            case 0x64U:
            case 0x65U:
            case 0x66U:
            case 0x67U: return string.Format("str   r{0},[r{1},#{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 4) & 124U);
            case 0x68U:
            case 0x69U:
            case 0x6AU:
            case 0x6BU:
            case 0x6CU:
            case 0x6DU:
            case 0x6EU:
            case 0x6FU: return string.Format("ldr   r{0},[r{1},#{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 4) & 124U);
            case 0x70U:
            case 0x71U:
            case 0x72U:
            case 0x73U:
            case 0x74U:
            case 0x75U:
            case 0x76U:
            case 0x77U: return string.Format("strb  r{0},[r{1},#{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 31U);
            case 0x78U:
            case 0x79U:
            case 0x7AU:
            case 0x7BU:
            case 0x7CU:
            case 0x7DU:
            case 0x7EU:
            case 0x7FU: return string.Format("ldrb  r{0},[r{1},#{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 6) & 31U);
            case 0x80U:
            case 0x81U:
            case 0x82U:
            case 0x83U:
            case 0x84U:
            case 0x85U:
            case 0x86U:
            case 0x87U: return string.Format("strh  r{0},[r{1},#{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 5) & 62U);
            case 0x88U:
            case 0x89U:
            case 0x8AU:
            case 0x8BU:
            case 0x8CU:
            case 0x8DU:
            case 0x8EU:
            case 0x8FU: return string.Format("ldrh  r{0},[r{1},#{2}]", (code >> 0) & 7U, (code >> 3) & 7U, (code >> 5) & 62U);
            case 0x90U:
            case 0x91U:
            case 0x92U:
            case 0x93U:
            case 0x94U:
            case 0x95U:
            case 0x96U:
            case 0x97U: return string.Format("str   r{0},[sp,#{1}]", (code >> 8) & 7U, (code << 2) & 1020U);
            case 0x98U:
            case 0x99U:
            case 0x9AU:
            case 0x9BU:
            case 0x9CU:
            case 0x9DU:
            case 0x9EU:
            case 0x9FU: return string.Format("ldr   r{0},[sp,#{1}]", (code >> 8) & 7U, (code << 2) & 1020U);
            case 0xA0U:
            case 0xA1U:
            case 0xA2U:
            case 0xA3U:
            case 0xA4U:
            case 0xA5U:
            case 0xA6U:
            case 0xA7U: return string.Format("add   r{0},pc,#{1}", (code >> 8) & 7U, (code << 2) & 1020U);
            case 0xA8U:
            case 0xA9U:
            case 0xAAU:
            case 0xABU:
            case 0xACU:
            case 0xADU:
            case 0xAEU:
            case 0xAFU: return string.Format("add   r{0},sp,#{1}", (code >> 8) & 7U, (code << 2) & 1020U);
            case 0xB0U:
                switch ((code >> 7) & 1U)
                {
                default:
                case 0U: return string.Format("add   sp,#{0:X2}", (code << 2) & 508U);
                case 1U: return string.Format("sub   sp,#{0:X2}", (code << 2) & 508U);
                }
            case 0xB1U: goto undefined_;
            case 0xB2U: goto undefined_;
            case 0xB3U: goto undefined_;
            case 0xB4U: return string.Format("push  {0}   ", GetRegisterList((code >> 0) & 255U));
            case 0xB5U: return string.Format("push  {0},lr", GetRegisterList((code >> 0) & 255U));
            case 0xB6U: goto undefined_;
            case 0xB7U: goto undefined_;
            case 0xB8U: goto undefined_;
            case 0xB9U: goto undefined_;
            case 0xBAU: goto undefined_;
            case 0xBBU: goto undefined_;
            case 0xBCU: return string.Format("pop   {0}   ", GetRegisterList((code >> 0) & 255U));
            case 0xBDU: return string.Format("pop   {0},pc", GetRegisterList((code >> 0) & 255U));
            case 0xBEU: goto undefined_;
            case 0xBFU: goto undefined_;
            case 0xC0U:
            case 0xC1U:
            case 0xC2U:
            case 0xC3U:
            case 0xC4U:
            case 0xC5U:
            case 0xC6U:
            case 0xC7U: return string.Format("stmia r{0}!,{1}", (code >> 8) & 7U, GetRegisterList((code >> 0) & 255U));
            case 0xC8U:
            case 0xC9U:
            case 0xCAU:
            case 0xCBU:
            case 0xCCU:
            case 0xCDU:
            case 0xCEU:
            case 0xCFU: return string.Format("ldmia r{0}!,{1}", (code >> 8) & 7U, GetRegisterList((code >> 0) & 255U));
            case 0xD0U: return string.Format("beq   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xD1U: return string.Format("bne   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xD2U: return string.Format("bcs   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xD3U: return string.Format("bcc   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xD4U: return string.Format("bmi   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xD5U: return string.Format("bpl   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xD6U: return string.Format("bvs   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xD7U: return string.Format("bvc   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xD8U: return string.Format("bhi   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xD9U: return string.Format("bls   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xDAU: return string.Format("bge   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xDBU: return string.Format("blt   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xDCU: return string.Format("bgt   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xDDU: return string.Format("ble   ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 8));
            case 0xDEU:
            undefined_: return string.Format("und   ${0:X2}", (code >> 0) & 255U);
            case 0xDFU: return string.Format("swi   ${0:X2}", (code >> 0) & 255U);
            case 0xE0U:
            case 0xE1U:
            case 0xE2U:
            case 0xE3U:
            case 0xE4U:
            case 0xE5U:
            case 0xE6U:
            case 0xE7U: return string.Format("b     ${0:X8}", pc + 4U + MathHelper.SignExtend(code << 1, 12));
            case 0xE8U: goto undefined_;
            case 0xE9U: goto undefined_;
            case 0xEAU: goto undefined_;
            case 0xEBU: goto undefined_;
            case 0xECU: goto undefined_;
            case 0xEDU: goto undefined_;
            case 0xEEU: goto undefined_;
            case 0xEFU: goto undefined_;
            case 0xF0U:
            case 0xF1U:
            case 0xF2U:
            case 0xF3U:
            case 0xF4U:
            case 0xF5U:
            case 0xF6U:
            case 0xF7U: return string.Format("bl1   ${0:X8}", MathHelper.SignExtend(code << 12, 23));
            case 0xF8U:
            case 0xF9U:
            case 0xFAU:
            case 0xFBU:
            case 0xFCU:
            case 0xFDU:
            case 0xFEU:
            case 0xFFU: return string.Format("bl2   ${0:X8}", (code << 1) & 0x000FFEU);
            }
        }
    }
}