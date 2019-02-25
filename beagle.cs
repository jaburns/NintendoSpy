/*=========================================================================
| Beagle Interface Library
|--------------------------------------------------------------------------
| Copyright (c) 2004-2011 Total Phase, Inc.
| All rights reserved.
| www.totalphase.com
|
| Redistribution and use in source and binary forms, with or without
| modification, are permitted provided that the following conditions
| are met:
|
| - Redistributions of source code must retain the above copyright
|   notice, this list of conditions and the following disclaimer.
|
| - Redistributions in binary form must reproduce the above copyright
|   notice, this list of conditions and the following disclaimer in the
|   documentation and/or other materials provided with the distribution.
|
| - Neither the name of Total Phase, Inc. nor the names of its
|   contributors may be used to endorse or promote products derived from
|   this software without specific prior written permission.
|
| THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
| "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
| LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
| FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE
| COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
| INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
| BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
| LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
| CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
| LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
| ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
| POSSIBILITY OF SUCH DAMAGE.
|--------------------------------------------------------------------------
| To access Total Phase Beagle devices through the API:
|
| 1) Use one of the following shared objects:
|      beagle.so        --  Linux shared object
|          or
|      beagle.dll       --  Windows dynamic link library
|
| 2) Along with one of the following language modules:
|      beagle.c/h       --  C/C++ API header file and interface module
|      beagle_py.py     --  Python API
|      beagle.bas       --  Visual Basic 6 API
|      beagle.cs        --  C# .NET source
|      beagle_net.dll   --  Compiled .NET binding
 ========================================================================*/

using System;
using System.Reflection;
using System.Runtime.InteropServices;

//[assembly: AssemblyTitleAttribute("Beagle .NET binding")]
//[assembly: AssemblyDescriptionAttribute(".NET binding for Beagle")]
//[assembly: AssemblyCompanyAttribute("Total Phase, Inc.")]
//[assembly: AssemblyProductAttribute("Beagle")]
//[assembly: AssemblyCopyrightAttribute("Total Phase, Inc. 2017")]

namespace TotalPhase {

public enum BeagleStatus : int {
    /* General codes (0 to -99) */
    BG_OK                                          =    0,
    BG_UNABLE_TO_LOAD_LIBRARY                      =   -1,
    BG_UNABLE_TO_LOAD_DRIVER                       =   -2,
    BG_UNABLE_TO_LOAD_FUNCTION                     =   -3,
    BG_INCOMPATIBLE_LIBRARY                        =   -4,
    BG_INCOMPATIBLE_DEVICE                         =   -5,
    BG_INCOMPATIBLE_DRIVER                         =   -6,
    BG_COMMUNICATION_ERROR                         =   -7,
    BG_UNABLE_TO_OPEN                              =   -8,
    BG_UNABLE_TO_CLOSE                             =   -9,
    BG_INVALID_HANDLE                              =  -10,
    BG_CONFIG_ERROR                                =  -11,
    BG_UNKNOWN_PROTOCOL                            =  -12,
    BG_STILL_ACTIVE                                =  -13,
    BG_FUNCTION_NOT_AVAILABLE                      =  -14,
    BG_INVALID_LICENSE                             =  -15,
    BG_CAPTURE_NOT_TRIGGERED                       =  -16,
    BG_CAPTURE_NOT_READY_FOR_DOWNLOAD              =  -17,

    /* COMMTEST codes (-100 to -199) */
    BG_COMMTEST_NOT_AVAILABLE                      = -100,
    BG_COMMTEST_NOT_ENABLED                        = -101,

    /* I2C codes (-200 to -299) */
    BG_I2C_NOT_AVAILABLE                           = -200,
    BG_I2C_NOT_ENABLED                             = -201,

    /* SPI codes (-300 to -399) */
    BG_SPI_NOT_AVAILABLE                           = -300,
    BG_SPI_NOT_ENABLED                             = -301,

    /* USB codes (-400 to -499) */
    BG_USB_NOT_AVAILABLE                           = -400,
    BG_USB_NOT_ENABLED                             = -401,
    BG_USB2_NOT_ENABLED                            = -402,
    BG_USB3_NOT_ENABLED                            = -403,

    /* Cross-Analyzer Sync codes (-410 to -413) */
    BG_CROSS_ANALYZER_SYNC_DISTURBED_RE_ENABLE     = -410,
    BG_CROSS_ANALYZER_SYNC_DISTURBED_RECONNECT     = -411,
    BG_CROSS_ANALYZER_SYNC_UNLICENSED_SELF         = -412,
    BG_CROSS_ANALYZER_SYNC_UNLICENSED_OTHER        = -413,

    /* Complex Triggering Config codes (-450 to -469) */
    BG_COMPLEX_CONFIG_ERROR_NO_STATES              = -450,
    BG_COMPLEX_CONFIG_ERROR_DATA_PACKET_TYPE       = -451,
    BG_COMPLEX_CONFIG_ERROR_DATA_FIELD             = -452,
    BG_COMPLEX_CONFIG_ERROR_ERR_MATCH_FIELD        = -453,
    BG_COMPLEX_CONFIG_ERROR_DATA_RESOURCES         = -454,
    BG_COMPLEX_CONFIG_ERROR_DP_MATCH_TYPE          = -455,
    BG_COMPLEX_CONFIG_ERROR_DP_MATCH_VAL           = -456,
    BG_COMPLEX_CONFIG_ERROR_DP_REQUIRED            = -457,
    BG_COMPLEX_CONFIG_ERROR_DP_RESOURCES           = -458,
    BG_COMPLEX_CONFIG_ERROR_TIMER_UNIT             = -459,
    BG_COMPLEX_CONFIG_ERROR_TIMER_BOUNDS           = -460,
    BG_COMPLEX_CONFIG_ERROR_ASYNC_EVENT            = -461,
    BG_COMPLEX_CONFIG_ERROR_ASYNC_EDGE             = -462,
    BG_COMPLEX_CONFIG_ERROR_ACTION_FILTER          = -463,
    BG_COMPLEX_CONFIG_ERROR_ACTION_GOTO_SEL        = -464,
    BG_COMPLEX_CONFIG_ERROR_ACTION_GOTO_DEST       = -465,
    BG_COMPLEX_CONFIG_ERROR_BAD_VBUS_TRIGGER_TYPE  = -466,
    BG_COMPLEX_CONFIG_ERROR_BAD_VBUS_TRIGGER_THRES = -467,
    BG_COMPLEX_CONFIG_ERROR_NO_MULTI_VBUS_TRIGGERS = -468,
    BG_COMPLEX_CONFIG_ERROR_IV_MONITOR_NOT_ENABLED = -469,

    /* MDIO codes (-500 to -599) */
    BG_MDIO_NOT_AVAILABLE                          = -500,
    BG_MDIO_NOT_ENABLED                            = -501,
    BG_MDIO_BAD_TURNAROUND                         = -502,

    /* IV MON codes (-600 to -699) */
    BG_IV_MON_NULL_PACKET                          = -600,
    BG_IV_MON_INVALID_PACKET_LENGTH                = -601
}

public enum BeagleProtocol : int {
    BG_PROTOCOL_NONE     = 0,
    BG_PROTOCOL_COMMTEST = 1,
    BG_PROTOCOL_USB      = 2,
    BG_PROTOCOL_I2C      = 3,
    BG_PROTOCOL_SPI      = 4,
    BG_PROTOCOL_MDIO     = 5
}

public enum BeagleCaptureStatus : int {
    BG_CAPTURE_STATUS_UNKNOWN          = -1,
    BG_CAPTURE_STATUS_INACTIVE         =  0,
    BG_CAPTURE_STATUS_SYNC_STANDBY     =  1,
    BG_CAPTURE_STATUS_PRE_TRIGGER      =  2,
    BG_CAPTURE_STATUS_PRE_TRIGGER_SYNC =  3,
    BG_CAPTURE_STATUS_POST_TRIGGER     =  4,
    BG_CAPTURE_STATUS_TRANSFER         =  5,
    BG_CAPTURE_STATUS_COMPLETE         =  6
}

public enum BeagleSpiSSPolarity : int {
    BG_SPI_SS_ACTIVE_LOW  = 0,
    BG_SPI_SS_ACTIVE_HIGH = 1
}

public enum BeagleSpiSckSamplingEdge : int {
    BG_SPI_SCK_SAMPLING_EDGE_RISING  = 0,
    BG_SPI_SCK_SAMPLING_EDGE_FALLING = 1
}

public enum BeagleSpiBitorder : int {
    BG_SPI_BITORDER_MSB = 0,
    BG_SPI_BITORDER_LSB = 1
}

public enum BeagleUsbTriggerMode : int {
    BG_USB_TRIGGER_MODE_EVENT     = 0,
    BG_USB_TRIGGER_MODE_IMMEDIATE = 1
}

public enum BeagleUsbTargetPower : int {
    BG_USB_TARGET_POWER_HOST_SUPPLIED = 0,
    BG_USB_TARGET_POWER_OFF           = 1
}

public enum BeagleUsb2CaptureMode : int {
    BG_USB2_CAPTURE_REALTIME                 = 0,
    BG_USB2_CAPTURE_REALTIME_WITH_PROTECTION = 1,
    BG_USB2_CAPTURE_DELAYED_DOWNLOAD         = 2
}

public enum BeagleUsb2DigitalOutMatchPins : int {
    BG_USB2_DIGITAL_OUT_MATCH_PIN3 = 3,
    BG_USB2_DIGITAL_OUT_MATCH_PIN4 = 4
}

public enum BeagleUsb2MatchType : int {
    BG_USB2_MATCH_TYPE_DISABLED  = 0,
    BG_USB2_MATCH_TYPE_EQUAL     = 1,
    BG_USB2_MATCH_TYPE_NOT_EQUAL = 2
}

public enum BeagleUsbMatchType : int {
    BG_USB_MATCH_TYPE_DISABLED      = 0,
    BG_USB_MATCH_TYPE_EQUAL         = 1,
    BG_USB_MATCH_TYPE_LESS_EQUAL    = 2,
    BG_USB_MATCH_TYPE_GREATER_EQUAL = 3,
    BG_USB_MATCH_TYPE_NOT_EQUAL     = 4
}

public enum BeagleUsb2DataMatchDirection : int {
    BG_USB2_MATCH_DIRECTION_DISABLED  = 0,
    BG_USB2_MATCH_DIRECTION_IN        = 1,
    BG_USB2_MATCH_DIRECTION_OUT_SETUP = 2,
    BG_USB2_MATCH_DIRECTION_SETUP     = 3
}

public enum BeagleUsb2PacketType : uint {
    BG_USB2_MATCH_PACKET_IN          = 0x0009,
    BG_USB2_MATCH_PACKET_OUT         = 0x0001,
    BG_USB2_MATCH_PACKET_SETUP       = 0x000d,
    BG_USB2_MATCH_PACKET_SOF         = 0x0005,

    BG_USB2_MATCH_PACKET_DATA0       = 0x0003,
    BG_USB2_MATCH_PACKET_DATA1       = 0x000b,
    BG_USB2_MATCH_PACKET_DATA2       = 0x0007,
    BG_USB2_MATCH_PACKET_MDATA       = 0x000f,

    BG_USB2_MATCH_PACKET_ACK         = 0x0002,
    BG_USB2_MATCH_PACKET_NAK         = 0x000a,
    BG_USB2_MATCH_PACKET_STALL       = 0x000e,
    BG_USB2_MATCH_PACKET_NYET        = 0x0006,
    BG_USB2_MATCH_PACKET_PRE         = 0x000c,
    BG_USB2_MATCH_PACKET_ERR         = 0x010c,
    BG_USB2_MATCH_PACKET_SPLIT       = 0x0008,
    BG_USB2_MATCH_PACKET_EXT         = 0x0000,

    BG_USB2_MATCH_PACKET_ANY         = 0x0010,
    BG_USB2_MATCH_PACKET_DATA0_DATA1 = 0x0020,
    BG_USB2_MATCH_PACKET_DATAX       = 0x0040,
    BG_USB2_MATCH_PACKET_SUBPID_MASK = 0x0100,

    BG_USB2_MATCH_PACKET_ERROR       = 0x1000
}

public enum BeagleUsb2DataMatchPrefix : int {
    BG_USB2_MATCH_PREFIX_DISABLED     = 0,
    BG_USB2_MATCH_PREFIX_IN           = 1,
    BG_USB2_MATCH_PREFIX_OUT          = 2,
    BG_USB2_MATCH_PREFIX_SETUP        = 3,
    BG_USB2_MATCH_PREFIX_CSPLIT       = 4,
    BG_USB2_MATCH_PREFIX_CSPLIT_IN    = 5,
    BG_USB2_MATCH_PREFIX_SSPLIT_OUT   = 6,
    BG_USB2_MATCH_PREFIX_SSPLIT_SETUP = 7
}

public enum BeagleUsb2ErrorType : int {
    BG_USB2_MATCH_CRC_DONT_CARE          =    0,
    BG_USB2_MATCH_CRC_VALID              =    1,
    BG_USB2_MATCH_CRC_INVALID            =    2,
    BG_USB2_MATCH_ERR_MASK_CORRUPTED_PID = 0x10,
    BG_USB2_MATCH_ERR_MASK_CRC           = 0x20,
    BG_USB2_MATCH_ERR_MASK_RXERROR       = 0x40,
    BG_USB2_MATCH_ERR_MASK_JABBER        = 0x80
}

public enum BeagleUsb2MatchModifier : byte {
    BG_USB2_MATCH_MODIFIER_0 = 0,
    BG_USB2_MATCH_MODIFIER_1 = 1,
    BG_USB2_MATCH_MODIFIER_2 = 2,
    BG_USB2_MATCH_MODIFIER_3 = 3
}

public enum BeagleUsbTimerUnit : int {
    BG_USB_TIMER_UNIT_DISABLED = 0,
    BG_USB_TIMER_UNIT_NS       = 1,
    BG_USB_TIMER_UNIT_US       = 2,
    BG_USB_TIMER_UNIT_MS       = 3,
    BG_USB_TIMER_UNIT_SEC      = 4
}

public enum BeagleUsb2AsyncEventType : int {
    BG_USB2_COMPLEX_MATCH_EVENT_DIGIN1        =  0,
    BG_USB2_COMPLEX_MATCH_EVENT_DIGIN2        =  1,
    BG_USB2_COMPLEX_MATCH_EVENT_DIGIN3        =  2,
    BG_USB2_COMPLEX_MATCH_EVENT_DIGIN4        =  3,



    BG_USB2_COMPLEX_MATCH_EVENT_CHIRP         = 13,
    BG_USB2_COMPLEX_MATCH_EVENT_SMA_EXTIN     = 14,
    BG_USB2_COMPLEX_MATCH_EVENT_CROSS_TRIGGER = 15,
    BG_USB2_COMPLEX_MATCH_EVENT_VBUS_TRIGGER  = 16
}

public enum BeagleUsb2VbusTriggerType : int {
    BG_USB2_VBUS_TRIGGER_TYPE_CURRENT = 1,
    BG_USB2_VBUS_TRIGGER_TYPE_VOLTAGE = 2
}

public enum BeagleUsbExtoutType : int {
    BG_USB_EXTOUT_LOW       = 0,
    BG_USB_EXTOUT_HIGH      = 1,
    BG_USB_EXTOUT_POS_PULSE = 2,
    BG_USB_EXTOUT_NEG_PULSE = 3,
    BG_USB_EXTOUT_TOGGLE_0  = 4,
    BG_USB_EXTOUT_TOGGLE_1  = 5
}

public enum BeagleMemoryTestResult : int {
    BG_USB_MEMORY_TEST_PASS = 0,
    BG_USB_MEMORY_TEST_FAIL = 1
}

public enum BeagleUsb3ExtoutMode : int {
    BG_USB3_EXTOUT_DISABLED     = 0,
    BG_USB3_EXTOUT_TRIGGER_MODE = 1,
    BG_USB3_EXTOUT_EVENTS_MODE  = 2
}

public enum BeagleUsb3IPSType : int {
    BG_USB3_IPS_TYPE_DISABLED = 0,
    BG_USB3_IPS_TYPE_TS1      = 1,
    BG_USB3_IPS_TYPE_TS2      = 2,
    BG_USB3_IPS_TYPE_TSEQ     = 3,
    BG_USB3_IPS_TYPE_TSx      = 4,
    BG_USB3_IPS_TYPE_TS_ANY   = 5
}

public enum BeagleUsbSource : int {
    BG_USB_SOURCE_USB3_ASYNC = 0,
    BG_USB_SOURCE_USB3_RX    = 1,
    BG_USB_SOURCE_USB3_TX    = 2,
    BG_USB_SOURCE_USB2       = 3,
    BG_USB_SOURCE_IV_MON     = 4
}

public enum BeagleUsb3PacketType : int {
    BG_USB3_MATCH_PACKET_SLC         = 0,
    BG_USB3_MATCH_PACKET_SHP         = 1,
    BG_USB3_MATCH_PACKET_SDP         = 2,
    BG_USB3_MATCH_PACKET_SHP_SDP     = 3,
    BG_USB3_MATCH_PACKET_TSx         = 4,
    BG_USB3_MATCH_PACKET_TSEQ        = 5,
    BG_USB3_MATCH_PACKET_ERROR       = 6,
    BG_USB3_MATCH_PACKET_5GBIT_START = 7,
    BG_USB3_MATCH_PACKET_5GBIT_STOP  = 8
}

public enum BeagleUsb3ErrorType : int {
    BG_USB3_MATCH_CRC_DONT_CARE    =    0,
    BG_USB3_MATCH_CRC_1_VALID      =    1,
    BG_USB3_MATCH_CRC_2_VALID      =    2,
    BG_USB3_MATCH_CRC_BOTH_VALID   =    3,
    BG_USB3_MATCH_CRC_EITHER_FAIL  =    4,
    BG_USB3_MATCH_CRC_1_FAIL       =    5,
    BG_USB3_MATCH_CRC_2_FAIL       =    6,
    BG_USB3_MATCH_CRC_BOTH_FAIL    =    7,
    BG_USB3_MATCH_ERR_MASK_CRC     = 0x10,
    BG_USB3_MATCH_ERR_MASK_FRAMING = 0x20,
    BG_USB3_MATCH_ERR_MASK_UNKNOWN = 0x40
}

public enum BeagleUsb3MatchModifier : int {
    BG_USB3_MATCH_MODIFIER_0 = 0,
    BG_USB3_MATCH_MODIFIER_1 = 1,
    BG_USB3_MATCH_MODIFIER_2 = 2,
    BG_USB3_MATCH_MODIFIER_3 = 3
}

public enum BeagleUsb3AsyncEventType : int {
    BG_USB3_COMPLEX_MATCH_EVENT_SSTX_LFPS     =  0,
    BG_USB3_COMPLEX_MATCH_EVENT_SSTX_POLARITY =  1,
    BG_USB3_COMPLEX_MATCH_EVENT_SSTX_DETECTED =  2,
    BG_USB3_COMPLEX_MATCH_EVENT_SSTX_SCRAMBLE =  3,
    BG_USB3_COMPLEX_MATCH_EVENT_SSRX_LFPS     =  4,
    BG_USB3_COMPLEX_MATCH_EVENT_SSRX_POLARITY =  5,
    BG_USB3_COMPLEX_MATCH_EVENT_SSRX_DETECTED =  6,
    BG_USB3_COMPLEX_MATCH_EVENT_SSRX_SCRAMBLE =  7,
    BG_USB3_COMPLEX_MATCH_EVENT_CROSS_TRIGGER =  8,

    BG_USB3_COMPLEX_MATCH_EVENT_VBUS_PRESENT  = 11,
    BG_USB3_COMPLEX_MATCH_EVENT_SSTX_PHYERR   = 12,
    BG_USB3_COMPLEX_MATCH_EVENT_SSRX_PHYERR   = 13,
    BG_USB3_COMPLEX_MATCH_EVENT_SMA_EXTIN     = 14
}

public enum BeagleUsb3MemoryTestType : int {
    BG_USB3_MEMORY_TEST_FAST =  0,
    BG_USB3_MEMORY_TEST_FULL =  1,
    BG_USB3_MEMORY_TEST_SKIP = -1
}

public enum Beagle5000CrossAnalyzerSyncMode : int {
    BG5000_CROSS_ANALYZER_SYNC_WAIT   = 0,
    BG5000_CROSS_ANALYZER_SYNC_BYPASS = 1
}

public enum Beagle5000CrossAnalyzerMode : int {
    BG5000_CROSS_ANALYZER_ACCEPT = 0,
    BG5000_CROSS_ANALYZER_IGNORE = 1
}

public enum BeagleMdioClause : int {
    BG_MDIO_CLAUSE_22    = 0,
    BG_MDIO_CLAUSE_45    = 1,
    BG_MDIO_CLAUSE_ERROR = 2
}


public class BeagleApi {

/*=========================================================================
| HELPER FUNCTIONS / CLASSES
 ========================================================================*/
static long tp_min(long x, long y) { return x < y ? x : y; }

private class GCContext {
    GCHandle[] handles;
    int index;
    public GCContext () {
        handles = new GCHandle[16];
        index   = 0;
    }
    public void add (GCHandle gch) {
        handles[index] = gch;
        index++;
    }
    public void free () {
        while (index != 0) {
            index--;
            handles[index].Free();
        }
    }
}

/*=========================================================================
| VERSION
 ========================================================================*/
[DllImport ("beagle")]
private static extern int bg_c_version ();

public const int BG_API_VERSION    = 0x050a;   // v5.10
public const int BG_REQ_SW_VERSION = 0x050a;   // v5.10

private static short BG_SW_VERSION;
private static short BG_REQ_API_VERSION;
private static bool  BG_LIBRARY_LOADED;

static BeagleApi () {
    BG_SW_VERSION      = (short)(bg_c_version() & 0xffff);
    BG_REQ_API_VERSION = (short)((bg_c_version() >> 16) & 0xffff);
    BG_LIBRARY_LOADED  = 
        ((BG_SW_VERSION >= BG_REQ_SW_VERSION) &&
         (BG_API_VERSION >= BG_REQ_API_VERSION));
}

/*=========================================================================
| STATUS CODES
 ========================================================================*/
/*
 * All API functions return an integer which is the result of the
 * transaction, or a status code if negative.  The status codes are
 * defined as follows:
 */
// enum BeagleStatus  (from declaration above)
//     BG_OK                                          =    0
//     BG_UNABLE_TO_LOAD_LIBRARY                      =   -1
//     BG_UNABLE_TO_LOAD_DRIVER                       =   -2
//     BG_UNABLE_TO_LOAD_FUNCTION                     =   -3
//     BG_INCOMPATIBLE_LIBRARY                        =   -4
//     BG_INCOMPATIBLE_DEVICE                         =   -5
//     BG_INCOMPATIBLE_DRIVER                         =   -6
//     BG_COMMUNICATION_ERROR                         =   -7
//     BG_UNABLE_TO_OPEN                              =   -8
//     BG_UNABLE_TO_CLOSE                             =   -9
//     BG_INVALID_HANDLE                              =  -10
//     BG_CONFIG_ERROR                                =  -11
//     BG_UNKNOWN_PROTOCOL                            =  -12
//     BG_STILL_ACTIVE                                =  -13
//     BG_FUNCTION_NOT_AVAILABLE                      =  -14
//     BG_INVALID_LICENSE                             =  -15
//     BG_CAPTURE_NOT_TRIGGERED                       =  -16
//     BG_CAPTURE_NOT_READY_FOR_DOWNLOAD              =  -17
//     BG_COMMTEST_NOT_AVAILABLE                      = -100
//     BG_COMMTEST_NOT_ENABLED                        = -101
//     BG_I2C_NOT_AVAILABLE                           = -200
//     BG_I2C_NOT_ENABLED                             = -201
//     BG_SPI_NOT_AVAILABLE                           = -300
//     BG_SPI_NOT_ENABLED                             = -301
//     BG_USB_NOT_AVAILABLE                           = -400
//     BG_USB_NOT_ENABLED                             = -401
//     BG_USB2_NOT_ENABLED                            = -402
//     BG_USB3_NOT_ENABLED                            = -403
//     BG_CROSS_ANALYZER_SYNC_DISTURBED_RE_ENABLE     = -410
//     BG_CROSS_ANALYZER_SYNC_DISTURBED_RECONNECT     = -411
//     BG_CROSS_ANALYZER_SYNC_UNLICENSED_SELF         = -412
//     BG_CROSS_ANALYZER_SYNC_UNLICENSED_OTHER        = -413
//     BG_COMPLEX_CONFIG_ERROR_NO_STATES              = -450
//     BG_COMPLEX_CONFIG_ERROR_DATA_PACKET_TYPE       = -451
//     BG_COMPLEX_CONFIG_ERROR_DATA_FIELD             = -452
//     BG_COMPLEX_CONFIG_ERROR_ERR_MATCH_FIELD        = -453
//     BG_COMPLEX_CONFIG_ERROR_DATA_RESOURCES         = -454
//     BG_COMPLEX_CONFIG_ERROR_DP_MATCH_TYPE          = -455
//     BG_COMPLEX_CONFIG_ERROR_DP_MATCH_VAL           = -456
//     BG_COMPLEX_CONFIG_ERROR_DP_REQUIRED            = -457
//     BG_COMPLEX_CONFIG_ERROR_DP_RESOURCES           = -458
//     BG_COMPLEX_CONFIG_ERROR_TIMER_UNIT             = -459
//     BG_COMPLEX_CONFIG_ERROR_TIMER_BOUNDS           = -460
//     BG_COMPLEX_CONFIG_ERROR_ASYNC_EVENT            = -461
//     BG_COMPLEX_CONFIG_ERROR_ASYNC_EDGE             = -462
//     BG_COMPLEX_CONFIG_ERROR_ACTION_FILTER          = -463
//     BG_COMPLEX_CONFIG_ERROR_ACTION_GOTO_SEL        = -464
//     BG_COMPLEX_CONFIG_ERROR_ACTION_GOTO_DEST       = -465
//     BG_COMPLEX_CONFIG_ERROR_BAD_VBUS_TRIGGER_TYPE  = -466
//     BG_COMPLEX_CONFIG_ERROR_BAD_VBUS_TRIGGER_THRES = -467
//     BG_COMPLEX_CONFIG_ERROR_NO_MULTI_VBUS_TRIGGERS = -468
//     BG_COMPLEX_CONFIG_ERROR_IV_MONITOR_NOT_ENABLED = -469
//     BG_MDIO_NOT_AVAILABLE                          = -500
//     BG_MDIO_NOT_ENABLED                            = -501
//     BG_MDIO_BAD_TURNAROUND                         = -502
//     BG_IV_MON_NULL_PACKET                          = -600
//     BG_IV_MON_INVALID_PACKET_LENGTH                = -601


/*=========================================================================
| GENERAL TYPE DEFINITIONS
 ========================================================================*/
/* Beagle handle type definition */
/* typedef Beagle => int */

/*
 * Beagle version matrix.
 *
 * This matrix describes the various version dependencies
 * of Beagle components.  It can be used to determine
 * which component caused an incompatibility error.
 *
 * All version numbers are of the format:
 *   (major << 8) | minor
 *
 * ex. v1.20 would be encoded as:  0x0114
 */
[StructLayout(LayoutKind.Sequential)]
public struct BeagleVersion {
    /* Software, firmware, and hardware versions. */
    public ushort software;
    public ushort firmware;
    public ushort hardware;

    /*
     * Hardware revisions that are compatible with this software version.
     * The top 16 bits gives the maximum accepted hw revision.
     * The lower 16 bits gives the minimum accepted hw revision.
     */
    public uint   hw_revs_for_sw;

    /*
     * Firmware revisions that are compatible with this software version.
     * The top 16 bits gives the maximum accepted fw revision.
     * The lower 16 bits gives the minimum accepted fw revision.
     */
    public uint   fw_revs_for_sw;

    /*
     * Driver revisions that are compatible with this software version.
     * The top 16 bits gives the maximum accepted driver revision.
     * The lower 16 bits gives the minimum accepted driver revision.
     * This version checking is currently only pertinent for WIN32
     * platforms.
     */
    public uint   drv_revs_for_sw;

    /* Software requires that the API interface must be >= this version. */
    public ushort api_req_by_sw;
}


/*=========================================================================
| GENERAL API
 ========================================================================*/
/*
 * Get a list of ports to which Beagle devices are attached.
 *
 * num_devices = maximum number of elements to return
 * devices     = array into which the port numbers are returned
 *
 * Each element of the array is written with the port number.
 * Devices that are in-use are ORed with BG_PORT_NOT_FREE
 * (0x8000).
 *
 * ex.  devices are attached to ports 0, 1, 2
 *      ports 0 and 2 are available, and port 1 is in-use.
 *      array => 0x0000, 0x8001, 0x0002
 *
 * If the array is NULL, it is not filled with any values.
 * If there are more devices than the array size, only the
 * first nmemb port numbers will be written into the array.
 *
 * Returns the number of devices found, regardless of the
 * array size.
 */
public const ushort BG_PORT_NOT_FREE = 0x8000;
public static int bg_find_devices (
    int       num_devices,
    ushort[]  devices
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int devices_num_devices = (int)tp_min(num_devices, devices.Length);
    return net_bg_find_devices(devices_num_devices, devices);
}

/*
 * Get a list of ports to which Beagle devices are attached
 *
 * This function is the same as bg_find_devices() except that
 * it returns the unique IDs of each Beagle device.  The IDs
 * are guaranteed to be non-zero if valid.
 *
 * The IDs are the unsigned integer representation of the 10-digit
 * serial numbers.
 */
public static int bg_find_devices_ext (
    int       num_devices,
    ushort[]  devices,
    int       num_ids,
    uint[]    unique_ids
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int devices_num_devices = (int)tp_min(num_devices, devices.Length);
    int unique_ids_num_ids = (int)tp_min(num_ids, unique_ids.Length);
    return net_bg_find_devices_ext(devices_num_devices, devices, unique_ids_num_ids, unique_ids);
}

/*
 * Open the Beagle port.
 *
 * The port number is a zero-indexed integer.
 *
 * The port number is the same as that obtained from the
 * bg_find_devices() function above.
 *
 * Returns an Beagle handle, which is guaranteed to be
 * greater than zero if it is valid.
 *
 * This function is recommended for use in simple applications
 * where extended information is not required.  For more complex
 * applications, the use of bg_open_ext() is recommended.
 */
public static int bg_open (
    int  port_number
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_open(port_number);
}

/*
 * Open the Beagle port, returning extended information
 * in the supplied structure.  Behavior is otherwise identical
 * to bg_open() above.  If 0 is passed as the pointer to the
 * structure, this function is exactly equivalent to bg_open().
 *
 * The structure is zeroed before the open is attempted.
 * It is filled with whatever information is available.
 *
 * For example, if the hardware version is not filled, then
 * the device could not be queried for its version number.
 *
 * This function is recommended for use in complex applications
 * where extended information is required.  For more simple
 * applications, the use of bg_open() is recommended.
 */
[StructLayout(LayoutKind.Sequential)]
public struct BeagleExt {
    /* Version matrix */
    public BeagleVersion version;

    /* Features of this device. */
    public int           features;
}

public static int bg_open_ext (
    int            port_number,
    ref BeagleExt  bg_ext
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_open_ext(port_number, ref bg_ext);
}

/* Close the Beagle port. */
public static int bg_close (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_close(beagle);
}

/*
 * Return the port for this Beagle handle.
 *
 * The port number is a zero-indexed integer.
 */
public static int bg_port (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_port(beagle);
}

/*
 * Return the device features as a bit-mask of values, or
 * an error code if the handle is not valid.
 */
public const int BG_FEATURE_NONE = 0x00000000;
public const int BG_FEATURE_I2C = 0x00000001;
public const int BG_FEATURE_SPI = 0x00000002;
public const int BG_FEATURE_USB = 0x00000004;
public const int BG_FEATURE_MDIO = 0x00000008;
public const int BG_FEATURE_USB_HS = 0x00000010;
public const int BG_FEATURE_USB_SS = 0x00000020;
public static int bg_features (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_features(beagle);
}

public static int bg_unique_id_to_features (
    uint  unique_id
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_unique_id_to_features(unique_id);
}

/*
 * Return the unique ID for this Beagle adapter.
 * IDs are guaranteed to be non-zero if valid.
 * The ID is the unsigned integer representation of the
 * 10-digit serial number.
 */
public static uint bg_unique_id (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return 0;
    return net_bg_unique_id(beagle);
}

/*
 * Return the status string for the given status code.
 * If the code is not valid or the library function cannot
 * be loaded, return a NULL string.
 */
public static string bg_status_string (
    int  status
)
{
    if (!BG_LIBRARY_LOADED) return null;
    return Marshal.PtrToStringAnsi(net_bg_status_string(status));
}

/*
 * Return the version matrix for the device attached to the
 * given handle.  If the handle is 0 or invalid, only the
 * software and required api versions are set.
 */
public static int bg_version (
    int                beagle,
    ref BeagleVersion  version
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_version(beagle, ref version);
}

/*
 * Set the capture latency to the specified number of milliseconds.
 * This number determines the minimum time that a read call will
 * block if there is no available data.  Lower times result in
 * faster turnaround at the expense of reduced buffering.  Setting
 * this parameter too low can cause packets to be dropped.
 */
public static int bg_latency (
    int   beagle,
    uint  milliseconds
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_latency(beagle, milliseconds);
}

/*
 * Set the capture timeout to the specified number of milliseconds.
 * If any read call has a longer idle than this value, that call
 * will return with a count of 0 bytes.
 */
public static int bg_timeout (
    int   beagle,
    uint  milliseconds
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_timeout(beagle, milliseconds);
}

/*
 * Sleep for the specified number of milliseconds
 * Accuracy depends on the operating system scheduler
 * Returns the number of milliseconds slept
 */
public static uint bg_sleep_ms (
    uint  milliseconds
)
{
    if (!BG_LIBRARY_LOADED) return 0;
    return net_bg_sleep_ms(milliseconds);
}

/* Configure the target power pin. */
public const byte BG_TARGET_POWER_OFF = 0x00;
public const byte BG_TARGET_POWER_ON = 0x01;
public const byte BG_TARGET_POWER_QUERY = 0x80;
public static int bg_target_power (
    int   beagle,
    byte  power_flag
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_target_power(beagle, power_flag);
}

public const byte BG_HOST_IFCE_FULL_SPEED = 0x00;
public const byte BG_HOST_IFCE_HIGH_SPEED = 0x01;
public const byte BG_HOST_IFCE_SUPER_SPEED = 0x02;
public static int bg_host_ifce_speed (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_host_ifce_speed(beagle);
}

/* Returns the device address that the beagle is attached to. */
public static int bg_dev_addr (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_dev_addr(beagle);
}


/*=========================================================================
| BUFFERING API
 ==========================================================================
| Set the amount of buffering that is to be allocated on the PC.
| Pass zero to num_bytes to query the existing buffer siz*/
public static int bg_host_buffer_size (
    int   beagle,
    uint  num_bytes
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_host_buffer_size(beagle, num_bytes);
}

/* Query the amount of buffering that is unused and free for buffering. */
public static int bg_host_buffer_free (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_host_buffer_free(beagle);
}

/* Query the amount of buffering that is used and no longer available. */
public static int bg_host_buffer_used (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_host_buffer_used(beagle);
}

/* Benchmark the speed of the host to Beagle interface */
public static int bg_commtest (
    int  beagle,
    int  num_samples,
    int  delay_count
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_commtest(beagle, num_samples, delay_count);
}


/*=========================================================================
| MONITORING API
 ========================================================================*/
/* Protocol codes */
// enum BeagleProtocol  (from declaration above)
//     BG_PROTOCOL_NONE     = 0
//     BG_PROTOCOL_COMMTEST = 1
//     BG_PROTOCOL_USB      = 2
//     BG_PROTOCOL_I2C      = 3
//     BG_PROTOCOL_SPI      = 4
//     BG_PROTOCOL_MDIO     = 5

/*
 * Common Beagle read status codes
 * PARTIAL_LAST_BYTE Unused by USB 480 and 5000
 */
public const uint BG_READ_OK = 0x00000000;
public const uint BG_READ_TIMEOUT = 0x00000100;
public const uint BG_READ_ERR_MIDDLE_OF_PACKET = 0x00000200;
public const uint BG_READ_ERR_SHORT_BUFFER = 0x00000400;
public const uint BG_READ_ERR_PARTIAL_LAST_BYTE = 0x00000800;
public const uint BG_READ_ERR_PARTIAL_LAST_BYTE_MASK = 0x0000000f;
public const uint BG_READ_ERR_UNEXPECTED = 0x00001000;
/* Enable the Beagle monitor */
public static int bg_enable (
    int             beagle,
    BeagleProtocol  protocol
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_enable(beagle, protocol);
}

/* Disable the Beagle monitor */
public static int bg_disable (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_disable(beagle);
}

/*
 * Capture stop function only supported for analyzers with
 * on-board triggering capability.  For other analyzers, use
 * bg_disable to stop the capture and download to PC.
 */
public static int bg_capture_stop (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_capture_stop(beagle);
}

public static int bg_capture_trigger (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_capture_trigger(beagle);
}

/*
 * Capture status; general across protocols but used in
 * protocol-specific capture-status-query functions as well
 */
// enum BeagleCaptureStatus  (from declaration above)
//     BG_CAPTURE_STATUS_UNKNOWN          = -1
//     BG_CAPTURE_STATUS_INACTIVE         =  0
//     BG_CAPTURE_STATUS_SYNC_STANDBY     =  1
//     BG_CAPTURE_STATUS_PRE_TRIGGER      =  2
//     BG_CAPTURE_STATUS_PRE_TRIGGER_SYNC =  3
//     BG_CAPTURE_STATUS_POST_TRIGGER     =  4
//     BG_CAPTURE_STATUS_TRANSFER         =  5
//     BG_CAPTURE_STATUS_COMPLETE         =  6

public static int bg_capture_trigger_wait (
    int                      beagle,
    uint                     timeout_ms,
    ref BeagleCaptureStatus  status
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_capture_trigger_wait(beagle, timeout_ms, ref status);
}

/* Set the sample rate in kilohertz. */
public static int bg_samplerate (
    int  beagle,
    int  samplerate_khz
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_samplerate(beagle, samplerate_khz);
}

/*
 * Get the number of bits for the given number of bytes in the
 * given protocol.
 * Use this to determine how large a bit_timing array to allocate
 * for bg_*_read_bit_timing functions.
 */
public static int bg_bit_timing_size (
    BeagleProtocol  protocol,
    int             num_data_bytes
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_bit_timing_size(protocol, num_data_bytes);
}


/*=========================================================================
| I2C API
 ========================================================================*/
/* Configure the I2C pullup resistors. */
public const byte BG_I2C_PULLUP_OFF = 0x00;
public const byte BG_I2C_PULLUP_ON = 0x01;
public const byte BG_I2C_PULLUP_QUERY = 0x80;
public static int bg_i2c_pullup (
    int   beagle,
    byte  pullup_flag
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_i2c_pullup(beagle, pullup_flag);
}

public const ushort BG_I2C_MONITOR_DATA = 0x00ff;
public const ushort BG_I2C_MONITOR_NACK = 0x0100;
public const uint BG_READ_I2C_NO_STOP = 0x00010000;
public static int bg_i2c_read (
    int        beagle,
    ref uint   status,
    ref ulong  time_sop,
    ref ulong  time_duration,
    ref uint   time_dataoffset,
    int        max_bytes,
    ushort[]   data_in
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int data_in_max_bytes = (int)tp_min(max_bytes, data_in.Length);
    return net_bg_i2c_read(beagle, ref status, ref time_sop, ref time_duration, ref time_dataoffset, data_in_max_bytes, data_in);
}

public static int bg_i2c_read_data_timing (
    int        beagle,
    ref uint   status,
    ref ulong  time_sop,
    ref ulong  time_duration,
    ref uint   time_dataoffset,
    int        max_bytes,
    ushort[]   data_in,
    int        max_timing,
    uint[]     data_timing
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int data_in_max_bytes = (int)tp_min(max_bytes, data_in.Length);
    int data_timing_max_timing = (int)tp_min(max_timing, data_timing.Length);
    return net_bg_i2c_read_data_timing(beagle, ref status, ref time_sop, ref time_duration, ref time_dataoffset, data_in_max_bytes, data_in, data_timing_max_timing, data_timing);
}

public static int bg_i2c_read_bit_timing (
    int        beagle,
    ref uint   status,
    ref ulong  time_sop,
    ref ulong  time_duration,
    ref uint   time_dataoffset,
    int        max_bytes,
    ushort[]   data_in,
    int        max_timing,
    uint[]     bit_timing
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int data_in_max_bytes = (int)tp_min(max_bytes, data_in.Length);
    int bit_timing_max_timing = (int)tp_min(max_timing, bit_timing.Length);
    return net_bg_i2c_read_bit_timing(beagle, ref status, ref time_sop, ref time_duration, ref time_dataoffset, data_in_max_bytes, data_in, bit_timing_max_timing, bit_timing);
}


/*=========================================================================
| SPI API
 ========================================================================*/
// enum BeagleSpiSSPolarity  (from declaration above)
//     BG_SPI_SS_ACTIVE_LOW  = 0
//     BG_SPI_SS_ACTIVE_HIGH = 1

// enum BeagleSpiSckSamplingEdge  (from declaration above)
//     BG_SPI_SCK_SAMPLING_EDGE_RISING  = 0
//     BG_SPI_SCK_SAMPLING_EDGE_FALLING = 1

// enum BeagleSpiBitorder  (from declaration above)
//     BG_SPI_BITORDER_MSB = 0
//     BG_SPI_BITORDER_LSB = 1

public static int bg_spi_configure (
    int                       beagle,
    BeagleSpiSSPolarity       ss_polarity,
    BeagleSpiSckSamplingEdge  sck_sampling_edge,
    BeagleSpiBitorder         bitorder
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_spi_configure(beagle, ss_polarity, sck_sampling_edge, bitorder);
}

public static int bg_spi_read (
    int        beagle,
    ref uint   status,
    ref ulong  time_sop,
    ref ulong  time_duration,
    ref uint   time_dataoffset,
    int        mosi_max_bytes,
    byte[]     data_mosi,
    int        miso_max_bytes,
    byte[]     data_miso
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int data_mosi_mosi_max_bytes = (int)tp_min(mosi_max_bytes, data_mosi.Length);
    int data_miso_miso_max_bytes = (int)tp_min(miso_max_bytes, data_miso.Length);
    return net_bg_spi_read(beagle, ref status, ref time_sop, ref time_duration, ref time_dataoffset, data_mosi_mosi_max_bytes, data_mosi, data_miso_miso_max_bytes, data_miso);
}

public static int bg_spi_read_data_timing (
    int        beagle,
    ref uint   status,
    ref ulong  time_sop,
    ref ulong  time_duration,
    ref uint   time_dataoffset,
    int        mosi_max_bytes,
    byte[]     data_mosi,
    int        miso_max_bytes,
    byte[]     data_miso,
    int        max_timing,
    uint[]     data_timing
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int data_mosi_mosi_max_bytes = (int)tp_min(mosi_max_bytes, data_mosi.Length);
    int data_miso_miso_max_bytes = (int)tp_min(miso_max_bytes, data_miso.Length);
    int data_timing_max_timing = (int)tp_min(max_timing, data_timing.Length);
    return net_bg_spi_read_data_timing(beagle, ref status, ref time_sop, ref time_duration, ref time_dataoffset, data_mosi_mosi_max_bytes, data_mosi, data_miso_miso_max_bytes, data_miso, data_timing_max_timing, data_timing);
}

public static int bg_spi_read_bit_timing (
    int        beagle,
    ref uint   status,
    ref ulong  time_sop,
    ref ulong  time_duration,
    ref uint   time_dataoffset,
    int        mosi_max_bytes,
    byte[]     data_mosi,
    int        miso_max_bytes,
    byte[]     data_miso,
    int        max_timing,
    uint[]     bit_timing
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int data_mosi_mosi_max_bytes = (int)tp_min(mosi_max_bytes, data_mosi.Length);
    int data_miso_miso_max_bytes = (int)tp_min(miso_max_bytes, data_miso.Length);
    int bit_timing_max_timing = (int)tp_min(max_timing, bit_timing.Length);
    return net_bg_spi_read_bit_timing(beagle, ref status, ref time_sop, ref time_duration, ref time_dataoffset, data_mosi_mosi_max_bytes, data_mosi, data_miso_miso_max_bytes, data_miso, bit_timing_max_timing, bit_timing);
}


/*=========================================================================
| USB API
 ========================================================================*/
/* USB packet PID definitions */
public const byte BG_USB_PID_OUT = 0xe1;
public const byte BG_USB_PID_IN = 0x69;
public const byte BG_USB_PID_SOF = 0xa5;
public const byte BG_USB_PID_SETUP = 0x2d;
public const byte BG_USB_PID_DATA0 = 0xc3;
public const byte BG_USB_PID_DATA1 = 0x4b;
public const byte BG_USB_PID_DATA2 = 0x87;
public const byte BG_USB_PID_MDATA = 0x0f;
public const byte BG_USB_PID_ACK = 0xd2;
public const byte BG_USB_PID_NAK = 0x5a;
public const byte BG_USB_PID_STALL = 0x1e;
public const byte BG_USB_PID_NYET = 0x96;
public const byte BG_USB_PID_PRE = 0x3c;
public const byte BG_USB_PID_ERR = 0x3c;
public const byte BG_USB_PID_SPLIT = 0x78;
public const byte BG_USB_PID_PING = 0xb4;
public const byte BG_USB_PID_EXT = 0xf0;
public const byte BG_USB_PID_CORRUPTED = 0xff;
/* The following codes are returned for USB 12, 480, and 5000 captures */
public const uint BG_READ_USB_ERR_BAD_SIGNALS = 0x00010000;
public const uint BG_READ_USB_ERR_BAD_PID = 0x00200000;
public const uint BG_READ_USB_ERR_BAD_CRC = 0x00400000;
/* The following codes are only returned for USB 12 captures */
public const uint BG_READ_USB_ERR_BAD_SYNC = 0x00020000;
public const uint BG_READ_USB_ERR_BIT_STUFF = 0x00040000;
public const uint BG_READ_USB_ERR_FALSE_EOP = 0x00080000;
public const uint BG_READ_USB_ERR_LONG_EOP = 0x00100000;
/* The following codes are only returned for USB 480 and 5000  captures */
public const uint BG_READ_USB_TRUNCATION_LEN_MASK = 0x000000ff;
public const uint BG_READ_USB_TRUNCATION_MODE = 0x20000000;
public const uint BG_READ_USB_END_OF_CAPTURE = 0x40000000;
/* The following codes are only returned for USB 5000 captures */
public const uint BG_READ_USB_ERR_BAD_SLC_CRC_1 = 0x00008000;
public const uint BG_READ_USB_ERR_BAD_SLC_CRC_2 = 0x00010000;
public const uint BG_READ_USB_ERR_BAD_SHP_CRC_16 = 0x00008000;
public const uint BG_READ_USB_ERR_BAD_SHP_CRC_5 = 0x00010000;
public const uint BG_READ_USB_ERR_BAD_SDP_CRC = 0x00008000;
public const uint BG_READ_USB_EDB_FRAMING = 0x00020000;
public const uint BG_READ_USB_ERR_UNK_END_OF_FRAME = 0x00040000;
public const uint BG_READ_USB_ERR_DATA_LEN_INVALID = 0x00080000;
public const uint BG_READ_USB_PKT_TYPE_LINK = 0x00100000;
public const uint BG_READ_USB_PKT_TYPE_HDR = 0x00200000;
public const uint BG_READ_USB_PKT_TYPE_DP = 0x00400000;
public const uint BG_READ_USB_PKT_TYPE_TSEQ = 0x00800000;
public const uint BG_READ_USB_PKT_TYPE_TS1 = 0x01000000;
public const uint BG_READ_USB_PKT_TYPE_TS2 = 0x02000000;
public const uint BG_READ_USB_ERR_BAD_TS = 0x04000000;
public const uint BG_READ_USB_ERR_FRAMING = 0x08000000;
/* The following events are returned for USB 12, 480, and 5000 captures */
public const uint BG_EVENT_USB_HOST_DISCONNECT = 0x00000100;
public const uint BG_EVENT_USB_TARGET_DISCONNECT = 0x00000200;
public const uint BG_EVENT_USB_HOST_CONNECT = 0x00000400;
public const uint BG_EVENT_USB_TARGET_CONNECT = 0x00000800;
public const uint BG_EVENT_USB_RESET = 0x00001000;
/*
 * USB 480 and 5000 specific event codes
 * USB 2.0
 */
public const uint BG_EVENT_USB_DIGITAL_INPUT_MASK = 0x0000000f;
public const uint BG_EVENT_USB_CHIRP_J = 0x00002000;
public const uint BG_EVENT_USB_CHIRP_K = 0x00004000;
public const uint BG_EVENT_USB_SPEED_UNKNOWN = 0x00008000;
public const uint BG_EVENT_USB_LOW_SPEED = 0x00010000;
public const uint BG_EVENT_USB_FULL_SPEED = 0x00020000;
public const uint BG_EVENT_USB_HIGH_SPEED = 0x00040000;
public const uint BG_EVENT_USB_LOW_OVER_FULL_SPEED = 0x00080000;
public const uint BG_EVENT_USB_SUSPEND = 0x00100000;
public const uint BG_EVENT_USB_RESUME = 0x00200000;
public const uint BG_EVENT_USB_KEEP_ALIVE = 0x00400000;
public const uint BG_EVENT_USB_DIGITAL_INPUT = 0x00800000;
public const uint BG_EVENT_USB_OTG_HNP = 0x01000000;
public const uint BG_EVENT_USB_OTG_SRP_DATA_PULSE = 0x02000000;
public const uint BG_EVENT_USB_OTG_SRP_VBUS_PULSE = 0x04000000;
/*
 * USB 5000 specific event codes
 * USB 2.0
 */
public const uint BG_EVENT_USB_SMA_EXTIN_DETECTED = 0x08000000;
public const uint BG_EVENT_USB_CHIRP_DETECTED = 0x00000080;
/* USB 2.0 and USB 3.0 Trigger Event information */
public const uint BG_EVENT_USB_VBUS_TRIGGER = 0x08000000;
public const uint BG_EVENT_USB_COMPLEX_TIMER = 0x10000000;
public const uint BG_EVENT_USB_CROSS_TRIGGER = 0x20000000;
public const uint BG_EVENT_USB_COMPLEX_TRIGGER = 0x40000000;
public const uint BG_EVENT_USB_TRIGGER = 0x80000000;
public const uint BG_EVENT_USB_TRIGGER_STATE_MASK = 0x00000070;
public const int BG_EVENT_USB_TRIGGER_STATE_SHIFT = 4;
public const uint BG_EVENT_USB_TRIGGER_STATE_0 = 0x00000000;
public const uint BG_EVENT_USB_TRIGGER_STATE_1 = 0x00000010;
public const uint BG_EVENT_USB_TRIGGER_STATE_2 = 0x00000020;
public const uint BG_EVENT_USB_TRIGGER_STATE_3 = 0x00000030;
public const uint BG_EVENT_USB_TRIGGER_STATE_4 = 0x00000040;
public const uint BG_EVENT_USB_TRIGGER_STATE_5 = 0x00000050;
public const uint BG_EVENT_USB_TRIGGER_STATE_6 = 0x00000060;
public const uint BG_EVENT_USB_TRIGGER_STATE_7 = 0x00000070;
/* USB 3.0 US and DS, and ASYNC streams */
public const uint BG_EVENT_USB_LFPS = 0x00001000;
public const uint BG_EVENT_USB_LTSSM = 0x00002000;
public const uint BG_EVENT_USB_VBUS_PRESENT = 0x00010000;
public const uint BG_EVENT_USB_VBUS_ABSENT = 0x00020000;
public const uint BG_EVENT_USB_SCRAMBLING_ENABLED = 0x00040000;
public const uint BG_EVENT_USB_SCRAMBLING_DISABLED = 0x00080000;
public const uint BG_EVENT_USB_POLARITY_NORMAL = 0x00100000;
public const uint BG_EVENT_USB_POLARITY_REVERSED = 0x00200000;
public const uint BG_EVENT_USB_PHY_ERROR = 0x00400000;
public const uint BG_EVENT_USB_LTSSM_MASK = 0x000000ff;
public const uint BG_EVENT_USB_LTSSM_STATE_UNKNOWN = 0x00000000;
public const uint BG_EVENT_USB_LTSSM_STATE_SS_DISABLED = 0x00000001;
public const uint BG_EVENT_USB_LTSSM_STATE_SS_INACTIVE = 0x00000002;
public const uint BG_EVENT_USB_LTSSM_STATE_RX_DETECT_RESET = 0x00000003;
public const uint BG_EVENT_USB_LTSSM_STATE_RX_DETECT_ACTIVE = 0x00000004;
public const uint BG_EVENT_USB_LTSSM_STATE_POLLING_LFPS = 0x00000005;
public const uint BG_EVENT_USB_LTSSM_STATE_POLLING_RXEQ = 0x00000006;
public const uint BG_EVENT_USB_LTSSM_STATE_POLLING_ACTIVE = 0x00000007;
public const uint BG_EVENT_USB_LTSSM_STATE_POLLING_CONFIG = 0x00000008;
public const uint BG_EVENT_USB_LTSSM_STATE_POLLING_IDLE = 0x00000009;
public const uint BG_EVENT_USB_LTSSM_STATE_U0 = 0x0000000a;
public const uint BG_EVENT_USB_LTSSM_STATE_U1 = 0x0000000b;
public const uint BG_EVENT_USB_LTSSM_STATE_U2 = 0x0000000c;
public const uint BG_EVENT_USB_LTSSM_STATE_U3 = 0x0000000d;
public const uint BG_EVENT_USB_LTSSM_STATE_RECOVERY_ACTIVE = 0x0000000e;
public const uint BG_EVENT_USB_LTSSM_STATE_RECOVERY_CONFIG = 0x0000000f;
public const uint BG_EVENT_USB_LTSSM_STATE_RECOVERY_IDLE = 0x00000010;
public const uint BG_EVENT_USB_LTSSM_STATE_HOT_RESET_ACTIVE = 0x00000011;
public const uint BG_EVENT_USB_LTSSM_STATE_HOT_RESET_EXIT = 0x00000012;
public const uint BG_EVENT_USB_LTSSM_STATE_LOOPBACK_ACTIVE = 0x00000013;
public const uint BG_EVENT_USB_LTSSM_STATE_LOOPBACK_EXIT = 0x00000014;
public const uint BG_EVENT_USB_LTSSM_STATE_COMPLIANCE = 0x00000015;
public const uint BG_EVENT_USB_SMA_EXTIN_ASSERTED = 0x01000000;
public const uint BG_EVENT_USB_SMA_EXTIN_DEASSERTED = 0x02000000;
public const uint BG_EVENT_USB_TRIGGER_5GBIT_START = 0x04000000;
public const uint BG_EVENT_USB_TRIGGER_5GBIT_STOP = 0x08000000;
/* Beagle USB feature bits */
public const int BG_USB_FEATURE_NONE = 0x00000000;
public const int BG_USB_FEATURE_USB2_MON = 0x00000001;
public const int BG_USB_FEATURE_USB3_MON = 0x00000002;
public const int BG_USB_FEATURE_SIMUL_23 = 0x00000004;
public const int BG_USB_FEATURE_USB3_CMP_TRIG = 0x00000008;
public const int BG_USB_FEATURE_USB3_4G_MEM = 0x00000020;
public const int BG_USB_FEATURE_USB2_CMP_TRIG = 0x00000080;
public const int BG_USB_FEATURE_CROSS_ANALYZER_SYNC = 0x00000100;
public const int BG_USB_FEATURE_USB3_DOWNLINK = 0x00000200;
public const int BG_USB_FEATURE_IV_MON_LITE = 0x00000400;
public static int bg_usb_features (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb_features(beagle);
}

/* License constants */
public const int BG_USB_LICENSE_LENGTH = 60;
/*
 * Read the license key string and return the features
 * Length must be set to BG_USB_LICENSE_LENGTH in order
 * for license_key to be populated.
 */
public static int bg_usb_license_read (
    int     beagle,
    int     length,
    byte[]  license_key
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int license_key_length = (int)tp_min(length, license_key.Length);
    return net_bg_usb_license_read(beagle, license_key_length, license_key);
}

/*
 * Write the license key string and return the features
 * Length must be set to BG_USB_LICENSE_LENGTH.  If
 * the license is not valid or the length is not set to
 * BG_USB_LICENSE_LENGTH, an invalid license error is
 * returned.
 */
public static int bg_usb_license_write (
    int     beagle,
    int     length,
    byte[]  license_key
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int license_key_length = (int)tp_min(length, license_key.Length);
    return net_bg_usb_license_write(beagle, license_key_length, license_key);
}

/* Capture modes */
public const byte BG_USB_CAPTURE_USB3 = 0x01;
public const byte BG_USB_CAPTURE_USB2 = 0x02;
public const byte BG_USB_CAPTURE_IV_MON_LITE = 0x08;
/* Trigger modes */
// enum BeagleUsbTriggerMode  (from declaration above)
//     BG_USB_TRIGGER_MODE_EVENT     = 0
//     BG_USB_TRIGGER_MODE_IMMEDIATE = 1

public static int bg_usb_configure (
    int                   beagle,
    byte                  cap_mask,
    BeagleUsbTriggerMode  trigger_mode
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb_configure(beagle, cap_mask, trigger_mode);
}

/* USB Target Power */
// enum BeagleUsbTargetPower  (from declaration above)
//     BG_USB_TARGET_POWER_HOST_SUPPLIED = 0
//     BG_USB_TARGET_POWER_OFF           = 1

public static int bg_usb_target_power (
    int                   beagle,
    BeagleUsbTargetPower  power_flag
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb_target_power(beagle, power_flag);
}

/* USB 2 Configuration */
/* USB 2 Capture modes */
// enum BeagleUsb2CaptureMode  (from declaration above)
//     BG_USB2_CAPTURE_REALTIME                 = 0
//     BG_USB2_CAPTURE_REALTIME_WITH_PROTECTION = 1
//     BG_USB2_CAPTURE_DELAYED_DOWNLOAD         = 2

public static int bg_usb2_capture_config (
    int                    beagle,
    BeagleUsb2CaptureMode  capture_mode
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_capture_config(beagle, capture_mode);
}

/* Target configs */
public const uint BG_USB2_AUTO_SPEED_DETECT = 0;
public const uint BG_USB2_LOW_SPEED = 1;
public const uint BG_USB2_FULL_SPEED = 2;
public const uint BG_USB2_HIGH_SPEED = 3;
public const uint BG_USB2_VBUS_OVERRIDE = 0x00000080;
public static int bg_usb2_target_config (
    int   beagle,
    uint  target_config
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_target_config(beagle, target_config);
}

/* General constants */
public const uint BG_USB_CAPTURE_SIZE_INFINITE = 0;
public const uint BG_USB_CAPTURE_SIZE_SCALE = 0xffffffff;
/* USB 2 Capture modes */
public static int bg_usb2_capture_buffer_config (
    int   beagle,
    uint  pretrig_kb,
    uint  capture_kb
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_capture_buffer_config(beagle, pretrig_kb, capture_kb);
}

public static int bg_usb2_capture_buffer_config_query (
    int       beagle,
    ref uint  pretrig_kb,
    ref uint  capture_kb
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_capture_buffer_config_query(beagle, ref pretrig_kb, ref capture_kb);
}

public static int bg_usb2_capture_status (
    int                      beagle,
    ref BeagleCaptureStatus  status,
    ref uint                 pretrig_remaining_kb,
    ref uint                 pretrig_total_kb,
    ref uint                 capture_remaining_kb,
    ref uint                 capture_total_kb
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_capture_status(beagle, ref status, ref pretrig_remaining_kb, ref pretrig_total_kb, ref capture_remaining_kb, ref capture_total_kb);
}

/* Digital output configuration */
public const byte BG_USB2_DIGITAL_OUT_ENABLE_PIN1 = 0x01;
public const byte BG_USB2_DIGITAL_OUT_PIN1_ACTIVE_HIGH = 0x01;
public const byte BG_USB2_DIGITAL_OUT_PIN1_ACTIVE_LOW = 0x00;
public const byte BG_USB2_DIGITAL_OUT_ENABLE_PIN2 = 0x02;
public const byte BG_USB2_DIGITAL_OUT_PIN2_ACTIVE_HIGH = 0x02;
public const byte BG_USB2_DIGITAL_OUT_PIN2_ACTIVE_LOW = 0x00;
public const byte BG_USB2_DIGITAL_OUT_ENABLE_PIN3 = 0x04;
public const byte BG_USB2_DIGITAL_OUT_PIN3_ACTIVE_HIGH = 0x04;
public const byte BG_USB2_DIGITAL_OUT_PIN3_ACTIVE_LOW = 0x00;
public const byte BG_USB2_DIGITAL_OUT_ENABLE_PIN4 = 0x08;
public const byte BG_USB2_DIGITAL_OUT_PIN4_ACTIVE_HIGH = 0x08;
public const byte BG_USB2_DIGITAL_OUT_PIN4_ACTIVE_LOW = 0x00;
public static int bg_usb2_digital_out_config (
    int   beagle,
    byte  out_enable_mask,
    byte  out_polarity_mask
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_digital_out_config(beagle, out_enable_mask, out_polarity_mask);
}

/* Digital output match pin configuration */
// enum BeagleUsb2DigitalOutMatchPins  (from declaration above)
//     BG_USB2_DIGITAL_OUT_MATCH_PIN3 = 3
//     BG_USB2_DIGITAL_OUT_MATCH_PIN4 = 4

/* Packet matching modes */
// enum BeagleUsb2MatchType  (from declaration above)
//     BG_USB2_MATCH_TYPE_DISABLED  = 0
//     BG_USB2_MATCH_TYPE_EQUAL     = 1
//     BG_USB2_MATCH_TYPE_NOT_EQUAL = 2

/* Digital ouput matching configuration */
[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb2PacketMatch {
    public BeagleUsb2MatchType pid_match_type;
    public byte                pid_match_val;
    public BeagleUsb2MatchType dev_match_type;
    public byte                dev_match_val;
    public BeagleUsb2MatchType ep_match_type;
    public byte                ep_match_val;
}

/* Data match PID mask */
public const byte BG_USB2_DATA_MATCH_DATA0 = 0x01;
public const byte BG_USB2_DATA_MATCH_DATA1 = 0x02;
public const byte BG_USB2_DATA_MATCH_DATA2 = 0x04;
public const byte BG_USB2_DATA_MATCH_MDATA = 0x08;
[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb2DataMatch {
    public BeagleUsb2MatchType data_match_type;
    public byte                data_match_pid;
    public byte[]              data;
    public byte[]              data_valid;
}

[StructLayout(LayoutKind.Sequential)]
private struct c_BeagleUsb2DataMatch {
    public BeagleUsb2MatchType   data_match_type;
    public byte                  data_match_pid;
    public ushort                data_length;
    public IntPtr                data;
    public ushort                data_valid_length;
    public IntPtr                data_valid;
}

private static c_BeagleUsb2DataMatch to_c_BeagleUsb2DataMatch (
    ref c_BeagleUsb2DataMatch c_s,
    ref BeagleUsb2DataMatch s,
    GCContext gcc,
    bool is_in_arg
)
{
    /* Process arrays */
    if (s.data != null) {
        c_s.data_length = (ushort)s.data.Length;
        GCHandle data_gch = GCHandle.Alloc(s.data, GCHandleType.Pinned);
        c_s.data = data_gch.AddrOfPinnedObject();
        gcc.add(data_gch);
    }
    else {
        c_s.data_length = 0;
        c_s.data = IntPtr.Zero;
    }
    if (s.data_valid != null) {
        c_s.data_valid_length = (ushort)s.data_valid.Length;
        GCHandle data_valid_gch = GCHandle.Alloc(s.data_valid, GCHandleType.Pinned);
        c_s.data_valid = data_valid_gch.AddrOfPinnedObject();
        gcc.add(data_valid_gch);
    }
    else {
        c_s.data_valid_length = 0;
        c_s.data_valid = IntPtr.Zero;
    }

    /* Process simple arguments */
    if (is_in_arg) {
        c_s.data_match_type = s.data_match_type;
        c_s.data_match_pid = s.data_match_pid;
    }

    return c_s;
}

public static int bg_usb2_digital_out_match (
    int                            beagle,
    BeagleUsb2DigitalOutMatchPins  pin_num,
    BeagleUsb2PacketMatch          packet_match,
    BeagleUsb2DataMatch            data_match
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    GCContext gcc = new GCContext();
    c_BeagleUsb2DataMatch c_data_match = new c_BeagleUsb2DataMatch();
    to_c_BeagleUsb2DataMatch(ref c_data_match, ref data_match, gcc, true);
    int ret = net_bg_usb2_digital_out_match(beagle, pin_num, ref packet_match, ref c_data_match);
    gcc.free();
    return ret;
}

/* Digital input pin configuration */
public const byte BG_USB2_DIGITAL_IN_ENABLE_PIN1 = 0x01;
public const byte BG_USB2_DIGITAL_IN_ENABLE_PIN2 = 0x02;
public const byte BG_USB2_DIGITAL_IN_ENABLE_PIN3 = 0x04;
public const byte BG_USB2_DIGITAL_IN_ENABLE_PIN4 = 0x08;
public static int bg_usb2_digital_in_config (
    int   beagle,
    byte  in_enable_mask
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_digital_in_config(beagle, in_enable_mask);
}

/* Hardware filtering configuration */
public const byte BG_USB2_HW_FILTER_PID_SOF = 0x01;
public const byte BG_USB2_HW_FILTER_PID_IN = 0x02;
public const byte BG_USB2_HW_FILTER_PID_PING = 0x04;
public const byte BG_USB2_HW_FILTER_PID_PRE = 0x08;
public const byte BG_USB2_HW_FILTER_PID_SPLIT = 0x10;
public const byte BG_USB2_HW_FILTER_SELF = 0x20;
public static int bg_usb2_hw_filter_config (
    int   beagle,
    byte  filter_enable_mask
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_hw_filter_config(beagle, filter_enable_mask);
}

public static int bg_usb2_simple_match_config (
    int   beagle,
    byte  dig_in_pin_pos_edge_mask,
    byte  dig_in_pin_neg_edge_mask,
    byte  dig_out_match_pin_mask
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_simple_match_config(beagle, dig_in_pin_pos_edge_mask, dig_in_pin_neg_edge_mask, dig_out_match_pin_mask);
}

/* USB 2.0 Complex matching enable/disable */
public static int bg_usb2_complex_match_enable (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_complex_match_enable(beagle);
}

public static int bg_usb2_complex_match_disable (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_complex_match_disable(beagle);
}

// enum BeagleUsbMatchType  (from declaration above)
//     BG_USB_MATCH_TYPE_DISABLED      = 0
//     BG_USB_MATCH_TYPE_EQUAL         = 1
//     BG_USB_MATCH_TYPE_LESS_EQUAL    = 2
//     BG_USB_MATCH_TYPE_GREATER_EQUAL = 3
//     BG_USB_MATCH_TYPE_NOT_EQUAL     = 4

// enum BeagleUsb2DataMatchDirection  (from declaration above)
//     BG_USB2_MATCH_DIRECTION_DISABLED  = 0
//     BG_USB2_MATCH_DIRECTION_IN        = 1
//     BG_USB2_MATCH_DIRECTION_OUT_SETUP = 2
//     BG_USB2_MATCH_DIRECTION_SETUP     = 3

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb2DataProperties {
    public BeagleUsb2DataMatchDirection direction;
    public BeagleUsbMatchType           ep_match_type;
    public byte                         ep_match_val;
    public BeagleUsbMatchType           dev_match_type;
    public byte                         dev_match_val;
    public BeagleUsbMatchType           data_len_match_type;
    public ushort                       data_len_match_val;
}

// enum BeagleUsb2PacketType  (from declaration above)
//     BG_USB2_MATCH_PACKET_IN          = 0x0009
//     BG_USB2_MATCH_PACKET_OUT         = 0x0001
//     BG_USB2_MATCH_PACKET_SETUP       = 0x000d
//     BG_USB2_MATCH_PACKET_SOF         = 0x0005
//     BG_USB2_MATCH_PACKET_DATA0       = 0x0003
//     BG_USB2_MATCH_PACKET_DATA1       = 0x000b
//     BG_USB2_MATCH_PACKET_DATA2       = 0x0007
//     BG_USB2_MATCH_PACKET_MDATA       = 0x000f
//     BG_USB2_MATCH_PACKET_ACK         = 0x0002
//     BG_USB2_MATCH_PACKET_NAK         = 0x000a
//     BG_USB2_MATCH_PACKET_STALL       = 0x000e
//     BG_USB2_MATCH_PACKET_NYET        = 0x0006
//     BG_USB2_MATCH_PACKET_PRE         = 0x000c
//     BG_USB2_MATCH_PACKET_ERR         = 0x010c
//     BG_USB2_MATCH_PACKET_SPLIT       = 0x0008
//     BG_USB2_MATCH_PACKET_EXT         = 0x0000
//     BG_USB2_MATCH_PACKET_ANY         = 0x0010
//     BG_USB2_MATCH_PACKET_DATA0_DATA1 = 0x0020
//     BG_USB2_MATCH_PACKET_DATAX       = 0x0040
//     BG_USB2_MATCH_PACKET_SUBPID_MASK = 0x0100
//     BG_USB2_MATCH_PACKET_ERROR       = 0x1000

// enum BeagleUsb2DataMatchPrefix  (from declaration above)
//     BG_USB2_MATCH_PREFIX_DISABLED     = 0
//     BG_USB2_MATCH_PREFIX_IN           = 1
//     BG_USB2_MATCH_PREFIX_OUT          = 2
//     BG_USB2_MATCH_PREFIX_SETUP        = 3
//     BG_USB2_MATCH_PREFIX_CSPLIT       = 4
//     BG_USB2_MATCH_PREFIX_CSPLIT_IN    = 5
//     BG_USB2_MATCH_PREFIX_SSPLIT_OUT   = 6
//     BG_USB2_MATCH_PREFIX_SSPLIT_SETUP = 7

public const int BG_USB2_MATCH_HANDSHAKE_MASK_DISABLED = 0x00000000;
public const int BG_USB2_MATCH_HANDSHAKE_MASK_NONE = 0x00000001;
public const int BG_USB2_MATCH_HANDSHAKE_MASK_ACK = 0x00000002;
public const int BG_USB2_MATCH_HANDSHAKE_MASK_NAK = 0x00000004;
public const int BG_USB2_MATCH_HANDSHAKE_MASK_NYET = 0x00000008;
public const int BG_USB2_MATCH_HANDSHAKE_MASK_STALL = 0x00000010;
// enum BeagleUsb2ErrorType  (from declaration above)
//     BG_USB2_MATCH_CRC_DONT_CARE          =    0
//     BG_USB2_MATCH_CRC_VALID              =    1
//     BG_USB2_MATCH_CRC_INVALID            =    2
//     BG_USB2_MATCH_ERR_MASK_CORRUPTED_PID = 0x10
//     BG_USB2_MATCH_ERR_MASK_CRC           = 0x20
//     BG_USB2_MATCH_ERR_MASK_RXERROR       = 0x40
//     BG_USB2_MATCH_ERR_MASK_JABBER        = 0x80

// enum BeagleUsb2MatchModifier  (from declaration above)
//     BG_USB2_MATCH_MODIFIER_0 = 0
//     BG_USB2_MATCH_MODIFIER_1 = 1
//     BG_USB2_MATCH_MODIFIER_2 = 2
//     BG_USB2_MATCH_MODIFIER_3 = 3

public const byte BG_USB_COMPLEX_MATCH_ACTION_EXTOUT = 0x01;
public const byte BG_USB_COMPLEX_MATCH_ACTION_TRIGGER = 0x02;
public const byte BG_USB_COMPLEX_MATCH_ACTION_FILTER = 0x04;
public const byte BG_USB_COMPLEX_MATCH_ACTION_GOTO = 0x08;
[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb2DataMatchUnit {
    public BeagleUsb2PacketType      packet_type;
    public BeagleUsb2DataMatchPrefix prefix;
    public byte                      handshake;
    public byte[]                    data;
    public byte[]                    data_valid;
    public BeagleUsb2ErrorType       err_match;
    public byte                      data_properties_valid;
    public BeagleUsb2DataProperties  data_properties;
    public BeagleUsb2MatchModifier   match_modifier;
    public ushort                    repeat_count;
    public byte                      sticky_action;
    public byte                      action_mask;
    public byte                      goto_selector;
}

[StructLayout(LayoutKind.Sequential)]
private struct c_BeagleUsb2DataMatchUnit {
    public BeagleUsb2PacketType        packet_type;
    public BeagleUsb2DataMatchPrefix   prefix;
    public byte                        handshake;
    public ushort                      data_length;
    public IntPtr                      data;
    public ushort                      data_valid_length;
    public IntPtr                      data_valid;
    public BeagleUsb2ErrorType         err_match;
    public byte                        data_properties_valid;
    public BeagleUsb2DataProperties    data_properties;
    public BeagleUsb2MatchModifier     match_modifier;
    public ushort                      repeat_count;
    public byte                        sticky_action;
    public byte                        action_mask;
    public byte                        goto_selector;
}

private static c_BeagleUsb2DataMatchUnit to_c_BeagleUsb2DataMatchUnit (
    ref c_BeagleUsb2DataMatchUnit c_s,
    ref BeagleUsb2DataMatchUnit s,
    GCContext gcc,
    bool is_in_arg
)
{
    /* Process arrays */
    if (s.data != null) {
        c_s.data_length = (ushort)s.data.Length;
        GCHandle data_gch = GCHandle.Alloc(s.data, GCHandleType.Pinned);
        c_s.data = data_gch.AddrOfPinnedObject();
        gcc.add(data_gch);
    }
    else {
        c_s.data_length = 0;
        c_s.data = IntPtr.Zero;
    }
    if (s.data_valid != null) {
        c_s.data_valid_length = (ushort)s.data_valid.Length;
        GCHandle data_valid_gch = GCHandle.Alloc(s.data_valid, GCHandleType.Pinned);
        c_s.data_valid = data_valid_gch.AddrOfPinnedObject();
        gcc.add(data_valid_gch);
    }
    else {
        c_s.data_valid_length = 0;
        c_s.data_valid = IntPtr.Zero;
    }

    /* Process simple arguments */
    if (is_in_arg) {
        c_s.packet_type = s.packet_type;
        c_s.prefix = s.prefix;
        c_s.handshake = s.handshake;
        c_s.err_match = s.err_match;
        c_s.data_properties_valid = s.data_properties_valid;
        c_s.match_modifier = s.match_modifier;
        c_s.repeat_count = s.repeat_count;
        c_s.sticky_action = s.sticky_action;
        c_s.action_mask = s.action_mask;
        c_s.goto_selector = s.goto_selector;
    }

    /* Process structures */
    c_s.data_properties = s.data_properties;

    return c_s;
}

// enum BeagleUsbTimerUnit  (from declaration above)
//     BG_USB_TIMER_UNIT_DISABLED = 0
//     BG_USB_TIMER_UNIT_NS       = 1
//     BG_USB_TIMER_UNIT_US       = 2
//     BG_USB_TIMER_UNIT_MS       = 3
//     BG_USB_TIMER_UNIT_SEC      = 4

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb2TimerMatchUnit {
    public BeagleUsbTimerUnit timer_unit;
    public uint               timer_val;
    public byte               action_mask;
    public byte               goto_selector;
}

// enum BeagleUsb2AsyncEventType  (from declaration above)
//     BG_USB2_COMPLEX_MATCH_EVENT_DIGIN1        =  0
//     BG_USB2_COMPLEX_MATCH_EVENT_DIGIN2        =  1
//     BG_USB2_COMPLEX_MATCH_EVENT_DIGIN3        =  2
//     BG_USB2_COMPLEX_MATCH_EVENT_DIGIN4        =  3
//     BG_USB2_COMPLEX_MATCH_EVENT_CHIRP         = 13
//     BG_USB2_COMPLEX_MATCH_EVENT_SMA_EXTIN     = 14
//     BG_USB2_COMPLEX_MATCH_EVENT_CROSS_TRIGGER = 15
//     BG_USB2_COMPLEX_MATCH_EVENT_VBUS_TRIGGER  = 16

// enum BeagleUsb2VbusTriggerType  (from declaration above)
//     BG_USB2_VBUS_TRIGGER_TYPE_CURRENT = 1
//     BG_USB2_VBUS_TRIGGER_TYPE_VOLTAGE = 2

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb2AsyncEventMatchUnit {
    public BeagleUsb2AsyncEventType  event_type;
    public byte                      edge_mask;
    public ushort                    repeat_count;
    public byte                      sticky_action;
    public byte                      action_mask;
    public byte                      goto_selector;
    public BeagleUsb2VbusTriggerType vbus_trigger_type;
    public float                     vbus_trigger_val;
}

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb2ComplexMatchState {
    public byte                          data_0_valid;
    public BeagleUsb2DataMatchUnit       data_0;
    public byte                          data_1_valid;
    public BeagleUsb2DataMatchUnit       data_1;
    public byte                          data_2_valid;
    public BeagleUsb2DataMatchUnit       data_2;
    public byte                          data_3_valid;
    public BeagleUsb2DataMatchUnit       data_3;
    public byte                          timer_valid;
    public BeagleUsb2TimerMatchUnit      timer;
    public byte                          async_valid;
    public BeagleUsb2AsyncEventMatchUnit async;
    public byte                          goto_0;
    public byte                          goto_1;
    public byte                          goto_2;
}

[StructLayout(LayoutKind.Sequential)]
private struct c_BeagleUsb2ComplexMatchState {
    public byte                            data_0_valid;
    public c_BeagleUsb2DataMatchUnit       data_0;
    public byte                            data_1_valid;
    public c_BeagleUsb2DataMatchUnit       data_1;
    public byte                            data_2_valid;
    public c_BeagleUsb2DataMatchUnit       data_2;
    public byte                            data_3_valid;
    public c_BeagleUsb2DataMatchUnit       data_3;
    public byte                            timer_valid;
    public BeagleUsb2TimerMatchUnit        timer;
    public byte                            async_valid;
    public BeagleUsb2AsyncEventMatchUnit   async;
    public byte                            goto_0;
    public byte                            goto_1;
    public byte                            goto_2;
}

private static c_BeagleUsb2ComplexMatchState to_c_BeagleUsb2ComplexMatchState (
    ref c_BeagleUsb2ComplexMatchState c_s,
    ref BeagleUsb2ComplexMatchState s,
    GCContext gcc,
    bool is_in_arg
)
{
    /* Process simple arguments */
    if (is_in_arg) {
        c_s.data_0_valid = s.data_0_valid;
        c_s.data_1_valid = s.data_1_valid;
        c_s.data_2_valid = s.data_2_valid;
        c_s.data_3_valid = s.data_3_valid;
        c_s.timer_valid = s.timer_valid;
        c_s.async_valid = s.async_valid;
        c_s.goto_0 = s.goto_0;
        c_s.goto_1 = s.goto_1;
        c_s.goto_2 = s.goto_2;
    }

    /* Process structures */
    to_c_BeagleUsb2DataMatchUnit(ref c_s.data_0, ref s.data_0, gcc, is_in_arg);
    to_c_BeagleUsb2DataMatchUnit(ref c_s.data_1, ref s.data_1, gcc, is_in_arg);
    to_c_BeagleUsb2DataMatchUnit(ref c_s.data_2, ref s.data_2, gcc, is_in_arg);
    to_c_BeagleUsb2DataMatchUnit(ref c_s.data_3, ref s.data_3, gcc, is_in_arg);
    c_s.timer = s.timer;
    c_s.async = s.async;

    return c_s;
}

public static int bg_usb2_complex_match_config (
    int                          beagle,
    byte                         validate,
    byte                         digout,
    BeagleUsb2ComplexMatchState  state_0,
    BeagleUsb2ComplexMatchState  state_1,
    BeagleUsb2ComplexMatchState  state_2,
    BeagleUsb2ComplexMatchState  state_3,
    BeagleUsb2ComplexMatchState  state_4,
    BeagleUsb2ComplexMatchState  state_5,
    BeagleUsb2ComplexMatchState  state_6,
    BeagleUsb2ComplexMatchState  state_7
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    GCContext gcc = new GCContext();
    c_BeagleUsb2ComplexMatchState c_state_0 = new c_BeagleUsb2ComplexMatchState();
    to_c_BeagleUsb2ComplexMatchState(ref c_state_0, ref state_0, gcc, true);
    c_BeagleUsb2ComplexMatchState c_state_1 = new c_BeagleUsb2ComplexMatchState();
    to_c_BeagleUsb2ComplexMatchState(ref c_state_1, ref state_1, gcc, true);
    c_BeagleUsb2ComplexMatchState c_state_2 = new c_BeagleUsb2ComplexMatchState();
    to_c_BeagleUsb2ComplexMatchState(ref c_state_2, ref state_2, gcc, true);
    c_BeagleUsb2ComplexMatchState c_state_3 = new c_BeagleUsb2ComplexMatchState();
    to_c_BeagleUsb2ComplexMatchState(ref c_state_3, ref state_3, gcc, true);
    c_BeagleUsb2ComplexMatchState c_state_4 = new c_BeagleUsb2ComplexMatchState();
    to_c_BeagleUsb2ComplexMatchState(ref c_state_4, ref state_4, gcc, true);
    c_BeagleUsb2ComplexMatchState c_state_5 = new c_BeagleUsb2ComplexMatchState();
    to_c_BeagleUsb2ComplexMatchState(ref c_state_5, ref state_5, gcc, true);
    c_BeagleUsb2ComplexMatchState c_state_6 = new c_BeagleUsb2ComplexMatchState();
    to_c_BeagleUsb2ComplexMatchState(ref c_state_6, ref state_6, gcc, true);
    c_BeagleUsb2ComplexMatchState c_state_7 = new c_BeagleUsb2ComplexMatchState();
    to_c_BeagleUsb2ComplexMatchState(ref c_state_7, ref state_7, gcc, true);
    int ret = net_bg_usb2_complex_match_config(beagle, validate, digout, ref c_state_0, ref c_state_1, ref c_state_2, ref c_state_3, ref c_state_4, ref c_state_5, ref c_state_6, ref c_state_7);
    gcc.free();
    return ret;
}

public static int bg_usb2_complex_match_config_single (
    int                          beagle,
    byte                         validate,
    byte                         digout,
    BeagleUsb2ComplexMatchState  state
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    GCContext gcc = new GCContext();
    c_BeagleUsb2ComplexMatchState c_state = new c_BeagleUsb2ComplexMatchState();
    to_c_BeagleUsb2ComplexMatchState(ref c_state, ref state, gcc, true);
    int ret = net_bg_usb2_complex_match_config_single(beagle, validate, digout, ref c_state);
    gcc.free();
    return ret;
}

// enum BeagleUsbExtoutType  (from declaration above)
//     BG_USB_EXTOUT_LOW       = 0
//     BG_USB_EXTOUT_HIGH      = 1
//     BG_USB_EXTOUT_POS_PULSE = 2
//     BG_USB_EXTOUT_NEG_PULSE = 3
//     BG_USB_EXTOUT_TOGGLE_0  = 4
//     BG_USB_EXTOUT_TOGGLE_1  = 5

public static int bg_usb2_extout_config (
    int                  beagle,
    BeagleUsbExtoutType  extout_modulation
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_extout_config(beagle, extout_modulation);
}

// enum BeagleMemoryTestResult  (from declaration above)
//     BG_USB_MEMORY_TEST_PASS = 0
//     BG_USB_MEMORY_TEST_FAIL = 1

public static int bg_usb2_memory_test (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_memory_test(beagle);
}

/* USB 3 Configuration */
/* USB 3 Capture modes */
public static int bg_usb3_capture_buffer_config (
    int   beagle,
    uint  pretrig_kb,
    uint  capture_kb
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb3_capture_buffer_config(beagle, pretrig_kb, capture_kb);
}

public static int bg_usb3_capture_buffer_config_query (
    int       beagle,
    ref uint  pretrig_kb,
    ref uint  capture_kb
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb3_capture_buffer_config_query(beagle, ref pretrig_kb, ref capture_kb);
}

public static int bg_usb3_capture_status (
    int                      beagle,
    ref BeagleCaptureStatus  status,
    ref uint                 pretrig_remaining_kb,
    ref uint                 pretrig_total_kb,
    ref uint                 capture_remaining_kb,
    ref uint                 capture_total_kb
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb3_capture_status(beagle, ref status, ref pretrig_remaining_kb, ref pretrig_total_kb, ref capture_remaining_kb, ref capture_total_kb);
}

public const byte BG_USB3_PHY_CONFIG_POLARITY_NON_INVERT = 0x00;
public const byte BG_USB3_PHY_CONFIG_POLARITY_INVERT = 0x01;
public const byte BG_USB3_PHY_CONFIG_POLARITY_AUTO = 0x02;
public const byte BG_USB3_PHY_CONFIG_POLARITY_MASK = 0x03;
public const byte BG_USB3_PHY_CONFIG_DESCRAMBLER_ON = 0x00;
public const byte BG_USB3_PHY_CONFIG_DESCRAMBLER_OFF = 0x04;
public const byte BG_USB3_PHY_CONFIG_DESCRAMBLER_AUTO = 0x08;
public const byte BG_USB3_PHY_CONFIG_DESCRAMBLER_MASK = 0x0c;
public const byte BG_USB3_PHY_CONFIG_RXTERM_ON = 0x00;
public const byte BG_USB3_PHY_CONFIG_RXTERM_OFF = 0x10;
public const byte BG_USB3_PHY_CONFIG_RXTERM_AUTO = 0x20;
public const byte BG_USB3_PHY_CONFIG_RXTERM_MASK = 0x30;
public static int bg_usb3_phy_config (
    int   beagle,
    byte  tx,
    byte  rx
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb3_phy_config(beagle, tx, rx);
}

public const byte BG_USB3_TRUNCATION_OFF = 0x00;
public const byte BG_USB3_TRUNCATION_20 = 0x01;
public const byte BG_USB3_TRUNCATION_36 = 0x02;
public const byte BG_USB3_TRUNCATION_68 = 0x03;
public static int bg_usb3_truncation_mode (
    int   beagle,
    byte  tx_truncation_mode,
    byte  rx_truncation_mode
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb3_truncation_mode(beagle, tx_truncation_mode, rx_truncation_mode);
}

/* Channel Configuration */
public const byte BG_USB3_EQUALIZATION_OFF = 0;
public const byte BG_USB3_EQUALIZATION_MIN = 1;
public const byte BG_USB3_EQUALIZATION_MOD = 2;
public const byte BG_USB3_EQUALIZATION_MAX = 3;
[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb3Channel {
    public byte input_equalization_short;
    public byte input_equalization_medium;
    public byte input_equalization_long;
    public byte pre_emphasis_short_level;
    public byte pre_emphasis_short_decay;
    public byte pre_emphasis_long_level;
    public byte pre_emphasis_long_decay;
    public byte output_level;
}

public static int bg_usb3_link_config (
    int                beagle,
    BeagleUsb3Channel  tx,
    BeagleUsb3Channel  rx
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb3_link_config(beagle, ref tx, ref rx);
}

/* Simple match configuration */
public const uint BG_USB3_SIMPLE_MATCH_NONE = 0x00000000;
public const uint BG_USB3_SIMPLE_MATCH_SSTX_IPS = 0x00000001;
public const uint BG_USB3_SIMPLE_MATCH_SSTX_SLC = 0x00000002;
public const uint BG_USB3_SIMPLE_MATCH_SSTX_SHP = 0x00000004;
public const uint BG_USB3_SIMPLE_MATCH_SSTX_SDP = 0x00000008;
public const uint BG_USB3_SIMPLE_MATCH_SSRX_IPS = 0x00000010;
public const uint BG_USB3_SIMPLE_MATCH_SSRX_SLC = 0x00000020;
public const uint BG_USB3_SIMPLE_MATCH_SSRX_SHP = 0x00000040;
public const uint BG_USB3_SIMPLE_MATCH_SSRX_SDP = 0x00000080;
public const uint BG_USB3_SIMPLE_MATCH_SSTX_SLC_CRC_5A_CRC_5B = 0x00000100;
public const uint BG_USB3_SIMPLE_MATCH_SSTX_SHP_CRC_5 = 0x00000200;
public const uint BG_USB3_SIMPLE_MATCH_SSTX_SHP_CRC_16 = 0x00000400;
public const uint BG_USB3_SIMPLE_MATCH_SSTX_SDP_CRC = 0x00000800;
public const uint BG_USB3_SIMPLE_MATCH_SSRX_SLC_CRC_5A_CRC_5B = 0x00001000;
public const uint BG_USB3_SIMPLE_MATCH_SSRX_SHP_CRC_5 = 0x00002000;
public const uint BG_USB3_SIMPLE_MATCH_SSRX_SHP_CRC_16 = 0x00004000;
public const uint BG_USB3_SIMPLE_MATCH_SSRX_SDP_CRC = 0x00008000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_SSTX_LFPS = 0x00010000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_SSTX_POLARITY = 0x00020000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_SSTX_DETECTED = 0x00400000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_SSTX_SCRAMBLE = 0x00080000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_SSRX_LFPS = 0x00100000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_SSRX_POLARITY = 0x00200000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_SSRX_DETECTED = 0x00040000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_SSRX_SCRAMBLE = 0x00800000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_VBUS_PRESENT = 0x08000000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_SSTX_PHYERR = 0x10000000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_SSRX_PHYERR = 0x20000000;
public const uint BG_USB3_SIMPLE_MATCH_EVENT_SMA_EXTIN = 0x40000000;
public const byte BG_USB_EDGE_RISING = 0x01;
public const byte BG_USB_EDGE_PULSE = 0x01;
public const byte BG_USB_EDGE_FALLING = 0x02;
public const byte BG_USB_EDGE_DEVICE_CHIRP = 0x01;
public const byte BG_USB_EDGE_HOST_CHIRP = 0x02;
// enum BeagleUsb3ExtoutMode  (from declaration above)
//     BG_USB3_EXTOUT_DISABLED     = 0
//     BG_USB3_EXTOUT_TRIGGER_MODE = 1
//     BG_USB3_EXTOUT_EVENTS_MODE  = 2

// enum BeagleUsb3IPSType  (from declaration above)
//     BG_USB3_IPS_TYPE_DISABLED = 0
//     BG_USB3_IPS_TYPE_TS1      = 1
//     BG_USB3_IPS_TYPE_TS2      = 2
//     BG_USB3_IPS_TYPE_TSEQ     = 3
//     BG_USB3_IPS_TYPE_TSx      = 4
//     BG_USB3_IPS_TYPE_TS_ANY   = 5

public static int bg_usb3_simple_match_config (
    int                   beagle,
    uint                  trigger_mask,
    uint                  extout_mask,
    BeagleUsb3ExtoutMode  extout_mode,
    byte                  extin_edge_mask,
    BeagleUsb3IPSType     tx_ips_type,
    BeagleUsb3IPSType     rx_ips_type
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb3_simple_match_config(beagle, trigger_mask, extout_mask, extout_mode, extin_edge_mask, tx_ips_type, rx_ips_type);
}

/* USB 3.0 Complex matching enable/disable */
public static int bg_usb3_complex_match_enable (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb3_complex_match_enable(beagle);
}

public static int bg_usb3_complex_match_disable (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb3_complex_match_disable(beagle);
}

// enum BeagleUsbSource  (from declaration above)
//     BG_USB_SOURCE_USB3_ASYNC = 0
//     BG_USB_SOURCE_USB3_RX    = 1
//     BG_USB_SOURCE_USB3_TX    = 2
//     BG_USB_SOURCE_USB2       = 3
//     BG_USB_SOURCE_IV_MON     = 4

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb3DataProperties {
    public BeagleUsbMatchType source_match_type;
    public BeagleUsbSource    source_match_val;
    public BeagleUsbMatchType ep_match_type;
    public byte               ep_match_val;
    public BeagleUsbMatchType dev_match_type;
    public byte               dev_match_val;
    public BeagleUsbMatchType stream_id_match_type;
    public ushort             stream_id_match_val;
    public BeagleUsbMatchType data_len_match_type;
    public ushort             data_len_match_val;
}

// enum BeagleUsb3PacketType  (from declaration above)
//     BG_USB3_MATCH_PACKET_SLC         = 0
//     BG_USB3_MATCH_PACKET_SHP         = 1
//     BG_USB3_MATCH_PACKET_SDP         = 2
//     BG_USB3_MATCH_PACKET_SHP_SDP     = 3
//     BG_USB3_MATCH_PACKET_TSx         = 4
//     BG_USB3_MATCH_PACKET_TSEQ        = 5
//     BG_USB3_MATCH_PACKET_ERROR       = 6
//     BG_USB3_MATCH_PACKET_5GBIT_START = 7
//     BG_USB3_MATCH_PACKET_5GBIT_STOP  = 8

// enum BeagleUsb3ErrorType  (from declaration above)
//     BG_USB3_MATCH_CRC_DONT_CARE    =    0
//     BG_USB3_MATCH_CRC_1_VALID      =    1
//     BG_USB3_MATCH_CRC_2_VALID      =    2
//     BG_USB3_MATCH_CRC_BOTH_VALID   =    3
//     BG_USB3_MATCH_CRC_EITHER_FAIL  =    4
//     BG_USB3_MATCH_CRC_1_FAIL       =    5
//     BG_USB3_MATCH_CRC_2_FAIL       =    6
//     BG_USB3_MATCH_CRC_BOTH_FAIL    =    7
//     BG_USB3_MATCH_ERR_MASK_CRC     = 0x10
//     BG_USB3_MATCH_ERR_MASK_FRAMING = 0x20
//     BG_USB3_MATCH_ERR_MASK_UNKNOWN = 0x40

// enum BeagleUsb3MatchModifier  (from declaration above)
//     BG_USB3_MATCH_MODIFIER_0 = 0
//     BG_USB3_MATCH_MODIFIER_1 = 1
//     BG_USB3_MATCH_MODIFIER_2 = 2
//     BG_USB3_MATCH_MODIFIER_3 = 3

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb3DataMatchUnit {
    public BeagleUsb3PacketType     packet_type;
    public byte[]                   data;
    public byte[]                   data_valid;
    public BeagleUsb3ErrorType      err_match;
    public byte                     data_properties_valid;
    public BeagleUsb3DataProperties data_properties;
    public BeagleUsb3MatchModifier  match_modifier;
    public ushort                   repeat_count;
    public byte                     sticky_action;
    public byte                     action_mask;
    public byte                     goto_selector;
}

[StructLayout(LayoutKind.Sequential)]
private struct c_BeagleUsb3DataMatchUnit {
    public BeagleUsb3PacketType       packet_type;
    public ushort                     data_length;
    public IntPtr                     data;
    public ushort                     data_valid_length;
    public IntPtr                     data_valid;
    public BeagleUsb3ErrorType        err_match;
    public byte                       data_properties_valid;
    public BeagleUsb3DataProperties   data_properties;
    public BeagleUsb3MatchModifier    match_modifier;
    public ushort                     repeat_count;
    public byte                       sticky_action;
    public byte                       action_mask;
    public byte                       goto_selector;
}

private static c_BeagleUsb3DataMatchUnit to_c_BeagleUsb3DataMatchUnit (
    ref c_BeagleUsb3DataMatchUnit c_s,
    ref BeagleUsb3DataMatchUnit s,
    GCContext gcc,
    bool is_in_arg
)
{
    /* Process arrays */
    if (s.data != null) {
        c_s.data_length = (ushort)s.data.Length;
        GCHandle data_gch = GCHandle.Alloc(s.data, GCHandleType.Pinned);
        c_s.data = data_gch.AddrOfPinnedObject();
        gcc.add(data_gch);
    }
    else {
        c_s.data_length = 0;
        c_s.data = IntPtr.Zero;
    }
    if (s.data_valid != null) {
        c_s.data_valid_length = (ushort)s.data_valid.Length;
        GCHandle data_valid_gch = GCHandle.Alloc(s.data_valid, GCHandleType.Pinned);
        c_s.data_valid = data_valid_gch.AddrOfPinnedObject();
        gcc.add(data_valid_gch);
    }
    else {
        c_s.data_valid_length = 0;
        c_s.data_valid = IntPtr.Zero;
    }

    /* Process simple arguments */
    if (is_in_arg) {
        c_s.packet_type = s.packet_type;
        c_s.err_match = s.err_match;
        c_s.data_properties_valid = s.data_properties_valid;
        c_s.match_modifier = s.match_modifier;
        c_s.repeat_count = s.repeat_count;
        c_s.sticky_action = s.sticky_action;
        c_s.action_mask = s.action_mask;
        c_s.goto_selector = s.goto_selector;
    }

    /* Process structures */
    c_s.data_properties = s.data_properties;

    return c_s;
}

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb3TimerMatchUnit {
    public BeagleUsbTimerUnit timer_unit;
    public uint               timer_val;
    public byte               action_mask;
    public byte               goto_selector;
}

// enum BeagleUsb3AsyncEventType  (from declaration above)
//     BG_USB3_COMPLEX_MATCH_EVENT_SSTX_LFPS     =  0
//     BG_USB3_COMPLEX_MATCH_EVENT_SSTX_POLARITY =  1
//     BG_USB3_COMPLEX_MATCH_EVENT_SSTX_DETECTED =  2
//     BG_USB3_COMPLEX_MATCH_EVENT_SSTX_SCRAMBLE =  3
//     BG_USB3_COMPLEX_MATCH_EVENT_SSRX_LFPS     =  4
//     BG_USB3_COMPLEX_MATCH_EVENT_SSRX_POLARITY =  5
//     BG_USB3_COMPLEX_MATCH_EVENT_SSRX_DETECTED =  6
//     BG_USB3_COMPLEX_MATCH_EVENT_SSRX_SCRAMBLE =  7
//     BG_USB3_COMPLEX_MATCH_EVENT_CROSS_TRIGGER =  8
//     BG_USB3_COMPLEX_MATCH_EVENT_VBUS_PRESENT  = 11
//     BG_USB3_COMPLEX_MATCH_EVENT_SSTX_PHYERR   = 12
//     BG_USB3_COMPLEX_MATCH_EVENT_SSRX_PHYERR   = 13
//     BG_USB3_COMPLEX_MATCH_EVENT_SMA_EXTIN     = 14

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb3AsyncEventMatchUnit {
    public BeagleUsb3AsyncEventType event_type;
    public byte                     edge_mask;
    public ushort                   repeat_count;
    public byte                     sticky_action;
    public byte                     action_mask;
    public byte                     goto_selector;
}

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb3ComplexMatchState {
    public byte                          tx_data_0_valid;
    public BeagleUsb3DataMatchUnit       tx_data_0;
    public byte                          tx_data_1_valid;
    public BeagleUsb3DataMatchUnit       tx_data_1;
    public byte                          tx_data_2_valid;
    public BeagleUsb3DataMatchUnit       tx_data_2;
    public byte                          rx_data_0_valid;
    public BeagleUsb3DataMatchUnit       rx_data_0;
    public byte                          rx_data_1_valid;
    public BeagleUsb3DataMatchUnit       rx_data_1;
    public byte                          rx_data_2_valid;
    public BeagleUsb3DataMatchUnit       rx_data_2;
    public byte                          timer_valid;
    public BeagleUsb3TimerMatchUnit      timer;
    public byte                          async_valid;
    public BeagleUsb3AsyncEventMatchUnit async;
    public byte                          goto_0;
    public byte                          goto_1;
    public byte                          goto_2;
}

[StructLayout(LayoutKind.Sequential)]
private struct c_BeagleUsb3ComplexMatchState {
    public byte                            tx_data_0_valid;
    public c_BeagleUsb3DataMatchUnit       tx_data_0;
    public byte                            tx_data_1_valid;
    public c_BeagleUsb3DataMatchUnit       tx_data_1;
    public byte                            tx_data_2_valid;
    public c_BeagleUsb3DataMatchUnit       tx_data_2;
    public byte                            rx_data_0_valid;
    public c_BeagleUsb3DataMatchUnit       rx_data_0;
    public byte                            rx_data_1_valid;
    public c_BeagleUsb3DataMatchUnit       rx_data_1;
    public byte                            rx_data_2_valid;
    public c_BeagleUsb3DataMatchUnit       rx_data_2;
    public byte                            timer_valid;
    public BeagleUsb3TimerMatchUnit        timer;
    public byte                            async_valid;
    public BeagleUsb3AsyncEventMatchUnit   async;
    public byte                            goto_0;
    public byte                            goto_1;
    public byte                            goto_2;
}

private static c_BeagleUsb3ComplexMatchState to_c_BeagleUsb3ComplexMatchState (
    ref c_BeagleUsb3ComplexMatchState c_s,
    ref BeagleUsb3ComplexMatchState s,
    GCContext gcc,
    bool is_in_arg
)
{
    /* Process simple arguments */
    if (is_in_arg) {
        c_s.tx_data_0_valid = s.tx_data_0_valid;
        c_s.tx_data_1_valid = s.tx_data_1_valid;
        c_s.tx_data_2_valid = s.tx_data_2_valid;
        c_s.rx_data_0_valid = s.rx_data_0_valid;
        c_s.rx_data_1_valid = s.rx_data_1_valid;
        c_s.rx_data_2_valid = s.rx_data_2_valid;
        c_s.timer_valid = s.timer_valid;
        c_s.async_valid = s.async_valid;
        c_s.goto_0 = s.goto_0;
        c_s.goto_1 = s.goto_1;
        c_s.goto_2 = s.goto_2;
    }

    /* Process structures */
    to_c_BeagleUsb3DataMatchUnit(ref c_s.tx_data_0, ref s.tx_data_0, gcc, is_in_arg);
    to_c_BeagleUsb3DataMatchUnit(ref c_s.tx_data_1, ref s.tx_data_1, gcc, is_in_arg);
    to_c_BeagleUsb3DataMatchUnit(ref c_s.tx_data_2, ref s.tx_data_2, gcc, is_in_arg);
    to_c_BeagleUsb3DataMatchUnit(ref c_s.rx_data_0, ref s.rx_data_0, gcc, is_in_arg);
    to_c_BeagleUsb3DataMatchUnit(ref c_s.rx_data_1, ref s.rx_data_1, gcc, is_in_arg);
    to_c_BeagleUsb3DataMatchUnit(ref c_s.rx_data_2, ref s.rx_data_2, gcc, is_in_arg);
    c_s.timer = s.timer;
    c_s.async = s.async;

    return c_s;
}

/* Complex matching configuration */
public static int bg_usb3_complex_match_config (
    int                          beagle,
    byte                         validate,
    byte                         extout,
    BeagleUsb3ComplexMatchState  state_0,
    BeagleUsb3ComplexMatchState  state_1,
    BeagleUsb3ComplexMatchState  state_2,
    BeagleUsb3ComplexMatchState  state_3,
    BeagleUsb3ComplexMatchState  state_4,
    BeagleUsb3ComplexMatchState  state_5,
    BeagleUsb3ComplexMatchState  state_6,
    BeagleUsb3ComplexMatchState  state_7
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    GCContext gcc = new GCContext();
    c_BeagleUsb3ComplexMatchState c_state_0 = new c_BeagleUsb3ComplexMatchState();
    to_c_BeagleUsb3ComplexMatchState(ref c_state_0, ref state_0, gcc, true);
    c_BeagleUsb3ComplexMatchState c_state_1 = new c_BeagleUsb3ComplexMatchState();
    to_c_BeagleUsb3ComplexMatchState(ref c_state_1, ref state_1, gcc, true);
    c_BeagleUsb3ComplexMatchState c_state_2 = new c_BeagleUsb3ComplexMatchState();
    to_c_BeagleUsb3ComplexMatchState(ref c_state_2, ref state_2, gcc, true);
    c_BeagleUsb3ComplexMatchState c_state_3 = new c_BeagleUsb3ComplexMatchState();
    to_c_BeagleUsb3ComplexMatchState(ref c_state_3, ref state_3, gcc, true);
    c_BeagleUsb3ComplexMatchState c_state_4 = new c_BeagleUsb3ComplexMatchState();
    to_c_BeagleUsb3ComplexMatchState(ref c_state_4, ref state_4, gcc, true);
    c_BeagleUsb3ComplexMatchState c_state_5 = new c_BeagleUsb3ComplexMatchState();
    to_c_BeagleUsb3ComplexMatchState(ref c_state_5, ref state_5, gcc, true);
    c_BeagleUsb3ComplexMatchState c_state_6 = new c_BeagleUsb3ComplexMatchState();
    to_c_BeagleUsb3ComplexMatchState(ref c_state_6, ref state_6, gcc, true);
    c_BeagleUsb3ComplexMatchState c_state_7 = new c_BeagleUsb3ComplexMatchState();
    to_c_BeagleUsb3ComplexMatchState(ref c_state_7, ref state_7, gcc, true);
    int ret = net_bg_usb3_complex_match_config(beagle, validate, extout, ref c_state_0, ref c_state_1, ref c_state_2, ref c_state_3, ref c_state_4, ref c_state_5, ref c_state_6, ref c_state_7);
    gcc.free();
    return ret;
}

/* Complex matching configuration for a single state */
public static int bg_usb3_complex_match_config_single (
    int                          beagle,
    byte                         validate,
    byte                         extout,
    BeagleUsb3ComplexMatchState  state
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    GCContext gcc = new GCContext();
    c_BeagleUsb3ComplexMatchState c_state = new c_BeagleUsb3ComplexMatchState();
    to_c_BeagleUsb3ComplexMatchState(ref c_state, ref state, gcc, true);
    int ret = net_bg_usb3_complex_match_config_single(beagle, validate, extout, ref c_state);
    gcc.free();
    return ret;
}

/* Extout configuration */
public static int bg_usb3_ext_io_config (
    int                  beagle,
    byte                 extin_enable,
    BeagleUsbExtoutType  extout_modulation
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb3_ext_io_config(beagle, extin_enable, extout_modulation);
}

// enum BeagleUsb3MemoryTestType  (from declaration above)
//     BG_USB3_MEMORY_TEST_FAST =  0
//     BG_USB3_MEMORY_TEST_FULL =  1
//     BG_USB3_MEMORY_TEST_SKIP = -1

public static int bg_usb3_memory_test (
    int                       beagle,
    BeagleUsb3MemoryTestType  test
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb3_memory_test(beagle, test);
}

/* Read functions */
public static int bg_usb2_read (
    int        beagle,
    ref uint   status,
    ref uint   events,
    ref ulong  time_sop,
    ref ulong  time_duration,
    ref uint   time_dataoffset,
    int        max_bytes,
    byte[]     packet
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int packet_max_bytes = (int)tp_min(max_bytes, packet.Length);
    return net_bg_usb2_read(beagle, ref status, ref events, ref time_sop, ref time_duration, ref time_dataoffset, packet_max_bytes, packet);
}

public static int bg_usb_read (
    int                  beagle,
    ref uint             status,
    ref uint             events,
    ref ulong            time_sop,
    ref ulong            time_duration,
    ref uint             time_dataoffset,
    ref BeagleUsbSource  source,
    int                  max_bytes,
    byte[]               packet,
    int                  max_k_bytes,
    byte[]               k_data
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int packet_max_bytes = (int)tp_min(max_bytes, packet.Length);
    int k_data_max_k_bytes = (int)tp_min(max_k_bytes, k_data.Length);
    return net_bg_usb_read(beagle, ref status, ref events, ref time_sop, ref time_duration, ref time_dataoffset, ref source, packet_max_bytes, packet, k_data_max_k_bytes, k_data);
}

/* | return / 8 */
public static int bg_usb2_read_data_timing (
    int        beagle,
    ref uint   status,
    ref uint   events,
    ref ulong  time_sop,
    ref ulong  time_duration,
    ref uint   time_dataoffset,
    int        max_bytes,
    byte[]     packet,
    int        max_timing,
    uint[]     data_timing
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int packet_max_bytes = (int)tp_min(max_bytes, packet.Length);
    int data_timing_max_timing = (int)tp_min(max_timing, data_timing.Length);
    return net_bg_usb2_read_data_timing(beagle, ref status, ref events, ref time_sop, ref time_duration, ref time_dataoffset, packet_max_bytes, packet, data_timing_max_timing, data_timing);
}

public static int bg_usb2_read_bit_timing (
    int        beagle,
    ref uint   status,
    ref uint   events,
    ref ulong  time_sop,
    ref ulong  time_duration,
    ref uint   time_dataoffset,
    int        max_bytes,
    byte[]     packet,
    int        max_timing,
    uint[]     bit_timing
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int packet_max_bytes = (int)tp_min(max_bytes, packet.Length);
    int bit_timing_max_timing = (int)tp_min(max_timing, bit_timing.Length);
    return net_bg_usb2_read_bit_timing(beagle, ref status, ref events, ref time_sop, ref time_duration, ref time_dataoffset, packet_max_bytes, packet, bit_timing_max_timing, bit_timing);
}

public static int bg_usb2_reconstruct_timing (
    uint    target_config,
    int     num_bytes,
    byte[]  packet,
    int     max_timing,
    uint[]  bit_timing
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int packet_num_bytes = (int)tp_min(num_bytes, packet.Length);
    int bit_timing_max_timing = (int)tp_min(max_timing, bit_timing.Length);
    return net_bg_usb2_reconstruct_timing(target_config, packet_num_bytes, packet, bit_timing_max_timing, bit_timing);
}

/* Hardware-based Statistics */
[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsbStatsConfig {
    public byte               auto_config;
    public BeagleUsbMatchType source_match_type;
    public BeagleUsbSource    source_match_val;
    public BeagleUsbMatchType ep_match_type;
    public byte               ep_match_val;
    public BeagleUsbMatchType dev_match_type;
    public byte               dev_match_val;
}

public static int bg_usb_stats_config (
    int                   beagle,
    BeagleUsbStatsConfig  config
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb_stats_config(beagle, ref config);
}

public static int bg_usb_stats_config_query (
    int                       beagle,
    ref BeagleUsbStatsConfig  config
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb_stats_config_query(beagle, ref config);
}

public static int bg_usb_stats_reset (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb_stats_reset(beagle);
}

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb3GenStats {
    public ulong link;
    public ulong lbad;
    public ulong slc_crc5;
    public ulong txn;
    public ulong lmp;
    public ulong lgo_u1;
    public ulong lgo_u2;
    public ulong lgo_u3;
    public ulong dp;
    public ulong itp;
    public ulong shp_crc16_crc5;
    public ulong sdp_crc32;
    public ulong slc_frm_err;
    public ulong shp_frm_err;
    public ulong sdp_end_edb_frm_err;
    public ulong iso_ips;
    public ulong para_ips;
    public ulong carry_1k_dp;
}

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb3ConnStats {
    public ulong txn;
    public ulong dp;
    public ulong ack;
    public ulong nrdy;
    public ulong erdy;
    public ulong retry_ack;
    public ulong carry_1k_dp;
}

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsb2Stats {
    public ulong sof;
    public ulong carry_1k_data;
    public ulong data;
    public ulong bad_pid;
    public ulong crc16;
    public ulong crc5;
    public ulong rx_error;
    public ulong in_nak;
    public ulong ping_nak;
}

[StructLayout(LayoutKind.Sequential)]
public struct BeagleUsbStats {
    public BeagleUsb3GenStats  usb3_tx_gen;
    public BeagleUsb3GenStats  usb3_rx_gen;
    public BeagleUsb3ConnStats usb3_tx_conn;
    public BeagleUsb3ConnStats usb3_rx_conn;
    public BeagleUsb2Stats     usb2;
}

public static int bg_usb_stats_read (
    int                 beagle,
    ref BeagleUsbStats  stats
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb_stats_read(beagle, ref stats);
}

public static int bg_usb2_stats_read (
    int                  beagle,
    ref BeagleUsb2Stats  stats
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_usb2_stats_read(beagle, ref stats);
}


/*=========================================================================
| USB 480 API
 ========================================================================*/
/* General constants */
public const byte BG480_TRUNCATION_LENGTH = 4;
public const uint BG480V2_USB2_BUFFER_SIZE_256MB = 256;

/*=========================================================================
| USB 5000 API
 ========================================================================*/
/* General constants */
public const uint BG5000_USB2_BUFFER_SIZE_128MB = 128;
public const uint BG5000_USB3_BUFFER_SIZE_2GB = 2;
public const uint BG5000_USB3_BUFFER_SIZE_4GB = 4;
/* Cross-Analyzer Sync Configuration */
// enum Beagle5000CrossAnalyzerSyncMode  (from declaration above)
//     BG5000_CROSS_ANALYZER_SYNC_WAIT   = 0
//     BG5000_CROSS_ANALYZER_SYNC_BYPASS = 1

// enum Beagle5000CrossAnalyzerMode  (from declaration above)
//     BG5000_CROSS_ANALYZER_ACCEPT = 0
//     BG5000_CROSS_ANALYZER_IGNORE = 1

public static int bg5000_cross_analyzer_sync_config (
    int                              beagle,
    Beagle5000CrossAnalyzerSyncMode  cross_sync_mode,
    Beagle5000CrossAnalyzerMode      cross_trigger_mode,
    Beagle5000CrossAnalyzerMode      cross_stop_mode
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg5000_cross_analyzer_sync_config(beagle, cross_sync_mode, cross_trigger_mode, cross_stop_mode);
}

public static int bg5000_cross_analyzer_sync_release (
    int  beagle
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg5000_cross_analyzer_sync_release(beagle);
}


/*=========================================================================
| MDIO API
 ========================================================================*/
// enum BeagleMdioClause  (from declaration above)
//     BG_MDIO_CLAUSE_22    = 0
//     BG_MDIO_CLAUSE_45    = 1
//     BG_MDIO_CLAUSE_ERROR = 2

public const byte BG_MDIO_OPCODE22_WRITE = 0x01;
public const byte BG_MDIO_OPCODE22_READ = 0x02;
public const byte BG_MDIO_OPCODE22_ERROR = 0xff;
public const byte BG_MDIO_OPCODE45_ADDR = 0x00;
public const byte BG_MDIO_OPCODE45_WRITE = 0x01;
public const byte BG_MDIO_OPCODE45_READ_POSTINC = 0x02;
public const byte BG_MDIO_OPCODE45_READ = 0x03;
/* Read the next MDIO frame. */
public static int bg_mdio_read (
    int        beagle,
    ref uint   status,
    ref ulong  time_sop,
    ref ulong  time_duration,
    ref uint   time_dataoffset,
    ref uint   data_in
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_mdio_read(beagle, ref status, ref time_sop, ref time_duration, ref time_dataoffset, ref data_in);
}

public static int bg_mdio_read_bit_timing (
    int        beagle,
    ref uint   status,
    ref ulong  time_sop,
    ref ulong  time_duration,
    ref uint   time_dataoffset,
    ref uint   data_in,
    int        max_timing,
    uint[]     bit_timing
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int bit_timing_max_timing = (int)tp_min(max_timing, bit_timing.Length);
    return net_bg_mdio_read_bit_timing(beagle, ref status, ref time_sop, ref time_duration, ref time_dataoffset, ref data_in, bit_timing_max_timing, bit_timing);
}

/*
 * Parse the raw MDIO data into the standard format.
 * This function will fill the supplied fields as per
 * the constants defined above.  If the raw data contains
 * a malformed turnaround field, the caller will be
 * notified of the error through the return value of
 * this function (BG_MDIO_BAD_TURNAROUND).
 */
public static int bg_mdio_parse (
    uint        packet,
    ref byte    clause,
    ref byte    opcode,
    ref byte    addr1,
    ref byte    addr2,
    ref ushort  data
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    return net_bg_mdio_parse(packet, ref clause, ref opcode, ref addr1, ref addr2, ref data);
}


/*=========================================================================
| IV MON API
 ==========================================================================
| Extract the current and voltage values in the packet returned by
| bg_usb_read(*/
public static int bg_iv_mon_parse (
    int        length,
    byte[]     packet,
    ref float  voltage,
    ref float  current
)
{
    if (!BG_LIBRARY_LOADED) return (int)BeagleStatus.BG_INCOMPATIBLE_LIBRARY;
    int packet_length = (int)tp_min(length, packet.Length);
    return net_bg_iv_mon_parse(packet_length, packet, ref voltage, ref current);
}


/*=========================================================================
| NATIVE DLL BINDINGS
 ========================================================================*/
[DllImport ("beagle")]
private static extern int net_bg_find_devices (int num_devices, [Out] ushort[] devices);

[DllImport ("beagle")]
private static extern int net_bg_find_devices_ext (int num_devices, [Out] ushort[] devices, int num_ids, [Out] uint[] unique_ids);

[DllImport ("beagle")]
private static extern int net_bg_open (int port_number);

[DllImport ("beagle")]
private static extern int net_bg_open_ext (int port_number, ref BeagleExt bg_ext);

[DllImport ("beagle")]
private static extern int net_bg_close (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_port (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_features (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_unique_id_to_features (uint unique_id);

[DllImport ("beagle")]
private static extern uint net_bg_unique_id (int beagle);

[DllImport ("beagle")]
private static extern IntPtr net_bg_status_string (int status);

[DllImport ("beagle")]
private static extern int net_bg_version (int beagle, ref BeagleVersion version);

[DllImport ("beagle")]
private static extern int net_bg_latency (int beagle, uint milliseconds);

[DllImport ("beagle")]
private static extern int net_bg_timeout (int beagle, uint milliseconds);

[DllImport ("beagle")]
private static extern uint net_bg_sleep_ms (uint milliseconds);

[DllImport ("beagle")]
private static extern int net_bg_target_power (int beagle, byte power_flag);

[DllImport ("beagle")]
private static extern int net_bg_host_ifce_speed (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_dev_addr (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_host_buffer_size (int beagle, uint num_bytes);

[DllImport ("beagle")]
private static extern int net_bg_host_buffer_free (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_host_buffer_used (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_commtest (int beagle, int num_samples, int delay_count);

[DllImport ("beagle")]
private static extern int net_bg_enable (int beagle, BeagleProtocol protocol);

[DllImport ("beagle")]
private static extern int net_bg_disable (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_capture_stop (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_capture_trigger (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_capture_trigger_wait (int beagle, uint timeout_ms, ref BeagleCaptureStatus status);

[DllImport ("beagle")]
private static extern int net_bg_samplerate (int beagle, int samplerate_khz);

[DllImport ("beagle")]
private static extern int net_bg_bit_timing_size (BeagleProtocol protocol, int num_data_bytes);

[DllImport ("beagle")]
private static extern int net_bg_i2c_pullup (int beagle, byte pullup_flag);

[DllImport ("beagle")]
private static extern int net_bg_i2c_read (int beagle, ref uint status, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, int max_bytes, [Out] ushort[] data_in);

[DllImport ("beagle")]
private static extern int net_bg_i2c_read_data_timing (int beagle, ref uint status, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, int max_bytes, [Out] ushort[] data_in, int max_timing, [Out] uint[] data_timing);

[DllImport ("beagle")]
private static extern int net_bg_i2c_read_bit_timing (int beagle, ref uint status, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, int max_bytes, [Out] ushort[] data_in, int max_timing, [Out] uint[] bit_timing);

[DllImport ("beagle")]
private static extern int net_bg_spi_configure (int beagle, BeagleSpiSSPolarity ss_polarity, BeagleSpiSckSamplingEdge sck_sampling_edge, BeagleSpiBitorder bitorder);

[DllImport ("beagle")]
private static extern int net_bg_spi_read (int beagle, ref uint status, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, int mosi_max_bytes, [Out] byte[] data_mosi, int miso_max_bytes, [Out] byte[] data_miso);

[DllImport ("beagle")]
private static extern int net_bg_spi_read_data_timing (int beagle, ref uint status, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, int mosi_max_bytes, [Out] byte[] data_mosi, int miso_max_bytes, [Out] byte[] data_miso, int max_timing, [Out] uint[] data_timing);

[DllImport ("beagle")]
private static extern int net_bg_spi_read_bit_timing (int beagle, ref uint status, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, int mosi_max_bytes, [Out] byte[] data_mosi, int miso_max_bytes, [Out] byte[] data_miso, int max_timing, [Out] uint[] bit_timing);

[DllImport ("beagle")]
private static extern int net_bg_usb_features (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_usb_license_read (int beagle, int length, [Out] byte[] license_key);

[DllImport ("beagle")]
private static extern int net_bg_usb_license_write (int beagle, int length, [In] byte[] license_key);

[DllImport ("beagle")]
private static extern int net_bg_usb_configure (int beagle, byte cap_mask, BeagleUsbTriggerMode trigger_mode);

[DllImport ("beagle")]
private static extern int net_bg_usb_target_power (int beagle, BeagleUsbTargetPower power_flag);

[DllImport ("beagle")]
private static extern int net_bg_usb2_capture_config (int beagle, BeagleUsb2CaptureMode capture_mode);

[DllImport ("beagle")]
private static extern int net_bg_usb2_target_config (int beagle, uint target_config);

[DllImport ("beagle")]
private static extern int net_bg_usb2_capture_buffer_config (int beagle, uint pretrig_kb, uint capture_kb);

[DllImport ("beagle")]
private static extern int net_bg_usb2_capture_buffer_config_query (int beagle, ref uint pretrig_kb, ref uint capture_kb);

[DllImport ("beagle")]
private static extern int net_bg_usb2_capture_status (int beagle, ref BeagleCaptureStatus status, ref uint pretrig_remaining_kb, ref uint pretrig_total_kb, ref uint capture_remaining_kb, ref uint capture_total_kb);

[DllImport ("beagle")]
private static extern int net_bg_usb2_digital_out_config (int beagle, byte out_enable_mask, byte out_polarity_mask);

[DllImport ("beagle")]
private static extern int net_bg_usb2_digital_out_match (int beagle, BeagleUsb2DigitalOutMatchPins pin_num, ref BeagleUsb2PacketMatch packet_match, ref c_BeagleUsb2DataMatch data_match);

[DllImport ("beagle")]
private static extern int net_bg_usb2_digital_in_config (int beagle, byte in_enable_mask);

[DllImport ("beagle")]
private static extern int net_bg_usb2_hw_filter_config (int beagle, byte filter_enable_mask);

[DllImport ("beagle")]
private static extern int net_bg_usb2_simple_match_config (int beagle, byte dig_in_pin_pos_edge_mask, byte dig_in_pin_neg_edge_mask, byte dig_out_match_pin_mask);

[DllImport ("beagle")]
private static extern int net_bg_usb2_complex_match_enable (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_usb2_complex_match_disable (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_usb2_complex_match_config (int beagle, byte validate, byte digout, ref c_BeagleUsb2ComplexMatchState state_0, ref c_BeagleUsb2ComplexMatchState state_1, ref c_BeagleUsb2ComplexMatchState state_2, ref c_BeagleUsb2ComplexMatchState state_3, ref c_BeagleUsb2ComplexMatchState state_4, ref c_BeagleUsb2ComplexMatchState state_5, ref c_BeagleUsb2ComplexMatchState state_6, ref c_BeagleUsb2ComplexMatchState state_7);

[DllImport ("beagle")]
private static extern int net_bg_usb2_complex_match_config_single (int beagle, byte validate, byte digout, ref c_BeagleUsb2ComplexMatchState state);

[DllImport ("beagle")]
private static extern int net_bg_usb2_extout_config (int beagle, BeagleUsbExtoutType extout_modulation);

[DllImport ("beagle")]
private static extern int net_bg_usb2_memory_test (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_usb3_capture_buffer_config (int beagle, uint pretrig_kb, uint capture_kb);

[DllImport ("beagle")]
private static extern int net_bg_usb3_capture_buffer_config_query (int beagle, ref uint pretrig_kb, ref uint capture_kb);

[DllImport ("beagle")]
private static extern int net_bg_usb3_capture_status (int beagle, ref BeagleCaptureStatus status, ref uint pretrig_remaining_kb, ref uint pretrig_total_kb, ref uint capture_remaining_kb, ref uint capture_total_kb);

[DllImport ("beagle")]
private static extern int net_bg_usb3_phy_config (int beagle, byte tx, byte rx);

[DllImport ("beagle")]
private static extern int net_bg_usb3_truncation_mode (int beagle, byte tx_truncation_mode, byte rx_truncation_mode);

[DllImport ("beagle")]
private static extern int net_bg_usb3_link_config (int beagle, ref BeagleUsb3Channel tx, ref BeagleUsb3Channel rx);

[DllImport ("beagle")]
private static extern int net_bg_usb3_simple_match_config (int beagle, uint trigger_mask, uint extout_mask, BeagleUsb3ExtoutMode extout_mode, byte extin_edge_mask, BeagleUsb3IPSType tx_ips_type, BeagleUsb3IPSType rx_ips_type);

[DllImport ("beagle")]
private static extern int net_bg_usb3_complex_match_enable (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_usb3_complex_match_disable (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_usb3_complex_match_config (int beagle, byte validate, byte extout, ref c_BeagleUsb3ComplexMatchState state_0, ref c_BeagleUsb3ComplexMatchState state_1, ref c_BeagleUsb3ComplexMatchState state_2, ref c_BeagleUsb3ComplexMatchState state_3, ref c_BeagleUsb3ComplexMatchState state_4, ref c_BeagleUsb3ComplexMatchState state_5, ref c_BeagleUsb3ComplexMatchState state_6, ref c_BeagleUsb3ComplexMatchState state_7);

[DllImport ("beagle")]
private static extern int net_bg_usb3_complex_match_config_single (int beagle, byte validate, byte extout, ref c_BeagleUsb3ComplexMatchState state);

[DllImport ("beagle")]
private static extern int net_bg_usb3_ext_io_config (int beagle, byte extin_enable, BeagleUsbExtoutType extout_modulation);

[DllImport ("beagle")]
private static extern int net_bg_usb3_memory_test (int beagle, BeagleUsb3MemoryTestType test);

[DllImport ("beagle")]
private static extern int net_bg_usb2_read (int beagle, ref uint status, ref uint events, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, int max_bytes, [Out] byte[] packet);

[DllImport ("beagle")]
private static extern int net_bg_usb_read (int beagle, ref uint status, ref uint events, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, ref BeagleUsbSource source, int max_bytes, [Out] byte[] packet, int max_k_bytes, [Out] byte[] k_data);

[DllImport ("beagle")]
private static extern int net_bg_usb2_read_data_timing (int beagle, ref uint status, ref uint events, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, int max_bytes, [Out] byte[] packet, int max_timing, [Out] uint[] data_timing);

[DllImport ("beagle")]
private static extern int net_bg_usb2_read_bit_timing (int beagle, ref uint status, ref uint events, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, int max_bytes, [Out] byte[] packet, int max_timing, [Out] uint[] bit_timing);

[DllImport ("beagle")]
private static extern int net_bg_usb2_reconstruct_timing (uint target_config, int num_bytes, [In] byte[] packet, int max_timing, [Out] uint[] bit_timing);

[DllImport ("beagle")]
private static extern int net_bg_usb_stats_config (int beagle, ref BeagleUsbStatsConfig config);

[DllImport ("beagle")]
private static extern int net_bg_usb_stats_config_query (int beagle, ref BeagleUsbStatsConfig config);

[DllImport ("beagle")]
private static extern int net_bg_usb_stats_reset (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_usb_stats_read (int beagle, ref BeagleUsbStats stats);

[DllImport ("beagle")]
private static extern int net_bg_usb2_stats_read (int beagle, ref BeagleUsb2Stats stats);

[DllImport ("beagle")]
private static extern int net_bg5000_cross_analyzer_sync_config (int beagle, Beagle5000CrossAnalyzerSyncMode cross_sync_mode, Beagle5000CrossAnalyzerMode cross_trigger_mode, Beagle5000CrossAnalyzerMode cross_stop_mode);

[DllImport ("beagle")]
private static extern int net_bg5000_cross_analyzer_sync_release (int beagle);

[DllImport ("beagle")]
private static extern int net_bg_mdio_read (int beagle, ref uint status, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, ref uint data_in);

[DllImport ("beagle")]
private static extern int net_bg_mdio_read_bit_timing (int beagle, ref uint status, ref ulong time_sop, ref ulong time_duration, ref uint time_dataoffset, ref uint data_in, int max_timing, [Out] uint[] bit_timing);

[DllImport ("beagle")]
private static extern int net_bg_mdio_parse (uint packet, ref byte clause, ref byte opcode, ref byte addr1, ref byte addr2, ref ushort data);

[DllImport ("beagle")]
private static extern int net_bg_iv_mon_parse (int length, [In] byte[] packet, ref float voltage, ref float current);


} // class BeagleApi

} // namespace TotalPhase
