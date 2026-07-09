/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDtype.h                                                                                                                    */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Messages types declaration                                                                                                      */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDtype_h
#define IOCARDtype_h

#include "IOCARDcomplex.h"
#include "typelib.h"

#ifdef __cplusplus
extern "C" {
#endif

#ifndef _INTERFACE_TYPE_HDR_ONLY_MESSAGE_IO2LCU
#define _INTERFACE_TYPE_HDR_ONLY_MESSAGE_IO2LCU

/* Interface data structure type definition */
typedef struct PHS_HDRONLYMESSAGEIO2LCU
{
	/* header of every message that is received by IO Unit */
	HDR_IO2LCU_STRUCT HDR_IO2LCU;
}
PHS_HDRONLYMESSAGEIO2LCU;

#endif /* _INTERFACE_TYPE_HDR_ONLY_MESSAGE_IO2LCU */

#ifndef _INTERFACE_TYPE_HDR_ONLY_MESSAGE_LCU2IO
#define _INTERFACE_TYPE_HDR_ONLY_MESSAGE_LCU2IO

/* Interface data structure type definition */
typedef struct PHS_HDRONLYMESSAGELCU2IO
{
	HDR_LCU2IO_STRUCT HDR_LCU2IO;
}
PHS_HDRONLYMESSAGELCU2IO;

#endif /* _INTERFACE_TYPE_HDR_ONLY_MESSAGE_LCU2IO */

#ifndef _INTERFACE_TYPE_KEEP_ALIVE_MESSAGE
#define _INTERFACE_TYPE_KEEP_ALIVE_MESSAGE

/* Interface data structure type definition */
typedef struct PHS_KEEPALIVEMESSAGE
{
	HDR_LCU2IO_STRUCT HDR_LCU2IO;
	/* Tail of each Tx message. This is mainly now used for CRC calculations */
	TL_LCU2IO_STRUCT TL_LCU2IO;
}
PHS_KEEPALIVEMESSAGE;

#endif /* _INTERFACE_TYPE_KEEP_ALIVE_MESSAGE */

#ifndef _INTERFACE_TYPE_LOG_DATA_MESSAGE
#define _INTERFACE_TYPE_LOG_DATA_MESSAGE

/* Interface data structure type definition */
typedef struct PHS_LOGDATAMESSAGE
{
	/* header of every message that is received by IO Unit */
	HDR_IO2LCU_STRUCT HDR_IO2LCU;
	DT_LOG_STRUCT DT_LOG;
	/* Tail of each Rx message. This is mainly now used for CRC calculations */
	TL_IO2LCU_STRUCT TL_IO2LCU;
}
PHS_LOGDATAMESSAGE;

#endif /* _INTERFACE_TYPE_LOG_DATA_MESSAGE */

#ifndef _INTERFACE_TYPE_NONCE_MESSAGE
#define _INTERFACE_TYPE_NONCE_MESSAGE

/* Interface data structure type definition */
typedef struct PHS_NONCEMESSAGE
{
	/* header of every message that is received by IO Unit */
	HDR_IO2LCU_STRUCT HDR_IO2LCU;
	DT_NONCE_STRUCT DT_NONCE;
	/* Tail of each Rx message. This is mainly now used for CRC calculations */
	TL_IO2LCU_STRUCT TL_IO2LCU;
}
PHS_NONCEMESSAGE;

#endif /* _INTERFACE_TYPE_NONCE_MESSAGE */

#ifndef _INTERFACE_TYPE_REQUEST_STATUS_MESSAGE
#define _INTERFACE_TYPE_REQUEST_STATUS_MESSAGE

/* Interface data structure type definition */
typedef struct PHS_REQUESTSTATUSMESSAGE
{
	/* header of every message that is received by IO Unit */
	HDR_IO2LCU_STRUCT HDR_IO2LCU;
	DT_REQUEST_STATUS_STRUCT DT_REQUEST_STATUS;
	/* Tail of each Rx message. This is mainly now used for CRC calculations */
	TL_IO2LCU_STRUCT TL_IO2LCU;
}
PHS_REQUESTSTATUSMESSAGE;

#endif /* _INTERFACE_TYPE_REQUEST_STATUS_MESSAGE */

#ifndef _INTERFACE_TYPE_SERIAL_IO2LCU_MESSAGE
#define _INTERFACE_TYPE_SERIAL_IO2LCU_MESSAGE

/* Interface data structure type definition */
typedef struct PHS_SERIALIO2LCUMESSAGE
{
	/* header of every message that is received by IO Unit */
	HDR_IO2LCU_STRUCT HDR_IO2LCU;
	DT_SERIAL_IO2LCU_STRUCT DT_SERIAL_IO2LCU;
	/* Tail of each Rx message. This is mainly now used for CRC calculations */
	TL_IO2LCU_STRUCT TL_IO2LCU;
}
PHS_SERIALIO2LCUMESSAGE;

#endif /* _INTERFACE_TYPE_SERIAL_IO2LCU_MESSAGE */

#ifndef _INTERFACE_TYPE_SERIAL_LCU2IO_MESSAGE
#define _INTERFACE_TYPE_SERIAL_LCU2IO_MESSAGE

/* Interface data structure type definition */
typedef struct PHS_SERIALLCU2IOMESSAGE
{
	HDR_LCU2IO_STRUCT HDR_LCU2IO;
	DT_SERIAL_LCU2IO_STRUCT DT_SERIAL_LCU2IO;
	/* Tail of each Tx message. This is mainly now used for CRC calculations */
	TL_LCU2IO_STRUCT TL_LCU2IO;
}
PHS_SERIALLCU2IOMESSAGE;

#endif /* _INTERFACE_TYPE_SERIAL_LCU2IO_MESSAGE */

#ifndef _INTERFACE_TYPE_SET_DISCRETES_MESSAGE
#define _INTERFACE_TYPE_SET_DISCRETES_MESSAGE

/* Interface data structure type definition */
typedef struct PHS_SETDISCRETESMESSAGE
{
	HDR_LCU2IO_STRUCT HDR_LCU2IO;
	DT_SET_DISC_STRUCT DT_SET_DISC;
	/* Tail of each Tx message. This is mainly now used for CRC calculations */
	TL_LCU2IO_STRUCT TL_LCU2IO;
}
PHS_SETDISCRETESMESSAGE;

#endif /* _INTERFACE_TYPE_SET_DISCRETES_MESSAGE */

#ifndef _INTERFACE_TYPE_SET_SYSTEM_STATE_MESSAGE
#define _INTERFACE_TYPE_SET_SYSTEM_STATE_MESSAGE

/* Interface data structure type definition */
typedef struct PHS_SETSYSTEMSTATEMESSAGE
{
	HDR_LCU2IO_STRUCT HDR_LCU2IO;
	DT_SET_SYS_STATE_STRUCT DT_SET_SYS_STATE;
	/* Tail of each Tx message. This is mainly now used for CRC calculations */
	TL_LCU2IO_STRUCT TL_LCU2IO;
}
PHS_SETSYSTEMSTATEMESSAGE;

#endif /* _INTERFACE_TYPE_SET_SYSTEM_STATE_MESSAGE */

#ifndef _INTERFACE_TYPE_START_INTERFACE_MESSAGE
#define _INTERFACE_TYPE_START_INTERFACE_MESSAGE

/* Interface data structure type definition */
typedef struct PHS_STARTINTERFACEMESSAGE
{
	HDR_LCU2IO_STRUCT HDR_LCU2IO;
	DT_START_INTERFACE_STRUCT DT_START_INTERFACE;
	/* Tail of each Tx message. This is mainly now used for CRC calculations */
	TL_LCU2IO_STRUCT TL_LCU2IO;
}
PHS_STARTINTERFACEMESSAGE;

#endif /* _INTERFACE_TYPE_START_INTERFACE_MESSAGE */

#ifndef _INTERFACE_TYPE_STATUS_MESSAGE
#define _INTERFACE_TYPE_STATUS_MESSAGE

/* Interface data structure type definition */
typedef struct PHS_STATUSMESSAGE
{
	/* header of every message that is received by IO Unit */
	HDR_IO2LCU_STRUCT HDR_IO2LCU;
	DT_MONITORED_DATA_STRUCT DT_MONITORED_DATA;
	/* Tail of each Rx message. This is mainly now used for CRC calculations */
	TL_IO2LCU_STRUCT TL_IO2LCU;
}
PHS_STATUSMESSAGE;

#endif /* _INTERFACE_TYPE_STATUS_MESSAGE */

#ifndef _INTERFACE_TYPE_STOP_INTERFACE_MESSAGE
#define _INTERFACE_TYPE_STOP_INTERFACE_MESSAGE

/* Interface data structure type definition */
typedef struct PHS_STOPINTERFACEMESSAGE
{
	HDR_LCU2IO_STRUCT HDR_LCU2IO;
	DT_STOP_INTERFACE_STRUCT DT_STOP_INTERFACE;
	/* Tail of each Tx message. This is mainly now used for CRC calculations */
	TL_LCU2IO_STRUCT TL_LCU2IO;
}
PHS_STOPINTERFACEMESSAGE;

#endif /* _INTERFACE_TYPE_STOP_INTERFACE_MESSAGE */

#ifndef _INTERFACE_TYPE_TL_ONLY_MESSAGE_IO2LCU
#define _INTERFACE_TYPE_TL_ONLY_MESSAGE_IO2LCU

/* Interface data structure type definition */
typedef struct PHS_TLONLYMESSAGEIO2LCU
{
	/* Tail of each Rx message. This is mainly now used for CRC calculations */
	TL_IO2LCU_STRUCT TL_IO2LCU;
}
PHS_TLONLYMESSAGEIO2LCU;

#endif /* _INTERFACE_TYPE_TL_ONLY_MESSAGE_IO2LCU */

#ifndef _INTERFACE_TYPE_TL_ONLY_MESSAGE_LCU2IO
#define _INTERFACE_TYPE_TL_ONLY_MESSAGE_LCU2IO

/* Interface data structure type definition */
typedef struct PHS_TLONLYMESSAGELCU2IO
{
	/* Tail of each Tx message. This is mainly now used for CRC calculations */
	TL_LCU2IO_STRUCT TL_LCU2IO;
}
PHS_TLONLYMESSAGELCU2IO;

#endif /* _INTERFACE_TYPE_TL_ONLY_MESSAGE_LCU2IO */




#ifdef __cplusplus
}
#endif

#endif 
