/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDflagtype.h                                                                                                                */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Messages flag types declaration                                                                                                 */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDflagtype_h
#define IOCARDflagtype_h

#include "typelib.h"

#ifdef __cplusplus
extern "C" {
#endif

/* HDR_ONLY_MESSAGE_LCU2IO */
/*  */
typedef struct {
	AdiUInt8                                        HDR_LCU2IO; // 
} PHS_HDRONLYMESSAGELCU2IO_FLAG;

/* KEEP_ALIVE_MESSAGE */
/*  */
typedef struct {
	AdiUInt8                                        HDR_LCU2IO; // 
	AdiUInt8                                        TL_LCU2IO; // Tail of each Tx message. This is mainly now used for CRC calculations
} PHS_KEEPALIVEMESSAGE_FLAG;

/* SERIAL_LCU2IO_MESSAGE */
/*  */
typedef struct {
	AdiUInt8                                        HDR_LCU2IO; // 
	AdiUInt8                                        DT_SERIAL_LCU2IO; // 
	AdiUInt8                                        TL_LCU2IO; // Tail of each Tx message. This is mainly now used for CRC calculations
} PHS_SERIALLCU2IOMESSAGE_FLAG;

/* SET_DISCRETES_MESSAGE */
/*  */
typedef struct {
	AdiUInt8                                        HDR_LCU2IO; // 
	AdiUInt8                                        DT_SET_DISC; // 
	AdiUInt8                                        TL_LCU2IO; // Tail of each Tx message. This is mainly now used for CRC calculations
} PHS_SETDISCRETESMESSAGE_FLAG;

/* SET_SYSTEM_STATE_MESSAGE */
/*  */
typedef struct {
	AdiUInt8                                        HDR_LCU2IO; // 
	AdiUInt8                                        DT_SET_SYS_STATE; // 
	AdiUInt8                                        TL_LCU2IO; // Tail of each Tx message. This is mainly now used for CRC calculations
} PHS_SETSYSTEMSTATEMESSAGE_FLAG;

/* START_INTERFACE_MESSAGE */
/*  */
typedef struct {
	AdiUInt8                                        HDR_LCU2IO; // 
	AdiUInt8                                        DT_START_INTERFACE; // 
	AdiUInt8                                        TL_LCU2IO; // Tail of each Tx message. This is mainly now used for CRC calculations
} PHS_STARTINTERFACEMESSAGE_FLAG;

/* STOP_INTERFACE_MESSAGE */
/*  */
typedef struct {
	AdiUInt8                                        HDR_LCU2IO; // 
	AdiUInt8                                        DT_STOP_INTERFACE; // 
	AdiUInt8                                        TL_LCU2IO; // Tail of each Tx message. This is mainly now used for CRC calculations
} PHS_STOPINTERFACEMESSAGE_FLAG;

/* TL_ONLY_MESSAGE_LCU2IO */
/*  */
typedef struct {
	AdiUInt8                                        TL_LCU2IO; // Tail of each Tx message. This is mainly now used for CRC calculations
} PHS_TLONLYMESSAGELCU2IO_FLAG;

/* HDR_ONLY_MESSAGE_IO2LCU */
/*  */
typedef struct {
	AdiUInt8                                        HDR_IO2LCU; // header of every message that is received by IO Unit
} PHS_HDRONLYMESSAGEIO2LCU_FLAG;

/* LOG_DATA_MESSAGE */
/*  */
typedef struct {
	AdiUInt8                                        HDR_IO2LCU; // header of every message that is received by IO Unit
	AdiUInt8                                        DT_LOG; // 
	AdiUInt8                                        TL_IO2LCU; // Tail of each Rx message. This is mainly now used for CRC calculations
} PHS_LOGDATAMESSAGE_FLAG;

/* NONCE_MESSAGE */
/*  */
typedef struct {
	AdiUInt8                                        HDR_IO2LCU; // header of every message that is received by IO Unit
	AdiUInt8                                        DT_NONCE; // 
	AdiUInt8                                        TL_IO2LCU; // Tail of each Rx message. This is mainly now used for CRC calculations
} PHS_NONCEMESSAGE_FLAG;

/* REQUEST_STATUS_MESSAGE */
/*  */
typedef struct {
	AdiUInt8                                        HDR_IO2LCU; // header of every message that is received by IO Unit
	AdiUInt8                                        DT_REQUEST_STATUS; // 
	AdiUInt8                                        TL_IO2LCU; // Tail of each Rx message. This is mainly now used for CRC calculations
} PHS_REQUESTSTATUSMESSAGE_FLAG;

/* SERIAL_IO2LCU_MESSAGE */
/*  */
typedef struct {
	AdiUInt8                                        HDR_IO2LCU; // header of every message that is received by IO Unit
	AdiUInt8                                        DT_SERIAL_IO2LCU; // 
	AdiUInt8                                        TL_IO2LCU; // Tail of each Rx message. This is mainly now used for CRC calculations
} PHS_SERIALIO2LCUMESSAGE_FLAG;

/* STATUS_MESSAGE */
/*  */
typedef struct {
	AdiUInt8                                        HDR_IO2LCU; // header of every message that is received by IO Unit
	AdiUInt8                                        DT_MONITORED_DATA; // 
	AdiUInt8                                        TL_IO2LCU; // Tail of each Rx message. This is mainly now used for CRC calculations
} PHS_STATUSMESSAGE_FLAG;

/* TL_ONLY_MESSAGE_IO2LCU */
/*  */
typedef struct {
	AdiUInt8                                        TL_IO2LCU; // Tail of each Rx message. This is mainly now used for CRC calculations
} PHS_TLONLYMESSAGEIO2LCU_FLAG;



#ifdef __cplusplus
}
#endif

#endif 
